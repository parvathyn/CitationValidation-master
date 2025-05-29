using System.Collections.Generic;
using System.Runtime.Serialization;

namespace DataAccess.Model
{
    [DataContract]
    public class PaymentData
    {
        [DataMember]
        public string MeterID { get; set; }

        [DataMember]
        public string MeterName { get; set; }

        [DataMember]
        public string SpaceNo { get; set; }

        //[DataMember]
        public string MeterStreet { get; set; }

       // [DataMember]
        public string MeterRTC { get; set; }

        //[DataMember]
        public string MeterLastUpdateTime { get; set; }

        //[DataMember]
        //public DateTime MeterLastUpdateTime { get; set; }

        [DataMember]
        public string PaymentTime { get; set; }

        //[DataMember]
        //public DateTime SensorEventTime { get; set; }

        [DataMember]
        public string ExpiredTime { get; set; }

        //[DataMember]
        //public DateTime ExpiredTime { get; set; }

        //[DataMember]
        public string MeterExpiredMinutes { get; set; }

        //[DataMember]
        public string METERLOC_BLOCKNUMBER { get; set; }

        //[DataMember]
        public string METERLOC_STREETDIRECTION { get; set; }

        [DataMember]
        public string METERLOC_STREETNAME { get; set; }

        //[DataMember]
        public string METERLOC_STREETTYPE { get; set; }

        //[DataMember]
        public string METERLOC_CROSSSTREET1 { get; set; }

        //[DataMember]
        public string METERLOC_CROSSSTREET2 { get; set; }

        //[DataMember]
        public string METERLOC_ENFORCEMENTHOURSDESC { get; set; }

        [DataMember]
        public string LicensePlate { get; set; }

        //[DataMember]
        public string State { get; set; }

        //[DataMember]
        public string Make { get; set; }

        //[DataMember]
        public string Model { get; set; }

        //[DataMember]
        public string IsOccupied { get; set; }

        [DataMember]
        public decimal? AmountIncents { get; set; }

        //[DataMember]
        public string ReturnCode { get; set; }

        [DataMember]
        public string PaymentDuration { get; set; }

        [DataMember]
        public string PaymentDay { get; set; }
    }

    [DataContract]
    public class PaymentsData
    {
        private List<PaymentData> _data;
        public PaymentsData()
        {
            _data = new List<PaymentData>();
        }

        [DataMember]
        public List<PaymentData> PaymentTransactions
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
        [DataMember]
        public string ReturnCode { get; set; }
    }
}
