using System;
using System.Collections.Generic;

namespace Ttlaixe.Models;

public partial class AuditLog
{
    public int Id { get; set; }

    public string TableName { get; set; }

    public string ActionType { get; set; }

    public string KeyValue { get; set; }

    public string OldData { get; set; }

    public string NewData { get; set; }

    public string ModifiedBy { get; set; }

    public DateTime? ModifiedAt { get; set; }
}
