using System.Collections.Generic;
using System.Threading.Tasks;
using System;
using Ttlaixe.AutoConfig;
using Ttlaixe.Models;
using Microsoft.EntityFrameworkCore;
using System.Linq;
using Ttlaixe.DTO.request;
using Ttlaixe.DTO.response;

namespace Ttlaixe.Businesses
{
    [ImplementBy(typeof(LichSuNopHocPhiBusiness))]
    public interface ILichSuNopHocPhiBusiness
    {
        Task<List<LichSuNopHocPhiReponse>> GetByMaDKAsync(string maDK);
        Task<LichSuNopHocPhiReponse> CreateAsync(LichSuNopHocPhiRequest model);
        Task<bool> DeleteAsync(int idNopTien);
    }

    public class LichSuNopHocPhiBusiness : ILichSuNopHocPhiBusiness
    {
        private readonly TeknovaContext _context;
        private readonly INhatKyChungTuBusiness _nhatKyChungTu;

        public LichSuNopHocPhiBusiness(TeknovaContext context, INhatKyChungTuBusiness nhatKyChungTu)
        {
            _context = context;
            _nhatKyChungTu = nhatKyChungTu;
        }

        public async Task<List<LichSuNopHocPhiReponse>> GetByMaDKAsync(string maDK)
        {
            var lichSuNopHocPhis =  await _context.LichSuNopHocPhis
                .AsNoTracking()
                .Where(x => x.MaDk == maDK)
                .OrderByDescending(x => x.NgayNop)
                .ThenByDescending(x => x.IdNopTien)
                .ToListAsync();

            var lichSuNopHocPhiReponses = new List<LichSuNopHocPhiReponse>();
            lichSuNopHocPhis.Patch(lichSuNopHocPhiReponses);

            return lichSuNopHocPhiReponses;
        }

        public async Task<LichSuNopHocPhiReponse> CreateAsync(LichSuNopHocPhiRequest model)
        {
            var hoSo = await _context.HoSoHocPhis
                .FirstOrDefaultAsync(x => x.MaDk == model.MaDk && (bool) !x.BoHoc);

            if (hoSo == null)
                throw new Exception("Không tìm thấy hồ sơ học phí.");

            if (model.SoTienNop <= 0)
                throw new Exception("Số tiền nộp phải lớn hơn 0.");

            var tongDaNopTruoc = await _context.LichSuNopHocPhis
                .Where(x => x.MaDk == model.MaDk)
                .SumAsync(x => (decimal?)x.SoTienNop) ?? 0;

            var tongSauLanNopNay = tongDaNopTruoc + model.SoTienNop;

            if (tongSauLanNopNay > hoSo.HocPhi)
                throw new Exception("Số tiền nộp vượt quá học phí phải nộp.");

            model.NgayNop = model.NgayNop == default ? DateTime.Now : model.NgayNop;
            var lichSuNop = new LichSuNopHocPhi();
            model.Patch(lichSuNop);
            lichSuNop.NgayKhoiTao = DateTime.Now;

            _context.LichSuNopHocPhis.Add(lichSuNop);

            hoSo.DaHoanThanhHp = tongSauLanNopNay >= hoSo.HocPhi;
            hoSo.NgayChinhSuaCuoiCung = DateTime.Now;

            await _context.SaveChangesAsync();
            var result = new LichSuNopHocPhiReponse();
            lichSuNop.Patch(result);

            var nhatKyChungTu = new NhatKyChungTuRequest();
            nhatKyChungTu.SoChungTu = model.MaDk;
            nhatKyChungTu.GhiChu = model.GhiChu;
            nhatKyChungTu.NgayLap = model.NgayNop;
            nhatKyChungTu.SoTien = model.SoTienNop;
            nhatKyChungTu.DienGiai = "Học viên " + hoSo.HoVaTen + " nộp tiền học phí";
            nhatKyChungTu.TaiKhoanCo = model.TaiKhoanCo;
            nhatKyChungTu.TaiKhoanNo = model.TaiKhoanNo;
            await _nhatKyChungTu.CreateAsync(nhatKyChungTu);
            return result;
        }

        public async Task<bool> DeleteAsync(int idNopTien)
        {
            var item = await _context.LichSuNopHocPhis
                .FirstOrDefaultAsync(x => x.IdNopTien == idNopTien);

            if (item == null)
                return false;

            var maDK = item.MaDk;

            _context.LichSuNopHocPhis.Remove(item);
            await _context.SaveChangesAsync();

            var hoSo = await _context.HoSoHocPhis
                .FirstOrDefaultAsync(x => x.MaDk == maDK);

            if (hoSo != null)
            {
                var tongDaNop = await _context.LichSuNopHocPhis
                    .Where(x => x.MaDk == maDK)
                    .SumAsync(x => (decimal?)x.SoTienNop) ?? 0;

                hoSo.DaHoanThanhHp = tongDaNop >= hoSo.HocPhi;
                hoSo.NgayChinhSuaCuoiCung = DateTime.Now;

                await _context.SaveChangesAsync();

                await _nhatKyChungTu.XoaChungTuTheoSoChungTu(item.MaDk, item.SoTienNop, item.NgayNop);
            }

            return true;
        }
    }
}
