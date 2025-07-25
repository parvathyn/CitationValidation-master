﻿//------------------------------------------------------------------------------
// <auto-generated>
//     This code was generated by a tool.
//     Runtime Version:4.0.30319.42000
//
//     Changes to this file may cause incorrect behavior and will be lost if
//     the code is regenerated.
// </auto-generated>
//------------------------------------------------------------------------------

namespace DataAccess.PangoProdService {
    
    
    [System.CodeDom.Compiler.GeneratedCodeAttribute("System.ServiceModel", "4.0.0.0")]
    [System.ServiceModel.ServiceContractAttribute(ConfigurationName="PangoProdService.wServiceSoap")]
    public interface wServiceSoap {
        
        [System.ServiceModel.OperationContractAttribute(Action="http://tempuri.org/Login", ReplyAction="*")]
        [System.ServiceModel.XmlSerializerFormatAttribute(SupportFaults=true)]
        string Login(string UserName, string Password);
        
        [System.ServiceModel.OperationContractAttribute(Action="http://tempuri.org/Login", ReplyAction="*")]
        System.Threading.Tasks.Task<string> LoginAsync(string UserName, string Password);
        
        [System.ServiceModel.OperationContractAttribute(Action="http://tempuri.org/LoginWithMode", ReplyAction="*")]
        [System.ServiceModel.XmlSerializerFormatAttribute(SupportFaults=true)]
        LoginResult LoginWithMode(string UserName, string Password);
        
        [System.ServiceModel.OperationContractAttribute(Action="http://tempuri.org/LoginWithMode", ReplyAction="*")]
        System.Threading.Tasks.Task<LoginResult> LoginWithModeAsync(string UserName, string Password);
        
        [System.ServiceModel.OperationContractAttribute(Action="http://tempuri.org/CheckPlateNumberWithUserName", ReplyAction="*")]
        [System.ServiceModel.XmlSerializerFormatAttribute(SupportFaults=true)]
        string CheckPlateNumberWithUserName(string UserName, string Password, string PlateNumber, string StateCode);
        
        [System.ServiceModel.OperationContractAttribute(Action="http://tempuri.org/CheckPlateNumberWithUserName", ReplyAction="*")]
        System.Threading.Tasks.Task<string> CheckPlateNumberWithUserNameAsync(string UserName, string Password, string PlateNumber, string StateCode);
        
        [System.ServiceModel.OperationContractAttribute(Action="http://tempuri.org/CheckPlateNumberWithUserNameAndZone", ReplyAction="*")]
        [System.ServiceModel.XmlSerializerFormatAttribute(SupportFaults=true)]
        string CheckPlateNumberWithUserNameAndZone(string UserName, string Password, string PlateNumber, string StateCode, string Zone);
        
        [System.ServiceModel.OperationContractAttribute(Action="http://tempuri.org/CheckPlateNumberWithUserNameAndZone", ReplyAction="*")]
        System.Threading.Tasks.Task<string> CheckPlateNumberWithUserNameAndZoneAsync(string UserName, string Password, string PlateNumber, string StateCode, string Zone);
        
        [System.ServiceModel.OperationContractAttribute(Action="http://tempuri.org/CheckPlateNumberWithUserNameAndZone_ex1", ReplyAction="*")]
        [System.ServiceModel.XmlSerializerFormatAttribute(SupportFaults=true)]
        AnswerDetails CheckPlateNumberWithUserNameAndZone_ex1(string UserName, string Password, string PlateNumber, string StateCode, string Zone);
        
        [System.ServiceModel.OperationContractAttribute(Action="http://tempuri.org/CheckPlateNumberWithUserNameAndZone_ex1", ReplyAction="*")]
        System.Threading.Tasks.Task<AnswerDetails> CheckPlateNumberWithUserNameAndZone_ex1Async(string UserName, string Password, string PlateNumber, string StateCode, string Zone);
        
