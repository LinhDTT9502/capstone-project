using _2Sport_BE.Repository.Models;
using Microsoft.EntityFrameworkCore;
using Microsoft.Extensions.Configuration;

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
        public virtual DbSet<Cart> Carts { get; set; }
        public virtual DbSet<CartItem> CartItems { get; set; }
        public virtual DbSet<Category> Categories { get; set; }
        public virtual DbSet<Comment> Comments { get; set; }
        public virtual DbSet<ImportHistory> ImportHistories { get; set; }
        public virtual DbSet<ImagesVideo> ImagesVideos { get; set; }
        public virtual DbSet<Like> Likes { get; set; }
        public virtual DbSet<SaleOrder> SaleOrder { get; set; }
        public virtual DbSet<OrderDetail> OrderDetails { get; set; }
        public virtual DbSet<PaymentMethod> PaymentMethods { get; set; }
        public virtual DbSet<Product> Products { get; set; }
        public virtual DbSet<RefreshToken> RefreshTokens { get; set; }
        public virtual DbSet<Review> Reviews { get; set; }
        public virtual DbSet<Role> Roles { get; set; }
        public virtual DbSet<ShipmentDetail> ShipmentDetails { get; set; }
        public virtual DbSet<Sport> Sport { get; set; }
        public virtual DbSet<User> Users { get; set; }
        public virtual DbSet<Staff> Staffs { get; set; }
        public virtual DbSet<TwilioAccount> TwilioAccount { get; set; }
        public virtual DbSet<Warehouse> Warehouses { get; set; }
        public virtual DbSet<ErrorLog> ErrorLogs { get; set; }
        public virtual DbSet<RentalOrder> RentalOrders { get; set; }
        public virtual DbSet<RefundRequest> RefundRequests { get; set; }

        protected override void OnModelCreating(ModelBuilder modelBuilder)
        {
            // Configure the relationship for CreatedByStaff
            modelBuilder.Entity<Blog>()
                .HasOne(b => b.CreatedByStaff)
                .WithMany(s => s.CreatedBlogs) // Add CreatedBlogs ICollection in Staff if not already
                .HasForeignKey(b => b.CreatedStaffId)
                .OnDelete(DeleteBehavior.Restrict);

            // Configure the relationship for EditedByStaff
            modelBuilder.Entity<Blog>()
                .HasOne(b => b.EditedByStaff)
                .WithMany(s => s.EditedBlogs) // Add EditedBlogs ICollection in Staff if not already
                .HasForeignKey(b => b.EditedByStaffId)
                .OnDelete(DeleteBehavior.Restrict);
        }
        protected override void OnConfiguring(DbContextOptionsBuilder optionsBuilder)
        {
            if (!optionsBuilder.IsConfigured)
            {
                optionsBuilder.UseSqlServer("Server=(local);uid=sa;pwd=12345;database=TwoSportDB;TrustServerCertificate=True");
            }
        }
        private string GetConnectionStrings()
        {
            IConfiguration config = new ConfigurationBuilder()
             .SetBasePath(Directory.GetCurrentDirectory())
            .AddJsonFile("appsettings.json", true, true)
            .Build();
            var strConn = config["ConnectionStrings:DefaultConnection"];

            return strConn;
        }
    }
}
