using _2Sport_BE.Repository.Models;
using System.ComponentModel.DataAnnotations.Schema;
using System.ComponentModel.DataAnnotations;

namespace _2Sport_BE.ViewModels
{
    public class BranchDTO
    {
        [Required]
        [MaxLength(50)]
        public string BranchName { get; set; }

        [Required]
        [MaxLength(255)]
        public string Location { get; set; }

        [Required]
        [Phone]
        public string Hotline { get; set; }


    }
    public class BranchVM : BranchDTO
    {
        public int Id { get; set; }
        public bool? Status { get; set; }
        public string ImgAvatarPath { get; set; }

    }

	public class BranchCM : BranchDTO
    {
        public IFormFile? ImageURL { get; set; }

    }

    public class BranchUM : BranchDTO
    {
        public IFormFile? ImageURL { get; set; }

    }

}
