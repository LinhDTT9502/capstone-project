using _2Sport_BE.Repository.Models;

namespace _2Sport_BE.ViewModels
{
    public class CartItemDTO
    {
        public int? ProductId { get; set; }
        public int? Quantity { get; set; }
    }
    public class CartItemVM : CartItemDTO
    {
        public int Id { get; set; }
        public string ProductName { get; set; }
        public string ProductCode { get; set; }
        public string Color { get; set; }
        public string Size { get; set; }
        public int Condition { get; set; }
        public decimal Price { get; set; }
        public decimal RentPrice { get; set; }
        public string ImgAvatarPath { get; set; }
    }

	public class CartItemCM : CartItemDTO
    {
    }

    public class CartItemUM : CartItemDTO
    {
    }

}
