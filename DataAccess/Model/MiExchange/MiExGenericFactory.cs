using System;

namespace DataAccess.Model.MiExchange
{
    public class MiExGenericFactory
    {
        protected MiExParameter _parameter;
        protected MiExCustomerElement _customer;
        protected int customerId;
        protected MiExchangeService.OutgoingSoapClient _service;
        public MiExGenericFactory(MiExCustomerElement customer, string plateNo, string zonelist)
        {
            this._customer = customer;
            this.customerId = Convert.ToInt32(customer.CustomerId);
            this._parameter = new MiExParameter(plateNo,zonelist);
            _service = new MiExchangeService.OutgoingSoapClient();
        }

        public virtual TransactionTransform GetPlateData(ref VendorResponseEntity venderResonse)
        {
            TransactionTransform refreshPlate = new TransactionTransform() { ReturnCode = ((int)ReturnCodeEnum.Error).ToString(), MeterExpiredMinutes="-1" };
            try
            {
                bool ixExpired = true;
                venderResonse.RequestURL = this._service.Endpoint.Address.ToString();
                venderResonse.MiExRequestData= string.Format("Plate No : {0}", this._parameter.PlateNo);
                var authenticateResult = this._service.Authenticate(this._customer.UserName, this._customer.UserPassword, this._customer.SiteCode, Guid.NewGuid().ToString());
                if (!string.IsNullOrWhiteSpace(authenticateResult))
                {
                    ixExpired = this._service.IsPlateExpired(authenticateResult, this._parameter.PlateNo, this._parameter.Zonelist);
                    venderResonse.ResponseHttpStatusCode = System.Net.HttpStatusCode.OK;
                    refreshPlate.ReturnCode = ((int)ReturnCodeEnum.Success).ToString();
                    venderResonse.MiExResponse = string.Format("Method IsPlateExpired Value : {0}", ixExpired.ToString());
                    if (ixExpired == false)
                        refreshPlate.MeterExpiredMinutes = "1";
                }
                else
                {
                    venderResonse.MiExResponse = string.Format("Method Authenticate respone : {0}", authenticateResult.ToString());
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
            }
            return refreshPlate;
        }

        private void GetTransaction()
        {
           
        }
    }
}
