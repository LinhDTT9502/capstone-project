using System.ComponentModel.DataAnnotations;
using System.ComponentModel.DataAnnotations.Schema;

namespace _2Sport_BE.Repository.Models
{
    [Table("Managers")]
    public class Manager
    {
        [Key]
        [DatabaseGenerated(DatabaseGeneratedOption.Identity)]
        public int Id { get; set; }

        [Column("UserId")]
        public int? UserId { get; set; }

        [Column("BranchId")]
        public int? BranchId { get; set; }

        [Column("StartDate")]
        [DisplayFormat(DataFormatString = "{0:HH-mm-ss:dd-MM-yyyy}", ApplyFormatInEditMode = true)]
        [DataType(DataType.DateTime)]
        public DateTime StartDate { get; set; }

        [Column("EndDate")]
        [DisplayFormat(DataFormatString = "{0:HH-mm-ss:dd-MM-yyyy}", ApplyFormatInEditMode = true)]
        [DataType(DataType.DateTime)]
        public DateTime? EndDate { get; set; }

        public virtual Branch Branch { get; set; }
        public virtual User User { get; set; }
    }
}