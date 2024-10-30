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
        public int UserId { get; set; }
        public string UserName { get; set; }
        public string FullName { get; set; }
    }

    public class BlogCM : BlogDTO
    {

    }

    public class BlogUM : BlogDTO
    {

    }
}
