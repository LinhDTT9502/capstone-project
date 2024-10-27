using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace _2Sport_BE.Infrastructure.DTOs
{
    public class PaymentDTO
    {
        public string? Status { get; set; }
        public string? Code { get; set; }
        public string? Id { get; set; }
        public bool Cancel { get; set; }
        public string? OrderCode { get; set; }
    }
    public class PaymentResponse : PaymentDTO
    {
      
    }
}
