namespace Ttlaixe.DTO.response
{
    public class UserTknLoginResponse
    {
        public string Token { get; set; }
        public string BearerToken { get; set; }

        public string UserName { get; set; }
        public string HoTen { get; set; }
        public string GioiTinh { get; set; }
        public string SoDienThoai { get; set; }

        public bool QuyenAdmin { get; set; }
        public bool QuyenKeToan { get; set; }
        public bool QuyenNhapLieu { get; set; }
        public bool QuyenThayGiao { get; set; }
    }
}
