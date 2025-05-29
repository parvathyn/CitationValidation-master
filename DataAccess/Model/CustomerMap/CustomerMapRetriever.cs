using System.Configuration;
namespace DataAccess.Model.CustomerMap
{
    public class CustomerMapRetriever
    {
        public static CustomerMapRetrieverSection _Config = ConfigurationManager.GetSection("CustomerMap") as CustomerMapRetrieverSection;
        public static CustomerMapElementCollection GetCustomers()
        {
            return _Config.Customers;
        }

        public static CustomerMapElement GetCustomer(int customerId)
        {
            foreach (CustomerMapElement item in _Config.Customers)
            {
                if (item.CustomerId == customerId.ToString())
                    return item;
            }
            return null;
        }
    }
}
