using System;
using System.Collections.Generic;
using System.Globalization;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace DataAccess.Model.Cale
{
    public class CaleParameter
    {
        private string _plateNo;
        private string _state;

        public CaleParameter(string plateNo)
        {
            this._plateNo = plateNo;
        }
        public CaleParameter(string plateNo, string state)
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

        public int ExpiredMinutes( int? gracePeriodInSeconds, DateTime endDateUtc)
        {
            int retValue = -1;
            try
            {
                if (true)
                {
                    DateTime toEndTime = DateTime.UtcNow;
                    //return (int)(toEndTime.AddSeconds(gracePeriodInSeconds.Value).Subtract(endDateUtc).TotalMinutes);
                    return (int)(endDateUtc.AddSeconds(gracePeriodInSeconds.Value).Subtract(toEndTime).TotalMinutes);
                }
            }
            catch (Exception ex)
            {

            }
            return retValue;




        }
    }
}
