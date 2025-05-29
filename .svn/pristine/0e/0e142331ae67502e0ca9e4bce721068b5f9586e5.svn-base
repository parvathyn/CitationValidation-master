using System.Configuration;

namespace DataAccess.Model.CustomerMap
{
    [ConfigurationCollection(typeof(CustomerMapElement))]
    public class CustomerMapElementCollection : ConfigurationElementCollection
    {
        public CustomerMapElement this[int index]
        {
            get { return (CustomerMapElement)BaseGet(index); }
            set
            {
                if (BaseGet(index) != null)
                    BaseRemoveAt(index);

                BaseAdd(index, value);
            }
        }
        protected override ConfigurationElement CreateNewElement()
        {
            return new CustomerMapElement();
        }

        protected override object GetElementKey(ConfigurationElement element)
        {
            return ((CustomerMapElement)element).VendorId;
        }
    }
}
