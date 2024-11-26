using System;
using System.Collections.Generic;
using System.ComponentModel.DataAnnotations;
using System.ComponentModel.DataAnnotations.Schema;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace _2Sport_BE.Repository.Models
{
    [Table("TwilioAccounts")]
    public class TwilioAccount
    {
        [Key]
        [DatabaseGenerated(DatabaseGeneratedOption.Identity)]
        public int Id { get; set; }

        [Column("AccountSId", TypeName = "nvarchar")]
        [MaxLength(255)]
        public string AccountSId{ get; set; }

        [Column("AuthToken", TypeName = "nvarchar")]
        [MaxLength(255)]
        public string AuthToken { get; set; }

        [Column("FromNumber", TypeName = "nvarchar")]
        [MaxLength(15)]
        public string FromNumber { get; set; }

        [Column("ToNumber", TypeName = "nvarchar")]
        [MaxLength(15)]
        public string ToNumber { get; set; }


    }
}
