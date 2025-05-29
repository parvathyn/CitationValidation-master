using System.Configuration;


namespace DataAccess.Model.PayByPhone
{
    [ConfigurationCollection(typeof(CustomerElement))]
    public class CustomerElementCollection : ConfigurationElementCollection
    {
        public CustomerElement this[int index]
        {
            get { return (CustomerElement)BaseGet(index); }
            set
            {
                if (BaseGet(index) != null)
                    BaseRemoveAt(index);

                BaseAdd(index, value);
            }
        }
        protected override ConfigurationElement CreateNewElement()
        {
            return new CustomerElement();
        }

        protected override object GetElementKey(ConfigurationElement element)
        {
            return ((CustomerElement)element).CustomerId;
        }
    }

}
