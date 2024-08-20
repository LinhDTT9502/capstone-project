using System;
using System.Collections.Generic;

namespace _2Sport_BE.Repository.Models
{
    public partial class Order
    {
        public Order()
        {
            OrderDetails = new HashSet<OrderDetail>();
        }

        public int Id { get; set; }
        public string OrderCode { get; set; }
        public int? OrderDetailId { get; set; }
        public int? Status { get; set; }
        public decimal? TotalPrice { get; set; }
        public decimal? TransportFee { get; set; }
        public decimal? IntoMoney { get; set; }
        public int? PaymentMethodId { get; set; }
        public int? ShipmentDetailId { get; set; }
        public DateTime? ReceivedDate { get; set; }
        public int? TransportUnitId { get; set; }
        public int? UserId { get; set; }

        public virtual PaymentMethod PaymentMethod { get; set; }
        public virtual ShipmentDetail ShipmentDetail { get; set; }
        public virtual User User { get; set; }
        public virtual ICollection<OrderDetail> OrderDetails { get; set; }
    }
}
