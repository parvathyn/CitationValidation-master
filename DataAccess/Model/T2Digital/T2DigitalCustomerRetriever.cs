using System.Configuration;

namespace DataAccess.Model.T2Digital
{
    public class T2DigitalCustomerRetriever
    {
        public static T2DigitalCustomerRetrieverSection _Config = ConfigurationManager.GetSection("t2digital") as T2DigitalCustomerRetrieverSection;
        public static T2DigitalCustomerElementCollection GetCustomers()
        {
            return _Config.Customers;
        }

        public static T2DigitalCustomerElement GetCustomer(int customerId)
        {
            foreach (T2DigitalCustomerElement item in _Config.Customers)
            {
                if (item.CustomerId == customerId.ToString())
                    return item;
            }
            return null;
        }
    }
}