        [System.ServiceModel.OperationContractAttribute(Action="http://tempuri.org/CheckPlateNumberWithUserNameAndZone_ex2", ReplyAction="*")]
        [System.ServiceModel.XmlSerializerFormatAttribute(SupportFaults=true)]
        AnswerDetails[] CheckPlateNumberWithUserNameAndZone_ex2(string UserName, string Password, string PlateNumber, string StateCode, string Zone);
        
        [System.ServiceModel.OperationContractAttribute(Action="http://tempuri.org/CheckPlateNumberWithUserNameAndZone_ex2", ReplyAction="*")]
        System.Threading.Tasks.Task<AnswerDetails[]> CheckPlateNumberWithUserNameAndZone_ex2Async(string UserName, string Password, string PlateNumber, string StateCode, string Zone);
        
        [System.ServiceModel.OperationContractAttribute(Action="http://tempuri.org/CheckPlateNumberWithUserNameAndZone_ex3", ReplyAction="*")]
        [System.ServiceModel.XmlSerializerFormatAttribute(SupportFaults=true)]
       AnswerDetails[] CheckPlateNumberWithUserNameAndZone_ex3(string UserName, string Password, string device_id, string officerID, string PlateNumber, string StateCode, string Zone);
        
        [System.ServiceModel.OperationContractAttribute(Action="http://tempuri.org/CheckPlateNumberWithUserNameAndZone_ex3", ReplyAction="*")]
        System.Threading.Tasks.Task<AnswerDetails[]> CheckPlateNumberWithUserNameAndZone_ex3Async(string UserName, string Password, string device_id, string officerID, string PlateNumber, string StateCode, string Zone);
        
        [System.ServiceModel.OperationContractAttribute(Action="http://tempuri.org/GetExpiredVehiclesWithUserName", ReplyAction="*")]
        [System.ServiceModel.XmlSerializerFormatAttribute(SupportFaults=true)]
       ExpiredVehicle[] GetExpiredVehiclesWithUserName(string UserName, string Password, string Timeout, string zoneNumber);
        
        [System.ServiceModel.OperationContractAttribute(Action="http://tempuri.org/GetExpiredVehiclesWithUserName", ReplyAction="*")]
        System.Threading.Tasks.Task<ExpiredVehicle[]> GetExpiredVehiclesWithUserNameAsync(string UserName, string Password, string Timeout, string zoneNumber);
        
        [System.ServiceModel.OperationContractAttribute(Action="http://tempuri.org/CheckPlateNumber", ReplyAction="*")]
        [System.ServiceModel.XmlSerializerFormatAttribute(SupportFaults=true)]
        string CheckPlateNumber(string PlateNumber, string StateCode);
        
        [System.ServiceModel.OperationContractAttribute(Action="http://tempuri.org/CheckPlateNumber", ReplyAction="*")]
        System.Threading.Tasks.Task<string> CheckPlateNumberAsync(string PlateNumber, string StateCode);
        
        [System.ServiceModel.OperationContractAttribute(Action="http://tempuri.org/CheckPlateNumberWithZone", ReplyAction="*")]
        [System.ServiceModel.XmlSerializerFormatAttribute(SupportFaults=true)]
        string CheckPlateNumberWithZone(string PlateNumber, string StateCode, string Zone);
        
        [System.ServiceModel.OperationContractAttribute(Action="http://tempuri.org/CheckPlateNumberWithZone", ReplyAction="*")]
        System.Threading.Tasks.Task<string> CheckPlateNumberWithZoneAsync(string PlateNumber, string StateCode, string Zone);
        
        [System.ServiceModel.OperationContractAttribute(Action="http://tempuri.org/GetZoneLists", ReplyAction="*")]
        [System.ServiceModel.XmlSerializerFormatAttribute(SupportFaults=true)]
        string[] GetZoneLists(string UserName, string Password);
        
