using _2Sport_BE.Repository.Models;
using System.ComponentModel.DataAnnotations.Schema;
using System.ComponentModel.DataAnnotations;

namespace _2Sport_BE.ViewModels
{
    public class ImagesVideoDTO
    {
        public string? ImageUrl { get; set; }

        public string? VideoUrl { get; set; }

        public DateTime? CreateAt { get; set; }

    }
    public class ImagesVideoVM : ImagesVideoDTO
    {
        public int Id { get; set; }
        public int? BlogId { get; set; }
        public int? ProductId { get; set; }
        public string? ProductName { get; set; }
    }
}
