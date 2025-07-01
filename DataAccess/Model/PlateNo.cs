using System;
using System.Collections.Generic;
using System.IO;
using System.Runtime.Serialization;
using System.Runtime.Serialization.Json;
using System.Text;
using System.Linq;
using Microsoft.Web.Services3.Security;
namespace DataAccess.Model
{
    public static class CustomerIds
    {
        public const int Chicago = 7012;
        public const int CoralGables = 7002;
        public const int SouthMiami = 7001;
        public const int Atlanta = 4120;
        public const int Surfside = 7007;
        public const int Detroit = 7034;
        public const int BayHarbor = 7008;
        public const int SunnyIslesBeach = 7009;
        public const int MetroRail = 7006;
        public const int NorthBayVillage = 7078;
        public const int Abudhabi = 8016;
        public const int SpokaneWA = 4140;
        public const int MiamiParkingAuthority = 7003;
        public const int Philadelphia = 7056;
        public const int TybeeIsland = 7030;
        public const int CityofChester = 4135;
        public const int Durango = 4147;
        public const int SanDiego = 7014;
        public const int Huntsville = 7036;
        public const int Vista = 7040;
        public const int Watsonville = 7046;
        public const int Sweetwater = 7055;
        public const int SanibelAI = 7061;
        public const int RoyalOak = 7029;
        public const int MiamiBeach = 7004;
        public const int Torrance = 7042;
        ///////////////////////////
        public const int Glendale = 4142;
        public const int BoyntonBeach = 7072;
        ///////////////////////////
        public const int InnovationCity = 7049;
        public const int PaloAltoCA = 7064;
        public const int ColoradoSprings = 4254;
        public const int Leavenworth = 4280;
        public const int MountRainier = 4265;
        public const int JacksonvilleFl = 7062;
        public const int ChillicotheOH = 7081;
        public const int SANDIEGOAIRPORT = 7083;
        public const int SanibelX3 = 7058;
        public const int MuscatMunicipality = 8075;

        public const int Doral = 7079;
        public const int NewSmyrnaBeachFL = 7086;
        ////////////////////////
        public const int HyderabadGHMC = 8066;
        public const int DammamKSA = 7073;
        public const int DammamKSADev = 7074;
        public const int NOLA = 4176;
        public const int BahrainDemo1 = 8042;
        public const int WestAllis = 7070;
        public const int SalemOR = 4337;
        public const int Tempe = 7010;
    }

    [DataContract]
    public class PlateNo
    {
        [DataMember(Name = "$type", Order = 9)]
        public string Ptype { get; set; }
        [DataMember(Order = 1)]
        public string plate { get; set; }
        [DataMember(Order = 2)]
        public double confidence { get; set; }
        [DataMember(Order = 3, Name = "matchesTemplate")]
        public int matches_template { get; set; }
        [DataMember(Order = 4)]
        public bool violation { get; set; }
        [DataMember(Order = 5)]
        public bool expired { get; set; }
        [DataMember(Order = 6)]
        public bool validPayments { get; set; }
        [DataMember(Order = 7)]
        public bool noMatches { get; set; }

        private string _assignedClass;
        [DataMember(Order = 8)]
        public string assignedClass
        {
            get
            {
                return _assignedClass;
            }
            set
            {
                _assignedClass = value;
            }
        }


        private string _zoneName;
        [DataMember(Order = 9, Name = "zone")]
        public string ZoneName
        {
            get
            {
                return _zoneName;
            }
            set
            {
                _zoneName = value;
            }
        }


        [DataMember(Order = 10, Name = "expirytime")]
        public string StrExpiryTime
        {
            get
            {
                if (this.ExpiryTime.HasValue)
                {
                    // 2019-01-18T15:05:26.2857841Z
                    return this.ExpiryTime.Value.ToString("yyyy-MM-ddTHH:mm:ss.ffffffZ");
                }
                else
                {
                    return string.Empty;
                }
            }
            set
            {

            }

        }

        private string _vendorName;
        [DataMember(Order = 11, Name = "VendorName", EmitDefaultValue = false)]
        public string VendorName
        {
            get
            {
                return _vendorName;
            }
            set
            {
                _vendorName = value;
            }
        }

        [DataMember(Order = 12, Name = "ValidAnyZone", EmitDefaultValue = false)]
        public bool? ValidAnyZone
        {
            get;
            set;
        }

        [DataMember(Order = 13, Name = "Amount", EmitDefaultValue = false)]
        public decimal? Amount { get; set; }


        private string _assignedClass2;
        [DataMember(Order = 14, Name = "assignedClass2", EmitDefaultValue = false)]
        public string assignedClass2
        {
            get
            {
                return _assignedClass2;
            }
            set
            {
                _assignedClass2 = value;
            }
        }

        private string _assignedClass3;
        [DataMember(Order = 15, Name = "assignedClass3", EmitDefaultValue = false)]
        public string assignedClass3
        {
            get
            {
                return _assignedClass3;
            }
            set
            {
                _assignedClass3 = value;
            }
        }

        private string _vendorLPR;
        [DataMember(Order = 16, Name = "VendorLpr", EmitDefaultValue = false)]
        public string VendorLpr
        {
            get
            {
                return _vendorLPR;
            }
            set
            {
                _vendorLPR = value;
            }
        }

        private string _eNFPermitNo;
        [DataMember(Order = 17, Name = "ENFPermitNo", EmitDefaultValue = false)]
        public string ENFPermitNo
        {
            get
            {
                return _eNFPermitNo;
            }
            set
            {
                _eNFPermitNo = value;
            }
        }



        private string _assignedClass4;
        public string assignedClass4
        {
            get
            {
                return _assignedClass4;
            }
            set
            {
                _assignedClass4 = value;
            }
        }

        private List<Permit> _listPermit;
        [DataMember(Order = 18, Name = "Permits")]
        public List<Permit> Permits
        {
            get
            {
                return this._listPermit;
            }
            set
            {
                this._listPermit = value;
            }
        }


        [DataMember(Order = 19, Name = "DeltaTime", EmitDefaultValue = false)]
        public string StrDeltaTime
        {
            get
            {
                if (this.DeltaTime.HasValue)
                {
                    return this.DeltaTime.Value.ToString("yyyy-MM-ddTHH:mm:ss.ffffffZ");
                }
                else
                {
                    return string.Empty;
                }
            }
            set
            {

            }

        }


        private string _le;
        [DataMember(Order = 20, Name = "Le", EmitDefaultValue = false)]
        public string LE
        {
            get
            {
                return _le;
            }
            set
            {
                _le = value;
            }
        }


      
        [DataMember(Order = 21, Name = "ImageType", EmitDefaultValue = false)]
        public ImageType ImageType
        {
            get
            {
                ImageType value = ImageType.None;
                if(this.VendorID.HasValue)
                {
                    switch (this.VendorID.Value)
                    {
                        case 5:  //Genetec
                            value = ImageType.Base64;
                            break;
                        case 18://Vigilant
                            value = ImageType.Hexa;
                            break;
                        //case 21://VigilantRest
                        //    value = ImageType.URL;
                        //    break;
                        default:
                            break;
                    }
                }
                else
                {
                   //Image type is NOne
                }

                return value;
            }
            set
            {
                
            }
        }

        [DataMember(Order = 22, Name = "Image", EmitDefaultValue = false)]
        public string Image
        {
            get
            {
                StringBuilder sb = new StringBuilder();
                switch (this.ImageType)
                {
                    case ImageType.None:
                        break;
                    case ImageType.Base64:
                    //case ImageType.URL:
                    case ImageType.Binary:
                        sb.Append(this.ImageRawData);
                        break;
                    case ImageType.Hexa:
                        sb.Append(this.ImageRawData);
                        break;
                    default:
                        
                        break;
                }
                return sb.ToString();
            }
            set
            {

            }
        }

        [DataMember(Order = 23, Name = "ObservedTime", EmitDefaultValue = false)]
        public string StrObservedTime
        {
            get
            {
                if (this.ObservedTime.HasValue)
                {
                    return this.ObservedTime.Value.ToString("yyyy-MM-ddTHH:mm:ss.ffffffZ");
                }
                else
                {
                    return string.Empty;
                }
            }
            set
            {

            }

        }


