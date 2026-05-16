using System;
using System.Collections.Generic;

namespace Ttlaixe.Models;

public partial class LichSuSoDu
{
    public int Nam { get; set; }

    public string MaTaiKhoan { get; set; }

    public string TenTaiKhoan { get; set; }

    public decimal No { get; set; }

    public decimal Co { get; set; }

    public bool? IsActive { get; set; }
}
