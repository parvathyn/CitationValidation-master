using Newtonsoft.Json;
using System;
using System.Collections.Specialized;
using System.Linq;
using System.Net;
using System.Net.Http;
using System.Text;
using System.Web;


namespace DataAccess.Model.Parkmobile
{
    public class ParkMobileGenericFactory
    {
        protected ParkMobileParameter _parameter;
        protected ParkmobileServiceParameter _serviceParameter;
        protected PMCustomerElement _customer;
        protected Uri uri;
        protected Uri plateUri;
        protected int customerId;
        protected string _zone = null;

        public ParkMobileGenericFactory(PMCustomerElement customer, int zoneId, int spaceNo)
        {
            this._customer = customer;
            this.customerId = Convert.ToInt32(customer.CustomerId);
            this._parameter = new ParkMobileParameter(zoneId, spaceNo);
            this._serviceParameter = new ParkmobileServiceParameter() { UserName = customer.UserName, UserPassword = customer.UserPassword };
            //var values = new NameValueCollection 
            //{ 
            //    {"format",  "json" }
            //};
            //uri = UrlBuilderParkMobile.BuildUri(string.Format("{0}/zone/{1}/{2}", this._customer.BaseURL, this._parameter.ZoneId.ToString(), this._parameter.SpaceId.ToString()), values);
            //uri = UrlBuilderParkMobile.BuildUri2(string.Format("{0}zone/", this._customer.BaseURL));
            uri = UrlBuilderParkMobile.BuildSpaceUrl(string.Format("{0}zone/{1}/{2}", this._customer.BaseURL, this._parameter.ZoneId.ToString(), this._parameter.SpaceId.ToString()));
        }
        public ParkMobileGenericFactory(PMCustomerElement customer, string plateNo)
        {
            this._customer = customer;
            this.customerId = Convert.ToInt32(customer.CustomerId);
            this._parameter = new ParkMobileParameter(plateNo);
            this._serviceParameter = new ParkmobileServiceParameter() { UserName = customer.UserName, UserPassword = customer.UserPassword };
            //var values = new NameValueCollection 
            //{ 
            //    {"format",  "json" }
            //};
            //plateUri = UrlBuilderParkMobile.BuildUri(string.Format("{0}/vehicle/{1}", this._customer.BaseURL, this._parameter.PlateNo), values);
            //plateUri = UrlBuilderParkMobile.BuildUri2((string.Format("{0}vehicle/", this._customer.BaseURL)));
            plateUri = UrlBuilderParkMobile.BuildPlateUrl(string.Format("{0}vehicle/{1}", this._customer.BaseURL, this._parameter.PlateNo));
        }

        public ParkMobileGenericFactory(PMCustomerElement customer, string plateNo, string zone)
        {
            this._customer = customer;
            this.customerId = Convert.ToInt32(customer.CustomerId);
            this._parameter = new ParkMobileParameter(plateNo);
            this._serviceParameter = new ParkmobileServiceParameter() { UserName = customer.UserName, UserPassword = customer.UserPassword };
            this._zone = zone;
            //var values = new NameValueCollection 
            //{ 
            //    {"format",  "json" }
            //};
            //plateUri = UrlBuilderParkMobile.BuildUri(string.Format("{0}/vehicle/{1}", this._customer.BaseURL, this._parameter.PlateNo), values);
            //plateUri = UrlBuilderParkMobile.BuildUri2((string.Format("{0}vehicle/", this._customer.BaseURL)));
            plateUri = UrlBuilderParkMobile.BuildPlateUrl(string.Format("{0}vehicle/{1}", this._customer.BaseURL, this._parameter.PlateNo));
        }

