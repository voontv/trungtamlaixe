
using Newtonsoft.Json;
using Ttlaixe.AutoConfig;
using Ttlaixe.Models;
using System;
using System.Collections.Generic;
using System.IO;
using System.Linq;
using System.Net;
using System.Net.Http.Headers;
using System.Net.Http;
using System.Net.Mail;
using System.Security.Cryptography;
using System.Text;
using System.Text.Json.Serialization;
using System.Threading.Tasks;
using System.Xml;
using System.Xml.Linq;
using System.Globalization;
using System.Drawing;

namespace Ttlaixe.LibsStartup
{
    public static class Utils
    {
        private static readonly log4net.ILog log
            = log4net.LogManager.GetLogger(System.Reflection.MethodBase.GetCurrentMethod().DeclaringType);
        public static readonly string mailServer = "trungtamcntt@dawaco.com.vn";
        public static readonly string passMailServer = "Hosting@57";
        public static readonly string hostMail = "mail.dawaco.com.vn";
        public static string MD5Hash(string pass)
        {
            var md5 = pass;
            using (var md5Hash = MD5.Create())
            {
                // Byte array representation of source string
                var sourceBytes = Encoding.UTF8.GetBytes(pass);

                // Generate hash value(Byte Array) for input data
                var hashBytes = md5Hash.ComputeHash(sourceBytes);

                // Convert hash byte array to string
                var hash = BitConverter.ToString(hashBytes).Replace("-", string.Empty);

                // Output the MD5 hash
                md5 = hash;
            }

            return md5;
        }

        public class PropertyCopier<TParent, TChild> where TParent : class
                                            where TChild : class
        {
            public static void Copy(TParent parent, TChild child)
            {
                var parentProperties = parent.GetType().GetProperties();
                var childProperties = child.GetType().GetProperties();

                foreach (var parentProperty in parentProperties)
                {
                    foreach (var childProperty in childProperties)
                    {
                        if (parentProperty.Name == childProperty.Name && parentProperty.PropertyType == childProperty.PropertyType)
                        {
                            childProperty.SetValue(child, parentProperty.GetValue(parent));
                            break;
                        }
                    }
                }
            }
        }

        public static DateTime firstDayInMonth()
        {
            var curentDate = DateTime.Now;
            var firstDate = new DateTime(curentDate.Year, curentDate.Month, 1);

            return firstDate;
        }

        public static DateTime firstDayInLastMonth()
        {
            var today = DateTime.Today;
            var fistDay = new DateTime(today.Year, lastMonth().Month, 1);

            return fistDay;
        }

        public static DateTime lastMonth()
        {
            var today = DateTime.Today;
            var lastMonth = new DateTime(today.Year, today.Month, 1).AddMonths(-1);

            return lastMonth;
        }

        public static DateTime lastDayInLastMonth()
        {
            var lastDay = firstDayInMonth().AddDays(-1);

            return lastDay;
        }
        public static string ConvertXmlToJson(string response_xml)
        {

            string json = string.Empty;

            if (!string.IsNullOrEmpty(response_xml))
            {
                var doc = XElement.Parse(response_xml);
                var node_cdata = doc.DescendantNodes().OfType<XCData>().ToList();

                foreach (var node in node_cdata)
                {
                    node.Parent.Add(node.Value);
                    node.Remove();
                }

                json = JsonConvert.SerializeXNode(doc, Newtonsoft.Json.Formatting.None, false).ToString();
            }
            json = json.Replace(@"{""Data"":{""Item"":", "");
            if (json.Length >= 2)
            {
                json = json.Substring(0, json.Length - 2);
            }
            return json;
        }
    }
    
}