        [System.ServiceModel.OperationContractAttribute(Action="http://tempuri.org/GetZoneLists", ReplyAction="*")]
        System.Threading.Tasks.Task<string[]> GetZoneListsAsync(string UserName, string Password);
        
        [System.ServiceModel.OperationContractAttribute(Action="http://tempuri.org/GetParkingSpacesStatus", ReplyAction="*")]
        [System.ServiceModel.XmlSerializerFormatAttribute(SupportFaults=true)]
        SpaceStatus[] GetParkingSpacesStatus(string UserName, string Password, string Zone, string CityID);
        
        [System.ServiceModel.OperationContractAttribute(Action="http://tempuri.org/GetParkingSpacesStatus", ReplyAction="*")]
        System.Threading.Tasks.Task<SpaceStatus[]> GetParkingSpacesStatusAsync(string UserName, string Password, string Zone, string CityID);
        
        [System.ServiceModel.OperationContractAttribute(Action="http://tempuri.org/GetParkedCarsList", ReplyAction="*")]
        [System.ServiceModel.XmlSerializerFormatAttribute(SupportFaults=true)]
        string[] GetParkedCarsList(string UserName, string Password, string Zone);
        
        [System.ServiceModel.OperationContractAttribute(Action="http://tempuri.org/GetParkedCarsList", ReplyAction="*")]
        System.Threading.Tasks.Task<string[]> GetParkedCarsListAsync(string UserName, string Password, string Zone);
        
        [System.ServiceModel.OperationContractAttribute(Action="http://tempuri.org/GetActiveVehicles", ReplyAction="*")]
        [System.ServiceModel.XmlSerializerFormatAttribute(SupportFaults=true)]
        string[] GetActiveVehicles(string UserName, string Password, string Zone, string Vehicle);
        
        [System.ServiceModel.OperationContractAttribute(Action="http://tempuri.org/GetActiveVehicles", ReplyAction="*")]
        System.Threading.Tasks.Task<string[]> GetActiveVehiclesAsync(string UserName, string Password, string Zone, string Vehicle);
    }
    
    /// <remarks/>
    [System.CodeDom.Compiler.GeneratedCodeAttribute("System.Xml", "4.6.1099.0")]
    [System.SerializableAttribute()]
    [System.Diagnostics.DebuggerStepThroughAttribute()]
    [System.ComponentModel.DesignerCategoryAttribute("code")]
    [System.Xml.Serialization.XmlTypeAttribute(Namespace="http://tempuri.org/")]
    public partial class LoginResult : object, System.ComponentModel.INotifyPropertyChanged {
        
        private string statusField;
        
        private int modeField;
        
        private int cityField;
        
        private string stateField;
        
        /// <remarks/>
        [System.Xml.Serialization.XmlElementAttribute(Order=0)]
        public string status {
            get {
                return this.statusField;
            }
            set {
                this.statusField = value;
                this.RaisePropertyChanged("status");
            }
        }
        
        /// <remarks/>
        [System.Xml.Serialization.XmlElementAttribute(Order=1)]
        public int mode {
            get {
                return this.modeField;
            }
            set {
                this.modeField = value;
                this.RaisePropertyChanged("mode");
            }
        }
        
        /// <remarks/>
        [System.Xml.Serialization.XmlElementAttribute(Order=2)]
        public int city {
            get {
                return this.cityField;
            }
            set {
                this.cityField = value;
                this.RaisePropertyChanged("city");
            }
        }
        
        /// <remarks/>
        [System.Xml.Serialization.XmlElementAttribute(Order=3)]
        public string state {
            get {
                return this.stateField;
            }
            set {
                this.stateField = value;
                this.RaisePropertyChanged("state");
            }
        }
        
        public event System.ComponentModel.PropertyChangedEventHandler PropertyChanged;
        
