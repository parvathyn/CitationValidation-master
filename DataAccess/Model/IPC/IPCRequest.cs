using System.Xml.Serialization;

namespace DataAccess.Model.IPC
{
    [XmlRoot("Request", Namespace = "http://www2.ipsmetersystems.com/meter")]

    public class IPCRequest
    {
        [XmlElement("LicensePlateNumber")]
        public string LicensePlateNumber { get; set; }
    }
}
