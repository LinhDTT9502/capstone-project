using System;
using System.Collections.Generic;

namespace _2Sport_BE.Repository.Models
{
    public partial class RefreshToken
    {
        public int RefreshTokenId { get; set; }
        public string Token { get; set; }
        public string JwtId { get; set; }
        public DateTime? CreateDate { get; set; }
        public DateTime? ExpiryDate { get; set; }
        public bool? Used { get; set; }
        public int? UserId { get; set; }

        public virtual User User { get; set; }
    }
}
