namespace Ttlaixe.DTO.request
{
    public class NguoiLxhsCreateRequest
    {
        public int MaGt { get; set; }      // Mã giấy tờ (DM_GiayTo.MaGt)
        public string TenGt { get; set; }  // Tên giấy tờ (có thể để null nếu muốn join DM_GiayTo khi hiển thị)
    }
}
