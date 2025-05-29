using System.Configuration;


namespace DataAccess.Model.Pango
{
    public class PangoCustomerRetriever
    {
        public static PangoCustomerRetrieverSection _Config = ConfigurationManager.GetSection("pango") as PangoCustomerRetrieverSection;
        public static PangoCustomerElementCollection GetCustomers()
        {
            return _Config.Customers;
        }

        public static PangoCustomerElement GetCustomer(int customerId)
        {
            foreach (PangoCustomerElement item in _Config.Customers)
            {
                if (item.CustomerId == customerId.ToString())
                    return item;
            }
            return null;
        }
    }
}
