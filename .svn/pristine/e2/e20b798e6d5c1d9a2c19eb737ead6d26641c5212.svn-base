using System.Configuration;

namespace DataAccess.Model.PayByPhone
{
    public class CustomerRetriever
    {
        public static CustomerRetrieverSection _Config = ConfigurationManager.GetSection("payByPhone") as CustomerRetrieverSection;
        public static CustomerElementCollection GetCustomers()
        {
            return _Config.Customers;
        }

        public static CustomerElement GetCustomer(int customerId)
        {
            foreach (CustomerElement item in _Config.Customers)
            {
                if (item.CustomerId == customerId.ToString())
                    return item;
            }
            return null;
        }
    }
}
