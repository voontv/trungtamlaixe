using Microsoft.AspNetCore.Http;
using System.Collections.Generic;
using System.Threading.Tasks;
using System.IO;

namespace Ttlaixe.FileManager
{
    public interface ISFTPFileService
    {
        Task<SFTPFileService.UploadResult> UploadFile(IFormFile file, string remoteFolder);
        Task<List<SFTPFileService.UploadResult>> UploadMultipleFiles(List<IFormFile> files, string remoteFolder);
        Task<Stream> DownloadFileAsStream(string remotePath);    
    }
}
