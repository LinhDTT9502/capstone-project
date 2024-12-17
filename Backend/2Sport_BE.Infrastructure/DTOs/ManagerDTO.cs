using System;
using System.Collections.Generic;
using System.ComponentModel.DataAnnotations.Schema;
using System.ComponentModel.DataAnnotations;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace _2Sport_BE.Infrastructure.DTOs
{
    public class ManagerDTO
    {
        public int? UserId { get; set; }
        public int? BranchId { get; set; }
        public DateTime StartDate { get; set; }
    }
    public class ManagerCM : ManagerDTO
    {

    }
    public class ManagerUM : ManagerDTO
    {
        public DateTime? EndDate { get; set; }
    }
    public class ManagerVM : ManagerDTO
    {
        public int Id { get; set; }
        public string BranchName { get; set; }
        public bool IsActive { get; set; }
        public UserVM UserVM { get; set; }
        public DateTime? EndDate { get; set; }

    }
}
