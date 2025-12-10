using System;
using System.Collections.Generic;

namespace Ttlaixe.Models;

public partial class ThiSatHachKetQuaPhanThiTkn
{
    public string MaKySh { get; set; }

    public string MaDk { get; set; }

    public byte MaPhanThi { get; set; }

    public string HangDaoTao { get; set; }

    public string MaNguoiCham { get; set; }

    public int DiemToiDa { get; set; }

    public int TongDiemTru { get; set; }

    public int DiemConLai { get; set; }

    public bool KetQua { get; set; }

    public string GhiChu { get; set; }

    public virtual DmPhanThiTkn MaPhanThiNavigation { get; set; }
}
