using System;
using System.Collections.Generic;

namespace Ttlaixe.Models;

public partial class DmPhanThiTkn
{
    public byte MaPhanThi { get; set; }

    public string TenPhanThi { get; set; }

    public string HangDaoTao { get; set; }

    public virtual ICollection<DmthiSatHachQuyTacTkn> DmthiSatHachQuyTacTkns { get; set; } = new List<DmthiSatHachQuyTacTkn>();

    public virtual ICollection<ThiSatHachKetQuaChiTietTkn> ThiSatHachKetQuaChiTietTkns { get; set; } = new List<ThiSatHachKetQuaChiTietTkn>();

    public virtual ICollection<ThiSatHachKetQuaPhanThiTkn> ThiSatHachKetQuaPhanThiTkns { get; set; } = new List<ThiSatHachKetQuaPhanThiTkn>();
}
