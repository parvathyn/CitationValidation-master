using System.Configuration;

namespace DataAccess.Model.PassportMonitoring
{
    public class PassportMonitoringCustomerElement : ConfigurationElement
    {
        public PassportMonitoringCustomerElement() { }

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

        [ConfigurationProperty("token", DefaultValue = "", IsKey = true, IsRequired = true)]
        public string Token
        {
            get { return (string)this["token"]; }
            set { this["token"] = value; }
        }

        [ConfigurationProperty("baseURL", DefaultValue = "", IsKey = true, IsRequired = true)]
        public string BaseURL
        {
            get { return (string)this["baseURL"]; }
            set { this["baseURL"] = value; }
        }

        [ConfigurationProperty("gracePeriodInSecond", DefaultValue = "", IsKey = true, IsRequired = true)]
        public string GracePeriodInSecond
        {
            get { return (string)this["gracePeriodInSecond"]; }
            set { this["gracePeriodInSecond"] = value; }
        }
    }
}
