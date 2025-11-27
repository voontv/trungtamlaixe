using ImageMagick;
using IronXL;
using Microsoft.AspNetCore.Http;
using Ttlaixe.Exceptions;
using Renci.SshNet;
using Renci.SshNet.Common;
using System;
using System.Collections.Generic;
using System.Drawing;
using System.Drawing.Imaging;
using System.IO;
using System.Linq;
using System.Net.Sockets;
using System.Text;
using System.Threading.Tasks;

namespace Ttlaixe.LibsStartup
{
    public static class Files
    {
      
        private static string wwwroot = "wwwroot";
        private static readonly log4net.ILog log
           = log4net.LogManager.GetLogger(System.Reflection.MethodBase.GetCurrentMethod().DeclaringType);
        public static async Task<bool> SendListFiles(List<IFormFile> iFromFiles)
        {

            foreach (IFormFile file in iFromFiles)
            {
                var result = await SendFiles(file);
                if (!result)
                {
                    return false;
                }
            }
            return true;
        }
        public static async Task<bool> SendFiles(IFormFile pathFileName,string directoryRoot = Constants.HOSOKHACHHANG)
        {
            return await SendFiles(pathFileName, "", "", directoryRoot);
        }
        public static async Task<bool> SendFiles(IFormFile pathFileName, string maDuAn, string maHopDong, string directoryRoot = Constants.HOSOKHACHHANG)
        {
            if (!UploadToWebRoot(pathFileName,maDuAn, maHopDong))
            {
                return false;
            }

            using SftpClient client = new SftpClient(Constants.host, Constants.port, Constants.username, Constants.password);
            try
            {
                client.Connect();
                if (client.IsConnected)
                {

                    using (var uplfileStream = pathFileName.OpenReadStream())
                    {
                        if (string.IsNullOrEmpty(maDuAn))
                        {
                            if (!client.Exists(directoryRoot + DateTime.Now.Year))
                            {
                                client.CreateDirectory(directoryRoot + DateTime.Now.Year);
                            }
                            client.UploadFile(uplfileStream, directoryRoot + DateTime.Now.Year + "/" + pathFileName.FileName, null);
                        } else
                        {
                            if (!client.Exists(directoryRoot + DateTime.Now.Year))
                            {
                                client.CreateDirectory(directoryRoot + DateTime.Now.Year);

                            }

                            if (!client.Exists(directoryRoot + DateTime.Now.Year + "/" + maDuAn))
                            {
                                client.CreateDirectory(directoryRoot + DateTime.Now.Year + "/" + maDuAn);

                            }

                            if (!client.Exists(directoryRoot + DateTime.Now.Year + "/" + maDuAn + "/" + maHopDong))
                            {
                                client.CreateDirectory(directoryRoot + DateTime.Now.Year + "/" + maDuAn + "/" + maHopDong);
                            }
                            client.UploadFile(uplfileStream, directoryRoot + DateTime.Now.Year + "/" + maDuAn + "/" + maHopDong + "/" + pathFileName.FileName, null);
                        }
                        
                    }
                    client.Disconnect();
                }
            }
            catch (Exception e)
            {
                log.Info("################################## Lỗi tìm thấy khi upload files  " + e.Message);
                throw new BadRequestException($"Error connecting to server: {e.Message}" + " xxxxx  ");
            }
            /*when (e is SshConnectionException || e is SocketException || e is ProxyException)
            {
                throw new BadRequestException($"Error connecting to server: {e.Message}" + " xxxxx  ");
            }
            catch (SshAuthenticationException e)
            {
                throw new BadRequestException($"Failed to authenticate: {e.Message}" + " xxxxx  ");
            }
            catch (SftpPermissionDeniedException e)
            {

                throw new BadRequestException($"Operation denied by the server: {e.Message}");
            }
            catch (SshException e)
            {
                throw new BadRequestException($"Operation denied by the server: {e.Message}");
            }*/

            return true;
        }

