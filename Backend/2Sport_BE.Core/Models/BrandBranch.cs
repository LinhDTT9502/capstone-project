using System;
using System.Collections.Generic;
using System.ComponentModel.DataAnnotations.Schema;
using System.ComponentModel.DataAnnotations;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace _2Sport_BE.Repository.Models
{
    [Table("BrandBranches")]
    public class BrandBranch
    {
        [Key]
        [DatabaseGenerated(DatabaseGeneratedOption.Identity)]
        public int Id { get; set; }

        [Column("BranchId")]
        public int? BranchId { get; set; }

        [Column("BrandId")]
        public int? BrandId { get; set; }

        public virtual Brand Brand { get; set; }

        public virtual Branch Branch { get; set; }
    }

}
