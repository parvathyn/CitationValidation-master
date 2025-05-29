using System;
using System.Collections.Generic;
using System.IO;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using System.Xml;
using System.Xml.Serialization;
using Microsoft.Web.Services3;

namespace DataAccess.Model.Parkeon
{
    [System.Xml.Serialization.XmlRoot("get")]
    public class ParkeonRequest
    {
        [System.Xml.Serialization.XmlElementAttribute("type", typeof(string))]
        public string TypeCondition { get; set; }

        [System.Xml.Serialization.XmlElementAttribute("condition", typeof(string))]
        public string Condition { get; set; }

        [System.Xml.Serialization.XmlElementAttribute("orderby", typeof(string))]
        public string OrderByCondition { get; set; }

        [System.Xml.Serialization.XmlElementAttribute("sort", typeof(string))]
        public string Sortcondition { get; set; }

        [System.Xml.Serialization.XmlElementAttribute("limit", typeof(string))]
        public string Limit { get; set; }

        public string GetXmlTextData()
        {
            SoapEnvelope soapRequest = new SoapEnvelope();
            XmlElement body = soapRequest.CreateBody();


            string xml = string.Empty;
            try
            {
                var serializer = new XmlSerializer(typeof(ParkeonRequest));
                var ns = new XmlSerializerNamespaces();
                ns.Add("", "");
                var sw = new StringWriter();
                var xmlWriter = XmlWriter.Create(sw, new XmlWriterSettings() { OmitXmlDeclaration = true });
                serializer.Serialize(xmlWriter, this, ns);
                body.InnerXml = sw.ToString(); ;
                xml = soapRequest.InnerXml;
                ////////////////////

            }
            catch (Exception ex)
            {
                throw ex;
            }
            return xml;
        }

    }
}
