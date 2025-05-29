using System.Configuration;

namespace DataAccess.Model.Pango
{
    public class PangoCustomerRetrieverSection : ConfigurationSection
    {
        [ConfigurationProperty("customers")]
        public PangoCustomerElementCollection Customers
        {
            get { return (PangoCustomerElementCollection)this["customers"]; }
        }
    }
}
