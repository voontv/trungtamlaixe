using Microsoft.AspNetCore.Http;
using Microsoft.EntityFrameworkCore;
using System;
using System.IO;
using System.Threading.Tasks;
using Ttlaixe.DTO.response;
using Ttlaixe.Models;

namespace Ttlaixe.Providers
{
    public interface IImageGplxService
    {
        Task<string> SaveAsync(IFormFile file, string maDk, string soCmt);
    }
    public class ImageGplxService : IImageGplxService
    {
        private readonly GplxCsdtContext _context;

        public ImageGplxService(GplxCsdtContext context)
        {
            _context = context;
        }

        public async Task<string> SaveAsync(IFormFile file, string maDk, string soCmt)
        {
            var ts = await _context.QthtThamSoHts
                .FirstOrDefaultAsync(x => x.TenTs == "IMG_PATH_CSDT");

            var image_path = ts?.GiaTriTs
                ?? @"\\192.168.100.248\d\2026\im_gplx";

            var resolver = new ImagePathResolver(image_path);
            var year = DateTime.Now.Year.ToString();

            var folder = Path.Combine(resolver.LocalRoot, year, resolver.BaseFolder, maDk);
            Directory.CreateDirectory(folder);

            var ext = Path.GetExtension(file.FileName);
            var fileName = $"{soCmt}-{DateTime.Now:yyyyMMdd-HHmmss}{ext}";
            var fullPath = Path.Combine(folder, fileName);

            using var stream = new FileStream(fullPath, FileMode.Create);
            await file.CopyToAsync(stream);

            return Path.Combine(resolver.UncRoot, year, resolver.BaseFolder, maDk, fileName);
        }
    }
}
