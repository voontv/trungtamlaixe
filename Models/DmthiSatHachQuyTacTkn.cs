using System;
using System.Collections.Generic;

namespace Ttlaixe.Models;

public partial class DmthiSatHachQuyTacTkn
{
    public int IdQuyTac { get; set; }

    public byte MaPhanThi { get; set; }

    public string NoiDung { get; set; }

    public int DonViDiemTru { get; set; }

    public bool IsRotNgay { get; set; }

    public virtual DmPhanThiTkn MaPhanThiNavigation { get; set; }

    public virtual ICollection<ThiSatHachKetQuaChiTietTkn> ThiSatHachKetQuaChiTietTkns { get; set; } = new List<ThiSatHachKetQuaChiTietTkn>();
}
