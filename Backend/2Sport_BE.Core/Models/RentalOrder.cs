using System;
using System.Collections.Generic;
using System.ComponentModel.DataAnnotations.Schema;
using System.ComponentModel.DataAnnotations;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace _2Sport_BE.Repository.Models
{
    [Table("RentalOrders")]
    public class RentalOrder
    {
        [Key]
        [DatabaseGenerated(DatabaseGeneratedOption.Identity)]
        public int Id { get; set; }
        public int OrderId { get; set; }

        [DisplayFormat(DataFormatString = "{0:HH-mm-ss:dd-MM-yyyy}", ApplyFormatInEditMode = true)]
        [DataType(DataType.DateTime)]
        public DateTime? RentalStartDate { get; set; }

        [DisplayFormat(DataFormatString = "{0:HH-mm-ss:dd-MM-yyyy}", ApplyFormatInEditMode = true)]
        [DataType(DataType.DateTime)]
        public DateTime? RentalEndDate { get; set; }

        [DisplayFormat(DataFormatString = "{0:HH-mm-ss:dd-MM-yyyy}", ApplyFormatInEditMode = true)]
        [DataType(DataType.DateTime)]
        public DateTime? ReturnDate { get; set; }

        [Column("LateFee", TypeName = "decimal")]
        public decimal LateFee { get; set; }
        public bool IsRestocked { get; set; } // Track if the product has been restocked
        public bool IsInspected { get; set; } // Track if the product is inspected upon return
        [Column("DamageFee", TypeName = "decimal")]
        public decimal? DamageFee { get; set; }
        [ForeignKey("OrderId")]
        public virtual Order Order { get; set; }
    }

}
