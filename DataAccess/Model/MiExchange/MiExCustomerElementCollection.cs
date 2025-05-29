using System.Configuration;

namespace DataAccess.Model.MiExchange
{
    [ConfigurationCollection(typeof(MiExCustomerElement))]
    public class MiExCustomerElementCollection : ConfigurationElementCollection
    {
        public MiExCustomerElement this[int index]
        {
            get { return (MiExCustomerElement)BaseGet(index); }
            set
            {
                if (BaseGet(index) != null)
                    BaseRemoveAt(index);

                BaseAdd(index, value);
            }
        }
        protected override ConfigurationElement CreateNewElement()
        {
            return new MiExCustomerElement();
        }

        protected override object GetElementKey(ConfigurationElement element)
        {
            return ((MiExCustomerElement)element).CustomerId;
        }
    }
}