        private PlateStatus _PlateStatus;
        public PlateStatus PlateStatus
        {
            //Occupied // Expired //Vacant
            get
            {
                if (string.IsNullOrEmpty(this.SpaceStatus))
                    return _PlateStatus;

                if (this.SpaceStatus.ToLower() == "Paid".ToLower())
                    this._PlateStatus = Model.PlateStatus.Paid;
                else if (this.SpaceStatus.ToLower() == "Expired".ToLower())
                    this._PlateStatus = Model.PlateStatus.Expired;
                else if (this.SpaceStatus.ToLower() == "Violation".ToLower())
                    this._PlateStatus = Model.PlateStatus.Violated;
                else if (this.SpaceStatus.ToLower() == "Permit".ToLower())
                    this._PlateStatus = Model.PlateStatus.Permits;
                else
                    this._PlateStatus = Model.PlateStatus.NoMatch;

                return _PlateStatus;

            }
            set { _PlateStatus = value; }
        }


        private PlateStatusMiamiAndAgencies _PlateStatusSpecificToMiamiAndAgencies;
        public PlateStatusMiamiAndAgencies PlateStatusSpecificToMiamiAndAgencies
        {
            //Occupied // Expired //Vacant
            get
            {
                if (string.IsNullOrEmpty(this.SpaceStatus))
                    return _PlateStatusSpecificToMiamiAndAgencies;

                if (this.SpaceStatus.ToLower() == "Paid".ToLower())
                    this._PlateStatusSpecificToMiamiAndAgencies = Model.PlateStatusMiamiAndAgencies.Paid;
                else if (this.SpaceStatus.ToLower() == "Expired".ToLower())
                    this._PlateStatusSpecificToMiamiAndAgencies = Model.PlateStatusMiamiAndAgencies.Expired;
                else if (this.SpaceStatus.ToLower() == "Violation".ToLower())
                    this._PlateStatusSpecificToMiamiAndAgencies = Model.PlateStatusMiamiAndAgencies.Violated;
                else if (this.SpaceStatus.ToLower() == "Permit".ToLower())
                    this._PlateStatusSpecificToMiamiAndAgencies = Model.PlateStatusMiamiAndAgencies.Permits;
                else if (this.SpaceStatus.ToLower() == "NoMatch".ToLower())
                    this._PlateStatusSpecificToMiamiAndAgencies = Model.PlateStatusMiamiAndAgencies.NoMatch;
                else
                    this._PlateStatusSpecificToMiamiAndAgencies = Model.PlateStatusMiamiAndAgencies.Others;

                return _PlateStatusSpecificToMiamiAndAgencies;

            }
            set { _PlateStatusSpecificToMiamiAndAgencies = value; }
        }

        private PlateStatusIncludingOtherAndGenetec _PlateStatusIncludingOther;
        public PlateStatusIncludingOtherAndGenetec PlateStatusIncludingOther
        {
            //Occupied // Expired //Vacant
            get
            {
                if (string.IsNullOrEmpty(this.SpaceStatus))
                    return _PlateStatusIncludingOther;

                if (this.SpaceStatus.ToLower() == "Paid".ToLower())
                    this._PlateStatusIncludingOther = Model.PlateStatusIncludingOtherAndGenetec.Paid;
                else if (this.SpaceStatus.ToLower() == "Expired".ToLower())
                    this._PlateStatusIncludingOther = Model.PlateStatusIncludingOtherAndGenetec.Expired;
                else if (this.SpaceStatus.ToLower() == "Violation".ToLower())
                    this._PlateStatusIncludingOther = Model.PlateStatusIncludingOtherAndGenetec.Violated;
                else if (this.SpaceStatus.ToLower() == "Permit".ToLower())
                    this._PlateStatusIncludingOther = Model.PlateStatusIncludingOtherAndGenetec.Permits;
                else if (this.SpaceStatus.ToLower() == "NoMatch".ToLower())
                    this._PlateStatusIncludingOther = Model.PlateStatusIncludingOtherAndGenetec.NoMatch;
                else
                    this._PlateStatusIncludingOther = Model.PlateStatusIncludingOtherAndGenetec.Others;

                return _PlateStatusIncludingOther;

            }
            set { _PlateStatusIncludingOther = value; }
        }

        public string RateName { get; set; }
        public int? CustomerId { get; set; }
        public string LicPlateWithOutState { get; set; }
        public string StateName { get; set; }
        public DateTime? PresentMeterTime { get; set; }

        public DateTime? ExpiryTime { get; set; }
        public DateTime? DeltaTime { get; set; }
        public DateTime? ObservedTime { get; set; }
        public string EnfType { get; set; }
        public string SpaceStatus { get; set; }
        public string FullPlateNo
        {
            get
            {
                return string.Format("{0}, {1}", this.LicPlateWithOutState, this.StateName);
            }
        }

        public int? ZoneId { get; set; }


        private LEStatus _LEStatus;
        public LEStatus LEStatus
        {
            get
            {
                if (string.IsNullOrEmpty(this.LE))
                    return _LEStatus;

                if (this.LE.Trim().ToLower() == "Boot".ToLower())
                    this._LEStatus = Model.LEStatus.Boot;
                else if (this.LE.Trim().ToLower() == "Scofflaw".ToLower())
                    this._LEStatus = Model.LEStatus.Scofflaw;
                else if (this.LE.Trim().ToLower() == "Tow".ToLower())
                    this._LEStatus = Model.LEStatus.Tow;
                else if (this.LE.Trim().ToLower() == "Expired Parking".ToLower())
                    this._LEStatus = Model.LEStatus.ExpiredParking;
                else
                    this._LEStatus = Model.LEStatus.NoMatch;

                return _LEStatus;

            }
            set { _LEStatus = value; }
        }

        public long? VendorID { get; set; }
        public string ImageRawData { get; set; }
        public long? EnfVendorLprID { get; set; }


    }

    public enum PlateStatus
    {
        NoMatch = 0,
        Violated = 1,
        Expired = 2,
        Paid = 3,
        Permits = 4
    }

    public enum LEStatus
    {
        NoMatch = 0,
        Scofflaw = 1, 
        Tow = 2, 
        Boot = 3,
        ExpiredParking = 4
    }

    public enum PlateStatusMiamiAndAgencies
    {
        NoMatch = 0,
        Violated = 1,
        Expired = 2,
        Paid = 3,
        Permits = 4,
        Others = 5
    }

    public enum ImageType
    {
        None = 0,
        Base64 = 1,
        Binary = 2,
        URL = 3,
        Hexa = 4
    }

    public enum PlateStatusIncludingOtherAndGenetec
    {
        NoMatch = 0,
        Violated = 1,
        Expired = 2,
        Paid = 3,
        Permits = 4,
        Others = 5,
        Genetec = 6
    }

    public class PlateNos
    {
        private List<PlateNo> _listPlateNo;
        public PlateNos()
        {
            _listPlateNo = new List<PlateNo>();
        }
        public List<PlateNo> Candidate
        {
            get
            {
                return this._listPlateNo;
            }
            set
            {
                this._listPlateNo = value;
            }
        }

        public string GetJsonData()
        {
            using (MemoryStream mem = new MemoryStream())
            {
                DataContractJsonSerializer ser =
                new DataContractJsonSerializer(typeof(PlateNos));

                ser.WriteObject(mem, this);
                string data =
               Encoding.UTF8.GetString(mem.ToArray(), 0, (int)mem.Length);
                return data;
            }
        }
    }

