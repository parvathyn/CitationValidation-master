
using System;
namespace DataAccess.Model.AceParking
{
    public class Plates
    {
        public string id { get; set; }
        public string plate_number { get; set; }
        public string plate_state { get; set; }
        public string reservation_id { get; set; }
        public string user_id { get; set; }
        public string date_start_time { get; set; }
        public string date_end_time { get; set; }
        public string source { get; set; }
        public string updated { get; set; }
        public string date_created { get; set; }
        public string amount { get; set; }
        public string tx_seq { get; set; }
        public string lot_number { get; set; }
        public string zone { get; set; }
        public string receipt_id { get; set; }

        public DateTime? EndDateDT
        {
            get
            {
                return this.date_end_time.ToNullableDate();
            }
        }

        public int ExpiredMinutes(CustomerTime customerTime, int? gracePeriodInSeconds)
        {
            int retValue = -1;
            try
            {
                if(this.EndDateDT.HasValue)
                    return (int)this.EndDateDT.Value.AddSeconds(gracePeriodInSeconds.Value).Subtract(customerTime.CurrentTime.Value).TotalMinutes;
            }
            catch (Exception ex)
            {
            }
            return retValue;
        }
    }
    public class AceObject
    {
        private Plates _plate;
        public AceObject()
        {
            _plate = new Plates();
        }
        public bool status { get; set; }
        public string error { get; set; }
        public Plates plates
        {
            get { return this._plate; }
            set { this._plate = value; }
        }
    }
}
