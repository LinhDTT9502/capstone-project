using System.ComponentModel.DataAnnotations;
using System.ComponentModel.DataAnnotations.Schema;

namespace _2Sport_BE.Repository.Models
{
    [Table("CartItems")]
    public class CartItem
    {
        [Key]
        [DatabaseGenerated(DatabaseGeneratedOption.Identity)]
        public int Id { get; set; }
        public int? ProductId { get; set; }
        public int? Quantity { get; set; }
        public decimal? Price { get; set; }
        public int? CartId { get; set; }

        public virtual Cart Cart { get; set; }
        public virtual Product Product { get; set; }
    }
}