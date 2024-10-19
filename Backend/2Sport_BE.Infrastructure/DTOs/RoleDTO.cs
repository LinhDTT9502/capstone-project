using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace _2Sport_BE.Service.DTOs
{
    public class RoleDTO
    {
        public string RoleName { get; set; }
        public string Description { get; set; }
        public DateTime CreatedDate { get; set; }
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
    }
}