        protected void RaisePropertyChanged(string propertyName) {
            System.ComponentModel.PropertyChangedEventHandler propertyChanged = this.PropertyChanged;
            if ((propertyChanged != null)) {
                propertyChanged(this, new System.ComponentModel.PropertyChangedEventArgs(propertyName));
            }
        }
    }
    
    /// <remarks/>
    [System.CodeDom.Compiler.GeneratedCodeAttribute("System.Xml", "4.6.1099.0")]
    [System.SerializableAttribute()]
    [System.Diagnostics.DebuggerStepThroughAttribute()]
    [System.ComponentModel.DesignerCategoryAttribute("code")]
    [System.Xml.Serialization.XmlTypeAttribute(Namespace="http://tempuri.org/")]
    public partial class SpaceStatus : object, System.ComponentModel.INotifyPropertyChanged {
        
        private string space_numberField;
        
        private int parking_statusField;
        
        /// <remarks/>
        [System.Xml.Serialization.XmlElementAttribute(Order=0)]
        public string space_number {
            get {
                return this.space_numberField;
            }
            set {
                this.space_numberField = value;
                this.RaisePropertyChanged("space_number");
            }
        }
        
        /// <remarks/>
        [System.Xml.Serialization.XmlElementAttribute(Order=1)]
        public int parking_status {
            get {
                return this.parking_statusField;
            }
            set {
                this.parking_statusField = value;
                this.RaisePropertyChanged("parking_status");
            }
        }
        
        public event System.ComponentModel.PropertyChangedEventHandler PropertyChanged;
        
        protected void RaisePropertyChanged(string propertyName) {
            System.ComponentModel.PropertyChangedEventHandler propertyChanged = this.PropertyChanged;
            if ((propertyChanged != null)) {
                propertyChanged(this, new System.ComponentModel.PropertyChangedEventArgs(propertyName));
            }
        }
    }
    
    /// <remarks/>
    [System.CodeDom.Compiler.GeneratedCodeAttribute("System.Xml", "4.6.1099.0")]
    [System.SerializableAttribute()]
    [System.Diagnostics.DebuggerStepThroughAttribute()]
    [System.ComponentModel.DesignerCategoryAttribute("code")]
    [System.Xml.Serialization.XmlTypeAttribute(Namespace="http://tempuri.org/")]
    public partial class ExpiredVehicle : object, System.ComponentModel.INotifyPropertyChanged {
        
        private string car_noField;
        
        private string zoneField;
        
        /// <remarks/>
        [System.Xml.Serialization.XmlElementAttribute(Order=0)]
        public string car_no {
            get {
                return this.car_noField;
            }
            set {
                this.car_noField = value;
                this.RaisePropertyChanged("car_no");
            }
        }
        
        /// <remarks/>
        [System.Xml.Serialization.XmlElementAttribute(Order=1)]
        public string zone {
            get {
                return this.zoneField;
            }
            set {
                this.zoneField = value;
                this.RaisePropertyChanged("zone");
            }
        }
        
        public event System.ComponentModel.PropertyChangedEventHandler PropertyChanged;
        
        protected void RaisePropertyChanged(string propertyName) {
            System.ComponentModel.PropertyChangedEventHandler propertyChanged = this.PropertyChanged;
            if ((propertyChanged != null)) {
                propertyChanged(this, new System.ComponentModel.PropertyChangedEventArgs(propertyName));
            }
        }
    }
    
    /// <remarks/>
    [System.CodeDom.Compiler.GeneratedCodeAttribute("System.Xml", "4.6.1099.0")]
    [System.SerializableAttribute()]
    [System.Diagnostics.DebuggerStepThroughAttribute()]
    [System.ComponentModel.DesignerCategoryAttribute("code")]
    [System.Xml.Serialization.XmlTypeAttribute(Namespace="http://tempuri.org/")]
    public partial class AnswerDetails : object, System.ComponentModel.INotifyPropertyChanged {
        
        private string descriptionField;
        
