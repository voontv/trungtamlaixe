using System;
using System.Collections.Generic;

namespace Ttlaixe.Models;

public partial class HocVienChuaPhanKhoa
{
    public int IdHs { get; set; }

    public string HoDemNlx { get; set; }

    public string TenNlx { get; set; }

    public string MaQuocTich { get; set; }

    public DateTime NgaySinh { get; set; }

    public string SoCmt { get; set; }

    public string HangDaoTao { get; set; }

    public string SoDienThoai { get; set; }

    public decimal SoTienNop { get; set; }

    public string MaGv { get; set; }

    public string GhiChu { get; set; }

    public DateTime NgayNopHoSo { get; set; }

    public string BangA1 { get; set; }

    public bool? CamKet { get; set; }

    public bool? AnhThe { get; set; }

    public bool? Don { get; set; }

    public bool? HopDong { get; set; }

    public bool? DonSatHach { get; set; }

    public bool? Gksk { get; set; }

    public bool? VanTayKhuonMat { get; set; }

    public bool? ChupAnh { get; set; }

    public string GioiTinh { get; set; }

    /// <summary>
    /// 0 = khong hieu luc; 1 = co hieu luc; mac dinh la 1;
    /// </summary>
    public bool? TrangThai { get; set; }

    public string MaDk { get; set; }

    public string DuongDanAnh { get; set; }

    public string MaKhoaHoc { get; set; }

    public string TenKhoaNoiBo { get; set; }

    public string NguonGioiThieu { get; set; }

    public DateTime NgayCapCmt { get; set; }

    public string NoiCapCmt { get; set; }

    public string SoGplxdaCo { get; set; }

    public string HangGplxdaCo { get; set; }

    public string DonViCapGplxdaCo { get; set; }

    public string NoiCapGplxdaCo { get; set; }

    public string NgayCapGplxdaCo { get; set; }

    public string NgayHhgplxdaCo { get; set; }

    public string NgayTtgplxdaCo { get; set; }

    public string DonViHocLx { get; set; }

    public string HangGplx { get; set; }

    public int? SoNamLx { get; set; }

    public int? SoKmLxanToan { get; set; }

    public string GiayTos { get; set; }

    public string NguoiTao { get; set; }

    public string NguoiSua { get; set; }

    public DateTime? NgayTao { get; set; }

    public DateTime? NgaySua { get; set; }

    public string DonViNhanHso { get; set; }

    public string NoiTt { get; set; }

    public string NoiTtMaDvhc { get; set; }

    public string NoiCt { get; set; }

    public string NoiCtMaDvhc { get; set; }

    public string SoCmndCu { get; set; }
}
