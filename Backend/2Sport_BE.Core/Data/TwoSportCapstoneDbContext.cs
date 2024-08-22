using _2Sport_BE.Repository.Models;
using Microsoft.EntityFrameworkCore;
using Microsoft.Extensions.Configuration;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace _2Sport_BE.Repository.Data
{
    public class TwoSportCapstoneDbContext : DbContext
    {

        public TwoSportCapstoneDbContext() { }

        public TwoSportCapstoneDbContext(DbContextOptions<TwoSportCapstoneDbContext> options) : base(options)
        {

        }

        public virtual DbSet<Blog> Blogs { get; set; }

        public virtual DbSet<Branch> Branches { get; set; }

        public virtual DbSet<Brand> Brands { get; set; }

        public virtual DbSet<BrandCategory> BrandCategories { get; set; }

        public virtual DbSet<Cart> Carts { get; set; }

        public virtual DbSet<CartItem> CartItems { get; set; }

        public virtual DbSet<Category> Categories { get; set; }

        public virtual DbSet<CustomerDetail> CustomerDetails { get; set; }

        public virtual DbSet<EmployeeDetail> EmployeeDetails { get; set; }

        public virtual DbSet<ImportHistory> ImportHistories { get; set; }
        public virtual DbSet<ImagesVideo> ImagesVideos { get; set; }

        public virtual DbSet<Like> Likes { get; set; }
        public virtual DbSet<Order> Orders { get; set; }

        public virtual DbSet<OrderDetail> OrderDetails { get; set; }

        public virtual DbSet<PaymentMethod> PaymentMethods { get; set; }

        public virtual DbSet<Product> Products { get; set; }

        public virtual DbSet<RefreshToken> RefreshTokens { get; set; }

        public virtual DbSet<Review> Reviews { get; set; }

        public virtual DbSet<Role> Roles { get; set; }

        public virtual DbSet<SalaryHistory> SalaryHistories { get; set; }

        public virtual DbSet<ShipmentDetail> ShipmentDetails { get; set; }

        public virtual DbSet<Sport> Sport { get; set; }
        public virtual DbSet<Supplier> Suppliers { get; set; }
        public virtual DbSet<User> Users { get; set; }

        public virtual DbSet<Warehouse> Warehouses { get; set; }

        //protected override void OnConfiguring(DbContextOptionsBuilder optionsBuilder)
        //{
        //    if (!optionsBuilder.IsConfigured)
        //    {
        //        optionsBuilder.UseSqlServer(GetConnectionStrings());
        //    }
        //}

        //private string GetConnectionStrings()
        //{
        //    IConfiguration config = new ConfigurationBuilder()
        //     .SetBasePath(Directory.GetCurrentDirectory())
        //    .AddJsonFile("appsettings.json", true, true)
        //    .Build();
        //    var strConn = config["ConnectionStrings:DefaultConnection"];

        //    return strConn;
        //}
    }
}
