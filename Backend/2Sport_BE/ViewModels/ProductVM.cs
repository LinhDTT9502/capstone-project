using _2Sport_BE.Repository.Models;
using System.ComponentModel.DataAnnotations;
using System.ComponentModel.DataAnnotations.Schema;

namespace _2Sport_BE.ViewModels
{
    public class ProductDTO
    {
        
        [Required]
        public string ProductName { get; set; }
        [Required]
        [Range(1, int.MaxValue, ErrorMessage = "Listed Price must be greater than 0.")]
        public decimal? ListedPrice { get; set; }

        [Required]
        public bool IsRent { get; set; }
        [Required]
        [Range(1, int.MaxValue, ErrorMessage = "Price must be greater than 0.")]
        public decimal? Price { get; set; }

        [Required]
        [Range(1, int.MaxValue, ErrorMessage = "Rent Price must be greater than 0.")]
        public decimal? RentPrice { get; set; }
        [Required]
        public string Size { get; set; }
        public string? Description { get; set; }
        public string Color { get; set; }
        public int Condition { get; set; }
        public decimal? Height { get; set; }
        public decimal? Length { get; set; }
        public decimal? Width { get; set; }
        public decimal? Weight { get; set; }
        public string? Offers { get; set; }
        public int? Discount { get; set; }

    }
    public class ProductVM : ProductDTO
    {
        public int Id { get; set; }
        public int? BrandId { get; set; }
        public string BrandName { get; set; }
		public int? SportId { get; set; }
		public string SportName { get; set; }
        public int? CategoryID { get; set; }
		public string CategoryName { get; set; }
		public List<string> ListImages { get; set; }
        public int Likes { get; set; }
        public ICollection<Review> Reviews { get; set; }
        public string ProductCode { get; set; }
        public bool? Status { get; set; }
        public string ImgAvatarPath { get; set; }
        public DateTime CreatedAt { get; set; }
    }

    public class ProductCM : ProductDTO
    {
        [Required]
        public int? CategoryId { get; set; }
        [Required]
        public int? BrandId { get; set; }
        [Required]
        public int? SportId { get; set; }
        [Required]
        public string ProductCode { get; set; }

        public IFormFile? MainImage { get; set; }

        public IFormFile[]? ProductImages { get; set; }
        [Required]
        [Range(1, int.MaxValue, ErrorMessage = "Quantity must be greater than 0.")]
        public int Quantity { get; set; }
    }

    public class ProductUM : ProductDTO
    {
        [Required]
        public int? CategoryId { get; set; }
        [Required]
        public int? BrandId { get; set; }
        [Required]
        public int? SportId { get; set; }
        [Required]
        public string ProductCode { get; set; }

        public IFormFile? MainImage { get; set; }

        public IFormFile[]? ProductImages { get; set; }

        [Required]
        [Range(1, int.MaxValue, ErrorMessage = "Quantity must be greater than 0.")]
        public int Quantity { get; set; }
    }


}
