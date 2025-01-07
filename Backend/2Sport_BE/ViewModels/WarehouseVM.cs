using _2Sport_BE.Repository.Models;

namespace _2Sport_BE.ViewModels
{
    public class WarehouseDTO
    {
        public int? ProductId { get; set; }
    }
    public class WarehouseVM : WarehouseDTO
    {
        public int Id { get; set; }
        public string? BranchName { get; set; }
        public string? ProductName { get; set; }
        public string? ProductCode { get; set; }
        public string? Color { get; set; }
        public string? Size { get; set; }
        public int? Condition { get; set; }
        public string? ImgAvatarPath { get; set; }
        public int? TotalQuantity{ get; set; }
        public int? AvailableQuantity { get; set; }
    }

    public class WarehouseCM : WarehouseDTO
    {
    }

    public class WarehouseUM : WarehouseDTO
    {

    }
}
