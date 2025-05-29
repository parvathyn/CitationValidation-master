using System.Configuration;

namespace DataAccess.Model.MiExchange
{
   public class MiExCustomerRetriever
    {
       public static MiExCustomerRetrieverSection _Config = ConfigurationManager.GetSection("miex") as MiExCustomerRetrieverSection;
        public static MiExCustomerElementCollection GetCustomers()
        {
            return _Config.Customers;
        }

        public static MiExCustomerElement GetCustomer(int customerId)
        {
            foreach (MiExCustomerElement item in _Config.Customers)
            {
                if (item.CustomerId == customerId.ToString())
                    return item;
            }
            return null;
        }
    }
}
