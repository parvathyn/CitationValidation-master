using System;
using System.IO;
using System.Net;
using System.Net.Http;
using System.Xml.Serialization;

namespace DataAccess.Model.AceParking
{
    public class AceParkingHttpClient
    {

        protected AceParkingCustomerElement _customer;
        protected int customerId;
        protected string Plate;

        public AceParkingHttpClient(AceParkingCustomerElement customer, string plateNo)
        {
            this._customer = customer;
            this.customerId = Convert.ToInt32(customer.CustomerId);
            Plate = plateNo;
        }

        public virtual TransactionTransform GetPlateData(ref VendorResponseEntity venderResonse)
        {
            System.Net.ServicePointManager.SecurityProtocol = SecurityProtocolType.Tls12;
            TransactionTransform refreshPlate = new TransactionTransform() { ReturnCode = ((int)ReturnCodeEnum.Error).ToString() };
            AceObject aceData = null;
            try
            {
                //venderResonse.
                using (var client = new HttpClient())
                {
                    System.Net.ServicePointManager.SecurityProtocol = SecurityProtocolType.Tls12 | SecurityProtocolType.Tls11 | SecurityProtocolType.Tls;
                    this._customer.PlateNo = Plate;
                    venderResonse.RequestURL = this._customer.FinalURL;
                    HttpResponseMessage response = client.GetAsync(this._customer.FinalURL).Result;
                    if (response.IsSuccessStatusCode)
                    {
                        string xmlData = response.Content.ReadAsStringAsync().Result;
                        venderResonse.XmlandJsonData = xmlData;
                        venderResonse.ResponseHttpStatusCode = response.StatusCode;
                        ////
                        if(xmlData.Contains("\"plates\":[]"))
                        {
                            aceData = new AceObject();
                        }
                        else
                        {
                            XmlSerializer serlializer = new XmlSerializer(typeof(AceObject));
                            aceData = (AceObject)serlializer.Deserialize(new StringReader(xmlData));
                        }

                    
                        if (aceData != null)
                        {
                            if (aceData.plates != null)
                            {
                                refreshPlate.LicensePlate = Plate;
                                refreshPlate.State = aceData.plates.plate_state;
                                refreshPlate.MeterExpiredMinutes = aceData.plates.ExpiredMinutes(CustomerTimes.GetCustomerTimeById(this.customerId),
                                     this._customer.GracePeriodInSecond.ToNullableInt()).ToString();
                                 
                                if (!string.IsNullOrWhiteSpace(aceData.plates.zone))
                                    refreshPlate.ZoneName = aceData.plates.zone;
                            }
                            else
                            {
                                refreshPlate.LicensePlate = Plate;
                                refreshPlate.MeterExpiredMinutes = "-1";
                            }
                            refreshPlate.ReturnCode = ((int)ReturnCodeEnum.Success).ToString();
                        }
                    }
                    else
                    {
                        venderResonse.ResponseHttpStatusCode = response.StatusCode;
                        venderResonse.XmlandJsonData = response.Content.ReadAsStringAsync().Result;
                        refreshPlate.LicensePlate = Plate;
                        refreshPlate.MeterExpiredMinutes = "-1";
                        refreshPlate.ReturnCode = ((int)ReturnCodeEnum.Success).ToString();
                    }
                }
            }
            catch (System.ServiceModel.CommunicationException commEx)
            {
                venderResonse.GeneralException = commEx;
                refreshPlate.ReturnCode = ((int)ReturnCodeEnum.Error).ToString();
            }
            catch (Exception ex)
            {
                venderResonse.GeneralException = ex;
                refreshPlate.ReturnCode = ((int)ReturnCodeEnum.Error).ToString();
            }

            return refreshPlate;
        }
    }
}
