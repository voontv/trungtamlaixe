using System;
using System.Collections.Generic;

namespace Ttlaixe.Models;

public partial class DmPhanThiTkn
{
    public byte MaPhanThi { get; set; }

    public string TenPhanThi { get; set; }

    public string HangDaoTao { get; set; }

    public virtual ICollection<DmthiSatHachQuyTacTkn> DmthiSatHachQuyTacTkns { get; set; } = new List<DmthiSatHachQuyTacTkn>();
}
