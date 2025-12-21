using System;

namespace Ttlaixe.DTO.request
{
    public class LichHocCreatedRequest
    {
        public string MaKh { get; set; }

        public int Thang { get; set; }

        public int Tuan { get; set; }

        public DateTime TuNgay { get; set; }

        public DateTime DenNgay { get; set; }

        /// <summary>
        /// LT=Lý thuyết; TH=Thực hành hình; TD=Thực hành đường; KT=Thi kiểm tra; NG=Nghỉ; DU=Dự phòng; SH=Sát hạch
        /// </summary>
        public string GiaiDoan { get; set; }

        public string KiemTra { get; set; }

        public string GhiChu { get; set; }

    }
}
