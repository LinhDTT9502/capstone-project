using _2Sport_BE.Repository.Models;

namespace _2Sport_BE.ViewModels
{
    public class ImportDTO
    {

        public int? Quantity { get; set; }

    }
    public class ImportVM : ImportDTO
    {
        public int Id { get; set; }
        public string? ProductName { get; set; }
        public string? SupplierName { get; set; }
        public DateTime? ImportDate { get; set; }
        public string LotCode { get; set; }
        public string ImportCode { get; set; }
    }

    public class ImportCM : ImportDTO
    {
        public int? ProductId { get; set; }
        public int? SupplierId { get; set; }
    }

    public class ImportUM : ImportDTO
    {
        public int? ProductId { get; set; }
        public int? SupplierId { get; set; }

    }
}