        private string city_stateField;
        
        private bool isPayingField;
        
        private string plateField;
        
        private string stateField;
        
        private string start_timeField;
        
        private string end_timeField;
        
        private bool usingPermitField;
        
        private string permitNameField;
        
        private string zoneField;
        
        /// <remarks/>
        [System.Xml.Serialization.XmlElementAttribute(Order=0)]
        public string description {
            get {
                return this.descriptionField;
            }
            set {
                this.descriptionField = value;
                this.RaisePropertyChanged("description");
            }
        }
        
        /// <remarks/>
        [System.Xml.Serialization.XmlElementAttribute(Order=1)]
        public string city_state {
            get {
                return this.city_stateField;
            }
            set {
                this.city_stateField = value;
                this.RaisePropertyChanged("city_state");
            }
        }
        
        /// <remarks/>
        [System.Xml.Serialization.XmlElementAttribute(Order=2)]
        public bool isPaying {
            get {
                return this.isPayingField;
            }
            set {
                this.isPayingField = value;
                this.RaisePropertyChanged("isPaying");
            }
        }
        
        /// <remarks/>
        [System.Xml.Serialization.XmlElementAttribute(Order=3)]
        public string plate {
            get {
                return this.plateField;
            }
            set {
                this.plateField = value;
                this.RaisePropertyChanged("plate");
            }
        }
        
        /// <remarks/>
        [System.Xml.Serialization.XmlElementAttribute(Order=4)]
        public string state {
            get {
                return this.stateField;
            }
            set {
                this.stateField = value;
                this.RaisePropertyChanged("state");
            }
        }
        
        /// <remarks/>
        [System.Xml.Serialization.XmlElementAttribute(Order=5)]
        public string start_time {
            get {
                return this.start_timeField;
            }
            set {
                this.start_timeField = value;
                this.RaisePropertyChanged("start_time");
            }
        }
        
        /// <remarks/>
        [System.Xml.Serialization.XmlElementAttribute(Order=6)]
        public string end_time {
            get {
                return this.end_timeField;
            }
            set {
                this.end_timeField = value;
                this.RaisePropertyChanged("end_time");
            }
        }
        
        /// <remarks/>
        [System.Xml.Serialization.XmlElementAttribute(Order=7)]
        public bool usingPermit {
            get {
                return this.usingPermitField;
            }
            set {
                this.usingPermitField = value;
                this.RaisePropertyChanged("usingPermit");
            }
        }
        
        /// <remarks/>
        [System.Xml.Serialization.XmlElementAttribute(Order=8)]
        public string permitName {
            get {
                return this.permitNameField;
            }
            set {
                this.permitNameField = value;
                this.RaisePropertyChanged("permitName");
            }
        }
        
        /// <remarks/>
        [System.Xml.Serialization.XmlElementAttribute(Order=9)]
        public string zone {
            get {
                return this.zoneField;
            }
            set {
                this.zoneField = value;
                this.RaisePropertyChanged("zone");
            }
        }
        
        public event System.ComponentModel.PropertyChangedEventHandler PropertyChanged;
        
        protected void RaisePropertyChanged(string propertyName) {
            System.ComponentModel.PropertyChangedEventHandler propertyChanged = this.PropertyChanged;
            if ((propertyChanged != null)) {
                propertyChanged(this, new System.ComponentModel.PropertyChangedEventArgs(propertyName));
            }
        }
    }
    
    [System.CodeDom.Compiler.GeneratedCodeAttribute("System.ServiceModel", "4.0.0.0")]
    public interface wServiceSoapChannel : wServiceSoap, System.ServiceModel.IClientChannel {
    }
    
    [System.Diagnostics.DebuggerStepThroughAttribute()]
    [System.CodeDom.Compiler.GeneratedCodeAttribute("System.ServiceModel", "4.0.0.0")]
    public partial class wServiceSoapClient : System.ServiceModel.ClientBase<wServiceSoap>, wServiceSoap {
        
