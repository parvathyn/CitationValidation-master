using System.IO;
using System.Runtime.Serialization;
using System.Runtime.Serialization.Json;
using System.Text;


namespace DataAccess.Model.CitationValidation
{
    [DataContract(Namespace = "")]
    [KnownType(typeof(ChicagoTransactionPaymentDetail))]
    public class TransactionPaymentDetail
    {
        [DataMember]
        public string MeterID { get; set; }

        [DataMember]
        public string MeterName { get; set; }

        [DataMember]
        public string SpaceNo { get; set; }

        [DataMember]
        public string MeterStreet { get; set; }

        [DataMember]
        public string MeterRTC { get; set; }

        [DataMember]
        public string MeterLastUpdateTime { get; set; }

        [DataMember]
        public string SensorEventTime { get; set; }

        [DataMember]
        public string ExpiredTime { get; set; }

        [DataMember]
        public string MeterExpiredMinutes { get; set; }

        [DataMember]
        public string METERLOC_BLOCKNUMBER { get; set; }

        [DataMember]
        public string METERLOC_STREETDIRECTION { get; set; }

        [DataMember]
        public string METERLOC_STREETNAME { get; set; }

        [DataMember]
        public string METERLOC_STREETTYPE { get; set; }

        [DataMember]
        public string METERLOC_CROSSSTREET1 { get; set; }

        [DataMember]
        public string METERLOC_CROSSSTREET2 { get; set; }

        [DataMember]
        public string METERLOC_ENFORCEMENTHOURSDESC { get; set; }

        [DataMember]
        public string LicensePlate { get; set; }

        [DataMember]
        public string State { get; set; }

        [DataMember]
        public string Make { get; set; }

        [DataMember]
        public string Model { get; set; }

        [DataMember]
        public string IsOccupied { get; set; }

        [DataMember]
        public string EnforcementKey { get; set; }

        [DataMember]
        public string ReturnCode { get; set; }

        [DataMember(EmitDefaultValue = false)]
        public int? MeterStatus { get; set; }

        [DataMember(EmitDefaultValue = false)]
        public string METERLOC_METERTYPE { get; set; }

        [DataMember(EmitDefaultValue = false)]
        public string ZoneName { get; set; }

        public virtual string GetJsonData()
        {
            using (MemoryStream mem = new MemoryStream())
            {
                DataContractJsonSerializer ser =
                new DataContractJsonSerializer(typeof(TransactionPaymentDetail));

                ser.WriteObject(mem, this);
                string data =
               Encoding.UTF8.GetString(mem.ToArray(), 0, (int)mem.Length);
                return data;
            }
        }
    }

    [DataContract(Namespace = "", Name = "chicago")]
    public class ChicagoTransactionPaymentDetail : TransactionPaymentDetail
    {
        [DataMember]
        public string RateNumber { get; set; }

        [DataMember]
        public string Recieptid { get; set; }

        public override string GetJsonData()
        {
            using (MemoryStream mem = new MemoryStream())
            {
                DataContractJsonSerializer ser =
                new DataContractJsonSerializer(typeof(ChicagoTransactionPaymentDetail));

                ser.WriteObject(mem, this);
                string data =
               Encoding.UTF8.GetString(mem.ToArray(), 0, (int)mem.Length);

                return data;
            }
        }

    }
}
