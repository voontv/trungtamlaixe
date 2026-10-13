using ClosedXML.Excel;
using Microsoft.AspNetCore.Http;
using Microsoft.EntityFrameworkCore;
using Microsoft.EntityFrameworkCore.Metadata;
using System;
using System.Collections.Generic;
using System.Globalization;
using System.IO;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using Ttlaixe.AutoConfig;
using Ttlaixe.DTO.request;
using Ttlaixe.DTO.response;
using Ttlaixe.Exceptions;
using Ttlaixe.Models;

namespace Ttlaixe.Businesses
{
    [ImplementBy(typeof(NguoiLxHoSoesBusinesses))]
    public interface INguoiLxHoSoesBusinesses
    {
        /// <summary>
        /// Cập nhật kết quả đào tạo 1 học viên theo logic store
        /// usp_NguoiLX_KetQuaDaoTao_CSDT_CapNhat.
        /// </summary>
        Task<bool> UpdateKetQuaDaoTaoAsync(NguoiLxHoSoKetQuaUpdateRequest request);

        /// <summary>
        /// Import bảng điểm Excel. Mã học viên (MaDK) là khóa duy nhất.
        /// Header được dò theo tên trường, không phụ thuộc số dòng/số cột cố định.
        /// </summary>
        Task<BangDiemImportResponse> ImportBangDiemAsync(IFormFile file);
    }

    public class NguoiLxHoSoesBusinesses : INguoiLxHoSoesBusinesses
    {
        private readonly GplxCsdtContext _context;

        private static readonly HashSet<string> ProtectedResultStatuses =
            new HashSet<string>(StringComparer.OrdinalIgnoreCase)
            {
                "90", "13", "14"
            };

        public NguoiLxHoSoesBusinesses(GplxCsdtContext context)
        {
            _context = context;
        }

        // =====================================================================
        // 1. UPDATE KET QUA DAO TAO - BAM SAT STORE CU
        // =====================================================================
        public async Task<bool> UpdateKetQuaDaoTaoAsync(NguoiLxHoSoKetQuaUpdateRequest rq)
        {
            if (rq == null)
                throw new BadRequestException("Dữ liệu cập nhật không hợp lệ.");

            if (string.IsNullOrWhiteSpace(rq.MaDk))
                throw new BadRequestException("MaDK không được để trống.");

            if (string.IsNullOrWhiteSpace(rq.MaKhoaHoc))
                throw new BadRequestException("MaKhoaHoc không được để trống.");

            var maDk = rq.MaDk.Trim();
            var maKhoaHoc = rq.MaKhoaHoc.Trim();

            var hoSo = await _context.NguoiLxHoSos
                .FirstOrDefaultAsync(x => x.MaDk == maDk && x.MaKhoaHoc == maKhoaHoc);

            if (hoSo == null)
            {
                throw new BadRequestException(
                    $"Không tìm thấy hồ sơ học viên MaDK = {maDk}, MaKhoaHoc = {maKhoaHoc}.");
            }

            // Store cũ là FULL UPDATE: truyền null thì cột tương ứng cũng về null.
            // Dùng tên CỘT DB để không phụ thuộc cách EF scaffold viết hoa/thường property.
            SetHoSoColumnValue(hoSo, "KQLyThuyet", NormalizeZeroOne(rq.KqLyThuyet));
            SetHoSoColumnValue(hoSo, "KQThucHanh", NormalizeZeroOne(rq.KqThucHanh));
            SetHoSoColumnValue(hoSo, "TGBatDau", EmptyToNull(rq.TgBatDau));
            SetHoSoColumnValue(hoSo, "TGKetThuc", EmptyToNull(rq.TgKetThuc));
            SetHoSoColumnValue(hoSo, "KetLuanCSDT", NormalizeZeroOne(rq.KetLuanCsdt));

            SetHoSoColumnValue(hoSo, "DiemKQLyThuyet", rq.DiemKqLyThuyet);
            SetHoSoColumnValue(hoSo, "DiemKQThucHanh", rq.DiemKqThucHanh);
            SetHoSoColumnValue(hoSo, "DiemKQMoPhong", rq.DiemKqMoPhong);
            SetHoSoColumnValue(hoSo, "DiemKQHinh", rq.DiemKqHinh);
            SetHoSoColumnValue(hoSo, "DiemKQTienLui", rq.DiemKqTienLui);

            SetHoSoColumnValue(hoSo, "TGThucHanhDuong", rq.TgThucHanhDuong);
            SetHoSoColumnValue(hoSo, "TGThucHanhHinh", rq.TgThucHanhHinh);
            SetHoSoColumnValue(hoSo, "TongQDThucHanh", rq.TongQdThucHanh);
            SetHoSoColumnValue(hoSo, "QDThucHanhHinh", rq.QdThucHanhHinh);

            SetHoSoColumnValue(hoSo, "SoGiayCNTN", EmptyToNull(rq.SoGiayCntn));
            SetHoSoColumnValue(hoSo, "NgayRaQDTN", rq.NgayRaQdtn);

            hoSo.NgaySua = DateTime.Now;
            hoSo.TtXuLy = ResolveTtXuLy(hoSo.TtXuLy, NormalizeZeroOne(rq.KetLuanCsdt));

            try
            {
                await _context.SaveChangesAsync();
                return true;
            }
            catch (Exception ex)
            {
                throw new BadRequestException(
                    $"Lỗi cập nhật kết quả đào tạo: {ex.GetBaseException().Message}");
            }
        }

