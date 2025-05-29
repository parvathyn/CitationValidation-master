using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace DataAccess.Model.Parkeon
{
    public class ParkeonResponsePlates
    {
        public string session_id { get; set; }
        public string plate_number { get; set; }
        public string zone_id { get; set; }
        public string start_date { get; set; }
        public string end_date { get; set; }
        public string type { get; set; }

        //In UTC Time
        public DateTime? StartDateDT
        {
            get
            {
                return this.start_date.ToNullableDate();
            }

        }

        public DateTime? EndDateDT
        {
            get
            {
                return this.end_date.ToNullableDate();
            }

        }

        public int? TypeId
        {
            get
            {
                return this.type.ToNullableInt();
            }
        }

        public int? ZoneIdInInt
        {
            get
            {
                return this.zone_id.ToNullableInt();
            }
        }

        public void ValidateMe()
        {
            try
            {
                if (this.StartDateDT.HasValue == false)
                    this.start_date = "2000-01-01 00:00:00";
                if (this.EndDateDT.HasValue == false)
                    this.end_date = "2000-01-01 00:00:00";
                if (this.ZoneIdInInt.HasValue == false)
                    this.zone_id = "0";
            }
            catch (Exception ex)
            {
                throw ex;
            }

        }

        //In Customer Time
        public DateTime? StartDateDTCustomer
        {
            get
            {
                if(this.StartDateDT.HasValue)
                {
                    bool isDayLight = TimeZoneInfo.Local.IsDaylightSavingTime(DateTime.Now);
                    if (isDayLight)
                        return this.StartDateDT.Value.Add(new TimeSpan(-4, 0, 0));
                    else
                        return this.StartDateDT.Value.Add(new TimeSpan(-5, 0, 0));
                }
                else
                {
                    return this.StartDateDT;
                }
               
            }
        }

        public DateTime? EndDateDTCustomer
        {
            get
            {

                if (this.EndDateDT.HasValue)
                {
                    bool isDayLight = TimeZoneInfo.Local.IsDaylightSavingTime(DateTime.Now);
                    if (isDayLight)
                        return this.EndDateDT.Value.Add(new TimeSpan(-4, 0, 0));
                    else
                        return this.EndDateDT.Value.Add(new TimeSpan(-5, 0, 0));
                }
                else
                {
                    return this.EndDateDT;
                }

            }

        }


        public DateTime? PresentMeterTime
        {
            get
            {
                bool isDayLight = TimeZoneInfo.Local.IsDaylightSavingTime(DateTime.Now);
                if (isDayLight)
                    return DateTime.UtcNow.Add(new TimeSpan(-4, 0, 0));
                else
                    return DateTime.UtcNow.Add(new TimeSpan(-5, 0, 0));
            }
        }

        public int ExpiredMinutes
        {
            get   
            {
                if(this.EndDateDTCustomer.HasValue)
                {
                    return (int)this.EndDateDTCustomer.Value.AddMinutes(5).Subtract(this.PresentMeterTime.Value).TotalMinutes;
                }
                else
                {
                    return (Int32.MaxValue * -1);
                }
            }
        }

        public int CalExpiredMinutes(int seconds)
        {
            if (this.EndDateDTCustomer.HasValue)
            {
                return (int)this.EndDateDTCustomer.Value.AddSeconds(seconds).Subtract(this.PresentMeterTime.Value).TotalMinutes;
            }
            else
            {
                return (Int32.MaxValue * -1);
            }
        }
    }
}
