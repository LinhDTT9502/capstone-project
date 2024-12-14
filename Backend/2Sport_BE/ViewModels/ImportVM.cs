using _2Sport_BE.Repository.Models;
using NuGet.Protocol.Core.Types;

namespace _2Sport_BE.ViewModels
{
    public class ImportDTO
    {
        public string Action { get; set; }
        public string? ProductName { get; set; }
        public string? ProductCode { get; set; }
        public decimal Price { get; set; }
        public decimal RentPrice { get; set; }
        public string Color { get; set; }
        public string Size { get; set; }
        public int Condition { get; set; }
        public int Quantity { get; set; }
        public int ManagerId { get; set; }

    }
    public class ImportVM : ImportDTO
    {
        public int Id { get; set; }

        public DateTime? ImportDate { get; set; }
        public string ManagerName { get; set; }
    }

    public class ImportCM : ImportDTO
    {
        public int? ProductId { get; set; }
    }

    public class ImportUM : ImportDTO
    {
        public int? ProductId { get; set; }

    }
}
