using _2Sport_BE.Repository.Models;

namespace _2Sport_BE.Service.DTOs
{
    public class WarehouseDTO
    {
        public Product Product { get; set; }
        public string ProductCode { get; set; }
        public string ProductName { get; set; }
        public int TotalQuantity { get; set; }
        public int AvailableQuantity { get; set; }
    }
}