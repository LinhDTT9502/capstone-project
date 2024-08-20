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
        public decimal TotalPrice { get; set; }
        public string MainImageName { get; set; }
        public string MainImagePath{ get; set; }
	}

	public class CartItemCM : CartItemDTO
    {
    }

    public class CartItemUM : CartItemDTO
    {
    }

}
