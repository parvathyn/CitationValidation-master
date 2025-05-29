using System.Configuration;

namespace DataAccess.Model.Parkmobile
{
    [ConfigurationCollection(typeof(PMCustomerElement))]
    public class PMCustomerElementCollection : ConfigurationElementCollection
    {
        public PMCustomerElement this[int index]
        {
            get { return (PMCustomerElement)BaseGet(index); }
            set
            {
                if (BaseGet(index) != null)
                    BaseRemoveAt(index);

                BaseAdd(index, value);
            }
        }
        protected override ConfigurationElement CreateNewElement()
        {
            return new PMCustomerElement();
        }

        protected override object GetElementKey(ConfigurationElement element)
        {
            return ((PMCustomerElement)element).CustomerId;
        }
    }
}
