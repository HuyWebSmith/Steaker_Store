using Microsoft.AspNetCore.Identity;
using System.ComponentModel.DataAnnotations;

namespace Steaker_Store.Models
{
    public class ApplicationUser : IdentityUser
    {
        [Required]
        public required string FullName { get; set; }
        public string? Address { get; set; }
        public string? Age { get; set; }
        public bool IsBlocked { get; set; } = false; // Mặc định chưa bị khóa
    }
}
