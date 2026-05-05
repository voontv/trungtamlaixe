using Microsoft.EntityFrameworkCore;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Threading.Tasks;
using Ttlaixe.AutoConfig;
using Ttlaixe.Exceptions;
using Ttlaixe.Models;

namespace Ttlaixe.Businesses
{
    [ImplementBy(typeof(LichSuSoDuBusiness))]
    public interface ILichSuSoDuBusiness
    {
        Task<List<LichSuSoDu>> GetByNamAsync(int nam);
        Task<bool> UpsertAsync(LichSuSoDu rq);
        Task<bool> UpdateAsync(LichSuSoDu rq);
    }
    public class LichSuSoDuBusiness : ILichSuSoDuBusiness
    {
        private readonly TeknovaContext _context;

        public LichSuSoDuBusiness(TeknovaContext context)
        {
            _context = context;
        }

        // 1. GET theo năm
        public async Task<List<LichSuSoDu>> GetByNamAsync(int nam)
        {
            return await _context.LichSuSoDus
                .AsNoTracking()
                .Where(x => x.Nam == nam)
                .OrderBy(x => x.MaTaiKhoan)
                .Select(x => new LichSuSoDu
                {
                    Nam = x.Nam,
                    MaTaiKhoan = x.MaTaiKhoan,
                    TenTaiKhoan = x.TenTaiKhoan,
                    No = x.No,
                    Co = x.Co
                })
                .ToListAsync();
        }

        // 2. POST: nếu chưa có thì tạo, có rồi thì cập nhật
        public async Task<bool> UpsertAsync(LichSuSoDu rq)
        {
            var entity = await _context.LichSuSoDus
                .FirstOrDefaultAsync(x => x.Nam == rq.Nam && x.MaTaiKhoan == rq.MaTaiKhoan);

            if (entity == null)
            {
                var tenTk = await _context.DmTaiKhoanKeToans
                    .Where(x => x.MaTaiKhoan == rq.MaTaiKhoan)
                    .Select(x => x.TenTaiKhoan)
                    .FirstOrDefaultAsync();
                if(tenTk == null)
                {
                    throw new BadRequestException("Mã tài khoảng không có trong hệ thống");
                }
                entity = new LichSuSoDu
                {
                    Nam = rq.Nam,
                    MaTaiKhoan = rq.MaTaiKhoan,
                    TenTaiKhoan = tenTk,
                    No = rq.No,
                    Co = rq.Co
                };

                _context.LichSuSoDus.Add(entity);
            }
            else
            {
                entity.No = rq.No;
                entity.Co = rq.Co;
            }

            await _context.SaveChangesAsync();
            return true;
        }

        // 3. PUT: bắt buộc phải tồn tại mới update
        public async Task<bool> UpdateAsync(LichSuSoDu rq)
        {
            var entity = await _context.LichSuSoDus
                .FirstOrDefaultAsync(x => x.Nam == rq.Nam && x.MaTaiKhoan == rq.MaTaiKhoan);

            if (entity == null)
                throw new Exception("Không tìm thấy số dư để cập nhật.");

            entity.No = rq.No;
            entity.Co = rq.Co;

            await _context.SaveChangesAsync();
            return true;
        }
    }
}
