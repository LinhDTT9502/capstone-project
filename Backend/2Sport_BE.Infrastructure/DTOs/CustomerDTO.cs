using System;
using System.Collections.Generic;
using System.ComponentModel.DataAnnotations;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace _2Sport_BE.Infrastructure.DTOs
{
    public class CustomerDTO
    {
        public int Id { get; set; }
        public int? UserId { get; set; } 
        public int? LoyaltyPoints { get; set; }
        public string MembershipLevel { get; set; }

        [DisplayFormat(DataFormatString = "{0:HH-mm-ss:dd-MM-yyyy}", ApplyFormatInEditMode = true)]
        [DataType(DataType.DateTime)]
        public DateTime? JoinedAt { get; set; }
    }
    public class CustomerCM : CustomerDTO
    {

    }
    public class CustomerUM : CustomerDTO
    {

    }
    public class CustomerVM : CustomerDTO
    {

    }
}
