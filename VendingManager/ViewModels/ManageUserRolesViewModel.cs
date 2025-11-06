using Microsoft.AspNetCore.Identity;
using System.Collections.Generic;

namespace VendingManager.ViewModels
{
    public class RoleCheckboxViewModel
    {
        public string RoleId { get; set; } = string.Empty;
        public string RoleName { get; set; } = string.Empty;
        public bool IsSelected { get; set; }
    }

    public class ManageUserRolesViewModel
    {
        public string UserId { get; set; } = string.Empty;
        public string UserName { get; set; } = string.Empty;

        public List<RoleCheckboxViewModel> Roles { get; set; } = new List<RoleCheckboxViewModel>();
    }
}