        protected virtual ParkMobileData GetSpaceData()
        {
            ParkMobileData spaceData = null;
            try
            {
                using (var client = new HttpClient())
                {
                    client.DefaultRequestHeaders.Accept.Clear();
                    var byteArray = Encoding.ASCII.GetBytes(string.Format("{0}:{1}", this._serviceParameter.UserName, this._serviceParameter.UserPassword));
                    client.DefaultRequestHeaders.Authorization = new System.Net.Http.Headers.AuthenticationHeaderValue("Basic", Convert.ToBase64String(byteArray));
                    HttpResponseMessage response = client.GetAsync(this.uri.ToString()).Result;
                    if (response.IsSuccessStatusCode)
                    {
                        var jsonData = response.Content.ReadAsStringAsync().Result;
                        try
                        {
                            var settings = new Newtonsoft.Json.JsonSerializerSettings()
                            {
                                DateTimeZoneHandling = DateTimeZoneHandling.Utc
                            };
                            spaceData = JsonConvert.DeserializeObject<ParkMobileData>(jsonData, settings);
                            spaceData.StatusCode = response.StatusCode;
                        }
                        catch (Exception ex)
                        {
                            //Error happened
                            spaceData = new ParkMobileData() { StatusCode = response.StatusCode, totalCount = 0, AnyException = ex };
                        }
                    }
                    else
                    {
                        //Other than 200 error code
                        spaceData = new ParkMobileData() { totalCount = 0, StatusCode = response.StatusCode };
                    }
                }
            }
            catch (Exception ex)
            {
                spaceData = new ParkMobileData() { StatusCode = System.Net.HttpStatusCode.SeeOther, totalCount = 0, AnyException = ex };
            }

            return spaceData;
        }
        protected virtual ParkMobileData GetSpaceOrPlateData(ParkingMobileDataType dataType)
        {
            ParkMobileData spaceData = null;
            try
            {
                using (var client = new HttpClient())
                {
                    string currentUrl = string.Empty;
                    if (dataType == ParkingMobileDataType.Plate)
                    {
                        currentUrl = this.plateUri.ToString();
                    }
                    else if (dataType == ParkingMobileDataType.Space)
                    {
                        currentUrl = this.uri.ToString();
                    }

                    client.DefaultRequestHeaders.Accept.Clear();
                    var byteArray = Encoding.ASCII.GetBytes(string.Format("{0}:{1}", this._serviceParameter.UserName, this._serviceParameter.UserPassword));
                    client.DefaultRequestHeaders.Authorization = new System.Net.Http.Headers.AuthenticationHeaderValue("Basic", Convert.ToBase64String(byteArray));
                    HttpResponseMessage response = client.GetAsync(currentUrl).Result;
                    if (response.IsSuccessStatusCode)
                    {
                        var jsonData = response.Content.ReadAsStringAsync().Result;
                        try
                        {
                            var settings = new Newtonsoft.Json.JsonSerializerSettings()
                            {
                                DateTimeZoneHandling = DateTimeZoneHandling.Utc
                            };
                            spaceData = JsonConvert.DeserializeObject<ParkMobileData>(jsonData, settings);
                            spaceData.StatusCode = response.StatusCode;
                        }
                        catch (Exception ex)
                        {
                            //Error happened
                            spaceData = new ParkMobileData() { StatusCode = response.StatusCode, totalCount = 0, AnyException = ex };
                        }
                    }
                    else
                    {
                        //Other than 200 error code
                        spaceData = new ParkMobileData() { totalCount = 0, StatusCode = response.StatusCode };
                    }
                }
            }
            catch (Exception ex)
            {
                spaceData = new ParkMobileData() { StatusCode = System.Net.HttpStatusCode.SeeOther, totalCount = 0, AnyException = ex };
            }

            return spaceData;
        }

