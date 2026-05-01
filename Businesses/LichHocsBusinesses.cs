using Microsoft.EntityFrameworkCore;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Threading.Tasks;
using Ttlaixe.AutoConfig;
using Ttlaixe.DTO.request;
using Ttlaixe.DTO.response;
using Ttlaixe.Exceptions;
using Ttlaixe.Models;

namespace Ttlaixe.Businesses
{
    [ImplementBy(typeof(LichHocsBusinesses))]
    public interface ILichHocsBusinesses
    {
        Task<List<LichHoc>> GetAllAsync();

        Task<LichHoc> GetByIdAsync(long maLichHoc);

        Task<List<LichHoc>> GetByMaKhAsync(string maKh);

        Task<List<LichHoc>> SearchAsync(string maKh, int? thang, int? tuan, DateTime? fromDate, DateTime? toDate);

        Task<List<LichHocCreatedRequest>> TaoMacDinhLichHoc(string maKhoaHoc);

        Task<List<LichHocsResponse>> ThongTinLichHocFull(LichHocSearchRequest req);

        Task<bool> CreateManyAsync(List<LichHocCreatedRequest> rqs);

        Task<bool> UpdateManyAsync(List<LichHocGiaiDoanRequest> rqs);

        Task<bool> DeleteByMaKhAsync(string maKh);
    }

    public class LichHocsBusinesses : ILichHocsBusinesses
    {
        private readonly GplxCsdtContext _context;
        private readonly IAuthenInfo _authenInfo;
        private readonly TeknovaContext _Tkcontext;
        public LichHocsBusinesses(GplxCsdtContext context, IAuthenInfo authenInfo, TeknovaContext tkcontext)
        {
            _context = context;
            _authenInfo = authenInfo;
            _Tkcontext = tkcontext;
        }

        private async Task EnsureCanEditAsync()
        {
            var logged = _authenInfo.Get();
            if (logged == null || string.IsNullOrWhiteSpace(logged.UserName))
            {
                throw new BadRequestException("Bạn chưa đăng nhập.");
            }

            var actor = await _Tkcontext.UserTkns.FindAsync(logged.UserName);
            if (actor == null)
            {
                throw new BadRequestException("Không tìm thấy thông tin người dùng.");
            }

            if (!actor.QuyenAdmin && !actor.QuyenNhapLieu)
            {
                throw new BadRequestException("Bạn không có quyền thực hiện tính năng này.");
            }
        }

        public async Task<List<LichHoc>> GetAllAsync()
        {
            return await _context.LichHocs
                .AsNoTracking()
                .OrderByDescending(x => x.MaLichHoc)
                .ToListAsync();
        }

        public async Task<LichHoc> GetByIdAsync(long maLichHoc)
        {
            var item = await _context.LichHocs
                .AsNoTracking()
                .FirstOrDefaultAsync(x => x.MaLichHoc == maLichHoc);

            if (item == null)
                throw new NotFoundException("Không tìm thấy lịch học.");

            return item;
        }

        public async Task<List<LichHoc>> GetByMaKhAsync(string maKh)
        {
            if (string.IsNullOrWhiteSpace(maKh))
                throw new BadRequestException("MaKh không hợp lệ.");
            return await _context.LichHocs
                .AsNoTracking()
                .Where(x => x.MaKh == maKh)
                .OrderBy(x => x.Tuan)
                .ToListAsync();
        }

        public async Task<List<LichHoc>> SearchAsync(string maKh, int? thang, int? tuan, DateTime? fromDate, DateTime? toDate)
        {
            var q = _context.LichHocs.AsNoTracking().AsQueryable();

            if (!string.IsNullOrWhiteSpace(maKh))
                q = q.Where(x => x.MaKh == maKh);

            if (thang.HasValue)
                q = q.Where(x => x.Thang == thang.Value);

            if (tuan.HasValue)
                q = q.Where(x => x.Tuan == tuan.Value);

            if (fromDate.HasValue)
                q = q.Where(x => x.TuNgay >= fromDate.Value);

            if (toDate.HasValue)
                q = q.Where(x => x.DenNgay <= toDate.Value);

            return await q
                .OrderBy(x => x.MaKh)
                .ThenBy(x => x.Tuan)
                .ToListAsync();
        }

