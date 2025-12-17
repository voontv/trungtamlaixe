using Microsoft.AspNetCore.Http;
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
        public string SoCmt { get; set; }
        public DateTime? NgayCapCmt { get; set; }
        public string NoiCapCmt { get; set; }

        public string GhiChu { get; set; }

        /// <summary>M = Nam, F = Nữ, U = Không xác định</summary>
        public string GioiTinh { get; set; }

        public string SoCmndCu { get; set; }


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
        /// Mã Xã/Phường/Thị Trấn/Quận/Huyện/Thị xã/Thành phố. Ghi MaDvhc trong bảng DM_DVHC
        /// </summary>
        public string NoiCtMaDvhc { get; set; }

        /// <summary>
        /// Mã Quận/Huyện/Thị xã/Thành phố Nơi cư trú. Ghi MaDVQL trong bảng DM_DVHC
        /// </summary>
        public string NoiCtMaDvql { get; set; }

        /// <summary>
        /// Số năm lái xe
        /// </summary>
        public int? SoNamLx { get; set; }

        /// <summary>
        /// Số Km lái xe an toàn
        /// </summary>
        public int? SoKmLxanToan { get; set; }

        /// <summary>
        /// Số GPLX hiện có của người lái xe.
        /// </summary>
        public string SoGplxdaCo { get; set; }

        /// <summary>
        /// Hạng GPLX hiện có của Người lái xe
        /// </summary>
        public string HangGplxdaCo { get; set; }

        /// <summary>
        /// Đơn vị cấp GPLX hiện có của Người lái xe
        /// </summary>
        public string DonViCapGplxdaCo { get; set; }

        public string NoiCapGplxdaCo { get; set; }

        /// <summary>
        /// Ngày cấp giấy phép lái xe hiện có của người lái xe
        /// </summary>
        public string NgayCapGplxdaCo { get; set; }

        /// <summary>
        /// Ngày hết hạn GPLX hiện có của NGười lái xe
        /// </summary>
        public string NgayHhgplxdaCo { get; set; }

        public string NgayTtgplxdaCo { get; set; }

        /// <summary>
        /// Nơi học lái xe trước đây. Ghi mã CSĐT trong bảng DM_DonViGTVT
        /// </summary>
        public string DonViHocLx { get; set; }


        public List<NguoiLxhsCreateRequest> GiayTos { get; set; } = new List<NguoiLxhsCreateRequest>();

        public IFormFile? File { get; set; }
    }

}
