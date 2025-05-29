using System.Configuration;

namespace DataAccess.Model.PassportMonitoring
{
    [ConfigurationCollection(typeof(PassportMonitoringCustomerElement))]
    public class PassportMonitoringCustomerElementCollection : ConfigurationElementCollection
    {
        public PassportMonitoringCustomerElement this[int index]
        {
            get { return (PassportMonitoringCustomerElement)BaseGet(index); }
            set
            {
                if (BaseGet(index) != null)
                    BaseRemoveAt(index);

                BaseAdd(index, value);
            }
        }
        protected override ConfigurationElement CreateNewElement()
        {
            return new PassportMonitoringCustomerElement();
        }

        protected override object GetElementKey(ConfigurationElement element)
        {
            return ((PassportMonitoringCustomerElement)element).CustomerId;
        }
    }
}
