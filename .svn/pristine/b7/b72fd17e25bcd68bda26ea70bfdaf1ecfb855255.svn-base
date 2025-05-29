using System.Configuration;

namespace DataAccess.Model.PassportMonitoring
{
    public class PassportMonitoringCustomerRetrieverSection : ConfigurationSection
    {
        [ConfigurationProperty("customers")]
        public PassportMonitoringCustomerElementCollection Customers
        {
            get { return (PassportMonitoringCustomerElementCollection)this["customers"]; }
        }

    }
}
