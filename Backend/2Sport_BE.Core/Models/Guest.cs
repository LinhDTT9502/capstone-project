/*using System;
using System.Collections.Generic;
using System.ComponentModel.DataAnnotations;
using System.ComponentModel.DataAnnotations.Schema;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace _2Sport_BE.Repository.Models
{
    [Table("Guests")]
    public class Guest
    {
        [Key]
        [DatabaseGenerated(DatabaseGeneratedOption.Identity)]
        public int Id { get; set; }
        [Column("Email", TypeName = "nvarchar")]
        [MaxLength(255)]
        public string? Email { get; set; }
        [Column("FullName", TypeName = "nvarchar")]
        [MaxLength(255)]
        public string? FullName { get; set; }
        [Column("Address", TypeName = "nvarchar")]
        [MaxLength(500)]
        public string? Address { get; set; }
        [Column("PhoneNumber", TypeName = "nvarchar")]
        [MaxLength(255)]
        public string? PhoneNumber { get; set; }

        //create indexer
        public object this[string data]
        {
            get
            {
                if (data.ToUpper() == "EMAIL")
                    return Email;
                else if (data.ToUpper() == "FULLNAME")
                    return FullName;
                else if (data.ToUpper() == "ADDRESS")
                    return Address;
                else if (data.ToUpper() == "PHONENUMBER")
                    return PhoneNumber;
                else return null;
            }
            set
            {
                if (data.ToUpper() == "EMAIL")
                    Email = value.ToString();
                else if (data.ToUpper() == "FULLNAME")
                    FullName = value.ToString();
                else if (data.ToUpper() == "ADDRESS")
                    Address = value.ToString();
                else if (data.ToUpper() == "PHONENUMBER")
                    PhoneNumber = value.ToString();
            }
        }
        
    }
}
*/