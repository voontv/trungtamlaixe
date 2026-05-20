using System.Collections.Generic;
using System.Threading.Tasks;
using System;
using Ttlaixe.AutoConfig;
using Ttlaixe.Models;
using Ttlaixe.DTO.response;
using Microsoft.EntityFrameworkCore;
using System.Linq;
using Ttlaixe.DTO.request;
using Ttlaixe.LibsStartup;
using Ttlaixe.Exceptions;

namespace Ttlaixe.Businesses
{
    [ImplementBy(typeof(NhatKyChungTuBusiness))]
    public interface INhatKyChungTuBusiness
    {
        Task<List<NhatKyChungTuResponse>> GetAllAsync(DateTime? fromDate, DateTime? toDate);
        Task<NhatKyChungTu?> GetByIdAsync(int idChungTu);
        Task CreateAsync(NhatKyChungTuRequest model);
        Task<bool> UpdateAsync(int idChungTu, NhatKyChungTu model);
        Task<bool> DeleteAsync(int idChungTu);
        Task XoaChungTuTheoSoChungTu(string SoChungTu, decimal SoTien, DateTime NgayNop);
        Task<List<TongHopChungTuDto>> TongHopTheoTaiKhoanChiTietAsync();
        Task<List<TongHopChungTuDto>> TongHopTheoTaiKhoanChaAsync();
        Task<List<TongHopChungTuDto>> TongHopTheoTaiKhoanChiTietAsync(DateTime? fromDate, DateTime? toDate);
        Task<List<TongHopChungTuDto>> TongHopTheoTaiKhoanChaAsync(DateTime? fromDate, DateTime? toDate);

        Task<byte[]> GetChungTuNopHocPhiHV(DateTime? fromDate, DateTime? toDate);
        Task<TongHopThangReponse> TongHopTheoThangAsync(DateTime? fromDate, DateTime toDate);
        Task<List<NhatKyChungTu>> TongHopChiTietAsync(TongHopChiTietRequest req);
    }

    public class NhatKyChungTuBusiness : INhatKyChungTuBusiness
    {
        private readonly TeknovaContext _context;

        public NhatKyChungTuBusiness(TeknovaContext context)
        {
            _context = context;
        }

        public async Task<List<NhatKyChungTuResponse>> GetAllAsync(DateTime? fromDate, DateTime? toDate)
        {
            var query = _context.NhatKyChungTus.AsNoTracking().AsQueryable();

            if (fromDate.HasValue)
                query = query.Where(x => x.NgayLap >= fromDate.Value.Date);

            if (toDate.HasValue)
                query = query.Where(x => x.NgayLap <= toDate.Value.Date);
            var nhatKyChungTus = await query
                .OrderByDescending(x => x.NgayLap)
                .ThenByDescending(x => x.IdChungTu)
                .ToListAsync();
            var results = new List<NhatKyChungTuResponse>();

            nhatKyChungTus.Patch(results);

            return results;
        }

        public async Task XoaChungTuTheoSoChungTu(string SoChungTu, decimal SoTien, DateTime NgayNop)
        {
            var nhatKyChungTu = await _context.NhatKyChungTus
                .Where(x => x.SoChungTu == SoChungTu && x.SoTien == SoTien && x.NgayLap.Equals(NgayNop)).FirstOrDefaultAsync();

            await DeleteAsync(nhatKyChungTu.IdChungTu);
        }
        public async Task<NhatKyChungTu?> GetByIdAsync(int idChungTu)
        {
            return await _context.NhatKyChungTus
                .AsNoTracking()
                .FirstOrDefaultAsync(x => x.IdChungTu == idChungTu);
        }

