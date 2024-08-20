using _2Sport_BE.Repository.Models;

namespace _2Sport_BE.ViewModels
{
	public class ReviewDTO
	{
		public decimal? Star { get; set; }
		public string Review1 { get; set; }
		public bool? Status { get; set; }

		
	}
	public class ReviewVM : ReviewDTO
	{
		public int Id { get; set; }
		public virtual Product Product { get; set; }
		public virtual User User { get; set; }
		public int? UserId { get; set; }
		public int? ProductId { get; set; }
	}

	public class ReviewCM : ReviewDTO
	{

	}

	public class ReviewUM : ReviewDTO
	{

	}
}