        // =====================================================================
        // 2. IMPORT EXCEL BANG DIEM - MA HOC VIEN / MaDK LA KHOA DUY NHAT
        // =====================================================================
        public async Task<BangDiemImportResponse> ImportBangDiemAsync(IFormFile file)
        {
            if (file == null || file.Length == 0)
                throw new BadRequestException("Vui lòng chọn file Excel.");

            var extension = Path.GetExtension(file.FileName);
            if (!string.Equals(extension, ".xlsx", StringComparison.OrdinalIgnoreCase)
                && !string.Equals(extension, ".xlsm", StringComparison.OrdinalIgnoreCase))
            {
                throw new BadRequestException("Chỉ hỗ trợ file Excel .xlsx hoặc .xlsm.");
            }

            var response = new BangDiemImportResponse
            {
                FileName = file.FileName
            };

            List<ExcelBangDiemRow> excelRows;
            ExcelHeaderMap headerMap;

            try
            {
                await using var ms = new MemoryStream();
                await file.CopyToAsync(ms);
                ms.Position = 0;

                using var workbook = new XLWorkbook(ms);
                var result = ReadBangDiemWorkbook(workbook, response);
                headerMap = result.Header;
                excelRows = result.Rows;
            }
            catch (BadRequestException)
            {
                throw;
            }
            catch (Exception ex)
            {
                throw new BadRequestException(
                    $"Không đọc được file Excel. Chi tiết: {ex.GetBaseException().Message}");
            }

            response.SheetName = headerMap.SheetName;
            response.HeaderRow = headerMap.MaDkHeaderRow;
            response.DataStartRow = headerMap.DataStartRow;
            response.TotalRows = excelRows.Count;

            if (excelRows.Count == 0)
                throw new BadRequestException("Không tìm thấy dòng dữ liệu học viên trong file Excel.");

            // Lỗi format điểm được parser gom lại để trả đúng dòng/cột cho client.
            // Chưa chạm DB nếu còn bất kỳ lỗi nào.
            if (response.Errors.Count > 0)
            {
                response.Success = false;
                response.Message = "File Excel có dữ liệu điểm không hợp lệ. Chưa cập nhật dữ liệu nào.";
                return response;
            }

            // -----------------------------------------------------------------
            // Validate MaDK trùng trong chính file Excel.
            // Không cho import nếu một MaDK xuất hiện nhiều hơn 1 lần vì MaDK là khóa duy nhất.
            // -----------------------------------------------------------------
            var duplicatedMaDks = excelRows
                .GroupBy(x => x.MaDk, StringComparer.OrdinalIgnoreCase)
                .Where(g => g.Count() > 1)
                .ToList();

            foreach (var duplicate in duplicatedMaDks)
            {
                var rowNumbers = string.Join(", ", duplicate.Select(x => x.ExcelRow));
                response.Errors.Add(new BangDiemImportIssue
                {
                    MaDk = duplicate.Key,
                    Message = $"Mã học viên bị trùng trong file tại các dòng: {rowNumbers}."
                });
            }

            if (response.Errors.Count > 0)
            {
                response.Success = false;
                response.Message = "File có Mã học viên trùng. Chưa cập nhật dữ liệu nào.";
                return response;
            }

            var maDks = excelRows
                .Select(x => x.MaDk)
                .Distinct(StringComparer.OrdinalIgnoreCase)
                .ToList();

            // -----------------------------------------------------------------
            // Load hồ sơ theo chunk để không phụ thuộc giới hạn số tham số SQL.
            // -----------------------------------------------------------------
            const int chunkSize = 500;
            var hoSos = new List<NguoiLxHoSo>();

            for (int i = 0; i < maDks.Count; i += chunkSize)
            {
                var chunk = maDks.Skip(i).Take(chunkSize).ToList();

                var found = await _context.NguoiLxHoSos
                    .Where(x => chunk.Contains(x.MaDk))
                    .ToListAsync();

                hoSos.AddRange(found);
            }

            // Nếu DB có nhiều NguoiLX_HoSo cùng MaDK thì khóa MaDK không còn duy nhất -> dừng.
            var duplicatedInDb = hoSos
                .GroupBy(x => x.MaDk, StringComparer.OrdinalIgnoreCase)
                .Where(g => g.Count() > 1)
                .ToList();

            foreach (var duplicate in duplicatedInDb)
            {
                response.Errors.Add(new BangDiemImportIssue
                {
                    MaDk = duplicate.Key,
                    Message = "Trong DB có nhiều hồ sơ cùng MaDK nên không thể xác định dòng cần cập nhật."
                });
            }

            var dbByMaDk = hoSos
                .GroupBy(x => x.MaDk, StringComparer.OrdinalIgnoreCase)
                .Where(g => g.Count() == 1)
                .ToDictionary(g => g.Key, g => g.Single(), StringComparer.OrdinalIgnoreCase);

            foreach (var row in excelRows)
            {
                if (!dbByMaDk.ContainsKey(row.MaDk))
                {
                    response.Errors.Add(new BangDiemImportIssue
                    {
                        ExcelRow = row.ExcelRow,
                        MaDk = row.MaDk,
                        Message = "Không tìm thấy Mã học viên trong bảng NguoiLX_HoSo."
                    });
                }
            }

            // Cẩn trọng: nếu có MaDK sai/không tồn tại thì KHÔNG update nửa file.
            if (response.Errors.Count > 0)
            {
                response.Success = false;
                response.Message = "File có dữ liệu không hợp lệ. Chưa cập nhật dữ liệu nào.";
                return response;
            }

            // -----------------------------------------------------------------
            // Apply vào entity nhưng chỉ SaveChanges 1 lần.
            // QUAN TRỌNG: import Excel là PARTIAL theo từng ô:
            // - ô có điểm -> cập nhật điểm đó;
            // - ô trống -> GIỮ điểm cũ, không ghi NULL.
            // File mẫu có nhiều học viên thi lại chỉ có 1-2 cột điểm.
            // -----------------------------------------------------------------
            var now = DateTime.Now;
            int updated = 0;
            int skipped = 0;

            foreach (var row in excelRows)
            {
                var hoSo = dbByMaDk[row.MaDk];
                var hasChange = false;

                if (row.DiemLyThuyet.HasValue)
                {
                    SetHoSoColumnValue(hoSo, "DiemKQLyThuyet", row.DiemLyThuyet.Value);
                    hasChange = true;
                }

                if (row.DiemMoPhong.HasValue)
                {
                    SetHoSoColumnValue(hoSo, "DiemKQMoPhong", row.DiemMoPhong.Value);
                    hasChange = true;
                }

                if (row.DiemTrongHinh.HasValue)
                {
                    SetHoSoColumnValue(hoSo, "DiemKQHinh", row.DiemTrongHinh.Value);
                    hasChange = true;
                }

                // File mẫu gọi cột này là "Chữ chi".
                // Mapping sang field store hiện có: DiemKQTienLui.
                if (row.DiemChuChi.HasValue)
                {
                    SetHoSoColumnValue(hoSo, "DiemKQTienLui", row.DiemChuChi.Value);
                    hasChange = true;
                }

                // "Trên đường" -> điểm thực hành trên đường.
                if (row.DiemTrenDuong.HasValue)
                {
                    SetHoSoColumnValue(hoSo, "DiemKQThucHanh", row.DiemTrenDuong.Value);
                    hasChange = true;
                }

                if (!string.IsNullOrWhiteSpace(row.KetLuanCsdt))
                {
                    SetHoSoColumnValue(hoSo, "KetLuanCSDT", row.KetLuanCsdt);
                    hoSo.TtXuLy = ResolveTtXuLy(hoSo.TtXuLy, row.KetLuanCsdt);
                    hasChange = true;
                }

                if (hasChange)
                {
                    hoSo.NgaySua = now;
                    updated++;
                }
                else
                {
                    skipped++;
                    response.Warnings.Add(new BangDiemImportIssue
                    {
                        ExcelRow = row.ExcelRow,
                        MaDk = row.MaDk,
                        Message = "Dòng không có điểm/kết quả cần cập nhật nên được bỏ qua."
                    });
                }
            }

            try
            {
                await _context.SaveChangesAsync();
            }
            catch (Exception ex)
            {
                throw new BadRequestException(
                    $"Lỗi lưu bảng điểm vào DB: {ex.GetBaseException().Message}");
            }

            response.UpdatedRows = updated;
            response.SkippedRows = skipped;
            response.Success = true;
            response.Message = $"Import thành công {updated}/{excelRows.Count} học viên.";
            return response;
        }

