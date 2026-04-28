namespace Ttlaixe.DTO.response
{
    public class TongHopChungTuDto
    {
        public string MaTaiKhoan { get; set; } = string.Empty;
        public string? TenTaiKhoan { get; set; }
        public decimal TongNo { get; set; }
        public decimal TongCo { get; set; }
        public decimal ChenhLech => TongNo - TongCo;
    }
}
