using System;
using System.Collections.Generic;
using System.Globalization;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace DataAccess.Model.AceParking
{


    public class AceParkingParameter
    {
        private string _plateNo;
        private string _key;
        private int _gracePeriodInSeconds;

        public AceParkingParameter(string plateNo)
        {
            this._plateNo = plateNo;
        }
        public AceParkingParameter(string plateNo, string key, int gracePeriodInSeconds)
        {
            this._plateNo = plateNo;
            this._key = key;
            this._gracePeriodInSeconds = gracePeriodInSeconds;
        }

        public string PlateNo
        {
            get
            {
                return this._plateNo;
            }
        }

        public string Key
        {
            get
            {
                return this._key;
            }
        }

        public int GracePeriodInSeconds
        {
            get
            {
                return this._gracePeriodInSeconds;
            }
        }

        public int ExpiredMinutes(CustomerTime customerTime, int? gracePeriodInSeconds, string endTimeOnly)
        {
            int retValue = -1;
            try
            {
                DateTime toTime;
                if (!String.IsNullOrEmpty((endTimeOnly ?? "").Trim()))
                {
                    string[] formats = { "hhmm", "hmm", @"hh\:mm", @"h\:mm\:ss", @"h:mm", @"h:mm tt" };
                    bool success = DateTime.TryParseExact(endTimeOnly, formats, CultureInfo.InvariantCulture, DateTimeStyles.None, out toTime);

                    if (success)
                    {
                        DateTime toEndTime = new DateTime(customerTime.CurrentTime.Value.Year, customerTime.CurrentTime.Value.Month, customerTime.CurrentTime.Value.Day, toTime.Hour, toTime.Minute, 0);

                        return (int)(toEndTime.AddSeconds(gracePeriodInSeconds.Value).Subtract(customerTime.CurrentTime.Value).TotalMinutes);
                    }
                    else
                        return (-1);
                }
                else
                {
                    return (-1);
                }
            }
            catch (Exception ex)
            {

            }
            return retValue;



            //if (toTime.HasValue)
            //{
            //    return (int)(toTime.Value.AddSeconds(gracePeriodInSeconds.Value).Subtract(customerTime.CurrentTime.Value).TotalMinutes);
            //}
            //else
            //{
            //    return (-1);
            //}
        }
    }

    public class  AceParkingParameterHttpClient
    {
        private string _plateNo;
        private string _key;
        private string _url = string.Empty;
        private string _action = string.Empty;
        private int _gracePeriodInSeconds;

        public AceParkingParameterHttpClient(string plateNo)
        {
            this._plateNo = plateNo;
        }
        public AceParkingParameterHttpClient(string plateNo, string key)
        {
            this._plateNo = plateNo;
            this._key = key;
        }
        public AceParkingParameterHttpClient(string plateNo, string key, string url, string action)
        {
            this._plateNo = plateNo;
            this._key = key;
            this._url = url;
            this._action = action;
        }
        public AceParkingParameterHttpClient(string plateNo, string key, string url, string action, int gracePeriodInSeconds)
        {
            this._plateNo = plateNo;
            this._key = key;
            this._url = url;
            this._action = action;
            this._gracePeriodInSeconds = gracePeriodInSeconds;
        }

        public string PlateNo
        {
            get
            {
                return this._plateNo;
            }
        }

        public string Key
        {
            get
            {
                return this._key;
            }
        }

        public string URL
        {
            get
            {
                return this._url;
            }
        }

        public string Action
        {
            get
            {
                return this._action;
            }
        }

        public int GracePeriodInSeconds
        {
            get
            {
                return this._gracePeriodInSeconds;
            }
        }
    
    }
}
