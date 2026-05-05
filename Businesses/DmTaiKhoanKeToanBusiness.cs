using System.Collections.Generic;
using System.Threading.Tasks;
using System;
using Ttlaixe.AutoConfig;
using System.Linq;
using Ttlaixe.DTO.response;
using Ttlaixe.Models;
using Microsoft.EntityFrameworkCore;

namespace Ttlaixe.Businesses
{
    [ImplementBy(typeof(DmTaiKhoanKeToanBusiness))]
    public interface IDmTaiKhoanKeToanBusiness
    {
        Task<List<DmTaiKhoanKeToanTreeDto>> GetTreeAsync();
        Task<List<DmTaiKhoanKeToanTreeDto>> GetTreeByLoaiAsync(string loaiTaiKhoan);
    }

    public class DmTaiKhoanKeToanBusiness : IDmTaiKhoanKeToanBusiness
    {
        private readonly TeknovaContext _context;

        public DmTaiKhoanKeToanBusiness(TeknovaContext context)
        {
            _context = context;
        }

        public async Task<List<DmTaiKhoanKeToanTreeDto>> GetTreeAsync()
        {
            var data = await _context.DmTaiKhoanKeToans
                .AsNoTracking()
                .Where(x => (bool)x.IsActive)
                .OrderBy(x => x.TenTaiKhoan)
                .ThenBy(x => x.MaTaiKhoan)
                .Select(x => new DmTaiKhoanKeToanTreeDto
                {
                    MaTaiKhoan = x.MaTaiKhoan,
                    TenTaiKhoan = x.TenTaiKhoan,
                    MaTaiKhoanCha = x.MaTaiKhoanCha,
                    Cap = x.Cap,
                    MaLoaiTaiKhoan = x.MaLoaiTaiKhoan,
                    SoThuTu = x.SoThuTu
                })
                .ToListAsync();

            return BuildTree(data);
        }

        public async Task<List<DmTaiKhoanKeToanTreeDto>> GetTreeByLoaiAsync(string loaiTaiKhoan)
        {
            var data = await _context.DmTaiKhoanKeToans
                .AsNoTracking()
                .Where(x => (bool) x.IsActive && x.MaLoaiTaiKhoan == loaiTaiKhoan)
                .OrderBy(x => x.SoThuTu)
                .ThenBy(x => x.MaTaiKhoan)
                .Select(x => new DmTaiKhoanKeToanTreeDto
                {
                    MaTaiKhoan = x.MaTaiKhoan,
                    TenTaiKhoan = x.TenTaiKhoan,
                    MaTaiKhoanCha = x.MaTaiKhoanCha,
                    Cap = x.Cap,
                    MaLoaiTaiKhoan = x.MaLoaiTaiKhoan,
                    SoThuTu = x.SoThuTu
                })
                .ToListAsync();

            return BuildTree(data);
        }

        private List<DmTaiKhoanKeToanTreeDto> BuildTree(List<DmTaiKhoanKeToanTreeDto> data)
        {
            var lookup = data.ToDictionary(x => x.MaTaiKhoan, x => x);

            foreach (var item in data)
            {
                if (!string.IsNullOrWhiteSpace(item.MaTaiKhoanCha)
                    && lookup.TryGetValue(item.MaTaiKhoanCha, out var parent))
                {
                    parent.Children.Add(item);
                }
            }

            return data
                .Where(x => string.IsNullOrWhiteSpace(x.MaTaiKhoanCha))
                .OrderBy(x => x.TenTaiKhoan)
                .ThenBy(x => x.MaTaiKhoan)
                .ToList();
        }
    }

}
