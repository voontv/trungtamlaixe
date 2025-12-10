namespace Ttlaixe.DTO.request
{
    namespace Ttlaixe.DTO.request
    {
        public class ThiSatHachKetQuaChiTietTknRequest
        {
            public string MaKySh { get; set; }      // Mã kỳ sát hạch
            public string MaDk { get; set; }        // Mã đăng ký
            public byte MaPhanThi { get; set; }     // 1..8
            public int IdQuyTac { get; set; }       // FK -> DMThiSatHach_QuyTacTkn
            public int SoLanPham { get; set; }      // Số lần phạm lỗi
        }
    }

}
