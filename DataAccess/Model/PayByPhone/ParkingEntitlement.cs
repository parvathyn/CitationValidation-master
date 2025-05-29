using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace DataAccess.Model.PayByPhone
{
    public class ParkingEntitlement
    {
        public string plate { get; set; }
        public string locationNumber { get; set; }
        public DateTime? startDateTime { get; set; }
        public DateTime? endDateTime { get; set; }
        public bool isActive { get; set; }
        public string eligibilityStatus { get; set; }
        public string vendorLocationId { get; set; }
        public string stall { get; set; }
        public string vehicleState { get; set; }
        public string vehicleType { get; set; }
        public IList<Amount> amounts { get; set; }

        public DateTime? CustomerStartDateTime(TimeSpan timeSpan)
        {
            if (this.startDateTime.HasValue)
                return this.startDateTime.Value.Add(timeSpan);
            else
                return null;

        }

        public DateTime? CustomerEndDateTime(TimeSpan timeSpan)
        {
            if (this.endDateTime.HasValue)
                return this.endDateTime.Value.Add(timeSpan);
            else
                return null;
        }

        public int ExpiredMinutes(CustomerTime customerTime)
        {
            if (this.CustomerEndDateTime(customerTime.TimeSpanDifferenceToUTC).HasValue)
            {
                return (int)this.CustomerEndDateTime(customerTime.TimeSpanDifferenceToUTC).Value.AddMinutes(0).Subtract(customerTime.CurrentTime.Value).TotalMinutes;
            }
            else
            {
                return (-1);
            }
        }
    }
}