        public async Task CreateAsync(NhatKyChungTuRequest model)
        {
            var tkNoExists = await _context.DmTaiKhoanKeToans
                .AnyAsync(x => x.MaTaiKhoan == model.TaiKhoanNo);

            if (!tkNoExists)
                throw new Exception("Tài khoản nợ không tồn tại.");

            var tkCoExists = await _context.DmTaiKhoanKeToans
                .AnyAsync(x => x.MaTaiKhoan == model.TaiKhoanCo);

            if (!tkCoExists)
                throw new BadRequestException("Tài khoản có không tồn tại.");

            if (model.SoTien <= 0)
                throw new BadRequestException("Số tiền phải lớn hơn 0.");

            var nhatKyChungTu = new NhatKyChungTu();
            model.Patch(nhatKyChungTu);

            nhatKyChungTu.NgayKhoiTao = DateTime.Now;

            try
            {
                _context.NhatKyChungTus.Add(nhatKyChungTu);
                await _context.SaveChangesAsync();
            }catch (Exception ex)
            {
                throw new BadRequestException(ex.Message.ToString());
            }
            

        }

        public async Task<bool> UpdateAsync(int idChungTu, NhatKyChungTu model)
        {
            var existing = await _context.NhatKyChungTus
                .FirstOrDefaultAsync(x => x.IdChungTu == idChungTu);

            if (existing == null)
                return false;

            var tkNoExists = await _context.DmTaiKhoanKeToans
                .AnyAsync(x => x.MaTaiKhoan == model.TaiKhoanNo);

            if (!tkNoExists)
                throw new BadRequestException("Tài khoản nợ không tồn tại.");

            var tkCoExists = await _context.DmTaiKhoanKeToans
                .AnyAsync(x => x.MaTaiKhoan == model.TaiKhoanCo);

            if (!tkCoExists)
                throw new BadRequestException("Tài khoản có không tồn tại.");

            if (model.SoTien <= 0)
                throw new BadRequestException("Số tiền phải lớn hơn 0.");

            existing.SoChungTu = model.SoChungTu;
            existing.NgayLap = model.NgayLap;
            existing.DienGiai = model.DienGiai;
            existing.TaiKhoanNo = model.TaiKhoanNo;
            existing.TaiKhoanCo = model.TaiKhoanCo;
            existing.SoTien = model.SoTien;
            existing.GhiChu = model.GhiChu;

            await _context.SaveChangesAsync();

            return true;
        }

        public async Task<bool> DeleteAsync(int idChungTu)
        {
            var existing = await _context.NhatKyChungTus
                .FirstOrDefaultAsync(x => x.IdChungTu == idChungTu);

            if(existing.GhiChu.Equals("nộp tiền học phí"))
            {
                throw new BadRequestException("Đây là chứng từ nộp tiền học phí. Vui lòng xóa bên lịch sử thu học phí");
            }    

            if (existing == null)
                return false;

            _context.NhatKyChungTus.Remove(existing);
            await _context.SaveChangesAsync();

            return true;
        }

        public async Task<List<TongHopChungTuDto>> TongHopTheoTaiKhoanChiTietAsync()
        {
            var noQuery =
                from nk in _context.NhatKyChungTus
                group nk by nk.TaiKhoanNo into g
                select new
                {
                    MaTaiKhoan = g.Key,
                    TongNo = g.Sum(x => x.SoTien),
                    TongCo = 0m
                };

            var coQuery =
                from nk in _context.NhatKyChungTus
                group nk by nk.TaiKhoanCo into g
                select new
                {
                    MaTaiKhoan = g.Key,
                    TongNo = 0m,
                    TongCo = g.Sum(x => x.SoTien)
                };

            var data = await noQuery
                .Concat(coQuery)
                .GroupBy(x => x.MaTaiKhoan)
                .Select(g => new
                {
                    MaTaiKhoan = g.Key,
                    TongNo = g.Sum(x => x.TongNo),
                    TongCo = g.Sum(x => x.TongCo)
                })
                .Join(
                    _context.DmTaiKhoanKeToans,
                    x => x.MaTaiKhoan,
                    tk => tk.MaTaiKhoan,
                    (x, tk) => new TongHopChungTuDto
                    {
                        MaTaiKhoan = x.MaTaiKhoan,
                        TenTaiKhoan = tk.TenTaiKhoan,
                        TongNo = x.TongNo,
                        TongCo = x.TongCo
                    }
                )
                .OrderBy(x => x.MaTaiKhoan)
                .ToListAsync();

            return data;
        }

