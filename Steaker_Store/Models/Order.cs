using Microsoft.AspNetCore.Mvc.ModelBinding.Validation;
using Microsoft.EntityFrameworkCore.Metadata.Internal;
using System.ComponentModel.DataAnnotations.Schema;

namespace Steaker_Store.Models
{
    public class Order
    {
        public int Id { get; set; }
        public string? UserId { get; set; } // Null nếu là khách vãng lai
        public DateTime OrderDate { get; set; } = DateTime.Now;
        public decimal TotalPrice { get; set; }

        public string? TableNumber { get; set; } // Số bàn nếu ăn tại quán
        public string? ShippingAddress { get; set; } // Địa chỉ giao hàng (nếu có)
        public string? Notes { get; set; }

        [ForeignKey("UserId")]
        [ValidateNever]
        public ApplicationUser? ApplicationUser { get; set; } // Liên kết với ApplicationUser

        public List<OrderDetail>? OrderDetails { get; set; }
    }
}
