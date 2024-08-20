using AutoMapper.Configuration.Annotations;

namespace _2Sport_BE.ViewModels
{
   
    public class ShipmentDetailDTO
    {
        public string FullName { get; set; }
        public string Address { get; set; }
        public string PhoneNumber { get; set; }

    }
    public class ShipmentDetailCM : ShipmentDetailDTO
    {

    }
    public class ShipmentDetailVM : ShipmentDetailDTO
    {
        public int Id { get; set; }
        [Ignore]
        public int UserId { get; set; }
    }
    public class ShipmentDetailUM : ShipmentDetailDTO
    {

    }
}
