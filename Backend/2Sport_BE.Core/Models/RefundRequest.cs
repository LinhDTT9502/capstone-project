using System;
using System.Collections.Generic;
using System.ComponentModel.DataAnnotations.Schema;
using System.ComponentModel.DataAnnotations;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace _2Sport_BE.Repository.Models
{
    [Table("RefundRequests")]
    public class RefundRequest
    {
        [Key]
        [DatabaseGenerated(DatabaseGeneratedOption.Identity)]
        public int RefundID { get; set; }

        public int? SaleOrderID { get; set; }
        public string? SaleOrderCode { get; set; }

        [ForeignKey("SaleOrderID")]
        public virtual SaleOrder SaleOrder { get; set; }
        public int? RentalOrderID { get; set; }
        public string? RentalOrderCode { get; set; }
        [ForeignKey("RentalOrderID")]
        public virtual RentalOrder RentalOrder { get; set; }

        [Column(TypeName = "decimal(18, 0)")]
        public decimal? RefundAmount { get; set; }

        [StringLength(50)]
        public string? RefundMethod { get; set; } 

        [StringLength(100)]
        public string? PaymentGatewayTransactionID { get; set; }

        [StringLength(255)]
        public string Reason { get; set; }

        [StringLength(500)]
        public string Notes { get; set; }

        public int BranchId { get; set; }
        public int? ProcessedBy { get; set; }

        public string? StaffName { get; set; }

        [StringLength(500)]
        public string? StaffNotes { get; set; }

        [StringLength(50)]
        public string? Status { get; set; }
        public DateTime CreatedAt { get; set; }
        public DateTime? UpdatedAt { get; set; }
            
        public bool IsAgreementAccepted { get; set; } = true;
    }
}
