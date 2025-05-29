using System.Configuration;

namespace DataAccess.Model.Cale
{
    [ConfigurationCollection(typeof(CaleCustomerElement))]
    public class CaleCustomerElementCollection : ConfigurationElementCollection
    {
        public CaleCustomerElement this[int index]
        {
            get { return (CaleCustomerElement)BaseGet(index); }
            set
            {
                if (BaseGet(index) != null)
                    BaseRemoveAt(index);

                BaseAdd(index, value);
            }
        }
        protected override ConfigurationElement CreateNewElement()
        {
            return new CaleCustomerElement();
        }

        protected override object GetElementKey(ConfigurationElement element)
        {
            return ((CaleCustomerElement)element).CustomerId;
        }
    }
}
