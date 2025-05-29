using System.Configuration;

namespace DataAccess.Model.PassportMonitoring
{
    public class PassportMonitoringCustomerRetriever
    {
        public static PassportMonitoringCustomerRetrieverSection _Config = ConfigurationManager.GetSection("passportMonitoring") as PassportMonitoringCustomerRetrieverSection;
        public static PassportMonitoringCustomerElementCollection GetCustomers()
        {
            return _Config.Customers;
        }

        public static PassportMonitoringCustomerElement GetCustomer(int customerId)
        {
            foreach (PassportMonitoringCustomerElement item in _Config.Customers)
            {
                if (item.CustomerId == customerId.ToString())
                    return item;
            }
            return null;
        }
    }
}