        public async Task<bool> CreateManyAsync(List<LichHocCreatedRequest> rqs)
        {
            if (rqs == null || rqs.Count == 0)
                throw new BadRequestException("Danh sách rỗng.");

            var maKh = rqs[0]?.MaKh;
            if (string.IsNullOrWhiteSpace(maKh))
                throw new BadRequestException("MaKh không được để trống.");

            if (rqs.Any(x => !string.Equals(x.MaKh, maKh, StringComparison.OrdinalIgnoreCase)))
                throw new BadRequestException("Danh sách chỉ được chứa 1 MaKh duy nhất.");

            // body không được trùng tuần
            var dupTuanBody = rqs.GroupBy(x => x.Tuan).FirstOrDefault(g => g.Count() > 1);
            if (dupTuanBody != null)
                throw new BadRequestException($"Body bị trùng Tuan={dupTuanBody.Key}.");

            foreach (var x in rqs)
            {
                if (x.Tuan <= 0)
                    throw new BadRequestException("Tuan không hợp lệ.");

                if (x.DenNgay < x.TuNgay)
                    throw new BadRequestException("DenNgay phải >= TuNgay.");

                if (string.IsNullOrWhiteSpace(x.GiaiDoan))
                    x.GiaiDoan = "L";
            }

            // chặn trùng trong DB theo (MaKh, Tuan)
            var tuans = rqs.Select(x => x.Tuan).Distinct().ToList();

            var existedTuans = await _context.LichHocs
                .AsNoTracking()
                .Where(x => x.MaKh == maKh && tuans.Contains(x.Tuan))
                .Select(x => x.Tuan)
                .ToListAsync();

            if (existedTuans.Count > 0)
                throw new BadRequestException($"Lịch học đã tồn tại cho MaKh={maKh}, trùng tuần: {string.Join(",", existedTuans)}");

            // map theo Patch
            var entities = new List<LichHoc>();
            rqs.Patch(entities);

            // đảm bảo MaKh set đúng
            foreach (var e in entities)
                e.MaKh = maKh;

            _context.LichHocs.AddRange(entities);

            try
            {
                await _context.SaveChangesAsync();
                return true;
            }
            catch (DbUpdateException ex)
            {
                var baseEx = ex.GetBaseException();
                var detail = baseEx?.Message ?? ex.InnerException?.Message ?? ex.Message;
                throw new BadRequestException("Error found is " + detail);
            }
        }

        public async Task<bool> UpdateManyAsync(List<LichHocGiaiDoanRequest> rqs)
        {
            if (rqs == null || rqs.Count == 0)
                throw new BadRequestException("Danh sách rỗng.");

            foreach (var rq in rqs)
            {
                var tuanHoc = await _context.LichHocs.FindAsync(rq.MaLichHoc)
                    ?? throw new BadRequestException($"Không tìm thấy mã tuần học: {rq.MaLichHoc}");

                tuanHoc.GiaiDoan = rq.GiaiDoan;
            }

            try
            {
                await _context.SaveChangesAsync();
                return true;
            }
            catch (DbUpdateException ex)
            {
                var baseEx = ex.GetBaseException();
                var detail = baseEx?.Message ?? ex.InnerException?.Message ?? ex.Message;
                throw new BadRequestException("Error found is " + detail);
            }
        }

        public async Task<bool> DeleteByMaKhAsync(string maKh)
        {
            await EnsureCanEditAsync();

            if (string.IsNullOrWhiteSpace(maKh))
                throw new BadRequestException("MaKh không hợp lệ.");

            var khoaHoc = await _context.KhoaHocs.FindAsync(maKh)
                ?? throw new NotFoundException("Không tìm thấy khóa học.");

            var now = DateTime.Now;
            if (now < khoaHoc.NgayBg)
                throw new BadRequestException("Khóa học chưa kết thúc nên không được xóa lịch học.");

            var lichHocs = await _context.LichHocs
                .Where(x => x.MaKh == maKh)
                .ToListAsync();

            if (lichHocs.Count == 0)
                throw new NotFoundException("Không có lịch học nào để xóa theo MaKh này.");

            _context.LichHocs.RemoveRange(lichHocs);

            try
            {
                await _context.SaveChangesAsync();
                return true;
            }
            catch (DbUpdateException ex)
            {
                var baseEx = ex.GetBaseException();
                var detail = baseEx?.Message ?? ex.InnerException?.Message ?? ex.Message;
                throw new BadRequestException("Error found is " + detail);
            }
        }

