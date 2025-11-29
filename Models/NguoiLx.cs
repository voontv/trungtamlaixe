using System;
using System.Collections.Generic;

namespace Ttlaixe.Models;

public partial class NguoiLx
{
    /// <summary>
    /// Mã đăng ký = &lt;Mã VPĐK&gt;-&lt;YYYYMMDD&gt;-&lt;HH24MISSMS&gt;
    /// </summary>
    public string MaDk { get; set; }

    public string DonViNhanHso { get; set; }

    /// <summary>
    /// Họ và tên đệm của Người lái xe
    /// </summary>
    public string HoDemNlx { get; set; }

    /// <summary>
    /// Tên của người lái xe
    /// </summary>
    public string TenNlx { get; set; }

    /// <summary>
    /// Họ và tên của Người lái xe
    /// </summary>
    public string HoVaTen { get; set; }

    /// <summary>
    /// Mã quốc gia theo bảng mã quốc tế ISO 3166-1 alpha-3. Ghi mã Quốc gia trong bảng DM_QuocTich
    /// </summary>
    public string MaQuocTich { get; set; }

    /// <summary>
    /// Ngày sinh, được ghi theo định dạng: YYYYMMDD.
    /// </summary>
    public string NgaySinh { get; set; }

    /// <summary>
    /// Nơi đăng ký hộ khẩu thường trú. Ghi chi tiết: Số nhà/Đường phố/Thôn xóm
    /// </summary>
    public string NoiTt { get; set; }

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

    /// <summary>
    /// Số CMT/Hộ chiếu
    /// </summary>
    public string SoCmt { get; set; }

    /// <summary>
    /// Ngày cấp CMT/Hộ chiếu
    /// </summary>
    public DateTime? NgayCapCmt { get; set; }

    /// <summary>
    /// Nơi cấp CMT/Hộ chiếu
    /// </summary>
    public string NoiCapCmt { get; set; }

    public string GhiChu { get; set; }

    /// <summary>
    /// 0 = khong hieu luc; 1 = co hieu luc; mac dinh la 1;
    /// </summary>
    public bool? TrangThai { get; set; }

    public string NguoiTao { get; set; }

    public string NguoiSua { get; set; }

    public DateTime NgayTao { get; set; }

    public DateTime NgaySua { get; set; }

    /// <summary>
    /// Giới tính. M=Nam; F=Nữ; U=Không xác định
    /// </summary>
    public string GioiTinh { get; set; }

    public string HoVaTenIn { get; set; }

    public string SoCmndCu { get; set; }

    public int? HosoDvcc4 { get; set; }

    public virtual NguoiLxHoSo NguoiLxHoSo { get; set; }

    public virtual DmDvhc NoiCtMaDv { get; set; }

    public virtual DmDvhc NoiTtMaDv { get; set; }
}
