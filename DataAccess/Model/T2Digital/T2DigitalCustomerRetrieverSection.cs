using System.Configuration;

namespace DataAccess.Model.T2Digital
{
    public class T2DigitalCustomerRetrieverSection : ConfigurationSection
    {
        [ConfigurationProperty("customers")]
        public T2DigitalCustomerElementCollection Customers
        {
            get { return (T2DigitalCustomerElementCollection)this["customers"]; }
        }
    }
}
