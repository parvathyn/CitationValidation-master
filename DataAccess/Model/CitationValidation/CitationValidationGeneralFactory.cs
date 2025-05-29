using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace DataAccess.Model.CitationValidation
{
    public abstract class CitationValidationGeneralFactory
    {
        private int _customerId;
        private string _key;
        private TransactionPaymentDetail _transData;
        private bool _IsInputValid;
        private InputDataGeneral _inputData;
        private string _timeZoneFilePath;
        private RequestResponseEntity _logData;
        private TimeZoneInfo _timeZoneInfo;
        public CitationValidationGeneralFactory(string key, int customerId, string timeZoneFilePath, RequestResponseEntity logData)
        {
            this._key = key;
            this._customerId = customerId;
            this._IsInputValid = false;
            this._transData = new TransactionPaymentDetail() { ReturnCode = ((int)ReturnCodeEnum.InvalidInputParameters).ToString() };
            this._inputData = new InputDataGeneral();
            this._timeZoneFilePath = timeZoneFilePath;
            this._logData = logData;
            _timeZoneInfo = TimeZoneInfo.Local;
        }

        protected string Key
        {
            get
            {
                return this._key;
            }
            set
            {
                this._key = value;
            }
        }
        protected int CustomerId
        {
            get
            {
                return this._customerId;
            }
            set
            {
                this._customerId = value;
            }
        }
        protected TransactionPaymentDetail TranData
        {
            get
            {
                return this._transData;
            }
            set
            {
                this._transData = value;
            }
        }
        protected bool IsInputValid
        {
            get
            {
                return this._IsInputValid;
            }
            set
            {
                this._IsInputValid = value;
            }
        }
        protected InputDataGeneral InputData
        {
            get
            {
                return this._inputData;
            }
            set
            {
                this._inputData = value;
            }
        }
        protected string TimeZoneFilePath
        {
            get
            {
                return this._timeZoneFilePath;
            }
        }
        protected RequestResponseEntity LogData
        {
            get
            {
                return this._logData;
            }
            set
            {
                this._logData = value;
            }
        }
        protected TimeZoneInfo MyTimeZoneInfo
        {
            get
            {
                return this._timeZoneInfo;
            }
            set
            {
                this._timeZoneInfo = value;
            }
        }

        protected bool Meter999999999()
        {
            bool retValue = false;
            if (this.InputData.MeterName == "999999999".ToString() || this.InputData.MeterId == 999999999)
            {
                this.TranData.ReturnCode = ((int)ReturnCodeEnum.NoRecordExists).ToString();
                this.LogData.Request.CustomerId = this.InputData.CustomerId.Value.ToString();
                this.LogData.Response = new ResponseEntity() { CustomerId = this.InputData.CustomerId.Value.ToString(), MeterId = this.InputData.MeterName, SpaceId = string.Empty, CityCurrentTime = DateTime.Now };
                retValue = true;
            }
            return retValue;
        }
        protected abstract void ValidateInputData();
        protected abstract void GetRepositoryData();
        public abstract TransactionPaymentDetail GetTransformData();

    }
    public class CitationValidationChicagoFactory : CitationValidationGeneralFactory
    {
        public CitationValidationChicagoFactory(string key, int customerId, string timeZoneFilePath, RequestResponseEntity logData)
            : base(key, customerId, timeZoneFilePath, logData)
        {
            this.TranData = new ChicagoTransactionPaymentDetail();
        }

        protected override void ValidateInputData()
        {
            try
            {
                string[] values = this.Key.Split('-');
                this.IsInputValid = true;
                ///////Customer Id////////////////////
                int? tempcustomerID = null;
                if (values[0] == null) //CustomerrId
                {
                    this.TranData.ReturnCode = ((int)ReturnCodeEnum.InvalidInputParameters).ToString();
                    this.IsInputValid = false;
                }
                else
                {
                    tempcustomerID = values[0].ToNullableInt();
                    if (tempcustomerID.HasValue == false)
                    {
                        this.TranData.ReturnCode = ((int)ReturnCodeEnum.InvalidInputParameters).ToString();
                        this.IsInputValid = false;
                    }
                }

                this.InputData.CustomerId = tempcustomerID;
                ///////End Customer Id////////////////////
                /////Use MeterName/////////////////////////////////////////////
                string tempMeterName = null;
                if (values[1] == null)
                {

                }
                else
                {
                    tempMeterName = values[1].ToString();
                    if (string.IsNullOrEmpty(tempMeterName))
                    {
                        //return tranData;
                    }
                }
                this.InputData.MeterName = tempMeterName;
                /////End MeterName/////////////////////////////////////////////
                ////ParkingSpaceID/////////////////////////////////////////////////////////////////
                string tempparkingSpaceID = null;
                if (values[2] == null)
                {

                }
                else
                {
                    tempparkingSpaceID = values[2].ToString();
                    if (string.IsNullOrEmpty(tempparkingSpaceID))
                    {
                        //return tranData;
                    }
                }
                this.InputData.parkingSpaceID = tempparkingSpaceID;
                ////End ParkingSpaceID/////////////////////////////////////////////////////////////////
                ////Plate Number///////////////////////////////////////
                string tempPlateNumber = null;
                if (values[3] == null) //Plate Number
                {

                }
                else
                {
                    tempPlateNumber = values[3].ToString();
                    if (string.IsNullOrEmpty(tempPlateNumber))
                    {
                        tempPlateNumber = null;
                    }
                }
                this.InputData.PlateNumber = tempPlateNumber;
                // //End Plate Number/////////////////////

                ////Plate Number///////////////////////////////////////
                string tempStatus = null;
                if (values[4] == null) //Plate Number
                {

                }
                else
                {
                    tempStatus = values[4].ToString();
                    if (string.IsNullOrEmpty(tempStatus))
                    {
                        tempStatus = null;
                    }
                }
                this.InputData.Status = tempStatus;
                // //End Plate Number/////////////////////

                /////Use State/////////////////////////////////////////////
                string tempState = null;
                if (values[5] == null)
                {

                }
                else
                {
                    tempState = values[5].ToString();
                    if (string.IsNullOrEmpty(tempState))
                    {
                        //return tranData;
                    }
                }
                this.InputData.StateName = tempState;
                /////End RateName/////////////////////////////////////////////

                /////Use RateName/////////////////////////////////////////////
                string tempRateName = null;
                if (values[6] == null)
                {

                }
                else
                {
                    tempRateName = values[6].ToString();
                    if (string.IsNullOrEmpty(tempRateName))
                    {
                        //return tranData;
                    }
                }
                this.InputData.RateName = tempRateName;
                /////End RateName/////////////////////////////////////////////

                /////Use RateName/////////////////////////////////////////////
                string tempReciptId = null;
                if (values[7] == null)
                {

                }
                else
                {
                    tempReciptId = values[7].ToString();
                    if (string.IsNullOrEmpty(tempReciptId))
                    {
                        //return tranData;
                    }
                }
                this.InputData.ReceiptId = tempReciptId;
                /////End RateName/////////////////////////////////////////////
            }
            catch
            {
                this.TranData.ReturnCode = ((int)ReturnCodeEnum.Error).ToString();
            }

        }

        protected override void GetRepositoryData()
        {
            try
            {
                Transaction data = new Transaction();
                List<Transaction> transactionList = new List<Transaction>();
                transactionList = DataAccess.GetCitationDataOthersWithPlateNameChicago(
                        this.InputData.CustomerId.Value,
                        null, // this.InputData.ZoneID,
                        this.InputData.MeterName,
                        this.InputData.parkingSpaceID,
                        this.InputData.PlateNumber,
                        null, //this.InputData.Status
                        //this.InputData.ReceiptId,
                        this.InputData.StateName,
                        this.InputData.RateName);

                //////////Check Multiplicity/////////////////
                if (transactionList.Count > 0)
                {
                    data = transactionList[0];
                }

                if (transactionList.Count > 1)
                {
                    data.ReturnCode = ((int)ReturnCodeEnum.MoreRecordsExists).ToString();
                }
                else if (transactionList.Count == 0)
                {
                    data.ReturnCode = ((int)ReturnCodeEnum.NoRecordExists).ToString();
                }
                else
                {
                    data.ReturnCode = ((int)ReturnCodeEnum.Success).ToString();
                }

                //////////End Check Multiplicity/////////////////

                if (data.TimeZoneId.HasValue)
                    this.MyTimeZoneInfo = Utility.GetTimeZoneInfo(data.TimeZoneId.Value, this.TimeZoneFilePath, this.InputData.CustomerId.Value.ToString());
                /////////////////////////////
                ///////////////Fill Out  Data/////////////////////////////////////
                //For South Miami : PayStationId. Others it is MeterID
                this.TranData.MeterID = data.PayStationID.HasValue ? data.PayStationID.Value.ToString() : "0";
                this.TranData.MeterName = (data.MeterName == null) ? string.Empty : data.MeterName;
                this.TranData.SpaceNo = data.ParkingSpaceID.ToString();

                this.TranData.MeterStreet = (data.MeterStreet == null) ? string.Empty : data.MeterStreet;

                DateTime _MeterRTC = data.PresentMeterTime.HasValue ? data.PresentMeterTime.Value : DateTime.Now;
                this.TranData.MeterRTC = String.Format("{0:yyyyMMdd_THHmmssZ}", Utility.findGMTTime(_MeterRTC, this.MyTimeZoneInfo));

                DateTime _SensorEventTime = data.SensorEventTime.HasValue ? data.SensorEventTime.Value : DateTime.Now;
                this.TranData.MeterLastUpdateTime = String.Format("{0:yyyyMMdd_THHmmssZ}", Utility.findGMTTime(_SensorEventTime, this.MyTimeZoneInfo));
                this.TranData.SensorEventTime = String.Format("{0:yyyyMMdd_THHmmssZ}", Utility.findGMTTime(_SensorEventTime, this.MyTimeZoneInfo));
                DateTime _endTime = data.ExpiryTime.HasValue ? data.ExpiryTime.Value : DateTime.Now;
                this.TranData.ExpiredTime = String.Format("{0:yyyyMMdd_THHmmssZ}", Utility.findGMTTime(_endTime, this.MyTimeZoneInfo));

                this.TranData.MeterExpiredMinutes = data.ExpiredMinutes.HasValue ? data.ExpiredMinutes.Value.ToString() : "0";
                this.TranData.METERLOC_BLOCKNUMBER = (data.Block == null) ? string.Empty : data.Block;
                this.TranData.METERLOC_STREETDIRECTION = (data.Direction == null) ? string.Empty : data.Direction;
                this.TranData.METERLOC_STREETNAME = (data.MeterStreet == null) ? string.Empty : data.MeterStreet;
                this.TranData.METERLOC_STREETTYPE = (data.StreetType == null) ? string.Empty : data.StreetType;
                this.TranData.METERLOC_CROSSSTREET1 = (data.CrossStreet1 == null) ? string.Empty : data.CrossStreet1;
                this.TranData.METERLOC_CROSSSTREET2 = (data.CrossStreet2 == null) ? string.Empty : data.CrossStreet2;
                this.TranData.METERLOC_ENFORCEMENTHOURSDESC = (data.EnfHourDesc == null) ? string.Empty : data.EnfHourDesc;
                this.TranData.LicensePlate = (data.PlateNumber == null) ? string.Empty : data.PlateNumber;
                this.TranData.State = (data.LPLocation == null) ? string.Empty : data.LPLocation;
                this.TranData.Make = (data.VMake == null) ? string.Empty : data.VMake;
                this.TranData.Model = (data.VModel == null) ? string.Empty : data.VModel;
                this.TranData.IsOccupied = Utility.getStatus((data.SpaceStatus == null) ? string.Empty : data.SpaceStatus).ToString();
                this.TranData.EnforcementKey = (data.EnforcementKey == null) ? string.Empty : data.EnforcementKey;
                this.TranData.MeterStatus = (data.MeterStatus == null) ? null : data.MeterStatus;
                this.TranData.METERLOC_METERTYPE = (data.METERLOC_METERTYPE == null) ? null : data.METERLOC_METERTYPE;
                this.TranData.ZoneName = (data.ZoneName == null) ? null : data.ZoneName;
                ((ChicagoTransactionPaymentDetail)(this.TranData)).Recieptid = (data.Recieptid == null) ? null : data.Recieptid;
                ((ChicagoTransactionPaymentDetail)(this.TranData)).RateNumber = (data.RateNumber == null) ? null : data.RateNumber;
                this.TranData.ReturnCode = data.ReturnCode;


                this.LogData.Response = new ResponseEntity() { CustomerId = this.InputData.CustomerId.Value.ToString(), MeterId = this.TranData.MeterName, SpaceId = this.TranData.SpaceNo, CityCurrentTime = data.PresentMeterTime.HasValue ? data.PresentMeterTime.Value : DateTime.Now };
                ///////////////End Fill Out Put Data///////////////////////////////
            }
            catch (Exception ex)
            {
                this.TranData.ReturnCode = ((int)ReturnCodeEnum.Error).ToString();
            }

        }

        public override TransactionPaymentDetail GetTransformData()
        {
            //Validate input-data////////
            this.ValidateInputData();
            if (this.TranData.ReturnCode == ((int)ReturnCodeEnum.Error).ToString())
                return this.TranData;
            if (this.IsInputValid == false)
                return this.TranData;
            ////End Validate input-data///
            this.LogData.Request.CustomerId = this.InputData.CustomerId.ToString();
            this.LogData.Request.SpaceId = this.InputData.parkingSpaceID.ToString();
            //Check No Data Enforcement
            if (this.Meter999999999())
                return this.TranData;

            //Call Database Data
            this.GetRepositoryData();
            if (this.TranData.ReturnCode == ((int)ReturnCodeEnum.Error).ToString())
                return this.TranData;

            return this.TranData;
        }
    }

    public class CitationValidationCoralGablesFactory : CitationValidationGeneralFactory
    {
        public CitationValidationCoralGablesFactory(string key, int customerId, string timeZoneFilePath, RequestResponseEntity logData)
            : base(key, customerId, timeZoneFilePath, logData)
        {

        }

        protected override void ValidateInputData()
        {
            try
            {
                string[] values = this.Key.Split('-');
                this.IsInputValid = true;
                ///////Customer Id////////////////////
                int? tempcustomerID = null;
                if (values[0] == null) //CustomerrId
                {
                    this.TranData.ReturnCode = ((int)ReturnCodeEnum.InvalidInputParameters).ToString();
                    this.IsInputValid = false;
                }
                else
                {
                    tempcustomerID = values[0].ToNullableInt();
                    if (tempcustomerID.HasValue == false)
                    {
                        this.TranData.ReturnCode = ((int)ReturnCodeEnum.InvalidInputParameters).ToString();
                        this.IsInputValid = false;
                    }
                }

                this.InputData.CustomerId = tempcustomerID;
                ///////End Customer Id////////////////////
                /////Use MeterName/////////////////////////////////////////////
                string tempMeterName = null;
                if (values[1] == null)
                {

                }
                else
                {
                    tempMeterName = values[1].ToString();
                    if (string.IsNullOrEmpty(tempMeterName))
                    {
                        //return tranData;
                    }
                }
                this.InputData.MeterName = tempMeterName;
                /////End MeterName/////////////////////////////////////////////
                ////ParkingSpaceID/////////////////////////////////////////////////////////////////
                string tempparkingSpaceID = null;
                if (values[2] == null)
                {

                }
                else
                {
                    tempparkingSpaceID = values[2].ToString();
                    if (string.IsNullOrEmpty(tempparkingSpaceID))
                    {
                        //return tranData;
                    }
                }
                this.InputData.parkingSpaceID = tempparkingSpaceID;
                ////End ParkingSpaceID/////////////////////////////////////////////////////////////////
                ////Plate Number///////////////////////////////////////
                string tempPlateNumber = null;
                if (values[3] == null) //Plate Number
                {

                }
                else
                {
                    tempPlateNumber = values[3].ToString();
                    if (string.IsNullOrEmpty(tempPlateNumber))
                    {
                        tempPlateNumber = null;
                    }
                }
                this.InputData.PlateNumber = tempPlateNumber;
                // //End Plate Number/////////////////////

                ////Plate Number///////////////////////////////////////
                string tempStatus = null;
                if (values[4] == null) //Plate Number
                {

                }
                else
                {
                    tempStatus = values[4].ToString();
                    if (string.IsNullOrEmpty(tempStatus))
                    {
                        tempStatus = null;
                    }
                }
                this.InputData.Status = tempStatus;
                // //End Plate Number/////////////////////
            }
            catch
            {
                this.TranData.ReturnCode = ((int)ReturnCodeEnum.Error).ToString();
            }

        }

        protected override void GetRepositoryData()
        {
            try
            {
                Transaction data = new Transaction();
                List<Transaction> transactionList = new List<Transaction>();
                transactionList = DataAccess.GetCitationDataOthersWithPlateNameCoralGables(
                        this.InputData.CustomerId.Value,
                        null, // this.InputData.ZoneID,
                        this.InputData.MeterName,
                        this.InputData.parkingSpaceID,
                        this.InputData.PlateNumber,
                        null);

                //////////Check Multiplicity/////////////////
                if (transactionList.Count > 0)
                {
                    data = transactionList[0];
                }

                if (transactionList.Count > 1)
                {
                    data.ReturnCode = ((int)ReturnCodeEnum.MoreRecordsExists).ToString();
                }
                else if (transactionList.Count == 0)
                {
                    data.ReturnCode = ((int)ReturnCodeEnum.NoRecordExists).ToString();
                }
                else
                {
                    data.ReturnCode = ((int)ReturnCodeEnum.Success).ToString();
                }

                //////////End Check Multiplicity/////////////////

                if (data.TimeZoneId.HasValue)
                    this.MyTimeZoneInfo = Utility.GetTimeZoneInfo(data.TimeZoneId.Value, this.TimeZoneFilePath, this.InputData.CustomerId.Value.ToString());
                /////////////////////////////
                ///////////////Fill Out  Data/////////////////////////////////////
                //For South Miami : PayStationId. Others it is MeterID
                this.TranData.MeterID = data.PayStationID.HasValue ? data.PayStationID.Value.ToString() : "0";
                this.TranData.MeterName = (data.MeterName == null) ? string.Empty : data.MeterName;
                this.TranData.SpaceNo = data.ParkingSpaceID.ToString();

                this.TranData.MeterStreet = (data.MeterStreet == null) ? string.Empty : data.MeterStreet;

                DateTime _MeterRTC = data.PresentMeterTime.HasValue ? data.PresentMeterTime.Value : DateTime.Now;
                this.TranData.MeterRTC = String.Format("{0:yyyyMMdd_THHmmssZ}", Utility.findGMTTime(_MeterRTC, this.MyTimeZoneInfo));

                DateTime _SensorEventTime = data.SensorEventTime.HasValue ? data.SensorEventTime.Value : DateTime.Now;
                this.TranData.MeterLastUpdateTime = String.Format("{0:yyyyMMdd_THHmmssZ}", Utility.findGMTTime(_SensorEventTime, this.MyTimeZoneInfo));
                this.TranData.SensorEventTime = String.Format("{0:yyyyMMdd_THHmmssZ}", Utility.findGMTTime(_SensorEventTime, this.MyTimeZoneInfo));
                DateTime _endTime = data.ExpiryTime.HasValue ? data.ExpiryTime.Value : DateTime.Now;
                this.TranData.ExpiredTime = String.Format("{0:yyyyMMdd_THHmmssZ}", Utility.findGMTTime(_endTime, this.MyTimeZoneInfo));

                this.TranData.MeterExpiredMinutes = data.ExpiredMinutes.HasValue ? data.ExpiredMinutes.Value.ToString() : "0";
                this.TranData.METERLOC_BLOCKNUMBER = (data.Block == null) ? string.Empty : data.Block;
                this.TranData.METERLOC_STREETDIRECTION = (data.Direction == null) ? string.Empty : data.Direction;
                this.TranData.METERLOC_STREETNAME = (data.MeterStreet == null) ? string.Empty : data.MeterStreet;
                this.TranData.METERLOC_STREETTYPE = (data.StreetType == null) ? string.Empty : data.StreetType;
                this.TranData.METERLOC_CROSSSTREET1 = (data.CrossStreet1 == null) ? string.Empty : data.CrossStreet1;
                this.TranData.METERLOC_CROSSSTREET2 = (data.CrossStreet2 == null) ? string.Empty : data.CrossStreet2;
                this.TranData.METERLOC_ENFORCEMENTHOURSDESC = (data.EnfHourDesc == null) ? string.Empty : data.EnfHourDesc;
                this.TranData.LicensePlate = (data.PlateNumber == null) ? string.Empty : data.PlateNumber;
                this.TranData.State = (data.LPLocation == null) ? string.Empty : data.LPLocation;
                this.TranData.Make = (data.VMake == null) ? string.Empty : data.VMake;
                this.TranData.Model = (data.VModel == null) ? string.Empty : data.VModel;
                this.TranData.IsOccupied = Utility.getStatus((data.SpaceStatus == null) ? string.Empty : data.SpaceStatus).ToString();
                this.TranData.EnforcementKey = (data.EnforcementKey == null) ? string.Empty : data.EnforcementKey;
                this.TranData.MeterStatus = (data.MeterStatus == null) ? null : data.MeterStatus;
                this.TranData.METERLOC_METERTYPE = (data.METERLOC_METERTYPE == null) ? null : data.METERLOC_METERTYPE;
                this.TranData.ZoneName = (data.ZoneName == null) ? null : data.ZoneName;

                this.TranData.ReturnCode = data.ReturnCode;


                this.LogData.Response = new ResponseEntity() { CustomerId = this.InputData.CustomerId.Value.ToString(), MeterId = this.TranData.MeterName, SpaceId = this.TranData.SpaceNo, CityCurrentTime = data.PresentMeterTime.HasValue ? data.PresentMeterTime.Value : DateTime.Now };
                ///////////////End Fill Out Put Data///////////////////////////////
            }
            catch (Exception ex)
            {
                this.TranData.ReturnCode = ((int)ReturnCodeEnum.Error).ToString();
            }

        }

        public override TransactionPaymentDetail GetTransformData()
        {
            //Validate input-data////////
            this.ValidateInputData();
            if (this.TranData.ReturnCode == ((int)ReturnCodeEnum.Error).ToString())
                return this.TranData;
            if (this.IsInputValid == false)
                return this.TranData;
            ////End Validate input-data///
            this.LogData.Request.CustomerId = this.InputData.CustomerId.ToString();
            this.LogData.Request.SpaceId = this.InputData.parkingSpaceID.ToString();
            //Check No Data Enforcement
            if (this.Meter999999999())
                return this.TranData;

            //Call Database Data
            this.GetRepositoryData();
            if (this.TranData.ReturnCode == ((int)ReturnCodeEnum.Error).ToString())
                return this.TranData;

            return this.TranData;
        }
    }

    public class CitationValidationRaleighFactory : CitationValidationGeneralFactory
    {
        public CitationValidationRaleighFactory(string key, int customerId, string timeZoneFilePath, RequestResponseEntity logData)
            : base(key, customerId, timeZoneFilePath, logData)
        {

        }

        protected override void ValidateInputData()
        {
            try
            {
                string[] values = this.Key.Split('-');
                this.IsInputValid = true;
                ///////Customer Id////////////////////
                int? tempcustomerID = null;
                if (values[0] == null) //CustomerrId
                {
                    this.TranData.ReturnCode = ((int)ReturnCodeEnum.InvalidInputParameters).ToString();
                    this.IsInputValid = false;
                }
                else
                {
                    tempcustomerID = values[0].ToNullableInt();
                    if (tempcustomerID.HasValue == false)
                    {
                        this.TranData.ReturnCode = ((int)ReturnCodeEnum.InvalidInputParameters).ToString();
                        this.IsInputValid = false;
                    }
                }

                this.InputData.CustomerId = tempcustomerID;
                ///////End Customer Id////////////////////
                /////Use MeterName/////////////////////////////////////////////
                string tempMeterName = null;
                if (values[1] == null)
                {

                }
                else
                {
                    tempMeterName = values[1].ToString();
                    if (string.IsNullOrEmpty(tempMeterName))
                    {
                        //return tranData;
                    }
                }
                this.InputData.MeterName = tempMeterName;
                /////End MeterName/////////////////////////////////////////////
                ////ParkingSpaceID/////////////////////////////////////////////////////////////////
                string tempparkingSpaceID = null;
                if (values[2] == null)
                {

                }
                else
                {
                    tempparkingSpaceID = values[2].ToString();
                    if (string.IsNullOrEmpty(tempparkingSpaceID))
                    {
                        //return tranData;
                    }
                }
                this.InputData.parkingSpaceID = tempparkingSpaceID;
                ////End ParkingSpaceID/////////////////////////////////////////////////////////////////
                ////Plate Number///////////////////////////////////////
                string tempPlateNumber = null;
                if (values[3] == null) //Plate Number
                {

                }
                else
                {
                    tempPlateNumber = values[3].ToString();
                    if (string.IsNullOrEmpty(tempPlateNumber))
                    {
                        tempPlateNumber = null;
                    }
                }
                this.InputData.PlateNumber = tempPlateNumber;
                // //End Plate Number/////////////////////

                ////Plate Number///////////////////////////////////////
                string tempStatus = null;
                if (values[4] == null) //Plate Number
                {

                }
                else
                {
                    tempStatus = values[4].ToString();
                    if (string.IsNullOrEmpty(tempStatus))
                    {
                        tempStatus = null;
                    }
                }
                this.InputData.Status = tempStatus;
                // //End Plate Number/////////////////////
            }
            catch
            {
                this.TranData.ReturnCode = ((int)ReturnCodeEnum.Error).ToString();
            }

        }

        protected override void GetRepositoryData()
        {
            try
            {
                Transaction data = new Transaction();
                List<Transaction> transactionList = new List<Transaction>();
                transactionList = DataAccess.GetCitationDataRaleigh(
                        this.InputData.CustomerId.Value,
                        null, // this.InputData.ZoneID,
                        this.InputData.MeterName,
                        this.InputData.parkingSpaceID,
                        this.InputData.PlateNumber,
                        null);

                //////////Check Multiplicity/////////////////
                if (transactionList.Count > 0)
                {
                    data = transactionList[0];
                }

                if (transactionList.Count > 1)
                {
                    data.ReturnCode = ((int)ReturnCodeEnum.MoreRecordsExists).ToString();
                }
                else if (transactionList.Count == 0)
                {
                    data.ReturnCode = ((int)ReturnCodeEnum.NoRecordExists).ToString();
                }
                else
                {
                    data.ReturnCode = ((int)ReturnCodeEnum.Success).ToString();
                }

                //////////End Check Multiplicity/////////////////

                if (data.TimeZoneId.HasValue)
                    this.MyTimeZoneInfo = Utility.GetTimeZoneInfo(data.TimeZoneId.Value, this.TimeZoneFilePath, this.InputData.CustomerId.Value.ToString());
                /////////////////////////////
                ///////////////Fill Out  Data/////////////////////////////////////
                //For South Miami : PayStationId. Others it is MeterID
                this.TranData.MeterID = data.PayStationID.HasValue ? data.PayStationID.Value.ToString() : "0";
                this.TranData.MeterName = (data.MeterName == null) ? string.Empty : data.MeterName;
                this.TranData.SpaceNo = data.ParkingSpaceID.ToString();

                this.TranData.MeterStreet = (data.MeterStreet == null) ? string.Empty : data.MeterStreet;

                DateTime _MeterRTC = data.PresentMeterTime.HasValue ? data.PresentMeterTime.Value : DateTime.Now;
                this.TranData.MeterRTC = String.Format("{0:yyyyMMdd_THHmmssZ}", Utility.findGMTTime(_MeterRTC, this.MyTimeZoneInfo));

                DateTime _SensorEventTime = data.SensorEventTime.HasValue ? data.SensorEventTime.Value : DateTime.Now;
                this.TranData.MeterLastUpdateTime = String.Format("{0:yyyyMMdd_THHmmssZ}", Utility.findGMTTime(_SensorEventTime, this.MyTimeZoneInfo));
                this.TranData.SensorEventTime = String.Format("{0:yyyyMMdd_THHmmssZ}", Utility.findGMTTime(_SensorEventTime, this.MyTimeZoneInfo));
                DateTime _endTime = data.ExpiryTime.HasValue ? data.ExpiryTime.Value : DateTime.Now;
                this.TranData.ExpiredTime = String.Format("{0:yyyyMMdd_THHmmssZ}", Utility.findGMTTime(_endTime, this.MyTimeZoneInfo));

                this.TranData.MeterExpiredMinutes = data.ExpiredMinutes.HasValue ? data.ExpiredMinutes.Value.ToString() : "0";
                this.TranData.METERLOC_BLOCKNUMBER = (data.Block == null) ? string.Empty : data.Block;
                this.TranData.METERLOC_STREETDIRECTION = (data.Direction == null) ? string.Empty : data.Direction;
                this.TranData.METERLOC_STREETNAME = (data.MeterStreet == null) ? string.Empty : data.MeterStreet;
                this.TranData.METERLOC_STREETTYPE = (data.StreetType == null) ? string.Empty : data.StreetType;
                this.TranData.METERLOC_CROSSSTREET1 = (data.CrossStreet1 == null) ? string.Empty : data.CrossStreet1;
                this.TranData.METERLOC_CROSSSTREET2 = (data.CrossStreet2 == null) ? string.Empty : data.CrossStreet2;
                this.TranData.METERLOC_ENFORCEMENTHOURSDESC = (data.EnfHourDesc == null) ? string.Empty : data.EnfHourDesc;
                this.TranData.LicensePlate = (data.PlateNumber == null) ? string.Empty : data.PlateNumber;
                this.TranData.State = (data.LPLocation == null) ? string.Empty : data.LPLocation;
                this.TranData.Make = (data.VMake == null) ? string.Empty : data.VMake;
                this.TranData.Model = (data.VModel == null) ? string.Empty : data.VModel;
                this.TranData.IsOccupied = Utility.getStatus((data.SpaceStatus == null) ? string.Empty : data.SpaceStatus).ToString();
                this.TranData.EnforcementKey = (data.EnforcementKey == null) ? string.Empty : data.EnforcementKey;
                this.TranData.MeterStatus = (data.MeterStatus == null) ? null : data.MeterStatus;
                this.TranData.METERLOC_METERTYPE = (data.METERLOC_METERTYPE == null) ? null : data.METERLOC_METERTYPE;
                this.TranData.ZoneName = (data.ZoneName == null) ? null : data.ZoneName;

                this.TranData.ReturnCode = data.ReturnCode;


                this.LogData.Response = new ResponseEntity() { CustomerId = this.InputData.CustomerId.Value.ToString(), MeterId = this.TranData.MeterName, SpaceId = this.TranData.SpaceNo, CityCurrentTime = data.PresentMeterTime.HasValue ? data.PresentMeterTime.Value : DateTime.Now };
                ///////////////End Fill Out Put Data///////////////////////////////
            }
            catch (Exception ex)
            {
                this.TranData.ReturnCode = ((int)ReturnCodeEnum.Error).ToString();
            }

        }

        public override TransactionPaymentDetail GetTransformData()
        {
            //Validate input-data////////
            this.ValidateInputData();
            if (this.TranData.ReturnCode == ((int)ReturnCodeEnum.Error).ToString())
                return this.TranData;
            if (this.IsInputValid == false)
                return this.TranData;
            ////End Validate input-data///
            this.LogData.Request.CustomerId = this.InputData.CustomerId.ToString();
            this.LogData.Request.SpaceId = this.InputData.parkingSpaceID.ToString();
            //Check No Data Enforcement
            if (this.Meter999999999())
                return this.TranData;

            //Call Database Data
            this.GetRepositoryData();
            if (this.TranData.ReturnCode == ((int)ReturnCodeEnum.Error).ToString())
                return this.TranData;

            return this.TranData;
        }
    }

    public class CitationValidationSouthMiamiFactory : CitationValidationGeneralFactory
    {
        public CitationValidationSouthMiamiFactory(string key, int customerId, string timeZoneFilePath, RequestResponseEntity logData)
            : base(key, customerId, timeZoneFilePath, logData)
        {

        }

        protected override void ValidateInputData()
        {
            try
            {
                string[] values = this.Key.Split('-');
                this.IsInputValid = true;
                ///////Customer Id////////////////////
                int? tempcustomerID = null;
                if (values[0] == null) //CustomerrId
                {
                    this.TranData.ReturnCode = ((int)ReturnCodeEnum.InvalidInputParameters).ToString();
                    this.IsInputValid = false;
                }
                else
                {
                    tempcustomerID = values[0].ToNullableInt();
                    if (tempcustomerID.HasValue == false)
                    {
                        this.TranData.ReturnCode = ((int)ReturnCodeEnum.InvalidInputParameters).ToString();
                        this.IsInputValid = false;
                    }
                }

                this.InputData.CustomerId = tempcustomerID;
                ///////End Customer Id////////////////////
                /////Use MeterName/////////////////////////////////////////////
                string tempMeterName = null;
                if (values[1] == null)
                {

                }
                else
                {
                    tempMeterName = values[1].ToString();
                    if (string.IsNullOrEmpty(tempMeterName))
                    {
                        //return tranData;
                    }
                }
                this.InputData.MeterName = tempMeterName;
                /////End MeterName/////////////////////////////////////////////
                ////ParkingSpaceID/////////////////////////////////////////////////////////////////
                string tempparkingSpaceID = null;
                if (values[2] == null)
                {

                }
                else
                {
                    tempparkingSpaceID = values[2].ToString();
                    if (string.IsNullOrEmpty(tempparkingSpaceID))
                    {
                        //return tranData;
                    }
                }
                this.InputData.parkingSpaceID = tempparkingSpaceID;
                ////End ParkingSpaceID/////////////////////////////////////////////////////////////////
                ////Plate Number///////////////////////////////////////
                string tempPlateNumber = null;
                if (values[3] == null) //Plate Number
                {

                }
                else
                {
                    tempPlateNumber = values[3].ToString();
                    if (string.IsNullOrEmpty(tempPlateNumber))
                    {
                        tempPlateNumber = null;
                    }
                }
                this.InputData.PlateNumber = tempPlateNumber;
                // //End Plate Number/////////////////////

                ////Plate Number///////////////////////////////////////
                string tempStatus = null;
                if (values[4] == null) //Plate Number
                {

                }
                else
                {
                    tempStatus = values[4].ToString();
                    if (string.IsNullOrEmpty(tempStatus))
                    {
                        tempStatus = null;
                    }
                }
                this.InputData.Status = tempStatus;
                // //End Plate Number/////////////////////
            }
            catch
            {
                this.TranData.ReturnCode = ((int)ReturnCodeEnum.Error).ToString();
            }

        }

        protected override void GetRepositoryData()
        {
            try
            {
                Transaction data = new Transaction();
                List<Transaction> transactionList = new List<Transaction>();
                transactionList = DataAccess.GetCitationDataSouthMaimi(
                        this.InputData.CustomerId.Value,
                        null, // this.InputData.ZoneID,
                        null,
                        this.InputData.parkingSpaceID,
                        this.InputData.PlateNumber,
                        null);

                //////////Check Multiplicity/////////////////
                if (transactionList.Count > 0)
                {
                    data = transactionList[0];
                }

                if (transactionList.Count > 1)
                {
                    data.ReturnCode = ((int)ReturnCodeEnum.MoreRecordsExists).ToString();
                }
                else if (transactionList.Count == 0)
                {
                    data.ReturnCode = ((int)ReturnCodeEnum.NoRecordExists).ToString();
                }
                else
                {
                    data.ReturnCode = ((int)ReturnCodeEnum.Success).ToString();
                }

                //////////End Check Multiplicity/////////////////

                if (data.TimeZoneId.HasValue)
                    this.MyTimeZoneInfo = Utility.GetTimeZoneInfo(data.TimeZoneId.Value, this.TimeZoneFilePath, this.InputData.CustomerId.Value.ToString());
                /////////////////////////////
                ///////////////Fill Out  Data/////////////////////////////////////
                //For South Miami : PayStationId. Others it is MeterID
                this.TranData.MeterID = data.PayStationID.HasValue ? data.PayStationID.Value.ToString() : "0";
                this.TranData.MeterName = (data.MeterName == null) ? string.Empty : data.MeterName;
                this.TranData.SpaceNo = data.ParkingSpaceID.ToString();

                this.TranData.MeterStreet = (data.MeterStreet == null) ? string.Empty : data.MeterStreet;

                DateTime _MeterRTC = data.PresentMeterTime.HasValue ? data.PresentMeterTime.Value : DateTime.Now;
                this.TranData.MeterRTC = String.Format("{0:yyyyMMdd_THHmmssZ}", Utility.findGMTTime(_MeterRTC, this.MyTimeZoneInfo));

                DateTime _SensorEventTime = data.SensorEventTime.HasValue ? data.SensorEventTime.Value : DateTime.Now;
                this.TranData.MeterLastUpdateTime = String.Format("{0:yyyyMMdd_THHmmssZ}", Utility.findGMTTime(_SensorEventTime, this.MyTimeZoneInfo));
                this.TranData.SensorEventTime = String.Format("{0:yyyyMMdd_THHmmssZ}", Utility.findGMTTime(_SensorEventTime, this.MyTimeZoneInfo));
                DateTime _endTime = data.ExpiryTime.HasValue ? data.ExpiryTime.Value : DateTime.Now;
                this.TranData.ExpiredTime = String.Format("{0:yyyyMMdd_THHmmssZ}", Utility.findGMTTime(_endTime, this.MyTimeZoneInfo));

                this.TranData.MeterExpiredMinutes = data.ExpiredMinutes.HasValue ? data.ExpiredMinutes.Value.ToString() : "0";
                this.TranData.METERLOC_BLOCKNUMBER = (data.Block == null) ? string.Empty : data.Block;
                this.TranData.METERLOC_STREETDIRECTION = (data.Direction == null) ? string.Empty : data.Direction;
                this.TranData.METERLOC_STREETNAME = (data.MeterStreet == null) ? string.Empty : data.MeterStreet;
                this.TranData.METERLOC_STREETTYPE = (data.StreetType == null) ? string.Empty : data.StreetType;
                this.TranData.METERLOC_CROSSSTREET1 = (data.CrossStreet1 == null) ? string.Empty : data.CrossStreet1;
                this.TranData.METERLOC_CROSSSTREET2 = (data.CrossStreet2 == null) ? string.Empty : data.CrossStreet2;
                this.TranData.METERLOC_ENFORCEMENTHOURSDESC = (data.EnfHourDesc == null) ? string.Empty : data.EnfHourDesc;
                this.TranData.LicensePlate = (data.PlateNumber == null) ? string.Empty : data.PlateNumber;
                this.TranData.State = (data.LPLocation == null) ? string.Empty : data.LPLocation;
                this.TranData.Make = (data.VMake == null) ? string.Empty : data.VMake;
                this.TranData.Model = (data.VModel == null) ? string.Empty : data.VModel;
                this.TranData.IsOccupied = Utility.getStatus((data.SpaceStatus == null) ? string.Empty : data.SpaceStatus).ToString();
                this.TranData.EnforcementKey = (data.EnforcementKey == null) ? string.Empty : data.EnforcementKey;
                this.TranData.MeterStatus = (data.MeterStatus == null) ? null : data.MeterStatus;
                this.TranData.METERLOC_METERTYPE = (data.METERLOC_METERTYPE == null) ? null : data.METERLOC_METERTYPE;
                this.TranData.ZoneName = (data.ZoneName == null) ? null : data.ZoneName;

                this.TranData.ReturnCode = data.ReturnCode;


                this.LogData.Response = new ResponseEntity() { CustomerId = this.InputData.CustomerId.Value.ToString(), MeterId = this.TranData.MeterName, SpaceId = this.TranData.SpaceNo, CityCurrentTime = data.PresentMeterTime.HasValue ? data.PresentMeterTime.Value : DateTime.Now };
                ///////////////End Fill Out Put Data///////////////////////////////
            }
            catch (Exception ex)
            {
                this.TranData.ReturnCode = ((int)ReturnCodeEnum.Error).ToString();
            }

        }

        public override TransactionPaymentDetail GetTransformData()
        {
            //Validate input-data////////
            this.ValidateInputData();
            if (this.TranData.ReturnCode == ((int)ReturnCodeEnum.Error).ToString())
                return this.TranData;
            if (this.IsInputValid == false)
                return this.TranData;
            ////End Validate input-data///
            this.LogData.Request.CustomerId = this.InputData.CustomerId.ToString();
            this.LogData.Request.SpaceId = this.InputData.parkingSpaceID.ToString();
            //Check No Data Enforcement
            if (this.Meter999999999())
                return this.TranData;

            //Call Database Data
            this.GetRepositoryData();
            if (this.TranData.ReturnCode == ((int)ReturnCodeEnum.Error).ToString())
                return this.TranData;

            return this.TranData;
        }
    }

    public class CitationValidationBirminghamMIFactory : CitationValidationGeneralFactory
    {
        public CitationValidationBirminghamMIFactory(string key, int customerId, string timeZoneFilePath, RequestResponseEntity logData)
            : base(key, customerId, timeZoneFilePath, logData)
        {

        }

        protected override void ValidateInputData()
        {
            try
            {
                string[] values = this.Key.Split('-');
                string tempKey = this.Key;   //e.g. sample 4194-7901-1-1--Expired
                string[] tempValues = new string[5];
                tempValues[0] = values[0];
                tempValues[1] = string.Format("{0}-{1}", values[1], values[2]);
                tempValues[2] = values[3];
                tempValues[3] = values[4];
                values = tempValues;



                this.IsInputValid = true;
                ///////Customer Id////////////////////
                int? tempcustomerID = null;
                if (values[0] == null) //CustomerrId
                {
                    this.TranData.ReturnCode = ((int)ReturnCodeEnum.InvalidInputParameters).ToString();
                    this.IsInputValid = false;
                }
                else
                {
                    tempcustomerID = values[0].ToNullableInt();
                    if (tempcustomerID.HasValue == false)
                    {
                        this.TranData.ReturnCode = ((int)ReturnCodeEnum.InvalidInputParameters).ToString();
                        this.IsInputValid = false;
                    }
                }

                this.InputData.CustomerId = tempcustomerID;
                ///////End Customer Id////////////////////
                /////Use MeterName/////////////////////////////////////////////
                string tempMeterName = null;
                if (values[1] == null)
                {

                }
                else
                {
                    tempMeterName = values[1].ToString();
                    if (string.IsNullOrEmpty(tempMeterName))
                    {
                        //return tranData;
                    }
                }
                this.InputData.MeterName = tempMeterName;
                /////End MeterName/////////////////////////////////////////////
                ////ParkingSpaceID/////////////////////////////////////////////////////////////////
                string tempparkingSpaceID = null;
                if (values[2] == null)
                {

                }
                else
                {
                    tempparkingSpaceID = values[2].ToString();
                    if (string.IsNullOrEmpty(tempparkingSpaceID))
                    {
                        //return tranData;
                    }
                }
                this.InputData.parkingSpaceID = tempparkingSpaceID;
                ////End ParkingSpaceID/////////////////////////////////////////////////////////////////
                ////Plate Number///////////////////////////////////////
                string tempPlateNumber = null;
                if (values[3] == null) //Plate Number
                {

                }
                else
                {
                    tempPlateNumber = values[3].ToString();
                    if (string.IsNullOrEmpty(tempPlateNumber))
                    {
                        tempPlateNumber = null;
                    }
                }
                this.InputData.PlateNumber = tempPlateNumber;
                // //End Plate Number/////////////////////

                ////Plate Number///////////////////////////////////////
                string tempStatus = null;
                if (values[4] == null) //Plate Number
                {

                }
                else
                {
                    tempStatus = values[4].ToString();
                    if (string.IsNullOrEmpty(tempStatus))
                    {
                        tempStatus = null;
                    }
                }
                this.InputData.Status = tempStatus;
                // //End Plate Number/////////////////////
            }
            catch
            {
                this.TranData.ReturnCode = ((int)ReturnCodeEnum.Error).ToString();
            }

        }

        protected override void GetRepositoryData()
        {
            try
            {
                string constValue = "999999999";
                if (this.InputData.MeterName == constValue || this.InputData.MeterName == "0".ToString() || this.InputData.MeterName == "9999".ToString())
                {
                    this.InputData.MeterName = null;
                    if (this.InputData.parkingSpaceID != constValue)
                    {
                        this.InputData.MeterName = this.InputData.parkingSpaceID;
                        this.InputData.parkingSpaceID = null;
                    }
                    else
                    {
                        this.InputData.parkingSpaceID = null;
                    }
                }
                else
                {
                    if (this.InputData.parkingSpaceID == constValue || this.InputData.parkingSpaceID == "0".ToString() || this.InputData.parkingSpaceID == "9999".ToString())
                    {
                        this.InputData.parkingSpaceID = null;
                    }
                }

                Transaction data = new Transaction();
                List<Transaction> transactionList = new List<Transaction>();
                transactionList = DataAccess.GetCitationDataRaleigh(
                         this.InputData.CustomerId.Value,
                         null, // this.InputData.ZoneID,
                         this.InputData.MeterName,
                         this.InputData.parkingSpaceID,
                         this.InputData.PlateNumber,
                         null);

                //////////Check Multiplicity/////////////////
                if (transactionList.Count > 0)
                {
                    data = transactionList[0];
                }

                if (transactionList.Count > 1)
                {
                    data.ReturnCode = ((int)ReturnCodeEnum.MoreRecordsExists).ToString();
                }
                else if (transactionList.Count == 0)
                {
                    data.ReturnCode = ((int)ReturnCodeEnum.NoRecordExists).ToString();
                }
                else
                {
                    data.ReturnCode = ((int)ReturnCodeEnum.Success).ToString();
                }

                //////////End Check Multiplicity/////////////////

                if (data.TimeZoneId.HasValue)
                    this.MyTimeZoneInfo = Utility.GetTimeZoneInfo(data.TimeZoneId.Value, this.TimeZoneFilePath, this.InputData.CustomerId.Value.ToString());
                /////////////////////////////
                ///////////////Fill Out  Data/////////////////////////////////////
                //For South Miami : PayStationId. Others it is MeterID
                this.TranData.MeterID = data.PayStationID.HasValue ? data.PayStationID.Value.ToString() : "0";

                var meternumberAndSpaceNo = (data.MeterName == null) ? string.Empty.Split('-') : data.MeterName.Split('-');
                if (meternumberAndSpaceNo.Length > 0)
                    this.TranData.MeterName = meternumberAndSpaceNo[0];
                else
                    this.TranData.MeterName = string.Empty;

                if (meternumberAndSpaceNo.Length > 1)
                    this.TranData.SpaceNo = meternumberAndSpaceNo[1];
                else
                    this.TranData.SpaceNo = string.Empty;

                this.TranData.MeterStreet = (data.MeterStreet == null) ? string.Empty : data.MeterStreet;

                DateTime _MeterRTC = data.PresentMeterTime.HasValue ? data.PresentMeterTime.Value : DateTime.Now;
                this.TranData.MeterRTC = String.Format("{0:yyyyMMdd_THHmmssZ}", Utility.findGMTTime(_MeterRTC, this.MyTimeZoneInfo));

                DateTime _SensorEventTime = data.SensorEventTime.HasValue ? data.SensorEventTime.Value : DateTime.Now;
                this.TranData.MeterLastUpdateTime = String.Format("{0:yyyyMMdd_THHmmssZ}", Utility.findGMTTime(_SensorEventTime, this.MyTimeZoneInfo));
                this.TranData.SensorEventTime = String.Format("{0:yyyyMMdd_THHmmssZ}", Utility.findGMTTime(_SensorEventTime, this.MyTimeZoneInfo));
                DateTime _endTime = data.ExpiryTime.HasValue ? data.ExpiryTime.Value : DateTime.Now;
                this.TranData.ExpiredTime = String.Format("{0:yyyyMMdd_THHmmssZ}", Utility.findGMTTime(_endTime, this.MyTimeZoneInfo));

                this.TranData.MeterExpiredMinutes = data.ExpiredMinutes.HasValue ? data.ExpiredMinutes.Value.ToString() : "0";
                this.TranData.METERLOC_BLOCKNUMBER = (data.Block == null) ? string.Empty : data.Block;
                this.TranData.METERLOC_STREETDIRECTION = (data.Direction == null) ? string.Empty : data.Direction;
                this.TranData.METERLOC_STREETNAME = (data.MeterStreet == null) ? string.Empty : data.MeterStreet;
                this.TranData.METERLOC_STREETTYPE = (data.StreetType == null) ? string.Empty : data.StreetType;
                this.TranData.METERLOC_CROSSSTREET1 = (data.CrossStreet1 == null) ? string.Empty : data.CrossStreet1;
                this.TranData.METERLOC_CROSSSTREET2 = (data.CrossStreet2 == null) ? string.Empty : data.CrossStreet2;
                this.TranData.METERLOC_ENFORCEMENTHOURSDESC = (data.EnfHourDesc == null) ? string.Empty : data.EnfHourDesc;
                this.TranData.LicensePlate = (data.PlateNumber == null) ? string.Empty : data.PlateNumber;
                this.TranData.State = (data.LPLocation == null) ? string.Empty : data.LPLocation;
                this.TranData.Make = (data.VMake == null) ? string.Empty : data.VMake;
                this.TranData.Model = (data.VModel == null) ? string.Empty : data.VModel;
                this.TranData.IsOccupied = Utility.getStatus((data.SpaceStatus == null) ? string.Empty : data.SpaceStatus).ToString();
                this.TranData.EnforcementKey = (data.EnforcementKey == null) ? string.Empty : data.EnforcementKey;
                this.TranData.MeterStatus = (data.MeterStatus == null) ? null : data.MeterStatus;
                this.TranData.METERLOC_METERTYPE = (data.METERLOC_METERTYPE == null) ? null : data.METERLOC_METERTYPE;
                this.TranData.ZoneName = (data.ZoneName == null) ? null : data.ZoneName;

                this.TranData.ReturnCode = data.ReturnCode;


                this.LogData.Response = new ResponseEntity() { CustomerId = this.InputData.CustomerId.Value.ToString(), MeterId = this.TranData.MeterName, SpaceId = this.TranData.SpaceNo, CityCurrentTime = data.PresentMeterTime.HasValue ? data.PresentMeterTime.Value : DateTime.Now };
                ///////////////End Fill Out Put Data///////////////////////////////
            }
            catch (Exception ex)
            {
                this.TranData.ReturnCode = ((int)ReturnCodeEnum.Error).ToString();
            }

        }

        public override TransactionPaymentDetail GetTransformData()
        {
            //Validate input-data////////
            this.ValidateInputData();
            if (this.TranData.ReturnCode == ((int)ReturnCodeEnum.Error).ToString())
                return this.TranData;
            if (this.IsInputValid == false)
                return this.TranData;
            ////End Validate input-data///
            this.LogData.Request.CustomerId = this.InputData.CustomerId.ToString();
            this.LogData.Request.SpaceId = this.InputData.parkingSpaceID.ToString();
            //Check No Data Enforcement
            if (this.Meter999999999())
                return this.TranData;

            //Call Database Data
            this.GetRepositoryData();
            if (this.TranData.ReturnCode == ((int)ReturnCodeEnum.Error).ToString())
                return this.TranData;

            return this.TranData;
        }
    }

    public class CitationValidationCrystalLakeFactory : CitationValidationGeneralFactory
    {
        public CitationValidationCrystalLakeFactory(string key, int customerId, string timeZoneFilePath, RequestResponseEntity logData)
            : base(key, customerId, timeZoneFilePath, logData)
        {

        }

        protected override void ValidateInputData()
        {
            try
            {
                string[] values = this.Key.Split('-');
                this.IsInputValid = true;
                ///////Customer Id////////////////////
                int? tempcustomerID = null;
                if (values[0] == null) //CustomerrId
                {
                    this.TranData.ReturnCode = ((int)ReturnCodeEnum.InvalidInputParameters).ToString();
                    this.IsInputValid = false;
                }
                else
                {
                    tempcustomerID = values[0].ToNullableInt();
                    if (tempcustomerID.HasValue == false)
                    {
                        this.TranData.ReturnCode = ((int)ReturnCodeEnum.InvalidInputParameters).ToString();
                        this.IsInputValid = false;
                    }
                }

                this.InputData.CustomerId = tempcustomerID;
                ///////End Customer Id////////////////////
                /////Use MeterName/////////////////////////////////////////////
                string tempMeterName = null;
                if (values[1] == null)
                {

                }
                else
                {
                    tempMeterName = values[1].ToString();
                    if (string.IsNullOrEmpty(tempMeterName))
                    {
                        //return tranData;
                    }
                }
                this.InputData.MeterName = tempMeterName;
                /////End MeterName/////////////////////////////////////////////
                ////ParkingSpaceID/////////////////////////////////////////////////////////////////
                string tempparkingSpaceID = null;
                if (values[2] == null)
                {

                }
                else
                {
                    tempparkingSpaceID = values[2].ToString();
                    if (string.IsNullOrEmpty(tempparkingSpaceID))
                    {
                        //return tranData;
                    }
                }
                this.InputData.parkingSpaceID = tempparkingSpaceID;
                ////End ParkingSpaceID/////////////////////////////////////////////////////////////////
                ////Plate Number///////////////////////////////////////
                string tempPlateNumber = null;
                if (values[3] == null) //Plate Number
                {

                }
                else
                {
                    tempPlateNumber = values[3].ToString();
                    if (string.IsNullOrEmpty(tempPlateNumber))
                    {
                        tempPlateNumber = null;
                    }
                }
                this.InputData.PlateNumber = tempPlateNumber;
                // //End Plate Number/////////////////////

                ////Plate Number///////////////////////////////////////
                string tempStatus = null;
                if (values[4] == null) //Plate Number
                {

                }
                else
                {
                    tempStatus = values[4].ToString();
                    if (string.IsNullOrEmpty(tempStatus))
                    {
                        tempStatus = null;
                    }
                }
                this.InputData.Status = tempStatus;
                // //End Plate Number/////////////////////
            }
            catch
            {
                this.TranData.ReturnCode = ((int)ReturnCodeEnum.Error).ToString();
            }

        }

        protected override void GetRepositoryData()
        {
            try
            {
                Transaction data = new Transaction();
                List<Transaction> transactionList = new List<Transaction>();
                transactionList = DataAccess.GetCitationDataSouthMaimi(
                        this.InputData.CustomerId.Value,
                        null, // this.InputData.ZoneID,
                        null,
                        this.InputData.parkingSpaceID,
                        this.InputData.PlateNumber,
                        null);

                //////////Check Multiplicity/////////////////
                if (transactionList.Count > 0)
                {
                    data = transactionList[0];
                }

                if (transactionList.Count > 1)
                {
                    data.ReturnCode = ((int)ReturnCodeEnum.MoreRecordsExists).ToString();
                }
                else if (transactionList.Count == 0)
                {
                    data.ReturnCode = ((int)ReturnCodeEnum.NoRecordExists).ToString();
                }
                else
                {
                    data.ReturnCode = ((int)ReturnCodeEnum.Success).ToString();
                }

                //////////End Check Multiplicity/////////////////

                if (data.TimeZoneId.HasValue)
                    this.MyTimeZoneInfo = Utility.GetTimeZoneInfo(data.TimeZoneId.Value, this.TimeZoneFilePath, this.InputData.CustomerId.Value.ToString());
                /////////////////////////////
                ///////////////Fill Out  Data/////////////////////////////////////
                //For South Miami : PayStationId. Others it is MeterID
                this.TranData.MeterID = data.PayStationID.HasValue ? data.PayStationID.Value.ToString() : "0";
                this.TranData.MeterName = (data.MeterName == null) ? string.Empty : data.MeterName;
                this.TranData.SpaceNo = data.ParkingSpaceID.ToString();

                this.TranData.MeterStreet = (data.MeterStreet == null) ? string.Empty : data.MeterStreet;

                DateTime _MeterRTC = data.PresentMeterTime.HasValue ? data.PresentMeterTime.Value : DateTime.Now;
                this.TranData.MeterRTC = String.Format("{0:yyyyMMdd_THHmmssZ}", Utility.findGMTTime(_MeterRTC, this.MyTimeZoneInfo));

                DateTime _SensorEventTime = data.SensorEventTime.HasValue ? data.SensorEventTime.Value : DateTime.Now;
                this.TranData.MeterLastUpdateTime = String.Format("{0:yyyyMMdd_THHmmssZ}", Utility.findGMTTime(_SensorEventTime, this.MyTimeZoneInfo));
                this.TranData.SensorEventTime = String.Format("{0:yyyyMMdd_THHmmssZ}", Utility.findGMTTime(_SensorEventTime, this.MyTimeZoneInfo));
                DateTime _endTime = data.ExpiryTime.HasValue ? data.ExpiryTime.Value : DateTime.Now;
                this.TranData.ExpiredTime = String.Format("{0:yyyyMMdd_THHmmssZ}", Utility.findGMTTime(_endTime, this.MyTimeZoneInfo));

                this.TranData.MeterExpiredMinutes = data.ExpiredMinutes.HasValue ? data.ExpiredMinutes.Value.ToString() : "0";
                this.TranData.METERLOC_BLOCKNUMBER = (data.Block == null) ? string.Empty : data.Block;
                this.TranData.METERLOC_STREETDIRECTION = (data.Direction == null) ? string.Empty : data.Direction;
                this.TranData.METERLOC_STREETNAME = (data.MeterStreet == null) ? string.Empty : data.MeterStreet;
                this.TranData.METERLOC_STREETTYPE = (data.StreetType == null) ? string.Empty : data.StreetType;
                this.TranData.METERLOC_CROSSSTREET1 = (data.CrossStreet1 == null) ? string.Empty : data.CrossStreet1;
                this.TranData.METERLOC_CROSSSTREET2 = (data.CrossStreet2 == null) ? string.Empty : data.CrossStreet2;
                this.TranData.METERLOC_ENFORCEMENTHOURSDESC = (data.EnfHourDesc == null) ? string.Empty : data.EnfHourDesc;
                this.TranData.LicensePlate = (data.PlateNumber == null) ? string.Empty : data.PlateNumber;
                this.TranData.State = (data.LPLocation == null) ? string.Empty : data.LPLocation;
                this.TranData.Make = (data.VMake == null) ? string.Empty : data.VMake;
                this.TranData.Model = (data.VModel == null) ? string.Empty : data.VModel;
                this.TranData.IsOccupied = Utility.getStatus((data.SpaceStatus == null) ? string.Empty : data.SpaceStatus).ToString();
                this.TranData.EnforcementKey = (data.EnforcementKey == null) ? string.Empty : data.EnforcementKey;
                this.TranData.MeterStatus = (data.MeterStatus == null) ? null : data.MeterStatus;
                this.TranData.METERLOC_METERTYPE = (data.METERLOC_METERTYPE == null) ? null : data.METERLOC_METERTYPE;
                this.TranData.ZoneName = (data.ZoneName == null) ? null : data.ZoneName;

                this.TranData.ReturnCode = data.ReturnCode;

                this.LogData.Response = new ResponseEntity() { CustomerId = this.InputData.CustomerId.Value.ToString(), MeterId = this.TranData.MeterName, SpaceId = this.TranData.SpaceNo, CityCurrentTime = data.PresentMeterTime.HasValue ? data.PresentMeterTime.Value : DateTime.Now };
                ///////////////End Fill Out Put Data///////////////////////////////
            }
            catch (Exception ex)
            {
                this.TranData.ReturnCode = ((int)ReturnCodeEnum.Error).ToString();
            }

        }

        public override TransactionPaymentDetail GetTransformData()
        {
            //Validate input-data////////
            this.ValidateInputData();
            if (this.TranData.ReturnCode == ((int)ReturnCodeEnum.Error).ToString())
                return this.TranData;
            if (this.IsInputValid == false)
                return this.TranData;
            ////End Validate input-data///
            this.LogData.Request.CustomerId = this.InputData.CustomerId.ToString();
            this.LogData.Request.SpaceId = this.InputData.parkingSpaceID.ToString();
            //Check No Data Enforcement
            if (this.Meter999999999())
                return this.TranData;

            //Call Database Data
            this.GetRepositoryData();
            if (this.TranData.ReturnCode == ((int)ReturnCodeEnum.Error).ToString())
                return this.TranData;

            return this.TranData;
        }
    }
    public class InputDataGeneral
    {
        public int? CustomerId { get; set; }
        public string MeterName { get; set; }
        public int MeterId { get; set; }
        public string parkingSpaceID { get; set; }
        public string PlateNumber { get; set; }
        public string Status { get; set; }
        public string StateName { get; set; }
        public string PlateWithOutState { get; set; }
        public string ZoneID { get; set; }
        public string RateName { get; set; }
        public string ReceiptId { get; set; }
    }
}