        public TransactionTransform GetRefreshSpaceData()
        {
            TransactionTransform refreshPlate = new TransactionTransform() { MeterID = this._parameter.ZoneId.ToString(), SpaceNo = this._parameter.SpaceId.ToString(), ReturnCode = ((int)ReturnCodeEnum.Error).ToString() };
            ParkMobileData spaceData = this.GetSpaceData();
            if (spaceData != null)
            {
                if (spaceData.parkingRights != null && spaceData.StatusCode == System.Net.HttpStatusCode.OK) // Have Entitlement data
                {
                    ParkingRight latestParkingRight = spaceData.parkingRights.OrderByDescending(o => o.endDateLocal.Value).FirstOrDefault<ParkingRight>();
                    refreshPlate.MeterExpiredMinutes = latestParkingRight.ExpiredMinutes(CustomerTimes.GetCustomerTimeById(this.customerId), this._customer.GracePeriodInSecond.ToNullableInt()).ToString();
                    refreshPlate.SpaceNo = latestParkingRight.spaceNumber;
                    refreshPlate.ZoneName = latestParkingRight.internalZoneCode;
                    refreshPlate.LicensePlate = latestParkingRight.lpn;
                    refreshPlate.State = latestParkingRight.lpnState;
                    refreshPlate.ReturnCode = ((int)ReturnCodeEnum.Success).ToString();
                }
                else if (spaceData.StatusCode == System.Net.HttpStatusCode.NotFound)
                {
                    refreshPlate.ReturnCode = ((int)ReturnCodeEnum.Success).ToString();
                    refreshPlate.MeterExpiredMinutes = "-1";
                }

            }
            else
            {
                ///do nothing
            }
            return refreshPlate;
        }
        public TransactionTransform GetRefreshSpaceData(ParkingMobileDataType dataType)
        {
            TransactionTransform refreshPlate = new TransactionTransform() { MeterID = this._parameter.ZoneId.ToString(), SpaceNo = this._parameter.SpaceId.ToString(), ReturnCode = ((int)ReturnCodeEnum.Error).ToString() };
            ParkMobileData spaceData = this.GetSpaceOrPlateData(dataType);
            if (spaceData != null)
            {
                if (spaceData.parkingRights != null && spaceData.StatusCode == System.Net.HttpStatusCode.OK) // Have Entitlement data
                {
                    ParkingRight latestParkingRight = spaceData.parkingRights.OrderByDescending(o => o.endDateLocal.Value).FirstOrDefault<ParkingRight>();
                    refreshPlate.MeterExpiredMinutes = latestParkingRight.ExpiredMinutes(CustomerTimes.GetCustomerTimeById(this.customerId), this._customer.GracePeriodInSecond.ToNullableInt()).ToString();
                    refreshPlate.SpaceNo = latestParkingRight.spaceNumber;
                    refreshPlate.ZoneName = latestParkingRight.internalZoneCode;
                    refreshPlate.LicensePlate = latestParkingRight.lpn;
                    refreshPlate.State = latestParkingRight.lpnState;
                    refreshPlate.ReturnCode = ((int)ReturnCodeEnum.Success).ToString();
                }
                else if (spaceData.StatusCode == System.Net.HttpStatusCode.NotFound)
                {
                    refreshPlate.ReturnCode = ((int)ReturnCodeEnum.Success).ToString();
                    refreshPlate.MeterExpiredMinutes = "-1";
                }

            }
            else
            {
                ///do nothing
            }
            return refreshPlate;
        }