    public class PlateFactory
    {
        public PlateNos GetData(int customerID, string PlateNumber)
        {
            GeneralFactory genfact = null;
            switch (customerID)
            {
                //case CustomerIds.CoralGables:
                //case CustomerIds.SouthMiami:
                //case CustomerIds.Surfside:
                //case CustomerIds.BayHarbor:
                //case CustomerIds.SunnyIslesBeach:
                //case CustomerIds.MetroRail:
                case CustomerIds.Abudhabi:
                //case CustomerIds.MiamiParkingAuthority:
                case CustomerIds.Durango:
                case CustomerIds.SanDiego:
                case CustomerIds.Huntsville:
                case CustomerIds.Vista:
                case CustomerIds.Watsonville:
                //case CustomerIds.Sweetwater:
                case CustomerIds.SanibelAI:
                case CustomerIds.RoyalOak:
                    //case CustomerIds.MiamiBeach:
                    //genfact = new CoralGablesFactory(customerID, PlateNumber);
                    genfact = new GlendaleFactory(customerID, PlateNumber);
                    break;
                case CustomerIds.Detroit:
                    genfact = new DetroitFactory(customerID, PlateNumber);
                    break;
                //case CustomerIds.SpokaneWA:
                //    genfact = new SpokaneWAFactory(customerID, PlateNumber);
                //    break;
                case CustomerIds.Chicago:
                    genfact = new ChicagoFactor(customerID, PlateNumber);
                    break;
                case CustomerIds.Atlanta:
                    genfact = new AtlantaFactory(customerID, PlateNumber);
                    break;
                case CustomerIds.TybeeIsland:
                    genfact = new TybeeIslandFactory(customerID, PlateNumber);
                    break;
                //case CustomerIds.Philadelphia:
                //case CustomerIds.CityofChester:
                //case CustomerIds.Glendale:
                case CustomerIds.MiamiParkingAuthority:
                case CustomerIds.CoralGables:
                case CustomerIds.SouthMiami:
                case CustomerIds.Surfside:
                case CustomerIds.BayHarbor:
                case CustomerIds.SunnyIslesBeach:
                case CustomerIds.MetroRail:
                case CustomerIds.NorthBayVillage:    
                case CustomerIds.MiamiBeach:
                case CustomerIds.Sweetwater:
                case CustomerIds.InnovationCity:
                case CustomerIds.SpokaneWA:
                    genfact = new PhiladelphiaFactory(customerID, PlateNumber);
                    break;
                case CustomerIds.Glendale:
                case CustomerIds.CityofChester:
                case CustomerIds.BoyntonBeach:
                //case CustomerIds.Atlanta:
                //case CustomerIds.Philadelphia:
                case CustomerIds.DammamKSA:
                case CustomerIds.DammamKSADev:
                case CustomerIds.Torrance:
                case CustomerIds.HyderabadGHMC:
                case CustomerIds.NOLA:
                case CustomerIds.Tempe:
                case CustomerIds.JacksonvilleFl:
                case CustomerIds.ChillicotheOH:
                case CustomerIds.SANDIEGOAIRPORT:
                case CustomerIds.SanibelX3:
                case CustomerIds.MuscatMunicipality:
                case CustomerIds.Doral:
                case CustomerIds.NewSmyrnaBeachFL:
                case CustomerIds.PaloAltoCA:
                case CustomerIds.SalemOR:
                    genfact = new GlendaleFactory(customerID, PlateNumber);
                    break;


                case CustomerIds.Philadelphia:
                case CustomerIds.Leavenworth:
                case CustomerIds.ColoradoSprings:
                case CustomerIds.MountRainier: 
                //case CustomerIds.PaloAltoCA:
                case CustomerIds.BahrainDemo1:
                case CustomerIds.WestAllis:
                    genfact = new PhilliFactoryDualState(customerID, PlateNumber);
                    break;
                default:
                    genfact = new GeneralFactory(customerID, PlateNumber);
                    break;
            }
            genfact.MatchPlate(PlateNumber, customerID);
            return genfact.Plates;
        }
        public PlateNos GetData(int customerID, string PlateNumber, string ZoneId)
        {
            GeneralFactory genfact = null;
            switch (customerID)
            {
                case CustomerIds.Detroit:
                    genfact = new DetroitFactory(customerID, PlateNumber, ZoneId);
                    genfact.MatchPlate(PlateNumber, customerID, ZoneId);
                    return genfact.Plates;
                    break;
                case CustomerIds.Philadelphia:
                    genfact = new GlendaleFactory(customerID, PlateNumber, ZoneId);
                    break;
                default:
                    genfact = new GeneralFactory(customerID, PlateNumber);
                    break;
            }
            genfact.MatchPlate(PlateNumber, customerID, ZoneId);
            return genfact.Plates;
        }
        //public PlateNos GetData(int customerID, string PlateNumber, string ZoneId)
        //{
        //    GeneralFactory genfact = null;
        //    switch (customerID)
        //    {
        //        //case CustomerIds.CoralGables:
        //        //case CustomerIds.SouthMiami:
        //        //case CustomerIds.Surfside:
        //        //case CustomerIds.BayHarbor:
        //        //case CustomerIds.SunnyIslesBeach:
        //        //case CustomerIds.MetroRail:
        //        case CustomerIds.Abudhabi:
        //        //case CustomerIds.MiamiParkingAuthority:
        //        case CustomerIds.Durango:
        //        case CustomerIds.SanDiego:
        //        case CustomerIds.Huntsville:
        //        case CustomerIds.Vista:
        //        case CustomerIds.Watsonville:
        //        //case CustomerIds.Sweetwater:
        //        case CustomerIds.SanibelAI:
        //        case CustomerIds.RoyalOak:
        //        //case CustomerIds.MiamiBeach:
        //            genfact = new CoralGablesFactory(customerID, PlateNumber);
        //            break;
        //        case CustomerIds.Detroit:
        //            genfact = new DetroitFactory(customerID, PlateNumber, ZoneId);
        //            genfact.MatchPlate(PlateNumber, customerID, ZoneId);
        //            return genfact.Plates;
        //            break;
        //        //case CustomerIds.SpokaneWA:
        //        //    genfact = new SpokaneWAFactory(customerID, PlateNumber);
        //        //    break;
        //        case CustomerIds.Chicago:
        //            genfact = new ChicagoFactor(customerID, PlateNumber);
        //            break;
        //        //case CustomerIds.Atlanta:
        //        //    genfact = new AtlantaFactory(customerID, PlateNumber);
        //        //    break;
        //        case CustomerIds.TybeeIsland:
        //            genfact = new TybeeIslandFactory(customerID, PlateNumber);
        //            break;
        //        //case CustomerIds.Philadelphia:
        //        //case CustomerIds.CityofChester:
        //        //case CustomerIds.Glendale:
        //        case CustomerIds.MiamiParkingAuthority:
        //        case CustomerIds.CoralGables:
        //        case CustomerIds.SouthMiami:
        //        case CustomerIds.Surfside:
        //        case CustomerIds.BayHarbor:
        //        case CustomerIds.SunnyIslesBeach:
        //        case CustomerIds.MetroRail:
        //        case CustomerIds.MiamiBeach:
        //        case CustomerIds.Sweetwater:
        //        case CustomerIds.InnovationCity:
        //        case CustomerIds.SpokaneWA:
        //            genfact = new PhiladelphiaFactory(customerID, PlateNumber);
        //            break;
        //        case CustomerIds.Glendale:
        //        case CustomerIds.CityofChester:
        //        case CustomerIds.BoyntonBeach:
        //        case CustomerIds.Atlanta:
        //        case CustomerIds.Philadelphia:
        //        case CustomerIds.DammamKSA:
        //        case CustomerIds.DammamKSADev:
        //        case CustomerIds.Torrance:
        //        case CustomerIds.HyderabadGHMC:
        //            genfact = new GlendaleFactory(customerID, PlateNumber);
        //            break;
        //        default:
        //            genfact = new GeneralFactory(customerID, PlateNumber);
        //            break;
        //    }
        //    genfact.MatchPlate(PlateNumber, customerID);
        //    return genfact.Plates;
        //}

        public static PlateNo GetErrorPlate
        {
            get
            {
                return new PlateNo() { assignedClass = "error" };
            }
        }
    }

