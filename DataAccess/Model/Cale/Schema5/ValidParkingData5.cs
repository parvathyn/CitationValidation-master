using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using System.Xml.Serialization;

namespace DataAccess.Model.Cale.Schema5
{

    [System.SerializableAttribute()]
    [System.Xml.Serialization.XmlTypeAttribute(Namespace = "http://schema.caleaccess.com/cwo2exportservice/Enforcement/5/")]
    [System.Xml.Serialization.XmlRootAttribute(Namespace = "http://schema.caleaccess.com/cwo2exportservice/Enforcement/5/", IsNullable = true)]
    public partial class ArrayOfValidParkingData
    {

        //private ValidParkingData[] validParkingDataField;


        //[System.Xml.Serialization.XmlElementAttribute("ValidParkingData", IsNullable=true)]
        //public ValidParkingData[] ValidParkingData {
        //    get {
        //        return this.validParkingDataField;
        //    }
        //    set {
        //        this.validParkingDataField = value;
        //    }
        //}

        private List<ValidParkingData> _validParkingDataField;

        public ArrayOfValidParkingData()
        {
            _validParkingDataField = new List<ValidParkingData>();
        }
        [System.Xml.Serialization.XmlElementAttribute("ValidParkingData")]
        public List<ValidParkingData> ValidParkingData
        {
            get
            {
                return this._validParkingDataField;
            }
            set
            {
                this._validParkingDataField = value;
            }
        }

    }



    [System.SerializableAttribute()]
    [System.Xml.Serialization.XmlTypeAttribute(Namespace = "http://schema.caleaccess.com/cwo2exportservice/Enforcement/5/")]
    [System.Xml.Serialization.XmlRootAttribute(Namespace = "http://schema.caleaccess.com/cwo2exportservice/Enforcement/5/", IsNullable = true)]
    public partial class ValidParkingData
    {

        private System.Nullable<float> amountField;

        private Article articleField;

        private string codeField;

        private System.Nullable<bool> containsTerminalOutOfCommunicationField;

        private bool containsTerminalOutOfCommunicationFieldSpecified;

        private System.DateTime dateChangedUtcField;

        private System.DateTime dateCreatedUtcField;

        private System.Nullable<System.DateTime> endDateUtcField;

        private bool endDateUtcFieldSpecified;

        private bool isExpiredField;

        private ParkingSpace parkingSpaceField;

        private ParkingZone parkingZoneField;

        private PostPayment postPaymentField;

        private System.DateTime purchaseUtcField;

        private System.DateTime startDateUtcField;

        private Tariff tariffField;

        private Terminal terminalField;

        private int ticketNumberField;

        private string zoneField;

        private string originCountryStateField;


        [System.Xml.Serialization.XmlElementAttribute(IsNullable = true)]
        public System.Nullable<float> Amount
        {
            get
            {
                return this.amountField;
            }
            set
            {
                this.amountField = value;
            }
        }


        [System.Xml.Serialization.XmlElementAttribute(IsNullable = true)]
        public Article Article
        {
            get
            {
                return this.articleField;
            }
            set
            {
                this.articleField = value;
            }
        }


        [System.Xml.Serialization.XmlElementAttribute(IsNullable = true)]
        public string Code
        {
            get
            {
                return this.codeField;
            }
            set
            {
                this.codeField = value;
            }
        }


        [System.Xml.Serialization.XmlElementAttribute(IsNullable = true)]
        public System.Nullable<bool> ContainsTerminalOutOfCommunication
        {
            get
            {
                return this.containsTerminalOutOfCommunicationField;
            }
            set
            {
                this.containsTerminalOutOfCommunicationField = value;
            }
        }


        [System.Xml.Serialization.XmlIgnoreAttribute()]
        public bool ContainsTerminalOutOfCommunicationSpecified
        {
            get
            {
                return this.containsTerminalOutOfCommunicationFieldSpecified;
            }
            set
            {
                this.containsTerminalOutOfCommunicationFieldSpecified = value;
            }
        }


        public System.DateTime DateChangedUtc
        {
            get
            {
                return this.dateChangedUtcField;
            }
            set
            {
                this.dateChangedUtcField = value;
            }
        }


        public System.DateTime DateCreatedUtc
        {
            get
            {
                return this.dateCreatedUtcField;
            }
            set
            {
                this.dateCreatedUtcField = value;
            }
        }


        [System.Xml.Serialization.XmlElementAttribute(IsNullable = true)]
        public System.Nullable<System.DateTime> EndDateUtc
        {
            get
            {
                return this.endDateUtcField;
            }
            set
            {
                this.endDateUtcField = value;
            }
        }


        [System.Xml.Serialization.XmlIgnoreAttribute()]
        public bool EndDateUtcSpecified
        {
            get
            {
                return this.endDateUtcFieldSpecified;
            }
            set
            {
                this.endDateUtcFieldSpecified = value;
            }
        }


