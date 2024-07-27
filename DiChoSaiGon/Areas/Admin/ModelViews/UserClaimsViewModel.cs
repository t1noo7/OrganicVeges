using DiChoSaiGon.Models;
using System.Collections.Generic;
using System.Data;
using System.Security.Claims;

namespace DiChoSaiGon.Areas.Admin.ModelViews
{
    public class UserClaimsViewModel
    {
        public List<Claim> Claims { get; set; }
    }
}
