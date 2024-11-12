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
        public string SubTitle { get; set; }

        [Required]
        public string Content { get; set; }
    }

    public class BlogVM : BlogDTO
    {
        public int BlogId { get; set; }
        public string CoverImgPath { get; set; }
        public int CreatedByStaffId { get; set; }
        public string CreatedByStaffName { get; set; }
        public string CreatedByStaffFullName { get; set; }
        public int EditedByStaffId { get; set; }
        public string EditedByStaffName { get; set; }
        public string EditedByStaffFullName { get; set; }
        public int Likes { get; set; }
        public bool Status { get; set; }
    }

    public class BlogCM : BlogDTO
    {
        public IFormFile? CoverImage { get; set; }
    }

    public class BlogUM : BlogDTO
    {
        public IFormFile? CoverImage { get; set; }

    }
}
