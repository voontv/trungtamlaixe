using Microsoft.AspNetCore.Mvc;
using Microsoft.EntityFrameworkCore;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Threading.Tasks;
using Ttlaixe.AutoConfig;
using Ttlaixe.DTO.request;
using Ttlaixe.Exceptions;
using Ttlaixe.Models;
using Ttlaixe.Providers;

namespace Ttlaixe.Businesses
{
    [ImplementBy(typeof(XeTapsBusinesses))]
    public interface IXeTapsBusinesses
    {
        Task<List<XeTap>> GetAllAsync();
        Task<XeTap> GetByIdAsync(string bienSoXe);

        Task<XeTap> CreateAsync(XeTapCreatedRequest rq);
        Task<bool> UpdateAsync(XeTap rq);
        Task<bool> DeleteAsync(string bienSoXe);

        Task<List<XeTap>> SearchAsync(string keyword, string maCsdt, string maSoGtvt, bool? trangThai);
    }

    public class XeTapsBusinesses : ControllerBase, IXeTapsBusinesses
    {
        private readonly GplxCsdtContext _context;
        private readonly IAuthenInfo _authenInfo;

        public XeTapsBusinesses(GplxCsdtContext context, IAuthenInfo authenInfo)
        {
            _context = context;
            _authenInfo = authenInfo;
        }

        private async Task CheckQuyen()
        {
            var logged = _authenInfo.Get();
            if (logged == null || string.IsNullOrWhiteSpace(logged.UserName))
                throw new BadRequestException("Bạn chưa đăng nhập hoặc token không hợp lệ.");

            var actor = await _context.UserTkns.FindAsync(logged.UserName);
            if (actor == null)
                throw new BadRequestException("Không tìm thấy thông tin người dùng.");

            if (!actor.QuyenAdmin && !actor.QuyenNhapLieu)
                throw new BadRequestException("Bạn không có quyền thực hiện tính năng này.");
        }

        public async Task<List<XeTap>> GetAllAsync()
        {
            return await _context.XeTaps
                .AsNoTracking()
                .OrderByDescending(x => x.NgayTao)
                .ToListAsync();
        }

        public async Task<XeTap> GetByIdAsync(string bienSoXe)
        {
            if (string.IsNullOrWhiteSpace(bienSoXe))
                throw new BadRequestException("BienSoXe không được để trống.");

            bienSoXe = bienSoXe.Trim();

            var entity = await _context.XeTaps
                .AsNoTracking()
                .FirstOrDefaultAsync(x => x.BienSoXe == bienSoXe);

            if (entity == null)
                throw new BadRequestException($"Không tìm thấy XeTap với BienSoXe = {bienSoXe}");

            return entity;
        }

        public async Task<XeTap> CreateAsync(XeTapCreatedRequest rq)
        {
            await CheckQuyen();

            if (rq == null)
                throw new BadRequestException("Dữ liệu không hợp lệ.");

            if (string.IsNullOrWhiteSpace(rq.BienSoXe))
                throw new BadRequestException("BienSoXe không được để trống.");

            rq.BienSoXe = rq.BienSoXe.Trim();

            var existed = await _context.XeTaps.AnyAsync(x => x.BienSoXe == rq.BienSoXe);
            if (existed)
                throw new BadRequestException($"Biển số xe {rq.BienSoXe} đã tồn tại.");

            var now = DateTime.Now;

            var xeTap = new XeTap();
            rq.Patch(xeTap);
            // Không mapping/patch gì thêm theo yêu cầu của bạn.
            // Chỉ set các field hệ thống tối thiểu:
            if (xeTap.TrangThai == null) rq.TrangThai = true;
            xeTap.NgayTao = now;
            xeTap.NgaySua = now;

            _context.XeTaps.Add(xeTap);

            try
            {
                await _context.SaveChangesAsync();
            }
            catch (DbUpdateException ex)
            {
                throw new BadRequestException("Lỗi lưu dữ liệu: " + ex.Message);
            }

            return xeTap;
        }

