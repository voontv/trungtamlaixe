using System;
using System.Collections.Generic;
using System.IO;
using System.Linq;
using System.Threading.Tasks;
using ImageMagick;
using log4net;
using Microsoft.Extensions.Configuration;

namespace Ttlaixe.DriverApi
{
    public interface IDriverPhotoService
    {
        /// <summary>The photo resized to the report's portrait box as base64 JPEG, or null when unavailable.</summary>
        Task<string> LoadAsync(string stored);
    }

    public class DriverPhotoService : IDriverPhotoService
    {
        /// <summary>32 x 44 mm portrait box of the exam report, at about 190 dpi.</summary>
        public const int PhotoWidth = 240;
        public const int PhotoHeight = 330;
        private const int JpegQuality = 82;

        private static readonly ILog Log = LogManager.GetLogger(typeof(DriverPhotoService));

        private readonly IConfiguration _config;

        public DriverPhotoService(IConfiguration config)
        {
            _config = config;
        }

        public async Task<string> LoadAsync(string stored)
        {
            if (string.IsNullOrEmpty(stored))
            {
                return null;
            }

            var file = Locate(stored);
            if (file == null)
            {
                Log.Warn("Photo path is not under any configured path map");
                return null;
            }

            try
            {
                var content = await File.ReadAllBytesAsync(file);
                return Convert.ToBase64String(Resize(content));
            }
            catch (Exception ex)
            {
                Log.Warn($"Photo could not be processed: {ex.Message}");
                return null;
            }
        }

        /// <summary>
        /// Turns a stored photo path (a Windows share path such as \\server\B\2026\x.jp2) into a path below the
        /// directory where that share is mounted, or null when the path is outside the share.
        /// </summary>
        public static string MapPhotoPath(string stored, string sharePrefix, string mountDir)
        {
            static string Normalize(string value) => value.Replace('\\', '/');

            var prefix = Normalize(sharePrefix).TrimEnd('/').ToLowerInvariant();
            var source = Normalize(stored.Trim());

            if (!source.ToLowerInvariant().StartsWith(prefix + "/", StringComparison.Ordinal))
            {
                return null;
            }

            var root = Path.GetFullPath(mountDir);
            var target = Path.GetFullPath(Path.Combine(root, source.Substring(prefix.Length + 1)));
            var relative = Path.GetRelativePath(root, target);

            return relative == "" || relative == "." || relative.StartsWith("..", StringComparison.Ordinal) || Path.IsPathRooted(relative)
                ? null
                : target;
        }

        /// <summary>First path map (`source=mountDir`, comma separated in Photo:PathMaps) that contains the path.</summary>
        private string Locate(string stored)
        {
            var maps = (_config["Photo:PathMaps"] ?? "")
                .Split(',')
                .Select(entry => entry.Trim())
                .Where(entry => entry.Length > 0);

            foreach (var entry in maps)
            {
                var separator = entry.LastIndexOf('=');
                if (separator < 0)
                {
                    continue;
                }

                var file = MapPhotoPath(stored, entry.Substring(0, separator), entry.Substring(separator + 1));
                if (file != null)
                {
                    return file;
                }
            }

            return null;
        }

        private static byte[] Resize(byte[] content)
        {
            using var image = new MagickImage(content);
            image.AutoOrient();
            // Fill the box, then keep its top: portraits lose their bottom, not the face.
            image.Resize(new MagickGeometry(PhotoWidth, PhotoHeight) { FillArea = true });
            image.Crop(PhotoWidth, PhotoHeight, Gravity.North);
            image.ResetPage();
            image.Strip();
            image.Format = MagickFormat.Jpeg;
            image.Quality = JpegQuality;

            return image.ToByteArray();
        }
    }
}
