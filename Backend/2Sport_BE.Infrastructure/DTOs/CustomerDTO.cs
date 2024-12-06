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
        public int? UserId { get; set; }

    }
    public class CustomerCM : CustomerDTO
    {

    }
    public class CustomerUM : CustomerDTO
    {
        public int? LoyaltyPoints { get; set; }
        public string MembershipLevel { get; set; }
    }
    public class CustomerVM : CustomerDTO
    {
        public int Id { get; set; }
        public DateTime? JoinedAt { get; set; }
        public int? LoyaltyPoints { get; set; }
        public string MembershipLevel { get; set; }
    }
}
