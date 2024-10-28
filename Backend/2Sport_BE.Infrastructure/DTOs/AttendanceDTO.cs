using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace _2Sport_BE.Infrastructure.DTOs
{
    public class AttendanceDTO
    {
        public int EmployeeId { get; set; }
        public int BranchId { get; set; }

        public DateTime AttendanceDate { get; set; }

        public string Status { get; set; } // Trạng thái (ví dụ: "Present", "Absent", "Late")

        public string? Reason { get; set; } // Lý do vắng mặt

        public int CheckedBy { get; set; }
    }
    public class AttendanceCM : AttendanceDTO
    {

    }
    public class AttendanceUM : AttendanceDTO
    {

    }
    public class AttendanceVM : AttendanceDTO
    {
        public string EmployeeName { get; set; }
        public string ManagerName { get; set; }
        public string BranchName { get; set; }

    }

}