        // =====================================================================
        // EXCEL PARSER
        // =====================================================================
        private static (ExcelHeaderMap Header, List<ExcelBangDiemRow> Rows)
            ReadBangDiemWorkbook(XLWorkbook workbook, BangDiemImportResponse response)
        {
            ExcelHeaderMap header = null;
            IXLWorksheet worksheet = null;

            foreach (var ws in workbook.Worksheets)
            {
                if (TryBuildHeaderMap(ws, out var map))
                {
                    worksheet = ws;
                    header = map;
                    break;
                }
            }

            if (worksheet == null || header == null)
            {
                throw new BadRequestException(
                    "Không tìm thấy bộ header bảng điểm. File phải có trường 'MÃ HỌC VIÊN'.");
            }

            var missingHeaders = header.GetMissingHeaders();
            if (missingHeaders.Count > 0)
            {
                throw new BadRequestException(
                    "File Excel thiếu cột bắt buộc: " + string.Join(", ", missingHeaders));
            }

            var lastRow = worksheet.LastRowUsed()?.RowNumber() ?? header.DataStartRow;
            var rows = new List<ExcelBangDiemRow>();

            for (int rowNumber = header.DataStartRow; rowNumber <= lastRow; rowNumber++)
            {
                var maDk = ReadCellText(worksheet.Cell(rowNumber, header.MaDkColumn));

                // Footer/chữ ký hoặc dòng trống không có MaDK -> bỏ qua.
                if (string.IsNullOrWhiteSpace(maDk))
                    continue;

                var row = new ExcelBangDiemRow
                {
                    ExcelRow = rowNumber,
                    MaDk = maDk.Trim(),
                    DiemLyThuyet = ReadNullableDouble(
                        worksheet.Cell(rowNumber, header.DiemLyThuyetColumn),
                        rowNumber,
                        "Lý thuyết",
                        response),
                    DiemMoPhong = ReadNullableDouble(
                        worksheet.Cell(rowNumber, header.DiemMoPhongColumn),
                        rowNumber,
                        "Mô phỏng",
                        response),
                    DiemTrongHinh = ReadNullableDouble(
                        worksheet.Cell(rowNumber, header.DiemTrongHinhColumn),
                        rowNumber,
                        "Trong hình",
                        response),
                    DiemChuChi = ReadNullableDouble(
                        worksheet.Cell(rowNumber, header.DiemChuChiColumn),
                        rowNumber,
                        "Chữ chi",
                        response),
                    DiemTrenDuong = ReadNullableDouble(
                        worksheet.Cell(rowNumber, header.DiemTrenDuongColumn),
                        rowNumber,
                        "Trên đường",
                        response)
                };

                var rawKetQua = ReadCellText(worksheet.Cell(rowNumber, header.KetQuaColumn));
                row.KetLuanCsdt = MapExcelKetQua(rawKetQua, rowNumber, row.MaDk, response);

                rows.Add(row);
            }

            return (header, rows);
        }

