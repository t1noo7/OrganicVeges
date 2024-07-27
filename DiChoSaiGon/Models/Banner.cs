using System;
using System.Collections.Generic;

namespace DiChoSaiGon.Models;

public partial class Banner
{
    public int BannerId { get; set; }

    public string? BannerName { get; set; }

    public string? Thumb { get; set; }

    public DateTime? DateModified { get; set; }

    public bool Active { get; set; }

    public string? BannerText { get; set; }

    public bool ActiveButton { get; set; }

    public string? BannerHeaderText { get; set; }

    public int OrderIndex { get; set; }
}
