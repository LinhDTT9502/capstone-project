using System;
using System.Collections.Generic;
using System.ComponentModel.DataAnnotations;
using System.ComponentModel.DataAnnotations.Schema;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace _2Sport_BE.Repository.Models
{
    [Table("Bookmarks")]
    public class Bookmark
    {
        [Key]
        [DatabaseGenerated(DatabaseGeneratedOption.Identity)]
        public Guid Id { get; set; }

        public int UserId { get; set; }

        public int BlogId { get; set; }

        public virtual User User { get; set; }

        public virtual Blog Blog { get; set; }
    }
}
