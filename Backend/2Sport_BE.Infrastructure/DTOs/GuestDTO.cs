using System;
using System.Collections.Generic;
using System.ComponentModel.DataAnnotations;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace _2Sport_BE.Service.DTOs
{
    public class GuestDTO
    {
        [Required(ErrorMessage = "FullName is required")]
        public string FullName { get; set; }
        
        [Required(ErrorMessage = "Email is required")]
        [EmailAddress(ErrorMessage = "Email Address is invalid format")]
        public string Email { get; set; }

        [Required(ErrorMessage = "Mobile no. is required")]
        [RegularExpression("^(?!0+$)(\\+\\d{1,3}[- ]?)?(?!0+$)\\d{10,15}$", ErrorMessage = "Please enter valid phone no.")]
        public string PhoneNumber { get; set; }

        [Required]
        public string Address { get; set; }
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