        public async Task<bool> UpdateAsync(XeTap rq)
        {
            await CheckQuyen();

            if (rq == null)
                throw new BadRequestException("Dữ liệu không hợp lệ.");

            if (string.IsNullOrWhiteSpace(rq.BienSoXe))
                throw new BadRequestException("BienSoXe không được để trống.");

            var bienSoXe = rq.BienSoXe.Trim();

            var entity = await _context.XeTaps.FirstOrDefaultAsync(x => x.BienSoXe == bienSoXe);
            if (entity == null)
                throw new BadRequestException($"Không tìm thấy XeTap với BienSoXe = {bienSoXe}");

            // KHÔNG patch mapping (bạn tự xử lý) => mình chỉ set NgaySua.
            // Nếu bạn đã Patch bên ngoài trước khi gọi UpdateAsync thì bỏ qua phần đó.
            // Ở đây mình vẫn chấp nhận bạn muốn copy property thủ công ở chỗ khác.
            // => Mình chỉ đảm bảo không đổi khóa chính, và update NgaySua.

            entity.NgaySua = DateTime.Now;

            try
            {
                await _context.SaveChangesAsync();
            }
            catch (DbUpdateException ex)
            {
                throw new BadRequestException("Lỗi cập nhật dữ liệu: " + ex.Message);
            }

            return true;
        }

        public async Task<bool> DeleteAsync(string bienSoXe)
        {
            await CheckQuyen();

            if (string.IsNullOrWhiteSpace(bienSoXe))
                throw new BadRequestException("BienSoXe không được để trống.");

            bienSoXe = bienSoXe.Trim();

            var entity = await _context.XeTaps.FirstOrDefaultAsync(x => x.BienSoXe == bienSoXe);
            if (entity == null)
                throw new BadRequestException($"Không tìm thấy XeTap với BienSoXe = {bienSoXe}");

            _context.XeTaps.Remove(entity);

            try
            {
                await _context.SaveChangesAsync();
            }
            catch (DbUpdateException ex)
            {
                throw new BadRequestException("Lỗi xóa dữ liệu: " + ex.Message);
            }

            return true;
        }

        public async Task<List<XeTap>> SearchAsync(string keyword, string maCsdt, string maSoGtvt, bool? trangThai)
        {
            var q = _context.XeTaps.AsNoTracking().AsQueryable();

            if (!string.IsNullOrWhiteSpace(keyword))
            {
                keyword = keyword.Trim();
                q = q.Where(x =>
                    x.BienSoXe.Contains(keyword) ||
                    (x.SoDk != null && x.SoDk.Contains(keyword)) ||
                    (x.NhanHieu != null && x.NhanHieu.Contains(keyword)) ||
                    (x.LoaiXe != null && x.LoaiXe.Contains(keyword)) ||
                    (x.HangXe != null && x.HangXe.Contains(keyword)) ||
                    (x.MauXe != null && x.MauXe.Contains(keyword)) ||
                    (x.SoDongCo != null && x.SoDongCo.Contains(keyword)) ||
                    (x.SoKhung != null && x.SoKhung.Contains(keyword))
                );
            }

            if (!string.IsNullOrWhiteSpace(maCsdt))
            {
                maCsdt = maCsdt.Trim();
                q = q.Where(x => x.MaCsdt == maCsdt);
            }

            if (!string.IsNullOrWhiteSpace(maSoGtvt))
            {
                maSoGtvt = maSoGtvt.Trim();
                q = q.Where(x => x.MaSoGtvt == maSoGtvt);
            }

            if (trangThai.HasValue)
            {
                q = q.Where(x => x.TrangThai == trangThai.Value);
            }

            return await q
                .OrderByDescending(x => x.NgayTao)
                .ToListAsync();
        }
    }
}
