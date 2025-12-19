using Microsoft.EntityFrameworkCore;
using System.Collections.Generic;
using System.Linq;
using System.Threading.Tasks;
using System;
using Ttlaixe.AutoConfig;
using Ttlaixe.Exceptions;
using Ttlaixe.Models;

namespace Ttlaixe.Businesses
{
    [ImplementBy(typeof(GiaoViensBusinesses))]
    public interface IGiaoViensBusinesses
    {
        Task<List<GiaoVien>> GetAllAsync();
        Task<GiaoVien> GetByIdAsync(string maGv);
        Task<GiaoVien> CreateAsync(GiaoVien request);
        Task<bool> UpdateAsync(GiaoVien request);
        Task<bool> DeleteAsync(string maGv);
    }

    public class GiaoViensBusinesses : IGiaoViensBusinesses
    {
        private readonly GplxCsdtContext _context;
        private readonly IAuthenInfo _authenInfo;

        public GiaoViensBusinesses(
            GplxCsdtContext context,
            IAuthenInfo authenInfo)
        {
            _context = context;
            _authenInfo = authenInfo;
        }

        public async Task<List<GiaoVien>> GetAllAsync()
        {
            return await _context.GiaoViens
                .OrderBy(x => x.MaGv)
                .ToListAsync();
        }

        public async Task<GiaoVien> GetByIdAsync(string maGv)
        {
            var entity = await _context.GiaoViens.FindAsync(maGv);
            if (entity == null)
                throw new NotFoundException($"Không tìm thấy giáo viên {maGv}");

            return entity;
        }

        public async Task<GiaoVien> CreateAsync(GiaoVien request)
        {
            var now = DateTime.Now;
            var user = _authenInfo.Get()?.UserName ?? "SYSTEM";

            if (string.IsNullOrWhiteSpace(request.MaCsdt))
                throw new BadRequestException("MaCsdt không được để trống");

            // ===== Sinh MaGv = <MaCsdt><001-999> =====
            var prefix = request.MaCsdt.Trim();

            var maGvList = await _context.GiaoViens
                .Where(x => x.MaCsdt == prefix && x.MaGv != null && x.MaGv.StartsWith(prefix))
                .Select(x => x.MaGv)
                .ToListAsync();

            var maxIndex = 0;
            foreach (var maGv in maGvList)
            {
                // đảm bảo đủ độ dài để lấy 3 số cuối
                if (maGv.Length < prefix.Length + 3) continue;

                var suffix = maGv.Substring(prefix.Length); // "001" / "015" / ...
                if (int.TryParse(suffix, out var n))
                    if (n > maxIndex) maxIndex = n;
            }

            var nextIndex = maxIndex + 1;
            if (nextIndex > 999)
                throw new BadRequestException("Số lượng giáo viên đã vượt quá 999");


            request.MaGv = $"{request.MaCsdt}{nextIndex:D3}";

            request.TrangThai ??= true;
            request.NguoiTao = user;
            request.NguoiSua = user;
            request.NgayTao = now;
            request.NgaySua = now;

            _context.GiaoViens.Add(request);
            await _context.SaveChangesAsync();

            return request;
        }

        public async Task<bool> UpdateAsync(GiaoVien request)
        {
            if (string.IsNullOrWhiteSpace(request.MaGv))
                throw new BadRequestException("MaGv không được để trống");

            var entity = await _context.GiaoViens.FindAsync(request.MaGv);
            if (entity == null)
                throw new NotFoundException($"Không tìm thấy giáo viên {request.MaGv}");

            var user = _authenInfo.Get()?.UserName ?? "SYSTEM";

            request.Patch(entity);
            entity.NguoiSua = user;
            entity.NgaySua = DateTime.Now;

            await _context.SaveChangesAsync();
            return true;
        }

        public async Task<bool> DeleteAsync(string maGv)
        {
            var entity = await _context.GiaoViens.FindAsync(maGv);
            if (entity == null)
                throw new NotFoundException($"Không tìm thấy giáo viên {maGv}");

            entity.TrangThai = false;
            await _context.SaveChangesAsync();
            return true;
        }
    }
}
