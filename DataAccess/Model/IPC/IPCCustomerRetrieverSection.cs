using System.Configuration;

namespace DataAccess.Model.IPC
{

    public class IPCCustomerRetrieverSection : ConfigurationSection
    {
        [ConfigurationProperty("customers")]
        public IPCCustomerElementCollection Customers
        {
            get { return (IPCCustomerElementCollection)this["customers"]; }
        }
    }
}
