using System.Configuration;

namespace DataAccess.Model.CustomerMap
{
    public class CustomerMapRetrieverSection : ConfigurationSection
    {
        [ConfigurationProperty("customers")]
        public CustomerMapElementCollection Customers
        {
            get { return (CustomerMapElementCollection)this["customers"]; }
        }
    }
}
