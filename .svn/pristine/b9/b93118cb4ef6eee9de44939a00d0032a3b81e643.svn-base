using System.Collections.Generic;
using System.Configuration;

namespace DataAccess.Model.Parkeon
{
    public class ParkeonCustomerRetriever
    {
        public static ParkeonCustomerRetrieverSection _Config = ConfigurationManager.GetSection("parkeon") as ParkeonCustomerRetrieverSection;
        public static ParkeonCustomerElementCollection GetCustomers()
        {
            return _Config.Customers;
        }

        public static ParkeonCustomerElement GetCustomer(int customerId)
        {
            foreach (ParkeonCustomerElement item in _Config.Customers)
            {
                if (item.CustomerId == customerId.ToString())
                    return item;
            }
            return null;
        }

        public static List<ParkeonCustomerElement> GetCustomers(int customerId)
        {
            List<ParkeonCustomerElement> coll = new List<ParkeonCustomerElement>();
            foreach (ParkeonCustomerElement item in _Config.Customers)
            {
                if (item.CustomerId == customerId.ToString())
                    coll.Add(item);
            }

            if (coll.Count == 0)
                return null;
            else
                return coll;
        }
    }
}
