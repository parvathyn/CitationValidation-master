using System.Configuration;

namespace DataAccess.Model.AceParking
{

    [ConfigurationCollection(typeof(AceParkingCustomerElement))]
    public class AceParkingCustomerElementCollection : ConfigurationElementCollection
    {
        public AceParkingCustomerElement this[int index]
        {
            get { return (AceParkingCustomerElement)BaseGet(index); }
            set
            {
                if (BaseGet(index) != null)
                    BaseRemoveAt(index);

                BaseAdd(index, value);
            }
        }
        protected override ConfigurationElement CreateNewElement()
        {
            return new AceParkingCustomerElement();
        }

        protected override object GetElementKey(ConfigurationElement element)
        {
            return ((AceParkingCustomerElement)element).CustomerId;
        }
    }
}
