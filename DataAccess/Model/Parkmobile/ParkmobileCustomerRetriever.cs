using System.Configuration;

namespace DataAccess.Model.Parkmobile
{
    public class ParkmobileCustomerRetriever
    {
        public static PMCustomerRetrieverSection _Config = ConfigurationManager.GetSection("parkmobile") as PMCustomerRetrieverSection;
        public static PMCustomerElementCollection GetCustomers()
        {
            return _Config.Customers;
        }

        public static PMCustomerElement GetCustomer(int customerId)
        {
            foreach (PMCustomerElement item in _Config.Customers)
            {
                if (item.CustomerId == customerId.ToString())
                    return item;
            }
            return null;
        }
    }
}
