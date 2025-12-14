namespace Ttlaixe.DTO.request
{
    public class UserTknCreateRequest
    {
        public string UserName { get; set; }
        public string Pass { get; set; }

        public string HoTen { get; set; }
        public string GioiTinh { get; set; } // "M"/"F"
        public string SoDienThoai { get; set; } // 10 số

        public bool QuyenAdmin { get; set; }
        public bool QuyenKeToan { get; set; }
        public bool QuyenNhapLieu { get; set; }
        public bool QuyenThayGiao { get; set; }
    }
}
