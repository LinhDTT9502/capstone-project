using _2Sport_BE.Repository.Models;

namespace _2Sport_BE.ViewModels
{
    public class BrandDTO
    {
        public string BrandName { get; set; }
        public string Logo { get; set; }

    }
    public class BrandVM : BrandDTO
    {
        public int Id { get; set; }
        public int? Quantity { get; set; }
        public bool? Status { get; set; }
    }

	public class BrandCM : BrandDTO
    {
    }

    public class BrandUM : BrandDTO
    {
    }

}
