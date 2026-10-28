using System;
using System.IO;
using System.Linq;
using System.Threading.Tasks;
using ImageMagick;
using log4net;

namespace Ttlaixe.DriverApi
{
    public interface IDriverPhotoService
    {
        /// <summary>The photo resized to the report's portrait box as base64 JPEG, or null when unavailable.</summary>
        Task<string> LoadAsync(string stored);
    }

    /// <summary>Turns the stored photo path (DuongDanAnh) into a file this server can read, or null.</summary>
    public interface IDriverPhotoPathResolver
    {
        string Resolve(string stored);
    }

    /// <summary>
    /// Reads the stored path as it is: DuongDanAnh holds a UNC path (\\server\B\2026\...) of the image share,
    /// which the Windows host running this API can open directly. Anything that is not an absolute path, or that
    /// climbs out of its folder with "..", is refused.
    /// </summary>
    public class DirectPhotoPathResolver : IDriverPhotoPathResolver
    {
        public string Resolve(string stored)
        {
            var path = stored.Trim();
            if (!Path.IsPathRooted(path))
            {
                return null;
            }

            var segments = path.Split('\\', '/');
            return segments.Contains("..") ? null : path;
        }
    }

    public class DriverPhotoService : IDriverPhotoService
    {
        /// <summary>32 x 44 mm portrait box of the exam report, at about 190 dpi.</summary>
        public const int PhotoWidth = 240;
        public const int PhotoHeight = 330;
        private const int JpegQuality = 82;

        private static readonly ILog Log = LogManager.GetLogger(typeof(DriverPhotoService));

        private readonly IDriverPhotoPathResolver _paths;

        public DriverPhotoService(IDriverPhotoPathResolver paths)
        {
            _paths = paths;
        }

        public async Task<string> LoadAsync(string stored)
        {
            if (string.IsNullOrEmpty(stored))
            {
                return null;
            }

            var file = _paths.Resolve(stored);
            if (file == null)
            {
                Log.Warn("Photo path is not readable by this server");
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