    public class GeneralFactory
    {
        protected int _customerId;
        protected PlateNos _PlateNos;
        protected string _plateNo;
        public GeneralFactory(int customerId, string plateNo)
        {
            _PlateNos = new PlateNos();
            this._customerId = customerId;
            this._plateNo = plateNo;
        }
        public GeneralFactory(int customerId, string plateNo, string zone)
        {
            _PlateNos = new PlateNos();
            this._customerId = customerId;
            this._plateNo = plateNo;
            this.Zone = zone;
        }

        public PlateNos Plates
        {
            get
            {
                return this._PlateNos;
            }
            set
            {
                this._PlateNos = value;
            }
        }
        public int CustomerId
        {
            get
            {
                return this._customerId;
            }
        }
        public string PlateNo
        {
            get
            {
                return this._plateNo;
            }
            set
            {
                this._plateNo = value;
            }
        }
        public string Zone { get; set; }


        public virtual void MatchPlate(string plateValue, int customerID)
        {
            try
            {
                //Get Data from DB
                this.GetRepositoryData(plateValue, customerID);

                //If no record  add, NoMatch recrod
                if (_PlateNos.Candidate.Count == 0)
                {
                    this._PlateNos.Candidate.Add(new PlateNo() { PlateStatus = PlateStatus.NoMatch, LicPlateWithOutState = plateValue, plate = plateValue });
                }


                //Occupied // Expired //Vacant
                foreach (var plate in this.Plates.Candidate)
                {
                    switch (plate.PlateStatus)
                    {
                        case PlateStatus.NoMatch:
                            this.FillNoMatchPlate(plate);
                            break;
                        case PlateStatus.Violated:
                            this.FillViolated(plate);
                            break;
                        case PlateStatus.Expired:
                            this.FillExpired(plate);
                            break;
                        case PlateStatus.Paid:
                            this.FillPaid(plate);
                            break;
                        case PlateStatus.Permits:
                            this.FillPermits(plate);
                            break;
                        default:
                            this.FillNoMatchPlate(plate);
                            break;
                    }
                }

            }
            catch (Exception ex)
            {
                throw ex;
            }
        }

        public virtual void MatchPlate(string plateValue, int customerID, string Zone)
        {
            try
            {
                //Get Data from DB
                this.GetRepositoryData(plateValue, customerID, Zone);

                //If no record  add, NoMatch recrod
                if (_PlateNos.Candidate.Count == 0)
                {
                    this._PlateNos.Candidate.Add(new PlateNo() { PlateStatus = PlateStatus.NoMatch, LicPlateWithOutState = plateValue, plate = plateValue });
                }


                //Occupied // Expired //Vacant
                foreach (var plate in this.Plates.Candidate)
                {
                    switch (plate.PlateStatus)
                    {
                        case PlateStatus.NoMatch:
                            this.FillNoMatchPlate(plate);
                            break;
                        case PlateStatus.Violated:
                            this.FillViolated(plate);
                            break;
                        case PlateStatus.Expired:
                            this.FillExpired(plate);
                            break;
                        case PlateStatus.Paid:
                            this.FillPaid(plate);
                            break;
                        case PlateStatus.Permits:
                            this.FillPermits(plate);
                            break;
                        default:
                            this.FillNoMatchPlate(plate);
                            break;
                    }
                }

            }
            catch (Exception ex)
            {
                throw ex;
            }
        }

        protected virtual void GetRepositoryData(string plateValue, int customerID)
        {
            this.Plates.Candidate = PlateDataAccess.GetPlateMatch(customerID, plateValue);
        }

        protected virtual void GetRepositoryData(string plateValue, int customerID, string zone)
        {
            this.Plates.Candidate = PlateDataAccess.GetPlateMatch(customerID, plateValue, zone);
        }

        protected virtual void FillNoMatchPlate(PlateNo plate)
        {
            plate.Ptype = "ANPR.Models.Candidate, ANPR";
            plate.confidence = 0;
            plate.matches_template = 0;
            plate.violation = false;
            plate.expired = false;
            plate.validPayments = false;
            plate.noMatches = true;
            plate.assignedClass = "NoMatch";

        }

        protected virtual void FillPaid(PlateNo plate)
        {
            plate.Ptype = "ANPR.Models.Candidate, ANPR";
            plate.confidence = 0;
            plate.matches_template = 0;
            plate.violation = false;
            plate.expired = false;
            plate.validPayments = true;
            plate.noMatches = false;
            plate.assignedClass = "Paid";
        }

        protected virtual void FillExpired(PlateNo plate)
        {
            plate.Ptype = "ANPR.Models.Candidate, ANPR";
            plate.confidence = 0;
            plate.matches_template = 0;
            plate.violation = false;
            plate.expired = true;
            plate.validPayments = false;
            plate.noMatches = false;
            plate.assignedClass = "Expired";
        }

        protected virtual void FillViolated(PlateNo plate)
        {
            plate.Ptype = "ANPR.Models.Candidate, ANPR";
            plate.confidence = 0;
            plate.matches_template = 0;
            plate.violation = true;
            plate.expired = true;
            plate.validPayments = false;
            plate.noMatches = false;
            plate.assignedClass = plate.EnfType;

        }

        protected virtual void FillPermits(PlateNo plate)
        {
            //Check permit status
            plate.Ptype = "ANPR.Models.Candidate, ANPR";
            plate.confidence = 0;
            plate.matches_template = 0;
            plate.violation = false;
            plate.expired = false;
            plate.validPayments = true;
            plate.noMatches = false;
            plate.assignedClass = "Exempt";
        }

        protected virtual void FillOthers(PlateNo plate, string value)
        {
            plate.Ptype = "ANPR.Models.Candidate, ANPR";
            plate.confidence = 0;
            plate.matches_template = 0;
            plate.expired = false;
            plate.noMatches = false;
            plate.violation = true;
            plate.validPayments = false;
            plate.noMatches = false;
            plate.assignedClass = value;
        }
    }

    public class ChicagoFactor : GeneralFactory
    {
        public ChicagoFactor(int customerId, string plateNo)
            : base(customerId, plateNo)
        {

        }

        public override void MatchPlate(string plateValue, int customerID)
        {
            try
            {
                //Get Data from DB
                this.GetRepositoryData(plateValue, customerID);

                //If no record  add, NoMatch recrod
                if (this.Plates.Candidate.Count == 0)
                {
                    this.Plates.Candidate.Add(new PlateNo() { PlateStatus = PlateStatus.NoMatch, LicPlateWithOutState = plateValue, plate = plateValue });
                }



                foreach (var plate in this.Plates.Candidate)
                {
                    switch (plate.PlateStatus)
                    {
                        case PlateStatus.NoMatch:
                            this.FillNoMatchPlate(plate);
                            break;
                        case PlateStatus.Violated:
                            this.FillViolated(plate);
                            break;
                        case PlateStatus.Expired:
                            this.FillExpired(plate);
                            break;
                        case PlateStatus.Paid:
                            this.FillPaid(plate);
                            break;
                        case PlateStatus.Permits:
                            this.FillPermits(plate);
                            break;
                        default:
                            this.FillNoMatchPlate(plate);
                            break;
                    }
                }

            }
            catch (Exception ex)
            {
                throw ex;
            }
        }
        protected override void GetRepositoryData(string plateValue, int customerID)
        {
            base.GetRepositoryData(plateValue, customerID);
        }
        protected override void FillPaid(PlateNo plate)
        {
            base.FillPaid(plate);
            plate.assignedClass = string.Format("{0}, {1}", "Paid", plate.RateName);
        }
        protected override void FillExpired(PlateNo plate)
        {
            base.FillExpired(plate);
            plate.assignedClass = string.Format("{0}", "Expired");
        }
        protected override void FillPermits(PlateNo plate)
        {
            base.FillPermits(plate);
            //plate.assignedClass = string.Format("{0}", "Permit");
            plate.assignedClass = string.Format("{0}", "Exempt");
        }
        protected override void FillViolated(PlateNo plate)
        {
            base.FillViolated(plate);
            plate.assignedClass = string.Format("{0}", plate.EnfType);
        }
    }