        public static async Task<bool> SendFilesGiamSatPMC(IFormFile pathFileName, string directoryRoot = Constants.GIAMSATMAYCHU)
        {
            if (!UploadToWebRootPMC(pathFileName))
            {
                return false;
            }

            using SftpClient client = new SftpClient(Constants.host, Constants.port, Constants.username, Constants.password);
            try
            {
                client.Connect();
                if (client.IsConnected)
                {

                    using (var uplfileStream = pathFileName.OpenReadStream())
                    {
                        if (!client.Exists(directoryRoot + DateTime.Now.Year))
                        {
                            client.CreateDirectory(directoryRoot + DateTime.Now.Year);

                        }


                        client.UploadFile(uplfileStream, directoryRoot + DateTime.Now.Year + "/" + pathFileName.FileName, null);

                    }
                    client.Disconnect();
                }
            }
            catch (Exception e)
            {
                log.Info("################################## Lỗi tìm thấy khi upload files  " + e.Message);
                throw new BadRequestException($"Error connecting to server: {e.Message}" + " xxxxx  ");
            }
            /*when (e is SshConnectionException || e is SocketException || e is ProxyException)
            {
                throw new BadRequestException($"Error connecting to server: {e.Message}" + " xxxxx  ");
            }
            catch (SshAuthenticationException e)
            {
                throw new BadRequestException($"Failed to authenticate: {e.Message}" + " xxxxx  ");
            }
            catch (SftpPermissionDeniedException e)
            {

                throw new BadRequestException($"Operation denied by the server: {e.Message}");
            }
            catch (SshException e)
            {
                throw new BadRequestException($"Operation denied by the server: {e.Message}");
            }*/

            return true;
        }

        public static bool CheckFilesGiamSatPMC(string fileName, string directoryRoot = Constants.GIAMSATMAYCHU)
        {

            using SftpClient client = new SftpClient(Constants.host, Constants.port, Constants.username, Constants.password);

            try
            {
                client.Connect();
                if (client.IsConnected)
                {

                    bool followLinks = true;
                    if (!client.Exists(directoryRoot + DateTime.Now.Year))
                    {
                        client.CreateDirectory(directoryRoot + DateTime.Now.Year);
                    }
                    var status = client.Exists(directoryRoot + DateTime.Now.Year + "/" + fileName);
                    client.Disconnect();
                    return status;
                }
            }
            catch (Exception e) when (e is SshConnectionException || e is SocketException || e is ProxyException)
            {
                throw new BadRequestException($"Error connecting to server: {e.Message}" + " xxxxx  ");
            }
            catch (SshAuthenticationException e)
            {
                throw new BadRequestException($"Failed to authenticate: {e.Message}" + " xxxxx  ");
            }
            catch (SftpPermissionDeniedException e)
            {

                throw new BadRequestException($"Operation denied by the server: {e.Message}");
            }
            catch (SshException e)
            {
                throw new BadRequestException($"Operation denied by the server: {e.Message}");
            }

            return false;
        }

        public static Stream GetStreamWithGetBytes(string sampleString, Encoding? encoding = null)
        {
            encoding ??= Encoding.UTF8;
            var byteArray = encoding.GetBytes(sampleString);
            var memoryStream = new MemoryStream(byteArray);
            return memoryStream;
        }

        public static async Task<bool> SendFiles(string stringBase64, string fileName, string directoryRoot = Constants.HOSOKHACHHANG)
        {

            using SftpClient client = new SftpClient(Constants.host, Constants.port, Constants.username, Constants.password);
            var image = LoadImage(stringBase64);

            try
            {
                client.Connect();
                if (client.IsConnected)
                {

                    using (var uplfileStream = image.ToStream(ImageFormat.Png))
                    {
                        if (!client.Exists(directoryRoot + DateTime.Now.Year))
                        {
                            client.CreateDirectory(directoryRoot + DateTime.Now.Year);
                        }
                        client.UploadFile(uplfileStream, directoryRoot + DateTime.Now.Year + "/" + fileName, null);
                    }
                    client.Disconnect();
                }
            }
            catch (Exception e)
            {
                log.Info("################################## Lỗi tìm thấy khi upload files  " + e.Message);
                throw new BadRequestException($"Error connecting to server: {e.Message}" + " xxxxx  ");
            }
            /*when (e is SshConnectionException || e is SocketException || e is ProxyException)
            {
                log.Info("################################## SendMessageQRcodeNotifycation json: " + e.Message);
                throw new BadRequestException($"Error connecting to server: {e.Message}" + " xxxxx  ");
            }
            catch (SshAuthenticationException e)
            {
                log.Info("################################## SendMessageQRcodeNotifycation json: " + e.Message);
                throw new BadRequestException($"Failed to authenticate: {e.Message}" + " xxxxx  ");
            }
            catch (SftpPermissionDeniedException e)
            {
                log.Info("################################## SendMessageQRcodeNotifycation json: " + e.Message);
                throw new BadRequestException($"Operation denied by the server: {e.Message}");
            }
            catch (SshException e)
            {
                log.Info("################################## SendMessageQRcodeNotifycation json: " + e.Message);
                throw new BadRequestException($"Operation denied by the server: {e.Message}");
            }*/

            return true;
        }