        private static bool TryBuildHeaderMap(IXLWorksheet ws, out ExcelHeaderMap map)
        {
            map = null;

            var lastColumn = ws.LastColumnUsed()?.ColumnNumber() ?? 0;
            var lastRow = ws.LastRowUsed()?.RowNumber() ?? 0;

            if (lastColumn == 0 || lastRow == 0)
                return false;

            // Chỉ dò vùng đầu file để tìm "MÃ HỌC VIÊN".
            var scanToRow = Math.Min(lastRow, 80);
            HeaderCell maDkHeader = null;

            for (int r = 1; r <= scanToRow && maDkHeader == null; r++)
            {
                for (int c = 1; c <= lastColumn; c++)
                {
                    var text = NormalizeHeader(ReadCellText(ws.Cell(r, c)));
                    if (IsAny(text, "MA HOC VIEN", "MA HV", "MA DK", "MADK"))
                    {
                        maDkHeader = new HeaderCell(r, c);
                        break;
                    }
                }
            }

            if (maDkHeader == null)
                return false;

            // Header có thể 2-3 tầng. Dò quanh dòng chứa MÃ HỌC VIÊN,
            // không dùng số cột cố định nên cột có thể xê dịch trái/phải.
            var fromRow = Math.Max(1, maDkHeader.Row - 2);
            var toRow = Math.Min(lastRow, maDkHeader.Row + 6);

            var lyThuyet = FindHeader(ws, fromRow, toRow, lastColumn,
                "LY THUYET", "DIEM LY THUYET");
            var moPhong = FindHeader(ws, fromRow, toRow, lastColumn,
                "MO PHONG", "DIEM MO PHONG");
            var trongHinh = FindHeader(ws, fromRow, toRow, lastColumn,
                "TRONG HINH", "DIEM TRONG HINH", "DIEM HINH");
            var chuChi = FindHeader(ws, fromRow, toRow, lastColumn,
                "CHU CHI", "TIEN LUI", "DIEM CHU CHI", "DIEM TIEN LUI");
            var trenDuong = FindHeader(ws, fromRow, toRow, lastColumn,
                "TREN DUONG", "DIEM TREN DUONG", "THUC HANH TREN DUONG");
            var ketQua = FindHeader(ws, fromRow, toRow, lastColumn,
                "KET QUA", "KET LUAN", "KET QUA KIEM TRA");

            var headerRows = new[]
            {
                maDkHeader.Row,
                lyThuyet?.Row ?? 0,
                moPhong?.Row ?? 0,
                trongHinh?.Row ?? 0,
                chuChi?.Row ?? 0,
                trenDuong?.Row ?? 0,
                ketQua?.Row ?? 0
            };

            map = new ExcelHeaderMap
            {
                SheetName = ws.Name,
                MaDkHeaderRow = maDkHeader.Row,
                DataStartRow = headerRows.Max() + 1,
                MaDkColumn = maDkHeader.Column,
                DiemLyThuyetColumn = lyThuyet?.Column ?? 0,
                DiemMoPhongColumn = moPhong?.Column ?? 0,
                DiemTrongHinhColumn = trongHinh?.Column ?? 0,
                DiemChuChiColumn = chuChi?.Column ?? 0,
                DiemTrenDuongColumn = trenDuong?.Column ?? 0,
                KetQuaColumn = ketQua?.Column ?? 0
            };

            return true;
        }

