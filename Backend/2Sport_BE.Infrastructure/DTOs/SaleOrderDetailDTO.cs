using System;
using System.Collections.Generic;
using System.ComponentModel.DataAnnotations.Schema;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace _2Sport_BE.Infrastructure.DTOs
{
    public class SaleOrderDetailDTO
    {
        public int? WarehouseId { get; set; }
        public int? Quantity { get; set; }
    }
    public class SaleOrderDetailCM : SaleOrderDetailDTO
    {
    }
    public class SaleOrderDetailUM : SaleOrderDetailDTO
    {
    }
    public class SaleOrderDetailVM
    {
        public int? ProductId { get; set; }
        public string? ProductName { get; set; }
        public decimal? Price { get; set; }
        public int? Quantity { get; set; }
        public int? BranchId { get; set; }
        public string? BranchName { get; set; }
        public DateTime CreatedAt { get; set; }
        public decimal? TotalPrice { get; set; }
    }
}
