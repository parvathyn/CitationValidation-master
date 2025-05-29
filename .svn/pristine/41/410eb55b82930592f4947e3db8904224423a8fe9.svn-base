
using System;
using System.Globalization;
namespace DataAccess.Model.Pango
{
    public class PangoParameter
    {
        private string _plateNo;
        private string _state;

        public PangoParameter(string plateNo)
        {
            this._plateNo = plateNo;
        }
        public PangoParameter(string plateNo, string state)
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
                  
                    if(success)
                    {
                        DateTime toEndTime = new DateTime( customerTime.CurrentTime.Value.Year, customerTime.CurrentTime.Value.Month, customerTime.CurrentTime.Value.Day, toTime.Hour, toTime.Minute,0);

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

    public class PangoParameterHttpClient
    {
        private string _plateNo;
        private string _state;
        private string _url = string.Empty;
        private string _action = string.Empty;

        public PangoParameterHttpClient(string plateNo)
        {
            this._plateNo = plateNo;
        }
        public PangoParameterHttpClient(string plateNo, string state)
        {
            this._plateNo = plateNo;
            this._state = state;
        }
        public PangoParameterHttpClient(string plateNo, string state, string url, string action)
        {
            this._plateNo = plateNo;
            this._state = state;
            this._url = url;
            this._action = action;
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
}
