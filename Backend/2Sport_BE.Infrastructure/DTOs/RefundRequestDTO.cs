using Microsoft.AspNetCore.Http;
using System;
using System.Collections.Generic;
using System.ComponentModel.DataAnnotations.Schema;
using System.ComponentModel.DataAnnotations;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace _2Sport_BE.Service.DTOs
{
    public class RefundRequestCM
    {
        public string OrderCode { get; set; }
        public int OrderType { get; set; }
        public string Reason { get; set; }
        //public IFormFile[]? Images { get; set; }
        public string? Notes { get; set; }
        public bool IsAgreementAccepted { get; set; }

    }
    public class RefundRequestUM
    {
        public int RefundRequestId { get; set; }

        public decimal RefundAmount { get; set; }

        public string RefundMethod { get; set; }

        public string? PaymentGatewayTransactionID { get; set; }

        public int? ProcessedBy { get; set; }
        public string StaffName { get; set; }
        public string StaffNotes { get; set; }
        public string? Status { get; set; }

    }
    public class RefundRequestVM
    {
        public int RefundID { get; set; }
        public int SaleOrderID { get; set; }
        public int RentalOrderID { get; set; }
        public string? SaleOrderCode { get; set; }
        public string RentalOrderCode { get; set; }
        public string Reason { get; set; }
        public string? Status { get; set; }
        public int BranchId { get; set; }
        public string StaffNotes { get; set; }
        public string ProcessedBy { get; set; }
        public DateTime CreatedAt { get; set; }
        public DateTime? UpdatedAt { get; set; }

    }
}
