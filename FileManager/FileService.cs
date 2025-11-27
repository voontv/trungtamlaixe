using Microsoft.AspNetCore.Hosting;
using Microsoft.AspNetCore.Http;
using System;
using System.Collections.Generic;
using System.IO;
using System.IO.Compression;
using System.Linq;
using System.Threading.Tasks;

namespace Ttlaixe.FileManager
{
    public class FileService : IFileService
    {
        #region Property  
        private IHostingEnvironment _hostingEnvironment;
        #endregion

        #region Constructor  
        public FileService(IHostingEnvironment hostingEnvironment)
        {
            _hostingEnvironment = hostingEnvironment;
        }
        #endregion

        #region Upload File  
        public async Task<List<string>> UploadFile(List<IFormFile> files, string subDirectory)
        {
            DateTime time = new DateTime(2000, 1, 1, 0, 0, 0, DateTimeKind.Utc);
            //string folder = ((long)(DateTime.UtcNow - time).TotalMilliseconds).ToString();
            subDirectory = subDirectory ?? string.Empty;
            var target = Path.Combine(_hostingEnvironment.ContentRootPath, subDirectory);

            Directory.CreateDirectory(target);
            // folder = folder ?? string.Empty;  
            var tar = Path.Combine(_hostingEnvironment.ContentRootPath + "/", subDirectory);

            Directory.CreateDirectory(tar);
            List<string> listFile = new List<string>();

            files.ForEach(async file =>
            {
                if (file.Length <= 0) return;
                string fileName = $"{file.FileName}";
                listFile.Add(subDirectory + "/" + $"{file.FileName}");
                var pathString = System.IO.Path.Combine(subDirectory, fileName);
                using (FileStream fs = System.IO.File.Create(pathString))
                {
                    file.CopyTo(fs);
                    fs.Flush();
                }
            });

            return listFile;
        }
        #endregion

        #region Download File  
        public (string fileType, byte[] archiveData, string archiveName) DownloadFiles(string subDirectory)
        {
            var zipName = $"archive-{DateTime.Now.ToString("yyyy_MM_dd-HH_mm_ss")}.zip";

            var files = Directory.GetFiles(Path.Combine(_hostingEnvironment.ContentRootPath, subDirectory)).ToList();

            using (var memoryStream = new MemoryStream())
            {
                using (var archive = new ZipArchive(memoryStream, ZipArchiveMode.Create, true))
                {
                    files.ForEach(file =>
                    {
                        var theFile = archive.CreateEntry(file);
                        using (var streamWriter = new StreamWriter(theFile.Open()))
                        {
                            streamWriter.Write(File.ReadAllText(file));
                        }

                    });
                }

                return ("application/zip", memoryStream.ToArray(), zipName);
            }

        }
        #endregion

        #region Size Converter  
        public string SizeConverter(long bytes)
        {
            var fileSize = new decimal(bytes);
            var kilobyte = new decimal(1024);
            var megabyte = new decimal(1024 * 1024);
            var gigabyte = new decimal(1024 * 1024 * 1024);

            switch (fileSize)
            {
                case var _ when fileSize < kilobyte:
                    return $"Less then 1KB";
                case var _ when fileSize < megabyte:
                    return $"{Math.Round(fileSize / kilobyte, 0, MidpointRounding.AwayFromZero):##,###.##}KB";
                case var _ when fileSize < gigabyte:
                    return $"{Math.Round(fileSize / megabyte, 2, MidpointRounding.AwayFromZero):##,###.##}MB";
                case var _ when fileSize >= gigabyte:
                    return $"{Math.Round(fileSize / gigabyte, 2, MidpointRounding.AwayFromZero):##,###.##}GB";
                default:
                    return "n/a";
            }
        }
        #endregion

    }
}