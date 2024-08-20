using System;
using System.Collections.Generic;

namespace _2Sport_BE.Repository.Models
{
    public partial class Blog
    {
        public Blog()
        {
            ImagesVideos = new HashSet<ImagesVideo>();
            Likes = new HashSet<Like>();
        }

        public int Id { get; set; }
        public string BlogName { get; set; }
        public string Content { get; set; }
        public DateTime? CreateAt { get; set; }
        public string MainImageName { get; set; }
        public string MainImagePath { get; set; }
        public int? UserId { get; set; }
        public int? SportId { get; set; }

        public virtual Sport Sport { get; set; }
        public virtual ICollection<ImagesVideo> ImagesVideos { get; set; }
        public virtual ICollection<Like> Likes { get; set; }
    }
}
