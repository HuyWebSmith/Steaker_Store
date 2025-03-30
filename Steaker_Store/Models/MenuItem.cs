using System.ComponentModel.DataAnnotations;

namespace Steaker_Store.Models
{
    public class MenuItem
    {
        public int Id { get; set; }
        [Required, StringLength(100)]
        public required string Name { get; set; }
        [Range(1, 1000000)]
        public decimal Price { get; set; }
        public string?  Description { get; set; }
        public string? ImageUrl { get; set; }
        public List<MenuItemImage>? Images { get; set; }
        public int CategoryId { get; set; }
        public Category? Category { get; set; }
    }
}
