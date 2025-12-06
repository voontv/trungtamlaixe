using System;
using System.Collections.Generic;

namespace Ttlaixe.DTO.request
{
    public class NguoiLxCreateRequest
    {
        /// <summary>
        /// Mã Cơ sở đào tạo lái xe. Ghi Mã CSDT trong bảng DM_DonViGTVT
        /// </summary>
        public string MaCsdt { get; set; }
        public string HoDemNlx { get; set; }
        public string TenNlx { get; set; }

        public string MaQuocTich { get; set; }      // nếu null sẽ default = "VNM"
        public string NgaySinh { get; set; }        // "YYYYMMDD"
        public string NoiTt { get; set; }
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

        /// <summary>Năm học lái xe (YYYY, nếu có)</summary>
        public int? NamHocLx { get; set; }

        /// <summary>
        /// Mã đơn vị Xã/Phường/Quận/Huyện của Nơi đăng ký thường trú. Ghi MaDVHC trong bảng DM_DVHC
        /// </summary>
        public string NoiTtMaDvhc { get; set; }

        /// <summary>
        /// Mã Huyện/Quận/Thành phố/Thị xã của Noi đăng ký hộ khẩu thường trú. Ghi MaDVQL trong bảng DM_DVHC
        /// </summary>
        public string NoiTtMaDvql { get; set; }

        /// <summary>
        /// Nơi cư trú. Ghi chi tiết Số nhà/Đường phố/Thôn/Xóm
        /// </summary>
        public string NoiCt { get; set; }

        /// <summary>
        /// Mã Xã/Phường/Thị Trấn/Quận/Huyện/Thị xã/Thành phố. Ghi MaDvhc trong bảng DM_DVHC
        /// </summary>
        public string NoiCtMaDvhc { get; set; }

        /// <summary>
        /// Mã Quận/Huyện/Thị xã/Thành phố Nơi cư trú. Ghi MaDVQL trong bảng DM_DVHC
        /// </summary>
        public string NoiCtMaDvql { get; set; }

        public string DuongDanAnh { get; set; }
        // ====== Danh sách giấy tờ kèm theo hồ sơ ======
        public List<NguoiLxhsCreateRequest> GiayTos { get; set; } = new List<NguoiLxhsCreateRequest>();
    }

}
