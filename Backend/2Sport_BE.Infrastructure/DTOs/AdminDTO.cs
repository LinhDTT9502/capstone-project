using System;
using System.Collections.Generic;
using System.ComponentModel.DataAnnotations;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace _2Sport_BE.Infrastructure.DTOs
{
    public class AdminDTO
    {
        [Required(ErrorMessage = "Username is required")]
        public string Username { get; set; }

        [Required(ErrorMessage = "FullName is required")]
        public string FullName { get; set; }

        [Required(ErrorMessage = "Email is required")]
        public string Email { get; set; }
    }
    public class AdminCM : AdminDTO
    {
        [Required(ErrorMessage = "Password is required")]
        public string Password { get; set; }
    }
    public class AdminUM : AdminDTO
    {
        [Required(ErrorMessage = "Password is required")]
        public string Password { get; set; }
    }
    public class AdminVM : AdminDTO
    {
        public int AdminId { get; set; } 
    }
}
