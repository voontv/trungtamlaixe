using System;
using System.Collections.Generic;

namespace Ttlaixe.Models;

public partial class ThiSatHachKetQuaChiTietTkn
{
    public string MaKySh { get; set; }

    public string MaDk { get; set; }

    public byte MaPhanThi { get; set; }

    public int IdQuyTac { get; set; }

    public int SoLanPham { get; set; }

    public virtual DmthiSatHachQuyTacTkn IdQuyTacNavigation { get; set; }

    public virtual DmPhanThiTkn MaPhanThiNavigation { get; set; }
}
