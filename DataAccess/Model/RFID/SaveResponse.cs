using System;
using System.Runtime.Serialization;

namespace DataAccess.Model.RFID
{
    [DataContract]
    public class SaveResponse
    {
        [DataMember]
        public Int32 ReturnCode { get; set; }
    }


    public enum RFIDReturnCodeEnum : int
    {
        Error = -1,
        Success = 0,
    }

    [DataContract]
    public class RFIDData
    {
        [DataMember]
        public long? Id { get; set; }
        [DataMember]
        public int? CustomerId { get; set; }
        [DataMember]
        public int? AreaId { get; set; }
        [DataMember]
        public int? MeterId { get; set; }
        [DataMember]
        public long? SlNo { get; set; }
        [DataMember]
        public long? SeqNo { get; set; }
        [DataMember]
        public decimal? Latitude { get; set; }
        [DataMember]
        public decimal? Longitude { get; set; }
        [DataMember]
        public string ImageLocation { get; set; }
    }
}