        public TransactionTransform GetRefreshSpaceData(ref VendorResponseEntity venderResonse)
        {
            TransactionTransform refreshPlate = new TransactionTransform() { MeterID = this._parameter.ZoneId.ToString(), SpaceNo = this._parameter.SpaceId.ToString(), ReturnCode = ((int)ReturnCodeEnum.Error).ToString() };
            ParkMobileData spaceData = this.GetSpaceData(ref venderResonse);
            if (spaceData != null)
            {
                if (spaceData.parkingRights != null && spaceData.StatusCode == System.Net.HttpStatusCode.OK) // Have Entitlement data
                {
                    ParkingRight latestParkingRight = spaceData.parkingRights.OrderByDescending(o => o.endDateLocal.Value).FirstOrDefault<ParkingRight>();
                    refreshPlate.MeterExpiredMinutes = latestParkingRight.ExpiredMinutes(CustomerTimes.GetCustomerTimeById(this.customerId), this._customer.GracePeriodInSecond.ToNullableInt()).ToString();
                    refreshPlate.SpaceNo = latestParkingRight.spaceNumber;
                    refreshPlate.ZoneName = latestParkingRight.internalZoneCode;
                    refreshPlate.LicensePlate = latestParkingRight.lpn;
                    refreshPlate.State = latestParkingRight.lpnState;
                    refreshPlate.ReturnCode = ((int)ReturnCodeEnum.Success).ToString();
                }
                else if (spaceData.StatusCode == System.Net.HttpStatusCode.NotFound)
                {
                    refreshPlate.ReturnCode = ((int)ReturnCodeEnum.Success).ToString();
                    refreshPlate.MeterExpiredMinutes = "-1";
                }
            }
            else
            {
                ///do nothing
            }
            return refreshPlate;
        }
        public TransactionTransform GetRefreshSpaceData(ParkingMobileDataType dataType, ref VendorResponseEntity venderResonse)
        {
            TransactionTransform refreshPlate = new TransactionTransform() { MeterID = this._parameter.ZoneId.ToString(), SpaceNo = this._parameter.SpaceId.ToString(), ReturnCode = ((int)ReturnCodeEnum.Error).ToString() };
            ParkMobileData spaceData = this.GetSpaceOrPlateData(dataType, ref venderResonse);
            if (spaceData != null)
            {
                if (spaceData.parkingRights != null && spaceData.StatusCode == System.Net.HttpStatusCode.OK) // Have Entitlement data
                {
                    ParkingRight latestParkingRight = spaceData.parkingRights.OrderByDescending(o => o.endDateLocal.Value).FirstOrDefault<ParkingRight>();
                    refreshPlate.MeterExpiredMinutes = latestParkingRight.ExpiredMinutes(CustomerTimes.GetCustomerTimeById(this.customerId), this._customer.GracePeriodInSecond.ToNullableInt()).ToString();
                    refreshPlate.SpaceNo = latestParkingRight.spaceNumber;
                    refreshPlate.ZoneName = latestParkingRight.internalZoneCode;
                    refreshPlate.LicensePlate = latestParkingRight.lpn;
                    refreshPlate.State = latestParkingRight.lpnState;
                    refreshPlate.ReturnCode = ((int)ReturnCodeEnum.Success).ToString();
                }
                else if (spaceData.StatusCode == System.Net.HttpStatusCode.NotFound)
                {
                    refreshPlate.ReturnCode = ((int)ReturnCodeEnum.Success).ToString();
                    refreshPlate.MeterExpiredMinutes = "-1";
                }

            }
            else
            {
                ///do nothing
            }
            return refreshPlate;
        }
        public TransactionTransform GetRefreshSpaceZoneData(ParkingMobileDataType dataType,string zone, ref VendorResponseEntity venderResonse)
        {
            TransactionTransform refreshPlate = new TransactionTransform() { MeterID = this._parameter.ZoneId.ToString(), SpaceNo = this._parameter.SpaceId.ToString(), ReturnCode = ((int)ReturnCodeEnum.Error).ToString() };
            ParkMobileData spaceData = this.GetSpaceOrPlateData(dataType, ref venderResonse);
            if (spaceData != null)
            {
                if (spaceData.parkingRights != null && spaceData.StatusCode == System.Net.HttpStatusCode.OK) // Have Entitlement data
                {
                    //ParkingRight latestParkingRight = spaceData.parkingRights.OrderByDescending(o => o.endDateLocal.Value).FirstOrDefault<ParkingRight>();
                    ParkingRight latestParkingRight = spaceData.parkingRights.Where(p => p.internalZoneCode.ToLower() == zone.Trim().ToLower()).OrderByDescending(o => o.endDateLocal.Value).FirstOrDefault<ParkingRight>();
                    if (latestParkingRight != null)
                    {
                        refreshPlate.MeterExpiredMinutes = latestParkingRight.ExpiredMinutes(CustomerTimes.GetCustomerTimeById(this.customerId), this._customer.GracePeriodInSecond.ToNullableInt()).ToString();
                        refreshPlate.SpaceNo = latestParkingRight.spaceNumber;
                        refreshPlate.ZoneName = latestParkingRight.internalZoneCode;
                        refreshPlate.LicensePlate = latestParkingRight.lpn;
                        refreshPlate.State = latestParkingRight.lpnState;
                        refreshPlate.ReturnCode = ((int)ReturnCodeEnum.Success).ToString();
                    }
                    else
                    {
                        refreshPlate.ReturnCode = ((int)ReturnCodeEnum.Success).ToString();
                        refreshPlate.MeterExpiredMinutes = "-1";
                    }
                }
                else if (spaceData.StatusCode == System.Net.HttpStatusCode.NotFound)
                {
                    refreshPlate.ReturnCode = ((int)ReturnCodeEnum.Success).ToString();
                    refreshPlate.MeterExpiredMinutes = "-1";
                }

            }
            else
            {
                ///do nothing
            }
            return refreshPlate;
        }
        protected virtual ParkMobileData GetSpaceOrPlateData(ParkingMobileDataType dataType, ref VendorResponseEntity venderResonse)
        {
            ParkMobileData spaceData = null;
            try
            {
                using (var client = new HttpClient())
                {
                    string currentUrl = string.Empty;
                    if (dataType == ParkingMobileDataType.Plate)
                    {
                        //currentUrl = UrlBuilderParkMobile.BuildPlateUrl(this.plateUri, this._parameter.PlateNo);
                        currentUrl = this.plateUri.ToString();
                    }
                    else if (dataType == ParkingMobileDataType.Space)
                    {
                        currentUrl = this.uri.ToString();
                        //currentUrl = UrlBuilderParkMobile.BuildSpaceUrl(this.uri, this._parameter.ZoneId.ToString(), this._parameter.SpaceId.ToString());
                    }

                    ServicePointManager.Expect100Continue = true;
                    ServicePointManager.SecurityProtocol = SecurityProtocolType.Tls12;
                    client.DefaultRequestHeaders.Accept.Clear();

                    var byteArray = Encoding.ASCII.GetBytes(string.Format("{0}:{1}", this._serviceParameter.UserName, this._serviceParameter.UserPassword));
                    client.DefaultRequestHeaders.Authorization = new System.Net.Http.Headers.AuthenticationHeaderValue("Basic", Convert.ToBase64String(byteArray));
                    client.DefaultRequestHeaders.Add("x-api-key", this._customer.Token);

                    venderResonse.RequestURL = currentUrl;
                    HttpResponseMessage response = client.GetAsync(currentUrl).Result;
                    if (response.IsSuccessStatusCode)
                    {
                        var jsonData = response.Content.ReadAsStringAsync().Result;
                        venderResonse.XmlandJsonData = jsonData;
                        venderResonse.ResponseHttpStatusCode = response.StatusCode;
                       
                        try
                        {
                            var settings = new Newtonsoft.Json.JsonSerializerSettings()
                            {
                                DateTimeZoneHandling = DateTimeZoneHandling.Utc
                            };
                            spaceData = JsonConvert.DeserializeObject<ParkMobileData>(jsonData, settings);
                            spaceData.StatusCode = response.StatusCode;

                        }
                        catch (Exception ex)
                        {
                            //Error happened
                            spaceData = new ParkMobileData() { StatusCode = response.StatusCode, totalCount = 0, AnyException = ex };
                        }
                    }
                    else
                    {
                        //Other than 200 error code
                        spaceData = new ParkMobileData() { totalCount = 0, StatusCode = response.StatusCode };
                        venderResonse.ResponseHttpStatusCode = response.StatusCode;
                    }
                }
            }
            catch (Exception ex)
            {
                venderResonse.GeneralException = ex;
                spaceData = new ParkMobileData() { StatusCode = System.Net.HttpStatusCode.SeeOther, totalCount = 0, AnyException = ex };
            }

            return spaceData;
        }
        protected virtual ParkMobileData GetSpaceData(ref VendorResponseEntity venderResonse)
        {
            ParkMobileData spaceData = null;
            try
            {
                using (var client = new HttpClient())
                {
                    client.DefaultRequestHeaders.Accept.Clear();
                    var byteArray = Encoding.ASCII.GetBytes(string.Format("{0}:{1}", this._serviceParameter.UserName, this._serviceParameter.UserPassword));
                    client.DefaultRequestHeaders.Authorization = new System.Net.Http.Headers.AuthenticationHeaderValue("Basic", Convert.ToBase64String(byteArray));
                    HttpResponseMessage response = client.GetAsync(this.uri.ToString()).Result;
                    venderResonse.RequestURL = this.uri.ToString();
                    if (response.IsSuccessStatusCode)
                    {
                        var jsonData = response.Content.ReadAsStringAsync().Result;
                        venderResonse.XmlandJsonData = jsonData;
                        venderResonse.ResponseHttpStatusCode = response.StatusCode;
                        try
                        {
                            var settings = new Newtonsoft.Json.JsonSerializerSettings()
                            {
                                DateTimeZoneHandling = DateTimeZoneHandling.Utc
                            };
                            spaceData = JsonConvert.DeserializeObject<ParkMobileData>(jsonData, settings);
                            spaceData.StatusCode = response.StatusCode;
                        }
                        catch (Exception ex)
                        {
                            venderResonse.GeneralException = ex;
                            //Error happened
                            spaceData = new ParkMobileData() { StatusCode = response.StatusCode, totalCount = 0, AnyException = ex };
                        }
                    }
                    else
                    {
                        venderResonse.ResponseHttpStatusCode = response.StatusCode;
                        //Other than 200 error code
                        spaceData = new ParkMobileData() { totalCount = 0, StatusCode = response.StatusCode };
                    }
                }
            }
            catch (Exception ex)
            {
                venderResonse.GeneralException = ex;
                spaceData = new ParkMobileData() { StatusCode = System.Net.HttpStatusCode.SeeOther, totalCount = 0, AnyException = ex };
            }

            return spaceData;
        }
    }

}
