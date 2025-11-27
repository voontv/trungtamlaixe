using System.Collections.Generic;

namespace Ttlaixe.LibsStartup
{
    public static class Constants
    {
        public const string P00_XN_hs_moi = "0"; //"00- Hồ sơ mới --> Chờ XN bàn giao..."
        public const string P01_XN_giao_NT = "1"; //'01- XN đã giao HS cho Nhóm trưởng --> Chờ xác nhận...
        public const string P02_NT_nhan_XN = "2";  //'02- Nhóm trưởng nhận HS --> Đang giao Công nhân...
        public const string P03_NT_giao_CN = "3"; // '03- Nhóm trưởng giao HS --- Chờ xác nhận...
        public const string P04_CN_nhan_NT = "4";  //'04- Công nhân nhận HS --> Đang thi công...
        public const string P05_CN_thi_cong_OK = "5";  //'05- Công nhân thi công hoàn thành
        public const string P06_CN_giao_NT = "6";  //'06- Công nhân giao HS Nhóm trưởng --> Chờ xác nhận...
        public const string P07_NT_nhan_CN = "7";  //'07 Nhóm trưởng nhận lại HS --> Đang kiểm tra...
        public const string P08_NT_giao_XN = "8";  //'08- Nhóm trưởng giao HS cho XN --> Chờ xác nhận...
        public const string P09_XN_nhan_NT = "9";  //'09- XN đã nhận lại hồ sơ --> Đang kiểm tra...
        public const string P10_XN_nhan_NT_OK = "10";  //'10- XN đã cập nhật Billing --> HOÀN THÀNH
        public const string agentCode = "27";
        public const string user = "web_loc";
        public const string pass = "DwA@WeBsite!88Fi";

        public const string HOSOKHACHHANG = "/DAWACO/HO_SO_KHACH_HANG/";
        public const string HOSONGUOILAODONG = "/DAWACO/NGUOI_LAO_DONG/";
        public const string HOSOTHUETAISAN = "/DAWACO/THUETAISAN/";
        public const string GIAMSATMAYCHU = "/DAWACO/GIAM_SAT_MAY_CHU/";

        public const int MaxLengthCustomerCode = 9;//Chieu dai ma khach hang quy dinh
        public const int ThanhCong = 0;
        public const int AgentCodeKoHopLe = 1;
        public const int PassWrong = 2;
        public const int IpWrong = 3;
        public const int CustomerNotExit = 8;
        public const int CustomerCodeWrong = 9;
        public const int DataNotFound = 17;
        public const int PhoneWrong = 36;
        public const int InvalidToken = 40;

        // Enter your host name or IP here
        public const string host = "200.201.222.88";

        // Enter your host name or IP here
        public const int port = 26;

        // Enter your sftp username here
        public const string username = "itdawaco01";
        // Enter your sftp password here
        public const string password = "Oz'x,F";
        public const string wwwroot = "wwwroot";
        public const int ActionDelete = -1;
        public const int ActionUpdate = 0;
        public const string VnptCompany = "vnpt";

        public static readonly Dictionary<int, string> ErrorCode = new Dictionary<int, string>()
        {
            { ThanhCong, "Thành công-Ok" },
            { AgentCodeKoHopLe,  "AgentCode không hợp lệ" },
            { PassWrong, "Mật khẩu (pass) không chính xác" },
            { IpWrong, "IP không hợp lệ" },
            { CustomerNotExit, "Khách hàng không tồn tại" },
            { CustomerCodeWrong, "Mã khách hàng không hợp lệ (phải 9 ký tự)" },
            { DataNotFound, "Không tìm thấy dữ liệu" },
            { 19, "Định danh (user) không chính xác" },
            { PhoneWrong, "Số điện thoại không đúng" },
            { InvalidToken, "Token không hợp lệ" }
        };
    }
}
