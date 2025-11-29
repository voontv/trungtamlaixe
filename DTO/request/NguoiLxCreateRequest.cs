using System;
using System.Collections.Generic;

namespace Ttlaixe.DTO.request
{
    public class NguoiLxCreateRequest
    {
        public string HoDemNlx { get; set; }
        public string TenNlx { get; set; }

        public string MaQuocTich { get; set; }      // nếu null sẽ default = "VNM"
        public string NgaySinh { get; set; }        // "YYYYMMDD"

        public string NoiTt { get; set; }
        public string NoiTtMaDvhc { get; set; }
        public string NoiTtMaDvql { get; set; }

        public string NoiCt { get; set; }
        public string NoiCtMaDvhc { get; set; }
        public string NoiCtMaDvql { get; set; }

        public string SoCmt { get; set; }
        public DateTime? NgayCapCmt { get; set; }
        public string NoiCapCmt { get; set; }

        public string GhiChu { get; set; }

        /// <summary>M = Nam, F = Nữ, U = Không xác định</summary>
        public string GioiTinh { get; set; }

        public string SoCmndCu { get; set; }

        // ====== Phần dùng cho hồ sơ ======

        /// <summary>Loại hồ sơ (DM_LoaiHS), ví dụ: 3 = đào tạo mới</summary>
        public int? MaLoaiHs { get; set; }

        /// <summary>Hạng GPLX đề nghị cấp (DM_HangGPLX), vd: B1, C…</summary>
        public string HangGplx { get; set; }

        /// <summary>Hạng đào tạo (DM_HangDT), vd: B, C…</summary>
        public string HangDaoTao { get; set; }

        /// <summary>Mã khóa học tham chiếu KhoaHoc.MaKh</summary>
        public string MaKhoaHoc { get; set; }

        /// <summary>Nơi học lái xe trước đây (nếu có)</summary>
        public string DonViHocLx { get; set; }

        /// <summary>Năm học lái xe (YYYY, nếu có)</summary>
        public int? NamHocLx { get; set; }

        // ====== Danh sách giấy tờ kèm theo hồ sơ ======
        public List<NguoiLxhsCreateRequest> GiayTos { get; set; } = new List<NguoiLxhsCreateRequest>();
    }

}
