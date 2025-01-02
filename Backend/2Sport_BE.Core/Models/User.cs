using System;
using System.Collections.Generic;
using System.ComponentModel.DataAnnotations;
using System.ComponentModel.DataAnnotations.Schema;

namespace _2Sport_BE.Repository.Models
{
    public partial class User
    {
        public User()
        {
            Likes = new HashSet<Like>();
            SaleOrders = new HashSet<SaleOrder>();
            RentalOrders = new HashSet<RentalOrder>();
            RefreshTokens = new HashSet<RefreshToken>();
            Reviews = new HashSet<Review>();
            ShipmentDetails = new HashSet<ShipmentDetail>();
        }
        [Key]
        [DatabaseGenerated(DatabaseGeneratedOption.Identity)]
        public int Id { get; set; }
        [Column("UserName")]
        public string? UserName { get; set; }
        [Column("HashPassword")]
        public string? HashPassword { get; set; }
        [Column("Email")]
        public string? Email { get; set; }
        [Column("FullName")]
        public string? FullName { get; set; }

        [Column("Gender")]
        public string? Gender { get; set; }

        [Column("PhoneNumber")]
        public string? PhoneNumber { get; set; }

        [Column("DOB")]
        public DateTime? DOB { get; set; }

        [Column("Address")]
        public string? Address { get; set; }

        [Column("CreatedAt")]
        public DateTime CreatedAt { get; set; }

        [Column("UpdatedAt")]
        public DateTime? UpdatedAt { get; set; }

        [Column("IsActived")]
        public bool? IsActived { get; set; }

        public int? RoleId { get; set; }
        //Thêm column mới
        public string? GoogleId { get; set; }
        public string? FacebookId { get; set; }
        [Column("IsEmailConfirmed")]
        public bool EmailConfirmed { get; set; }

        [Column("IsPhoneNumberConfirmed")]
        public bool PhoneNumberConfirmed { get; set; }

        [Column("OTP")]
        public int OTP { get; set; }
        public string? PasswordResetToken { get; set; }
        public string? Token { get; set; }
        [Column("ImgAvatarPath", TypeName = "varchar")]
        [MaxLength(500)]
        public string? ImgAvatarPath { get; set; }
        public virtual Role Role { get; set; }
        public virtual ICollection<Like> Likes { get; set; }
        public virtual ICollection<SaleOrder> SaleOrders { get; set; }
        public virtual ICollection<RentalOrder> RentalOrders { get; set; }
        public virtual ICollection<RefreshToken> RefreshTokens { get; set; }
        public virtual ICollection<Review> Reviews { get; set; }
        public virtual ICollection<ShipmentDetail> ShipmentDetails { get; set; }
        public virtual ICollection<Manager> Managers { get; set; }
        public virtual ICollection<Staff> Staffs { get; set; }
        public virtual ICollection<Customer> Customers { get; set; }
        public virtual ICollection<CartItem> CartItems { get; set; }
        public virtual ICollection<Bookmark> Bookmarks { get; set; }
        public virtual ICollection<Blog> CreatedBlogs { get; set; } // Blogs created by this staff
        public virtual ICollection<Blog> EditedBlogs { get; set; } // Blogs edited by this staff
    }
}