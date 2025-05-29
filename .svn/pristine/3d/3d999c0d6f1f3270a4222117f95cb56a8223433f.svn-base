using System.Configuration;

namespace DataAccess.Model.T2Digital
{
    [ConfigurationCollection(typeof(T2DigitalCustomerElement))]
    public class T2DigitalCustomerElementCollection : ConfigurationElementCollection
    {
        public T2DigitalCustomerElement this[int index]
        {
            get { return (T2DigitalCustomerElement)BaseGet(index); }
            set
            {
                if (BaseGet(index) != null)
                    BaseRemoveAt(index);

                BaseAdd(index, value);
            }
        }
        protected override ConfigurationElement CreateNewElement()
        {
            return new T2DigitalCustomerElement();
        }

        protected override object GetElementKey(ConfigurationElement element)
        {
            return ((T2DigitalCustomerElement)element).CustomerId;
        }
    }
}
