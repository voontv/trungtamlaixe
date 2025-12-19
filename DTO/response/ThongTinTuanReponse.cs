using System;

namespace Ttlaixe.DTO.response
{
    public class ThongTinTuanReponse
    {
        public string TenTuan { get; set; }       // "Tuần 1"...
        public DateTime NgayBatDau { get; set; }  // Thứ 2
        public DateTime NgayKetThuc { get; set; } // Chủ nhật (hoặc 31/12 nếu tuần cuối bị cắt)
        public int Thang { get; set; }            // theo rule 4 tuần/tháng
        public int TuanTrongThang { get; set; }   // 1..4
    }
}
