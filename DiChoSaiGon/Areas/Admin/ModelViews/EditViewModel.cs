using DiChoSaiGon.Models;
using System.Collections.Generic;

namespace DiChoSaiGon.Areas.Admin.ViewModel
{
    public class EditViewModel
    {
        public Role singleRole { get; set; }
        public List<RoleViewModel> RelatedRoles { get; set; }
    }

    public class RoleViewModel
    {
        public Role Role { get; set; }
        public Function Function { get; set; }
        public Permission Permission { get; set; }
    }
}