        public wServiceSoapClient() {
        }
        
        public wServiceSoapClient(string endpointConfigurationName) : 
                base(endpointConfigurationName) {
        }
        
        public wServiceSoapClient(string endpointConfigurationName, string remoteAddress) : 
                base(endpointConfigurationName, remoteAddress) {
        }
        
        public wServiceSoapClient(string endpointConfigurationName, System.ServiceModel.EndpointAddress remoteAddress) : 
                base(endpointConfigurationName, remoteAddress) {
        }
        
        public wServiceSoapClient(System.ServiceModel.Channels.Binding binding, System.ServiceModel.EndpointAddress remoteAddress) : 
                base(binding, remoteAddress) {
        }
        
        public string Login(string UserName, string Password) {
            return base.Channel.Login(UserName, Password);
        }
        
        public System.Threading.Tasks.Task<string> LoginAsync(string UserName, string Password) {
            return base.Channel.LoginAsync(UserName, Password);
        }
        
        public LoginResult LoginWithMode(string UserName, string Password) {
            return base.Channel.LoginWithMode(UserName, Password);
        }
        
        public System.Threading.Tasks.Task<LoginResult> LoginWithModeAsync(string UserName, string Password) {
            return base.Channel.LoginWithModeAsync(UserName, Password);
        }
        
        public string CheckPlateNumberWithUserName(string UserName, string Password, string PlateNumber, string StateCode) {
            return base.Channel.CheckPlateNumberWithUserName(UserName, Password, PlateNumber, StateCode);
        }
        
        public System.Threading.Tasks.Task<string> CheckPlateNumberWithUserNameAsync(string UserName, string Password, string PlateNumber, string StateCode) {
            return base.Channel.CheckPlateNumberWithUserNameAsync(UserName, Password, PlateNumber, StateCode);
        }
        
        public string CheckPlateNumberWithUserNameAndZone(string UserName, string Password, string PlateNumber, string StateCode, string Zone) {
            return base.Channel.CheckPlateNumberWithUserNameAndZone(UserName, Password, PlateNumber, StateCode, Zone);
        }
        
        public System.Threading.Tasks.Task<string> CheckPlateNumberWithUserNameAndZoneAsync(string UserName, string Password, string PlateNumber, string StateCode, string Zone) {
            return base.Channel.CheckPlateNumberWithUserNameAndZoneAsync(UserName, Password, PlateNumber, StateCode, Zone);
        }
        
        public AnswerDetails CheckPlateNumberWithUserNameAndZone_ex1(string UserName, string Password, string PlateNumber, string StateCode, string Zone) {
            return base.Channel.CheckPlateNumberWithUserNameAndZone_ex1(UserName, Password, PlateNumber, StateCode, Zone);
        }
        
        public System.Threading.Tasks.Task<AnswerDetails> CheckPlateNumberWithUserNameAndZone_ex1Async(string UserName, string Password, string PlateNumber, string StateCode, string Zone) {
            return base.Channel.CheckPlateNumberWithUserNameAndZone_ex1Async(UserName, Password, PlateNumber, StateCode, Zone);
        }
        
        public AnswerDetails[] CheckPlateNumberWithUserNameAndZone_ex2(string UserName, string Password, string PlateNumber, string StateCode, string Zone) {
            return base.Channel.CheckPlateNumberWithUserNameAndZone_ex2(UserName, Password, PlateNumber, StateCode, Zone);
        }
        
        public System.Threading.Tasks.Task<AnswerDetails[]> CheckPlateNumberWithUserNameAndZone_ex2Async(string UserName, string Password, string PlateNumber, string StateCode, string Zone) {
            return base.Channel.CheckPlateNumberWithUserNameAndZone_ex2Async(UserName, Password, PlateNumber, StateCode, Zone);
        }
        