        private static HeaderCell FindHeader(
            IXLWorksheet ws,
            int fromRow,
            int toRow,
            int lastColumn,
            params string[] aliases)
        {
            var normalizedAliases = new HashSet<string>(
                aliases.Select(NormalizeHeader),
                StringComparer.OrdinalIgnoreCase);

            for (int r = fromRow; r <= toRow; r++)
            {
                for (int c = 1; c <= lastColumn; c++)
                {
                    var text = NormalizeHeader(ReadCellText(ws.Cell(r, c)));
                    if (normalizedAliases.Contains(text))
                        return new HeaderCell(r, c);
                }
            }

            return null;
        }

        private static double? ReadNullableDouble(
            IXLCell cell,
            int excelRow,
            string fieldName,
            BangDiemImportResponse response)
        {
            var text = ReadCellText(cell);
            if (string.IsNullOrWhiteSpace(text))
                return null;

            if (cell.TryGetValue<double>(out var number))
                return number;

            if (double.TryParse(
                text,
                NumberStyles.Any,
                CultureInfo.GetCultureInfo("vi-VN"),
                out number))
            {
                return number;
            }

            if (double.TryParse(
                text,
                NumberStyles.Any,
                CultureInfo.InvariantCulture,
                out number))
            {
                return number;
            }

            response.Errors.Add(new BangDiemImportIssue
            {
                ExcelRow = excelRow,
                Message = $"Cột '{fieldName}' có giá trị '{text}' không phải số."
            });

            return null;
        }