        public bool IsExpired
        {
            get
            {
                return this.isExpiredField;
            }
            set
            {
                this.isExpiredField = value;
            }
        }


        [System.Xml.Serialization.XmlElementAttribute(IsNullable = true)]
        public ParkingSpace ParkingSpace
        {
            get
            {
                return this.parkingSpaceField;
            }
            set
            {
                this.parkingSpaceField = value;
            }
        }


        [System.Xml.Serialization.XmlElementAttribute(IsNullable = true)]
        public ParkingZone ParkingZone
        {
            get
            {
                return this.parkingZoneField;
            }
            set
            {
                this.parkingZoneField = value;
            }
        }


        [System.Xml.Serialization.XmlElementAttribute(IsNullable = true)]
        public PostPayment PostPayment
        {
            get
            {
                return this.postPaymentField;
            }
            set
            {
                this.postPaymentField = value;
            }
        }


        public System.DateTime PurchaseUtc
        {
            get
            {
                return this.purchaseUtcField;
            }
            set
            {
                this.purchaseUtcField = value;
            }
        }


        public System.DateTime StartDateUtc
        {
            get
            {
                return this.startDateUtcField;
            }
            set
            {
                this.startDateUtcField = value;
            }
        }


        [System.Xml.Serialization.XmlElementAttribute(IsNullable = true)]
        public Tariff Tariff
        {
            get
            {
                return this.tariffField;
            }
            set
            {
                this.tariffField = value;
            }
        }


        [System.Xml.Serialization.XmlElementAttribute(IsNullable = true)]
        public Terminal Terminal
        {
            get
            {
                return this.terminalField;
            }
            set
            {
                this.terminalField = value;
            }
        }


        public int TicketNumber
        {
            get
            {
                return this.ticketNumberField;
            }
            set
            {
                this.ticketNumberField = value;
            }
        }


        [System.Xml.Serialization.XmlElementAttribute(IsNullable = true)]
        public string Zone
        {
            get
            {
                return this.zoneField;
            }
            set
            {
                this.zoneField = value;
            }
        }


        [System.Xml.Serialization.XmlElementAttribute(IsNullable = true)]
        public string OriginCountryState
        {
            get
            {
                return this.originCountryStateField;
            }
            set
            {
                this.originCountryStateField = value;
            }
        }
    }



    [System.SerializableAttribute()]
    [System.Xml.Serialization.XmlTypeAttribute(Namespace = "http://schema.caleaccess.com/cwo2exportservice/Enforcement/5/")]
    [System.Xml.Serialization.XmlRootAttribute(Namespace = "http://schema.caleaccess.com/cwo2exportservice/Enforcement/5/", IsNullable = true)]
    public partial class Article
    {

        private string idField;

        private string nameField;


        [System.Xml.Serialization.XmlElementAttribute(IsNullable = true)]
        public string Id
        {
            get
            {
                return this.idField;
            }
            set
            {
                this.idField = value;
            }
        }


        [System.Xml.Serialization.XmlElementAttribute(IsNullable = true)]
        public string Name
        {
            get
            {
                return this.nameField;
            }
            set
            {
                this.nameField = value;
            }
        }
    }



    [System.SerializableAttribute()]
    [System.Xml.Serialization.XmlTypeAttribute(Namespace = "http://schema.caleaccess.com/cwo2exportservice/Enforcement/5/")]
    [System.Xml.Serialization.XmlRootAttribute(Namespace = "http://schema.caleaccess.com/cwo2exportservice/Enforcement/5/", IsNullable = true)]
    public partial class ParkingSpace
    {

        private string idField;

        private string locationField;


        [System.Xml.Serialization.XmlElementAttribute(IsNullable = true)]
        public string Id
        {
            get
            {
                return this.idField;
            }
            set
            {
                this.idField = value;
            }
        }


        [System.Xml.Serialization.XmlElementAttribute(IsNullable = true)]
        public string Location
        {
            get
            {
                return this.locationField;
            }
            set
            {
                this.locationField = value;
            }
        }
    }



    [System.SerializableAttribute()]
    [System.Xml.Serialization.XmlTypeAttribute(Namespace = "http://schema.caleaccess.com/cwo2exportservice/Enforcement/5/")]
    [System.Xml.Serialization.XmlRootAttribute(Namespace = "http://schema.caleaccess.com/cwo2exportservice/Enforcement/5/", IsNullable = true)]
    public partial class ParkingZone
    {

        private string keyField;

        private string nameField;

        private string numberField;


        [System.Xml.Serialization.XmlElementAttribute(IsNullable = true)]
        public string Key
        {
            get
            {
                return this.keyField;
            }
            set
            {
                this.keyField = value;
            }
        }


