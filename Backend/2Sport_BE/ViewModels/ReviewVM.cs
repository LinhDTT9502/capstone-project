using _2Sport_BE.Repository.Models;
using System.ComponentModel.DataAnnotations;

namespace _2Sport_BE.ViewModels
{
	public class ReviewDTO
	{
		[Required]
        public string ProductCode { get; set; }
		[Required]
        [Range(1, 5, ErrorMessage = "Star must be between 1 and 5.")]
        public decimal Star { get; set; }
        public string Review { get; set; }

    }
    public class ReviewVM : ReviewDTO
	{
		public int Id { get; set; }
		public virtual Product Product { get; set; }
		public virtual User User { get; set; }
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
