using System.Configuration;

namespace DataAccess.Model.Parkeon
{

    public class ParkeonCustomerRetrieverSection : ConfigurationSection
    {
        [ConfigurationProperty("customers")]
        public ParkeonCustomerElementCollection Customers
        {
            get { return (ParkeonCustomerElementCollection)this["customers"]; }
        }
    }
}
