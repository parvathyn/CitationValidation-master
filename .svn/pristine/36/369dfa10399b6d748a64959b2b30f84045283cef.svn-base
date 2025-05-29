using System.Configuration;

namespace DataAccess.Model.Cale
{
    public class CaleCustomerElement : ConfigurationElement
    {
        public CaleCustomerElement() { }
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




        [ConfigurationProperty("gracePeriodInSecond", DefaultValue = "", IsKey = true, IsRequired = true)]
        public string GracePeriodInSecond
        {
            get { return (string)this["gracePeriodInSecond"]; }
            set { this["gracePeriodInSecond"] = value; }
        }

        [ConfigurationProperty("baseURL", DefaultValue = "", IsKey = true, IsRequired = true)]
        public string BaseURL
        {
            get { return (string)this["baseURL"]; }
            set { this["baseURL"] = value; }
        }

        [ConfigurationProperty("enforcementType", DefaultValue = "", IsRequired = true)]
        public string EnforcementType
        {
            get { return (string)this["enforcementType"]; }
            set { this["enforcementType"] = value; }
        }
    }
}
