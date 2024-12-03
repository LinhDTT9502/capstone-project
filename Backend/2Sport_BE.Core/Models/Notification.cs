using System.ComponentModel.DataAnnotations;
using System.ComponentModel.DataAnnotations.Schema;

namespace _2Sport_BE.Repository.Models
{
    [Table("Notifications")]
    public class Notification
    {
        [Key]
        [DatabaseGenerated(DatabaseGeneratedOption.Identity)]
        public int Id { get; set; }

        [Column("UserId")]
        public int? UserId { get; set; }

        public virtual User User { get; set; }

        [Column("Message", TypeName = "nvarchar")]
        [MaxLength(500)]
        public string? Message { get; set; }

        [Column("IsRead")]
        public bool IsRead { get; set; } = false;

        [Column("Type")]
        public string? Type { get; set; }

        [Column("ReadAt")]
        public DateTime? ReadAt { get; set; }  // Thay đổi tên cột

        [Column("CreatedAt")]
        public DateTime CreatedAt { get; set; } = DateTime.UtcNow;  // Thay đổi tên cột
    }

}
