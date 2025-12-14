using System;
using System.Collections.Generic;

namespace Ttlaixe.Models;

public partial class UserTkn
{
    public string UserName { get; set; }

    public string PasswordHash { get; set; }

    public string HoTen { get; set; }

    public string GioiTinh { get; set; }

    public string SoDienThoai { get; set; }

    public bool QuyenAdmin { get; set; }

    public bool QuyenKeToan { get; set; }

    public bool QuyenNhapLieu { get; set; }

    public bool QuyenThayGiao { get; set; }

    public DateTime NgayKhoiTao { get; set; }

    public DateTime? NgayChinhSuaCuoiCung { get; set; }

    public string MaNguoiNhap { get; set; }

    public string TenNguoiNhap { get; set; }

    public string MaNguoiChinhSua { get; set; }

    public string TenNguoiChinhSua { get; set; }
}