        private static string MapExcelKetQua(
            string rawValue,
            int excelRow,
            string maDk,
            BangDiemImportResponse response)
        {
            if (string.IsNullOrWhiteSpace(rawValue))
                return null;

            var value = NormalizeHeader(rawValue);

            // File mẫu:
            // ĐẠT -> 1
            // VẮNG, RLT, RM, R(LT+M), ... -> 0
            if (value == "DAT")
                return "1";

            if (value == "VANG"
                || value == "KHONG DAT"
                || value.StartsWith("R ", StringComparison.OrdinalIgnoreCase)
                || value == "R"
                || value.StartsWith("RLT", StringComparison.OrdinalIgnoreCase)
                || value.StartsWith("RM", StringComparison.OrdinalIgnoreCase))
            {
                return "0";
            }

            // Không tự đoán kết luận với mã lạ: vẫn cho import điểm,
            // nhưng không thay KetLuanCSDT/TT_XuLy của dòng này.
            response.Warnings.Add(new BangDiemImportIssue
            {
                ExcelRow = excelRow,
                MaDk = maDk,
                Message = $"KẾT QUẢ '{rawValue}' chưa có mapping; chỉ cập nhật các cột điểm."
            });

            return null;
        }

        private static string ReadCellText(IXLCell cell)
        {
            if (cell == null || cell.IsEmpty())
                return null;

            return cell.GetFormattedString()?.Trim();
        }

        private static string NormalizeHeader(string value)
        {
            if (string.IsNullOrWhiteSpace(value))
                return string.Empty;

            // Unicode NFD không tách ký tự Đ nên phải đổi riêng.
            value = value.Replace('đ', 'd').Replace('Đ', 'D');

            var normalized = value.Normalize(NormalizationForm.FormD);
            var sb = new StringBuilder(normalized.Length);

            foreach (var ch in normalized)
            {
                var category = CharUnicodeInfo.GetUnicodeCategory(ch);
                if (category == UnicodeCategory.NonSpacingMark)
                    continue;

                if (char.IsLetterOrDigit(ch))
                    sb.Append(char.ToUpperInvariant(ch));
                else
                    sb.Append(' ');
            }

            return string.Join(
                " ",
                sb.ToString()
                    .Split(new[] { ' ', '\t', '\r', '\n' }, StringSplitOptions.RemoveEmptyEntries));
        }

        private static bool IsAny(string value, params string[] expected)
        {
            return expected.Any(x =>
                string.Equals(value, NormalizeHeader(x), StringComparison.OrdinalIgnoreCase));
        }

        // =====================================================================
        // TT_XULY - GIU DUNG LOGIC STORE
        // =====================================================================
        private static string ResolveTtXuLy(string currentTtXuLy, string ketLuanCsdt)
        {
            if (!string.IsNullOrWhiteSpace(currentTtXuLy)
                && ProtectedResultStatuses.Contains(currentTtXuLy))
            {
                return currentTtXuLy;
            }

            if (ketLuanCsdt == "1")
                return "09";

            if (ketLuanCsdt == "0")
                return "10";

            return currentTtXuLy;
        }

        private static string NormalizeZeroOne(string value)
        {
            if (string.IsNullOrWhiteSpace(value))
                return null;

            value = value.Trim();
            return value == "1" || value == "0" ? value : value;
        }

        private static string EmptyToNull(string value)
        {
            return string.IsNullOrWhiteSpace(value) ? null : value.Trim();
        }

        // =====================================================================
        // EF COLUMN MAPPING
        // Dùng tên cột DB thay vì hard-code tên property scaffold của entity.
        // Ví dụ DB là DiemKQLyThuyet dù property C# có thể được scaffold khác casing.
        // =====================================================================
        private void SetHoSoColumnValue(NguoiLxHoSo entity, string dbColumnName, object value)
        {
            var entityType = _context.Model.FindEntityType(typeof(NguoiLxHoSo));
            if (entityType == null)
                throw new InvalidOperationException("Không tìm thấy metadata entity NguoiLxHoSo.");

            var tableName = entityType.GetTableName();
            if (string.IsNullOrWhiteSpace(tableName))
                throw new InvalidOperationException("Không xác định được table của NguoiLxHoSo.");

            var storeObject = StoreObjectIdentifier.Table(tableName, entityType.GetSchema());

            var property = entityType
                .GetProperties()
                .FirstOrDefault(p => string.Equals(
                    p.GetColumnName(storeObject),
                    dbColumnName,
                    StringComparison.OrdinalIgnoreCase));

            if (property == null)
            {
                throw new InvalidOperationException(
                    $"Entity NguoiLxHoSo chưa map cột DB '{dbColumnName}'.");
            }

            var converted = ConvertToPropertyType(value, property.ClrType, dbColumnName);
            _context.Entry(entity).Property(property.Name).CurrentValue = converted;
        }

