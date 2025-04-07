using System.ComponentModel.DataAnnotations;

namespace Steaker_Store.Models
{
    public class CheckoutViewModel
    {
        [Required]
        [Display(Name = "Full Name")]
        public string FullName { get; set; }

        [Required]
        [EmailAddress]
        public string Email { get; set; }

        [Required]
        [Phone]
        [Display(Name = "Phone Number")]
        public string PhoneNumber { get; set; }

        [Required]
        [Display(Name = "Shipping Address")]
        public string ShippingAddress { get; set; }

        [Display(Name = "Notes")]
        public string? Notes { get; set; }
    }
}
