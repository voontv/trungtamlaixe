using System;

namespace Ttlaixe.DTO.response
{
    public class NopTienSearchResponse
    {
        public int IdNopTien { get; set; }
        public string MaDk { get; set; }
        public decimal SoTienNop { get; set; }
        public DateTime NgayNop { get; set; }
        public string HinhThucThanhToan { get; set; }
        public string SoBienLai { get; set; }
        public string GhiChu { get; set; }

        public string HoVaTen { get; set; }
        public string NgaySinh { get; set; }
        public string SoCmt { get; set; }
    }
}