        public async Task<List<TongHopChungTuDto>> TongHopTheoTaiKhoanChaAsync()
        {
            var noQuery =
                from nk in _context.NhatKyChungTus
                join tk in _context.DmTaiKhoanKeToans
                    on nk.TaiKhoanNo equals tk.MaTaiKhoan
                select new
                {
                    MaTaiKhoan = tk.MaTaiKhoanCha ?? tk.MaTaiKhoan,
                    TongNo = nk.SoTien,
                    TongCo = 0m
                };

            var coQuery =
                from nk in _context.NhatKyChungTus
                join tk in _context.DmTaiKhoanKeToans
                    on nk.TaiKhoanCo equals tk.MaTaiKhoan
                select new
                {
                    MaTaiKhoan = tk.MaTaiKhoanCha ?? tk.MaTaiKhoan,
                    TongNo = 0m,
                    TongCo = nk.SoTien
                };

            var data = await noQuery
                .Concat(coQuery)
                .GroupBy(x => x.MaTaiKhoan)
                .Select(g => new
                {
                    MaTaiKhoan = g.Key,
                    TongNo = g.Sum(x => x.TongNo),
                    TongCo = g.Sum(x => x.TongCo)
                })
                .Join(
                    _context.DmTaiKhoanKeToans,
                    x => x.MaTaiKhoan,
                    tk => tk.MaTaiKhoan,
                    (x, tk) => new TongHopChungTuDto
                    {
                        MaTaiKhoan = x.MaTaiKhoan,
                        TenTaiKhoan = tk.TenTaiKhoan,
                        TongNo = x.TongNo,
                        TongCo = x.TongCo
                    }
                )
                .OrderBy(x => x.MaTaiKhoan)
                .ToListAsync();

            return data;
        }

        public async Task<List<TongHopChungTuDto>> TongHopTheoTaiKhoanChiTietAsync(DateTime? fromDate, DateTime? toDate)
        {
            var query = _context.NhatKyChungTus.AsNoTracking().AsQueryable();

            if (fromDate.HasValue)
                query = query.Where(x => x.NgayLap >= fromDate.Value.Date);

            if (toDate.HasValue)
                query = query.Where(x => x.NgayLap <= toDate.Value.Date);

            var noQuery =
                from nk in query
                group nk by nk.TaiKhoanNo into g
                select new
                {
                    MaTaiKhoan = g.Key,
                    TongNo = g.Sum(x => x.SoTien),
                    TongCo = 0m
                };

            var coQuery =
                from nk in query
                group nk by nk.TaiKhoanCo into g
                select new
                {
                    MaTaiKhoan = g.Key,
                    TongNo = 0m,
                    TongCo = g.Sum(x => x.SoTien)
                };

            return await noQuery
                .Concat(coQuery)
                .GroupBy(x => x.MaTaiKhoan)
                .Select(g => new
                {
                    MaTaiKhoan = g.Key,
                    TongNo = g.Sum(x => x.TongNo),
                    TongCo = g.Sum(x => x.TongCo)
                })
                .Join(
                    _context.DmTaiKhoanKeToans,
                    x => x.MaTaiKhoan,
                    tk => tk.MaTaiKhoan,
                    (x, tk) => new TongHopChungTuDto
                    {
                        MaTaiKhoan = x.MaTaiKhoan,
                        TenTaiKhoan = tk.TenTaiKhoan,
                        TongNo = x.TongNo,
                        TongCo = x.TongCo
                    }
                )
                .OrderBy(x => x.MaTaiKhoan)
                .ToListAsync();
        }

