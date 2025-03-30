using System.ComponentModel.DataAnnotations;

namespace Steaker_Store.Models
{
    public class EditRoleViewModel
    {
        public string UserId { get; set; }
        public string UserName { get; set; }

        public List<RoleSelection> Roles { get; set; } = new List<RoleSelection>();
        [Required]
        public List<string> SelectedRoles { get; set; } = new List<string>(); // ✅ Lưu danh sách role từ form
    }
}

