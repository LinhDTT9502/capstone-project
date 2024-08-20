using System;
using System.Collections.Generic;

namespace _2Sport_BE.Repository.Models
{
    public partial class Product
    {
        public Product()
        {
            CartItems = new HashSet<CartItem>();
            ImagesVideos = new HashSet<ImagesVideo>();
            ImportHistories = new HashSet<ImportHistory>();
            Likes = new HashSet<Like>();
            OrderDetails = new HashSet<OrderDetail>();
            Reviews = new HashSet<Review>();
            Warehouses = new HashSet<Warehouse>();
        }

        public int Id { get; set; }
        public string ProductName { get; set; }
        public decimal? ListedPrice { get; set; }
        public decimal? Price { get; set; }
        public string Size { get; set; }
        public string Description { get; set; }
        public bool? Status { get; set; }
        public string Color { get; set; }
        public string Offers { get; set; }
        public string MainImageName { get; set; }
        public string MainImagePath { get; set; }
        public int? CategoryId { get; set; }
        public int? BrandId { get; set; }
        public int? SportId { get; set; }
        public int? ClassificationId { get; set; }
        public string ProductCode { get; set; }

        public virtual Brand Brand { get; set; }
        public virtual Category Category { get; set; }
        public virtual Classification Classification { get; set; }
        public virtual Sport Sport { get; set; }
        public virtual ICollection<CartItem> CartItems { get; set; }
        public virtual ICollection<ImagesVideo> ImagesVideos { get; set; }
        public virtual ICollection<ImportHistory> ImportHistories { get; set; }
        public virtual ICollection<Like> Likes { get; set; }
        public virtual ICollection<OrderDetail> OrderDetails { get; set; }
        public virtual ICollection<Review> Reviews { get; set; }
        public virtual ICollection<Warehouse> Warehouses { get; set; }
    }
}
