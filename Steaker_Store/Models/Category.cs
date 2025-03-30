using System.ComponentModel.DataAnnotations;

namespace Steaker_Store.Models
{
    public class Category
    {
        public int Id { get; set; }
        [Required, StringLength(50)]
        public required string Name { get; set; }
        public List<MenuItem>? MenuItems { get; set; }
    }
}
