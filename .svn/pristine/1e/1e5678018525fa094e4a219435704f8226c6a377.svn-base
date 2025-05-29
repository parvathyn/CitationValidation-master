using System.Configuration;

namespace DataAccess.Model.PayByPhone
{
    public class CustomerRetrieverSection : ConfigurationSection
    {
        [ConfigurationProperty("customers")]
        public CustomerElementCollection Customers
        {
            get { return (CustomerElementCollection)this["customers"]; }
        }

    }
}
