using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace _2Sport_BE.Service.DTOs
{
    public class GuestDTO
    {
        public string? FullName { get; set; }
        public string? Email { get; set; }
        public string? PhoneNumber { get; set; }
        public string? Address { get; set; }
    }
    public class GuestCM : GuestDTO
    {

    }
    public class GuestUM : GuestDTO
    {

    }
    public class GuesVM : GuestDTO
    {

    }
}
