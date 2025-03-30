namespace Steaker_Store.Models
{
    public class OrderDetail
    {
        public int Id { get; set; }
        public int OrderId { get; set; }
        public int MenuItemId { get; set; } // Thay ProductId bằng MenuItemId
        public int Quantity { get; set; }
        public decimal Price { get; set; }

        public Order? Order { get; set; }
        public MenuItem? MenuItem { get; set; }
    }
}
