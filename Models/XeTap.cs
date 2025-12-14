using System;
using System.Collections.Generic;

namespace Ttlaixe.Models;

public partial class XeTap
{
    /// <summary>
    /// Biển số xe. Là khóa chính của bảng XeTap
    /// </summary>
    public string BienSoXe { get; set; }

    /// <summary>
    /// Mã Sở GTVT. Tham chiếu đến DM_DonViGTVT
    /// </summary>
    public string MaSoGtvt { get; set; }

    /// <summary>
    /// Mã CSĐT. Tham chiếu đến bảng DM_DonViGTVT
    /// </summary>
    public string MaCsdt { get; set; }

    /// <summary>
    /// Số giấy đăng ký xe
    /// </summary>
    public string SoDk { get; set; }

    /// <summary>
    /// Xe hợp đồng hay xe của CSĐT. 0=&apos;Chính chủ&apos;; 1=&apos;Hợp đồng&apos;. Mặc định = 0
    /// </summary>
    public bool SoHuu { get; set; }

    public string NhanHieu { get; set; }

    public string LoaiXe { get; set; }

    public string MacXe { get; set; }

    public string HangXe { get; set; }

    public string MauXe { get; set; }

    public string SoDongCo { get; set; }

    public string SoKhung { get; set; }

    /// <summary>
    /// Giấy phép xe tập lái (Có/Không). 0=Không; 1=Có
    /// </summary>
    public bool? GiayPhepXtl { get; set; }

    /// <summary>
    /// Số giấy phép xe tập lái
    /// </summary>
    public string SoGpxtl { get; set; }

    /// <summary>
    /// Cơ quan cấp Giấy phép xe tập lái
    /// </summary>
    public string CoQuanCapGpxtl { get; set; }

    /// <summary>
    /// Ngày, tháng, năm cấp giấy phép xe tập lái
    /// </summary>
    public DateTime? NgayCapGpxtl { get; set; }

    /// <summary>
    /// Ngày hết hạn Giấy phép xe tập lái
    /// </summary>
    public DateTime? NgayHhgpxtl { get; set; }

    /// <summary>
    /// Năm sản xuất xe
    /// </summary>
    public int? NamSx { get; set; }

    /// <summary>
    /// Hệ thống phanh phụ (Có/Không). 1=Có; 0=Không.
    /// </summary>
    public bool? HeThongPp { get; set; }

    /// <summary>
    /// Ngày cấp Giấy chứng nhận kiểm định ATKT &amp; BVMT
    /// </summary>
    public DateTime? NgayCapGcnkd { get; set; }

    /// <summary>
    /// Ngày hết hạn Giấy chứng nhận kiểm định ATKT &amp; BVMT
    /// </summary>
    public DateTime? NgayHhgcnkd { get; set; }

    /// <summary>
    /// Bảo hiểm trách nhiệm dân sự còn hiệu lực. 0=Không còn hiệu lực; 1=Còn hiệu lực
    /// </summary>
    public bool? BaoHiem { get; set; }

    /// <summary>
    /// Tuyến đường tập lái
    /// </summary>
    public string TuyenDuong { get; set; }

    /// <summary>
    /// Chất lượng
    /// </summary>
    public string ChatLuong { get; set; }

    /// <summary>
    /// Ghi chú
    /// </summary>
    public string GhiChu { get; set; }

    /// <summary>
    /// 0 = khong hieu luc; 1 = co hieu luc; mac dinh la 1;
    /// </summary>
    public bool? TrangThai { get; set; }

    public string NguoiTao { get; set; }

    public string NguoiSua { get; set; }

    public DateTime NgayTao { get; set; }

    public DateTime NgaySua { get; set; }

    public string DuongDanAnh { get; set; }
}
