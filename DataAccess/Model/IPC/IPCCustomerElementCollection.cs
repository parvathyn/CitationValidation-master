using System.Configuration;

namespace DataAccess.Model.IPC
{

    [ConfigurationCollection(typeof(IPCCustomerElement))]
    public class IPCCustomerElementCollection : ConfigurationElementCollection
    {
        public IPCCustomerElement this[int index]
        {
            get { return (IPCCustomerElement)BaseGet(index); }
            set
            {
                if (BaseGet(index) != null)
                    BaseRemoveAt(index);

                BaseAdd(index, value);
            }
        }
        protected override ConfigurationElement CreateNewElement()
        {
            return new IPCCustomerElement();
        }

        protected override object GetElementKey(ConfigurationElement element)
        {
            return ((IPCCustomerElement)element).CustomerId;
        }
    }
}
