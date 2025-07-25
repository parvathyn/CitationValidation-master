﻿using System;
using System.Collections.Generic;
using System.IO;
using System.Linq;
using System.Runtime.Serialization;
using System.Runtime.Serialization.Json;
using System.Text;
using System.Threading.Tasks;

namespace DataAccess.Model
{
    [DataContract]
    public class TransactionTransform
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

        //[DataMember]
        //public DateTime MeterLastUpdateTime { get; set; }

        [DataMember]
        public string SensorEventTime { get; set; }

        //[DataMember]
        //public DateTime SensorEventTime { get; set; }

        [DataMember]
        public string ExpiredTime { get; set; }

        //[DataMember]
        //public DateTime ExpiredTime { get; set; }

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

        public string GetJsonData()
        {
            using( MemoryStream mem =  new MemoryStream())
            {
               DataContractJsonSerializer ser =
               new DataContractJsonSerializer(typeof(TransactionTransform));

                ser.WriteObject(mem, this);
                string data =
               Encoding.UTF8.GetString(mem.ToArray(), 0, (int)mem.Length);
                return data;
            }
        }
    }

    [DataContract]
    public class ChicagoTransactionTransform
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

        [DataMember(EmitDefaultValue = false, Name = "METERLOC_ZONE")]
        public string ZoneName { get; set; }

        public string GetJsonData()
        {
            using (MemoryStream mem = new MemoryStream())
            {
                DataContractJsonSerializer ser =
                new DataContractJsonSerializer(typeof(ChicagoTransactionTransform));

                ser.WriteObject(mem, this);
                string data =
               Encoding.UTF8.GetString(mem.ToArray(), 0, (int)mem.Length);
                return data;
            }
        }
        [DataMember(Name = "RateName")]
        public string RateNumber { get; set; }

       
        public string Recieptid { get; set; }
    }

    [DataContract]
    public class ParkMobile 
    {
        private string _xmlandJsonData;
        private Exception _generalException;
        public ParkMobile()
        {

        }
        [DataMember]
        public System.Net.HttpStatusCode ResponseHttpStatusCode { get; set; }
        [DataMember]
        public string RequestURL { get; set; }
        [DataMember(Name = "Data", EmitDefaultValue = false)]
        public string XmlandJsonData
        {
            get { return this._xmlandJsonData; }
            set { this._xmlandJsonData = value; }
        }
       
        public Exception GeneralException
        {
            get
            {
                return this._generalException;
            }
            set
            {
                this._generalException = value;
            }
        }

        private string _exceptMsg = null;
        [DataMember(Name = "ErrorMsg", EmitDefaultValue = false)]
        public string ExceptMsg
        {
            get
            {
                if(this._generalException != null)
                {
                    this._exceptMsg = this._generalException.Message;
                    return this._exceptMsg;
                }
                else
                {
                    return null;
                }
            }
            set
            {
                this._exceptMsg = value;
            }
        }
    }

    [DataContract]
    public class CaleData
    {
        private string _xmlandJsonData;
        private Exception _generalException;
        public CaleData()
        {

        }
        [DataMember]
        public System.Net.HttpStatusCode ResponseHttpStatusCode { get; set; }
        [DataMember]
        public string RequestURL { get; set; }
        [DataMember(Name = "Data", EmitDefaultValue = false)]
        public string XmlandJsonData
        {
            get { return this._xmlandJsonData; }
            set { this._xmlandJsonData = value; }
        }
      
        public Exception GeneralException
        {
            get
            {
                return this._generalException;
            }
            set
            {
                this._generalException = value;
            }
        }

        private string _exceptMsg = null;
        [DataMember(Name = "ErrorMsg", EmitDefaultValue = false)]
        public string ExceptMsg
        {
            get
            {
                if (this._generalException != null)
                {
                    this._exceptMsg = this._generalException.Message;
                    return this._exceptMsg;
                }
                else
                {
                    return null;
                }
            }
            set
            {
                this._exceptMsg = value;
            }
        }
    }

    [DataContract]
    public class Database
    {
        private Exception _generalException;
        public Database()
        {

        }
     
        public Exception GeneralException
        {
            get
            {
                return this._generalException;
            }
            set
            {
                this._generalException = value;
            }
        }
        private string _exceptMsg = null;
        [DataMember(Name = "ErrorMsg", EmitDefaultValue = false)]
        public string ExceptMsg
        {
            get
            {
                if (this._generalException != null)
                {
                    this._exceptMsg = this._generalException.Message;
                    return this._exceptMsg;
                }
                else
                {
                    return null;
                }
            }
            set
            {
                this._exceptMsg = value;
            }
        }
    }
}
