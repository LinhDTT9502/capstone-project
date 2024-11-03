namespace _2Sport_BE.Infrastructure.Helpers
{
    public interface IDeliveryMethodService
    {
        string ConvertToDatabaseFormat(string displayValue);
        string ConvertToDisplayFormat(string dbValue);
        List<string> GetAllMethods();
        bool IsValidMethod(string method);
    }

    public class DeliveryMethodService : IDeliveryMethodService
    {
        public const string HomeDelivery = "HOME_DELIVERY";
        public const string StorePickup = "STORE_PICKUP";

        private static readonly List<string> ValidMethods = new List<string>
    {
        HomeDelivery,
        StorePickup
    };
        public bool IsValidMethod(string method)
        {
            return ValidMethods.Contains(method);
        }
        public string ConvertToDatabaseFormat(string displayValue)
        {
            return displayValue.ToUpper().Replace(" ", "_");
        }

        public string ConvertToDisplayFormat(string dbValue)
        {
            return dbValue.Replace("_", " ").ToLower();
        }

        public List<string> GetAllMethods()
        {
            return new List<string>(ValidMethods);
        }
    }

}
