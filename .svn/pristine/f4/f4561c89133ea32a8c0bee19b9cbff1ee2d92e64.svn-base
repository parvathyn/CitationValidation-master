using DataAccess.PangoProdService;
using System;
using System.IO;
using System.Net;
using System.Xml;
using System.Xml.Serialization;

namespace DataAccess.Model.Pango
{
    public class PangoHttpClient
    {
        private string url = string.Empty;
        private string action = string.Empty;
        protected PangoCustomerElement _customer;
        protected int customerId;
        protected PangoParameterHttpClient _parameter;
        public PangoHttpClient(PangoCustomerElement customer, string plateNo, string state, string url, string action)
        {
            this._customer = customer;
            this.customerId = Convert.ToInt32(customer.CustomerId);
            this._parameter = new PangoParameterHttpClient(plateNo, state, url, action);
            this.url = url;
            this.action = action;
        }
        public virtual TransactionTransform GetPlateData(ref VendorResponseEntity venderResonse)
        {
            System.Net.ServicePointManager.SecurityProtocol = SecurityProtocolType.Tls12;
            TransactionTransform refreshPlate = new TransactionTransform() { ReturnCode = ((int)ReturnCodeEnum.Error).ToString() };
            AnswerDetails pangoData = null;
            try
            {
                //venderResonse.
                string dataXml = this.GetPlateDataXML();
                if (!string.IsNullOrEmpty(dataXml))
                {
                    venderResonse.ResponseHttpStatusCode = System.Net.HttpStatusCode.OK;
                    XmlSerializer xsrequest = new XmlSerializer(typeof(AnswerDetails));
                    venderResonse.PangoResponse = dataXml;
                    ///////////////////////////////////////////////////

                    ////////////////////////////////////////////
                }
                else
                {
                    venderResonse.PangoResponse = string.Empty;
                    refreshPlate.ReturnCode = ((int)ReturnCodeEnum.NoRecordExists).ToString();
                    refreshPlate.MeterExpiredMinutes = (-1).ToString();
                }
            }
            catch (System.ServiceModel.FaultException<TimeoutException> timeoutEx)
            {
                venderResonse.GeneralException = timeoutEx;
            }
            catch (System.ServiceModel.CommunicationException commEx)
            {
                venderResonse.GeneralException = commEx;
            }
            catch (Exception ex)
            {
                venderResonse.GeneralException = ex;
                refreshPlate.ReturnCode = ((int)ReturnCodeEnum.Error).ToString();
                //Nothing to do.
            }

            return refreshPlate;
        }
        private String GetPlateDataXML()
        {
            XmlDocument soapEnvelopeXml = CreateSoapEnvelope();
            HttpWebRequest webRequest = CreateWebRequest();

            InsertSoapEnvelopeIntoWebRequest(soapEnvelopeXml, webRequest);

            string result= string.Empty;
            using (WebResponse response = webRequest.GetResponse())
            {
                using (StreamReader rd = new StreamReader(response.GetResponseStream()))
                {
                    //result = rd.ReadToEnd();
                    ////////////////////////////////////////////////
                    var responsedata = rd.ReadToEnd();

                    var serializer =
                    new XmlSerializer(typeof(AnswerDetails[]), new XmlRootAttribute("CheckPlateNumberWithUserNameAndZone_ex3Result"));
                    //////////////////////////////////////////////
                }
            }
            return result;
        }

        #region Private
        private XmlDocument CreateSoapEnvelope()
        {
            XmlDocument soapEnvelopeXml = new XmlDocument();
            soapEnvelopeXml.LoadXml(@"<?xml version=""1.0"" encoding=""utf-8""?>
                    <soap:Envelope xmlns:soap=""http://schemas.xmlsoap.org/soap/envelope/"" xmlns:xsi=""http://www.w3.org/2001/XMLSchema-instance"" xmlns:xsd=""http://www.w3.org/2001/XMLSchema"">
                    <soap:Body>
                       <CheckPlateNumberWithUserNameAndZone_ex3 xmlns=""http://tempuri.org/"">
                          <UserName>" + this._customer.UserName + @"</UserName>
                          <Password>" + this._customer.UserPassword + @"</Password>
                          <device_id></device_id>
                          <officerID></officerID>
                          <PlateNumber>" + this._parameter.PlateNo + @"</PlateNumber>
                          <StateCode>" + this._parameter.State + @"</StateCode>
                          <Zone></Zone>
                    
                      </CheckPlateNumberWithUserNameAndZone_ex3>
                    </soap:Body>
                    </soap:Envelope>");
            return soapEnvelopeXml;
        }
        private HttpWebRequest CreateWebRequest()
        {
            HttpWebRequest webRequest = (HttpWebRequest)WebRequest.Create(this.url);
            webRequest.Headers.Clear();
            //webRequest.Headers.Add("SOAPAction");
            webRequest.ContentType = "text/xml;charset=\"utf-8\"";
            webRequest.Accept = "text/xml";
            webRequest.Method = "POST";
            return webRequest;
        }
        private void InsertSoapEnvelopeIntoWebRequest(XmlDocument soapEnvelopeXml, HttpWebRequest webRequest)
        {
            using (Stream stream = webRequest.GetRequestStream())
            {
                soapEnvelopeXml.Save(stream);
            }
        }
        #endregion

    }
}
