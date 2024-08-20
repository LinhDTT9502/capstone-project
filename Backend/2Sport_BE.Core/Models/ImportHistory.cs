using System;
using System.Collections.Generic;

namespace _2Sport_BE.Repository.Models
{
    public partial class ImportHistory
    {
        public int Id { get; set; }
        public int? ProductId { get; set; }
        public DateTime? ImportDate { get; set; }
        public int? Quantity { get; set; }
        public int? SupplierId { get; set; }
        public string LotCode { get; set; }
        public string ImportCode { get; set; }

        public virtual Product Product { get; set; }
        public virtual Supplier Supplier { get; set; }
    }
}
