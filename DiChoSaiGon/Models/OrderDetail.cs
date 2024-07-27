using System;
using System.Collections.Generic;

namespace DiChoSaiGon.Models;

public partial class OrderDetail
{
    public int OrderDetailsId { get; set; }

    public int? OrderId { get; set; }

    public int? ProductId { get; set; }

    public string? ProductName { get; set; }

    public int? OrderNumber { get; set; }

    public int? Amount { get; set; }

    public int? Discount { get; set; }

    public int? TotalMoney { get; set; }

    public DateTime? CreateDate { get; set; }

    public int? Price { get; set; }

    public virtual Order? Order { get; set; }

    public virtual Product? Product { get; set; }
}
