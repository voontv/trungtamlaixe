using log4net;
using Microsoft.AspNetCore.Hosting;
using System;
using System.Collections.Generic;
using System.IO;
using System.Text;

namespace Ttlaixe.Extensions
{
    public class ImageHelper
    {
        private readonly IWebHostEnvironment _webHostEnvironment;
        private static readonly log4net.ILog log
            = log4net.LogManager.GetLogger(System.Reflection.MethodBase.GetCurrentMethod().DeclaringType);
        public ImageHelper(IWebHostEnvironment webHostEnvironment)
        {
            _webHostEnvironment = webHostEnvironment;
        }

        public string SaveImage(byte[] imageBytes, string fileName, string subFolder)
        {
            try
            {
                var timeNow = DateTime.Now.ToString("MMyyyy");
                var pathSaveImage = "images/" + timeNow + "/" + subFolder;
                // Ensure wwwroot/images directory exists
                string imagesPath = Path.Combine(_webHostEnvironment.WebRootPath, pathSaveImage);
                if (!Directory.Exists(imagesPath))
                {
                    Directory.CreateDirectory(imagesPath);
                }

                // Set full file path
                string filePath = Path.Combine(imagesPath, fileName);

                // Write bytes to file
                File.WriteAllBytes(filePath, imageBytes);

                // Return the relative URL
                return $"/{pathSaveImage}/{fileName}";
            }
            catch (Exception ex)
            {
                // Handle exceptions
                log.Info("********************************************** voon debug json send" + ex.Message);
                Console.WriteLine($"Error saving image: {ex.Message}");
                return null;
            }
        }
    }
}
