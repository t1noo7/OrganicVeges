using System;
using System.Collections.Generic;

namespace DiChoSaiGon.Models;

public partial class Permission
{
    public int PermissionId { get; set; }

    public int FunctionId { get; set; }

    public bool CanCreate { get; set; }

    public bool CanRead { get; set; }

    public bool CanEdit { get; set; }

    public bool CanDelete { get; set; }

    public bool AccessPermission { get; set; }

    public int RoleId { get; set; }

    public virtual Function? Function { get; set; } = null!;

    public virtual Role? Role { get; set; } = null!;
}
