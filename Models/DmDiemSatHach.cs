using System;
using System.Collections.Generic;

namespace Ttlaixe.Models;

public partial class DmDiemSatHach
{
    public int Stt { get; set; }

    public string Hang { get; set; }

    public int? LtDiemToiDa { get; set; }

    public int? LtDiemToiThieu { get; set; }

    public int? MpDiemToiDa { get; set; }

    public int? MpDiemToiThieu { get; set; }

    public int? HiDiemToiDa { get; set; }

    public int? HiDiemToiThieu { get; set; }

    public int? DdiemToiDa { get; set; }

    public int? DdDiemToiThieu { get; set; }
}