    public class CoralGablesFactory : GeneralFactory
    {
        public CoralGablesFactory(int customerId, string plateNo)
            : base(customerId, plateNo)
        {

        }


        public override void MatchPlate(string plateValue, int customerID)
        {
            try
            {
                //Get Data from DB
                this.GetRepositoryData(plateValue, customerID);

                //If no record  add, NoMatch recrod
                if (_PlateNos.Candidate.Count == 0)
                {
                    this._PlateNos.Candidate.Add(new PlateNo() { PlateStatus = PlateStatus.NoMatch, LicPlateWithOutState = plateValue, plate = plateValue });
                }


                //Occupied // Expired //Vacant
                foreach (var plate in this.Plates.Candidate)
                {
                    switch (plate.PlateStatusSpecificToMiamiAndAgencies)
                    {
                        case PlateStatusMiamiAndAgencies.NoMatch:
                            this.FillNoMatchPlate(plate);
                            break;
                        case PlateStatusMiamiAndAgencies.Violated:
                            this.FillViolated(plate);
                            break;
                        case PlateStatusMiamiAndAgencies.Expired:
                            this.FillExpired(plate);
                            break;
                        case PlateStatusMiamiAndAgencies.Paid:
                            this.FillPaid(plate);
                            break;
                        case PlateStatusMiamiAndAgencies.Permits:
                            this.FillPermits(plate);
                            break;
                        default:
                            this.FillOthers(plate, plate.SpaceStatus);
                            break;
                    }
                }

            }
            catch (Exception ex)
            {
                throw ex;
            }
        }

        protected override void FillPermits(PlateNo plate)
        {
            base.FillPermits(plate);
            plate.violation = true;
            plate.validPayments = false;
            plate.assignedClass = string.Format("{0}", "Exempt");
        }
    }

    public class AtlantaFactory : GeneralFactory
    {
        public AtlantaFactory(int customerId, string plateNo)
            : base(customerId, plateNo)
        {

        }

        public override void MatchPlate(string plateValue, int customerID)
        {
            try
            {
                //Get Data from DB
                this.GetRepositoryData(plateValue, customerID);

                //If no record  add, NoMatch recrod
                if (_PlateNos.Candidate.Count == 0)
                {
                    this._PlateNos.Candidate.Add(new PlateNo() { PlateStatus = PlateStatus.NoMatch, LicPlateWithOutState = plateValue, plate = plateValue });
                }


                //Occupied // Expired //Vacant
                foreach (var plate in this.Plates.Candidate)
                {
                    switch (plate.PlateStatusIncludingOther)
                    {
                        case PlateStatusIncludingOtherAndGenetec.NoMatch:
                            this.FillNoMatchPlate(plate);
                            break;
                        case PlateStatusIncludingOtherAndGenetec.Violated:
                            this.FillViolated(plate);
                            break;
                        case PlateStatusIncludingOtherAndGenetec.Expired:
                            this.FillExpired(plate);
                            break;
                        case PlateStatusIncludingOtherAndGenetec.Paid:
                            this.FillPaid(plate);
                            break;

                        case PlateStatusIncludingOtherAndGenetec.Permits:
                            this.FillPermits(plate);
                            break;
                        default:
                            this.FillOthers(plate, plate.SpaceStatus);
                            break;
                    }
                }
            }
            catch (Exception ex)
            {
                throw ex;
            }
        }

        protected override void FillViolated(PlateNo plate)
        {
            plate.Ptype = "ANPR.Models.Candidate, ANPR";
            plate.confidence = 0;
            plate.matches_template = 0;
            plate.violation = true;
            plate.expired = true;
            plate.validPayments = false;
            plate.noMatches = false;
            plate.assignedClass = plate.EnfType;

        }
        protected override void FillOthers(PlateNo plate, string value)
        {
            plate.Ptype = "ANPR.Models.Candidate, ANPR";
            plate.confidence = 0;
            plate.matches_template = 0;
            plate.expired = false;
            plate.noMatches = false;
            plate.violation = false;
            plate.validPayments = true;
            plate.noMatches = false;
            plate.assignedClass = value;
        }
    }

    public class DetroitFactory : GeneralFactory
    {
        public DetroitFactory(int customerId, string plateNo)
            : base(customerId, plateNo)
        {

        }
        public DetroitFactory(int customerId, string plateNo,string zone)
            : base(customerId, plateNo)
        {

        }


        public override void MatchPlate(string plateValue, int customerID)
        {
            try
            {
                //Get Data from DB
                this.GetRepositoryData(plateValue, customerID);

                //If no record  add, NoMatch recrod
                if (_PlateNos.Candidate.Count == 0)
                {
                    this._PlateNos.Candidate.Add(new PlateNo() { PlateStatus = PlateStatus.NoMatch, LicPlateWithOutState = plateValue, plate = plateValue });
                }

                //Occupied // Expired //Vacant
                foreach (var plate in this.Plates.Candidate)
                {
                    switch (plate.PlateStatusIncludingOther)
                    {
                        case PlateStatusIncludingOtherAndGenetec.NoMatch:
                            this.FillNoMatchPlate(plate);
                            break;
                        case PlateStatusIncludingOtherAndGenetec.Violated:
                            this.FillViolated(plate);
                            break;
                        case PlateStatusIncludingOtherAndGenetec.Expired:
                            this.FillExpired(plate);
                            break;
                        case PlateStatusIncludingOtherAndGenetec.Paid:
                            this.FillPaid(plate);
                            break;
                        case PlateStatusIncludingOtherAndGenetec.Permits:
                            this.FillPermits(plate);
                            break;
                        default:
                            this.FillOthers(plate, plate.SpaceStatus);
                            break;
                    }
                }
            }
            catch (Exception ex)
            {
                throw ex;
            }
        }
        public override void MatchPlate(string plateValue, int customerID, string ZoneId)
        {
            try
            {
                //Get Data from DB
                this.GetRepositoryData(plateValue, customerID, ZoneId);

                //If no record  add, NoMatch recrod
                if (_PlateNos.Candidate.Count == 0)
                {
                    this._PlateNos.Candidate.Add(new PlateNo() { PlateStatus = PlateStatus.NoMatch, LicPlateWithOutState = plateValue, plate = plateValue });
                }

                //Occupied // Expired //Vacant
                foreach (var plate in this.Plates.Candidate)
                {
                    switch (plate.PlateStatusIncludingOther)
                    {
                        case PlateStatusIncludingOtherAndGenetec.NoMatch:
                            this.FillNoMatchPlate(plate);
                            break;
                        case PlateStatusIncludingOtherAndGenetec.Violated:
                            this.FillViolated(plate);
                            break;
                        case PlateStatusIncludingOtherAndGenetec.Expired:
                            this.FillExpired(plate);
                            break;
                        case PlateStatusIncludingOtherAndGenetec.Paid:
                            this.FillPaid(plate);
                            break;
                        case PlateStatusIncludingOtherAndGenetec.Permits:
                            this.FillPermits(plate);
                            break;
                        default:
                            this.FillOthers(plate, plate.SpaceStatus);
                            break;
                    }
                }
            }
            catch (Exception ex)
            {
                throw ex;
            }
        }
        protected override void FillOthers(PlateNo plate, string value)
        {
            plate.Ptype = "ANPR.Models.Candidate, ANPR";
            plate.confidence = 0;
            plate.matches_template = 0;
            plate.expired = false;
            plate.noMatches = false;
            plate.violation = false;
            plate.validPayments = true;
            plate.noMatches = false;
            plate.assignedClass = value;
        }
    }

    public class SpokaneWAFactory : GeneralFactory
    {
        public SpokaneWAFactory(int customerId, string plateNo)
            : base(customerId, plateNo)
        {

        }


