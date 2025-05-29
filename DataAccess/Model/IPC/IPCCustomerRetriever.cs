using System.Configuration;

namespace DataAccess.Model.IPC
{

    public class IPCCustomerRetriever
    {
        public static IPCCustomerRetrieverSection _Config = ConfigurationManager.GetSection("ipc") as IPCCustomerRetrieverSection;
        public static IPCCustomerElementCollection GetCustomers()
        {
            return _Config.Customers;
        }

        public static IPCCustomerElement GetCustomer(int customerId)
        {
            foreach (IPCCustomerElement item in _Config.Customers)
            {
                if (item.CustomerId == customerId.ToString())
                    return item;
            }
            return null;
        }
    }
}
