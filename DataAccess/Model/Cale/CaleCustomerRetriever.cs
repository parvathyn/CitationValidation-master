using System.Configuration;

namespace DataAccess.Model.Cale
{
   public class CaleCustomerRetriever
    {
       public static CaleCustomerRetrieverSection _Config = ConfigurationManager.GetSection("cale") as CaleCustomerRetrieverSection;
        public static CaleCustomerElementCollection GetCustomers()
        {
            return _Config.Customers;
        }

        public static CaleCustomerElement GetCustomer(int customerId)
        {
            foreach (CaleCustomerElement item in _Config.Customers)
            {
                if (item.CustomerId == customerId.ToString())
                    return item;
            }
            return null;
        }
    }
}
