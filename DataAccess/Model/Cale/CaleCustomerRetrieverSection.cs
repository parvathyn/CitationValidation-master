using System.Configuration;

namespace DataAccess.Model.Cale
{
   public class CaleCustomerRetrieverSection : ConfigurationSection
    {
       [ConfigurationProperty("customers")]
       public CaleCustomerElementCollection Customers
       {
           get { return (CaleCustomerElementCollection)this["customers"]; }
       }
    }
}
