using System;
using System.IO;

namespace Ttlaixe.Constants
{
    public static class ConstantsApp
    {
        // Cấu hình SFTP
        public const string SftpHost = "200.201.222.88";
        public const int SftpPort = 26;
        public const string SftpUsername = "itdawaco01";
        public const string SftpPassword = "Oz'x,F";

        // Đường dẫn lưu trữ chính
        public const string RemoteRoot = "/DAWACO/D_OFFICE/QUANLICONGVAN";
        public static string CONGVAN = "/CONGVAN/";
        public static string AVATAR =   "/AVATAR/";
        public static string ESIGNATURE = "/ESIGNATURE/";
        // Cấu hình thư mục tạm (local)
        public static string WebRootPath = "wwwroot";
        public static string BaseUrl = "https://quan1125.dawaco.com.vn";
        // public static string WebRootPath = Path.Combine(Directory.GetCurrentDirectory(), "wwwroot");

        // //Thông tin hệ thống
        // public static string SystemName = "Vegan Food eCommerce";
        // public static string DefaultLanguage = "vi-VN";

        //Cấu hình upload
        public static long MaxUploadSize = 100 * 1024 * 1024; // 100 MB
        public static string[] AllowedImageExtensions = { ".jpg", ".jpeg", ".png" };
        public static string[] AllowedDocumentExtensions = { ".pdf", ".docx", ".xlsx" };

        public static string LogPath = "logs/Ttlaixe.log";

        public static string RabbitMQHost = "localhost";
        public static int RabbitMQPort = 5672;
        public static string RabbitMQUsername = "dawaco";
        public static string RabbitMQPassword = "Dawaco@57";
    }
}
