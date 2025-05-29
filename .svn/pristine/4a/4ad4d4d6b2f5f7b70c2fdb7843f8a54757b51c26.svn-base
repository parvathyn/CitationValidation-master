using System;
using System.Collections.Generic;
using System.Configuration;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace DataAccess.Model.AceParking
{


    public class AceParkingCustomerElement : ConfigurationElement
    {
        public AceParkingCustomerElement() { }


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


        [ConfigurationProperty("key", DefaultValue = "", IsKey = true, IsRequired = true)]
        public string Key
        {
            get { return (string)this["key"]; }
            set { this["key"] = value; }
        }

        [ConfigurationProperty("url", DefaultValue = "", IsKey = true, IsRequired = true)]
        public string Url
        {
            get { return (string)this["url"]; }
            set { this["url"] = value; }
        }

        //[ConfigurationProperty("action", DefaultValue = "", IsKey = true, IsRequired = true)]
        //public string Action
        //{
        //    get { return (string)this["action"]; }
        //    set { this["action"] = value; }
        //}

        public string PlateNo { get; set; }
        public string FinalURL
        {
            get
            {
                //https://spacestage.aceparking.com/services/getLicensePlates?key=EAEDF787E029BB0E&plate_number=adsfd
                return string.Format("{0}?key={1}&plate_number={2}", this.Url, this.Key, this.PlateNo);
            }
        }

        [ConfigurationProperty("gracePeriodInSecond", DefaultValue = "", IsKey = true, IsRequired = true)]
        public string GracePeriodInSecond
        {
            get { return (string)this["gracePeriodInSecond"]; }
            set { this["gracePeriodInSecond"] = value; }
        }
    }
}
