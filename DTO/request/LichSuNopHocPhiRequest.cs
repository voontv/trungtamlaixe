using System;

namespace Ttlaixe.DTO.request
{
    public class LichSuNopHocPhiRequest
    {
        public string MaDk { get; set; }

        public decimal SoTienNop { get; set; }

        public DateTime NgayNop { get; set; }

        public string HinhThucThanhToan { get; set; }

        public string SoBienLai { get; set; }

        public string GhiChu { get; set; }
    }
}
