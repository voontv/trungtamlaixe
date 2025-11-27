using Microsoft.AspNetCore.Http;
using Microsoft.Extensions.Logging;
using Renci.SshNet;
using Renci.SshNet.Common;
using System;
using System.Collections.Generic;
using System.IO;
using System.Net.Sockets;
using System.Threading.Tasks;
using Ttlaixe.Constants;

namespace Ttlaixe.FileManager
{
    public class SFTPFileService : ISFTPFileService
    {
        private readonly ILogger<SFTPFileService> _logger;

        public SFTPFileService(ILogger<SFTPFileService> logger)
        {
            _logger = logger;
        }

        public class UploadResult
        {
            public string FileName { get; set; }
            public string LocalPath { get; set; }
            public string SftpPath { get; set; }
            public string FileUrl { get; set; }
        }

        public async Task<UploadResult> UploadFile(IFormFile file, string remoteFolder)
        {
            if (file == null || file.Length == 0)
                throw new Exception("File không hợp lệ.");

            var year = DateTime.Now.Year.ToString();
            var month = DateTime.Now.Month.ToString("D2");
            var fileName = $"{Guid.NewGuid()}_{Path.GetFileName(file.FileName)}";

            // === LOCAL SAVE ===
            var localDir = Path.Combine(ConstantsApp.WebRootPath,remoteFolder.Trim('/'), year, month);
            if (!Directory.Exists(localDir))
                Directory.CreateDirectory(localDir);

            var localPath = Path.Combine(localDir, fileName);
            using (var stream = new FileStream(localPath, FileMode.Create))
            {
                await file.CopyToAsync(stream);
            }

            _logger.LogInformation($"[LOCAL] Đã lưu file {fileName} tại {localPath}");

            // === SFTP UPLOAD ===
            string remoteDir = $"{ConstantsApp.RemoteRoot.TrimEnd('/')}/{remoteFolder.Trim('/')}/{year}/{month}/";
            string remotePath = $"{remoteDir}{fileName}";

            try
            {
                using var sftp = new SftpClient(
                    ConstantsApp.SftpHost,
                    ConstantsApp.SftpPort,
                    ConstantsApp.SftpUsername,
                    ConstantsApp.SftpPassword
                );

                sftp.Connect();
                if (!sftp.IsConnected)
                    throw new Exception("Không thể kết nối SFTP server.");

                CreateRemoteDirectoryRecursive(sftp, remoteDir);

                await Task.Run(() =>
                {
                    using var uplStream = File.OpenRead(localPath);
                    sftp.UploadFile(uplStream, remotePath);
                });

                sftp.Disconnect();
                _logger.LogInformation($"[SFTP] Đã upload file lên {remotePath}");
            }
            catch (Exception ex) when (ex is SshConnectionException || ex is SocketException || ex is ProxyException)
            {
                _logger.LogError($"[SFTP ERROR] {ex.Message}");
                throw new Exception("Không thể upload file lên SFTP: " + ex.Message);
            }

            return new UploadResult
            {
                FileName = fileName,
                LocalPath = localPath,
                SftpPath = remotePath,
                FileUrl = $"{ConstantsApp.BaseUrl}/{remoteFolder.Trim('/')}/{year}/{month}/{fileName}"
            };
        }

        public async Task<List<UploadResult>> UploadMultipleFiles(List<IFormFile> files, string remoteFolder)
        {
            var results = new List<UploadResult>();
            foreach (var file in files)
            {
                results.Add(await UploadFile(file, remoteFolder));
            }
            return results;
        }

        private void CreateRemoteDirectoryRecursive(SftpClient sftp, string path)
        {
            var parts = path.Split('/', StringSplitOptions.RemoveEmptyEntries);
            var current = "/";
            foreach (var part in parts)
            {
                current = $"{current}{part}/";
                if (!sftp.Exists(current))
                {
                    _logger.LogInformation($"[SFTP] Tạo thư mục {current}");
                    sftp.CreateDirectory(current);
                }
            }
        }

        public async Task<Stream> DownloadFileAsStream(string remoteFilePath)
        {
            using var sftp = new SftpClient(
                ConstantsApp.SftpHost,
                ConstantsApp.SftpPort,
                ConstantsApp.SftpUsername,
                ConstantsApp.SftpPassword
            );

            sftp.Connect();
            if (!sftp.Exists(remoteFilePath))
                throw new FileNotFoundException("Không tìm thấy file trên SFTP server.");

            var memStream = new MemoryStream();
            await Task.Run(() => sftp.DownloadFile(remoteFilePath, memStream));
            memStream.Position = 0;

            sftp.Disconnect();
            return memStream;
        }
    }
}
