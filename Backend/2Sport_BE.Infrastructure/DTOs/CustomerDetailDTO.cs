using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace _2Sport_BE.Service.DTOs
{
    public class CustomerDetailDTO
    {
        public int? UserId { get; set; } 
        public int? LoyaltyPoints { get; set; }
        public string? MembershipLevel { get; set; }
        public DateTime? JoinDate { get; set; }
    }
    public class CustomerDetailVM : CustomerDetailDTO
    {

    }
    public class CustomerDetailUM : CustomerDetailDTO
    {

    }
    public class CustomerDetailCM : CustomerDetailDTO
    {

    }
}
