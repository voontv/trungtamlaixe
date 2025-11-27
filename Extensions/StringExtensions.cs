using System.Collections.Generic;

namespace Ttlaixe.Extensions
{
    public static class StringExtensions
    {
        public static string DawacoAswerRegister(this string str, string idKhachHang)
        {
            return str.Replace("{DawacoCustomerId}", idKhachHang);
        }

        public static string FormatDanhSachKhachHangDaDangKy(List<string> strs)
        {
            string result = "";
            for (int i = 1; i <= strs.Count; i++)
            {
                result += "Mã " + i + ": " + strs[i];
                if (i < strs.Count)
                    result += "\n";
            }
            return result;
        }

    }
}
