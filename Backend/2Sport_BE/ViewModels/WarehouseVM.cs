using _2Sport_BE.Repository.Models;

namespace _2Sport_BE.ViewModels
{
    public class WarehouseDTO
    {
        public int? ProductId { get; set; }
        public int? Quantity { get; set; }
    }
    public class WarehouseVM : WarehouseDTO
    {
        public int Id { get; set; }
        public string? ProductName { get; set; }
    }

    public class WarehouseCM : WarehouseDTO
    {
    }

    public class WarehouseUM : WarehouseDTO
    {

    }
}
