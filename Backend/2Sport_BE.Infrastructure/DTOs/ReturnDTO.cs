using _2Sport_BE.Repository.Models;
using Microsoft.AspNetCore.Http;
using System;
using System.Collections.Generic;
using System.ComponentModel.DataAnnotations;
using System.ComponentModel.DataAnnotations.Schema;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace _2Sport_BE.Service.DTOs
{
    public class ReturnRequestDTO
    {
        [Required]
        public int OrderId { get; set; }

        [Required]
        public string ProductCode { get; set; }

        public string? Size { get; set; }

        public string? Color { get; set; }

        public int? Condition { get; set; }

        [Required]
        public string Reason { get; set; }

        public string Notes { get; set; }

        public decimal? ReturnAmount { get; set; }

        public IFormFile Video { get; set; }
    }
    public class ReturnResponseDto
    {
        public int ReturnId { get; set; }
        public int OrderId { get; set; }
        public string ProductCode { get; set; }
        public string? Size { get; set; }
        public string? Color { get; set; }
        public int? Condition { get; set; }
        public string Status { get; set; } // PENDING, APPROVED, REJECTED
        public DateTime CreatedAt { get; set; }
    }
    public class ReturnRequestVM
    {
        public int ReturnID { get; set; }
        public int? SaleOrderID { get; set; }
        public int? RentalOrderID { get; set; }
        public string? ProductCode { get; set; }
        public string? Size { get; set; }
        public string? Color { get; set; }
        public int? Condition { get; set; }
        public decimal? ReturnAmount { get; set; }
        public string Reason { get; set; }
        public string Notes { get; set; }
        public int BranchId { get; set; }
        public int? ProcessedBy { get; set; }
        public string? VideoUrl { get; set; }
        public string? Status { get; set; }
        public DateTime CreatedAt { get; set; }
        public DateTime? UpdatedAt { get; set; }

    }
    public class ReturnRequestUM
    {
        public string? ProductCode { get; set; }
        public string? Size { get; set; }
        public string? Color { get; set; }
        public int? Condition { get; set; }
        public decimal? ReturnAmount { get; set; }
        public string Reason { get; set; }
        public string Notes { get; set; }
        public int? ProcessedBy { get; set; }
        public string? VideoUrl { get; set; }
        public string? Status { get; set; }
        public DateTime? UpdatedAt { get; set; }

    }
}