        public override void MatchPlate(string plateValue, int customerID)
        {
            try
            {
                //Get Data from DB
                this.GetRepositoryData(plateValue, customerID);

                //If no record  add, NoMatch recrod
                if (_PlateNos.Candidate.Count == 0)
                {
                    this._PlateNos.Candidate.Add(new PlateNo() { PlateStatus = PlateStatus.NoMatch, LicPlateWithOutState = plateValue, plate = plateValue });
                }

                //Occupied // Expired //Vacant
                foreach (var plate in this.Plates.Candidate)
                {
                    switch (plate.PlateStatusIncludingOther)
                    {
                        case PlateStatusIncludingOtherAndGenetec.NoMatch:
                            this.FillNoMatchPlate(plate);
                            break;
                        case PlateStatusIncludingOtherAndGenetec.Violated:
                            this.FillViolated(plate);
                            break;
                        case PlateStatusIncludingOtherAndGenetec.Expired:
                            this.FillExpired(plate);
                            break;
                        case PlateStatusIncludingOtherAndGenetec.Paid:
                            this.FillPaid(plate);
                            break;
                        case PlateStatusIncludingOtherAndGenetec.Permits:
                            this.FillPermits(plate);
                            break;
                        default:
                            this.FillOthers(plate, plate.SpaceStatus);
                            break;
                    }
                }
            }
            catch (Exception ex)
            {
                throw ex;
            }
        }

        protected override void FillOthers(PlateNo plate, string value)
        {
            plate.Ptype = "ANPR.Models.Candidate, ANPR";
            plate.confidence = 0;
            plate.matches_template = 0;
            plate.expired = false;
            plate.noMatches = false;
            plate.violation = false;
            plate.validPayments = true;
            plate.noMatches = false;
            plate.assignedClass = value;
        }
    }

    public class TybeeIslandFactory : GeneralFactory
    {
        public TybeeIslandFactory(int customerId, string plateNo)
            : base(customerId, plateNo)
        {

        }


        public override void MatchPlate(string plateValue, int customerID)
        {
            try
            {
                //Get Data from DB
                this.GetRepositoryData(plateValue, customerID);

                //If no record  add, NoMatch recrod
                if (_PlateNos.Candidate.Count == 0)
                {
                    this._PlateNos.Candidate.Add(new PlateNo() { PlateStatus = PlateStatus.NoMatch, LicPlateWithOutState = plateValue, plate = plateValue });
                }


                //Occupied // Expired //Vacant
                foreach (var plate in this.Plates.Candidate)
                {
                    switch (plate.PlateStatusSpecificToMiamiAndAgencies)
                    {
                        case PlateStatusMiamiAndAgencies.NoMatch:
                            this.FillNoMatchPlate(plate);
                            break;
                        case PlateStatusMiamiAndAgencies.Violated:
                            this.FillViolated(plate);
                            break;
                        case PlateStatusMiamiAndAgencies.Expired:
                            this.FillExpired(plate);
                            break;
                        case PlateStatusMiamiAndAgencies.Paid:
                            this.FillPaid(plate);
                            break;
                        case PlateStatusMiamiAndAgencies.Permits:
                            this.FillPermits(plate);
                            break;
                        default:
                            this.FillOthers(plate, plate.SpaceStatus);
                            break;
                    }
                }

            }
            catch (Exception ex)
            {
                throw ex;
            }
        }

        protected override void FillPermits(PlateNo plate)
        {
            base.FillPermits(plate);
            plate.violation = true;
            plate.validPayments = false;
            plate.assignedClass = string.Format("{0}", "Exempt");
        }

        protected virtual void FillOthers(PlateNo plate, string value)
        {
            base.FillOthers(plate, value);
            plate.validPayments = true;
            plate.violation = false;
        }
    }

    public class PhiladelphiaFactory : GeneralFactory
    {
        public PhiladelphiaFactory(int customerId, string plateNo)
            : base(customerId, plateNo)
        {

        }

        public override void MatchPlate(string plateValue, int customerID)
        {
            try
            {
                //Get Data from DB
                this.GetRepositoryData(plateValue, customerID);

                //If no record  add, NoMatch recrod
                if (_PlateNos.Candidate.Count == 0)
                {
                    this._PlateNos.Candidate.Add(new PlateNo() { PlateStatus = PlateStatus.NoMatch, LicPlateWithOutState = plateValue, plate = plateValue });
                }


                //Occupied // Expired //Vacant
                foreach (var plate in this.Plates.Candidate)
                {
                    switch (plate.PlateStatus)
                    {
                        case PlateStatus.NoMatch:
                            this.FillNoMatchPlate(plate);
                            break;
                        case PlateStatus.Violated:
                            this.FillViolated(plate);
                            break;
                        case PlateStatus.Expired:
                            this.FillExpired(plate);
                            break;
                        case PlateStatus.Paid:
                            this.FillPaid(plate);
                            break;
                        case PlateStatus.Permits:
                            this.FillPermits(plate);
                            break;
                        default:
                            this.FillNoMatchPlate(plate);
                            break;
                    }
                }


            }
            catch (Exception ex)
            {
                throw ex;
            }
        }

        protected virtual void FillNoMatchPlate(PlateNo plate)
        {
            plate.Ptype = "ANPR.Models.Candidate, ANPR";
            plate.confidence = 0;
            plate.matches_template = 0;
            plate.violation = false;
            plate.expired = false;
            plate.validPayments = false;
            plate.noMatches = true;


        }

        protected virtual void FillPaid(PlateNo plate)
        {
            plate.Ptype = "ANPR.Models.Candidate, ANPR";
            plate.confidence = 0;
            plate.matches_template = 0;
            plate.violation = false;
            plate.expired = false;
            plate.validPayments = true;
            plate.noMatches = false;

        }

        protected virtual void FillExpired(PlateNo plate)
        {
            plate.Ptype = "ANPR.Models.Candidate, ANPR";
            plate.confidence = 0;
            plate.matches_template = 0;
            plate.violation = false;
            plate.expired = true;
            plate.validPayments = false;
            plate.noMatches = false;

        }

        protected virtual void FillViolated(PlateNo plate)
        {
            plate.Ptype = "ANPR.Models.Candidate, ANPR";
            plate.confidence = 0;
            plate.matches_template = 0;
            plate.violation = true;
            plate.expired = true;
            plate.validPayments = false;
            plate.noMatches = false;


        }

        protected virtual void FillPermits(PlateNo plate)
        {
            //Check permit status
            plate.Ptype = "ANPR.Models.Candidate, ANPR";
            plate.confidence = 0;
            plate.matches_template = 0;
            plate.violation = false;
            plate.expired = false;
            plate.validPayments = true;
            plate.noMatches = false;

        }

        protected virtual void FillOthers(PlateNo plate, string value)
        {
            plate.Ptype = "ANPR.Models.Candidate, ANPR";
            plate.confidence = 0;
            plate.matches_template = 0;
            plate.expired = false;
            plate.noMatches = false;
            plate.violation = true;
            plate.validPayments = false;
            plate.noMatches = false;

        }

        //public override void MatchPlate(string plateValue, int customerID)
        //{
        //    try
        //    {
        //        ////Get Data from DB
        //        //this.GetRepositoryData(plateValue, customerID);

        //        //Tempory hard-coding
        //        var datalist = new PlateNos();

        //        datalist.Candidate.Add
        //            (
        //             new PlateNo()
        //             {
        //                 LicPlateWithOutState = "ZXC123",
        //                 StateName = "[PA]",
        //                 plate = "ZXC123 [PA]",
        //                 confidence = 0,
        //                 matches_template = 0,
        //                 violation = false,
        //                 expired = true,
        //                 validPayments = false,
        //                 noMatches = false,
        //                 assignedClass = "ANNUAL PERMIT A1",
        //                 assignedClass2 = "EXPIRED",
        //                 assignedClass3 = "",
        //                 ValidAnyZone = null
        //             }
        //            );



        //        datalist.Candidate.Add
        //            (
        //             new PlateNo()
        //             {
        //                 LicPlateWithOutState = "QWE123",
        //                 StateName = "[PA]",
        //                 plate = "QWE123 [PA]",
        //                 confidence = 0,
        //                 matches_template = 0,
        //                 violation = false,
        //                 expired = false,
        //                 validPayments = true,
        //                 noMatches = false,
        //                 assignedClass = "ANNUAL PERMIT A1",
        //                 assignedClass2 = "PAID",
        //                 assignedClass3 = "",
        //                 ZoneName = "93101",
        //                 ExpiryTime = new DateTime(2019, 12, 12, 18, 50, 00),
        //                 ValidAnyZone = null
        //             }
        //            );

