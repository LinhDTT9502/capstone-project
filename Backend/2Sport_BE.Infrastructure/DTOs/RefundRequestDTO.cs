using Microsoft.AspNetCore.Http;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace _2Sport_BE.Service.DTOs
{
    public class RefundRequestDTO
    {
        
        public decimal RefundAmount { get; set; }
       
    }
    public class RefundRequestCM
    {
        public string CustomerName { get; set; }
        public string CustomerEmail { get; set; }
        public string CustomerPhone { get; set; }
        public string OrderCode { get; set; }
        public int OrderType { get; set; }
        public string OrderDate { get; set; }
        public string Reason { get; set; }
        public IFormFile[]? Images { get; set; }
        public string PaymentMethod { get; set; }
        public string Notes { get; set; }
        public bool IsAgreementAccepted { get; set; }

    }
    public class RefundRequestUM : RefundRequestDTO
    {
        public DateTime RefundDate { get; set; }
        public string RefundMethod { get; set; }
        public string? PaymentGatewayTransactionID { get; set; }
        public int? ProcessedBy { get; set; }//Có branch sẽ trích xuất được staff
        public string StaffName { get; set; }
    }
    public class RefundRequestVM : RefundRequestDTO
    {
        public int RefundID { get; set; }
        public int SaleOrderID { get; set; }
        public int RentalOrderID { get; set; }
        public string? SaleOrderCode { get; set; }
        public string RentalOrderCode { get; set; }
        public string? Status { get; set; }
        public int BranchId { get; set; }

    }
}