        public AnswerDetails[] CheckPlateNumberWithUserNameAndZone_ex3(string UserName, string Password, string device_id, string officerID, string PlateNumber, string StateCode, string Zone) {
            return base.Channel.CheckPlateNumberWithUserNameAndZone_ex3(UserName, Password, device_id, officerID, PlateNumber, StateCode, Zone);
        }
        
        public System.Threading.Tasks.Task<AnswerDetails[]> CheckPlateNumberWithUserNameAndZone_ex3Async(string UserName, string Password, string device_id, string officerID, string PlateNumber, string StateCode, string Zone) {
            return base.Channel.CheckPlateNumberWithUserNameAndZone_ex3Async(UserName, Password, device_id, officerID, PlateNumber, StateCode, Zone);
        }
        
        public ExpiredVehicle[] GetExpiredVehiclesWithUserName(string UserName, string Password, string Timeout, string zoneNumber) {
            return base.Channel.GetExpiredVehiclesWithUserName(UserName, Password, Timeout, zoneNumber);
        }
        
        public System.Threading.Tasks.Task<ExpiredVehicle[]> GetExpiredVehiclesWithUserNameAsync(string UserName, string Password, string Timeout, string zoneNumber) {
            return base.Channel.GetExpiredVehiclesWithUserNameAsync(UserName, Password, Timeout, zoneNumber);
        }
        
        public string CheckPlateNumber(string PlateNumber, string StateCode) {
            return base.Channel.CheckPlateNumber(PlateNumber, StateCode);
        }
        
        public System.Threading.Tasks.Task<string> CheckPlateNumberAsync(string PlateNumber, string StateCode) {
            return base.Channel.CheckPlateNumberAsync(PlateNumber, StateCode);
        }
        
        public string CheckPlateNumberWithZone(string PlateNumber, string StateCode, string Zone) {
            return base.Channel.CheckPlateNumberWithZone(PlateNumber, StateCode, Zone);
        }
        
        public System.Threading.Tasks.Task<string> CheckPlateNumberWithZoneAsync(string PlateNumber, string StateCode, string Zone) {
            return base.Channel.CheckPlateNumberWithZoneAsync(PlateNumber, StateCode, Zone);
        }
        
        public string[] GetZoneLists(string UserName, string Password) {
            return base.Channel.GetZoneLists(UserName, Password);
        }
        
        public System.Threading.Tasks.Task<string[]> GetZoneListsAsync(string UserName, string Password) {
            return base.Channel.GetZoneListsAsync(UserName, Password);
        }
        
        public SpaceStatus[] GetParkingSpacesStatus(string UserName, string Password, string Zone, string CityID) {
            return base.Channel.GetParkingSpacesStatus(UserName, Password, Zone, CityID);
        }
        
        public System.Threading.Tasks.Task<SpaceStatus[]> GetParkingSpacesStatusAsync(string UserName, string Password, string Zone, string CityID) {
            return base.Channel.GetParkingSpacesStatusAsync(UserName, Password, Zone, CityID);
        }
        
        public string[] GetParkedCarsList(string UserName, string Password, string Zone) {
            return base.Channel.GetParkedCarsList(UserName, Password, Zone);
        }
        
        public System.Threading.Tasks.Task<string[]> GetParkedCarsListAsync(string UserName, string Password, string Zone) {
            return base.Channel.GetParkedCarsListAsync(UserName, Password, Zone);
        }
        
        public string[] GetActiveVehicles(string UserName, string Password, string Zone, string Vehicle) {
            return base.Channel.GetActiveVehicles(UserName, Password, Zone, Vehicle);
        }
        
        public System.Threading.Tasks.Task<string[]> GetActiveVehiclesAsync(string UserName, string Password, string Zone, string Vehicle) {
            return base.Channel.GetActiveVehiclesAsync(UserName, Password, Zone, Vehicle);
        }
    }
}
