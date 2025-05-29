using System.Configuration;
namespace DataAccess.Model.Pango
{
    [ConfigurationCollection(typeof(PangoCustomerElement))]
    public class PangoCustomerElementCollection : ConfigurationElementCollection
    {
        public PangoCustomerElement this[int index]
        {
            get { return (PangoCustomerElement)BaseGet(index); }
            set
            {
                if (BaseGet(index) != null)
                    BaseRemoveAt(index);

                BaseAdd(index, value);
            }
        }
        protected override ConfigurationElement CreateNewElement()
        {
            return new PangoCustomerElement();
        }

        protected override object GetElementKey(ConfigurationElement element)
        {
            return ((PangoCustomerElement)element).CustomerId;
        }
    }
}
