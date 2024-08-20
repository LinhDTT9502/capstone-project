using System;
using System.Collections.Generic;

namespace _2Sport_BE.Repository.Models
{
    public partial class ImagesVideo
    {
        public int Id { get; set; }
        public string ImageName { get; set; }
        public string ImagePath { get; set; }
        public string VideoName { get; set; }
        public string VideoPath { get; set; }
        public int? BlogId { get; set; }
        public int? ProductId { get; set; }

        public virtual Blog Blog { get; set; }
        public virtual Product Product { get; set; }
    }
}
