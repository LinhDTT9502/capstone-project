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
        [Required(ErrorMessage = "FullName is required")]
        public string FullName { get; set; }

        [Required(ErrorMessage = "Gender is required")]
        public string Gender { get; set; }

        [Required(ErrorMessage = "Phone is required")]
        [RegularExpression(@"^(\+84|0)(3|5|7|8|9)([0-9]{8})$", ErrorMessage = "Please enter a valid phone number.")]
        public string Phone { get; set; }

        [Required(ErrorMessage = "Address is required")]
        public string Address { get; set; }

        [Required(ErrorMessage = "FullName is required")]
        [DataType(DataType.DateTime)]
        public DateTime? DOB { get; set; }

        public string? AvatarUrl { get; set; }

        [Required(ErrorMessage = "IsActive is required")]
        public bool IsActive { get; set; }

        [Required(ErrorMessage = "RoleId is required")]
        public int RoleId { get; set; }

        // Fields from EmployeeDetail Table
        [Required]
        public int? BranchId { get; set; }

        [Required(ErrorMessage = "HireDate is required")]
        [DataType(DataType.Date)]
        [Range(typeof(DateTime), "1/1/2000", "12/31/2024", ErrorMessage = "BirthDate must be between {1} and {2}.")]
        public DateTime HireDate { get; set; }

        [Required(ErrorMessage = "Position is required")]
        public string Position { get; set; }

        public int SupervisorId { get; set; }
    }
    public class EmployeeCM : EmployeeDTO
    {
        [Required]
        [StringLength(50)]
        public string UserName { get; set; }

        [Required(ErrorMessage = "Password is required")]
        [StringLength(18, ErrorMessage = "The {0} must be at least {2} characters long.", MinimumLength = 6)]
        [RegularExpression(@"^((?=.*[a-z])(?=.*[A-Z])(?=.*\d)).+$")]
        public string Password { get; set; }

        [Required(ErrorMessage = "Email is required")]
        [EmailAddress(ErrorMessage = "Email Address is invalid format")]
        [StringLength(100, ErrorMessage = "Email cannot be longer than 100 characters.")]
        public string Email { get; set; }
    }
    public class EmployeeVM : EmployeeDTO
    {
        public int EmployeeId { get; set; }

        public string UserName { get; set; }

        public string Email { get; set; }

        public DateTime CreatedDate { get; set; }

        public DateTime LastUpdate { get; set; }
    }

    public class EmployeeUM : EmployeeDTO
    {
    }
}
