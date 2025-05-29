using System.Configuration;

namespace DataAccess.Model.CustomerMap
{
    public class CustomerMapElement : ConfigurationElement
    {
        [ConfigurationProperty("customerId", DefaultValue = "", IsRequired = true)]
        public string CustomerId
        {
            get { return (string)this["customerId"]; }
            set { this["customerId"] = value; }
        }

        [ConfigurationProperty("name", DefaultValue = "", IsKey = true, IsRequired = true)]
        public string Name
        {
            get { return (string)this["name"]; }
            set { this["name"] = value; }
        }

        [ConfigurationProperty("vendorName", DefaultValue = "", IsRequired = true)]
        public string VendorName
        {
            get { return (string)this["vendorName"]; }
            set { this["vendorName"] = value; }
        }

        [ConfigurationProperty("vendorId", DefaultValue = "", IsKey = true, IsRequired = true)]
        public string VendorId
        {
            get { return (string)this["vendorId"]; }
            set { this["vendorId"] = value; }
        }
    }
}
