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
            Carts = new HashSet<Cart>();
            Likes = new HashSet<Like>();
            Orders = new HashSet<Order>();
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
        []
        public string? Email { get; set; }
        public string? FullName { get; set; }
        public string? Gender { get; set; }
        public string? Phone { get; set; }
        public DateTime? BirthDate { get; set; }
        public DateTime CreatedDate { get; set; }
        public DateTime? LastUpdate { get; set; }
        public bool? IsActive { get; set; }
        public int? RoleId { get; set; }
        public string? Address { get; set; }
        //Thêm column mới
        public string? GoogleId { get; set; }
        public string? FacebookId { get; set; }
        public bool EmailConfirmed { get; set; }
        public string? PasswordResetToken { get; set; }
        public string? Token { get; set; }

        public virtual Role Role { get; set; }
        public virtual ICollection<Cart> Carts { get; set; }
        public virtual ICollection<Like> Likes { get; set; }
        public virtual ICollection<Order> Orders { get; set; }
        public virtual ICollection<RefreshToken> RefreshTokens { get; set; }
        public virtual ICollection<Review> Reviews { get; set; }
        public virtual ICollection<ShipmentDetail> ShipmentDetails { get; set; }
    }
}