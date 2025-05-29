using System;
using System.Collections.Generic;
using System.IO;
using System.Linq;
using System.Net;
using System.Net.Http;
using System.Text;
using System.Threading.Tasks;
using System.Xml.Serialization;

namespace DataAccess.Model.Cale.Schema5
{
    public class CaleSchema5GenericFactory
    {
        protected CaleParameter _parameter;
        protected CaleCustomerElement _customer;
        protected int customerId;
        protected string currentURL = string.Empty;

        public CaleSchema5GenericFactory(CaleCustomerElement customer, string plateNo, string state)
        {
            this._customer = customer;
            this.customerId = Convert.ToInt32(customer.CustomerId);
            this._parameter = new CaleParameter(plateNo, state);
        }

        public TransactionTransform GetRefreshPlateData(ref VendorResponseEntity venderResonse)
        {
            TransactionTransform refreshPlate = new TransactionTransform() { ReturnCode = ((int)ReturnCodeEnum.Error).ToString() };
            try
            {
                using (var client = new HttpClient())
                {
                    System.Net.ServicePointManager.SecurityProtocol = SecurityProtocolType.Tls12 | SecurityProtocolType.Tls11 | SecurityProtocolType.Tls;

                    var currentURL = UrlList.GetPlateStatusURL(this._customer, this._parameter);
                    client.DefaultRequestHeaders.Accept.Clear();
                    var byteArray = Encoding.ASCII.GetBytes(string.Format("{0}:{1}", this._customer.UserName, this._customer.UserPassword));
                    client.DefaultRequestHeaders.Authorization = new System.Net.Http.Headers.AuthenticationHeaderValue("Basic", Convert.ToBase64String(byteArray));
                    venderResonse.RequestURL = currentURL;
                    HttpResponseMessage response = client.GetAsync(currentURL).Result;
                    if (response.IsSuccessStatusCode)
                    {
                        try
                        {
                            string xmlData = response.Content.ReadAsStringAsync().Result;
                            venderResonse.XmlandJsonData = xmlData;
                            venderResonse.ResponseHttpStatusCode = response.StatusCode;
                            XmlSerializer serlializer = new XmlSerializer(typeof(ArrayOfValidParkingData));
                            ArrayOfValidParkingData data = (ArrayOfValidParkingData)serlializer.Deserialize(new StringReader(xmlData));
                            if (data != null)
                            {
                                if (data.ValidParkingData.Count > 0)
                                {
                                    //ArrayOfValidParkingDataValidParkingData validData = data.ValidParkingData.OrderByDescending(t => t.EndDateUtc).FirstOrDefault();
                                    ValidParkingData validData = data.ValidParkingData.Where( t => t.EndDateUtc.HasValue == true).OrderByDescending(t => t.EndDateUtc).FirstOrDefault();
                                    if (validData != null)
                                    {
                                        refreshPlate.LicensePlate = validData.Code;
                                        //refreshPlate.MeterExpiredMinutes = this._parameter.ExpiredMinutes(this._customer.GracePeriodInSecond.ToNullableInt(), validData.EndDateUtc).ToString();
                                        refreshPlate.MeterExpiredMinutes = this._parameter.ExpiredMinutes(this._customer.GracePeriodInSecond.ToNullableInt(), validData.EndDateUtc.Value).ToString();
                                        //////
                                        if (!string.IsNullOrWhiteSpace(validData.Zone))
                                            refreshPlate.ZoneName = validData.Zone;
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
        public TransactionTransform GetRefreshPlateDataPhiladelphia(ref VendorResponseEntity venderResonse)
        {
            TransactionTransform refreshPlate = new TransactionTransform() { ReturnCode = ((int)ReturnCodeEnum.Error).ToString() };
            try
            {
                using (var client = new HttpClient())
                {
                    System.Net.ServicePointManager.SecurityProtocol = SecurityProtocolType.Tls12 | SecurityProtocolType.Tls11 | SecurityProtocolType.Tls;

                    var currentURL = UrlList.GetPlateStatusURL(this._customer, this._parameter);
                    client.DefaultRequestHeaders.Accept.Clear();
                    var byteArray = Encoding.ASCII.GetBytes(string.Format("{0}:{1}", this._customer.UserName, this._customer.UserPassword));
                    client.DefaultRequestHeaders.Authorization = new System.Net.Http.Headers.AuthenticationHeaderValue("Basic", Convert.ToBase64String(byteArray));
                    venderResonse.RequestURL = currentURL;
                    HttpResponseMessage response = client.GetAsync(currentURL).Result;
                    if (response.IsSuccessStatusCode)
                    {
                        try
                        {
                            string xmlData = response.Content.ReadAsStringAsync().Result;
                            venderResonse.XmlandJsonData = xmlData;
                            venderResonse.ResponseHttpStatusCode = response.StatusCode;
                            XmlSerializer serlializer = new XmlSerializer(typeof(ArrayOfValidParkingData));
                            ArrayOfValidParkingData data = (ArrayOfValidParkingData)serlializer.Deserialize(new StringReader(xmlData));
                            if (data != null)
                            {
                                if (data.ValidParkingData.Count > 0)
                                {
                                    ValidParkingData validData = data.ValidParkingData.Where(t => t.EndDateUtc.HasValue == true).OrderByDescending(t => t.EndDateUtc).FirstOrDefault();
                                    if (validData != null)
                                    {
                                        refreshPlate.LicensePlate = validData.Code;
                                        refreshPlate.MeterExpiredMinutes = this._parameter.ExpiredMinutes(this._customer.GracePeriodInSecond.ToNullableInt(), validData.EndDateUtc.Value).ToString();
                                        //////
                                        if (this.customerId != 7056)
                                        {
                                            if (!string.IsNullOrWhiteSpace(validData.Article.Name))
                                                refreshPlate.ZoneName = validData.Zone;
                                        }
                                        else
                                        {
                                            if (!string.IsNullOrWhiteSpace(validData.Article.Name))
                                                refreshPlate.ZoneName = validData.Article.Name;
                                        }

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
        public TransactionTransform GetRefreshPlateData(ref VendorResponseEntity venderResonse, string zone)
        {
            TransactionTransform refreshPlate = new TransactionTransform() { ReturnCode = ((int)ReturnCodeEnum.Error).ToString() };
            try
            {
                using (var client = new HttpClient())
                {
                    System.Net.ServicePointManager.SecurityProtocol = SecurityProtocolType.Tls12 | SecurityProtocolType.Tls11 | SecurityProtocolType.Tls;

                    var currentURL = UrlList.GetPlateStatusURLSchema5(this._customer, this._parameter);
                    client.DefaultRequestHeaders.Accept.Clear();
                    var byteArray = Encoding.ASCII.GetBytes(string.Format("{0}:{1}", this._customer.UserName, this._customer.UserPassword));
                    client.DefaultRequestHeaders.Authorization = new System.Net.Http.Headers.AuthenticationHeaderValue("Basic", Convert.ToBase64String(byteArray));
                    venderResonse.RequestURL = currentURL;
                    HttpResponseMessage response = client.GetAsync(currentURL).Result;
                    if (response.IsSuccessStatusCode)
                    {
                        try
                        {
                            string xmlData = response.Content.ReadAsStringAsync().Result;
                            venderResonse.XmlandJsonData = xmlData;
                            venderResonse.ResponseHttpStatusCode = response.StatusCode;
                            XmlSerializer serlializer = new XmlSerializer(typeof(ArrayOfValidParkingData));
                            ArrayOfValidParkingData data = (ArrayOfValidParkingData)serlializer.Deserialize(new StringReader(xmlData));
                            if (data != null)
                            {
                                if (data.ValidParkingData.Count > 0)
                                {
                                    ValidParkingData validData = data.ValidParkingData.Where(t => t.EndDateUtc.HasValue == true).Where(t => t.Article.Name.ToLower() == zone.ToLower()).OrderByDescending(t => t.EndDateUtc).FirstOrDefault();
                                    if (validData != null)
                                    {
                                        refreshPlate.LicensePlate = validData.Code;
                                        refreshPlate.MeterExpiredMinutes = this._parameter.ExpiredMinutes(this._customer.GracePeriodInSecond.ToNullableInt(), validData.EndDateUtc.Value).ToString();
                                        //////
                                        if (this.customerId != 7056)
                                        {
                                            if (!string.IsNullOrWhiteSpace(validData.Article.Name))
                                                refreshPlate.ZoneName = validData.Zone;
                                        }
                                        else
                                        {
                                            if (!string.IsNullOrWhiteSpace(validData.Article.Name))
                                                refreshPlate.ZoneName = validData.Article.Name;
                                        }

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