        public async Task<List<TongHopChungTuDto>> TongHopTheoTaiKhoanChaAsync(DateTime? fromDate, DateTime? toDate)
        {
            var query = _context.NhatKyChungTus.AsNoTracking().AsQueryable();

            if (fromDate.HasValue)
                query = query.Where(x => x.NgayLap >= fromDate.Value.Date);

            if (toDate.HasValue)
                query = query.Where(x => x.NgayLap <= toDate.Value.Date);

            var noQuery =
                from nk in query
                join tk in _context.DmTaiKhoanKeToans
                    on nk.TaiKhoanNo equals tk.MaTaiKhoan
                select new
                {
                    MaTaiKhoan = tk.MaTaiKhoanCha ?? tk.MaTaiKhoan,
                    TongNo = nk.SoTien,
                    TongCo = 0m
                };

            var coQuery =
                from nk in query
                join tk in _context.DmTaiKhoanKeToans
                    on nk.TaiKhoanCo equals tk.MaTaiKhoan
                select new
                {
                    MaTaiKhoan = tk.MaTaiKhoanCha ?? tk.MaTaiKhoan,
                    TongNo = 0m,
                    TongCo = nk.SoTien
                };

            return await noQuery
                .Concat(coQuery)
                .GroupBy(x => x.MaTaiKhoan)
                .Select(g => new
                {
                    MaTaiKhoan = g.Key,
                    TongNo = g.Sum(x => x.TongNo),
                    TongCo = g.Sum(x => x.TongCo)
                })
                .Join(
                    _context.DmTaiKhoanKeToans,
                    x => x.MaTaiKhoan,
                    tk => tk.MaTaiKhoan,
                    (x, tk) => new TongHopChungTuDto
                    {
                        MaTaiKhoan = x.MaTaiKhoan,
                        TenTaiKhoan = tk.TenTaiKhoan,
                        TongNo = x.TongNo,
                        TongCo = x.TongCo
                    }
                )
                .OrderBy(x => x.MaTaiKhoan)
                .ToListAsync();
        }

        public async Task<byte[]> GetChungTuNopHocPhiHV(DateTime? fromDate, DateTime? toDate)
        {
            // B1: Query lịch sử nộp và lọc ngày
            var lichSuQuery = _context.LichSuNopHocPhis.AsQueryable();

            if (fromDate.HasValue)
                lichSuQuery = lichSuQuery.Where(x => x.NgayNop >= fromDate.Value);

            if (toDate.HasValue)
                lichSuQuery = lichSuQuery.Where(x => x.NgayNop <= toDate.Value);

            // B2: Sum tiền TRƯỚC khi join
            var tongTheoMaDk = await lichSuQuery
                .GroupBy(x => x.MaDk)
                .Select(g => new
                {
                    MaDk = g.Key,
                    TongTien = g.Sum(x => x.SoTienNop),
                    NgayCuoi = g.Max(x => x.NgayNop),
                    HinhThuc = g
                        .OrderByDescending(x => x.NgayNop)
                        .Select(x => x.HinhThucThanhToan)
                        .FirstOrDefault()
                })
                .ToListAsync();

            // B3: Join sang hồ sơ học phí
            var rows = (
                from t in tongTheoMaDk
                join hp in _context.HoSoHocPhis.AsNoTracking()
                    on t.MaDk equals hp.MaDk
                select new HoaDonRow
                {
                    NgayHoaDon = t.NgayCuoi.ToString("dd/MM/yyyy"),
                    MaKhachHang = hp.MaDk,
                    TenNguoiMua = hp.HoVaTen,
                    DiaChiKhachHang = hp.NoiThuongTru,
                    HinhThucThanhToan = t.HinhThuc,
                    ThueSuat = Constants.ThueSuat,
                    TenHangHoa = Constants.TenHangHoa + " " + hp.MaHangGplx,
                    DVT = "HV",
                    ThanhTien = t.TongTien,              // ✅ luôn đúng
                    SoTT = 1,
                    TinhChat = 1,
                    TienThue = t.TongTien * Constants.ThueSuat / 100,
                    CanCuocCongDan = hp.SoCmt
                })
                .ToList();

            return await ExportExcelAsync(rows);
        }

        public async Task<byte[]> ExportExcelAsync(List<HoaDonRow> data)
        {
            return await ExcelExporter.ExportExcelAsync(data);
        }

