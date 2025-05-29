using System.Configuration;


namespace DataAccess.Model.Parkmobile
{
    public class PMCustomerRetrieverSection : ConfigurationSection
    {
        [ConfigurationProperty("customers")]
        public PMCustomerElementCollection Customers
        {
            get { return (PMCustomerElementCollection)this["customers"]; }
        }
    }
}
