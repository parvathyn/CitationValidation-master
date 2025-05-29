using System.Configuration;

namespace DataAccess.Model.AceParking
{
    public class AceParkingCustomerRetrieverSection : ConfigurationSection
    {
        [ConfigurationProperty("customers")]
        public AceParkingCustomerElementCollection Customers
        {
            get { return (AceParkingCustomerElementCollection)this["customers"]; }
        }
    }
}
