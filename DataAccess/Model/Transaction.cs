using System;
using System.Collections.Generic;
using System.Runtime.Serialization;

namespace DataAccess.Model
{
    public class Transaction
    {
        public string EnforcementKey { get; set; }

        public string CustomerId { get; set; }
        // public int CustomerId { get; set; }


        public int ParkingSpaceID { get; set; }


        public string PlateNumber { get; set; }


        public string MeterName { get; set; }


        public double? Latitude { get; set; }
        //public decimal? Latitude { get; set; }


        public double? Longitude { get; set; }
        //public decimal? Longitude { get; set; }


        public string LPLocation { get; set; }

        public string VMake { get; set; }


        public string VModel { get; set; }


        public string VColour { get; set; }


        public string MeterStreet { get; set; }


        public string Location { get; set; }


        public int? EnfVendorStallId { get; set; }


        public DateTime? PresentMeterTime { get; set; }


        public string SpaceStatus { get; set; }


        public DateTime? SensorEventTime { get; set; }


        public int? ExpiredMinutes { get; set; }


        public decimal? AmountIncents { get; set; }


        public DateTime? ExpiryTime { get; set; }


        public int? TimeZoneId { get; set; }


        public int? PayStationID { get; set; }


        public int? ZoneID { get; set; }


        public string ZoneName { get; set; }


        public string Block { get; set; }


        public string Direction { get; set; }


        public string StreetType { get; set; }


        public string CrossStreet1 { get; set; }


        public string CrossStreet2 { get; set; }


        public int? DigiELType { get; set; }


        public string DigiErrorDescr { get; set; }


        public int? GeneticELType { get; set; }


        public string GeneticErrorDescr { get; set; }


        public int? PMELType { get; set; }


        public string PMErrorDescr { get; set; }


        public string EnfHourDesc { get; set; }


        public DateTime? MarkedSince { get; set; }


        public string HitDescription { get; set; }


        public string ReturnCode { get; set; }


        public int? MeterStatus { get; set; }

        public long VendorId { get; set; }


        public string MeterId { get; set; }


        public string METERLOC_METERTYPE { get; set; }


        public string State { get; set; }

        public string RateNumber { get; set; }

        public string Recieptid { get; set; }


        public int RemainingTime(int gracePeriodinMinutes)
        {
                int retValue = -1;
                if (this.PresentMeterTime.HasValue && this.ExpiryTime.HasValue)
                {
                    int totalMinutes =(int)((this.ExpiryTime.Value.Subtract(this.PresentMeterTime.Value)).TotalMinutes);
                    totalMinutes = totalMinutes + gracePeriodinMinutes;
                    retValue = totalMinutes;
                }
                return retValue;
        }
    }


    public class Transactions
    {
        private List<Transaction> _data;
        public Transactions()
        {
            _data = new List<Transaction>();
        }

        public List<Transaction> GetTransactions
        {
            get
            {
                return this._data;
            }
            set
            {
                this._data = value;
            }
        }

        public string ReturnCode { get; set; }
    }
}

