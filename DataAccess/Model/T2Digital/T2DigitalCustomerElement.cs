using System;
using System.Configuration;

namespace DataAccess.Model.T2Digital
{
    public class T2DigitalCustomerElement : ConfigurationElement
    {
        public T2DigitalCustomerElement() { }
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

        [ConfigurationProperty("token", DefaultValue = "", IsKey = true, IsRequired = true)]
        public string Token
        {
            get { return (string)this["token"]; }
            set { this["token"] = value; }
        }

        [ConfigurationProperty("gracePeriodInSecond", DefaultValue = "", IsKey = true, IsRequired = true)]
        public string GracePeriodInSecond
        {
            get { return (string)this["gracePeriodInSecond"]; }
            set { this["gracePeriodInSecond"] = value; }
        }

        [ConfigurationProperty("enforcementAPI", DefaultValue = "", IsRequired = true)]
        public string EnforcementAPI
        {
            get { return (string)this["enforcementAPI"]; }
            set { this["enforcementAPI"] = value; }
        }

        public ServiceType ServiceToAccess
        {
            get
            {
                return (ServiceType)Enum.Parse(typeof(ServiceType), this.EnforcementAPI, true);
            }
        }
    }
}