        //        datalist.Candidate.Add
        //         (
        //          new PlateNo()
        //          {
        //              LicPlateWithOutState = "ASD123",
        //              StateName = "[PA]",
        //              plate = "ASD123 [PA]",
        //              confidence = 0,
        //              matches_template = 0,
        //              violation = true,
        //              expired = false,
        //              validPayments = true,
        //              noMatches = false,
        //              assignedClass = "ANNUAL PERMIT A1",
        //              assignedClass2 = "PAID",
        //              assignedClass3 = "BOOT",
        //              ZoneName = "93101",
        //              ExpiryTime = new DateTime(2019, 12, 12, 18, 50, 00),
        //              ValidAnyZone = null
        //          }
        //         );




        //        datalist.Candidate.Add
        //        (
        //         new PlateNo()
        //         {
        //             LicPlateWithOutState = "POI987",
        //             StateName = "[PA]",
        //             plate = "POI987 [PA]",
        //             confidence = 0,
        //             matches_template = 0,
        //             violation = false,
        //             expired = true,
        //             validPayments = false,
        //             noMatches = false,
        //             assignedClass = "ANNUAL PERMIT A1",
        //             assignedClass2 = "EXPIRED",
        //             assignedClass3 = "",
        //             ValidAnyZone = null
        //         }
        //        );


        //        datalist.Candidate.Add
        //        (
        //         new PlateNo()
        //         {
        //             LicPlateWithOutState = "LKJ987",
        //             StateName = "[PA]",
        //             plate = "LKJ987 [PA]",
        //             confidence = 0,
        //             matches_template = 0,
        //             violation = false,
        //             expired = false,
        //             validPayments = true,
        //             noMatches = false,
        //             assignedClass = "ANNUAL PERMIT A1",
        //             assignedClass2 = "PAID",
        //             assignedClass3 = "",
        //             ZoneName = "93101",
        //             ExpiryTime = new DateTime(2019, 12, 12, 18, 50, 00),
        //             ValidAnyZone = null
        //         }
        //        );



        //        datalist.Candidate.Add
        //        (
        //         new PlateNo()
        //         {
        //             LicPlateWithOutState = "MNB987",
        //             StateName = "[PA]",
        //             plate = "MNB987 [PA]",
        //             confidence = 0,
        //             matches_template = 0,
        //             violation = true,
        //             expired = true,
        //             validPayments = false,
        //             noMatches = false,
        //             assignedClass = "",
        //             assignedClass2 = "EXPIRED",
        //             assignedClass3 = "BOOT",
        //             ValidAnyZone = null
        //         }
        //        );

        //        var singleData = datalist.Candidate.Where(t => t.LicPlateWithOutState.ToLower() == plateValue.ToLower()).FirstOrDefault();
        //        if (singleData != default(PlateNo))
        //            this._PlateNos.Candidate.Add(
        //               singleData
        //                );
        //        //end hard-coding

        //        //If no record  add, NoMatch recrod
        //        if (_PlateNos.Candidate.Count == 0)
        //        {
        //            this._PlateNos.Candidate.Add(new PlateNo() { PlateStatus = PlateStatus.NoMatch, LicPlateWithOutState = plateValue, plate = plateValue });
        //        }
        //        foreach (var plate in this.Plates.Candidate)
        //        {
        //            switch (plate.PlateStatusIncludingOther)
        //            {
        //                //case PlateStatusIncludingOtherAndGenetec.NoMatch:
        //                //    this.FillNoMatchPlate(plate);
        //                //    break;
        //            }
        //        }
        //        /*
        //        //Occupied // Expired //Vacant
        //        foreach (var plate in this.Plates.Candidate)
        //        {
        //            switch (plate.PlateStatusSpecificToMiamiAndAgencies)
        //            {
        //                case PlateStatusMiamiAndAgencies.NoMatch:
        //                    this.FillNoMatchPlate(plate);
        //                    break;
        //                case PlateStatusMiamiAndAgencies.Violated:
        //                    this.FillViolated(plate);
        //                    break;
        //                case PlateStatusMiamiAndAgencies.Expired:
        //                    this.FillExpired(plate);
        //                    break;
        //                case PlateStatusMiamiAndAgencies.Paid:
        //                    this.FillPaid(plate);
        //                    break;
        //                case PlateStatusMiamiAndAgencies.Permits:
        //                    this.FillPermits(plate);
        //                    break;
        //                default:
        //                    this.FillOthers(plate, plate.SpaceStatus);
        //                    break;
        //            }
        //        }
        //         * */

        //    }
        //    catch (Exception ex)
        //    {
        //        throw ex;
        //    }
        //}
    }

    public class GlendaleFactory : GeneralFactory
    {
        public GlendaleFactory(int customerId, string plateNo)
            : base(customerId, plateNo)
        {

        }
        public GlendaleFactory(int customerId, string plateNo, string zone)
            : base(customerId, plateNo, zone)
        {

        }

        public override void MatchPlate(string plateValue, int customerID)
        {
            try
            {
                //Get Data from DB
                this.GetRepositoryData(plateValue, customerID);

                //If no record  add, NoMatch recrod
                if (_PlateNos.Candidate.Count == 0)
                {
                    this._PlateNos.Candidate.Add(new PlateNo() { PlateStatus = PlateStatus.NoMatch, LicPlateWithOutState = plateValue, plate = plateValue });
                }


                //Occupied // Expired //Vacant
                foreach (var plate in this.Plates.Candidate)
                {
                    switch (plate.PlateStatus)
                    {
                        case PlateStatus.NoMatch:
                            this.FillNoMatchPlate(plate);
                            break;
                        case PlateStatus.Violated:
                            this.FillViolated(plate);
                            break;
                        case PlateStatus.Expired:
                            this.FillExpired(plate);
                            break;
                        case PlateStatus.Paid:
                            this.FillPaid(plate);
                            break;
                        case PlateStatus.Permits:
                            this.FillPermits(plate);
                            break;
                        default:
                            this.FillNoMatchPlate(plate);
                            break;
                    }
                }

                foreach (var plate in this.Plates.Candidate)
                {
                    switch (plate.LEStatus)
                    {
                        case LEStatus.Scofflaw:
                        case LEStatus.Tow:
                        case LEStatus.Boot:
                        case LEStatus.ExpiredParking:
                            plate.violation = true;
                            break;
                        default:
                            break;
                    }
                }
            }
            catch (Exception ex)
            {
                throw ex;
            }
        }

        public override void MatchPlate(string plateValue, int customerID, string Zone)
        {
            try
            {
                ///not fully implemented.never use
                //Get Data from DB
                this.GetRepositoryData(plateValue, customerID, Zone);

                //If no record  add, NoMatch recrod
                if (_PlateNos.Candidate.Count == 0)
                {
                    this._PlateNos.Candidate.Add(new PlateNo() { PlateStatus = PlateStatus.NoMatch, LicPlateWithOutState = plateValue, plate = plateValue });
                }


                //Occupied // Expired //Vacant
                foreach (var plate in this.Plates.Candidate)
                {
                    switch (plate.PlateStatus)
                    {
                        case PlateStatus.NoMatch:
                            this.FillNoMatchPlate(plate);
                            break;
                        case PlateStatus.Violated:
                            this.FillViolated(plate);
                            break;
                        case PlateStatus.Expired:
                            this.FillExpired(plate);
                            break;
                        case PlateStatus.Paid:
                            this.FillPaid(plate);
                            break;
                        case PlateStatus.Permits:
                            this.FillPermits(plate);
                            break;
                        default:
                            this.FillNoMatchPlate(plate);
                            break;
                    }
                }

                foreach (var plate in this.Plates.Candidate)
                {
                    switch (plate.LEStatus)
                    {
                        case LEStatus.Scofflaw:
                        case LEStatus.Tow:
                        case LEStatus.Boot:
                        case LEStatus.ExpiredParking:
                            plate.violation = true;
                            break;
                        default:
                            break;
                    }
                }
            }
            catch (Exception ex)
            {
                throw ex;
            }
        }


