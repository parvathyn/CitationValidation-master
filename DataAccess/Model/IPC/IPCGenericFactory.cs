using Newtonsoft.Json;
using Newtonsoft.Json.Linq;
using System;
using System.Collections.Generic;
using System.IO;
using System.Linq;
using System.Net;
using System.Net.Http;
using System.Text;
using System.Xml;
using System.Xml.Serialization;

namespace DataAccess.Model.IPC
{
    public class IPCGenericFactory
    {


        protected IPCParameter _parameter;
        protected IPCCustomerElement _customer;
        protected int customerId;
        protected string currentURL = string.Empty;
        protected CustomerTime customerTime = null;
        public IPCGenericFactory(IPCCustomerElement customer, string plateNo, string state, CustomerTime customerCurrentTime)
        {
            this._customer = customer;
            this.customerId = Convert.ToInt32(customer.CustomerId);
            this._parameter = new IPCParameter(plateNo);
            this.customerTime = customerCurrentTime;
        }

        public   TransactionTransform GetRefreshPlateData(ref VendorResponseEntity venderResonse)
        {
            TransactionTransform refreshPlate = new TransactionTransform() { ReturnCode = ((int)ReturnCodeEnum.Error).ToString() };
            try
            {
                using (var client = new HttpClient())
                {
                    System.Net.ServicePointManager.SecurityProtocol = SecurityProtocolType.Tls12 | SecurityProtocolType.Tls11 | SecurityProtocolType.Tls;

                    var currentURL = UrlList.GetPlateStatusURL(this._customer, this._parameter);
                    client.DefaultRequestHeaders.Accept.Clear();
                    client.DefaultRequestHeaders.Add("IPSToken", this._customer.Token);

                    var request = new IPCRequest
                    {
                        LicensePlateNumber = this._parameter.PlateNo
                    };


                    var serializer = new XmlSerializer(typeof(IPCRequest));
                    var xmlString = string.Empty;

                    using (var stringWriter = new StringWriter())
                    {
                        serializer.Serialize(stringWriter, request);
                        xmlString = stringWriter.ToString();
                    }


                    XmlDocument xmlDocument = new XmlDocument();
                    xmlDocument.LoadXml(xmlString);
                    xmlDocument.RemoveChild(xmlDocument.FirstChild);

                    // Get the modified XML string
                    StringBuilder sb = new StringBuilder();
                    XmlWriterSettings settings = new XmlWriterSettings
                    {
                        OmitXmlDeclaration = true,
                        Indent = true
                    };
                    using (XmlWriter writer = XmlWriter.Create(sb, settings))
                    {
                        xmlDocument.WriteTo(writer);
                    }

                    string modifiedXmlString = sb.ToString();


                    var content = new StringContent(modifiedXmlString, System.Text.Encoding.UTF8, "application/xml");
                    venderResonse.RequestURL = currentURL ;
                    venderResonse.IPCRequestData = modifiedXmlString;
                    var response =  client.PostAsync(currentURL, content).Result;

                    if (response.IsSuccessStatusCode)
                    {
                        try
                        {
                            string jsonData =  response.Content.ReadAsStringAsync().Result;
                            venderResonse.XmlandJsonData = jsonData;
                            venderResonse.IPCResponse = jsonData;
                            venderResonse.ResponseHttpStatusCode = response.StatusCode;
                            List<IPCParkingData> parkingDataList = JsonConvert.DeserializeObject<List<IPCParkingData>>(jsonData);

                            if (parkingDataList != null)
                            {
                                if (parkingDataList.Count > 0)
                                {
                                    IPCParkingData validData = parkingDataList.OrderByDescending(t => t.ParkingExpiryTime).FirstOrDefault();
                                    if (validData != null)
                                    {
                                        refreshPlate.LicensePlate = validData.LicensePlateNumber;
                                        refreshPlate.MeterID = validData.PoleNumber;
                                        refreshPlate.ZoneName = validData.SubAreaName;
                                        refreshPlate.MeterExpiredMinutes = this._parameter.ExpiredMinutes(this._customer.GracePeriodInSecond.ToNullableInt(), customerTime.CurrentTime.Value,validData.ParkingExpiryTime).ToString();
                                    }
                                    else
                                    {
                                        refreshPlate.LicensePlate = this._parameter.PlateNo;
                                        refreshPlate.MeterExpiredMinutes = "-1";
                                    }
                                    refreshPlate.ReturnCode = ((int)ReturnCodeEnum.Success).ToString();
                                }
                                else
                                {
                                    refreshPlate.LicensePlate = this._parameter.PlateNo;
                                    refreshPlate.MeterExpiredMinutes = "-1";
                                }
                                refreshPlate.ReturnCode = ((int)ReturnCodeEnum.Success).ToString();
                            }
                        }
                        catch (Exception ex)
                        {
                            venderResonse.GeneralException = ex;
                        }
                    }
                    else
                    {
                        venderResonse.ResponseHttpStatusCode = response.StatusCode;
                        //Error in status code. Do nothing
                    }
                }
            }
            catch (Exception ex)
            {
                venderResonse.GeneralException = ex;
            }
            return refreshPlate;
        }
    }
}