        public static bool CheckFiles(string fileName, string directoryRoot = Constants.HOSOKHACHHANG)
        {

            using SftpClient client = new SftpClient(Constants.host, Constants.port, Constants.username, Constants.password);

            try
            {
                client.Connect();
                if (client.IsConnected)
                {

                    bool followLinks = true;
                    if (!client.Exists(directoryRoot + DateTime.Now.Year))
                    {
                        client.CreateDirectory(directoryRoot + DateTime.Now.Year);
                    }
                    var status = client.Exists(directoryRoot + DateTime.Now.Year + "/" + fileName);
                    client.Disconnect();
                    return status;
                }
            }
            catch (Exception e) when (e is SshConnectionException || e is SocketException || e is ProxyException)
            {
                throw new BadRequestException($"Error connecting to server: {e.Message}" + " xxxxx  ");
            }
            catch (SshAuthenticationException e)
            {
                throw new BadRequestException($"Failed to authenticate: {e.Message}" + " xxxxx  ");
            }
            catch (SftpPermissionDeniedException e)
            {

                throw new BadRequestException($"Operation denied by the server: {e.Message}");
            }
            catch (SshException e)
            {
                throw new BadRequestException($"Operation denied by the server: {e.Message}");
            }

            return false;
        }


        public static string convertFilesSftpToBase64(string nameFile, string directoryRoot = Constants.HOSOKHACHHANG)
        {
            string base64 = null;
            DateTime start = new DateTime(1970, 1, 1, 0, 0, 0, DateTimeKind.Utc);
            long longDate;
            try
            {
                var cutTypeImage = nameFile.Split('.')[0];

                longDate = long.Parse(cutTypeImage.Split('_')[2]);
            }
            catch (Exception e)
            {
                return e.ToString();
            }
            DateTime dateTime = start.AddMilliseconds(longDate).ToLocalTime();
            using SftpClient client = new SftpClient(Constants.host, Constants.port, Constants.username, Constants.password);

            try
            {
                client.Connect();
                var test = directoryRoot + dateTime.Year + "/" + nameFile;

                if (client.IsConnected)
                {
                    byte[] buffer = client.ReadAllBytes(directoryRoot + dateTime.Year + "/" + nameFile);

                    var ext = Path.GetExtension(nameFile).ToLower();

                    if (ext == ".heic")
                    {
                        using var image = new MagickImage(buffer); // đọc HEIC
                        image.Format = MagickFormat.Jpeg; // convert sang JPG
                        base64 = Convert.ToBase64String(image.ToByteArray());
                    }
                    else
                    {
                        base64 = Convert.ToBase64String(buffer); // giữ nguyên JPG, PNG...
                    }

                    client.Disconnect();
                }

            }
            catch (Exception e) when (e is SshConnectionException || e is SocketException || e is ProxyException)
            {
                return $"Error connecting to server: {e.Message}" + " xxxxx  ";
            }
            catch (SshAuthenticationException e)
            {
                return $"Failed to authenticate: {e.Message}" + " xxxxx  ";
            }
            catch (SftpPermissionDeniedException e)
            {

                return $"Operation denied by the server: {e.Message}";
            }
            catch (SshException e)
            {
                return $"Operation denied by the server: {e.Message}";
            }

            return base64;
        }

        public static Stream ToStream(this Image image, ImageFormat format)
        {
            var stream = new MemoryStream();
            image.Save(stream, format);
            stream.Position = 0;
            return stream;
        }

        public static Image LoadImage(string base64)
        {
            //data:image/gif;base64,
            //this image is a single pixel (black)
            byte[] bytes = Convert.FromBase64String(base64);

            Image image;
            using (MemoryStream ms = new MemoryStream(bytes))
            {
                image = Image.FromStream(ms);
            }

            return image;
        }

