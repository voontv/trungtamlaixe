using IronXL;
using System.Collections.Generic;
using System.IO;
using System.Threading.Tasks;
using System;
using Ttlaixe.DTO.response;
using ClosedXML.Excel;
using System.Globalization;

namespace Ttlaixe.LibsStartup
{
    public static class ExcelExporter
    {
        public static async Task<byte[]> ExportExcelAsync(List<HoaDonRow> data)
        {
            using var wb = new XLWorkbook();
            var ws = wb.Worksheets.Add("HoaDon");

            string[] headers = new[]
            {
            "MaHD","NgayHoaDon","MaKhachHang","TenNguoiMua","TenDonVi","MaSoThue",
            "DiaChiKhachHang","TENNHKHACH","HinhThucThanhToan","ThueSuat","ThueSuatKhac",
            "MaHang","TenHangHoa","DVT","SoLuong","DonGia","ThanhTien","TienTe",
            "SoTT","TinhChat","TienThue","MaDonViQuanHeNganSach","CanCuocCongDan",
            "SoHoChieu","SoKhung","SoMay","BienKiemSoatPhuongTienVanchuyen",
            "TenNguoiGuiHang","DiaChiNguoiGuiHang","MaSoThueNguoiGuiHang","SoDinhDanhNguoiGuiHang"
        };

            for (int i = 0; i < headers.Length; i++)
                ws.Cell(1, i + 1).Value = headers[i];

            int row = 2;

            foreach (var x in data)
            {
                ws.Cell(row, 1).Value = x.MaHD ?? "";
                ws.Cell(row, 2).Value = x.NgayHoaDon;
                ws.Cell(row, 3).Value = x.MaKhachHang ?? "";
                ws.Cell(row, 4).Value = x.TenNguoiMua ?? "";
                ws.Cell(row, 5).Value = x.TenDonVi ?? "";
                ws.Cell(row, 6).Value = x.MaSoThue ?? "";
                ws.Cell(row, 7).Value = x.DiaChiKhachHang ?? "";
                ws.Cell(row, 8).Value = x.TENNHKHACH ?? "";
                ws.Cell(row, 9).Value = x.HinhThucThanhToan ?? "";
                ws.Cell(row, 10).Value = x.ThueSuat ?? 0;
                ws.Cell(row, 11).Value = x.ThueSuatKhac ?? 0;
                ws.Cell(row, 12).Value = x.MaHang ?? "";
                ws.Cell(row, 13).Value = x.TenHangHoa ?? "";
                ws.Cell(row, 14).Value = x.DVT ?? "";
                ws.Cell(row, 15).Value = x.SoLuong ?? 0;
                ws.Cell(row, 16).Value = x.DonGia ?? 0;

                var tongTien = x.ThanhTien ?? 0m;

                ws.Cell(row, 17).FormulaA1 =
                    $"={tongTien.ToString(CultureInfo.InvariantCulture)}/1.1";

                ws.Cell(row, 17).Style.NumberFormat.Format = "#,##0";

                ws.Cell(row, 18).Value = x.TienTe ?? "VND";
                ws.Cell(row, 19).Value = x.SoTT ?? (row - 1);
                ws.Cell(row, 20).Value = x.TinhChat ?? 1;

                // Cột 17: Thành tiền chưa thuế
                ws.Cell(row, 17).FormulaA1 =
                    $"={tongTien.ToString(System.Globalization.CultureInfo.InvariantCulture)}/1.1";
                ws.Cell(row, 17).Style.NumberFormat.Format = "#,##0";

                // Cột 21: Tiền thuế = Tổng - Thành tiền
                ws.Cell(row, 21).FormulaA1 =
                    $"={tongTien.ToString(System.Globalization.CultureInfo.InvariantCulture)}-Q{row}";
                ws.Cell(row, 21).Style.NumberFormat.Format = "#,##0";
                ws.Cell(row, 22).Value = x.MaDonViQuanHeNganSach ?? "";
                ws.Cell(row, 23).Value = x.CanCuocCongDan ?? "";
                ws.Cell(row, 24).Value = x.SoHoChieu ?? "";
                ws.Cell(row, 25).Value = x.SoKhung ?? "";
                ws.Cell(row, 26).Value = x.SoMay ?? "";
                ws.Cell(row, 27).Value = x.BienKiemSoatPhuongTienVanchuyen ?? "";
                ws.Cell(row, 28).Value = x.TenNguoiGuiHang ?? "";
                ws.Cell(row, 29).Value = x.DiaChiNguoiGuiHang ?? "";
                ws.Cell(row, 30).Value = x.MaSoThueNguoiGuiHang ?? "";
                ws.Cell(row, 31).Value = x.SoDinhDanhNguoiGuiHang ?? "";
                row++;
            }

            using var stream = new MemoryStream();
            wb.SaveAs(stream);
            return await Task.FromResult(stream.ToArray());
        }
    }
}