        protected virtual void FillNoMatchPlate(PlateNo plate)
        {
            plate.Ptype = "ANPR.Models.Candidate, ANPR";
            plate.confidence = 0;
            plate.matches_template = 0;
            plate.violation = false;
            plate.expired = false;
            plate.validPayments = false;
            plate.noMatches = true;


        }

        protected virtual void FillPaid(PlateNo plate)
        {
            plate.Ptype = "ANPR.Models.Candidate, ANPR";
            plate.confidence = 0;
            plate.matches_template = 0;
            plate.violation = false;
            plate.expired = false;
            plate.validPayments = true;
            plate.noMatches = false;

        }

        protected virtual void FillExpired(PlateNo plate)
        {
            plate.Ptype = "ANPR.Models.Candidate, ANPR";
            plate.confidence = 0;
            plate.matches_template = 0;
            plate.violation = false;
            plate.expired = true;
            plate.validPayments = false;
            plate.noMatches = false;

        }

        protected virtual void FillViolated(PlateNo plate)
        {
            plate.Ptype = "ANPR.Models.Candidate, ANPR";
            plate.confidence = 0;
            plate.matches_template = 0;
            plate.violation = true;
            plate.expired = true;
            plate.validPayments = false;
            plate.noMatches = false;


        }

        protected virtual void FillPermits(PlateNo plate)
        {
            //Check permit status
            plate.Ptype = "ANPR.Models.Candidate, ANPR";
            plate.confidence = 0;
            plate.matches_template = 0;
            plate.violation = false;
            plate.expired = false;
            plate.validPayments = true;
            plate.noMatches = false;

        }

        protected virtual void FillOthers(PlateNo plate, string value)
        {
            plate.Ptype = "ANPR.Models.Candidate, ANPR";
            plate.confidence = 0;
            plate.matches_template = 0;
            plate.expired = false;
            plate.noMatches = false;
            plate.violation = true;
            plate.validPayments = false;
            plate.noMatches = false;

        }


    }


    public class PhilliFactoryDualState : GeneralFactory
    {
        public PhilliFactoryDualState(int customerId, string plateNo)
            : base(customerId, plateNo)
        {

        }
        public PhilliFactoryDualState(int customerId, string plateNo, string zone)
            : base(customerId, plateNo, zone)
        {

        }

        public override void MatchPlate(string plateValue, int customerID)
        {
            try
            {
                //Get Data from DB
                this.GetRepositoryData(plateValue, customerID);

                //If no record  add, NoMatch recrod
                if (_PlateNos.Candidate.Count == 0)
                {
                    this._PlateNos.Candidate.Add(new PlateNo() { PlateStatus = PlateStatus.NoMatch, LicPlateWithOutState = plateValue, plate = plateValue });
                }


                //Occupied // Expired //Vacant
                foreach (var plate in this.Plates.Candidate)
                {
                    switch (plate.PlateStatus)
                    {
                        case PlateStatus.NoMatch:
                            this.FillNoMatchPlate(plate);
                            break;
                        case PlateStatus.Violated:
                            this.FillViolated(plate);
                            break;
                        case PlateStatus.Expired:
                            this.FillExpired(plate);
                            break;
                        case PlateStatus.Paid:
                            this.FillPaid(plate);
                            break;
                        case PlateStatus.Permits:
                            this.FillPermits(plate);
                            break;
                        default:
                            this.FillNoMatchPlate(plate);
                            break;
                    }
                }

                foreach (var plate in this.Plates.Candidate)
                {
                    switch (plate.LEStatus)
                    {
                        case LEStatus.Scofflaw:
                        case LEStatus.Tow:
                        case LEStatus.Boot:
                        case LEStatus.ExpiredParking:
                            plate.violation = true;
                            break;
                        default:
                            break;
                    }
                }
            }
            catch (Exception ex)
            {
                throw ex;
            }
        }

        public override void MatchPlate(string plateValue, int customerID, string Zone)
        {
            try
            {
                ///not fully implemented.never use
                //Get Data from DB
                this.GetRepositoryData(plateValue, customerID, Zone);

                //If no record  add, NoMatch recrod
                if (_PlateNos.Candidate.Count == 0)
                {
                    this._PlateNos.Candidate.Add(new PlateNo() { PlateStatus = PlateStatus.NoMatch, LicPlateWithOutState = plateValue, plate = plateValue });
                }


                //Occupied // Expired //Vacant
                foreach (var plate in this.Plates.Candidate)
                {
                    switch (plate.PlateStatus)
                    {
                        case PlateStatus.NoMatch:
                            this.FillNoMatchPlate(plate);
                            break;
                        case PlateStatus.Violated:
                            this.FillViolated(plate);
                            break;
                        case PlateStatus.Expired:
                            this.FillExpired(plate);
                            break;
                        case PlateStatus.Paid:
                            this.FillPaid(plate);
                            break;
                        case PlateStatus.Permits:
                            this.FillPermits(plate);
                            break;
                        default:
                            this.FillNoMatchPlate(plate);
                            break;
                    }
                }

                foreach (var plate in this.Plates.Candidate)
                {
                    switch (plate.LEStatus)
                    {
                        case LEStatus.Scofflaw:
                        case LEStatus.Tow:
                        case LEStatus.Boot:
                        case LEStatus.ExpiredParking:
                            plate.violation = true;
                            break;
                        default:
                            break;
                    }
                }
            }
            catch (Exception ex)
            {
                throw ex;
            }
        }


        protected virtual void FillNoMatchPlate(PlateNo plate)
        {
            plate.Ptype = "ANPR.Models.Candidate, ANPR";
            plate.confidence = 0;
            plate.matches_template = 0;
            plate.violation = false;
            plate.expired = false;
            plate.validPayments = false;
            plate.noMatches = true;


        }

        protected virtual void FillPaid(PlateNo plate)
        {
            plate.Ptype = "ANPR.Models.Candidate, ANPR";
            plate.confidence = 0;
            plate.matches_template = 0;
            plate.violation = false;
            plate.expired = false;
            plate.validPayments = true;
            plate.noMatches = false;

        }

        protected virtual void FillExpired(PlateNo plate)
        {
            plate.Ptype = "ANPR.Models.Candidate, ANPR";
            plate.confidence = 0;
            plate.matches_template = 0;
            plate.violation = false;
            plate.expired = true;
            plate.validPayments = false;
            plate.noMatches = false;

        }

        protected virtual void FillViolated(PlateNo plate)
        {
            plate.Ptype = "ANPR.Models.Candidate, ANPR";
            plate.confidence = 0;
            plate.matches_template = 0;
            plate.violation = true;

            if (string.IsNullOrEmpty(plate.assignedClass2))
            {
                plate.expired = true;
                plate.validPayments = false;
            }
            else
            {
                if (plate.assignedClass2.ToLower() == "paid".ToLower())
                {
                    plate.expired = false;
                    plate.validPayments = true;
                }
                else
                {
                    plate.expired = true;
                    plate.validPayments = false;
                }

            }
           
             
            plate.noMatches = false;


        }

        protected virtual void FillPermits(PlateNo plate)
        {
            //Check permit status
            plate.Ptype = "ANPR.Models.Candidate, ANPR";
            plate.confidence = 0;
            plate.matches_template = 0;
            plate.violation = false;
            plate.expired = false;
            plate.validPayments = true;
            plate.noMatches = false;

        }

        protected virtual void FillOthers(PlateNo plate, string value)
        {
            plate.Ptype = "ANPR.Models.Candidate, ANPR";
            plate.confidence = 0;
            plate.matches_template = 0;
            plate.expired = false;
            plate.noMatches = false;
            plate.violation = true;
            plate.validPayments = false;
            plate.noMatches = false;

        }


    }

}
