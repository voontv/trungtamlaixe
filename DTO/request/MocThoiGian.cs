using System;

namespace Ttlaixe.DTO.request
{
    public class MocThoiGian
    {
        public DateTime? NgayBatDau { get; set; }
        public DateTime? NgayKetThuc { get; set; }
    }

    public class HangDaoTao
    {
        public DateTime? NgayBatDau { get; set; }
        public DateTime? NgayKetThuc { get; set; }
        public string HangDt { get; set; }
    }
}
