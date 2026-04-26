namespace Ttlaixe.Businesses
{
    [ImplementBy(typeof(LichSuNopHocPhiBusiness))]
    public interface ILichSuNopHocPhiBusiness
    {
        Task<List<LichSuNopHocPhi>> GetByMaDKAsync(string maDK);
        Task<LichSuNopHocPhi> CreateAsync(LichSuNopHocPhi model);
        Task<bool> DeleteAsync(int idNopTien);
    }

    public class LichSuNopHocPhiBusiness : ILichSuNopHocPhiBusiness
    {
        private readonly AppDbContext _context;

        public LichSuNopHocPhiBusiness(AppDbContext context)
        {
            _context = context;
        }

        public async Task<List<LichSuNopHocPhi>> GetByMaDKAsync(string maDK)
        {
            return await _context.LichSuNopHocPhis
                .AsNoTracking()
                .Where(x => x.MaDK == maDK)
                .OrderByDescending(x => x.NgayNop)
                .ThenByDescending(x => x.IdNopTien)
                .ToListAsync();
        }

        public async Task<LichSuNopHocPhi> CreateAsync(LichSuNopHocPhi model)
        {
            var hoSo = await _context.HoSoHocPhis
                .FirstOrDefaultAsync(x => x.MaDK == model.MaDK && !x.BoHoc);

            if (hoSo == null)
                throw new Exception("Không tìm thấy hồ sơ học phí.");

            if (model.SoTienNop <= 0)
                throw new Exception("Số tiền nộp phải lớn hơn 0.");

            var tongDaNopTruoc = await _context.LichSuNopHocPhis
                .Where(x => x.MaDK == model.MaDK)
                .SumAsync(x => (decimal?)x.SoTienNop) ?? 0;

            var tongSauLanNopNay = tongDaNopTruoc + model.SoTienNop;

            if (tongSauLanNopNay > hoSo.HocPhi)
                throw new Exception("Số tiền nộp vượt quá học phí phải nộp.");

            model.NgayNop = model.NgayNop == default ? DateTime.Now : model.NgayNop;
            model.NgayKhoiTao = DateTime.Now;

            _context.LichSuNopHocPhis.Add(model);

            hoSo.DaHoanThanhHp = tongSauLanNopNay >= hoSo.HocPhi;
            hoSo.NgayChinhSuaCuoiCung = DateTime.Now;

            await _context.SaveChangesAsync();

            return model;
        }

        public async Task<bool> DeleteAsync(int idNopTien)
        {
            var item = await _context.LichSuNopHocPhis
                .FirstOrDefaultAsync(x => x.IdNopTien == idNopTien);

            if (item == null)
                return false;

            var maDK = item.MaDK;

            _context.LichSuNopHocPhis.Remove(item);
            await _context.SaveChangesAsync();

            var hoSo = await _context.HoSoHocPhis
                .FirstOrDefaultAsync(x => x.MaDK == maDK);

            if (hoSo != null)
            {
                var tongDaNop = await _context.LichSuNopHocPhis
                    .Where(x => x.MaDK == maDK)
                    .SumAsync(x => (decimal?)x.SoTienNop) ?? 0;

                hoSo.DaHoanThanhHp = tongDaNop >= hoSo.HocPhi;
                hoSo.NgayChinhSuaCuoiCung = DateTime.Now;

                await _context.SaveChangesAsync();
            }

            return true;
        }
    }
}
