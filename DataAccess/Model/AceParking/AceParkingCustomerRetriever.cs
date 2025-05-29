using System.Configuration;

namespace DataAccess.Model.AceParking
{
   

    public class AceParkingCustomerRetriever
    {
        public static AceParkingCustomerRetrieverSection _Config = ConfigurationManager.GetSection("aceparking") as AceParkingCustomerRetrieverSection;
        public static AceParkingCustomerElementCollection GetCustomers()
        {
            return _Config.Customers;
        }

        public static AceParkingCustomerElement GetCustomer(int customerId)
        {
            foreach (AceParkingCustomerElement item in _Config.Customers)
            {
                if (item.CustomerId == customerId.ToString())
                    return item;
            }
            return null;
        }
    }
}
