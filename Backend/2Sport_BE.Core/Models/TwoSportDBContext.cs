using System;
using System.Collections.Generic;
using Microsoft.EntityFrameworkCore;
using Microsoft.EntityFrameworkCore.Metadata;

namespace _2Sport_BE.Repository.Models
{
    public partial class TwoSportDBContext : DbContext
    {
        public TwoSportDBContext()
        {
        }

        public TwoSportDBContext(DbContextOptions<TwoSportDBContext> options)
            : base(options)
        {
        }

        public virtual DbSet<Blog> Blogs { get; set; }
        public virtual DbSet<Brand> Brands { get; set; }
        public virtual DbSet<BrandCategory> BrandCategories { get; set; }
        public virtual DbSet<Cart> Carts { get; set; }
        public virtual DbSet<CartItem> CartItems { get; set; }
        public virtual DbSet<Category> Categories { get; set; }
        public virtual DbSet<Classification> Classifications { get; set; }
        public virtual DbSet<ImagesVideo> ImagesVideos { get; set; }
        public virtual DbSet<ImportHistory> ImportHistories { get; set; }
        public virtual DbSet<Like> Likes { get; set; }
        public virtual DbSet<Order> Orders { get; set; }
        public virtual DbSet<OrderDetail> OrderDetails { get; set; }
        public virtual DbSet<PaymentMethod> PaymentMethods { get; set; }
        public virtual DbSet<Product> Products { get; set; }
        public virtual DbSet<RefreshToken> RefreshTokens { get; set; }
        public virtual DbSet<Review> Reviews { get; set; }
        public virtual DbSet<Role> Roles { get; set; }
        public virtual DbSet<ShipmentDetail> ShipmentDetails { get; set; }
        public virtual DbSet<Sport> Sports { get; set; }
        public virtual DbSet<Supplier> Suppliers { get; set; }
        public virtual DbSet<User> Users { get; set; }
        public virtual DbSet<Warehouse> Warehouses { get; set; }

        /*protected override void OnConfiguring(DbContextOptionsBuilder optionsBuilder)
        {
            if (!optionsBuilder.IsConfigured)
            {
#warning To protect potentially sensitive information in your connection string, you should move it out of source code. You can avoid scaffolding the connection string by using the Name= syntax to read it from configuration - see https://go.microsoft.com/fwlink/?linkid=2131148. For more guidance on storing connection strings, see http://go.microsoft.com/fwlink/?LinkId=723263.
                optionsBuilder.UseSqlServer("Server=DESKTOP-E9T9GDT;uid=sa;pwd=12345;database= TwoSportDB;TrustServerCertificate=True");
            }
        }*/

