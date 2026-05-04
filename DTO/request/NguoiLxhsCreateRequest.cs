using System.Text.Json.Serialization;

namespace Ttlaixe.DTO.request
{
    public class NguoiLxhsCreateRequest
    {
        [JsonPropertyName("ma_gt")]
        public int MaGt { get; set; }      // Mã giấy tờ (DM_GiayTo.MaGt)
        [JsonPropertyName("ten_gt")]
        public string TenGt { get; set; }  // Tên giấy tờ (có thể để null nếu muốn join DM_GiayTo khi hiển thị)
    }
}
