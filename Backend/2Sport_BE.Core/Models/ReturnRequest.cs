using System;
using System.Collections.Generic;
using System.ComponentModel.DataAnnotations.Schema;
using System.ComponentModel.DataAnnotations;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace _2Sport_BE.Repository.Models
{
    [Table("ReturnRequests")]
    public class ReturnRequest
    {
        [Key]
        [DatabaseGenerated(DatabaseGeneratedOption.Identity)]
        public int ReturnID { get; set; }

        public int? SaleOrderID { get; set; }

        [ForeignKey("SaleOrderID")]
        public virtual SaleOrder SaleOrder { get; set; }
        public int? RentalOrderID { get; set; }

        [ForeignKey("RentalOrderID")]
        public virtual RentalOrder RentalOrder { get; set; }

        [Column("ProductCode")]
        public string? ProductCode { get; set; }

        [Column("Size")]
        public string? Size { get; set; }

        [Column("Color")]
        public string? Color { get; set; }

        [Column("Condition")]
        public int? Condition { get; set; }


        [Column(TypeName = "decimal(18, 0)")]
        public decimal? ReturnAmount { get; set; }

        [StringLength(255)]
        public string Reason { get; set; }

        [StringLength(500)]
        public string Notes { get; set; }

        public int BranchId { get; set; }
        public int? ProcessedBy { get; set; }

        [StringLength(500)]
        public string? VideoUrl { get; set; }

        [StringLength(50)]
        public string? Status { get; set; }
        public DateTime CreatedAt { get; set; }
        public DateTime? UpdatedAt { get; set; }

    }
}