        private static object ConvertToPropertyType(object value, Type propertyType, string columnName)
        {
            if (value == null)
                return null;

            var targetType = Nullable.GetUnderlyingType(propertyType) ?? propertyType;

            if (targetType == typeof(string))
                return Convert.ToString(value, CultureInfo.InvariantCulture);

            if (targetType == typeof(bool))
            {
                if (value is bool b)
                    return b;

                var text = Convert.ToString(value, CultureInfo.InvariantCulture)?.Trim();
                if (text == "1" || string.Equals(text, "true", StringComparison.OrdinalIgnoreCase))
                    return true;
                if (text == "0" || string.Equals(text, "false", StringComparison.OrdinalIgnoreCase))
                    return false;

                throw new InvalidOperationException(
                    $"Không convert được giá trị '{text}' sang bool cho cột {columnName}.");
            }

            if (targetType == typeof(DateTime))
            {
                if (value is DateTime dateTime)
                    return dateTime;

                return Convert.ToDateTime(value, CultureInfo.InvariantCulture);
            }

            if (targetType == typeof(double))
                return Convert.ToDouble(value, CultureInfo.InvariantCulture);

            if (targetType == typeof(float))
                return Convert.ToSingle(value, CultureInfo.InvariantCulture);

            if (targetType == typeof(decimal))
                return Convert.ToDecimal(value, CultureInfo.InvariantCulture);

            if (targetType == typeof(int))
                return Convert.ToInt32(value, CultureInfo.InvariantCulture);

            if (targetType == typeof(long))
                return Convert.ToInt64(value, CultureInfo.InvariantCulture);

            return Convert.ChangeType(value, targetType, CultureInfo.InvariantCulture);
        }

        // =====================================================================
        // INTERNAL EXCEL MODELS
        // =====================================================================
        private sealed class HeaderCell
        {
            public HeaderCell(int row, int column)
            {
                Row = row;
                Column = column;
            }

            public int Row { get; }
            public int Column { get; }
        }

        private sealed class ExcelHeaderMap
        {
            public string SheetName { get; set; }
            public int MaDkHeaderRow { get; set; }
            public int DataStartRow { get; set; }
            public int MaDkColumn { get; set; }
            public int DiemLyThuyetColumn { get; set; }
            public int DiemMoPhongColumn { get; set; }
            public int DiemTrongHinhColumn { get; set; }
            public int DiemChuChiColumn { get; set; }
            public int DiemTrenDuongColumn { get; set; }
            public int KetQuaColumn { get; set; }

            public List<string> GetMissingHeaders()
            {
                var missing = new List<string>();

                if (MaDkColumn <= 0) missing.Add("MÃ HỌC VIÊN");
                if (DiemLyThuyetColumn <= 0) missing.Add("Lý thuyết");
                if (DiemMoPhongColumn <= 0) missing.Add("Mô phỏng");
                if (DiemTrongHinhColumn <= 0) missing.Add("Trong hình");
                if (DiemChuChiColumn <= 0) missing.Add("Chữ chi");
                if (DiemTrenDuongColumn <= 0) missing.Add("Trên đường");
                if (KetQuaColumn <= 0) missing.Add("KẾT QUẢ");

                return missing;
            }
        }

        private sealed class ExcelBangDiemRow
        {
            public int ExcelRow { get; set; }
            public string MaDk { get; set; }
            public double? DiemLyThuyet { get; set; }
            public double? DiemMoPhong { get; set; }
            public double? DiemTrongHinh { get; set; }
            public double? DiemChuChi { get; set; }
            public double? DiemTrenDuong { get; set; }
            public string KetLuanCsdt { get; set; }
        }
    }
}
