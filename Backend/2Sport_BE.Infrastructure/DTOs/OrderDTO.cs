using System;
using System.Collections.Generic;
using System.ComponentModel.DataAnnotations.Schema;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace _2Sport_BE.Service.DTOs
{
    public class OrderDTO
    {
        public List<OrderDetailCM> orderDetailCMs;
    }
    public class OrderCM : OrderDTO
    {
        public int userID { get; set; }
        public int shipmentDetailID { get; set; }
        public int paymentMethodID { get; set; }
        public string? discountCode { get; set; }
    }
    public class OrderUM : OrderDTO
    {
       
    }
    public class OrderVM : OrderDTO
    {
        public int OrderID { get; set; }
        public string? OrderCode { get; set; }
        public DateTime? CreateDate { get; set; }
        public string? CustomerName { get; set; }
        public string? Status { get; set; }
        public string? IntoMoney { get; set; }
        public string? TotalPrice { get; set; }
        public int? PaymentMethodId { get; set; }
        public int? ShipmentDetailId { get; set; }
        public string? PaymentLink { get; set; }
    }
}
