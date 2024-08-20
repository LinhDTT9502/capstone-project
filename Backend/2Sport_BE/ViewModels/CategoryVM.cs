using _2Sport_BE.Repository.Models;

namespace _2Sport_BE.ViewModels
{
    public class CategoryDTO
    {
        public string CategoryName { get; set; }
        public int? Quantity { get; set; }
        public bool? Status { get; set; }
        public int? SportId { get; set; }
    }
    public class CategoryVM : CategoryDTO
    {
        public int? Id { get; set; }
        public virtual Sport Sport { get; set; }
    }

    public class CategoryCM : CategoryDTO
    {
    }

    public class CategoryUM : CategoryDTO
    {
    }
}
