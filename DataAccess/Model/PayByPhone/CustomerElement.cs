using System.Configuration;

namespace DataAccess.Model.PayByPhone
{
    public class CustomerElement : ConfigurationElement
    {
        public CustomerElement() { }


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

        [ConfigurationProperty("vendorId", DefaultValue = "", IsKey = true, IsRequired = true)]
        public string VendorId
        {
            get { return (string)this["vendorId"]; }
            set { this["vendorId"] = value; }
        }

        [ConfigurationProperty("userName", DefaultValue = "", IsKey = true, IsRequired = true)]
        public string UserName
        {
            get { return (string)this["userName"]; }
            set { this["userName"] = value; }
        }

        [ConfigurationProperty("userPassword", DefaultValue = "", IsKey = true, IsRequired = true)]
        public string UserPassword
        {
            get { return (string)this["userPassword"]; }
            set { this["userPassword"] = value; }
        }



        [ConfigurationProperty("baseURL", DefaultValue = "", IsKey = true, IsRequired = true)]
        public string BaseURL
        {
            get { return (string)this["baseURL"]; }
            set { this["baseURL"] = value; }
        }
    }
}
