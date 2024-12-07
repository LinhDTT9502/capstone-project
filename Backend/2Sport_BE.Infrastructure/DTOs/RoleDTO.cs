using System;
using System.Collections.Generic;
using System.ComponentModel.DataAnnotations;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace _2Sport_BE.Infrastructure.DTOs
{
    public class RoleDTO
    {
        [Required(ErrorMessage ="RoleName is required!")]
        public string RoleName { get; set; }
        [Required(ErrorMessage = "Description is required!")]
        public string Description { get; set; }

    }
    public class RoleCM : RoleDTO
    {
    }
    public class RoleUM : RoleDTO
    {
    }
    public class RoleVM : RoleDTO
    {
        public int RoleId { get; set; }
        [Required(ErrorMessage = "ErrorMessage is required!")]
        public DateTime CreatedDate { get; set; }
    }
}
