using _2Sport_BE.Repository.Models;
using System.ComponentModel.DataAnnotations.Schema;
using System.ComponentModel.DataAnnotations;

namespace _2Sport_BE.ViewModels
{
    public class BlogDTO
    {
        [Required]
        public string Title { get; set; }

        [Required]
        public string Content { get; set; }
    }

    public class BlogVM : BlogDTO
    {
        public int BlogId { get; set; }
        public DateTime CreateAt { get; set; }
        public int CreatedByStaffId { get; set; }
        public string CreatedByStaffName { get; set; }
        public string CreatedByStaffFullName { get; set; }
        public int EditedByStaffId { get; set; }
        public string EditedByStaffName { get; set; }
        public string EditedByStaffFullName { get; set; }
        public bool Status { get; set; }
    }

    public class BlogCM : BlogDTO
    {

    }

    public class BlogUM : BlogDTO
    {

    }
}
