using System;
using System.Collections.Generic;

namespace Ttlaixe.DTO.response
{
    public class LichHocsResponse
    {
        public string MaKh { get; set; }
        public List<LichHocCoBan> thongTinChiTiets { get; set; }
    }

    public class LichHocCoBan
    {
        public long MaLichHoc { get; set; }

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
