using System;
using System.Linq;

namespace Ttlaixe.DTO.response
{
    public class ImagePathResolver
    {
        public string UncRoot { get; }
        public string LocalRoot { get; }
        public string BaseFolder { get; }

        public ImagePathResolver(string pathImageHt)
        {
            // \\192.168.100.248\d\2026\im_gplx

            var parts = pathImageHt
                .Trim('\\')
                .Split('\\', StringSplitOptions.RemoveEmptyEntries);

            // IP = phần đầu
            var ip = parts.First();

            // Drive = phần thứ 2
            var drive = parts.Skip(1).First();

            // BaseFolder = phần cuối
            BaseFolder = parts.Last();

            UncRoot = $@"\\{ip}\{drive}\";
            LocalRoot = $@"{drive.ToUpper()}:\";
        }
    }
}
