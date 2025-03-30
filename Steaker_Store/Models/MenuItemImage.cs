namespace Steaker_Store.Models
{
    public class MenuItemImage
    {
        public int Id { get; set; }
        public string? Url { get; set; }
        public int MenuItemId { get; set; }
        public MenuItem? MenuItem { get; set; }
    }
}
