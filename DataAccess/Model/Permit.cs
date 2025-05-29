using System;
using System.Runtime.Serialization;

namespace DataAccess.Model
{
    [DataContract]
    public class Permit
    {
        [DataMember]
        public string permitNo { get; set; }

        [DataMember]
        public string permitType { get; set; }

        [DataMember]
        public string permitSource { get; set; }

        [DataMember]
        public string permitZoneId { get; set; }

        [DataMember]
        public string permitZoneName { get; set; }

        [DataMember]
        public string permitStartDateTime { get; set; }

        [DataMember]
        public string permitEndDateTime { get; set; }

        [DataMember]
        public string PermitTxnDateTime { get; set; }

        [DataMember]
        public string PermitTxnReferenceId { get; set; }

      


        public DateTime? StartDateTime { get; set; }
        public DateTime? EndDateTime { get; set; }
        public DateTime? TxnDateTime { get; set; }

    }
}