        [System.Xml.Serialization.XmlElementAttribute(IsNullable = true)]
        public string Name
        {
            get
            {
                return this.nameField;
            }
            set
            {
                this.nameField = value;
            }
        }


        [System.Xml.Serialization.XmlElementAttribute(IsNullable = true)]
        public string Number
        {
            get
            {
                return this.numberField;
            }
            set
            {
                this.numberField = value;
            }
        }
    }



    [System.SerializableAttribute()]
    [System.Xml.Serialization.XmlTypeAttribute(Namespace = "http://schema.caleaccess.com/cwo2exportservice/Enforcement/5/")]
    [System.Xml.Serialization.XmlRootAttribute(Namespace = "http://schema.caleaccess.com/cwo2exportservice/Enforcement/5/", IsNullable = true)]
    public partial class PostPayment
    {

        private string postPaymentNetworkNameField;

        private System.Nullable<int> postPaymentTransactionIDField;

        private bool postPaymentTransactionIDFieldSpecified;

        private System.Nullable<byte> postPaymentTransactionStatusKeyField;

        private bool postPaymentTransactionStatusKeyFieldSpecified;


        [System.Xml.Serialization.XmlElementAttribute(IsNullable = true)]
        public string PostPaymentNetworkName
        {
            get
            {
                return this.postPaymentNetworkNameField;
            }
            set
            {
                this.postPaymentNetworkNameField = value;
            }
        }


        [System.Xml.Serialization.XmlElementAttribute(IsNullable = true)]
        public System.Nullable<int> PostPaymentTransactionID
        {
            get
            {
                return this.postPaymentTransactionIDField;
            }
            set
            {
                this.postPaymentTransactionIDField = value;
            }
        }


        [System.Xml.Serialization.XmlIgnoreAttribute()]
        public bool PostPaymentTransactionIDSpecified
        {
            get
            {
                return this.postPaymentTransactionIDFieldSpecified;
            }
            set
            {
                this.postPaymentTransactionIDFieldSpecified = value;
            }
        }


        [System.Xml.Serialization.XmlElementAttribute(IsNullable = true)]
        public System.Nullable<byte> PostPaymentTransactionStatusKey
        {
            get
            {
                return this.postPaymentTransactionStatusKeyField;
            }
            set
            {
                this.postPaymentTransactionStatusKeyField = value;
            }
        }


        [System.Xml.Serialization.XmlIgnoreAttribute()]
        public bool PostPaymentTransactionStatusKeySpecified
        {
            get
            {
                return this.postPaymentTransactionStatusKeyFieldSpecified;
            }
            set
            {
                this.postPaymentTransactionStatusKeyFieldSpecified = value;
            }
        }
    }



    [System.SerializableAttribute()]
    [System.Xml.Serialization.XmlTypeAttribute(Namespace = "http://schema.caleaccess.com/cwo2exportservice/Enforcement/5/")]
    [System.Xml.Serialization.XmlRootAttribute(Namespace = "http://schema.caleaccess.com/cwo2exportservice/Enforcement/5/", IsNullable = true)]
    public partial class Tariff
    {

        private string idField;

        private string nameField;


        [System.Xml.Serialization.XmlElementAttribute(IsNullable = true)]
        public string Id
        {
            get
            {
                return this.idField;
            }
            set
            {
                this.idField = value;
            }
        }


        [System.Xml.Serialization.XmlElementAttribute(IsNullable = true)]
        public string Name
        {
            get
            {
                return this.nameField;
            }
            set
            {
                this.nameField = value;
            }
        }
    }



    [System.SerializableAttribute()]
    [System.Xml.Serialization.XmlTypeAttribute(Namespace = "http://schema.caleaccess.com/cwo2exportservice/Enforcement/5/")]
    [System.Xml.Serialization.XmlRootAttribute(Namespace = "http://schema.caleaccess.com/cwo2exportservice/Enforcement/5/", IsNullable = true)]
    public partial class Terminal
    {

        private string idField;

        private string latitudeField;

        private string longitudeField;

        private string parentNodeField;


        [System.Xml.Serialization.XmlElementAttribute(IsNullable = true)]
        public string Id
        {
            get
            {
                return this.idField;
            }
            set
            {
                this.idField = value;
            }
        }


        [System.Xml.Serialization.XmlElementAttribute(IsNullable = true)]
        public string Latitude
        {
            get
            {
                return this.latitudeField;
            }
            set
            {
                this.latitudeField = value;
            }
        }


        [System.Xml.Serialization.XmlElementAttribute(IsNullable = true)]
        public string Longitude
        {
            get
            {
                return this.longitudeField;
            }
            set
            {
                this.longitudeField = value;
            }
        }


        [System.Xml.Serialization.XmlElementAttribute(IsNullable = true)]
        public string ParentNode
        {
            get
            {
                return this.parentNodeField;
            }
            set
            {
                this.parentNodeField = value;
            }
        }
    }

}
