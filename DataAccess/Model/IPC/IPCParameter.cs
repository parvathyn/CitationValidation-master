using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace DataAccess.Model.IPC
{
    public  class IPCParameter
    {
        private string _plateNo;
        private string _state;

        public IPCParameter(string plateNo)
        {
            this._plateNo = plateNo;
        }
        public IPCParameter(string plateNo, string state)
        {
            this._plateNo = plateNo;
            this._state = state;
        }

        public string PlateNo
        {
            get
            {
                return this._plateNo;
            }
        }

        public string State
        {
            get
            {
                return this._state;
            }
        }

        public int ExpiredMinutes(int? gracePeriodInSeconds, DateTime customerTime, DateTime expiryTime)
        {
            int retValue = -1;
            try
            {
                if (true)
                {
                    return (int)(expiryTime.AddSeconds(gracePeriodInSeconds.Value).Subtract(customerTime).TotalMinutes);
                }
            }
            catch (Exception ex)
            {

            }
            return retValue;




        }
    }
}
