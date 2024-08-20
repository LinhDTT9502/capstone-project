using System;
using System.Collections.Generic;

namespace _2Sport_BE.Repository.Models
{
    public partial class Supplier
    {
        public Supplier()
        {
            ImportHistories = new HashSet<ImportHistory>();
        }

        public int Id { get; set; }
        public string SupplierName { get; set; }
        public string Location { get; set; }

        public virtual ICollection<ImportHistory> ImportHistories { get; set; }
    }
}
