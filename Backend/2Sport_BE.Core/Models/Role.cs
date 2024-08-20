using System;
using System.Collections.Generic;

namespace _2Sport_BE.Repository.Models
{
    public partial class Role
    {
        public Role()
        {
            Users = new HashSet<User>();
        }

        public int Id { get; set; }
        public string RoleName { get; set; }
        public DateTime? CreatedOn { get; set; }
        public DateTime? UpdatedOn { get; set; }

        public virtual ICollection<User> Users { get; set; }
    }
}
