using System;
using System.Collections.Generic;
using System.ComponentModel.DataAnnotations;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace _2Sport_BE.Service.DTOs
{
    public class EmployeeDTO
    {
        [StringLength(100)]
        public string? FullName { get; set; }

        [StringLength(10)]
        public string? Gender { get; set; }

        [StringLength(15)]
        public string? Phone { get; set; }

        [StringLength(200)]
        public string? Address { get; set; }

        [DataType(DataType.DateTime)]
        public DateTime? DOB { get; set; }
        public string? AvatarUrl { get; set; }

        public bool IsActive { get; set; }

        public int? RoleId { get; set; }

        // Fields from EmployeeDetail Table
        public int? BranchId { get; set; }

        [DataType(DataType.DateTime)]
        public DateTime? HireDate { get; set; }

        [StringLength(50)]
        public string? Position { get; set; }
        public int? SupervisorId { get; set; }
    }
    public class EmployeeCM : EmployeeDTO
    {
        [Required]
        [StringLength(50)]
        public string UserName { get; set; }

        [Required]
        [StringLength(100)]
        public string Password { get; set; }

        [Required]
        [StringLength(100)]
        public string Email { get; set; }
    }
    public class EmployeeVM : EmployeeDTO
    {
        public int EmployeeId { get; set; }

        public string? UserName { get; set; }

        public string? Email { get; set; }

        public DateTime CreatedDate { get; set; }

        public DateTime? LastUpdate { get; set; }
    }

    public class EmployeeUM : EmployeeDTO
    {
    }
}