        public async Task<TongHopThangReponse> TongHopTheoThangAsync(DateTime? tuNgay, DateTime denNgay)
        {
            // Chuẩn hóa ngày
            denNgay = denNgay.Date.AddDays(1).AddTicks(-1);

            var nam = denNgay.Year;
            var fromDauNam = new DateTime(nam, 1, 1);

            // Nếu không truyền từ ngày => lấy từ đầu năm
            var fromKy = tuNgay.HasValue
                ? tuNgay.Value.Date
                : fromDauNam;

            // ===== 1. Lũy kế trước kỳ =====
            List<TongHopChungTuDto> luyKe;

            if (fromKy == fromDauNam)
            {
                // Không có lũy kế trước kỳ
                luyKe = new List<TongHopChungTuDto>();
            }
            else
            {
                luyKe = await TongHopTheoTaiKhoanChaAsync(fromDauNam, fromKy.AddTicks(-1));
            }

            // Số dư đầu năm luôn cộng vào
            var duDauNam = await _context.LichSuSoDus
                .Where(x => x.Nam == nam)
                .ToListAsync();

            var dictLuyKe = luyKe.ToDictionary(x => x.MaTaiKhoan);

            foreach (var du in duDauNam)
            {
                if (dictLuyKe.TryGetValue(du.MaTaiKhoan, out var item))
                {
                    item.TongNo += du.No;
                    item.TongCo += du.Co;
                }
                else
                {
                    luyKe.Add(new TongHopChungTuDto
                    {
                        MaTaiKhoan = du.MaTaiKhoan,
                        TenTaiKhoan = du.TenTaiKhoan,
                        TongNo = du.No,
                        TongCo = du.Co
                    });
                }
            }

            // ===== 2. Phát sinh trong kỳ =====
            var phatSinh = await TongHopTheoTaiKhoanChaAsync(fromKy, denNgay);
            var dictPhatSinh = phatSinh.ToDictionary(x => x.MaTaiKhoan);

            // ===== 3. Tính số dư cuối =====
            var allKeys = dictLuyKe.Keys
                .Union(dictPhatSinh.Keys)
                .ToList();

            var soDuCuoi = new List<TongHopChungTuDto>();

            foreach (var key in allKeys)
            {
                var lk = dictLuyKe.ContainsKey(key) ? dictLuyKe[key] : new TongHopChungTuDto { MaTaiKhoan = key };
                var ps = dictPhatSinh.ContainsKey(key) ? dictPhatSinh[key] : new TongHopChungTuDto { MaTaiKhoan = key };

                var tongNo = lk.TongNo + ps.TongNo;
                var tongCo = lk.TongCo + ps.TongCo;

                var chenhlech = tongCo - tongNo;

                soDuCuoi.Add(new TongHopChungTuDto
                {
                    MaTaiKhoan = key,
                    TenTaiKhoan = lk.TenTaiKhoan ?? ps.TenTaiKhoan,
                    TongNo = chenhlech < 0 ? Math.Abs(chenhlech) : 0,
                    TongCo = chenhlech > 0 ? chenhlech : 0
                });
            }

            return new TongHopThangReponse
            {
                SoDuDauKy = luyKe,
                SoPhatSinhTrongKy = phatSinh,
                SoDuCuoiKy = soDuCuoi.OrderBy(x => x.MaTaiKhoan).ToList()
            };
        }

        public async Task<List<NhatKyChungTu>> TongHopChiTietAsync(TongHopChiTietRequest req)
        {
            if (req.MaTaiKhoans == null || !req.MaTaiKhoans.Any())
            {
                req.MaTaiKhoans = await _context.DmTaiKhoanKeToans.Where(x => x.IsActive == true).Select(x => x.MaTaiKhoan).ToListAsync();
            }    
            
            var fromDate = req.TuNgay.Date;
            var denDate = req.DenNgay.Date.AddDays(1).AddTicks(-1);

            var result = await _context.NhatKyChungTus
                .Where(x =>
                    (req.MaTaiKhoans.Contains(x.TaiKhoanCo) || req.MaTaiKhoans.Contains(x.TaiKhoanNo)) &&
                    x.NgayLap >= fromDate &&
                    x.NgayLap <= denDate)
                .OrderBy(x => x.TaiKhoanCo)
                .ThenBy(x => x.NgayLap)
                .ToListAsync();

            return result;
        }

    }
}
