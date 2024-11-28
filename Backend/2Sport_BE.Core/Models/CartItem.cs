using System.ComponentModel.DataAnnotations;
using System.ComponentModel.DataAnnotations.Schema;

namespace _2Sport_BE.Repository.Models
{
    [Table("CartItems")]
    public class CartItem
    {
        [Key]
        public Guid CartItemId { get; set; }
        public int? ProductId { get; set; }
        public int? Quantity { get; set; }
        public decimal? Price { get; set; }
        public int? UserId { get; set; }

        public virtual User User { get; set; }
        public virtual Product Product { get; set; }
    }
}