        public static bool UploadToWebRoot(IFormFile file,string maDuAn, string maHopDong)
        {

            string path = Path.Combine(Environment.CurrentDirectory + "/" + wwwroot + "/" + DateTime.Now.Year, "");

            if (!string.IsNullOrEmpty(maDuAn))
            {
                path = Path.Combine(Environment.CurrentDirectory + "/" + wwwroot + "/" + DateTime.Now.Year, maDuAn);

            }

            if (!string.IsNullOrEmpty(maHopDong))
            {
                path = Path.Combine(Environment.CurrentDirectory + "/" + wwwroot + "/" + DateTime.Now.Year, maDuAn+"/"+maHopDong);

            }

            if (!Directory.Exists(path))
            {
                Directory.CreateDirectory(path);
            }

            List<string> uploadedFiles = new List<string>();

            string fileName = Path.GetFileName(file.FileName);
            try
            {
                using (FileStream stream = new FileStream(Path.Combine(path, fileName), FileMode.Create))
                {
                    file.CopyTo(stream);
                    uploadedFiles.Add(fileName);
                }
            }
            catch (Exception e)
            {
                log.Info("################################## Lỗi tìm thấy khi upload files root folder  " + e.Message);
                throw new BadRequestException($"Error connecting to server: {e.Message}" + " xxxxx  ");
            }

            return true;
        }

        public static bool UploadToWebRootPMC(IFormFile file)
        {

            string path = Path.Combine(Environment.CurrentDirectory + "/" + wwwroot + "/" + "upload" + "/" + "GIAM_SAT_MAY_CHU" + "/" + DateTime.Now.Year, "");

            if (!Directory.Exists(path))
            {
                Directory.CreateDirectory(path);
            }

            List<string> uploadedFiles = new List<string>();

            string fileName = Path.GetFileName(file.FileName);
            try
            {
                using (FileStream stream = new FileStream(Path.Combine(path, fileName), FileMode.Create))
                {
                    file.CopyTo(stream);
                    uploadedFiles.Add(fileName);
                }
            }
            catch (Exception e)
            {
                log.Info("################################## Lỗi tìm thấy khi upload files root folder PMC " + e.Message);
                throw new BadRequestException($"Error connecting to server: {e.Message}" + " xxxxx  ");
            }

            return true;
        }

        public static async Task<List<ChuanHoa>> getExcelChuanHoaDuong(IFormFile file)
        {
            
            string path = Path.Combine(Environment.CurrentDirectory + "/" + Constants.wwwroot + "/" + "uploads/");
            if (!Directory.Exists(path))
            {
                Directory.CreateDirectory(path);
            }

            string fileName = Path.GetFileName(file.FileName);
            try
            {
                using (FileStream stream = new FileStream(Path.Combine(path, fileName), FileMode.Create))
                {
                    file.CopyTo(stream);
                }
            }
            catch (Exception e)
            {
                throw new BadRequestException("Error found " + e);
            }


            var cotCanLay = 2;
            var CotDongBatDau = "A2";
            var CotDongketThuc = "B300";
            var nhanSus = new List<ChuanHoa>();
            //StreamWriter sw = new StreamWriter(path+"result.txt");
            WorkBook workBook = WorkBook.Load(path + fileName);
            WorkSheet workSheet = workBook.WorkSheets.First();
            // Select cells easily in Excel notation and return the calculated value
            int cellValue = workSheet[CotDongBatDau].IntValue;
            // Read from Ranges of cells elegantly.
            int count = 0;
            string MaDuong = "";
            string MaPhuong = "";
            DateTime NgayCapCchn = new DateTime();
            foreach (var cell in workSheet[CotDongBatDau + ":" + CotDongketThuc])
            {
                var dataLine = new ChuanHoa();
                if (count % cotCanLay == 0)
                {
                    if (String.IsNullOrEmpty(cell.Text))
                    {
                        break;
                    }
                    MaDuong = cell.Text;
                }
                else if (count % cotCanLay == cotCanLay - 1)
                {
                    
                    dataLine.MaDuong = MaDuong;
                    dataLine.MaPhuong = cell.Text;
                    nhanSus.Add(dataLine);
                }
                count++;
            }
            //sw.Close();
            return nhanSus;

        }

    }

    public class ChuanHoa
    {
        public string MaDuong { get; set; }

        public string MaPhuong { get; set; }
    }
}
