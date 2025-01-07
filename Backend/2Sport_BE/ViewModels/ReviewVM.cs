using _2Sport_BE.Repository.Models;
using System.ComponentModel.DataAnnotations;

namespace _2Sport_BE.ViewModels
{
	public class ReviewDTO
	{
		[Required]
        public int ProductId { get; set; }
		[Required]
        [Range(1, 5, ErrorMessage = "Star must be between 1 and 5.")]
        public decimal Star { get; set; }
        public string ReviewContent { get; set; }

    }
    public class ReviewVM : ReviewDTO
	{
		public int Id { get; set; }
		public string Color{ get; set; }
		public string Size{ get; set; }
		public int Condition { get; set; }
		public string UserName { get; set; }
        public int? SaleOrderId { get; set; }
		public int? UserId { get; set; }
		public bool? Status { get; set; }
    }

    public class ReviewCM : ReviewDTO
	{

	}

	public class ReviewUM : ReviewDTO
	{

	}
}