        protected override void OnModelCreating(ModelBuilder modelBuilder)
        {
            modelBuilder.Entity<Blog>(entity =>
            {
                entity.HasIndex(e => e.Id, "UQ__Blogs__3214EC0624FD893D")
                    .IsUnique();

                entity.Property(e => e.BlogName).HasMaxLength(255);

                entity.Property(e => e.CreateAt).HasColumnType("datetime");

                entity.Property(e => e.MainImageName)
                    .HasMaxLength(255)
                    .IsUnicode(false);

                entity.Property(e => e.MainImagePath).IsUnicode(false);

                entity.HasOne(d => d.Sport)
                    .WithMany(p => p.Blogs)
                    .HasForeignKey(d => d.SportId)
                    .HasConstraintName("FK__Blogs__SportId__73BA3083");
            });

            modelBuilder.Entity<Brand>(entity =>
            {
                entity.HasIndex(e => e.Id, "UQ__Brands__3214EC06D1E78A79")
                    .IsUnique();

                entity.Property(e => e.BrandName).HasMaxLength(255);
            });

            modelBuilder.Entity<BrandCategory>(entity =>
            {
                entity.ToTable("BrandCategory");

                entity.HasIndex(e => e.Id, "UQ__BrandCat__3214EC069448474C")
                    .IsUnique();

                entity.HasOne(d => d.Brand)
                    .WithMany(p => p.BrandCategories)
                    .HasForeignKey(d => d.BrandId)
                    .HasConstraintName("FK__BrandCate__Brand__75A278F5");

                entity.HasOne(d => d.Category)
                    .WithMany(p => p.BrandCategories)
                    .HasForeignKey(d => d.CategoryId)
                    .HasConstraintName("FK__BrandCate__Categ__74AE54BC");
            });

            modelBuilder.Entity<Cart>(entity =>
            {
                entity.HasIndex(e => e.Id, "UQ__Carts__3214EC067447A3E7")
                    .IsUnique();

                entity.HasOne(d => d.User)
                    .WithMany(p => p.Carts)
                    .HasForeignKey(d => d.UserId)
                    .HasConstraintName("FK__Carts__UserId__787EE5A0");
            });

            modelBuilder.Entity<CartItem>(entity =>
            {
                entity.HasIndex(e => e.Id, "UQ__CartItem__3214EC0670276BEC")
                    .IsUnique();

                entity.Property(e => e.TotalPrice).HasColumnType("decimal(18, 0)");

                entity.HasOne(d => d.Cart)
                    .WithMany(p => p.CartItems)
                    .HasForeignKey(d => d.CartId)
                    .HasConstraintName("FK__CartItems__CartI__778AC167");

                entity.HasOne(d => d.Product)
                    .WithMany(p => p.CartItems)
                    .HasForeignKey(d => d.ProductId)
                    .HasConstraintName("FK__CartItems__Produ__76969D2E");
            });

            modelBuilder.Entity<Category>(entity =>
            {
                entity.HasIndex(e => e.Id, "UQ__Categori__3214EC06CEC2D1B1")
                    .IsUnique();

                entity.Property(e => e.CategoryName).HasMaxLength(255);
            });

            modelBuilder.Entity<Classification>(entity =>
            {
                entity.HasIndex(e => e.Id, "UQ__Classifi__3214EC06EE82EB43")
                    .IsUnique();

                entity.Property(e => e.ClassificationName).HasMaxLength(255);
            });

            modelBuilder.Entity<ImagesVideo>(entity =>
            {
                entity.HasIndex(e => e.Id, "UQ__ImagesVi__3214EC06F5E0B1F0")
                    .IsUnique();

                entity.Property(e => e.ImageName).HasMaxLength(50);

                entity.Property(e => e.ImagePath).IsUnicode(false);

                entity.Property(e => e.VideoName).HasMaxLength(50);

                entity.Property(e => e.VideoPath).IsUnicode(false);

                entity.HasOne(d => d.Blog)
                    .WithMany(p => p.ImagesVideos)
                    .HasForeignKey(d => d.BlogId)
                    .HasConstraintName("FK__ImagesVid__BlogI__7A672E12");

                entity.HasOne(d => d.Product)
                    .WithMany(p => p.ImagesVideos)
                    .HasForeignKey(d => d.ProductId)
                    .HasConstraintName("FK__ImagesVid__Produ__797309D9");
            });

            modelBuilder.Entity<ImportHistory>(entity =>
            {
                entity.HasIndex(e => e.Id, "UQ__ImportHi__3214EC06CD9E1B2D")
                    .IsUnique();

                entity.Property(e => e.ImportCode).HasMaxLength(255);

                entity.Property(e => e.ImportDate).HasColumnType("datetime");

                entity.Property(e => e.LotCode).HasMaxLength(255);

                entity.HasOne(d => d.Product)
                    .WithMany(p => p.ImportHistories)
                    .HasForeignKey(d => d.ProductId)
                    .HasConstraintName("FK__ImportHis__Produ__7B5B524B");

                entity.HasOne(d => d.Supplier)
                    .WithMany(p => p.ImportHistories)
                    .HasForeignKey(d => d.SupplierId)
                    .HasConstraintName("FK__ImportHis__Suppl__7C4F7684");
            });

            modelBuilder.Entity<Like>(entity =>
            {
                entity.HasIndex(e => e.Id, "UQ__Likes__3214EC062F085310")
                    .IsUnique();

                entity.HasOne(d => d.Blog)
                    .WithMany(p => p.Likes)
                    .HasForeignKey(d => d.BlogId)
                    .HasConstraintName("FK__Likes__BlogId__7E37BEF6");

                entity.HasOne(d => d.Product)
                    .WithMany(p => p.Likes)
                    .HasForeignKey(d => d.ProductId)
                    .HasConstraintName("FK__Likes__ProductId__7D439ABD");

                entity.HasOne(d => d.User)
                    .WithMany(p => p.Likes)
                    .HasForeignKey(d => d.UserId)
                    .HasConstraintName("FK__Likes__UserId__7F2BE32F");
            });

            modelBuilder.Entity<Order>(entity =>
            {
                entity.HasIndex(e => e.Id, "UQ__Orders__3214EC069E0C3512")
                    .IsUnique();

                entity.Property(e => e.IntoMoney).HasColumnType("decimal(18, 0)");

                entity.Property(e => e.OrderCode).HasMaxLength(255);

                entity.Property(e => e.ReceivedDate).HasColumnType("datetime");

                entity.Property(e => e.TotalPrice).HasColumnType("decimal(18, 0)");

                entity.Property(e => e.TransportFee).HasColumnType("decimal(18, 0)");

                entity.HasOne(d => d.PaymentMethod)
                    .WithMany(p => p.Orders)
                    .HasForeignKey(d => d.PaymentMethodId)
                    .HasConstraintName("FK__Orders__PaymentM__02FC7413");

                entity.HasOne(d => d.ShipmentDetail)
                    .WithMany(p => p.Orders)
                    .HasForeignKey(d => d.ShipmentDetailId)
                    .HasConstraintName("FK__Orders__Shipment__02084FDA");

                entity.HasOne(d => d.User)
                    .WithMany(p => p.Orders)
                    .HasForeignKey(d => d.UserId)
                    .HasConstraintName("FK__Orders__UserId__03F0984C");
            });

            modelBuilder.Entity<OrderDetail>(entity =>
            {
                entity.HasIndex(e => e.Id, "UQ__OrderDet__3214EC06FADB848B")
                    .IsUnique();

                entity.HasOne(d => d.Order)
                    .WithMany(p => p.OrderDetails)
                    .HasForeignKey(d => d.OrderId)
                    .OnDelete(DeleteBehavior.ClientSetNull)
                    .HasConstraintName("FK__OrderDeta__Order__00200768");

                entity.HasOne(d => d.Product)
                    .WithMany(p => p.OrderDetails)
                    .HasForeignKey(d => d.ProductId)
                    .OnDelete(DeleteBehavior.ClientSetNull)
                    .HasConstraintName("FK__OrderDeta__Produ__01142BA1");
            });

            modelBuilder.Entity<PaymentMethod>(entity =>
            {
                entity.ToTable("PaymentMethod");

                entity.HasIndex(e => e.Id, "UQ__PaymentM__3214EC068855EE74")
                    .IsUnique();

                entity.Property(e => e.PaymentMethodName).HasMaxLength(255);
            });

            modelBuilder.Entity<Product>(entity =>
            {
                entity.HasIndex(e => e.Id, "UQ__Products__3214EC06CF2D8A01")
                    .IsUnique();

                entity.Property(e => e.Color)
                    .IsRequired()
                    .HasMaxLength(255);

                entity.Property(e => e.ListedPrice).HasColumnType("decimal(18, 0)");

                entity.Property(e => e.MainImageName).HasMaxLength(255);

                entity.Property(e => e.MainImagePath).IsUnicode(false);

                entity.Property(e => e.Offers).IsRequired();

                entity.Property(e => e.Price).HasColumnType("decimal(18, 0)");

                entity.Property(e => e.ProductCode).HasMaxLength(255);

                entity.Property(e => e.ProductName).HasMaxLength(255);

                entity.Property(e => e.Size)
                    .IsRequired()
                    .HasMaxLength(5)
                    .IsUnicode(false);

                entity.HasOne(d => d.Brand)
                    .WithMany(p => p.Products)
                    .HasForeignKey(d => d.BrandId)
                    .HasConstraintName("FK__Products__BrandI__04E4BC85");

                entity.HasOne(d => d.Category)
                    .WithMany(p => p.Products)
                    .HasForeignKey(d => d.CategoryId)
                    .HasConstraintName("FK__Products__Catego__05D8E0BE");

                entity.HasOne(d => d.Classification)
                    .WithMany(p => p.Products)
                    .HasForeignKey(d => d.ClassificationId)
                    .HasConstraintName("FK__Products__Classi__06CD04F7");

                entity.HasOne(d => d.Sport)
                    .WithMany(p => p.Products)
                    .HasForeignKey(d => d.SportId)
                    .HasConstraintName("FK__Products__SportI__07C12930");
            });

            modelBuilder.Entity<RefreshToken>(entity =>
            {
                entity.ToTable("RefreshToken");

                entity.HasIndex(e => e.RefreshTokenId, "UQ__RefreshT__F5845E38F471A76F")
                    .IsUnique();

                entity.Property(e => e.CreateDate).HasColumnType("datetime");

                entity.Property(e => e.ExpiryDate).HasColumnType("datetime");

                entity.HasOne(d => d.User)
                    .WithMany(p => p.RefreshTokens)
                    .HasForeignKey(d => d.UserId)
                    .HasConstraintName("FK__RefreshTo__UserI__08B54D69");
            });

            modelBuilder.Entity<Review>(entity =>
            {
                entity.HasIndex(e => e.Id, "UQ__Reviews__3214EC0637DB1E90")
                    .IsUnique();

                entity.Property(e => e.Review1)
                    .HasMaxLength(255)
                    .HasColumnName("Review");

                entity.Property(e => e.Star).HasColumnType("numeric(18, 0)");

                entity.HasOne(d => d.Product)
                    .WithMany(p => p.Reviews)
                    .HasForeignKey(d => d.ProductId)
                    .HasConstraintName("FK__Reviews__Product__09A971A2");

                entity.HasOne(d => d.User)
                    .WithMany(p => p.Reviews)
                    .HasForeignKey(d => d.UserId)
                    .HasConstraintName("FK__Reviews__UserId__0A9D95DB");
            });

            modelBuilder.Entity<Role>(entity =>
            {
                entity.HasIndex(e => e.Id, "UQ__Roles__3214EC06A0E5B809")
                    .IsUnique();

                entity.Property(e => e.CreatedOn).HasColumnType("datetime");

                entity.Property(e => e.RoleName).HasMaxLength(255);

                entity.Property(e => e.UpdatedOn).HasColumnType("datetime");
            });

            modelBuilder.Entity<ShipmentDetail>(entity =>
            {
                entity.HasIndex(e => e.Id, "UQ__Shipment__3214EC06D1B6B308")
                    .IsUnique();

                entity.Property(e => e.Address).HasMaxLength(255);

                entity.Property(e => e.FullName).HasMaxLength(255);

                entity.Property(e => e.PhoneNumber).HasMaxLength(255);

                entity.HasOne(d => d.User)
                    .WithMany(p => p.ShipmentDetails)
                    .HasForeignKey(d => d.UserId)
                    .HasConstraintName("FK__ShipmentD__UserI__0B91BA14");
            });

            modelBuilder.Entity<Sport>(entity =>
            {
                entity.HasIndex(e => e.Id, "UQ__Sports__3214EC063D489370")
                    .IsUnique();
            });

            modelBuilder.Entity<Supplier>(entity =>
            {
                entity.HasIndex(e => e.Id, "UQ__Supplier__3214EC061A804565")
                    .IsUnique();

                entity.Property(e => e.Location).HasMaxLength(255);

                entity.Property(e => e.SupplierName).HasMaxLength(255);
            });

            modelBuilder.Entity<User>(entity =>
            {
                entity.HasIndex(e => e.Id, "UQ__Users__3214EC065D802F08")
                    .IsUnique();

                entity.Property(e => e.Address).HasMaxLength(255);

                entity.Property(e => e.BirthDate).HasColumnType("datetime");

                entity.Property(e => e.CreatedDate).HasColumnType("datetime");

                entity.Property(e => e.Email).HasMaxLength(255);

                entity.Property(e => e.FullName).HasMaxLength(255);

                entity.Property(e => e.Gender).HasMaxLength(20);

                entity.Property(e => e.LastUpdate).HasColumnType("datetime");

                entity.Property(e => e.Password).HasMaxLength(255);

                entity.Property(e => e.Phone).HasMaxLength(20);

                entity.Property(e => e.Salary).HasMaxLength(20);

                entity.Property(e => e.UserName).HasMaxLength(255);

                entity.HasOne(d => d.Role)
                    .WithMany(p => p.Users)
                    .HasForeignKey(d => d.RoleId)
                    .HasConstraintName("FK__Users__RoleId__0C85DE4D");
            });

            modelBuilder.Entity<Warehouse>(entity =>
            {
                entity.HasIndex(e => e.Id, "UQ__Warehous__3214EC06033043FA")
                    .IsUnique();

                entity.HasOne(d => d.Product)
                    .WithMany(p => p.Warehouses)
                    .HasForeignKey(d => d.ProductId)
                    .HasConstraintName("FK__Warehouse__Produ__0D7A0286");
            });

            OnModelCreatingPartial(modelBuilder);
        }

        partial void OnModelCreatingPartial(ModelBuilder modelBuilder);
    }
}
