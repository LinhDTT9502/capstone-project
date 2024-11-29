namespace _2Sport_BE.Infrastructure.DTOs
{
    public class StaffDTO
    {
        public int? UserId { get; set; }
        public int? BranchId { get; set; }
        public int? ManagerId { get; set; }
        public DateTime StartDate { get; set; }
        
        public string Position { get; set; }
    }
   public class StaffCM : StaffDTO
    {

    }
    public class StaffUM : StaffDTO
    {
        public DateTime? EndDate { get; set; }
        public bool? IsActive { get; set; }
    }
    public class StaffVM : StaffDTO
    {
        public int StaffId { get; set; }
        public DateTime? EndDate { get; set; }
        public UserVM UserVM { get; set; }
    }
}
