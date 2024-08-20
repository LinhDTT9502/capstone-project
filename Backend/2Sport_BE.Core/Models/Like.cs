using System;
using System.Collections.Generic;

namespace _2Sport_BE.Repository.Models
{
    public partial class Like
    {
        public int Id { get; set; }
        public int? UserId { get; set; }
        public int? ProductId { get; set; }
        public int? BlogId { get; set; }

        public virtual Blog Blog { get; set; }
        public virtual Product Product { get; set; }
        public virtual User User { get; set; }
    }
}
