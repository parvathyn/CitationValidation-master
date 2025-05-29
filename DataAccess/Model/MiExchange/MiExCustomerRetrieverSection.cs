using System.Configuration;

namespace DataAccess.Model.MiExchange
{
   public class MiExCustomerRetrieverSection : ConfigurationSection
    {
       [ConfigurationProperty("customers")]
       public MiExCustomerElementCollection Customers
       {
           get { return (MiExCustomerElementCollection)this["customers"]; }
       }
    }
}
