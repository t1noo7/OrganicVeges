using System;
using System.Collections.Generic;

namespace DiChoSaiGon.Models;

public partial class Function
{
    public int FunctionId { get; set; }

    public string? FunctionName { get; set; }

    public virtual ICollection<Permission> Permissions { get; set; } = new List<Permission>();
}
