using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace _2Sport_BE.Service.DTOs
{
    public class OrderDetailDTO
    {
        public int? ProductID { get; set; }
        public int? Quantity { get; set; }
        public decimal Price { get; set; }
        public int? WarehouseId { get; set; }
    }
    public class OrderDetailCM : OrderDetailDTO
    {
    }
    public class OrderDetailUM : OrderDetailDTO
    {
    }
    public class OrderDetailVM : OrderDetailDTO
    {
    }
}