        public async Task<List<LichHocCreatedRequest>> TaoMacDinhLichHoc(string maKhoaHoc)
        {
            if (string.IsNullOrWhiteSpace(maKhoaHoc))
                throw new BadRequestException("Mã khóa học không hợp lệ.");

            // Nếu đã có lịch thì không cho generate
            if (await _context.LichHocs.AnyAsync(x => x.MaKh == maKhoaHoc))
                throw new BadRequestException($"Mã {maKhoaHoc} này đã được tạo lịch. Vui lòng kiểm tra lại.");

            var khoaHoc = await _context.KhoaHocs.FindAsync(maKhoaHoc)
                ?? throw new BadRequestException("Không tìm thấy mã khóa học.");

            if (!khoaHoc.NgayKg.HasValue || !khoaHoc.NgayBg.HasValue)
                throw new BadRequestException("Khóa học chưa có ngày khai giảng hoặc ngày bế giảng.");

            var start = khoaHoc.NgayKg.Value.Date;
            var end = khoaHoc.NgayBg.Value.Date;

            if (end < start)
                throw new BadRequestException("Ngày bế giảng không được nhỏ hơn ngày khai giảng.");

            // ===== Generate lịch tuần =====
            var result = new List<LichHocCreatedRequest>();

            var tuNgay = start;
            var tuan = 1;

            while (tuNgay <= end)
            {
                var denNgay = tuNgay.AddDays(6);
                if (denNgay > end)
                    denNgay = end;

                result.Add(new LichHocCreatedRequest
                {
                    MaKh = maKhoaHoc,
                    Thang = tuNgay.Month,
                    Tuan = tuan,
                    TuNgay = tuNgay,
                    DenNgay = denNgay,

                    // mặc định để FE chỉnh lại
                    GiaiDoan = "L",
                    KiemTra = null,
                    GhiChu = null
                });

                tuan++;
                tuNgay = denNgay.AddDays(1);
            }

            return result;
        }

        public async Task<List<LichHocsResponse>> ThongTinLichHocFull(LichHocSearchRequest req)
        {
            if (req.PageIndex <= 0) req.PageIndex = 1;
            if (req.PageSize <= 0) req.PageSize = 10;

            int skip = (req.PageIndex - 1) * req.PageSize;

            // 1. Lấy danh sách MaKh theo thứ tự mới nhất
            var maKhList = await _context.LichHocs
                .AsNoTracking()
                .GroupBy(x => x.MaKh)
                .Select(g => new
                {
                    MaKh = g.Key,
                    MaxTuNgay = g.Max(x => x.TuNgay)
                })
                .OrderByDescending(x => x.MaxTuNgay)
                .Skip(skip)
                .Take(req.PageSize)
                .Select(x => x.MaKh)
                .ToListAsync();

            if (!maKhList.Any())
                return new List<LichHocsResponse>();

            // 2. Lấy toàn bộ lịch của các MaKh trong page
            var lichHocs = await _context.LichHocs
                .AsNoTracking()
                .Where(x => maKhList.Contains(x.MaKh))
                .OrderBy(x => x.MaKh)
                .ThenBy(x => x.Tuan)
                .ToListAsync();

            // 3. Group lại cho response
            var result = lichHocs
                .GroupBy(x => x.MaKh)
                .Select(g => new LichHocsResponse
                {
                    MaKh = g.Key,
                    thongTinChiTiets = g.Select(x => new LichHocCoBan
                    {
                        MaLichHoc = x.MaLichHoc,
                        Thang = x.Thang,
                        Tuan = x.Tuan,
                        TuNgay = x.TuNgay,
                        DenNgay = x.DenNgay,
                        GiaiDoan = x.GiaiDoan,
                        KiemTra = x.KiemTra,
                        GhiChu = x.GhiChu
                    })
                    .OrderBy(x => x.Tuan)
                    .ToList()
                })
                // đảm bảo đúng thứ tự page (mới nhất trước)
                .OrderByDescending(x =>
                    x.thongTinChiTiets.Max(l => l.TuNgay)
                )
                .ToList();

            return result;
        }


    }
}
