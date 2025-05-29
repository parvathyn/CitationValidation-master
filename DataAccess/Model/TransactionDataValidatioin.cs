using DataAccess.Model.AceParking;
using DataAccess.Model.Pango;
using DataAccess.Model.Parkeon;
using DataAccess.Model.Parkmobile;
using DataAccess.Model.PassportMonitoring;
using DataAccess.Model.PayByPhone;
using DataAccess.Model.IPC;
using System;
using System.Collections.Generic;
using System.Collections.Specialized;
using System.IO;
using System.Linq;
using System.Text.RegularExpressions;
using System.Data.SqlClient;
using System.Data;

namespace DataAccess.Model
{
    public class TransactionDataValidatioin
    {

        public static TransactionTransform GetTransformData(string EnforcementKey, string filePath, RequestResponseEntity logData)
        {
            TransactionTransform tranData = new TransactionTransform();
            tranData.ReturnCode = ((int)ReturnCodeEnum.InvalidInputParameters).ToString();
            int? customerID = null;
            string MeterName = null;
            string parkingSpaceID;
            string PlateNumber = null;

            //Validate Input data
            string[] values = EnforcementKey.Split('-');
            /////Check existance of Customerid and metername as minimum/////////////////
            if (values[0] == null) //CustomerrId
            {
                return tranData;
            }
            else
            {
                customerID = values[0].ToNullableInt();
                if (customerID.HasValue == false)
                {
                    return tranData;
                }
            }



            ///////////////////
            TimeZoneInfo TZ = null;
            if (customerID.Value == 4102)
            {
                TZ = TimeZoneInfo.FindSystemTimeZoneById("Central Standard Time");
                // TZ = TimeZoneInfo.FindSystemTimeZoneById("Eastern Standard Time");
            }
            else
            {
                TZ = TimeZoneInfo.FindSystemTimeZoneById("Eastern Standard Time");
            }

            ////Special Cases/////////////////////////////
            if (customerID.Value == 4194)
            {
                string tempKey = EnforcementKey;   //e.g. sample 4194-7901-1-1--Expired
                string[] tempValues = new string[5];
                tempValues[0] = values[0];
                tempValues[1] = string.Format("{0}-{1}", values[1], values[2]);
                tempValues[2] = values[3];
                tempValues[3] = values[4];
                values = tempValues;
            }
            ////End Special Cases///////////////////////
            /////Use MeterName/////////////////////////////////////////////
            if (values[1] == null)
            {
                return tranData;
            }
            else
            {
                MeterName = values[1].ToString();
                if (string.IsNullOrEmpty(MeterName))
                {
                    //return tranData;
                }
            }
            /////End MeterName/////////////////////////////////////////////
            //Special case ///
            //If metername = 999999999 then simply return and no need to check
            if (MeterName == "999999999".ToString())
            {
                tranData.ReturnCode = ((int)ReturnCodeEnum.NoRecordExists).ToString();
                logData.Request.CustomerId = customerID.ToString();
                logData.Response = new ResponseEntity() { CustomerId = customerID.Value.ToString(), MeterId = MeterName, SpaceId = string.Empty, CityCurrentTime = DateTime.Now };
                return tranData;
            }
            //End Special case ///



            ////ParkingSpaceID/////////////////////////////////////////////////////////////////
            if (values[2] == null)
            {
                return tranData;
            }
            else
            {
                parkingSpaceID = values[2].ToString();
                if (string.IsNullOrEmpty(parkingSpaceID))
                {
                    //return tranData;
                }
            }
            ////End ParkingSpaceID/////////////////////////////////////////////////////////////////
            //Plate Number//
            if (values[3] != null) //Plate Number
            {
                PlateNumber = values[3].ToString();
                if (string.IsNullOrEmpty(PlateNumber))
                {
                    PlateNumber = null;
                }
            }
            //End Plate Number//



            try
            {
                Transaction data = TransactionDataValidatioin.GetData(customerID.Value, MeterName, parkingSpaceID, PlateNumber, EnforcementKey, filePath, logData);


                if (data.TimeZoneId.HasValue)
                    TZ = Utility.GetTimeZoneInfo(data.TimeZoneId.Value, filePath, customerID.Value.ToString());

                //For South Miami : PayStationId. Others it is MeterID
                tranData.MeterID = data.PayStationID.HasValue ? data.PayStationID.Value.ToString() : "0";
                if (customerID.HasValue)
                    if (customerID.Value == 4194)
                    {
                        var meternumberAndSpaceNo = (data.MeterName == null) ? string.Empty.Split('-') : data.MeterName.Split('-');
                        if (meternumberAndSpaceNo.Length > 0)
                            tranData.MeterName = meternumberAndSpaceNo[0];
                        else
                            tranData.MeterName = string.Empty;

                        if (meternumberAndSpaceNo.Length > 1)
                            tranData.SpaceNo = meternumberAndSpaceNo[1];
                        else
                            tranData.SpaceNo = string.Empty;
                    }
                    else
                    {
                        tranData.MeterName = (data.MeterName == null) ? string.Empty : data.MeterName;
                        tranData.SpaceNo = data.ParkingSpaceID.ToString();
                    }

                tranData.MeterStreet = (data.MeterStreet == null) ? string.Empty : data.MeterStreet;

                DateTime _MeterRTC = data.PresentMeterTime.HasValue ? data.PresentMeterTime.Value : DateTime.Now;
                tranData.MeterRTC = String.Format("{0:yyyyMMdd_THHmmssZ}", Utility.findGMTTime(_MeterRTC, TZ));

                DateTime _SensorEventTime = data.SensorEventTime.HasValue ? data.SensorEventTime.Value : DateTime.Now;
                tranData.MeterLastUpdateTime = String.Format("{0:yyyyMMdd_THHmmssZ}", Utility.findGMTTime(_SensorEventTime, TZ));

                tranData.SensorEventTime = String.Format("{0:yyyyMMdd_THHmmssZ}", Utility.findGMTTime(_SensorEventTime, TZ));

                DateTime _endTime = data.ExpiryTime.HasValue ? data.ExpiryTime.Value : DateTime.Now;
                tranData.ExpiredTime = String.Format("{0:yyyyMMdd_THHmmssZ}", Utility.findGMTTime(_endTime, TZ));

                tranData.MeterExpiredMinutes = data.ExpiredMinutes.HasValue ? data.ExpiredMinutes.Value.ToString() : "0";
                tranData.METERLOC_BLOCKNUMBER = (data.Block == null) ? string.Empty : data.Block;
                tranData.METERLOC_STREETDIRECTION = (data.Direction == null) ? string.Empty : data.Direction;
                tranData.METERLOC_STREETNAME = (data.MeterStreet == null) ? string.Empty : data.MeterStreet;
                tranData.METERLOC_STREETTYPE = (data.StreetType == null) ? string.Empty : data.StreetType;
                tranData.METERLOC_CROSSSTREET1 = (data.CrossStreet1 == null) ? string.Empty : data.CrossStreet1;
                tranData.METERLOC_CROSSSTREET2 = (data.CrossStreet2 == null) ? string.Empty : data.CrossStreet2;
                tranData.METERLOC_ENFORCEMENTHOURSDESC = (data.EnfHourDesc == null) ? string.Empty : data.EnfHourDesc;
                tranData.LicensePlate = (data.PlateNumber == null) ? string.Empty : data.PlateNumber;
                tranData.State = (data.LPLocation == null) ? string.Empty : data.LPLocation;
                tranData.Make = (data.VMake == null) ? string.Empty : data.VMake;
                tranData.Model = (data.VModel == null) ? string.Empty : data.VModel;
                tranData.IsOccupied = Utility.getStatus((data.SpaceStatus == null) ? string.Empty : data.SpaceStatus).ToString();
                tranData.EnforcementKey = (data.EnforcementKey == null) ? string.Empty : data.EnforcementKey;
                tranData.MeterStatus = (data.MeterStatus == null) ? null : data.MeterStatus;

                tranData.METERLOC_METERTYPE = (data.METERLOC_METERTYPE == null) ? null : data.METERLOC_METERTYPE;
                tranData.ZoneName = (data.ZoneName == null) ? null : data.ZoneName;

                tranData.ReturnCode = data.ReturnCode;



                if (customerID == 7001 || customerID == 4210)
                {
                    logData.Response = new ResponseEntity() { CustomerId = customerID.Value.ToString(), MeterId = tranData.MeterName, SpaceId = tranData.SpaceNo, CityCurrentTime = data.PresentMeterTime.HasValue ? data.PresentMeterTime.Value : DateTime.Now };
                }
                else
                {
                    logData.Response = new ResponseEntity() { CustomerId = customerID.Value.ToString(), MeterId = tranData.MeterName, SpaceId = tranData.SpaceNo, CityCurrentTime = data.PresentMeterTime.HasValue ? data.PresentMeterTime.Value : DateTime.Now };
                }
            }
            catch (Exception ex)
            {
                var appPath = System.Web.Hosting.HostingEnvironment.ApplicationPhysicalPath;
                string errorPath = Path.Combine(appPath, "GetTransformData", "Timezones");
                ExceptionLogging.SendErrorToText(ex, "GetCitationData", errorPath);
                tranData.ReturnCode = ((int)ReturnCodeEnum.Error).ToString();

            }
            return tranData;
        }

        private static Transaction GetData(int customerID, string MeterName, string parkingSpaceID, string PlateNumber, string EnforcementKey, string filePath, RequestResponseEntity logData)
        {
            string ZoneID = null;
            string SpaceStatus = null;
            Transaction data = new Transaction();
            try
            {
                List<Transaction> transactionList = new List<Transaction>();

                logData.Request.CustomerId = customerID.ToString();
                logData.Request.SpaceId = parkingSpaceID.ToString();
                if (customerID == 7001 || customerID == 4210)
                {
                    logData.Request.MeterId = MeterName;
                    MeterName = null; // south Miami is not meter based but Space no
                    transactionList = DataAccess.GetCitationData(customerID, ZoneID, MeterName, parkingSpaceID, PlateNumber, SpaceStatus);
                }


                else
                {
                    logData.Request.MeterId = MeterName;
                    if (customerID == 4194)
                    {
                        string constValue = "999999999";
                        if (MeterName == constValue || MeterName == "0".ToString() || MeterName == "9999".ToString())
                        {
                            MeterName = null;
                            if (parkingSpaceID != constValue)
                            {
                                MeterName = parkingSpaceID;
                                parkingSpaceID = null;
                            }
                            else
                            {
                                parkingSpaceID = null;
                            }
                        }
                        else
                        {
                            if (parkingSpaceID == constValue || parkingSpaceID == "0".ToString() || parkingSpaceID == "9999".ToString())
                            {
                                parkingSpaceID = null;
                            }
                        }

                        transactionList = DataAccess.GetCitationDataOthers(customerID, ZoneID, MeterName,
                            parkingSpaceID,
                            PlateNumber, SpaceStatus);
                    }
                    else
                        transactionList = DataAccess.GetCitationDataOthers(customerID, ZoneID, MeterName, parkingSpaceID, PlateNumber, SpaceStatus);
                }


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
            }
            catch (Exception ex)
            {
                var appPath = System.Web.Hosting.HostingEnvironment.ApplicationPhysicalPath;
                string errorPath = Path.Combine(appPath, "GetData", "Timezones");
                ExceptionLogging.SendErrorToText(ex, "GetCitationData", errorPath);
                data.ReturnCode = ((int)ReturnCodeEnum.Error).ToString();
                throw;
            }
            return data;
        }

        #region Chicago
        public static ChicagoTransactionTransform GetTransformDataChicago(string EnforcementKey, string filePath, RequestResponseEntity logData)
        {
            //  {Customerid} ’-‘ {metername} ’-‘ {spaceid} ’-‘ {LicensePlateNo} ’-‘{spacestatus} '-'{State}'-'{rateName}


            ChicagoTransactionTransform tranData = new ChicagoTransactionTransform();
            tranData.ReturnCode = ((int)ReturnCodeEnum.InvalidInputParameters).ToString();
            int? customerID = null;
            string MeterName = null;
            string parkingSpaceID;
            string PlateNumber = null;

            //Validate Input data
            string[] values = EnforcementKey.Split('-');
            /////Check existance of Customerid and metername as minimum/////////////////
            if (values[0] == null) //CustomerrId
            {
                return tranData;
            }
            else
            {
                customerID = values[0].ToNullableInt();
                if (customerID.HasValue == false)
                {
                    return tranData;
                }
            }

            ///////////////////
            TimeZoneInfo TZ = null;
            if (customerID.Value == 4102)
            {
                TZ = TimeZoneInfo.FindSystemTimeZoneById("Central Standard Time");
                // TZ = TimeZoneInfo.FindSystemTimeZoneById("Eastern Standard Time");
            }
            else
            {
                TZ = TimeZoneInfo.FindSystemTimeZoneById("Eastern Standard Time");
            }


            /////Use MeterName/////////////////////////////////////////////
            if (values[1] == null)
            {
                return tranData;
            }
            else
            {
                MeterName = values[1].ToString();
                if (string.IsNullOrEmpty(MeterName))
                {
                    //return tranData;
                }
            }
            /////End MeterName/////////////////////////////////////////////
            //Special case ///
            //If metername = 999999999 then simply return and no need to check
            if (MeterName == "999999999".ToString())
            {
                tranData.ReturnCode = ((int)ReturnCodeEnum.NoRecordExists).ToString();
                logData.Request.CustomerId = customerID.ToString();
                logData.Response = new ResponseEntity() { CustomerId = customerID.Value.ToString(), MeterId = MeterName, SpaceId = string.Empty, CityCurrentTime = DateTime.Now };
                return tranData;
            }
            //End Special case ///



            ////ParkingSpaceID/////////////////////////////////////////////////////////////////
            if (values[2] == null)
            {
                return tranData;
            }
            else
            {
                parkingSpaceID = values[2].ToString();
                if (string.IsNullOrEmpty(parkingSpaceID))
                {
                    //return tranData;
                }
            }
            ////End ParkingSpaceID/////////////////////////////////////////////////////////////////
            //Plate Number//
            if (values[3] == null)
            {
                return tranData;
            }
            else
            {
                PlateNumber = values[3].ToString();
                if (string.IsNullOrEmpty(PlateNumber))
                {
                    //return tranData;
                }
            }
            //End Plate Number//

            //State//
            string stateName = null;
            if (values[5] == null)
            {

            }
            else
            {
                stateName = values[5].ToString();
                if (string.IsNullOrEmpty(stateName))
                {

                }
            }
            //End State Name////

            //rateNumber//
            string rateNumber = null;
            if (values[6] == null)
            {

            }
            else
            {
                rateNumber = values[6].ToString();
                if (string.IsNullOrEmpty(rateNumber))
                {

                }
            }
            //End rateNumber Name////



            try
            {
                //Transaction data = TransactionDataValidatioin.GetDataChicago(customerID.Value, MeterName, parkingSpaceID, PlateNumber
                //    , receiptNumber, stateName, rateNumber
                //    , EnforcementKey, filePath, logData);

                Transaction data = TransactionDataValidatioin.GetDataChicago(customerID.Value, MeterName, parkingSpaceID, PlateNumber
                   , stateName, rateNumber
                   , EnforcementKey, filePath, logData);


                if (data.TimeZoneId.HasValue)
                    TZ = Utility.GetTimeZoneInfo(data.TimeZoneId.Value, filePath, customerID.Value.ToString());

                //For South Miami : PayStationId. Others it is MeterID
                tranData.MeterID = data.PayStationID.HasValue ? data.PayStationID.Value.ToString() : "0";

                tranData.MeterName = (data.MeterName == null) ? string.Empty : data.MeterName;
                tranData.SpaceNo = data.ParkingSpaceID.ToString();

                tranData.MeterStreet = (data.MeterStreet == null) ? string.Empty : data.MeterStreet;

                DateTime _MeterRTC = data.PresentMeterTime.HasValue ? data.PresentMeterTime.Value : DateTime.Now;
                tranData.MeterRTC = String.Format("{0:yyyyMMdd_THHmmssZ}", Utility.findGMTTime(_MeterRTC, TZ));

                DateTime _SensorEventTime = data.SensorEventTime.HasValue ? data.SensorEventTime.Value : DateTime.Now;
                tranData.MeterLastUpdateTime = String.Format("{0:yyyyMMdd_THHmmssZ}", Utility.findGMTTime(_SensorEventTime, TZ));

                tranData.SensorEventTime = String.Format("{0:yyyyMMdd_THHmmssZ}", Utility.findGMTTime(_SensorEventTime, TZ));

                DateTime _endTime = data.ExpiryTime.HasValue ? data.ExpiryTime.Value : DateTime.Now;
                tranData.ExpiredTime = String.Format("{0:yyyyMMdd_THHmmssZ}", Utility.findGMTTime(_endTime, TZ));

                tranData.MeterExpiredMinutes = data.ExpiredMinutes.HasValue ? data.ExpiredMinutes.Value.ToString() : "0";
                tranData.METERLOC_BLOCKNUMBER = (data.Block == null) ? string.Empty : data.Block;
                tranData.METERLOC_STREETDIRECTION = (data.Direction == null) ? string.Empty : data.Direction;
                tranData.METERLOC_STREETNAME = (data.MeterStreet == null) ? string.Empty : data.MeterStreet;
                tranData.METERLOC_STREETTYPE = (data.StreetType == null) ? string.Empty : data.StreetType;
                tranData.METERLOC_CROSSSTREET1 = (data.CrossStreet1 == null) ? string.Empty : data.CrossStreet1;
                tranData.METERLOC_CROSSSTREET2 = (data.CrossStreet2 == null) ? string.Empty : data.CrossStreet2;
                tranData.METERLOC_ENFORCEMENTHOURSDESC = (data.EnfHourDesc == null) ? string.Empty : data.EnfHourDesc;
                tranData.LicensePlate = (data.PlateNumber == null) ? string.Empty : data.PlateNumber;
                tranData.State = (data.LPLocation == null) ? string.Empty : data.LPLocation;
                tranData.Make = (data.VMake == null) ? string.Empty : data.VMake;
                tranData.Model = (data.VModel == null) ? string.Empty : data.VModel;
                tranData.IsOccupied = Utility.getStatus((data.SpaceStatus == null) ? string.Empty : data.SpaceStatus).ToString();
                tranData.EnforcementKey = (data.EnforcementKey == null) ? string.Empty : data.EnforcementKey;
                tranData.MeterStatus = (data.MeterStatus == null) ? null : data.MeterStatus;

                tranData.METERLOC_METERTYPE = (data.METERLOC_METERTYPE == null) ? null : data.METERLOC_METERTYPE;
                tranData.ZoneName = (data.ZoneName == null) ? null : data.ZoneName;
                tranData.Recieptid = (data.Recieptid == null) ? null : data.Recieptid;
                tranData.RateNumber = (data.RateNumber == null) ? null : data.RateNumber;

                tranData.ReturnCode = data.ReturnCode;


                logData.Response = new ResponseEntity() { CustomerId = customerID.Value.ToString(), MeterId = tranData.MeterName, SpaceId = tranData.SpaceNo, CityCurrentTime = data.PresentMeterTime.HasValue ? data.PresentMeterTime.Value : DateTime.Now };

            }
            catch (Exception ex)
            {
                var appPath = System.Web.Hosting.HostingEnvironment.ApplicationPhysicalPath;
                string errorPath = Path.Combine(appPath, "GetTransformData", "Timezones");
                ExceptionLogging.SendErrorToText(ex, "GetCitationData", errorPath);
                tranData.ReturnCode = ((int)ReturnCodeEnum.Error).ToString();

            }
            return tranData;
        }
        private static Transaction GetDataChicago(int customerID, string MeterName, string parkingSpaceID, string PlateNumber, string State, string RateNumber, string EnforcementKey, string filePath, RequestResponseEntity logData)
        {
            string ZoneID = null;
            string SpaceStatus = null;
            Transaction data = new Transaction();
            try
            {
                List<Transaction> transactionList = new List<Transaction>();

                logData.Request.CustomerId = customerID.ToString();
                logData.Request.SpaceId = parkingSpaceID.ToString();


                logData.Request.MeterId = MeterName;
                transactionList = DataAccess.GetCitationDataOthersWithPlateNameChicago(customerID, ZoneID, MeterName, parkingSpaceID, PlateNumber, SpaceStatus, State, RateNumber);





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
            }
            catch (Exception ex)
            {
                var appPath = System.Web.Hosting.HostingEnvironment.ApplicationPhysicalPath;
                string errorPath = Path.Combine(appPath, "GetData", "Timezones");
                ExceptionLogging.SendErrorToText(ex, "GetCitationData", errorPath);
                data.ReturnCode = ((int)ReturnCodeEnum.Error).ToString();
                throw;
            }
            return data;
        }
        #endregion

        #region CoralGables
        public static TransactionTransform GetCoralGablesTransformDataOld(string EnforcementKey, string filePath, RequestResponseEntity logData)
        {
            TransactionTransform tranData = new TransactionTransform();
            tranData.ReturnCode = ((int)ReturnCodeEnum.InvalidInputParameters).ToString();
            TimeZoneInfo TZ = null;
            TZ = TimeZoneInfo.FindSystemTimeZoneById("Eastern Standard Time");
            //int? customerID = null;
            //string MeterName = null;
            //string parkingSpaceID;
            //string PlateNumber = null;
            //string stateName = null;
            //////////////////////////////////////
            KeyParameter param = PrepareKeyParameters.PrepareCoralGables(EnforcementKey);
            if (param.ReturnCode == ((int)ReturnCodeEnum.InvalidInputParameters).ToString())
            {
                return tranData;
            }
            /////////////////////////////////////
            try
            {
                Transaction data = TransactionDataValidatioin.GetDataCoralGables(param.CustomerId.Value, param.MeterName, param.parkingSpaceID, param.PlateNumber, param.StateName, EnforcementKey, filePath, logData);


                if (data.TimeZoneId.HasValue)
                    TZ = Utility.GetTimeZoneInfo(data.TimeZoneId.Value, filePath, param.CustomerId.Value.ToString());

                //For South Miami : PayStationId. Others it is MeterID
                tranData.MeterName = string.Empty; // (data.MeterName == null) ? string.Empty : data.MeterName;
                tranData.SpaceNo = string.Empty;  //data.ParkingSpaceID.ToString();

                tranData.MeterStreet = (data.MeterStreet == null) ? string.Empty : data.MeterStreet;

                DateTime _MeterRTC = data.PresentMeterTime.HasValue ? data.PresentMeterTime.Value : DateTime.Now;
                tranData.MeterRTC = String.Format("{0:yyyyMMdd_THHmmssZ}", Utility.findGMTTime(_MeterRTC, TZ));

                DateTime _SensorEventTime = data.SensorEventTime.HasValue ? data.SensorEventTime.Value : DateTime.Now;
                tranData.MeterLastUpdateTime = String.Format("{0:yyyyMMdd_THHmmssZ}", Utility.findGMTTime(_SensorEventTime, TZ));

                tranData.SensorEventTime = String.Format("{0:yyyyMMdd_THHmmssZ}", Utility.findGMTTime(_SensorEventTime, TZ));

                DateTime _endTime = data.ExpiryTime.HasValue ? data.ExpiryTime.Value : DateTime.Now;
                tranData.ExpiredTime = String.Format("{0:yyyyMMdd_THHmmssZ}", Utility.findGMTTime(_endTime, TZ));

                tranData.MeterExpiredMinutes = data.ExpiredMinutes.HasValue ? data.ExpiredMinutes.Value.ToString() : "0";
                tranData.METERLOC_BLOCKNUMBER = (data.Block == null) ? string.Empty : data.Block;
                tranData.METERLOC_STREETDIRECTION = (data.Direction == null) ? string.Empty : data.Direction;
                tranData.METERLOC_STREETNAME = (data.MeterStreet == null) ? string.Empty : data.MeterStreet;
                tranData.METERLOC_STREETTYPE = (data.StreetType == null) ? string.Empty : data.StreetType;
                tranData.METERLOC_CROSSSTREET1 = (data.CrossStreet1 == null) ? string.Empty : data.CrossStreet1;
                tranData.METERLOC_CROSSSTREET2 = (data.CrossStreet2 == null) ? string.Empty : data.CrossStreet2;
                tranData.METERLOC_ENFORCEMENTHOURSDESC = (data.EnfHourDesc == null) ? string.Empty : data.EnfHourDesc;
                tranData.LicensePlate = (data.PlateNumber == null) ? string.Empty : data.PlateNumber;
                tranData.State = (data.LPLocation == null) ? string.Empty : data.LPLocation;
                tranData.Make = (data.VMake == null) ? string.Empty : data.VMake;
                tranData.Model = (data.VModel == null) ? string.Empty : data.VModel;
                tranData.IsOccupied = Utility.getStatus((data.SpaceStatus == null) ? string.Empty : data.SpaceStatus).ToString();
                tranData.EnforcementKey = (data.EnforcementKey == null) ? string.Empty : data.EnforcementKey;
                tranData.MeterStatus = (data.MeterStatus == null) ? null : data.MeterStatus;

                tranData.METERLOC_METERTYPE = (data.METERLOC_METERTYPE == null) ? null : data.METERLOC_METERTYPE;
                tranData.ZoneName = (data.ZoneName == null) ? null : data.ZoneName;
                tranData.MeterID = String.Empty;
                tranData.ReturnCode = data.ReturnCode;

                logData.Response = new ResponseEntity() { CustomerId = param.CustomerId.Value.ToString(), MeterId = tranData.MeterName, SpaceId = tranData.SpaceNo, CityCurrentTime = data.PresentMeterTime.HasValue ? data.PresentMeterTime.Value : DateTime.Now };
            }
            catch (Exception ex)
            {
                var appPath = System.Web.Hosting.HostingEnvironment.ApplicationPhysicalPath;
                string errorPath = Path.Combine(appPath, "GetTransformData", "Timezones");
                ExceptionLogging.SendErrorToText(ex, "GetCitationData", errorPath);
                tranData.ReturnCode = ((int)ReturnCodeEnum.Error).ToString();

            }
            return tranData;
        }
        public static TransactionTransform GetCoralGablesTransformData(string EnforcementKey, string filePath, RequestResponseEntity logData, CustomerTime customerCurrentTime)
        {
            TransactionTransform tranData = new TransactionTransform();
            tranData.ReturnCode = ((int)ReturnCodeEnum.InvalidInputParameters).ToString();
            TimeZoneInfo TZ = null;
            TZ = TimeZoneInfo.FindSystemTimeZoneById("Eastern Standard Time");
            //int? customerID = null;
            //string MeterName = null;
            //string parkingSpaceID;
            //string PlateNumber = null;
            //string stateName = null;
            //////////////////////////////////////
            KeyParameter param = PrepareKeyParameters.PrepareCoralGables(EnforcementKey);
            if (param.ReturnCode == ((int)ReturnCodeEnum.InvalidInputParameters).ToString())
            {
                return tranData;
            }
            /////////////////////////////////////
            #region "DB Check"
            try
            {
                Transaction data = TransactionDataValidatioin.GetDataCoralGables(param.CustomerId.Value, param.MeterName, param.parkingSpaceID, param.PlateNumber, param.StateName, EnforcementKey, filePath, logData);


                if (data.TimeZoneId.HasValue)
                    TZ = Utility.GetTimeZoneInfo(data.TimeZoneId.Value, filePath, param.CustomerId.Value.ToString());

                //For South Miami : PayStationId. Others it is MeterID
                tranData.MeterName = string.Empty; // (data.MeterName == null) ? string.Empty : data.MeterName;
                tranData.SpaceNo = string.Empty;  //data.ParkingSpaceID.ToString();

                tranData.MeterStreet = (data.MeterStreet == null) ? string.Empty : data.MeterStreet;

                DateTime _MeterRTC = data.PresentMeterTime.HasValue ? data.PresentMeterTime.Value : DateTime.Now;
                tranData.MeterRTC = String.Format("{0:yyyyMMdd_THHmmssZ}", Utility.findGMTTime(_MeterRTC, TZ));

                DateTime _SensorEventTime = data.SensorEventTime.HasValue ? data.SensorEventTime.Value : DateTime.Now;
                tranData.MeterLastUpdateTime = String.Format("{0:yyyyMMdd_THHmmssZ}", Utility.findGMTTime(_SensorEventTime, TZ));

                tranData.SensorEventTime = String.Format("{0:yyyyMMdd_THHmmssZ}", Utility.findGMTTime(_SensorEventTime, TZ));

                DateTime _endTime = data.ExpiryTime.HasValue ? data.ExpiryTime.Value : DateTime.Now;
                tranData.ExpiredTime = String.Format("{0:yyyyMMdd_THHmmssZ}", Utility.findGMTTime(_endTime, TZ));

                tranData.MeterExpiredMinutes = data.ExpiredMinutes.HasValue ? data.ExpiredMinutes.Value.ToString() : "0";
                tranData.METERLOC_BLOCKNUMBER = (data.Block == null) ? string.Empty : data.Block;
                tranData.METERLOC_STREETDIRECTION = (data.Direction == null) ? string.Empty : data.Direction;
                tranData.METERLOC_STREETNAME = (data.MeterStreet == null) ? string.Empty : data.MeterStreet;
                tranData.METERLOC_STREETTYPE = (data.StreetType == null) ? string.Empty : data.StreetType;
                tranData.METERLOC_CROSSSTREET1 = (data.CrossStreet1 == null) ? string.Empty : data.CrossStreet1;
                tranData.METERLOC_CROSSSTREET2 = (data.CrossStreet2 == null) ? string.Empty : data.CrossStreet2;
                tranData.METERLOC_ENFORCEMENTHOURSDESC = (data.EnfHourDesc == null) ? string.Empty : data.EnfHourDesc;
                tranData.LicensePlate = (data.PlateNumber == null) ? string.Empty : data.PlateNumber;
                tranData.State = (data.LPLocation == null) ? string.Empty : data.LPLocation;
                tranData.Make = (data.VMake == null) ? string.Empty : data.VMake;
                tranData.Model = (data.VModel == null) ? string.Empty : data.VModel;
                tranData.IsOccupied = Utility.getStatus((data.SpaceStatus == null) ? string.Empty : data.SpaceStatus).ToString();
                tranData.EnforcementKey = (data.EnforcementKey == null) ? string.Empty : data.EnforcementKey;
                tranData.MeterStatus = (data.MeterStatus == null) ? null : data.MeterStatus;

                tranData.METERLOC_METERTYPE = (data.METERLOC_METERTYPE == null) ? null : data.METERLOC_METERTYPE;
                tranData.ZoneName = (data.ZoneName == null) ? null : data.ZoneName;
                tranData.MeterID = String.Empty;
                tranData.ReturnCode = data.ReturnCode;

                logData.Response = new ResponseEntity() { CustomerId = param.CustomerId.Value.ToString(), MeterId = tranData.MeterName, SpaceId = tranData.SpaceNo, CityCurrentTime = data.PresentMeterTime.HasValue ? data.PresentMeterTime.Value : DateTime.Now };
            }
            catch (Exception ex)
            {
                //var appPath = System.Web.Hosting.HostingEnvironment.ApplicationPhysicalPath;
                //string errorPath = Path.Combine(appPath, "GetTransformData", "Timezones");
                //ExceptionLogging.SendErrorToText(ex, "GetCitationData", errorPath);
                //tranData.ReturnCode = ((int)ReturnCodeEnum.Error).ToString();

                tranData.ReturnCode = ((int)ReturnCodeEnum.Error).ToString();
            }
            #endregion
            #region "PayByPhone Service Call"
            /////////////////////////////////////////////////
            TransactionTransform tranDataOfService = new TransactionTransform();
            try
            {
                //////////////////////////////////////////
                logData.Request.CustomerId = param.CustomerId.Value.ToString();
                logData.Request.SpaceId = param.parkingSpaceID;
                logData.Request.MeterId = param.MeterName;
                //////////////////////////////////////////////////
                DateTime? currentCustTime = customerCurrentTime.CurrentTime;
                CustomerElement customer = CustomerRetriever.GetCustomer(param.CustomerId.Value);
                PayByPhoneGenericFactory factory = new PayByPhoneGenericFactory(customer, param.PlateNumber);
                tranDataOfService = factory.GetRefreshPlateData();
                tranDataOfService.EnforcementKey = EnforcementKey;

                ///////////////////////////////////////////////////
                logData.Response = new ResponseEntity() { CustomerId = param.CustomerId.Value.ToString(), MeterId = tranData.MeterName, SpaceId = tranData.SpaceNo, CityCurrentTime = currentCustTime.HasValue ? currentCustTime.Value : DateTime.Now };

            }
            catch (Exception ex)
            {
                logData.Response = new ResponseEntity() { CustomerId = param.CustomerId.Value.ToString(), MeterId = tranData.MeterName, SpaceId = tranData.SpaceNo, CityCurrentTime = customerCurrentTime.CurrentTime.HasValue ? customerCurrentTime.CurrentTime.Value : DateTime.Now };
                //throw ex;
                tranDataOfService.ReturnCode = ((int)ReturnCodeEnum.Error).ToString();
            }
            finally
            {

            }
            #endregion

            //Reconcile data
            if ((tranData.ReturnCode == ((int)ReturnCodeEnum.NoRecordExists).ToString()) || (tranData.ReturnCode == ((int)ReturnCodeEnum.Error).ToString()))
            {
                tranData = tranDataOfService;
            }
            else if (tranData.ReturnCode == ((int)ReturnCodeEnum.Success).ToString())
            {
                if (tranDataOfService.ReturnCode == ((int)ReturnCodeEnum.Success).ToString())
                {
                    ////Select one record of highest expiry time
                    int? dbExpiredMinutes = tranData.MeterExpiredMinutes.ToNullableInt();
                    int? parkeonExpiredMinutes = tranDataOfService.MeterExpiredMinutes.ToNullableInt();
                    tranData = Nullable.Compare(dbExpiredMinutes, parkeonExpiredMinutes) > 0 ? tranData : tranDataOfService;
                }

            }

            else if (tranData.ReturnCode == ((int)ReturnCodeEnum.MoreRecordsExists).ToString())
            {
                // DB never give more record. hence ignore
            }

            return tranData;
        }
        public static TransactionTransform GetCoralGablesTransformDataV2(string EnforcementKey, string filePath, RequestResponseEntity logData, CustomerTime customerCurrentTime)
        {
            TransactionTransform tranData = new TransactionTransform();
            tranData.ReturnCode = ((int)ReturnCodeEnum.InvalidInputParameters).ToString();
            TimeZoneInfo TZ = null;
            TZ = TimeZoneInfo.FindSystemTimeZoneById("Eastern Standard Time");
            //int? customerID = null;
            //string MeterName = null;
            //string parkingSpaceID;
            //string PlateNumber = null;
            //string stateName = null;
            //////////////////////////////////////
            KeyParameter param = PrepareKeyParameters.PrepareCoralGables(EnforcementKey);
            if (param.ReturnCode == ((int)ReturnCodeEnum.InvalidInputParameters).ToString())
            {
                return tranData;
            }
            /////////////////////////////////////
            #region "DB Check"
            try
            {
                Transaction data = TransactionDataValidatioin.GetDataCoralGables(param.CustomerId.Value, param.MeterName, param.parkingSpaceID, param.PlateNumber, param.StateName, EnforcementKey, filePath, logData);


                if (data.TimeZoneId.HasValue)
                    TZ = Utility.GetTimeZoneInfo(data.TimeZoneId.Value, filePath, param.CustomerId.Value.ToString());

                //For South Miami : PayStationId. Others it is MeterID
                tranData.MeterName = string.Empty; // (data.MeterName == null) ? string.Empty : data.MeterName;
                tranData.SpaceNo = string.Empty;  //data.ParkingSpaceID.ToString();

                tranData.MeterStreet = (data.MeterStreet == null) ? string.Empty : data.MeterStreet;

                DateTime _MeterRTC = data.PresentMeterTime.HasValue ? data.PresentMeterTime.Value : DateTime.Now;
                tranData.MeterRTC = String.Format("{0:yyyyMMdd_THHmmssZ}", Utility.findGMTTime(_MeterRTC, TZ));

                DateTime _SensorEventTime = data.SensorEventTime.HasValue ? data.SensorEventTime.Value : DateTime.Now;
                tranData.MeterLastUpdateTime = String.Format("{0:yyyyMMdd_THHmmssZ}", Utility.findGMTTime(_SensorEventTime, TZ));

                tranData.SensorEventTime = String.Format("{0:yyyyMMdd_THHmmssZ}", Utility.findGMTTime(_SensorEventTime, TZ));

                DateTime _endTime = data.ExpiryTime.HasValue ? data.ExpiryTime.Value : DateTime.Now;
                tranData.ExpiredTime = String.Format("{0:yyyyMMdd_THHmmssZ}", Utility.findGMTTime(_endTime, TZ));

                tranData.MeterExpiredMinutes = data.ExpiredMinutes.HasValue ? data.ExpiredMinutes.Value.ToString() : "0";
                tranData.METERLOC_BLOCKNUMBER = (data.Block == null) ? string.Empty : data.Block;
                tranData.METERLOC_STREETDIRECTION = (data.Direction == null) ? string.Empty : data.Direction;
                tranData.METERLOC_STREETNAME = (data.MeterStreet == null) ? string.Empty : data.MeterStreet;
                tranData.METERLOC_STREETTYPE = (data.StreetType == null) ? string.Empty : data.StreetType;
                tranData.METERLOC_CROSSSTREET1 = (data.CrossStreet1 == null) ? string.Empty : data.CrossStreet1;
                tranData.METERLOC_CROSSSTREET2 = (data.CrossStreet2 == null) ? string.Empty : data.CrossStreet2;
                tranData.METERLOC_ENFORCEMENTHOURSDESC = (data.EnfHourDesc == null) ? string.Empty : data.EnfHourDesc;
                tranData.LicensePlate = (data.PlateNumber == null) ? string.Empty : data.PlateNumber;
                tranData.State = (data.LPLocation == null) ? string.Empty : data.LPLocation;
                tranData.Make = (data.VMake == null) ? string.Empty : data.VMake;
                tranData.Model = (data.VModel == null) ? string.Empty : data.VModel;
                tranData.IsOccupied = Utility.getStatus((data.SpaceStatus == null) ? string.Empty : data.SpaceStatus).ToString();
                tranData.EnforcementKey = (data.EnforcementKey == null) ? string.Empty : data.EnforcementKey;
                tranData.MeterStatus = (data.MeterStatus == null) ? null : data.MeterStatus;

                tranData.METERLOC_METERTYPE = (data.METERLOC_METERTYPE == null) ? null : data.METERLOC_METERTYPE;
                tranData.ZoneName = (data.ZoneName == null) ? null : data.ZoneName;
                tranData.MeterID = String.Empty;
                tranData.ReturnCode = data.ReturnCode;

                logData.Response = new ResponseEntity() { CustomerId = param.CustomerId.Value.ToString(), MeterId = tranData.MeterName, SpaceId = tranData.SpaceNo, CityCurrentTime = data.PresentMeterTime.HasValue ? data.PresentMeterTime.Value : DateTime.Now };
            }
            catch (Exception ex)
            {
                //var appPath = System.Web.Hosting.HostingEnvironment.ApplicationPhysicalPath;
                //string errorPath = Path.Combine(appPath, "GetTransformData", "Timezones");
                //ExceptionLogging.SendErrorToText(ex, "GetCitationData", errorPath);
                //tranData.ReturnCode = ((int)ReturnCodeEnum.Error).ToString();

                tranData.ReturnCode = ((int)ReturnCodeEnum.Error).ToString();
            }
            #endregion
            #region "PayByPhone Service Call"
            /////////////////////////////////////////////////
            TransactionTransform tranDataOfService = new TransactionTransform();
            try
            {
                //////////////////////////////////////////
                logData.Request.CustomerId = param.CustomerId.Value.ToString();
                logData.Request.SpaceId = param.parkingSpaceID;
                logData.Request.MeterId = param.MeterName;
                //////////////////////////////////////////////////
                DateTime? currentCustTime = customerCurrentTime.CurrentTime;
                CustomerElement customer = CustomerRetriever.GetCustomer(param.CustomerId.Value);
                PayByPhoneGenericFactory factory = new PayByPhoneGenericFactory(customer, param.PlateNumber);
                var paybyPhoneResLog = new VendorResponseEntity();
                logData.VendorData.Add(VendorNames.PayByPhone, paybyPhoneResLog);
                tranDataOfService = factory.GetRefreshPlateData(ref paybyPhoneResLog);
                tranDataOfService.EnforcementKey = EnforcementKey;

                ///////////////////////////////////////////////////
                logData.Response = new ResponseEntity() { CustomerId = param.CustomerId.Value.ToString(), MeterId = tranData.MeterName, SpaceId = tranData.SpaceNo, CityCurrentTime = currentCustTime.HasValue ? currentCustTime.Value : DateTime.Now };

            }
            catch (Exception ex)
            {
                logData.Response = new ResponseEntity() { CustomerId = param.CustomerId.Value.ToString(), MeterId = tranData.MeterName, SpaceId = tranData.SpaceNo, CityCurrentTime = customerCurrentTime.CurrentTime.HasValue ? customerCurrentTime.CurrentTime.Value : DateTime.Now };
                //throw ex;
                tranDataOfService.ReturnCode = ((int)ReturnCodeEnum.Error).ToString();
            }
            finally
            {

            }
            #endregion

            //Reconcile data
            if ((tranData.ReturnCode == ((int)ReturnCodeEnum.NoRecordExists).ToString()) || (tranData.ReturnCode == ((int)ReturnCodeEnum.Error).ToString()))
            {
                tranData = tranDataOfService;
            }
            else if (tranData.ReturnCode == ((int)ReturnCodeEnum.Success).ToString())
            {
                if (tranDataOfService.ReturnCode == ((int)ReturnCodeEnum.Success).ToString())
                {
                    ////Select one record of highest expiry time
                    int? dbExpiredMinutes = tranData.MeterExpiredMinutes.ToNullableInt();
                    int? parkeonExpiredMinutes = tranDataOfService.MeterExpiredMinutes.ToNullableInt();
                    tranData = Nullable.Compare(dbExpiredMinutes, parkeonExpiredMinutes) > 0 ? tranData : tranDataOfService;
                }

            }

            else if (tranData.ReturnCode == ((int)ReturnCodeEnum.MoreRecordsExists).ToString())
            {
                // DB never give more record. hence ignore
            }

            return tranData;
        }
        private static Transaction GetDataCoralGables(int customerID, string MeterName, string parkingSpaceID, string PlateNumber, string statename, string EnforcementKey, string filePath, RequestResponseEntity logData)
        {
            string ZoneID = null;
            string SpaceStatus = null;
            Transaction data = new Transaction();
            try
            {
                List<Transaction> transactionList = new List<Transaction>();

                logData.Request.CustomerId = customerID.ToString();
                logData.Request.SpaceId = parkingSpaceID;
                logData.Request.MeterId = MeterName;

                transactionList = DataAccess.GetCoralGablesCitationDataOthersWithPlateName(customerID, null, null, null, PlateNumber, statename, null);

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
            }
            catch (Exception ex)
            {
                var appPath = System.Web.Hosting.HostingEnvironment.ApplicationPhysicalPath;
                string errorPath = Path.Combine(appPath, "GetData", "Timezones");
                ExceptionLogging.SendErrorToText(ex, "GetCitationData", errorPath);
                data.ReturnCode = ((int)ReturnCodeEnum.Error).ToString();
                throw;
            }
            return data;
        }
        #endregion

        #region Atlanta
        public static TransactionTransform GetAtlantaTransformDataV5(string EnforcementKey, string filePath, RequestResponseEntity logData, CustomerTime customerCurrentTime)
        {
            TransactionTransform tranData = new TransactionTransform();
            tranData.ReturnCode = ((int)ReturnCodeEnum.InvalidInputParameters).ToString();
            TimeZoneInfo TZ = null;
            TZ = TimeZoneInfo.FindSystemTimeZoneById("Eastern Standard Time");
            //int? customerID = null;
            //string MeterName = null;
            //string parkingSpaceID;
            //string PlateNumber = null;
            //string stateName = null;
            //////////////////////////////////////
            KeyParameter param = PrepareKeyParameters.PrepareAtlanta(EnforcementKey);
            if (param.ReturnCode == ((int)ReturnCodeEnum.InvalidInputParameters).ToString())
            {
                return tranData;
            }

            #region "DB Check"
            /////////////////////////////////////
            try
            {
                Transaction data = TransactionDataValidatioin.GetDataAtlanta(param.CustomerId.Value, param.MeterName, param.parkingSpaceID, param.PlateNumber, param.StateName, EnforcementKey, filePath, logData);


                if (data.TimeZoneId.HasValue)
                    TZ = Utility.GetTimeZoneInfo(data.TimeZoneId.Value, filePath, param.CustomerId.Value.ToString());

                //For South Miami : PayStationId. Others it is MeterID
                tranData.MeterName = string.Empty; // (data.MeterName == null) ? string.Empty : data.MeterName;
                tranData.SpaceNo = string.Empty;  //data.ParkingSpaceID.ToString();

                tranData.MeterStreet = (data.MeterStreet == null) ? string.Empty : data.MeterStreet;

                DateTime _MeterRTC = data.PresentMeterTime.HasValue ? data.PresentMeterTime.Value : DateTime.Now;
                tranData.MeterRTC = String.Format("{0:yyyyMMdd_THHmmssZ}", Utility.findGMTTime(_MeterRTC, TZ));

                DateTime _SensorEventTime = data.SensorEventTime.HasValue ? data.SensorEventTime.Value : DateTime.Now;
                tranData.MeterLastUpdateTime = String.Format("{0:yyyyMMdd_THHmmssZ}", Utility.findGMTTime(_SensorEventTime, TZ));

                tranData.SensorEventTime = String.Format("{0:yyyyMMdd_THHmmssZ}", Utility.findGMTTime(_SensorEventTime, TZ));

                DateTime _endTime = data.ExpiryTime.HasValue ? data.ExpiryTime.Value : DateTime.Now;
                tranData.ExpiredTime = String.Format("{0:yyyyMMdd_THHmmssZ}", Utility.findGMTTime(_endTime, TZ));

                tranData.MeterExpiredMinutes = data.ExpiredMinutes.HasValue ? data.ExpiredMinutes.Value.ToString() : "0";
                tranData.METERLOC_BLOCKNUMBER = (data.Block == null) ? string.Empty : data.Block;
                tranData.METERLOC_STREETDIRECTION = (data.Direction == null) ? string.Empty : data.Direction;
                tranData.METERLOC_STREETNAME = (data.MeterStreet == null) ? string.Empty : data.MeterStreet;
                tranData.METERLOC_STREETTYPE = (data.StreetType == null) ? string.Empty : data.StreetType;
                tranData.METERLOC_CROSSSTREET1 = (data.CrossStreet1 == null) ? string.Empty : data.CrossStreet1;
                tranData.METERLOC_CROSSSTREET2 = (data.CrossStreet2 == null) ? string.Empty : data.CrossStreet2;
                tranData.METERLOC_ENFORCEMENTHOURSDESC = (data.EnfHourDesc == null) ? string.Empty : data.EnfHourDesc;
                tranData.LicensePlate = (data.PlateNumber == null) ? string.Empty : data.PlateNumber;
                tranData.State = (data.LPLocation == null) ? string.Empty : data.LPLocation;
                tranData.Make = (data.VMake == null) ? string.Empty : data.VMake;
                tranData.Model = (data.VModel == null) ? string.Empty : data.VModel;
                tranData.IsOccupied = Utility.getStatus((data.SpaceStatus == null) ? string.Empty : data.SpaceStatus).ToString();
                tranData.EnforcementKey = (data.EnforcementKey == null) ? string.Empty : data.EnforcementKey;
                tranData.MeterStatus = (data.MeterStatus == null) ? null : data.MeterStatus;

                tranData.METERLOC_METERTYPE = (data.METERLOC_METERTYPE == null) ? null : data.METERLOC_METERTYPE;
                tranData.ZoneName = (data.ZoneName == null) ? null : data.ZoneName;
                tranData.MeterID = String.Empty;
                tranData.ReturnCode = data.ReturnCode;

                logData.Response = new ResponseEntity() { CustomerId = param.CustomerId.Value.ToString(), MeterId = tranData.MeterName, SpaceId = tranData.SpaceNo, CityCurrentTime = data.PresentMeterTime.HasValue ? data.PresentMeterTime.Value : DateTime.Now };
            }
            catch (Exception ex)
            {
                ////Intentionally Error Condition Commented
                ////var appPath = System.Web.Hosting.HostingEnvironment.ApplicationPhysicalPath;
                ////string errorPath = Path.Combine(appPath, "GetTransformData", "Timezones");
                ////ExceptionLogging.SendErrorToText(ex, "GetCitationData", errorPath);
                ////tranData.ReturnCode = ((int)ReturnCodeEnum.Error).ToString();

                tranData.ReturnCode = ((int)ReturnCodeEnum.Error).ToString();
            }

            #endregion
            #region "Parkeon Service Call"
            /////////////////////////////////////////////////
            TransactionTransform tranDataOfService = new TransactionTransform();
            ServiceParameters serviceParam = null;
            try
            {
                //////////////////////////////////////////
                logData.Request.CustomerId = param.CustomerId.Value.ToString();
                logData.Request.SpaceId = param.parkingSpaceID;
                logData.Request.MeterId = param.MeterName;
                //////////////////////////////////////////////////
                DateTime? currentCustTime = customerCurrentTime.CurrentTime;
                tranData.EnforcementKey = EnforcementKey;
                //serviceParam = new ServiceParameters() { customerId = 4120, userName = "civicsmart_115_peo", userPassword = @"4x+W6fWc/7,QT?VQ", vendorId = 3 };
                //ParkeonServiceAccess parkeonService = new ParkeonServiceAccess(serviceParam, param);
                ParkeonCustomerElement customer = ParkeonCustomerRetriever.GetCustomer(param.CustomerId.Value);
                serviceParam = new ServiceParameters() { customerId = (Convert.ToInt64(customer.CustomerId)), userName = customer.UserName, userPassword = customer.UserPassword, vendorId = (Convert.ToInt64(customer.VendorId)) };
                ParkeonServiceAccess parkeonService = new ParkeonServiceAccess(serviceParam, param, customer);
                ///////////////////
                var parkeonResLog = new VendorResponseEntity();
                logData.VendorData.Add(VendorNames.Parkeon, parkeonResLog);
                parkeonService.ProcessRequest(ref tranDataOfService, TypeConditions.check_plate, TZ, ref currentCustTime, ref parkeonResLog);
                serviceParam = null;
                ///////////////////////////////////////////////////
                logData.Response = new ResponseEntity() { CustomerId = param.CustomerId.Value.ToString(), MeterId = tranData.MeterName, SpaceId = tranData.SpaceNo, CityCurrentTime = currentCustTime.HasValue ? currentCustTime.Value : DateTime.Now };

            }
            catch (Exception ex)
            {
                logData.Response = new ResponseEntity() { CustomerId = param.CustomerId.Value.ToString(), MeterId = tranData.MeterName, SpaceId = tranData.SpaceNo, CityCurrentTime = customerCurrentTime.CurrentTime.HasValue ? customerCurrentTime.CurrentTime.Value : DateTime.Now };
                //throw ex;
                tranDataOfService.ReturnCode = ((int)ReturnCodeEnum.Error).ToString();
            }
            finally
            {
                serviceParam = null;
            }
            #endregion
            #region "Parkmobile Service Call"
            /////////////////////////////////////////////////
            TransactionTransform tranDataOfServiceParkMobile = new TransactionTransform();
            try
            {
                //////////////////////////////////////////
                logData.Request.CustomerId = param.CustomerId.Value.ToString();
                logData.Request.SpaceId = param.parkingSpaceID;
                logData.Request.MeterId = param.MeterName;
                //////////////////////////////////////////////////
                DateTime? currentCustTime = customerCurrentTime.CurrentTime;
                PMCustomerElement customer = ParkmobileCustomerRetriever.GetCustomer(param.CustomerId.Value);
                ParkMobileGenericFactory factory = new ParkMobileGenericFactory(customer, param.PlateNumber);
                var parkMobileResLog = new VendorResponseEntity();
                logData.VendorData.Add(VendorNames.ParkMobile, parkMobileResLog);
                tranDataOfServiceParkMobile = factory.GetRefreshSpaceData(ParkingMobileDataType.Plate, ref parkMobileResLog);
                tranDataOfServiceParkMobile.EnforcementKey = EnforcementKey;

                ///////////////////////////////////////////////////
                logData.Response = new ResponseEntity() { CustomerId = param.CustomerId.Value.ToString(), MeterId = tranData.MeterName, SpaceId = tranData.SpaceNo, CityCurrentTime = currentCustTime.HasValue ? currentCustTime.Value : DateTime.Now };

            }
            catch (Exception ex)
            {
                logData.Response = new ResponseEntity() { CustomerId = param.CustomerId.Value.ToString(), MeterId = tranData.MeterName, SpaceId = tranData.SpaceNo, CityCurrentTime = customerCurrentTime.CurrentTime.HasValue ? customerCurrentTime.CurrentTime.Value : DateTime.Now };
                //throw ex;
                tranDataOfServiceParkMobile.ReturnCode = ((int)ReturnCodeEnum.Error).ToString();
            }
            finally
            {

            }
            #endregion
            #region "Passport Monitoring"
            /////////////////////////////////////////////////
            TransactionTransform tranDataOfPassport = new TransactionTransform();
            try
            {
                //////////////////////////////////////////
                logData.Request.CustomerId = param.CustomerId.Value.ToString();
                logData.Request.SpaceId = param.parkingSpaceID;
                logData.Request.MeterId = param.MeterName;
                //////////////////////////////////////////////////
                DateTime? currentCustTime = customerCurrentTime.CurrentTime;
                PassportMonitoringCustomerElement customer = PassportMonitoringCustomerRetriever.GetCustomer(param.CustomerId.Value);
                LookUpLPNFactory factory = new LookUpLPNFactory(customer, param.PlateNumber);
                var passportMonResLog = new VendorResponseEntity();
                logData.VendorData.Add(VendorNames.PassportMonitoring, passportMonResLog);
                tranDataOfPassport = factory.GetRefreshPlateData(ref passportMonResLog);
                tranDataOfPassport.EnforcementKey = EnforcementKey;

                ///////////////////////////////////////////////////
                logData.Response = new ResponseEntity() { CustomerId = param.CustomerId.Value.ToString(), MeterId = tranData.MeterName, SpaceId = tranData.SpaceNo, CityCurrentTime = currentCustTime.HasValue ? currentCustTime.Value : DateTime.Now };
            }
            catch (Exception ex)
            {
                logData.Response = new ResponseEntity() { CustomerId = param.CustomerId.Value.ToString(), MeterId = tranData.MeterName, SpaceId = tranData.SpaceNo, CityCurrentTime = customerCurrentTime.CurrentTime.HasValue ? customerCurrentTime.CurrentTime.Value : DateTime.Now };
                //throw ex;
                tranData.ReturnCode = ((int)ReturnCodeEnum.Error).ToString();
            }
            finally
            {

            }
            #endregion
            //////////////////  //Reconcile data

            List<TransactionTransform> trans = new List<TransactionTransform>();
            trans.Add(tranDataOfServiceParkMobile); // Parkmobile
            trans.Add(tranData); // Database
            trans.Add(tranDataOfService); //Parkeon
            trans.Add(tranDataOfPassport); //Passport


            var rightValue = trans.Where(t => t.ReturnCode == ((int)ReturnCodeEnum.Success).ToString()).OrderByDescending(t => t.MeterExpiredMinutes.ToNullableInt()).FirstOrDefault<TransactionTransform>();
            if (rightValue == default(TransactionTransform))
            {
                //Do nothing.just return database  object.
            }
            else
            {
                tranData = rightValue;
            }

            return tranData;
        }
        public static TransactionTransform GetAtlantaTransformDataV4(string EnforcementKey, string filePath, RequestResponseEntity logData, CustomerTime customerCurrentTime)
        {
            TransactionTransform tranData = new TransactionTransform();
            tranData.ReturnCode = ((int)ReturnCodeEnum.InvalidInputParameters).ToString();
            TimeZoneInfo TZ = null;
            TZ = TimeZoneInfo.FindSystemTimeZoneById("Eastern Standard Time");
            //int? customerID = null;
            //string MeterName = null;
            //string parkingSpaceID;
            //string PlateNumber = null;
            //string stateName = null;
            //////////////////////////////////////
            KeyParameter param = PrepareKeyParameters.PrepareAtlanta(EnforcementKey);
            if (param.ReturnCode == ((int)ReturnCodeEnum.InvalidInputParameters).ToString())
            {
                return tranData;
            }

            #region "DB Check"
            /////////////////////////////////////
            try
            {
                Transaction data = TransactionDataValidatioin.GetDataAtlanta(param.CustomerId.Value, param.MeterName, param.parkingSpaceID, param.PlateNumber, param.StateName, EnforcementKey, filePath, logData);


                if (data.TimeZoneId.HasValue)
                    TZ = Utility.GetTimeZoneInfo(data.TimeZoneId.Value, filePath, param.CustomerId.Value.ToString());

                //For South Miami : PayStationId. Others it is MeterID
                tranData.MeterName = string.Empty; // (data.MeterName == null) ? string.Empty : data.MeterName;
                tranData.SpaceNo = string.Empty;  //data.ParkingSpaceID.ToString();

                tranData.MeterStreet = (data.MeterStreet == null) ? string.Empty : data.MeterStreet;

                DateTime _MeterRTC = data.PresentMeterTime.HasValue ? data.PresentMeterTime.Value : DateTime.Now;
                tranData.MeterRTC = String.Format("{0:yyyyMMdd_THHmmssZ}", Utility.findGMTTime(_MeterRTC, TZ));

                DateTime _SensorEventTime = data.SensorEventTime.HasValue ? data.SensorEventTime.Value : DateTime.Now;
                tranData.MeterLastUpdateTime = String.Format("{0:yyyyMMdd_THHmmssZ}", Utility.findGMTTime(_SensorEventTime, TZ));

                tranData.SensorEventTime = String.Format("{0:yyyyMMdd_THHmmssZ}", Utility.findGMTTime(_SensorEventTime, TZ));

                DateTime _endTime = data.ExpiryTime.HasValue ? data.ExpiryTime.Value : DateTime.Now;
                tranData.ExpiredTime = String.Format("{0:yyyyMMdd_THHmmssZ}", Utility.findGMTTime(_endTime, TZ));

                tranData.MeterExpiredMinutes = data.ExpiredMinutes.HasValue ? data.ExpiredMinutes.Value.ToString() : "0";
                tranData.METERLOC_BLOCKNUMBER = (data.Block == null) ? string.Empty : data.Block;
                tranData.METERLOC_STREETDIRECTION = (data.Direction == null) ? string.Empty : data.Direction;
                tranData.METERLOC_STREETNAME = (data.MeterStreet == null) ? string.Empty : data.MeterStreet;
                tranData.METERLOC_STREETTYPE = (data.StreetType == null) ? string.Empty : data.StreetType;
                tranData.METERLOC_CROSSSTREET1 = (data.CrossStreet1 == null) ? string.Empty : data.CrossStreet1;
                tranData.METERLOC_CROSSSTREET2 = (data.CrossStreet2 == null) ? string.Empty : data.CrossStreet2;
                tranData.METERLOC_ENFORCEMENTHOURSDESC = (data.EnfHourDesc == null) ? string.Empty : data.EnfHourDesc;
                tranData.LicensePlate = (data.PlateNumber == null) ? string.Empty : data.PlateNumber;
                tranData.State = (data.LPLocation == null) ? string.Empty : data.LPLocation;
                tranData.Make = (data.VMake == null) ? string.Empty : data.VMake;
                tranData.Model = (data.VModel == null) ? string.Empty : data.VModel;
                tranData.IsOccupied = Utility.getStatus((data.SpaceStatus == null) ? string.Empty : data.SpaceStatus).ToString();
                tranData.EnforcementKey = (data.EnforcementKey == null) ? string.Empty : data.EnforcementKey;
                tranData.MeterStatus = (data.MeterStatus == null) ? null : data.MeterStatus;

                tranData.METERLOC_METERTYPE = (data.METERLOC_METERTYPE == null) ? null : data.METERLOC_METERTYPE;
                tranData.ZoneName = (data.ZoneName == null) ? null : data.ZoneName;
                tranData.MeterID = String.Empty;
                tranData.ReturnCode = data.ReturnCode;

                logData.Response = new ResponseEntity() { CustomerId = param.CustomerId.Value.ToString(), MeterId = tranData.MeterName, SpaceId = tranData.SpaceNo, CityCurrentTime = data.PresentMeterTime.HasValue ? data.PresentMeterTime.Value : DateTime.Now };
            }
            catch (Exception ex)
            {
                ////Intentionally Error Condition Commented
                ////var appPath = System.Web.Hosting.HostingEnvironment.ApplicationPhysicalPath;
                ////string errorPath = Path.Combine(appPath, "GetTransformData", "Timezones");
                ////ExceptionLogging.SendErrorToText(ex, "GetCitationData", errorPath);
                ////tranData.ReturnCode = ((int)ReturnCodeEnum.Error).ToString();

                tranData.ReturnCode = ((int)ReturnCodeEnum.Error).ToString();
            }

            #endregion
            #region "Parkeon Service Call"
            /////////////////////////////////////////////////
            TransactionTransform tranDataOfService = new TransactionTransform();
            ServiceParameters serviceParam = null;
            try
            {
                //////////////////////////////////////////
                logData.Request.CustomerId = param.CustomerId.Value.ToString();
                logData.Request.SpaceId = param.parkingSpaceID;
                logData.Request.MeterId = param.MeterName;
                //////////////////////////////////////////////////
                DateTime? currentCustTime = customerCurrentTime.CurrentTime;
                tranData.EnforcementKey = EnforcementKey;
                //serviceParam = new ServiceParameters() { customerId = 4120, userName = "civicsmart_115_peo", userPassword = @"4x+W6fWc/7,QT?VQ", vendorId = 3 };
                //ParkeonServiceAccess parkeonService = new ParkeonServiceAccess(serviceParam, param);
                ParkeonCustomerElement customer = ParkeonCustomerRetriever.GetCustomer(param.CustomerId.Value);
                serviceParam = new ServiceParameters() { customerId = (Convert.ToInt64(customer.CustomerId)), userName = customer.UserName, userPassword = customer.UserPassword, vendorId = (Convert.ToInt64(customer.VendorId)) };
                ParkeonServiceAccess parkeonService = new ParkeonServiceAccess(serviceParam, param, customer);
                ///////////////////
                var parkeonResLog = new VendorResponseEntity();
                logData.VendorData.Add(VendorNames.Parkeon, parkeonResLog);
                parkeonService.ProcessRequest(ref tranDataOfService, TypeConditions.check_plate, TZ, ref currentCustTime, ref parkeonResLog);
                serviceParam = null;
                ///////////////////////////////////////////////////
                logData.Response = new ResponseEntity() { CustomerId = param.CustomerId.Value.ToString(), MeterId = tranData.MeterName, SpaceId = tranData.SpaceNo, CityCurrentTime = currentCustTime.HasValue ? currentCustTime.Value : DateTime.Now };

            }
            catch (Exception ex)
            {
                logData.Response = new ResponseEntity() { CustomerId = param.CustomerId.Value.ToString(), MeterId = tranData.MeterName, SpaceId = tranData.SpaceNo, CityCurrentTime = customerCurrentTime.CurrentTime.HasValue ? customerCurrentTime.CurrentTime.Value : DateTime.Now };
                //throw ex;
                tranDataOfService.ReturnCode = ((int)ReturnCodeEnum.Error).ToString();
            }
            finally
            {
                serviceParam = null;
            }
            #endregion
            #region "Parkmobile Service Call"
            /////////////////////////////////////////////////
            TransactionTransform tranDataOfServiceParkMobile = new TransactionTransform();
            try
            {
                //////////////////////////////////////////
                logData.Request.CustomerId = param.CustomerId.Value.ToString();
                logData.Request.SpaceId = param.parkingSpaceID;
                logData.Request.MeterId = param.MeterName;
                //////////////////////////////////////////////////
                DateTime? currentCustTime = customerCurrentTime.CurrentTime;
                PMCustomerElement customer = ParkmobileCustomerRetriever.GetCustomer(param.CustomerId.Value);
                ParkMobileGenericFactory factory = new ParkMobileGenericFactory(customer, param.PlateNumber);
                var parkMobileResLog = new VendorResponseEntity();
                logData.VendorData.Add(VendorNames.ParkMobile, parkMobileResLog);
                tranDataOfServiceParkMobile = factory.GetRefreshSpaceData(ParkingMobileDataType.Plate, ref parkMobileResLog);
                tranDataOfServiceParkMobile.EnforcementKey = EnforcementKey;

                ///////////////////////////////////////////////////
                logData.Response = new ResponseEntity() { CustomerId = param.CustomerId.Value.ToString(), MeterId = tranData.MeterName, SpaceId = tranData.SpaceNo, CityCurrentTime = currentCustTime.HasValue ? currentCustTime.Value : DateTime.Now };

            }
            catch (Exception ex)
            {
                logData.Response = new ResponseEntity() { CustomerId = param.CustomerId.Value.ToString(), MeterId = tranData.MeterName, SpaceId = tranData.SpaceNo, CityCurrentTime = customerCurrentTime.CurrentTime.HasValue ? customerCurrentTime.CurrentTime.Value : DateTime.Now };
                //throw ex;
                tranDataOfServiceParkMobile.ReturnCode = ((int)ReturnCodeEnum.Error).ToString();
            }
            finally
            {

            }
            #endregion

            //////////////////  //Reconcile data

            List<TransactionTransform> trans = new List<TransactionTransform>();
            trans.Add(tranDataOfServiceParkMobile); // Parkmobile
            trans.Add(tranData); // Database
            trans.Add(tranDataOfService); //Parkeon


            var rightValue = trans.Where(t => t.ReturnCode == ((int)ReturnCodeEnum.Success).ToString()).OrderByDescending(t => t.MeterExpiredMinutes.ToNullableInt()).FirstOrDefault<TransactionTransform>();
            if (rightValue == default(TransactionTransform))
            {
                //Do nothing.just return database  object.
            }
            else
            {
                tranData = rightValue;
            }

            return tranData;
        }
        public static TransactionTransform GetAtlantaTransformDataV3(string EnforcementKey, string filePath, RequestResponseEntity logData, CustomerTime customerCurrentTime)
        {
            TransactionTransform tranData = new TransactionTransform();
            tranData.ReturnCode = ((int)ReturnCodeEnum.InvalidInputParameters).ToString();
            TimeZoneInfo TZ = null;
            TZ = TimeZoneInfo.FindSystemTimeZoneById("Eastern Standard Time");
            //int? customerID = null;
            //string MeterName = null;
            //string parkingSpaceID;
            //string PlateNumber = null;
            //string stateName = null;
            //////////////////////////////////////
            KeyParameter param = PrepareKeyParameters.PrepareAtlanta(EnforcementKey);
            if (param.ReturnCode == ((int)ReturnCodeEnum.InvalidInputParameters).ToString())
            {
                return tranData;
            }

            #region "DB Check"
            /////////////////////////////////////
            try
            {
                Transaction data = TransactionDataValidatioin.GetDataAtlanta(param.CustomerId.Value, param.MeterName, param.parkingSpaceID, param.PlateNumber, param.StateName, EnforcementKey, filePath, logData);


                if (data.TimeZoneId.HasValue)
                    TZ = Utility.GetTimeZoneInfo(data.TimeZoneId.Value, filePath, param.CustomerId.Value.ToString());

                //For South Miami : PayStationId. Others it is MeterID
                tranData.MeterName = string.Empty; // (data.MeterName == null) ? string.Empty : data.MeterName;
                tranData.SpaceNo = string.Empty;  //data.ParkingSpaceID.ToString();

                tranData.MeterStreet = (data.MeterStreet == null) ? string.Empty : data.MeterStreet;

                DateTime _MeterRTC = data.PresentMeterTime.HasValue ? data.PresentMeterTime.Value : DateTime.Now;
                tranData.MeterRTC = String.Format("{0:yyyyMMdd_THHmmssZ}", Utility.findGMTTime(_MeterRTC, TZ));

                DateTime _SensorEventTime = data.SensorEventTime.HasValue ? data.SensorEventTime.Value : DateTime.Now;
                tranData.MeterLastUpdateTime = String.Format("{0:yyyyMMdd_THHmmssZ}", Utility.findGMTTime(_SensorEventTime, TZ));

                tranData.SensorEventTime = String.Format("{0:yyyyMMdd_THHmmssZ}", Utility.findGMTTime(_SensorEventTime, TZ));

                DateTime _endTime = data.ExpiryTime.HasValue ? data.ExpiryTime.Value : DateTime.Now;
                tranData.ExpiredTime = String.Format("{0:yyyyMMdd_THHmmssZ}", Utility.findGMTTime(_endTime, TZ));

                tranData.MeterExpiredMinutes = data.ExpiredMinutes.HasValue ? data.ExpiredMinutes.Value.ToString() : "0";
                tranData.METERLOC_BLOCKNUMBER = (data.Block == null) ? string.Empty : data.Block;
                tranData.METERLOC_STREETDIRECTION = (data.Direction == null) ? string.Empty : data.Direction;
                tranData.METERLOC_STREETNAME = (data.MeterStreet == null) ? string.Empty : data.MeterStreet;
                tranData.METERLOC_STREETTYPE = (data.StreetType == null) ? string.Empty : data.StreetType;
                tranData.METERLOC_CROSSSTREET1 = (data.CrossStreet1 == null) ? string.Empty : data.CrossStreet1;
                tranData.METERLOC_CROSSSTREET2 = (data.CrossStreet2 == null) ? string.Empty : data.CrossStreet2;
                tranData.METERLOC_ENFORCEMENTHOURSDESC = (data.EnfHourDesc == null) ? string.Empty : data.EnfHourDesc;
                tranData.LicensePlate = (data.PlateNumber == null) ? string.Empty : data.PlateNumber;
                tranData.State = (data.LPLocation == null) ? string.Empty : data.LPLocation;
                tranData.Make = (data.VMake == null) ? string.Empty : data.VMake;
                tranData.Model = (data.VModel == null) ? string.Empty : data.VModel;
                tranData.IsOccupied = Utility.getStatus((data.SpaceStatus == null) ? string.Empty : data.SpaceStatus).ToString();
                tranData.EnforcementKey = (data.EnforcementKey == null) ? string.Empty : data.EnforcementKey;
                tranData.MeterStatus = (data.MeterStatus == null) ? null : data.MeterStatus;

                tranData.METERLOC_METERTYPE = (data.METERLOC_METERTYPE == null) ? null : data.METERLOC_METERTYPE;
                tranData.ZoneName = (data.ZoneName == null) ? null : data.ZoneName;
                tranData.MeterID = String.Empty;
                tranData.ReturnCode = data.ReturnCode;

                logData.Response = new ResponseEntity() { CustomerId = param.CustomerId.Value.ToString(), MeterId = tranData.MeterName, SpaceId = tranData.SpaceNo, CityCurrentTime = data.PresentMeterTime.HasValue ? data.PresentMeterTime.Value : DateTime.Now };
            }
            catch (Exception ex)
            {
                ////Intentionally Error Condition Commented
                ////var appPath = System.Web.Hosting.HostingEnvironment.ApplicationPhysicalPath;
                ////string errorPath = Path.Combine(appPath, "GetTransformData", "Timezones");
                ////ExceptionLogging.SendErrorToText(ex, "GetCitationData", errorPath);
                ////tranData.ReturnCode = ((int)ReturnCodeEnum.Error).ToString();

                tranData.ReturnCode = ((int)ReturnCodeEnum.Error).ToString();
            }

            #endregion
            #region "Parkeon Service Call"
            /////////////////////////////////////////////////
            TransactionTransform tranDataOfService = new TransactionTransform();
            ServiceParameters serviceParam = null;
            try
            {
                //////////////////////////////////////////
                logData.Request.CustomerId = param.CustomerId.Value.ToString();
                logData.Request.SpaceId = param.parkingSpaceID;
                logData.Request.MeterId = param.MeterName;
                //////////////////////////////////////////////////
                DateTime? currentCustTime = customerCurrentTime.CurrentTime;
                tranData.EnforcementKey = EnforcementKey;
                //serviceParam = new ServiceParameters() { customerId = 4120, userName = "civicsmart_115_peo", userPassword = @"4x+W6fWc/7,QT?VQ", vendorId = 3 };
                //ParkeonServiceAccess parkeonService = new ParkeonServiceAccess(serviceParam, param);
                ParkeonCustomerElement customer = ParkeonCustomerRetriever.GetCustomer(param.CustomerId.Value);
                serviceParam = new ServiceParameters() { customerId = (Convert.ToInt64(customer.CustomerId)), userName = customer.UserName, userPassword = customer.UserPassword, vendorId = (Convert.ToInt64(customer.VendorId)) };
                ParkeonServiceAccess parkeonService = new ParkeonServiceAccess(serviceParam, param, customer);
                ///////////////////

                parkeonService.ProcessRequest(ref tranDataOfService, TypeConditions.check_plate, TZ, ref currentCustTime);
                serviceParam = null;
                ///////////////////////////////////////////////////
                logData.Response = new ResponseEntity() { CustomerId = param.CustomerId.Value.ToString(), MeterId = tranData.MeterName, SpaceId = tranData.SpaceNo, CityCurrentTime = currentCustTime.HasValue ? currentCustTime.Value : DateTime.Now };

            }
            catch (Exception ex)
            {
                logData.Response = new ResponseEntity() { CustomerId = param.CustomerId.Value.ToString(), MeterId = tranData.MeterName, SpaceId = tranData.SpaceNo, CityCurrentTime = customerCurrentTime.CurrentTime.HasValue ? customerCurrentTime.CurrentTime.Value : DateTime.Now };
                //throw ex;
                tranDataOfService.ReturnCode = ((int)ReturnCodeEnum.Error).ToString();
            }
            finally
            {
                serviceParam = null;
            }
            #endregion
            #region "Parkmobile Service Call"
            /////////////////////////////////////////////////
            TransactionTransform tranDataOfServiceParkMobile = new TransactionTransform();
            try
            {
                //////////////////////////////////////////
                logData.Request.CustomerId = param.CustomerId.Value.ToString();
                logData.Request.SpaceId = param.parkingSpaceID;
                logData.Request.MeterId = param.MeterName;
                //////////////////////////////////////////////////
                DateTime? currentCustTime = customerCurrentTime.CurrentTime;
                PMCustomerElement customer = ParkmobileCustomerRetriever.GetCustomer(param.CustomerId.Value);
                ParkMobileGenericFactory factory = new ParkMobileGenericFactory(customer, param.PlateNumber);
                tranDataOfServiceParkMobile = factory.GetRefreshSpaceData(ParkingMobileDataType.Plate);
                tranDataOfServiceParkMobile.EnforcementKey = EnforcementKey;

                ///////////////////////////////////////////////////
                logData.Response = new ResponseEntity() { CustomerId = param.CustomerId.Value.ToString(), MeterId = tranData.MeterName, SpaceId = tranData.SpaceNo, CityCurrentTime = currentCustTime.HasValue ? currentCustTime.Value : DateTime.Now };

            }
            catch (Exception ex)
            {
                logData.Response = new ResponseEntity() { CustomerId = param.CustomerId.Value.ToString(), MeterId = tranData.MeterName, SpaceId = tranData.SpaceNo, CityCurrentTime = customerCurrentTime.CurrentTime.HasValue ? customerCurrentTime.CurrentTime.Value : DateTime.Now };
                //throw ex;
                tranDataOfServiceParkMobile.ReturnCode = ((int)ReturnCodeEnum.Error).ToString();
            }
            finally
            {

            }
            #endregion

            //////////////////  //Reconcile data

            List<TransactionTransform> trans = new List<TransactionTransform>();
            trans.Add(tranDataOfServiceParkMobile); // Parkmobile
            trans.Add(tranData); // Database
            trans.Add(tranDataOfService); //Parkeon


            var rightValue = trans.Where(t => t.ReturnCode == ((int)ReturnCodeEnum.Success).ToString()).OrderByDescending(t => t.MeterExpiredMinutes.ToNullableInt()).FirstOrDefault<TransactionTransform>();
            if (rightValue == default(TransactionTransform))
            {
                //Do nothing.just return database  object.
            }
            else
            {
                tranData = rightValue;
            }

            return tranData;
        }
        public static TransactionTransform GetAtlantaTransformData(string EnforcementKey, string filePath, RequestResponseEntity logData, CustomerTime customerCurrentTime)
        {
            TransactionTransform tranData = new TransactionTransform();
            tranData.ReturnCode = ((int)ReturnCodeEnum.InvalidInputParameters).ToString();
            TimeZoneInfo TZ = null;
            TZ = TimeZoneInfo.FindSystemTimeZoneById("Eastern Standard Time");
            //int? customerID = null;
            //string MeterName = null;
            //string parkingSpaceID;
            //string PlateNumber = null;
            //string stateName = null;
            //////////////////////////////////////
            KeyParameter param = PrepareKeyParameters.PrepareAtlanta(EnforcementKey);
            if (param.ReturnCode == ((int)ReturnCodeEnum.InvalidInputParameters).ToString())
            {
                return tranData;
            }

            #region "DB Check"
            /////////////////////////////////////
            try
            {
                Transaction data = TransactionDataValidatioin.GetDataAtlanta(param.CustomerId.Value, param.MeterName, param.parkingSpaceID, param.PlateNumber, param.StateName, EnforcementKey, filePath, logData);


                if (data.TimeZoneId.HasValue)
                    TZ = Utility.GetTimeZoneInfo(data.TimeZoneId.Value, filePath, param.CustomerId.Value.ToString());

                //For South Miami : PayStationId. Others it is MeterID
                tranData.MeterName = string.Empty; // (data.MeterName == null) ? string.Empty : data.MeterName;
                tranData.SpaceNo = string.Empty;  //data.ParkingSpaceID.ToString();

                tranData.MeterStreet = (data.MeterStreet == null) ? string.Empty : data.MeterStreet;

                DateTime _MeterRTC = data.PresentMeterTime.HasValue ? data.PresentMeterTime.Value : DateTime.Now;
                tranData.MeterRTC = String.Format("{0:yyyyMMdd_THHmmssZ}", Utility.findGMTTime(_MeterRTC, TZ));

                DateTime _SensorEventTime = data.SensorEventTime.HasValue ? data.SensorEventTime.Value : DateTime.Now;
                tranData.MeterLastUpdateTime = String.Format("{0:yyyyMMdd_THHmmssZ}", Utility.findGMTTime(_SensorEventTime, TZ));

                tranData.SensorEventTime = String.Format("{0:yyyyMMdd_THHmmssZ}", Utility.findGMTTime(_SensorEventTime, TZ));

                DateTime _endTime = data.ExpiryTime.HasValue ? data.ExpiryTime.Value : DateTime.Now;
                tranData.ExpiredTime = String.Format("{0:yyyyMMdd_THHmmssZ}", Utility.findGMTTime(_endTime, TZ));

                tranData.MeterExpiredMinutes = data.ExpiredMinutes.HasValue ? data.ExpiredMinutes.Value.ToString() : "0";
                tranData.METERLOC_BLOCKNUMBER = (data.Block == null) ? string.Empty : data.Block;
                tranData.METERLOC_STREETDIRECTION = (data.Direction == null) ? string.Empty : data.Direction;
                tranData.METERLOC_STREETNAME = (data.MeterStreet == null) ? string.Empty : data.MeterStreet;
                tranData.METERLOC_STREETTYPE = (data.StreetType == null) ? string.Empty : data.StreetType;
                tranData.METERLOC_CROSSSTREET1 = (data.CrossStreet1 == null) ? string.Empty : data.CrossStreet1;
                tranData.METERLOC_CROSSSTREET2 = (data.CrossStreet2 == null) ? string.Empty : data.CrossStreet2;
                tranData.METERLOC_ENFORCEMENTHOURSDESC = (data.EnfHourDesc == null) ? string.Empty : data.EnfHourDesc;
                tranData.LicensePlate = (data.PlateNumber == null) ? string.Empty : data.PlateNumber;
                tranData.State = (data.LPLocation == null) ? string.Empty : data.LPLocation;
                tranData.Make = (data.VMake == null) ? string.Empty : data.VMake;
                tranData.Model = (data.VModel == null) ? string.Empty : data.VModel;
                tranData.IsOccupied = Utility.getStatus((data.SpaceStatus == null) ? string.Empty : data.SpaceStatus).ToString();
                tranData.EnforcementKey = (data.EnforcementKey == null) ? string.Empty : data.EnforcementKey;
                tranData.MeterStatus = (data.MeterStatus == null) ? null : data.MeterStatus;

                tranData.METERLOC_METERTYPE = (data.METERLOC_METERTYPE == null) ? null : data.METERLOC_METERTYPE;
                tranData.ZoneName = (data.ZoneName == null) ? null : data.ZoneName;
                tranData.MeterID = String.Empty;
                tranData.ReturnCode = data.ReturnCode;

                logData.Response = new ResponseEntity() { CustomerId = param.CustomerId.Value.ToString(), MeterId = tranData.MeterName, SpaceId = tranData.SpaceNo, CityCurrentTime = data.PresentMeterTime.HasValue ? data.PresentMeterTime.Value : DateTime.Now };
            }
            catch (Exception ex)
            {
                ////Intentionally Error Condition Commented
                ////var appPath = System.Web.Hosting.HostingEnvironment.ApplicationPhysicalPath;
                ////string errorPath = Path.Combine(appPath, "GetTransformData", "Timezones");
                ////ExceptionLogging.SendErrorToText(ex, "GetCitationData", errorPath);
                ////tranData.ReturnCode = ((int)ReturnCodeEnum.Error).ToString();

                tranData.ReturnCode = ((int)ReturnCodeEnum.Error).ToString();
            }

            #endregion
            #region "Parkeon Service Call"
            /////////////////////////////////////////////////
            TransactionTransform tranDataOfService = new TransactionTransform();
            ServiceParameters serviceParam = null;
            try
            {
                //////////////////////////////////////////
                logData.Request.CustomerId = param.CustomerId.Value.ToString();
                logData.Request.SpaceId = param.parkingSpaceID;
                logData.Request.MeterId = param.MeterName;
                //////////////////////////////////////////////////
                DateTime? currentCustTime = customerCurrentTime.CurrentTime;
                tranData.EnforcementKey = EnforcementKey;
                serviceParam = new ServiceParameters() { customerId = 4120, userName = "civicsmart_115_peo", userPassword = @"4x+W6fWc/7,QT?VQ", vendorId = 3 };
                ParkeonServiceAccess parkeonService = new ParkeonServiceAccess(serviceParam, param);
                parkeonService.ProcessRequest(ref tranDataOfService, TypeConditions.check_plate, TZ, ref currentCustTime);
                serviceParam = null;
                ///////////////////////////////////////////////////
                logData.Response = new ResponseEntity() { CustomerId = param.CustomerId.Value.ToString(), MeterId = tranData.MeterName, SpaceId = tranData.SpaceNo, CityCurrentTime = currentCustTime.HasValue ? currentCustTime.Value : DateTime.Now };

            }
            catch (Exception ex)
            {
                logData.Response = new ResponseEntity() { CustomerId = param.CustomerId.Value.ToString(), MeterId = tranData.MeterName, SpaceId = tranData.SpaceNo, CityCurrentTime = customerCurrentTime.CurrentTime.HasValue ? customerCurrentTime.CurrentTime.Value : DateTime.Now };
                //throw ex;
                tranData.ReturnCode = ((int)ReturnCodeEnum.Error).ToString();
            }
            finally
            {
                serviceParam = null;
            }
            #endregion


            //Reconcile data
            if ((tranData.ReturnCode == ((int)ReturnCodeEnum.NoRecordExists).ToString()) || (tranData.ReturnCode == ((int)ReturnCodeEnum.Error).ToString()))
            {
                tranData = tranDataOfService;
            }
            else if (tranData.ReturnCode == ((int)ReturnCodeEnum.Success).ToString())
            {
                if (tranDataOfService.ReturnCode == ((int)ReturnCodeEnum.Success).ToString())
                {
                    ////Select one record of highest expiry time
                    int? dbExpiredMinutes = tranData.MeterExpiredMinutes.ToNullableInt();
                    int? parkeonExpiredMinutes = tranDataOfService.MeterExpiredMinutes.ToNullableInt();
                    tranData = Nullable.Compare(dbExpiredMinutes, parkeonExpiredMinutes) > 0 ? tranData : tranDataOfService;
                }

            }

            else if (tranData.ReturnCode == ((int)ReturnCodeEnum.MoreRecordsExists).ToString())
            {
                // DB never give more record. hence ignore
            }


            return tranData;
        }

        public static TransactionTransform GetAtlantaTransformDataV1(string EnforcementKey, string filePath, RequestResponseEntity logData, CustomerTime customerCurrentTime)
        {
            TransactionTransform tranData = new TransactionTransform();
            tranData.ReturnCode = ((int)ReturnCodeEnum.InvalidInputParameters).ToString();
            TimeZoneInfo TZ = null;
            TZ = TimeZoneInfo.FindSystemTimeZoneById("Eastern Standard Time");
            //int? customerID = null;
            //string MeterName = null;
            //string parkingSpaceID;
            //string PlateNumber = null;
            //string stateName = null;
            //////////////////////////////////////
            KeyParameter param = PrepareKeyParameters.PrepareAtlanta(EnforcementKey);
            if (param.ReturnCode == ((int)ReturnCodeEnum.InvalidInputParameters).ToString())
            {
                return tranData;
            }


            #region "Parkeon Service Call"
            /////////////////////////////////////////////////
            ServiceParameters serviceParam = null;
            try
            {
                //////////////////////////////////////////
                logData.Request.CustomerId = param.CustomerId.Value.ToString();
                logData.Request.SpaceId = param.parkingSpaceID;
                logData.Request.MeterId = param.MeterName;
                //////////////////////////////////////////////////
                DateTime? currentCustTime = customerCurrentTime.CurrentTime;
                tranData.EnforcementKey = EnforcementKey;
                serviceParam = new ServiceParameters() { customerId = 4120, userName = "civicsmart_115_peo", userPassword = @"4x+W6fWc/7,QT?VQ", vendorId = 3 };
                ParkeonServiceAccess parkeonService = new ParkeonServiceAccess(serviceParam, param);
                parkeonService.ProcessRequest(ref tranData, TypeConditions.check_plate, TZ, ref currentCustTime);
                serviceParam = null;
                ///////////////////////////////////////////////////
                logData.Response = new ResponseEntity() { CustomerId = param.CustomerId.Value.ToString(), MeterId = tranData.MeterName, SpaceId = tranData.SpaceNo, CityCurrentTime = currentCustTime.HasValue ? currentCustTime.Value : DateTime.Now };

            }
            catch (Exception ex)
            {
                logData.Response = new ResponseEntity() { CustomerId = param.CustomerId.Value.ToString(), MeterId = tranData.MeterName, SpaceId = tranData.SpaceNo, CityCurrentTime = customerCurrentTime.CurrentTime.HasValue ? customerCurrentTime.CurrentTime.Value : DateTime.Now };
                throw ex;
            }
            finally
            {
                serviceParam = null;
            }
            #endregion

            #region "DB Check"
            /////////////////////////////////////
            try
            {
                Transaction data = TransactionDataValidatioin.GetDataAtlanta(param.CustomerId.Value, param.MeterName, param.parkingSpaceID, param.PlateNumber, param.StateName, EnforcementKey, filePath, logData);


                if (data.TimeZoneId.HasValue)
                    TZ = Utility.GetTimeZoneInfo(data.TimeZoneId.Value, filePath, param.CustomerId.Value.ToString());

                //For South Miami : PayStationId. Others it is MeterID
                tranData.MeterName = string.Empty; // (data.MeterName == null) ? string.Empty : data.MeterName;
                tranData.SpaceNo = string.Empty;  //data.ParkingSpaceID.ToString();

                tranData.MeterStreet = (data.MeterStreet == null) ? string.Empty : data.MeterStreet;

                DateTime _MeterRTC = data.PresentMeterTime.HasValue ? data.PresentMeterTime.Value : DateTime.Now;
                tranData.MeterRTC = String.Format("{0:yyyyMMdd_THHmmssZ}", Utility.findGMTTime(_MeterRTC, TZ));

                DateTime _SensorEventTime = data.SensorEventTime.HasValue ? data.SensorEventTime.Value : DateTime.Now;
                tranData.MeterLastUpdateTime = String.Format("{0:yyyyMMdd_THHmmssZ}", Utility.findGMTTime(_SensorEventTime, TZ));

                tranData.SensorEventTime = String.Format("{0:yyyyMMdd_THHmmssZ}", Utility.findGMTTime(_SensorEventTime, TZ));

                DateTime _endTime = data.ExpiryTime.HasValue ? data.ExpiryTime.Value : DateTime.Now;
                tranData.ExpiredTime = String.Format("{0:yyyyMMdd_THHmmssZ}", Utility.findGMTTime(_endTime, TZ));

                tranData.MeterExpiredMinutes = data.ExpiredMinutes.HasValue ? data.ExpiredMinutes.Value.ToString() : "0";
                tranData.METERLOC_BLOCKNUMBER = (data.Block == null) ? string.Empty : data.Block;
                tranData.METERLOC_STREETDIRECTION = (data.Direction == null) ? string.Empty : data.Direction;
                tranData.METERLOC_STREETNAME = (data.MeterStreet == null) ? string.Empty : data.MeterStreet;
                tranData.METERLOC_STREETTYPE = (data.StreetType == null) ? string.Empty : data.StreetType;
                tranData.METERLOC_CROSSSTREET1 = (data.CrossStreet1 == null) ? string.Empty : data.CrossStreet1;
                tranData.METERLOC_CROSSSTREET2 = (data.CrossStreet2 == null) ? string.Empty : data.CrossStreet2;
                tranData.METERLOC_ENFORCEMENTHOURSDESC = (data.EnfHourDesc == null) ? string.Empty : data.EnfHourDesc;
                tranData.LicensePlate = (data.PlateNumber == null) ? string.Empty : data.PlateNumber;
                tranData.State = (data.LPLocation == null) ? string.Empty : data.LPLocation;
                tranData.Make = (data.VMake == null) ? string.Empty : data.VMake;
                tranData.Model = (data.VModel == null) ? string.Empty : data.VModel;
                tranData.IsOccupied = Utility.getStatus((data.SpaceStatus == null) ? string.Empty : data.SpaceStatus).ToString();
                tranData.EnforcementKey = (data.EnforcementKey == null) ? string.Empty : data.EnforcementKey;
                tranData.MeterStatus = (data.MeterStatus == null) ? null : data.MeterStatus;

                tranData.METERLOC_METERTYPE = (data.METERLOC_METERTYPE == null) ? null : data.METERLOC_METERTYPE;
                tranData.ZoneName = (data.ZoneName == null) ? null : data.ZoneName;
                tranData.MeterID = String.Empty;
                tranData.ReturnCode = data.ReturnCode;

                logData.Response = new ResponseEntity() { CustomerId = param.CustomerId.Value.ToString(), MeterId = tranData.MeterName, SpaceId = tranData.SpaceNo, CityCurrentTime = data.PresentMeterTime.HasValue ? data.PresentMeterTime.Value : DateTime.Now };
            }
            catch (Exception ex)
            {
                var appPath = System.Web.Hosting.HostingEnvironment.ApplicationPhysicalPath;
                string errorPath = Path.Combine(appPath, "GetTransformData", "Timezones");
                ExceptionLogging.SendErrorToText(ex, "GetCitationData", errorPath);
                tranData.ReturnCode = ((int)ReturnCodeEnum.Error).ToString();

            }
            return tranData;
            #endregion
        }
        private static Transaction GetDataAtlanta(int customerID, string MeterName, string parkingSpaceID, string PlateNumber, string statename, string EnforcementKey, string filePath, RequestResponseEntity logData)
        {
            string ZoneID = null;
            string SpaceStatus = null;
            Transaction data = new Transaction();
            try
            {
                List<Transaction> transactionList = new List<Transaction>();

                logData.Request.CustomerId = customerID.ToString();
                logData.Request.SpaceId = parkingSpaceID;
                logData.Request.MeterId = MeterName;

                transactionList = DataAccess.GetAtlantaCitationDataOthersWithPlateName(customerID, null, null, null, PlateNumber, statename, null);

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
            }
            catch (Exception ex)
            {
                var appPath = System.Web.Hosting.HostingEnvironment.ApplicationPhysicalPath;
                //var appPath = @"G:\Projects\Services-AutoIssueNEW\CitationValidation\CitationValidation\TestApplication";
                string errorPath = Path.Combine(appPath, "GetData", "Timezones");
                ExceptionLogging.SendErrorToText(ex, "GetDataAtlanta", errorPath);
                data.ReturnCode = ((int)ReturnCodeEnum.Error).ToString();
                throw;
            }
            return data;
        }
        private static Transaction GetDataAtlantaV5(int customerID, string MeterName, string parkingSpaceID, string PlateNumber, string statename, string EnforcementKey, string filePath, RequestResponseEntity logData)
        {
            string ZoneID = null;
            string SpaceStatus = null;
            Transaction data = new Transaction();
            try
            {
                List<Transaction> transactionList = new List<Transaction>();

                logData.Request.CustomerId = customerID.ToString();
                logData.Request.SpaceId = parkingSpaceID;
                logData.Request.MeterId = MeterName;

                transactionList = DataAccess.GetAtlantaCitationDataOthersWithPlateNameV5(customerID, null, null, null, PlateNumber, statename, null);

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
            }
            catch (Exception ex)
            {
                var appPath = System.Web.Hosting.HostingEnvironment.ApplicationPhysicalPath;
                string errorPath = Path.Combine(appPath, "GetData", "Timezones");
                ExceptionLogging.SendErrorToText(ex, "GetDataAtlanta", errorPath);
                data.ReturnCode = ((int)ReturnCodeEnum.Error).ToString();
                throw;
            }
            return data;
        }
        #endregion

        #region SouthMiami
        public static TransactionTransform GetSouthMiamiTransformData(string EnforcementKey, string filePath, RequestResponseEntity logData)
        {
            TransactionTransform tranData = new TransactionTransform();
            tranData.ReturnCode = ((int)ReturnCodeEnum.InvalidInputParameters).ToString();
            TimeZoneInfo TZ = null;
            TZ = TimeZoneInfo.FindSystemTimeZoneById("Eastern Standard Time");
            //int? customerID = null;
            //string MeterName = null;
            //string parkingSpaceID;
            //string PlateNumber = null;
            //string stateName = null;
            //////////////////////////////////////
            KeyParameter param = PrepareKeyParameters.PrepareSouthMiami(EnforcementKey);
            if (param.ReturnCode == ((int)ReturnCodeEnum.InvalidInputParameters).ToString())
            {
                return tranData;
            }

            ///Special case //As specified by Alan
            ///When the client is sending alphanumeric space numbers for validation, “61A610” etc, the service is responding with an response code of “-1” and error text  “Specified cast is not valid”
            //Please update this exception handling to instead return a response code of “0” and an expired minutes of “-1”
            if (HandleSepecialSpace(param, EnforcementKey, ref tranData, ref logData, TZ))
                return tranData;
            /////////////end special Case/////////////////
            /////////////////////////////////////
            try
            {
                Transaction data = TransactionDataValidatioin.GetDataSouthMiami(param.CustomerId.Value, param.MeterName, param.parkingSpaceID, param.PlateNumber, param.StateName, EnforcementKey, filePath, logData);


                if (data.TimeZoneId.HasValue)
                    TZ = Utility.GetTimeZoneInfo(data.TimeZoneId.Value, filePath, param.CustomerId.Value.ToString());

                //For South Miami : PayStationId. Others it is MeterID
                tranData.MeterName = (data.MeterName == null) ? string.Empty : data.MeterName;
                tranData.SpaceNo = data.ParkingSpaceID.ToString();

                tranData.MeterStreet = (data.MeterStreet == null) ? string.Empty : data.MeterStreet;

                DateTime _MeterRTC = data.PresentMeterTime.HasValue ? data.PresentMeterTime.Value : DateTime.Now;
                tranData.MeterRTC = String.Format("{0:yyyyMMdd_THHmmssZ}", Utility.findGMTTime(_MeterRTC, TZ));

                DateTime _SensorEventTime = data.SensorEventTime.HasValue ? data.SensorEventTime.Value : DateTime.Now;
                tranData.MeterLastUpdateTime = String.Format("{0:yyyyMMdd_THHmmssZ}", Utility.findGMTTime(_SensorEventTime, TZ));

                tranData.SensorEventTime = String.Format("{0:yyyyMMdd_THHmmssZ}", Utility.findGMTTime(_SensorEventTime, TZ));

                DateTime _endTime = data.ExpiryTime.HasValue ? data.ExpiryTime.Value : DateTime.Now;
                tranData.ExpiredTime = String.Format("{0:yyyyMMdd_THHmmssZ}", Utility.findGMTTime(_endTime, TZ));

                tranData.MeterExpiredMinutes = data.ExpiredMinutes.HasValue ? data.ExpiredMinutes.Value.ToString() : "0";
                tranData.METERLOC_BLOCKNUMBER = (data.Block == null) ? string.Empty : data.Block;
                tranData.METERLOC_STREETDIRECTION = (data.Direction == null) ? string.Empty : data.Direction;
                tranData.METERLOC_STREETNAME = (data.MeterStreet == null) ? string.Empty : data.MeterStreet;
                tranData.METERLOC_STREETTYPE = (data.StreetType == null) ? string.Empty : data.StreetType;
                tranData.METERLOC_CROSSSTREET1 = (data.CrossStreet1 == null) ? string.Empty : data.CrossStreet1;
                tranData.METERLOC_CROSSSTREET2 = (data.CrossStreet2 == null) ? string.Empty : data.CrossStreet2;
                tranData.METERLOC_ENFORCEMENTHOURSDESC = (data.EnfHourDesc == null) ? string.Empty : data.EnfHourDesc;
                tranData.LicensePlate = (data.PlateNumber == null) ? string.Empty : data.PlateNumber;
                tranData.State = (data.LPLocation == null) ? string.Empty : data.LPLocation;
                tranData.Make = (data.VMake == null) ? string.Empty : data.VMake;
                tranData.Model = (data.VModel == null) ? string.Empty : data.VModel;
                tranData.IsOccupied = Utility.getStatus((data.SpaceStatus == null) ? string.Empty : data.SpaceStatus).ToString();
                tranData.EnforcementKey = (data.EnforcementKey == null) ? string.Empty : data.EnforcementKey;
                tranData.MeterStatus = (data.MeterStatus == null) ? null : data.MeterStatus;

                tranData.METERLOC_METERTYPE = (data.METERLOC_METERTYPE == null) ? null : data.METERLOC_METERTYPE;
                tranData.ZoneName = (data.ZoneName == null) ? null : data.ZoneName;

                tranData.MeterID = data.PayStationID.HasValue ? data.PayStationID.Value.ToString() : "0";
                tranData.ReturnCode = data.ReturnCode;

                logData.Response = new ResponseEntity() { CustomerId = param.CustomerId.Value.ToString(), MeterId = tranData.MeterName, SpaceId = tranData.SpaceNo, CityCurrentTime = data.PresentMeterTime.HasValue ? data.PresentMeterTime.Value : DateTime.Now };
            }
            catch (Exception ex)
            {
                var appPath = System.Web.Hosting.HostingEnvironment.ApplicationPhysicalPath;
                string errorPath = Path.Combine(appPath, "GetTransformData", "Timezones");
                ExceptionLogging.SendErrorToText(ex, "GetCitationData", errorPath);
                tranData.ReturnCode = ((int)ReturnCodeEnum.Error).ToString();

            }
            return tranData;
        }
        public static TransactionTransform GetSouthMiamiTransformDataV2(string EnforcementKey, string filePath, RequestResponseEntity logData, CustomerTime customerCurrentTime)
        {
            TransactionTransform tranData = new TransactionTransform();
            tranData.ReturnCode = ((int)ReturnCodeEnum.InvalidInputParameters).ToString();
            TimeZoneInfo TZ = null;
            TZ = TimeZoneInfo.FindSystemTimeZoneById("Eastern Standard Time");
            //int? customerID = null;
            //string MeterName = null;
            //string parkingSpaceID;
            //string PlateNumber = null;
            //string stateName = null;
            //////////////////////////////////////
            KeyParameter param = PrepareKeyParameters.PrepareSouthMiami(EnforcementKey);
            if (param.ReturnCode == ((int)ReturnCodeEnum.InvalidInputParameters).ToString())
            {
                return tranData;
            }

            ///Special case //As specified by Alan
            ///When the client is sending alphanumeric space numbers for validation, “61A610” etc, the service is responding with an response code of “-1” and error text  “Specified cast is not valid”
            //Please update this exception handling to instead return a response code of “0” and an expired minutes of “-1”
            if (HandleSepecialSpace(param, EnforcementKey, ref tranData, ref logData, TZ))
                return tranData;
            /////////////end special Case/////////////////
            /////////////////////////////////////
            #region DB
            try
            {
                Transaction data = TransactionDataValidatioin.GetDataSouthMiami(param.CustomerId.Value, param.MeterName, param.parkingSpaceID, param.PlateNumber, param.StateName, EnforcementKey, filePath, logData);


                if (data.TimeZoneId.HasValue)
                    TZ = Utility.GetTimeZoneInfo(data.TimeZoneId.Value, filePath, param.CustomerId.Value.ToString());

                //For South Miami : PayStationId. Others it is MeterID
                tranData.MeterName = (data.MeterName == null) ? string.Empty : data.MeterName;
                tranData.SpaceNo = data.ParkingSpaceID.ToString();

                tranData.MeterStreet = (data.MeterStreet == null) ? string.Empty : data.MeterStreet;

                DateTime _MeterRTC = data.PresentMeterTime.HasValue ? data.PresentMeterTime.Value : DateTime.Now;
                tranData.MeterRTC = String.Format("{0:yyyyMMdd_THHmmssZ}", Utility.findGMTTime(_MeterRTC, TZ));

                DateTime _SensorEventTime = data.SensorEventTime.HasValue ? data.SensorEventTime.Value : DateTime.Now;
                tranData.MeterLastUpdateTime = String.Format("{0:yyyyMMdd_THHmmssZ}", Utility.findGMTTime(_SensorEventTime, TZ));

                tranData.SensorEventTime = String.Format("{0:yyyyMMdd_THHmmssZ}", Utility.findGMTTime(_SensorEventTime, TZ));

                DateTime _endTime = data.ExpiryTime.HasValue ? data.ExpiryTime.Value : DateTime.Now;
                tranData.ExpiredTime = String.Format("{0:yyyyMMdd_THHmmssZ}", Utility.findGMTTime(_endTime, TZ));

                tranData.MeterExpiredMinutes = data.ExpiredMinutes.HasValue ? data.ExpiredMinutes.Value.ToString() : "0";
                tranData.METERLOC_BLOCKNUMBER = (data.Block == null) ? string.Empty : data.Block;
                tranData.METERLOC_STREETDIRECTION = (data.Direction == null) ? string.Empty : data.Direction;
                tranData.METERLOC_STREETNAME = (data.MeterStreet == null) ? string.Empty : data.MeterStreet;
                tranData.METERLOC_STREETTYPE = (data.StreetType == null) ? string.Empty : data.StreetType;
                tranData.METERLOC_CROSSSTREET1 = (data.CrossStreet1 == null) ? string.Empty : data.CrossStreet1;
                tranData.METERLOC_CROSSSTREET2 = (data.CrossStreet2 == null) ? string.Empty : data.CrossStreet2;
                tranData.METERLOC_ENFORCEMENTHOURSDESC = (data.EnfHourDesc == null) ? string.Empty : data.EnfHourDesc;
                tranData.LicensePlate = (data.PlateNumber == null) ? string.Empty : data.PlateNumber;
                tranData.State = (data.LPLocation == null) ? string.Empty : data.LPLocation;
                tranData.Make = (data.VMake == null) ? string.Empty : data.VMake;
                tranData.Model = (data.VModel == null) ? string.Empty : data.VModel;
                tranData.IsOccupied = Utility.getStatus((data.SpaceStatus == null) ? string.Empty : data.SpaceStatus).ToString();
                tranData.EnforcementKey = (data.EnforcementKey == null) ? string.Empty : data.EnforcementKey;
                tranData.MeterStatus = (data.MeterStatus == null) ? null : data.MeterStatus;

                tranData.METERLOC_METERTYPE = (data.METERLOC_METERTYPE == null) ? null : data.METERLOC_METERTYPE;
                tranData.ZoneName = (data.ZoneName == null) ? null : data.ZoneName;

                tranData.MeterID = data.PayStationID.HasValue ? data.PayStationID.Value.ToString() : "0";
                tranData.ReturnCode = data.ReturnCode;

                logData.Response = new ResponseEntity() { CustomerId = param.CustomerId.Value.ToString(), MeterId = tranData.MeterName, SpaceId = tranData.SpaceNo, CityCurrentTime = data.PresentMeterTime.HasValue ? data.PresentMeterTime.Value : DateTime.Now };
            }
            catch (Exception ex)
            {
                //var appPath = System.Web.Hosting.HostingEnvironment.ApplicationPhysicalPath;
                //string errorPath = Path.Combine(appPath, "GetTransformData", "Timezones");
                //ExceptionLogging.SendErrorToText(ex, "GetCitationData", errorPath);
                //tranData.ReturnCode = ((int)ReturnCodeEnum.Error).ToString();
                tranData.ReturnCode = ((int)ReturnCodeEnum.Error).ToString();
            }
            #endregion
            #region T2Digital
            TransactionTransform tranDataOfServiceT2 = new TransactionTransform();
            try
            {
                //////////////////////////////////////////
                logData.Request.CustomerId = param.CustomerId.Value.ToString();
                logData.Request.SpaceId = param.parkingSpaceID;
                logData.Request.MeterId = param.MeterName;
                //////////////////////////////////////////////////
                DateTime? currentCustTime = customerCurrentTime.CurrentTime;
                T2Digital.T2DigitalCustomerElement customer = T2Digital.T2DigitalCustomerRetriever.GetCustomer(param.CustomerId.Value);
                T2Digital.T2DigitalGenericFactory factory = new T2Digital.T2DigitalGenericFactory(customer, Convert.ToInt32(param.parkingSpaceID));
                tranDataOfServiceT2 = factory.GetRefreshStallNo();
                tranDataOfServiceT2.EnforcementKey = EnforcementKey;

                ///////////////////////////////////////////////////
                logData.Response = new ResponseEntity() { CustomerId = param.CustomerId.Value.ToString(), MeterId = tranData.MeterName, SpaceId = tranData.SpaceNo, CityCurrentTime = currentCustTime.HasValue ? currentCustTime.Value : DateTime.Now };

            }
            catch (Exception ex)
            {
                logData.Response = new ResponseEntity() { CustomerId = param.CustomerId.Value.ToString(), MeterId = tranData.MeterName, SpaceId = tranData.SpaceNo, CityCurrentTime = customerCurrentTime.CurrentTime.HasValue ? customerCurrentTime.CurrentTime.Value : DateTime.Now };
                //throw ex;
                tranData.ReturnCode = ((int)ReturnCodeEnum.Error).ToString();
            }
            finally
            {

            }
            #endregion
            List<TransactionTransform> trans = new List<TransactionTransform>();
            trans.Add(tranData); // Database
            trans.Add(tranDataOfServiceT2); //T2Digital


            var rightValue = trans.Where(t => t.ReturnCode == ((int)ReturnCodeEnum.Success).ToString()).OrderByDescending(t => t.MeterExpiredMinutes.ToNullableInt()).FirstOrDefault<TransactionTransform>();
            if (rightValue == default(TransactionTransform))
            {
                //Do nothing.just return database  object.
            }
            else
            {
                tranData = rightValue;
            }

            return tranData;
        }
        public static TransactionTransform GetSouthMiamiTransformDataV3(string EnforcementKey, string filePath, RequestResponseEntity logData, CustomerTime customerCurrentTime)
        {
            TransactionTransform tranData = new TransactionTransform();
            tranData.ReturnCode = ((int)ReturnCodeEnum.InvalidInputParameters).ToString();
            TimeZoneInfo TZ = null;
            TZ = TimeZoneInfo.FindSystemTimeZoneById("Eastern Standard Time");
            //int? customerID = null;
            //string MeterName = null;
            //string parkingSpaceID;
            //string PlateNumber = null;
            //string stateName = null;
            //////////////////////////////////////
            KeyParameter param = PrepareKeyParameters.PrepareSouthMiami(EnforcementKey);
            if (param.ReturnCode == ((int)ReturnCodeEnum.InvalidInputParameters).ToString())
            {
                return tranData;
            }

            ///Special case //As specified by Alan
            ///When the client is sending alphanumeric space numbers for validation, “61A610” etc, the service is responding with an response code of “-1” and error text  “Specified cast is not valid”
            //Please update this exception handling to instead return a response code of “0” and an expired minutes of “-1”
            if (HandleSepecialSpace(param, EnforcementKey, ref tranData, ref logData, TZ))
                return tranData;
            /////////////end special Case/////////////////
            /////////////////////////////////////
            #region DB
            try
            {
                Transaction data = TransactionDataValidatioin.GetDataSouthMiami(param.CustomerId.Value, param.MeterName, param.parkingSpaceID, param.PlateNumber, param.StateName, EnforcementKey, filePath, logData);


                if (data.TimeZoneId.HasValue)
                    TZ = Utility.GetTimeZoneInfo(data.TimeZoneId.Value, filePath, param.CustomerId.Value.ToString());

                //For South Miami : PayStationId. Others it is MeterID
                tranData.MeterName = (data.MeterName == null) ? string.Empty : data.MeterName;
                tranData.SpaceNo = data.ParkingSpaceID.ToString();

                tranData.MeterStreet = (data.MeterStreet == null) ? string.Empty : data.MeterStreet;

                DateTime _MeterRTC = data.PresentMeterTime.HasValue ? data.PresentMeterTime.Value : DateTime.Now;
                tranData.MeterRTC = String.Format("{0:yyyyMMdd_THHmmssZ}", Utility.findGMTTime(_MeterRTC, TZ));

                DateTime _SensorEventTime = data.SensorEventTime.HasValue ? data.SensorEventTime.Value : DateTime.Now;
                tranData.MeterLastUpdateTime = String.Format("{0:yyyyMMdd_THHmmssZ}", Utility.findGMTTime(_SensorEventTime, TZ));

                tranData.SensorEventTime = String.Format("{0:yyyyMMdd_THHmmssZ}", Utility.findGMTTime(_SensorEventTime, TZ));

                DateTime _endTime = data.ExpiryTime.HasValue ? data.ExpiryTime.Value : DateTime.Now;
                tranData.ExpiredTime = String.Format("{0:yyyyMMdd_THHmmssZ}", Utility.findGMTTime(_endTime, TZ));

                tranData.MeterExpiredMinutes = data.ExpiredMinutes.HasValue ? data.ExpiredMinutes.Value.ToString() : "0";
                tranData.METERLOC_BLOCKNUMBER = (data.Block == null) ? string.Empty : data.Block;
                tranData.METERLOC_STREETDIRECTION = (data.Direction == null) ? string.Empty : data.Direction;
                tranData.METERLOC_STREETNAME = (data.MeterStreet == null) ? string.Empty : data.MeterStreet;
                tranData.METERLOC_STREETTYPE = (data.StreetType == null) ? string.Empty : data.StreetType;
                tranData.METERLOC_CROSSSTREET1 = (data.CrossStreet1 == null) ? string.Empty : data.CrossStreet1;
                tranData.METERLOC_CROSSSTREET2 = (data.CrossStreet2 == null) ? string.Empty : data.CrossStreet2;
                tranData.METERLOC_ENFORCEMENTHOURSDESC = (data.EnfHourDesc == null) ? string.Empty : data.EnfHourDesc;
                tranData.LicensePlate = (data.PlateNumber == null) ? string.Empty : data.PlateNumber;
                tranData.State = (data.LPLocation == null) ? string.Empty : data.LPLocation;
                tranData.Make = (data.VMake == null) ? string.Empty : data.VMake;
                tranData.Model = (data.VModel == null) ? string.Empty : data.VModel;
                tranData.IsOccupied = Utility.getStatus((data.SpaceStatus == null) ? string.Empty : data.SpaceStatus).ToString();
                tranData.EnforcementKey = (data.EnforcementKey == null) ? string.Empty : data.EnforcementKey;
                tranData.MeterStatus = (data.MeterStatus == null) ? null : data.MeterStatus;

                tranData.METERLOC_METERTYPE = (data.METERLOC_METERTYPE == null) ? null : data.METERLOC_METERTYPE;
                tranData.ZoneName = (data.ZoneName == null) ? null : data.ZoneName;

                tranData.MeterID = data.PayStationID.HasValue ? data.PayStationID.Value.ToString() : "0";
                tranData.ReturnCode = data.ReturnCode;

                logData.Response = new ResponseEntity() { CustomerId = param.CustomerId.Value.ToString(), MeterId = tranData.MeterName, SpaceId = tranData.SpaceNo, CityCurrentTime = data.PresentMeterTime.HasValue ? data.PresentMeterTime.Value : DateTime.Now };
            }
            catch (Exception ex)
            {
                //var appPath = System.Web.Hosting.HostingEnvironment.ApplicationPhysicalPath;
                //string errorPath = Path.Combine(appPath, "GetTransformData", "Timezones");
                //ExceptionLogging.SendErrorToText(ex, "GetCitationData", errorPath);
                //tranData.ReturnCode = ((int)ReturnCodeEnum.Error).ToString();
                tranData.ReturnCode = ((int)ReturnCodeEnum.Error).ToString();
            }
            #endregion
            #region T2Digital
            TransactionTransform tranDataOfServiceT2 = new TransactionTransform();
            try
            {
                //////////////////////////////////////////
                logData.Request.CustomerId = param.CustomerId.Value.ToString();
                logData.Request.SpaceId = param.parkingSpaceID;
                logData.Request.MeterId = param.MeterName;
                //////////////////////////////////////////////////
                DateTime? currentCustTime = customerCurrentTime.CurrentTime;
                T2Digital.T2DigitalCustomerElement customer = T2Digital.T2DigitalCustomerRetriever.GetCustomer(param.CustomerId.Value);
                T2Digital.T2DigitalGenericFactory factory = new T2Digital.T2DigitalGenericFactory(customer, Convert.ToInt32(param.parkingSpaceID));
                var t2ResLog = new VendorResponseEntity();
                logData.VendorData.Add(VendorNames.T2Digital, t2ResLog);
                tranDataOfServiceT2 = factory.GetRefreshStallNo(ref t2ResLog);
                tranDataOfServiceT2.EnforcementKey = EnforcementKey;

                ///////////////////////////////////////////////////
                logData.Response = new ResponseEntity() { CustomerId = param.CustomerId.Value.ToString(), MeterId = tranData.MeterName, SpaceId = tranData.SpaceNo, CityCurrentTime = currentCustTime.HasValue ? currentCustTime.Value : DateTime.Now };

            }
            catch (Exception ex)
            {
                logData.Response = new ResponseEntity() { CustomerId = param.CustomerId.Value.ToString(), MeterId = tranData.MeterName, SpaceId = tranData.SpaceNo, CityCurrentTime = customerCurrentTime.CurrentTime.HasValue ? customerCurrentTime.CurrentTime.Value : DateTime.Now };
                //throw ex;
                tranData.ReturnCode = ((int)ReturnCodeEnum.Error).ToString();
            }
            finally
            {

            }
            #endregion
            List<TransactionTransform> trans = new List<TransactionTransform>();
            trans.Add(tranData); // Database
            trans.Add(tranDataOfServiceT2); //T2Digital


            var rightValue = trans.Where(t => t.ReturnCode == ((int)ReturnCodeEnum.Success).ToString()).OrderByDescending(t => t.MeterExpiredMinutes.ToNullableInt()).FirstOrDefault<TransactionTransform>();
            if (rightValue == default(TransactionTransform))
            {
                //Do nothing.just return database  object.
            }
            else
            {
                tranData = rightValue;
            }

            return tranData;
        }

        public static TransactionTransform GetSouthMiamiTransformSpacePlateV1(string EnforcementKey, string filePath, RequestResponseEntity logData, CustomerTime customerCurrentTime, EnumEnforcementType typeOfEnf)
        {
            TransactionTransform tranData = new TransactionTransform();
            tranData.ReturnCode = ((int)ReturnCodeEnum.InvalidInputParameters).ToString();
            TimeZoneInfo TZ = null;
            TZ = TimeZoneInfo.FindSystemTimeZoneById("Eastern Standard Time");
            //int? customerID = null;
            //string MeterName = null;
            //string parkingSpaceID;
            //string PlateNumber = null;
            //string stateName = null;
            //////////////////////////////////////
            KeyParameter param = PrepareKeyParameters.PrepareSouthMiami(EnforcementKey);
            if (param.ReturnCode == ((int)ReturnCodeEnum.InvalidInputParameters).ToString())
            {
                return tranData;
            }

            ///Special case //As specified by Alan
            ///When the client is sending alphanumeric space numbers for validation, “61A610” etc, the service is responding with an response code of “-1” and error text  “Specified cast is not valid”
            //Please update this exception handling to instead return a response code of “0” and an expired minutes of “-1”
            if (typeOfEnf == EnumEnforcementType.PayBySpace)
            {
                if (HandleSepecialSpace(param, EnforcementKey, ref tranData, ref logData, TZ))
                    return tranData;
            }
            /////////////end special Case/////////////////
            /////////////////////////////////////
            #region DB
            try
            {
                Transaction data = TransactionDataValidatioin.GetDataSouthMiami(param.CustomerId.Value, param.MeterName, param.parkingSpaceID, param.PlateNumber, param.StateName, EnforcementKey, filePath, logData, typeOfEnf);


                if (data.TimeZoneId.HasValue)
                    TZ = Utility.GetTimeZoneInfo(data.TimeZoneId.Value, filePath, param.CustomerId.Value.ToString());

                //For South Miami : PayStationId. Others it is MeterID
                tranData.MeterName = (data.MeterName == null) ? string.Empty : data.MeterName;
                tranData.SpaceNo = data.ParkingSpaceID.ToString();

                tranData.MeterStreet = (data.MeterStreet == null) ? string.Empty : data.MeterStreet;

                DateTime _MeterRTC = data.PresentMeterTime.HasValue ? data.PresentMeterTime.Value : DateTime.Now;
                tranData.MeterRTC = String.Format("{0:yyyyMMdd_THHmmssZ}", Utility.findGMTTime(_MeterRTC, TZ));

                DateTime _SensorEventTime = data.SensorEventTime.HasValue ? data.SensorEventTime.Value : DateTime.Now;
                tranData.MeterLastUpdateTime = String.Format("{0:yyyyMMdd_THHmmssZ}", Utility.findGMTTime(_SensorEventTime, TZ));

                tranData.SensorEventTime = String.Format("{0:yyyyMMdd_THHmmssZ}", Utility.findGMTTime(_SensorEventTime, TZ));

                DateTime _endTime = data.ExpiryTime.HasValue ? data.ExpiryTime.Value : DateTime.Now;
                tranData.ExpiredTime = String.Format("{0:yyyyMMdd_THHmmssZ}", Utility.findGMTTime(_endTime, TZ));

                tranData.MeterExpiredMinutes = data.ExpiredMinutes.HasValue ? data.ExpiredMinutes.Value.ToString() : "0";
                tranData.METERLOC_BLOCKNUMBER = (data.Block == null) ? string.Empty : data.Block;
                tranData.METERLOC_STREETDIRECTION = (data.Direction == null) ? string.Empty : data.Direction;
                tranData.METERLOC_STREETNAME = (data.MeterStreet == null) ? string.Empty : data.MeterStreet;
                tranData.METERLOC_STREETTYPE = (data.StreetType == null) ? string.Empty : data.StreetType;
                tranData.METERLOC_CROSSSTREET1 = (data.CrossStreet1 == null) ? string.Empty : data.CrossStreet1;
                tranData.METERLOC_CROSSSTREET2 = (data.CrossStreet2 == null) ? string.Empty : data.CrossStreet2;
                tranData.METERLOC_ENFORCEMENTHOURSDESC = (data.EnfHourDesc == null) ? string.Empty : data.EnfHourDesc;
                tranData.LicensePlate = (data.PlateNumber == null) ? string.Empty : data.PlateNumber;
                tranData.State = (data.LPLocation == null) ? string.Empty : data.LPLocation;
                tranData.Make = (data.VMake == null) ? string.Empty : data.VMake;
                tranData.Model = (data.VModel == null) ? string.Empty : data.VModel;
                tranData.IsOccupied = Utility.getStatus((data.SpaceStatus == null) ? string.Empty : data.SpaceStatus).ToString();
                tranData.EnforcementKey = (data.EnforcementKey == null) ? string.Empty : data.EnforcementKey;
                tranData.MeterStatus = (data.MeterStatus == null) ? null : data.MeterStatus;

                tranData.METERLOC_METERTYPE = (data.METERLOC_METERTYPE == null) ? null : data.METERLOC_METERTYPE;
                tranData.ZoneName = (data.ZoneName == null) ? null : data.ZoneName;

                tranData.MeterID = data.PayStationID.HasValue ? data.PayStationID.Value.ToString() : "0";
                tranData.ReturnCode = data.ReturnCode;

                logData.Response = new ResponseEntity() { CustomerId = param.CustomerId.Value.ToString(), MeterId = tranData.MeterName, SpaceId = tranData.SpaceNo, CityCurrentTime = data.PresentMeterTime.HasValue ? data.PresentMeterTime.Value : DateTime.Now };
            }
            catch (Exception ex)
            {
                //var appPath = System.Web.Hosting.HostingEnvironment.ApplicationPhysicalPath;
                //string errorPath = Path.Combine(appPath, "GetTransformData", "Timezones");
                //ExceptionLogging.SendErrorToText(ex, "GetCitationData", errorPath);
                //tranData.ReturnCode = ((int)ReturnCodeEnum.Error).ToString();
                tranData.ReturnCode = ((int)ReturnCodeEnum.Error).ToString();
            }
            #endregion
            #region T2Digital
            TransactionTransform tranDataOfServiceT2 = new TransactionTransform();
            try
            {
                //////////////////////////////////////////
                logData.Request.CustomerId = param.CustomerId.Value.ToString();
                logData.Request.SpaceId = param.parkingSpaceID;
                logData.Request.MeterId = param.MeterName;
                //////////////////////////////////////////////////
                DateTime? currentCustTime = customerCurrentTime.CurrentTime;
                T2Digital.T2DigitalCustomerElement customer = T2Digital.T2DigitalCustomerRetriever.GetCustomer(param.CustomerId.Value);
                var t2ResLog = new VendorResponseEntity();
                logData.VendorData.Add(VendorNames.T2Digital, t2ResLog);
                if (typeOfEnf == EnumEnforcementType.PayBySpace)
                {
                    T2Digital.T2DigitalGenericFactory factory = new T2Digital.T2DigitalGenericFactory(customer, Convert.ToInt32(param.parkingSpaceID));
                    tranDataOfServiceT2 = factory.GetRefreshStallNo(ref t2ResLog);
                }
                else if (typeOfEnf == EnumEnforcementType.PayByPlate)
                {
                    T2Digital.T2DigitalCustomerElement customer1 = T2Digital.T2DigitalCustomerRetriever.GetCustomer(700101);
                    T2Digital.T2DigitalGenericFactory factory = new T2Digital.T2DigitalGenericFactory(customer1, param.PlateNumber, string.Empty);
                    tranDataOfServiceT2 = factory.GetRefreshPlateData(ref t2ResLog);
                }
                tranDataOfServiceT2.EnforcementKey = EnforcementKey;

                ///////////////////////////////////////////////////
                logData.Response = new ResponseEntity() { CustomerId = param.CustomerId.Value.ToString(), MeterId = tranData.MeterName, SpaceId = tranData.SpaceNo, CityCurrentTime = currentCustTime.HasValue ? currentCustTime.Value : DateTime.Now };

            }
            catch (Exception ex)
            {
                logData.Response = new ResponseEntity() { CustomerId = param.CustomerId.Value.ToString(), MeterId = tranData.MeterName, SpaceId = tranData.SpaceNo, CityCurrentTime = customerCurrentTime.CurrentTime.HasValue ? customerCurrentTime.CurrentTime.Value : DateTime.Now };
                //throw ex;
                tranData.ReturnCode = ((int)ReturnCodeEnum.Error).ToString();
            }
            finally
            {

            }
            #endregion
            List<TransactionTransform> trans = new List<TransactionTransform>();
            trans.Add(tranData); // Database
            trans.Add(tranDataOfServiceT2); //T2Digital


            var rightValue = trans.Where(t => t.ReturnCode == ((int)ReturnCodeEnum.Success).ToString()).OrderByDescending(t => t.MeterExpiredMinutes.ToNullableInt()).FirstOrDefault<TransactionTransform>();
            if (rightValue == default(TransactionTransform))
            {
                //Do nothing.just return database  object.
            }
            else
            {
                tranData = rightValue;
            }

            return tranData;
        }

        public static TransactionTransform GetSouthMiamiTransformDataFinal(string EnforcementKey, string filePath, RequestResponseEntity logData, CustomerTime customerCurrentTime)
        {
            TransactionTransform tranData = new TransactionTransform();
            tranData.ReturnCode = ((int)ReturnCodeEnum.InvalidInputParameters).ToString();
            TimeZoneInfo TZ = null;
            TZ = TimeZoneInfo.FindSystemTimeZoneById("Eastern Standard Time");

            EnumEnforcementType typeOfEnf = PrepareKeyParameters.GetEnforceType(EnforcementKey);
            if (typeOfEnf == EnumEnforcementType.InValid)
                return tranData;
            tranData = TransactionDataValidatioin.GetSouthMiamiTransformSpacePlateV1(EnforcementKey, filePath, logData, customerCurrentTime, typeOfEnf);
            return tranData;
        }

        private static Transaction GetDataSouthMiami(int customerID, string MeterName, string parkingSpaceID, string PlateNumber, string statename, string EnforcementKey, string filePath, RequestResponseEntity logData)
        {
            string ZoneID = null;
            string SpaceStatus = null;
            Transaction data = new Transaction();
            try
            {
                List<Transaction> transactionList = new List<Transaction>();

                logData.Request.CustomerId = customerID.ToString();
                logData.Request.SpaceId = parkingSpaceID;
                logData.Request.MeterId = MeterName;

                //transactionList = DataAccess.GetSouthMiamiCitationData(customerID, null, null, null, PlateNumber, statename, null);
                transactionList = DataAccess.GetSouthMiamiCitationData(customerID, null, MeterName, parkingSpaceID, null, null);

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
            }
            catch (Exception ex)
            {
                var appPath = System.Web.Hosting.HostingEnvironment.ApplicationPhysicalPath;
                string errorPath = Path.Combine(appPath, "GetData", "Timezones");
                ExceptionLogging.SendErrorToText(ex, "GetCitationData", errorPath);
                data.ReturnCode = ((int)ReturnCodeEnum.Error).ToString();
                throw;
            }
            return data;
        }

        private static Transaction GetDataSouthMiami(int customerID, string MeterName, string parkingSpaceID, string PlateNumber, string statename, string EnforcementKey, string filePath, RequestResponseEntity logData, EnumEnforcementType typeOfEnf)
        {
            string ZoneID = null;
            string SpaceStatus = null;
            Transaction data = new Transaction();
            try
            {
                List<Transaction> transactionList = new List<Transaction>();

                logData.Request.CustomerId = customerID.ToString();
                logData.Request.SpaceId = parkingSpaceID;
                logData.Request.MeterId = MeterName;

                //transactionList = DataAccess.GetSouthMiamiCitationData(customerID, null, null, null, PlateNumber, statename, null);
                if (typeOfEnf == EnumEnforcementType.PayBySpace)
                    transactionList = DataAccess.GetSouthMiamiCitationData(customerID, null, MeterName, parkingSpaceID, null, null);
                else if (typeOfEnf == EnumEnforcementType.PayByPlate)
                    transactionList = DataAccess.GetSouthMiamiCitationData(customerID, null, null, null, PlateNumber, null);

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
            }
            catch (Exception ex)
            {
                var appPath = System.Web.Hosting.HostingEnvironment.ApplicationPhysicalPath;
                string errorPath = Path.Combine(appPath, "GetData", "Timezones");
                ExceptionLogging.SendErrorToText(ex, "GetCitationData", errorPath);
                data.ReturnCode = ((int)ReturnCodeEnum.Error).ToString();
                throw;
            }
            return data;
        }
        private static bool HandleSepecialSpace(KeyParameter param, string EnforcementKey, ref TransactionTransform tranData, ref RequestResponseEntity logData, TimeZoneInfo TZ)
        {
            bool isSpecialCase = false;
            int count = Regex.Matches(param.parkingSpaceID, @"[a-zA-Z]").Count;
            if (count > 0)
            {
                isSpecialCase = true;
            }

            if (isSpecialCase == false)
            {
                var excludeAssetSection = System.Configuration.ConfigurationManager.GetSection("ExcludeAssetSection") as NameValueCollection;
                if (excludeAssetSection != null)
                {
                    string assets = excludeAssetSection[param.CustomerId.Value.ToString()].ToString();
                    if (assets.Split(',').ToList().Contains(param.parkingSpaceID))
                    {
                        isSpecialCase = true;
                    }
                }
            }

            if (isSpecialCase == true)
            {
                //DateTime _MeterRTC =  DateTime.Now;
                ////tranData.MeterRTC = String.Format("{0:yyyyMMdd_THHmmssZ}", Utility.findGMTTime(_MeterRTC, TZ));
                //tranData.ExpiredTime = String.Format("{0:yyyyMMdd_THHmmssZ}", Utility.findGMTTime(_MeterRTC, TZ));
                ////tranData.MeterLastUpdateTime = String.Format("{0:yyyyMMdd_THHmmssZ}", Utility.findGMTTime(_MeterRTC, TZ));
                //tranData.SensorEventTime = String.Format("{0:yyyyMMdd_THHmmssZ}", Utility.findGMTTime(_MeterRTC, TZ));

                tranData.ExpiredTime = "-1";
                tranData.ReturnCode = ((int)ReturnCodeEnum.Success).ToString();
                tranData.SpaceNo = param.parkingSpaceID;
                tranData.MeterName = param.MeterName;
                tranData.EnforcementKey = EnforcementKey;
                tranData.MeterExpiredMinutes = "-1";
                logData.Response = new ResponseEntity() { CustomerId = param.CustomerId.Value.ToString(), MeterId = param.MeterName, SpaceId = param.parkingSpaceID };
                logData.Request.CustomerId = param.CustomerId.ToString();
                logData.Request.SpaceId = param.parkingSpaceID;
                logData.Request.MeterId = param.MeterName; ;
            }
            return isSpecialCase;
        }
        #endregion

        #region Raleigh
        public static TransactionTransform GetRaleighTransformData(string EnforcementKey, string filePath, RequestResponseEntity logData)
        {
            TransactionTransform tranData = new TransactionTransform();
            tranData.ReturnCode = ((int)ReturnCodeEnum.InvalidInputParameters).ToString();
            TimeZoneInfo TZ = null;
            TZ = TimeZoneInfo.FindSystemTimeZoneById("Eastern Standard Time");
            //int? customerID = null;
            //string MeterName = null;
            //string parkingSpaceID;
            //string PlateNumber = null;
            //string stateName = null;
            //////////////////////////////////////
            KeyParameter param = PrepareKeyParameters.PrepareRaleigh(EnforcementKey);
            if (param.ReturnCode == ((int)ReturnCodeEnum.InvalidInputParameters).ToString())
            {
                return tranData;
            }
            /////////////////////////////////////
            try
            {
                Transaction data = TransactionDataValidatioin.GetDataRaleigh(param.CustomerId.Value, param.MeterName, param.parkingSpaceID, param.PlateNumber, param.StateName, EnforcementKey, filePath, logData);


                if (data.TimeZoneId.HasValue)
                    TZ = Utility.GetTimeZoneInfo(data.TimeZoneId.Value, filePath, param.CustomerId.Value.ToString());

                //For South Miami : PayStationId. Others it is MeterID
                tranData.MeterName = (data.MeterName == null) ? string.Empty : data.MeterName;
                tranData.SpaceNo = data.ParkingSpaceID.ToString();

                tranData.MeterStreet = (data.MeterStreet == null) ? string.Empty : data.MeterStreet;

                DateTime _MeterRTC = data.PresentMeterTime.HasValue ? data.PresentMeterTime.Value : DateTime.Now;
                tranData.MeterRTC = String.Format("{0:yyyyMMdd_THHmmssZ}", Utility.findGMTTime(_MeterRTC, TZ));

                DateTime _SensorEventTime = data.SensorEventTime.HasValue ? data.SensorEventTime.Value : DateTime.Now;
                tranData.MeterLastUpdateTime = String.Format("{0:yyyyMMdd_THHmmssZ}", Utility.findGMTTime(_SensorEventTime, TZ));

                tranData.SensorEventTime = String.Format("{0:yyyyMMdd_THHmmssZ}", Utility.findGMTTime(_SensorEventTime, TZ));

                DateTime _endTime = data.ExpiryTime.HasValue ? data.ExpiryTime.Value : DateTime.Now;
                tranData.ExpiredTime = String.Format("{0:yyyyMMdd_THHmmssZ}", Utility.findGMTTime(_endTime, TZ));

                tranData.MeterExpiredMinutes = data.ExpiredMinutes.HasValue ? data.ExpiredMinutes.Value.ToString() : "0";
                tranData.METERLOC_BLOCKNUMBER = (data.Block == null) ? string.Empty : data.Block;
                tranData.METERLOC_STREETDIRECTION = (data.Direction == null) ? string.Empty : data.Direction;
                tranData.METERLOC_STREETNAME = (data.MeterStreet == null) ? string.Empty : data.MeterStreet;
                tranData.METERLOC_STREETTYPE = (data.StreetType == null) ? string.Empty : data.StreetType;
                tranData.METERLOC_CROSSSTREET1 = (data.CrossStreet1 == null) ? string.Empty : data.CrossStreet1;
                tranData.METERLOC_CROSSSTREET2 = (data.CrossStreet2 == null) ? string.Empty : data.CrossStreet2;
                tranData.METERLOC_ENFORCEMENTHOURSDESC = (data.EnfHourDesc == null) ? string.Empty : data.EnfHourDesc;
                tranData.LicensePlate = (data.PlateNumber == null) ? string.Empty : data.PlateNumber;
                tranData.State = (data.LPLocation == null) ? string.Empty : data.LPLocation;
                tranData.Make = (data.VMake == null) ? string.Empty : data.VMake;
                tranData.Model = (data.VModel == null) ? string.Empty : data.VModel;
                tranData.IsOccupied = Utility.getStatus((data.SpaceStatus == null) ? string.Empty : data.SpaceStatus).ToString();
                tranData.EnforcementKey = (data.EnforcementKey == null) ? string.Empty : data.EnforcementKey;
                tranData.MeterStatus = (data.MeterStatus == null) ? null : data.MeterStatus;

                tranData.METERLOC_METERTYPE = (data.METERLOC_METERTYPE == null) ? null : data.METERLOC_METERTYPE;
                tranData.ZoneName = (data.ZoneName == null) ? null : data.ZoneName;

                tranData.MeterID = data.PayStationID.HasValue ? data.PayStationID.Value.ToString() : "0";
                tranData.ReturnCode = data.ReturnCode;

                logData.Response = new ResponseEntity() { CustomerId = param.CustomerId.Value.ToString(), MeterId = tranData.MeterName, SpaceId = tranData.SpaceNo, CityCurrentTime = data.PresentMeterTime.HasValue ? data.PresentMeterTime.Value : DateTime.Now };
            }
            catch (Exception ex)
            {
                var appPath = System.Web.Hosting.HostingEnvironment.ApplicationPhysicalPath;
                string errorPath = Path.Combine(appPath, "GetTransformData", "Timezones");
                ExceptionLogging.SendErrorToText(ex, "GetCitationData", errorPath);
                tranData.ReturnCode = ((int)ReturnCodeEnum.Error).ToString();

            }
            return tranData;
        }
        private static Transaction GetDataRaleigh(int customerID, string MeterName, string parkingSpaceID, string PlateNumber, string statename, string EnforcementKey, string filePath, RequestResponseEntity logData)
        {
            string ZoneID = null;
            string SpaceStatus = null;
            Transaction data = new Transaction();
            try
            {
                List<Transaction> transactionList = new List<Transaction>();

                logData.Request.CustomerId = customerID.ToString();
                logData.Request.SpaceId = parkingSpaceID;
                logData.Request.MeterId = MeterName;

                //transactionList = DataAccess.GetSouthMiamiCitationData(customerID, null, null, null, PlateNumber, statename, null);
                transactionList = DataAccess.GetCitationDataRaleigh(customerID, null, MeterName, parkingSpaceID, null, null);

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
            }
            catch (Exception ex)
            {
                var appPath = System.Web.Hosting.HostingEnvironment.ApplicationPhysicalPath;
                string errorPath = Path.Combine(appPath, "GetData", "Timezones");
                ExceptionLogging.SendErrorToText(ex, "GetCitationData", errorPath);
                data.ReturnCode = ((int)ReturnCodeEnum.Error).ToString();
                throw;
            }
            return data;
        }
        #endregion

        #region Birmingham, MI
        public static TransactionTransform GetBirminghamMITransformData(string EnforcementKey, string filePath, RequestResponseEntity logData)
        {
            TransactionTransform tranData = new TransactionTransform();
            tranData.ReturnCode = ((int)ReturnCodeEnum.InvalidInputParameters).ToString();
            TimeZoneInfo TZ = null;
            TZ = TimeZoneInfo.FindSystemTimeZoneById("Eastern Standard Time");
            //int? customerID = null;
            //string MeterName = null;
            //string parkingSpaceID;
            //string PlateNumber = null;
            //string stateName = null;
            //////////////////////////////////////
            KeyParameter param = PrepareKeyParameters.PrepareBirminghamMI(EnforcementKey);
            if (param.ReturnCode == ((int)ReturnCodeEnum.InvalidInputParameters).ToString())
            {
                return tranData;
            }
            /////////////////////////////////////
            try
            {
                Transaction data = TransactionDataValidatioin.GetDataBirminghamMI(param.CustomerId.Value, param.MeterName, param.parkingSpaceID, param.PlateNumber, param.StateName, EnforcementKey, filePath, logData);


                if (data.TimeZoneId.HasValue)
                    TZ = Utility.GetTimeZoneInfo(data.TimeZoneId.Value, filePath, param.CustomerId.Value.ToString());

                /////////////////////
                var meternumberAndSpaceNo = (data.MeterName == null) ? string.Empty.Split('-') : data.MeterName.Split('-');
                if (meternumberAndSpaceNo.Length > 0)
                    tranData.MeterName = meternumberAndSpaceNo[0];
                else
                    tranData.MeterName = string.Empty;

                if (meternumberAndSpaceNo.Length > 1)
                    tranData.SpaceNo = meternumberAndSpaceNo[1];
                else
                    tranData.SpaceNo = string.Empty;

                tranData.MeterStreet = (data.MeterStreet == null) ? string.Empty : data.MeterStreet;

                DateTime _MeterRTC = data.PresentMeterTime.HasValue ? data.PresentMeterTime.Value : DateTime.Now;
                tranData.MeterRTC = String.Format("{0:yyyyMMdd_THHmmssZ}", Utility.findGMTTime(_MeterRTC, TZ));

                DateTime _SensorEventTime = data.SensorEventTime.HasValue ? data.SensorEventTime.Value : DateTime.Now;
                tranData.MeterLastUpdateTime = String.Format("{0:yyyyMMdd_THHmmssZ}", Utility.findGMTTime(_SensorEventTime, TZ));

                tranData.SensorEventTime = String.Format("{0:yyyyMMdd_THHmmssZ}", Utility.findGMTTime(_SensorEventTime, TZ));

                DateTime _endTime = data.ExpiryTime.HasValue ? data.ExpiryTime.Value : DateTime.Now;
                tranData.ExpiredTime = String.Format("{0:yyyyMMdd_THHmmssZ}", Utility.findGMTTime(_endTime, TZ));

                tranData.MeterExpiredMinutes = data.ExpiredMinutes.HasValue ? data.ExpiredMinutes.Value.ToString() : "0";
                tranData.METERLOC_BLOCKNUMBER = (data.Block == null) ? string.Empty : data.Block;
                tranData.METERLOC_STREETDIRECTION = (data.Direction == null) ? string.Empty : data.Direction;
                tranData.METERLOC_STREETNAME = (data.MeterStreet == null) ? string.Empty : data.MeterStreet;
                tranData.METERLOC_STREETTYPE = (data.StreetType == null) ? string.Empty : data.StreetType;
                tranData.METERLOC_CROSSSTREET1 = (data.CrossStreet1 == null) ? string.Empty : data.CrossStreet1;
                tranData.METERLOC_CROSSSTREET2 = (data.CrossStreet2 == null) ? string.Empty : data.CrossStreet2;
                tranData.METERLOC_ENFORCEMENTHOURSDESC = (data.EnfHourDesc == null) ? string.Empty : data.EnfHourDesc;
                tranData.LicensePlate = (data.PlateNumber == null) ? string.Empty : data.PlateNumber;
                tranData.State = (data.LPLocation == null) ? string.Empty : data.LPLocation;
                tranData.Make = (data.VMake == null) ? string.Empty : data.VMake;
                tranData.Model = (data.VModel == null) ? string.Empty : data.VModel;
                tranData.IsOccupied = Utility.getStatus((data.SpaceStatus == null) ? string.Empty : data.SpaceStatus).ToString();
                tranData.EnforcementKey = (data.EnforcementKey == null) ? string.Empty : data.EnforcementKey;
                tranData.MeterStatus = (data.MeterStatus == null) ? null : data.MeterStatus;

                tranData.METERLOC_METERTYPE = (data.METERLOC_METERTYPE == null) ? null : data.METERLOC_METERTYPE;
                tranData.ZoneName = (data.ZoneName == null) ? null : data.ZoneName;

                tranData.MeterID = data.PayStationID.HasValue ? data.PayStationID.Value.ToString() : "0";
                tranData.ReturnCode = data.ReturnCode;

                logData.Response = new ResponseEntity() { CustomerId = param.CustomerId.Value.ToString(), MeterId = tranData.MeterName, SpaceId = tranData.SpaceNo, CityCurrentTime = data.PresentMeterTime.HasValue ? data.PresentMeterTime.Value : DateTime.Now };
            }
            catch (Exception ex)
            {
                var appPath = System.Web.Hosting.HostingEnvironment.ApplicationPhysicalPath;
                string errorPath = Path.Combine(appPath, "GetTransformData", "Timezones");
                ExceptionLogging.SendErrorToText(ex, "GetCitationData", errorPath);
                tranData.ReturnCode = ((int)ReturnCodeEnum.Error).ToString();

            }
            return tranData;
        }
        public static TransactionTransform GetBirminghamMITransformDataNew(string EnforcementKey, string filePath, RequestResponseEntity logData, CustomerTime customerCurrentTime)
        {
            TransactionTransform tranData = new TransactionTransform();
            tranData.ReturnCode = ((int)ReturnCodeEnum.InvalidInputParameters).ToString();
            TimeZoneInfo TZ = null;
            TZ = TimeZoneInfo.FindSystemTimeZoneById("Eastern Standard Time");
            //int? customerID = null;
            //string MeterName = null;
            //string parkingSpaceID;
            //string PlateNumber = null;
            //string stateName = null;
            //////////////////////////////////////
            KeyParameter param = PrepareKeyParameters.PrepareBirminghamMI(EnforcementKey);
            if (param.ReturnCode == ((int)ReturnCodeEnum.InvalidInputParameters).ToString())
            {
                return tranData;
            }
            /////////////////////////////////////

            //////////Split MeterName//////////////////////
            string[] values = param.MeterName.Split('-');
            int? parmmobileZoneId = null;
            int? parkmobileSpaceNo = null;
            /////Zone number/////////////////////////////////////////////
            int index0 = 0;
            if (index0 < values.Length)
            {
                parmmobileZoneId = values[index0].ToNullableInt();
            }
            int index1 = 1;
            if (index1 < values.Length)
            {
                parkmobileSpaceNo = values[index1].ToNullableInt();
            }

            if (!parmmobileZoneId.HasValue)
            {
                param.ReturnCode = ((int)ReturnCodeEnum.InvalidInputParameters).ToString();
                return tranData;
            }
            if (!parkmobileSpaceNo.HasValue)
            {
                param.ReturnCode = ((int)ReturnCodeEnum.InvalidInputParameters).ToString();
                return tranData;
            }
            /////////////////////////////
            /////////////////////////////
            #region "DB Check"
            try
            {
                Transaction data = TransactionDataValidatioin.GetDataBirminghamMI(param.CustomerId.Value, param.MeterName, param.parkingSpaceID, param.PlateNumber, param.StateName, EnforcementKey, filePath, logData);


                if (data.TimeZoneId.HasValue)
                    TZ = Utility.GetTimeZoneInfo(data.TimeZoneId.Value, filePath, param.CustomerId.Value.ToString());

                /////////////////////
                var meternumberAndSpaceNo = (data.MeterName == null) ? string.Empty.Split('-') : data.MeterName.Split('-');
                if (meternumberAndSpaceNo.Length > 0)
                    tranData.MeterName = meternumberAndSpaceNo[0];
                else
                    tranData.MeterName = string.Empty;

                if (meternumberAndSpaceNo.Length > 1)
                    tranData.SpaceNo = meternumberAndSpaceNo[1];
                else
                    tranData.SpaceNo = string.Empty;

                tranData.MeterStreet = (data.MeterStreet == null) ? string.Empty : data.MeterStreet;

                DateTime _MeterRTC = data.PresentMeterTime.HasValue ? data.PresentMeterTime.Value : DateTime.Now;
                tranData.MeterRTC = String.Format("{0:yyyyMMdd_THHmmssZ}", Utility.findGMTTime(_MeterRTC, TZ));

                DateTime _SensorEventTime = data.SensorEventTime.HasValue ? data.SensorEventTime.Value : DateTime.Now;
                tranData.MeterLastUpdateTime = String.Format("{0:yyyyMMdd_THHmmssZ}", Utility.findGMTTime(_SensorEventTime, TZ));

                tranData.SensorEventTime = String.Format("{0:yyyyMMdd_THHmmssZ}", Utility.findGMTTime(_SensorEventTime, TZ));

                DateTime _endTime = data.ExpiryTime.HasValue ? data.ExpiryTime.Value : DateTime.Now;
                tranData.ExpiredTime = String.Format("{0:yyyyMMdd_THHmmssZ}", Utility.findGMTTime(_endTime, TZ));

                tranData.MeterExpiredMinutes = data.ExpiredMinutes.HasValue ? data.ExpiredMinutes.Value.ToString() : "0";
                tranData.METERLOC_BLOCKNUMBER = (data.Block == null) ? string.Empty : data.Block;
                tranData.METERLOC_STREETDIRECTION = (data.Direction == null) ? string.Empty : data.Direction;
                tranData.METERLOC_STREETNAME = (data.MeterStreet == null) ? string.Empty : data.MeterStreet;
                tranData.METERLOC_STREETTYPE = (data.StreetType == null) ? string.Empty : data.StreetType;
                tranData.METERLOC_CROSSSTREET1 = (data.CrossStreet1 == null) ? string.Empty : data.CrossStreet1;
                tranData.METERLOC_CROSSSTREET2 = (data.CrossStreet2 == null) ? string.Empty : data.CrossStreet2;
                tranData.METERLOC_ENFORCEMENTHOURSDESC = (data.EnfHourDesc == null) ? string.Empty : data.EnfHourDesc;
                tranData.LicensePlate = (data.PlateNumber == null) ? string.Empty : data.PlateNumber;
                tranData.State = (data.LPLocation == null) ? string.Empty : data.LPLocation;
                tranData.Make = (data.VMake == null) ? string.Empty : data.VMake;
                tranData.Model = (data.VModel == null) ? string.Empty : data.VModel;
                tranData.IsOccupied = Utility.getStatus((data.SpaceStatus == null) ? string.Empty : data.SpaceStatus).ToString();
                tranData.EnforcementKey = (data.EnforcementKey == null) ? string.Empty : data.EnforcementKey;
                tranData.MeterStatus = (data.MeterStatus == null) ? null : data.MeterStatus;

                tranData.METERLOC_METERTYPE = (data.METERLOC_METERTYPE == null) ? null : data.METERLOC_METERTYPE;
                tranData.ZoneName = (data.ZoneName == null) ? null : data.ZoneName;

                tranData.MeterID = data.PayStationID.HasValue ? data.PayStationID.Value.ToString() : "0";
                tranData.ReturnCode = data.ReturnCode;

                logData.Response = new ResponseEntity() { CustomerId = param.CustomerId.Value.ToString(), MeterId = tranData.MeterName, SpaceId = tranData.SpaceNo, CityCurrentTime = data.PresentMeterTime.HasValue ? data.PresentMeterTime.Value : DateTime.Now };
            }
            catch (Exception ex)
            {
                //var appPath = System.Web.Hosting.HostingEnvironment.ApplicationPhysicalPath;
                //string errorPath = Path.Combine(appPath, "GetTransformData", "Timezones");
                //ExceptionLogging.SendErrorToText(ex, "GetCitationData", errorPath);
                //tranData.ReturnCode = ((int)ReturnCodeEnum.Error).ToString();

                tranData.ReturnCode = ((int)ReturnCodeEnum.Error).ToString();
            }
            #endregion

            #region "Parkmobile Service Call"
            /////////////////////////////////////////////////
            TransactionTransform tranDataOfService = new TransactionTransform();
            try
            {
                //////////////////////////////////////////
                logData.Request.CustomerId = param.CustomerId.Value.ToString();
                logData.Request.SpaceId = param.parkingSpaceID;
                logData.Request.MeterId = param.MeterName;


                //////////////////////////////////////////////////
                DateTime? currentCustTime = customerCurrentTime.CurrentTime;
                PMCustomerElement customer = ParkmobileCustomerRetriever.GetCustomer(param.CustomerId.Value);
                ParkMobileGenericFactory factory = new ParkMobileGenericFactory(customer, parmmobileZoneId.Value, parkmobileSpaceNo.Value);
                tranDataOfService = factory.GetRefreshSpaceData();
                tranDataOfService.EnforcementKey = EnforcementKey;

                ///////////////////////////////////////////////////
                logData.Response = new ResponseEntity() { CustomerId = param.CustomerId.Value.ToString(), MeterId = tranData.MeterName, SpaceId = tranData.SpaceNo, CityCurrentTime = currentCustTime.HasValue ? currentCustTime.Value : DateTime.Now };

            }
            catch (Exception ex)
            {
                logData.Response = new ResponseEntity() { CustomerId = param.CustomerId.Value.ToString(), MeterId = tranData.MeterName, SpaceId = tranData.SpaceNo, CityCurrentTime = customerCurrentTime.CurrentTime.HasValue ? customerCurrentTime.CurrentTime.Value : DateTime.Now };
                //throw ex;
                tranDataOfService.ReturnCode = ((int)ReturnCodeEnum.Error).ToString();
            }
            finally
            {

            }
            #endregion

            //Reconcile data
            if ((tranData.ReturnCode == ((int)ReturnCodeEnum.NoRecordExists).ToString()) || (tranData.ReturnCode == ((int)ReturnCodeEnum.Error).ToString()))
            {
                tranData = tranDataOfService;
            }
            else if (tranData.ReturnCode == ((int)ReturnCodeEnum.Success).ToString())
            {
                if (tranDataOfService.ReturnCode == ((int)ReturnCodeEnum.Success).ToString())
                {
                    ////Select one record of highest expiry time
                    int? dbExpiredMinutes = tranData.MeterExpiredMinutes.ToNullableInt();
                    int? parkmobileExpiredMinutes = tranDataOfService.MeterExpiredMinutes.ToNullableInt();
                    //tranData = Nullable.Compare(dbExpiredMinutes, parkmobileExpiredMinutes) > 0 ? tranData : tranDataOfService;

                    //Check both value have negative value.
                    if (dbExpiredMinutes.Value < 0 && parkmobileExpiredMinutes.Value < 0)
                    {
                        tranData = Nullable.Compare(dbExpiredMinutes, parkmobileExpiredMinutes) < 0 ? tranData : tranDataOfService;
                    }
                    else
                    {
                        tranData = Nullable.Compare(dbExpiredMinutes, parkmobileExpiredMinutes) > 0 ? tranData : tranDataOfService;
                    }
                }
            }

            else if (tranData.ReturnCode == ((int)ReturnCodeEnum.MoreRecordsExists).ToString())
            {
                // DB never give more record. hence ignore
            }

            return tranData;
        }
        public static TransactionTransform GetBirminghamMITransformDataNewV2(string EnforcementKey, string filePath, RequestResponseEntity logData, CustomerTime customerCurrentTime)
        {
            TransactionTransform tranData = new TransactionTransform();
            tranData.ReturnCode = ((int)ReturnCodeEnum.InvalidInputParameters).ToString();
            TimeZoneInfo TZ = null;
            TZ = TimeZoneInfo.FindSystemTimeZoneById("Eastern Standard Time");
            //int? customerID = null;
            //string MeterName = null;
            //string parkingSpaceID;
            //string PlateNumber = null;
            //string stateName = null;
            //////////////////////////////////////
            KeyParameter param = PrepareKeyParameters.PrepareBirminghamMI(EnforcementKey);
            if (param.ReturnCode == ((int)ReturnCodeEnum.InvalidInputParameters).ToString())
            {
                return tranData;
            }
            /////////////////////////////////////

            //////////Split MeterName//////////////////////
            string[] values = param.MeterName.Split('-');
            int? parmmobileZoneId = null;
            int? parkmobileSpaceNo = null;
            /////Zone number/////////////////////////////////////////////
            int index0 = 0;
            if (index0 < values.Length)
            {
                parmmobileZoneId = values[index0].ToNullableInt();
            }
            int index1 = 1;
            if (index1 < values.Length)
            {
                parkmobileSpaceNo = values[index1].ToNullableInt();
            }

            if (!parmmobileZoneId.HasValue)
            {
                param.ReturnCode = ((int)ReturnCodeEnum.InvalidInputParameters).ToString();
                return tranData;
            }
            if (!parkmobileSpaceNo.HasValue)
            {
                param.ReturnCode = ((int)ReturnCodeEnum.InvalidInputParameters).ToString();
                return tranData;
            }
            /////////////////////////////
            /////////////////////////////
            #region "DB Check"
            try
            {
                Transaction data = TransactionDataValidatioin.GetDataBirminghamMI(param.CustomerId.Value, param.MeterName, param.parkingSpaceID, param.PlateNumber, param.StateName, EnforcementKey, filePath, logData);


                if (data.TimeZoneId.HasValue)
                    TZ = Utility.GetTimeZoneInfo(data.TimeZoneId.Value, filePath, param.CustomerId.Value.ToString());

                /////////////////////
                var meternumberAndSpaceNo = (data.MeterName == null) ? string.Empty.Split('-') : data.MeterName.Split('-');
                if (meternumberAndSpaceNo.Length > 0)
                    tranData.MeterName = meternumberAndSpaceNo[0];
                else
                    tranData.MeterName = string.Empty;

                if (meternumberAndSpaceNo.Length > 1)
                    tranData.SpaceNo = meternumberAndSpaceNo[1];
                else
                    tranData.SpaceNo = string.Empty;

                tranData.MeterStreet = (data.MeterStreet == null) ? string.Empty : data.MeterStreet;

                DateTime _MeterRTC = data.PresentMeterTime.HasValue ? data.PresentMeterTime.Value : DateTime.Now;
                tranData.MeterRTC = String.Format("{0:yyyyMMdd_THHmmssZ}", Utility.findGMTTime(_MeterRTC, TZ));

                DateTime _SensorEventTime = data.SensorEventTime.HasValue ? data.SensorEventTime.Value : DateTime.Now;
                tranData.MeterLastUpdateTime = String.Format("{0:yyyyMMdd_THHmmssZ}", Utility.findGMTTime(_SensorEventTime, TZ));

                tranData.SensorEventTime = String.Format("{0:yyyyMMdd_THHmmssZ}", Utility.findGMTTime(_SensorEventTime, TZ));

                DateTime _endTime = data.ExpiryTime.HasValue ? data.ExpiryTime.Value : DateTime.Now;
                tranData.ExpiredTime = String.Format("{0:yyyyMMdd_THHmmssZ}", Utility.findGMTTime(_endTime, TZ));

                tranData.MeterExpiredMinutes = data.ExpiredMinutes.HasValue ? data.ExpiredMinutes.Value.ToString() : "0";
                tranData.METERLOC_BLOCKNUMBER = (data.Block == null) ? string.Empty : data.Block;
                tranData.METERLOC_STREETDIRECTION = (data.Direction == null) ? string.Empty : data.Direction;
                tranData.METERLOC_STREETNAME = (data.MeterStreet == null) ? string.Empty : data.MeterStreet;
                tranData.METERLOC_STREETTYPE = (data.StreetType == null) ? string.Empty : data.StreetType;
                tranData.METERLOC_CROSSSTREET1 = (data.CrossStreet1 == null) ? string.Empty : data.CrossStreet1;
                tranData.METERLOC_CROSSSTREET2 = (data.CrossStreet2 == null) ? string.Empty : data.CrossStreet2;
                tranData.METERLOC_ENFORCEMENTHOURSDESC = (data.EnfHourDesc == null) ? string.Empty : data.EnfHourDesc;
                tranData.LicensePlate = (data.PlateNumber == null) ? string.Empty : data.PlateNumber;
                tranData.State = (data.LPLocation == null) ? string.Empty : data.LPLocation;
                tranData.Make = (data.VMake == null) ? string.Empty : data.VMake;
                tranData.Model = (data.VModel == null) ? string.Empty : data.VModel;
                tranData.IsOccupied = Utility.getStatus((data.SpaceStatus == null) ? string.Empty : data.SpaceStatus).ToString();
                tranData.EnforcementKey = (data.EnforcementKey == null) ? string.Empty : data.EnforcementKey;
                tranData.MeterStatus = (data.MeterStatus == null) ? null : data.MeterStatus;

                tranData.METERLOC_METERTYPE = (data.METERLOC_METERTYPE == null) ? null : data.METERLOC_METERTYPE;
                tranData.ZoneName = (data.ZoneName == null) ? null : data.ZoneName;

                tranData.MeterID = data.PayStationID.HasValue ? data.PayStationID.Value.ToString() : "0";
                tranData.ReturnCode = data.ReturnCode;

                logData.Response = new ResponseEntity() { CustomerId = param.CustomerId.Value.ToString(), MeterId = tranData.MeterName, SpaceId = tranData.SpaceNo, CityCurrentTime = data.PresentMeterTime.HasValue ? data.PresentMeterTime.Value : DateTime.Now };
            }
            catch (Exception ex)
            {
                //var appPath = System.Web.Hosting.HostingEnvironment.ApplicationPhysicalPath;
                //string errorPath = Path.Combine(appPath, "GetTransformData", "Timezones");
                //ExceptionLogging.SendErrorToText(ex, "GetCitationData", errorPath);
                //tranData.ReturnCode = ((int)ReturnCodeEnum.Error).ToString();

                tranData.ReturnCode = ((int)ReturnCodeEnum.Error).ToString();
            }
            #endregion

            #region "Parkmobile Service Call"
            /////////////////////////////////////////////////
            TransactionTransform tranDataOfService = new TransactionTransform();
            try
            {
                //////////////////////////////////////////
                logData.Request.CustomerId = param.CustomerId.Value.ToString();
                logData.Request.SpaceId = param.parkingSpaceID;
                logData.Request.MeterId = param.MeterName;
                //////////////////////////////////////////////////
                DateTime? currentCustTime = customerCurrentTime.CurrentTime;
                PMCustomerElement customer = ParkmobileCustomerRetriever.GetCustomer(param.CustomerId.Value);
                ParkMobileGenericFactory factory = new ParkMobileGenericFactory(customer, parmmobileZoneId.Value, parkmobileSpaceNo.Value);
                var parkMobileResLog = new VendorResponseEntity();
                logData.VendorData.Add(VendorNames.ParkMobile, parkMobileResLog);
                //tranDataOfService = factory.GetRefreshSpaceData(ref parkMobileResLog);//Original
                tranDataOfService = factory.GetRefreshSpaceData(ParkingMobileDataType.Space, ref parkMobileResLog);//modified
                tranDataOfService.EnforcementKey = EnforcementKey;

                ///////////////////////////////////////////////////
                logData.Response = new ResponseEntity() { CustomerId = param.CustomerId.Value.ToString(), MeterId = tranData.MeterName, SpaceId = tranData.SpaceNo, CityCurrentTime = currentCustTime.HasValue ? currentCustTime.Value : DateTime.Now };

            }
            catch (Exception ex)
            {
                logData.Response = new ResponseEntity() { CustomerId = param.CustomerId.Value.ToString(), MeterId = tranData.MeterName, SpaceId = tranData.SpaceNo, CityCurrentTime = customerCurrentTime.CurrentTime.HasValue ? customerCurrentTime.CurrentTime.Value : DateTime.Now };
                //throw ex;
                tranDataOfService.ReturnCode = ((int)ReturnCodeEnum.Error).ToString();
            }
            finally
            {

            }
            #endregion

            //Reconcile data
            if ((tranData.ReturnCode == ((int)ReturnCodeEnum.NoRecordExists).ToString()) || (tranData.ReturnCode == ((int)ReturnCodeEnum.Error).ToString()))
            {
                tranData = tranDataOfService;
            }
            else if (tranData.ReturnCode == ((int)ReturnCodeEnum.Success).ToString())
            {
                if (tranDataOfService.ReturnCode == ((int)ReturnCodeEnum.Success).ToString())
                {
                    ////Select one record of highest expiry time
                    int? dbExpiredMinutes = tranData.MeterExpiredMinutes.ToNullableInt();
                    int? parkmobileExpiredMinutes = tranDataOfService.MeterExpiredMinutes.ToNullableInt();
                    //tranData = Nullable.Compare(dbExpiredMinutes, parkmobileExpiredMinutes) > 0 ? tranData : tranDataOfService;

                    //Check both value have negative value.
                    if (dbExpiredMinutes.Value < 0 && parkmobileExpiredMinutes.Value < 0)
                    {
                        tranData = Nullable.Compare(dbExpiredMinutes, parkmobileExpiredMinutes) < 0 ? tranData : tranDataOfService;
                    }
                    else
                    {
                        tranData = Nullable.Compare(dbExpiredMinutes, parkmobileExpiredMinutes) > 0 ? tranData : tranDataOfService;
                    }
                }
            }

            else if (tranData.ReturnCode == ((int)ReturnCodeEnum.MoreRecordsExists).ToString())
            {
                // DB never give more record. hence ignore
            }

            return tranData;
        }
        private static Transaction GetDataBirminghamMI(int customerID, string MeterName, string parkingSpaceID, string PlateNumber, string statename, string EnforcementKey, string filePath, RequestResponseEntity logData)
        {
            string ZoneID = null;
            string SpaceStatus = null;
            Transaction data = new Transaction();
            try
            {
                List<Transaction> transactionList = new List<Transaction>();

                logData.Request.CustomerId = customerID.ToString();
                logData.Request.SpaceId = parkingSpaceID;
                logData.Request.MeterId = MeterName;

                //transactionList = DataAccess.GetSouthMiamiCitationData(customerID, null, null, null, PlateNumber, statename, null);
                transactionList = DataAccess.GetCitationDataBirminghamMI(customerID, null, MeterName, parkingSpaceID, null, null);

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
            }
            catch (Exception ex)
            {
                var appPath = System.Web.Hosting.HostingEnvironment.ApplicationPhysicalPath;
                string errorPath = Path.Combine(appPath, "GetData", "Timezones");
                ExceptionLogging.SendErrorToText(ex, "GetCitationData", errorPath);
                data.ReturnCode = ((int)ReturnCodeEnum.Error).ToString();
                throw;
            }
            return data;
        }
        #endregion

        #region CrystalLake
        public static TransactionTransform GetCrystalLakeTransformData(string EnforcementKey, string filePath, RequestResponseEntity logData, CustomerTime customerCurrentTime)
        {
            TransactionTransform tranData = new TransactionTransform();
            tranData.ReturnCode = ((int)ReturnCodeEnum.InvalidInputParameters).ToString();
            TimeZoneInfo TZ = null;
            TZ = TimeZoneInfo.FindSystemTimeZoneById("Eastern Standard Time");

            KeyParameter param = PrepareKeyParameters.PrepareCrystalLake(EnforcementKey);
            if (param.ReturnCode == ((int)ReturnCodeEnum.InvalidInputParameters).ToString())
            {
                return tranData;
            }
            /////////////////////////////////////


            #region "Cale Service Call"
            /////////////////////////////////////////////////
            try
            {
                //////////////////////////////////////////
                logData.Request.CustomerId = param.CustomerId.Value.ToString();
                logData.Request.SpaceId = param.parkingSpaceID;
                logData.Request.MeterId = param.MeterName;
                //////////////////////////////////////////////////
                DateTime? currentCustTime = customerCurrentTime.CurrentTime;
                Cale.CaleCustomerElement customer = Cale.CaleCustomerRetriever.GetCustomer(param.CustomerId.Value);
                Cale.CaleGenericFactory factory = new Cale.CaleGenericFactory(customer, param.PlateNumber, null);
                var caleResLog = new VendorResponseEntity();
                logData.VendorData.Add(VendorNames.Cale, caleResLog);
                if (customer.EnforcementType == EnumEnforcementType.PayByPlate.ToString())
                    tranData = factory.GetRefreshPlateData(ref caleResLog);

                tranData.EnforcementKey = EnforcementKey;

                ///////////////////////////////////////////////////
                logData.Response = new ResponseEntity() { CustomerId = param.CustomerId.Value.ToString(), MeterId = tranData.MeterName, SpaceId = tranData.SpaceNo, CityCurrentTime = currentCustTime.HasValue ? currentCustTime.Value : DateTime.Now };
            }
            catch (Exception ex)
            {
                logData.Response = new ResponseEntity() { CustomerId = param.CustomerId.Value.ToString(), MeterId = tranData.MeterName, SpaceId = tranData.SpaceNo, CityCurrentTime = customerCurrentTime.CurrentTime.HasValue ? customerCurrentTime.CurrentTime.Value : DateTime.Now };
                //throw ex;
                tranData.ReturnCode = ((int)ReturnCodeEnum.Error).ToString();
            }
            finally
            {

            }
            #endregion

            return tranData;

        }

        #endregion

        #region SharjahMunicipality
        public static TransactionTransform GetSharjahMunicipalityTransformData(string EnforcementKey, string filePath, RequestResponseEntity logData)
        {
            TransactionTransform tranData = new TransactionTransform();
            tranData.ReturnCode = ((int)ReturnCodeEnum.InvalidInputParameters).ToString();
            TimeZoneInfo TZ = null;
            TZ = TimeZoneInfo.FindSystemTimeZoneById("Arabian Standard Time");
            //int? customerID = null;
            //string MeterName = null;
            //string parkingSpaceID;
            //string PlateNumber = null;
            //string stateName = null;
            //////////////////////////////////////
            KeyParameter param = PrepareKeyParameters.PrepareSharjahMunicipality(EnforcementKey);
            if (param.ReturnCode == ((int)ReturnCodeEnum.InvalidInputParameters).ToString())
            {
                return tranData;
            }
            /////////////////////////////////////
            try
            {
                Transaction data = TransactionDataValidatioin.GetDataSharjahMunicipality(param.CustomerId.Value, param.MeterName, param.parkingSpaceID, param.PlateNumber, param.StateName, EnforcementKey, filePath, logData);


                if (data.TimeZoneId.HasValue)
                    TZ = Utility.GetTimeZoneInfo(data.TimeZoneId.Value, filePath, param.CustomerId.Value.ToString());

                //For South Miami : PayStationId. Others it is MeterID
                tranData.MeterName = (data.MeterName == null) ? string.Empty : data.MeterName;
                tranData.SpaceNo = data.ParkingSpaceID.ToString();

                tranData.MeterStreet = (data.MeterStreet == null) ? string.Empty : data.MeterStreet;

                DateTime _MeterRTC = data.PresentMeterTime.HasValue ? data.PresentMeterTime.Value : DateTime.Now;
                tranData.MeterRTC = String.Format("{0:yyyyMMdd_THHmmssZ}", Utility.findGMTTime(_MeterRTC, TZ));

                DateTime _SensorEventTime = data.SensorEventTime.HasValue ? data.SensorEventTime.Value : DateTime.Now;
                tranData.MeterLastUpdateTime = String.Format("{0:yyyyMMdd_THHmmssZ}", Utility.findGMTTime(_SensorEventTime, TZ));

                tranData.SensorEventTime = String.Format("{0:yyyyMMdd_THHmmssZ}", Utility.findGMTTime(_SensorEventTime, TZ));

                DateTime _endTime = data.ExpiryTime.HasValue ? data.ExpiryTime.Value : DateTime.Now;
                tranData.ExpiredTime = String.Format("{0:yyyyMMdd_THHmmssZ}", Utility.findGMTTime(_endTime, TZ));

                tranData.MeterExpiredMinutes = data.ExpiredMinutes.HasValue ? data.ExpiredMinutes.Value.ToString() : "0";
                tranData.METERLOC_BLOCKNUMBER = (data.Block == null) ? string.Empty : data.Block;
                tranData.METERLOC_STREETDIRECTION = (data.Direction == null) ? string.Empty : data.Direction;
                tranData.METERLOC_STREETNAME = (data.MeterStreet == null) ? string.Empty : data.MeterStreet;
                tranData.METERLOC_STREETTYPE = (data.StreetType == null) ? string.Empty : data.StreetType;
                tranData.METERLOC_CROSSSTREET1 = (data.CrossStreet1 == null) ? string.Empty : data.CrossStreet1;
                tranData.METERLOC_CROSSSTREET2 = (data.CrossStreet2 == null) ? string.Empty : data.CrossStreet2;
                tranData.METERLOC_ENFORCEMENTHOURSDESC = (data.EnfHourDesc == null) ? string.Empty : data.EnfHourDesc;
                tranData.LicensePlate = (data.PlateNumber == null) ? string.Empty : data.PlateNumber;
                tranData.State = (data.LPLocation == null) ? string.Empty : data.LPLocation;
                tranData.Make = (data.VMake == null) ? string.Empty : data.VMake;
                tranData.Model = (data.VModel == null) ? string.Empty : data.VModel;
                tranData.IsOccupied = Utility.getStatus((data.SpaceStatus == null) ? string.Empty : data.SpaceStatus).ToString();
                tranData.EnforcementKey = (data.EnforcementKey == null) ? string.Empty : data.EnforcementKey;
                tranData.MeterStatus = (data.MeterStatus == null) ? null : data.MeterStatus;

                tranData.METERLOC_METERTYPE = (data.METERLOC_METERTYPE == null) ? null : data.METERLOC_METERTYPE;
                tranData.ZoneName = (data.ZoneName == null) ? null : data.ZoneName;

                tranData.MeterID = data.PayStationID.HasValue ? data.PayStationID.Value.ToString() : "0";
                tranData.ReturnCode = data.ReturnCode;

                logData.Response = new ResponseEntity() { CustomerId = param.CustomerId.Value.ToString(), MeterId = tranData.MeterName, SpaceId = tranData.SpaceNo, CityCurrentTime = data.PresentMeterTime.HasValue ? data.PresentMeterTime.Value : DateTime.Now };
            }
            catch (Exception ex)
            {
                var appPath = System.Web.Hosting.HostingEnvironment.ApplicationPhysicalPath;
                string errorPath = Path.Combine(appPath, "GetTransformData", "Timezones");
                ExceptionLogging.SendErrorToText(ex, "GetCitationData", errorPath);
                tranData.ReturnCode = ((int)ReturnCodeEnum.Error).ToString();

            }
            return tranData;
        }
        private static Transaction GetDataSharjahMunicipality(int customerID, string MeterName, string parkingSpaceID, string PlateNumber, string statename, string EnforcementKey, string filePath, RequestResponseEntity logData)
        {
            string ZoneID = null;
            string SpaceStatus = null;
            Transaction data = new Transaction();
            try
            {
                List<Transaction> transactionList = new List<Transaction>();

                logData.Request.CustomerId = customerID.ToString();
                logData.Request.SpaceId = parkingSpaceID;
                logData.Request.MeterId = MeterName;

                transactionList = DataAccess.GetSharjahMunicipalityData(customerID, null, MeterName, parkingSpaceID, null, null);

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
            }
            catch (Exception ex)
            {
                var appPath = System.Web.Hosting.HostingEnvironment.ApplicationPhysicalPath;
                string errorPath = Path.Combine(appPath, "GetData", "Timezones");
                ExceptionLogging.SendErrorToText(ex, "GetCitationData", errorPath);
                data.ReturnCode = ((int)ReturnCodeEnum.Error).ToString();
                throw;
            }
            return data;
        }
        #endregion

        #region IndianaBorough
        public static TransactionTransform GetIndianaBoroughTransformData(string EnforcementKey, string filePath, RequestResponseEntity logData)
        {
            TransactionTransform tranData = new TransactionTransform();
            tranData.ReturnCode = ((int)ReturnCodeEnum.InvalidInputParameters).ToString();
            TimeZoneInfo TZ = null;
            TZ = TimeZoneInfo.FindSystemTimeZoneById("Eastern Standard Time");
            //int? customerID = null;
            //string MeterName = null;
            //string parkingSpaceID;
            //string PlateNumber = null;
            //string stateName = null;
            //////////////////////////////////////
            KeyParameter param = PrepareKeyParameters.PrepareIndianaBorough(EnforcementKey);
            if (param.ReturnCode == ((int)ReturnCodeEnum.InvalidInputParameters).ToString())
            {
                return tranData;
            }

            /////////////////////////////////////
            try
            {
                Transaction data = TransactionDataValidatioin.GetDataIndianaBorough(param.CustomerId.Value, param.MeterName, param.parkingSpaceID, param.PlateNumber, param.StateName, EnforcementKey, filePath, logData);


                if (data.TimeZoneId.HasValue)
                    TZ = Utility.GetTimeZoneInfo(data.TimeZoneId.Value, filePath, param.CustomerId.Value.ToString());

                //For South Miami : PayStationId. Others it is MeterID
                tranData.MeterName = (data.MeterName == null) ? string.Empty : data.MeterName;
                tranData.SpaceNo = data.ParkingSpaceID.ToString();

                tranData.MeterStreet = (data.MeterStreet == null) ? string.Empty : data.MeterStreet;

                DateTime _MeterRTC = data.PresentMeterTime.HasValue ? data.PresentMeterTime.Value : DateTime.Now;
                tranData.MeterRTC = String.Format("{0:yyyyMMdd_THHmmssZ}", Utility.findGMTTime(_MeterRTC, TZ));

                DateTime _SensorEventTime = data.SensorEventTime.HasValue ? data.SensorEventTime.Value : DateTime.Now;
                tranData.MeterLastUpdateTime = String.Format("{0:yyyyMMdd_THHmmssZ}", Utility.findGMTTime(_SensorEventTime, TZ));

                tranData.SensorEventTime = String.Format("{0:yyyyMMdd_THHmmssZ}", Utility.findGMTTime(_SensorEventTime, TZ));

                DateTime _endTime = data.ExpiryTime.HasValue ? data.ExpiryTime.Value : DateTime.Now;
                tranData.ExpiredTime = String.Format("{0:yyyyMMdd_THHmmssZ}", Utility.findGMTTime(_endTime, TZ));

                tranData.MeterExpiredMinutes = data.ExpiredMinutes.HasValue ? data.ExpiredMinutes.Value.ToString() : "0";
                tranData.METERLOC_BLOCKNUMBER = (data.Block == null) ? string.Empty : data.Block;
                tranData.METERLOC_STREETDIRECTION = (data.Direction == null) ? string.Empty : data.Direction;
                tranData.METERLOC_STREETNAME = (data.MeterStreet == null) ? string.Empty : data.MeterStreet;
                tranData.METERLOC_STREETTYPE = (data.StreetType == null) ? string.Empty : data.StreetType;
                tranData.METERLOC_CROSSSTREET1 = (data.CrossStreet1 == null) ? string.Empty : data.CrossStreet1;
                tranData.METERLOC_CROSSSTREET2 = (data.CrossStreet2 == null) ? string.Empty : data.CrossStreet2;
                tranData.METERLOC_ENFORCEMENTHOURSDESC = (data.EnfHourDesc == null) ? string.Empty : data.EnfHourDesc;
                tranData.LicensePlate = (data.PlateNumber == null) ? string.Empty : data.PlateNumber;
                tranData.State = (data.LPLocation == null) ? string.Empty : data.LPLocation;
                tranData.Make = (data.VMake == null) ? string.Empty : data.VMake;
                tranData.Model = (data.VModel == null) ? string.Empty : data.VModel;
                tranData.IsOccupied = Utility.getStatus((data.SpaceStatus == null) ? string.Empty : data.SpaceStatus).ToString();
                tranData.EnforcementKey = (data.EnforcementKey == null) ? string.Empty : data.EnforcementKey;
                tranData.MeterStatus = (data.MeterStatus == null) ? null : data.MeterStatus;

                tranData.METERLOC_METERTYPE = (data.METERLOC_METERTYPE == null) ? null : data.METERLOC_METERTYPE;
                tranData.ZoneName = (data.ZoneName == null) ? null : data.ZoneName;

                tranData.MeterID = data.PayStationID.HasValue ? data.PayStationID.Value.ToString() : "0";
                tranData.ReturnCode = data.ReturnCode;

                logData.Response = new ResponseEntity() { CustomerId = param.CustomerId.Value.ToString(), MeterId = tranData.MeterName, SpaceId = tranData.SpaceNo, CityCurrentTime = data.PresentMeterTime.HasValue ? data.PresentMeterTime.Value : DateTime.Now };
            }
            catch (Exception ex)
            {
                var appPath = System.Web.Hosting.HostingEnvironment.ApplicationPhysicalPath;
                string errorPath = Path.Combine(appPath, "GetTransformData", "Timezones");
                ExceptionLogging.SendErrorToText(ex, "GetCitationData", errorPath);
                tranData.ReturnCode = ((int)ReturnCodeEnum.Error).ToString();

            }
            return tranData;
        }
        public static TransactionTransform GetIndianaBoroughTransformDataV2(string EnforcementKey, string filePath, RequestResponseEntity logData, CustomerTime customerCurrentTime)
        {
            TransactionTransform tranData = new TransactionTransform();
            tranData.ReturnCode = ((int)ReturnCodeEnum.InvalidInputParameters).ToString();
            TimeZoneInfo TZ = null;
            TZ = TimeZoneInfo.FindSystemTimeZoneById("Eastern Standard Time");
            //int? customerID = null;
            //string MeterName = null;
            //string parkingSpaceID;
            //string PlateNumber = null;
            //string stateName = null;
            //////////////////////////////////////
            KeyParameter param = PrepareKeyParameters.PrepareIndianaBorough(EnforcementKey);
            if (param.ReturnCode == ((int)ReturnCodeEnum.InvalidInputParameters).ToString())
            {
                return tranData;
            }

            /////////////////////////////////////
            #region "DB Check"
            try
            {
                Transaction data = TransactionDataValidatioin.GetDataIndianaBorough(param.CustomerId.Value, param.MeterName, param.parkingSpaceID, param.PlateNumber, param.StateName, EnforcementKey, filePath, logData);


                if (data.TimeZoneId.HasValue)
                    TZ = Utility.GetTimeZoneInfo(data.TimeZoneId.Value, filePath, param.CustomerId.Value.ToString());

                //For South Miami : PayStationId. Others it is MeterID
                tranData.MeterName = (data.MeterName == null) ? string.Empty : data.MeterName;
                tranData.SpaceNo = data.ParkingSpaceID.ToString();

                tranData.MeterStreet = (data.MeterStreet == null) ? string.Empty : data.MeterStreet;

                DateTime _MeterRTC = data.PresentMeterTime.HasValue ? data.PresentMeterTime.Value : DateTime.Now;
                tranData.MeterRTC = String.Format("{0:yyyyMMdd_THHmmssZ}", Utility.findGMTTime(_MeterRTC, TZ));

                DateTime _SensorEventTime = data.SensorEventTime.HasValue ? data.SensorEventTime.Value : DateTime.Now;
                tranData.MeterLastUpdateTime = String.Format("{0:yyyyMMdd_THHmmssZ}", Utility.findGMTTime(_SensorEventTime, TZ));

                tranData.SensorEventTime = String.Format("{0:yyyyMMdd_THHmmssZ}", Utility.findGMTTime(_SensorEventTime, TZ));

                DateTime _endTime = data.ExpiryTime.HasValue ? data.ExpiryTime.Value : DateTime.Now;
                tranData.ExpiredTime = String.Format("{0:yyyyMMdd_THHmmssZ}", Utility.findGMTTime(_endTime, TZ));

                tranData.MeterExpiredMinutes = data.ExpiredMinutes.HasValue ? data.ExpiredMinutes.Value.ToString() : "0";
                tranData.METERLOC_BLOCKNUMBER = (data.Block == null) ? string.Empty : data.Block;
                tranData.METERLOC_STREETDIRECTION = (data.Direction == null) ? string.Empty : data.Direction;
                tranData.METERLOC_STREETNAME = (data.MeterStreet == null) ? string.Empty : data.MeterStreet;
                tranData.METERLOC_STREETTYPE = (data.StreetType == null) ? string.Empty : data.StreetType;
                tranData.METERLOC_CROSSSTREET1 = (data.CrossStreet1 == null) ? string.Empty : data.CrossStreet1;
                tranData.METERLOC_CROSSSTREET2 = (data.CrossStreet2 == null) ? string.Empty : data.CrossStreet2;
                tranData.METERLOC_ENFORCEMENTHOURSDESC = (data.EnfHourDesc == null) ? string.Empty : data.EnfHourDesc;
                tranData.LicensePlate = (data.PlateNumber == null) ? string.Empty : data.PlateNumber;
                tranData.State = (data.LPLocation == null) ? string.Empty : data.LPLocation;
                tranData.Make = (data.VMake == null) ? string.Empty : data.VMake;
                tranData.Model = (data.VModel == null) ? string.Empty : data.VModel;
                tranData.IsOccupied = Utility.getStatus((data.SpaceStatus == null) ? string.Empty : data.SpaceStatus).ToString();
                tranData.EnforcementKey = (data.EnforcementKey == null) ? string.Empty : data.EnforcementKey;
                tranData.MeterStatus = (data.MeterStatus == null) ? null : data.MeterStatus;

                tranData.METERLOC_METERTYPE = (data.METERLOC_METERTYPE == null) ? null : data.METERLOC_METERTYPE;
                tranData.ZoneName = (data.ZoneName == null) ? null : data.ZoneName;

                tranData.MeterID = data.PayStationID.HasValue ? data.PayStationID.Value.ToString() : "0";
                tranData.ReturnCode = data.ReturnCode;

                logData.Response = new ResponseEntity() { CustomerId = param.CustomerId.Value.ToString(), MeterId = tranData.MeterName, SpaceId = tranData.SpaceNo, CityCurrentTime = data.PresentMeterTime.HasValue ? data.PresentMeterTime.Value : DateTime.Now };
            }
            catch (Exception ex)
            {
                //var appPath = System.Web.Hosting.HostingEnvironment.ApplicationPhysicalPath;
                //string errorPath = Path.Combine(appPath, "GetTransformData", "Timezones");
                //ExceptionLogging.SendErrorToText(ex, "GetCitationData", errorPath);
                //tranData.ReturnCode = ((int)ReturnCodeEnum.Error).ToString();
                tranData.ReturnCode = ((int)ReturnCodeEnum.Error).ToString();
            }
            #endregion

            #region "Passport Monitoring"
            /////////////////////////////////////////////////
            TransactionTransform tranDataOfService = new TransactionTransform();
            try
            {
                //////////////////////////////////////////
                logData.Request.CustomerId = param.CustomerId.Value.ToString();
                logData.Request.SpaceId = param.parkingSpaceID;
                logData.Request.MeterId = param.MeterName;
                //////////////////////////////////////////////////
                DateTime? currentCustTime = customerCurrentTime.CurrentTime;
                PassportMonitoringCustomerElement customer = PassportMonitoringCustomerRetriever.GetCustomer(param.CustomerId.Value);
                LookUpLPNFactory factory = new LookUpLPNFactory(customer, param.PlateNumber);
                tranDataOfService = factory.GetRefreshPlateData();
                tranDataOfService.EnforcementKey = EnforcementKey;

                ///////////////////////////////////////////////////
                logData.Response = new ResponseEntity() { CustomerId = param.CustomerId.Value.ToString(), MeterId = tranData.MeterName, SpaceId = tranData.SpaceNo, CityCurrentTime = currentCustTime.HasValue ? currentCustTime.Value : DateTime.Now };
            }
            catch (Exception ex)
            {
                logData.Response = new ResponseEntity() { CustomerId = param.CustomerId.Value.ToString(), MeterId = tranData.MeterName, SpaceId = tranData.SpaceNo, CityCurrentTime = customerCurrentTime.CurrentTime.HasValue ? customerCurrentTime.CurrentTime.Value : DateTime.Now };
                //throw ex;
                tranData.ReturnCode = ((int)ReturnCodeEnum.Error).ToString();
            }
            finally
            {

            }
            #endregion

            //Reconcile data
            if ((tranData.ReturnCode == ((int)ReturnCodeEnum.NoRecordExists).ToString()) || (tranData.ReturnCode == ((int)ReturnCodeEnum.Error).ToString()))
            {
                tranData = tranDataOfService;
            }
            else if (tranData.ReturnCode == ((int)ReturnCodeEnum.Success).ToString())
            {
                if (tranDataOfService.ReturnCode == ((int)ReturnCodeEnum.Success).ToString())
                {
                    ////Select one record of highest expiry time
                    int? dbExpiredMinutes = tranData.MeterExpiredMinutes.ToNullableInt();
                    int? parkeonExpiredMinutes = tranDataOfService.MeterExpiredMinutes.ToNullableInt();
                    tranData = Nullable.Compare(dbExpiredMinutes, parkeonExpiredMinutes) > 0 ? tranData : tranDataOfService;
                }
            }

            else if (tranData.ReturnCode == ((int)ReturnCodeEnum.MoreRecordsExists).ToString())
            {
                // DB never give more record. hence ignore
            }

            return tranData;
        }
        public static TransactionTransform GetIndianaBoroughTransformDataV3(string EnforcementKey, string filePath, RequestResponseEntity logData, CustomerTime customerCurrentTime)
        {
            TransactionTransform tranData = new TransactionTransform();
            tranData.ReturnCode = ((int)ReturnCodeEnum.InvalidInputParameters).ToString();
            TimeZoneInfo TZ = null;
            TZ = TimeZoneInfo.FindSystemTimeZoneById("Eastern Standard Time");
            //int? customerID = null;
            //string MeterName = null;
            //string parkingSpaceID;
            //string PlateNumber = null;
            //string stateName = null;
            //////////////////////////////////////
            KeyParameter param = PrepareKeyParameters.PrepareIndianaBorough(EnforcementKey);
            if (param.ReturnCode == ((int)ReturnCodeEnum.InvalidInputParameters).ToString())
            {
                return tranData;
            }

            /////////////////////////////////////
            #region "DB Check"
            try
            {
                Transaction data = TransactionDataValidatioin.GetDataIndianaBorough(param.CustomerId.Value, param.MeterName, param.parkingSpaceID, param.PlateNumber, param.StateName, EnforcementKey, filePath, logData);


                if (data.TimeZoneId.HasValue)
                    TZ = Utility.GetTimeZoneInfo(data.TimeZoneId.Value, filePath, param.CustomerId.Value.ToString());

                //For South Miami : PayStationId. Others it is MeterID
                tranData.MeterName = (data.MeterName == null) ? string.Empty : data.MeterName;
                tranData.SpaceNo = data.ParkingSpaceID.ToString();

                tranData.MeterStreet = (data.MeterStreet == null) ? string.Empty : data.MeterStreet;

                DateTime _MeterRTC = data.PresentMeterTime.HasValue ? data.PresentMeterTime.Value : DateTime.Now;
                tranData.MeterRTC = String.Format("{0:yyyyMMdd_THHmmssZ}", Utility.findGMTTime(_MeterRTC, TZ));

                DateTime _SensorEventTime = data.SensorEventTime.HasValue ? data.SensorEventTime.Value : DateTime.Now;
                tranData.MeterLastUpdateTime = String.Format("{0:yyyyMMdd_THHmmssZ}", Utility.findGMTTime(_SensorEventTime, TZ));

                tranData.SensorEventTime = String.Format("{0:yyyyMMdd_THHmmssZ}", Utility.findGMTTime(_SensorEventTime, TZ));

                DateTime _endTime = data.ExpiryTime.HasValue ? data.ExpiryTime.Value : DateTime.Now;
                tranData.ExpiredTime = String.Format("{0:yyyyMMdd_THHmmssZ}", Utility.findGMTTime(_endTime, TZ));

                tranData.MeterExpiredMinutes = data.ExpiredMinutes.HasValue ? data.ExpiredMinutes.Value.ToString() : "0";
                tranData.METERLOC_BLOCKNUMBER = (data.Block == null) ? string.Empty : data.Block;
                tranData.METERLOC_STREETDIRECTION = (data.Direction == null) ? string.Empty : data.Direction;
                tranData.METERLOC_STREETNAME = (data.MeterStreet == null) ? string.Empty : data.MeterStreet;
                tranData.METERLOC_STREETTYPE = (data.StreetType == null) ? string.Empty : data.StreetType;
                tranData.METERLOC_CROSSSTREET1 = (data.CrossStreet1 == null) ? string.Empty : data.CrossStreet1;
                tranData.METERLOC_CROSSSTREET2 = (data.CrossStreet2 == null) ? string.Empty : data.CrossStreet2;
                tranData.METERLOC_ENFORCEMENTHOURSDESC = (data.EnfHourDesc == null) ? string.Empty : data.EnfHourDesc;
                tranData.LicensePlate = (data.PlateNumber == null) ? string.Empty : data.PlateNumber;
                tranData.State = (data.LPLocation == null) ? string.Empty : data.LPLocation;
                tranData.Make = (data.VMake == null) ? string.Empty : data.VMake;
                tranData.Model = (data.VModel == null) ? string.Empty : data.VModel;
                tranData.IsOccupied = Utility.getStatus((data.SpaceStatus == null) ? string.Empty : data.SpaceStatus).ToString();
                tranData.EnforcementKey = (data.EnforcementKey == null) ? string.Empty : data.EnforcementKey;
                tranData.MeterStatus = (data.MeterStatus == null) ? null : data.MeterStatus;

                tranData.METERLOC_METERTYPE = (data.METERLOC_METERTYPE == null) ? null : data.METERLOC_METERTYPE;
                tranData.ZoneName = (data.ZoneName == null) ? null : data.ZoneName;

                tranData.MeterID = data.PayStationID.HasValue ? data.PayStationID.Value.ToString() : "0";
                tranData.ReturnCode = data.ReturnCode;

                logData.Response = new ResponseEntity() { CustomerId = param.CustomerId.Value.ToString(), MeterId = tranData.MeterName, SpaceId = tranData.SpaceNo, CityCurrentTime = data.PresentMeterTime.HasValue ? data.PresentMeterTime.Value : DateTime.Now };
            }
            catch (Exception ex)
            {
                //var appPath = System.Web.Hosting.HostingEnvironment.ApplicationPhysicalPath;
                //string errorPath = Path.Combine(appPath, "GetTransformData", "Timezones");
                //ExceptionLogging.SendErrorToText(ex, "GetCitationData", errorPath);
                //tranData.ReturnCode = ((int)ReturnCodeEnum.Error).ToString();
                tranData.ReturnCode = ((int)ReturnCodeEnum.Error).ToString();
            }
            #endregion

            #region "Passport Monitoring"
            /////////////////////////////////////////////////
            TransactionTransform tranDataOfService = new TransactionTransform();
            try
            {
                //////////////////////////////////////////
                logData.Request.CustomerId = param.CustomerId.Value.ToString();
                logData.Request.SpaceId = param.parkingSpaceID;
                logData.Request.MeterId = param.MeterName;
                //////////////////////////////////////////////////
                DateTime? currentCustTime = customerCurrentTime.CurrentTime;
                PassportMonitoringCustomerElement customer = PassportMonitoringCustomerRetriever.GetCustomer(param.CustomerId.Value);
                LookUpLPNFactory factory = new LookUpLPNFactory(customer, param.PlateNumber);
                var passportMonResLog = new VendorResponseEntity();
                logData.VendorData.Add(VendorNames.PassportMonitoring, passportMonResLog);
                tranDataOfService = factory.GetRefreshPlateData(ref passportMonResLog);
                tranDataOfService.EnforcementKey = EnforcementKey;

                ///////////////////////////////////////////////////
                logData.Response = new ResponseEntity() { CustomerId = param.CustomerId.Value.ToString(), MeterId = tranData.MeterName, SpaceId = tranData.SpaceNo, CityCurrentTime = currentCustTime.HasValue ? currentCustTime.Value : DateTime.Now };
            }
            catch (Exception ex)
            {
                logData.Response = new ResponseEntity() { CustomerId = param.CustomerId.Value.ToString(), MeterId = tranData.MeterName, SpaceId = tranData.SpaceNo, CityCurrentTime = customerCurrentTime.CurrentTime.HasValue ? customerCurrentTime.CurrentTime.Value : DateTime.Now };
                //throw ex;
                tranData.ReturnCode = ((int)ReturnCodeEnum.Error).ToString();
            }
            finally
            {

            }
            #endregion

            //Reconcile data
            if ((tranData.ReturnCode == ((int)ReturnCodeEnum.NoRecordExists).ToString()) || (tranData.ReturnCode == ((int)ReturnCodeEnum.Error).ToString()))
            {
                tranData = tranDataOfService;
            }
            else if (tranData.ReturnCode == ((int)ReturnCodeEnum.Success).ToString())
            {
                if (tranDataOfService.ReturnCode == ((int)ReturnCodeEnum.Success).ToString())
                {
                    ////Select one record of highest expiry time
                    int? dbExpiredMinutes = tranData.MeterExpiredMinutes.ToNullableInt();
                    int? parkeonExpiredMinutes = tranDataOfService.MeterExpiredMinutes.ToNullableInt();
                    tranData = Nullable.Compare(dbExpiredMinutes, parkeonExpiredMinutes) > 0 ? tranData : tranDataOfService;
                }
            }

            else if (tranData.ReturnCode == ((int)ReturnCodeEnum.MoreRecordsExists).ToString())
            {
                // DB never give more record. hence ignore
            }

            return tranData;
        }
        private static Transaction GetDataIndianaBorough(int customerID, string MeterName, string parkingSpaceID, string PlateNumber, string statename, string EnforcementKey, string filePath, RequestResponseEntity logData)
        {
            string ZoneID = null;
            string SpaceStatus = null;
            Transaction data = new Transaction();
            try
            {
                List<Transaction> transactionList = new List<Transaction>();

                logData.Request.CustomerId = customerID.ToString();
                logData.Request.SpaceId = parkingSpaceID;
                logData.Request.MeterId = MeterName;

                transactionList = DataAccess.GetIndianaBoroughCitationData(customerID, null, MeterName, parkingSpaceID, PlateNumber, statename, null);

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
            }
            catch (Exception ex)
            {
                var appPath = System.Web.Hosting.HostingEnvironment.ApplicationPhysicalPath;
                string errorPath = Path.Combine(appPath, "GetData", "Timezones");
                ExceptionLogging.SendErrorToText(ex, "GetCitationData", errorPath);
                data.ReturnCode = ((int)ReturnCodeEnum.Error).ToString();
                throw;
            }
            return data;
        }

        #endregion

        #region Franklin
        public static TransactionTransform GetFranklinTransformData(string EnforcementKey, string filePath, RequestResponseEntity logData)
        {
            TransactionTransform tranData = new TransactionTransform();
            tranData.ReturnCode = ((int)ReturnCodeEnum.InvalidInputParameters).ToString();
            TimeZoneInfo TZ = null;
            TZ = TimeZoneInfo.FindSystemTimeZoneById("Eastern Standard Time");
            //int? customerID = null;
            //string MeterName = null;
            //string parkingSpaceID;
            //string PlateNumber = null;
            //string stateName = null;
            //////////////////////////////////////
            KeyParameter param = PrepareKeyParameters.PrepareFranklin(EnforcementKey);
            if (param.ReturnCode == ((int)ReturnCodeEnum.InvalidInputParameters).ToString())
            {
                return tranData;
            }

            /////////////////////////////////////
            try
            {
                Transaction data = TransactionDataValidatioin.GetDataFranklin(param.CustomerId.Value, param.MeterName, param.parkingSpaceID, param.PlateNumber, param.StateName, EnforcementKey, filePath, logData);


                if (data.TimeZoneId.HasValue)
                    TZ = Utility.GetTimeZoneInfo(data.TimeZoneId.Value, filePath, param.CustomerId.Value.ToString());

                //For South Miami : PayStationId. Others it is MeterID
                tranData.MeterName = (data.MeterName == null) ? string.Empty : data.MeterName;
                tranData.SpaceNo = data.ParkingSpaceID.ToString();

                tranData.MeterStreet = (data.MeterStreet == null) ? string.Empty : data.MeterStreet;

                DateTime _MeterRTC = data.PresentMeterTime.HasValue ? data.PresentMeterTime.Value : DateTime.Now;
                tranData.MeterRTC = String.Format("{0:yyyyMMdd_THHmmssZ}", Utility.findGMTTime(_MeterRTC, TZ));

                DateTime _SensorEventTime = data.SensorEventTime.HasValue ? data.SensorEventTime.Value : DateTime.Now;
                tranData.MeterLastUpdateTime = String.Format("{0:yyyyMMdd_THHmmssZ}", Utility.findGMTTime(_SensorEventTime, TZ));

                tranData.SensorEventTime = String.Format("{0:yyyyMMdd_THHmmssZ}", Utility.findGMTTime(_SensorEventTime, TZ));

                DateTime _endTime = data.ExpiryTime.HasValue ? data.ExpiryTime.Value : DateTime.Now;
                tranData.ExpiredTime = String.Format("{0:yyyyMMdd_THHmmssZ}", Utility.findGMTTime(_endTime, TZ));

                tranData.MeterExpiredMinutes = data.ExpiredMinutes.HasValue ? data.ExpiredMinutes.Value.ToString() : "0";
                tranData.METERLOC_BLOCKNUMBER = (data.Block == null) ? string.Empty : data.Block;
                tranData.METERLOC_STREETDIRECTION = (data.Direction == null) ? string.Empty : data.Direction;
                tranData.METERLOC_STREETNAME = (data.MeterStreet == null) ? string.Empty : data.MeterStreet;
                tranData.METERLOC_STREETTYPE = (data.StreetType == null) ? string.Empty : data.StreetType;
                tranData.METERLOC_CROSSSTREET1 = (data.CrossStreet1 == null) ? string.Empty : data.CrossStreet1;
                tranData.METERLOC_CROSSSTREET2 = (data.CrossStreet2 == null) ? string.Empty : data.CrossStreet2;
                tranData.METERLOC_ENFORCEMENTHOURSDESC = (data.EnfHourDesc == null) ? string.Empty : data.EnfHourDesc;
                tranData.LicensePlate = (data.PlateNumber == null) ? string.Empty : data.PlateNumber;
                tranData.State = (data.LPLocation == null) ? string.Empty : data.LPLocation;
                tranData.Make = (data.VMake == null) ? string.Empty : data.VMake;
                tranData.Model = (data.VModel == null) ? string.Empty : data.VModel;
                tranData.IsOccupied = Utility.getStatus((data.SpaceStatus == null) ? string.Empty : data.SpaceStatus).ToString();
                tranData.EnforcementKey = (data.EnforcementKey == null) ? string.Empty : data.EnforcementKey;
                tranData.MeterStatus = (data.MeterStatus == null) ? null : data.MeterStatus;

                tranData.METERLOC_METERTYPE = (data.METERLOC_METERTYPE == null) ? null : data.METERLOC_METERTYPE;
                tranData.ZoneName = (data.ZoneName == null) ? null : data.ZoneName;

                tranData.MeterID = data.PayStationID.HasValue ? data.PayStationID.Value.ToString() : "0";
                tranData.ReturnCode = data.ReturnCode;

                logData.Response = new ResponseEntity() { CustomerId = param.CustomerId.Value.ToString(), MeterId = tranData.MeterName, SpaceId = tranData.SpaceNo, CityCurrentTime = data.PresentMeterTime.HasValue ? data.PresentMeterTime.Value : DateTime.Now };
            }
            catch (Exception ex)
            {
                var appPath = System.Web.Hosting.HostingEnvironment.ApplicationPhysicalPath;
                string errorPath = Path.Combine(appPath, "GetTransformData", "Timezones");
                ExceptionLogging.SendErrorToText(ex, "GetCitationData", errorPath);
                tranData.ReturnCode = ((int)ReturnCodeEnum.Error).ToString();

            }
            return tranData;
        }
        public static TransactionTransform GetFranklinTransformDataV2(string EnforcementKey, string filePath, RequestResponseEntity logData, CustomerTime customerCurrentTime)
        {
            TransactionTransform tranData = new TransactionTransform();
            tranData.ReturnCode = ((int)ReturnCodeEnum.InvalidInputParameters).ToString();
            TimeZoneInfo TZ = null;
            TZ = TimeZoneInfo.FindSystemTimeZoneById("Eastern Standard Time");
            //int? customerID = null;
            //string MeterName = null;
            //string parkingSpaceID;
            //string PlateNumber = null;
            //string stateName = null;
            //////////////////////////////////////
            KeyParameter param = PrepareKeyParameters.PrepareFranklin(EnforcementKey);
            if (param.ReturnCode == ((int)ReturnCodeEnum.InvalidInputParameters).ToString())
            {
                return tranData;
            }

            /////////////////////////////////////
            #region DB
            try
            {
                Transaction data = TransactionDataValidatioin.GetDataFranklin(param.CustomerId.Value, param.MeterName, param.parkingSpaceID, param.PlateNumber, param.StateName, EnforcementKey, filePath, logData);


                if (data.TimeZoneId.HasValue)
                    TZ = Utility.GetTimeZoneInfo(data.TimeZoneId.Value, filePath, param.CustomerId.Value.ToString());

                //For South Miami : PayStationId. Others it is MeterID
                tranData.MeterName = (data.MeterName == null) ? string.Empty : data.MeterName;
                tranData.SpaceNo = data.ParkingSpaceID.ToString();

                tranData.MeterStreet = (data.MeterStreet == null) ? string.Empty : data.MeterStreet;

                DateTime _MeterRTC = data.PresentMeterTime.HasValue ? data.PresentMeterTime.Value : DateTime.Now;
                tranData.MeterRTC = String.Format("{0:yyyyMMdd_THHmmssZ}", Utility.findGMTTime(_MeterRTC, TZ));

                DateTime _SensorEventTime = data.SensorEventTime.HasValue ? data.SensorEventTime.Value : DateTime.Now;
                tranData.MeterLastUpdateTime = String.Format("{0:yyyyMMdd_THHmmssZ}", Utility.findGMTTime(_SensorEventTime, TZ));

                tranData.SensorEventTime = String.Format("{0:yyyyMMdd_THHmmssZ}", Utility.findGMTTime(_SensorEventTime, TZ));

                DateTime _endTime = data.ExpiryTime.HasValue ? data.ExpiryTime.Value : DateTime.Now;
                tranData.ExpiredTime = String.Format("{0:yyyyMMdd_THHmmssZ}", Utility.findGMTTime(_endTime, TZ));

                tranData.MeterExpiredMinutes = data.ExpiredMinutes.HasValue ? data.ExpiredMinutes.Value.ToString() : "0";
                tranData.METERLOC_BLOCKNUMBER = (data.Block == null) ? string.Empty : data.Block;
                tranData.METERLOC_STREETDIRECTION = (data.Direction == null) ? string.Empty : data.Direction;
                tranData.METERLOC_STREETNAME = (data.MeterStreet == null) ? string.Empty : data.MeterStreet;
                tranData.METERLOC_STREETTYPE = (data.StreetType == null) ? string.Empty : data.StreetType;
                tranData.METERLOC_CROSSSTREET1 = (data.CrossStreet1 == null) ? string.Empty : data.CrossStreet1;
                tranData.METERLOC_CROSSSTREET2 = (data.CrossStreet2 == null) ? string.Empty : data.CrossStreet2;
                tranData.METERLOC_ENFORCEMENTHOURSDESC = (data.EnfHourDesc == null) ? string.Empty : data.EnfHourDesc;
                tranData.LicensePlate = (data.PlateNumber == null) ? string.Empty : data.PlateNumber;
                tranData.State = (data.LPLocation == null) ? string.Empty : data.LPLocation;
                tranData.Make = (data.VMake == null) ? string.Empty : data.VMake;
                tranData.Model = (data.VModel == null) ? string.Empty : data.VModel;
                tranData.IsOccupied = Utility.getStatus((data.SpaceStatus == null) ? string.Empty : data.SpaceStatus).ToString();
                tranData.EnforcementKey = (data.EnforcementKey == null) ? string.Empty : data.EnforcementKey;
                tranData.MeterStatus = (data.MeterStatus == null) ? null : data.MeterStatus;

                tranData.METERLOC_METERTYPE = (data.METERLOC_METERTYPE == null) ? null : data.METERLOC_METERTYPE;
                tranData.ZoneName = (data.ZoneName == null) ? null : data.ZoneName;

                tranData.MeterID = data.PayStationID.HasValue ? data.PayStationID.Value.ToString() : "0";
                tranData.ReturnCode = data.ReturnCode;

                logData.Response = new ResponseEntity() { CustomerId = param.CustomerId.Value.ToString(), MeterId = tranData.MeterName, SpaceId = tranData.SpaceNo, CityCurrentTime = data.PresentMeterTime.HasValue ? data.PresentMeterTime.Value : DateTime.Now };
            }
            catch (Exception ex)
            {
                //var appPath = System.Web.Hosting.HostingEnvironment.ApplicationPhysicalPath;
                //string errorPath = Path.Combine(appPath, "GetTransformData", "Timezones");
                //ExceptionLogging.SendErrorToText(ex, "GetCitationData", errorPath);
                //tranData.ReturnCode = ((int)ReturnCodeEnum.Error).ToString();

                tranData.ReturnCode = ((int)ReturnCodeEnum.Error).ToString();
            }
            #endregion
            #region "Cale Service Call"
            /////////////////////////////////////////////////
            TransactionTransform tranDataOfService = new TransactionTransform();
            try
            {
                //////////////////////////////////////////
                logData.Request.CustomerId = param.CustomerId.Value.ToString();
                logData.Request.SpaceId = param.parkingSpaceID;
                logData.Request.MeterId = param.MeterName;
                //////////////////////////////////////////////////
                DateTime? currentCustTime = customerCurrentTime.CurrentTime;
                Cale.CaleCustomerElement customer = Cale.CaleCustomerRetriever.GetCustomer(param.CustomerId.Value);
                Cale.CaleGenericFactory factory = new Cale.CaleGenericFactory(customer, param.PlateNumber, null);
                var caleResLog = new VendorResponseEntity();
                logData.VendorData.Add(VendorNames.Cale, caleResLog);
                if (customer.EnforcementType == EnumEnforcementType.PayByPlate.ToString())
                    tranDataOfService = factory.GetRefreshPlateData(ref caleResLog);
                //tranDataOfService = factory.GetRefreshPlateData();
                tranDataOfService.EnforcementKey = EnforcementKey;

                ///////////////////////////////////////////////////
                logData.Response = new ResponseEntity() { CustomerId = param.CustomerId.Value.ToString(), MeterId = tranData.MeterName, SpaceId = tranData.SpaceNo, CityCurrentTime = currentCustTime.HasValue ? currentCustTime.Value : DateTime.Now };
            }
            catch (Exception ex)
            {
                logData.Response = new ResponseEntity() { CustomerId = param.CustomerId.Value.ToString(), MeterId = tranData.MeterName, SpaceId = tranData.SpaceNo, CityCurrentTime = customerCurrentTime.CurrentTime.HasValue ? customerCurrentTime.CurrentTime.Value : DateTime.Now };
                //throw ex;
                tranData.ReturnCode = ((int)ReturnCodeEnum.Error).ToString();
            }
            finally
            {

            }
            #endregion
            List<TransactionTransform> trans = new List<TransactionTransform>();
            trans.Add(tranDataOfService); //Cale
            trans.Add(tranData); // Database

            var rightValue = trans.Where(t => t.ReturnCode == ((int)ReturnCodeEnum.Success).ToString()).OrderByDescending(t => t.MeterExpiredMinutes.ToNullableInt()).FirstOrDefault<TransactionTransform>();
            if (rightValue == default(TransactionTransform))
            {
                //Do nothing.just return database  object.
            }
            else
            {
                tranData = rightValue;
            }

            return tranData;
        }
        private static Transaction GetDataFranklin(int customerID, string MeterName, string parkingSpaceID, string PlateNumber, string statename, string EnforcementKey, string filePath, RequestResponseEntity logData)
        {
            string ZoneID = null;
            string SpaceStatus = null;
            Transaction data = new Transaction();
            try
            {
                List<Transaction> transactionList = new List<Transaction>();

                logData.Request.CustomerId = customerID.ToString();
                logData.Request.SpaceId = parkingSpaceID;
                logData.Request.MeterId = MeterName;

                transactionList = DataAccess.GetFranklinCitationData(customerID, null, MeterName, parkingSpaceID, PlateNumber, statename, null);

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
            }
            catch (Exception ex)
            {
                var appPath = System.Web.Hosting.HostingEnvironment.ApplicationPhysicalPath;
                string errorPath = Path.Combine(appPath, "GetData", "Timezones");
                ExceptionLogging.SendErrorToText(ex, "GetCitationData", errorPath);
                data.ReturnCode = ((int)ReturnCodeEnum.Error).ToString();
                throw;
            }
            return data;
        }

        #endregion

        #region Sunny Isles Beach
        public static TransactionTransform GetSunnyIslesBeachTransformData(string EnforcementKey, string filePath, RequestResponseEntity logData)
        {
            TransactionTransform tranData = new TransactionTransform();
            tranData.ReturnCode = ((int)ReturnCodeEnum.InvalidInputParameters).ToString();
            TimeZoneInfo TZ = null;
            TZ = TimeZoneInfo.FindSystemTimeZoneById("Eastern Standard Time");
            //int? customerID = null;
            //string MeterName = null;
            //string parkingSpaceID;
            //string PlateNumber = null;
            //string stateName = null;
            //////////////////////////////////////
            KeyParameter param = PrepareKeyParameters.PrepareSunnyIslesBeach(EnforcementKey);
            if (param.ReturnCode == ((int)ReturnCodeEnum.InvalidInputParameters).ToString())
            {
                return tranData;
            }

            /////////////////////////////////////
            try
            {
                Transaction data = TransactionDataValidatioin.GetDataSunnyIslesBeach(param.CustomerId.Value, param.MeterName, param.parkingSpaceID, param.PlateNumber, param.StateName, EnforcementKey, filePath, logData);


                if (data.TimeZoneId.HasValue)
                    TZ = Utility.GetTimeZoneInfo(data.TimeZoneId.Value, filePath, param.CustomerId.Value.ToString());

                //For South Miami : PayStationId. Others it is MeterID
                tranData.MeterName = (data.MeterName == null) ? string.Empty : data.MeterName;
                tranData.SpaceNo = data.ParkingSpaceID.ToString();

                tranData.MeterStreet = (data.MeterStreet == null) ? string.Empty : data.MeterStreet;

                DateTime _MeterRTC = data.PresentMeterTime.HasValue ? data.PresentMeterTime.Value : DateTime.Now;
                tranData.MeterRTC = String.Format("{0:yyyyMMdd_THHmmssZ}", Utility.findGMTTime(_MeterRTC, TZ));

                DateTime _SensorEventTime = data.SensorEventTime.HasValue ? data.SensorEventTime.Value : DateTime.Now;
                tranData.MeterLastUpdateTime = String.Format("{0:yyyyMMdd_THHmmssZ}", Utility.findGMTTime(_SensorEventTime, TZ));

                tranData.SensorEventTime = String.Format("{0:yyyyMMdd_THHmmssZ}", Utility.findGMTTime(_SensorEventTime, TZ));

                DateTime _endTime = data.ExpiryTime.HasValue ? data.ExpiryTime.Value : DateTime.Now;
                tranData.ExpiredTime = String.Format("{0:yyyyMMdd_THHmmssZ}", Utility.findGMTTime(_endTime, TZ));

                tranData.MeterExpiredMinutes = data.ExpiredMinutes.HasValue ? data.ExpiredMinutes.Value.ToString() : "0";
                tranData.METERLOC_BLOCKNUMBER = (data.Block == null) ? string.Empty : data.Block;
                tranData.METERLOC_STREETDIRECTION = (data.Direction == null) ? string.Empty : data.Direction;
                tranData.METERLOC_STREETNAME = (data.MeterStreet == null) ? string.Empty : data.MeterStreet;
                tranData.METERLOC_STREETTYPE = (data.StreetType == null) ? string.Empty : data.StreetType;
                tranData.METERLOC_CROSSSTREET1 = (data.CrossStreet1 == null) ? string.Empty : data.CrossStreet1;
                tranData.METERLOC_CROSSSTREET2 = (data.CrossStreet2 == null) ? string.Empty : data.CrossStreet2;
                tranData.METERLOC_ENFORCEMENTHOURSDESC = (data.EnfHourDesc == null) ? string.Empty : data.EnfHourDesc;
                tranData.LicensePlate = (data.PlateNumber == null) ? string.Empty : data.PlateNumber;
                tranData.State = (data.LPLocation == null) ? string.Empty : data.LPLocation;
                tranData.Make = (data.VMake == null) ? string.Empty : data.VMake;
                tranData.Model = (data.VModel == null) ? string.Empty : data.VModel;
                tranData.IsOccupied = Utility.getStatus((data.SpaceStatus == null) ? string.Empty : data.SpaceStatus).ToString();
                tranData.EnforcementKey = (data.EnforcementKey == null) ? string.Empty : data.EnforcementKey;
                tranData.MeterStatus = (data.MeterStatus == null) ? null : data.MeterStatus;

                tranData.METERLOC_METERTYPE = (data.METERLOC_METERTYPE == null) ? null : data.METERLOC_METERTYPE;
                tranData.ZoneName = (data.ZoneName == null) ? null : data.ZoneName;

                tranData.MeterID = data.PayStationID.HasValue ? data.PayStationID.Value.ToString() : "0";
                tranData.ReturnCode = data.ReturnCode;

                logData.Response = new ResponseEntity() { CustomerId = param.CustomerId.Value.ToString(), MeterId = tranData.MeterName, SpaceId = tranData.SpaceNo, CityCurrentTime = data.PresentMeterTime.HasValue ? data.PresentMeterTime.Value : DateTime.Now };
            }
            catch (Exception ex)
            {
                var appPath = System.Web.Hosting.HostingEnvironment.ApplicationPhysicalPath;
                string errorPath = Path.Combine(appPath, "GetTransformData", "Timezones");
                ExceptionLogging.SendErrorToText(ex, "GetCitationData", errorPath);
                tranData.ReturnCode = ((int)ReturnCodeEnum.Error).ToString();

            }
            return tranData;
        }
        public static TransactionTransform GetSunnyIslesBeachTransformDataV2(string EnforcementKey, string filePath, RequestResponseEntity logData, CustomerTime customerCurrentTime)
        {
            TransactionTransform tranData = new TransactionTransform();
            tranData.ReturnCode = ((int)ReturnCodeEnum.InvalidInputParameters).ToString();
            TimeZoneInfo TZ = null;
            TZ = TimeZoneInfo.FindSystemTimeZoneById("Eastern Standard Time");
            //int? customerID = null;
            //string MeterName = null;
            //string parkingSpaceID;
            //string PlateNumber = null;
            //string stateName = null;
            //////////////////////////////////////
            KeyParameter param = PrepareKeyParameters.PrepareSunnyIslesBeach(EnforcementKey);
            if (param.ReturnCode == ((int)ReturnCodeEnum.InvalidInputParameters).ToString())
            {
                return tranData;
            }

            /////////////////////////////////////
            #region "DB Check"
            try
            {
                Transaction data = TransactionDataValidatioin.GetDataSunnyIslesBeach(param.CustomerId.Value, param.MeterName, param.parkingSpaceID, param.PlateNumber, param.StateName, EnforcementKey, filePath, logData);


                if (data.TimeZoneId.HasValue)
                    TZ = Utility.GetTimeZoneInfo(data.TimeZoneId.Value, filePath, param.CustomerId.Value.ToString());

                //For South Miami : PayStationId. Others it is MeterID
                tranData.MeterName = (data.MeterName == null) ? string.Empty : data.MeterName;
                tranData.SpaceNo = data.ParkingSpaceID.ToString();

                tranData.MeterStreet = (data.MeterStreet == null) ? string.Empty : data.MeterStreet;

                DateTime _MeterRTC = data.PresentMeterTime.HasValue ? data.PresentMeterTime.Value : DateTime.Now;
                tranData.MeterRTC = String.Format("{0:yyyyMMdd_THHmmssZ}", Utility.findGMTTime(_MeterRTC, TZ));

                DateTime _SensorEventTime = data.SensorEventTime.HasValue ? data.SensorEventTime.Value : DateTime.Now;
                tranData.MeterLastUpdateTime = String.Format("{0:yyyyMMdd_THHmmssZ}", Utility.findGMTTime(_SensorEventTime, TZ));

                tranData.SensorEventTime = String.Format("{0:yyyyMMdd_THHmmssZ}", Utility.findGMTTime(_SensorEventTime, TZ));

                DateTime _endTime = data.ExpiryTime.HasValue ? data.ExpiryTime.Value : DateTime.Now;
                tranData.ExpiredTime = String.Format("{0:yyyyMMdd_THHmmssZ}", Utility.findGMTTime(_endTime, TZ));

                tranData.MeterExpiredMinutes = data.ExpiredMinutes.HasValue ? data.ExpiredMinutes.Value.ToString() : "0";
                tranData.METERLOC_BLOCKNUMBER = (data.Block == null) ? string.Empty : data.Block;
                tranData.METERLOC_STREETDIRECTION = (data.Direction == null) ? string.Empty : data.Direction;
                tranData.METERLOC_STREETNAME = (data.MeterStreet == null) ? string.Empty : data.MeterStreet;
                tranData.METERLOC_STREETTYPE = (data.StreetType == null) ? string.Empty : data.StreetType;
                tranData.METERLOC_CROSSSTREET1 = (data.CrossStreet1 == null) ? string.Empty : data.CrossStreet1;
                tranData.METERLOC_CROSSSTREET2 = (data.CrossStreet2 == null) ? string.Empty : data.CrossStreet2;
                tranData.METERLOC_ENFORCEMENTHOURSDESC = (data.EnfHourDesc == null) ? string.Empty : data.EnfHourDesc;
                tranData.LicensePlate = (data.PlateNumber == null) ? string.Empty : data.PlateNumber;
                tranData.State = (data.LPLocation == null) ? string.Empty : data.LPLocation;
                tranData.Make = (data.VMake == null) ? string.Empty : data.VMake;
                tranData.Model = (data.VModel == null) ? string.Empty : data.VModel;
                tranData.IsOccupied = Utility.getStatus((data.SpaceStatus == null) ? string.Empty : data.SpaceStatus).ToString();
                tranData.EnforcementKey = (data.EnforcementKey == null) ? string.Empty : data.EnforcementKey;
                tranData.MeterStatus = (data.MeterStatus == null) ? null : data.MeterStatus;

                tranData.METERLOC_METERTYPE = (data.METERLOC_METERTYPE == null) ? null : data.METERLOC_METERTYPE;
                tranData.ZoneName = (data.ZoneName == null) ? null : data.ZoneName;

                tranData.MeterID = data.PayStationID.HasValue ? data.PayStationID.Value.ToString() : "0";
                tranData.ReturnCode = data.ReturnCode;

                logData.Response = new ResponseEntity() { CustomerId = param.CustomerId.Value.ToString(), MeterId = tranData.MeterName, SpaceId = tranData.SpaceNo, CityCurrentTime = data.PresentMeterTime.HasValue ? data.PresentMeterTime.Value : DateTime.Now };
            }
            catch (Exception ex)
            {
                //var appPath = System.Web.Hosting.HostingEnvironment.ApplicationPhysicalPath;
                //string errorPath = Path.Combine(appPath, "GetTransformData", "Timezones");
                //ExceptionLogging.SendErrorToText(ex, "GetCitationData", errorPath);
                //tranData.ReturnCode = ((int)ReturnCodeEnum.Error).ToString();

                tranData.ReturnCode = ((int)ReturnCodeEnum.Error).ToString();
            }
            #endregion
            #region "PayByPhone Service Call"
            /////////////////////////////////////////////////
            TransactionTransform tranDataOfService = new TransactionTransform();
            try
            {
                //////////////////////////////////////////
                logData.Request.CustomerId = param.CustomerId.Value.ToString();
                logData.Request.SpaceId = param.parkingSpaceID;
                logData.Request.MeterId = param.MeterName;
                //////////////////////////////////////////////////
                DateTime? currentCustTime = customerCurrentTime.CurrentTime;
                CustomerElement customer = CustomerRetriever.GetCustomer(param.CustomerId.Value);
                PayByPhoneGenericFactory factory = new PayByPhoneGenericFactory(customer, param.PlateNumber);
                tranDataOfService = factory.GetRefreshPlateData();
                tranDataOfService.EnforcementKey = EnforcementKey;

                ///////////////////////////////////////////////////
                logData.Response = new ResponseEntity() { CustomerId = param.CustomerId.Value.ToString(), MeterId = tranData.MeterName, SpaceId = tranData.SpaceNo, CityCurrentTime = currentCustTime.HasValue ? currentCustTime.Value : DateTime.Now };

            }
            catch (Exception ex)
            {
                logData.Response = new ResponseEntity() { CustomerId = param.CustomerId.Value.ToString(), MeterId = tranData.MeterName, SpaceId = tranData.SpaceNo, CityCurrentTime = customerCurrentTime.CurrentTime.HasValue ? customerCurrentTime.CurrentTime.Value : DateTime.Now };
                //throw ex;
                tranData.ReturnCode = ((int)ReturnCodeEnum.Error).ToString();
            }
            finally
            {

            }
            #endregion

            //Reconcile data
            if ((tranData.ReturnCode == ((int)ReturnCodeEnum.NoRecordExists).ToString()) || (tranData.ReturnCode == ((int)ReturnCodeEnum.Error).ToString()))
            {
                tranData = tranDataOfService;
            }
            else if (tranData.ReturnCode == ((int)ReturnCodeEnum.Success).ToString())
            {
                if (tranDataOfService.ReturnCode == ((int)ReturnCodeEnum.Success).ToString())
                {
                    ////Select one record of highest expiry time
                    int? dbExpiredMinutes = tranData.MeterExpiredMinutes.ToNullableInt();
                    int? parkeonExpiredMinutes = tranDataOfService.MeterExpiredMinutes.ToNullableInt();
                    tranData = Nullable.Compare(dbExpiredMinutes, parkeonExpiredMinutes) > 0 ? tranData : tranDataOfService;
                }

            }

            else if (tranData.ReturnCode == ((int)ReturnCodeEnum.MoreRecordsExists).ToString())
            {
                // DB never give more record. hence ignore
            }

            return tranData;
        }
        public static TransactionTransform GetSunnyIslesBeachTransformDataV3(string EnforcementKey, string filePath, RequestResponseEntity logData, CustomerTime customerCurrentTime)
        {
            TransactionTransform tranData = new TransactionTransform();
            tranData.ReturnCode = ((int)ReturnCodeEnum.InvalidInputParameters).ToString();
            TimeZoneInfo TZ = null;
            TZ = TimeZoneInfo.FindSystemTimeZoneById("Eastern Standard Time");
            //int? customerID = null;
            //string MeterName = null;
            //string parkingSpaceID;
            //string PlateNumber = null;
            //string stateName = null;
            //////////////////////////////////////
            KeyParameter param = PrepareKeyParameters.PrepareSunnyIslesBeach(EnforcementKey);
            if (param.ReturnCode == ((int)ReturnCodeEnum.InvalidInputParameters).ToString())
            {
                return tranData;
            }

            /////////////////////////////////////
            #region "DB Check"
            try
            {
                Transaction data = TransactionDataValidatioin.GetDataSunnyIslesBeach(param.CustomerId.Value, param.MeterName, param.parkingSpaceID, param.PlateNumber, param.StateName, EnforcementKey, filePath, logData);


                if (data.TimeZoneId.HasValue)
                    TZ = Utility.GetTimeZoneInfo(data.TimeZoneId.Value, filePath, param.CustomerId.Value.ToString());

                //For South Miami : PayStationId. Others it is MeterID
                tranData.MeterName = (data.MeterName == null) ? string.Empty : data.MeterName;
                tranData.SpaceNo = data.ParkingSpaceID.ToString();

                tranData.MeterStreet = (data.MeterStreet == null) ? string.Empty : data.MeterStreet;

                DateTime _MeterRTC = data.PresentMeterTime.HasValue ? data.PresentMeterTime.Value : DateTime.Now;
                tranData.MeterRTC = String.Format("{0:yyyyMMdd_THHmmssZ}", Utility.findGMTTime(_MeterRTC, TZ));

                DateTime _SensorEventTime = data.SensorEventTime.HasValue ? data.SensorEventTime.Value : DateTime.Now;
                tranData.MeterLastUpdateTime = String.Format("{0:yyyyMMdd_THHmmssZ}", Utility.findGMTTime(_SensorEventTime, TZ));

                tranData.SensorEventTime = String.Format("{0:yyyyMMdd_THHmmssZ}", Utility.findGMTTime(_SensorEventTime, TZ));

                DateTime _endTime = data.ExpiryTime.HasValue ? data.ExpiryTime.Value : DateTime.Now;
                tranData.ExpiredTime = String.Format("{0:yyyyMMdd_THHmmssZ}", Utility.findGMTTime(_endTime, TZ));

                tranData.MeterExpiredMinutes = data.ExpiredMinutes.HasValue ? data.ExpiredMinutes.Value.ToString() : "0";
                tranData.METERLOC_BLOCKNUMBER = (data.Block == null) ? string.Empty : data.Block;
                tranData.METERLOC_STREETDIRECTION = (data.Direction == null) ? string.Empty : data.Direction;
                tranData.METERLOC_STREETNAME = (data.MeterStreet == null) ? string.Empty : data.MeterStreet;
                tranData.METERLOC_STREETTYPE = (data.StreetType == null) ? string.Empty : data.StreetType;
                tranData.METERLOC_CROSSSTREET1 = (data.CrossStreet1 == null) ? string.Empty : data.CrossStreet1;
                tranData.METERLOC_CROSSSTREET2 = (data.CrossStreet2 == null) ? string.Empty : data.CrossStreet2;
                tranData.METERLOC_ENFORCEMENTHOURSDESC = (data.EnfHourDesc == null) ? string.Empty : data.EnfHourDesc;
                tranData.LicensePlate = (data.PlateNumber == null) ? string.Empty : data.PlateNumber;
                tranData.State = (data.LPLocation == null) ? string.Empty : data.LPLocation;
                tranData.Make = (data.VMake == null) ? string.Empty : data.VMake;
                tranData.Model = (data.VModel == null) ? string.Empty : data.VModel;
                tranData.IsOccupied = Utility.getStatus((data.SpaceStatus == null) ? string.Empty : data.SpaceStatus).ToString();
                tranData.EnforcementKey = (data.EnforcementKey == null) ? string.Empty : data.EnforcementKey;
                tranData.MeterStatus = (data.MeterStatus == null) ? null : data.MeterStatus;

                tranData.METERLOC_METERTYPE = (data.METERLOC_METERTYPE == null) ? null : data.METERLOC_METERTYPE;
                tranData.ZoneName = (data.ZoneName == null) ? null : data.ZoneName;

                tranData.MeterID = data.PayStationID.HasValue ? data.PayStationID.Value.ToString() : "0";
                tranData.ReturnCode = data.ReturnCode;

                logData.Response = new ResponseEntity() { CustomerId = param.CustomerId.Value.ToString(), MeterId = tranData.MeterName, SpaceId = tranData.SpaceNo, CityCurrentTime = data.PresentMeterTime.HasValue ? data.PresentMeterTime.Value : DateTime.Now };
            }
            catch (Exception ex)
            {
                //var appPath = System.Web.Hosting.HostingEnvironment.ApplicationPhysicalPath;
                //string errorPath = Path.Combine(appPath, "GetTransformData", "Timezones");
                //ExceptionLogging.SendErrorToText(ex, "GetCitationData", errorPath);
                //tranData.ReturnCode = ((int)ReturnCodeEnum.Error).ToString();

                tranData.ReturnCode = ((int)ReturnCodeEnum.Error).ToString();
            }
            #endregion
            #region "PayByPhone Service Call"
            /////////////////////////////////////////////////
            TransactionTransform tranDataOfService = new TransactionTransform();
            try
            {
                //////////////////////////////////////////
                logData.Request.CustomerId = param.CustomerId.Value.ToString();
                logData.Request.SpaceId = param.parkingSpaceID;
                logData.Request.MeterId = param.MeterName;
                //////////////////////////////////////////////////
                DateTime? currentCustTime = customerCurrentTime.CurrentTime;
                CustomerElement customer = CustomerRetriever.GetCustomer(param.CustomerId.Value);
                PayByPhoneGenericFactory factory = new PayByPhoneGenericFactory(customer, param.PlateNumber);
                tranDataOfService = factory.GetRefreshPlateData();
                tranDataOfService.EnforcementKey = EnforcementKey;

                ///////////////////////////////////////////////////
                logData.Response = new ResponseEntity() { CustomerId = param.CustomerId.Value.ToString(), MeterId = tranData.MeterName, SpaceId = tranData.SpaceNo, CityCurrentTime = currentCustTime.HasValue ? currentCustTime.Value : DateTime.Now };

            }
            catch (Exception ex)
            {
                logData.Response = new ResponseEntity() { CustomerId = param.CustomerId.Value.ToString(), MeterId = tranData.MeterName, SpaceId = tranData.SpaceNo, CityCurrentTime = customerCurrentTime.CurrentTime.HasValue ? customerCurrentTime.CurrentTime.Value : DateTime.Now };
                //throw ex;
                tranDataOfService.ReturnCode = ((int)ReturnCodeEnum.Error).ToString();
            }
            finally
            {

            }
            #endregion
            #region T2Digital
            TransactionTransform tranDataOfServiceT2 = new TransactionTransform();
            try
            {
                //////////////////////////////////////////
                logData.Request.CustomerId = param.CustomerId.Value.ToString();
                logData.Request.SpaceId = param.parkingSpaceID;
                logData.Request.MeterId = param.MeterName;
                //////////////////////////////////////////////////
                DateTime? currentCustTime = customerCurrentTime.CurrentTime;
                T2Digital.T2DigitalCustomerElement customer = T2Digital.T2DigitalCustomerRetriever.GetCustomer(param.CustomerId.Value);
                T2Digital.T2DigitalGenericFactory factory = new T2Digital.T2DigitalGenericFactory(customer, param.PlateNumber, string.Empty);
                tranDataOfServiceT2 = factory.GetRefreshPlateData();
                tranDataOfServiceT2.EnforcementKey = EnforcementKey;

                ///////////////////////////////////////////////////
                logData.Response = new ResponseEntity() { CustomerId = param.CustomerId.Value.ToString(), MeterId = tranData.MeterName, SpaceId = tranData.SpaceNo, CityCurrentTime = currentCustTime.HasValue ? currentCustTime.Value : DateTime.Now };

            }
            catch (Exception ex)
            {
                logData.Response = new ResponseEntity() { CustomerId = param.CustomerId.Value.ToString(), MeterId = tranData.MeterName, SpaceId = tranData.SpaceNo, CityCurrentTime = customerCurrentTime.CurrentTime.HasValue ? customerCurrentTime.CurrentTime.Value : DateTime.Now };
                //throw ex;
                tranDataOfServiceT2.ReturnCode = ((int)ReturnCodeEnum.Error).ToString();
            }
            finally
            {

            }
            #endregion
            //////////////////  //Reconcile data
            List<TransactionTransform> trans = new List<TransactionTransform>();
            trans.Add(tranDataOfServiceT2); // T2Digital
            trans.Add(tranData); // Database
            trans.Add(tranDataOfService); //PayByPhone


            var rightValue = trans.Where(t => t.ReturnCode == ((int)ReturnCodeEnum.Success).ToString()).OrderByDescending(t => t.MeterExpiredMinutes.ToNullableInt()).FirstOrDefault<TransactionTransform>();
            if (rightValue == default(TransactionTransform))
            {
                //Do nothing.just return database  object.
            }
            else
            {
                tranData = rightValue;
            }

            return tranData;
        }
        private static Transaction GetDataSunnyIslesBeach(int customerID, string MeterName, string parkingSpaceID, string PlateNumber, string statename, string EnforcementKey, string filePath, RequestResponseEntity logData)
        {
            string ZoneID = null;
            string SpaceStatus = null;
            Transaction data = new Transaction();
            try
            {
                List<Transaction> transactionList = new List<Transaction>();

                logData.Request.CustomerId = customerID.ToString();
                logData.Request.SpaceId = parkingSpaceID;
                logData.Request.MeterId = MeterName;

                transactionList = DataAccess.GetSunnyIslesBeachCitationData(customerID, null, MeterName, parkingSpaceID, PlateNumber, statename, null);

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
            }
            catch (Exception ex)
            {
                var appPath = System.Web.Hosting.HostingEnvironment.ApplicationPhysicalPath;
                string errorPath = Path.Combine(appPath, "GetData", "Timezones");
                ExceptionLogging.SendErrorToText(ex, "GetCitationData", errorPath);
                data.ReturnCode = ((int)ReturnCodeEnum.Error).ToString();
                throw;
            }
            return data;
        }

        #endregion

        #region Surfside
        public static TransactionTransform GetSurfsideTransformData(string EnforcementKey, string filePath, RequestResponseEntity logData)
        {
            TransactionTransform tranData = new TransactionTransform();
            tranData.ReturnCode = ((int)ReturnCodeEnum.InvalidInputParameters).ToString();
            TimeZoneInfo TZ = null;
            TZ = TimeZoneInfo.FindSystemTimeZoneById("Eastern Standard Time");
            //int? customerID = null;
            //string MeterName = null;
            //string parkingSpaceID;
            //string PlateNumber = null;
            //string stateName = null;
            //////////////////////////////////////
            KeyParameter param = PrepareKeyParameters.PrepareSurfside(EnforcementKey);
            if (param.ReturnCode == ((int)ReturnCodeEnum.InvalidInputParameters).ToString())
            {
                return tranData;
            }

            /////////////////////////////////////
            try
            {
                Transaction data = TransactionDataValidatioin.GetDataSurfside(param.CustomerId.Value, param.MeterName, param.parkingSpaceID, param.PlateNumber, param.StateName, EnforcementKey, filePath, logData);


                if (data.TimeZoneId.HasValue)
                    TZ = Utility.GetTimeZoneInfo(data.TimeZoneId.Value, filePath, param.CustomerId.Value.ToString());

                //For South Miami : PayStationId. Others it is MeterID
                tranData.MeterName = (data.MeterName == null) ? string.Empty : data.MeterName;
                tranData.SpaceNo = data.ParkingSpaceID.ToString();

                tranData.MeterStreet = (data.MeterStreet == null) ? string.Empty : data.MeterStreet;

                DateTime _MeterRTC = data.PresentMeterTime.HasValue ? data.PresentMeterTime.Value : DateTime.Now;
                tranData.MeterRTC = String.Format("{0:yyyyMMdd_THHmmssZ}", Utility.findGMTTime(_MeterRTC, TZ));

                DateTime _SensorEventTime = data.SensorEventTime.HasValue ? data.SensorEventTime.Value : DateTime.Now;
                tranData.MeterLastUpdateTime = String.Format("{0:yyyyMMdd_THHmmssZ}", Utility.findGMTTime(_SensorEventTime, TZ));

                tranData.SensorEventTime = String.Format("{0:yyyyMMdd_THHmmssZ}", Utility.findGMTTime(_SensorEventTime, TZ));

                DateTime _endTime = data.ExpiryTime.HasValue ? data.ExpiryTime.Value : DateTime.Now;
                tranData.ExpiredTime = String.Format("{0:yyyyMMdd_THHmmssZ}", Utility.findGMTTime(_endTime, TZ));

                tranData.MeterExpiredMinutes = data.ExpiredMinutes.HasValue ? data.ExpiredMinutes.Value.ToString() : "0";
                tranData.METERLOC_BLOCKNUMBER = (data.Block == null) ? string.Empty : data.Block;
                tranData.METERLOC_STREETDIRECTION = (data.Direction == null) ? string.Empty : data.Direction;
                tranData.METERLOC_STREETNAME = (data.MeterStreet == null) ? string.Empty : data.MeterStreet;
                tranData.METERLOC_STREETTYPE = (data.StreetType == null) ? string.Empty : data.StreetType;
                tranData.METERLOC_CROSSSTREET1 = (data.CrossStreet1 == null) ? string.Empty : data.CrossStreet1;
                tranData.METERLOC_CROSSSTREET2 = (data.CrossStreet2 == null) ? string.Empty : data.CrossStreet2;
                tranData.METERLOC_ENFORCEMENTHOURSDESC = (data.EnfHourDesc == null) ? string.Empty : data.EnfHourDesc;
                tranData.LicensePlate = (data.PlateNumber == null) ? string.Empty : data.PlateNumber;
                tranData.State = (data.LPLocation == null) ? string.Empty : data.LPLocation;
                tranData.Make = (data.VMake == null) ? string.Empty : data.VMake;
                tranData.Model = (data.VModel == null) ? string.Empty : data.VModel;
                tranData.IsOccupied = Utility.getStatus((data.SpaceStatus == null) ? string.Empty : data.SpaceStatus).ToString();
                tranData.EnforcementKey = (data.EnforcementKey == null) ? string.Empty : data.EnforcementKey;
                tranData.MeterStatus = (data.MeterStatus == null) ? null : data.MeterStatus;

                tranData.METERLOC_METERTYPE = (data.METERLOC_METERTYPE == null) ? null : data.METERLOC_METERTYPE;
                tranData.ZoneName = (data.ZoneName == null) ? null : data.ZoneName;

                tranData.MeterID = data.PayStationID.HasValue ? data.PayStationID.Value.ToString() : "0";
                tranData.ReturnCode = data.ReturnCode;

                logData.Response = new ResponseEntity() { CustomerId = param.CustomerId.Value.ToString(), MeterId = tranData.MeterName, SpaceId = tranData.SpaceNo, CityCurrentTime = data.PresentMeterTime.HasValue ? data.PresentMeterTime.Value : DateTime.Now };
            }
            catch (Exception ex)
            {
                var appPath = System.Web.Hosting.HostingEnvironment.ApplicationPhysicalPath;
                string errorPath = Path.Combine(appPath, "GetTransformData", "Timezones");
                ExceptionLogging.SendErrorToText(ex, "GetCitationData", errorPath);
                tranData.ReturnCode = ((int)ReturnCodeEnum.Error).ToString();

            }
            return tranData;
        }
        public static TransactionTransform GetSurfsideTransformDataV2(string EnforcementKey, string filePath, RequestResponseEntity logData, CustomerTime customerCurrentTime)
        {
            TransactionTransform tranData = new TransactionTransform();
            tranData.ReturnCode = ((int)ReturnCodeEnum.InvalidInputParameters).ToString();
            TimeZoneInfo TZ = null;
            TZ = TimeZoneInfo.FindSystemTimeZoneById("Eastern Standard Time");
            //int? customerID = null;
            //string MeterName = null;
            //string parkingSpaceID;
            //string PlateNumber = null;
            //string stateName = null;
            //////////////////////////////////////
            KeyParameter param = PrepareKeyParameters.PrepareSurfside(EnforcementKey);
            if (param.ReturnCode == ((int)ReturnCodeEnum.InvalidInputParameters).ToString())
            {
                return tranData;
            }

            /////////////////////////////////////
            #region "DB Check"
            try
            {
                Transaction data = TransactionDataValidatioin.GetDataSurfside(param.CustomerId.Value, param.MeterName, param.parkingSpaceID, param.PlateNumber, param.StateName, EnforcementKey, filePath, logData);


                if (data.TimeZoneId.HasValue)
                    TZ = Utility.GetTimeZoneInfo(data.TimeZoneId.Value, filePath, param.CustomerId.Value.ToString());

                //For South Miami : PayStationId. Others it is MeterID
                tranData.MeterName = (data.MeterName == null) ? string.Empty : data.MeterName;
                tranData.SpaceNo = data.ParkingSpaceID.ToString();

                tranData.MeterStreet = (data.MeterStreet == null) ? string.Empty : data.MeterStreet;

                DateTime _MeterRTC = data.PresentMeterTime.HasValue ? data.PresentMeterTime.Value : DateTime.Now;
                tranData.MeterRTC = String.Format("{0:yyyyMMdd_THHmmssZ}", Utility.findGMTTime(_MeterRTC, TZ));

                DateTime _SensorEventTime = data.SensorEventTime.HasValue ? data.SensorEventTime.Value : DateTime.Now;
                tranData.MeterLastUpdateTime = String.Format("{0:yyyyMMdd_THHmmssZ}", Utility.findGMTTime(_SensorEventTime, TZ));

                tranData.SensorEventTime = String.Format("{0:yyyyMMdd_THHmmssZ}", Utility.findGMTTime(_SensorEventTime, TZ));

                DateTime _endTime = data.ExpiryTime.HasValue ? data.ExpiryTime.Value : DateTime.Now;
                tranData.ExpiredTime = String.Format("{0:yyyyMMdd_THHmmssZ}", Utility.findGMTTime(_endTime, TZ));

                tranData.MeterExpiredMinutes = data.ExpiredMinutes.HasValue ? data.ExpiredMinutes.Value.ToString() : "0";
                tranData.METERLOC_BLOCKNUMBER = (data.Block == null) ? string.Empty : data.Block;
                tranData.METERLOC_STREETDIRECTION = (data.Direction == null) ? string.Empty : data.Direction;
                tranData.METERLOC_STREETNAME = (data.MeterStreet == null) ? string.Empty : data.MeterStreet;
                tranData.METERLOC_STREETTYPE = (data.StreetType == null) ? string.Empty : data.StreetType;
                tranData.METERLOC_CROSSSTREET1 = (data.CrossStreet1 == null) ? string.Empty : data.CrossStreet1;
                tranData.METERLOC_CROSSSTREET2 = (data.CrossStreet2 == null) ? string.Empty : data.CrossStreet2;
                tranData.METERLOC_ENFORCEMENTHOURSDESC = (data.EnfHourDesc == null) ? string.Empty : data.EnfHourDesc;
                tranData.LicensePlate = (data.PlateNumber == null) ? string.Empty : data.PlateNumber;
                tranData.State = (data.LPLocation == null) ? string.Empty : data.LPLocation;
                tranData.Make = (data.VMake == null) ? string.Empty : data.VMake;
                tranData.Model = (data.VModel == null) ? string.Empty : data.VModel;
                tranData.IsOccupied = Utility.getStatus((data.SpaceStatus == null) ? string.Empty : data.SpaceStatus).ToString();
                tranData.EnforcementKey = (data.EnforcementKey == null) ? string.Empty : data.EnforcementKey;
                tranData.MeterStatus = (data.MeterStatus == null) ? null : data.MeterStatus;

                tranData.METERLOC_METERTYPE = (data.METERLOC_METERTYPE == null) ? null : data.METERLOC_METERTYPE;
                tranData.ZoneName = (data.ZoneName == null) ? null : data.ZoneName;

                tranData.MeterID = data.PayStationID.HasValue ? data.PayStationID.Value.ToString() : "0";
                tranData.ReturnCode = data.ReturnCode;

                logData.Response = new ResponseEntity() { CustomerId = param.CustomerId.Value.ToString(), MeterId = tranData.MeterName, SpaceId = tranData.SpaceNo, CityCurrentTime = data.PresentMeterTime.HasValue ? data.PresentMeterTime.Value : DateTime.Now };
            }
            catch (Exception ex)
            {
                //var appPath = System.Web.Hosting.HostingEnvironment.ApplicationPhysicalPath;
                //string errorPath = Path.Combine(appPath, "GetTransformData", "Timezones");
                //ExceptionLogging.SendErrorToText(ex, "GetCitationData", errorPath);
                //tranData.ReturnCode = ((int)ReturnCodeEnum.Error).ToString();

                tranData.ReturnCode = ((int)ReturnCodeEnum.Error).ToString();
            }
            #endregion
            #region "PayByPhone Service Call"
            /////////////////////////////////////////////////
            TransactionTransform tranDataOfService = new TransactionTransform();
            try
            {
                //////////////////////////////////////////
                logData.Request.CustomerId = param.CustomerId.Value.ToString();
                logData.Request.SpaceId = param.parkingSpaceID;
                logData.Request.MeterId = param.MeterName;
                //////////////////////////////////////////////////
                DateTime? currentCustTime = customerCurrentTime.CurrentTime;
                CustomerElement customer = CustomerRetriever.GetCustomer(param.CustomerId.Value);
                PayByPhoneGenericFactory factory = new PayByPhoneGenericFactory(customer, param.PlateNumber);
                tranDataOfService = factory.GetRefreshPlateData();
                tranDataOfService.EnforcementKey = EnforcementKey;

                ///////////////////////////////////////////////////
                logData.Response = new ResponseEntity() { CustomerId = param.CustomerId.Value.ToString(), MeterId = tranData.MeterName, SpaceId = tranData.SpaceNo, CityCurrentTime = currentCustTime.HasValue ? currentCustTime.Value : DateTime.Now };

            }
            catch (Exception ex)
            {
                logData.Response = new ResponseEntity() { CustomerId = param.CustomerId.Value.ToString(), MeterId = tranData.MeterName, SpaceId = tranData.SpaceNo, CityCurrentTime = customerCurrentTime.CurrentTime.HasValue ? customerCurrentTime.CurrentTime.Value : DateTime.Now };
                //throw ex;
                tranData.ReturnCode = ((int)ReturnCodeEnum.Error).ToString();
            }
            finally
            {

            }
            #endregion
            //Reconcile data
            if ((tranData.ReturnCode == ((int)ReturnCodeEnum.NoRecordExists).ToString()) || (tranData.ReturnCode == ((int)ReturnCodeEnum.Error).ToString()))
            {
                tranData = tranDataOfService;
            }
            else if (tranData.ReturnCode == ((int)ReturnCodeEnum.Success).ToString())
            {
                if (tranDataOfService.ReturnCode == ((int)ReturnCodeEnum.Success).ToString())
                {
                    ////Select one record of highest expiry time
                    int? dbExpiredMinutes = tranData.MeterExpiredMinutes.ToNullableInt();
                    int? parkeonExpiredMinutes = tranDataOfService.MeterExpiredMinutes.ToNullableInt();
                    tranData = Nullable.Compare(dbExpiredMinutes, parkeonExpiredMinutes) > 0 ? tranData : tranDataOfService;
                }
            }

            else if (tranData.ReturnCode == ((int)ReturnCodeEnum.MoreRecordsExists).ToString())
            {
                // DB never give more record. hence ignore
            }

            return tranData;
        }
        public static TransactionTransform GetSurfsideTransformDataV3(string EnforcementKey, string filePath, RequestResponseEntity logData, CustomerTime customerCurrentTime)
        {
            TransactionTransform tranData = new TransactionTransform();
            tranData.ReturnCode = ((int)ReturnCodeEnum.InvalidInputParameters).ToString();
            TimeZoneInfo TZ = null;
            TZ = TimeZoneInfo.FindSystemTimeZoneById("Eastern Standard Time");
            //int? customerID = null;
            //string MeterName = null;
            //string parkingSpaceID;
            //string PlateNumber = null;
            //string stateName = null;
            //////////////////////////////////////
            KeyParameter param = PrepareKeyParameters.PrepareSurfside(EnforcementKey);
            if (param.ReturnCode == ((int)ReturnCodeEnum.InvalidInputParameters).ToString())
            {
                return tranData;
            }

            /////////////////////////////////////
            #region "DB Check"
            try
            {
                Transaction data = TransactionDataValidatioin.GetDataSurfside(param.CustomerId.Value, param.MeterName, param.parkingSpaceID, param.PlateNumber, param.StateName, EnforcementKey, filePath, logData);


                if (data.TimeZoneId.HasValue)
                    TZ = Utility.GetTimeZoneInfo(data.TimeZoneId.Value, filePath, param.CustomerId.Value.ToString());

                //For South Miami : PayStationId. Others it is MeterID
                tranData.MeterName = (data.MeterName == null) ? string.Empty : data.MeterName;
                tranData.SpaceNo = data.ParkingSpaceID.ToString();

                tranData.MeterStreet = (data.MeterStreet == null) ? string.Empty : data.MeterStreet;

                DateTime _MeterRTC = data.PresentMeterTime.HasValue ? data.PresentMeterTime.Value : DateTime.Now;
                tranData.MeterRTC = String.Format("{0:yyyyMMdd_THHmmssZ}", Utility.findGMTTime(_MeterRTC, TZ));

                DateTime _SensorEventTime = data.SensorEventTime.HasValue ? data.SensorEventTime.Value : DateTime.Now;
                tranData.MeterLastUpdateTime = String.Format("{0:yyyyMMdd_THHmmssZ}", Utility.findGMTTime(_SensorEventTime, TZ));

                tranData.SensorEventTime = String.Format("{0:yyyyMMdd_THHmmssZ}", Utility.findGMTTime(_SensorEventTime, TZ));

                DateTime _endTime = data.ExpiryTime.HasValue ? data.ExpiryTime.Value : DateTime.Now;
                tranData.ExpiredTime = String.Format("{0:yyyyMMdd_THHmmssZ}", Utility.findGMTTime(_endTime, TZ));

                tranData.MeterExpiredMinutes = data.ExpiredMinutes.HasValue ? data.ExpiredMinutes.Value.ToString() : "0";
                tranData.METERLOC_BLOCKNUMBER = (data.Block == null) ? string.Empty : data.Block;
                tranData.METERLOC_STREETDIRECTION = (data.Direction == null) ? string.Empty : data.Direction;
                tranData.METERLOC_STREETNAME = (data.MeterStreet == null) ? string.Empty : data.MeterStreet;
                tranData.METERLOC_STREETTYPE = (data.StreetType == null) ? string.Empty : data.StreetType;
                tranData.METERLOC_CROSSSTREET1 = (data.CrossStreet1 == null) ? string.Empty : data.CrossStreet1;
                tranData.METERLOC_CROSSSTREET2 = (data.CrossStreet2 == null) ? string.Empty : data.CrossStreet2;
                tranData.METERLOC_ENFORCEMENTHOURSDESC = (data.EnfHourDesc == null) ? string.Empty : data.EnfHourDesc;
                tranData.LicensePlate = (data.PlateNumber == null) ? string.Empty : data.PlateNumber;
                tranData.State = (data.LPLocation == null) ? string.Empty : data.LPLocation;
                tranData.Make = (data.VMake == null) ? string.Empty : data.VMake;
                tranData.Model = (data.VModel == null) ? string.Empty : data.VModel;
                tranData.IsOccupied = Utility.getStatus((data.SpaceStatus == null) ? string.Empty : data.SpaceStatus).ToString();
                tranData.EnforcementKey = (data.EnforcementKey == null) ? string.Empty : data.EnforcementKey;
                tranData.MeterStatus = (data.MeterStatus == null) ? null : data.MeterStatus;

                tranData.METERLOC_METERTYPE = (data.METERLOC_METERTYPE == null) ? null : data.METERLOC_METERTYPE;
                tranData.ZoneName = (data.ZoneName == null) ? null : data.ZoneName;

                tranData.MeterID = data.PayStationID.HasValue ? data.PayStationID.Value.ToString() : "0";
                tranData.ReturnCode = data.ReturnCode;

                logData.Response = new ResponseEntity() { CustomerId = param.CustomerId.Value.ToString(), MeterId = tranData.MeterName, SpaceId = tranData.SpaceNo, CityCurrentTime = data.PresentMeterTime.HasValue ? data.PresentMeterTime.Value : DateTime.Now };
            }
            catch (Exception ex)
            {
                //var appPath = System.Web.Hosting.HostingEnvironment.ApplicationPhysicalPath;
                //string errorPath = Path.Combine(appPath, "GetTransformData", "Timezones");
                //ExceptionLogging.SendErrorToText(ex, "GetCitationData", errorPath);
                //tranData.ReturnCode = ((int)ReturnCodeEnum.Error).ToString();

                tranData.ReturnCode = ((int)ReturnCodeEnum.Error).ToString();
            }
            #endregion
            #region "PayByPhone Service Call"
            /////////////////////////////////////////////////
            TransactionTransform tranDataOfService = new TransactionTransform();
            try
            {
                //////////////////////////////////////////

                logData.Request.CustomerId = param.CustomerId.Value.ToString();
                logData.Request.SpaceId = param.parkingSpaceID;
                logData.Request.MeterId = param.MeterName;
                //////////////////////////////////////////////////
                DateTime? currentCustTime = customerCurrentTime.CurrentTime;
                CustomerElement customer = CustomerRetriever.GetCustomer(param.CustomerId.Value);
                PayByPhoneGenericFactory factory = new PayByPhoneGenericFactory(customer, param.PlateNumber);
                tranDataOfService = factory.GetRefreshPlateData();
                tranDataOfService.EnforcementKey = EnforcementKey;

                ///////////////////////////////////////////////////
                logData.Response = new ResponseEntity() { CustomerId = param.CustomerId.Value.ToString(), MeterId = tranData.MeterName, SpaceId = tranData.SpaceNo, CityCurrentTime = currentCustTime.HasValue ? currentCustTime.Value : DateTime.Now };

            }
            catch (Exception ex)
            {
                logData.Response = new ResponseEntity() { CustomerId = param.CustomerId.Value.ToString(), MeterId = tranData.MeterName, SpaceId = tranData.SpaceNo, CityCurrentTime = customerCurrentTime.CurrentTime.HasValue ? customerCurrentTime.CurrentTime.Value : DateTime.Now };
                //throw ex;
                tranDataOfService.ReturnCode = ((int)ReturnCodeEnum.Error).ToString();
            }
            finally
            {

            }
            #endregion
            #region T2Digital
            TransactionTransform tranDataOfServiceT2 = new TransactionTransform();
            try
            {
                //////////////////////////////////////////
                logData.Request.CustomerId = param.CustomerId.Value.ToString();
                logData.Request.SpaceId = param.parkingSpaceID;
                logData.Request.MeterId = param.MeterName;
                //////////////////////////////////////////////////
                DateTime? currentCustTime = customerCurrentTime.CurrentTime;
                T2Digital.T2DigitalCustomerElement customer = T2Digital.T2DigitalCustomerRetriever.GetCustomer(param.CustomerId.Value);
                T2Digital.T2DigitalGenericFactory factory = new T2Digital.T2DigitalGenericFactory(customer, param.PlateNumber, string.Empty);
                tranDataOfServiceT2 = factory.GetRefreshPlateData();
                tranDataOfServiceT2.EnforcementKey = EnforcementKey;

                ///////////////////////////////////////////////////
                logData.Response = new ResponseEntity() { CustomerId = param.CustomerId.Value.ToString(), MeterId = tranData.MeterName, SpaceId = tranData.SpaceNo, CityCurrentTime = currentCustTime.HasValue ? currentCustTime.Value : DateTime.Now };

            }
            catch (Exception ex)
            {
                logData.Response = new ResponseEntity() { CustomerId = param.CustomerId.Value.ToString(), MeterId = tranData.MeterName, SpaceId = tranData.SpaceNo, CityCurrentTime = customerCurrentTime.CurrentTime.HasValue ? customerCurrentTime.CurrentTime.Value : DateTime.Now };
                //throw ex;
                tranDataOfServiceT2.ReturnCode = ((int)ReturnCodeEnum.Error).ToString();
            }
            finally
            {

            }
            #endregion
            //////////////////  //Reconcile data
            List<TransactionTransform> trans = new List<TransactionTransform>();
            trans.Add(tranDataOfServiceT2); // T2Digital
            trans.Add(tranData); // Database
            trans.Add(tranDataOfService); //PayByPhone


            var rightValue = trans.Where(t => t.ReturnCode == ((int)ReturnCodeEnum.Success).ToString()).OrderByDescending(t => t.MeterExpiredMinutes.ToNullableInt()).FirstOrDefault<TransactionTransform>();
            if (rightValue == default(TransactionTransform))
            {
                //Do nothing.just return database  object.
            }
            else
            {
                tranData = rightValue;
            }
            return tranData;
        }
        public static TransactionTransform GetSurfsideTransformDataV4(string EnforcementKey, string filePath, RequestResponseEntity logData, CustomerTime customerCurrentTime)
        {
            TransactionTransform tranData = new TransactionTransform();
            tranData.ReturnCode = ((int)ReturnCodeEnum.InvalidInputParameters).ToString();
            TimeZoneInfo TZ = null;
            TZ = TimeZoneInfo.FindSystemTimeZoneById("Eastern Standard Time");
            //int? customerID = null;
            //string MeterName = null;
            //string parkingSpaceID;
            //string PlateNumber = null;
            //string stateName = null;
            //////////////////////////////////////
            KeyParameter param = PrepareKeyParameters.PrepareSurfside(EnforcementKey);
            if (param.ReturnCode == ((int)ReturnCodeEnum.InvalidInputParameters).ToString())
            {
                return tranData;
            }

            /////////////////////////////////////
            #region "DB Check"
            try
            {
                Transaction data = TransactionDataValidatioin.GetDataSurfside(param.CustomerId.Value, param.MeterName, param.parkingSpaceID, param.PlateNumber, param.StateName, EnforcementKey, filePath, logData);


                if (data.TimeZoneId.HasValue)
                    TZ = Utility.GetTimeZoneInfo(data.TimeZoneId.Value, filePath, param.CustomerId.Value.ToString());

                //For South Miami : PayStationId. Others it is MeterID
                tranData.MeterName = (data.MeterName == null) ? string.Empty : data.MeterName;
                tranData.SpaceNo = data.ParkingSpaceID.ToString();

                tranData.MeterStreet = (data.MeterStreet == null) ? string.Empty : data.MeterStreet;

                DateTime _MeterRTC = data.PresentMeterTime.HasValue ? data.PresentMeterTime.Value : DateTime.Now;
                tranData.MeterRTC = String.Format("{0:yyyyMMdd_THHmmssZ}", Utility.findGMTTime(_MeterRTC, TZ));

                DateTime _SensorEventTime = data.SensorEventTime.HasValue ? data.SensorEventTime.Value : DateTime.Now;
                tranData.MeterLastUpdateTime = String.Format("{0:yyyyMMdd_THHmmssZ}", Utility.findGMTTime(_SensorEventTime, TZ));

                tranData.SensorEventTime = String.Format("{0:yyyyMMdd_THHmmssZ}", Utility.findGMTTime(_SensorEventTime, TZ));

                DateTime _endTime = data.ExpiryTime.HasValue ? data.ExpiryTime.Value : DateTime.Now;
                tranData.ExpiredTime = String.Format("{0:yyyyMMdd_THHmmssZ}", Utility.findGMTTime(_endTime, TZ));

                tranData.MeterExpiredMinutes = data.ExpiredMinutes.HasValue ? data.ExpiredMinutes.Value.ToString() : "0";
                tranData.METERLOC_BLOCKNUMBER = (data.Block == null) ? string.Empty : data.Block;
                tranData.METERLOC_STREETDIRECTION = (data.Direction == null) ? string.Empty : data.Direction;
                tranData.METERLOC_STREETNAME = (data.MeterStreet == null) ? string.Empty : data.MeterStreet;
                tranData.METERLOC_STREETTYPE = (data.StreetType == null) ? string.Empty : data.StreetType;
                tranData.METERLOC_CROSSSTREET1 = (data.CrossStreet1 == null) ? string.Empty : data.CrossStreet1;
                tranData.METERLOC_CROSSSTREET2 = (data.CrossStreet2 == null) ? string.Empty : data.CrossStreet2;
                tranData.METERLOC_ENFORCEMENTHOURSDESC = (data.EnfHourDesc == null) ? string.Empty : data.EnfHourDesc;
                tranData.LicensePlate = (data.PlateNumber == null) ? string.Empty : data.PlateNumber;
                tranData.State = (data.LPLocation == null) ? string.Empty : data.LPLocation;
                tranData.Make = (data.VMake == null) ? string.Empty : data.VMake;
                tranData.Model = (data.VModel == null) ? string.Empty : data.VModel;
                tranData.IsOccupied = Utility.getStatus((data.SpaceStatus == null) ? string.Empty : data.SpaceStatus).ToString();
                tranData.EnforcementKey = (data.EnforcementKey == null) ? string.Empty : data.EnforcementKey;
                tranData.MeterStatus = (data.MeterStatus == null) ? null : data.MeterStatus;

                tranData.METERLOC_METERTYPE = (data.METERLOC_METERTYPE == null) ? null : data.METERLOC_METERTYPE;
                tranData.ZoneName = (data.ZoneName == null) ? null : data.ZoneName;

                tranData.MeterID = data.PayStationID.HasValue ? data.PayStationID.Value.ToString() : "0";
                tranData.ReturnCode = data.ReturnCode;

                logData.Response = new ResponseEntity() { CustomerId = param.CustomerId.Value.ToString(), MeterId = tranData.MeterName, SpaceId = tranData.SpaceNo, CityCurrentTime = data.PresentMeterTime.HasValue ? data.PresentMeterTime.Value : DateTime.Now };
            }
            catch (Exception ex)
            {
                //var appPath = System.Web.Hosting.HostingEnvironment.ApplicationPhysicalPath;
                //string errorPath = Path.Combine(appPath, "GetTransformData", "Timezones");
                //ExceptionLogging.SendErrorToText(ex, "GetCitationData", errorPath);
                //tranData.ReturnCode = ((int)ReturnCodeEnum.Error).ToString();

                tranData.ReturnCode = ((int)ReturnCodeEnum.Error).ToString();
            }
            #endregion
            #region "PayByPhone Service Call"
            /////////////////////////////////////////////////
            TransactionTransform tranDataOfService = new TransactionTransform();
            try
            {
                //////////////////////////////////////////

                logData.Request.CustomerId = param.CustomerId.Value.ToString();
                logData.Request.SpaceId = param.parkingSpaceID;
                logData.Request.MeterId = param.MeterName;
                //////////////////////////////////////////////////
                DateTime? currentCustTime = customerCurrentTime.CurrentTime;
                CustomerElement customer = CustomerRetriever.GetCustomer(param.CustomerId.Value);
                PayByPhoneGenericFactory factory = new PayByPhoneGenericFactory(customer, param.PlateNumber);
                var paybyPhoneResLog = new VendorResponseEntity();
                logData.VendorData.Add(VendorNames.PayByPhone, paybyPhoneResLog);
                tranDataOfService = factory.GetRefreshPlateData(ref paybyPhoneResLog);
                tranDataOfService.EnforcementKey = EnforcementKey;

                ///////////////////////////////////////////////////
                logData.Response = new ResponseEntity() { CustomerId = param.CustomerId.Value.ToString(), MeterId = tranData.MeterName, SpaceId = tranData.SpaceNo, CityCurrentTime = currentCustTime.HasValue ? currentCustTime.Value : DateTime.Now };

            }
            catch (Exception ex)
            {
                logData.Response = new ResponseEntity() { CustomerId = param.CustomerId.Value.ToString(), MeterId = tranData.MeterName, SpaceId = tranData.SpaceNo, CityCurrentTime = customerCurrentTime.CurrentTime.HasValue ? customerCurrentTime.CurrentTime.Value : DateTime.Now };
                //throw ex;
                tranDataOfService.ReturnCode = ((int)ReturnCodeEnum.Error).ToString();
            }
            finally
            {

            }
            #endregion
            #region T2Digital
            TransactionTransform tranDataOfServiceT2 = new TransactionTransform();
            try
            {
                //////////////////////////////////////////
                logData.Request.CustomerId = param.CustomerId.Value.ToString();
                logData.Request.SpaceId = param.parkingSpaceID;
                logData.Request.MeterId = param.MeterName;
                //////////////////////////////////////////////////
                DateTime? currentCustTime = customerCurrentTime.CurrentTime;
                T2Digital.T2DigitalCustomerElement customer = T2Digital.T2DigitalCustomerRetriever.GetCustomer(param.CustomerId.Value);
                T2Digital.T2DigitalGenericFactory factory = new T2Digital.T2DigitalGenericFactory(customer, param.PlateNumber, string.Empty);
                var t2ResLog = new VendorResponseEntity();
                logData.VendorData.Add(VendorNames.T2Digital, t2ResLog);
                tranDataOfServiceT2 = factory.GetRefreshPlateData(ref t2ResLog);
                tranDataOfServiceT2.EnforcementKey = EnforcementKey;

                ///////////////////////////////////////////////////
                logData.Response = new ResponseEntity() { CustomerId = param.CustomerId.Value.ToString(), MeterId = tranData.MeterName, SpaceId = tranData.SpaceNo, CityCurrentTime = currentCustTime.HasValue ? currentCustTime.Value : DateTime.Now };

            }
            catch (Exception ex)
            {
                logData.Response = new ResponseEntity() { CustomerId = param.CustomerId.Value.ToString(), MeterId = tranData.MeterName, SpaceId = tranData.SpaceNo, CityCurrentTime = customerCurrentTime.CurrentTime.HasValue ? customerCurrentTime.CurrentTime.Value : DateTime.Now };
                //throw ex;
                tranDataOfServiceT2.ReturnCode = ((int)ReturnCodeEnum.Error).ToString();
            }
            finally
            {

            }
            #endregion
            //////////////////  //Reconcile data
            List<TransactionTransform> trans = new List<TransactionTransform>();
            trans.Add(tranDataOfServiceT2); // T2Digital
            trans.Add(tranData); // Database
            trans.Add(tranDataOfService); //PayByPhone


            var rightValue = trans.Where(t => t.ReturnCode == ((int)ReturnCodeEnum.Success).ToString()).OrderByDescending(t => t.MeterExpiredMinutes.ToNullableInt()).FirstOrDefault<TransactionTransform>();
            if (rightValue == default(TransactionTransform))
            {
                //Do nothing.just return database  object.
            }
            else
            {
                tranData = rightValue;
            }
            return tranData;
        }
        private static Transaction GetDataSurfside(int customerID, string MeterName, string parkingSpaceID, string PlateNumber, string statename, string EnforcementKey, string filePath, RequestResponseEntity logData)
        {
            string ZoneID = null;
            string SpaceStatus = null;
            Transaction data = new Transaction();
            try
            {
                List<Transaction> transactionList = new List<Transaction>();

                logData.Request.CustomerId = customerID.ToString();
                logData.Request.SpaceId = parkingSpaceID;
                logData.Request.MeterId = MeterName;

                transactionList = DataAccess.GetSurfsideBeachCitationData(customerID, null, MeterName, parkingSpaceID, PlateNumber, statename, null);

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
            }
            catch (Exception ex)
            {
                var appPath = System.Web.Hosting.HostingEnvironment.ApplicationPhysicalPath;
                string errorPath = Path.Combine(appPath, "GetData", "Timezones");
                ExceptionLogging.SendErrorToText(ex, "GetCitationData", errorPath);
                data.ReturnCode = ((int)ReturnCodeEnum.Error).ToString();
                throw;
            }
            return data;
        }

        #endregion

        #region NewOrleans
        //public static TransactionTransform GetNewOrleansTransformData(string EnforcementKey, string filePath, RequestResponseEntity logData)
        //{
        //    TransactionTransform tranData = new TransactionTransform();
        //    tranData.ReturnCode = ((int)ReturnCodeEnum.InvalidInputParameters).ToString();
        //    TimeZoneInfo TZ = null;
        //    TZ = TimeZoneInfo.FindSystemTimeZoneById("Eastern Standard Time");
        //    //int? customerID = null;
        //    //string MeterName = null;
        //    //string parkingSpaceID;
        //    //string PlateNumber = null;
        //    //string stateName = null;
        //    //////////////////////////////////////
        //    KeyParameter param = PrepareKeyParameters.PrepareNewOrleans(EnforcementKey);
        //    if (param.ReturnCode == ((int)ReturnCodeEnum.InvalidInputParameters).ToString())
        //    {
        //        return tranData;
        //    }

        //    /////////////////////////////////////
        //    try
        //    {
        //        Transaction data = TransactionDataValidatioin.GetDataNewOrleans(param.CustomerId.Value, param.MeterName, param.parkingSpaceID, param.PlateNumber, param.StateName, EnforcementKey, filePath, logData);


        //        if (data.TimeZoneId.HasValue)
        //            TZ = Utility.GetTimeZoneInfo(data.TimeZoneId.Value, filePath, param.CustomerId.Value.ToString());

        //        //For South Miami : PayStationId. Others it is MeterID
        //        tranData.MeterName = (data.MeterName == null) ? string.Empty : data.MeterName;
        //        tranData.SpaceNo = data.ParkingSpaceID.ToString();

        //        tranData.MeterStreet = (data.MeterStreet == null) ? string.Empty : data.MeterStreet;

        //        DateTime _MeterRTC = data.PresentMeterTime.HasValue ? data.PresentMeterTime.Value : DateTime.Now;
        //        tranData.MeterRTC = String.Format("{0:yyyyMMdd_THHmmssZ}", Utility.findGMTTime(_MeterRTC, TZ));

        //        DateTime _SensorEventTime = data.SensorEventTime.HasValue ? data.SensorEventTime.Value : DateTime.Now;
        //        tranData.MeterLastUpdateTime = String.Format("{0:yyyyMMdd_THHmmssZ}", Utility.findGMTTime(_SensorEventTime, TZ));

        //        tranData.SensorEventTime = String.Format("{0:yyyyMMdd_THHmmssZ}", Utility.findGMTTime(_SensorEventTime, TZ));

        //        DateTime _endTime = data.ExpiryTime.HasValue ? data.ExpiryTime.Value : DateTime.Now;
        //        tranData.ExpiredTime = String.Format("{0:yyyyMMdd_THHmmssZ}", Utility.findGMTTime(_endTime, TZ));

        //        tranData.MeterExpiredMinutes = data.ExpiredMinutes.HasValue ? data.ExpiredMinutes.Value.ToString() : "0";
        //        tranData.METERLOC_BLOCKNUMBER = (data.Block == null) ? string.Empty : data.Block;
        //        tranData.METERLOC_STREETDIRECTION = (data.Direction == null) ? string.Empty : data.Direction;
        //        tranData.METERLOC_STREETNAME = (data.MeterStreet == null) ? string.Empty : data.MeterStreet;
        //        tranData.METERLOC_STREETTYPE = (data.StreetType == null) ? string.Empty : data.StreetType;
        //        tranData.METERLOC_CROSSSTREET1 = (data.CrossStreet1 == null) ? string.Empty : data.CrossStreet1;
        //        tranData.METERLOC_CROSSSTREET2 = (data.CrossStreet2 == null) ? string.Empty : data.CrossStreet2;
        //        tranData.METERLOC_ENFORCEMENTHOURSDESC = (data.EnfHourDesc == null) ? string.Empty : data.EnfHourDesc;
        //        tranData.LicensePlate = (data.PlateNumber == null) ? string.Empty : data.PlateNumber;
        //        tranData.State = (data.LPLocation == null) ? string.Empty : data.LPLocation;
        //        tranData.Make = (data.VMake == null) ? string.Empty : data.VMake;
        //        tranData.Model = (data.VModel == null) ? string.Empty : data.VModel;
        //        tranData.IsOccupied = Utility.getStatus((data.SpaceStatus == null) ? string.Empty : data.SpaceStatus).ToString();
        //        tranData.EnforcementKey = (data.EnforcementKey == null) ? string.Empty : data.EnforcementKey;
        //        tranData.MeterStatus = (data.MeterStatus == null) ? null : data.MeterStatus;

        //        tranData.METERLOC_METERTYPE = (data.METERLOC_METERTYPE == null) ? null : data.METERLOC_METERTYPE;
        //        tranData.ZoneName = (data.ZoneName == null) ? null : data.ZoneName;

        //        tranData.MeterID = data.PayStationID.HasValue ? data.PayStationID.Value.ToString() : "0";
        //        tranData.ReturnCode = data.ReturnCode;

        //        logData.Response = new ResponseEntity() { CustomerId = param.CustomerId.Value.ToString(), MeterId = tranData.MeterName, SpaceId = tranData.SpaceNo, CityCurrentTime = data.PresentMeterTime.HasValue ? data.PresentMeterTime.Value : DateTime.Now };
        //    }
        //    catch (Exception ex)
        //    {
        //        var appPath = System.Web.Hosting.HostingEnvironment.ApplicationPhysicalPath;
        //        string errorPath = Path.Combine(appPath, "GetTransformData", "Timezones");
        //        ExceptionLogging.SendErrorToText(ex, "GetCitationData", errorPath);
        //        tranData.ReturnCode = ((int)ReturnCodeEnum.Error).ToString();

        //    }
        //    return tranData;
        //}
        public static TransactionTransform GetNewOrleansTransformDataV2(string EnforcementKey, string filePath, RequestResponseEntity logData, CustomerTime customerCurrentTime)
        {
            TransactionTransform tranData = new TransactionTransform();
            tranData.ReturnCode = ((int)ReturnCodeEnum.InvalidInputParameters).ToString();
            TimeZoneInfo TZ = null;
            TZ = TimeZoneInfo.FindSystemTimeZoneById("Eastern Standard Time");
            //int? customerID = null;
            //string MeterName = null;
            //string parkingSpaceID;
            //string PlateNumber = null;
            //string stateName = null;
            //////////////////////////////////////
            KeyParameter param = PrepareKeyParameters.PrepareNewOrleans(EnforcementKey);
            if (param.ReturnCode == ((int)ReturnCodeEnum.InvalidInputParameters).ToString())
            {
                return tranData;
            }

            /////////////////////////////////////
            #region "DB Check"
            try
            {
                Transaction data = TransactionDataValidatioin.GetDataNewOrleans(param.CustomerId.Value, param.MeterName, param.parkingSpaceID, param.PlateNumber, param.StateName, EnforcementKey, filePath, logData);


                if (data.TimeZoneId.HasValue)
                    TZ = Utility.GetTimeZoneInfo(data.TimeZoneId.Value, filePath, param.CustomerId.Value.ToString());

                //For South Miami : PayStationId. Others it is MeterID
                tranData.MeterName = (data.MeterName == null) ? string.Empty : data.MeterName;
                tranData.SpaceNo = data.ParkingSpaceID.ToString();

                tranData.MeterStreet = (data.MeterStreet == null) ? string.Empty : data.MeterStreet;

                DateTime _MeterRTC = data.PresentMeterTime.HasValue ? data.PresentMeterTime.Value : DateTime.Now;
                tranData.MeterRTC = String.Format("{0:yyyyMMdd_THHmmssZ}", Utility.findGMTTime(_MeterRTC, TZ));

                DateTime _SensorEventTime = data.SensorEventTime.HasValue ? data.SensorEventTime.Value : DateTime.Now;
                tranData.MeterLastUpdateTime = String.Format("{0:yyyyMMdd_THHmmssZ}", Utility.findGMTTime(_SensorEventTime, TZ));

                tranData.SensorEventTime = String.Format("{0:yyyyMMdd_THHmmssZ}", Utility.findGMTTime(_SensorEventTime, TZ));

                DateTime _endTime = data.ExpiryTime.HasValue ? data.ExpiryTime.Value : DateTime.Now;
                tranData.ExpiredTime = String.Format("{0:yyyyMMdd_THHmmssZ}", Utility.findGMTTime(_endTime, TZ));

                tranData.MeterExpiredMinutes = data.ExpiredMinutes.HasValue ? data.ExpiredMinutes.Value.ToString() : "0";
                tranData.METERLOC_BLOCKNUMBER = (data.Block == null) ? string.Empty : data.Block;
                tranData.METERLOC_STREETDIRECTION = (data.Direction == null) ? string.Empty : data.Direction;
                tranData.METERLOC_STREETNAME = (data.MeterStreet == null) ? string.Empty : data.MeterStreet;
                tranData.METERLOC_STREETTYPE = (data.StreetType == null) ? string.Empty : data.StreetType;
                tranData.METERLOC_CROSSSTREET1 = (data.CrossStreet1 == null) ? string.Empty : data.CrossStreet1;
                tranData.METERLOC_CROSSSTREET2 = (data.CrossStreet2 == null) ? string.Empty : data.CrossStreet2;
                tranData.METERLOC_ENFORCEMENTHOURSDESC = (data.EnfHourDesc == null) ? string.Empty : data.EnfHourDesc;
                tranData.LicensePlate = (data.PlateNumber == null) ? string.Empty : data.PlateNumber;
                tranData.State = (data.LPLocation == null) ? string.Empty : data.LPLocation;
                tranData.Make = (data.VMake == null) ? string.Empty : data.VMake;
                tranData.Model = (data.VModel == null) ? string.Empty : data.VModel;
                tranData.IsOccupied = Utility.getStatus((data.SpaceStatus == null) ? string.Empty : data.SpaceStatus).ToString();
                tranData.EnforcementKey = (data.EnforcementKey == null) ? string.Empty : data.EnforcementKey;
                tranData.MeterStatus = (data.MeterStatus == null) ? null : data.MeterStatus;

                tranData.METERLOC_METERTYPE = (data.METERLOC_METERTYPE == null) ? null : data.METERLOC_METERTYPE;
                tranData.ZoneName = (data.ZoneName == null) ? null : data.ZoneName;

                tranData.MeterID = data.PayStationID.HasValue ? data.PayStationID.Value.ToString() : "0";
                tranData.ReturnCode = data.ReturnCode;

                logData.Response = new ResponseEntity() { CustomerId = param.CustomerId.Value.ToString(), MeterId = tranData.MeterName, SpaceId = tranData.SpaceNo, CityCurrentTime = data.PresentMeterTime.HasValue ? data.PresentMeterTime.Value : DateTime.Now };
            }
            catch (Exception ex)
            {
                //var appPath = System.Web.Hosting.HostingEnvironment.ApplicationPhysicalPath;
                //string errorPath = Path.Combine(appPath, "GetTransformData", "Timezones");
                //ExceptionLogging.SendErrorToText(ex, "GetCitationData", errorPath);
                //tranData.ReturnCode = ((int)ReturnCodeEnum.Error).ToString();

                tranData.ReturnCode = ((int)ReturnCodeEnum.Error).ToString();
            }
            #endregion
            #region "Parkmobile Service Call"
            /////////////////////////////////////////////////
            TransactionTransform tranDataOfServiceParkMobile = new TransactionTransform();
            try
            {
                //////////////////////////////////////////
                logData.Request.CustomerId = param.CustomerId.Value.ToString();
                logData.Request.SpaceId = param.parkingSpaceID;
                logData.Request.MeterId = param.MeterName;
                //////////////////////////////////////////////////
                DateTime? currentCustTime = customerCurrentTime.CurrentTime;
                PMCustomerElement customer = ParkmobileCustomerRetriever.GetCustomer(param.CustomerId.Value);
                ParkMobileGenericFactory factory = new ParkMobileGenericFactory(customer, param.PlateNumber);
                var parkMobileResLog = new VendorResponseEntity();
                logData.VendorData.Add(VendorNames.ParkMobile, parkMobileResLog);
                tranDataOfServiceParkMobile = factory.GetRefreshSpaceData(ParkingMobileDataType.Plate, ref parkMobileResLog);
                tranDataOfServiceParkMobile.EnforcementKey = EnforcementKey;

                ///////////////////////////////////////////////////
                logData.Response = new ResponseEntity() { CustomerId = param.CustomerId.Value.ToString(), MeterId = tranData.MeterName, SpaceId = tranData.SpaceNo, CityCurrentTime = currentCustTime.HasValue ? currentCustTime.Value : DateTime.Now };
            }
            catch (Exception ex)
            {
                logData.Response = new ResponseEntity() { CustomerId = param.CustomerId.Value.ToString(), MeterId = tranData.MeterName, SpaceId = tranData.SpaceNo, CityCurrentTime = customerCurrentTime.CurrentTime.HasValue ? customerCurrentTime.CurrentTime.Value : DateTime.Now };
                //throw ex;
                tranDataOfServiceParkMobile.ReturnCode = ((int)ReturnCodeEnum.Error).ToString();
            }
            finally
            {

            }
            #endregion
            //////////////////  //Reconcile data

            List<TransactionTransform> trans = new List<TransactionTransform>();
            trans.Add(tranDataOfServiceParkMobile); // Parkmobile
            trans.Add(tranData); // Database

            var rightValue = trans.Where(t => t.ReturnCode == ((int)ReturnCodeEnum.Success).ToString()).OrderByDescending(t => t.MeterExpiredMinutes.ToNullableInt()).FirstOrDefault<TransactionTransform>();
            if (rightValue == default(TransactionTransform))
            {
                //Do nothing.just return database  object.
            }
            else
            {
                tranData = rightValue;
            }

            return tranData;
        }
        public static TransactionTransform GetNewOrleansTransformDataV3(string EnforcementKey, string filePath, RequestResponseEntity logData, CustomerTime customerCurrentTime)
        {
            TransactionTransform tranData = new TransactionTransform();
            tranData.ReturnCode = ((int)ReturnCodeEnum.InvalidInputParameters).ToString();
            TimeZoneInfo TZ = null;
            TZ = TimeZoneInfo.FindSystemTimeZoneById("Eastern Standard Time");
            //int? customerID = null;
            //string MeterName = null;
            //string parkingSpaceID;
            //string PlateNumber = null;
            //string stateName = null;
            //////////////////////////////////////
            KeyParameter param = PrepareKeyParameters.PrepareNewOrleans(EnforcementKey);
            if (param.ReturnCode == ((int)ReturnCodeEnum.InvalidInputParameters).ToString())
            {
                return tranData;
            }

            /////////////////////////////////////
            #region "DB Check"
            try
            {
                Transaction data = TransactionDataValidatioin.GetDataNewOrleans(param.CustomerId.Value, param.MeterName, param.parkingSpaceID, param.PlateNumber, param.StateName, EnforcementKey, filePath, logData);


                if (data.TimeZoneId.HasValue)
                    TZ = Utility.GetTimeZoneInfo(data.TimeZoneId.Value, filePath, param.CustomerId.Value.ToString());

                //For South Miami : PayStationId. Others it is MeterID
                tranData.MeterName = (data.MeterName == null) ? string.Empty : data.MeterName;
                tranData.SpaceNo = data.ParkingSpaceID.ToString();

                tranData.MeterStreet = (data.MeterStreet == null) ? string.Empty : data.MeterStreet;

                DateTime _MeterRTC = data.PresentMeterTime.HasValue ? data.PresentMeterTime.Value : DateTime.Now;
                tranData.MeterRTC = String.Format("{0:yyyyMMdd_THHmmssZ}", Utility.findGMTTime(_MeterRTC, TZ));

                DateTime _SensorEventTime = data.SensorEventTime.HasValue ? data.SensorEventTime.Value : DateTime.Now;
                tranData.MeterLastUpdateTime = String.Format("{0:yyyyMMdd_THHmmssZ}", Utility.findGMTTime(_SensorEventTime, TZ));

                tranData.SensorEventTime = String.Format("{0:yyyyMMdd_THHmmssZ}", Utility.findGMTTime(_SensorEventTime, TZ));

                DateTime _endTime = data.ExpiryTime.HasValue ? data.ExpiryTime.Value : DateTime.Now;
                tranData.ExpiredTime = String.Format("{0:yyyyMMdd_THHmmssZ}", Utility.findGMTTime(_endTime, TZ));

                tranData.MeterExpiredMinutes = data.ExpiredMinutes.HasValue ? data.ExpiredMinutes.Value.ToString() : "0";
                tranData.METERLOC_BLOCKNUMBER = (data.Block == null) ? string.Empty : data.Block;
                tranData.METERLOC_STREETDIRECTION = (data.Direction == null) ? string.Empty : data.Direction;
                tranData.METERLOC_STREETNAME = (data.MeterStreet == null) ? string.Empty : data.MeterStreet;
                tranData.METERLOC_STREETTYPE = (data.StreetType == null) ? string.Empty : data.StreetType;
                tranData.METERLOC_CROSSSTREET1 = (data.CrossStreet1 == null) ? string.Empty : data.CrossStreet1;
                tranData.METERLOC_CROSSSTREET2 = (data.CrossStreet2 == null) ? string.Empty : data.CrossStreet2;
                tranData.METERLOC_ENFORCEMENTHOURSDESC = (data.EnfHourDesc == null) ? string.Empty : data.EnfHourDesc;
                tranData.LicensePlate = (data.PlateNumber == null) ? string.Empty : data.PlateNumber;
                tranData.State = (data.LPLocation == null) ? string.Empty : data.LPLocation;
                tranData.Make = (data.VMake == null) ? string.Empty : data.VMake;
                tranData.Model = (data.VModel == null) ? string.Empty : data.VModel;
                tranData.IsOccupied = Utility.getStatus((data.SpaceStatus == null) ? string.Empty : data.SpaceStatus).ToString();
                tranData.EnforcementKey = (data.EnforcementKey == null) ? string.Empty : data.EnforcementKey;
                tranData.MeterStatus = (data.MeterStatus == null) ? null : data.MeterStatus;

                tranData.METERLOC_METERTYPE = (data.METERLOC_METERTYPE == null) ? null : data.METERLOC_METERTYPE;
                tranData.ZoneName = (data.ZoneName == null) ? null : data.ZoneName;

                tranData.MeterID = data.PayStationID.HasValue ? data.PayStationID.Value.ToString() : "0";
                tranData.ReturnCode = data.ReturnCode;

                logData.Response = new ResponseEntity() { CustomerId = param.CustomerId.Value.ToString(), MeterId = tranData.MeterName, SpaceId = tranData.SpaceNo, CityCurrentTime = data.PresentMeterTime.HasValue ? data.PresentMeterTime.Value : DateTime.Now };
            }
            catch (Exception ex)
            {
                //var appPath = System.Web.Hosting.HostingEnvironment.ApplicationPhysicalPath;
                //string errorPath = Path.Combine(appPath, "GetTransformData", "Timezones");
                //ExceptionLogging.SendErrorToText(ex, "GetCitationData", errorPath);
                //tranData.ReturnCode = ((int)ReturnCodeEnum.Error).ToString();

                tranData.ReturnCode = ((int)ReturnCodeEnum.Error).ToString();
            }
            #endregion
            #region "Parkmobile Service Call"
            /////////////////////////////////////////////////
            TransactionTransform tranDataOfServiceParkMobile = new TransactionTransform();
            try
            {
                //////////////////////////////////////////
                logData.Request.CustomerId = param.CustomerId.Value.ToString();
                logData.Request.SpaceId = param.parkingSpaceID;
                logData.Request.MeterId = param.MeterName;
                //////////////////////////////////////////////////
                DateTime? currentCustTime = customerCurrentTime.CurrentTime;
                PMCustomerElement customer = ParkmobileCustomerRetriever.GetCustomer(param.CustomerId.Value);
                ParkMobileGenericFactory factory = new ParkMobileGenericFactory(customer, param.PlateNumber);
                var parkMobileResLog = new VendorResponseEntity();
                logData.VendorData.Add(VendorNames.ParkMobile, parkMobileResLog);
                tranDataOfServiceParkMobile = factory.GetRefreshSpaceData(ParkingMobileDataType.Plate, ref parkMobileResLog);
                tranDataOfServiceParkMobile.EnforcementKey = EnforcementKey;

                ///////////////////////////////////////////////////
                logData.Response = new ResponseEntity() { CustomerId = param.CustomerId.Value.ToString(), MeterId = tranData.MeterName, SpaceId = tranData.SpaceNo, CityCurrentTime = currentCustTime.HasValue ? currentCustTime.Value : DateTime.Now };
            }
            catch (Exception ex)
            {
                logData.Response = new ResponseEntity() { CustomerId = param.CustomerId.Value.ToString(), MeterId = tranData.MeterName, SpaceId = tranData.SpaceNo, CityCurrentTime = customerCurrentTime.CurrentTime.HasValue ? customerCurrentTime.CurrentTime.Value : DateTime.Now };
                //throw ex;
                tranDataOfServiceParkMobile.ReturnCode = ((int)ReturnCodeEnum.Error).ToString();
            }
            finally
            {

            }
            #endregion
            #region "Passport Monitoring"
            /////////////////////////////////////////////////
            TransactionTransform tranDataOfPassport = new TransactionTransform();
            try
            {
                //////////////////////////////////////////
                logData.Request.CustomerId = param.CustomerId.Value.ToString();
                logData.Request.SpaceId = param.parkingSpaceID;
                logData.Request.MeterId = param.MeterName;
                //////////////////////////////////////////////////
                DateTime? currentCustTime = customerCurrentTime.CurrentTime;
                PassportMonitoringCustomerElement customer = PassportMonitoringCustomerRetriever.GetCustomer(param.CustomerId.Value);
                LookUpLPNFactory factory = new LookUpLPNFactory(customer, param.PlateNumber);
                var passportMonResLog = new VendorResponseEntity();
                logData.VendorData.Add(VendorNames.PassportMonitoring, passportMonResLog);
                tranDataOfPassport = factory.GetRefreshPlateData(ref passportMonResLog);
                tranDataOfPassport.EnforcementKey = EnforcementKey;

                ///////////////////////////////////////////////////
                logData.Response = new ResponseEntity() { CustomerId = param.CustomerId.Value.ToString(), MeterId = tranData.MeterName, SpaceId = tranData.SpaceNo, CityCurrentTime = currentCustTime.HasValue ? currentCustTime.Value : DateTime.Now };
            }
            catch (Exception ex)
            {
                logData.Response = new ResponseEntity() { CustomerId = param.CustomerId.Value.ToString(), MeterId = tranData.MeterName, SpaceId = tranData.SpaceNo, CityCurrentTime = customerCurrentTime.CurrentTime.HasValue ? customerCurrentTime.CurrentTime.Value : DateTime.Now };
                //throw ex;
                tranData.ReturnCode = ((int)ReturnCodeEnum.Error).ToString();
            }
            finally
            {

            }
            #endregion
            //////////////////  //Reconcile data

            List<TransactionTransform> trans = new List<TransactionTransform>();
            trans.Add(tranDataOfServiceParkMobile); // Parkmobile
            trans.Add(tranData); // Database
            trans.Add(tranDataOfPassport); // Passport
            var rightValue = trans.Where(t => t.ReturnCode == ((int)ReturnCodeEnum.Success).ToString()).OrderByDescending(t => t.MeterExpiredMinutes.ToNullableInt()).FirstOrDefault<TransactionTransform>();
            if (rightValue == default(TransactionTransform))
            {
                //Do nothing.just return database  object.
            }
            else
            {
                tranData = rightValue;
            }

            return tranData;
        }

        /// <summary>
        /// T2 Added
        /// </summary>
        /// <param name="EnforcementKey"></param>
        /// <param name="filePath"></param>
        /// <param name="logData"></param>
        /// <param name="customerCurrentTime"></param>
        /// <returns></returns>
        public static TransactionTransform GetNewOrleansTransformDataV4(string EnforcementKey, string filePath, RequestResponseEntity logData, CustomerTime customerCurrentTime)
        {
            TransactionTransform tranData = new TransactionTransform();
            tranData.ReturnCode = ((int)ReturnCodeEnum.InvalidInputParameters).ToString();
            TimeZoneInfo TZ = null;
            TZ = TimeZoneInfo.FindSystemTimeZoneById("Eastern Standard Time");
            //int? customerID = null;
            //string MeterName = null;
            //string parkingSpaceID;
            //string PlateNumber = null;
            //string stateName = null;
            //////////////////////////////////////
            KeyParameter param = PrepareKeyParameters.PrepareNewOrleans(EnforcementKey);
            if (param.ReturnCode == ((int)ReturnCodeEnum.InvalidInputParameters).ToString())
            {
                return tranData;
            }

            /////////////////////////////////////
            #region "DB Check"
            try
            {
                Transaction data = TransactionDataValidatioin.GetDataNewOrleans(param.CustomerId.Value, param.MeterName, param.parkingSpaceID, param.PlateNumber, param.StateName, EnforcementKey, filePath, logData);


                if (data.TimeZoneId.HasValue)
                    TZ = Utility.GetTimeZoneInfo(data.TimeZoneId.Value, filePath, param.CustomerId.Value.ToString());

                //For South Miami : PayStationId. Others it is MeterID
                tranData.MeterName = (data.MeterName == null) ? string.Empty : data.MeterName;
                tranData.SpaceNo = data.ParkingSpaceID.ToString();

                tranData.MeterStreet = (data.MeterStreet == null) ? string.Empty : data.MeterStreet;

                DateTime _MeterRTC = data.PresentMeterTime.HasValue ? data.PresentMeterTime.Value : DateTime.Now;
                tranData.MeterRTC = String.Format("{0:yyyyMMdd_THHmmssZ}", Utility.findGMTTime(_MeterRTC, TZ));

                DateTime _SensorEventTime = data.SensorEventTime.HasValue ? data.SensorEventTime.Value : DateTime.Now;
                tranData.MeterLastUpdateTime = String.Format("{0:yyyyMMdd_THHmmssZ}", Utility.findGMTTime(_SensorEventTime, TZ));

                tranData.SensorEventTime = String.Format("{0:yyyyMMdd_THHmmssZ}", Utility.findGMTTime(_SensorEventTime, TZ));

                DateTime _endTime = data.ExpiryTime.HasValue ? data.ExpiryTime.Value : DateTime.Now;
                tranData.ExpiredTime = String.Format("{0:yyyyMMdd_THHmmssZ}", Utility.findGMTTime(_endTime, TZ));

                tranData.MeterExpiredMinutes = data.ExpiredMinutes.HasValue ? data.ExpiredMinutes.Value.ToString() : "0";
                tranData.METERLOC_BLOCKNUMBER = (data.Block == null) ? string.Empty : data.Block;
                tranData.METERLOC_STREETDIRECTION = (data.Direction == null) ? string.Empty : data.Direction;
                tranData.METERLOC_STREETNAME = (data.MeterStreet == null) ? string.Empty : data.MeterStreet;
                tranData.METERLOC_STREETTYPE = (data.StreetType == null) ? string.Empty : data.StreetType;
                tranData.METERLOC_CROSSSTREET1 = (data.CrossStreet1 == null) ? string.Empty : data.CrossStreet1;
                tranData.METERLOC_CROSSSTREET2 = (data.CrossStreet2 == null) ? string.Empty : data.CrossStreet2;
                tranData.METERLOC_ENFORCEMENTHOURSDESC = (data.EnfHourDesc == null) ? string.Empty : data.EnfHourDesc;
                tranData.LicensePlate = (data.PlateNumber == null) ? string.Empty : data.PlateNumber;
                tranData.State = (data.LPLocation == null) ? string.Empty : data.LPLocation;
                tranData.Make = (data.VMake == null) ? string.Empty : data.VMake;
                tranData.Model = (data.VModel == null) ? string.Empty : data.VModel;
                tranData.IsOccupied = Utility.getStatus((data.SpaceStatus == null) ? string.Empty : data.SpaceStatus).ToString();
                tranData.EnforcementKey = (data.EnforcementKey == null) ? string.Empty : data.EnforcementKey;
                tranData.MeterStatus = (data.MeterStatus == null) ? null : data.MeterStatus;

                tranData.METERLOC_METERTYPE = (data.METERLOC_METERTYPE == null) ? null : data.METERLOC_METERTYPE;
                tranData.ZoneName = (data.ZoneName == null) ? null : data.ZoneName;

                tranData.MeterID = data.PayStationID.HasValue ? data.PayStationID.Value.ToString() : "0";
                tranData.ReturnCode = data.ReturnCode;

                logData.Response = new ResponseEntity() { CustomerId = param.CustomerId.Value.ToString(), MeterId = tranData.MeterName, SpaceId = tranData.SpaceNo, CityCurrentTime = data.PresentMeterTime.HasValue ? data.PresentMeterTime.Value : DateTime.Now };
            }
            catch (Exception ex)
            {
                //var appPath = System.Web.Hosting.HostingEnvironment.ApplicationPhysicalPath;
                //string errorPath = Path.Combine(appPath, "GetTransformData", "Timezones");
                //ExceptionLogging.SendErrorToText(ex, "GetCitationData", errorPath);
                //tranData.ReturnCode = ((int)ReturnCodeEnum.Error).ToString();

                tranData.ReturnCode = ((int)ReturnCodeEnum.Error).ToString();
            }
            #endregion
            #region "Parkmobile Service Call"
            /////////////////////////////////////////////////
            TransactionTransform tranDataOfServiceParkMobile = new TransactionTransform();
            try
            {
                //////////////////////////////////////////
                logData.Request.CustomerId = param.CustomerId.Value.ToString();
                logData.Request.SpaceId = param.parkingSpaceID;
                logData.Request.MeterId = param.MeterName;
                //////////////////////////////////////////////////
                DateTime? currentCustTime = customerCurrentTime.CurrentTime;
                PMCustomerElement customer = ParkmobileCustomerRetriever.GetCustomer(param.CustomerId.Value);
                ParkMobileGenericFactory factory = new ParkMobileGenericFactory(customer, param.PlateNumber);
                var parkMobileResLog = new VendorResponseEntity();
                logData.VendorData.Add(VendorNames.ParkMobile, parkMobileResLog);
                tranDataOfServiceParkMobile = factory.GetRefreshSpaceData(ParkingMobileDataType.Plate, ref parkMobileResLog);
                tranDataOfServiceParkMobile.EnforcementKey = EnforcementKey;

                ///////////////////////////////////////////////////
                logData.Response = new ResponseEntity() { CustomerId = param.CustomerId.Value.ToString(), MeterId = tranData.MeterName, SpaceId = tranData.SpaceNo, CityCurrentTime = currentCustTime.HasValue ? currentCustTime.Value : DateTime.Now };
            }
            catch (Exception ex)
            {
                logData.Response = new ResponseEntity() { CustomerId = param.CustomerId.Value.ToString(), MeterId = tranData.MeterName, SpaceId = tranData.SpaceNo, CityCurrentTime = customerCurrentTime.CurrentTime.HasValue ? customerCurrentTime.CurrentTime.Value : DateTime.Now };
                //throw ex;
                tranDataOfServiceParkMobile.ReturnCode = ((int)ReturnCodeEnum.Error).ToString();
            }
            finally
            {

            }
            #endregion
            #region "Passport Monitoring"
            /////////////////////////////////////////////////
            TransactionTransform tranDataOfPassport = new TransactionTransform();
            try
            {
                //////////////////////////////////////////
                logData.Request.CustomerId = param.CustomerId.Value.ToString();
                logData.Request.SpaceId = param.parkingSpaceID;
                logData.Request.MeterId = param.MeterName;
                //////////////////////////////////////////////////
                DateTime? currentCustTime = customerCurrentTime.CurrentTime;
                PassportMonitoringCustomerElement customer = PassportMonitoringCustomerRetriever.GetCustomer(param.CustomerId.Value);
                LookUpLPNFactory factory = new LookUpLPNFactory(customer, param.PlateNumber);
                var passportMonResLog = new VendorResponseEntity();
                logData.VendorData.Add(VendorNames.PassportMonitoring, passportMonResLog);
                tranDataOfPassport = factory.GetRefreshPlateData(ref passportMonResLog);
                tranDataOfPassport.EnforcementKey = EnforcementKey;

                ///////////////////////////////////////////////////
                logData.Response = new ResponseEntity() { CustomerId = param.CustomerId.Value.ToString(), MeterId = tranData.MeterName, SpaceId = tranData.SpaceNo, CityCurrentTime = currentCustTime.HasValue ? currentCustTime.Value : DateTime.Now };
            }
            catch (Exception ex)
            {
                logData.Response = new ResponseEntity() { CustomerId = param.CustomerId.Value.ToString(), MeterId = tranData.MeterName, SpaceId = tranData.SpaceNo, CityCurrentTime = customerCurrentTime.CurrentTime.HasValue ? customerCurrentTime.CurrentTime.Value : DateTime.Now };
                //throw ex;
                tranData.ReturnCode = ((int)ReturnCodeEnum.Error).ToString();
            }
            finally
            {

            }
            #endregion
            #region T2Digital
            TransactionTransform tranDataOfServiceT2 = new TransactionTransform();
            try
            {
                //////////////////////////////////////////
                logData.Request.CustomerId = param.CustomerId.Value.ToString();
                logData.Request.SpaceId = param.parkingSpaceID;
                logData.Request.MeterId = param.MeterName;
                //////////////////////////////////////////////////
                DateTime? currentCustTime = customerCurrentTime.CurrentTime;
                T2Digital.T2DigitalCustomerElement customer = T2Digital.T2DigitalCustomerRetriever.GetCustomer(param.CustomerId.Value);
                T2Digital.T2DigitalGenericFactory factory = new T2Digital.T2DigitalGenericFactory(customer, param.PlateNumber, string.Empty);
                var t2ResLog = new VendorResponseEntity();
                logData.VendorData.Add(VendorNames.T2Digital, t2ResLog);
                tranDataOfServiceT2 = factory.GetRefreshPlateData(ref t2ResLog);
                tranDataOfServiceT2.EnforcementKey = EnforcementKey;

                ///////////////////////////////////////////////////
                logData.Response = new ResponseEntity() { CustomerId = param.CustomerId.Value.ToString(), MeterId = tranData.MeterName, SpaceId = tranData.SpaceNo, CityCurrentTime = currentCustTime.HasValue ? currentCustTime.Value : DateTime.Now };

            }
            catch (Exception ex)
            {
                logData.Response = new ResponseEntity() { CustomerId = param.CustomerId.Value.ToString(), MeterId = tranData.MeterName, SpaceId = tranData.SpaceNo, CityCurrentTime = customerCurrentTime.CurrentTime.HasValue ? customerCurrentTime.CurrentTime.Value : DateTime.Now };
                //throw ex;
                tranDataOfServiceT2.ReturnCode = ((int)ReturnCodeEnum.Error).ToString();
            }
            finally
            {

            }
            #endregion
            //////////////////  //Reconcile data

            List<TransactionTransform> trans = new List<TransactionTransform>();
            trans.Add(tranDataOfServiceParkMobile); // Parkmobile
            trans.Add(tranData); // Database
            trans.Add(tranDataOfPassport); // Passport
            trans.Add(tranDataOfServiceT2); // T2 digital
            var rightValue = trans.Where(t => t.ReturnCode == ((int)ReturnCodeEnum.Success).ToString()).OrderByDescending(t => t.MeterExpiredMinutes.ToNullableInt()).FirstOrDefault<TransactionTransform>();
            if (rightValue == default(TransactionTransform))
            {
                //Do nothing.just return database  object.
            }
            else
            {
                tranData = rightValue;
            }

            return tranData;
        }
        private static Transaction GetDataNewOrleans(int customerID, string MeterName, string parkingSpaceID, string PlateNumber, string statename, string EnforcementKey, string filePath, RequestResponseEntity logData)
        {
            string ZoneID = null;
            string SpaceStatus = null;
            Transaction data = new Transaction();
            try
            {
                List<Transaction> transactionList = new List<Transaction>();

                logData.Request.CustomerId = customerID.ToString();
                logData.Request.SpaceId = parkingSpaceID;
                logData.Request.MeterId = MeterName;

                transactionList = DataAccess.GetNewOrleansCitationData(customerID, null, MeterName, parkingSpaceID, PlateNumber, statename, null);

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
            }
            catch (Exception ex)
            {
                var appPath = System.Web.Hosting.HostingEnvironment.ApplicationPhysicalPath;
                string errorPath = Path.Combine(appPath, "GetData", "Timezones");
                ExceptionLogging.SendErrorToText(ex, "GetCitationData", errorPath);
                data.ReturnCode = ((int)ReturnCodeEnum.Error).ToString();
                throw;
            }
            return data;
        }

        #endregion

        #region Philadelphia
        public static TransactionTransform GetPhiladelphiaTransformData(string EnforcementKey, string filePath, RequestResponseEntity logData, CustomerTime customerCurrentTime)
        {
            TransactionTransform tranData = new TransactionTransform();
            tranData.ReturnCode = ((int)ReturnCodeEnum.InvalidInputParameters).ToString();
            TimeZoneInfo TZ = null;
            TZ = TimeZoneInfo.FindSystemTimeZoneById("Eastern Standard Time");

            KeyParameter param = PrepareKeyParameters.PreparePhiladelphia(EnforcementKey);
            if (param.ReturnCode == ((int)ReturnCodeEnum.InvalidInputParameters).ToString())
            {
                return tranData;
            }


            #region "DB Check"
            try
            {
                Transaction data = TransactionDataValidatioin.GetDataPhiladelphia(param.CustomerId.Value, param.MeterName, param.parkingSpaceID, param.PlateNumber, param.StateName, EnforcementKey, filePath, logData);


                if (data.TimeZoneId.HasValue)
                    TZ = Utility.GetTimeZoneInfo(data.TimeZoneId.Value, filePath, param.CustomerId.Value.ToString());

                //For South Miami : PayStationId. Others it is MeterID
                tranData.MeterName = (data.MeterName == null) ? string.Empty : data.MeterName;
                tranData.SpaceNo = data.ParkingSpaceID.ToString();

                tranData.MeterStreet = (data.MeterStreet == null) ? string.Empty : data.MeterStreet;

                DateTime _MeterRTC = data.PresentMeterTime.HasValue ? data.PresentMeterTime.Value : DateTime.Now;
                tranData.MeterRTC = String.Format("{0:yyyyMMdd_THHmmssZ}", Utility.findGMTTime(_MeterRTC, TZ));

                DateTime _SensorEventTime = data.SensorEventTime.HasValue ? data.SensorEventTime.Value : DateTime.Now;
                tranData.MeterLastUpdateTime = String.Format("{0:yyyyMMdd_THHmmssZ}", Utility.findGMTTime(_SensorEventTime, TZ));

                tranData.SensorEventTime = String.Format("{0:yyyyMMdd_THHmmssZ}", Utility.findGMTTime(_SensorEventTime, TZ));

                DateTime _endTime = data.ExpiryTime.HasValue ? data.ExpiryTime.Value : DateTime.Now;
                tranData.ExpiredTime = String.Format("{0:yyyyMMdd_THHmmssZ}", Utility.findGMTTime(_endTime, TZ));

                tranData.MeterExpiredMinutes = data.ExpiredMinutes.HasValue ? data.ExpiredMinutes.Value.ToString() : "0";
                tranData.METERLOC_BLOCKNUMBER = (data.Block == null) ? string.Empty : data.Block;
                tranData.METERLOC_STREETDIRECTION = (data.Direction == null) ? string.Empty : data.Direction;
                tranData.METERLOC_STREETNAME = (data.MeterStreet == null) ? string.Empty : data.MeterStreet;
                tranData.METERLOC_STREETTYPE = (data.StreetType == null) ? string.Empty : data.StreetType;
                tranData.METERLOC_CROSSSTREET1 = (data.CrossStreet1 == null) ? string.Empty : data.CrossStreet1;
                tranData.METERLOC_CROSSSTREET2 = (data.CrossStreet2 == null) ? string.Empty : data.CrossStreet2;
                tranData.METERLOC_ENFORCEMENTHOURSDESC = (data.EnfHourDesc == null) ? string.Empty : data.EnfHourDesc;
                tranData.LicensePlate = (data.PlateNumber == null) ? string.Empty : data.PlateNumber;
                tranData.State = (data.LPLocation == null) ? string.Empty : data.LPLocation;
                tranData.Make = (data.VMake == null) ? string.Empty : data.VMake;
                tranData.Model = (data.VModel == null) ? string.Empty : data.VModel;
                tranData.IsOccupied = Utility.getStatus((data.SpaceStatus == null) ? string.Empty : data.SpaceStatus).ToString();
                tranData.EnforcementKey = (data.EnforcementKey == null) ? string.Empty : data.EnforcementKey;
                tranData.MeterStatus = (data.MeterStatus == null) ? null : data.MeterStatus;

                tranData.METERLOC_METERTYPE = (data.METERLOC_METERTYPE == null) ? null : data.METERLOC_METERTYPE;
                tranData.ZoneName = (data.ZoneName == null) ? null : data.ZoneName;

                tranData.MeterID = data.PayStationID.HasValue ? data.PayStationID.Value.ToString() : "0";
                tranData.ReturnCode = data.ReturnCode;

                logData.Response = new ResponseEntity() { CustomerId = param.CustomerId.Value.ToString(), MeterId = tranData.MeterName, SpaceId = tranData.SpaceNo, CityCurrentTime = data.PresentMeterTime.HasValue ? data.PresentMeterTime.Value : DateTime.Now };
            }
            catch (Exception ex)
            {
                //var appPath = System.Web.Hosting.HostingEnvironment.ApplicationPhysicalPath;
                //string errorPath = Path.Combine(appPath, "GetTransformData", "Timezones");
                //ExceptionLogging.SendErrorToText(ex, "GetCitationData", errorPath);
                //tranData.ReturnCode = ((int)ReturnCodeEnum.Error).ToString();

                tranData.ReturnCode = ((int)ReturnCodeEnum.Error).ToString();
            }
            #endregion


            return tranData;
        }

        public static TransactionTransform GetPhiladelphiaTransformDataV2(string EnforcementKey, string filePath, RequestResponseEntity logData, CustomerTime customerCurrentTime)
        {
            TransactionTransform tranData = new TransactionTransform();
            tranData.ReturnCode = ((int)ReturnCodeEnum.InvalidInputParameters).ToString();
            TimeZoneInfo TZ = null;
            TZ = TimeZoneInfo.FindSystemTimeZoneById("Eastern Standard Time");

            KeyParameter param = PrepareKeyParameters.PreparePhiladelphia(EnforcementKey);
            if (param.ReturnCode == ((int)ReturnCodeEnum.InvalidInputParameters).ToString())
            {
                return tranData;
            }


            #region "DB Check"
            try
            {
                Transaction data = TransactionDataValidatioin.GetDataPhiladelphia(param.CustomerId.Value, param.MeterName, param.parkingSpaceID, param.PlateNumber, param.StateName, EnforcementKey, filePath, logData);


                if (data.TimeZoneId.HasValue)
                    TZ = Utility.GetTimeZoneInfo(data.TimeZoneId.Value, filePath, param.CustomerId.Value.ToString());

                //For South Miami : PayStationId. Others it is MeterID
                tranData.MeterName = (data.MeterName == null) ? string.Empty : data.MeterName;
                tranData.SpaceNo = data.ParkingSpaceID.ToString();

                tranData.MeterStreet = (data.MeterStreet == null) ? string.Empty : data.MeterStreet;

                DateTime _MeterRTC = data.PresentMeterTime.HasValue ? data.PresentMeterTime.Value : DateTime.Now;
                tranData.MeterRTC = String.Format("{0:yyyyMMdd_THHmmssZ}", Utility.findGMTTime(_MeterRTC, TZ));

                DateTime _SensorEventTime = data.SensorEventTime.HasValue ? data.SensorEventTime.Value : DateTime.Now;
                tranData.MeterLastUpdateTime = String.Format("{0:yyyyMMdd_THHmmssZ}", Utility.findGMTTime(_SensorEventTime, TZ));

                tranData.SensorEventTime = String.Format("{0:yyyyMMdd_THHmmssZ}", Utility.findGMTTime(_SensorEventTime, TZ));

                DateTime _endTime = data.ExpiryTime.HasValue ? data.ExpiryTime.Value : DateTime.Now;
                tranData.ExpiredTime = String.Format("{0:yyyyMMdd_THHmmssZ}", Utility.findGMTTime(_endTime, TZ));

                tranData.MeterExpiredMinutes = data.ExpiredMinutes.HasValue ? data.ExpiredMinutes.Value.ToString() : "0";
                tranData.METERLOC_BLOCKNUMBER = (data.Block == null) ? string.Empty : data.Block;
                tranData.METERLOC_STREETDIRECTION = (data.Direction == null) ? string.Empty : data.Direction;
                tranData.METERLOC_STREETNAME = (data.MeterStreet == null) ? string.Empty : data.MeterStreet;
                tranData.METERLOC_STREETTYPE = (data.StreetType == null) ? string.Empty : data.StreetType;
                tranData.METERLOC_CROSSSTREET1 = (data.CrossStreet1 == null) ? string.Empty : data.CrossStreet1;
                tranData.METERLOC_CROSSSTREET2 = (data.CrossStreet2 == null) ? string.Empty : data.CrossStreet2;
                tranData.METERLOC_ENFORCEMENTHOURSDESC = (data.EnfHourDesc == null) ? string.Empty : data.EnfHourDesc;
                tranData.LicensePlate = (data.PlateNumber == null) ? string.Empty : data.PlateNumber;
                tranData.State = (data.LPLocation == null) ? string.Empty : data.LPLocation;
                tranData.Make = (data.VMake == null) ? string.Empty : data.VMake;
                tranData.Model = (data.VModel == null) ? string.Empty : data.VModel;
                tranData.IsOccupied = Utility.getStatus((data.SpaceStatus == null) ? string.Empty : data.SpaceStatus).ToString();
                tranData.EnforcementKey = (data.EnforcementKey == null) ? string.Empty : data.EnforcementKey;
                tranData.MeterStatus = (data.MeterStatus == null) ? null : data.MeterStatus;

                tranData.METERLOC_METERTYPE = (data.METERLOC_METERTYPE == null) ? null : data.METERLOC_METERTYPE;
                tranData.ZoneName = (data.ZoneName == null) ? null : data.ZoneName;

                tranData.MeterID = data.PayStationID.HasValue ? data.PayStationID.Value.ToString() : "0";
                tranData.ReturnCode = data.ReturnCode;

                logData.Response = new ResponseEntity() { CustomerId = param.CustomerId.Value.ToString(), MeterId = tranData.MeterName, SpaceId = tranData.SpaceNo, CityCurrentTime = data.PresentMeterTime.HasValue ? data.PresentMeterTime.Value : DateTime.Now };
            }
            catch (Exception ex)
            {
                //var appPath = System.Web.Hosting.HostingEnvironment.ApplicationPhysicalPath;
                //string errorPath = Path.Combine(appPath, "GetTransformData", "Timezones");
                //ExceptionLogging.SendErrorToText(ex, "GetCitationData", errorPath);
                //tranData.ReturnCode = ((int)ReturnCodeEnum.Error).ToString();

                tranData.ReturnCode = ((int)ReturnCodeEnum.Error).ToString();
            }
            #endregion
            #region "Parkmobile Service Call"
            /////////////////////////////////////////////////
            TransactionTransform tranDataOfServiceParkMobile = new TransactionTransform();
            try
            {
                //////////////////////////////////////////
                logData.Request.CustomerId = param.CustomerId.Value.ToString();
                logData.Request.SpaceId = param.parkingSpaceID;
                logData.Request.MeterId = param.MeterName;
                //////////////////////////////////////////////////
                DateTime? currentCustTime = customerCurrentTime.CurrentTime;
                PMCustomerElement customer = ParkmobileCustomerRetriever.GetCustomer(param.CustomerId.Value);
                ParkMobileGenericFactory factory = new ParkMobileGenericFactory(customer, param.PlateNumber);
                var parkMobileResLog = new VendorResponseEntity();
                logData.VendorData.Add(VendorNames.ParkMobile, parkMobileResLog);
                tranDataOfServiceParkMobile = factory.GetRefreshSpaceData(ParkingMobileDataType.Plate, ref parkMobileResLog);
                tranDataOfServiceParkMobile.EnforcementKey = EnforcementKey;

                ///////////////////////////////////////////////////
                logData.Response = new ResponseEntity() { CustomerId = param.CustomerId.Value.ToString(), MeterId = tranData.MeterName, SpaceId = tranData.SpaceNo, CityCurrentTime = currentCustTime.HasValue ? currentCustTime.Value : DateTime.Now };
            }
            catch (Exception ex)
            {
                logData.Response = new ResponseEntity() { CustomerId = param.CustomerId.Value.ToString(), MeterId = tranData.MeterName, SpaceId = tranData.SpaceNo, CityCurrentTime = customerCurrentTime.CurrentTime.HasValue ? customerCurrentTime.CurrentTime.Value : DateTime.Now };
                //throw ex;
                tranDataOfServiceParkMobile.ReturnCode = ((int)ReturnCodeEnum.Error).ToString();
            }
            finally
            {

            }
            #endregion
            #region "Cale Service Call"
            /////////////////////////////////////////////////
            TransactionTransform tranDataOfServiceCale = new TransactionTransform();
            try
            {
                //////////////////////////////////////////
                logData.Request.CustomerId = param.CustomerId.Value.ToString();
                logData.Request.SpaceId = param.parkingSpaceID;
                logData.Request.MeterId = param.MeterName;
                //////////////////////////////////////////////////
                DateTime? currentCustTime = customerCurrentTime.CurrentTime;
                Cale.CaleCustomerElement customer = Cale.CaleCustomerRetriever.GetCustomer(param.CustomerId.Value);
                Cale.CaleGenericFactory factory = new Cale.CaleGenericFactory(customer, param.PlateNumber, null);
                var caleResLog = new VendorResponseEntity();
                logData.VendorData.Add(VendorNames.Cale, caleResLog);
                if (customer.EnforcementType == EnumEnforcementType.PayByPlate.ToString())
                    tranDataOfServiceCale = factory.GetRefreshPlateDataPhiladelphia(ref caleResLog);

                tranDataOfServiceCale.EnforcementKey = EnforcementKey;

                ///////////////////////////////////////////////////
                logData.Response = new ResponseEntity() { CustomerId = param.CustomerId.Value.ToString(), MeterId = tranData.MeterName, SpaceId = tranData.SpaceNo, CityCurrentTime = currentCustTime.HasValue ? currentCustTime.Value : DateTime.Now };
            }
            catch (Exception ex)
            {
                logData.Response = new ResponseEntity() { CustomerId = param.CustomerId.Value.ToString(), MeterId = tranData.MeterName, SpaceId = tranData.SpaceNo, CityCurrentTime = customerCurrentTime.CurrentTime.HasValue ? customerCurrentTime.CurrentTime.Value : DateTime.Now };
                //throw ex;
                tranDataOfServiceCale.ReturnCode = ((int)ReturnCodeEnum.Error).ToString();
            }
            finally
            {

            }
            #endregion
            //////////////////  //Reconcile data

            List<TransactionTransform> trans = new List<TransactionTransform>();
            trans.Add(tranDataOfServiceParkMobile); // Parkmobile
            trans.Add(tranDataOfServiceCale); // Cale
            trans.Add(tranData); // Database

            var rightValue = trans.Where(t => t.ReturnCode == ((int)ReturnCodeEnum.Success).ToString()).OrderByDescending(t => t.MeterExpiredMinutes.ToNullableInt()).FirstOrDefault<TransactionTransform>();
            if (rightValue == default(TransactionTransform))
            {
                //Do nothing.just return database  object.
            }
            else
            {
                tranData = rightValue;
            }
            return tranData;
        }

        public static TransactionTransform GetPhiladelphiaTransformDataV3(string EnforcementKey, string filePath, RequestResponseEntity logData, CustomerTime customerCurrentTime)
        {
            TransactionTransform tranData = new TransactionTransform();
            tranData.ReturnCode = ((int)ReturnCodeEnum.InvalidInputParameters).ToString();
            TimeZoneInfo TZ = null;
            TZ = TimeZoneInfo.FindSystemTimeZoneById("Eastern Standard Time");

            KeyParameter param = PrepareKeyParameters.PreparePhiladelphia(EnforcementKey);
            if (param.ReturnCode == ((int)ReturnCodeEnum.InvalidInputParameters).ToString())
            {
                return tranData;
            }


            #region "DB Check"
            try
            {
                Transaction data = TransactionDataValidatioin.GetDataPhiladelphia(param.CustomerId.Value, param.MeterName, param.parkingSpaceID, param.PlateNumber, param.StateName, EnforcementKey, filePath, logData);


                if (data.TimeZoneId.HasValue)
                    TZ = Utility.GetTimeZoneInfo(data.TimeZoneId.Value, filePath, param.CustomerId.Value.ToString());

                //For South Miami : PayStationId. Others it is MeterID
                tranData.MeterName = (data.MeterName == null) ? string.Empty : data.MeterName;
                tranData.SpaceNo = data.ParkingSpaceID.ToString();

                tranData.MeterStreet = (data.MeterStreet == null) ? string.Empty : data.MeterStreet;

                DateTime _MeterRTC = data.PresentMeterTime.HasValue ? data.PresentMeterTime.Value : DateTime.Now;
                tranData.MeterRTC = String.Format("{0:yyyyMMdd_THHmmssZ}", Utility.findGMTTime(_MeterRTC, TZ));

                DateTime _SensorEventTime = data.SensorEventTime.HasValue ? data.SensorEventTime.Value : DateTime.Now;
                tranData.MeterLastUpdateTime = String.Format("{0:yyyyMMdd_THHmmssZ}", Utility.findGMTTime(_SensorEventTime, TZ));

                tranData.SensorEventTime = String.Format("{0:yyyyMMdd_THHmmssZ}", Utility.findGMTTime(_SensorEventTime, TZ));

                DateTime _endTime = data.ExpiryTime.HasValue ? data.ExpiryTime.Value : DateTime.Now;
                tranData.ExpiredTime = String.Format("{0:yyyyMMdd_THHmmssZ}", Utility.findGMTTime(_endTime, TZ));

                tranData.MeterExpiredMinutes = data.ExpiredMinutes.HasValue ? data.ExpiredMinutes.Value.ToString() : "0";
                tranData.METERLOC_BLOCKNUMBER = (data.Block == null) ? string.Empty : data.Block;
                tranData.METERLOC_STREETDIRECTION = (data.Direction == null) ? string.Empty : data.Direction;
                tranData.METERLOC_STREETNAME = (data.MeterStreet == null) ? string.Empty : data.MeterStreet;
                tranData.METERLOC_STREETTYPE = (data.StreetType == null) ? string.Empty : data.StreetType;
                tranData.METERLOC_CROSSSTREET1 = (data.CrossStreet1 == null) ? string.Empty : data.CrossStreet1;
                tranData.METERLOC_CROSSSTREET2 = (data.CrossStreet2 == null) ? string.Empty : data.CrossStreet2;
                tranData.METERLOC_ENFORCEMENTHOURSDESC = (data.EnfHourDesc == null) ? string.Empty : data.EnfHourDesc;
                tranData.LicensePlate = (data.PlateNumber == null) ? string.Empty : data.PlateNumber;
                tranData.State = (data.LPLocation == null) ? string.Empty : data.LPLocation;
                tranData.Make = (data.VMake == null) ? string.Empty : data.VMake;
                tranData.Model = (data.VModel == null) ? string.Empty : data.VModel;
                tranData.IsOccupied = Utility.getStatus((data.SpaceStatus == null) ? string.Empty : data.SpaceStatus).ToString();
                tranData.EnforcementKey = (data.EnforcementKey == null) ? string.Empty : data.EnforcementKey;
                tranData.MeterStatus = (data.MeterStatus == null) ? null : data.MeterStatus;

                tranData.METERLOC_METERTYPE = (data.METERLOC_METERTYPE == null) ? null : data.METERLOC_METERTYPE;
                tranData.ZoneName = (data.ZoneName == null) ? null : data.ZoneName;

                tranData.MeterID = data.PayStationID.HasValue ? data.PayStationID.Value.ToString() : "0";
                tranData.ReturnCode = data.ReturnCode;

                logData.Response = new ResponseEntity() { CustomerId = param.CustomerId.Value.ToString(), MeterId = tranData.MeterName, SpaceId = tranData.SpaceNo, CityCurrentTime = data.PresentMeterTime.HasValue ? data.PresentMeterTime.Value : DateTime.Now };
            }
            catch (Exception ex)
            {
                //var appPath = System.Web.Hosting.HostingEnvironment.ApplicationPhysicalPath;
                //string errorPath = Path.Combine(appPath, "GetTransformData", "Timezones");
                //ExceptionLogging.SendErrorToText(ex, "GetCitationData", errorPath);
                //tranData.ReturnCode = ((int)ReturnCodeEnum.Error).ToString();

                tranData.ReturnCode = ((int)ReturnCodeEnum.Error).ToString();
            }
            #endregion
            #region "Parkmobile Service Call"
            /////////////////////////////////////////////////
            TransactionTransform tranDataOfServiceParkMobile = new TransactionTransform();
            try
            {
                //////////////////////////////////////////
                logData.Request.CustomerId = param.CustomerId.Value.ToString();
                logData.Request.SpaceId = param.parkingSpaceID;
                logData.Request.MeterId = param.MeterName;
                //////////////////////////////////////////////////
                DateTime? currentCustTime = customerCurrentTime.CurrentTime;
                PMCustomerElement customer = ParkmobileCustomerRetriever.GetCustomer(param.CustomerId.Value);
                ParkMobileGenericFactory factory = new ParkMobileGenericFactory(customer, param.PlateNumber);
                var parkMobileResLog = new VendorResponseEntity();
                logData.VendorData.Add(VendorNames.ParkMobile, parkMobileResLog);
                tranDataOfServiceParkMobile = factory.GetRefreshSpaceData(ParkingMobileDataType.Plate, ref parkMobileResLog);
                tranDataOfServiceParkMobile.EnforcementKey = EnforcementKey;

                ///////////////////////////////////////////////////
                logData.Response = new ResponseEntity() { CustomerId = param.CustomerId.Value.ToString(), MeterId = tranData.MeterName, SpaceId = tranData.SpaceNo, CityCurrentTime = currentCustTime.HasValue ? currentCustTime.Value : DateTime.Now };
            }
            catch (Exception ex)
            {
                logData.Response = new ResponseEntity() { CustomerId = param.CustomerId.Value.ToString(), MeterId = tranData.MeterName, SpaceId = tranData.SpaceNo, CityCurrentTime = customerCurrentTime.CurrentTime.HasValue ? customerCurrentTime.CurrentTime.Value : DateTime.Now };
                //throw ex;
                tranDataOfServiceParkMobile.ReturnCode = ((int)ReturnCodeEnum.Error).ToString();
            }
            finally
            {

            }
            #endregion
            #region "Parkmobile Service Call2"
            /////////////////////////////////////////////////
            TransactionTransform tranDataOfServiceParkMobile2 = new TransactionTransform();
            try
            {
                //////////////////////////////////////////
                logData.Request.CustomerId = param.CustomerId.Value.ToString();
                logData.Request.SpaceId = param.parkingSpaceID;
                logData.Request.MeterId = param.MeterName;
                //////////////////////////////////////////////////
                DateTime? currentCustTime = customerCurrentTime.CurrentTime;
                PMCustomerElement customer = ParkmobileCustomerRetriever.GetCustomer(70562);
                ParkMobileGenericFactory factory = new ParkMobileGenericFactory(customer, param.PlateNumber);
                var parkMobileResLog2 = new VendorResponseEntity();
                logData.VendorData.Add(VendorNames.ParkMobile2, parkMobileResLog2);
                tranDataOfServiceParkMobile2 = factory.GetRefreshSpaceData(ParkingMobileDataType.Plate, ref parkMobileResLog2);
                tranDataOfServiceParkMobile2.EnforcementKey = EnforcementKey;

                ///////////////////////////////////////////////////
                logData.Response = new ResponseEntity() { CustomerId = param.CustomerId.Value.ToString(), MeterId = tranData.MeterName, SpaceId = tranData.SpaceNo, CityCurrentTime = currentCustTime.HasValue ? currentCustTime.Value : DateTime.Now };
            }
            catch (Exception ex)
            {
                logData.Response = new ResponseEntity() { CustomerId = param.CustomerId.Value.ToString(), MeterId = tranData.MeterName, SpaceId = tranData.SpaceNo, CityCurrentTime = customerCurrentTime.CurrentTime.HasValue ? customerCurrentTime.CurrentTime.Value : DateTime.Now };
                //throw ex;
                tranDataOfServiceParkMobile.ReturnCode = ((int)ReturnCodeEnum.Error).ToString();
            }
            finally
            {

            }
            #endregion
            #region "Cale Service Call"
            /////////////////////////////////////////////////
            TransactionTransform tranDataOfServiceCale = new TransactionTransform();
            try
            {
                //////////////////////////////////////////
                logData.Request.CustomerId = param.CustomerId.Value.ToString();
                logData.Request.SpaceId = param.parkingSpaceID;
                logData.Request.MeterId = param.MeterName;
                //////////////////////////////////////////////////
                DateTime? currentCustTime = customerCurrentTime.CurrentTime;
                Cale.CaleCustomerElement customer = Cale.CaleCustomerRetriever.GetCustomer(param.CustomerId.Value);
                Cale.CaleGenericFactory factory = new Cale.CaleGenericFactory(customer, param.PlateNumber, null);
                var caleResLog = new VendorResponseEntity();
                logData.VendorData.Add(VendorNames.Cale, caleResLog);
                if (customer.EnforcementType == EnumEnforcementType.PayByPlate.ToString())
                    tranDataOfServiceCale = factory.GetRefreshPlateDataPhiladelphia(ref caleResLog);

                tranDataOfServiceCale.EnforcementKey = EnforcementKey;

                ///////////////////////////////////////////////////
                logData.Response = new ResponseEntity() { CustomerId = param.CustomerId.Value.ToString(), MeterId = tranData.MeterName, SpaceId = tranData.SpaceNo, CityCurrentTime = currentCustTime.HasValue ? currentCustTime.Value : DateTime.Now };
            }
            catch (Exception ex)
            {
                logData.Response = new ResponseEntity() { CustomerId = param.CustomerId.Value.ToString(), MeterId = tranData.MeterName, SpaceId = tranData.SpaceNo, CityCurrentTime = customerCurrentTime.CurrentTime.HasValue ? customerCurrentTime.CurrentTime.Value : DateTime.Now };
                //throw ex;
                tranDataOfServiceCale.ReturnCode = ((int)ReturnCodeEnum.Error).ToString();
            }
            finally
            {

            }
            #endregion
            //////////////////  //Reconcile data

            List<TransactionTransform> trans = new List<TransactionTransform>();
            trans.Add(tranDataOfServiceParkMobile); // Parkmobile
            trans.Add(tranDataOfServiceParkMobile2); // Parkmobile2
            trans.Add(tranDataOfServiceCale); // Cale
            trans.Add(tranData); // Database

            var rightValue = trans.Where(t => t.ReturnCode == ((int)ReturnCodeEnum.Success).ToString()).OrderByDescending(t => t.MeterExpiredMinutes.ToNullableInt()).FirstOrDefault<TransactionTransform>();
            if (rightValue == default(TransactionTransform))
            {
                //Do nothing.just return database  object.
            }
            else
            {
                tranData = rightValue;
            }
            return tranData;
        }

        public static TransactionTransform GetPhiladelphiaTransformDataV4(string EnforcementKey, string filePath, RequestResponseEntity logData, CustomerTime customerCurrentTime)
        {
            TransactionTransform tranData = new TransactionTransform();
            tranData.ReturnCode = ((int)ReturnCodeEnum.InvalidInputParameters).ToString();
            TimeZoneInfo TZ = null;
            TZ = TimeZoneInfo.FindSystemTimeZoneById("Eastern Standard Time");

            KeyParameter param = PrepareKeyParameters.PreparePhiladelphia(EnforcementKey);
            if (param.ReturnCode == ((int)ReturnCodeEnum.InvalidInputParameters).ToString())
            {
                return tranData;
            }


            #region "DB Check"
            try
            {
                //ZoneId is ZoneName i.e. Article Name 
                Transaction data = TransactionDataValidatioin.GetDataPhiladelphiaZone(param.CustomerId.Value, param.MeterName, param.parkingSpaceID, param.PlateNumber, param.StateName, param.ZoneID, EnforcementKey, filePath, logData);


                if (data.TimeZoneId.HasValue)
                    TZ = Utility.GetTimeZoneInfo(data.TimeZoneId.Value, filePath, param.CustomerId.Value.ToString());

                //For South Miami : PayStationId. Others it is MeterID
                tranData.MeterName = (data.MeterName == null) ? string.Empty : data.MeterName;
                tranData.SpaceNo = data.ParkingSpaceID.ToString();

                tranData.MeterStreet = (data.MeterStreet == null) ? string.Empty : data.MeterStreet;

                DateTime _MeterRTC = data.PresentMeterTime.HasValue ? data.PresentMeterTime.Value : DateTime.Now;
                tranData.MeterRTC = String.Format("{0:yyyyMMdd_THHmmssZ}", Utility.findGMTTime(_MeterRTC, TZ));

                DateTime _SensorEventTime = data.SensorEventTime.HasValue ? data.SensorEventTime.Value : DateTime.Now;
                tranData.MeterLastUpdateTime = String.Format("{0:yyyyMMdd_THHmmssZ}", Utility.findGMTTime(_SensorEventTime, TZ));

                tranData.SensorEventTime = String.Format("{0:yyyyMMdd_THHmmssZ}", Utility.findGMTTime(_SensorEventTime, TZ));

                DateTime _endTime = data.ExpiryTime.HasValue ? data.ExpiryTime.Value : DateTime.Now;
                tranData.ExpiredTime = String.Format("{0:yyyyMMdd_THHmmssZ}", Utility.findGMTTime(_endTime, TZ));

                tranData.MeterExpiredMinutes = data.ExpiredMinutes.HasValue ? data.ExpiredMinutes.Value.ToString() : "0";
                tranData.METERLOC_BLOCKNUMBER = (data.Block == null) ? string.Empty : data.Block;
                tranData.METERLOC_STREETDIRECTION = (data.Direction == null) ? string.Empty : data.Direction;
                tranData.METERLOC_STREETNAME = (data.MeterStreet == null) ? string.Empty : data.MeterStreet;
                tranData.METERLOC_STREETTYPE = (data.StreetType == null) ? string.Empty : data.StreetType;
                tranData.METERLOC_CROSSSTREET1 = (data.CrossStreet1 == null) ? string.Empty : data.CrossStreet1;
                tranData.METERLOC_CROSSSTREET2 = (data.CrossStreet2 == null) ? string.Empty : data.CrossStreet2;
                tranData.METERLOC_ENFORCEMENTHOURSDESC = (data.EnfHourDesc == null) ? string.Empty : data.EnfHourDesc;
                tranData.LicensePlate = (data.PlateNumber == null) ? string.Empty : data.PlateNumber;
                tranData.State = (data.LPLocation == null) ? string.Empty : data.LPLocation;
                tranData.Make = (data.VMake == null) ? string.Empty : data.VMake;
                tranData.Model = (data.VModel == null) ? string.Empty : data.VModel;
                tranData.IsOccupied = Utility.getStatus((data.SpaceStatus == null) ? string.Empty : data.SpaceStatus).ToString();
                tranData.EnforcementKey = (data.EnforcementKey == null) ? string.Empty : data.EnforcementKey;
                tranData.MeterStatus = (data.MeterStatus == null) ? null : data.MeterStatus;

                tranData.METERLOC_METERTYPE = (data.METERLOC_METERTYPE == null) ? null : data.METERLOC_METERTYPE;
                tranData.ZoneName = (data.ZoneName == null) ? null : data.ZoneName;

                tranData.MeterID = data.PayStationID.HasValue ? data.PayStationID.Value.ToString() : "0";
                tranData.ReturnCode = data.ReturnCode;

                logData.Response = new ResponseEntity() { CustomerId = param.CustomerId.Value.ToString(), MeterId = tranData.MeterName, SpaceId = tranData.SpaceNo, CityCurrentTime = data.PresentMeterTime.HasValue ? data.PresentMeterTime.Value : DateTime.Now };
            }
            catch (Exception ex)
            {
                //var appPath = System.Web.Hosting.HostingEnvironment.ApplicationPhysicalPath;
                //string errorPath = Path.Combine(appPath, "GetTransformData", "Timezones");
                //ExceptionLogging.SendErrorToText(ex, "GetCitationData", errorPath);
                //tranData.ReturnCode = ((int)ReturnCodeEnum.Error).ToString();

                tranData.ReturnCode = ((int)ReturnCodeEnum.Error).ToString();
            }
            #endregion
            #region "Parkmobile Service Call"
            /////////////////////////////////////////////////
            TransactionTransform tranDataOfServiceParkMobile = new TransactionTransform();
            try
            {
                //////////////////////////////////////////
                logData.Request.CustomerId = param.CustomerId.Value.ToString();
                logData.Request.SpaceId = param.parkingSpaceID;
                logData.Request.MeterId = param.MeterName;
                //////////////////////////////////////////////////
                DateTime? currentCustTime = customerCurrentTime.CurrentTime;
                PMCustomerElement customer = ParkmobileCustomerRetriever.GetCustomer(param.CustomerId.Value);
                ParkMobileGenericFactory factory = new ParkMobileGenericFactory(customer, param.PlateNumber);
                var parkMobileResLog = new VendorResponseEntity();
                logData.VendorData.Add(VendorNames.ParkMobile, parkMobileResLog);
                tranDataOfServiceParkMobile = factory.GetRefreshSpaceData(ParkingMobileDataType.Plate, ref parkMobileResLog);
                tranDataOfServiceParkMobile.EnforcementKey = EnforcementKey;



                ///////////////////////////////////////////////////
                logData.Response = new ResponseEntity() { CustomerId = param.CustomerId.Value.ToString(), MeterId = tranData.MeterName, SpaceId = tranData.SpaceNo, CityCurrentTime = currentCustTime.HasValue ? currentCustTime.Value : DateTime.Now };
            }
            catch (Exception ex)
            {
                logData.Response = new ResponseEntity() { CustomerId = param.CustomerId.Value.ToString(), MeterId = tranData.MeterName, SpaceId = tranData.SpaceNo, CityCurrentTime = customerCurrentTime.CurrentTime.HasValue ? customerCurrentTime.CurrentTime.Value : DateTime.Now };
                //throw ex;
                tranDataOfServiceParkMobile.ReturnCode = ((int)ReturnCodeEnum.Error).ToString();
            }
            finally
            {

            }
            #endregion
            #region "Parkmobile Service Call2"
            /////////////////////////////////////////////////
            TransactionTransform tranDataOfServiceParkMobile2 = new TransactionTransform();
            try
            {
                //////////////////////////////////////////
                logData.Request.CustomerId = param.CustomerId.Value.ToString();
                logData.Request.SpaceId = param.parkingSpaceID;
                logData.Request.MeterId = param.MeterName;
                //////////////////////////////////////////////////
                DateTime? currentCustTime = customerCurrentTime.CurrentTime;
                PMCustomerElement customer = ParkmobileCustomerRetriever.GetCustomer(70562);
                ParkMobileGenericFactory factory = new ParkMobileGenericFactory(customer, param.PlateNumber);
                var parkMobileResLog2 = new VendorResponseEntity();
                logData.VendorData.Add(VendorNames.ParkMobile2, parkMobileResLog2);
                tranDataOfServiceParkMobile2 = factory.GetRefreshSpaceData(ParkingMobileDataType.Plate, ref parkMobileResLog2);
                tranDataOfServiceParkMobile2.EnforcementKey = EnforcementKey;

                ///////////////////////////////////////////////////
                logData.Response = new ResponseEntity() { CustomerId = param.CustomerId.Value.ToString(), MeterId = tranData.MeterName, SpaceId = tranData.SpaceNo, CityCurrentTime = currentCustTime.HasValue ? currentCustTime.Value : DateTime.Now };
            }
            catch (Exception ex)
            {
                logData.Response = new ResponseEntity() { CustomerId = param.CustomerId.Value.ToString(), MeterId = tranData.MeterName, SpaceId = tranData.SpaceNo, CityCurrentTime = customerCurrentTime.CurrentTime.HasValue ? customerCurrentTime.CurrentTime.Value : DateTime.Now };
                //throw ex;
                tranDataOfServiceParkMobile.ReturnCode = ((int)ReturnCodeEnum.Error).ToString();
            }
            finally
            {

            }
            #endregion
            #region "Cale Service Call"
            /////////////////////////////////////////////////
            TransactionTransform tranDataOfServiceCale = new TransactionTransform();
            try
            {
                //////////////////////////////////////////
                logData.Request.CustomerId = param.CustomerId.Value.ToString();
                logData.Request.SpaceId = param.parkingSpaceID;
                logData.Request.MeterId = param.MeterName;
                //////////////////////////////////////////////////
                DateTime? currentCustTime = customerCurrentTime.CurrentTime;
                Cale.CaleCustomerElement customer = Cale.CaleCustomerRetriever.GetCustomer(param.CustomerId.Value);
                Cale.CaleGenericFactory factory = new Cale.CaleGenericFactory(customer, param.PlateNumber, null);
                var caleResLog = new VendorResponseEntity();
                logData.VendorData.Add(VendorNames.Cale, caleResLog);
                if (customer.EnforcementType == EnumEnforcementType.PayByPlate.ToString())
                    tranDataOfServiceCale = factory.GetRefreshPlateDataPhiladelphia(ref caleResLog);

                tranDataOfServiceCale.EnforcementKey = EnforcementKey;

                ///////////////////////////////////////////////////
                logData.Response = new ResponseEntity() { CustomerId = param.CustomerId.Value.ToString(), MeterId = tranData.MeterName, SpaceId = tranData.SpaceNo, CityCurrentTime = currentCustTime.HasValue ? currentCustTime.Value : DateTime.Now };
            }
            catch (Exception ex)
            {
                logData.Response = new ResponseEntity() { CustomerId = param.CustomerId.Value.ToString(), MeterId = tranData.MeterName, SpaceId = tranData.SpaceNo, CityCurrentTime = customerCurrentTime.CurrentTime.HasValue ? customerCurrentTime.CurrentTime.Value : DateTime.Now };
                //throw ex;
                tranDataOfServiceCale.ReturnCode = ((int)ReturnCodeEnum.Error).ToString();
            }
            finally
            {

            }
            #endregion
            //////////////////  //Reconcile data

            List<TransactionTransform> trans = new List<TransactionTransform>();
            trans.Add(tranDataOfServiceParkMobile); // Parkmobile
            trans.Add(tranDataOfServiceParkMobile2); // Parkmobile2
            trans.Add(tranDataOfServiceCale); // Cale
            trans.Add(tranData); // Database

            var rightValue = trans.Where(t => t.ReturnCode == ((int)ReturnCodeEnum.Success).ToString()).OrderByDescending(t => t.MeterExpiredMinutes.ToNullableInt()).FirstOrDefault<TransactionTransform>();
            if (rightValue == default(TransactionTransform))
            {
                //Do nothing.just return database  object.
            }
            else
            {
                tranData = rightValue;
            }
            return tranData;
        }

        public static TransactionTransform GetPhiladelphiaTransformDataV5(string EnforcementKey, string filePath, RequestResponseEntity logData, CustomerTime customerCurrentTime)
        {
            TransactionTransform tranData = new TransactionTransform();
            tranData.ReturnCode = ((int)ReturnCodeEnum.InvalidInputParameters).ToString();
            TimeZoneInfo TZ = null;
            TZ = TimeZoneInfo.FindSystemTimeZoneById("Eastern Standard Time");

            KeyParameter param = PrepareKeyParameters.PreparePhiladelphia2(EnforcementKey);
            if (param.ReturnCode == ((int)ReturnCodeEnum.InvalidInputParameters).ToString())
            {
                return tranData;
            }

            #region "BySpace"
            if (param.EnforecementMode.ToLower() == "paybyspace".ToLower())
            {
                tranData = GetPhiladelphiaTransformDataBySpace(EnforcementKey, filePath, logData, customerCurrentTime, param);
                //tranData = new TransactionTransform()
                //{
                //    MeterExpiredMinutes = "-1",
                //    EnforcementKey = EnforcementKey,
                //    SpaceNo = param.parkingSpaceID,
                //    ZoneName = param.ZoneID,
                //    MeterID = param.MeterName,
                //    ReturnCode = ((int)ReturnCodeEnum.Success).ToString()
                //};
                //logData.Request.CustomerId = param.CustomerId.Value.ToString();
                //logData.Response = new ResponseEntity() { CustomerId = param.CustomerId.ToString(), MeterId = param.MeterName, SpaceId = param.parkingSpaceID, CityCurrentTime = customerCurrentTime.CurrentTime.Value };
                return tranData;
            }


            #endregion
            #region "DB Check"
            try
            {
                //ZoneId is ZoneName i.e. Article Name 
                Transaction data = TransactionDataValidatioin.GetDataPhiladelphiaZone(param.CustomerId.Value, param.MeterName, param.parkingSpaceID, param.PlateNumber, param.StateName, param.ZoneID, EnforcementKey, filePath, logData);


                if (data.TimeZoneId.HasValue)
                    TZ = Utility.GetTimeZoneInfo(data.TimeZoneId.Value, filePath, param.CustomerId.Value.ToString());

                //For South Miami : PayStationId. Others it is MeterID
                tranData.MeterName = (data.MeterName == null) ? string.Empty : data.MeterName;
                tranData.SpaceNo = data.ParkingSpaceID.ToString();

                tranData.MeterStreet = (data.MeterStreet == null) ? string.Empty : data.MeterStreet;

                DateTime _MeterRTC = data.PresentMeterTime.HasValue ? data.PresentMeterTime.Value : DateTime.Now;
                tranData.MeterRTC = String.Format("{0:yyyyMMdd_THHmmssZ}", Utility.findGMTTime(_MeterRTC, TZ));

                DateTime _SensorEventTime = data.SensorEventTime.HasValue ? data.SensorEventTime.Value : DateTime.Now;
                tranData.MeterLastUpdateTime = String.Format("{0:yyyyMMdd_THHmmssZ}", Utility.findGMTTime(_SensorEventTime, TZ));

                tranData.SensorEventTime = String.Format("{0:yyyyMMdd_THHmmssZ}", Utility.findGMTTime(_SensorEventTime, TZ));

                DateTime _endTime = data.ExpiryTime.HasValue ? data.ExpiryTime.Value : DateTime.Now;
                tranData.ExpiredTime = String.Format("{0:yyyyMMdd_THHmmssZ}", Utility.findGMTTime(_endTime, TZ));

                tranData.MeterExpiredMinutes = data.ExpiredMinutes.HasValue ? data.ExpiredMinutes.Value.ToString() : "0";
                tranData.METERLOC_BLOCKNUMBER = (data.Block == null) ? string.Empty : data.Block;
                tranData.METERLOC_STREETDIRECTION = (data.Direction == null) ? string.Empty : data.Direction;
                tranData.METERLOC_STREETNAME = (data.MeterStreet == null) ? string.Empty : data.MeterStreet;
                tranData.METERLOC_STREETTYPE = (data.StreetType == null) ? string.Empty : data.StreetType;
                tranData.METERLOC_CROSSSTREET1 = (data.CrossStreet1 == null) ? string.Empty : data.CrossStreet1;
                tranData.METERLOC_CROSSSTREET2 = (data.CrossStreet2 == null) ? string.Empty : data.CrossStreet2;
                tranData.METERLOC_ENFORCEMENTHOURSDESC = (data.EnfHourDesc == null) ? string.Empty : data.EnfHourDesc;
                tranData.LicensePlate = (data.PlateNumber == null) ? string.Empty : data.PlateNumber;
                tranData.State = (data.LPLocation == null) ? string.Empty : data.LPLocation;
                tranData.Make = (data.VMake == null) ? string.Empty : data.VMake;
                tranData.Model = (data.VModel == null) ? string.Empty : data.VModel;
                tranData.IsOccupied = Utility.getStatus((data.SpaceStatus == null) ? string.Empty : data.SpaceStatus).ToString();
                tranData.EnforcementKey = (data.EnforcementKey == null) ? string.Empty : data.EnforcementKey;
                tranData.MeterStatus = (data.MeterStatus == null) ? null : data.MeterStatus;

                tranData.METERLOC_METERTYPE = (data.METERLOC_METERTYPE == null) ? null : data.METERLOC_METERTYPE;
                tranData.ZoneName = (data.ZoneName == null) ? null : data.ZoneName;

                tranData.MeterID = data.PayStationID.HasValue ? data.PayStationID.Value.ToString() : "0";
                tranData.ReturnCode = data.ReturnCode;

                logData.Response = new ResponseEntity() { CustomerId = param.CustomerId.Value.ToString(), MeterId = tranData.MeterName, SpaceId = tranData.SpaceNo, CityCurrentTime = data.PresentMeterTime.HasValue ? data.PresentMeterTime.Value : DateTime.Now };
            }
            catch (Exception ex)
            {
                //var appPath = System.Web.Hosting.HostingEnvironment.ApplicationPhysicalPath;
                //string errorPath = Path.Combine(appPath, "GetTransformData", "Timezones");
                //ExceptionLogging.SendErrorToText(ex, "GetCitationData", errorPath);
                //tranData.ReturnCode = ((int)ReturnCodeEnum.Error).ToString();

                tranData.ReturnCode = ((int)ReturnCodeEnum.Error).ToString();
            }
            #endregion
            #region "Parkmobile Service Call"
            /////////////////////////////////////////////////
            TransactionTransform tranDataOfServiceParkMobile = new TransactionTransform();
            try
            {
                //////////////////////////////////////////
                logData.Request.CustomerId = param.CustomerId.Value.ToString();
                logData.Request.SpaceId = param.parkingSpaceID;
                logData.Request.MeterId = param.MeterName;
                //////////////////////////////////////////////////
                DateTime? currentCustTime = customerCurrentTime.CurrentTime;
                PMCustomerElement customer = ParkmobileCustomerRetriever.GetCustomer(param.CustomerId.Value);
                ParkMobileGenericFactory factory = new ParkMobileGenericFactory(customer, param.PlateNumber);
                var parkMobileResLog = new VendorResponseEntity();
                logData.VendorData.Add(VendorNames.ParkMobile, parkMobileResLog);
                if (!string.IsNullOrEmpty(param.ZoneID))
                    tranDataOfServiceParkMobile = factory.GetRefreshSpaceZoneData(ParkingMobileDataType.Plate, param.ZoneID, ref parkMobileResLog);
                else
                    tranDataOfServiceParkMobile = factory.GetRefreshSpaceData(ParkingMobileDataType.Plate, ref parkMobileResLog);
                tranDataOfServiceParkMobile.EnforcementKey = EnforcementKey;

                ///////////////////////////////////////////////////
                logData.Response = new ResponseEntity() { CustomerId = param.CustomerId.Value.ToString(), MeterId = tranData.MeterName, SpaceId = tranData.SpaceNo, CityCurrentTime = currentCustTime.HasValue ? currentCustTime.Value : DateTime.Now };
            }
            catch (Exception ex)
            {
                logData.Response = new ResponseEntity() { CustomerId = param.CustomerId.Value.ToString(), MeterId = tranData.MeterName, SpaceId = tranData.SpaceNo, CityCurrentTime = customerCurrentTime.CurrentTime.HasValue ? customerCurrentTime.CurrentTime.Value : DateTime.Now };
                //throw ex;
                tranDataOfServiceParkMobile.ReturnCode = ((int)ReturnCodeEnum.Error).ToString();
            }
            finally
            {

            }
            #endregion
            #region "Parkmobile Service Call2"
            /////////////////////////////////////////////////
            TransactionTransform tranDataOfServiceParkMobile2 = new TransactionTransform();
            try
            {
                //////////////////////////////////////////
                logData.Request.CustomerId = param.CustomerId.Value.ToString();
                logData.Request.SpaceId = param.parkingSpaceID;
                logData.Request.MeterId = param.MeterName;
                //////////////////////////////////////////////////
                DateTime? currentCustTime = customerCurrentTime.CurrentTime;
                PMCustomerElement customer = ParkmobileCustomerRetriever.GetCustomer(70562);
                ParkMobileGenericFactory factory = new ParkMobileGenericFactory(customer, param.PlateNumber);
                var parkMobileResLog2 = new VendorResponseEntity();
                logData.VendorData.Add(VendorNames.ParkMobile2, parkMobileResLog2);
                if (!string.IsNullOrEmpty(param.ZoneID))
                    tranDataOfServiceParkMobile2 = factory.GetRefreshSpaceZoneData(ParkingMobileDataType.Plate, param.ZoneID, ref parkMobileResLog2);
                else
                    tranDataOfServiceParkMobile2 = factory.GetRefreshSpaceData(ParkingMobileDataType.Plate, ref parkMobileResLog2);
                tranDataOfServiceParkMobile2.EnforcementKey = EnforcementKey;

                ///////////////////////////////////////////////////
                logData.Response = new ResponseEntity() { CustomerId = param.CustomerId.Value.ToString(), MeterId = tranData.MeterName, SpaceId = tranData.SpaceNo, CityCurrentTime = currentCustTime.HasValue ? currentCustTime.Value : DateTime.Now };
            }
            catch (Exception ex)
            {
                logData.Response = new ResponseEntity() { CustomerId = param.CustomerId.Value.ToString(), MeterId = tranData.MeterName, SpaceId = tranData.SpaceNo, CityCurrentTime = customerCurrentTime.CurrentTime.HasValue ? customerCurrentTime.CurrentTime.Value : DateTime.Now };
                //throw ex;
                tranDataOfServiceParkMobile.ReturnCode = ((int)ReturnCodeEnum.Error).ToString();
            }
            finally
            {

            }
            #endregion
            #region "Cale Service Call"
            /////////////////////////////////////////////////
            TransactionTransform tranDataOfServiceCale = new TransactionTransform();
            try
            {

                //////////////////////////////////////////
                logData.Request.CustomerId = param.CustomerId.Value.ToString();
                logData.Request.SpaceId = param.parkingSpaceID;
                logData.Request.MeterId = param.MeterName;
                //////////////////////////////////////////////////
                DateTime? currentCustTime = customerCurrentTime.CurrentTime;
                Cale.CaleCustomerElement customer = Cale.CaleCustomerRetriever.GetCustomer(param.CustomerId.Value);
                Cale.CaleGenericFactory factory = new Cale.CaleGenericFactory(customer, param.PlateNumber, null);
                var caleResLog = new VendorResponseEntity();
                logData.VendorData.Add(VendorNames.Cale, caleResLog);
                if (customer.EnforcementType == EnumEnforcementType.PayByPlate.ToString())
                    if (string.IsNullOrEmpty(param.ZoneID))
                        tranDataOfServiceCale = factory.GetRefreshPlateDataPhiladelphia(ref caleResLog);
                    else
                        tranDataOfServiceCale = factory.GetRefreshPlateDataPhiladelphia(ref caleResLog, param.ZoneID);

                tranDataOfServiceCale.EnforcementKey = EnforcementKey;

                ///////////////////////////////////////////////////
                logData.Response = new ResponseEntity() { CustomerId = param.CustomerId.Value.ToString(), MeterId = tranData.MeterName, SpaceId = tranData.SpaceNo, CityCurrentTime = currentCustTime.HasValue ? currentCustTime.Value : DateTime.Now };
            }
            catch (Exception ex)
            {
                logData.Response = new ResponseEntity() { CustomerId = param.CustomerId.Value.ToString(), MeterId = tranData.MeterName, SpaceId = tranData.SpaceNo, CityCurrentTime = customerCurrentTime.CurrentTime.HasValue ? customerCurrentTime.CurrentTime.Value : DateTime.Now };
                //throw ex;
                tranDataOfServiceCale.ReturnCode = ((int)ReturnCodeEnum.Error).ToString();
            }
            finally
            {

            }
            #endregion
            //////////////////  //Reconcile data

            List<TransactionTransform> trans = new List<TransactionTransform>();
            trans.Add(tranDataOfServiceParkMobile); // Parkmobile
            trans.Add(tranDataOfServiceParkMobile2); // Parkmobile2
            trans.Add(tranDataOfServiceCale); // Cale  //revert after testing
            trans.Add(tranData); // Database          //revert after testing

            var rightValue = trans.Where(t => t.ReturnCode == ((int)ReturnCodeEnum.Success).ToString()).OrderByDescending(t => t.MeterExpiredMinutes.ToNullableInt()).FirstOrDefault<TransactionTransform>();
            if (rightValue == default(TransactionTransform))
            {
                //Do nothing.just return database  object.
            }
            else
            {
                tranData = rightValue;
            }
            return tranData;
        }


        public static TransactionTransform GetPhiladelphiaTransformDataV5TestForSchema5(string EnforcementKey, string filePath, RequestResponseEntity logData, CustomerTime customerCurrentTime)
        {
            TransactionTransform tranData = new TransactionTransform();
            tranData.ReturnCode = ((int)ReturnCodeEnum.InvalidInputParameters).ToString();
            TimeZoneInfo TZ = null;
            TZ = TimeZoneInfo.FindSystemTimeZoneById("Eastern Standard Time");

            KeyParameter param = PrepareKeyParameters.PreparePhiladelphia2(EnforcementKey);
            if (param.ReturnCode == ((int)ReturnCodeEnum.InvalidInputParameters).ToString())
            {
                return tranData;
            }

            #region "BySpace"
            if (param.EnforecementMode.ToLower() == "paybyspace".ToLower())
            {
                tranData = GetPhiladelphiaTransformDataBySpace(EnforcementKey, filePath, logData, customerCurrentTime, param);
                //tranData = new TransactionTransform()
                //{
                //    MeterExpiredMinutes = "-1",
                //    EnforcementKey = EnforcementKey,
                //    SpaceNo = param.parkingSpaceID,
                //    ZoneName = param.ZoneID,
                //    MeterID = param.MeterName,
                //    ReturnCode = ((int)ReturnCodeEnum.Success).ToString()
                //};
                //logData.Request.CustomerId = param.CustomerId.Value.ToString();
                //logData.Response = new ResponseEntity() { CustomerId = param.CustomerId.ToString(), MeterId = param.MeterName, SpaceId = param.parkingSpaceID, CityCurrentTime = customerCurrentTime.CurrentTime.Value };
                return tranData;
            }


            #endregion
            #region "DB Check"
            try
            {
                //ZoneId is ZoneName i.e. Article Name 
                Transaction data = TransactionDataValidatioin.GetDataPhiladelphiaZone(param.CustomerId.Value, param.MeterName, param.parkingSpaceID, param.PlateNumber, param.StateName, param.ZoneID, EnforcementKey, filePath, logData);


                if (data.TimeZoneId.HasValue)
                    TZ = Utility.GetTimeZoneInfo(data.TimeZoneId.Value, filePath, param.CustomerId.Value.ToString());

                //For South Miami : PayStationId. Others it is MeterID
                tranData.MeterName = (data.MeterName == null) ? string.Empty : data.MeterName;
                tranData.SpaceNo = data.ParkingSpaceID.ToString();

                tranData.MeterStreet = (data.MeterStreet == null) ? string.Empty : data.MeterStreet;

                DateTime _MeterRTC = data.PresentMeterTime.HasValue ? data.PresentMeterTime.Value : DateTime.Now;
                tranData.MeterRTC = String.Format("{0:yyyyMMdd_THHmmssZ}", Utility.findGMTTime(_MeterRTC, TZ));

                DateTime _SensorEventTime = data.SensorEventTime.HasValue ? data.SensorEventTime.Value : DateTime.Now;
                tranData.MeterLastUpdateTime = String.Format("{0:yyyyMMdd_THHmmssZ}", Utility.findGMTTime(_SensorEventTime, TZ));

                tranData.SensorEventTime = String.Format("{0:yyyyMMdd_THHmmssZ}", Utility.findGMTTime(_SensorEventTime, TZ));

                DateTime _endTime = data.ExpiryTime.HasValue ? data.ExpiryTime.Value : DateTime.Now;
                tranData.ExpiredTime = String.Format("{0:yyyyMMdd_THHmmssZ}", Utility.findGMTTime(_endTime, TZ));

                tranData.MeterExpiredMinutes = data.ExpiredMinutes.HasValue ? data.ExpiredMinutes.Value.ToString() : "0";
                tranData.METERLOC_BLOCKNUMBER = (data.Block == null) ? string.Empty : data.Block;
                tranData.METERLOC_STREETDIRECTION = (data.Direction == null) ? string.Empty : data.Direction;
                tranData.METERLOC_STREETNAME = (data.MeterStreet == null) ? string.Empty : data.MeterStreet;
                tranData.METERLOC_STREETTYPE = (data.StreetType == null) ? string.Empty : data.StreetType;
                tranData.METERLOC_CROSSSTREET1 = (data.CrossStreet1 == null) ? string.Empty : data.CrossStreet1;
                tranData.METERLOC_CROSSSTREET2 = (data.CrossStreet2 == null) ? string.Empty : data.CrossStreet2;
                tranData.METERLOC_ENFORCEMENTHOURSDESC = (data.EnfHourDesc == null) ? string.Empty : data.EnfHourDesc;
                tranData.LicensePlate = (data.PlateNumber == null) ? string.Empty : data.PlateNumber;
                tranData.State = (data.LPLocation == null) ? string.Empty : data.LPLocation;
                tranData.Make = (data.VMake == null) ? string.Empty : data.VMake;
                tranData.Model = (data.VModel == null) ? string.Empty : data.VModel;
                tranData.IsOccupied = Utility.getStatus((data.SpaceStatus == null) ? string.Empty : data.SpaceStatus).ToString();
                tranData.EnforcementKey = (data.EnforcementKey == null) ? string.Empty : data.EnforcementKey;
                tranData.MeterStatus = (data.MeterStatus == null) ? null : data.MeterStatus;

                tranData.METERLOC_METERTYPE = (data.METERLOC_METERTYPE == null) ? null : data.METERLOC_METERTYPE;
                tranData.ZoneName = (data.ZoneName == null) ? null : data.ZoneName;

                tranData.MeterID = data.PayStationID.HasValue ? data.PayStationID.Value.ToString() : "0";
                tranData.ReturnCode = data.ReturnCode;

                logData.Response = new ResponseEntity() { CustomerId = param.CustomerId.Value.ToString(), MeterId = tranData.MeterName, SpaceId = tranData.SpaceNo, CityCurrentTime = data.PresentMeterTime.HasValue ? data.PresentMeterTime.Value : DateTime.Now };
            }
            catch (Exception ex)
            {
                //var appPath = System.Web.Hosting.HostingEnvironment.ApplicationPhysicalPath;
                //string errorPath = Path.Combine(appPath, "GetTransformData", "Timezones");
                //ExceptionLogging.SendErrorToText(ex, "GetCitationData", errorPath);
                //tranData.ReturnCode = ((int)ReturnCodeEnum.Error).ToString();

                tranData.ReturnCode = ((int)ReturnCodeEnum.Error).ToString();
            }
            #endregion
            #region "Parkmobile Service Call"
            /////////////////////////////////////////////////
            TransactionTransform tranDataOfServiceParkMobile = new TransactionTransform();
            try
            {
                //////////////////////////////////////////
                logData.Request.CustomerId = param.CustomerId.Value.ToString();
                logData.Request.SpaceId = param.parkingSpaceID;
                logData.Request.MeterId = param.MeterName;
                //////////////////////////////////////////////////
                DateTime? currentCustTime = customerCurrentTime.CurrentTime;
                PMCustomerElement customer = ParkmobileCustomerRetriever.GetCustomer(param.CustomerId.Value);
                ParkMobileGenericFactory factory = new ParkMobileGenericFactory(customer, param.PlateNumber);
                var parkMobileResLog = new VendorResponseEntity();
                logData.VendorData.Add(VendorNames.ParkMobile, parkMobileResLog);
                if (!string.IsNullOrEmpty(param.ZoneID))
                    tranDataOfServiceParkMobile = factory.GetRefreshSpaceZoneData(ParkingMobileDataType.Plate, param.ZoneID, ref parkMobileResLog);
                else
                    tranDataOfServiceParkMobile = factory.GetRefreshSpaceData(ParkingMobileDataType.Plate, ref parkMobileResLog);
                tranDataOfServiceParkMobile.EnforcementKey = EnforcementKey;

                ///////////////////////////////////////////////////
                logData.Response = new ResponseEntity() { CustomerId = param.CustomerId.Value.ToString(), MeterId = tranData.MeterName, SpaceId = tranData.SpaceNo, CityCurrentTime = currentCustTime.HasValue ? currentCustTime.Value : DateTime.Now };
            }
            catch (Exception ex)
            {
                logData.Response = new ResponseEntity() { CustomerId = param.CustomerId.Value.ToString(), MeterId = tranData.MeterName, SpaceId = tranData.SpaceNo, CityCurrentTime = customerCurrentTime.CurrentTime.HasValue ? customerCurrentTime.CurrentTime.Value : DateTime.Now };
                //throw ex;
                tranDataOfServiceParkMobile.ReturnCode = ((int)ReturnCodeEnum.Error).ToString();
            }
            finally
            {

            }
            #endregion
            #region "Parkmobile Service Call2"
            /////////////////////////////////////////////////
            TransactionTransform tranDataOfServiceParkMobile2 = new TransactionTransform();
            try
            {
                //////////////////////////////////////////
                logData.Request.CustomerId = param.CustomerId.Value.ToString();
                logData.Request.SpaceId = param.parkingSpaceID;
                logData.Request.MeterId = param.MeterName;
                //////////////////////////////////////////////////
                DateTime? currentCustTime = customerCurrentTime.CurrentTime;
                PMCustomerElement customer = ParkmobileCustomerRetriever.GetCustomer(70562);
                ParkMobileGenericFactory factory = new ParkMobileGenericFactory(customer, param.PlateNumber);
                var parkMobileResLog2 = new VendorResponseEntity();
                logData.VendorData.Add(VendorNames.ParkMobile2, parkMobileResLog2);
                if (!string.IsNullOrEmpty(param.ZoneID))
                    tranDataOfServiceParkMobile2 = factory.GetRefreshSpaceZoneData(ParkingMobileDataType.Plate, param.ZoneID, ref parkMobileResLog2);
                else
                    tranDataOfServiceParkMobile2 = factory.GetRefreshSpaceData(ParkingMobileDataType.Plate, ref parkMobileResLog2);
                tranDataOfServiceParkMobile2.EnforcementKey = EnforcementKey;

                ///////////////////////////////////////////////////
                logData.Response = new ResponseEntity() { CustomerId = param.CustomerId.Value.ToString(), MeterId = tranData.MeterName, SpaceId = tranData.SpaceNo, CityCurrentTime = currentCustTime.HasValue ? currentCustTime.Value : DateTime.Now };
            }
            catch (Exception ex)
            {
                logData.Response = new ResponseEntity() { CustomerId = param.CustomerId.Value.ToString(), MeterId = tranData.MeterName, SpaceId = tranData.SpaceNo, CityCurrentTime = customerCurrentTime.CurrentTime.HasValue ? customerCurrentTime.CurrentTime.Value : DateTime.Now };
                //throw ex;
                tranDataOfServiceParkMobile.ReturnCode = ((int)ReturnCodeEnum.Error).ToString();
            }
            finally
            {

            }
            #endregion
            #region "Cale Service Call"
            /////////////////////////////////////////////////
            TransactionTransform tranDataOfServiceCale = new TransactionTransform();
            try
            {

                //////////////////////////////////////////
                logData.Request.CustomerId = param.CustomerId.Value.ToString();
                logData.Request.SpaceId = param.parkingSpaceID;
                logData.Request.MeterId = param.MeterName;
                //////////////////////////////////////////////////
                DateTime? currentCustTime = customerCurrentTime.CurrentTime;
                Cale.CaleCustomerElement customer = Cale.CaleCustomerRetriever.GetCustomer(param.CustomerId.Value);
                Cale.Schema5.CaleSchema5GenericFactory factory = new Cale.Schema5.CaleSchema5GenericFactory(customer, param.PlateNumber, null);
                var caleResLog = new VendorResponseEntity();
                logData.VendorData.Add(VendorNames.Cale, caleResLog);
                if (customer.EnforcementType == EnumEnforcementType.PayByPlate.ToString())
                    if (string.IsNullOrEmpty(param.ZoneID))
                        tranDataOfServiceCale = factory.GetRefreshPlateDataPhiladelphia(ref caleResLog);
                    else
                        tranDataOfServiceCale = factory.GetRefreshPlateData(ref caleResLog, param.ZoneID);

                tranDataOfServiceCale.EnforcementKey = EnforcementKey;

                ///////////////////////////////////////////////////
                logData.Response = new ResponseEntity() { CustomerId = param.CustomerId.Value.ToString(), MeterId = tranData.MeterName, SpaceId = tranData.SpaceNo, CityCurrentTime = currentCustTime.HasValue ? currentCustTime.Value : DateTime.Now };
            }
            catch (Exception ex)
            {
                logData.Response = new ResponseEntity() { CustomerId = param.CustomerId.Value.ToString(), MeterId = tranData.MeterName, SpaceId = tranData.SpaceNo, CityCurrentTime = customerCurrentTime.CurrentTime.HasValue ? customerCurrentTime.CurrentTime.Value : DateTime.Now };
                //throw ex;
                tranDataOfServiceCale.ReturnCode = ((int)ReturnCodeEnum.Error).ToString();
            }
            finally
            {

            }
            #endregion
            //////////////////  //Reconcile data

            List<TransactionTransform> trans = new List<TransactionTransform>();
            trans.Add(tranDataOfServiceParkMobile); // Parkmobile
            trans.Add(tranDataOfServiceParkMobile2); // Parkmobile2
            trans.Add(tranDataOfServiceCale); // Cale  //revert after testing
            trans.Add(tranData); // Database          //revert after testing

            var rightValue = trans.Where(t => t.ReturnCode == ((int)ReturnCodeEnum.Success).ToString()).OrderByDescending(t => t.MeterExpiredMinutes.ToNullableInt()).FirstOrDefault<TransactionTransform>();
            if (rightValue == default(TransactionTransform))
            {
                //Do nothing.just return database  object.
            }
            else
            {
                tranData = rightValue;
            }
            return tranData;
        }



        private static Transaction GetDataPhiladelphia(int customerID, string MeterName, string parkingSpaceID, string PlateNumber, string statename, string EnforcementKey, string filePath, RequestResponseEntity logData)
        {
            string ZoneID = null;
            string SpaceStatus = null;
            Transaction data = new Transaction();
            try
            {
                List<Transaction> transactionList = new List<Transaction>();

                logData.Request.CustomerId = customerID.ToString();
                logData.Request.SpaceId = parkingSpaceID;
                logData.Request.MeterId = MeterName;

                transactionList = DataAccess.GetPhiladelphiaCitationData(customerID, null, MeterName, parkingSpaceID, PlateNumber, statename, null);

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
            }
            catch (Exception ex)
            {
                var appPath = System.Web.Hosting.HostingEnvironment.ApplicationPhysicalPath;
                string errorPath = Path.Combine(appPath, "GetData", "Timezones");
                ExceptionLogging.SendErrorToText(ex, "GetCitationData", errorPath);
                data.ReturnCode = ((int)ReturnCodeEnum.Error).ToString();
                throw;
            }
            return data;
        }

        private static Transaction GetDataPhiladelphiaZone(int customerID, string MeterName, string parkingSpaceID, string PlateNumber, string statename, string zoneName, string EnforcementKey, string filePath, RequestResponseEntity logData)
        {
            string ZoneID = null;
            string SpaceStatus = null;
            Transaction data = new Transaction();
            try
            {
                List<Transaction> transactionList = new List<Transaction>();

                logData.Request.CustomerId = customerID.ToString();
                logData.Request.SpaceId = parkingSpaceID;
                logData.Request.MeterId = MeterName;
                ZoneID = zoneName;

                transactionList = DataAccess.GetPhiladelphiaCitationData(customerID, ZoneID, MeterName, parkingSpaceID, PlateNumber, statename, null);

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
            }
            catch (Exception ex)
            {
                var appPath = System.Web.Hosting.HostingEnvironment.ApplicationPhysicalPath;
                string errorPath = Path.Combine(appPath, "GetData", "Timezones");
                ExceptionLogging.SendErrorToText(ex, "GetCitationData", errorPath);
                data.ReturnCode = ((int)ReturnCodeEnum.Error).ToString();
                throw;
            }
            return data;
        }


        private static TransactionTransform GetPhiladelphiaTransformDataBySpace(string EnforcementKey, string filePath, RequestResponseEntity logData, CustomerTime customerCurrentTime, KeyParameter param)
        {
            TransactionTransform tranData = new TransactionTransform();
            TimeZoneInfo TZ = null;
            TZ = TimeZoneInfo.FindSystemTimeZoneById("Eastern Standard Time");

            #region "DB Check"
            try
            {
                //ZoneId is ZoneName i.e. Article Name 
                Transaction data = TransactionDataValidatioin.GetDataPhiladelphiaZoneSpace(param.CustomerId.Value, param.MeterName, param.parkingSpaceID, param.PlateNumber, param.StateName, param.ZoneID, EnforcementKey, filePath, logData);


                if (data.TimeZoneId.HasValue)
                    TZ = Utility.GetTimeZoneInfo(data.TimeZoneId.Value, filePath, param.CustomerId.Value.ToString());

                //For South Miami : PayStationId. Others it is MeterID
                tranData.MeterName = (data.MeterName == null) ? string.Empty : data.MeterName;
                tranData.SpaceNo = data.ParkingSpaceID.ToString();

                tranData.MeterStreet = (data.MeterStreet == null) ? string.Empty : data.MeterStreet;

                DateTime _MeterRTC = data.PresentMeterTime.HasValue ? data.PresentMeterTime.Value : DateTime.Now;
                tranData.MeterRTC = String.Format("{0:yyyyMMdd_THHmmssZ}", Utility.findGMTTime(_MeterRTC, TZ));

                DateTime _SensorEventTime = data.SensorEventTime.HasValue ? data.SensorEventTime.Value : DateTime.Now;
                tranData.MeterLastUpdateTime = String.Format("{0:yyyyMMdd_THHmmssZ}", Utility.findGMTTime(_SensorEventTime, TZ));

                tranData.SensorEventTime = String.Format("{0:yyyyMMdd_THHmmssZ}", Utility.findGMTTime(_SensorEventTime, TZ));

                DateTime _endTime = data.ExpiryTime.HasValue ? data.ExpiryTime.Value : DateTime.Now;
                tranData.ExpiredTime = String.Format("{0:yyyyMMdd_THHmmssZ}", Utility.findGMTTime(_endTime, TZ));

                tranData.MeterExpiredMinutes = data.ExpiredMinutes.HasValue ? data.ExpiredMinutes.Value.ToString() : "0";
                tranData.METERLOC_BLOCKNUMBER = (data.Block == null) ? string.Empty : data.Block;
                tranData.METERLOC_STREETDIRECTION = (data.Direction == null) ? string.Empty : data.Direction;
                tranData.METERLOC_STREETNAME = (data.MeterStreet == null) ? string.Empty : data.MeterStreet;
                tranData.METERLOC_STREETTYPE = (data.StreetType == null) ? string.Empty : data.StreetType;
                tranData.METERLOC_CROSSSTREET1 = (data.CrossStreet1 == null) ? string.Empty : data.CrossStreet1;
                tranData.METERLOC_CROSSSTREET2 = (data.CrossStreet2 == null) ? string.Empty : data.CrossStreet2;
                tranData.METERLOC_ENFORCEMENTHOURSDESC = (data.EnfHourDesc == null) ? string.Empty : data.EnfHourDesc;
                tranData.LicensePlate = (data.PlateNumber == null) ? string.Empty : data.PlateNumber;
                tranData.State = (data.LPLocation == null) ? string.Empty : data.LPLocation;
                tranData.Make = (data.VMake == null) ? string.Empty : data.VMake;
                tranData.Model = (data.VModel == null) ? string.Empty : data.VModel;
                tranData.IsOccupied = Utility.getStatus((data.SpaceStatus == null) ? string.Empty : data.SpaceStatus).ToString();
                tranData.EnforcementKey = (data.EnforcementKey == null) ? string.Empty : data.EnforcementKey;
                tranData.MeterStatus = (data.MeterStatus == null) ? null : data.MeterStatus;

                tranData.METERLOC_METERTYPE = (data.METERLOC_METERTYPE == null) ? null : data.METERLOC_METERTYPE;
                tranData.ZoneName = (data.ZoneName == null) ? null : data.ZoneName;

                tranData.MeterID = data.PayStationID.HasValue ? data.PayStationID.Value.ToString() : "0";
                tranData.ReturnCode = data.ReturnCode;

                logData.Response = new ResponseEntity() { CustomerId = param.CustomerId.Value.ToString(), MeterId = tranData.MeterName, SpaceId = tranData.SpaceNo, CityCurrentTime = data.PresentMeterTime.HasValue ? data.PresentMeterTime.Value : DateTime.Now };
            }
            catch (Exception ex)
            {
                //var appPath = System.Web.Hosting.HostingEnvironment.ApplicationPhysicalPath;
                //string errorPath = Path.Combine(appPath, "GetTransformData", "Timezones");
                //ExceptionLogging.SendErrorToText(ex, "GetCitationData", errorPath);
                //tranData.ReturnCode = ((int)ReturnCodeEnum.Error).ToString();

                tranData.ReturnCode = ((int)ReturnCodeEnum.Error).ToString();
            }
            #endregion

            return tranData;
        }

        private static Transaction GetDataPhiladelphiaZoneSpace(int customerID, string MeterName, string parkingSpaceID, string PlateNumber, string statename, string zoneName, string EnforcementKey, string filePath, RequestResponseEntity logData)
        {
            string ZoneID = null;
            string SpaceStatus = null;
            Transaction data = new Transaction();
            try
            {
                List<Transaction> transactionList = new List<Transaction>();

                logData.Request.CustomerId = customerID.ToString();
                logData.Request.SpaceId = parkingSpaceID;
                logData.Request.MeterId = MeterName;
                ZoneID = zoneName;

                transactionList = DataAccess.GetPhiladelphiaCitationDataSpace(customerID, ZoneID, MeterName, parkingSpaceID, PlateNumber, statename, null);

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
            }
            catch (Exception ex)
            {
                var appPath = System.Web.Hosting.HostingEnvironment.ApplicationPhysicalPath;
                string errorPath = Path.Combine(appPath, "GetData", "Timezones");
                ExceptionLogging.SendErrorToText(ex, "GetCitationData", errorPath);
                data.ReturnCode = ((int)ReturnCodeEnum.Error).ToString();
                throw;
            }
            return data;
        }

        public static TransactionTransform GetPhiladelphiaTransformVendor(VendorNames vendor, string EnforcementKey, string filePath, RequestResponseEntity logData, CustomerTime customerCurrentTime)
        {
            TransactionTransform tranData = new TransactionTransform();
            tranData.ReturnCode = ((int)ReturnCodeEnum.InvalidInputParameters).ToString();
            TimeZoneInfo TZ = null;
            TZ = TimeZoneInfo.FindSystemTimeZoneById("Eastern Standard Time");

            KeyParameter param = PrepareKeyParameters.PreparePhiladelphia(EnforcementKey);
            if (param.ReturnCode == ((int)ReturnCodeEnum.InvalidInputParameters).ToString())
            {
                return tranData;
            }



            return tranData;
        }

        #endregion

        #region Auburn
        public static TransactionTransform GetAuburnTransformData(string EnforcementKey, string filePath, RequestResponseEntity logData, CustomerTime customerCurrentTime)
        {
            TransactionTransform tranData = new TransactionTransform();
            tranData.ReturnCode = ((int)ReturnCodeEnum.InvalidInputParameters).ToString();
            TimeZoneInfo TZ = null;
            TZ = TimeZoneInfo.FindSystemTimeZoneById("Eastern Standard Time");
            //int? customerID = null;
            //string MeterName = null;
            //string parkingSpaceID;
            //string PlateNumber = null;
            //string stateName = null;
            //////////////////////////////////////
            KeyParameter param = PrepareKeyParameters.PrepareAuburn(EnforcementKey);
            if (param.ReturnCode == ((int)ReturnCodeEnum.InvalidInputParameters).ToString())
            {
                return tranData;
            }
            /////////////////////////////////////
            #region "DB Check"
            try
            {
                Transaction data = TransactionDataValidatioin.GetDataAuburn(param.CustomerId.Value, param.MeterName, param.parkingSpaceID, param.PlateNumber, param.StateName, EnforcementKey, filePath, logData);


                if (data.TimeZoneId.HasValue)
                    TZ = Utility.GetTimeZoneInfo(data.TimeZoneId.Value, filePath, param.CustomerId.Value.ToString());

                //For South Miami : PayStationId. Others it is MeterID
                tranData.MeterName = string.Empty; // (data.MeterName == null) ? string.Empty : data.MeterName;
                tranData.SpaceNo = string.Empty;  //data.ParkingSpaceID.ToString();

                tranData.MeterStreet = (data.MeterStreet == null) ? string.Empty : data.MeterStreet;

                DateTime _MeterRTC = data.PresentMeterTime.HasValue ? data.PresentMeterTime.Value : DateTime.Now;
                tranData.MeterRTC = String.Format("{0:yyyyMMdd_THHmmssZ}", Utility.findGMTTime(_MeterRTC, TZ));

                DateTime _SensorEventTime = data.SensorEventTime.HasValue ? data.SensorEventTime.Value : DateTime.Now;
                tranData.MeterLastUpdateTime = String.Format("{0:yyyyMMdd_THHmmssZ}", Utility.findGMTTime(_SensorEventTime, TZ));

                tranData.SensorEventTime = String.Format("{0:yyyyMMdd_THHmmssZ}", Utility.findGMTTime(_SensorEventTime, TZ));

                DateTime _endTime = data.ExpiryTime.HasValue ? data.ExpiryTime.Value : DateTime.Now;
                tranData.ExpiredTime = String.Format("{0:yyyyMMdd_THHmmssZ}", Utility.findGMTTime(_endTime, TZ));

                tranData.MeterExpiredMinutes = data.ExpiredMinutes.HasValue ? data.ExpiredMinutes.Value.ToString() : "0";
                tranData.METERLOC_BLOCKNUMBER = (data.Block == null) ? string.Empty : data.Block;
                tranData.METERLOC_STREETDIRECTION = (data.Direction == null) ? string.Empty : data.Direction;
                tranData.METERLOC_STREETNAME = (data.MeterStreet == null) ? string.Empty : data.MeterStreet;
                tranData.METERLOC_STREETTYPE = (data.StreetType == null) ? string.Empty : data.StreetType;
                tranData.METERLOC_CROSSSTREET1 = (data.CrossStreet1 == null) ? string.Empty : data.CrossStreet1;
                tranData.METERLOC_CROSSSTREET2 = (data.CrossStreet2 == null) ? string.Empty : data.CrossStreet2;
                tranData.METERLOC_ENFORCEMENTHOURSDESC = (data.EnfHourDesc == null) ? string.Empty : data.EnfHourDesc;
                tranData.LicensePlate = (data.PlateNumber == null) ? string.Empty : data.PlateNumber;
                tranData.State = (data.LPLocation == null) ? string.Empty : data.LPLocation;
                tranData.Make = (data.VMake == null) ? string.Empty : data.VMake;
                tranData.Model = (data.VModel == null) ? string.Empty : data.VModel;
                tranData.IsOccupied = Utility.getStatus((data.SpaceStatus == null) ? string.Empty : data.SpaceStatus).ToString();
                tranData.EnforcementKey = (data.EnforcementKey == null) ? string.Empty : data.EnforcementKey;
                tranData.MeterStatus = (data.MeterStatus == null) ? null : data.MeterStatus;

                tranData.METERLOC_METERTYPE = (data.METERLOC_METERTYPE == null) ? null : data.METERLOC_METERTYPE;
                tranData.ZoneName = (data.ZoneName == null) ? null : data.ZoneName;
                tranData.MeterID = String.Empty;
                tranData.ReturnCode = data.ReturnCode;

                logData.Response = new ResponseEntity() { CustomerId = param.CustomerId.Value.ToString(), MeterId = tranData.MeterName, SpaceId = tranData.SpaceNo, CityCurrentTime = data.PresentMeterTime.HasValue ? data.PresentMeterTime.Value : DateTime.Now };
            }
            catch (Exception ex)
            {
                //var appPath = System.Web.Hosting.HostingEnvironment.ApplicationPhysicalPath;
                //string errorPath = Path.Combine(appPath, "GetTransformData", "Timezones");
                //ExceptionLogging.SendErrorToText(ex, "GetCitationData", errorPath);
                //tranData.ReturnCode = ((int)ReturnCodeEnum.Error).ToString();

                tranData.ReturnCode = ((int)ReturnCodeEnum.Error).ToString();
            }
            #endregion
            #region "PANGO Service Call"
            /////////////////////////////////////////////////
            TransactionTransform tranDataOfService = new TransactionTransform();
            try
            {
                //////////////////////////////////////////
                logData.Request.CustomerId = param.CustomerId.Value.ToString();
                logData.Request.SpaceId = param.parkingSpaceID;
                logData.Request.MeterId = param.MeterName;
                //////////////////////////////////////////////////
                DateTime? currentCustTime = customerCurrentTime.CurrentTime;
                PangoCustomerElement customer = PangoCustomerRetriever.GetCustomer(param.CustomerId.Value);//CustomerRetriever.GetCustomer(param.CustomerId.Value);
                PangoGenericFactory factory = new PangoGenericFactory(customer, param.PlateNumber, param.StateName);
                tranDataOfService = factory.GetPlateData();
                tranDataOfService.EnforcementKey = EnforcementKey;

                ///////////////////////////////////////////////////
                logData.Response = new ResponseEntity() { CustomerId = param.CustomerId.Value.ToString(), MeterId = tranData.MeterName, SpaceId = tranData.SpaceNo, CityCurrentTime = currentCustTime.HasValue ? currentCustTime.Value : DateTime.Now };

            }
            catch (Exception ex)
            {
                logData.Response = new ResponseEntity() { CustomerId = param.CustomerId.Value.ToString(), MeterId = tranData.MeterName, SpaceId = tranData.SpaceNo, CityCurrentTime = customerCurrentTime.CurrentTime.HasValue ? customerCurrentTime.CurrentTime.Value : DateTime.Now };
                //throw ex;
                tranData.ReturnCode = ((int)ReturnCodeEnum.Error).ToString();
            }
            finally
            {

            }
            #endregion

            //Reconcile data
            if ((tranData.ReturnCode == ((int)ReturnCodeEnum.NoRecordExists).ToString()) || (tranData.ReturnCode == ((int)ReturnCodeEnum.Error).ToString()))
            {
                tranData = tranDataOfService;
            }
            else if (tranData.ReturnCode == ((int)ReturnCodeEnum.Success).ToString())
            {
                if (tranDataOfService.ReturnCode == ((int)ReturnCodeEnum.Success).ToString())
                {
                    ////Select one record of highest expiry time
                    int? dbExpiredMinutes = tranData.MeterExpiredMinutes.ToNullableInt();
                    int? parkeonExpiredMinutes = tranDataOfService.MeterExpiredMinutes.ToNullableInt();
                    tranData = Nullable.Compare(dbExpiredMinutes, parkeonExpiredMinutes) > 0 ? tranData : tranDataOfService;
                }

            }

            else if (tranData.ReturnCode == ((int)ReturnCodeEnum.MoreRecordsExists).ToString())
            {
                // DB never give more record. hence ignore
            }

            return tranData;
        }
        public static TransactionTransform GetAuburnTransformDataV2(string EnforcementKey, string filePath, RequestResponseEntity logData, CustomerTime customerCurrentTime)
        {
            TransactionTransform tranData = new TransactionTransform();
            tranData.ReturnCode = ((int)ReturnCodeEnum.InvalidInputParameters).ToString();
            TimeZoneInfo TZ = null;
            TZ = TimeZoneInfo.FindSystemTimeZoneById("Eastern Standard Time");
            //int? customerID = null;
            //string MeterName = null;
            //string parkingSpaceID;
            //string PlateNumber = null;
            //string stateName = null;
            //////////////////////////////////////
            KeyParameter param = PrepareKeyParameters.PrepareAuburn(EnforcementKey);
            if (param.ReturnCode == ((int)ReturnCodeEnum.InvalidInputParameters).ToString())
            {
                return tranData;
            }
            /////////////////////////////////////
            #region "DB Check"
            try
            {
                Transaction data = TransactionDataValidatioin.GetDataAuburn(param.CustomerId.Value, param.MeterName, param.parkingSpaceID, param.PlateNumber, param.StateName, EnforcementKey, filePath, logData);


                if (data.TimeZoneId.HasValue)
                    TZ = Utility.GetTimeZoneInfo(data.TimeZoneId.Value, filePath, param.CustomerId.Value.ToString());

                //For South Miami : PayStationId. Others it is MeterID
                tranData.MeterName = string.Empty; // (data.MeterName == null) ? string.Empty : data.MeterName;
                tranData.SpaceNo = string.Empty;  //data.ParkingSpaceID.ToString();

                tranData.MeterStreet = (data.MeterStreet == null) ? string.Empty : data.MeterStreet;

                DateTime _MeterRTC = data.PresentMeterTime.HasValue ? data.PresentMeterTime.Value : DateTime.Now;
                tranData.MeterRTC = String.Format("{0:yyyyMMdd_THHmmssZ}", Utility.findGMTTime(_MeterRTC, TZ));

                DateTime _SensorEventTime = data.SensorEventTime.HasValue ? data.SensorEventTime.Value : DateTime.Now;
                tranData.MeterLastUpdateTime = String.Format("{0:yyyyMMdd_THHmmssZ}", Utility.findGMTTime(_SensorEventTime, TZ));

                tranData.SensorEventTime = String.Format("{0:yyyyMMdd_THHmmssZ}", Utility.findGMTTime(_SensorEventTime, TZ));

                DateTime _endTime = data.ExpiryTime.HasValue ? data.ExpiryTime.Value : DateTime.Now;
                tranData.ExpiredTime = String.Format("{0:yyyyMMdd_THHmmssZ}", Utility.findGMTTime(_endTime, TZ));

                tranData.MeterExpiredMinutes = data.ExpiredMinutes.HasValue ? data.ExpiredMinutes.Value.ToString() : "0";
                tranData.METERLOC_BLOCKNUMBER = (data.Block == null) ? string.Empty : data.Block;
                tranData.METERLOC_STREETDIRECTION = (data.Direction == null) ? string.Empty : data.Direction;
                tranData.METERLOC_STREETNAME = (data.MeterStreet == null) ? string.Empty : data.MeterStreet;
                tranData.METERLOC_STREETTYPE = (data.StreetType == null) ? string.Empty : data.StreetType;
                tranData.METERLOC_CROSSSTREET1 = (data.CrossStreet1 == null) ? string.Empty : data.CrossStreet1;
                tranData.METERLOC_CROSSSTREET2 = (data.CrossStreet2 == null) ? string.Empty : data.CrossStreet2;
                tranData.METERLOC_ENFORCEMENTHOURSDESC = (data.EnfHourDesc == null) ? string.Empty : data.EnfHourDesc;
                tranData.LicensePlate = (data.PlateNumber == null) ? string.Empty : data.PlateNumber;
                tranData.State = (data.LPLocation == null) ? string.Empty : data.LPLocation;
                tranData.Make = (data.VMake == null) ? string.Empty : data.VMake;
                tranData.Model = (data.VModel == null) ? string.Empty : data.VModel;
                tranData.IsOccupied = Utility.getStatus((data.SpaceStatus == null) ? string.Empty : data.SpaceStatus).ToString();
                tranData.EnforcementKey = (data.EnforcementKey == null) ? string.Empty : data.EnforcementKey;
                tranData.MeterStatus = (data.MeterStatus == null) ? null : data.MeterStatus;

                tranData.METERLOC_METERTYPE = (data.METERLOC_METERTYPE == null) ? null : data.METERLOC_METERTYPE;
                tranData.ZoneName = (data.ZoneName == null) ? null : data.ZoneName;
                tranData.MeterID = String.Empty;
                tranData.ReturnCode = data.ReturnCode;

                logData.Response = new ResponseEntity() { CustomerId = param.CustomerId.Value.ToString(), MeterId = tranData.MeterName, SpaceId = tranData.SpaceNo, CityCurrentTime = data.PresentMeterTime.HasValue ? data.PresentMeterTime.Value : DateTime.Now };
            }
            catch (Exception ex)
            {
                //var appPath = System.Web.Hosting.HostingEnvironment.ApplicationPhysicalPath;
                //string errorPath = Path.Combine(appPath, "GetTransformData", "Timezones");
                //ExceptionLogging.SendErrorToText(ex, "GetCitationData", errorPath);
                //tranData.ReturnCode = ((int)ReturnCodeEnum.Error).ToString();

                tranData.ReturnCode = ((int)ReturnCodeEnum.Error).ToString();
            }
            #endregion
            #region "PANGO Service Call"
            /////////////////////////////////////////////////
            TransactionTransform tranDataOfService = new TransactionTransform();
            try
            {
                //////////////////////////////////////////
                logData.Request.CustomerId = param.CustomerId.Value.ToString();
                logData.Request.SpaceId = param.parkingSpaceID;
                logData.Request.MeterId = param.MeterName;
                //////////////////////////////////////////////////
                DateTime? currentCustTime = customerCurrentTime.CurrentTime;
                PangoCustomerElement customer = PangoCustomerRetriever.GetCustomer(param.CustomerId.Value);//CustomerRetriever.GetCustomer(param.CustomerId.Value);
                PangoGenericFactory factory = new PangoGenericFactory(customer, param.PlateNumber, param.StateName);
                var pangoResLog = new VendorResponseEntity();
                logData.VendorData.Add(VendorNames.Pango, pangoResLog);
                tranDataOfService = factory.GetPlateData(ref pangoResLog);
                tranDataOfService.EnforcementKey = EnforcementKey;

                ///////////////////////////////////////////////////
                logData.Response = new ResponseEntity() { CustomerId = param.CustomerId.Value.ToString(), MeterId = tranData.MeterName, SpaceId = tranData.SpaceNo, CityCurrentTime = currentCustTime.HasValue ? currentCustTime.Value : DateTime.Now };

            }
            catch (Exception ex)
            {
                logData.Response = new ResponseEntity() { CustomerId = param.CustomerId.Value.ToString(), MeterId = tranData.MeterName, SpaceId = tranData.SpaceNo, CityCurrentTime = customerCurrentTime.CurrentTime.HasValue ? customerCurrentTime.CurrentTime.Value : DateTime.Now };
                //throw ex;
                tranData.ReturnCode = ((int)ReturnCodeEnum.Error).ToString();
            }
            finally
            {

            }
            #endregion

            //Reconcile data
            if ((tranData.ReturnCode == ((int)ReturnCodeEnum.NoRecordExists).ToString()) || (tranData.ReturnCode == ((int)ReturnCodeEnum.Error).ToString()))
            {
                tranData = tranDataOfService;
            }
            else if (tranData.ReturnCode == ((int)ReturnCodeEnum.Success).ToString())
            {
                if (tranDataOfService.ReturnCode == ((int)ReturnCodeEnum.Success).ToString())
                {
                    ////Select one record of highest expiry time
                    int? dbExpiredMinutes = tranData.MeterExpiredMinutes.ToNullableInt();
                    int? parkeonExpiredMinutes = tranDataOfService.MeterExpiredMinutes.ToNullableInt();
                    tranData = Nullable.Compare(dbExpiredMinutes, parkeonExpiredMinutes) > 0 ? tranData : tranDataOfService;
                }

            }

            else if (tranData.ReturnCode == ((int)ReturnCodeEnum.MoreRecordsExists).ToString())
            {
                // DB never give more record. hence ignore
            }

            return tranData;
        }
        public static TransactionTransform GetAuburnTransformDataV3Test(string EnforcementKey, string filePath, RequestResponseEntity logData, CustomerTime customerCurrentTime)
        {
            TransactionTransform tranData = new TransactionTransform();
            tranData.ReturnCode = ((int)ReturnCodeEnum.InvalidInputParameters).ToString();
            TimeZoneInfo TZ = null;
            TZ = TimeZoneInfo.FindSystemTimeZoneById("Eastern Standard Time");
            //int? customerID = null;
            //string MeterName = null;
            //string parkingSpaceID;
            //string PlateNumber = null;
            //string stateName = null;
            //////////////////////////////////////
            KeyParameter param = PrepareKeyParameters.PrepareAuburn(EnforcementKey);
            if (param.ReturnCode == ((int)ReturnCodeEnum.InvalidInputParameters).ToString())
            {
                return tranData;
            }
            /////////////////////////////////////
            #region "DB Check"
            try
            {
                Transaction data = TransactionDataValidatioin.GetDataAuburn(param.CustomerId.Value, param.MeterName, param.parkingSpaceID, param.PlateNumber, param.StateName, EnforcementKey, filePath, logData);


                if (data.TimeZoneId.HasValue)
                    TZ = Utility.GetTimeZoneInfo(data.TimeZoneId.Value, filePath, param.CustomerId.Value.ToString());

                //For South Miami : PayStationId. Others it is MeterID
                tranData.MeterName = string.Empty; // (data.MeterName == null) ? string.Empty : data.MeterName;
                tranData.SpaceNo = string.Empty;  //data.ParkingSpaceID.ToString();

                tranData.MeterStreet = (data.MeterStreet == null) ? string.Empty : data.MeterStreet;

                DateTime _MeterRTC = data.PresentMeterTime.HasValue ? data.PresentMeterTime.Value : DateTime.Now;
                tranData.MeterRTC = String.Format("{0:yyyyMMdd_THHmmssZ}", Utility.findGMTTime(_MeterRTC, TZ));

                DateTime _SensorEventTime = data.SensorEventTime.HasValue ? data.SensorEventTime.Value : DateTime.Now;
                tranData.MeterLastUpdateTime = String.Format("{0:yyyyMMdd_THHmmssZ}", Utility.findGMTTime(_SensorEventTime, TZ));

                tranData.SensorEventTime = String.Format("{0:yyyyMMdd_THHmmssZ}", Utility.findGMTTime(_SensorEventTime, TZ));

                DateTime _endTime = data.ExpiryTime.HasValue ? data.ExpiryTime.Value : DateTime.Now;
                tranData.ExpiredTime = String.Format("{0:yyyyMMdd_THHmmssZ}", Utility.findGMTTime(_endTime, TZ));

                tranData.MeterExpiredMinutes = data.ExpiredMinutes.HasValue ? data.ExpiredMinutes.Value.ToString() : "0";
                tranData.METERLOC_BLOCKNUMBER = (data.Block == null) ? string.Empty : data.Block;
                tranData.METERLOC_STREETDIRECTION = (data.Direction == null) ? string.Empty : data.Direction;
                tranData.METERLOC_STREETNAME = (data.MeterStreet == null) ? string.Empty : data.MeterStreet;
                tranData.METERLOC_STREETTYPE = (data.StreetType == null) ? string.Empty : data.StreetType;
                tranData.METERLOC_CROSSSTREET1 = (data.CrossStreet1 == null) ? string.Empty : data.CrossStreet1;
                tranData.METERLOC_CROSSSTREET2 = (data.CrossStreet2 == null) ? string.Empty : data.CrossStreet2;
                tranData.METERLOC_ENFORCEMENTHOURSDESC = (data.EnfHourDesc == null) ? string.Empty : data.EnfHourDesc;
                tranData.LicensePlate = (data.PlateNumber == null) ? string.Empty : data.PlateNumber;
                tranData.State = (data.LPLocation == null) ? string.Empty : data.LPLocation;
                tranData.Make = (data.VMake == null) ? string.Empty : data.VMake;
                tranData.Model = (data.VModel == null) ? string.Empty : data.VModel;
                tranData.IsOccupied = Utility.getStatus((data.SpaceStatus == null) ? string.Empty : data.SpaceStatus).ToString();
                tranData.EnforcementKey = (data.EnforcementKey == null) ? string.Empty : data.EnforcementKey;
                tranData.MeterStatus = (data.MeterStatus == null) ? null : data.MeterStatus;

                tranData.METERLOC_METERTYPE = (data.METERLOC_METERTYPE == null) ? null : data.METERLOC_METERTYPE;
                tranData.ZoneName = (data.ZoneName == null) ? null : data.ZoneName;
                tranData.MeterID = String.Empty;
                tranData.ReturnCode = data.ReturnCode;

                logData.Response = new ResponseEntity() { CustomerId = param.CustomerId.Value.ToString(), MeterId = tranData.MeterName, SpaceId = tranData.SpaceNo, CityCurrentTime = data.PresentMeterTime.HasValue ? data.PresentMeterTime.Value : DateTime.Now };
            }
            catch (Exception ex)
            {
                //var appPath = System.Web.Hosting.HostingEnvironment.ApplicationPhysicalPath;
                //string errorPath = Path.Combine(appPath, "GetTransformData", "Timezones");
                //ExceptionLogging.SendErrorToText(ex, "GetCitationData", errorPath);
                //tranData.ReturnCode = ((int)ReturnCodeEnum.Error).ToString();

                tranData.ReturnCode = ((int)ReturnCodeEnum.Error).ToString();
            }
            #endregion
            #region "PANGO Service Call"
            /////////////////////////////////////////////////
            TransactionTransform tranDataOfService = new TransactionTransform();
            try
            {
                //////////////////////////////////////////
                logData.Request.CustomerId = param.CustomerId.Value.ToString();
                logData.Request.SpaceId = param.parkingSpaceID;
                logData.Request.MeterId = param.MeterName;
                //////////////////////////////////////////////////
                DateTime? currentCustTime = customerCurrentTime.CurrentTime;
                PangoCustomerElement customer = PangoCustomerRetriever.GetCustomer(param.CustomerId.Value);//CustomerRetriever.GetCustomer(param.CustomerId.Value);
                //PangoGenericFactory factory = new PangoGenericFactory(customer, param.PlateNumber, param.StateName);
                PangoHttpClient factory = new PangoHttpClient(customer, param.PlateNumber, param.StateName, @"https://pango.mypango.com/EnforcementSVC/wservice.asmx", @"CheckPlateNumberWithUserNameAndZone_ex3");
                var pangoResLog = new VendorResponseEntity();
                logData.VendorData.Add(VendorNames.Pango, pangoResLog);
                tranDataOfService = factory.GetPlateData(ref pangoResLog);
                tranDataOfService.EnforcementKey = EnforcementKey;

                ///////////////////////////////////////////////////
                logData.Response = new ResponseEntity() { CustomerId = param.CustomerId.Value.ToString(), MeterId = tranData.MeterName, SpaceId = tranData.SpaceNo, CityCurrentTime = currentCustTime.HasValue ? currentCustTime.Value : DateTime.Now };

            }
            catch (Exception ex)
            {
                logData.Response = new ResponseEntity() { CustomerId = param.CustomerId.Value.ToString(), MeterId = tranData.MeterName, SpaceId = tranData.SpaceNo, CityCurrentTime = customerCurrentTime.CurrentTime.HasValue ? customerCurrentTime.CurrentTime.Value : DateTime.Now };
                //throw ex;
                tranData.ReturnCode = ((int)ReturnCodeEnum.Error).ToString();
            }
            finally
            {

            }
            #endregion

            //Reconcile data
            if ((tranData.ReturnCode == ((int)ReturnCodeEnum.NoRecordExists).ToString()) || (tranData.ReturnCode == ((int)ReturnCodeEnum.Error).ToString()))
            {
                tranData = tranDataOfService;
            }
            else if (tranData.ReturnCode == ((int)ReturnCodeEnum.Success).ToString())
            {
                if (tranDataOfService.ReturnCode == ((int)ReturnCodeEnum.Success).ToString())
                {
                    ////Select one record of highest expiry time
                    int? dbExpiredMinutes = tranData.MeterExpiredMinutes.ToNullableInt();
                    int? parkeonExpiredMinutes = tranDataOfService.MeterExpiredMinutes.ToNullableInt();
                    tranData = Nullable.Compare(dbExpiredMinutes, parkeonExpiredMinutes) > 0 ? tranData : tranDataOfService;
                }

            }

            else if (tranData.ReturnCode == ((int)ReturnCodeEnum.MoreRecordsExists).ToString())
            {
                // DB never give more record. hence ignore
            }

            return tranData;
        }
        private static Transaction GetDataAuburn(int customerID, string MeterName, string parkingSpaceID, string PlateNumber, string statename, string EnforcementKey, string filePath, RequestResponseEntity logData)
        {
            string ZoneID = null;
            string SpaceStatus = null;
            Transaction data = new Transaction();
            try
            {
                List<Transaction> transactionList = new List<Transaction>();

                logData.Request.CustomerId = customerID.ToString();
                logData.Request.SpaceId = parkingSpaceID;
                logData.Request.MeterId = MeterName;

                transactionList = DataAccess.GetAuburnWithPlateName(customerID, null, null, null, PlateNumber, statename, null);

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
            }
            catch (Exception ex)
            {
                var appPath = System.Web.Hosting.HostingEnvironment.ApplicationPhysicalPath;
                string errorPath = Path.Combine(appPath, "GetData", "Timezones");
                ExceptionLogging.SendErrorToText(ex, "GetCitationData", errorPath);
                data.ReturnCode = ((int)ReturnCodeEnum.Error).ToString();
                throw;
            }
            return data;
        }
        #endregion

        #region Detroit
        public static TransactionTransform GetDetroitTransformData(string EnforcementKey, string filePath, RequestResponseEntity logData, CustomerTime customerCurrentTime)
        {
            TransactionTransform tranData = new TransactionTransform();
            tranData.ReturnCode = ((int)ReturnCodeEnum.InvalidInputParameters).ToString();
            TimeZoneInfo TZ = null;
            TZ = TimeZoneInfo.FindSystemTimeZoneById("Eastern Standard Time");
            //int? customerID = null;
            //string MeterName = null;
            //string parkingSpaceID;
            //string PlateNumber = null;
            //string stateName = null;
            //////////////////////////////////////
            KeyParameter param = PrepareKeyParameters.PrepareDetroit(EnforcementKey);
            if (param.ReturnCode == ((int)ReturnCodeEnum.InvalidInputParameters).ToString())
            {
                return tranData;
            }

            /////////////////////////////////////
            #region "DB Check"
            try
            {
                Transaction data = TransactionDataValidatioin.GetDataDetroit(param.CustomerId.Value, param.MeterName, param.parkingSpaceID, param.PlateNumber, param.StateName, EnforcementKey, filePath, logData);


                if (data.TimeZoneId.HasValue)
                    TZ = Utility.GetTimeZoneInfo(data.TimeZoneId.Value, filePath, param.CustomerId.Value.ToString());

                //For South Miami : PayStationId. Others it is MeterID
                tranData.MeterName = (data.MeterName == null) ? string.Empty : data.MeterName;
                tranData.SpaceNo = data.ParkingSpaceID.ToString();

                tranData.MeterStreet = (data.MeterStreet == null) ? string.Empty : data.MeterStreet;

                DateTime _MeterRTC = data.PresentMeterTime.HasValue ? data.PresentMeterTime.Value : DateTime.Now;
                tranData.MeterRTC = String.Format("{0:yyyyMMdd_THHmmssZ}", Utility.findGMTTime(_MeterRTC, TZ));

                DateTime _SensorEventTime = data.SensorEventTime.HasValue ? data.SensorEventTime.Value : DateTime.Now;
                tranData.MeterLastUpdateTime = String.Format("{0:yyyyMMdd_THHmmssZ}", Utility.findGMTTime(_SensorEventTime, TZ));

                tranData.SensorEventTime = String.Format("{0:yyyyMMdd_THHmmssZ}", Utility.findGMTTime(_SensorEventTime, TZ));

                DateTime _endTime = data.ExpiryTime.HasValue ? data.ExpiryTime.Value : DateTime.Now;
                tranData.ExpiredTime = String.Format("{0:yyyyMMdd_THHmmssZ}", Utility.findGMTTime(_endTime, TZ));

                tranData.MeterExpiredMinutes = data.ExpiredMinutes.HasValue ? data.ExpiredMinutes.Value.ToString() : "0";
                tranData.METERLOC_BLOCKNUMBER = (data.Block == null) ? string.Empty : data.Block;
                tranData.METERLOC_STREETDIRECTION = (data.Direction == null) ? string.Empty : data.Direction;
                tranData.METERLOC_STREETNAME = (data.MeterStreet == null) ? string.Empty : data.MeterStreet;
                tranData.METERLOC_STREETTYPE = (data.StreetType == null) ? string.Empty : data.StreetType;
                tranData.METERLOC_CROSSSTREET1 = (data.CrossStreet1 == null) ? string.Empty : data.CrossStreet1;
                tranData.METERLOC_CROSSSTREET2 = (data.CrossStreet2 == null) ? string.Empty : data.CrossStreet2;
                tranData.METERLOC_ENFORCEMENTHOURSDESC = (data.EnfHourDesc == null) ? string.Empty : data.EnfHourDesc;
                tranData.LicensePlate = (data.PlateNumber == null) ? string.Empty : data.PlateNumber;
                tranData.State = (data.LPLocation == null) ? string.Empty : data.LPLocation;
                tranData.Make = (data.VMake == null) ? string.Empty : data.VMake;
                tranData.Model = (data.VModel == null) ? string.Empty : data.VModel;
                tranData.IsOccupied = Utility.getStatus((data.SpaceStatus == null) ? string.Empty : data.SpaceStatus).ToString();
                tranData.EnforcementKey = (data.EnforcementKey == null) ? string.Empty : data.EnforcementKey;
                tranData.MeterStatus = (data.MeterStatus == null) ? null : data.MeterStatus;

                tranData.METERLOC_METERTYPE = (data.METERLOC_METERTYPE == null) ? null : data.METERLOC_METERTYPE;
                tranData.ZoneName = (data.ZoneName == null) ? null : data.ZoneName;

                tranData.MeterID = data.PayStationID.HasValue ? data.PayStationID.Value.ToString() : "0";
                tranData.ReturnCode = data.ReturnCode;

                logData.Response = new ResponseEntity() { CustomerId = param.CustomerId.Value.ToString(), MeterId = tranData.MeterName, SpaceId = tranData.SpaceNo, CityCurrentTime = data.PresentMeterTime.HasValue ? data.PresentMeterTime.Value : DateTime.Now };
            }
            catch (Exception ex)
            {
                //var appPath = System.Web.Hosting.HostingEnvironment.ApplicationPhysicalPath;
                //string errorPath = Path.Combine(appPath, "GetTransformData", "Timezones");
                //ExceptionLogging.SendErrorToText(ex, "GetCitationData", errorPath);
                //tranData.ReturnCode = ((int)ReturnCodeEnum.Error).ToString();

                tranData.ReturnCode = ((int)ReturnCodeEnum.Error).ToString();
            }
            #endregion

            #region "Cale Service Call"
            /////////////////////////////////////////////////
            TransactionTransform tranDataOfService = new TransactionTransform();
            try
            {
                //////////////////////////////////////////
                logData.Request.CustomerId = param.CustomerId.Value.ToString();
                logData.Request.SpaceId = param.parkingSpaceID;
                logData.Request.MeterId = param.MeterName;
                //////////////////////////////////////////////////
                DateTime? currentCustTime = customerCurrentTime.CurrentTime;
                Cale.CaleCustomerElement customer = Cale.CaleCustomerRetriever.GetCustomer(param.CustomerId.Value);
                Cale.CaleGenericFactory factory = new Cale.CaleGenericFactory(customer, param.PlateNumber, null);
                if (customer.EnforcementType == EnumEnforcementType.PayByPlate.ToString())
                    tranDataOfService = factory.GetRefreshPlateData();
                tranDataOfService.EnforcementKey = EnforcementKey;

                ///////////////////////////////////////////////////
                logData.Response = new ResponseEntity() { CustomerId = param.CustomerId.Value.ToString(), MeterId = tranData.MeterName, SpaceId = tranData.SpaceNo, CityCurrentTime = currentCustTime.HasValue ? currentCustTime.Value : DateTime.Now };
            }
            catch (Exception ex)
            {
                logData.Response = new ResponseEntity() { CustomerId = param.CustomerId.Value.ToString(), MeterId = tranData.MeterName, SpaceId = tranData.SpaceNo, CityCurrentTime = customerCurrentTime.CurrentTime.HasValue ? customerCurrentTime.CurrentTime.Value : DateTime.Now };
                //throw ex;
                tranData.ReturnCode = ((int)ReturnCodeEnum.Error).ToString();
            }
            finally
            {

            }
            #endregion
            //Reconcile data
            if ((tranData.ReturnCode == ((int)ReturnCodeEnum.NoRecordExists).ToString()) || (tranData.ReturnCode == ((int)ReturnCodeEnum.Error).ToString()))
            {
                tranData = tranDataOfService;
            }
            else if (tranData.ReturnCode == ((int)ReturnCodeEnum.Success).ToString())
            {
                if (tranDataOfService.ReturnCode == ((int)ReturnCodeEnum.Success).ToString())
                {
                    ////Select one record of highest expiry time
                    int? dbExpiredMinutes = tranData.MeterExpiredMinutes.ToNullableInt();
                    int? caleExpiredMinutes = tranDataOfService.MeterExpiredMinutes.ToNullableInt();
                    tranData = Nullable.Compare(dbExpiredMinutes, caleExpiredMinutes) > 0 ? tranData : tranDataOfService;
                }
            }

            else if (tranData.ReturnCode == ((int)ReturnCodeEnum.MoreRecordsExists).ToString())
            {
                // DB never give more record. hence ignore
            }

            return tranData;
        }
        public static TransactionTransform GetDetroitTransformDataV2(string EnforcementKey, string filePath, RequestResponseEntity logData, CustomerTime customerCurrentTime)
        {
            TransactionTransform tranData = new TransactionTransform();
            tranData.ReturnCode = ((int)ReturnCodeEnum.InvalidInputParameters).ToString();
            TimeZoneInfo TZ = null;
            TZ = TimeZoneInfo.FindSystemTimeZoneById("Eastern Standard Time");
            //int? customerID = null;
            //string MeterName = null;
            //string parkingSpaceID;
            //string PlateNumber = null;
            //string stateName = null;
            //////////////////////////////////////
            KeyParameter param = PrepareKeyParameters.PrepareDetroit(EnforcementKey);
            if (param.ReturnCode == ((int)ReturnCodeEnum.InvalidInputParameters).ToString())
            {
                return tranData;
            }

            /////////////////////////////////////
            #region "DB Check"
            try
            {
                Transaction data = TransactionDataValidatioin.GetDataDetroit(param.CustomerId.Value, param.MeterName, param.parkingSpaceID, param.PlateNumber, param.StateName, EnforcementKey, filePath, logData);


                if (data.TimeZoneId.HasValue)
                    TZ = Utility.GetTimeZoneInfo(data.TimeZoneId.Value, filePath, param.CustomerId.Value.ToString());

                //For South Miami : PayStationId. Others it is MeterID
                tranData.MeterName = (data.MeterName == null) ? string.Empty : data.MeterName;
                tranData.SpaceNo = data.ParkingSpaceID.ToString();

                tranData.MeterStreet = (data.MeterStreet == null) ? string.Empty : data.MeterStreet;

                DateTime _MeterRTC = data.PresentMeterTime.HasValue ? data.PresentMeterTime.Value : DateTime.Now;
                tranData.MeterRTC = String.Format("{0:yyyyMMdd_THHmmssZ}", Utility.findGMTTime(_MeterRTC, TZ));

                DateTime _SensorEventTime = data.SensorEventTime.HasValue ? data.SensorEventTime.Value : DateTime.Now;
                tranData.MeterLastUpdateTime = String.Format("{0:yyyyMMdd_THHmmssZ}", Utility.findGMTTime(_SensorEventTime, TZ));

                tranData.SensorEventTime = String.Format("{0:yyyyMMdd_THHmmssZ}", Utility.findGMTTime(_SensorEventTime, TZ));

                DateTime _endTime = data.ExpiryTime.HasValue ? data.ExpiryTime.Value : DateTime.Now;
                tranData.ExpiredTime = String.Format("{0:yyyyMMdd_THHmmssZ}", Utility.findGMTTime(_endTime, TZ));

                tranData.MeterExpiredMinutes = data.ExpiredMinutes.HasValue ? data.ExpiredMinutes.Value.ToString() : "0";
                tranData.METERLOC_BLOCKNUMBER = (data.Block == null) ? string.Empty : data.Block;
                tranData.METERLOC_STREETDIRECTION = (data.Direction == null) ? string.Empty : data.Direction;
                tranData.METERLOC_STREETNAME = (data.MeterStreet == null) ? string.Empty : data.MeterStreet;
                tranData.METERLOC_STREETTYPE = (data.StreetType == null) ? string.Empty : data.StreetType;
                tranData.METERLOC_CROSSSTREET1 = (data.CrossStreet1 == null) ? string.Empty : data.CrossStreet1;
                tranData.METERLOC_CROSSSTREET2 = (data.CrossStreet2 == null) ? string.Empty : data.CrossStreet2;
                tranData.METERLOC_ENFORCEMENTHOURSDESC = (data.EnfHourDesc == null) ? string.Empty : data.EnfHourDesc;
                tranData.LicensePlate = (data.PlateNumber == null) ? string.Empty : data.PlateNumber;
                tranData.State = (data.LPLocation == null) ? string.Empty : data.LPLocation;
                tranData.Make = (data.VMake == null) ? string.Empty : data.VMake;
                tranData.Model = (data.VModel == null) ? string.Empty : data.VModel;
                tranData.IsOccupied = Utility.getStatus((data.SpaceStatus == null) ? string.Empty : data.SpaceStatus).ToString();
                tranData.EnforcementKey = (data.EnforcementKey == null) ? string.Empty : data.EnforcementKey;
                tranData.MeterStatus = (data.MeterStatus == null) ? null : data.MeterStatus;

                tranData.METERLOC_METERTYPE = (data.METERLOC_METERTYPE == null) ? null : data.METERLOC_METERTYPE;
                tranData.ZoneName = (data.ZoneName == null) ? null : data.ZoneName;

                tranData.MeterID = data.PayStationID.HasValue ? data.PayStationID.Value.ToString() : "0";
                tranData.ReturnCode = data.ReturnCode;

                logData.Response = new ResponseEntity() { CustomerId = param.CustomerId.Value.ToString(), MeterId = tranData.MeterName, SpaceId = tranData.SpaceNo, CityCurrentTime = data.PresentMeterTime.HasValue ? data.PresentMeterTime.Value : DateTime.Now };
            }
            catch (Exception ex)
            {
                //var appPath = System.Web.Hosting.HostingEnvironment.ApplicationPhysicalPath;
                //string errorPath = Path.Combine(appPath, "GetTransformData", "Timezones");
                //ExceptionLogging.SendErrorToText(ex, "GetCitationData", errorPath);
                //tranData.ReturnCode = ((int)ReturnCodeEnum.Error).ToString();

                tranData.ReturnCode = ((int)ReturnCodeEnum.Error).ToString();
            }
            #endregion

            #region "Cale Service Call"
            /////////////////////////////////////////////////
            TransactionTransform tranDataOfService = new TransactionTransform();
            try
            {
                //////////////////////////////////////////
                logData.Request.CustomerId = param.CustomerId.Value.ToString();
                logData.Request.SpaceId = param.parkingSpaceID;
                logData.Request.MeterId = param.MeterName;
                //////////////////////////////////////////////////
                DateTime? currentCustTime = customerCurrentTime.CurrentTime;
                Cale.CaleCustomerElement customer = Cale.CaleCustomerRetriever.GetCustomer(param.CustomerId.Value);
                Cale.CaleGenericFactory factory = new Cale.CaleGenericFactory(customer, param.PlateNumber, null);
                var caleResLog = new VendorResponseEntity();
                logData.VendorData.Add(VendorNames.Cale, caleResLog);
                if (customer.EnforcementType == EnumEnforcementType.PayByPlate.ToString())
                    tranDataOfService = factory.GetRefreshPlateData(ref caleResLog);
                tranDataOfService.EnforcementKey = EnforcementKey;

                ///////////////////////////////////////////////////
                logData.Response = new ResponseEntity() { CustomerId = param.CustomerId.Value.ToString(), MeterId = tranData.MeterName, SpaceId = tranData.SpaceNo, CityCurrentTime = currentCustTime.HasValue ? currentCustTime.Value : DateTime.Now };
            }
            catch (Exception ex)
            {
                logData.Response = new ResponseEntity() { CustomerId = param.CustomerId.Value.ToString(), MeterId = tranData.MeterName, SpaceId = tranData.SpaceNo, CityCurrentTime = customerCurrentTime.CurrentTime.HasValue ? customerCurrentTime.CurrentTime.Value : DateTime.Now };
                //throw ex;
                tranData.ReturnCode = ((int)ReturnCodeEnum.Error).ToString();
            }
            finally
            {

            }
            #endregion
            //Reconcile data
            if ((tranData.ReturnCode == ((int)ReturnCodeEnum.NoRecordExists).ToString()) || (tranData.ReturnCode == ((int)ReturnCodeEnum.Error).ToString()))
            {
                tranData = tranDataOfService;
            }
            else if (tranData.ReturnCode == ((int)ReturnCodeEnum.Success).ToString())
            {
                if (tranDataOfService.ReturnCode == ((int)ReturnCodeEnum.Success).ToString())
                {
                    ////Select one record of highest expiry time
                    int? dbExpiredMinutes = tranData.MeterExpiredMinutes.ToNullableInt();
                    int? caleExpiredMinutes = tranDataOfService.MeterExpiredMinutes.ToNullableInt();
                    tranData = Nullable.Compare(dbExpiredMinutes, caleExpiredMinutes) > 0 ? tranData : tranDataOfService;
                }
            }

            else if (tranData.ReturnCode == ((int)ReturnCodeEnum.MoreRecordsExists).ToString())
            {
                // DB never give more record. hence ignore
            }

            return tranData;
        }
        public static TransactionTransform GetDetroitTransformDataV3(string EnforcementKey, string filePath, RequestResponseEntity logData, CustomerTime customerCurrentTime)
        {
            TransactionTransform tranData = new TransactionTransform();
            tranData.ReturnCode = ((int)ReturnCodeEnum.InvalidInputParameters).ToString();
            TimeZoneInfo TZ = null;
            TZ = TimeZoneInfo.FindSystemTimeZoneById("Eastern Standard Time");
            //int? customerID = null;
            //string MeterName = null;
            //string parkingSpaceID;
            //string PlateNumber = null;
            //string stateName = null;

            //logData.Response = new ResponseEntity() { CustomerId = "TEST" , MeterId = "METERPAR", SpaceId = "SQ" };
            //RequestResponseLog.ReqResLogV2(logData, logFilePath);
            //////////////////////////////////////
            KeyParameter param = PrepareKeyParameters.PrepareDetroit(EnforcementKey);
            if (param.ReturnCode == ((int)ReturnCodeEnum.InvalidInputParameters).ToString())
            {
                //logData.Response = new ResponseEntity() { CustomerId = param.CustomerId.Value.ToString() + "retZone=" + tranData.ZoneName, MeterId = tranData.MeterName, SpaceId = tranData.SpaceNo };

                return tranData;
            }

            /////////////////////////////////////
            #region "DB Check"
            try
            {


                Transaction data = TransactionDataValidatioin.GetDataDetroitZone(param.CustomerId.Value, param.MeterName, param.parkingSpaceID, param.PlateNumber, param.ZoneID, param.StateName, EnforcementKey, filePath, logData);


                if (data.TimeZoneId.HasValue)
                    TZ = Utility.GetTimeZoneInfo(data.TimeZoneId.Value, filePath, param.CustomerId.Value.ToString());

                //For South Miami : PayStationId. Others it is MeterID;
                tranData.MeterName = (data.MeterName == null) ? string.Empty : data.MeterName;
                tranData.SpaceNo = data.ParkingSpaceID.ToString();

                tranData.MeterStreet = (data.MeterStreet == null) ? string.Empty : data.MeterStreet;

                DateTime _MeterRTC = data.PresentMeterTime.HasValue ? data.PresentMeterTime.Value : DateTime.Now;
                tranData.MeterRTC = String.Format("{0:yyyyMMdd_THHmmssZ}", Utility.findGMTTime(_MeterRTC, TZ));

                DateTime _SensorEventTime = data.SensorEventTime.HasValue ? data.SensorEventTime.Value : DateTime.Now;
                tranData.MeterLastUpdateTime = String.Format("{0:yyyyMMdd_THHmmssZ}", Utility.findGMTTime(_SensorEventTime, TZ));

                tranData.SensorEventTime = String.Format("{0:yyyyMMdd_THHmmssZ}", Utility.findGMTTime(_SensorEventTime, TZ));

                DateTime _endTime = data.ExpiryTime.HasValue ? data.ExpiryTime.Value : DateTime.Now;
                tranData.ExpiredTime = String.Format("{0:yyyyMMdd_THHmmssZ}", Utility.findGMTTime(_endTime, TZ));

                tranData.MeterExpiredMinutes = data.ExpiredMinutes.HasValue ? data.ExpiredMinutes.Value.ToString() : "0";
                tranData.METERLOC_BLOCKNUMBER = (data.Block == null) ? string.Empty : data.Block;
                tranData.METERLOC_STREETDIRECTION = (data.Direction == null) ? string.Empty : data.Direction;
                tranData.METERLOC_STREETNAME = (data.MeterStreet == null) ? string.Empty : data.MeterStreet;
                tranData.METERLOC_STREETTYPE = (data.StreetType == null) ? string.Empty : data.StreetType;
                tranData.METERLOC_CROSSSTREET1 = (data.CrossStreet1 == null) ? string.Empty : data.CrossStreet1;
                tranData.METERLOC_CROSSSTREET2 = (data.CrossStreet2 == null) ? string.Empty : data.CrossStreet2;
                tranData.METERLOC_ENFORCEMENTHOURSDESC = (data.EnfHourDesc == null) ? string.Empty : data.EnfHourDesc;
                tranData.LicensePlate = (data.PlateNumber == null) ? string.Empty : data.PlateNumber;
                tranData.State = (data.LPLocation == null) ? string.Empty : data.LPLocation;
                tranData.Make = (data.VMake == null) ? string.Empty : data.VMake;
                tranData.Model = (data.VModel == null) ? string.Empty : data.VModel;
                tranData.IsOccupied = Utility.getStatus((data.SpaceStatus == null) ? string.Empty : data.SpaceStatus).ToString();
                tranData.EnforcementKey = (data.EnforcementKey == null) ? string.Empty : data.EnforcementKey;
                tranData.MeterStatus = (data.MeterStatus == null) ? null : data.MeterStatus;

                tranData.METERLOC_METERTYPE = (data.METERLOC_METERTYPE == null) ? null : data.METERLOC_METERTYPE;
                tranData.ZoneName = (data.ZoneName == null) ? null : data.ZoneName;

                tranData.MeterID = data.PayStationID.HasValue ? data.PayStationID.Value.ToString() : "0";
                tranData.ReturnCode = data.ReturnCode;

                //logData.Response = new ResponseEntity() { CustomerId = param.CustomerId.Value.ToString() + "Zone=" + tranData.ZoneName, MeterId = tranData.MeterName, SpaceId = tranData.SpaceNo, CityCurrentTime = data.PresentMeterTime.HasValue ? data.PresentMeterTime.Value : DateTime.Now };
            }
            catch (Exception ex)
            {
                //var appPath = System.Web.Hosting.HostingEnvironment.ApplicationPhysicalPath;
                //string errorPath = Path.Combine(appPath, "GetTransformData", "Timezones");
                //ExceptionLogging.SendErrorToText(ex, "GetCitationData", errorPath);
                //tranData.ReturnCode = ((int)ReturnCodeEnum.Error).ToString();

                tranData.ReturnCode = ((int)ReturnCodeEnum.Error).ToString();
            }
            #endregion

            #region "Cale Service Call"
            /////////////////////////////////////////////////
            TransactionTransform tranDataOfService = new TransactionTransform();
            try
            {
                //////////////////////////////////////////
                logData.Request.CustomerId = param.CustomerId.Value.ToString();
                logData.Request.SpaceId = param.parkingSpaceID;
                logData.Request.MeterId = param.MeterName;
                //////////////////////////////////////////////////
                DateTime? currentCustTime = customerCurrentTime.CurrentTime;
                Cale.CaleCustomerElement customer = Cale.CaleCustomerRetriever.GetCustomer(param.CustomerId.Value);
                Cale.CaleGenericFactory factory = new Cale.CaleGenericFactory(customer, param.PlateNumber, null);
                var caleResLog = new VendorResponseEntity();
                logData.VendorData.Add(VendorNames.Cale, caleResLog);
                if (customer.EnforcementType == EnumEnforcementType.PayByPlate.ToString())
                    tranDataOfService = factory.GetRefreshPlateData(ref caleResLog);
                tranDataOfService.EnforcementKey = EnforcementKey;

                ///////////////////////////////////////////////////
                //logData.Response = new ResponseEntity() { CustomerId = param.CustomerId.Value.ToString(), MeterId = tranData.MeterName, SpaceId = tranData.SpaceNo, CityCurrentTime = currentCustTime.HasValue ? currentCustTime.Value : DateTime.Now };
            }
            catch (Exception ex)
            {
                //logData.Response = new ResponseEntity() { CustomerId = param.CustomerId.Value.ToString(), MeterId = tranData.MeterName, SpaceId = tranData.SpaceNo, CityCurrentTime = customerCurrentTime.CurrentTime.HasValue ? customerCurrentTime.CurrentTime.Value : DateTime.Now };
                //throw ex;
                tranData.ReturnCode = ((int)ReturnCodeEnum.Error).ToString();
            }
            finally
            {

            }
            #endregion

            //Reconcile data
            if (tranData.ReturnCode == ((int)ReturnCodeEnum.NoRecordExists).ToString())
            {
                tranData = new TransactionTransform();
                tranData.ReturnCode = ((int)ReturnCodeEnum.Success).ToString();
                tranData.LicensePlate = param.PlateNumber;
                tranData.MeterExpiredMinutes = "-1";
                tranData.EnforcementKey = EnforcementKey;
            }
            else if ((tranData.ReturnCode == ((int)ReturnCodeEnum.Error).ToString()))
            {
                tranData = tranDataOfService;
            }
            else if (tranData.ReturnCode == ((int)ReturnCodeEnum.Success).ToString())
            {
                if (tranDataOfService.ReturnCode == ((int)ReturnCodeEnum.Success).ToString())
                {
                    ////Select one record of highest expiry time
                    int? dbExpiredMinutes = tranData.MeterExpiredMinutes.ToNullableInt();
                    int? caleExpiredMinutes = tranDataOfService.MeterExpiredMinutes.ToNullableInt();
                    tranData = Nullable.Compare(dbExpiredMinutes, caleExpiredMinutes) > 0 ? tranData : tranDataOfService;

                }
            }

            else if (tranData.ReturnCode == ((int)ReturnCodeEnum.MoreRecordsExists).ToString())
            {
                // DB never give more record. hence ignore
            }
            //tranData.ZoneName = "Testpar";
            logData.Response = new ResponseEntity() { CustomerId = param.CustomerId.Value.ToString(), MeterId = tranData.MeterName, SpaceId = tranData.SpaceNo, CityCurrentTime = customerCurrentTime.CurrentTime.HasValue ? customerCurrentTime.CurrentTime.Value : DateTime.Now };

            //
            return tranData;
        }
        private static Transaction GetDataDetroit(int customerID, string MeterName, string parkingSpaceID, string PlateNumber, string statename, string EnforcementKey, string filePath, RequestResponseEntity logData)
        {
            string ZoneID = null;
            string SpaceStatus = null;
            Transaction data = new Transaction();
            try
            {
                List<Transaction> transactionList = new List<Transaction>();

                logData.Request.CustomerId = customerID.ToString();
                logData.Request.SpaceId = parkingSpaceID;
                logData.Request.MeterId = MeterName;

                transactionList = DataAccess.GetDetroitCitationData(customerID, null, MeterName, parkingSpaceID, PlateNumber, statename, null);

                if (transactionList.Count > 0)
                {
                    //data = transactionList[0];
                    transactionList = transactionList.OrderByDescending(t => t.ExpiredMinutes != null ? t.ExpiredMinutes.Value : Int32.MinValue).ToList();
                    data = transactionList[0];
                }

                if (transactionList.Count > 1)
                {
                    //data.ReturnCode = ((int)ReturnCodeEnum.MoreRecordsExists).ToString();
                    data.ReturnCode = ((int)ReturnCodeEnum.Success).ToString();
                }
                else if (transactionList.Count == 0)
                {
                    data.ReturnCode = ((int)ReturnCodeEnum.NoRecordExists).ToString();
                }
                else
                {
                    data.ReturnCode = ((int)ReturnCodeEnum.Success).ToString();
                }
            }
            catch (Exception ex)
            {
                var appPath = System.Web.Hosting.HostingEnvironment.ApplicationPhysicalPath;
                string errorPath = Path.Combine(appPath, "GetData", "Timezones");
                ExceptionLogging.SendErrorToText(ex, "GetCitationData", errorPath);
                data.ReturnCode = ((int)ReturnCodeEnum.Error).ToString();
                throw;
            }
            return data;
        }
        private static Transaction GetDataDetroitZone(int customerID, string MeterName, string parkingSpaceID, string PlateNumber, string ZoneID, string statename, string EnforcementKey, string filePath, RequestResponseEntity logData)
        {//for detroit
            //string ZoneID = null;
            string SpaceStatus = null;
            Transaction data = new Transaction();
            try
            {
                List<Transaction> transactionList = new List<Transaction>();

                logData.Request.CustomerId = customerID.ToString();
                logData.Request.SpaceId = parkingSpaceID;
                logData.Request.MeterId = MeterName;
                logData.Request.StallId = ZoneID;

                //transactionList = DataAccess.GetDetroitCitationData(customerID, null, MeterName, parkingSpaceID, PlateNumber, statename, null);
                //N. Parvathy on 09 April 2021 Zone id was passed as null now sending it
                transactionList = DataAccess.GetDetroitCitationData(customerID, ZoneID, MeterName, parkingSpaceID, PlateNumber, statename, null);

                if (transactionList.Count > 0)
                {
                    //data = transactionList[0];
                    transactionList = transactionList.OrderByDescending(t => t.ExpiredMinutes != null ? t.ExpiredMinutes.Value : Int32.MinValue).ToList();
                    data = transactionList[0];
                }

                if (transactionList.Count > 1)
                {
                    //data.ReturnCode = ((int)ReturnCodeEnum.MoreRecordsExists).ToString();
                    data.ReturnCode = ((int)ReturnCodeEnum.Success).ToString();
                }
                else if (transactionList.Count == 0)
                {
                    data.ReturnCode = ((int)ReturnCodeEnum.NoRecordExists).ToString();
                }
                else
                {
                    data.ReturnCode = ((int)ReturnCodeEnum.Success).ToString();
                }
            }
            catch (Exception ex)
            {
                var appPath = System.Web.Hosting.HostingEnvironment.ApplicationPhysicalPath;
                string errorPath = Path.Combine(appPath, "GetData", "Timezones");
                ExceptionLogging.SendErrorToText(ex, "GetCitationData", errorPath);
                data.ReturnCode = ((int)ReturnCodeEnum.Error).ToString();
                throw;
            }
            return data;
        }
        #endregion

        #region SiouxCity
        public static TransactionTransform GetSiouxCityTransformData(string EnforcementKey, string filePath, RequestResponseEntity logData)
        {
            TransactionTransform tranData = new TransactionTransform();
            tranData.ReturnCode = ((int)ReturnCodeEnum.InvalidInputParameters).ToString();
            TimeZoneInfo TZ = null;
            TZ = TimeZoneInfo.FindSystemTimeZoneById("Eastern Standard Time");
            //int? customerID = null;
            //string MeterName = null;
            //string parkingSpaceID;
            //string PlateNumber = null;
            //string stateName = null;
            //////////////////////////////////////
            KeyParameter param = PrepareKeyParameters.PrepareSiouxCity(EnforcementKey);
            if (param.ReturnCode == ((int)ReturnCodeEnum.InvalidInputParameters).ToString())
            {
                return tranData;
            }

            /////////////////////////////////////
            try
            {
                Transaction data = TransactionDataValidatioin.GetDataSiouxCity(param.CustomerId.Value, param.MeterName, param.parkingSpaceID, param.PlateNumber, param.StateName, EnforcementKey, filePath, logData);


                if (data.TimeZoneId.HasValue)
                    TZ = Utility.GetTimeZoneInfo(data.TimeZoneId.Value, filePath, param.CustomerId.Value.ToString());

                //For South Miami : PayStationId. Others it is MeterID
                tranData.MeterName = (data.MeterName == null) ? string.Empty : data.MeterName;
                tranData.SpaceNo = data.ParkingSpaceID.ToString();

                tranData.MeterStreet = (data.MeterStreet == null) ? string.Empty : data.MeterStreet;

                DateTime _MeterRTC = data.PresentMeterTime.HasValue ? data.PresentMeterTime.Value : DateTime.Now;
                tranData.MeterRTC = String.Format("{0:yyyyMMdd_THHmmssZ}", Utility.findGMTTime(_MeterRTC, TZ));

                DateTime _SensorEventTime = data.SensorEventTime.HasValue ? data.SensorEventTime.Value : DateTime.Now;
                tranData.MeterLastUpdateTime = String.Format("{0:yyyyMMdd_THHmmssZ}", Utility.findGMTTime(_SensorEventTime, TZ));

                tranData.SensorEventTime = String.Format("{0:yyyyMMdd_THHmmssZ}", Utility.findGMTTime(_SensorEventTime, TZ));

                DateTime _endTime = data.ExpiryTime.HasValue ? data.ExpiryTime.Value : DateTime.Now;
                tranData.ExpiredTime = String.Format("{0:yyyyMMdd_THHmmssZ}", Utility.findGMTTime(_endTime, TZ));

                tranData.MeterExpiredMinutes = data.ExpiredMinutes.HasValue ? data.ExpiredMinutes.Value.ToString() : "0";
                tranData.METERLOC_BLOCKNUMBER = (data.Block == null) ? string.Empty : data.Block;
                tranData.METERLOC_STREETDIRECTION = (data.Direction == null) ? string.Empty : data.Direction;
                tranData.METERLOC_STREETNAME = (data.MeterStreet == null) ? string.Empty : data.MeterStreet;
                tranData.METERLOC_STREETTYPE = (data.StreetType == null) ? string.Empty : data.StreetType;
                tranData.METERLOC_CROSSSTREET1 = (data.CrossStreet1 == null) ? string.Empty : data.CrossStreet1;
                tranData.METERLOC_CROSSSTREET2 = (data.CrossStreet2 == null) ? string.Empty : data.CrossStreet2;
                tranData.METERLOC_ENFORCEMENTHOURSDESC = (data.EnfHourDesc == null) ? string.Empty : data.EnfHourDesc;
                tranData.LicensePlate = (data.PlateNumber == null) ? string.Empty : data.PlateNumber;
                tranData.State = (data.LPLocation == null) ? string.Empty : data.LPLocation;
                tranData.Make = (data.VMake == null) ? string.Empty : data.VMake;
                tranData.Model = (data.VModel == null) ? string.Empty : data.VModel;
                tranData.IsOccupied = Utility.getStatus((data.SpaceStatus == null) ? string.Empty : data.SpaceStatus).ToString();
                tranData.EnforcementKey = (data.EnforcementKey == null) ? string.Empty : data.EnforcementKey;
                tranData.MeterStatus = (data.MeterStatus == null) ? null : data.MeterStatus;

                tranData.METERLOC_METERTYPE = (data.METERLOC_METERTYPE == null) ? null : data.METERLOC_METERTYPE;
                tranData.ZoneName = (data.ZoneName == null) ? null : data.ZoneName;

                tranData.MeterID = data.PayStationID.HasValue ? data.PayStationID.Value.ToString() : "0";
                tranData.ReturnCode = data.ReturnCode;

                logData.Response = new ResponseEntity() { CustomerId = param.CustomerId.Value.ToString(), MeterId = tranData.MeterName, SpaceId = tranData.SpaceNo, CityCurrentTime = data.PresentMeterTime.HasValue ? data.PresentMeterTime.Value : DateTime.Now };
            }
            catch (Exception ex)
            {
                var appPath = System.Web.Hosting.HostingEnvironment.ApplicationPhysicalPath;
                string errorPath = Path.Combine(appPath, "GetTransformData", "Timezones");
                ExceptionLogging.SendErrorToText(ex, "GetCitationData", errorPath);
                tranData.ReturnCode = ((int)ReturnCodeEnum.Error).ToString();

            }
            return tranData;
        }
        public static TransactionTransform GetSiouxCityTransformDataV2(string EnforcementKey, string filePath, RequestResponseEntity logData, CustomerTime customerCurrentTime)
        {
            TransactionTransform tranData = new TransactionTransform();
            tranData.ReturnCode = ((int)ReturnCodeEnum.InvalidInputParameters).ToString();
            TimeZoneInfo TZ = null;
            TZ = TimeZoneInfo.FindSystemTimeZoneById("Eastern Standard Time");
            //int? customerID = null;
            //string MeterName = null;
            //string parkingSpaceID;
            //string PlateNumber = null;
            //string stateName = null;
            //////////////////////////////////////
            KeyParameter param = PrepareKeyParameters.PrepareSiouxCity(EnforcementKey);
            if (param.ReturnCode == ((int)ReturnCodeEnum.InvalidInputParameters).ToString())
            {
                return tranData;
            }
            #region "DB Check"
            /////////////////////////////////////
            try
            {
                Transaction data = TransactionDataValidatioin.GetDataSiouxCity(param.CustomerId.Value, param.MeterName, param.parkingSpaceID, param.PlateNumber, param.StateName, EnforcementKey, filePath, logData);


                if (data.TimeZoneId.HasValue)
                    TZ = Utility.GetTimeZoneInfo(data.TimeZoneId.Value, filePath, param.CustomerId.Value.ToString());

                //For South Miami : PayStationId. Others it is MeterID
                tranData.MeterName = (data.MeterName == null) ? string.Empty : data.MeterName;
                tranData.SpaceNo = data.ParkingSpaceID.ToString();

                tranData.MeterStreet = (data.MeterStreet == null) ? string.Empty : data.MeterStreet;

                DateTime _MeterRTC = data.PresentMeterTime.HasValue ? data.PresentMeterTime.Value : DateTime.Now;
                tranData.MeterRTC = String.Format("{0:yyyyMMdd_THHmmssZ}", Utility.findGMTTime(_MeterRTC, TZ));

                DateTime _SensorEventTime = data.SensorEventTime.HasValue ? data.SensorEventTime.Value : DateTime.Now;
                tranData.MeterLastUpdateTime = String.Format("{0:yyyyMMdd_THHmmssZ}", Utility.findGMTTime(_SensorEventTime, TZ));

                tranData.SensorEventTime = String.Format("{0:yyyyMMdd_THHmmssZ}", Utility.findGMTTime(_SensorEventTime, TZ));

                DateTime _endTime = data.ExpiryTime.HasValue ? data.ExpiryTime.Value : DateTime.Now;
                tranData.ExpiredTime = String.Format("{0:yyyyMMdd_THHmmssZ}", Utility.findGMTTime(_endTime, TZ));

                tranData.MeterExpiredMinutes = data.ExpiredMinutes.HasValue ? data.ExpiredMinutes.Value.ToString() : "0";
                tranData.METERLOC_BLOCKNUMBER = (data.Block == null) ? string.Empty : data.Block;
                tranData.METERLOC_STREETDIRECTION = (data.Direction == null) ? string.Empty : data.Direction;
                tranData.METERLOC_STREETNAME = (data.MeterStreet == null) ? string.Empty : data.MeterStreet;
                tranData.METERLOC_STREETTYPE = (data.StreetType == null) ? string.Empty : data.StreetType;
                tranData.METERLOC_CROSSSTREET1 = (data.CrossStreet1 == null) ? string.Empty : data.CrossStreet1;
                tranData.METERLOC_CROSSSTREET2 = (data.CrossStreet2 == null) ? string.Empty : data.CrossStreet2;
                tranData.METERLOC_ENFORCEMENTHOURSDESC = (data.EnfHourDesc == null) ? string.Empty : data.EnfHourDesc;
                tranData.LicensePlate = (data.PlateNumber == null) ? string.Empty : data.PlateNumber;
                tranData.State = (data.LPLocation == null) ? string.Empty : data.LPLocation;
                tranData.Make = (data.VMake == null) ? string.Empty : data.VMake;
                tranData.Model = (data.VModel == null) ? string.Empty : data.VModel;
                tranData.IsOccupied = Utility.getStatus((data.SpaceStatus == null) ? string.Empty : data.SpaceStatus).ToString();
                tranData.EnforcementKey = (data.EnforcementKey == null) ? string.Empty : data.EnforcementKey;
                tranData.MeterStatus = (data.MeterStatus == null) ? null : data.MeterStatus;

                tranData.METERLOC_METERTYPE = (data.METERLOC_METERTYPE == null) ? null : data.METERLOC_METERTYPE;
                tranData.ZoneName = (data.ZoneName == null) ? null : data.ZoneName;

                tranData.MeterID = data.PayStationID.HasValue ? data.PayStationID.Value.ToString() : "0";
                tranData.ReturnCode = data.ReturnCode;

                logData.Response = new ResponseEntity() { CustomerId = param.CustomerId.Value.ToString(), MeterId = tranData.MeterName, SpaceId = tranData.SpaceNo, CityCurrentTime = data.PresentMeterTime.HasValue ? data.PresentMeterTime.Value : DateTime.Now };


            }
            catch (Exception ex)
            {
                //var appPath = System.Web.Hosting.HostingEnvironment.ApplicationPhysicalPath;
                //string errorPath = Path.Combine(appPath, "GetTransformData", "Timezones");
                //ExceptionLogging.SendErrorToText(ex, "GetCitationData", errorPath);
                //tranData.ReturnCode = ((int)ReturnCodeEnum.Error).ToString();
                tranData.ReturnCode = ((int)ReturnCodeEnum.Error).ToString();
            }
            #endregion
            #region "Parkmobile Service Call"
            /////////////////////////////////////////////////
            TransactionTransform tranDataOfServiceParkMobile = new TransactionTransform();
            try
            {
                //////////////////////////////////////////
                logData.Request.CustomerId = param.CustomerId.Value.ToString();
                logData.Request.SpaceId = param.parkingSpaceID;
                logData.Request.MeterId = param.MeterName;
                //////////////////////////////////////////////////
                DateTime? currentCustTime = customerCurrentTime.CurrentTime;
                PMCustomerElement customer = ParkmobileCustomerRetriever.GetCustomer(param.CustomerId.Value);
                ParkMobileGenericFactory factory = new ParkMobileGenericFactory(customer, param.PlateNumber);
                tranDataOfServiceParkMobile = factory.GetRefreshSpaceData(ParkingMobileDataType.Plate);
                tranDataOfServiceParkMobile.EnforcementKey = EnforcementKey;

                ///////////////////////////////////////////////////
                logData.Response = new ResponseEntity() { CustomerId = param.CustomerId.Value.ToString(), MeterId = tranData.MeterName, SpaceId = tranData.SpaceNo, CityCurrentTime = currentCustTime.HasValue ? currentCustTime.Value : DateTime.Now };
            }
            catch (Exception ex)
            {
                logData.Response = new ResponseEntity() { CustomerId = param.CustomerId.Value.ToString(), MeterId = tranData.MeterName, SpaceId = tranData.SpaceNo, CityCurrentTime = customerCurrentTime.CurrentTime.HasValue ? customerCurrentTime.CurrentTime.Value : DateTime.Now };
                //throw ex;
                tranDataOfServiceParkMobile.ReturnCode = ((int)ReturnCodeEnum.Error).ToString();
            }
            finally
            {

            }
            #endregion
            //////////////////  //Reconcile data

            List<TransactionTransform> trans = new List<TransactionTransform>();
            trans.Add(tranDataOfServiceParkMobile); // Parkmobile
            trans.Add(tranData); // Database

            var rightValue = trans.Where(t => t.ReturnCode == ((int)ReturnCodeEnum.Success).ToString()).OrderByDescending(t => t.MeterExpiredMinutes.ToNullableInt()).FirstOrDefault<TransactionTransform>();
            if (rightValue == default(TransactionTransform))
            {
                //Do nothing.just return database  object.
            }
            else
            {
                tranData = rightValue;
            }

            return tranData;
        }
        public static TransactionTransform GetSiouxCityTransformDataV3(string EnforcementKey, string filePath, RequestResponseEntity logData, CustomerTime customerCurrentTime)
        {
            TransactionTransform tranData = new TransactionTransform();
            tranData.ReturnCode = ((int)ReturnCodeEnum.InvalidInputParameters).ToString();
            TimeZoneInfo TZ = null;
            TZ = TimeZoneInfo.FindSystemTimeZoneById("Eastern Standard Time");
            //int? customerID = null;
            //string MeterName = null;
            //string parkingSpaceID;
            //string PlateNumber = null;
            //string stateName = null;
            //////////////////////////////////////
            KeyParameter param = PrepareKeyParameters.PrepareSiouxCity(EnforcementKey);
            if (param.ReturnCode == ((int)ReturnCodeEnum.InvalidInputParameters).ToString())
            {
                return tranData;
            }
            #region "DB Check"
            /////////////////////////////////////
            try
            {
                Transaction data = TransactionDataValidatioin.GetDataSiouxCity(param.CustomerId.Value, param.MeterName, param.parkingSpaceID, param.PlateNumber, param.StateName, EnforcementKey, filePath, logData);


                if (data.TimeZoneId.HasValue)
                    TZ = Utility.GetTimeZoneInfo(data.TimeZoneId.Value, filePath, param.CustomerId.Value.ToString());

                //For South Miami : PayStationId. Others it is MeterID
                tranData.MeterName = (data.MeterName == null) ? string.Empty : data.MeterName;
                tranData.SpaceNo = data.ParkingSpaceID.ToString();

                tranData.MeterStreet = (data.MeterStreet == null) ? string.Empty : data.MeterStreet;

                DateTime _MeterRTC = data.PresentMeterTime.HasValue ? data.PresentMeterTime.Value : DateTime.Now;
                tranData.MeterRTC = String.Format("{0:yyyyMMdd_THHmmssZ}", Utility.findGMTTime(_MeterRTC, TZ));

                DateTime _SensorEventTime = data.SensorEventTime.HasValue ? data.SensorEventTime.Value : DateTime.Now;
                tranData.MeterLastUpdateTime = String.Format("{0:yyyyMMdd_THHmmssZ}", Utility.findGMTTime(_SensorEventTime, TZ));

                tranData.SensorEventTime = String.Format("{0:yyyyMMdd_THHmmssZ}", Utility.findGMTTime(_SensorEventTime, TZ));

                DateTime _endTime = data.ExpiryTime.HasValue ? data.ExpiryTime.Value : DateTime.Now;
                tranData.ExpiredTime = String.Format("{0:yyyyMMdd_THHmmssZ}", Utility.findGMTTime(_endTime, TZ));

                tranData.MeterExpiredMinutes = data.ExpiredMinutes.HasValue ? data.ExpiredMinutes.Value.ToString() : "0";
                tranData.METERLOC_BLOCKNUMBER = (data.Block == null) ? string.Empty : data.Block;
                tranData.METERLOC_STREETDIRECTION = (data.Direction == null) ? string.Empty : data.Direction;
                tranData.METERLOC_STREETNAME = (data.MeterStreet == null) ? string.Empty : data.MeterStreet;
                tranData.METERLOC_STREETTYPE = (data.StreetType == null) ? string.Empty : data.StreetType;
                tranData.METERLOC_CROSSSTREET1 = (data.CrossStreet1 == null) ? string.Empty : data.CrossStreet1;
                tranData.METERLOC_CROSSSTREET2 = (data.CrossStreet2 == null) ? string.Empty : data.CrossStreet2;
                tranData.METERLOC_ENFORCEMENTHOURSDESC = (data.EnfHourDesc == null) ? string.Empty : data.EnfHourDesc;
                tranData.LicensePlate = (data.PlateNumber == null) ? string.Empty : data.PlateNumber;
                tranData.State = (data.LPLocation == null) ? string.Empty : data.LPLocation;
                tranData.Make = (data.VMake == null) ? string.Empty : data.VMake;
                tranData.Model = (data.VModel == null) ? string.Empty : data.VModel;
                tranData.IsOccupied = Utility.getStatus((data.SpaceStatus == null) ? string.Empty : data.SpaceStatus).ToString();
                tranData.EnforcementKey = (data.EnforcementKey == null) ? string.Empty : data.EnforcementKey;
                tranData.MeterStatus = (data.MeterStatus == null) ? null : data.MeterStatus;

                tranData.METERLOC_METERTYPE = (data.METERLOC_METERTYPE == null) ? null : data.METERLOC_METERTYPE;
                tranData.ZoneName = (data.ZoneName == null) ? null : data.ZoneName;

                tranData.MeterID = data.PayStationID.HasValue ? data.PayStationID.Value.ToString() : "0";
                tranData.ReturnCode = data.ReturnCode;

                logData.Response = new ResponseEntity() { CustomerId = param.CustomerId.Value.ToString(), MeterId = tranData.MeterName, SpaceId = tranData.SpaceNo, CityCurrentTime = data.PresentMeterTime.HasValue ? data.PresentMeterTime.Value : DateTime.Now };


            }
            catch (Exception ex)
            {
                //var appPath = System.Web.Hosting.HostingEnvironment.ApplicationPhysicalPath;
                //string errorPath = Path.Combine(appPath, "GetTransformData", "Timezones");
                //ExceptionLogging.SendErrorToText(ex, "GetCitationData", errorPath);
                //tranData.ReturnCode = ((int)ReturnCodeEnum.Error).ToString();
                tranData.ReturnCode = ((int)ReturnCodeEnum.Error).ToString();
            }
            #endregion
            #region "Parkmobile Service Call"
            /////////////////////////////////////////////////
            TransactionTransform tranDataOfServiceParkMobile = new TransactionTransform();
            try
            {
                //////////////////////////////////////////
                logData.Request.CustomerId = param.CustomerId.Value.ToString();
                logData.Request.SpaceId = param.parkingSpaceID;
                logData.Request.MeterId = param.MeterName;
                //////////////////////////////////////////////////
                DateTime? currentCustTime = customerCurrentTime.CurrentTime;
                PMCustomerElement customer = ParkmobileCustomerRetriever.GetCustomer(param.CustomerId.Value);
                ParkMobileGenericFactory factory = new ParkMobileGenericFactory(customer, param.PlateNumber);
                var parkMobileResLog = new VendorResponseEntity();
                logData.VendorData.Add(VendorNames.ParkMobile, parkMobileResLog);
                tranDataOfServiceParkMobile = factory.GetRefreshSpaceData(ParkingMobileDataType.Plate, ref parkMobileResLog);
                tranDataOfServiceParkMobile.EnforcementKey = EnforcementKey;

                ///////////////////////////////////////////////////
                logData.Response = new ResponseEntity() { CustomerId = param.CustomerId.Value.ToString(), MeterId = tranData.MeterName, SpaceId = tranData.SpaceNo, CityCurrentTime = currentCustTime.HasValue ? currentCustTime.Value : DateTime.Now };
            }
            catch (Exception ex)
            {
                logData.Response = new ResponseEntity() { CustomerId = param.CustomerId.Value.ToString(), MeterId = tranData.MeterName, SpaceId = tranData.SpaceNo, CityCurrentTime = customerCurrentTime.CurrentTime.HasValue ? customerCurrentTime.CurrentTime.Value : DateTime.Now };
                //throw ex;
                tranDataOfServiceParkMobile.ReturnCode = ((int)ReturnCodeEnum.Error).ToString();
            }
            finally
            {

            }
            #endregion
            //////////////////  //Reconcile data

            List<TransactionTransform> trans = new List<TransactionTransform>();
            trans.Add(tranDataOfServiceParkMobile); // Parkmobile
            trans.Add(tranData); // Database

            var rightValue = trans.Where(t => t.ReturnCode == ((int)ReturnCodeEnum.Success).ToString()).OrderByDescending(t => t.MeterExpiredMinutes.ToNullableInt()).FirstOrDefault<TransactionTransform>();
            if (rightValue == default(TransactionTransform))
            {
                //Do nothing.just return database  object.
            }
            else
            {
                tranData = rightValue;
            }

            return tranData;
        }

        public static TransactionTransform GetSiouxCityTransformDataV4(string EnforcementKey, string filePath, RequestResponseEntity logData, CustomerTime customerCurrentTime)
        {
            TransactionTransform tranData = new TransactionTransform();
            tranData.ReturnCode = ((int)ReturnCodeEnum.InvalidInputParameters).ToString();
            TimeZoneInfo TZ = null;
            TZ = TimeZoneInfo.FindSystemTimeZoneById("Eastern Standard Time");
            //int? customerID = null;
            //string MeterName = null;
            //string parkingSpaceID;
            //string PlateNumber = null;
            //string stateName = null;
            //////////////////////////////////////
            KeyParameter param = PrepareKeyParameters.PrepareSiouxCityZone(EnforcementKey);
            if (param.ReturnCode == ((int)ReturnCodeEnum.InvalidInputParameters).ToString())
            {
                return tranData;
            }

            var zoneDetail = DataAccess.GetZones(param.CustomerId.Value, param.ZoneID);
            if(zoneDetail.Count == 0 )
            {
                tranData.ReturnCode = ((int)ReturnCodeEnum.NoRecordExists).ToString();
                logData.Request.CustomerId = param.CustomerId.Value.ToString();
                logData.Request.SpaceId = param.parkingSpaceID;
                logData.Request.MeterId = param.MeterName;
                tranData.EnforcementKey = EnforcementKey;
            }
            else
            {
   
                #region "DB Check"
                /////////////////////////////////////
                try
                {
                    Transaction data = TransactionDataValidatioin.GetDataSiouxCityZone(param.CustomerId.Value, param.MeterName, param.parkingSpaceID, param.PlateNumber, param.StateName, zoneDetail[0].Id.ToString(), EnforcementKey, filePath, logData);


                    if (data.TimeZoneId.HasValue)
                        TZ = Utility.GetTimeZoneInfo(data.TimeZoneId.Value, filePath, param.CustomerId.Value.ToString());

                    //For South Miami : PayStationId. Others it is MeterID
                    tranData.MeterName = (data.MeterName == null) ? string.Empty : data.MeterName;
                    tranData.SpaceNo = data.ParkingSpaceID.ToString();

                    tranData.MeterStreet = (data.MeterStreet == null) ? string.Empty : data.MeterStreet;

                    DateTime _MeterRTC = data.PresentMeterTime.HasValue ? data.PresentMeterTime.Value : DateTime.Now;
                    tranData.MeterRTC = String.Format("{0:yyyyMMdd_THHmmssZ}", Utility.findGMTTime(_MeterRTC, TZ));

                    DateTime _SensorEventTime = data.SensorEventTime.HasValue ? data.SensorEventTime.Value : DateTime.Now;
                    tranData.MeterLastUpdateTime = String.Format("{0:yyyyMMdd_THHmmssZ}", Utility.findGMTTime(_SensorEventTime, TZ));

                    tranData.SensorEventTime = String.Format("{0:yyyyMMdd_THHmmssZ}", Utility.findGMTTime(_SensorEventTime, TZ));

                    DateTime _endTime = data.ExpiryTime.HasValue ? data.ExpiryTime.Value : DateTime.Now;
                    tranData.ExpiredTime = String.Format("{0:yyyyMMdd_THHmmssZ}", Utility.findGMTTime(_endTime, TZ));

                    tranData.MeterExpiredMinutes = data.ExpiredMinutes.HasValue ? data.ExpiredMinutes.Value.ToString() : "0";
                    tranData.METERLOC_BLOCKNUMBER = (data.Block == null) ? string.Empty : data.Block;
                    tranData.METERLOC_STREETDIRECTION = (data.Direction == null) ? string.Empty : data.Direction;
                    tranData.METERLOC_STREETNAME = (data.MeterStreet == null) ? string.Empty : data.MeterStreet;
                    tranData.METERLOC_STREETTYPE = (data.StreetType == null) ? string.Empty : data.StreetType;
                    tranData.METERLOC_CROSSSTREET1 = (data.CrossStreet1 == null) ? string.Empty : data.CrossStreet1;
                    tranData.METERLOC_CROSSSTREET2 = (data.CrossStreet2 == null) ? string.Empty : data.CrossStreet2;
                    tranData.METERLOC_ENFORCEMENTHOURSDESC = (data.EnfHourDesc == null) ? string.Empty : data.EnfHourDesc;
                    tranData.LicensePlate = (data.PlateNumber == null) ? string.Empty : data.PlateNumber;
                    tranData.State = (data.LPLocation == null) ? string.Empty : data.LPLocation;
                    tranData.Make = (data.VMake == null) ? string.Empty : data.VMake;
                    tranData.Model = (data.VModel == null) ? string.Empty : data.VModel;
                    tranData.IsOccupied = Utility.getStatus((data.SpaceStatus == null) ? string.Empty : data.SpaceStatus).ToString();
                    tranData.EnforcementKey = (data.EnforcementKey == null) ? string.Empty : data.EnforcementKey;
                    tranData.MeterStatus = (data.MeterStatus == null) ? null : data.MeterStatus;

                    tranData.METERLOC_METERTYPE = (data.METERLOC_METERTYPE == null) ? null : data.METERLOC_METERTYPE;
                    tranData.ZoneName = (data.ZoneName == null) ? null : data.ZoneName;

                    tranData.MeterID = data.PayStationID.HasValue ? data.PayStationID.Value.ToString() : "0";
                    tranData.ReturnCode = data.ReturnCode;

                    logData.Response = new ResponseEntity() { CustomerId = param.CustomerId.Value.ToString(), MeterId = tranData.MeterName, SpaceId = tranData.SpaceNo, CityCurrentTime = data.PresentMeterTime.HasValue ? data.PresentMeterTime.Value : DateTime.Now };


                }
                catch (Exception ex)
                {
                    //var appPath = System.Web.Hosting.HostingEnvironment.ApplicationPhysicalPath;
                    //string errorPath = Path.Combine(appPath, "GetTransformData", "Timezones");
                    //ExceptionLogging.SendErrorToText(ex, "GetCitationData", errorPath);
                    //tranData.ReturnCode = ((int)ReturnCodeEnum.Error).ToString();
                    tranData.ReturnCode = ((int)ReturnCodeEnum.Error).ToString();
                }
                #endregion
           
            }


            #region "Parkmobile Service Call"
            /////////////////////////////////////////////////
            TransactionTransform tranDataOfServiceParkMobile = new TransactionTransform();
            try
            {
                //////////////////////////////////////////
                logData.Request.CustomerId = param.CustomerId.Value.ToString();
                logData.Request.SpaceId = param.parkingSpaceID;
                logData.Request.MeterId = param.MeterName;
                //////////////////////////////////////////////////
                DateTime? currentCustTime = customerCurrentTime.CurrentTime;
                PMCustomerElement customer = ParkmobileCustomerRetriever.GetCustomer(param.CustomerId.Value);
                ParkMobileGenericFactory factory = new ParkMobileGenericFactory(customer, param.PlateNumber);
                var parkMobileResLog = new VendorResponseEntity();
                logData.VendorData.Add(VendorNames.ParkMobile, parkMobileResLog);
                //tranDataOfServiceParkMobile = factory.GetRefreshSpaceData(ParkingMobileDataType.Plate, ref parkMobileResLog);

                tranDataOfServiceParkMobile = factory.GetRefreshSpaceZoneData(ParkingMobileDataType.Plate, param.ZoneID, ref parkMobileResLog);
                tranDataOfServiceParkMobile.EnforcementKey = EnforcementKey;

                ///////////////////////////////////////////////////
                logData.Response = new ResponseEntity() { CustomerId = param.CustomerId.Value.ToString(), MeterId = tranData.MeterName, SpaceId = tranData.SpaceNo, CityCurrentTime = currentCustTime.HasValue ? currentCustTime.Value : DateTime.Now };
            }
            catch (Exception ex)
            {
                logData.Response = new ResponseEntity() { CustomerId = param.CustomerId.Value.ToString(), MeterId = tranData.MeterName, SpaceId = tranData.SpaceNo, CityCurrentTime = customerCurrentTime.CurrentTime.HasValue ? customerCurrentTime.CurrentTime.Value : DateTime.Now };
                //throw ex;
                tranDataOfServiceParkMobile.ReturnCode = ((int)ReturnCodeEnum.Error).ToString();
            }
            finally
            {

            }
            #endregion
            //////////////////  //Reconcile data

            List<TransactionTransform> trans = new List<TransactionTransform>();
            trans.Add(tranDataOfServiceParkMobile); // Parkmobile
            //trans.Add(tranData); // Database

            var rightValue = trans.Where(t => t.ReturnCode == ((int)ReturnCodeEnum.Success).ToString()).OrderByDescending(t => t.MeterExpiredMinutes.ToNullableInt()).FirstOrDefault<TransactionTransform>();
            if (rightValue == default(TransactionTransform))
            {
                //Do nothing.just return database  object.
            }
            else
            {
                tranData = rightValue;
            }

            return tranData;
        }
        private static Transaction GetDataSiouxCity(int customerID, string MeterName, string parkingSpaceID, string PlateNumber, string statename, string EnforcementKey, string filePath, RequestResponseEntity logData)
        {
            string ZoneID = null;
            string SpaceStatus = null;
            Transaction data = new Transaction();
            try
            {
                List<Transaction> transactionList = new List<Transaction>();

                logData.Request.CustomerId = customerID.ToString();
                logData.Request.SpaceId = parkingSpaceID;
                logData.Request.MeterId = MeterName;

                transactionList = DataAccess.GetSiouxCityCitationData(customerID, null, null, null, PlateNumber, null);

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
            }
            catch (Exception ex)
            {
                var appPath = System.Web.Hosting.HostingEnvironment.ApplicationPhysicalPath;
                string errorPath = Path.Combine(appPath, "GetData", "Timezones");
                ExceptionLogging.SendErrorToText(ex, "GetCitationData", errorPath);
                data.ReturnCode = ((int)ReturnCodeEnum.Error).ToString();
                throw;
            }
            return data;
        }


        private static Transaction GetDataSiouxCityZone(int customerID, string MeterName, string parkingSpaceID, string PlateNumber, string statename,string ZoneId, string EnforcementKey, string filePath, RequestResponseEntity logData)
        {
            string ZoneID = ZoneId;
            string SpaceStatus = null;
            Transaction data = new Transaction();
            try
            {
                List<Transaction> transactionList = new List<Transaction>();

                logData.Request.CustomerId = customerID.ToString();
                logData.Request.SpaceId = parkingSpaceID;
                logData.Request.MeterId = MeterName;

                transactionList = DataAccess.GetSiouxCityCitationDatZone(customerID, ZoneID, null, null, PlateNumber, null);

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
            }
            catch (Exception ex)
            {
                var appPath = System.Web.Hosting.HostingEnvironment.ApplicationPhysicalPath;
                string errorPath = Path.Combine(appPath, "GetData", "Timezones");
                ExceptionLogging.SendErrorToText(ex, "GetCitationData", errorPath);
                data.ReturnCode = ((int)ReturnCodeEnum.Error).ToString();
                throw;
            }
            return data;
        }

        #endregion

        #region SanJose
        public static TransactionTransform GetSanJoseTransformData(string EnforcementKey, string filePath, RequestResponseEntity logData, CustomerTime customerCurrentTime)
        {
            TransactionTransform tranData = new TransactionTransform();
            tranData.ReturnCode = ((int)ReturnCodeEnum.InvalidInputParameters).ToString();
            TimeZoneInfo TZ = null;
            TZ = TimeZoneInfo.FindSystemTimeZoneById("Eastern Standard Time");
            //int? customerID = null;
            //string MeterName = null;
            //string parkingSpaceID;
            //string PlateNumber = null;
            //string stateName = null;
            //////////////////////////////////////
            KeyParameter param = PrepareKeyParameters.PrepareSanJose(EnforcementKey);
            if (param.ReturnCode == ((int)ReturnCodeEnum.InvalidInputParameters).ToString())
            {
                return tranData;
            }
            #region "DB Check"
            /////////////////////////////////////
            try
            {
                Transaction data = TransactionDataValidatioin.GetDataSanJose(param.CustomerId.Value, param.MeterName, param.parkingSpaceID, param.PlateNumber, param.StateName, EnforcementKey, filePath, logData);


                if (data.TimeZoneId.HasValue)
                    TZ = Utility.GetTimeZoneInfo(data.TimeZoneId.Value, filePath, param.CustomerId.Value.ToString());

                //For South Miami : PayStationId. Others it is MeterID
                tranData.MeterName = (data.MeterName == null) ? string.Empty : data.MeterName;
                tranData.SpaceNo = data.ParkingSpaceID.ToString();

                tranData.MeterStreet = (data.MeterStreet == null) ? string.Empty : data.MeterStreet;

                DateTime _MeterRTC = data.PresentMeterTime.HasValue ? data.PresentMeterTime.Value : DateTime.Now;
                tranData.MeterRTC = String.Format("{0:yyyyMMdd_THHmmssZ}", Utility.findGMTTime(_MeterRTC, TZ));

                DateTime _SensorEventTime = data.SensorEventTime.HasValue ? data.SensorEventTime.Value : DateTime.Now;
                tranData.MeterLastUpdateTime = String.Format("{0:yyyyMMdd_THHmmssZ}", Utility.findGMTTime(_SensorEventTime, TZ));

                tranData.SensorEventTime = String.Format("{0:yyyyMMdd_THHmmssZ}", Utility.findGMTTime(_SensorEventTime, TZ));

                DateTime _endTime = data.ExpiryTime.HasValue ? data.ExpiryTime.Value : DateTime.Now;
                tranData.ExpiredTime = String.Format("{0:yyyyMMdd_THHmmssZ}", Utility.findGMTTime(_endTime, TZ));

                tranData.MeterExpiredMinutes = data.ExpiredMinutes.HasValue ? data.ExpiredMinutes.Value.ToString() : "0";
                tranData.METERLOC_BLOCKNUMBER = (data.Block == null) ? string.Empty : data.Block;
                tranData.METERLOC_STREETDIRECTION = (data.Direction == null) ? string.Empty : data.Direction;
                tranData.METERLOC_STREETNAME = (data.MeterStreet == null) ? string.Empty : data.MeterStreet;
                tranData.METERLOC_STREETTYPE = (data.StreetType == null) ? string.Empty : data.StreetType;
                tranData.METERLOC_CROSSSTREET1 = (data.CrossStreet1 == null) ? string.Empty : data.CrossStreet1;
                tranData.METERLOC_CROSSSTREET2 = (data.CrossStreet2 == null) ? string.Empty : data.CrossStreet2;
                tranData.METERLOC_ENFORCEMENTHOURSDESC = (data.EnfHourDesc == null) ? string.Empty : data.EnfHourDesc;
                tranData.LicensePlate = (data.PlateNumber == null) ? string.Empty : data.PlateNumber;
                tranData.State = (data.LPLocation == null) ? string.Empty : data.LPLocation;
                tranData.Make = (data.VMake == null) ? string.Empty : data.VMake;
                tranData.Model = (data.VModel == null) ? string.Empty : data.VModel;
                tranData.IsOccupied = Utility.getStatus((data.SpaceStatus == null) ? string.Empty : data.SpaceStatus).ToString();
                tranData.EnforcementKey = (data.EnforcementKey == null) ? string.Empty : data.EnforcementKey;
                tranData.MeterStatus = (data.MeterStatus == null) ? null : data.MeterStatus;

                tranData.METERLOC_METERTYPE = (data.METERLOC_METERTYPE == null) ? null : data.METERLOC_METERTYPE;
                tranData.ZoneName = (data.ZoneName == null) ? null : data.ZoneName;

                tranData.MeterID = data.PayStationID.HasValue ? data.PayStationID.Value.ToString() : "0";
                tranData.ReturnCode = data.ReturnCode;

                logData.Response = new ResponseEntity() { CustomerId = param.CustomerId.Value.ToString(), MeterId = tranData.MeterName, SpaceId = tranData.SpaceNo, CityCurrentTime = data.PresentMeterTime.HasValue ? data.PresentMeterTime.Value : DateTime.Now };


            }
            catch (Exception ex)
            {
                //var appPath = System.Web.Hosting.HostingEnvironment.ApplicationPhysicalPath;
                //string errorPath = Path.Combine(appPath, "GetTransformData", "Timezones");
                //ExceptionLogging.SendErrorToText(ex, "GetCitationData", errorPath);
                //tranData.ReturnCode = ((int)ReturnCodeEnum.Error).ToString();
                tranData.ReturnCode = ((int)ReturnCodeEnum.Error).ToString();
            }
            #endregion
            //////////////////  //Reconcile data

            List<TransactionTransform> trans = new List<TransactionTransform>();
            trans.Add(tranData); // Database

            var rightValue = trans.Where(t => t.ReturnCode == ((int)ReturnCodeEnum.Success).ToString()).OrderByDescending(t => t.MeterExpiredMinutes.ToNullableInt()).FirstOrDefault<TransactionTransform>();
            if (rightValue == default(TransactionTransform))
            {
                //Do nothing.just return database  object.
            }
            else
            {
                tranData = rightValue;
            }

            return tranData;
        }
        private static Transaction GetDataSanJose(int customerID, string MeterName, string parkingSpaceID, string PlateNumber, string statename, string EnforcementKey, string filePath, RequestResponseEntity logData)
        {
            string ZoneID = null;
            string SpaceStatus = null;
            Transaction data = new Transaction();
            try
            {
                List<Transaction> transactionList = new List<Transaction>();

                logData.Request.CustomerId = customerID.ToString();
                logData.Request.SpaceId = parkingSpaceID;
                logData.Request.MeterId = MeterName;

                transactionList = DataAccess.GetSanJoseCitationData(customerID, null, MeterName, parkingSpaceID, null, null);

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
            }
            catch (Exception ex)
            {
                var appPath = System.Web.Hosting.HostingEnvironment.ApplicationPhysicalPath;
                string errorPath = Path.Combine(appPath, "GetData", "Timezones");
                ExceptionLogging.SendErrorToText(ex, "GetCitationData", errorPath);
                data.ReturnCode = ((int)ReturnCodeEnum.Error).ToString();
                throw;
            }
            return data;
        }

        #endregion

        #region Bay Harbor

        public static TransactionTransform GetBayHarborTransformData(string EnforcementKey, string filePath, RequestResponseEntity logData, CustomerTime customerCurrentTime)
        {
            TransactionTransform tranData = new TransactionTransform();
            tranData.ReturnCode = ((int)ReturnCodeEnum.InvalidInputParameters).ToString();
            TimeZoneInfo TZ = null;
            TZ = TimeZoneInfo.FindSystemTimeZoneById("Eastern Standard Time");
            //int? customerID = null;
            //string MeterName = null;
            //string parkingSpaceID;
            //string PlateNumber = null;
            //string stateName = null;
            //////////////////////////////////////
            KeyParameter param = PrepareKeyParameters.PrepareBayHarbor(EnforcementKey);
            if (param.ReturnCode == ((int)ReturnCodeEnum.InvalidInputParameters).ToString())
            {
                return tranData;
            }

            #region "DB Check"
            /////////////////////////////////////
            try
            {
                Transaction data = TransactionDataValidatioin.GetDataBayHarbor(param.CustomerId.Value, param.MeterName, param.parkingSpaceID, param.PlateNumber, param.StateName, EnforcementKey, filePath, logData);


                if (data.TimeZoneId.HasValue)
                    TZ = Utility.GetTimeZoneInfo(data.TimeZoneId.Value, filePath, param.CustomerId.Value.ToString());

                //For South Miami : PayStationId. Others it is MeterID
                tranData.MeterName = string.Empty; // (data.MeterName == null) ? string.Empty : data.MeterName;
                tranData.SpaceNo = string.Empty;  //data.ParkingSpaceID.ToString();

                tranData.MeterStreet = (data.MeterStreet == null) ? string.Empty : data.MeterStreet;

                DateTime _MeterRTC = data.PresentMeterTime.HasValue ? data.PresentMeterTime.Value : DateTime.Now;
                tranData.MeterRTC = String.Format("{0:yyyyMMdd_THHmmssZ}", Utility.findGMTTime(_MeterRTC, TZ));

                DateTime _SensorEventTime = data.SensorEventTime.HasValue ? data.SensorEventTime.Value : DateTime.Now;
                tranData.MeterLastUpdateTime = String.Format("{0:yyyyMMdd_THHmmssZ}", Utility.findGMTTime(_SensorEventTime, TZ));

                tranData.SensorEventTime = String.Format("{0:yyyyMMdd_THHmmssZ}", Utility.findGMTTime(_SensorEventTime, TZ));

                DateTime _endTime = data.ExpiryTime.HasValue ? data.ExpiryTime.Value : DateTime.Now;
                tranData.ExpiredTime = String.Format("{0:yyyyMMdd_THHmmssZ}", Utility.findGMTTime(_endTime, TZ));

                tranData.MeterExpiredMinutes = data.ExpiredMinutes.HasValue ? data.ExpiredMinutes.Value.ToString() : "0";
                tranData.METERLOC_BLOCKNUMBER = (data.Block == null) ? string.Empty : data.Block;
                tranData.METERLOC_STREETDIRECTION = (data.Direction == null) ? string.Empty : data.Direction;
                tranData.METERLOC_STREETNAME = (data.MeterStreet == null) ? string.Empty : data.MeterStreet;
                tranData.METERLOC_STREETTYPE = (data.StreetType == null) ? string.Empty : data.StreetType;
                tranData.METERLOC_CROSSSTREET1 = (data.CrossStreet1 == null) ? string.Empty : data.CrossStreet1;
                tranData.METERLOC_CROSSSTREET2 = (data.CrossStreet2 == null) ? string.Empty : data.CrossStreet2;
                tranData.METERLOC_ENFORCEMENTHOURSDESC = (data.EnfHourDesc == null) ? string.Empty : data.EnfHourDesc;
                tranData.LicensePlate = (data.PlateNumber == null) ? string.Empty : data.PlateNumber;
                tranData.State = (data.LPLocation == null) ? string.Empty : data.LPLocation;
                tranData.Make = (data.VMake == null) ? string.Empty : data.VMake;
                tranData.Model = (data.VModel == null) ? string.Empty : data.VModel;
                tranData.IsOccupied = Utility.getStatus((data.SpaceStatus == null) ? string.Empty : data.SpaceStatus).ToString();
                tranData.EnforcementKey = (data.EnforcementKey == null) ? string.Empty : data.EnforcementKey;
                tranData.MeterStatus = (data.MeterStatus == null) ? null : data.MeterStatus;

                tranData.METERLOC_METERTYPE = (data.METERLOC_METERTYPE == null) ? null : data.METERLOC_METERTYPE;
                tranData.ZoneName = (data.ZoneName == null) ? null : data.ZoneName;
                tranData.MeterID = String.Empty;
                tranData.ReturnCode = data.ReturnCode;

                logData.Response = new ResponseEntity() { CustomerId = param.CustomerId.Value.ToString(), MeterId = tranData.MeterName, SpaceId = tranData.SpaceNo, CityCurrentTime = data.PresentMeterTime.HasValue ? data.PresentMeterTime.Value : DateTime.Now };
            }
            catch (Exception ex)
            {
                ////Intentionally Error Condition Commented
                ////var appPath = System.Web.Hosting.HostingEnvironment.ApplicationPhysicalPath;
                ////string errorPath = Path.Combine(appPath, "GetTransformData", "Timezones");
                ////ExceptionLogging.SendErrorToText(ex, "GetCitationData", errorPath);
                ////tranData.ReturnCode = ((int)ReturnCodeEnum.Error).ToString();

                tranData.ReturnCode = ((int)ReturnCodeEnum.Error).ToString();
            }

            #endregion
            #region "Parkeon Service Call"
            /////////////////////////////////////////////////
            TransactionTransform tranDataOfService = new TransactionTransform();
            ServiceParameters serviceParam = null;
            try
            {
                //////////////////////////////////////////
                logData.Request.CustomerId = param.CustomerId.Value.ToString();
                logData.Request.SpaceId = param.parkingSpaceID;
                logData.Request.MeterId = param.MeterName;
                //////////////////////////////////////////////////
                DateTime? currentCustTime = customerCurrentTime.CurrentTime;
                tranData.EnforcementKey = EnforcementKey;
                ParkeonCustomerElement customer = ParkeonCustomerRetriever.GetCustomer(param.CustomerId.Value);
                serviceParam = new ServiceParameters() { customerId = (Convert.ToInt64(customer.CustomerId)), userName = customer.UserName, userPassword = customer.UserPassword, vendorId = (Convert.ToInt64(customer.VendorId)) };
                ParkeonServiceAccess parkeonService = new ParkeonServiceAccess(serviceParam, param, customer);
                parkeonService.ProcessRequest(ref tranDataOfService, TypeConditions.check_plate, TZ, ref currentCustTime);
                serviceParam = null;
                ///////////////////////////////////////////////////
                logData.Response = new ResponseEntity() { CustomerId = param.CustomerId.Value.ToString(), MeterId = tranData.MeterName, SpaceId = tranData.SpaceNo, CityCurrentTime = currentCustTime.HasValue ? currentCustTime.Value : DateTime.Now };

            }
            catch (Exception ex)
            {
                logData.Response = new ResponseEntity() { CustomerId = param.CustomerId.Value.ToString(), MeterId = tranData.MeterName, SpaceId = tranData.SpaceNo, CityCurrentTime = customerCurrentTime.CurrentTime.HasValue ? customerCurrentTime.CurrentTime.Value : DateTime.Now };
                //throw ex;
                tranDataOfService.ReturnCode = ((int)ReturnCodeEnum.Error).ToString();
            }
            finally
            {
                serviceParam = null;
            }
            #endregion

            //////////////////  //Reconcile data
            List<TransactionTransform> trans = new List<TransactionTransform>();
            trans.Add(tranData); // Database
            trans.Add(tranDataOfService); //Parkeon

            var rightValue = trans.Where(t => t.ReturnCode == ((int)ReturnCodeEnum.Success).ToString()).OrderByDescending(t => t.MeterExpiredMinutes.ToNullableInt()).FirstOrDefault<TransactionTransform>();
            if (rightValue == default(TransactionTransform))
            {
                //Do nothing.just return database  object.
            }
            else
            {
                tranData = rightValue;
            }

            return tranData;
        }
        public static TransactionTransform GetBayHarborTransformDataV2(string EnforcementKey, string filePath, RequestResponseEntity logData, CustomerTime customerCurrentTime)
        {
            TransactionTransform tranData = new TransactionTransform();
            tranData.ReturnCode = ((int)ReturnCodeEnum.InvalidInputParameters).ToString();
            TimeZoneInfo TZ = null;
            TZ = TimeZoneInfo.FindSystemTimeZoneById("Eastern Standard Time");
            //int? customerID = null;
            //string MeterName = null;
            //string parkingSpaceID;
            //string PlateNumber = null;
            //string stateName = null;
            //////////////////////////////////////
            KeyParameter param = PrepareKeyParameters.PrepareBayHarbor(EnforcementKey);
            if (param.ReturnCode == ((int)ReturnCodeEnum.InvalidInputParameters).ToString())
            {
                return tranData;
            }

            #region "DB Check"
            /////////////////////////////////////
            try
            {
                Transaction data = TransactionDataValidatioin.GetDataBayHarbor(param.CustomerId.Value, param.MeterName, param.parkingSpaceID, param.PlateNumber, param.StateName, EnforcementKey, filePath, logData);


                if (data.TimeZoneId.HasValue)
                    TZ = Utility.GetTimeZoneInfo(data.TimeZoneId.Value, filePath, param.CustomerId.Value.ToString());

                //For South Miami : PayStationId. Others it is MeterID
                tranData.MeterName = string.Empty; // (data.MeterName == null) ? string.Empty : data.MeterName;
                tranData.SpaceNo = string.Empty;  //data.ParkingSpaceID.ToString();

                tranData.MeterStreet = (data.MeterStreet == null) ? string.Empty : data.MeterStreet;

                DateTime _MeterRTC = data.PresentMeterTime.HasValue ? data.PresentMeterTime.Value : DateTime.Now;
                tranData.MeterRTC = String.Format("{0:yyyyMMdd_THHmmssZ}", Utility.findGMTTime(_MeterRTC, TZ));

                DateTime _SensorEventTime = data.SensorEventTime.HasValue ? data.SensorEventTime.Value : DateTime.Now;
                tranData.MeterLastUpdateTime = String.Format("{0:yyyyMMdd_THHmmssZ}", Utility.findGMTTime(_SensorEventTime, TZ));

                tranData.SensorEventTime = String.Format("{0:yyyyMMdd_THHmmssZ}", Utility.findGMTTime(_SensorEventTime, TZ));

                DateTime _endTime = data.ExpiryTime.HasValue ? data.ExpiryTime.Value : DateTime.Now;
                tranData.ExpiredTime = String.Format("{0:yyyyMMdd_THHmmssZ}", Utility.findGMTTime(_endTime, TZ));

                tranData.MeterExpiredMinutes = data.ExpiredMinutes.HasValue ? data.ExpiredMinutes.Value.ToString() : "0";
                tranData.METERLOC_BLOCKNUMBER = (data.Block == null) ? string.Empty : data.Block;
                tranData.METERLOC_STREETDIRECTION = (data.Direction == null) ? string.Empty : data.Direction;
                tranData.METERLOC_STREETNAME = (data.MeterStreet == null) ? string.Empty : data.MeterStreet;
                tranData.METERLOC_STREETTYPE = (data.StreetType == null) ? string.Empty : data.StreetType;
                tranData.METERLOC_CROSSSTREET1 = (data.CrossStreet1 == null) ? string.Empty : data.CrossStreet1;
                tranData.METERLOC_CROSSSTREET2 = (data.CrossStreet2 == null) ? string.Empty : data.CrossStreet2;
                tranData.METERLOC_ENFORCEMENTHOURSDESC = (data.EnfHourDesc == null) ? string.Empty : data.EnfHourDesc;
                tranData.LicensePlate = (data.PlateNumber == null) ? string.Empty : data.PlateNumber;
                tranData.State = (data.LPLocation == null) ? string.Empty : data.LPLocation;
                tranData.Make = (data.VMake == null) ? string.Empty : data.VMake;
                tranData.Model = (data.VModel == null) ? string.Empty : data.VModel;
                tranData.IsOccupied = Utility.getStatus((data.SpaceStatus == null) ? string.Empty : data.SpaceStatus).ToString();
                tranData.EnforcementKey = (data.EnforcementKey == null) ? string.Empty : data.EnforcementKey;
                tranData.MeterStatus = (data.MeterStatus == null) ? null : data.MeterStatus;

                tranData.METERLOC_METERTYPE = (data.METERLOC_METERTYPE == null) ? null : data.METERLOC_METERTYPE;
                tranData.ZoneName = (data.ZoneName == null) ? null : data.ZoneName;
                tranData.MeterID = String.Empty;
                tranData.ReturnCode = data.ReturnCode;

                logData.Response = new ResponseEntity() { CustomerId = param.CustomerId.Value.ToString(), MeterId = tranData.MeterName, SpaceId = tranData.SpaceNo, CityCurrentTime = data.PresentMeterTime.HasValue ? data.PresentMeterTime.Value : DateTime.Now };
            }
            catch (Exception ex)
            {
                ////Intentionally Error Condition Commented
                ////var appPath = System.Web.Hosting.HostingEnvironment.ApplicationPhysicalPath;
                ////string errorPath = Path.Combine(appPath, "GetTransformData", "Timezones");
                ////ExceptionLogging.SendErrorToText(ex, "GetCitationData", errorPath);
                ////tranData.ReturnCode = ((int)ReturnCodeEnum.Error).ToString();

                tranData.ReturnCode = ((int)ReturnCodeEnum.Error).ToString();
            }

            #endregion
            #region "Parkeon Service Call"
            /////////////////////////////////////////////////
            TransactionTransform tranDataOfService = new TransactionTransform();
            ServiceParameters serviceParam = null;
            try
            {
                //////////////////////////////////////////
                logData.Request.CustomerId = param.CustomerId.Value.ToString();
                logData.Request.SpaceId = param.parkingSpaceID;
                logData.Request.MeterId = param.MeterName;
                //////////////////////////////////////////////////
                DateTime? currentCustTime = customerCurrentTime.CurrentTime;
                tranData.EnforcementKey = EnforcementKey;
                ParkeonCustomerElement customer = ParkeonCustomerRetriever.GetCustomer(param.CustomerId.Value);
                serviceParam = new ServiceParameters() { customerId = (Convert.ToInt64(customer.CustomerId)), userName = customer.UserName, userPassword = customer.UserPassword, vendorId = (Convert.ToInt64(customer.VendorId)) };
                ParkeonServiceAccess parkeonService = new ParkeonServiceAccess(serviceParam, param, customer);
                parkeonService.ProcessRequest(ref tranDataOfService, TypeConditions.check_plate, TZ, ref currentCustTime);
                serviceParam = null;
                ///////////////////////////////////////////////////
                logData.Response = new ResponseEntity() { CustomerId = param.CustomerId.Value.ToString(), MeterId = tranData.MeterName, SpaceId = tranData.SpaceNo, CityCurrentTime = currentCustTime.HasValue ? currentCustTime.Value : DateTime.Now };

            }
            catch (Exception ex)
            {
                logData.Response = new ResponseEntity() { CustomerId = param.CustomerId.Value.ToString(), MeterId = tranData.MeterName, SpaceId = tranData.SpaceNo, CityCurrentTime = customerCurrentTime.CurrentTime.HasValue ? customerCurrentTime.CurrentTime.Value : DateTime.Now };
                //throw ex;
                tranDataOfService.ReturnCode = ((int)ReturnCodeEnum.Error).ToString();
            }
            finally
            {
                serviceParam = null;
            }
            #endregion
            #region "PayByPhone Service Call"
            /////////////////////////////////////////////////
            TransactionTransform tranDataOfPBPService = new TransactionTransform();
            try
            {
                //////////////////////////////////////////

                logData.Request.CustomerId = param.CustomerId.Value.ToString();
                logData.Request.SpaceId = param.parkingSpaceID;
                logData.Request.MeterId = param.MeterName;
                //////////////////////////////////////////////////
                DateTime? currentCustTime = customerCurrentTime.CurrentTime;
                CustomerElement customer = CustomerRetriever.GetCustomer(param.CustomerId.Value);
                PayByPhoneGenericFactory factory = new PayByPhoneGenericFactory(customer, param.PlateNumber);
                tranDataOfPBPService = factory.GetRefreshPlateData();
                tranDataOfPBPService.EnforcementKey = EnforcementKey;

                ///////////////////////////////////////////////////
                logData.Response = new ResponseEntity() { CustomerId = param.CustomerId.Value.ToString(), MeterId = tranData.MeterName, SpaceId = tranData.SpaceNo, CityCurrentTime = currentCustTime.HasValue ? currentCustTime.Value : DateTime.Now };

            }
            catch (Exception ex)
            {
                logData.Response = new ResponseEntity() { CustomerId = param.CustomerId.Value.ToString(), MeterId = tranData.MeterName, SpaceId = tranData.SpaceNo, CityCurrentTime = customerCurrentTime.CurrentTime.HasValue ? customerCurrentTime.CurrentTime.Value : DateTime.Now };
                //throw ex;
                tranDataOfPBPService.ReturnCode = ((int)ReturnCodeEnum.Error).ToString();
            }
            finally
            {

            }
            #endregion
            //////////////////  //Reconcile data
            List<TransactionTransform> trans = new List<TransactionTransform>();
            trans.Add(tranData); // Database
            trans.Add(tranDataOfService); //Parkeon
            trans.Add(tranDataOfPBPService); //PayByCell

            var rightValue = trans.Where(t => t.ReturnCode == ((int)ReturnCodeEnum.Success).ToString()).OrderByDescending(t => t.MeterExpiredMinutes.ToNullableInt()).FirstOrDefault<TransactionTransform>();
            if (rightValue == default(TransactionTransform))
            {
                //Do nothing.just return database  object.
            }
            else
            {
                tranData = rightValue;
            }

            return tranData;
        }
        public static TransactionTransform GetBayHarborTransformDataV3(string EnforcementKey, string filePath, RequestResponseEntity logData, CustomerTime customerCurrentTime)
        {
            TransactionTransform tranData = new TransactionTransform();
            tranData.ReturnCode = ((int)ReturnCodeEnum.InvalidInputParameters).ToString();
            TimeZoneInfo TZ = null;
            TZ = TimeZoneInfo.FindSystemTimeZoneById("Eastern Standard Time");
            //int? customerID = null;
            //string MeterName = null;
            //string parkingSpaceID;
            //string PlateNumber = null;
            //string stateName = null;
            //////////////////////////////////////
            KeyParameter param = PrepareKeyParameters.PrepareBayHarbor(EnforcementKey);
            if (param.ReturnCode == ((int)ReturnCodeEnum.InvalidInputParameters).ToString())
            {
                return tranData;
            }

            #region "DB Check"
            /////////////////////////////////////
            try
            {
                Transaction data = TransactionDataValidatioin.GetDataBayHarbor(param.CustomerId.Value, param.MeterName, param.parkingSpaceID, param.PlateNumber, param.StateName, EnforcementKey, filePath, logData);


                if (data.TimeZoneId.HasValue)
                    TZ = Utility.GetTimeZoneInfo(data.TimeZoneId.Value, filePath, param.CustomerId.Value.ToString());

                //For South Miami : PayStationId. Others it is MeterID
                tranData.MeterName = string.Empty; // (data.MeterName == null) ? string.Empty : data.MeterName;
                tranData.SpaceNo = string.Empty;  //data.ParkingSpaceID.ToString();

                tranData.MeterStreet = (data.MeterStreet == null) ? string.Empty : data.MeterStreet;

                DateTime _MeterRTC = data.PresentMeterTime.HasValue ? data.PresentMeterTime.Value : DateTime.Now;
                tranData.MeterRTC = String.Format("{0:yyyyMMdd_THHmmssZ}", Utility.findGMTTime(_MeterRTC, TZ));

                DateTime _SensorEventTime = data.SensorEventTime.HasValue ? data.SensorEventTime.Value : DateTime.Now;
                tranData.MeterLastUpdateTime = String.Format("{0:yyyyMMdd_THHmmssZ}", Utility.findGMTTime(_SensorEventTime, TZ));

                tranData.SensorEventTime = String.Format("{0:yyyyMMdd_THHmmssZ}", Utility.findGMTTime(_SensorEventTime, TZ));

                DateTime _endTime = data.ExpiryTime.HasValue ? data.ExpiryTime.Value : DateTime.Now;
                tranData.ExpiredTime = String.Format("{0:yyyyMMdd_THHmmssZ}", Utility.findGMTTime(_endTime, TZ));

                tranData.MeterExpiredMinutes = data.ExpiredMinutes.HasValue ? data.ExpiredMinutes.Value.ToString() : "0";
                tranData.METERLOC_BLOCKNUMBER = (data.Block == null) ? string.Empty : data.Block;
                tranData.METERLOC_STREETDIRECTION = (data.Direction == null) ? string.Empty : data.Direction;
                tranData.METERLOC_STREETNAME = (data.MeterStreet == null) ? string.Empty : data.MeterStreet;
                tranData.METERLOC_STREETTYPE = (data.StreetType == null) ? string.Empty : data.StreetType;
                tranData.METERLOC_CROSSSTREET1 = (data.CrossStreet1 == null) ? string.Empty : data.CrossStreet1;
                tranData.METERLOC_CROSSSTREET2 = (data.CrossStreet2 == null) ? string.Empty : data.CrossStreet2;
                tranData.METERLOC_ENFORCEMENTHOURSDESC = (data.EnfHourDesc == null) ? string.Empty : data.EnfHourDesc;
                tranData.LicensePlate = (data.PlateNumber == null) ? string.Empty : data.PlateNumber;
                tranData.State = (data.LPLocation == null) ? string.Empty : data.LPLocation;
                tranData.Make = (data.VMake == null) ? string.Empty : data.VMake;
                tranData.Model = (data.VModel == null) ? string.Empty : data.VModel;
                tranData.IsOccupied = Utility.getStatus((data.SpaceStatus == null) ? string.Empty : data.SpaceStatus).ToString();
                tranData.EnforcementKey = (data.EnforcementKey == null) ? string.Empty : data.EnforcementKey;
                tranData.MeterStatus = (data.MeterStatus == null) ? null : data.MeterStatus;

                tranData.METERLOC_METERTYPE = (data.METERLOC_METERTYPE == null) ? null : data.METERLOC_METERTYPE;
                tranData.ZoneName = (data.ZoneName == null) ? null : data.ZoneName;
                tranData.MeterID = String.Empty;
                tranData.ReturnCode = data.ReturnCode;

                logData.Response = new ResponseEntity() { CustomerId = param.CustomerId.Value.ToString(), MeterId = tranData.MeterName, SpaceId = tranData.SpaceNo, CityCurrentTime = data.PresentMeterTime.HasValue ? data.PresentMeterTime.Value : DateTime.Now };
            }
            catch (Exception ex)
            {
                ////Intentionally Error Condition Commented
                ////var appPath = System.Web.Hosting.HostingEnvironment.ApplicationPhysicalPath;
                ////string errorPath = Path.Combine(appPath, "GetTransformData", "Timezones");
                ////ExceptionLogging.SendErrorToText(ex, "GetCitationData", errorPath);
                ////tranData.ReturnCode = ((int)ReturnCodeEnum.Error).ToString();

                tranData.ReturnCode = ((int)ReturnCodeEnum.Error).ToString();
            }

            #endregion
            #region "Parkeon Service Call"
            /////////////////////////////////////////////////
            TransactionTransform tranDataOfService = new TransactionTransform();
            ServiceParameters serviceParam = null;
            try
            {
                //////////////////////////////////////////
                logData.Request.CustomerId = param.CustomerId.Value.ToString();
                logData.Request.SpaceId = param.parkingSpaceID;
                logData.Request.MeterId = param.MeterName;
                //////////////////////////////////////////////////
                DateTime? currentCustTime = customerCurrentTime.CurrentTime;
                tranData.EnforcementKey = EnforcementKey;
                ParkeonCustomerElement customer = ParkeonCustomerRetriever.GetCustomer(param.CustomerId.Value);
                serviceParam = new ServiceParameters() { customerId = (Convert.ToInt64(customer.CustomerId)), userName = customer.UserName, userPassword = customer.UserPassword, vendorId = (Convert.ToInt64(customer.VendorId)) };
                ParkeonServiceAccess parkeonService = new ParkeonServiceAccess(serviceParam, param, customer);
                var parkeonResLog = new VendorResponseEntity();
                logData.VendorData.Add(VendorNames.Parkeon, parkeonResLog);
                parkeonService.ProcessRequest(ref tranDataOfService, TypeConditions.check_plate, TZ, ref currentCustTime, ref parkeonResLog);
                serviceParam = null;
                ///////////////////////////////////////////////////
                logData.Response = new ResponseEntity() { CustomerId = param.CustomerId.Value.ToString(), MeterId = tranData.MeterName, SpaceId = tranData.SpaceNo, CityCurrentTime = currentCustTime.HasValue ? currentCustTime.Value : DateTime.Now };

            }
            catch (Exception ex)
            {
                logData.Response = new ResponseEntity() { CustomerId = param.CustomerId.Value.ToString(), MeterId = tranData.MeterName, SpaceId = tranData.SpaceNo, CityCurrentTime = customerCurrentTime.CurrentTime.HasValue ? customerCurrentTime.CurrentTime.Value : DateTime.Now };
                //throw ex;
                tranDataOfService.ReturnCode = ((int)ReturnCodeEnum.Error).ToString();
            }
            finally
            {
                serviceParam = null;
            }
            #endregion
            #region "PayByPhone Service Call"
            /////////////////////////////////////////////////
            TransactionTransform tranDataOfPBPService = new TransactionTransform();
            try
            {
                //////////////////////////////////////////

                logData.Request.CustomerId = param.CustomerId.Value.ToString();
                logData.Request.SpaceId = param.parkingSpaceID;
                logData.Request.MeterId = param.MeterName;
                //////////////////////////////////////////////////
                DateTime? currentCustTime = customerCurrentTime.CurrentTime;
                CustomerElement customer = CustomerRetriever.GetCustomer(param.CustomerId.Value);
                PayByPhoneGenericFactory factory = new PayByPhoneGenericFactory(customer, param.PlateNumber);
                var paybyPhoneResLog = new VendorResponseEntity();
                logData.VendorData.Add(VendorNames.PayByPhone, paybyPhoneResLog);
                tranDataOfService = factory.GetRefreshPlateData(ref paybyPhoneResLog);
                tranDataOfPBPService.EnforcementKey = EnforcementKey;

                ///////////////////////////////////////////////////
                logData.Response = new ResponseEntity() { CustomerId = param.CustomerId.Value.ToString(), MeterId = tranData.MeterName, SpaceId = tranData.SpaceNo, CityCurrentTime = currentCustTime.HasValue ? currentCustTime.Value : DateTime.Now };

            }
            catch (Exception ex)
            {
                logData.Response = new ResponseEntity() { CustomerId = param.CustomerId.Value.ToString(), MeterId = tranData.MeterName, SpaceId = tranData.SpaceNo, CityCurrentTime = customerCurrentTime.CurrentTime.HasValue ? customerCurrentTime.CurrentTime.Value : DateTime.Now };
                //throw ex;
                tranDataOfPBPService.ReturnCode = ((int)ReturnCodeEnum.Error).ToString();
            }
            finally
            {

            }
            #endregion
            //////////////////  //Reconcile data
            List<TransactionTransform> trans = new List<TransactionTransform>();
            trans.Add(tranData); // Database
            trans.Add(tranDataOfService); //Parkeon
            trans.Add(tranDataOfPBPService); //PayByCell

            var rightValue = trans.Where(t => t.ReturnCode == ((int)ReturnCodeEnum.Success).ToString()).OrderByDescending(t => t.MeterExpiredMinutes.ToNullableInt()).FirstOrDefault<TransactionTransform>();
            if (rightValue == default(TransactionTransform))
            {
                //Do nothing.just return database  object.
            }
            else
            {
                tranData = rightValue;
            }

            return tranData;
        }
        private static Transaction GetDataBayHarbor(int customerID, string MeterName, string parkingSpaceID, string PlateNumber, string statename, string EnforcementKey, string filePath, RequestResponseEntity logData)
        {
            string ZoneID = null;
            string SpaceStatus = null;
            Transaction data = new Transaction();
            try
            {
                List<Transaction> transactionList = new List<Transaction>();

                logData.Request.CustomerId = customerID.ToString();
                logData.Request.SpaceId = parkingSpaceID;
                logData.Request.MeterId = MeterName;

                transactionList = DataAccess.GetBayHarborCitationData(customerID, null, null, null, PlateNumber, statename, null);

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
            }
            catch (Exception ex)
            {
                var appPath = System.Web.Hosting.HostingEnvironment.ApplicationPhysicalPath;
                string errorPath = Path.Combine(appPath, "GetData", "Timezones");
                ExceptionLogging.SendErrorToText(ex, "GetDataAtlanta", errorPath);
                data.ReturnCode = ((int)ReturnCodeEnum.Error).ToString();
                throw;
            }
            return data;
        }
        #endregion


        #region North Bay Village
        public static TransactionTransform GetNorthBayVillage(string EnforcementKey, string filePath, RequestResponseEntity logData, CustomerTime customerCurrentTime)
        {
            TransactionTransform tranData = new TransactionTransform();
            tranData.ReturnCode = ((int)ReturnCodeEnum.InvalidInputParameters).ToString();
            TimeZoneInfo TZ = null;
            TZ = TimeZoneInfo.FindSystemTimeZoneById("Eastern Standard Time");
            //int? customerID = null;
            //string MeterName = null;
            //string parkingSpaceID;
            //string PlateNumber = null;
            //string stateName = null;
            //////////////////////////////////////
            KeyParameter param = PrepareKeyParameters.PrepareGenericPayByPlate(EnforcementKey);
            if (param.ReturnCode == ((int)ReturnCodeEnum.InvalidInputParameters).ToString())
            {
                return tranData;
            }

            #region "DB Check"
            /////////////////////////////////////
            try
            {
                Transaction data = TransactionDataValidatioin.GetDataBayHarbor(param.CustomerId.Value, param.MeterName, param.parkingSpaceID, param.PlateNumber, param.StateName, EnforcementKey, filePath, logData);


                if (data.TimeZoneId.HasValue)
                    TZ = Utility.GetTimeZoneInfo(data.TimeZoneId.Value, filePath, param.CustomerId.Value.ToString());

                //For South Miami : PayStationId. Others it is MeterID
                tranData.MeterName = string.Empty; // (data.MeterName == null) ? string.Empty : data.MeterName;
                tranData.SpaceNo = string.Empty;  //data.ParkingSpaceID.ToString();

                tranData.MeterStreet = (data.MeterStreet == null) ? string.Empty : data.MeterStreet;

                DateTime _MeterRTC = data.PresentMeterTime.HasValue ? data.PresentMeterTime.Value : DateTime.Now;
                tranData.MeterRTC = String.Format("{0:yyyyMMdd_THHmmssZ}", Utility.findGMTTime(_MeterRTC, TZ));

                DateTime _SensorEventTime = data.SensorEventTime.HasValue ? data.SensorEventTime.Value : DateTime.Now;
                tranData.MeterLastUpdateTime = String.Format("{0:yyyyMMdd_THHmmssZ}", Utility.findGMTTime(_SensorEventTime, TZ));

                tranData.SensorEventTime = String.Format("{0:yyyyMMdd_THHmmssZ}", Utility.findGMTTime(_SensorEventTime, TZ));

                DateTime _endTime = data.ExpiryTime.HasValue ? data.ExpiryTime.Value : DateTime.Now;
                tranData.ExpiredTime = String.Format("{0:yyyyMMdd_THHmmssZ}", Utility.findGMTTime(_endTime, TZ));

                tranData.MeterExpiredMinutes = data.ExpiredMinutes.HasValue ? data.ExpiredMinutes.Value.ToString() : "0";
                tranData.METERLOC_BLOCKNUMBER = (data.Block == null) ? string.Empty : data.Block;
                tranData.METERLOC_STREETDIRECTION = (data.Direction == null) ? string.Empty : data.Direction;
                tranData.METERLOC_STREETNAME = (data.MeterStreet == null) ? string.Empty : data.MeterStreet;
                tranData.METERLOC_STREETTYPE = (data.StreetType == null) ? string.Empty : data.StreetType;
                tranData.METERLOC_CROSSSTREET1 = (data.CrossStreet1 == null) ? string.Empty : data.CrossStreet1;
                tranData.METERLOC_CROSSSTREET2 = (data.CrossStreet2 == null) ? string.Empty : data.CrossStreet2;
                tranData.METERLOC_ENFORCEMENTHOURSDESC = (data.EnfHourDesc == null) ? string.Empty : data.EnfHourDesc;
                tranData.LicensePlate = (data.PlateNumber == null) ? string.Empty : data.PlateNumber;
                tranData.State = (data.LPLocation == null) ? string.Empty : data.LPLocation;
                tranData.Make = (data.VMake == null) ? string.Empty : data.VMake;
                tranData.Model = (data.VModel == null) ? string.Empty : data.VModel;
                tranData.IsOccupied = Utility.getStatus((data.SpaceStatus == null) ? string.Empty : data.SpaceStatus).ToString();
                tranData.EnforcementKey = (data.EnforcementKey == null) ? string.Empty : data.EnforcementKey;
                tranData.MeterStatus = (data.MeterStatus == null) ? null : data.MeterStatus;

                tranData.METERLOC_METERTYPE = (data.METERLOC_METERTYPE == null) ? null : data.METERLOC_METERTYPE;
                tranData.ZoneName = (data.ZoneName == null) ? null : data.ZoneName;
                tranData.MeterID = String.Empty;
                tranData.ReturnCode = data.ReturnCode;

                logData.Response = new ResponseEntity() { CustomerId = param.CustomerId.Value.ToString(), MeterId = tranData.MeterName, SpaceId = tranData.SpaceNo, CityCurrentTime = data.PresentMeterTime.HasValue ? data.PresentMeterTime.Value : DateTime.Now };
            }
            catch (Exception ex)
            {
                ////Intentionally Error Condition Commented
                ////var appPath = System.Web.Hosting.HostingEnvironment.ApplicationPhysicalPath;
                ////string errorPath = Path.Combine(appPath, "GetTransformData", "Timezones");
                ////ExceptionLogging.SendErrorToText(ex, "GetCitationData", errorPath);
                ////tranData.ReturnCode = ((int)ReturnCodeEnum.Error).ToString();

                tranData.ReturnCode = ((int)ReturnCodeEnum.Error).ToString();
            }

            #endregion
            #region "PayByPhone Service Call"
            /////////////////////////////////////////////////
            TransactionTransform tranDataOfPBPService = new TransactionTransform();
            try
            {
                //////////////////////////////////////////

                logData.Request.CustomerId = param.CustomerId.Value.ToString();
                logData.Request.SpaceId = param.parkingSpaceID;
                logData.Request.MeterId = param.MeterName;
                //////////////////////////////////////////////////
                DateTime? currentCustTime = customerCurrentTime.CurrentTime;
                CustomerElement customer = CustomerRetriever.GetCustomer(param.CustomerId.Value);
                PayByPhoneGenericFactory factory = new PayByPhoneGenericFactory(customer, param.PlateNumber);
                var paybyPhoneResLog = new VendorResponseEntity();
                logData.VendorData.Add(VendorNames.PayByPhone, paybyPhoneResLog);
                tranDataOfPBPService = factory.GetRefreshPlateData(ref paybyPhoneResLog);
                tranDataOfPBPService.EnforcementKey = EnforcementKey;

                ///////////////////////////////////////////////////
                logData.Response = new ResponseEntity() { CustomerId = param.CustomerId.Value.ToString(), MeterId = tranData.MeterName, SpaceId = tranData.SpaceNo, CityCurrentTime = currentCustTime.HasValue ? currentCustTime.Value : DateTime.Now };

            }
            catch (Exception ex)
            {
                logData.Response = new ResponseEntity() { CustomerId = param.CustomerId.Value.ToString(), MeterId = tranData.MeterName, SpaceId = tranData.SpaceNo, CityCurrentTime = customerCurrentTime.CurrentTime.HasValue ? customerCurrentTime.CurrentTime.Value : DateTime.Now };
                //throw ex;
                tranDataOfPBPService.ReturnCode = ((int)ReturnCodeEnum.Error).ToString();
            }
            finally
            {

            }
            #endregion
            //////////////////  //Reconcile data
            List<TransactionTransform> trans = new List<TransactionTransform>();
            trans.Add(tranData); // Database
            trans.Add(tranDataOfPBPService); //PayByCell

            var rightValue = trans.Where(t => t.ReturnCode == ((int)ReturnCodeEnum.Success).ToString()).OrderByDescending(t => t.MeterExpiredMinutes.ToNullableInt()).FirstOrDefault<TransactionTransform>();
            if (rightValue == default(TransactionTransform))
            {
                //Do nothing.just return database  object.
            }
            else
            {
                tranData = rightValue;
            }

            return tranData;
        }
        #endregion

        #region MetroRail
        public static TransactionTransform GetMetroRailTransformData(string EnforcementKey, string filePath, RequestResponseEntity logData, CustomerTime customerCurrentTime)
        {
            TransactionTransform tranData = new TransactionTransform();
            tranData.ReturnCode = ((int)ReturnCodeEnum.InvalidInputParameters).ToString();
            TimeZoneInfo TZ = null;
            TZ = TimeZoneInfo.FindSystemTimeZoneById("Eastern Standard Time");
            //int? customerID = null;
            //string MeterName = null;
            //string parkingSpaceID;
            //string PlateNumber = null;
            //string stateName = null;
            //////////////////////////////////////
            KeyParameter param = PrepareKeyParameters.PrepareMetroRail(EnforcementKey);
            if (param.ReturnCode == ((int)ReturnCodeEnum.InvalidInputParameters).ToString())
            {
                return tranData;
            }

            #region "DB Check"
            /////////////////////////////////////
            try
            {
                Transaction data = TransactionDataValidatioin.GetDataMetroRail(param.CustomerId.Value, param.MeterName, param.parkingSpaceID, param.PlateNumber, param.StateName, EnforcementKey, filePath, logData);


                if (data.TimeZoneId.HasValue)
                    TZ = Utility.GetTimeZoneInfo(data.TimeZoneId.Value, filePath, param.CustomerId.Value.ToString());

                //For South Miami : PayStationId. Others it is MeterID
                tranData.MeterName = string.Empty; // (data.MeterName == null) ? string.Empty : data.MeterName;
                tranData.SpaceNo = string.Empty;  //data.ParkingSpaceID.ToString();

                tranData.MeterStreet = (data.MeterStreet == null) ? string.Empty : data.MeterStreet;

                DateTime _MeterRTC = data.PresentMeterTime.HasValue ? data.PresentMeterTime.Value : DateTime.Now;
                tranData.MeterRTC = String.Format("{0:yyyyMMdd_THHmmssZ}", Utility.findGMTTime(_MeterRTC, TZ));

                DateTime _SensorEventTime = data.SensorEventTime.HasValue ? data.SensorEventTime.Value : DateTime.Now;
                tranData.MeterLastUpdateTime = String.Format("{0:yyyyMMdd_THHmmssZ}", Utility.findGMTTime(_SensorEventTime, TZ));

                tranData.SensorEventTime = String.Format("{0:yyyyMMdd_THHmmssZ}", Utility.findGMTTime(_SensorEventTime, TZ));

                DateTime _endTime = data.ExpiryTime.HasValue ? data.ExpiryTime.Value : DateTime.Now;
                tranData.ExpiredTime = String.Format("{0:yyyyMMdd_THHmmssZ}", Utility.findGMTTime(_endTime, TZ));

                tranData.MeterExpiredMinutes = data.ExpiredMinutes.HasValue ? data.ExpiredMinutes.Value.ToString() : "0";
                tranData.METERLOC_BLOCKNUMBER = (data.Block == null) ? string.Empty : data.Block;
                tranData.METERLOC_STREETDIRECTION = (data.Direction == null) ? string.Empty : data.Direction;
                tranData.METERLOC_STREETNAME = (data.MeterStreet == null) ? string.Empty : data.MeterStreet;
                tranData.METERLOC_STREETTYPE = (data.StreetType == null) ? string.Empty : data.StreetType;
                tranData.METERLOC_CROSSSTREET1 = (data.CrossStreet1 == null) ? string.Empty : data.CrossStreet1;
                tranData.METERLOC_CROSSSTREET2 = (data.CrossStreet2 == null) ? string.Empty : data.CrossStreet2;
                tranData.METERLOC_ENFORCEMENTHOURSDESC = (data.EnfHourDesc == null) ? string.Empty : data.EnfHourDesc;
                tranData.LicensePlate = (data.PlateNumber == null) ? string.Empty : data.PlateNumber;
                tranData.State = (data.LPLocation == null) ? string.Empty : data.LPLocation;
                tranData.Make = (data.VMake == null) ? string.Empty : data.VMake;
                tranData.Model = (data.VModel == null) ? string.Empty : data.VModel;
                tranData.IsOccupied = Utility.getStatus((data.SpaceStatus == null) ? string.Empty : data.SpaceStatus).ToString();
                tranData.EnforcementKey = (data.EnforcementKey == null) ? string.Empty : data.EnforcementKey;
                tranData.MeterStatus = (data.MeterStatus == null) ? null : data.MeterStatus;

                tranData.METERLOC_METERTYPE = (data.METERLOC_METERTYPE == null) ? null : data.METERLOC_METERTYPE;
                tranData.ZoneName = (data.ZoneName == null) ? null : data.ZoneName;
                tranData.MeterID = String.Empty;
                tranData.ReturnCode = data.ReturnCode;

                logData.Response = new ResponseEntity() { CustomerId = param.CustomerId.Value.ToString(), MeterId = tranData.MeterName, SpaceId = tranData.SpaceNo, CityCurrentTime = data.PresentMeterTime.HasValue ? data.PresentMeterTime.Value : DateTime.Now };
            }
            catch (Exception ex)
            {
                ////Intentionally Error Condition Commented
                ////var appPath = System.Web.Hosting.HostingEnvironment.ApplicationPhysicalPath;
                ////string errorPath = Path.Combine(appPath, "GetTransformData", "Timezones");
                ////ExceptionLogging.SendErrorToText(ex, "GetCitationData", errorPath);
                ////tranData.ReturnCode = ((int)ReturnCodeEnum.Error).ToString();

                tranData.ReturnCode = ((int)ReturnCodeEnum.Error).ToString();
            }

            #endregion
            #region "PayByPhone Service Call"
            /////////////////////////////////////////////////
            TransactionTransform tranDataOfPBPService = new TransactionTransform();
            try
            {
                //////////////////////////////////////////

                logData.Request.CustomerId = param.CustomerId.Value.ToString();
                logData.Request.SpaceId = param.parkingSpaceID;
                logData.Request.MeterId = param.MeterName;
                //////////////////////////////////////////////////
                DateTime? currentCustTime = customerCurrentTime.CurrentTime;
                CustomerElement customer = CustomerRetriever.GetCustomer(param.CustomerId.Value);
                PayByPhoneGenericFactory factory = new PayByPhoneGenericFactory(customer, param.PlateNumber);
                tranDataOfPBPService = factory.GetRefreshPlateData();
                tranDataOfPBPService.EnforcementKey = EnforcementKey;

                ///////////////////////////////////////////////////
                logData.Response = new ResponseEntity() { CustomerId = param.CustomerId.Value.ToString(), MeterId = tranData.MeterName, SpaceId = tranData.SpaceNo, CityCurrentTime = currentCustTime.HasValue ? currentCustTime.Value : DateTime.Now };

            }
            catch (Exception ex)
            {
                logData.Response = new ResponseEntity() { CustomerId = param.CustomerId.Value.ToString(), MeterId = tranData.MeterName, SpaceId = tranData.SpaceNo, CityCurrentTime = customerCurrentTime.CurrentTime.HasValue ? customerCurrentTime.CurrentTime.Value : DateTime.Now };
                //throw ex;
                tranDataOfPBPService.ReturnCode = ((int)ReturnCodeEnum.Error).ToString();
            }
            finally
            {

            }
            #endregion
            //////////////////  //Reconcile data
            List<TransactionTransform> trans = new List<TransactionTransform>();
            trans.Add(tranData); // Database
            trans.Add(tranDataOfPBPService); //PayByCell

            var rightValue = trans.Where(t => t.ReturnCode == ((int)ReturnCodeEnum.Success).ToString()).OrderByDescending(t => t.MeterExpiredMinutes.ToNullableInt()).FirstOrDefault<TransactionTransform>();
            if (rightValue == default(TransactionTransform))
            {
                //Do nothing.just return database  object.
            }
            else
            {
                tranData = rightValue;
            }

            return tranData;
        }
        public static TransactionTransform GetMetroRailTransformDataV2(string EnforcementKey, string filePath, RequestResponseEntity logData, CustomerTime customerCurrentTime)
        {
            TransactionTransform tranData = new TransactionTransform();
            tranData.ReturnCode = ((int)ReturnCodeEnum.InvalidInputParameters).ToString();
            TimeZoneInfo TZ = null;
            TZ = TimeZoneInfo.FindSystemTimeZoneById("Eastern Standard Time");
            //int? customerID = null;
            //string MeterName = null;
            //string parkingSpaceID;
            //string PlateNumber = null;
            //string stateName = null;
            //////////////////////////////////////
            KeyParameter param = PrepareKeyParameters.PrepareMetroRail(EnforcementKey);
            if (param.ReturnCode == ((int)ReturnCodeEnum.InvalidInputParameters).ToString())
            {
                return tranData;
            }

            #region "DB Check"
            /////////////////////////////////////
            try
            {
                Transaction data = TransactionDataValidatioin.GetDataMetroRail(param.CustomerId.Value, param.MeterName, param.parkingSpaceID, param.PlateNumber, param.StateName, EnforcementKey, filePath, logData);


                if (data.TimeZoneId.HasValue)
                    TZ = Utility.GetTimeZoneInfo(data.TimeZoneId.Value, filePath, param.CustomerId.Value.ToString());

                //For South Miami : PayStationId. Others it is MeterID
                tranData.MeterName = string.Empty; // (data.MeterName == null) ? string.Empty : data.MeterName;
                tranData.SpaceNo = string.Empty;  //data.ParkingSpaceID.ToString();

                tranData.MeterStreet = (data.MeterStreet == null) ? string.Empty : data.MeterStreet;

                DateTime _MeterRTC = data.PresentMeterTime.HasValue ? data.PresentMeterTime.Value : DateTime.Now;
                tranData.MeterRTC = String.Format("{0:yyyyMMdd_THHmmssZ}", Utility.findGMTTime(_MeterRTC, TZ));

                DateTime _SensorEventTime = data.SensorEventTime.HasValue ? data.SensorEventTime.Value : DateTime.Now;
                tranData.MeterLastUpdateTime = String.Format("{0:yyyyMMdd_THHmmssZ}", Utility.findGMTTime(_SensorEventTime, TZ));

                tranData.SensorEventTime = String.Format("{0:yyyyMMdd_THHmmssZ}", Utility.findGMTTime(_SensorEventTime, TZ));

                DateTime _endTime = data.ExpiryTime.HasValue ? data.ExpiryTime.Value : DateTime.Now;
                tranData.ExpiredTime = String.Format("{0:yyyyMMdd_THHmmssZ}", Utility.findGMTTime(_endTime, TZ));

                tranData.MeterExpiredMinutes = data.ExpiredMinutes.HasValue ? data.ExpiredMinutes.Value.ToString() : "0";
                tranData.METERLOC_BLOCKNUMBER = (data.Block == null) ? string.Empty : data.Block;
                tranData.METERLOC_STREETDIRECTION = (data.Direction == null) ? string.Empty : data.Direction;
                tranData.METERLOC_STREETNAME = (data.MeterStreet == null) ? string.Empty : data.MeterStreet;
                tranData.METERLOC_STREETTYPE = (data.StreetType == null) ? string.Empty : data.StreetType;
                tranData.METERLOC_CROSSSTREET1 = (data.CrossStreet1 == null) ? string.Empty : data.CrossStreet1;
                tranData.METERLOC_CROSSSTREET2 = (data.CrossStreet2 == null) ? string.Empty : data.CrossStreet2;
                tranData.METERLOC_ENFORCEMENTHOURSDESC = (data.EnfHourDesc == null) ? string.Empty : data.EnfHourDesc;
                tranData.LicensePlate = (data.PlateNumber == null) ? string.Empty : data.PlateNumber;
                tranData.State = (data.LPLocation == null) ? string.Empty : data.LPLocation;
                tranData.Make = (data.VMake == null) ? string.Empty : data.VMake;
                tranData.Model = (data.VModel == null) ? string.Empty : data.VModel;
                tranData.IsOccupied = Utility.getStatus((data.SpaceStatus == null) ? string.Empty : data.SpaceStatus).ToString();
                tranData.EnforcementKey = (data.EnforcementKey == null) ? string.Empty : data.EnforcementKey;
                tranData.MeterStatus = (data.MeterStatus == null) ? null : data.MeterStatus;

                tranData.METERLOC_METERTYPE = (data.METERLOC_METERTYPE == null) ? null : data.METERLOC_METERTYPE;
                tranData.ZoneName = (data.ZoneName == null) ? null : data.ZoneName;
                tranData.MeterID = String.Empty;
                tranData.ReturnCode = data.ReturnCode;

                logData.Response = new ResponseEntity() { CustomerId = param.CustomerId.Value.ToString(), MeterId = tranData.MeterName, SpaceId = tranData.SpaceNo, CityCurrentTime = data.PresentMeterTime.HasValue ? data.PresentMeterTime.Value : DateTime.Now };
            }
            catch (Exception ex)
            {
                ////Intentionally Error Condition Commented
                ////var appPath = System.Web.Hosting.HostingEnvironment.ApplicationPhysicalPath;
                ////string errorPath = Path.Combine(appPath, "GetTransformData", "Timezones");
                ////ExceptionLogging.SendErrorToText(ex, "GetCitationData", errorPath);
                ////tranData.ReturnCode = ((int)ReturnCodeEnum.Error).ToString();

                tranData.ReturnCode = ((int)ReturnCodeEnum.Error).ToString();
            }

            #endregion
            #region "PayByPhone Service Call"
            /////////////////////////////////////////////////
            TransactionTransform tranDataOfPBPService = new TransactionTransform();
            try
            {
                //////////////////////////////////////////

                logData.Request.CustomerId = param.CustomerId.Value.ToString();
                logData.Request.SpaceId = param.parkingSpaceID;
                logData.Request.MeterId = param.MeterName;
                //////////////////////////////////////////////////
                DateTime? currentCustTime = customerCurrentTime.CurrentTime;
                CustomerElement customer = CustomerRetriever.GetCustomer(param.CustomerId.Value);
                PayByPhoneGenericFactory factory = new PayByPhoneGenericFactory(customer, param.PlateNumber);
                var paybyPhoneResLog = new VendorResponseEntity();
                logData.VendorData.Add(VendorNames.PayByPhone, paybyPhoneResLog);
                tranDataOfPBPService = factory.GetRefreshPlateData(ref paybyPhoneResLog);
                tranDataOfPBPService.EnforcementKey = EnforcementKey;

                ///////////////////////////////////////////////////
                logData.Response = new ResponseEntity() { CustomerId = param.CustomerId.Value.ToString(), MeterId = tranData.MeterName, SpaceId = tranData.SpaceNo, CityCurrentTime = currentCustTime.HasValue ? currentCustTime.Value : DateTime.Now };

            }
            catch (Exception ex)
            {
                logData.Response = new ResponseEntity() { CustomerId = param.CustomerId.Value.ToString(), MeterId = tranData.MeterName, SpaceId = tranData.SpaceNo, CityCurrentTime = customerCurrentTime.CurrentTime.HasValue ? customerCurrentTime.CurrentTime.Value : DateTime.Now };
                //throw ex;
                tranDataOfPBPService.ReturnCode = ((int)ReturnCodeEnum.Error).ToString();
            }
            finally
            {

            }
            #endregion
            //////////////////  //Reconcile data
            List<TransactionTransform> trans = new List<TransactionTransform>();
            trans.Add(tranData); // Database
            trans.Add(tranDataOfPBPService); //PayByCell

            var rightValue = trans.Where(t => t.ReturnCode == ((int)ReturnCodeEnum.Success).ToString()).OrderByDescending(t => t.MeterExpiredMinutes.ToNullableInt()).FirstOrDefault<TransactionTransform>();
            if (rightValue == default(TransactionTransform))
            {
                //Do nothing.just return database  object.
            }
            else
            {
                tranData = rightValue;
            }

            return tranData;
        }
        private static Transaction GetDataMetroRail(int customerID, string MeterName, string parkingSpaceID, string PlateNumber, string statename, string EnforcementKey, string filePath, RequestResponseEntity logData)
        {
            string ZoneID = null;
            string SpaceStatus = null;
            Transaction data = new Transaction();
            try
            {
                List<Transaction> transactionList = new List<Transaction>();

                logData.Request.CustomerId = customerID.ToString();
                logData.Request.SpaceId = parkingSpaceID;
                logData.Request.MeterId = MeterName;

                transactionList = DataAccess.GetMetroRailCitationData(customerID, null, null, null, PlateNumber, statename, null);

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
            }
            catch (Exception ex)
            {
                var appPath = System.Web.Hosting.HostingEnvironment.ApplicationPhysicalPath;
                string errorPath = Path.Combine(appPath, "GetData", "Timezones");
                ExceptionLogging.SendErrorToText(ex, "GetDataAtlanta", errorPath);
                data.ReturnCode = ((int)ReturnCodeEnum.Error).ToString();
                throw;
            }
            return data;
        }
        #endregion

        #region PortHoodRiver
        public static TransactionTransform GetPortHoodRiverTransformData(string EnforcementKey, string filePath, RequestResponseEntity logData, CustomerTime customerCurrentTime)
        {
            TransactionTransform tranData = new TransactionTransform();
            tranData.ReturnCode = ((int)ReturnCodeEnum.InvalidInputParameters).ToString();
            TimeZoneInfo TZ = null;
            TZ = TimeZoneInfo.FindSystemTimeZoneById("Eastern Standard Time");
            //int? customerID = null;
            //string MeterName = null;
            //string parkingSpaceID;
            //string PlateNumber = null;
            //string stateName = null;
            //////////////////////////////////////
            KeyParameter param = PrepareKeyParameters.PreparePortHoodRiver(EnforcementKey);
            if (param.ReturnCode == ((int)ReturnCodeEnum.InvalidInputParameters).ToString())
            {
                return tranData;
            }
            /////////////////////////////////////
            #region "DB Check"
            try
            {
                Transaction data = TransactionDataValidatioin.GetDataPortHoodRiver(param.CustomerId.Value, param.MeterName, param.parkingSpaceID, param.PlateNumber, param.StateName, EnforcementKey, filePath, logData);


                if (data.TimeZoneId.HasValue)
                    TZ = Utility.GetTimeZoneInfo(data.TimeZoneId.Value, filePath, param.CustomerId.Value.ToString());

                //For South Miami : PayStationId. Others it is MeterID
                tranData.MeterName = string.Empty; // (data.MeterName == null) ? string.Empty : data.MeterName;
                tranData.SpaceNo = string.Empty;  //data.ParkingSpaceID.ToString();

                tranData.MeterStreet = (data.MeterStreet == null) ? string.Empty : data.MeterStreet;

                DateTime _MeterRTC = data.PresentMeterTime.HasValue ? data.PresentMeterTime.Value : DateTime.Now;
                tranData.MeterRTC = String.Format("{0:yyyyMMdd_THHmmssZ}", Utility.findGMTTime(_MeterRTC, TZ));

                DateTime _SensorEventTime = data.SensorEventTime.HasValue ? data.SensorEventTime.Value : DateTime.Now;
                tranData.MeterLastUpdateTime = String.Format("{0:yyyyMMdd_THHmmssZ}", Utility.findGMTTime(_SensorEventTime, TZ));

                tranData.SensorEventTime = String.Format("{0:yyyyMMdd_THHmmssZ}", Utility.findGMTTime(_SensorEventTime, TZ));

                DateTime _endTime = data.ExpiryTime.HasValue ? data.ExpiryTime.Value : DateTime.Now;
                tranData.ExpiredTime = String.Format("{0:yyyyMMdd_THHmmssZ}", Utility.findGMTTime(_endTime, TZ));

                tranData.MeterExpiredMinutes = data.ExpiredMinutes.HasValue ? data.ExpiredMinutes.Value.ToString() : "0";
                tranData.METERLOC_BLOCKNUMBER = (data.Block == null) ? string.Empty : data.Block;
                tranData.METERLOC_STREETDIRECTION = (data.Direction == null) ? string.Empty : data.Direction;
                tranData.METERLOC_STREETNAME = (data.MeterStreet == null) ? string.Empty : data.MeterStreet;
                tranData.METERLOC_STREETTYPE = (data.StreetType == null) ? string.Empty : data.StreetType;
                tranData.METERLOC_CROSSSTREET1 = (data.CrossStreet1 == null) ? string.Empty : data.CrossStreet1;
                tranData.METERLOC_CROSSSTREET2 = (data.CrossStreet2 == null) ? string.Empty : data.CrossStreet2;
                tranData.METERLOC_ENFORCEMENTHOURSDESC = (data.EnfHourDesc == null) ? string.Empty : data.EnfHourDesc;
                tranData.LicensePlate = (data.PlateNumber == null) ? string.Empty : data.PlateNumber;
                tranData.State = (data.LPLocation == null) ? string.Empty : data.LPLocation;
                tranData.Make = (data.VMake == null) ? string.Empty : data.VMake;
                tranData.Model = (data.VModel == null) ? string.Empty : data.VModel;
                tranData.IsOccupied = Utility.getStatus((data.SpaceStatus == null) ? string.Empty : data.SpaceStatus).ToString();
                tranData.EnforcementKey = (data.EnforcementKey == null) ? string.Empty : data.EnforcementKey;
                tranData.MeterStatus = (data.MeterStatus == null) ? null : data.MeterStatus;

                tranData.METERLOC_METERTYPE = (data.METERLOC_METERTYPE == null) ? null : data.METERLOC_METERTYPE;
                tranData.ZoneName = (data.ZoneName == null) ? null : data.ZoneName;
                tranData.MeterID = String.Empty;
                tranData.ReturnCode = data.ReturnCode;

                logData.Response = new ResponseEntity() { CustomerId = param.CustomerId.Value.ToString(), MeterId = tranData.MeterName, SpaceId = tranData.SpaceNo, CityCurrentTime = data.PresentMeterTime.HasValue ? data.PresentMeterTime.Value : DateTime.Now };
            }
            catch (Exception ex)
            {
                //var appPath = System.Web.Hosting.HostingEnvironment.ApplicationPhysicalPath;
                //string errorPath = Path.Combine(appPath, "GetTransformData", "Timezones");
                //ExceptionLogging.SendErrorToText(ex, "GetCitationData", errorPath);
                //tranData.ReturnCode = ((int)ReturnCodeEnum.Error).ToString();

                tranData.ReturnCode = ((int)ReturnCodeEnum.Error).ToString();
            }
            #endregion
            #region "Cale Service Call"
            /////////////////////////////////////////////////
            TransactionTransform tranDataOfService = new TransactionTransform();
            try
            {
                //////////////////////////////////////////
                logData.Request.CustomerId = param.CustomerId.Value.ToString();
                logData.Request.SpaceId = param.parkingSpaceID;
                logData.Request.MeterId = param.MeterName;
                //////////////////////////////////////////////////
                DateTime? currentCustTime = customerCurrentTime.CurrentTime;
                Cale.CaleCustomerElement customer = Cale.CaleCustomerRetriever.GetCustomer(param.CustomerId.Value);
                Cale.CaleGenericFactory factory = new Cale.CaleGenericFactory(customer, param.PlateNumber, null);
                if (customer.EnforcementType == EnumEnforcementType.PayByPlate.ToString())
                    tranDataOfService = factory.GetRefreshPlateData();
                tranDataOfService.EnforcementKey = EnforcementKey;

                ///////////////////////////////////////////////////
                logData.Response = new ResponseEntity() { CustomerId = param.CustomerId.Value.ToString(), MeterId = tranData.MeterName, SpaceId = tranData.SpaceNo, CityCurrentTime = currentCustTime.HasValue ? currentCustTime.Value : DateTime.Now };
            }
            catch (Exception ex)
            {
                logData.Response = new ResponseEntity() { CustomerId = param.CustomerId.Value.ToString(), MeterId = tranData.MeterName, SpaceId = tranData.SpaceNo, CityCurrentTime = customerCurrentTime.CurrentTime.HasValue ? customerCurrentTime.CurrentTime.Value : DateTime.Now };
                //throw ex;
                tranData.ReturnCode = ((int)ReturnCodeEnum.Error).ToString();
            }
            finally
            {

            }
            #endregion

            //Reconcile data
            if ((tranData.ReturnCode == ((int)ReturnCodeEnum.NoRecordExists).ToString()) || (tranData.ReturnCode == ((int)ReturnCodeEnum.Error).ToString()))
            {
                tranData = tranDataOfService;
            }
            else if (tranData.ReturnCode == ((int)ReturnCodeEnum.Success).ToString())
            {
                if (tranDataOfService.ReturnCode == ((int)ReturnCodeEnum.Success).ToString())
                {
                    ////Select one record of highest expiry time
                    int? dbExpiredMinutes = tranData.MeterExpiredMinutes.ToNullableInt();
                    int? parkeonExpiredMinutes = tranDataOfService.MeterExpiredMinutes.ToNullableInt();
                    tranData = Nullable.Compare(dbExpiredMinutes, parkeonExpiredMinutes) > 0 ? tranData : tranDataOfService;
                }
            }

            else if (tranData.ReturnCode == ((int)ReturnCodeEnum.MoreRecordsExists).ToString())
            {
                // DB never give more record. hence ignore
            }

            return tranData;
        }
        public static TransactionTransform GetPortHoodRiverTransformDataV2(string EnforcementKey, string filePath, RequestResponseEntity logData, CustomerTime customerCurrentTime)
        {
            TransactionTransform tranData = new TransactionTransform();
            tranData.ReturnCode = ((int)ReturnCodeEnum.InvalidInputParameters).ToString();
            TimeZoneInfo TZ = null;
            TZ = TimeZoneInfo.FindSystemTimeZoneById("Eastern Standard Time");
            //int? customerID = null;
            //string MeterName = null;
            //string parkingSpaceID;
            //string PlateNumber = null;
            //string stateName = null;
            //////////////////////////////////////
            KeyParameter param = PrepareKeyParameters.PreparePortHoodRiver(EnforcementKey);
            if (param.ReturnCode == ((int)ReturnCodeEnum.InvalidInputParameters).ToString())
            {
                return tranData;
            }
            /////////////////////////////////////
            #region "DB Check"
            try
            {
                Transaction data = TransactionDataValidatioin.GetDataPortHoodRiver(param.CustomerId.Value, param.MeterName, param.parkingSpaceID, param.PlateNumber, param.StateName, EnforcementKey, filePath, logData);


                if (data.TimeZoneId.HasValue)
                    TZ = Utility.GetTimeZoneInfo(data.TimeZoneId.Value, filePath, param.CustomerId.Value.ToString());

                //For South Miami : PayStationId. Others it is MeterID
                tranData.MeterName = string.Empty; // (data.MeterName == null) ? string.Empty : data.MeterName;
                tranData.SpaceNo = string.Empty;  //data.ParkingSpaceID.ToString();

                tranData.MeterStreet = (data.MeterStreet == null) ? string.Empty : data.MeterStreet;

                DateTime _MeterRTC = data.PresentMeterTime.HasValue ? data.PresentMeterTime.Value : DateTime.Now;
                tranData.MeterRTC = String.Format("{0:yyyyMMdd_THHmmssZ}", Utility.findGMTTime(_MeterRTC, TZ));

                DateTime _SensorEventTime = data.SensorEventTime.HasValue ? data.SensorEventTime.Value : DateTime.Now;
                tranData.MeterLastUpdateTime = String.Format("{0:yyyyMMdd_THHmmssZ}", Utility.findGMTTime(_SensorEventTime, TZ));

                tranData.SensorEventTime = String.Format("{0:yyyyMMdd_THHmmssZ}", Utility.findGMTTime(_SensorEventTime, TZ));

                DateTime _endTime = data.ExpiryTime.HasValue ? data.ExpiryTime.Value : DateTime.Now;
                tranData.ExpiredTime = String.Format("{0:yyyyMMdd_THHmmssZ}", Utility.findGMTTime(_endTime, TZ));

                tranData.MeterExpiredMinutes = data.ExpiredMinutes.HasValue ? data.ExpiredMinutes.Value.ToString() : "0";
                tranData.METERLOC_BLOCKNUMBER = (data.Block == null) ? string.Empty : data.Block;
                tranData.METERLOC_STREETDIRECTION = (data.Direction == null) ? string.Empty : data.Direction;
                tranData.METERLOC_STREETNAME = (data.MeterStreet == null) ? string.Empty : data.MeterStreet;
                tranData.METERLOC_STREETTYPE = (data.StreetType == null) ? string.Empty : data.StreetType;
                tranData.METERLOC_CROSSSTREET1 = (data.CrossStreet1 == null) ? string.Empty : data.CrossStreet1;
                tranData.METERLOC_CROSSSTREET2 = (data.CrossStreet2 == null) ? string.Empty : data.CrossStreet2;
                tranData.METERLOC_ENFORCEMENTHOURSDESC = (data.EnfHourDesc == null) ? string.Empty : data.EnfHourDesc;
                tranData.LicensePlate = (data.PlateNumber == null) ? string.Empty : data.PlateNumber;
                tranData.State = (data.LPLocation == null) ? string.Empty : data.LPLocation;
                tranData.Make = (data.VMake == null) ? string.Empty : data.VMake;
                tranData.Model = (data.VModel == null) ? string.Empty : data.VModel;
                tranData.IsOccupied = Utility.getStatus((data.SpaceStatus == null) ? string.Empty : data.SpaceStatus).ToString();
                tranData.EnforcementKey = (data.EnforcementKey == null) ? string.Empty : data.EnforcementKey;
                tranData.MeterStatus = (data.MeterStatus == null) ? null : data.MeterStatus;

                tranData.METERLOC_METERTYPE = (data.METERLOC_METERTYPE == null) ? null : data.METERLOC_METERTYPE;
                tranData.ZoneName = (data.ZoneName == null) ? null : data.ZoneName;
                tranData.MeterID = String.Empty;
                tranData.ReturnCode = data.ReturnCode;

                logData.Response = new ResponseEntity() { CustomerId = param.CustomerId.Value.ToString(), MeterId = tranData.MeterName, SpaceId = tranData.SpaceNo, CityCurrentTime = data.PresentMeterTime.HasValue ? data.PresentMeterTime.Value : DateTime.Now };
            }
            catch (Exception ex)
            {
                //var appPath = System.Web.Hosting.HostingEnvironment.ApplicationPhysicalPath;
                //string errorPath = Path.Combine(appPath, "GetTransformData", "Timezones");
                //ExceptionLogging.SendErrorToText(ex, "GetCitationData", errorPath);
                //tranData.ReturnCode = ((int)ReturnCodeEnum.Error).ToString();

                tranData.ReturnCode = ((int)ReturnCodeEnum.Error).ToString();
            }
            #endregion
            #region "Cale Service Call"
            /////////////////////////////////////////////////
            TransactionTransform tranDataOfService = new TransactionTransform();
            try
            {
                //////////////////////////////////////////
                logData.Request.CustomerId = param.CustomerId.Value.ToString();
                logData.Request.SpaceId = param.parkingSpaceID;
                logData.Request.MeterId = param.MeterName;
                //////////////////////////////////////////////////
                DateTime? currentCustTime = customerCurrentTime.CurrentTime;
                Cale.CaleCustomerElement customer = Cale.CaleCustomerRetriever.GetCustomer(param.CustomerId.Value);
                Cale.CaleGenericFactory factory = new Cale.CaleGenericFactory(customer, param.PlateNumber, null);
                var caleResLog = new VendorResponseEntity();
                logData.VendorData.Add(VendorNames.Cale, caleResLog);
                if (customer.EnforcementType == EnumEnforcementType.PayByPlate.ToString())
                    tranDataOfService = factory.GetRefreshPlateData(ref caleResLog);
                //tranDataOfService = factory.GetRefreshPlateData();
                tranDataOfService.EnforcementKey = EnforcementKey;

                ///////////////////////////////////////////////////
                logData.Response = new ResponseEntity() { CustomerId = param.CustomerId.Value.ToString(), MeterId = tranData.MeterName, SpaceId = tranData.SpaceNo, CityCurrentTime = currentCustTime.HasValue ? currentCustTime.Value : DateTime.Now };
            }
            catch (Exception ex)
            {
                logData.Response = new ResponseEntity() { CustomerId = param.CustomerId.Value.ToString(), MeterId = tranData.MeterName, SpaceId = tranData.SpaceNo, CityCurrentTime = customerCurrentTime.CurrentTime.HasValue ? customerCurrentTime.CurrentTime.Value : DateTime.Now };
                //throw ex;
                tranData.ReturnCode = ((int)ReturnCodeEnum.Error).ToString();
            }
            finally
            {

            }
            #endregion

            //Reconcile data
            if ((tranData.ReturnCode == ((int)ReturnCodeEnum.NoRecordExists).ToString()) || (tranData.ReturnCode == ((int)ReturnCodeEnum.Error).ToString()))
            {
                tranData = tranDataOfService;
            }
            else if (tranData.ReturnCode == ((int)ReturnCodeEnum.Success).ToString())
            {
                if (tranDataOfService.ReturnCode == ((int)ReturnCodeEnum.Success).ToString())
                {
                    ////Select one record of highest expiry time
                    int? dbExpiredMinutes = tranData.MeterExpiredMinutes.ToNullableInt();
                    int? parkeonExpiredMinutes = tranDataOfService.MeterExpiredMinutes.ToNullableInt();
                    tranData = Nullable.Compare(dbExpiredMinutes, parkeonExpiredMinutes) > 0 ? tranData : tranDataOfService;
                }
            }

            else if (tranData.ReturnCode == ((int)ReturnCodeEnum.MoreRecordsExists).ToString())
            {
                // DB never give more record. hence ignore
            }

            return tranData;
        }
        private static Transaction GetDataPortHoodRiver(int customerID, string MeterName, string parkingSpaceID, string PlateNumber, string statename, string EnforcementKey, string filePath, RequestResponseEntity logData)
        {
            string ZoneID = null;
            string SpaceStatus = null;
            Transaction data = new Transaction();
            try
            {
                List<Transaction> transactionList = new List<Transaction>();

                logData.Request.CustomerId = customerID.ToString();
                logData.Request.SpaceId = parkingSpaceID;
                logData.Request.MeterId = MeterName;

                transactionList = DataAccess.GetPortHoodRiverWithPlateName(customerID, null, null, null, PlateNumber, statename, null);

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
            }
            catch (Exception ex)
            {
                var appPath = System.Web.Hosting.HostingEnvironment.ApplicationPhysicalPath;
                string errorPath = Path.Combine(appPath, "GetData", "Timezones");
                ExceptionLogging.SendErrorToText(ex, "GetCitationData", errorPath);
                data.ReturnCode = ((int)ReturnCodeEnum.Error).ToString();
                throw;
            }
            return data;
        }
        #endregion

        #region Tybee Island
        public static TransactionTransform GetTybeeIslandTransformData(string EnforcementKey, string filePath, RequestResponseEntity logData, CustomerTime customerCurrentTime)
        {
            TransactionTransform tranData = new TransactionTransform();
            tranData.ReturnCode = ((int)ReturnCodeEnum.InvalidInputParameters).ToString();
            TimeZoneInfo TZ = null;
            TZ = TimeZoneInfo.FindSystemTimeZoneById("Eastern Standard Time");
            //int? customerID = null;
            //string MeterName = null;
            //string parkingSpaceID;
            //string PlateNumber = null;
            //string stateName = null;
            //////////////////////////////////////
            KeyParameter param = PrepareKeyParameters.PrepareTybeeIsland(EnforcementKey);
            if (param.ReturnCode == ((int)ReturnCodeEnum.InvalidInputParameters).ToString())
            {
                return tranData;
            }
            /////////////////////////////////////
            #region "DB Check"
            try
            {
                Transaction data = TransactionDataValidatioin.GetDataTybeeIsland(param.CustomerId.Value, param.MeterName, param.parkingSpaceID, param.PlateNumber, param.StateName, EnforcementKey, filePath, logData);


                if (data.TimeZoneId.HasValue)
                    TZ = Utility.GetTimeZoneInfo(data.TimeZoneId.Value, filePath, param.CustomerId.Value.ToString());

                //For South Miami : PayStationId. Others it is MeterID
                tranData.MeterName = string.Empty; // (data.MeterName == null) ? string.Empty : data.MeterName;
                tranData.SpaceNo = string.Empty;  //data.ParkingSpaceID.ToString();

                tranData.MeterStreet = (data.MeterStreet == null) ? string.Empty : data.MeterStreet;

                DateTime _MeterRTC = data.PresentMeterTime.HasValue ? data.PresentMeterTime.Value : DateTime.Now;
                tranData.MeterRTC = String.Format("{0:yyyyMMdd_THHmmssZ}", Utility.findGMTTime(_MeterRTC, TZ));

                DateTime _SensorEventTime = data.SensorEventTime.HasValue ? data.SensorEventTime.Value : DateTime.Now;
                tranData.MeterLastUpdateTime = String.Format("{0:yyyyMMdd_THHmmssZ}", Utility.findGMTTime(_SensorEventTime, TZ));

                tranData.SensorEventTime = String.Format("{0:yyyyMMdd_THHmmssZ}", Utility.findGMTTime(_SensorEventTime, TZ));

                DateTime _endTime = data.ExpiryTime.HasValue ? data.ExpiryTime.Value : DateTime.Now;
                tranData.ExpiredTime = String.Format("{0:yyyyMMdd_THHmmssZ}", Utility.findGMTTime(_endTime, TZ));

                tranData.MeterExpiredMinutes = data.ExpiredMinutes.HasValue ? data.ExpiredMinutes.Value.ToString() : "0";
                tranData.METERLOC_BLOCKNUMBER = (data.Block == null) ? string.Empty : data.Block;
                tranData.METERLOC_STREETDIRECTION = (data.Direction == null) ? string.Empty : data.Direction;
                tranData.METERLOC_STREETNAME = (data.MeterStreet == null) ? string.Empty : data.MeterStreet;
                tranData.METERLOC_STREETTYPE = (data.StreetType == null) ? string.Empty : data.StreetType;
                tranData.METERLOC_CROSSSTREET1 = (data.CrossStreet1 == null) ? string.Empty : data.CrossStreet1;
                tranData.METERLOC_CROSSSTREET2 = (data.CrossStreet2 == null) ? string.Empty : data.CrossStreet2;
                tranData.METERLOC_ENFORCEMENTHOURSDESC = (data.EnfHourDesc == null) ? string.Empty : data.EnfHourDesc;
                tranData.LicensePlate = (data.PlateNumber == null) ? string.Empty : data.PlateNumber;
                tranData.State = (data.LPLocation == null) ? string.Empty : data.LPLocation;
                tranData.Make = (data.VMake == null) ? string.Empty : data.VMake;
                tranData.Model = (data.VModel == null) ? string.Empty : data.VModel;
                tranData.IsOccupied = Utility.getStatus((data.SpaceStatus == null) ? string.Empty : data.SpaceStatus).ToString();
                tranData.EnforcementKey = (data.EnforcementKey == null) ? string.Empty : data.EnforcementKey;
                tranData.MeterStatus = (data.MeterStatus == null) ? null : data.MeterStatus;

                tranData.METERLOC_METERTYPE = (data.METERLOC_METERTYPE == null) ? null : data.METERLOC_METERTYPE;
                tranData.ZoneName = (data.ZoneName == null) ? null : data.ZoneName;
                tranData.MeterID = String.Empty;
                tranData.ReturnCode = data.ReturnCode;

                logData.Response = new ResponseEntity() { CustomerId = param.CustomerId.Value.ToString(), MeterId = tranData.MeterName, SpaceId = tranData.SpaceNo, CityCurrentTime = data.PresentMeterTime.HasValue ? data.PresentMeterTime.Value : DateTime.Now };
            }
            catch (Exception ex)
            {
                //var appPath = System.Web.Hosting.HostingEnvironment.ApplicationPhysicalPath;
                //string errorPath = Path.Combine(appPath, "GetTransformData", "Timezones");
                //ExceptionLogging.SendErrorToText(ex, "GetCitationData", errorPath);
                //tranData.ReturnCode = ((int)ReturnCodeEnum.Error).ToString();

                tranData.ReturnCode = ((int)ReturnCodeEnum.Error).ToString();
            }
            #endregion
            #region "Passport Monitoring"
            /////////////////////////////////////////////////
            TransactionTransform tranDataOfService = new TransactionTransform();
            try
            {
                //////////////////////////////////////////
                logData.Request.CustomerId = param.CustomerId.Value.ToString();
                logData.Request.SpaceId = param.parkingSpaceID;
                logData.Request.MeterId = param.MeterName;
                //////////////////////////////////////////////////
                DateTime? currentCustTime = customerCurrentTime.CurrentTime;
                PassportMonitoringCustomerElement customer = PassportMonitoringCustomerRetriever.GetCustomer(param.CustomerId.Value);
                LookUpLPNFactory factory = new LookUpLPNFactory(customer, param.PlateNumber);
                tranDataOfService = factory.GetRefreshPlateData();
                tranDataOfService.EnforcementKey = EnforcementKey;

                ///////////////////////////////////////////////////
                logData.Response = new ResponseEntity() { CustomerId = param.CustomerId.Value.ToString(), MeterId = tranData.MeterName, SpaceId = tranData.SpaceNo, CityCurrentTime = currentCustTime.HasValue ? currentCustTime.Value : DateTime.Now };
            }
            catch (Exception ex)
            {
                logData.Response = new ResponseEntity() { CustomerId = param.CustomerId.Value.ToString(), MeterId = tranData.MeterName, SpaceId = tranData.SpaceNo, CityCurrentTime = customerCurrentTime.CurrentTime.HasValue ? customerCurrentTime.CurrentTime.Value : DateTime.Now };
                //throw ex;
                tranData.ReturnCode = ((int)ReturnCodeEnum.Error).ToString();
            }
            finally
            {

            }
            #endregion

            //Reconcile data
            if ((tranData.ReturnCode == ((int)ReturnCodeEnum.NoRecordExists).ToString()) || (tranData.ReturnCode == ((int)ReturnCodeEnum.Error).ToString()))
            {
                tranData = tranDataOfService;
            }
            else if (tranData.ReturnCode == ((int)ReturnCodeEnum.Success).ToString())
            {
                if (tranDataOfService.ReturnCode == ((int)ReturnCodeEnum.Success).ToString())
                {
                    ////Select one record of highest expiry time
                    int? dbExpiredMinutes = tranData.MeterExpiredMinutes.ToNullableInt();
                    int? parkeonExpiredMinutes = tranDataOfService.MeterExpiredMinutes.ToNullableInt();
                    tranData = Nullable.Compare(dbExpiredMinutes, parkeonExpiredMinutes) > 0 ? tranData : tranDataOfService;
                }
            }

            else if (tranData.ReturnCode == ((int)ReturnCodeEnum.MoreRecordsExists).ToString())
            {
                // DB never give more record. hence ignore
            }

            return tranData;
        }
        public static TransactionTransform GetTybeeIslandTransformDataV2(string EnforcementKey, string filePath, RequestResponseEntity logData, CustomerTime customerCurrentTime)
        {
            TransactionTransform tranData = new TransactionTransform();
            tranData.ReturnCode = ((int)ReturnCodeEnum.InvalidInputParameters).ToString();
            TimeZoneInfo TZ = null;
            TZ = TimeZoneInfo.FindSystemTimeZoneById("Eastern Standard Time");
            //int? customerID = null;
            //string MeterName = null;
            //string parkingSpaceID;
            //string PlateNumber = null;
            //string stateName = null;
            //////////////////////////////////////
            KeyParameter param = PrepareKeyParameters.PrepareTybeeIsland(EnforcementKey);
            if (param.ReturnCode == ((int)ReturnCodeEnum.InvalidInputParameters).ToString())
            {
                return tranData;
            }
            /////////////////////////////////////
            #region "DB Check"
            try
            {
                Transaction data = TransactionDataValidatioin.GetDataTybeeIsland(param.CustomerId.Value, param.MeterName, param.parkingSpaceID, param.PlateNumber, param.StateName, EnforcementKey, filePath, logData);


                if (data.TimeZoneId.HasValue)
                    TZ = Utility.GetTimeZoneInfo(data.TimeZoneId.Value, filePath, param.CustomerId.Value.ToString());

                //For South Miami : PayStationId. Others it is MeterID
                tranData.MeterName = string.Empty; // (data.MeterName == null) ? string.Empty : data.MeterName;
                tranData.SpaceNo = string.Empty;  //data.ParkingSpaceID.ToString();

                tranData.MeterStreet = (data.MeterStreet == null) ? string.Empty : data.MeterStreet;

                DateTime _MeterRTC = data.PresentMeterTime.HasValue ? data.PresentMeterTime.Value : DateTime.Now;
                tranData.MeterRTC = String.Format("{0:yyyyMMdd_THHmmssZ}", Utility.findGMTTime(_MeterRTC, TZ));

                DateTime _SensorEventTime = data.SensorEventTime.HasValue ? data.SensorEventTime.Value : DateTime.Now;
                tranData.MeterLastUpdateTime = String.Format("{0:yyyyMMdd_THHmmssZ}", Utility.findGMTTime(_SensorEventTime, TZ));

                tranData.SensorEventTime = String.Format("{0:yyyyMMdd_THHmmssZ}", Utility.findGMTTime(_SensorEventTime, TZ));

                DateTime _endTime = data.ExpiryTime.HasValue ? data.ExpiryTime.Value : DateTime.Now;
                tranData.ExpiredTime = String.Format("{0:yyyyMMdd_THHmmssZ}", Utility.findGMTTime(_endTime, TZ));

                tranData.MeterExpiredMinutes = data.ExpiredMinutes.HasValue ? data.ExpiredMinutes.Value.ToString() : "0";
                tranData.METERLOC_BLOCKNUMBER = (data.Block == null) ? string.Empty : data.Block;
                tranData.METERLOC_STREETDIRECTION = (data.Direction == null) ? string.Empty : data.Direction;
                tranData.METERLOC_STREETNAME = (data.MeterStreet == null) ? string.Empty : data.MeterStreet;
                tranData.METERLOC_STREETTYPE = (data.StreetType == null) ? string.Empty : data.StreetType;
                tranData.METERLOC_CROSSSTREET1 = (data.CrossStreet1 == null) ? string.Empty : data.CrossStreet1;
                tranData.METERLOC_CROSSSTREET2 = (data.CrossStreet2 == null) ? string.Empty : data.CrossStreet2;
                tranData.METERLOC_ENFORCEMENTHOURSDESC = (data.EnfHourDesc == null) ? string.Empty : data.EnfHourDesc;
                tranData.LicensePlate = (data.PlateNumber == null) ? string.Empty : data.PlateNumber;
                tranData.State = (data.LPLocation == null) ? string.Empty : data.LPLocation;
                tranData.Make = (data.VMake == null) ? string.Empty : data.VMake;
                tranData.Model = (data.VModel == null) ? string.Empty : data.VModel;
                tranData.IsOccupied = Utility.getStatus((data.SpaceStatus == null) ? string.Empty : data.SpaceStatus).ToString();
                tranData.EnforcementKey = (data.EnforcementKey == null) ? string.Empty : data.EnforcementKey;
                tranData.MeterStatus = (data.MeterStatus == null) ? null : data.MeterStatus;

                tranData.METERLOC_METERTYPE = (data.METERLOC_METERTYPE == null) ? null : data.METERLOC_METERTYPE;
                tranData.ZoneName = (data.ZoneName == null) ? null : data.ZoneName;
                tranData.MeterID = String.Empty;
                tranData.ReturnCode = data.ReturnCode;

                logData.Response = new ResponseEntity() { CustomerId = param.CustomerId.Value.ToString(), MeterId = tranData.MeterName, SpaceId = tranData.SpaceNo, CityCurrentTime = data.PresentMeterTime.HasValue ? data.PresentMeterTime.Value : DateTime.Now };
            }
            catch (Exception ex)
            {
                //var appPath = System.Web.Hosting.HostingEnvironment.ApplicationPhysicalPath;
                //string errorPath = Path.Combine(appPath, "GetTransformData", "Timezones");
                //ExceptionLogging.SendErrorToText(ex, "GetCitationData", errorPath);
                //tranData.ReturnCode = ((int)ReturnCodeEnum.Error).ToString();

                tranData.ReturnCode = ((int)ReturnCodeEnum.Error).ToString();
            }
            #endregion
            #region "Passport Monitoring"
            /////////////////////////////////////////////////
            TransactionTransform tranDataOfService = new TransactionTransform();
            try
            {
                //////////////////////////////////////////
                logData.Request.CustomerId = param.CustomerId.Value.ToString();
                logData.Request.SpaceId = param.parkingSpaceID;
                logData.Request.MeterId = param.MeterName;
                //////////////////////////////////////////////////
                DateTime? currentCustTime = customerCurrentTime.CurrentTime;
                PassportMonitoringCustomerElement customer = PassportMonitoringCustomerRetriever.GetCustomer(param.CustomerId.Value);
                LookUpLPNFactory factory = new LookUpLPNFactory(customer, param.PlateNumber);
                var passportMonResLog = new VendorResponseEntity();
                logData.VendorData.Add(VendorNames.PassportMonitoring, passportMonResLog);
                tranDataOfService = factory.GetRefreshPlateData(ref passportMonResLog);
                tranDataOfService.EnforcementKey = EnforcementKey;

                ///////////////////////////////////////////////////
                logData.Response = new ResponseEntity() { CustomerId = param.CustomerId.Value.ToString(), MeterId = tranData.MeterName, SpaceId = tranData.SpaceNo, CityCurrentTime = currentCustTime.HasValue ? currentCustTime.Value : DateTime.Now };
            }
            catch (Exception ex)
            {
                logData.Response = new ResponseEntity() { CustomerId = param.CustomerId.Value.ToString(), MeterId = tranData.MeterName, SpaceId = tranData.SpaceNo, CityCurrentTime = customerCurrentTime.CurrentTime.HasValue ? customerCurrentTime.CurrentTime.Value : DateTime.Now };
                //throw ex;
                tranData.ReturnCode = ((int)ReturnCodeEnum.Error).ToString();
            }
            finally
            {

            }
            #endregion

            //Reconcile data
            if ((tranData.ReturnCode == ((int)ReturnCodeEnum.NoRecordExists).ToString()) || (tranData.ReturnCode == ((int)ReturnCodeEnum.Error).ToString()))
            {
                tranData = tranDataOfService;
            }
            else if (tranData.ReturnCode == ((int)ReturnCodeEnum.Success).ToString())
            {
                if (tranDataOfService.ReturnCode == ((int)ReturnCodeEnum.Success).ToString())
                {
                    ////Select one record of highest expiry time
                    int? dbExpiredMinutes = tranData.MeterExpiredMinutes.ToNullableInt();
                    int? parkeonExpiredMinutes = tranDataOfService.MeterExpiredMinutes.ToNullableInt();
                    tranData = Nullable.Compare(dbExpiredMinutes, parkeonExpiredMinutes) > 0 ? tranData : tranDataOfService;
                }
            }

            else if (tranData.ReturnCode == ((int)ReturnCodeEnum.MoreRecordsExists).ToString())
            {
                // DB never give more record. hence ignore
            }

            return tranData;
        }
        private static Transaction GetDataTybeeIsland(int customerID, string MeterName, string parkingSpaceID, string PlateNumber, string statename, string EnforcementKey, string filePath, RequestResponseEntity logData)
        {
            string ZoneID = null;
            string SpaceStatus = null;
            Transaction data = new Transaction();
            try
            {
                List<Transaction> transactionList = new List<Transaction>();

                logData.Request.CustomerId = customerID.ToString();
                logData.Request.SpaceId = parkingSpaceID;
                logData.Request.MeterId = MeterName;

                transactionList = DataAccess.GetTybeeIslandWithPlateName(customerID, null, null, null, PlateNumber, statename, null);

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
            }
            catch (Exception ex)
            {
                var appPath = System.Web.Hosting.HostingEnvironment.ApplicationPhysicalPath;
                string errorPath = Path.Combine(appPath, "GetData", "Timezones");
                ExceptionLogging.SendErrorToText(ex, "GetCitationData", errorPath);
                data.ReturnCode = ((int)ReturnCodeEnum.Error).ToString();
                throw;
            }
            return data;
        }
        #endregion

        #region RoyalOak
        public static TransactionTransform GetRoyalOakTransformData(string EnforcementKey, string filePath, RequestResponseEntity logData)
        {
            TransactionTransform tranData = new TransactionTransform();
            tranData.ReturnCode = ((int)ReturnCodeEnum.InvalidInputParameters).ToString();
            TimeZoneInfo TZ = null;
            TZ = TimeZoneInfo.FindSystemTimeZoneById("Eastern Standard Time");
            //int? customerID = null;
            //string MeterName = null;
            //string parkingSpaceID;
            //string PlateNumber = null;
            //string stateName = null;
            //////////////////////////////////////
            KeyParameter param = PrepareKeyParameters.PrepareRoyalOak(EnforcementKey);
            if (param.ReturnCode == ((int)ReturnCodeEnum.InvalidInputParameters).ToString())
            {
                return tranData;
            }


            /////////////////////////////////////
            try
            {
                Transaction data = TransactionDataValidatioin.GetDataRoyalOak(param.CustomerId.Value, param.MeterName, param.parkingSpaceID, param.PlateNumber, param.StateName, EnforcementKey, filePath, logData);


                if (data.TimeZoneId.HasValue)
                    TZ = Utility.GetTimeZoneInfo(data.TimeZoneId.Value, filePath, param.CustomerId.Value.ToString());

                //For South Miami : PayStationId. Others it is MeterID
                tranData.MeterName = (data.MeterName == null) ? string.Empty : data.MeterName;
                tranData.SpaceNo = data.ParkingSpaceID.ToString();

                tranData.MeterStreet = (data.MeterStreet == null) ? string.Empty : data.MeterStreet;

                DateTime _MeterRTC = data.PresentMeterTime.HasValue ? data.PresentMeterTime.Value : DateTime.Now;
                tranData.MeterRTC = String.Format("{0:yyyyMMdd_THHmmssZ}", Utility.findGMTTime(_MeterRTC, TZ));

                DateTime _SensorEventTime = data.SensorEventTime.HasValue ? data.SensorEventTime.Value : DateTime.Now;
                tranData.MeterLastUpdateTime = String.Format("{0:yyyyMMdd_THHmmssZ}", Utility.findGMTTime(_SensorEventTime, TZ));

                tranData.SensorEventTime = String.Format("{0:yyyyMMdd_THHmmssZ}", Utility.findGMTTime(_SensorEventTime, TZ));

                DateTime _endTime = data.ExpiryTime.HasValue ? data.ExpiryTime.Value : DateTime.Now;
                tranData.ExpiredTime = String.Format("{0:yyyyMMdd_THHmmssZ}", Utility.findGMTTime(_endTime, TZ));

                tranData.MeterExpiredMinutes = data.ExpiredMinutes.HasValue ? data.ExpiredMinutes.Value.ToString() : "0";
                tranData.METERLOC_BLOCKNUMBER = (data.Block == null) ? string.Empty : data.Block;
                tranData.METERLOC_STREETDIRECTION = (data.Direction == null) ? string.Empty : data.Direction;
                tranData.METERLOC_STREETNAME = (data.MeterStreet == null) ? string.Empty : data.MeterStreet;
                tranData.METERLOC_STREETTYPE = (data.StreetType == null) ? string.Empty : data.StreetType;
                tranData.METERLOC_CROSSSTREET1 = (data.CrossStreet1 == null) ? string.Empty : data.CrossStreet1;
                tranData.METERLOC_CROSSSTREET2 = (data.CrossStreet2 == null) ? string.Empty : data.CrossStreet2;
                tranData.METERLOC_ENFORCEMENTHOURSDESC = (data.EnfHourDesc == null) ? string.Empty : data.EnfHourDesc;
                tranData.LicensePlate = (data.PlateNumber == null) ? string.Empty : data.PlateNumber;
                tranData.State = (data.LPLocation == null) ? string.Empty : data.LPLocation;
                tranData.Make = (data.VMake == null) ? string.Empty : data.VMake;
                tranData.Model = (data.VModel == null) ? string.Empty : data.VModel;
                tranData.IsOccupied = Utility.getStatus((data.SpaceStatus == null) ? string.Empty : data.SpaceStatus).ToString();
                tranData.EnforcementKey = (data.EnforcementKey == null) ? string.Empty : data.EnforcementKey;
                tranData.MeterStatus = (data.MeterStatus == null) ? null : data.MeterStatus;

                tranData.METERLOC_METERTYPE = (data.METERLOC_METERTYPE == null) ? null : data.METERLOC_METERTYPE;
                tranData.ZoneName = (data.ZoneName == null) ? null : data.ZoneName;

                tranData.MeterID = data.PayStationID.HasValue ? data.PayStationID.Value.ToString() : "0";
                tranData.ReturnCode = data.ReturnCode;

                logData.Response = new ResponseEntity() { CustomerId = param.CustomerId.Value.ToString(), MeterId = tranData.MeterName, SpaceId = tranData.SpaceNo, CityCurrentTime = data.PresentMeterTime.HasValue ? data.PresentMeterTime.Value : DateTime.Now };
            }
            catch (Exception ex)
            {
                var appPath = System.Web.Hosting.HostingEnvironment.ApplicationPhysicalPath;
                string errorPath = Path.Combine(appPath, "GetTransformData", "Timezones");
                ExceptionLogging.SendErrorToText(ex, "GetCitationData", errorPath);
                tranData.ReturnCode = ((int)ReturnCodeEnum.Error).ToString();

            }
            return tranData;
        }
        public static TransactionTransform GetRoyalOakTransformDataV2(string EnforcementKey, string filePath, RequestResponseEntity logData, CustomerTime customerCurrentTime)
        {
            TransactionTransform tranData = new TransactionTransform();
            tranData.ReturnCode = ((int)ReturnCodeEnum.InvalidInputParameters).ToString();
            TimeZoneInfo TZ = null;
            TZ = TimeZoneInfo.FindSystemTimeZoneById("Eastern Standard Time");
            //int? customerID = null;
            //string MeterName = null;
            //string parkingSpaceID;
            //string PlateNumber = null;
            //string stateName = null;
            //////////////////////////////////////
            KeyParameter param = PrepareKeyParameters.PrepareRoyalOak(EnforcementKey);
            if (param.ReturnCode == ((int)ReturnCodeEnum.InvalidInputParameters).ToString())
            {
                return tranData;
            }

            #region "DB Check"
            /////////////////////////////////////
            try
            {
                Transaction data = TransactionDataValidatioin.GetDataRoyalOak(param.CustomerId.Value, param.MeterName, param.parkingSpaceID, param.PlateNumber, param.StateName, EnforcementKey, filePath, logData);


                if (data.TimeZoneId.HasValue)
                    TZ = Utility.GetTimeZoneInfo(data.TimeZoneId.Value, filePath, param.CustomerId.Value.ToString());

                //For South Miami : PayStationId. Others it is MeterID
                tranData.MeterName = (data.MeterName == null) ? string.Empty : data.MeterName;
                tranData.SpaceNo = data.ParkingSpaceID.ToString();

                tranData.MeterStreet = (data.MeterStreet == null) ? string.Empty : data.MeterStreet;

                DateTime _MeterRTC = data.PresentMeterTime.HasValue ? data.PresentMeterTime.Value : DateTime.Now;
                tranData.MeterRTC = String.Format("{0:yyyyMMdd_THHmmssZ}", Utility.findGMTTime(_MeterRTC, TZ));

                DateTime _SensorEventTime = data.SensorEventTime.HasValue ? data.SensorEventTime.Value : DateTime.Now;
                tranData.MeterLastUpdateTime = String.Format("{0:yyyyMMdd_THHmmssZ}", Utility.findGMTTime(_SensorEventTime, TZ));

                tranData.SensorEventTime = String.Format("{0:yyyyMMdd_THHmmssZ}", Utility.findGMTTime(_SensorEventTime, TZ));

                DateTime _endTime = data.ExpiryTime.HasValue ? data.ExpiryTime.Value : DateTime.Now;
                tranData.ExpiredTime = String.Format("{0:yyyyMMdd_THHmmssZ}", Utility.findGMTTime(_endTime, TZ));

                tranData.MeterExpiredMinutes = data.ExpiredMinutes.HasValue ? data.ExpiredMinutes.Value.ToString() : "0";
                tranData.METERLOC_BLOCKNUMBER = (data.Block == null) ? string.Empty : data.Block;
                tranData.METERLOC_STREETDIRECTION = (data.Direction == null) ? string.Empty : data.Direction;
                tranData.METERLOC_STREETNAME = (data.MeterStreet == null) ? string.Empty : data.MeterStreet;
                tranData.METERLOC_STREETTYPE = (data.StreetType == null) ? string.Empty : data.StreetType;
                tranData.METERLOC_CROSSSTREET1 = (data.CrossStreet1 == null) ? string.Empty : data.CrossStreet1;
                tranData.METERLOC_CROSSSTREET2 = (data.CrossStreet2 == null) ? string.Empty : data.CrossStreet2;
                tranData.METERLOC_ENFORCEMENTHOURSDESC = (data.EnfHourDesc == null) ? string.Empty : data.EnfHourDesc;
                tranData.LicensePlate = (data.PlateNumber == null) ? string.Empty : data.PlateNumber;
                tranData.State = (data.LPLocation == null) ? string.Empty : data.LPLocation;
                tranData.Make = (data.VMake == null) ? string.Empty : data.VMake;
                tranData.Model = (data.VModel == null) ? string.Empty : data.VModel;
                tranData.IsOccupied = Utility.getStatus((data.SpaceStatus == null) ? string.Empty : data.SpaceStatus).ToString();
                tranData.EnforcementKey = (data.EnforcementKey == null) ? string.Empty : data.EnforcementKey;
                tranData.MeterStatus = (data.MeterStatus == null) ? null : data.MeterStatus;

                tranData.METERLOC_METERTYPE = (data.METERLOC_METERTYPE == null) ? null : data.METERLOC_METERTYPE;
                tranData.ZoneName = (data.ZoneName == null) ? null : data.ZoneName;

                tranData.MeterID = data.PayStationID.HasValue ? data.PayStationID.Value.ToString() : "0";
                tranData.ReturnCode = data.ReturnCode;

                logData.Response = new ResponseEntity() { CustomerId = param.CustomerId.Value.ToString(), MeterId = tranData.MeterName, SpaceId = tranData.SpaceNo, CityCurrentTime = data.PresentMeterTime.HasValue ? data.PresentMeterTime.Value : DateTime.Now };
            }
            catch (Exception ex)
            {
                //var appPath = System.Web.Hosting.HostingEnvironment.ApplicationPhysicalPath;
                //string errorPath = Path.Combine(appPath, "GetTransformData", "Timezones");
                //ExceptionLogging.SendErrorToText(ex, "GetCitationData", errorPath);
                //tranData.ReturnCode = ((int)ReturnCodeEnum.Error).ToString();
                tranData.ReturnCode = ((int)ReturnCodeEnum.Error).ToString();
            }
            #endregion
            #region "Parkmobile Service Call"
            /////////////////////////////////////////////////
            TransactionTransform tranDataOfServiceParkMobile = new TransactionTransform();
            try
            {
                //////////////////////////////////////////
                logData.Request.CustomerId = param.CustomerId.Value.ToString();
                logData.Request.SpaceId = param.parkingSpaceID;
                logData.Request.MeterId = param.MeterName;
                //////////////////////////////////////////////////
                DateTime? currentCustTime = customerCurrentTime.CurrentTime;
                PMCustomerElement customer = ParkmobileCustomerRetriever.GetCustomer(param.CustomerId.Value);
                ParkMobileGenericFactory factory = new ParkMobileGenericFactory(customer, param.PlateNumber);
                var parkMobileResLog = new VendorResponseEntity();
                logData.VendorData.Add(VendorNames.ParkMobile, parkMobileResLog);
                tranDataOfServiceParkMobile = factory.GetRefreshSpaceData(ParkingMobileDataType.Plate, ref parkMobileResLog);
                tranDataOfServiceParkMobile.EnforcementKey = EnforcementKey;

                ///////////////////////////////////////////////////
                logData.Response = new ResponseEntity() { CustomerId = param.CustomerId.Value.ToString(), MeterId = tranData.MeterName, SpaceId = tranData.SpaceNo, CityCurrentTime = currentCustTime.HasValue ? currentCustTime.Value : DateTime.Now };
            }
            catch (Exception ex)
            {
                logData.Response = new ResponseEntity() { CustomerId = param.CustomerId.Value.ToString(), MeterId = tranData.MeterName, SpaceId = tranData.SpaceNo, CityCurrentTime = customerCurrentTime.CurrentTime.HasValue ? customerCurrentTime.CurrentTime.Value : DateTime.Now };
                //throw ex;
                tranDataOfServiceParkMobile.ReturnCode = ((int)ReturnCodeEnum.Error).ToString();
            }
            finally
            {

            }
            #endregion
            //////////////////  //Reconcile data

            List<TransactionTransform> trans = new List<TransactionTransform>();
            trans.Add(tranDataOfServiceParkMobile); // Parkmobile
            trans.Add(tranData); // Database

            var rightValue = trans.Where(t => t.ReturnCode == ((int)ReturnCodeEnum.Success).ToString()).OrderByDescending(t => t.MeterExpiredMinutes.ToNullableInt()).FirstOrDefault<TransactionTransform>();
            if (rightValue == default(TransactionTransform))
            {
                //Do nothing.just return database  object.
            }
            else
            {
                tranData = rightValue;
            }

            return tranData;
        }
        private static Transaction GetDataRoyalOak(int customerID, string MeterName, string parkingSpaceID, string PlateNumber, string statename, string EnforcementKey, string filePath, RequestResponseEntity logData)
        {
            string ZoneID = null;
            string SpaceStatus = null;
            Transaction data = new Transaction();
            try
            {
                List<Transaction> transactionList = new List<Transaction>();

                logData.Request.CustomerId = customerID.ToString();
                logData.Request.SpaceId = parkingSpaceID;
                logData.Request.MeterId = MeterName;

                transactionList = DataAccess.GetTempeCitationData(customerID, null, MeterName, parkingSpaceID, PlateNumber, null);

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
            }
            catch (Exception ex)
            {
                var appPath = System.Web.Hosting.HostingEnvironment.ApplicationPhysicalPath;
                string errorPath = Path.Combine(appPath, "GetData", "Timezones");
                ExceptionLogging.SendErrorToText(ex, "GetCitationData", errorPath);
                data.ReturnCode = ((int)ReturnCodeEnum.Error).ToString();
                throw;
            }
            return data;
        }

        #endregion

        #region Tempe
        public static TransactionTransform GetTempeTransformData(string EnforcementKey, string filePath, RequestResponseEntity logData, CustomerTime customerCurrentTime)
        {
            TransactionTransform tranData = new TransactionTransform();
            tranData.ReturnCode = ((int)ReturnCodeEnum.InvalidInputParameters).ToString();
            TimeZoneInfo TZ = null;
            TZ = TimeZoneInfo.FindSystemTimeZoneById("Eastern Standard Time");
            //////////////////////////////////////
            KeyParameter param = PrepareKeyParameters.PrepareTempe(EnforcementKey);
            if (param.ReturnCode == ((int)ReturnCodeEnum.InvalidInputParameters).ToString())
            {
                return tranData;
            }

            #region "DB Check"
            /////////////////////////////////////
            try
            {
                Transaction data = TransactionDataValidatioin.GetDataTempe(param.CustomerId.Value, param.MeterName, param.parkingSpaceID, param.PlateNumber, param.StateName, EnforcementKey, filePath, logData);


                if (data.TimeZoneId.HasValue)
                    TZ = Utility.GetTimeZoneInfo(data.TimeZoneId.Value, filePath, param.CustomerId.Value.ToString());

                //For South Miami : PayStationId. Others it is MeterID
                tranData.MeterName = (data.MeterName == null) ? string.Empty : data.MeterName;
                tranData.SpaceNo = data.ParkingSpaceID.ToString();

                tranData.MeterStreet = (data.MeterStreet == null) ? string.Empty : data.MeterStreet;

                DateTime _MeterRTC = data.PresentMeterTime.HasValue ? data.PresentMeterTime.Value : DateTime.Now;
                tranData.MeterRTC = String.Format("{0:yyyyMMdd_THHmmssZ}", Utility.findGMTTime(_MeterRTC, TZ));

                DateTime _SensorEventTime = data.SensorEventTime.HasValue ? data.SensorEventTime.Value : DateTime.Now;
                tranData.MeterLastUpdateTime = String.Format("{0:yyyyMMdd_THHmmssZ}", Utility.findGMTTime(_SensorEventTime, TZ));

                tranData.SensorEventTime = String.Format("{0:yyyyMMdd_THHmmssZ}", Utility.findGMTTime(_SensorEventTime, TZ));

                DateTime _endTime = data.ExpiryTime.HasValue ? data.ExpiryTime.Value : DateTime.Now;
                tranData.ExpiredTime = String.Format("{0:yyyyMMdd_THHmmssZ}", Utility.findGMTTime(_endTime, TZ));

                tranData.MeterExpiredMinutes = data.ExpiredMinutes.HasValue ? data.ExpiredMinutes.Value.ToString() : "0";
                tranData.METERLOC_BLOCKNUMBER = (data.Block == null) ? string.Empty : data.Block;
                tranData.METERLOC_STREETDIRECTION = (data.Direction == null) ? string.Empty : data.Direction;
                tranData.METERLOC_STREETNAME = (data.MeterStreet == null) ? string.Empty : data.MeterStreet;
                tranData.METERLOC_STREETTYPE = (data.StreetType == null) ? string.Empty : data.StreetType;
                tranData.METERLOC_CROSSSTREET1 = (data.CrossStreet1 == null) ? string.Empty : data.CrossStreet1;
                tranData.METERLOC_CROSSSTREET2 = (data.CrossStreet2 == null) ? string.Empty : data.CrossStreet2;
                tranData.METERLOC_ENFORCEMENTHOURSDESC = (data.EnfHourDesc == null) ? string.Empty : data.EnfHourDesc;
                tranData.LicensePlate = (data.PlateNumber == null) ? string.Empty : data.PlateNumber;
                tranData.State = (data.LPLocation == null) ? string.Empty : data.LPLocation;
                tranData.Make = (data.VMake == null) ? string.Empty : data.VMake;
                tranData.Model = (data.VModel == null) ? string.Empty : data.VModel;
                tranData.IsOccupied = Utility.getStatus((data.SpaceStatus == null) ? string.Empty : data.SpaceStatus).ToString();
                tranData.EnforcementKey = (data.EnforcementKey == null) ? string.Empty : data.EnforcementKey;
                tranData.MeterStatus = (data.MeterStatus == null) ? null : data.MeterStatus;

                tranData.METERLOC_METERTYPE = (data.METERLOC_METERTYPE == null) ? null : data.METERLOC_METERTYPE;
                tranData.ZoneName = (data.ZoneName == null) ? null : data.ZoneName;

                tranData.MeterID = data.PayStationID.HasValue ? data.PayStationID.Value.ToString() : "0";
                tranData.ReturnCode = data.ReturnCode;

                logData.Response = new ResponseEntity() { CustomerId = param.CustomerId.Value.ToString(), MeterId = tranData.MeterName, SpaceId = tranData.SpaceNo, CityCurrentTime = data.PresentMeterTime.HasValue ? data.PresentMeterTime.Value : DateTime.Now };
            }
            catch (Exception ex)
            {
                //var appPath = System.Web.Hosting.HostingEnvironment.ApplicationPhysicalPath;
                //string errorPath = Path.Combine(appPath, "GetTransformData", "Timezones");
                //ExceptionLogging.SendErrorToText(ex, "GetCitationData", errorPath);
                //tranData.ReturnCode = ((int)ReturnCodeEnum.Error).ToString();
                tranData.ReturnCode = ((int)ReturnCodeEnum.Error).ToString();
            }
            #endregion
            #region "Parkmobile Service Call"
            /////////////////////////////////////////////////
            TransactionTransform tranDataOfServiceParkMobile = new TransactionTransform();
            try
            {
                //////////////////////////////////////////
                logData.Request.CustomerId = param.CustomerId.Value.ToString();
                logData.Request.SpaceId = param.parkingSpaceID;
                logData.Request.MeterId = param.MeterName;
                //////////////////////////////////////////////////
                DateTime? currentCustTime = customerCurrentTime.CurrentTime;
                PMCustomerElement customer = ParkmobileCustomerRetriever.GetCustomer(param.CustomerId.Value);
                ParkMobileGenericFactory factory = new ParkMobileGenericFactory(customer, param.PlateNumber);
                var parkMobileResLog = new VendorResponseEntity();
                logData.VendorData.Add(VendorNames.ParkMobile, parkMobileResLog);
                tranDataOfServiceParkMobile = factory.GetRefreshSpaceData(ParkingMobileDataType.Plate, ref parkMobileResLog);
                tranDataOfServiceParkMobile.EnforcementKey = EnforcementKey;

                ///////////////////////////////////////////////////
                logData.Response = new ResponseEntity() { CustomerId = param.CustomerId.Value.ToString(), MeterId = tranData.MeterName, SpaceId = tranData.SpaceNo, CityCurrentTime = currentCustTime.HasValue ? currentCustTime.Value : DateTime.Now };
            }
            catch (Exception ex)
            {
                logData.Response = new ResponseEntity() { CustomerId = param.CustomerId.Value.ToString(), MeterId = tranData.MeterName, SpaceId = tranData.SpaceNo, CityCurrentTime = customerCurrentTime.CurrentTime.HasValue ? customerCurrentTime.CurrentTime.Value : DateTime.Now };
                //throw ex;
                tranDataOfServiceParkMobile.ReturnCode = ((int)ReturnCodeEnum.Error).ToString();
            }
            finally
            {

            }
            #endregion
            //////////////////  //Reconcile data

            List<TransactionTransform> trans = new List<TransactionTransform>();
            trans.Add(tranDataOfServiceParkMobile); // Parkmobile
            trans.Add(tranData); // Database

            var rightValue = trans.Where(t => t.ReturnCode == ((int)ReturnCodeEnum.Success).ToString()).OrderByDescending(t => t.MeterExpiredMinutes.ToNullableInt()).FirstOrDefault<TransactionTransform>();
            if (rightValue == default(TransactionTransform))
            {
                //Do nothing.just return database  object.
            }
            else
            {
                tranData = rightValue;
            }

            return tranData;
        }
        public static TransactionTransform GetTempeTransformDataV3(string EnforcementKey, string filePath, RequestResponseEntity logData, CustomerTime customerCurrentTime)
        {
            TransactionTransform tranData = new TransactionTransform();
            tranData.ReturnCode = ((int)ReturnCodeEnum.InvalidInputParameters).ToString();
            TimeZoneInfo TZ = null;
            TZ = TimeZoneInfo.FindSystemTimeZoneById("Eastern Standard Time");
            //////////////////////////////////////
            KeyParameter param = PrepareKeyParameters.PrepareTempe(EnforcementKey);
            if (param.ReturnCode == ((int)ReturnCodeEnum.InvalidInputParameters).ToString())
            {
                return tranData;
            }

            #region "DB Check"
            /////////////////////////////////////
            try
            {
                Transaction data = TransactionDataValidatioin.GetDataTempe(param.CustomerId.Value, param.MeterName, param.parkingSpaceID, param.PlateNumber, param.StateName, EnforcementKey, filePath, logData);


                if (data.TimeZoneId.HasValue)
                    TZ = Utility.GetTimeZoneInfo(data.TimeZoneId.Value, filePath, param.CustomerId.Value.ToString());

                //For South Miami : PayStationId. Others it is MeterID
                tranData.MeterName = (data.MeterName == null) ? string.Empty : data.MeterName;
                tranData.SpaceNo = data.ParkingSpaceID.ToString();

                tranData.MeterStreet = (data.MeterStreet == null) ? string.Empty : data.MeterStreet;

                DateTime _MeterRTC = data.PresentMeterTime.HasValue ? data.PresentMeterTime.Value : DateTime.Now;
                tranData.MeterRTC = String.Format("{0:yyyyMMdd_THHmmssZ}", Utility.findGMTTime(_MeterRTC, TZ));

                DateTime _SensorEventTime = data.SensorEventTime.HasValue ? data.SensorEventTime.Value : DateTime.Now;
                tranData.MeterLastUpdateTime = String.Format("{0:yyyyMMdd_THHmmssZ}", Utility.findGMTTime(_SensorEventTime, TZ));

                tranData.SensorEventTime = String.Format("{0:yyyyMMdd_THHmmssZ}", Utility.findGMTTime(_SensorEventTime, TZ));

                DateTime _endTime = data.ExpiryTime.HasValue ? data.ExpiryTime.Value : DateTime.Now;
                tranData.ExpiredTime = String.Format("{0:yyyyMMdd_THHmmssZ}", Utility.findGMTTime(_endTime, TZ));

                tranData.MeterExpiredMinutes = data.ExpiredMinutes.HasValue ? data.ExpiredMinutes.Value.ToString() : "0";
                tranData.METERLOC_BLOCKNUMBER = (data.Block == null) ? string.Empty : data.Block;
                tranData.METERLOC_STREETDIRECTION = (data.Direction == null) ? string.Empty : data.Direction;
                tranData.METERLOC_STREETNAME = (data.MeterStreet == null) ? string.Empty : data.MeterStreet;
                tranData.METERLOC_STREETTYPE = (data.StreetType == null) ? string.Empty : data.StreetType;
                tranData.METERLOC_CROSSSTREET1 = (data.CrossStreet1 == null) ? string.Empty : data.CrossStreet1;
                tranData.METERLOC_CROSSSTREET2 = (data.CrossStreet2 == null) ? string.Empty : data.CrossStreet2;
                tranData.METERLOC_ENFORCEMENTHOURSDESC = (data.EnfHourDesc == null) ? string.Empty : data.EnfHourDesc;
                tranData.LicensePlate = (data.PlateNumber == null) ? string.Empty : data.PlateNumber;
                tranData.State = (data.LPLocation == null) ? string.Empty : data.LPLocation;
                tranData.Make = (data.VMake == null) ? string.Empty : data.VMake;
                tranData.Model = (data.VModel == null) ? string.Empty : data.VModel;
                tranData.IsOccupied = Utility.getStatus((data.SpaceStatus == null) ? string.Empty : data.SpaceStatus).ToString();
                tranData.EnforcementKey = (data.EnforcementKey == null) ? string.Empty : data.EnforcementKey;
                tranData.MeterStatus = (data.MeterStatus == null) ? null : data.MeterStatus;

                tranData.METERLOC_METERTYPE = (data.METERLOC_METERTYPE == null) ? null : data.METERLOC_METERTYPE;
                tranData.ZoneName = (data.ZoneName == null) ? null : data.ZoneName;

                tranData.MeterID = data.PayStationID.HasValue ? data.PayStationID.Value.ToString() : "0";
                tranData.ReturnCode = data.ReturnCode;

                logData.Response = new ResponseEntity() { CustomerId = param.CustomerId.Value.ToString(), MeterId = tranData.MeterName, SpaceId = tranData.SpaceNo, CityCurrentTime = data.PresentMeterTime.HasValue ? data.PresentMeterTime.Value : DateTime.Now };
            }
            catch (Exception ex)
            {
                //var appPath = System.Web.Hosting.HostingEnvironment.ApplicationPhysicalPath;
                //string errorPath = Path.Combine(appPath, "GetTransformData", "Timezones");
                //ExceptionLogging.SendErrorToText(ex, "GetCitationData", errorPath);
                //tranData.ReturnCode = ((int)ReturnCodeEnum.Error).ToString();
                tranData.ReturnCode = ((int)ReturnCodeEnum.Error).ToString();
            }
            #endregion
            #region "Parkmobile Service Call"
            /////////////////////////////////////////////////
            TransactionTransform tranDataOfServiceParkMobile = new TransactionTransform();
            try
            {
                //////////////////////////////////////////
                logData.Request.CustomerId = param.CustomerId.Value.ToString();
                logData.Request.SpaceId = param.parkingSpaceID;
                logData.Request.MeterId = param.MeterName;
                //////////////////////////////////////////////////
                DateTime? currentCustTime = customerCurrentTime.CurrentTime;
                PMCustomerElement customer = ParkmobileCustomerRetriever.GetCustomer(param.CustomerId.Value);
                ParkMobileGenericFactory factory = new ParkMobileGenericFactory(customer, param.PlateNumber);
                var parkMobileResLog = new VendorResponseEntity();
                logData.VendorData.Add(VendorNames.ParkMobile, parkMobileResLog);
                tranDataOfServiceParkMobile = factory.GetRefreshSpaceData(ParkingMobileDataType.Plate, ref parkMobileResLog);
                tranDataOfServiceParkMobile.EnforcementKey = EnforcementKey;

                ///////////////////////////////////////////////////
                logData.Response = new ResponseEntity() { CustomerId = param.CustomerId.Value.ToString(), MeterId = tranData.MeterName, SpaceId = tranData.SpaceNo, CityCurrentTime = currentCustTime.HasValue ? currentCustTime.Value : DateTime.Now };
            }
            catch (Exception ex)
            {
                logData.Response = new ResponseEntity() { CustomerId = param.CustomerId.Value.ToString(), MeterId = tranData.MeterName, SpaceId = tranData.SpaceNo, CityCurrentTime = customerCurrentTime.CurrentTime.HasValue ? customerCurrentTime.CurrentTime.Value : DateTime.Now };
                //throw ex;
                tranDataOfServiceParkMobile.ReturnCode = ((int)ReturnCodeEnum.Error).ToString();
            }
            finally
            {

            }
            #endregion
            #region T2Digital
            TransactionTransform tranDataOfServiceT2 = new TransactionTransform();
            try
            {
                //////////////////////////////////////////
                logData.Request.CustomerId = param.CustomerId.Value.ToString();
                logData.Request.SpaceId = param.parkingSpaceID;
                logData.Request.MeterId = param.MeterName;
                //////////////////////////////////////////////////
                DateTime? currentCustTime = customerCurrentTime.CurrentTime;
                T2Digital.T2DigitalCustomerElement customer = T2Digital.T2DigitalCustomerRetriever.GetCustomer(param.CustomerId.Value);
                T2Digital.T2DigitalGenericFactory factory = new T2Digital.T2DigitalGenericFactory(customer, param.PlateNumber, string.Empty);
                var t2ResLog = new VendorResponseEntity();
                logData.VendorData.Add(VendorNames.T2Digital, t2ResLog);
                tranDataOfServiceT2 = factory.GetRefreshPlateData(ref t2ResLog);
                tranDataOfServiceT2.EnforcementKey = EnforcementKey;

                ///////////////////////////////////////////////////
                logData.Response = new ResponseEntity() { CustomerId = param.CustomerId.Value.ToString(), MeterId = tranData.MeterName, SpaceId = tranData.SpaceNo, CityCurrentTime = currentCustTime.HasValue ? currentCustTime.Value : DateTime.Now };

            }
            catch (Exception ex)
            {
                logData.Response = new ResponseEntity() { CustomerId = param.CustomerId.Value.ToString(), MeterId = tranData.MeterName, SpaceId = tranData.SpaceNo, CityCurrentTime = customerCurrentTime.CurrentTime.HasValue ? customerCurrentTime.CurrentTime.Value : DateTime.Now };
                //throw ex;
                tranDataOfServiceT2.ReturnCode = ((int)ReturnCodeEnum.Error).ToString();
            }
            finally
            {

            }
            #endregion
            #region "IPC"
            TransactionTransform tranDataOfServiceIPC = new TransactionTransform();
            try
            {
                //////////////////////////////////////////
                logData.Request.CustomerId = param.CustomerId.Value.ToString();
                logData.Request.SpaceId = param.parkingSpaceID;
                logData.Request.MeterId = param.MeterName;
                //////////////////////////////////////////////////
                DateTime? currentCustTime = customerCurrentTime.CurrentTime;
                IPC.IPCCustomerElement customer = IPCCustomerRetriever.GetCustomer(param.CustomerId.Value);
                IPC.IPCGenericFactory factory = new IPC.IPCGenericFactory(customer, param.PlateNumber, string.Empty, customerCurrentTime);
                var ipcResLog = new VendorResponseEntity();
                logData.VendorData.Add(VendorNames.IPC, ipcResLog);
                tranDataOfServiceIPC = factory.GetRefreshPlateData(ref ipcResLog);
                tranDataOfServiceIPC.EnforcementKey = EnforcementKey;

                ///////////////////////////////////////////////////
                logData.Response = new ResponseEntity() { CustomerId = param.CustomerId.Value.ToString(), MeterId = tranData.MeterName, SpaceId = tranData.SpaceNo, CityCurrentTime = currentCustTime.HasValue ? currentCustTime.Value : DateTime.Now };

            }
            catch (Exception ex)
            {
                logData.Response = new ResponseEntity() { CustomerId = param.CustomerId.Value.ToString(), MeterId = tranData.MeterName, SpaceId = tranData.SpaceNo, CityCurrentTime = customerCurrentTime.CurrentTime.HasValue ? customerCurrentTime.CurrentTime.Value : DateTime.Now };
                //throw ex;
                tranDataOfServiceIPC.ReturnCode = ((int)ReturnCodeEnum.Error).ToString();
            }
            finally
            {

            }
            #endregion
            //////////////////  //Reconcile data

            List<TransactionTransform> trans = new List<TransactionTransform>();
            trans.Add(tranDataOfServiceParkMobile); // Parkmobile
            trans.Add(tranData); // Database
            trans.Add(tranDataOfServiceT2); // T2 Data
            trans.Add(tranDataOfServiceIPC); // IPC Data

            var rightValue = trans.Where(t => t.ReturnCode == ((int)ReturnCodeEnum.Success).ToString()).OrderByDescending(t => t.MeterExpiredMinutes.ToNullableInt()).FirstOrDefault<TransactionTransform>();
            if (rightValue == default(TransactionTransform))
            {
                //Do nothing.just return database  object.
            }
            else
            {
                tranData = rightValue;
            }

            return tranData;
        }
        public static TransactionTransform GetTempeTransformDataV2(string EnforcementKey, string filePath, RequestResponseEntity logData, CustomerTime customerCurrentTime)
        {
            TransactionTransform tranData = new TransactionTransform();
            tranData.ReturnCode = ((int)ReturnCodeEnum.InvalidInputParameters).ToString();
            TimeZoneInfo TZ = null;
            TZ = TimeZoneInfo.FindSystemTimeZoneById("Eastern Standard Time");
            //////////////////////////////////////
            KeyParameter param = PrepareKeyParameters.PrepareTempe(EnforcementKey);
            if (param.ReturnCode == ((int)ReturnCodeEnum.InvalidInputParameters).ToString())
            {
                return tranData;
            }

            #region "DB Check"
            /////////////////////////////////////
            try
            {
                Transaction data = TransactionDataValidatioin.GetDataTempe(param.CustomerId.Value, param.MeterName, param.parkingSpaceID, param.PlateNumber, param.StateName, EnforcementKey, filePath, logData);


                if (data.TimeZoneId.HasValue)
                    TZ = Utility.GetTimeZoneInfo(data.TimeZoneId.Value, filePath, param.CustomerId.Value.ToString());

                //For South Miami : PayStationId. Others it is MeterID
                tranData.MeterName = (data.MeterName == null) ? string.Empty : data.MeterName;
                tranData.SpaceNo = data.ParkingSpaceID.ToString();

                tranData.MeterStreet = (data.MeterStreet == null) ? string.Empty : data.MeterStreet;

                DateTime _MeterRTC = data.PresentMeterTime.HasValue ? data.PresentMeterTime.Value : DateTime.Now;
                tranData.MeterRTC = String.Format("{0:yyyyMMdd_THHmmssZ}", Utility.findGMTTime(_MeterRTC, TZ));

                DateTime _SensorEventTime = data.SensorEventTime.HasValue ? data.SensorEventTime.Value : DateTime.Now;
                tranData.MeterLastUpdateTime = String.Format("{0:yyyyMMdd_THHmmssZ}", Utility.findGMTTime(_SensorEventTime, TZ));

                tranData.SensorEventTime = String.Format("{0:yyyyMMdd_THHmmssZ}", Utility.findGMTTime(_SensorEventTime, TZ));

                DateTime _endTime = data.ExpiryTime.HasValue ? data.ExpiryTime.Value : DateTime.Now;
                tranData.ExpiredTime = String.Format("{0:yyyyMMdd_THHmmssZ}", Utility.findGMTTime(_endTime, TZ));

                tranData.MeterExpiredMinutes = data.ExpiredMinutes.HasValue ? data.ExpiredMinutes.Value.ToString() : "0";
                tranData.METERLOC_BLOCKNUMBER = (data.Block == null) ? string.Empty : data.Block;
                tranData.METERLOC_STREETDIRECTION = (data.Direction == null) ? string.Empty : data.Direction;
                tranData.METERLOC_STREETNAME = (data.MeterStreet == null) ? string.Empty : data.MeterStreet;
                tranData.METERLOC_STREETTYPE = (data.StreetType == null) ? string.Empty : data.StreetType;
                tranData.METERLOC_CROSSSTREET1 = (data.CrossStreet1 == null) ? string.Empty : data.CrossStreet1;
                tranData.METERLOC_CROSSSTREET2 = (data.CrossStreet2 == null) ? string.Empty : data.CrossStreet2;
                tranData.METERLOC_ENFORCEMENTHOURSDESC = (data.EnfHourDesc == null) ? string.Empty : data.EnfHourDesc;
                tranData.LicensePlate = (data.PlateNumber == null) ? string.Empty : data.PlateNumber;
                tranData.State = (data.LPLocation == null) ? string.Empty : data.LPLocation;
                tranData.Make = (data.VMake == null) ? string.Empty : data.VMake;
                tranData.Model = (data.VModel == null) ? string.Empty : data.VModel;
                tranData.IsOccupied = Utility.getStatus((data.SpaceStatus == null) ? string.Empty : data.SpaceStatus).ToString();
                tranData.EnforcementKey = (data.EnforcementKey == null) ? string.Empty : data.EnforcementKey;
                tranData.MeterStatus = (data.MeterStatus == null) ? null : data.MeterStatus;

                tranData.METERLOC_METERTYPE = (data.METERLOC_METERTYPE == null) ? null : data.METERLOC_METERTYPE;
                tranData.ZoneName = (data.ZoneName == null) ? null : data.ZoneName;

                tranData.MeterID = data.PayStationID.HasValue ? data.PayStationID.Value.ToString() : "0";
                tranData.ReturnCode = data.ReturnCode;

                logData.Response = new ResponseEntity() { CustomerId = param.CustomerId.Value.ToString(), MeterId = tranData.MeterName, SpaceId = tranData.SpaceNo, CityCurrentTime = data.PresentMeterTime.HasValue ? data.PresentMeterTime.Value : DateTime.Now };
            }
            catch (Exception ex)
            {
                //var appPath = System.Web.Hosting.HostingEnvironment.ApplicationPhysicalPath;
                //string errorPath = Path.Combine(appPath, "GetTransformData", "Timezones");
                //ExceptionLogging.SendErrorToText(ex, "GetCitationData", errorPath);
                //tranData.ReturnCode = ((int)ReturnCodeEnum.Error).ToString();
                tranData.ReturnCode = ((int)ReturnCodeEnum.Error).ToString();
            }
            #endregion
            #region "Parkmobile Service Call"
            /////////////////////////////////////////////////
            TransactionTransform tranDataOfServiceParkMobile = new TransactionTransform();
            try
            {
                //////////////////////////////////////////
                logData.Request.CustomerId = param.CustomerId.Value.ToString();
                logData.Request.SpaceId = param.parkingSpaceID;
                logData.Request.MeterId = param.MeterName;
                //////////////////////////////////////////////////
                DateTime? currentCustTime = customerCurrentTime.CurrentTime;
                PMCustomerElement customer = ParkmobileCustomerRetriever.GetCustomer(param.CustomerId.Value);
                ParkMobileGenericFactory factory = new ParkMobileGenericFactory(customer, param.PlateNumber);
                var parkMobileResLog = new VendorResponseEntity();
                logData.VendorData.Add(VendorNames.ParkMobile, parkMobileResLog);
                tranDataOfServiceParkMobile = factory.GetRefreshSpaceData(ParkingMobileDataType.Plate, ref parkMobileResLog);
                tranDataOfServiceParkMobile.EnforcementKey = EnforcementKey;

                ///////////////////////////////////////////////////
                logData.Response = new ResponseEntity() { CustomerId = param.CustomerId.Value.ToString(), MeterId = tranData.MeterName, SpaceId = tranData.SpaceNo, CityCurrentTime = currentCustTime.HasValue ? currentCustTime.Value : DateTime.Now };
            }
            catch (Exception ex)
            {
                logData.Response = new ResponseEntity() { CustomerId = param.CustomerId.Value.ToString(), MeterId = tranData.MeterName, SpaceId = tranData.SpaceNo, CityCurrentTime = customerCurrentTime.CurrentTime.HasValue ? customerCurrentTime.CurrentTime.Value : DateTime.Now };
                //throw ex;
                tranDataOfServiceParkMobile.ReturnCode = ((int)ReturnCodeEnum.Error).ToString();
            }
            finally
            {

            }
            #endregion
            #region T2Digital
            TransactionTransform tranDataOfServiceT2 = new TransactionTransform();
            try
            {
                //////////////////////////////////////////
                logData.Request.CustomerId = param.CustomerId.Value.ToString();
                logData.Request.SpaceId = param.parkingSpaceID;
                logData.Request.MeterId = param.MeterName;
                //////////////////////////////////////////////////
                DateTime? currentCustTime = customerCurrentTime.CurrentTime;
                T2Digital.T2DigitalCustomerElement customer = T2Digital.T2DigitalCustomerRetriever.GetCustomer(param.CustomerId.Value);
                T2Digital.T2DigitalGenericFactory factory = new T2Digital.T2DigitalGenericFactory(customer, param.PlateNumber, string.Empty);
                var t2ResLog = new VendorResponseEntity();
                logData.VendorData.Add(VendorNames.T2Digital, t2ResLog);
                tranDataOfServiceT2 = factory.GetRefreshPlateData(ref t2ResLog);
                tranDataOfServiceT2.EnforcementKey = EnforcementKey;

                ///////////////////////////////////////////////////
                logData.Response = new ResponseEntity() { CustomerId = param.CustomerId.Value.ToString(), MeterId = tranData.MeterName, SpaceId = tranData.SpaceNo, CityCurrentTime = currentCustTime.HasValue ? currentCustTime.Value : DateTime.Now };

            }
            catch (Exception ex)
            {
                logData.Response = new ResponseEntity() { CustomerId = param.CustomerId.Value.ToString(), MeterId = tranData.MeterName, SpaceId = tranData.SpaceNo, CityCurrentTime = customerCurrentTime.CurrentTime.HasValue ? customerCurrentTime.CurrentTime.Value : DateTime.Now };
                //throw ex;
                tranDataOfServiceT2.ReturnCode = ((int)ReturnCodeEnum.Error).ToString();
            }
            finally
            {

            }
            #endregion
            //////////////////  //Reconcile data

            List<TransactionTransform> trans = new List<TransactionTransform>();
            trans.Add(tranDataOfServiceParkMobile); // Parkmobile
            trans.Add(tranData); // Database
            trans.Add(tranDataOfServiceT2); // T2 Data

            var rightValue = trans.Where(t => t.ReturnCode == ((int)ReturnCodeEnum.Success).ToString()).OrderByDescending(t => t.MeterExpiredMinutes.ToNullableInt()).FirstOrDefault<TransactionTransform>();
            if (rightValue == default(TransactionTransform))
            {
                //Do nothing.just return database  object.
            }
            else
            {
                tranData = rightValue;
            }

            return tranData;
        }
        private static Transaction GetDataTempe(int customerID, string MeterName, string parkingSpaceID, string PlateNumber, string statename, string EnforcementKey, string filePath, RequestResponseEntity logData)
        {
            string ZoneID = null;
            string SpaceStatus = null;
            Transaction data = new Transaction();
            try
            {
                List<Transaction> transactionList = new List<Transaction>();

                logData.Request.CustomerId = customerID.ToString();
                logData.Request.SpaceId = parkingSpaceID;
                logData.Request.MeterId = MeterName;

                transactionList = DataAccess.GetTempeCitationData(customerID, null, MeterName, parkingSpaceID, PlateNumber, null);

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
            }
            catch (Exception ex)
            {
                var appPath = System.Web.Hosting.HostingEnvironment.ApplicationPhysicalPath;
                string errorPath = Path.Combine(appPath, "GetData", "Timezones");
                ExceptionLogging.SendErrorToText(ex, "GetCitationData", errorPath);
                data.ReturnCode = ((int)ReturnCodeEnum.Error).ToString();
                throw;
            }
            return data;
        }

        #endregion

        #region Miami Parking Authority

        public static TransactionTransform GetMiamiParkingAuthorityTransformData(string EnforcementKey, string filePath, RequestResponseEntity logData, CustomerTime customerCurrentTime)
        {
            TransactionTransform tranData = new TransactionTransform();
            tranData.ReturnCode = ((int)ReturnCodeEnum.InvalidInputParameters).ToString();
            TimeZoneInfo TZ = null;
            TZ = TimeZoneInfo.FindSystemTimeZoneById("Eastern Standard Time");

            //////////////////////////////////////
            KeyParameter param = PrepareKeyParameters.PrepareMiamiParkingAuthority(EnforcementKey);
            if (param.ReturnCode == ((int)ReturnCodeEnum.InvalidInputParameters).ToString())
            {
                return tranData;
            }


            #region "DB Check"
            /////////////////////////////////////
            try
            {
                Transaction data = TransactionDataValidatioin.GetDataMiamiParkingAuthority(param.CustomerId.Value, param.MeterName, param.parkingSpaceID, param.PlateNumber, param.StateName, EnforcementKey, filePath, logData);


                if (data.TimeZoneId.HasValue)
                    TZ = Utility.GetTimeZoneInfo(data.TimeZoneId.Value, filePath, param.CustomerId.Value.ToString());

                //For South Miami : PayStationId. Others it is MeterID
                tranData.MeterName = string.Empty; // (data.MeterName == null) ? string.Empty : data.MeterName;
                tranData.SpaceNo = string.Empty;  //data.ParkingSpaceID.ToString();

                tranData.MeterStreet = (data.MeterStreet == null) ? string.Empty : data.MeterStreet;

                DateTime _MeterRTC = data.PresentMeterTime.HasValue ? data.PresentMeterTime.Value : DateTime.Now;
                tranData.MeterRTC = String.Format("{0:yyyyMMdd_THHmmssZ}", Utility.findGMTTime(_MeterRTC, TZ));

                DateTime _SensorEventTime = data.SensorEventTime.HasValue ? data.SensorEventTime.Value : DateTime.Now;
                tranData.MeterLastUpdateTime = String.Format("{0:yyyyMMdd_THHmmssZ}", Utility.findGMTTime(_SensorEventTime, TZ));

                tranData.SensorEventTime = String.Format("{0:yyyyMMdd_THHmmssZ}", Utility.findGMTTime(_SensorEventTime, TZ));

                DateTime _endTime = data.ExpiryTime.HasValue ? data.ExpiryTime.Value : DateTime.Now;
                tranData.ExpiredTime = String.Format("{0:yyyyMMdd_THHmmssZ}", Utility.findGMTTime(_endTime, TZ));

                tranData.MeterExpiredMinutes = data.ExpiredMinutes.HasValue ? data.ExpiredMinutes.Value.ToString() : "0";
                tranData.METERLOC_BLOCKNUMBER = (data.Block == null) ? string.Empty : data.Block;
                tranData.METERLOC_STREETDIRECTION = (data.Direction == null) ? string.Empty : data.Direction;
                tranData.METERLOC_STREETNAME = (data.MeterStreet == null) ? string.Empty : data.MeterStreet;
                tranData.METERLOC_STREETTYPE = (data.StreetType == null) ? string.Empty : data.StreetType;
                tranData.METERLOC_CROSSSTREET1 = (data.CrossStreet1 == null) ? string.Empty : data.CrossStreet1;
                tranData.METERLOC_CROSSSTREET2 = (data.CrossStreet2 == null) ? string.Empty : data.CrossStreet2;
                tranData.METERLOC_ENFORCEMENTHOURSDESC = (data.EnfHourDesc == null) ? string.Empty : data.EnfHourDesc;
                tranData.LicensePlate = (data.PlateNumber == null) ? string.Empty : data.PlateNumber;
                tranData.State = (data.LPLocation == null) ? string.Empty : data.LPLocation;
                tranData.Make = (data.VMake == null) ? string.Empty : data.VMake;
                tranData.Model = (data.VModel == null) ? string.Empty : data.VModel;
                tranData.IsOccupied = Utility.getStatus((data.SpaceStatus == null) ? string.Empty : data.SpaceStatus).ToString();
                tranData.EnforcementKey = (data.EnforcementKey == null) ? string.Empty : data.EnforcementKey;
                tranData.MeterStatus = (data.MeterStatus == null) ? null : data.MeterStatus;

                tranData.METERLOC_METERTYPE = (data.METERLOC_METERTYPE == null) ? null : data.METERLOC_METERTYPE;
                tranData.ZoneName = (data.ZoneName == null) ? null : data.ZoneName;
                tranData.MeterID = String.Empty;
                tranData.ReturnCode = data.ReturnCode;

                logData.Response = new ResponseEntity() { CustomerId = param.CustomerId.Value.ToString(), MeterId = tranData.MeterName, SpaceId = tranData.SpaceNo, CityCurrentTime = data.PresentMeterTime.HasValue ? data.PresentMeterTime.Value : DateTime.Now };
            }
            catch (Exception ex)
            {
                ////Intentionally Error Condition Commented
                ////var appPath = System.Web.Hosting.HostingEnvironment.ApplicationPhysicalPath;
                ////string errorPath = Path.Combine(appPath, "GetTransformData", "Timezones");
                ////ExceptionLogging.SendErrorToText(ex, "GetCitationData", errorPath);
                ////tranData.ReturnCode = ((int)ReturnCodeEnum.Error).ToString();

                tranData.ReturnCode = ((int)ReturnCodeEnum.Error).ToString();
            }

            #endregion

            #region "Parkeon Service Call"
            /////////////////////////////////////////////////
            TransactionTransform tranDataOfService = null;
            List<TransactionTransform> tranDataOfServiceList = new List<TransactionTransform>();
            ServiceParameters serviceParam = null;
            ParkeonServiceAccess parkeonService = null;
            try
            {
                //////////////////////////////////////////
                logData.Request.CustomerId = param.CustomerId.Value.ToString();
                logData.Request.SpaceId = param.parkingSpaceID;
                logData.Request.MeterId = param.MeterName;
                //////////////////////////////////////////////////
                DateTime? currentCustTime = customerCurrentTime.CurrentTime;
                tranData.EnforcementKey = EnforcementKey;

                var customerList = ParkeonCustomerRetriever.GetCustomers(param.CustomerId.Value);
                foreach (var item in customerList)
                {
                    serviceParam = new ServiceParameters() { customerId = (Convert.ToInt64(item.CustomerId)), userName = item.UserName, userPassword = item.UserPassword, vendorId = (Convert.ToInt64(item.VendorId)) };
                    tranDataOfService = new TransactionTransform();
                    try
                    {
                        parkeonService = new ParkeonServiceAccess(serviceParam, param, item);
                        parkeonService.ProcessRequest(ref tranDataOfService, TypeConditions.check_plate, TZ, ref currentCustTime);
                        tranDataOfServiceList.Add(tranDataOfService);
                    }
                    catch (Exception ex)
                    {

                    }
                    ///////////////////////////////////////////////////
                    //logData.Response = new ResponseEntity() { CustomerId = param.CustomerId.Value.ToString(), MeterId = tranData.MeterName, SpaceId = tranData.SpaceNo, CityCurrentTime = currentCustTime.HasValue ? currentCustTime.Value : DateTime.Now };
                }
            }
            catch (Exception ex)
            {
                logData.Response = new ResponseEntity() { CustomerId = param.CustomerId.Value.ToString(), MeterId = tranData.MeterName, SpaceId = tranData.SpaceNo, CityCurrentTime = customerCurrentTime.CurrentTime.HasValue ? customerCurrentTime.CurrentTime.Value : DateTime.Now };
                //throw ex;
                tranDataOfService.ReturnCode = ((int)ReturnCodeEnum.Error).ToString();
            }
            finally
            {
                serviceParam = null;
            }
            #endregion



            //////////////////  //Reconcile data
            List<TransactionTransform> trans = new List<TransactionTransform>();
            trans.Add(tranData); // Database
            trans.AddRange(tranDataOfServiceList); //Parkeons

            var rightValue = trans.Where(t => t.ReturnCode == ((int)ReturnCodeEnum.Success).ToString()).OrderByDescending(t => t.MeterExpiredMinutes.ToNullableInt()).FirstOrDefault<TransactionTransform>();
            if (rightValue == default(TransactionTransform))
            {
                //Do nothing.just return database  object.
            }
            else
            {
                tranData = rightValue;
            }

            return tranData;
        }
        public static TransactionTransform GetMiamiParkingAuthorityTransformDataV2(string EnforcementKey, string filePath, RequestResponseEntity logData, CustomerTime customerCurrentTime)
        {
            TransactionTransform tranData = new TransactionTransform();
            tranData.ReturnCode = ((int)ReturnCodeEnum.InvalidInputParameters).ToString();
            TimeZoneInfo TZ = null;
            TZ = TimeZoneInfo.FindSystemTimeZoneById("Eastern Standard Time");

            //////////////////////////////////////
            KeyParameter param = PrepareKeyParameters.PrepareMiamiParkingAuthority(EnforcementKey);
            if (param.ReturnCode == ((int)ReturnCodeEnum.InvalidInputParameters).ToString())
            {
                return tranData;
            }

            #region "Parkeon Service Call"
            /////////////////////////////////////////////////
            TransactionTransform tranDataOfService = null;
            List<TransactionTransform> tranDataOfServiceList = new List<TransactionTransform>();
            ServiceParameters serviceParam = null;
            ParkeonServiceAccess parkeonService = null;
            try
            {
                //////////////////////////////////////////
                logData.Request.CustomerId = param.CustomerId.Value.ToString();
                logData.Request.SpaceId = param.parkingSpaceID;
                logData.Request.MeterId = param.MeterName;
                //////////////////////////////////////////////////
                DateTime? currentCustTime = customerCurrentTime.CurrentTime;
                tranData.EnforcementKey = EnforcementKey;

                var customerList = ParkeonCustomerRetriever.GetCustomers(param.CustomerId.Value);
                foreach (var item in customerList)
                {
                    serviceParam = new ServiceParameters() { customerId = (Convert.ToInt64(item.CustomerId)), userName = item.UserName, userPassword = item.UserPassword, vendorId = (Convert.ToInt64(item.VendorId)) };
                    tranDataOfService = new TransactionTransform();
                    try
                    {
                        parkeonService = new ParkeonServiceAccess(serviceParam, param, item);
                        parkeonService.ProcessRequest(ref tranDataOfService, TypeConditions.check_plate, TZ, ref currentCustTime);
                        tranDataOfServiceList.Add(tranDataOfService);
                    }
                    catch (Exception ex)
                    {

                    }
                    ///////////////////////////////////////////////////
                    //logData.Response = new ResponseEntity() { CustomerId = param.CustomerId.Value.ToString(), MeterId = tranData.MeterName, SpaceId = tranData.SpaceNo, CityCurrentTime = currentCustTime.HasValue ? currentCustTime.Value : DateTime.Now };
                }
            }
            catch (Exception ex)
            {
                logData.Response = new ResponseEntity() { CustomerId = param.CustomerId.Value.ToString(), MeterId = tranData.MeterName, SpaceId = tranData.SpaceNo, CityCurrentTime = customerCurrentTime.CurrentTime.HasValue ? customerCurrentTime.CurrentTime.Value : DateTime.Now };
                //throw ex;
                tranDataOfService.ReturnCode = ((int)ReturnCodeEnum.Error).ToString();
            }
            finally
            {
                serviceParam = null;
            }
            #endregion
            #region "PayByPhone Service Call"
            /////////////////////////////////////////////////
            TransactionTransform tranDataOfServicePBP = new TransactionTransform();
            try
            {
                //////////////////////////////////////////

                logData.Request.CustomerId = param.CustomerId.Value.ToString();
                logData.Request.SpaceId = param.parkingSpaceID;
                logData.Request.MeterId = param.MeterName;
                //////////////////////////////////////////////////
                DateTime? currentCustTime = customerCurrentTime.CurrentTime;
                CustomerElement customer = CustomerRetriever.GetCustomer(param.CustomerId.Value);
                PayByPhoneGenericFactory factory = new PayByPhoneGenericFactory(customer, param.PlateNumber);
                tranDataOfServicePBP = factory.GetRefreshPlateData();
                tranDataOfServicePBP.EnforcementKey = EnforcementKey;

                ///////////////////////////////////////////////////
                logData.Response = new ResponseEntity() { CustomerId = param.CustomerId.Value.ToString(), MeterId = tranData.MeterName, SpaceId = tranData.SpaceNo, CityCurrentTime = currentCustTime.HasValue ? currentCustTime.Value : DateTime.Now };

            }
            catch (Exception ex)
            {
                logData.Response = new ResponseEntity() { CustomerId = param.CustomerId.Value.ToString(), MeterId = tranData.MeterName, SpaceId = tranData.SpaceNo, CityCurrentTime = customerCurrentTime.CurrentTime.HasValue ? customerCurrentTime.CurrentTime.Value : DateTime.Now };
                //throw ex;
                tranDataOfServicePBP.ReturnCode = ((int)ReturnCodeEnum.Error).ToString();
            }
            finally
            {

            }
            #endregion

            //////////////////  //Reconcile data
            List<TransactionTransform> trans = new List<TransactionTransform>();
            //trans.Add(tranData); // Database
            trans.AddRange(tranDataOfServiceList); //Parkeons
            trans.Add(tranDataOfServicePBP); //PayByPhone

            var rightValue = trans.Where(t => t.ReturnCode == ((int)ReturnCodeEnum.Success).ToString()).OrderByDescending(t => t.MeterExpiredMinutes.ToNullableInt()).FirstOrDefault<TransactionTransform>();
            if (rightValue == default(TransactionTransform))
            {
                //Do nothing.just return PBC  object.
                return tranDataOfServicePBP;
            }
            else
            {
                tranData = rightValue;
            }

            return tranData;
        }
        public static TransactionTransform GetMiamiParkingAuthorityTransformDataV3(string EnforcementKey, string filePath, RequestResponseEntity logData, CustomerTime customerCurrentTime)
        {
            TransactionTransform tranData = new TransactionTransform();
            tranData.ReturnCode = ((int)ReturnCodeEnum.InvalidInputParameters).ToString();
            TimeZoneInfo TZ = null;
            TZ = TimeZoneInfo.FindSystemTimeZoneById("Eastern Standard Time");

            //////////////////////////////////////
            KeyParameter param = PrepareKeyParameters.PrepareMiamiParkingAuthority(EnforcementKey);
            if (param.ReturnCode == ((int)ReturnCodeEnum.InvalidInputParameters).ToString())
            {
                return tranData;
            }

            #region "Parkeon Service Call"
            /////////////////////////////////////////////////
            TransactionTransform tranDataOfService = null;
            List<TransactionTransform> tranDataOfServiceList = new List<TransactionTransform>();
            ServiceParameters serviceParam = null;
            ParkeonServiceAccess parkeonService = null;
            try
            {
                //////////////////////////////////////////
                logData.Request.CustomerId = param.CustomerId.Value.ToString();
                logData.Request.SpaceId = param.parkingSpaceID;
                logData.Request.MeterId = param.MeterName;
                //////////////////////////////////////////////////
                DateTime? currentCustTime = customerCurrentTime.CurrentTime;
                tranData.EnforcementKey = EnforcementKey;

                var customerList = ParkeonCustomerRetriever.GetCustomers(param.CustomerId.Value);
                foreach (var item in customerList)
                {
                    serviceParam = new ServiceParameters() { customerId = (Convert.ToInt64(item.CustomerId)), userName = item.UserName, userPassword = item.UserPassword, vendorId = (Convert.ToInt64(item.VendorId)) };
                    tranDataOfService = new TransactionTransform();
                    try
                    {
                        parkeonService = new ParkeonServiceAccess(serviceParam, param, item);
                        var parkeonResLog = new VendorResponseEntity();
                        logData.VendorData.Add(VendorNames.Parkeon, parkeonResLog);
                        parkeonService.ProcessRequest(ref tranDataOfService, TypeConditions.check_plate, TZ, ref currentCustTime, ref parkeonResLog);
                        tranDataOfService.EnforcementKey = EnforcementKey;
                        tranDataOfServiceList.Add(tranDataOfService);
                    }
                    catch (Exception ex)
                    {

                    }
                    ///////////////////////////////////////////////////
                    //logData.Response = new ResponseEntity() { CustomerId = param.CustomerId.Value.ToString(), MeterId = tranData.MeterName, SpaceId = tranData.SpaceNo, CityCurrentTime = currentCustTime.HasValue ? currentCustTime.Value : DateTime.Now };
                }
            }
            catch (Exception ex)
            {
                logData.Response = new ResponseEntity() { CustomerId = param.CustomerId.Value.ToString(), MeterId = tranData.MeterName, SpaceId = tranData.SpaceNo, CityCurrentTime = customerCurrentTime.CurrentTime.HasValue ? customerCurrentTime.CurrentTime.Value : DateTime.Now };
                //throw ex;
                tranDataOfService.ReturnCode = ((int)ReturnCodeEnum.Error).ToString();
            }
            finally
            {
                serviceParam = null;
            }
            #endregion
            #region "PayByPhone Service Call"
            /////////////////////////////////////////////////
            TransactionTransform tranDataOfServicePBP = new TransactionTransform();
            try
            {
                //////////////////////////////////////////

                logData.Request.CustomerId = param.CustomerId.Value.ToString();
                logData.Request.SpaceId = param.parkingSpaceID;
                logData.Request.MeterId = param.MeterName;
                //////////////////////////////////////////////////
                DateTime? currentCustTime = customerCurrentTime.CurrentTime;
                CustomerElement customer = CustomerRetriever.GetCustomer(param.CustomerId.Value);
                PayByPhoneGenericFactory factory = new PayByPhoneGenericFactory(customer, param.PlateNumber);
                var paybyPhoneResLog = new VendorResponseEntity();
                logData.VendorData.Add(VendorNames.PayByPhone, paybyPhoneResLog);
                tranDataOfServicePBP = factory.GetRefreshPlateData(ref paybyPhoneResLog);
                tranDataOfServicePBP.EnforcementKey = EnforcementKey;

                ///////////////////////////////////////////////////
                logData.Response = new ResponseEntity() { CustomerId = param.CustomerId.Value.ToString(), MeterId = tranData.MeterName, SpaceId = tranData.SpaceNo, CityCurrentTime = currentCustTime.HasValue ? currentCustTime.Value : DateTime.Now };

            }
            catch (Exception ex)
            {
                logData.Response = new ResponseEntity() { CustomerId = param.CustomerId.Value.ToString(), MeterId = tranData.MeterName, SpaceId = tranData.SpaceNo, CityCurrentTime = customerCurrentTime.CurrentTime.HasValue ? customerCurrentTime.CurrentTime.Value : DateTime.Now };
                //throw ex;
                tranDataOfServicePBP.ReturnCode = ((int)ReturnCodeEnum.Error).ToString();
            }
            finally
            {

            }
            #endregion

            //////////////////  //Reconcile data
            List<TransactionTransform> trans = new List<TransactionTransform>();
            //trans.Add(tranData); // Database
            trans.AddRange(tranDataOfServiceList); //Parkeons
            trans.Add(tranDataOfServicePBP); //PayByPhone

            var rightValue = trans.Where(t => t.ReturnCode == ((int)ReturnCodeEnum.Success).ToString()).OrderByDescending(t => t.MeterExpiredMinutes.ToNullableInt()).FirstOrDefault<TransactionTransform>();
            if (rightValue == default(TransactionTransform))
            {
                //Do nothing.just return PBC  object.
                return tranDataOfServicePBP;
            }
            else
            {
                tranData = rightValue;
            }

            return tranData;
        }
        private static Transaction GetDataMiamiParkingAuthority(int customerID, string MeterName, string parkingSpaceID, string PlateNumber, string statename, string EnforcementKey, string filePath, RequestResponseEntity logData)
        {
            string ZoneID = null;
            string SpaceStatus = null;
            Transaction data = new Transaction();
            try
            {
                List<Transaction> transactionList = new List<Transaction>();

                logData.Request.CustomerId = customerID.ToString();
                logData.Request.SpaceId = parkingSpaceID;
                logData.Request.MeterId = MeterName;

                transactionList = DataAccess.GetMiamiParkingAuthorityCitationData(customerID, null, null, null, PlateNumber, statename, null);

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
            }
            catch (Exception ex)
            {
                var appPath = System.Web.Hosting.HostingEnvironment.ApplicationPhysicalPath;
                string errorPath = Path.Combine(appPath, "GetData", "Timezones");
                ExceptionLogging.SendErrorToText(ex, "GetDataAtlanta", errorPath);
                data.ReturnCode = ((int)ReturnCodeEnum.Error).ToString();
                throw;
            }
            return data;
        }
        #endregion

        #region Abu Dhabi
        public static TransactionTransform GetAbuDhabiTransformData(string EnforcementKey, string filePath, RequestResponseEntity logData)
        {
            TransactionTransform tranData = new TransactionTransform();
            tranData.ReturnCode = ((int)ReturnCodeEnum.InvalidInputParameters).ToString();
            TimeZoneInfo TZ = null;
            TZ = TimeZoneInfo.FindSystemTimeZoneById("Eastern Standard Time");
            //int? customerID = null;
            //string MeterName = null;
            //string parkingSpaceID;
            //string PlateNumber = null;
            //string stateName = null;
            //////////////////////////////////////
            KeyParameter param = PrepareKeyParameters.PrepareAbuDhabi(EnforcementKey);
            if (param.ReturnCode == ((int)ReturnCodeEnum.InvalidInputParameters).ToString())
            {
                return tranData;
            }

            /////////////////////////////////////
            try
            {
                Transaction data = TransactionDataValidatioin.GetDataAbuDhabi(param.CustomerId.Value, param.MeterName, param.parkingSpaceID, param.PlateNumber, param.StateName, EnforcementKey, filePath, logData);


                if (data.TimeZoneId.HasValue)
                    TZ = Utility.GetTimeZoneInfo(data.TimeZoneId.Value, filePath, param.CustomerId.Value.ToString());

                //For South Miami : PayStationId. Others it is MeterID
                tranData.MeterName = (data.MeterName == null) ? string.Empty : data.MeterName;
                tranData.SpaceNo = data.ParkingSpaceID.ToString();

                tranData.MeterStreet = (data.MeterStreet == null) ? string.Empty : data.MeterStreet;

                DateTime _MeterRTC = data.PresentMeterTime.HasValue ? data.PresentMeterTime.Value : DateTime.Now;
                tranData.MeterRTC = String.Format("{0:yyyyMMdd_THHmmssZ}", Utility.findGMTTime(_MeterRTC, TZ));

                DateTime _SensorEventTime = data.SensorEventTime.HasValue ? data.SensorEventTime.Value : DateTime.Now;
                tranData.MeterLastUpdateTime = String.Format("{0:yyyyMMdd_THHmmssZ}", Utility.findGMTTime(_SensorEventTime, TZ));

                tranData.SensorEventTime = String.Format("{0:yyyyMMdd_THHmmssZ}", Utility.findGMTTime(_SensorEventTime, TZ));

                DateTime _endTime = data.ExpiryTime.HasValue ? data.ExpiryTime.Value : DateTime.Now;
                tranData.ExpiredTime = String.Format("{0:yyyyMMdd_THHmmssZ}", Utility.findGMTTime(_endTime, TZ));

                tranData.MeterExpiredMinutes = data.ExpiredMinutes.HasValue ? data.ExpiredMinutes.Value.ToString() : "0";
                tranData.METERLOC_BLOCKNUMBER = (data.Block == null) ? string.Empty : data.Block;
                tranData.METERLOC_STREETDIRECTION = (data.Direction == null) ? string.Empty : data.Direction;
                tranData.METERLOC_STREETNAME = (data.MeterStreet == null) ? string.Empty : data.MeterStreet;
                tranData.METERLOC_STREETTYPE = (data.StreetType == null) ? string.Empty : data.StreetType;
                tranData.METERLOC_CROSSSTREET1 = (data.CrossStreet1 == null) ? string.Empty : data.CrossStreet1;
                tranData.METERLOC_CROSSSTREET2 = (data.CrossStreet2 == null) ? string.Empty : data.CrossStreet2;
                tranData.METERLOC_ENFORCEMENTHOURSDESC = (data.EnfHourDesc == null) ? string.Empty : data.EnfHourDesc;
                tranData.LicensePlate = (data.PlateNumber == null) ? string.Empty : data.PlateNumber;
                tranData.State = (data.LPLocation == null) ? string.Empty : data.LPLocation;
                tranData.Make = (data.VMake == null) ? string.Empty : data.VMake;
                tranData.Model = (data.VModel == null) ? string.Empty : data.VModel;
                tranData.IsOccupied = Utility.getStatus((data.SpaceStatus == null) ? string.Empty : data.SpaceStatus).ToString();
                tranData.EnforcementKey = (data.EnforcementKey == null) ? string.Empty : data.EnforcementKey;
                tranData.MeterStatus = (data.MeterStatus == null) ? null : data.MeterStatus;

                tranData.METERLOC_METERTYPE = (data.METERLOC_METERTYPE == null) ? null : data.METERLOC_METERTYPE;
                tranData.ZoneName = (data.ZoneName == null) ? null : data.ZoneName;

                tranData.MeterID = data.PayStationID.HasValue ? data.PayStationID.Value.ToString() : "0";
                tranData.ReturnCode = data.ReturnCode;

                logData.Response = new ResponseEntity() { CustomerId = param.CustomerId.Value.ToString(), MeterId = tranData.MeterName, SpaceId = tranData.SpaceNo, CityCurrentTime = data.PresentMeterTime.HasValue ? data.PresentMeterTime.Value : DateTime.Now };
            }
            catch (Exception ex)
            {
                var appPath = System.Web.Hosting.HostingEnvironment.ApplicationPhysicalPath;
                string errorPath = Path.Combine(appPath, "GetTransformData", "Timezones");
                ExceptionLogging.SendErrorToText(ex, "GetCitationData", errorPath);
                tranData.ReturnCode = ((int)ReturnCodeEnum.Error).ToString();
            }
            return tranData;
        }
        private static Transaction GetDataAbuDhabi(int customerID, string MeterName, string parkingSpaceID, string PlateNumber, string statename, string EnforcementKey, string filePath, RequestResponseEntity logData)
        {
            string ZoneID = null;
            string SpaceStatus = null;
            Transaction data = new Transaction();
            try
            {
                List<Transaction> transactionList = new List<Transaction>();

                logData.Request.CustomerId = customerID.ToString();
                logData.Request.SpaceId = parkingSpaceID;
                logData.Request.MeterId = MeterName;

                transactionList = DataAccess.GetAbuDhabiCitationData(customerID, null, MeterName, parkingSpaceID, null, null);

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
            }
            catch (Exception ex)
            {
                var appPath = System.Web.Hosting.HostingEnvironment.ApplicationPhysicalPath;
                string errorPath = Path.Combine(appPath, "GetData", "Timezones");
                ExceptionLogging.SendErrorToText(ex, "GetCitationData", errorPath);
                data.ReturnCode = ((int)ReturnCodeEnum.Error).ToString();
                throw;
            }
            return data;
        }

        #endregion

        #region Charleston
        public static TransactionTransform GeCharlestonTransformData(string EnforcementKey, string filePath, RequestResponseEntity logData)
        {
            TransactionTransform tranData = new TransactionTransform();
            tranData.ReturnCode = ((int)ReturnCodeEnum.InvalidInputParameters).ToString();
            TimeZoneInfo TZ = null;
            TZ = TimeZoneInfo.FindSystemTimeZoneById("Eastern Standard Time");
            //int? customerID = null;
            //string MeterName = null;
            //string parkingSpaceID;
            //string PlateNumber = null;
            //string stateName = null;
            //////////////////////////////////////
            KeyParameter param = PrepareKeyParameters.PrepareAbuDhabi(EnforcementKey);
            if (param.ReturnCode == ((int)ReturnCodeEnum.InvalidInputParameters).ToString())
            {
                return tranData;
            }

            /////////////////////////////////////
            try
            {
                Transaction data = TransactionDataValidatioin.GetDataCharleston(param.CustomerId.Value, param.MeterName, param.parkingSpaceID, param.PlateNumber, param.StateName, EnforcementKey, filePath, logData);


                if (data.TimeZoneId.HasValue)
                    TZ = Utility.GetTimeZoneInfo(data.TimeZoneId.Value, filePath, param.CustomerId.Value.ToString());

                //For South Miami : PayStationId. Others it is MeterID
                tranData.MeterName = (data.MeterName == null) ? string.Empty : data.MeterName;
                tranData.SpaceNo = data.ParkingSpaceID.ToString();

                tranData.MeterStreet = (data.MeterStreet == null) ? string.Empty : data.MeterStreet;

                DateTime _MeterRTC = data.PresentMeterTime.HasValue ? data.PresentMeterTime.Value : DateTime.Now;
                tranData.MeterRTC = String.Format("{0:yyyyMMdd_THHmmssZ}", Utility.findGMTTime(_MeterRTC, TZ));

                DateTime _SensorEventTime = data.SensorEventTime.HasValue ? data.SensorEventTime.Value : DateTime.Now;
                tranData.MeterLastUpdateTime = String.Format("{0:yyyyMMdd_THHmmssZ}", Utility.findGMTTime(_SensorEventTime, TZ));

                tranData.SensorEventTime = String.Format("{0:yyyyMMdd_THHmmssZ}", Utility.findGMTTime(_SensorEventTime, TZ));

                DateTime _endTime = data.ExpiryTime.HasValue ? data.ExpiryTime.Value : DateTime.Now;
                tranData.ExpiredTime = String.Format("{0:yyyyMMdd_THHmmssZ}", Utility.findGMTTime(_endTime, TZ));

                tranData.MeterExpiredMinutes = data.ExpiredMinutes.HasValue ? data.ExpiredMinutes.Value.ToString() : "0";
                tranData.METERLOC_BLOCKNUMBER = (data.Block == null) ? string.Empty : data.Block;
                tranData.METERLOC_STREETDIRECTION = (data.Direction == null) ? string.Empty : data.Direction;
                tranData.METERLOC_STREETNAME = (data.MeterStreet == null) ? string.Empty : data.MeterStreet;
                tranData.METERLOC_STREETTYPE = (data.StreetType == null) ? string.Empty : data.StreetType;
                tranData.METERLOC_CROSSSTREET1 = (data.CrossStreet1 == null) ? string.Empty : data.CrossStreet1;
                tranData.METERLOC_CROSSSTREET2 = (data.CrossStreet2 == null) ? string.Empty : data.CrossStreet2;
                tranData.METERLOC_ENFORCEMENTHOURSDESC = (data.EnfHourDesc == null) ? string.Empty : data.EnfHourDesc;
                tranData.LicensePlate = (data.PlateNumber == null) ? string.Empty : data.PlateNumber;
                tranData.State = (data.LPLocation == null) ? string.Empty : data.LPLocation;
                tranData.Make = (data.VMake == null) ? string.Empty : data.VMake;
                tranData.Model = (data.VModel == null) ? string.Empty : data.VModel;
                tranData.IsOccupied = Utility.getStatus((data.SpaceStatus == null) ? string.Empty : data.SpaceStatus).ToString();
                tranData.EnforcementKey = (data.EnforcementKey == null) ? string.Empty : data.EnforcementKey;
                tranData.MeterStatus = (data.MeterStatus == null) ? null : data.MeterStatus;

                tranData.METERLOC_METERTYPE = (data.METERLOC_METERTYPE == null) ? null : data.METERLOC_METERTYPE;
                tranData.ZoneName = (data.ZoneName == null) ? null : data.ZoneName;

                tranData.MeterID = data.PayStationID.HasValue ? data.PayStationID.Value.ToString() : "0";
                tranData.ReturnCode = data.ReturnCode;

                logData.Response = new ResponseEntity() { CustomerId = param.CustomerId.Value.ToString(), MeterId = tranData.MeterName, SpaceId = tranData.SpaceNo, CityCurrentTime = data.PresentMeterTime.HasValue ? data.PresentMeterTime.Value : DateTime.Now };
            }
            catch (Exception ex)
            {
                var appPath = System.Web.Hosting.HostingEnvironment.ApplicationPhysicalPath;
                string errorPath = Path.Combine(appPath, "GetTransformData", "Timezones");
                ExceptionLogging.SendErrorToText(ex, "GetCitationData", errorPath);
                tranData.ReturnCode = ((int)ReturnCodeEnum.Error).ToString();
            }
            return tranData;
        }
        private static Transaction GetDataCharleston(int customerID, string MeterName, string parkingSpaceID, string PlateNumber, string statename, string EnforcementKey, string filePath, RequestResponseEntity logData)
        {
            string ZoneID = null;
            string SpaceStatus = null;
            Transaction data = new Transaction();
            try
            {
                List<Transaction> transactionList = new List<Transaction>();

                logData.Request.CustomerId = customerID.ToString();
                logData.Request.SpaceId = parkingSpaceID;
                logData.Request.MeterId = MeterName;

                transactionList = DataAccess.GetCharlestonCitationData(customerID, null, MeterName, parkingSpaceID, null, null);

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
            }
            catch (Exception ex)
            {
                var appPath = System.Web.Hosting.HostingEnvironment.ApplicationPhysicalPath;
                string errorPath = Path.Combine(appPath, "GetData", "Timezones");
                ExceptionLogging.SendErrorToText(ex, "GetCitationData", errorPath);
                data.ReturnCode = ((int)ReturnCodeEnum.Error).ToString();
                throw;
            }
            return data;
        }

        #endregion

        #region Ardsley,NY
        public static TransactionTransform GetArdsleyNYTransformData(string EnforcementKey, string filePath, RequestResponseEntity logData, CustomerTime customerCurrentTime)
        {
            TransactionTransform tranData = new TransactionTransform();
            tranData.ReturnCode = ((int)ReturnCodeEnum.InvalidInputParameters).ToString();
            TimeZoneInfo TZ = null;
            TZ = TimeZoneInfo.FindSystemTimeZoneById("Eastern Standard Time");
            //int? customerID = null;
            //string MeterName = null;
            //string parkingSpaceID;
            //string PlateNumber = null;
            //string stateName = null;
            //////////////////////////////////////
            KeyParameter param = PrepareKeyParameters.PrepareArdsleyNY(EnforcementKey);
            if (param.ReturnCode == ((int)ReturnCodeEnum.InvalidInputParameters).ToString())
            {
                return tranData;
            }

            /////////////////////////////////////
            #region "DB Check"
            try
            {
                Transaction data = TransactionDataValidatioin.GetDataArdsleyNY(param.CustomerId.Value, param.MeterName, param.parkingSpaceID, param.PlateNumber, param.StateName, EnforcementKey, filePath, logData);


                if (data.TimeZoneId.HasValue)
                    TZ = Utility.GetTimeZoneInfo(data.TimeZoneId.Value, filePath, param.CustomerId.Value.ToString());

                //For South Miami : PayStationId. Others it is MeterID
                tranData.MeterName = (data.MeterName == null) ? string.Empty : data.MeterName;
                tranData.SpaceNo = data.ParkingSpaceID.ToString();

                tranData.MeterStreet = (data.MeterStreet == null) ? string.Empty : data.MeterStreet;

                DateTime _MeterRTC = data.PresentMeterTime.HasValue ? data.PresentMeterTime.Value : DateTime.Now;
                tranData.MeterRTC = String.Format("{0:yyyyMMdd_THHmmssZ}", Utility.findGMTTime(_MeterRTC, TZ));

                DateTime _SensorEventTime = data.SensorEventTime.HasValue ? data.SensorEventTime.Value : DateTime.Now;
                tranData.MeterLastUpdateTime = String.Format("{0:yyyyMMdd_THHmmssZ}", Utility.findGMTTime(_SensorEventTime, TZ));

                tranData.SensorEventTime = String.Format("{0:yyyyMMdd_THHmmssZ}", Utility.findGMTTime(_SensorEventTime, TZ));

                DateTime _endTime = data.ExpiryTime.HasValue ? data.ExpiryTime.Value : DateTime.Now;
                tranData.ExpiredTime = String.Format("{0:yyyyMMdd_THHmmssZ}", Utility.findGMTTime(_endTime, TZ));

                tranData.MeterExpiredMinutes = data.ExpiredMinutes.HasValue ? data.ExpiredMinutes.Value.ToString() : "0";
                tranData.METERLOC_BLOCKNUMBER = (data.Block == null) ? string.Empty : data.Block;
                tranData.METERLOC_STREETDIRECTION = (data.Direction == null) ? string.Empty : data.Direction;
                tranData.METERLOC_STREETNAME = (data.MeterStreet == null) ? string.Empty : data.MeterStreet;
                tranData.METERLOC_STREETTYPE = (data.StreetType == null) ? string.Empty : data.StreetType;
                tranData.METERLOC_CROSSSTREET1 = (data.CrossStreet1 == null) ? string.Empty : data.CrossStreet1;
                tranData.METERLOC_CROSSSTREET2 = (data.CrossStreet2 == null) ? string.Empty : data.CrossStreet2;
                tranData.METERLOC_ENFORCEMENTHOURSDESC = (data.EnfHourDesc == null) ? string.Empty : data.EnfHourDesc;
                tranData.LicensePlate = (data.PlateNumber == null) ? string.Empty : data.PlateNumber;
                tranData.State = (data.LPLocation == null) ? string.Empty : data.LPLocation;
                tranData.Make = (data.VMake == null) ? string.Empty : data.VMake;
                tranData.Model = (data.VModel == null) ? string.Empty : data.VModel;
                tranData.IsOccupied = Utility.getStatus((data.SpaceStatus == null) ? string.Empty : data.SpaceStatus).ToString();
                tranData.EnforcementKey = (data.EnforcementKey == null) ? string.Empty : data.EnforcementKey;
                tranData.MeterStatus = (data.MeterStatus == null) ? null : data.MeterStatus;

                tranData.METERLOC_METERTYPE = (data.METERLOC_METERTYPE == null) ? null : data.METERLOC_METERTYPE;
                tranData.ZoneName = (data.ZoneName == null) ? null : data.ZoneName;

                tranData.MeterID = data.PayStationID.HasValue ? data.PayStationID.Value.ToString() : "0";
                tranData.ReturnCode = data.ReturnCode;

                logData.Response = new ResponseEntity() { CustomerId = param.CustomerId.Value.ToString(), MeterId = tranData.MeterName, SpaceId = tranData.SpaceNo, CityCurrentTime = data.PresentMeterTime.HasValue ? data.PresentMeterTime.Value : DateTime.Now };
            }


            catch (Exception ex)
            {
                var appPath = System.Web.Hosting.HostingEnvironment.ApplicationPhysicalPath;
                string errorPath = Path.Combine(appPath, "GetTransformData", "Timezones");
                ExceptionLogging.SendErrorToText(ex, "GetCitationData", errorPath);
                tranData.ReturnCode = ((int)ReturnCodeEnum.Error).ToString();
                // tranData.ReturnCode = ((int)ReturnCodeEnum.Error).ToString();
            }
            #endregion



            return tranData;
        }
        private static Transaction GetDataArdsleyNY(int customerID, string MeterName, string parkingSpaceID, string PlateNumber, string statename, string EnforcementKey, string filePath, RequestResponseEntity logData)
        {
            string ZoneID = null;
            string SpaceStatus = null;
            Transaction data = new Transaction();
            try
            {
                List<Transaction> transactionList = new List<Transaction>();

                logData.Request.CustomerId = customerID.ToString();
                logData.Request.SpaceId = parkingSpaceID;
                logData.Request.MeterId = MeterName;

                transactionList = DataAccess.GetArdsleyNYCitationData(customerID, null, MeterName, parkingSpaceID, null, null);

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
            }
            catch (Exception ex)
            {
                var appPath = System.Web.Hosting.HostingEnvironment.ApplicationPhysicalPath;
                string errorPath = Path.Combine(appPath, "GetData", "Timezones");
                ExceptionLogging.SendErrorToText(ex, "GetCitationData", errorPath);
                data.ReturnCode = ((int)ReturnCodeEnum.Error).ToString();
                throw;
            }
            return data;
        }

        #endregion

        #region MiamiBeach
        public static TransactionTransform GetMiamiBeachTransformData(string EnforcementKey, string filePath, RequestResponseEntity logData, CustomerTime customerCurrentTime)
        {
            TransactionTransform tranData = new TransactionTransform();
            tranData.ReturnCode = ((int)ReturnCodeEnum.InvalidInputParameters).ToString();
            TimeZoneInfo TZ = null;
            TZ = TimeZoneInfo.FindSystemTimeZoneById("Eastern Standard Time");
            //int? customerID = null;
            //string MeterName = null;
            //string parkingSpaceID;
            //string PlateNumber = null;
            //string stateName = null;
            //////////////////////////////////////
            KeyParameter param = PrepareKeyParameters.PrepareSurfside(EnforcementKey);
            if (param.ReturnCode == ((int)ReturnCodeEnum.InvalidInputParameters).ToString())
            {
                return tranData;
            }

            /////////////////////////////////////

            #region T2Digital
            TransactionTransform tranDataOfServiceT2 = new TransactionTransform();
            try
            {
                //////////////////////////////////////////
                logData.Request.CustomerId = param.CustomerId.Value.ToString();
                logData.Request.SpaceId = param.parkingSpaceID;
                logData.Request.MeterId = param.MeterName;
                //////////////////////////////////////////////////
                DateTime? currentCustTime = customerCurrentTime.CurrentTime;
                T2Digital.T2DigitalCustomerElement customer = T2Digital.T2DigitalCustomerRetriever.GetCustomer(param.CustomerId.Value);
                T2Digital.T2DigitalGenericFactory factory = new T2Digital.T2DigitalGenericFactory(customer, param.PlateNumber, string.Empty);
                var t2ResLog = new VendorResponseEntity();
                logData.VendorData.Add(VendorNames.T2Digital, t2ResLog);
                tranDataOfServiceT2 = factory.GetRefreshPlateData(ref t2ResLog);
                tranDataOfServiceT2.EnforcementKey = EnforcementKey;

                ///////////////////////////////////////////////////
                logData.Response = new ResponseEntity() { CustomerId = param.CustomerId.Value.ToString(), MeterId = tranData.MeterName, SpaceId = tranData.SpaceNo, CityCurrentTime = currentCustTime.HasValue ? currentCustTime.Value : DateTime.Now };

            }
            catch (Exception ex)
            {
                logData.Response = new ResponseEntity() { CustomerId = param.CustomerId.Value.ToString(), MeterId = tranData.MeterName, SpaceId = tranData.SpaceNo, CityCurrentTime = customerCurrentTime.CurrentTime.HasValue ? customerCurrentTime.CurrentTime.Value : DateTime.Now };
                //throw ex;
                tranDataOfServiceT2.ReturnCode = ((int)ReturnCodeEnum.Error).ToString();
            }
            finally
            {

            }
            #endregion
            //////////////////  //Reconcile data
            List<TransactionTransform> trans = new List<TransactionTransform>();
            trans.Add(tranDataOfServiceT2); // T2Digital

            var rightValue = trans.Where(t => t.ReturnCode == ((int)ReturnCodeEnum.Success).ToString()).OrderByDescending(t => t.MeterExpiredMinutes.ToNullableInt()).FirstOrDefault<TransactionTransform>();
            if (rightValue == default(TransactionTransform))
            {
                //Do nothing.just return database  object.
            }
            else
            {
                tranData = rightValue;
            }
            return tranData;
        }
        private static Transaction GetDataMiamiBeach(int customerID, string MeterName, string parkingSpaceID, string PlateNumber, string statename, string EnforcementKey, string filePath, RequestResponseEntity logData)
        {
            string ZoneID = null;
            string SpaceStatus = null;
            Transaction data = new Transaction();
            try
            {
                List<Transaction> transactionList = new List<Transaction>();

                logData.Request.CustomerId = customerID.ToString();
                logData.Request.SpaceId = parkingSpaceID;
                logData.Request.MeterId = MeterName;

                transactionList = DataAccess.GetSurfsideBeachCitationData(customerID, null, MeterName, parkingSpaceID, PlateNumber, statename, null);

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
            }
            catch (Exception ex)
            {
                var appPath = System.Web.Hosting.HostingEnvironment.ApplicationPhysicalPath;
                string errorPath = Path.Combine(appPath, "GetData", "Timezones");
                ExceptionLogging.SendErrorToText(ex, "GetCitationData", errorPath);
                data.ReturnCode = ((int)ReturnCodeEnum.Error).ToString();
                throw;
            }
            return data;
        }

        #endregion

        #region SpokaneWA
        public static TransactionTransform GetSpokaneWATransformData(string EnforcementKey, string filePath, RequestResponseEntity logData, CustomerTime customerCurrentTime)
        {
            TransactionTransform tranData = new TransactionTransform();
            tranData.ReturnCode = ((int)ReturnCodeEnum.InvalidInputParameters).ToString();
            TimeZoneInfo TZ = null;
            TZ = TimeZoneInfo.FindSystemTimeZoneById("Eastern Standard Time");
            //int? customerID = null;
            //string MeterName = null;
            //string parkingSpaceID;
            //string PlateNumber = null;
            //string stateName = null;
            //////////////////////////////////////
            KeyParameter param = PrepareKeyParameters.PrepareSpokaneWA(EnforcementKey);
            if (param.ReturnCode == ((int)ReturnCodeEnum.InvalidInputParameters).ToString())
            {
                return tranData;
            }

            #region "DB Check"

            /////////////////////////////////////
            try
            {
                Transaction data = TransactionDataValidatioin.GetDataSpokaneWA(param.CustomerId.Value, param.MeterName, param.parkingSpaceID, param.PlateNumber, param.StateName, EnforcementKey, filePath, logData);


                if (data.TimeZoneId.HasValue)
                    TZ = Utility.GetTimeZoneInfo(data.TimeZoneId.Value, filePath, param.CustomerId.Value.ToString());

                //For South Miami : PayStationId. Others it is MeterID
                tranData.MeterName = string.Empty; // (data.MeterName == null) ? string.Empty : data.MeterName;
                tranData.SpaceNo = string.Empty;  //data.ParkingSpaceID.ToString();

                tranData.MeterStreet = (data.MeterStreet == null) ? string.Empty : data.MeterStreet;

                DateTime _MeterRTC = data.PresentMeterTime.HasValue ? data.PresentMeterTime.Value : DateTime.Now;
                tranData.MeterRTC = String.Format("{0:yyyyMMdd_THHmmssZ}", Utility.findGMTTime(_MeterRTC, TZ));

                DateTime _SensorEventTime = data.SensorEventTime.HasValue ? data.SensorEventTime.Value : DateTime.Now;
                tranData.MeterLastUpdateTime = String.Format("{0:yyyyMMdd_THHmmssZ}", Utility.findGMTTime(_SensorEventTime, TZ));

                tranData.SensorEventTime = String.Format("{0:yyyyMMdd_THHmmssZ}", Utility.findGMTTime(_SensorEventTime, TZ));

                DateTime _endTime = data.ExpiryTime.HasValue ? data.ExpiryTime.Value : DateTime.Now;
                tranData.ExpiredTime = String.Format("{0:yyyyMMdd_THHmmssZ}", Utility.findGMTTime(_endTime, TZ));

                tranData.MeterExpiredMinutes = data.ExpiredMinutes.HasValue ? data.ExpiredMinutes.Value.ToString() : "0";
                tranData.METERLOC_BLOCKNUMBER = (data.Block == null) ? string.Empty : data.Block;
                tranData.METERLOC_STREETDIRECTION = (data.Direction == null) ? string.Empty : data.Direction;
                tranData.METERLOC_STREETNAME = (data.MeterStreet == null) ? string.Empty : data.MeterStreet;
                tranData.METERLOC_STREETTYPE = (data.StreetType == null) ? string.Empty : data.StreetType;
                tranData.METERLOC_CROSSSTREET1 = (data.CrossStreet1 == null) ? string.Empty : data.CrossStreet1;
                tranData.METERLOC_CROSSSTREET2 = (data.CrossStreet2 == null) ? string.Empty : data.CrossStreet2;
                tranData.METERLOC_ENFORCEMENTHOURSDESC = (data.EnfHourDesc == null) ? string.Empty : data.EnfHourDesc;
                tranData.LicensePlate = (data.PlateNumber == null) ? string.Empty : data.PlateNumber;
                tranData.State = (data.LPLocation == null) ? string.Empty : data.LPLocation;
                tranData.Make = (data.VMake == null) ? string.Empty : data.VMake;
                tranData.Model = (data.VModel == null) ? string.Empty : data.VModel;
                tranData.IsOccupied = Utility.getStatus((data.SpaceStatus == null) ? string.Empty : data.SpaceStatus).ToString();
                tranData.EnforcementKey = (data.EnforcementKey == null) ? string.Empty : data.EnforcementKey;
                tranData.MeterStatus = (data.MeterStatus == null) ? null : data.MeterStatus;

                tranData.METERLOC_METERTYPE = (data.METERLOC_METERTYPE == null) ? null : data.METERLOC_METERTYPE;
                tranData.ZoneName = (data.ZoneName == null) ? null : data.ZoneName;
                tranData.MeterID = String.Empty;
                tranData.ReturnCode = data.ReturnCode;

                logData.Response = new ResponseEntity() { CustomerId = param.CustomerId.Value.ToString(), MeterId = tranData.MeterName, SpaceId = tranData.SpaceNo, CityCurrentTime = data.PresentMeterTime.HasValue ? data.PresentMeterTime.Value : DateTime.Now };
            }
            catch (Exception ex)
            {
                ////Intentionally Error Condition Commented
                ////var appPath = System.Web.Hosting.HostingEnvironment.ApplicationPhysicalPath;
                ////string errorPath = Path.Combine(appPath, "GetTransformData", "Timezones");
                ////ExceptionLogging.SendErrorToText(ex, "GetCitationData", errorPath);
                ////tranData.ReturnCode = ((int)ReturnCodeEnum.Error).ToString();

                tranData.ReturnCode = ((int)ReturnCodeEnum.Error).ToString();
            }

            #endregion
            #region "Parkeon Service Call"
            /////////////////////////////////////////////////
            TransactionTransform tranDataOfService = new TransactionTransform();
            ServiceParameters serviceParam = null;
            try
            {
                //////////////////////////////////////////
                logData.Request.CustomerId = param.CustomerId.Value.ToString();
                logData.Request.SpaceId = param.parkingSpaceID;
                logData.Request.MeterId = param.MeterName;
                //////////////////////////////////////////////////
                DateTime? currentCustTime = customerCurrentTime.CurrentTime;
                tranDataOfService.EnforcementKey = EnforcementKey;
                ParkeonCustomerElement customer = ParkeonCustomerRetriever.GetCustomer(param.CustomerId.Value);
                serviceParam = new ServiceParameters() { customerId = (Convert.ToInt64(customer.CustomerId)), userName = customer.UserName, userPassword = customer.UserPassword, vendorId = (Convert.ToInt64(customer.VendorId)) };
                ParkeonServiceAccess parkeonService = new ParkeonServiceAccess(serviceParam, param, customer);
                var parkeonResLog = new VendorResponseEntity();
                logData.VendorData.Add(VendorNames.Parkeon, parkeonResLog);
                parkeonService.ProcessRequest(ref tranDataOfService, TypeConditions.check_plate, TZ, ref currentCustTime, ref parkeonResLog);
                serviceParam = null;
                ///////////////////////////////////////////////////
                logData.Response = new ResponseEntity() { CustomerId = param.CustomerId.Value.ToString(), MeterId = tranData.MeterName, SpaceId = tranData.SpaceNo, CityCurrentTime = currentCustTime.HasValue ? currentCustTime.Value : DateTime.Now };

            }
            catch (Exception ex)
            {
                logData.Response = new ResponseEntity() { CustomerId = param.CustomerId.Value.ToString(), MeterId = tranData.MeterName, SpaceId = tranData.SpaceNo, CityCurrentTime = customerCurrentTime.CurrentTime.HasValue ? customerCurrentTime.CurrentTime.Value : DateTime.Now };
                //throw ex;
                tranDataOfService.ReturnCode = ((int)ReturnCodeEnum.Error).ToString();
            }
            finally
            {
                serviceParam = null;
            }
            #endregion
            #region "Passport Monitoring"
            /////////////////////////////////////////////////
            TransactionTransform tranDataOfServiceSpokan = new TransactionTransform();
            try
            {
                //////////////////////////////////////////
                logData.Request.CustomerId = param.CustomerId.Value.ToString();
                logData.Request.SpaceId = param.parkingSpaceID;
                logData.Request.MeterId = param.MeterName;
                //////////////////////////////////////////////////
                DateTime? currentCustTime = customerCurrentTime.CurrentTime;
                PassportMonitoringCustomerElement customer = PassportMonitoringCustomerRetriever.GetCustomer(param.CustomerId.Value);
                LookUpLPNFactory factory = new LookUpLPNFactory(customer, param.PlateNumber);
                var passportMonResLog = new VendorResponseEntity();
                logData.VendorData.Add(VendorNames.PassportMonitoring, passportMonResLog);
                tranDataOfServiceSpokan = factory.GetRefreshPlateData(ref passportMonResLog);
                tranDataOfServiceSpokan.EnforcementKey = EnforcementKey;

                ///////////////////////////////////////////////////
                logData.Response = new ResponseEntity() { CustomerId = param.CustomerId.Value.ToString(), MeterId = tranData.MeterName, SpaceId = tranData.SpaceNo, CityCurrentTime = currentCustTime.HasValue ? currentCustTime.Value : DateTime.Now };
            }
            catch (Exception ex)
            {
                logData.Response = new ResponseEntity() { CustomerId = param.CustomerId.Value.ToString(), MeterId = tranData.MeterName, SpaceId = tranData.SpaceNo, CityCurrentTime = customerCurrentTime.CurrentTime.HasValue ? customerCurrentTime.CurrentTime.Value : DateTime.Now };
                //throw ex;
                tranData.ReturnCode = ((int)ReturnCodeEnum.Error).ToString();
            }
            finally
            {

            }
            #endregion
            //////////////////  //Reconcile data
            List<TransactionTransform> trans = new List<TransactionTransform>();
            trans.Add(tranData); // Database
            trans.Add(tranDataOfService); //Parkeon
            trans.Add(tranDataOfServiceSpokan); //Passport-Monitoring

            var rightValue = trans.Where(t => t.ReturnCode == ((int)ReturnCodeEnum.Success).ToString()).OrderByDescending(t => t.MeterExpiredMinutes.ToNullableInt()).FirstOrDefault<TransactionTransform>();
            if (rightValue == default(TransactionTransform))
            {
                //Do nothing.just return database  object.
            }
            else
            {
                tranData = rightValue;
            }

            return tranData;
        }
        private static Transaction GetDataSpokaneWA(int customerID, string MeterName, string parkingSpaceID, string PlateNumber, string statename, string EnforcementKey, string filePath, RequestResponseEntity logData)
        {
            string ZoneID = null;
            string SpaceStatus = null;
            Transaction data = new Transaction();
            try
            {
                List<Transaction> transactionList = new List<Transaction>();

                logData.Request.CustomerId = customerID.ToString();
                logData.Request.SpaceId = parkingSpaceID;
                logData.Request.MeterId = MeterName;

                transactionList = DataAccess.GetDataSpokaneWACitationData(customerID, null, null, null, PlateNumber, null, null);

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
            }
            catch (Exception ex)
            {
                var appPath = System.Web.Hosting.HostingEnvironment.ApplicationPhysicalPath;
                string errorPath = Path.Combine(appPath, "GetData", "Timezones");
                ExceptionLogging.SendErrorToText(ex, "GetCitationData", errorPath);
                data.ReturnCode = ((int)ReturnCodeEnum.Error).ToString();
                throw;
            }
            return data;
        }

        #endregion

        #region Huntsville
        public static TransactionTransform GetHuntsvilleTransformData(string EnforcementKey, string filePath, RequestResponseEntity logData, CustomerTime customerCurrentTime)
        {
            TransactionTransform tranData = new TransactionTransform();
            tranData.ReturnCode = ((int)ReturnCodeEnum.InvalidInputParameters).ToString();
            TimeZoneInfo TZ = null;
            TZ = TimeZoneInfo.FindSystemTimeZoneById("Eastern Standard Time");
            //int? customerID = null;
            //string MeterName = null;
            //string parkingSpaceID;
            //string PlateNumber = null;
            //string stateName = null;
            //////////////////////////////////////
            KeyParameter param = PrepareKeyParameters.PrepareHuntsville(EnforcementKey);
            if (param.ReturnCode == ((int)ReturnCodeEnum.InvalidInputParameters).ToString())
            {
                return tranData;
            }

            /////////////////////////////////////
            #region "DB Check"
            /////////////////////////////////////
            try
            {
                Transaction data = TransactionDataValidatioin.GetDataHuntsville(param.CustomerId.Value, param.MeterName, param.parkingSpaceID, param.PlateNumber, param.StateName, EnforcementKey, filePath, logData);


                if (data.TimeZoneId.HasValue)
                    TZ = Utility.GetTimeZoneInfo(data.TimeZoneId.Value, filePath, param.CustomerId.Value.ToString());

                //For South Miami : PayStationId. Others it is MeterID
                tranData.MeterName = string.Empty; // (data.MeterName == null) ? string.Empty : data.MeterName;
                tranData.SpaceNo = string.Empty;  //data.ParkingSpaceID.ToString();

                tranData.MeterStreet = (data.MeterStreet == null) ? string.Empty : data.MeterStreet;

                DateTime _MeterRTC = data.PresentMeterTime.HasValue ? data.PresentMeterTime.Value : DateTime.Now;
                tranData.MeterRTC = String.Format("{0:yyyyMMdd_THHmmssZ}", Utility.findGMTTime(_MeterRTC, TZ));

                DateTime _SensorEventTime = data.SensorEventTime.HasValue ? data.SensorEventTime.Value : DateTime.Now;
                tranData.MeterLastUpdateTime = String.Format("{0:yyyyMMdd_THHmmssZ}", Utility.findGMTTime(_SensorEventTime, TZ));

                tranData.SensorEventTime = String.Format("{0:yyyyMMdd_THHmmssZ}", Utility.findGMTTime(_SensorEventTime, TZ));

                DateTime _endTime = data.ExpiryTime.HasValue ? data.ExpiryTime.Value : DateTime.Now;
                tranData.ExpiredTime = String.Format("{0:yyyyMMdd_THHmmssZ}", Utility.findGMTTime(_endTime, TZ));

                tranData.MeterExpiredMinutes = data.ExpiredMinutes.HasValue ? data.ExpiredMinutes.Value.ToString() : "0";
                tranData.METERLOC_BLOCKNUMBER = (data.Block == null) ? string.Empty : data.Block;
                tranData.METERLOC_STREETDIRECTION = (data.Direction == null) ? string.Empty : data.Direction;
                tranData.METERLOC_STREETNAME = (data.MeterStreet == null) ? string.Empty : data.MeterStreet;
                tranData.METERLOC_STREETTYPE = (data.StreetType == null) ? string.Empty : data.StreetType;
                tranData.METERLOC_CROSSSTREET1 = (data.CrossStreet1 == null) ? string.Empty : data.CrossStreet1;
                tranData.METERLOC_CROSSSTREET2 = (data.CrossStreet2 == null) ? string.Empty : data.CrossStreet2;
                tranData.METERLOC_ENFORCEMENTHOURSDESC = (data.EnfHourDesc == null) ? string.Empty : data.EnfHourDesc;
                tranData.LicensePlate = (data.PlateNumber == null) ? string.Empty : data.PlateNumber;
                tranData.State = (data.LPLocation == null) ? string.Empty : data.LPLocation;
                tranData.Make = (data.VMake == null) ? string.Empty : data.VMake;
                tranData.Model = (data.VModel == null) ? string.Empty : data.VModel;
                tranData.IsOccupied = Utility.getStatus((data.SpaceStatus == null) ? string.Empty : data.SpaceStatus).ToString();
                tranData.EnforcementKey = (data.EnforcementKey == null) ? string.Empty : data.EnforcementKey;
                tranData.MeterStatus = (data.MeterStatus == null) ? null : data.MeterStatus;

                tranData.METERLOC_METERTYPE = (data.METERLOC_METERTYPE == null) ? null : data.METERLOC_METERTYPE;
                tranData.ZoneName = (data.ZoneName == null) ? null : data.ZoneName;
                tranData.MeterID = String.Empty;
                tranData.ReturnCode = data.ReturnCode;

                logData.Response = new ResponseEntity() { CustomerId = param.CustomerId.Value.ToString(), MeterId = tranData.MeterName, SpaceId = tranData.SpaceNo, CityCurrentTime = data.PresentMeterTime.HasValue ? data.PresentMeterTime.Value : DateTime.Now };
            }
            catch (Exception ex)
            {
                ////Intentionally Error Condition Commented
                ////var appPath = System.Web.Hosting.HostingEnvironment.ApplicationPhysicalPath;
                ////string errorPath = Path.Combine(appPath, "GetTransformData", "Timezones");
                ////ExceptionLogging.SendErrorToText(ex, "GetCitationData", errorPath);
                ////tranData.ReturnCode = ((int)ReturnCodeEnum.Error).ToString();

                tranData.ReturnCode = ((int)ReturnCodeEnum.Error).ToString();
            }

            #endregion
            #region T2Digital
            TransactionTransform tranDataOfServiceT2 = new TransactionTransform();
            try
            {
                //////////////////////////////////////////
                logData.Request.CustomerId = param.CustomerId.Value.ToString();
                logData.Request.SpaceId = param.parkingSpaceID;
                logData.Request.MeterId = param.MeterName;
                //////////////////////////////////////////////////
                DateTime? currentCustTime = customerCurrentTime.CurrentTime;
                T2Digital.T2DigitalCustomerElement customer = T2Digital.T2DigitalCustomerRetriever.GetCustomer(param.CustomerId.Value);
                T2Digital.T2DigitalGenericFactory factory = new T2Digital.T2DigitalGenericFactory(customer, param.PlateNumber, string.Empty);
                var t2ResLog = new VendorResponseEntity();
                logData.VendorData.Add(VendorNames.T2Digital, t2ResLog);
                tranDataOfServiceT2 = factory.GetRefreshPlateData(ref t2ResLog);
                tranDataOfServiceT2.EnforcementKey = EnforcementKey;

                ///////////////////////////////////////////////////
                logData.Response = new ResponseEntity() { CustomerId = param.CustomerId.Value.ToString(), MeterId = tranData.MeterName, SpaceId = tranData.SpaceNo, CityCurrentTime = currentCustTime.HasValue ? currentCustTime.Value : DateTime.Now };

            }
            catch (Exception ex)
            {
                logData.Response = new ResponseEntity() { CustomerId = param.CustomerId.Value.ToString(), MeterId = tranData.MeterName, SpaceId = tranData.SpaceNo, CityCurrentTime = customerCurrentTime.CurrentTime.HasValue ? customerCurrentTime.CurrentTime.Value : DateTime.Now };
                //throw ex;
                tranDataOfServiceT2.ReturnCode = ((int)ReturnCodeEnum.Error).ToString();
            }
            finally
            {

            }
            #endregion
            #region "Passport Monitoring"
            /////////////////////////////////////////////////
            TransactionTransform tranDataOfService = new TransactionTransform();
            try
            {
                //////////////////////////////////////////
                logData.Request.CustomerId = param.CustomerId.Value.ToString();
                logData.Request.SpaceId = param.parkingSpaceID;
                logData.Request.MeterId = param.MeterName;
                //////////////////////////////////////////////////
                DateTime? currentCustTime = customerCurrentTime.CurrentTime;
                PassportMonitoringCustomerElement customer = PassportMonitoringCustomerRetriever.GetCustomer(param.CustomerId.Value);
                LookUpLPNFactory factory = new LookUpLPNFactory(customer, param.PlateNumber);
                var passportMonResLog = new VendorResponseEntity();
                logData.VendorData.Add(VendorNames.PassportMonitoring, passportMonResLog);
                tranDataOfService = factory.GetRefreshPlateData(ref passportMonResLog);
                tranDataOfService.EnforcementKey = EnforcementKey;

                ///////////////////////////////////////////////////
                logData.Response = new ResponseEntity() { CustomerId = param.CustomerId.Value.ToString(), MeterId = tranData.MeterName, SpaceId = tranData.SpaceNo, CityCurrentTime = currentCustTime.HasValue ? currentCustTime.Value : DateTime.Now };
            }
            catch (Exception ex)
            {
                logData.Response = new ResponseEntity() { CustomerId = param.CustomerId.Value.ToString(), MeterId = tranData.MeterName, SpaceId = tranData.SpaceNo, CityCurrentTime = customerCurrentTime.CurrentTime.HasValue ? customerCurrentTime.CurrentTime.Value : DateTime.Now };
                //throw ex;
                tranData.ReturnCode = ((int)ReturnCodeEnum.Error).ToString();
            }
            finally
            {

            }
            #endregion

            ////////////////////Reconcile data
            List<TransactionTransform> trans = new List<TransactionTransform>();
            trans.Add(tranData); // Database
            trans.Add(tranDataOfServiceT2); // T2Digital
            trans.Add(tranDataOfService); //Passport

            var rightValue = trans.Where(t => t.ReturnCode == ((int)ReturnCodeEnum.Success).ToString()).OrderByDescending(t => t.MeterExpiredMinutes.ToNullableInt()).FirstOrDefault<TransactionTransform>();
            if (rightValue == default(TransactionTransform))
            {
                //Do nothing.just return database  object.
            }
            else
            {
                tranData = rightValue;
            }
            return tranData;
        }
        private static Transaction GetDataHuntsville(int customerID, string MeterName, string parkingSpaceID, string PlateNumber, string statename, string EnforcementKey, string filePath, RequestResponseEntity logData)
        {
            string ZoneID = null;
            string SpaceStatus = null;
            Transaction data = new Transaction();
            try
            {
                List<Transaction> transactionList = new List<Transaction>();

                logData.Request.CustomerId = customerID.ToString();
                logData.Request.SpaceId = parkingSpaceID;
                logData.Request.MeterId = MeterName;

                transactionList = DataAccess.GetHuntsvilleCitationData(customerID, null, null, null, PlateNumber, statename, null);

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
            }
            catch (Exception ex)
            {
                var appPath = System.Web.Hosting.HostingEnvironment.ApplicationPhysicalPath;
                string errorPath = Path.Combine(appPath, "GetData", "Timezones");
                ExceptionLogging.SendErrorToText(ex, "GetDataAtlanta", errorPath);
                data.ReturnCode = ((int)ReturnCodeEnum.Error).ToString();
                throw;
            }
            return data;
        }

        #endregion

        #region Ace Parking
        public static TransactionTransform GetAceParkingTransformData(string EnforcementKey, string filePath, RequestResponseEntity logData, CustomerTime customerCurrentTime)
        {
            TransactionTransform tranData = new TransactionTransform();
            tranData.ReturnCode = ((int)ReturnCodeEnum.InvalidInputParameters).ToString();
            TimeZoneInfo TZ = null;
            TZ = TimeZoneInfo.FindSystemTimeZoneById("Eastern Standard Time");

            KeyParameter param = PrepareKeyParameters.PrepareAceParking(EnforcementKey);
            if (param.ReturnCode == ((int)ReturnCodeEnum.InvalidInputParameters).ToString())
            {
                return tranData;
            }

            /////////////////////////////////////
            #region "DB Check"
            ///////////////////////////////////////
            //try
            //{
            //    Transaction data = TransactionDataValidatioin.GetDataHuntsville(param.CustomerId.Value, param.MeterName, param.parkingSpaceID, param.PlateNumber, param.StateName, EnforcementKey, filePath, logData);


            //    if (data.TimeZoneId.HasValue)
            //        TZ = Utility.GetTimeZoneInfo(data.TimeZoneId.Value, filePath, param.CustomerId.Value.ToString());

            //    //For South Miami : PayStationId. Others it is MeterID
            //    tranData.MeterName = string.Empty; // (data.MeterName == null) ? string.Empty : data.MeterName;
            //    tranData.SpaceNo = string.Empty;  //data.ParkingSpaceID.ToString();

            //    tranData.MeterStreet = (data.MeterStreet == null) ? string.Empty : data.MeterStreet;

            //    DateTime _MeterRTC = data.PresentMeterTime.HasValue ? data.PresentMeterTime.Value : DateTime.Now;
            //    tranData.MeterRTC = String.Format("{0:yyyyMMdd_THHmmssZ}", Utility.findGMTTime(_MeterRTC, TZ));

            //    DateTime _SensorEventTime = data.SensorEventTime.HasValue ? data.SensorEventTime.Value : DateTime.Now;
            //    tranData.MeterLastUpdateTime = String.Format("{0:yyyyMMdd_THHmmssZ}", Utility.findGMTTime(_SensorEventTime, TZ));

            //    tranData.SensorEventTime = String.Format("{0:yyyyMMdd_THHmmssZ}", Utility.findGMTTime(_SensorEventTime, TZ));

            //    DateTime _endTime = data.ExpiryTime.HasValue ? data.ExpiryTime.Value : DateTime.Now;
            //    tranData.ExpiredTime = String.Format("{0:yyyyMMdd_THHmmssZ}", Utility.findGMTTime(_endTime, TZ));

            //    tranData.MeterExpiredMinutes = data.ExpiredMinutes.HasValue ? data.ExpiredMinutes.Value.ToString() : "0";
            //    tranData.METERLOC_BLOCKNUMBER = (data.Block == null) ? string.Empty : data.Block;
            //    tranData.METERLOC_STREETDIRECTION = (data.Direction == null) ? string.Empty : data.Direction;
            //    tranData.METERLOC_STREETNAME = (data.MeterStreet == null) ? string.Empty : data.MeterStreet;
            //    tranData.METERLOC_STREETTYPE = (data.StreetType == null) ? string.Empty : data.StreetType;
            //    tranData.METERLOC_CROSSSTREET1 = (data.CrossStreet1 == null) ? string.Empty : data.CrossStreet1;
            //    tranData.METERLOC_CROSSSTREET2 = (data.CrossStreet2 == null) ? string.Empty : data.CrossStreet2;
            //    tranData.METERLOC_ENFORCEMENTHOURSDESC = (data.EnfHourDesc == null) ? string.Empty : data.EnfHourDesc;
            //    tranData.LicensePlate = (data.PlateNumber == null) ? string.Empty : data.PlateNumber;
            //    tranData.State = (data.LPLocation == null) ? string.Empty : data.LPLocation;
            //    tranData.Make = (data.VMake == null) ? string.Empty : data.VMake;
            //    tranData.Model = (data.VModel == null) ? string.Empty : data.VModel;
            //    tranData.IsOccupied = Utility.getStatus((data.SpaceStatus == null) ? string.Empty : data.SpaceStatus).ToString();
            //    tranData.EnforcementKey = (data.EnforcementKey == null) ? string.Empty : data.EnforcementKey;
            //    tranData.MeterStatus = (data.MeterStatus == null) ? null : data.MeterStatus;

            //    tranData.METERLOC_METERTYPE = (data.METERLOC_METERTYPE == null) ? null : data.METERLOC_METERTYPE;
            //    tranData.ZoneName = (data.ZoneName == null) ? null : data.ZoneName;
            //    tranData.MeterID = String.Empty;
            //    tranData.ReturnCode = data.ReturnCode;

            //    logData.Response = new ResponseEntity() { CustomerId = param.CustomerId.Value.ToString(), MeterId = tranData.MeterName, SpaceId = tranData.SpaceNo, CityCurrentTime = data.PresentMeterTime.HasValue ? data.PresentMeterTime.Value : DateTime.Now };
            //}
            //catch (Exception ex)
            //{
            //    ////Intentionally Error Condition Commented
            //    ////var appPath = System.Web.Hosting.HostingEnvironment.ApplicationPhysicalPath;
            //    ////string errorPath = Path.Combine(appPath, "GetTransformData", "Timezones");
            //    ////ExceptionLogging.SendErrorToText(ex, "GetCitationData", errorPath);
            //    ////tranData.ReturnCode = ((int)ReturnCodeEnum.Error).ToString();

            //    tranData.ReturnCode = ((int)ReturnCodeEnum.Error).ToString();
            //}

            #endregion
            #region "Ace Parking"
            /////////////////////////////////////////////////
            TransactionTransform tranDataOfService = new TransactionTransform();
            try
            {
                //////////////////////////////////////////
                logData.Request.CustomerId = param.CustomerId.Value.ToString();
                logData.Request.SpaceId = param.parkingSpaceID;
                logData.Request.MeterId = param.MeterName;
                //////////////////////////////////////////////////
                DateTime? currentCustTime = customerCurrentTime.CurrentTime;
                AceParkingCustomerElement customer = AceParkingCustomerRetriever.GetCustomer(param.CustomerId.Value);
                AceParkingHttpClient factory = new AceParkingHttpClient(customer, param.PlateNumber);
                var aceParkingResLog = new VendorResponseEntity();
                logData.VendorData.Add(VendorNames.AceParking, aceParkingResLog);
                tranDataOfService = factory.GetPlateData(ref aceParkingResLog);
                tranDataOfService.EnforcementKey = EnforcementKey;

                ///////////////////////////////////////////////////
                logData.Response = new ResponseEntity() { CustomerId = param.CustomerId.Value.ToString(), MeterId = tranData.MeterName, SpaceId = tranData.SpaceNo, CityCurrentTime = currentCustTime.HasValue ? currentCustTime.Value : DateTime.Now };
            }
            catch (Exception ex)
            {
                logData.Response = new ResponseEntity() { CustomerId = param.CustomerId.Value.ToString(), MeterId = tranData.MeterName, SpaceId = tranData.SpaceNo, CityCurrentTime = customerCurrentTime.CurrentTime.HasValue ? customerCurrentTime.CurrentTime.Value : DateTime.Now };
                //throw ex;
                tranData.ReturnCode = ((int)ReturnCodeEnum.Error).ToString();
            }
            finally
            {

            }
            #endregion

            ////////////////////Reconcile data
            List<TransactionTransform> trans = new List<TransactionTransform>();
            trans.Add(tranDataOfService); //Ace Parking

            var rightValue = trans.Where(t => t.ReturnCode == ((int)ReturnCodeEnum.Success).ToString()).OrderByDescending(t => t.MeterExpiredMinutes.ToNullableInt()).FirstOrDefault<TransactionTransform>();
            if (rightValue == default(TransactionTransform))
            {
                //Do nothing.just return database  object.
            }
            else
            {
                tranData = rightValue;
            }
            return tranData;
        }
        private static Transaction GetDataTest(int customerID, string MeterName, string parkingSpaceID, string PlateNumber, string statename, string EnforcementKey, string filePath, RequestResponseEntity logData)
        {
            string ZoneID = null;
            string SpaceStatus = null;
            Transaction data = new Transaction();
            try
            {
                List<Transaction> transactionList = new List<Transaction>();

                logData.Request.CustomerId = customerID.ToString();
                logData.Request.SpaceId = parkingSpaceID;
                logData.Request.MeterId = MeterName;

                transactionList = DataAccess.GetHuntsvilleCitationData(customerID, null, null, null, PlateNumber, statename, null);

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
            }
            catch (Exception ex)
            {
                var appPath = System.Web.Hosting.HostingEnvironment.ApplicationPhysicalPath;
                string errorPath = Path.Combine(appPath, "GetData", "Timezones");
                ExceptionLogging.SendErrorToText(ex, "GetDataAtlanta", errorPath);
                data.ReturnCode = ((int)ReturnCodeEnum.Error).ToString();
                throw;
            }
            return data;
        }

        #endregion

        #region CityofChester
        public static TransactionTransform GetCityofChesterData(string EnforcementKey, string filePath, RequestResponseEntity logData)
        {
            TransactionTransform tranData = new TransactionTransform();
            tranData.ReturnCode = ((int)ReturnCodeEnum.InvalidInputParameters).ToString();
            TimeZoneInfo TZ = null;
            TZ = TimeZoneInfo.FindSystemTimeZoneById("Eastern Standard Time");
            //int? customerID = null;
            //string MeterName = null;
            //string parkingSpaceID;
            //string PlateNumber = null;
            //string stateName = null;
            //////////////////////////////////////
            KeyParameter param = PrepareKeyParameters.GenericBySpace(EnforcementKey);
            if (param.ReturnCode == ((int)ReturnCodeEnum.InvalidInputParameters).ToString())
            {
                return tranData;
            }
            /////////////////////////////////////
            try
            {
                Transaction data = TransactionDataValidatioin.GetDataCityofChester(param.CustomerId.Value, param.MeterName, param.parkingSpaceID, param.PlateNumber, param.StateName, EnforcementKey, filePath, logData);


                if (data.TimeZoneId.HasValue)
                    TZ = Utility.GetTimeZoneInfo(data.TimeZoneId.Value, filePath, param.CustomerId.Value.ToString());

                //For South Miami : PayStationId. Others it is MeterID
                tranData.MeterName = (data.MeterName == null) ? string.Empty : data.MeterName;
                tranData.SpaceNo = data.ParkingSpaceID.ToString();

                tranData.MeterStreet = (data.MeterStreet == null) ? string.Empty : data.MeterStreet;

                DateTime _MeterRTC = data.PresentMeterTime.HasValue ? data.PresentMeterTime.Value : DateTime.Now;
                tranData.MeterRTC = String.Format("{0:yyyyMMdd_THHmmssZ}", Utility.findGMTTime(_MeterRTC, TZ));

                DateTime _SensorEventTime = data.SensorEventTime.HasValue ? data.SensorEventTime.Value : DateTime.Now;
                tranData.MeterLastUpdateTime = String.Format("{0:yyyyMMdd_THHmmssZ}", Utility.findGMTTime(_SensorEventTime, TZ));

                tranData.SensorEventTime = String.Format("{0:yyyyMMdd_THHmmssZ}", Utility.findGMTTime(_SensorEventTime, TZ));

                DateTime _endTime = data.ExpiryTime.HasValue ? data.ExpiryTime.Value : DateTime.Now;
                tranData.ExpiredTime = String.Format("{0:yyyyMMdd_THHmmssZ}", Utility.findGMTTime(_endTime, TZ));

                tranData.MeterExpiredMinutes = data.ExpiredMinutes.HasValue ? data.ExpiredMinutes.Value.ToString() : "0";
                tranData.METERLOC_BLOCKNUMBER = (data.Block == null) ? string.Empty : data.Block;
                tranData.METERLOC_STREETDIRECTION = (data.Direction == null) ? string.Empty : data.Direction;
                tranData.METERLOC_STREETNAME = (data.MeterStreet == null) ? string.Empty : data.MeterStreet;
                tranData.METERLOC_STREETTYPE = (data.StreetType == null) ? string.Empty : data.StreetType;
                tranData.METERLOC_CROSSSTREET1 = (data.CrossStreet1 == null) ? string.Empty : data.CrossStreet1;
                tranData.METERLOC_CROSSSTREET2 = (data.CrossStreet2 == null) ? string.Empty : data.CrossStreet2;
                tranData.METERLOC_ENFORCEMENTHOURSDESC = (data.EnfHourDesc == null) ? string.Empty : data.EnfHourDesc;
                tranData.LicensePlate = (data.PlateNumber == null) ? string.Empty : data.PlateNumber;
                tranData.State = (data.LPLocation == null) ? string.Empty : data.LPLocation;
                tranData.Make = (data.VMake == null) ? string.Empty : data.VMake;
                tranData.Model = (data.VModel == null) ? string.Empty : data.VModel;
                tranData.IsOccupied = Utility.getStatus((data.SpaceStatus == null) ? string.Empty : data.SpaceStatus).ToString();
                tranData.EnforcementKey = (data.EnforcementKey == null) ? string.Empty : data.EnforcementKey;
                tranData.MeterStatus = (data.MeterStatus == null) ? null : data.MeterStatus;

                tranData.METERLOC_METERTYPE = (data.METERLOC_METERTYPE == null) ? null : data.METERLOC_METERTYPE;
                tranData.ZoneName = (data.ZoneName == null) ? null : data.ZoneName;

                tranData.MeterID = data.PayStationID.HasValue ? data.PayStationID.Value.ToString() : "0";
                tranData.ReturnCode = data.ReturnCode;

                logData.Response = new ResponseEntity() { CustomerId = param.CustomerId.Value.ToString(), MeterId = tranData.MeterName, SpaceId = tranData.SpaceNo, CityCurrentTime = data.PresentMeterTime.HasValue ? data.PresentMeterTime.Value : DateTime.Now };
            }
            catch (Exception ex)
            {
                var appPath = System.Web.Hosting.HostingEnvironment.ApplicationPhysicalPath;
                string errorPath = Path.Combine(appPath, "GetTransformData", "Timezones");
                ExceptionLogging.SendErrorToText(ex, "GetCitationData", errorPath);
                tranData.ReturnCode = ((int)ReturnCodeEnum.Error).ToString();

            }
            return tranData;
        }
        public static TransactionTransform GetCityofChesterDataV2(string EnforcementKey, string filePath, RequestResponseEntity logData, CustomerTime customerCurrentTime)
        {
            TransactionTransform tranData = new TransactionTransform();
            tranData.ReturnCode = ((int)ReturnCodeEnum.InvalidInputParameters).ToString();
            TimeZoneInfo TZ = null;
            TZ = TimeZoneInfo.FindSystemTimeZoneById("Eastern Standard Time");
            //int? customerID = null;
            //string MeterName = null;
            //string parkingSpaceID;
            //string PlateNumber = null;
            //string stateName = null;
            //////////////////////////////////////
            KeyParameter param = PrepareKeyParameters.PrepareCityofChester(EnforcementKey);
            if (param.ReturnCode == ((int)ReturnCodeEnum.InvalidInputParameters).ToString())
            {
                return tranData;
            }
            #region DB
            /////////////////////////////////////
            try
            {
                Transaction data = TransactionDataValidatioin.GetDataCityofChester(param.CustomerId.Value, param.MeterName, param.parkingSpaceID, param.PlateNumber, param.StateName, EnforcementKey, filePath, logData);


                if (data.TimeZoneId.HasValue)
                    TZ = Utility.GetTimeZoneInfo(data.TimeZoneId.Value, filePath, param.CustomerId.Value.ToString());

                //For South Miami : PayStationId. Others it is MeterID
                tranData.MeterName = (data.MeterName == null) ? string.Empty : data.MeterName;
                tranData.SpaceNo = data.ParkingSpaceID.ToString();

                tranData.MeterStreet = (data.MeterStreet == null) ? string.Empty : data.MeterStreet;

                DateTime _MeterRTC = data.PresentMeterTime.HasValue ? data.PresentMeterTime.Value : DateTime.Now;
                tranData.MeterRTC = String.Format("{0:yyyyMMdd_THHmmssZ}", Utility.findGMTTime(_MeterRTC, TZ));

                DateTime _SensorEventTime = data.SensorEventTime.HasValue ? data.SensorEventTime.Value : DateTime.Now;
                tranData.MeterLastUpdateTime = String.Format("{0:yyyyMMdd_THHmmssZ}", Utility.findGMTTime(_SensorEventTime, TZ));

                tranData.SensorEventTime = String.Format("{0:yyyyMMdd_THHmmssZ}", Utility.findGMTTime(_SensorEventTime, TZ));

                DateTime _endTime = data.ExpiryTime.HasValue ? data.ExpiryTime.Value : DateTime.Now;
                tranData.ExpiredTime = String.Format("{0:yyyyMMdd_THHmmssZ}", Utility.findGMTTime(_endTime, TZ));

                tranData.MeterExpiredMinutes = data.ExpiredMinutes.HasValue ? data.ExpiredMinutes.Value.ToString() : "0";
                tranData.METERLOC_BLOCKNUMBER = (data.Block == null) ? string.Empty : data.Block;
                tranData.METERLOC_STREETDIRECTION = (data.Direction == null) ? string.Empty : data.Direction;
                tranData.METERLOC_STREETNAME = (data.MeterStreet == null) ? string.Empty : data.MeterStreet;
                tranData.METERLOC_STREETTYPE = (data.StreetType == null) ? string.Empty : data.StreetType;
                tranData.METERLOC_CROSSSTREET1 = (data.CrossStreet1 == null) ? string.Empty : data.CrossStreet1;
                tranData.METERLOC_CROSSSTREET2 = (data.CrossStreet2 == null) ? string.Empty : data.CrossStreet2;
                tranData.METERLOC_ENFORCEMENTHOURSDESC = (data.EnfHourDesc == null) ? string.Empty : data.EnfHourDesc;
                tranData.LicensePlate = (data.PlateNumber == null) ? string.Empty : data.PlateNumber;
                tranData.State = (data.LPLocation == null) ? string.Empty : data.LPLocation;
                tranData.Make = (data.VMake == null) ? string.Empty : data.VMake;
                tranData.Model = (data.VModel == null) ? string.Empty : data.VModel;
                tranData.IsOccupied = Utility.getStatus((data.SpaceStatus == null) ? string.Empty : data.SpaceStatus).ToString();
                tranData.EnforcementKey = (data.EnforcementKey == null) ? string.Empty : data.EnforcementKey;
                tranData.MeterStatus = (data.MeterStatus == null) ? null : data.MeterStatus;

                tranData.METERLOC_METERTYPE = (data.METERLOC_METERTYPE == null) ? null : data.METERLOC_METERTYPE;
                tranData.ZoneName = (data.ZoneName == null) ? null : data.ZoneName;

                tranData.MeterID = data.PayStationID.HasValue ? data.PayStationID.Value.ToString() : "0";
                tranData.ReturnCode = data.ReturnCode;

                logData.Response = new ResponseEntity() { CustomerId = param.CustomerId.Value.ToString(), MeterId = tranData.MeterName, SpaceId = tranData.SpaceNo, CityCurrentTime = data.PresentMeterTime.HasValue ? data.PresentMeterTime.Value : DateTime.Now };
            }
            catch (Exception ex)
            {
                //var appPath = System.Web.Hosting.HostingEnvironment.ApplicationPhysicalPath;
                //string errorPath = Path.Combine(appPath, "GetTransformData", "Timezones");
                //ExceptionLogging.SendErrorToText(ex, "GetCitationData", errorPath);
                //tranData.ReturnCode = ((int)ReturnCodeEnum.Error).ToString();


            }
            #endregion
            #region "PANGO Service Call"
            /////////////////////////////////////////////////
            TransactionTransform tranDataOfService = new TransactionTransform();
            try
            {
                //////////////////////////////////////////
                logData.Request.CustomerId = param.CustomerId.Value.ToString();
                logData.Request.SpaceId = param.parkingSpaceID;
                logData.Request.MeterId = param.MeterName;
                //////////////////////////////////////////////////
                DateTime? currentCustTime = customerCurrentTime.CurrentTime;
                PangoCustomerElement customer = PangoCustomerRetriever.GetCustomer(param.CustomerId.Value);//CustomerRetriever.GetCustomer(param.CustomerId.Value);
                PangoGenericFactory factory = new PangoGenericFactory(customer, param.PlateNumber, param.StateName);
                var pangoResLog = new VendorResponseEntity();
                logData.VendorData.Add(VendorNames.Pango, pangoResLog);
                tranDataOfService = factory.GetPlateData(ref pangoResLog);
                tranDataOfService.EnforcementKey = EnforcementKey;

                ///////////////////////////////////////////////////
                logData.Response = new ResponseEntity() { CustomerId = param.CustomerId.Value.ToString(), MeterId = tranData.MeterName, SpaceId = tranData.SpaceNo, CityCurrentTime = currentCustTime.HasValue ? currentCustTime.Value : DateTime.Now };

            }
            catch (Exception ex)
            {
                logData.Response = new ResponseEntity() { CustomerId = param.CustomerId.Value.ToString(), MeterId = tranData.MeterName, SpaceId = tranData.SpaceNo, CityCurrentTime = customerCurrentTime.CurrentTime.HasValue ? customerCurrentTime.CurrentTime.Value : DateTime.Now };
                //throw ex;
                tranData.ReturnCode = ((int)ReturnCodeEnum.Error).ToString();
            }
            finally
            {

            }
            #endregion
            List<TransactionTransform> trans = new List<TransactionTransform>();
            trans.Add(tranData); // Database
            trans.Add(tranDataOfService); //Pango


            var rightValue = trans.Where(t => t.ReturnCode == ((int)ReturnCodeEnum.Success).ToString()).OrderByDescending(t => t.MeterExpiredMinutes.ToNullableInt()).FirstOrDefault<TransactionTransform>();
            if (rightValue == default(TransactionTransform))
            {
                //Do nothing.just return database  object.
            }
            else
            {
                tranData = rightValue;
            }

            return tranData;
        }
        public static TransactionTransform GetCityofChesterDataV3(string EnforcementKey, string filePath, RequestResponseEntity logData, CustomerTime customerCurrentTime)
        {
            TransactionTransform tranData = new TransactionTransform();
            tranData.ReturnCode = ((int)ReturnCodeEnum.InvalidInputParameters).ToString();
            TimeZoneInfo TZ = null;
            TZ = TimeZoneInfo.FindSystemTimeZoneById("Eastern Standard Time");
            //int? customerID = null;
            //string MeterName = null;
            //string parkingSpaceID;
            //string PlateNumber = null;
            //string stateName = null;
            //////////////////////////////////////
            KeyParameter param = PrepareKeyParameters.PrepareCityofChester(EnforcementKey);
            if (param.ReturnCode == ((int)ReturnCodeEnum.InvalidInputParameters).ToString())
            {
                return tranData;
            }
            #region DB
            /////////////////////////////////////
            try
            {
                Transaction data = TransactionDataValidatioin.GetDataCityofChester(param.CustomerId.Value, param.MeterName, param.parkingSpaceID, param.PlateNumber, param.StateName, EnforcementKey, filePath, logData);


                if (data.TimeZoneId.HasValue)
                    TZ = Utility.GetTimeZoneInfo(data.TimeZoneId.Value, filePath, param.CustomerId.Value.ToString());

                //For South Miami : PayStationId. Others it is MeterID
                tranData.MeterName = (data.MeterName == null) ? string.Empty : data.MeterName;
                tranData.SpaceNo = data.ParkingSpaceID.ToString();

                tranData.MeterStreet = (data.MeterStreet == null) ? string.Empty : data.MeterStreet;

                DateTime _MeterRTC = data.PresentMeterTime.HasValue ? data.PresentMeterTime.Value : DateTime.Now;
                tranData.MeterRTC = String.Format("{0:yyyyMMdd_THHmmssZ}", Utility.findGMTTime(_MeterRTC, TZ));

                DateTime _SensorEventTime = data.SensorEventTime.HasValue ? data.SensorEventTime.Value : DateTime.Now;
                tranData.MeterLastUpdateTime = String.Format("{0:yyyyMMdd_THHmmssZ}", Utility.findGMTTime(_SensorEventTime, TZ));

                tranData.SensorEventTime = String.Format("{0:yyyyMMdd_THHmmssZ}", Utility.findGMTTime(_SensorEventTime, TZ));

                DateTime _endTime = data.ExpiryTime.HasValue ? data.ExpiryTime.Value : DateTime.Now;
                tranData.ExpiredTime = String.Format("{0:yyyyMMdd_THHmmssZ}", Utility.findGMTTime(_endTime, TZ));

                tranData.MeterExpiredMinutes = data.ExpiredMinutes.HasValue ? data.ExpiredMinutes.Value.ToString() : "0";
                tranData.METERLOC_BLOCKNUMBER = (data.Block == null) ? string.Empty : data.Block;
                tranData.METERLOC_STREETDIRECTION = (data.Direction == null) ? string.Empty : data.Direction;
                tranData.METERLOC_STREETNAME = (data.MeterStreet == null) ? string.Empty : data.MeterStreet;
                tranData.METERLOC_STREETTYPE = (data.StreetType == null) ? string.Empty : data.StreetType;
                tranData.METERLOC_CROSSSTREET1 = (data.CrossStreet1 == null) ? string.Empty : data.CrossStreet1;
                tranData.METERLOC_CROSSSTREET2 = (data.CrossStreet2 == null) ? string.Empty : data.CrossStreet2;
                tranData.METERLOC_ENFORCEMENTHOURSDESC = (data.EnfHourDesc == null) ? string.Empty : data.EnfHourDesc;
                tranData.LicensePlate = (data.PlateNumber == null) ? string.Empty : data.PlateNumber;
                tranData.State = (data.LPLocation == null) ? string.Empty : data.LPLocation;
                tranData.Make = (data.VMake == null) ? string.Empty : data.VMake;
                tranData.Model = (data.VModel == null) ? string.Empty : data.VModel;
                tranData.IsOccupied = Utility.getStatus((data.SpaceStatus == null) ? string.Empty : data.SpaceStatus).ToString();
                tranData.EnforcementKey = (data.EnforcementKey == null) ? string.Empty : data.EnforcementKey;
                tranData.MeterStatus = (data.MeterStatus == null) ? null : data.MeterStatus;

                tranData.METERLOC_METERTYPE = (data.METERLOC_METERTYPE == null) ? null : data.METERLOC_METERTYPE;
                tranData.ZoneName = (data.ZoneName == null) ? null : data.ZoneName;

                tranData.MeterID = data.PayStationID.HasValue ? data.PayStationID.Value.ToString() : "0";
                tranData.ReturnCode = data.ReturnCode;

                logData.Response = new ResponseEntity() { CustomerId = param.CustomerId.Value.ToString(), MeterId = tranData.MeterName, SpaceId = tranData.SpaceNo, CityCurrentTime = data.PresentMeterTime.HasValue ? data.PresentMeterTime.Value : DateTime.Now };
            }
            catch (Exception ex)
            {
                //var appPath = System.Web.Hosting.HostingEnvironment.ApplicationPhysicalPath;
                //string errorPath = Path.Combine(appPath, "GetTransformData", "Timezones");
                //ExceptionLogging.SendErrorToText(ex, "GetCitationData", errorPath);
                //tranData.ReturnCode = ((int)ReturnCodeEnum.Error).ToString();


            }
            #endregion
            #region "PANGO Service Call"
            /////////////////////////////////////////////////
            TransactionTransform tranDataOfService = new TransactionTransform();
            try
            {
                //////////////////////////////////////////
                logData.Request.CustomerId = param.CustomerId.Value.ToString();
                logData.Request.SpaceId = param.parkingSpaceID;
                logData.Request.MeterId = param.MeterName;
                //////////////////////////////////////////////////
                DateTime? currentCustTime = customerCurrentTime.CurrentTime;
                PangoCustomerElement customer = PangoCustomerRetriever.GetCustomer(param.CustomerId.Value);//CustomerRetriever.GetCustomer(param.CustomerId.Value);
                PangoGenericFactory factory = new PangoGenericFactory(customer, param.PlateNumber, param.StateName);
                var pangoResLog = new VendorResponseEntity();
                logData.VendorData.Add(VendorNames.Pango, pangoResLog);
                tranDataOfService = factory.GetPlateData(ref pangoResLog);
                tranDataOfService.EnforcementKey = EnforcementKey;

                ///////////////////////////////////////////////////
                logData.Response = new ResponseEntity() { CustomerId = param.CustomerId.Value.ToString(), MeterId = tranData.MeterName, SpaceId = tranData.SpaceNo, CityCurrentTime = currentCustTime.HasValue ? currentCustTime.Value : DateTime.Now };

            }
            catch (Exception ex)
            {
                logData.Response = new ResponseEntity() { CustomerId = param.CustomerId.Value.ToString(), MeterId = tranData.MeterName, SpaceId = tranData.SpaceNo, CityCurrentTime = customerCurrentTime.CurrentTime.HasValue ? customerCurrentTime.CurrentTime.Value : DateTime.Now };
                //throw ex;
                tranData.ReturnCode = ((int)ReturnCodeEnum.Error).ToString();
            }
            finally
            {

            }
            #endregion
            #region "Cale Service Call"
            /////////////////////////////////////////////////
            TransactionTransform tranDataOfServiceCale = new TransactionTransform();
            try
            {
                //////////////////////////////////////////
                logData.Request.CustomerId = param.CustomerId.Value.ToString();
                logData.Request.SpaceId = param.parkingSpaceID;
                logData.Request.MeterId = param.MeterName;
                //////////////////////////////////////////////////
                DateTime? currentCustTime = customerCurrentTime.CurrentTime;
                Cale.CaleCustomerElement customer = Cale.CaleCustomerRetriever.GetCustomer(param.CustomerId.Value);
                Cale.CaleGenericFactory factory = new Cale.CaleGenericFactory(customer, param.PlateNumber, null);
                var caleResLog = new VendorResponseEntity();
                logData.VendorData.Add(VendorNames.Cale, caleResLog);
                if (customer.EnforcementType == EnumEnforcementType.PayByPlate.ToString())
                    tranDataOfServiceCale = factory.GetRefreshPlateData(ref caleResLog);
                tranDataOfServiceCale.EnforcementKey = EnforcementKey;

                ///////////////////////////////////////////////////
                logData.Response = new ResponseEntity() { CustomerId = param.CustomerId.Value.ToString(), MeterId = tranData.MeterName, SpaceId = tranData.SpaceNo, CityCurrentTime = currentCustTime.HasValue ? currentCustTime.Value : DateTime.Now };
            }
            catch (Exception ex)
            {
                logData.Response = new ResponseEntity() { CustomerId = param.CustomerId.Value.ToString(), MeterId = tranData.MeterName, SpaceId = tranData.SpaceNo, CityCurrentTime = customerCurrentTime.CurrentTime.HasValue ? customerCurrentTime.CurrentTime.Value : DateTime.Now };
                //throw ex;
                tranDataOfServiceCale.ReturnCode = ((int)ReturnCodeEnum.Error).ToString();
            }
            finally
            {

            }
            #endregion
            List<TransactionTransform> trans = new List<TransactionTransform>();
            trans.Add(tranData); // Database
            trans.Add(tranDataOfService); //Pango
            trans.Add(tranDataOfServiceCale); //Cale


            var rightValue = trans.Where(t => t.ReturnCode == ((int)ReturnCodeEnum.Success).ToString()).OrderByDescending(t => t.MeterExpiredMinutes.ToNullableInt()).FirstOrDefault<TransactionTransform>();
            if (rightValue == default(TransactionTransform))
            {
                //Do nothing.just return database  object.
            }
            else
            {
                tranData = rightValue;
            }

            return tranData;
        }
        private static Transaction GetDataCityofChester(int customerID, string MeterName, string parkingSpaceID, string PlateNumber, string statename, string EnforcementKey, string filePath, RequestResponseEntity logData)
        {
            string ZoneID = null;
            string SpaceStatus = null;
            Transaction data = new Transaction();
            try
            {
                List<Transaction> transactionList = new List<Transaction>();

                logData.Request.CustomerId = customerID.ToString();
                logData.Request.SpaceId = parkingSpaceID;
                logData.Request.MeterId = MeterName;

                transactionList = DataAccess.GetGenericBySpaceData(customerID, null, MeterName, parkingSpaceID, null, null);

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
            }
            catch (Exception ex)
            {
                var appPath = System.Web.Hosting.HostingEnvironment.ApplicationPhysicalPath;
                string errorPath = Path.Combine(appPath, "GetData", "Timezones");
                ExceptionLogging.SendErrorToText(ex, "GetCitationData", errorPath);
                data.ReturnCode = ((int)ReturnCodeEnum.Error).ToString();
                throw;
            }
            return data;
        }
        #endregion


        #region Glendale
        public static TransactionTransform GetGlendaleTransformData(string EnforcementKey, string filePath, RequestResponseEntity logData, CustomerTime customerCurrentTime)
        {
            TransactionTransform tranData = new TransactionTransform();
            tranData.ReturnCode = ((int)ReturnCodeEnum.InvalidInputParameters).ToString();
            TimeZoneInfo TZ = null;
            TZ = TimeZoneInfo.FindSystemTimeZoneById("Eastern Standard Time");
            //int? customerID = null;
            //string MeterName = null;
            //string parkingSpaceID;
            //string PlateNumber = null;
            //string stateName = null;
            //////////////////////////////////////
            KeyParameter param = PrepareKeyParameters.PrepareSouthMiami(EnforcementKey);
            if (param.ReturnCode == ((int)ReturnCodeEnum.InvalidInputParameters).ToString())
            {
                return tranData;
            }

            /////Special case //As specified by Alan
            /////When the client is sending alphanumeric space numbers for validation, “61A610” etc, the service is responding with an response code of “-1” and error text  “Specified cast is not valid”
            ////Please update this exception handling to instead return a response code of “0” and an expired minutes of “-1”
            //if (HandleSepecialSpace(param, EnforcementKey, ref tranData, ref logData, TZ))
            //    return tranData;
            ///////////////end special Case/////////////////
            /////////////////////////////////////
            #region DB
            try
            {
                Transaction data = TransactionDataValidatioin.GetDataSouthMiami(param.CustomerId.Value, param.MeterName, param.parkingSpaceID, param.PlateNumber, param.StateName, EnforcementKey, filePath, logData);


                if (data.TimeZoneId.HasValue)
                    TZ = Utility.GetTimeZoneInfo(data.TimeZoneId.Value, filePath, param.CustomerId.Value.ToString());

                //For South Miami : PayStationId. Others it is MeterID
                tranData.MeterName = (data.MeterName == null) ? string.Empty : data.MeterName;
                tranData.SpaceNo = data.ParkingSpaceID.ToString();

                tranData.MeterStreet = (data.MeterStreet == null) ? string.Empty : data.MeterStreet;

                DateTime _MeterRTC = data.PresentMeterTime.HasValue ? data.PresentMeterTime.Value : DateTime.Now;
                tranData.MeterRTC = String.Format("{0:yyyyMMdd_THHmmssZ}", Utility.findGMTTime(_MeterRTC, TZ));

                DateTime _SensorEventTime = data.SensorEventTime.HasValue ? data.SensorEventTime.Value : DateTime.Now;
                tranData.MeterLastUpdateTime = String.Format("{0:yyyyMMdd_THHmmssZ}", Utility.findGMTTime(_SensorEventTime, TZ));

                tranData.SensorEventTime = String.Format("{0:yyyyMMdd_THHmmssZ}", Utility.findGMTTime(_SensorEventTime, TZ));

                DateTime _endTime = data.ExpiryTime.HasValue ? data.ExpiryTime.Value : DateTime.Now;
                tranData.ExpiredTime = String.Format("{0:yyyyMMdd_THHmmssZ}", Utility.findGMTTime(_endTime, TZ));

                tranData.MeterExpiredMinutes = data.ExpiredMinutes.HasValue ? data.ExpiredMinutes.Value.ToString() : "0";
                tranData.METERLOC_BLOCKNUMBER = (data.Block == null) ? string.Empty : data.Block;
                tranData.METERLOC_STREETDIRECTION = (data.Direction == null) ? string.Empty : data.Direction;
                tranData.METERLOC_STREETNAME = (data.MeterStreet == null) ? string.Empty : data.MeterStreet;
                tranData.METERLOC_STREETTYPE = (data.StreetType == null) ? string.Empty : data.StreetType;
                tranData.METERLOC_CROSSSTREET1 = (data.CrossStreet1 == null) ? string.Empty : data.CrossStreet1;
                tranData.METERLOC_CROSSSTREET2 = (data.CrossStreet2 == null) ? string.Empty : data.CrossStreet2;
                tranData.METERLOC_ENFORCEMENTHOURSDESC = (data.EnfHourDesc == null) ? string.Empty : data.EnfHourDesc;
                tranData.LicensePlate = (data.PlateNumber == null) ? string.Empty : data.PlateNumber;
                tranData.State = (data.LPLocation == null) ? string.Empty : data.LPLocation;
                tranData.Make = (data.VMake == null) ? string.Empty : data.VMake;
                tranData.Model = (data.VModel == null) ? string.Empty : data.VModel;
                tranData.IsOccupied = Utility.getStatus((data.SpaceStatus == null) ? string.Empty : data.SpaceStatus).ToString();
                tranData.EnforcementKey = (data.EnforcementKey == null) ? string.Empty : data.EnforcementKey;
                tranData.MeterStatus = (data.MeterStatus == null) ? null : data.MeterStatus;

                tranData.METERLOC_METERTYPE = (data.METERLOC_METERTYPE == null) ? null : data.METERLOC_METERTYPE;
                tranData.ZoneName = (data.ZoneName == null) ? null : data.ZoneName;

                tranData.MeterID = data.PayStationID.HasValue ? data.PayStationID.Value.ToString() : "0";
                tranData.ReturnCode = data.ReturnCode;

                logData.Response = new ResponseEntity() { CustomerId = param.CustomerId.Value.ToString(), MeterId = tranData.MeterName, SpaceId = tranData.SpaceNo, CityCurrentTime = data.PresentMeterTime.HasValue ? data.PresentMeterTime.Value : DateTime.Now };
            }
            catch (Exception ex)
            {
                //var appPath = System.Web.Hosting.HostingEnvironment.ApplicationPhysicalPath;
                //string errorPath = Path.Combine(appPath, "GetTransformData", "Timezones");
                //ExceptionLogging.SendErrorToText(ex, "GetCitationData", errorPath);
                //tranData.ReturnCode = ((int)ReturnCodeEnum.Error).ToString();
                tranData.ReturnCode = ((int)ReturnCodeEnum.Error).ToString();
            }
            #endregion
            #region T2Digital
            TransactionTransform tranDataOfServiceT2 = new TransactionTransform();
            try
            {
                //////////////////////////////////////////
                logData.Request.CustomerId = param.CustomerId.Value.ToString();
                logData.Request.SpaceId = param.parkingSpaceID;
                logData.Request.MeterId = param.MeterName;
                //////////////////////////////////////////////////
                DateTime? currentCustTime = customerCurrentTime.CurrentTime;
                T2Digital.T2DigitalCustomerElement customer = T2Digital.T2DigitalCustomerRetriever.GetCustomer(param.CustomerId.Value);
                T2Digital.T2DigitalGenericFactory factory = new T2Digital.T2DigitalGenericFactory(customer, Convert.ToInt32(param.parkingSpaceID));
                var t2ResLog = new VendorResponseEntity();
                logData.VendorData.Add(VendorNames.T2Digital, t2ResLog);
                tranDataOfServiceT2 = factory.GetRefreshStallNo(ref t2ResLog);
                tranDataOfServiceT2.EnforcementKey = EnforcementKey;

                ///////////////////////////////////////////////////
                logData.Response = new ResponseEntity() { CustomerId = param.CustomerId.Value.ToString(), MeterId = tranData.MeterName, SpaceId = tranData.SpaceNo, CityCurrentTime = currentCustTime.HasValue ? currentCustTime.Value : DateTime.Now };

            }
            catch (Exception ex)
            {
                logData.Response = new ResponseEntity() { CustomerId = param.CustomerId.Value.ToString(), MeterId = tranData.MeterName, SpaceId = tranData.SpaceNo, CityCurrentTime = customerCurrentTime.CurrentTime.HasValue ? customerCurrentTime.CurrentTime.Value : DateTime.Now };
                //throw ex;
                tranData.ReturnCode = ((int)ReturnCodeEnum.Error).ToString();
            }
            finally
            {

            }
            #endregion
            List<TransactionTransform> trans = new List<TransactionTransform>();
            trans.Add(tranData); // Database
            trans.Add(tranDataOfServiceT2); //T2Digital


            var rightValue = trans.Where(t => t.ReturnCode == ((int)ReturnCodeEnum.Success).ToString()).OrderByDescending(t => t.MeterExpiredMinutes.ToNullableInt()).FirstOrDefault<TransactionTransform>();
            if (rightValue == default(TransactionTransform))
            {
                //Do nothing.just return database  object.
            }
            else
            {
                tranData = rightValue;
            }

            return tranData;
        }
        #endregion


        #region BoyntonBeach
        public static TransactionTransform GetBoyntonBeachTransformData(string EnforcementKey, string filePath, RequestResponseEntity logData, CustomerTime customerCurrentTime)
        {
            TransactionTransform tranData = new TransactionTransform();
            tranData.ReturnCode = ((int)ReturnCodeEnum.InvalidInputParameters).ToString();
            TimeZoneInfo TZ = null;
            TZ = TimeZoneInfo.FindSystemTimeZoneById("Eastern Standard Time");
            //////////////////////////////////////
            KeyParameter param = PrepareKeyParameters.PrepareGenericPayByPlate(EnforcementKey);
            if (param.ReturnCode == ((int)ReturnCodeEnum.InvalidInputParameters).ToString())
            {
                return tranData;
            }
            /////////////////////////////////////
            #region "DB Check"
            try
            {
                Transaction data = TransactionDataValidatioin.GetDataGenericPayByPlate(param.CustomerId.Value, param.MeterName, param.parkingSpaceID, param.PlateNumber, param.StateName, EnforcementKey, filePath, logData);


                if (data.TimeZoneId.HasValue)
                    TZ = Utility.GetTimeZoneInfo(data.TimeZoneId.Value, filePath, param.CustomerId.Value.ToString());

                //For South Miami : PayStationId. Others it is MeterID
                tranData.MeterName = string.Empty; // (data.MeterName == null) ? string.Empty : data.MeterName;
                tranData.SpaceNo = string.Empty;  //data.ParkingSpaceID.ToString();

                tranData.MeterStreet = (data.MeterStreet == null) ? string.Empty : data.MeterStreet;

                DateTime _MeterRTC = data.PresentMeterTime.HasValue ? data.PresentMeterTime.Value : DateTime.Now;
                tranData.MeterRTC = String.Format("{0:yyyyMMdd_THHmmssZ}", Utility.findGMTTime(_MeterRTC, TZ));

                DateTime _SensorEventTime = data.SensorEventTime.HasValue ? data.SensorEventTime.Value : DateTime.Now;
                tranData.MeterLastUpdateTime = String.Format("{0:yyyyMMdd_THHmmssZ}", Utility.findGMTTime(_SensorEventTime, TZ));

                tranData.SensorEventTime = String.Format("{0:yyyyMMdd_THHmmssZ}", Utility.findGMTTime(_SensorEventTime, TZ));

                DateTime _endTime = data.ExpiryTime.HasValue ? data.ExpiryTime.Value : DateTime.Now;
                tranData.ExpiredTime = String.Format("{0:yyyyMMdd_THHmmssZ}", Utility.findGMTTime(_endTime, TZ));

                tranData.MeterExpiredMinutes = data.ExpiredMinutes.HasValue ? data.ExpiredMinutes.Value.ToString() : "0";
                tranData.METERLOC_BLOCKNUMBER = (data.Block == null) ? string.Empty : data.Block;
                tranData.METERLOC_STREETDIRECTION = (data.Direction == null) ? string.Empty : data.Direction;
                tranData.METERLOC_STREETNAME = (data.MeterStreet == null) ? string.Empty : data.MeterStreet;
                tranData.METERLOC_STREETTYPE = (data.StreetType == null) ? string.Empty : data.StreetType;
                tranData.METERLOC_CROSSSTREET1 = (data.CrossStreet1 == null) ? string.Empty : data.CrossStreet1;
                tranData.METERLOC_CROSSSTREET2 = (data.CrossStreet2 == null) ? string.Empty : data.CrossStreet2;
                tranData.METERLOC_ENFORCEMENTHOURSDESC = (data.EnfHourDesc == null) ? string.Empty : data.EnfHourDesc;
                tranData.LicensePlate = (data.PlateNumber == null) ? string.Empty : data.PlateNumber;
                tranData.State = (data.LPLocation == null) ? string.Empty : data.LPLocation;
                tranData.Make = (data.VMake == null) ? string.Empty : data.VMake;
                tranData.Model = (data.VModel == null) ? string.Empty : data.VModel;
                tranData.IsOccupied = Utility.getStatus((data.SpaceStatus == null) ? string.Empty : data.SpaceStatus).ToString();
                tranData.EnforcementKey = (data.EnforcementKey == null) ? string.Empty : data.EnforcementKey;
                tranData.MeterStatus = (data.MeterStatus == null) ? null : data.MeterStatus;

                tranData.METERLOC_METERTYPE = (data.METERLOC_METERTYPE == null) ? null : data.METERLOC_METERTYPE;
                tranData.ZoneName = (data.ZoneName == null) ? null : data.ZoneName;
                tranData.MeterID = String.Empty;
                tranData.ReturnCode = data.ReturnCode;

                logData.Response = new ResponseEntity() { CustomerId = param.CustomerId.Value.ToString(), MeterId = tranData.MeterName, SpaceId = tranData.SpaceNo, CityCurrentTime = data.PresentMeterTime.HasValue ? data.PresentMeterTime.Value : DateTime.Now };
            }
            catch (Exception ex)
            {
                //var appPath = System.Web.Hosting.HostingEnvironment.ApplicationPhysicalPath;
                //string errorPath = Path.Combine(appPath, "GetTransformData", "Timezones");
                //ExceptionLogging.SendErrorToText(ex, "GetCitationData", errorPath);
                //tranData.ReturnCode = ((int)ReturnCodeEnum.Error).ToString();

                tranData.ReturnCode = ((int)ReturnCodeEnum.Error).ToString();
            }
            #endregion
            #region "Parkeon Service Call"
            /////////////////////////////////////////////////
            TransactionTransform tranDataOfService = new TransactionTransform();
            ServiceParameters serviceParam = null;
            try
            {
                //////////////////////////////////////////
                logData.Request.CustomerId = param.CustomerId.Value.ToString();
                logData.Request.SpaceId = param.parkingSpaceID;
                logData.Request.MeterId = param.MeterName;
                //////////////////////////////////////////////////
                DateTime? currentCustTime = customerCurrentTime.CurrentTime;
                tranDataOfService.EnforcementKey = EnforcementKey;
                ParkeonCustomerElement customer = ParkeonCustomerRetriever.GetCustomer(param.CustomerId.Value);
                serviceParam = new ServiceParameters() { customerId = (Convert.ToInt64(customer.CustomerId)), userName = customer.UserName, userPassword = customer.UserPassword, vendorId = (Convert.ToInt64(customer.VendorId)) };
                ParkeonServiceAccess parkeonService = new ParkeonServiceAccess(serviceParam, param, customer);
                var parkeonResLog = new VendorResponseEntity();
                logData.VendorData.Add(VendorNames.Parkeon, parkeonResLog);
                parkeonService.ProcessRequest(ref tranDataOfService, TypeConditions.check_plate, TZ, ref currentCustTime, ref parkeonResLog);
                serviceParam = null;
                ///////////////////////////////////////////////////
                logData.Response = new ResponseEntity() { CustomerId = param.CustomerId.Value.ToString(), MeterId = tranData.MeterName, SpaceId = tranData.SpaceNo, CityCurrentTime = currentCustTime.HasValue ? currentCustTime.Value : DateTime.Now };

            }
            catch (Exception ex)
            {
                logData.Response = new ResponseEntity() { CustomerId = param.CustomerId.Value.ToString(), MeterId = tranData.MeterName, SpaceId = tranData.SpaceNo, CityCurrentTime = customerCurrentTime.CurrentTime.HasValue ? customerCurrentTime.CurrentTime.Value : DateTime.Now };
                //throw ex;
                tranDataOfService.ReturnCode = ((int)ReturnCodeEnum.Error).ToString();
            }
            finally
            {
                serviceParam = null;
            }
            #endregion
            #region "Parkmobile Service Call"
            /////////////////////////////////////////////////
            TransactionTransform tranDataOfServiceParkMobile = new TransactionTransform();
            try
            {
                //////////////////////////////////////////
                logData.Request.CustomerId = param.CustomerId.Value.ToString();
                logData.Request.SpaceId = param.parkingSpaceID;
                logData.Request.MeterId = param.MeterName;
                //////////////////////////////////////////////////
                DateTime? currentCustTime = customerCurrentTime.CurrentTime;
                PMCustomerElement customer = ParkmobileCustomerRetriever.GetCustomer(param.CustomerId.Value);
                ParkMobileGenericFactory factory = new ParkMobileGenericFactory(customer, param.PlateNumber);
                var parkMobileResLog = new VendorResponseEntity();
                logData.VendorData.Add(VendorNames.ParkMobile, parkMobileResLog);
                tranDataOfServiceParkMobile = factory.GetRefreshSpaceData(ParkingMobileDataType.Plate, ref parkMobileResLog);
                tranDataOfServiceParkMobile.EnforcementKey = EnforcementKey;

                ///////////////////////////////////////////////////
                logData.Response = new ResponseEntity() { CustomerId = param.CustomerId.Value.ToString(), MeterId = tranData.MeterName, SpaceId = tranData.SpaceNo, CityCurrentTime = currentCustTime.HasValue ? currentCustTime.Value : DateTime.Now };

            }
            catch (Exception ex)
            {
                logData.Response = new ResponseEntity() { CustomerId = param.CustomerId.Value.ToString(), MeterId = tranData.MeterName, SpaceId = tranData.SpaceNo, CityCurrentTime = customerCurrentTime.CurrentTime.HasValue ? customerCurrentTime.CurrentTime.Value : DateTime.Now };
                //throw ex;
                tranDataOfServiceParkMobile.ReturnCode = ((int)ReturnCodeEnum.Error).ToString();
            }
            finally
            {

            }
            #endregion

            //////////////////  //Reconcile data
            List<TransactionTransform> trans = new List<TransactionTransform>();
            trans.Add(tranDataOfServiceParkMobile); // Parkmobile
            trans.Add(tranData); // Database
            trans.Add(tranDataOfService); //Parkeon


            var rightValue = trans.Where(t => t.ReturnCode == ((int)ReturnCodeEnum.Success).ToString()).OrderByDescending(t => t.MeterExpiredMinutes.ToNullableInt()).FirstOrDefault<TransactionTransform>();
            if (rightValue == default(TransactionTransform))
            {
                //Do nothing.just return database  object.
            }
            else
            {
                tranData = rightValue;
            }

            return tranData;
        }
        #endregion

        #region CityofMountRainier

        public static TransactionTransform GetCityofMountRainier(string EnforcementKey, string filePath, RequestResponseEntity logData, CustomerTime customerCurrentTime)
        {
            TransactionTransform tranData = new TransactionTransform();
            tranData.ReturnCode = ((int)ReturnCodeEnum.InvalidInputParameters).ToString();
            TimeZoneInfo TZ = null;
            TZ = TimeZoneInfo.FindSystemTimeZoneById("Eastern Standard Time");
            KeyParameter param = PrepareKeyParameters.GenericBySpace(EnforcementKey);
            if (param.ReturnCode == ((int)ReturnCodeEnum.InvalidInputParameters).ToString())
            {
                return tranData;
            }
            /////////////////////////////////////
            #region "DB Check"
            try
            {
                Transaction data = TransactionDataValidatioin.GetDataCityofMountRainier(param.CustomerId.Value, param.MeterName, param.parkingSpaceID, param.PlateNumber, param.StateName, EnforcementKey, filePath, logData);


                if (data.TimeZoneId.HasValue)
                    TZ = Utility.GetTimeZoneInfo(data.TimeZoneId.Value, filePath, param.CustomerId.Value.ToString());

                //For South Miami : PayStationId. Others it is MeterID
                tranData.MeterName = string.Empty; // (data.MeterName == null) ? string.Empty : data.MeterName;
                tranData.SpaceNo = string.Empty;  //data.ParkingSpaceID.ToString();

                tranData.MeterStreet = (data.MeterStreet == null) ? string.Empty : data.MeterStreet;

                DateTime _MeterRTC = data.PresentMeterTime.HasValue ? data.PresentMeterTime.Value : DateTime.Now;
                tranData.MeterRTC = String.Format("{0:yyyyMMdd_THHmmssZ}", Utility.findGMTTime(_MeterRTC, TZ));

                DateTime _SensorEventTime = data.SensorEventTime.HasValue ? data.SensorEventTime.Value : DateTime.Now;
                tranData.MeterLastUpdateTime = String.Format("{0:yyyyMMdd_THHmmssZ}", Utility.findGMTTime(_SensorEventTime, TZ));

                tranData.SensorEventTime = String.Format("{0:yyyyMMdd_THHmmssZ}", Utility.findGMTTime(_SensorEventTime, TZ));

                DateTime _endTime = data.ExpiryTime.HasValue ? data.ExpiryTime.Value : DateTime.Now;
                tranData.ExpiredTime = String.Format("{0:yyyyMMdd_THHmmssZ}", Utility.findGMTTime(_endTime, TZ));

                tranData.MeterExpiredMinutes = data.ExpiredMinutes.HasValue ? data.ExpiredMinutes.Value.ToString() : "0";
                tranData.METERLOC_BLOCKNUMBER = (data.Block == null) ? string.Empty : data.Block;
                tranData.METERLOC_STREETDIRECTION = (data.Direction == null) ? string.Empty : data.Direction;
                tranData.METERLOC_STREETNAME = (data.MeterStreet == null) ? string.Empty : data.MeterStreet;
                tranData.METERLOC_STREETTYPE = (data.StreetType == null) ? string.Empty : data.StreetType;
                tranData.METERLOC_CROSSSTREET1 = (data.CrossStreet1 == null) ? string.Empty : data.CrossStreet1;
                tranData.METERLOC_CROSSSTREET2 = (data.CrossStreet2 == null) ? string.Empty : data.CrossStreet2;
                tranData.METERLOC_ENFORCEMENTHOURSDESC = (data.EnfHourDesc == null) ? string.Empty : data.EnfHourDesc;
                tranData.LicensePlate = (data.PlateNumber == null) ? string.Empty : data.PlateNumber;
                tranData.State = (data.LPLocation == null) ? string.Empty : data.LPLocation;
                tranData.Make = (data.VMake == null) ? string.Empty : data.VMake;
                tranData.Model = (data.VModel == null) ? string.Empty : data.VModel;
                tranData.IsOccupied = Utility.getStatus((data.SpaceStatus == null) ? string.Empty : data.SpaceStatus).ToString();
                tranData.EnforcementKey = (data.EnforcementKey == null) ? string.Empty : data.EnforcementKey;
                tranData.MeterStatus = (data.MeterStatus == null) ? null : data.MeterStatus;

                tranData.METERLOC_METERTYPE = (data.METERLOC_METERTYPE == null) ? null : data.METERLOC_METERTYPE;
                tranData.ZoneName = (data.ZoneName == null) ? null : data.ZoneName;
                tranData.MeterID = String.Empty;
                tranData.ReturnCode = data.ReturnCode;

                logData.Response = new ResponseEntity() { CustomerId = param.CustomerId.Value.ToString(), MeterId = tranData.MeterName, SpaceId = tranData.SpaceNo, CityCurrentTime = data.PresentMeterTime.HasValue ? data.PresentMeterTime.Value : DateTime.Now };
            }
            catch (Exception ex)
            {
                //var appPath = System.Web.Hosting.HostingEnvironment.ApplicationPhysicalPath;
                //string errorPath = Path.Combine(appPath, "GetTransformData", "Timezones");
                //ExceptionLogging.SendErrorToText(ex, "GetCitationData", errorPath);
                //tranData.ReturnCode = ((int)ReturnCodeEnum.Error).ToString();

                tranData.ReturnCode = ((int)ReturnCodeEnum.Error).ToString();
            }
            #endregion


            //Reconcile data
            List<TransactionTransform> trans = new List<TransactionTransform>();
            trans.Add(tranData); // Database

            var rightValue = trans.Where(t => t.ReturnCode == ((int)ReturnCodeEnum.Success).ToString()).OrderByDescending(t => t.MeterExpiredMinutes.ToNullableInt()).FirstOrDefault<TransactionTransform>();
            if (rightValue == default(TransactionTransform))
            {
                //Do nothing.just return database  object.
            }
            else
            {
                tranData = rightValue;
            }

            return tranData;
        }

        private static Transaction GetDataCityofMountRainier(int customerID, string MeterName, string parkingSpaceID, string PlateNumber, string statename, string EnforcementKey, string filePath, RequestResponseEntity logData)
        {
            string ZoneID = null;
            string SpaceStatus = null;
            Transaction data = new Transaction();
            try
            {
                List<Transaction> transactionList = new List<Transaction>();

                logData.Request.CustomerId = customerID.ToString();
                logData.Request.SpaceId = parkingSpaceID;
                logData.Request.MeterId = MeterName;

                transactionList = DataAccess.GetGenericBySpaceData(customerID, null, MeterName, parkingSpaceID, null, null);

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
            }
            catch (Exception ex)
            {
                var appPath = System.Web.Hosting.HostingEnvironment.ApplicationPhysicalPath;
                string errorPath = Path.Combine(appPath, "GetData", "Timezones");
                ExceptionLogging.SendErrorToText(ex, "GetCitationData", errorPath);
                data.ReturnCode = ((int)ReturnCodeEnum.Error).ToString();
                throw;
            }
            return data;
        }
        #endregion

        #region Dammam
        public static TransactionTransform GetDammamTransformData(string EnforcementKey, string filePath, RequestResponseEntity logData)
        {
            TransactionTransform tranData = new TransactionTransform();
            tranData.ReturnCode = ((int)ReturnCodeEnum.InvalidInputParameters).ToString();
            TimeZoneInfo TZ = null;
            TZ = TimeZoneInfo.FindSystemTimeZoneById("Eastern Standard Time");
            //////////////////////////////////////
            KeyParameter param = PrepareKeyParameters.PrepareGenericPayByPlate(EnforcementKey);
            if (param.ReturnCode == ((int)ReturnCodeEnum.InvalidInputParameters).ToString())
            {
                return tranData;
            }
            /////////////////////////////////////
            #region "DB Check"
            try
            {
                Transaction data = TransactionDataValidatioin.GetDataGenericPayByPlate(param.CustomerId.Value, param.MeterName, param.parkingSpaceID, param.PlateNumber, param.StateName, EnforcementKey, filePath, logData);


                if (data.TimeZoneId.HasValue)
                    TZ = Utility.GetTimeZoneInfo(data.TimeZoneId.Value, filePath, param.CustomerId.Value.ToString());

                //For South Miami : PayStationId. Others it is MeterID
                tranData.MeterName = string.Empty; // (data.MeterName == null) ? string.Empty : data.MeterName;
                tranData.SpaceNo = string.Empty;  //data.ParkingSpaceID.ToString();

                tranData.MeterStreet = (data.MeterStreet == null) ? string.Empty : data.MeterStreet;

                DateTime _MeterRTC = data.PresentMeterTime.HasValue ? data.PresentMeterTime.Value : DateTime.Now;
                tranData.MeterRTC = String.Format("{0:yyyyMMdd_THHmmssZ}", Utility.findGMTTime(_MeterRTC, TZ));

                DateTime _SensorEventTime = data.SensorEventTime.HasValue ? data.SensorEventTime.Value : DateTime.Now;
                tranData.MeterLastUpdateTime = String.Format("{0:yyyyMMdd_THHmmssZ}", Utility.findGMTTime(_SensorEventTime, TZ));

                tranData.SensorEventTime = String.Format("{0:yyyyMMdd_THHmmssZ}", Utility.findGMTTime(_SensorEventTime, TZ));

                DateTime _endTime = data.ExpiryTime.HasValue ? data.ExpiryTime.Value : DateTime.Now;
                tranData.ExpiredTime = String.Format("{0:yyyyMMdd_THHmmssZ}", Utility.findGMTTime(_endTime, TZ));

                tranData.MeterExpiredMinutes = data.ExpiredMinutes.HasValue ? data.ExpiredMinutes.Value.ToString() : "0";
                tranData.METERLOC_BLOCKNUMBER = (data.Block == null) ? string.Empty : data.Block;
                tranData.METERLOC_STREETDIRECTION = (data.Direction == null) ? string.Empty : data.Direction;
                tranData.METERLOC_STREETNAME = (data.MeterStreet == null) ? string.Empty : data.MeterStreet;
                tranData.METERLOC_STREETTYPE = (data.StreetType == null) ? string.Empty : data.StreetType;
                tranData.METERLOC_CROSSSTREET1 = (data.CrossStreet1 == null) ? string.Empty : data.CrossStreet1;
                tranData.METERLOC_CROSSSTREET2 = (data.CrossStreet2 == null) ? string.Empty : data.CrossStreet2;
                tranData.METERLOC_ENFORCEMENTHOURSDESC = (data.EnfHourDesc == null) ? string.Empty : data.EnfHourDesc;
                tranData.LicensePlate = (data.PlateNumber == null) ? string.Empty : data.PlateNumber;
                tranData.State = (data.LPLocation == null) ? string.Empty : data.LPLocation;
                tranData.Make = (data.VMake == null) ? string.Empty : data.VMake;
                tranData.Model = (data.VModel == null) ? string.Empty : data.VModel;
                tranData.IsOccupied = Utility.getStatus((data.SpaceStatus == null) ? string.Empty : data.SpaceStatus).ToString();
                tranData.EnforcementKey = (data.EnforcementKey == null) ? string.Empty : data.EnforcementKey;
                tranData.MeterStatus = (data.MeterStatus == null) ? null : data.MeterStatus;

                tranData.METERLOC_METERTYPE = (data.METERLOC_METERTYPE == null) ? null : data.METERLOC_METERTYPE;
                tranData.ZoneName = (data.ZoneName == null) ? null : data.ZoneName;
                tranData.MeterID = String.Empty;
                tranData.ReturnCode = data.ReturnCode;

                logData.Response = new ResponseEntity() { CustomerId = param.CustomerId.Value.ToString(), MeterId = tranData.MeterName, SpaceId = tranData.SpaceNo, CityCurrentTime = data.PresentMeterTime.HasValue ? data.PresentMeterTime.Value : DateTime.Now };
            }
            catch (Exception ex)
            {
                //var appPath = System.Web.Hosting.HostingEnvironment.ApplicationPhysicalPath;
                //string errorPath = Path.Combine(appPath, "GetTransformData", "Timezones");
                //ExceptionLogging.SendErrorToText(ex, "GetCitationData", errorPath);
                //tranData.ReturnCode = ((int)ReturnCodeEnum.Error).ToString();

                tranData.ReturnCode = ((int)ReturnCodeEnum.Error).ToString();
            }
            #endregion
            #region "MiEx Service Call"
            /////////////////////////////////////////////////
            TransactionTransform tranDataOfService = new TransactionTransform();
            DateTime? currentCustTime = DateTime.Now;//customerCurrentTime.CurrentTime
            try
            {
                //////////////////////////////////////////
                logData.Request.CustomerId = param.CustomerId.Value.ToString();
                logData.Request.SpaceId = param.parkingSpaceID;
                logData.Request.MeterId = param.MeterName;
                //////////////////////////////////////////////////

                MiExchange.MiExCustomerElement customer = MiExchange.MiExCustomerRetriever.GetCustomer(param.CustomerId.Value);
                MiExchange.MiExGenericFactory factory = new MiExchange.MiExGenericFactory(customer, param.PlateNumber, string.Empty);
                var miExResLog = new VendorResponseEntity();
                logData.VendorData.Add(VendorNames.MiXchange, miExResLog);
                tranDataOfService = factory.GetPlateData(ref miExResLog);
                tranDataOfService.EnforcementKey = EnforcementKey;

                ///////////////////////////////////////////////////
                logData.Response = new ResponseEntity() { CustomerId = param.CustomerId.Value.ToString(), MeterId = tranData.MeterName, SpaceId = tranData.SpaceNo, CityCurrentTime = currentCustTime.HasValue ? currentCustTime.Value : DateTime.Now };

            }
            catch (Exception ex)
            {
                tranDataOfService.ReturnCode = ((int)ReturnCodeEnum.Error).ToString();
            }
            finally
            {

            }
            #endregion
            //////////////////  //Reconcile data
            List<TransactionTransform> trans = new List<TransactionTransform>();
            trans.Add(tranData); // Database
            trans.Add(tranDataOfService);

            var rightValue = trans.Where(t => t.ReturnCode == ((int)ReturnCodeEnum.Success).ToString()).OrderByDescending(t => t.MeterExpiredMinutes.ToNullableInt()).FirstOrDefault<TransactionTransform>();
            if (rightValue == default(TransactionTransform))
            {
                //Do nothing.just return database  object.
            }
            else
            {
                tranData = rightValue;
            }

            return tranData;
        }

        #endregion


        #region Muscat
        public static TransactionTransform GetMuscatTransformData(string EnforcementKey, string filePath, RequestResponseEntity logData)
        {
            TransactionTransform tranData = new TransactionTransform();
            tranData.ReturnCode = ((int)ReturnCodeEnum.InvalidInputParameters).ToString();
            TimeZoneInfo TZ = null;
            TZ = TimeZoneInfo.FindSystemTimeZoneById("Eastern Standard Time");
            //////////////////////////////////////
            KeyParameter param = PrepareKeyParameters.PrepareGenericPayByPlate(EnforcementKey);
            if (param.ReturnCode == ((int)ReturnCodeEnum.InvalidInputParameters).ToString())
            {
                return tranData;
            }
            /////////////////////////////////////
            #region "DB Check"
            try
            {
                Transaction data = TransactionDataValidatioin.GetDataMuscatPayByPlate(param.CustomerId.Value, param.MeterName, param.parkingSpaceID, param.PlateNumber, param.StateName, EnforcementKey, filePath, logData);


                if (data.TimeZoneId.HasValue)
                    TZ = Utility.GetTimeZoneInfo(data.TimeZoneId.Value, filePath, param.CustomerId.Value.ToString());

                //For South Miami : PayStationId. Others it is MeterID
                tranData.MeterName = string.Empty; // (data.MeterName == null) ? string.Empty : data.MeterName;
                tranData.SpaceNo = string.Empty;  //data.ParkingSpaceID.ToString();

                tranData.MeterStreet = (data.MeterStreet == null) ? string.Empty : data.MeterStreet;

                DateTime _MeterRTC = data.PresentMeterTime.HasValue ? data.PresentMeterTime.Value : DateTime.Now;
                tranData.MeterRTC = String.Format("{0:yyyyMMdd_THHmmssZ}", Utility.findGMTTime(_MeterRTC, TZ));

                DateTime _SensorEventTime = data.SensorEventTime.HasValue ? data.SensorEventTime.Value : DateTime.Now;
                tranData.MeterLastUpdateTime = String.Format("{0:yyyyMMdd_THHmmssZ}", Utility.findGMTTime(_SensorEventTime, TZ));

                tranData.SensorEventTime = String.Format("{0:yyyyMMdd_THHmmssZ}", Utility.findGMTTime(_SensorEventTime, TZ));

                DateTime _endTime = data.ExpiryTime.HasValue ? data.ExpiryTime.Value : DateTime.Now;
                tranData.ExpiredTime = String.Format("{0:yyyyMMdd_THHmmssZ}", Utility.findGMTTime(_endTime, TZ));

                tranData.MeterExpiredMinutes = data.ExpiredMinutes.HasValue ? data.ExpiredMinutes.Value.ToString() : "0";
                tranData.METERLOC_BLOCKNUMBER = (data.Block == null) ? string.Empty : data.Block;
                tranData.METERLOC_STREETDIRECTION = (data.Direction == null) ? string.Empty : data.Direction;
                tranData.METERLOC_STREETNAME = (data.MeterStreet == null) ? string.Empty : data.MeterStreet;
                tranData.METERLOC_STREETTYPE = (data.StreetType == null) ? string.Empty : data.StreetType;
                tranData.METERLOC_CROSSSTREET1 = (data.CrossStreet1 == null) ? string.Empty : data.CrossStreet1;
                tranData.METERLOC_CROSSSTREET2 = (data.CrossStreet2 == null) ? string.Empty : data.CrossStreet2;
                tranData.METERLOC_ENFORCEMENTHOURSDESC = (data.EnfHourDesc == null) ? string.Empty : data.EnfHourDesc;
                tranData.LicensePlate = (data.PlateNumber == null) ? string.Empty : data.PlateNumber;
                tranData.State = (data.LPLocation == null) ? string.Empty : data.LPLocation;
                tranData.Make = (data.VMake == null) ? string.Empty : data.VMake;
                tranData.Model = (data.VModel == null) ? string.Empty : data.VModel;
                tranData.IsOccupied = Utility.getStatus((data.SpaceStatus == null) ? string.Empty : data.SpaceStatus).ToString();
                tranData.EnforcementKey = (data.EnforcementKey == null) ? string.Empty : data.EnforcementKey;
                tranData.MeterStatus = (data.MeterStatus == null) ? null : data.MeterStatus;

                tranData.METERLOC_METERTYPE = (data.METERLOC_METERTYPE == null) ? null : data.METERLOC_METERTYPE;
                tranData.ZoneName = (data.ZoneName == null) ? null : data.ZoneName;
                tranData.MeterID = String.Empty;
                tranData.ReturnCode = data.ReturnCode;

                logData.Response = new ResponseEntity() { CustomerId = param.CustomerId.Value.ToString(), MeterId = tranData.MeterName, SpaceId = tranData.SpaceNo, CityCurrentTime = data.PresentMeterTime.HasValue ? data.PresentMeterTime.Value : DateTime.Now };
            }
            catch (Exception ex)
            {
                //var appPath = System.Web.Hosting.HostingEnvironment.ApplicationPhysicalPath;
                //string errorPath = Path.Combine(appPath, "GetTransformData", "Timezones");
                //ExceptionLogging.SendErrorToText(ex, "GetCitationData", errorPath);
                //tranData.ReturnCode = ((int)ReturnCodeEnum.Error).ToString();

                tranData.ReturnCode = ((int)ReturnCodeEnum.Error).ToString();
            }
            #endregion

            //////////////////  //Reconcile data
            List<TransactionTransform> trans = new List<TransactionTransform>();
            trans.Add(tranData); // Database


            var rightValue = trans.Where(t => t.ReturnCode == ((int)ReturnCodeEnum.Success).ToString()).OrderByDescending(t => t.MeterExpiredMinutes.ToNullableInt()).FirstOrDefault<TransactionTransform>();
            if (rightValue == default(TransactionTransform))
            {
                //Do nothing.just return database  object.
            }
            else
            {
                tranData = rightValue;
            }

            return tranData;
        }

        private static Transaction GetDataMuscatPayByPlate(int customerID, string MeterName, string parkingSpaceID, string PlateNumber, string statename, string EnforcementKey, string filePath, RequestResponseEntity logData)
        {
            string ZoneID = null;
            string SpaceStatus = null;
            Transaction data = new Transaction();
            try
            {
                List<Transaction> transactionList = new List<Transaction>();

                logData.Request.CustomerId = customerID.ToString();
                logData.Request.SpaceId = parkingSpaceID;
                logData.Request.MeterId = MeterName;

                transactionList = DataAccess.GeMuscatPlateName(customerID, null, null, null, PlateNumber, statename, null);

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
            }
            catch (Exception ex)
            {
                var appPath = System.Web.Hosting.HostingEnvironment.ApplicationPhysicalPath;
                string errorPath = Path.Combine(appPath, "GetData", "Timezones");
                ExceptionLogging.SendErrorToText(ex, "GetCitationData", errorPath);
                data.ReturnCode = ((int)ReturnCodeEnum.Error).ToString();
                throw;
            }
            return data;
        }

        #endregion


        #region CityofLeavenworth
        public static TransactionTransform GetCityofLeavenworth(string EnforcementKey, string filePath, RequestResponseEntity logData, CustomerTime customerCurrentTime)
        {
            TransactionTransform tranData = new TransactionTransform();
            tranData.ReturnCode = ((int)ReturnCodeEnum.InvalidInputParameters).ToString();
            TimeZoneInfo TZ = null;
            TZ = TimeZoneInfo.FindSystemTimeZoneById("Eastern Standard Time");
            KeyParameter param = PrepareKeyParameters.PrepareLevenworthKey(EnforcementKey);  //Todo:To make WithoutZone, invalid-until Alan gives Zone number.
            if (param.ReturnCode == ((int)ReturnCodeEnum.InvalidInputParameters).ToString())
            {
                return tranData;
            }



            #region MeterName "999999999"
            if (param.MeterName.Contains("999999999") == true)
            {
                tranData.ReturnCode = ((int)ReturnCodeEnum.NoRecordExists).ToString();
                tranData.EnforcementKey = EnforcementKey;
                logData.Request.CustomerId = param.CustomerId.Value.ToString();
                logData.Response = new ResponseEntity() { CustomerId = param.CustomerId.Value.ToString(), MeterId = param.MeterName, SpaceId = param.parkingSpaceID, CityCurrentTime = customerCurrentTime.CurrentTime.Value };
                return tranData;
            }
            #endregion
            #region "DB Check"
            try
            {
                Transaction data = TransactionDataValidatioin.GetDataGenericPayBySpace(param.CustomerId.Value, param.MeterName, param.parkingSpaceID, param.PlateNumber, param.StateName, EnforcementKey, filePath, logData);


                if (data.TimeZoneId.HasValue)
                    TZ = Utility.GetTimeZoneInfo(data.TimeZoneId.Value, filePath, param.CustomerId.Value.ToString());

                //For South Miami : PayStationId. Others it is MeterID
                tranData.MeterName = (data.MeterName == null) ? string.Empty : data.MeterName;
                tranData.SpaceNo = data.ParkingSpaceID.ToString();

                tranData.MeterStreet = (data.MeterStreet == null) ? string.Empty : data.MeterStreet;

                DateTime _MeterRTC = data.PresentMeterTime.HasValue ? data.PresentMeterTime.Value : DateTime.Now;
                tranData.MeterRTC = String.Format("{0:yyyyMMdd_THHmmssZ}", Utility.findGMTTime(_MeterRTC, TZ));

                DateTime _SensorEventTime = data.SensorEventTime.HasValue ? data.SensorEventTime.Value : DateTime.Now;
                tranData.MeterLastUpdateTime = String.Format("{0:yyyyMMdd_THHmmssZ}", Utility.findGMTTime(_SensorEventTime, TZ));

                tranData.SensorEventTime = String.Format("{0:yyyyMMdd_THHmmssZ}", Utility.findGMTTime(_SensorEventTime, TZ));

                DateTime _endTime = data.ExpiryTime.HasValue ? data.ExpiryTime.Value : DateTime.Now;
                tranData.ExpiredTime = String.Format("{0:yyyyMMdd_THHmmssZ}", Utility.findGMTTime(_endTime, TZ));

                tranData.MeterExpiredMinutes = data.ExpiredMinutes.HasValue ? data.ExpiredMinutes.Value.ToString() : "0";
                tranData.METERLOC_BLOCKNUMBER = (data.Block == null) ? string.Empty : data.Block;
                tranData.METERLOC_STREETDIRECTION = (data.Direction == null) ? string.Empty : data.Direction;
                tranData.METERLOC_STREETNAME = (data.MeterStreet == null) ? string.Empty : data.MeterStreet;
                tranData.METERLOC_STREETTYPE = (data.StreetType == null) ? string.Empty : data.StreetType;
                tranData.METERLOC_CROSSSTREET1 = (data.CrossStreet1 == null) ? string.Empty : data.CrossStreet1;
                tranData.METERLOC_CROSSSTREET2 = (data.CrossStreet2 == null) ? string.Empty : data.CrossStreet2;
                tranData.METERLOC_ENFORCEMENTHOURSDESC = (data.EnfHourDesc == null) ? string.Empty : data.EnfHourDesc;
                tranData.LicensePlate = (data.PlateNumber == null) ? string.Empty : data.PlateNumber;
                tranData.State = (data.LPLocation == null) ? string.Empty : data.LPLocation;
                tranData.Make = (data.VMake == null) ? string.Empty : data.VMake;
                tranData.Model = (data.VModel == null) ? string.Empty : data.VModel;
                tranData.IsOccupied = Utility.getStatus((data.SpaceStatus == null) ? string.Empty : data.SpaceStatus).ToString();
                tranData.EnforcementKey = (data.EnforcementKey == null) ? string.Empty : data.EnforcementKey;
                tranData.MeterStatus = (data.MeterStatus == null) ? null : data.MeterStatus;

                tranData.METERLOC_METERTYPE = (data.METERLOC_METERTYPE == null) ? null : data.METERLOC_METERTYPE;
                tranData.ZoneName = (data.ZoneName == null) ? null : data.ZoneName;
                tranData.MeterID = String.Empty;
                tranData.ReturnCode = data.ReturnCode;

                logData.Response = new ResponseEntity() { CustomerId = param.CustomerId.Value.ToString(), MeterId = tranData.MeterName, SpaceId = tranData.SpaceNo, CityCurrentTime = data.PresentMeterTime.HasValue ? data.PresentMeterTime.Value : DateTime.Now };
            }
            catch (Exception ex)
            {
                //var appPath = System.Web.Hosting.HostingEnvironment.ApplicationPhysicalPath;
                //string errorPath = Path.Combine(appPath, "GetTransformData", "Timezones");
                //ExceptionLogging.SendErrorToText(ex, "GetCitationData", errorPath);
                //tranData.ReturnCode = ((int)ReturnCodeEnum.Error).ToString();

                tranData.ReturnCode = ((int)ReturnCodeEnum.Error).ToString();
            }
            #endregion
            #region "Check Zone and Space"
            TransactionTransform tranDataOfService = new TransactionTransform();
            ZoneSpace zoneSpace = null;
            bool checkParkMobile = false;
            zoneSpace = DataAccess.GetZoneSpaceForMeter(param.CustomerId.Value, param.MeterName);
            if (zoneSpace != null)
            {
                if ((!string.IsNullOrEmpty(zoneSpace.Zone)) && (!string.IsNullOrEmpty(zoneSpace.Space)))
                {
                    checkParkMobile = true;

                    #region "Parkmobile Service Call"
                    /////////////////////////////////////////////////

                    try
                    {
                        //////////////////////////////////////////
                        logData.Request.CustomerId = param.CustomerId.Value.ToString();
                        logData.Request.SpaceId = param.parkingSpaceID;
                        logData.Request.MeterId = param.MeterName;

                        int? parmmobileZoneId = zoneSpace.Zone.ToNullableInt(); // param.ZoneID.ToNullableInt();
                        int? parkmobileSpaceNo = param.parkingSpaceID.ToNullableInt();  // param.parkingSpaceID.ToNullableInt();
                        //////////////////////////////////////////////////
                        DateTime? currentCustTime = customerCurrentTime.CurrentTime;
                        PMCustomerElement customer = ParkmobileCustomerRetriever.GetCustomer(param.CustomerId.Value);
                        ParkMobileGenericFactory factory = new ParkMobileGenericFactory(customer, parmmobileZoneId.Value, parkmobileSpaceNo.Value);
                        var parkMobileResLog = new VendorResponseEntity();
                        logData.VendorData.Add(VendorNames.ParkMobile, parkMobileResLog);
                        //tranDataOfService = factory.GetRefreshSpaceData(ref parkMobileResLog);//Original
                        tranDataOfService = factory.GetRefreshSpaceData(ParkingMobileDataType.Space, ref parkMobileResLog);//modified
                        tranDataOfService.EnforcementKey = EnforcementKey;

                        ///////////////////////////////////////////////////
                        logData.Response = new ResponseEntity() { CustomerId = param.CustomerId.Value.ToString(), MeterId = tranData.MeterName, SpaceId = tranData.SpaceNo, CityCurrentTime = currentCustTime.HasValue ? currentCustTime.Value : DateTime.Now };

                    }
                    catch (Exception ex)
                    {
                        logData.Response = new ResponseEntity() { CustomerId = param.CustomerId.Value.ToString(), MeterId = tranData.MeterName, SpaceId = tranData.SpaceNo, CityCurrentTime = customerCurrentTime.CurrentTime.HasValue ? customerCurrentTime.CurrentTime.Value : DateTime.Now };
                        //throw ex;
                        tranDataOfService.ReturnCode = ((int)ReturnCodeEnum.Error).ToString();
                    }
                    finally
                    {

                    }
                    #endregion
                }
            }
            #endregion


            List<TransactionTransform> trans = new List<TransactionTransform>();
            trans.Add(tranData); // Database
            if (checkParkMobile == true)
                trans.Add(tranDataOfService); // ParkMobile


            var rightValue = trans.Where(t => t.ReturnCode == ((int)ReturnCodeEnum.Success).ToString()).OrderByDescending(t => t.MeterExpiredMinutes.ToNullableInt()).FirstOrDefault<TransactionTransform>();
            if (rightValue == default(TransactionTransform))
            {
                //Do nothing.just return database  object.
            }
            else
            {
                tranData = rightValue;
            }

            return tranData;
        }
        public static TransactionTransform GetCityofLeavenworth2(string EnforcementKey, string filePath, RequestResponseEntity logData, CustomerTime customerCurrentTime)
        {
            TransactionTransform tranData = new TransactionTransform();
            TransactionTransform tranDataOfService = new TransactionTransform();
            tranData.ReturnCode = ((int)ReturnCodeEnum.InvalidInputParameters).ToString();
            TimeZoneInfo TZ = null;
            TZ = TimeZoneInfo.FindSystemTimeZoneById("Eastern Standard Time");
            KeyParameter param = PrepareKeyParameters.PrepareLevenworthKey(EnforcementKey);  //Todo:To make WithoutZone, invalid-until Alan gives Zone number.
            if (param.ReturnCode == ((int)ReturnCodeEnum.InvalidInputParameters).ToString())
            {
                return tranData;
            }



            #region MeterName "999999999"
            if (param.MeterName.Contains("999999999") == true)
            {
                tranData.ReturnCode = ((int)ReturnCodeEnum.NoRecordExists).ToString();
                tranData.EnforcementKey = EnforcementKey;
                logData.Request.CustomerId = param.CustomerId.Value.ToString();
                logData.Response = new ResponseEntity() { CustomerId = param.CustomerId.Value.ToString(), MeterId = param.MeterName, SpaceId = param.parkingSpaceID, CityCurrentTime = customerCurrentTime.CurrentTime.Value };
                return tranData;
            }
            #endregion
            #region "Check Zone and Space"
            ZoneSpace zoneSpace = null;
            bool checkParkMobile = false;
            zoneSpace = DataAccess.GetZoneSpaceForMeter(param.CustomerId.Value, param.MeterName);
            if (zoneSpace != null)
            {
                if ((!string.IsNullOrEmpty(zoneSpace.Zone)) && (!string.IsNullOrEmpty(zoneSpace.Space)))
                {
                    #region "DB Check"
                    try
                    {
                        //Transaction data = TransactionDataValidatioin.GetDataGenericPayBySpace(param.CustomerId.Value, param.MeterName, param.parkingSpaceID, param.PlateNumber, param.StateName, EnforcementKey, filePath, logData);

                        Transaction data = TransactionDataValidatioin.GetDataGenericPayBySpace(param.CustomerId.Value, param.MeterName, zoneSpace.Space, param.PlateNumber, param.StateName, EnforcementKey, filePath, logData);


                        if (data.TimeZoneId.HasValue)
                            TZ = Utility.GetTimeZoneInfo(data.TimeZoneId.Value, filePath, param.CustomerId.Value.ToString());

                        //For South Miami : PayStationId. Others it is MeterID
                        tranData.MeterName = (data.MeterName == null) ? string.Empty : data.MeterName;
                        tranData.SpaceNo = data.ParkingSpaceID.ToString();

                        tranData.MeterStreet = (data.MeterStreet == null) ? string.Empty : data.MeterStreet;

                        DateTime _MeterRTC = data.PresentMeterTime.HasValue ? data.PresentMeterTime.Value : DateTime.Now;
                        tranData.MeterRTC = String.Format("{0:yyyyMMdd_THHmmssZ}", Utility.findGMTTime(_MeterRTC, TZ));

                        DateTime _SensorEventTime = data.SensorEventTime.HasValue ? data.SensorEventTime.Value : DateTime.Now;
                        tranData.MeterLastUpdateTime = String.Format("{0:yyyyMMdd_THHmmssZ}", Utility.findGMTTime(_SensorEventTime, TZ));

                        tranData.SensorEventTime = String.Format("{0:yyyyMMdd_THHmmssZ}", Utility.findGMTTime(_SensorEventTime, TZ));

                        DateTime _endTime = data.ExpiryTime.HasValue ? data.ExpiryTime.Value : DateTime.Now;
                        tranData.ExpiredTime = String.Format("{0:yyyyMMdd_THHmmssZ}", Utility.findGMTTime(_endTime, TZ));

                        tranData.MeterExpiredMinutes = data.ExpiredMinutes.HasValue ? data.ExpiredMinutes.Value.ToString() : "0";
                        tranData.METERLOC_BLOCKNUMBER = (data.Block == null) ? string.Empty : data.Block;
                        tranData.METERLOC_STREETDIRECTION = (data.Direction == null) ? string.Empty : data.Direction;
                        tranData.METERLOC_STREETNAME = (data.MeterStreet == null) ? string.Empty : data.MeterStreet;
                        tranData.METERLOC_STREETTYPE = (data.StreetType == null) ? string.Empty : data.StreetType;
                        tranData.METERLOC_CROSSSTREET1 = (data.CrossStreet1 == null) ? string.Empty : data.CrossStreet1;
                        tranData.METERLOC_CROSSSTREET2 = (data.CrossStreet2 == null) ? string.Empty : data.CrossStreet2;
                        tranData.METERLOC_ENFORCEMENTHOURSDESC = (data.EnfHourDesc == null) ? string.Empty : data.EnfHourDesc;
                        tranData.LicensePlate = (data.PlateNumber == null) ? string.Empty : data.PlateNumber;
                        tranData.State = (data.LPLocation == null) ? string.Empty : data.LPLocation;
                        tranData.Make = (data.VMake == null) ? string.Empty : data.VMake;
                        tranData.Model = (data.VModel == null) ? string.Empty : data.VModel;
                        tranData.IsOccupied = Utility.getStatus((data.SpaceStatus == null) ? string.Empty : data.SpaceStatus).ToString();
                        tranData.EnforcementKey = (data.EnforcementKey == null) ? string.Empty : data.EnforcementKey;
                        tranData.MeterStatus = (data.MeterStatus == null) ? null : data.MeterStatus;

                        tranData.METERLOC_METERTYPE = (data.METERLOC_METERTYPE == null) ? null : data.METERLOC_METERTYPE;
                        tranData.ZoneName = (data.ZoneName == null) ? null : data.ZoneName;
                        tranData.MeterID = String.Empty;
                        tranData.ReturnCode = data.ReturnCode;

                        logData.Response = new ResponseEntity() { CustomerId = param.CustomerId.Value.ToString(), MeterId = tranData.MeterName, SpaceId = tranData.SpaceNo, CityCurrentTime = data.PresentMeterTime.HasValue ? data.PresentMeterTime.Value : DateTime.Now };
                    }
                    catch (Exception ex)
                    {
                        //var appPath = System.Web.Hosting.HostingEnvironment.ApplicationPhysicalPath;
                        //string errorPath = Path.Combine(appPath, "GetTransformData", "Timezones");
                        //ExceptionLogging.SendErrorToText(ex, "GetCitationData", errorPath);
                        //tranData.ReturnCode = ((int)ReturnCodeEnum.Error).ToString();

                        tranData.ReturnCode = ((int)ReturnCodeEnum.Error).ToString();
                    }
                    #endregion
                    #region "Parkmobile Service Call"
                    /////////////////////////////////////////////////
                    try
                    {
                        //////////////////////////////////////////
                        logData.Request.CustomerId = param.CustomerId.Value.ToString();
                        logData.Request.SpaceId = param.parkingSpaceID;
                        logData.Request.MeterId = param.MeterName;

                        int? parmmobileZoneId = zoneSpace.Zone.ToNullableInt(); // param.ZoneID.ToNullableInt();
                        int? parkmobileSpaceNo = zoneSpace.Space.ToNullableInt();  // param.parkingSpaceID.ToNullableInt();
                        //////////////////////////////////////////////////
                        DateTime? currentCustTime = customerCurrentTime.CurrentTime;
                        PMCustomerElement customer = ParkmobileCustomerRetriever.GetCustomer(param.CustomerId.Value);
                        ParkMobileGenericFactory factory = new ParkMobileGenericFactory(customer, parmmobileZoneId.Value, parkmobileSpaceNo.Value);
                        var parkMobileResLog = new VendorResponseEntity();
                        logData.VendorData.Add(VendorNames.ParkMobile, parkMobileResLog);
                        //tranDataOfService = factory.GetRefreshSpaceData(ref parkMobileResLog);//Original
                        tranDataOfService = factory.GetRefreshSpaceData(ParkingMobileDataType.Space, ref parkMobileResLog);//modified
                        tranDataOfService.EnforcementKey = EnforcementKey;

                        ///////////////////////////////////////////////////
                        logData.Response = new ResponseEntity() { CustomerId = param.CustomerId.Value.ToString(), MeterId = tranData.MeterName, SpaceId = tranData.SpaceNo, CityCurrentTime = currentCustTime.HasValue ? currentCustTime.Value : DateTime.Now };

                    }
                    catch (Exception ex)
                    {
                        logData.Response = new ResponseEntity() { CustomerId = param.CustomerId.Value.ToString(), MeterId = tranData.MeterName, SpaceId = tranData.SpaceNo, CityCurrentTime = customerCurrentTime.CurrentTime.HasValue ? customerCurrentTime.CurrentTime.Value : DateTime.Now };
                        //throw ex;
                        tranDataOfService.ReturnCode = ((int)ReturnCodeEnum.Error).ToString();
                    }
                    finally
                    {

                    }
                    #endregion
                }
            }
            #endregion



            List<TransactionTransform> trans = new List<TransactionTransform>();
            trans.Add(tranData); // Database
            trans.Add(tranDataOfService); // ParkMobile


            var rightValue = trans.Where(t => t.ReturnCode == ((int)ReturnCodeEnum.Success).ToString()).OrderByDescending(t => t.MeterExpiredMinutes.ToNullableInt()).FirstOrDefault<TransactionTransform>();
            if (rightValue == default(TransactionTransform))
            {
                //Do nothing.just return database  object.
            }
            else
            {
                tranData = rightValue;
            }

            return tranData;
        }
        #endregion

        #region ColoradoSprings
        public static TransactionTransform GetColoradoSprings(string EnforcementKey, string filePath, RequestResponseEntity logData, CustomerTime customerCurrentTime)
        {
            TransactionTransform tranData = new TransactionTransform();
            tranData.ReturnCode = ((int)ReturnCodeEnum.InvalidInputParameters).ToString();
            TimeZoneInfo TZ = null;
            TZ = TimeZoneInfo.FindSystemTimeZoneById("Eastern Standard Time");
            KeyParameter param = PrepareKeyParameters.GenericBySpace(EnforcementKey);
            if (param.ReturnCode == ((int)ReturnCodeEnum.InvalidInputParameters).ToString())
            {
                return tranData;
            }
            /////////////////////////////////////
            #region "DB Check"
            try
            {
                Transaction data = TransactionDataValidatioin.GetDataGenericPayBySpace(param.CustomerId.Value, param.MeterName, param.parkingSpaceID, param.PlateNumber, param.StateName, EnforcementKey, filePath, logData);


                if (data.TimeZoneId.HasValue)
                    TZ = Utility.GetTimeZoneInfo(data.TimeZoneId.Value, filePath, param.CustomerId.Value.ToString());

                //For South Miami : PayStationId. Others it is MeterID
                tranData.MeterName = string.Empty; // (data.MeterName == null) ? string.Empty : data.MeterName;
                tranData.SpaceNo = string.Empty;  //data.ParkingSpaceID.ToString();

                tranData.MeterStreet = (data.MeterStreet == null) ? string.Empty : data.MeterStreet;

                DateTime _MeterRTC = data.PresentMeterTime.HasValue ? data.PresentMeterTime.Value : DateTime.Now;
                tranData.MeterRTC = String.Format("{0:yyyyMMdd_THHmmssZ}", Utility.findGMTTime(_MeterRTC, TZ));

                DateTime _SensorEventTime = data.SensorEventTime.HasValue ? data.SensorEventTime.Value : DateTime.Now;
                tranData.MeterLastUpdateTime = String.Format("{0:yyyyMMdd_THHmmssZ}", Utility.findGMTTime(_SensorEventTime, TZ));

                tranData.SensorEventTime = String.Format("{0:yyyyMMdd_THHmmssZ}", Utility.findGMTTime(_SensorEventTime, TZ));

                DateTime _endTime = data.ExpiryTime.HasValue ? data.ExpiryTime.Value : DateTime.Now;
                tranData.ExpiredTime = String.Format("{0:yyyyMMdd_THHmmssZ}", Utility.findGMTTime(_endTime, TZ));

                tranData.MeterExpiredMinutes = data.ExpiredMinutes.HasValue ? data.ExpiredMinutes.Value.ToString() : "0";
                tranData.METERLOC_BLOCKNUMBER = (data.Block == null) ? string.Empty : data.Block;
                tranData.METERLOC_STREETDIRECTION = (data.Direction == null) ? string.Empty : data.Direction;
                tranData.METERLOC_STREETNAME = (data.MeterStreet == null) ? string.Empty : data.MeterStreet;
                tranData.METERLOC_STREETTYPE = (data.StreetType == null) ? string.Empty : data.StreetType;
                tranData.METERLOC_CROSSSTREET1 = (data.CrossStreet1 == null) ? string.Empty : data.CrossStreet1;
                tranData.METERLOC_CROSSSTREET2 = (data.CrossStreet2 == null) ? string.Empty : data.CrossStreet2;
                tranData.METERLOC_ENFORCEMENTHOURSDESC = (data.EnfHourDesc == null) ? string.Empty : data.EnfHourDesc;
                tranData.LicensePlate = (data.PlateNumber == null) ? string.Empty : data.PlateNumber;
                tranData.State = (data.LPLocation == null) ? string.Empty : data.LPLocation;
                tranData.Make = (data.VMake == null) ? string.Empty : data.VMake;
                tranData.Model = (data.VModel == null) ? string.Empty : data.VModel;
                tranData.IsOccupied = Utility.getStatus((data.SpaceStatus == null) ? string.Empty : data.SpaceStatus).ToString();
                tranData.EnforcementKey = (data.EnforcementKey == null) ? string.Empty : data.EnforcementKey;
                tranData.MeterStatus = (data.MeterStatus == null) ? null : data.MeterStatus;

                tranData.METERLOC_METERTYPE = (data.METERLOC_METERTYPE == null) ? null : data.METERLOC_METERTYPE;
                tranData.ZoneName = (data.ZoneName == null) ? null : data.ZoneName;
                tranData.MeterID = String.Empty;
                tranData.ReturnCode = data.ReturnCode;

                logData.Response = new ResponseEntity() { CustomerId = param.CustomerId.Value.ToString(), MeterId = tranData.MeterName, SpaceId = tranData.SpaceNo, CityCurrentTime = data.PresentMeterTime.HasValue ? data.PresentMeterTime.Value : DateTime.Now };
            }
            catch (Exception ex)
            {
                //var appPath = System.Web.Hosting.HostingEnvironment.ApplicationPhysicalPath;
                //string errorPath = Path.Combine(appPath, "GetTransformData", "Timezones");
                //ExceptionLogging.SendErrorToText(ex, "GetCitationData", errorPath);
                //tranData.ReturnCode = ((int)ReturnCodeEnum.Error).ToString();

                tranData.ReturnCode = ((int)ReturnCodeEnum.Error).ToString();
            }
            #endregion


            //Reconcile data
            List<TransactionTransform> trans = new List<TransactionTransform>();
            trans.Add(tranData); // Database

            var rightValue = trans.Where(t => t.ReturnCode == ((int)ReturnCodeEnum.Success).ToString()).OrderByDescending(t => t.MeterExpiredMinutes.ToNullableInt()).FirstOrDefault<TransactionTransform>();
            if (rightValue == default(TransactionTransform))
            {
                //Do nothing.just return database  object.
            }
            else
            {
                tranData = rightValue;
            }

            return tranData;
        }
        public static TransactionTransform GetColoradoSprings2(string EnforcementKey, string filePath, RequestResponseEntity logData, CustomerTime customerCurrentTime)
        {
            TransactionTransform tranData = new TransactionTransform();
            tranData.ReturnCode = ((int)ReturnCodeEnum.InvalidInputParameters).ToString();
            TimeZoneInfo TZ = null;
            TZ = TimeZoneInfo.FindSystemTimeZoneById("Eastern Standard Time");
            KeyParameter param = PrepareKeyParameters.PrepareGenericPayByPlate(EnforcementKey);
            if (param.ReturnCode == ((int)ReturnCodeEnum.InvalidInputParameters).ToString())
            {
                return tranData;
            }
            /////////////////////////////////////
            #region "DB Check"
            try
            {
                Transaction data = TransactionDataValidatioin.GetDataGenericPayByPlate2(param.CustomerId.Value, param.MeterName, param.parkingSpaceID, param.PlateNumber, param.StateName, EnforcementKey, filePath, logData);


                if (data.TimeZoneId.HasValue)
                    TZ = Utility.GetTimeZoneInfo(data.TimeZoneId.Value, filePath, param.CustomerId.Value.ToString());

                //For South Miami : PayStationId. Others it is MeterID
                tranData.MeterName = string.Empty; // (data.MeterName == null) ? string.Empty : data.MeterName;
                tranData.SpaceNo = string.Empty;  //data.ParkingSpaceID.ToString();

                tranData.MeterStreet = (data.MeterStreet == null) ? string.Empty : data.MeterStreet;

                DateTime _MeterRTC = data.PresentMeterTime.HasValue ? data.PresentMeterTime.Value : DateTime.Now;
                tranData.MeterRTC = String.Format("{0:yyyyMMdd_THHmmssZ}", Utility.findGMTTime(_MeterRTC, TZ));

                DateTime _SensorEventTime = data.SensorEventTime.HasValue ? data.SensorEventTime.Value : DateTime.Now;
                tranData.MeterLastUpdateTime = String.Format("{0:yyyyMMdd_THHmmssZ}", Utility.findGMTTime(_SensorEventTime, TZ));

                tranData.SensorEventTime = String.Format("{0:yyyyMMdd_THHmmssZ}", Utility.findGMTTime(_SensorEventTime, TZ));

                DateTime _endTime = data.ExpiryTime.HasValue ? data.ExpiryTime.Value : DateTime.Now;
                tranData.ExpiredTime = String.Format("{0:yyyyMMdd_THHmmssZ}", Utility.findGMTTime(_endTime, TZ));

                tranData.MeterExpiredMinutes = data.ExpiredMinutes.HasValue ? data.ExpiredMinutes.Value.ToString() : "0";
                tranData.METERLOC_BLOCKNUMBER = (data.Block == null) ? string.Empty : data.Block;
                tranData.METERLOC_STREETDIRECTION = (data.Direction == null) ? string.Empty : data.Direction;
                tranData.METERLOC_STREETNAME = (data.MeterStreet == null) ? string.Empty : data.MeterStreet;
                tranData.METERLOC_STREETTYPE = (data.StreetType == null) ? string.Empty : data.StreetType;
                tranData.METERLOC_CROSSSTREET1 = (data.CrossStreet1 == null) ? string.Empty : data.CrossStreet1;
                tranData.METERLOC_CROSSSTREET2 = (data.CrossStreet2 == null) ? string.Empty : data.CrossStreet2;
                tranData.METERLOC_ENFORCEMENTHOURSDESC = (data.EnfHourDesc == null) ? string.Empty : data.EnfHourDesc;
                tranData.LicensePlate = (data.PlateNumber == null) ? string.Empty : data.PlateNumber;
                tranData.State = (data.LPLocation == null) ? string.Empty : data.LPLocation;
                tranData.Make = (data.VMake == null) ? string.Empty : data.VMake;
                tranData.Model = (data.VModel == null) ? string.Empty : data.VModel;
                tranData.IsOccupied = Utility.getStatus((data.SpaceStatus == null) ? string.Empty : data.SpaceStatus).ToString();
                tranData.EnforcementKey = (data.EnforcementKey == null) ? string.Empty : data.EnforcementKey;
                tranData.MeterStatus = (data.MeterStatus == null) ? null : data.MeterStatus;

                tranData.METERLOC_METERTYPE = (data.METERLOC_METERTYPE == null) ? null : data.METERLOC_METERTYPE;
                tranData.ZoneName = (data.ZoneName == null) ? null : data.ZoneName;
                tranData.MeterID = String.Empty;
                tranData.ReturnCode = data.ReturnCode;

                logData.Response = new ResponseEntity() { CustomerId = param.CustomerId.Value.ToString(), MeterId = tranData.MeterName, SpaceId = tranData.SpaceNo, CityCurrentTime = data.PresentMeterTime.HasValue ? data.PresentMeterTime.Value : DateTime.Now };
            }
            catch (Exception ex)
            {
                //var appPath = System.Web.Hosting.HostingEnvironment.ApplicationPhysicalPath;
                //string errorPath = Path.Combine(appPath, "GetTransformData", "Timezones");
                //ExceptionLogging.SendErrorToText(ex, "GetCitationData", errorPath);
                //tranData.ReturnCode = ((int)ReturnCodeEnum.Error).ToString();

                tranData.ReturnCode = ((int)ReturnCodeEnum.Error).ToString();
            }
            #endregion
            #region "Cale Service Call"
            /////////////////////////////////////////////////
            TransactionTransform tranDataOfService = new TransactionTransform();
            try
            {
                //////////////////////////////////////////
                logData.Request.CustomerId = param.CustomerId.Value.ToString();
                logData.Request.SpaceId = param.parkingSpaceID;
                logData.Request.MeterId = param.MeterName;
                //////////////////////////////////////////////////
                DateTime? currentCustTime = customerCurrentTime.CurrentTime;
                Cale.CaleCustomerElement customer = Cale.CaleCustomerRetriever.GetCustomer(param.CustomerId.Value);
                Cale.CaleGenericFactory factory = new Cale.CaleGenericFactory(customer, param.PlateNumber, null);
                var caleResLog = new VendorResponseEntity();
                logData.VendorData.Add(VendorNames.Cale, caleResLog);
                if (customer.EnforcementType == EnumEnforcementType.PayByPlate.ToString())
                    tranDataOfService = factory.GetRefreshPlateData(ref caleResLog);
                //tranDataOfService = factory.GetRefreshPlateData();
                tranDataOfService.EnforcementKey = EnforcementKey;

                ///////////////////////////////////////////////////
                logData.Response = new ResponseEntity() { CustomerId = param.CustomerId.Value.ToString(), MeterId = tranData.MeterName, SpaceId = tranData.SpaceNo, CityCurrentTime = currentCustTime.HasValue ? currentCustTime.Value : DateTime.Now };
            }
            catch (Exception ex)
            {
                logData.Response = new ResponseEntity() { CustomerId = param.CustomerId.Value.ToString(), MeterId = tranData.MeterName, SpaceId = tranData.SpaceNo, CityCurrentTime = customerCurrentTime.CurrentTime.HasValue ? customerCurrentTime.CurrentTime.Value : DateTime.Now };
                //throw ex;
                tranData.ReturnCode = ((int)ReturnCodeEnum.Error).ToString();
            }
            finally
            {

            }
            #endregion
            #region "Parkmobile Service Call"
            /////////////////////////////////////////////////
            TransactionTransform tranDataOfServiceParkMobile = new TransactionTransform();
            try
            {
                //////////////////////////////////////////
                logData.Request.CustomerId = param.CustomerId.Value.ToString();
                logData.Request.SpaceId = param.parkingSpaceID;
                logData.Request.MeterId = param.MeterName;
                //////////////////////////////////////////////////
                DateTime? currentCustTime = customerCurrentTime.CurrentTime;
                PMCustomerElement customer = ParkmobileCustomerRetriever.GetCustomer(param.CustomerId.Value);
                ParkMobileGenericFactory factory = new ParkMobileGenericFactory(customer, param.PlateNumber);
                var parkMobileResLog = new VendorResponseEntity();
                logData.VendorData.Add(VendorNames.ParkMobile, parkMobileResLog);
                tranDataOfServiceParkMobile = factory.GetRefreshSpaceData(ParkingMobileDataType.Plate, ref parkMobileResLog);
                tranDataOfServiceParkMobile.EnforcementKey = EnforcementKey;

                ///////////////////////////////////////////////////
                logData.Response = new ResponseEntity() { CustomerId = param.CustomerId.Value.ToString(), MeterId = tranData.MeterName, SpaceId = tranData.SpaceNo, CityCurrentTime = currentCustTime.HasValue ? currentCustTime.Value : DateTime.Now };

            }
            catch (Exception ex)
            {
                logData.Response = new ResponseEntity() { CustomerId = param.CustomerId.Value.ToString(), MeterId = tranData.MeterName, SpaceId = tranData.SpaceNo, CityCurrentTime = customerCurrentTime.CurrentTime.HasValue ? customerCurrentTime.CurrentTime.Value : DateTime.Now };
                //throw ex;
                tranDataOfServiceParkMobile.ReturnCode = ((int)ReturnCodeEnum.Error).ToString();
            }
            finally
            {

            }
            #endregion

            //Reconcile data
            List<TransactionTransform> trans = new List<TransactionTransform>();
            trans.Add(tranData); // Database
            trans.Add(tranDataOfService); // Cale
            trans.Add(tranDataOfServiceParkMobile); // ParkMobile

            var rightValue = trans.Where(t => t.ReturnCode == ((int)ReturnCodeEnum.Success).ToString()).OrderByDescending(t => t.MeterExpiredMinutes.ToNullableInt()).FirstOrDefault<TransactionTransform>();
            if (rightValue == default(TransactionTransform))
            {
                //Do nothing.just return database  object.
            }
            else
            {
                tranData = rightValue;
            }

            return tranData;
        }
        #endregion



        #region Fuelcity
        public static TransactionTransform GetCityofFuelcity(string EnforcementKey, string filePath, RequestResponseEntity logData, CustomerTime customerCurrentTime)
        {
            TransactionTransform tranData = new TransactionTransform();
            tranData.ReturnCode = ((int)ReturnCodeEnum.InvalidInputParameters).ToString();
            TimeZoneInfo TZ = null;
            TZ = TimeZoneInfo.FindSystemTimeZoneById("Eastern Standard Time");
            //int? customerID = null;
            //string MeterName = null;
            //string parkingSpaceID;
            //string PlateNumber = null;
            //string stateName = null;
            //////////////////////////////////////
            KeyParameter param = PrepareKeyParameters.PrepareCityofChester(EnforcementKey);
            if (param.ReturnCode == ((int)ReturnCodeEnum.InvalidInputParameters).ToString())
            {
                return tranData;
            }
            #region DB
            /////////////////////////////////////
            try
            {
                Transaction data = TransactionDataValidatioin.GetDataFuelcity(param.CustomerId.Value, param.MeterName, param.parkingSpaceID, param.PlateNumber, param.StateName, EnforcementKey, filePath, logData);


                if (data.TimeZoneId.HasValue)
                    TZ = Utility.GetTimeZoneInfo(data.TimeZoneId.Value, filePath, param.CustomerId.Value.ToString());

                //For South Miami : PayStationId. Others it is MeterID
                tranData.MeterName = (data.MeterName == null) ? string.Empty : data.MeterName;
                tranData.SpaceNo = data.ParkingSpaceID.ToString();

                tranData.MeterStreet = (data.MeterStreet == null) ? string.Empty : data.MeterStreet;

                DateTime _MeterRTC = data.PresentMeterTime.HasValue ? data.PresentMeterTime.Value : DateTime.Now;
                tranData.MeterRTC = String.Format("{0:yyyyMMdd_THHmmssZ}", Utility.findGMTTime(_MeterRTC, TZ));

                DateTime _SensorEventTime = data.SensorEventTime.HasValue ? data.SensorEventTime.Value : DateTime.Now;
                tranData.MeterLastUpdateTime = String.Format("{0:yyyyMMdd_THHmmssZ}", Utility.findGMTTime(_SensorEventTime, TZ));

                tranData.SensorEventTime = String.Format("{0:yyyyMMdd_THHmmssZ}", Utility.findGMTTime(_SensorEventTime, TZ));

                DateTime _endTime = data.ExpiryTime.HasValue ? data.ExpiryTime.Value : DateTime.Now;
                tranData.ExpiredTime = String.Format("{0:yyyyMMdd_THHmmssZ}", Utility.findGMTTime(_endTime, TZ));

                tranData.MeterExpiredMinutes = data.ExpiredMinutes.HasValue ? data.ExpiredMinutes.Value.ToString() : "0";
                tranData.METERLOC_BLOCKNUMBER = (data.Block == null) ? string.Empty : data.Block;
                tranData.METERLOC_STREETDIRECTION = (data.Direction == null) ? string.Empty : data.Direction;
                tranData.METERLOC_STREETNAME = (data.MeterStreet == null) ? string.Empty : data.MeterStreet;
                tranData.METERLOC_STREETTYPE = (data.StreetType == null) ? string.Empty : data.StreetType;
                tranData.METERLOC_CROSSSTREET1 = (data.CrossStreet1 == null) ? string.Empty : data.CrossStreet1;
                tranData.METERLOC_CROSSSTREET2 = (data.CrossStreet2 == null) ? string.Empty : data.CrossStreet2;
                tranData.METERLOC_ENFORCEMENTHOURSDESC = (data.EnfHourDesc == null) ? string.Empty : data.EnfHourDesc;
                tranData.LicensePlate = (data.PlateNumber == null) ? string.Empty : data.PlateNumber;
                tranData.State = (data.LPLocation == null) ? string.Empty : data.LPLocation;
                tranData.Make = (data.VMake == null) ? string.Empty : data.VMake;
                tranData.Model = (data.VModel == null) ? string.Empty : data.VModel;
                tranData.IsOccupied = Utility.getStatus((data.SpaceStatus == null) ? string.Empty : data.SpaceStatus).ToString();
                tranData.EnforcementKey = (data.EnforcementKey == null) ? string.Empty : data.EnforcementKey;
                tranData.MeterStatus = (data.MeterStatus == null) ? null : data.MeterStatus;

                tranData.METERLOC_METERTYPE = (data.METERLOC_METERTYPE == null) ? null : data.METERLOC_METERTYPE;
                tranData.ZoneName = (data.ZoneName == null) ? null : data.ZoneName;

                tranData.MeterID = data.PayStationID.HasValue ? data.PayStationID.Value.ToString() : "0";
                tranData.ReturnCode = data.ReturnCode;

                logData.Response = new ResponseEntity() { CustomerId = param.CustomerId.Value.ToString(), MeterId = tranData.MeterName, SpaceId = tranData.SpaceNo, CityCurrentTime = data.PresentMeterTime.HasValue ? data.PresentMeterTime.Value : DateTime.Now };
            }
            catch (Exception ex)
            {
                //var appPath = System.Web.Hosting.HostingEnvironment.ApplicationPhysicalPath;
                //string errorPath = Path.Combine(appPath, "GetTransformData", "Timezones");
                //ExceptionLogging.SendErrorToText(ex, "GetCitationData", errorPath);
                //tranData.ReturnCode = ((int)ReturnCodeEnum.Error).ToString();


            }
            #endregion

            List<TransactionTransform> trans = new List<TransactionTransform>();
            trans.Add(tranData); // Database

            var rightValue = trans.Where(t => t.ReturnCode == ((int)ReturnCodeEnum.Success).ToString()).OrderByDescending(t => t.MeterExpiredMinutes.ToNullableInt()).FirstOrDefault<TransactionTransform>();
            if (rightValue == default(TransactionTransform))
            {
                //Do nothing.just return database  object.
            }
            else
            {
                tranData = rightValue;
            }

            return tranData;
        }
        private static Transaction GetDataFuelcity(int customerID, string MeterName, string parkingSpaceID, string PlateNumber, string statename, string EnforcementKey, string filePath, RequestResponseEntity logData)
        {
            string ZoneID = null;
            string SpaceStatus = null;
            Transaction data = new Transaction();
            try
            {
                List<Transaction> transactionList = new List<Transaction>();

                logData.Request.CustomerId = customerID.ToString();
                logData.Request.SpaceId = parkingSpaceID;
                logData.Request.MeterId = MeterName;

                transactionList = DataAccess.GetFuelCityCitationData(customerID, null, MeterName, parkingSpaceID, null, null);

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
            }
            catch (Exception ex)
            {
                var appPath = System.Web.Hosting.HostingEnvironment.ApplicationPhysicalPath;
                string errorPath = Path.Combine(appPath, "GetData", "Timezones");
                ExceptionLogging.SendErrorToText(ex, "GetCitationData", errorPath);
                data.ReturnCode = ((int)ReturnCodeEnum.Error).ToString();
                throw;
            }
            return data;
        }
        #endregion


        #region JacksonvilleFL
        public static TransactionTransform GetJacksonvilleFLTransformData(string EnforcementKey, string filePath, RequestResponseEntity logData, CustomerTime customerCurrentTime)
        {
            TransactionTransform tranData = new TransactionTransform();
            tranData.ReturnCode = ((int)ReturnCodeEnum.InvalidInputParameters).ToString();
            TimeZoneInfo TZ = null;
            TZ = TimeZoneInfo.FindSystemTimeZoneById("Eastern Standard Time");
            //////////////////////////////////////
            KeyParameter param = PrepareKeyParameters.PrepareTempe(EnforcementKey);
            if (param.ReturnCode == ((int)ReturnCodeEnum.InvalidInputParameters).ToString())
            {
                return tranData;
            }

            #region "DB Check"
            try
            {
                Transaction data = TransactionDataValidatioin.GetDataGenericPayByPlate2(param.CustomerId.Value, param.MeterName, param.parkingSpaceID, param.PlateNumber, param.StateName, EnforcementKey, filePath, logData);


                if (data.TimeZoneId.HasValue)
                    TZ = Utility.GetTimeZoneInfo(data.TimeZoneId.Value, filePath, param.CustomerId.Value.ToString());

                //For South Miami : PayStationId. Others it is MeterID
                tranData.MeterName = string.Empty; // (data.MeterName == null) ? string.Empty : data.MeterName;
                tranData.SpaceNo = string.Empty;  //data.ParkingSpaceID.ToString();

                tranData.MeterStreet = (data.MeterStreet == null) ? string.Empty : data.MeterStreet;

                DateTime _MeterRTC = data.PresentMeterTime.HasValue ? data.PresentMeterTime.Value : DateTime.Now;
                tranData.MeterRTC = String.Format("{0:yyyyMMdd_THHmmssZ}", Utility.findGMTTime(_MeterRTC, TZ));

                DateTime _SensorEventTime = data.SensorEventTime.HasValue ? data.SensorEventTime.Value : DateTime.Now;
                tranData.MeterLastUpdateTime = String.Format("{0:yyyyMMdd_THHmmssZ}", Utility.findGMTTime(_SensorEventTime, TZ));

                tranData.SensorEventTime = String.Format("{0:yyyyMMdd_THHmmssZ}", Utility.findGMTTime(_SensorEventTime, TZ));

                DateTime _endTime = data.ExpiryTime.HasValue ? data.ExpiryTime.Value : DateTime.Now;
                tranData.ExpiredTime = String.Format("{0:yyyyMMdd_THHmmssZ}", Utility.findGMTTime(_endTime, TZ));

                tranData.MeterExpiredMinutes = data.ExpiredMinutes.HasValue ? data.ExpiredMinutes.Value.ToString() : "0";
                tranData.METERLOC_BLOCKNUMBER = (data.Block == null) ? string.Empty : data.Block;
                tranData.METERLOC_STREETDIRECTION = (data.Direction == null) ? string.Empty : data.Direction;
                tranData.METERLOC_STREETNAME = (data.MeterStreet == null) ? string.Empty : data.MeterStreet;
                tranData.METERLOC_STREETTYPE = (data.StreetType == null) ? string.Empty : data.StreetType;
                tranData.METERLOC_CROSSSTREET1 = (data.CrossStreet1 == null) ? string.Empty : data.CrossStreet1;
                tranData.METERLOC_CROSSSTREET2 = (data.CrossStreet2 == null) ? string.Empty : data.CrossStreet2;
                tranData.METERLOC_ENFORCEMENTHOURSDESC = (data.EnfHourDesc == null) ? string.Empty : data.EnfHourDesc;
                tranData.LicensePlate = (data.PlateNumber == null) ? string.Empty : data.PlateNumber;
                tranData.State = (data.LPLocation == null) ? string.Empty : data.LPLocation;
                tranData.Make = (data.VMake == null) ? string.Empty : data.VMake;
                tranData.Model = (data.VModel == null) ? string.Empty : data.VModel;
                tranData.IsOccupied = Utility.getStatus((data.SpaceStatus == null) ? string.Empty : data.SpaceStatus).ToString();
                tranData.EnforcementKey = (data.EnforcementKey == null) ? string.Empty : data.EnforcementKey;
                tranData.MeterStatus = (data.MeterStatus == null) ? null : data.MeterStatus;

                tranData.METERLOC_METERTYPE = (data.METERLOC_METERTYPE == null) ? null : data.METERLOC_METERTYPE;
                tranData.ZoneName = (data.ZoneName == null) ? null : data.ZoneName;
                tranData.MeterID = String.Empty;
                tranData.ReturnCode = data.ReturnCode;

                logData.Response = new ResponseEntity() { CustomerId = param.CustomerId.Value.ToString(), MeterId = tranData.MeterName, SpaceId = tranData.SpaceNo, CityCurrentTime = data.PresentMeterTime.HasValue ? data.PresentMeterTime.Value : DateTime.Now };
            }
            catch (Exception ex)
            {
                //var appPath = System.Web.Hosting.HostingEnvironment.ApplicationPhysicalPath;
                //string errorPath = Path.Combine(appPath, "GetTransformData", "Timezones");
                //ExceptionLogging.SendErrorToText(ex, "GetCitationData", errorPath);
                //tranData.ReturnCode = ((int)ReturnCodeEnum.Error).ToString();

                tranData.ReturnCode = ((int)ReturnCodeEnum.Error).ToString();
            }
            #endregion
            #region "Parkmobile Service Call"
            /////////////////////////////////////////////////
            TransactionTransform tranDataOfServiceParkMobile = new TransactionTransform();
            try
            {
                //////////////////////////////////////////
                logData.Request.CustomerId = param.CustomerId.Value.ToString();
                logData.Request.SpaceId = param.parkingSpaceID;
                logData.Request.MeterId = param.MeterName;
                //////////////////////////////////////////////////
                DateTime? currentCustTime = customerCurrentTime.CurrentTime;
                PMCustomerElement customer = ParkmobileCustomerRetriever.GetCustomer(param.CustomerId.Value);
                ParkMobileGenericFactory factory = new ParkMobileGenericFactory(customer, param.PlateNumber);
                var parkMobileResLog = new VendorResponseEntity();
                logData.VendorData.Add(VendorNames.ParkMobile, parkMobileResLog);
                tranDataOfServiceParkMobile = factory.GetRefreshSpaceData(ParkingMobileDataType.Plate, ref parkMobileResLog);
                tranDataOfServiceParkMobile.EnforcementKey = EnforcementKey;

                ///////////////////////////////////////////////////
                logData.Response = new ResponseEntity() { CustomerId = param.CustomerId.Value.ToString(), MeterId = tranData.MeterName, SpaceId = tranData.SpaceNo, CityCurrentTime = currentCustTime.HasValue ? currentCustTime.Value : DateTime.Now };
            }
            catch (Exception ex)
            {
                logData.Response = new ResponseEntity() { CustomerId = param.CustomerId.Value.ToString(), MeterId = tranData.MeterName, SpaceId = tranData.SpaceNo, CityCurrentTime = customerCurrentTime.CurrentTime.HasValue ? customerCurrentTime.CurrentTime.Value : DateTime.Now };
                //throw ex;
                tranDataOfServiceParkMobile.ReturnCode = ((int)ReturnCodeEnum.Error).ToString();
            }
            finally
            {

            }
            #endregion
            //////////////////  //Reconcile data

            List<TransactionTransform> trans = new List<TransactionTransform>();
            trans.Add(tranDataOfServiceParkMobile); // Parkmobile
            trans.Add(tranData); // Database

            var rightValue = trans.Where(t => t.ReturnCode == ((int)ReturnCodeEnum.Success).ToString()).OrderByDescending(t => t.MeterExpiredMinutes.ToNullableInt()).FirstOrDefault<TransactionTransform>();
            if (rightValue == default(TransactionTransform))
            {
                //Do nothing.just return database  object.
            }
            else
            {
                tranData = rightValue;
            }

            return tranData;
        }
        public static TransactionTransform GetJacksonvilleFLTransformDataV1(string EnforcementKey, string filePath, RequestResponseEntity logData, CustomerTime customerCurrentTime)
        {
            TransactionTransform tranData = new TransactionTransform();
            tranData.ReturnCode = ((int)ReturnCodeEnum.InvalidInputParameters).ToString();
            TimeZoneInfo TZ = null;
            TZ = TimeZoneInfo.FindSystemTimeZoneById("Eastern Standard Time");
            //////////////////////////////////////
            KeyParameter param = PrepareKeyParameters.PrepareTempe(EnforcementKey);
            if (param.ReturnCode == ((int)ReturnCodeEnum.InvalidInputParameters).ToString())
            {
                return tranData;
            }

            #region "DB Check"
            try
            {
                Transaction data = TransactionDataValidatioin.GetDataGenericPayByPlate2(param.CustomerId.Value, param.MeterName, param.parkingSpaceID, param.PlateNumber, param.StateName, EnforcementKey, filePath, logData);


                if (data.TimeZoneId.HasValue)
                    TZ = Utility.GetTimeZoneInfo(data.TimeZoneId.Value, filePath, param.CustomerId.Value.ToString());

                //For South Miami : PayStationId. Others it is MeterID
                tranData.MeterName = string.Empty; // (data.MeterName == null) ? string.Empty : data.MeterName;
                tranData.SpaceNo = string.Empty;  //data.ParkingSpaceID.ToString();

                tranData.MeterStreet = (data.MeterStreet == null) ? string.Empty : data.MeterStreet;

                DateTime _MeterRTC = data.PresentMeterTime.HasValue ? data.PresentMeterTime.Value : DateTime.Now;
                tranData.MeterRTC = String.Format("{0:yyyyMMdd_THHmmssZ}", Utility.findGMTTime(_MeterRTC, TZ));

                DateTime _SensorEventTime = data.SensorEventTime.HasValue ? data.SensorEventTime.Value : DateTime.Now;
                tranData.MeterLastUpdateTime = String.Format("{0:yyyyMMdd_THHmmssZ}", Utility.findGMTTime(_SensorEventTime, TZ));

                tranData.SensorEventTime = String.Format("{0:yyyyMMdd_THHmmssZ}", Utility.findGMTTime(_SensorEventTime, TZ));

                DateTime _endTime = data.ExpiryTime.HasValue ? data.ExpiryTime.Value : DateTime.Now;
                tranData.ExpiredTime = String.Format("{0:yyyyMMdd_THHmmssZ}", Utility.findGMTTime(_endTime, TZ));

                tranData.MeterExpiredMinutes = data.ExpiredMinutes.HasValue ? data.ExpiredMinutes.Value.ToString() : "0";
                tranData.METERLOC_BLOCKNUMBER = (data.Block == null) ? string.Empty : data.Block;
                tranData.METERLOC_STREETDIRECTION = (data.Direction == null) ? string.Empty : data.Direction;
                tranData.METERLOC_STREETNAME = (data.MeterStreet == null) ? string.Empty : data.MeterStreet;
                tranData.METERLOC_STREETTYPE = (data.StreetType == null) ? string.Empty : data.StreetType;
                tranData.METERLOC_CROSSSTREET1 = (data.CrossStreet1 == null) ? string.Empty : data.CrossStreet1;
                tranData.METERLOC_CROSSSTREET2 = (data.CrossStreet2 == null) ? string.Empty : data.CrossStreet2;
                tranData.METERLOC_ENFORCEMENTHOURSDESC = (data.EnfHourDesc == null) ? string.Empty : data.EnfHourDesc;
                tranData.LicensePlate = (data.PlateNumber == null) ? string.Empty : data.PlateNumber;
                tranData.State = (data.LPLocation == null) ? string.Empty : data.LPLocation;
                tranData.Make = (data.VMake == null) ? string.Empty : data.VMake;
                tranData.Model = (data.VModel == null) ? string.Empty : data.VModel;
                tranData.IsOccupied = Utility.getStatus((data.SpaceStatus == null) ? string.Empty : data.SpaceStatus).ToString();
                tranData.EnforcementKey = (data.EnforcementKey == null) ? string.Empty : data.EnforcementKey;
                tranData.MeterStatus = (data.MeterStatus == null) ? null : data.MeterStatus;

                tranData.METERLOC_METERTYPE = (data.METERLOC_METERTYPE == null) ? null : data.METERLOC_METERTYPE;
                tranData.ZoneName = (data.ZoneName == null) ? null : data.ZoneName;
                tranData.MeterID = String.Empty;
                tranData.ReturnCode = data.ReturnCode;

                logData.Response = new ResponseEntity() { CustomerId = param.CustomerId.Value.ToString(), MeterId = tranData.MeterName, SpaceId = tranData.SpaceNo, CityCurrentTime = data.PresentMeterTime.HasValue ? data.PresentMeterTime.Value : DateTime.Now };
            }
            catch (Exception ex)
            {
                //var appPath = System.Web.Hosting.HostingEnvironment.ApplicationPhysicalPath;
                //string errorPath = Path.Combine(appPath, "GetTransformData", "Timezones");
                //ExceptionLogging.SendErrorToText(ex, "GetCitationData", errorPath);
                //tranData.ReturnCode = ((int)ReturnCodeEnum.Error).ToString();

                tranData.ReturnCode = ((int)ReturnCodeEnum.Error).ToString();
            }
            #endregion
            #region "Parkmobile Service Call"
            /////////////////////////////////////////////////
            TransactionTransform tranDataOfServiceParkMobile = new TransactionTransform();
            try
            {
                //////////////////////////////////////////
                logData.Request.CustomerId = param.CustomerId.Value.ToString();
                logData.Request.SpaceId = param.parkingSpaceID;
                logData.Request.MeterId = param.MeterName;
                //////////////////////////////////////////////////
                DateTime? currentCustTime = customerCurrentTime.CurrentTime;
                PMCustomerElement customer = ParkmobileCustomerRetriever.GetCustomer(param.CustomerId.Value);
                ParkMobileGenericFactory factory = new ParkMobileGenericFactory(customer, param.PlateNumber);
                var parkMobileResLog = new VendorResponseEntity();
                logData.VendorData.Add(VendorNames.ParkMobile, parkMobileResLog);
                tranDataOfServiceParkMobile = factory.GetRefreshSpaceData(ParkingMobileDataType.Plate, ref parkMobileResLog);
                tranDataOfServiceParkMobile.EnforcementKey = EnforcementKey;

                ///////////////////////////////////////////////////
                logData.Response = new ResponseEntity() { CustomerId = param.CustomerId.Value.ToString(), MeterId = tranData.MeterName, SpaceId = tranData.SpaceNo, CityCurrentTime = currentCustTime.HasValue ? currentCustTime.Value : DateTime.Now };
            }
            catch (Exception ex)
            {
                logData.Response = new ResponseEntity() { CustomerId = param.CustomerId.Value.ToString(), MeterId = tranData.MeterName, SpaceId = tranData.SpaceNo, CityCurrentTime = customerCurrentTime.CurrentTime.HasValue ? customerCurrentTime.CurrentTime.Value : DateTime.Now };
                //throw ex;
                tranDataOfServiceParkMobile.ReturnCode = ((int)ReturnCodeEnum.Error).ToString();
            }
            finally
            {

            }
            #endregion
            #region "IPC"
            TransactionTransform tranDataOfServiceIPC = new TransactionTransform();
            try
            {
                //////////////////////////////////////////
                logData.Request.CustomerId = param.CustomerId.Value.ToString();
                logData.Request.SpaceId = param.parkingSpaceID;
                logData.Request.MeterId = param.MeterName;
                //////////////////////////////////////////////////
                DateTime? currentCustTime = customerCurrentTime.CurrentTime;
                IPC.IPCCustomerElement customer = IPCCustomerRetriever.GetCustomer(param.CustomerId.Value);
                IPC.IPCGenericFactory factory = new IPC.IPCGenericFactory(customer, param.PlateNumber, string.Empty, customerCurrentTime);
                var ipcResLog = new VendorResponseEntity();
                logData.VendorData.Add(VendorNames.IPC, ipcResLog);
                tranDataOfServiceIPC = factory.GetRefreshPlateData(ref ipcResLog);
                tranDataOfServiceIPC.EnforcementKey = EnforcementKey;

                ///////////////////////////////////////////////////
                logData.Response = new ResponseEntity() { CustomerId = param.CustomerId.Value.ToString(), MeterId = tranData.MeterName, SpaceId = tranData.SpaceNo, CityCurrentTime = currentCustTime.HasValue ? currentCustTime.Value : DateTime.Now };

            }
            catch (Exception ex)
            {
                logData.Response = new ResponseEntity() { CustomerId = param.CustomerId.Value.ToString(), MeterId = tranData.MeterName, SpaceId = tranData.SpaceNo, CityCurrentTime = customerCurrentTime.CurrentTime.HasValue ? customerCurrentTime.CurrentTime.Value : DateTime.Now };
                //throw ex;
                tranDataOfServiceIPC.ReturnCode = ((int)ReturnCodeEnum.Error).ToString();
            }
            finally
            {

            }
            #endregion

            //////////////////  //Reconcile data

            List<TransactionTransform> trans = new List<TransactionTransform>();
            trans.Add(tranDataOfServiceParkMobile); // Parkmobile
            trans.Add(tranData); // Database
            trans.Add(tranDataOfServiceIPC); //IPS

            var rightValue = trans.Where(t => t.ReturnCode == ((int)ReturnCodeEnum.Success).ToString()).OrderByDescending(t => t.MeterExpiredMinutes.ToNullableInt()).FirstOrDefault<TransactionTransform>();
            if (rightValue == default(TransactionTransform))
            {
                //Do nothing.just return database  object.
            }
            else
            {
                tranData = rightValue;
            }

            return tranData;
        }


        #endregion


        #region Chillicothe OH
        public static TransactionTransform GetChillicotheOHTransformData(string EnforcementKey, string filePath, RequestResponseEntity logData, CustomerTime customerCurrentTime)
        {
            TransactionTransform tranData = new TransactionTransform();
            tranData.ReturnCode = ((int)ReturnCodeEnum.InvalidInputParameters).ToString();
            TimeZoneInfo TZ = null;
            TZ = TimeZoneInfo.FindSystemTimeZoneById("Eastern Standard Time");
            //////////////////////////////////////
            KeyParameter param = PrepareKeyParameters.PrepareTempe(EnforcementKey);
            if (param.ReturnCode == ((int)ReturnCodeEnum.InvalidInputParameters).ToString())
            {
                return tranData;
            }

            #region "DB Check"
            try
            {
                Transaction data = TransactionDataValidatioin.GetDataGenericPayByPlate2(param.CustomerId.Value, param.MeterName, param.parkingSpaceID, param.PlateNumber, param.StateName, EnforcementKey, filePath, logData);


                if (data.TimeZoneId.HasValue)
                    TZ = Utility.GetTimeZoneInfo(data.TimeZoneId.Value, filePath, param.CustomerId.Value.ToString());

                //For South Miami : PayStationId. Others it is MeterID
                tranData.MeterName = string.Empty; // (data.MeterName == null) ? string.Empty : data.MeterName;
                tranData.SpaceNo = string.Empty;  //data.ParkingSpaceID.ToString();

                tranData.MeterStreet = (data.MeterStreet == null) ? string.Empty : data.MeterStreet;

                DateTime _MeterRTC = data.PresentMeterTime.HasValue ? data.PresentMeterTime.Value : DateTime.Now;
                tranData.MeterRTC = String.Format("{0:yyyyMMdd_THHmmssZ}", Utility.findGMTTime(_MeterRTC, TZ));

                DateTime _SensorEventTime = data.SensorEventTime.HasValue ? data.SensorEventTime.Value : DateTime.Now;
                tranData.MeterLastUpdateTime = String.Format("{0:yyyyMMdd_THHmmssZ}", Utility.findGMTTime(_SensorEventTime, TZ));

                tranData.SensorEventTime = String.Format("{0:yyyyMMdd_THHmmssZ}", Utility.findGMTTime(_SensorEventTime, TZ));

                DateTime _endTime = data.ExpiryTime.HasValue ? data.ExpiryTime.Value : DateTime.Now;
                tranData.ExpiredTime = String.Format("{0:yyyyMMdd_THHmmssZ}", Utility.findGMTTime(_endTime, TZ));

                tranData.MeterExpiredMinutes = data.ExpiredMinutes.HasValue ? data.ExpiredMinutes.Value.ToString() : "0";
                tranData.METERLOC_BLOCKNUMBER = (data.Block == null) ? string.Empty : data.Block;
                tranData.METERLOC_STREETDIRECTION = (data.Direction == null) ? string.Empty : data.Direction;
                tranData.METERLOC_STREETNAME = (data.MeterStreet == null) ? string.Empty : data.MeterStreet;
                tranData.METERLOC_STREETTYPE = (data.StreetType == null) ? string.Empty : data.StreetType;
                tranData.METERLOC_CROSSSTREET1 = (data.CrossStreet1 == null) ? string.Empty : data.CrossStreet1;
                tranData.METERLOC_CROSSSTREET2 = (data.CrossStreet2 == null) ? string.Empty : data.CrossStreet2;
                tranData.METERLOC_ENFORCEMENTHOURSDESC = (data.EnfHourDesc == null) ? string.Empty : data.EnfHourDesc;
                tranData.LicensePlate = (data.PlateNumber == null) ? string.Empty : data.PlateNumber;
                tranData.State = (data.LPLocation == null) ? string.Empty : data.LPLocation;
                tranData.Make = (data.VMake == null) ? string.Empty : data.VMake;
                tranData.Model = (data.VModel == null) ? string.Empty : data.VModel;
                tranData.IsOccupied = Utility.getStatus((data.SpaceStatus == null) ? string.Empty : data.SpaceStatus).ToString();
                tranData.EnforcementKey = (data.EnforcementKey == null) ? string.Empty : data.EnforcementKey;
                tranData.MeterStatus = (data.MeterStatus == null) ? null : data.MeterStatus;

                tranData.METERLOC_METERTYPE = (data.METERLOC_METERTYPE == null) ? null : data.METERLOC_METERTYPE;
                tranData.ZoneName = (data.ZoneName == null) ? null : data.ZoneName;
                tranData.MeterID = String.Empty;
                tranData.ReturnCode = data.ReturnCode;

                logData.Response = new ResponseEntity() { CustomerId = param.CustomerId.Value.ToString(), MeterId = tranData.MeterName, SpaceId = tranData.SpaceNo, CityCurrentTime = data.PresentMeterTime.HasValue ? data.PresentMeterTime.Value : DateTime.Now };
            }
            catch (Exception ex)
            {
                //var appPath = System.Web.Hosting.HostingEnvironment.ApplicationPhysicalPath;
                //string errorPath = Path.Combine(appPath, "GetTransformData", "Timezones");
                //ExceptionLogging.SendErrorToText(ex, "GetCitationData", errorPath);
                //tranData.ReturnCode = ((int)ReturnCodeEnum.Error).ToString();

                tranData.ReturnCode = ((int)ReturnCodeEnum.Error).ToString();
            }
            #endregion
            #region "Parkmobile Service Call"
            /////////////////////////////////////////////////
            TransactionTransform tranDataOfServiceParkMobile = new TransactionTransform();
            try
            {
                //////////////////////////////////////////
                logData.Request.CustomerId = param.CustomerId.Value.ToString();
                logData.Request.SpaceId = param.parkingSpaceID;
                logData.Request.MeterId = param.MeterName;
                //////////////////////////////////////////////////
                DateTime? currentCustTime = customerCurrentTime.CurrentTime;
                PMCustomerElement customer = ParkmobileCustomerRetriever.GetCustomer(param.CustomerId.Value);
                ParkMobileGenericFactory factory = new ParkMobileGenericFactory(customer, param.PlateNumber);
                var parkMobileResLog = new VendorResponseEntity();
                logData.VendorData.Add(VendorNames.ParkMobile, parkMobileResLog);
                tranDataOfServiceParkMobile = factory.GetRefreshSpaceData(ParkingMobileDataType.Plate, ref parkMobileResLog);
                tranDataOfServiceParkMobile.EnforcementKey = EnforcementKey;

                ///////////////////////////////////////////////////
                logData.Response = new ResponseEntity() { CustomerId = param.CustomerId.Value.ToString(), MeterId = tranData.MeterName, SpaceId = tranData.SpaceNo, CityCurrentTime = currentCustTime.HasValue ? currentCustTime.Value : DateTime.Now };
            }
            catch (Exception ex)
            {
                logData.Response = new ResponseEntity() { CustomerId = param.CustomerId.Value.ToString(), MeterId = tranData.MeterName, SpaceId = tranData.SpaceNo, CityCurrentTime = customerCurrentTime.CurrentTime.HasValue ? customerCurrentTime.CurrentTime.Value : DateTime.Now };
                //throw ex;
                tranDataOfServiceParkMobile.ReturnCode = ((int)ReturnCodeEnum.Error).ToString();
            }
            finally
            {

            }
            #endregion
            //////////////////  //Reconcile data

            List<TransactionTransform> trans = new List<TransactionTransform>();
            trans.Add(tranDataOfServiceParkMobile); // Parkmobile
            trans.Add(tranData); // Database

            var rightValue = trans.Where(t => t.ReturnCode == ((int)ReturnCodeEnum.Success).ToString()).OrderByDescending(t => t.MeterExpiredMinutes.ToNullableInt()).FirstOrDefault<TransactionTransform>();
            if (rightValue == default(TransactionTransform))
            {
                //Do nothing.just return database  object.
            }
            else
            {
                tranData = rightValue;
            }

            return tranData;
        }

        #endregion


        #region SanibelX3
        public static TransactionTransform GetSanibelX3TransformData(string EnforcementKey, string filePath, RequestResponseEntity logData, CustomerTime customerCurrentTime)
        {
            TransactionTransform tranData = new TransactionTransform();
            tranData.ReturnCode = ((int)ReturnCodeEnum.InvalidInputParameters).ToString();
            TimeZoneInfo TZ = null;
            TZ = TimeZoneInfo.FindSystemTimeZoneById("Eastern Standard Time");
            //////////////////////////////////////
            KeyParameter param = PrepareKeyParameters.PrepareGenericPayByPlate(EnforcementKey);
            if (param.ReturnCode == ((int)ReturnCodeEnum.InvalidInputParameters).ToString())
            {
                return tranData;
            }
            /////////////////////////////////////
            #region "DB Check"
            try
            {
                Transaction data = TransactionDataValidatioin.GetDataGenericPayByPlate(param.CustomerId.Value, param.MeterName, param.parkingSpaceID, param.PlateNumber, param.StateName, EnforcementKey, filePath, logData);


                if (data.TimeZoneId.HasValue)
                    TZ = Utility.GetTimeZoneInfo(data.TimeZoneId.Value, filePath, param.CustomerId.Value.ToString());

                //For South Miami : PayStationId. Others it is MeterID
                tranData.MeterName = string.Empty; // (data.MeterName == null) ? string.Empty : data.MeterName;
                tranData.SpaceNo = string.Empty;  //data.ParkingSpaceID.ToString();

                tranData.MeterStreet = (data.MeterStreet == null) ? string.Empty : data.MeterStreet;

                DateTime _MeterRTC = data.PresentMeterTime.HasValue ? data.PresentMeterTime.Value : DateTime.Now;
                tranData.MeterRTC = String.Format("{0:yyyyMMdd_THHmmssZ}", Utility.findGMTTime(_MeterRTC, TZ));

                DateTime _SensorEventTime = data.SensorEventTime.HasValue ? data.SensorEventTime.Value : DateTime.Now;
                tranData.MeterLastUpdateTime = String.Format("{0:yyyyMMdd_THHmmssZ}", Utility.findGMTTime(_SensorEventTime, TZ));

                tranData.SensorEventTime = String.Format("{0:yyyyMMdd_THHmmssZ}", Utility.findGMTTime(_SensorEventTime, TZ));

                DateTime _endTime = data.ExpiryTime.HasValue ? data.ExpiryTime.Value : DateTime.Now;
                tranData.ExpiredTime = String.Format("{0:yyyyMMdd_THHmmssZ}", Utility.findGMTTime(_endTime, TZ));

                tranData.MeterExpiredMinutes = data.ExpiredMinutes.HasValue ? data.ExpiredMinutes.Value.ToString() : "0";
                tranData.METERLOC_BLOCKNUMBER = (data.Block == null) ? string.Empty : data.Block;
                tranData.METERLOC_STREETDIRECTION = (data.Direction == null) ? string.Empty : data.Direction;
                tranData.METERLOC_STREETNAME = (data.MeterStreet == null) ? string.Empty : data.MeterStreet;
                tranData.METERLOC_STREETTYPE = (data.StreetType == null) ? string.Empty : data.StreetType;
                tranData.METERLOC_CROSSSTREET1 = (data.CrossStreet1 == null) ? string.Empty : data.CrossStreet1;
                tranData.METERLOC_CROSSSTREET2 = (data.CrossStreet2 == null) ? string.Empty : data.CrossStreet2;
                tranData.METERLOC_ENFORCEMENTHOURSDESC = (data.EnfHourDesc == null) ? string.Empty : data.EnfHourDesc;
                tranData.LicensePlate = (data.PlateNumber == null) ? string.Empty : data.PlateNumber;
                tranData.State = (data.LPLocation == null) ? string.Empty : data.LPLocation;
                tranData.Make = (data.VMake == null) ? string.Empty : data.VMake;
                tranData.Model = (data.VModel == null) ? string.Empty : data.VModel;
                tranData.IsOccupied = Utility.getStatus((data.SpaceStatus == null) ? string.Empty : data.SpaceStatus).ToString();
                tranData.EnforcementKey = (data.EnforcementKey == null) ? string.Empty : data.EnforcementKey;
                tranData.MeterStatus = (data.MeterStatus == null) ? null : data.MeterStatus;

                tranData.METERLOC_METERTYPE = (data.METERLOC_METERTYPE == null) ? null : data.METERLOC_METERTYPE;
                tranData.ZoneName = (data.ZoneName == null) ? null : data.ZoneName;
                tranData.MeterID = String.Empty;
                tranData.ReturnCode = data.ReturnCode;

                logData.Response = new ResponseEntity() { CustomerId = param.CustomerId.Value.ToString(), MeterId = tranData.MeterName, SpaceId = tranData.SpaceNo, CityCurrentTime = data.PresentMeterTime.HasValue ? data.PresentMeterTime.Value : DateTime.Now };
            }
            catch (Exception ex)
            {
                //var appPath = System.Web.Hosting.HostingEnvironment.ApplicationPhysicalPath;
                //string errorPath = Path.Combine(appPath, "GetTransformData", "Timezones");
                //ExceptionLogging.SendErrorToText(ex, "GetCitationData", errorPath);
                //tranData.ReturnCode = ((int)ReturnCodeEnum.Error).ToString();

                tranData.ReturnCode = ((int)ReturnCodeEnum.Error).ToString();
            }
            #endregion
            #region T2Digital
            TransactionTransform tranDataOfServiceT2 = new TransactionTransform();
            try
            {
                //////////////////////////////////////////
                logData.Request.CustomerId = param.CustomerId.Value.ToString();
                logData.Request.SpaceId = param.parkingSpaceID;
                logData.Request.MeterId = param.MeterName;
                //////////////////////////////////////////////////
                DateTime? currentCustTime = customerCurrentTime.CurrentTime;
                T2Digital.T2DigitalCustomerElement customer = T2Digital.T2DigitalCustomerRetriever.GetCustomer(param.CustomerId.Value);
                T2Digital.T2DigitalGenericFactory factory = new T2Digital.T2DigitalGenericFactory(customer, param.PlateNumber, string.Empty);
                var t2ResLog = new VendorResponseEntity();
                logData.VendorData.Add(VendorNames.T2Digital, t2ResLog);
                tranDataOfServiceT2 = factory.GetRefreshPlateData(ref t2ResLog);
                tranDataOfServiceT2.EnforcementKey = EnforcementKey;

                ///////////////////////////////////////////////////
                logData.Response = new ResponseEntity() { CustomerId = param.CustomerId.Value.ToString(), MeterId = tranData.MeterName, SpaceId = tranData.SpaceNo, CityCurrentTime = currentCustTime.HasValue ? currentCustTime.Value : DateTime.Now };

            }
            catch (Exception ex)
            {
                logData.Response = new ResponseEntity() { CustomerId = param.CustomerId.Value.ToString(), MeterId = tranData.MeterName, SpaceId = tranData.SpaceNo, CityCurrentTime = customerCurrentTime.CurrentTime.HasValue ? customerCurrentTime.CurrentTime.Value : DateTime.Now };
                //throw ex;
                tranDataOfServiceT2.ReturnCode = ((int)ReturnCodeEnum.Error).ToString();
            }
            finally
            {

            }
            #endregion
            //////////////////  //Reconcile data
            List<TransactionTransform> trans = new List<TransactionTransform>();
            trans.Add(tranData); // Database
            trans.Add(tranDataOfServiceT2); //T2Digitial


            var rightValue = trans.Where(t => t.ReturnCode == ((int)ReturnCodeEnum.Success).ToString()).OrderByDescending(t => t.MeterExpiredMinutes.ToNullableInt()).FirstOrDefault<TransactionTransform>();
            if (rightValue == default(TransactionTransform))
            {
                //Do nothing.just return database  object.
            }
            else
            {
                tranData = rightValue;
            }

            return tranData;
        }
        #endregion

        #region Doral
        public static TransactionTransform GetDoral(string EnforcementKey, string filePath, RequestResponseEntity logData, CustomerTime customerCurrentTime)
        {
            TransactionTransform tranData = new TransactionTransform();
            tranData.ReturnCode = ((int)ReturnCodeEnum.InvalidInputParameters).ToString();
            TimeZoneInfo TZ = null;
            TZ = TimeZoneInfo.FindSystemTimeZoneById("Eastern Standard Time");
            //int? customerID = null;
            //string MeterName = null;
            //string parkingSpaceID;
            //string PlateNumber = null;
            //string stateName = null;
            //////////////////////////////////////
            KeyParameter param = PrepareKeyParameters.PrepareGenericPayByPlate(EnforcementKey);
            if (param.ReturnCode == ((int)ReturnCodeEnum.InvalidInputParameters).ToString())
            {
                return tranData;
            }

            #region "DB Check"
            /////////////////////////////////////
            try
            {
                Transaction data = TransactionDataValidatioin.GetDataBayHarbor(param.CustomerId.Value, param.MeterName, param.parkingSpaceID, param.PlateNumber, param.StateName, EnforcementKey, filePath, logData);


                if (data.TimeZoneId.HasValue)
                    TZ = Utility.GetTimeZoneInfo(data.TimeZoneId.Value, filePath, param.CustomerId.Value.ToString());

                //For South Miami : PayStationId. Others it is MeterID
                tranData.MeterName = string.Empty; // (data.MeterName == null) ? string.Empty : data.MeterName;
                tranData.SpaceNo = string.Empty;  //data.ParkingSpaceID.ToString();

                tranData.MeterStreet = (data.MeterStreet == null) ? string.Empty : data.MeterStreet;

                DateTime _MeterRTC = data.PresentMeterTime.HasValue ? data.PresentMeterTime.Value : DateTime.Now;
                tranData.MeterRTC = String.Format("{0:yyyyMMdd_THHmmssZ}", Utility.findGMTTime(_MeterRTC, TZ));

                DateTime _SensorEventTime = data.SensorEventTime.HasValue ? data.SensorEventTime.Value : DateTime.Now;
                tranData.MeterLastUpdateTime = String.Format("{0:yyyyMMdd_THHmmssZ}", Utility.findGMTTime(_SensorEventTime, TZ));

                tranData.SensorEventTime = String.Format("{0:yyyyMMdd_THHmmssZ}", Utility.findGMTTime(_SensorEventTime, TZ));

                DateTime _endTime = data.ExpiryTime.HasValue ? data.ExpiryTime.Value : DateTime.Now;
                tranData.ExpiredTime = String.Format("{0:yyyyMMdd_THHmmssZ}", Utility.findGMTTime(_endTime, TZ));

                tranData.MeterExpiredMinutes = data.ExpiredMinutes.HasValue ? data.ExpiredMinutes.Value.ToString() : "0";
                tranData.METERLOC_BLOCKNUMBER = (data.Block == null) ? string.Empty : data.Block;
                tranData.METERLOC_STREETDIRECTION = (data.Direction == null) ? string.Empty : data.Direction;
                tranData.METERLOC_STREETNAME = (data.MeterStreet == null) ? string.Empty : data.MeterStreet;
                tranData.METERLOC_STREETTYPE = (data.StreetType == null) ? string.Empty : data.StreetType;
                tranData.METERLOC_CROSSSTREET1 = (data.CrossStreet1 == null) ? string.Empty : data.CrossStreet1;
                tranData.METERLOC_CROSSSTREET2 = (data.CrossStreet2 == null) ? string.Empty : data.CrossStreet2;
                tranData.METERLOC_ENFORCEMENTHOURSDESC = (data.EnfHourDesc == null) ? string.Empty : data.EnfHourDesc;
                tranData.LicensePlate = (data.PlateNumber == null) ? string.Empty : data.PlateNumber;
                tranData.State = (data.LPLocation == null) ? string.Empty : data.LPLocation;
                tranData.Make = (data.VMake == null) ? string.Empty : data.VMake;
                tranData.Model = (data.VModel == null) ? string.Empty : data.VModel;
                tranData.IsOccupied = Utility.getStatus((data.SpaceStatus == null) ? string.Empty : data.SpaceStatus).ToString();
                tranData.EnforcementKey = (data.EnforcementKey == null) ? string.Empty : data.EnforcementKey;
                tranData.MeterStatus = (data.MeterStatus == null) ? null : data.MeterStatus;

                tranData.METERLOC_METERTYPE = (data.METERLOC_METERTYPE == null) ? null : data.METERLOC_METERTYPE;
                tranData.ZoneName = (data.ZoneName == null) ? null : data.ZoneName;
                tranData.MeterID = String.Empty;
                tranData.ReturnCode = data.ReturnCode;

                logData.Response = new ResponseEntity() { CustomerId = param.CustomerId.Value.ToString(), MeterId = tranData.MeterName, SpaceId = tranData.SpaceNo, CityCurrentTime = data.PresentMeterTime.HasValue ? data.PresentMeterTime.Value : DateTime.Now };
            }
            catch (Exception ex)
            {
                ////Intentionally Error Condition Commented
                ////var appPath = System.Web.Hosting.HostingEnvironment.ApplicationPhysicalPath;
                ////string errorPath = Path.Combine(appPath, "GetTransformData", "Timezones");
                ////ExceptionLogging.SendErrorToText(ex, "GetCitationData", errorPath);
                ////tranData.ReturnCode = ((int)ReturnCodeEnum.Error).ToString();

                tranData.ReturnCode = ((int)ReturnCodeEnum.Error).ToString();
            }

            #endregion
            #region "PayByPhone Service Call"
            /////////////////////////////////////////////////
            TransactionTransform tranDataOfPBPService = new TransactionTransform();
            try
            {
                //////////////////////////////////////////

                logData.Request.CustomerId = param.CustomerId.Value.ToString();
                logData.Request.SpaceId = param.parkingSpaceID;
                logData.Request.MeterId = param.MeterName;
                //////////////////////////////////////////////////
                DateTime? currentCustTime = customerCurrentTime.CurrentTime;
                CustomerElement customer = CustomerRetriever.GetCustomer(param.CustomerId.Value);
                PayByPhoneGenericFactory factory = new PayByPhoneGenericFactory(customer, param.PlateNumber);
                var paybyPhoneResLog = new VendorResponseEntity();
                logData.VendorData.Add(VendorNames.PayByPhone, paybyPhoneResLog);
                tranDataOfPBPService = factory.GetRefreshPlateData(ref paybyPhoneResLog);
                tranDataOfPBPService.EnforcementKey = EnforcementKey;

                ///////////////////////////////////////////////////
                logData.Response = new ResponseEntity() { CustomerId = param.CustomerId.Value.ToString(), MeterId = tranData.MeterName, SpaceId = tranData.SpaceNo, CityCurrentTime = currentCustTime.HasValue ? currentCustTime.Value : DateTime.Now };

            }
            catch (Exception ex)
            {
                logData.Response = new ResponseEntity() { CustomerId = param.CustomerId.Value.ToString(), MeterId = tranData.MeterName, SpaceId = tranData.SpaceNo, CityCurrentTime = customerCurrentTime.CurrentTime.HasValue ? customerCurrentTime.CurrentTime.Value : DateTime.Now };
                //throw ex;
                tranDataOfPBPService.ReturnCode = ((int)ReturnCodeEnum.Error).ToString();
            }
            finally
            {

            }
            #endregion
            //////////////////  //Reconcile data
            List<TransactionTransform> trans = new List<TransactionTransform>();
            trans.Add(tranData); // Database
            trans.Add(tranDataOfPBPService); //PayByCell

            var rightValue = trans.Where(t => t.ReturnCode == ((int)ReturnCodeEnum.Success).ToString()).OrderByDescending(t => t.MeterExpiredMinutes.ToNullableInt()).FirstOrDefault<TransactionTransform>();
            if (rightValue == default(TransactionTransform))
            {
                //Do nothing.just return database  object.
            }
            else
            {
                tranData = rightValue;
            }

            return tranData;
        }
        #endregion

        #region New Smyrna Beach FL
        public static TransactionTransform GetNewSmyrnaBeachFL(string EnforcementKey, string filePath, RequestResponseEntity logData, CustomerTime customerCurrentTime)
        {
            TransactionTransform tranData = new TransactionTransform();
            tranData.ReturnCode = ((int)ReturnCodeEnum.InvalidInputParameters).ToString();
            TimeZoneInfo TZ = null;
            TZ = TimeZoneInfo.FindSystemTimeZoneById("Eastern Standard Time");
            //int? customerID = null;
            //string MeterName = null;
            //string parkingSpaceID;
            //string PlateNumber = null;
            //string stateName = null;
            //////////////////////////////////////
            KeyParameter param = PrepareKeyParameters.PrepareGenericPayByPlate(EnforcementKey);
            if (param.ReturnCode == ((int)ReturnCodeEnum.InvalidInputParameters).ToString())
            {
                return tranData;
            }

            #region "DB Check"
            /////////////////////////////////////
            try
            {
                Transaction data = TransactionDataValidatioin.GetDataBayHarbor(param.CustomerId.Value, param.MeterName, param.parkingSpaceID, param.PlateNumber, param.StateName, EnforcementKey, filePath, logData);


                if (data.TimeZoneId.HasValue)
                    TZ = Utility.GetTimeZoneInfo(data.TimeZoneId.Value, filePath, param.CustomerId.Value.ToString());

                //For South Miami : PayStationId. Others it is MeterID
                tranData.MeterName = string.Empty; // (data.MeterName == null) ? string.Empty : data.MeterName;
                tranData.SpaceNo = string.Empty;  //data.ParkingSpaceID.ToString();

                tranData.MeterStreet = (data.MeterStreet == null) ? string.Empty : data.MeterStreet;

                DateTime _MeterRTC = data.PresentMeterTime.HasValue ? data.PresentMeterTime.Value : DateTime.Now;
                tranData.MeterRTC = String.Format("{0:yyyyMMdd_THHmmssZ}", Utility.findGMTTime(_MeterRTC, TZ));

                DateTime _SensorEventTime = data.SensorEventTime.HasValue ? data.SensorEventTime.Value : DateTime.Now;
                tranData.MeterLastUpdateTime = String.Format("{0:yyyyMMdd_THHmmssZ}", Utility.findGMTTime(_SensorEventTime, TZ));

                tranData.SensorEventTime = String.Format("{0:yyyyMMdd_THHmmssZ}", Utility.findGMTTime(_SensorEventTime, TZ));

                DateTime _endTime = data.ExpiryTime.HasValue ? data.ExpiryTime.Value : DateTime.Now;
                tranData.ExpiredTime = String.Format("{0:yyyyMMdd_THHmmssZ}", Utility.findGMTTime(_endTime, TZ));

                tranData.MeterExpiredMinutes = data.ExpiredMinutes.HasValue ? data.ExpiredMinutes.Value.ToString() : "0";
                tranData.METERLOC_BLOCKNUMBER = (data.Block == null) ? string.Empty : data.Block;
                tranData.METERLOC_STREETDIRECTION = (data.Direction == null) ? string.Empty : data.Direction;
                tranData.METERLOC_STREETNAME = (data.MeterStreet == null) ? string.Empty : data.MeterStreet;
                tranData.METERLOC_STREETTYPE = (data.StreetType == null) ? string.Empty : data.StreetType;
                tranData.METERLOC_CROSSSTREET1 = (data.CrossStreet1 == null) ? string.Empty : data.CrossStreet1;
                tranData.METERLOC_CROSSSTREET2 = (data.CrossStreet2 == null) ? string.Empty : data.CrossStreet2;
                tranData.METERLOC_ENFORCEMENTHOURSDESC = (data.EnfHourDesc == null) ? string.Empty : data.EnfHourDesc;
                tranData.LicensePlate = (data.PlateNumber == null) ? string.Empty : data.PlateNumber;
                tranData.State = (data.LPLocation == null) ? string.Empty : data.LPLocation;
                tranData.Make = (data.VMake == null) ? string.Empty : data.VMake;
                tranData.Model = (data.VModel == null) ? string.Empty : data.VModel;
                tranData.IsOccupied = Utility.getStatus((data.SpaceStatus == null) ? string.Empty : data.SpaceStatus).ToString();
                tranData.EnforcementKey = (data.EnforcementKey == null) ? string.Empty : data.EnforcementKey;
                tranData.MeterStatus = (data.MeterStatus == null) ? null : data.MeterStatus;

                tranData.METERLOC_METERTYPE = (data.METERLOC_METERTYPE == null) ? null : data.METERLOC_METERTYPE;
                tranData.ZoneName = (data.ZoneName == null) ? null : data.ZoneName;
                tranData.MeterID = String.Empty;
                tranData.ReturnCode = data.ReturnCode;

                logData.Response = new ResponseEntity() { CustomerId = param.CustomerId.Value.ToString(), MeterId = tranData.MeterName, SpaceId = tranData.SpaceNo, CityCurrentTime = data.PresentMeterTime.HasValue ? data.PresentMeterTime.Value : DateTime.Now };
            }
            catch (Exception ex)
            {
                ////Intentionally Error Condition Commented
                ////var appPath = System.Web.Hosting.HostingEnvironment.ApplicationPhysicalPath;
                ////string errorPath = Path.Combine(appPath, "GetTransformData", "Timezones");
                ////ExceptionLogging.SendErrorToText(ex, "GetCitationData", errorPath);
                ////tranData.ReturnCode = ((int)ReturnCodeEnum.Error).ToString();

                tranData.ReturnCode = ((int)ReturnCodeEnum.Error).ToString();
            }

            #endregion
            #region T2Digital
            TransactionTransform tranDataOfServiceT2 = new TransactionTransform();
            try
            {
                //////////////////////////////////////////
                logData.Request.CustomerId = param.CustomerId.Value.ToString();
                logData.Request.SpaceId = param.parkingSpaceID;
                logData.Request.MeterId = param.MeterName;
                //////////////////////////////////////////////////
                DateTime? currentCustTime = customerCurrentTime.CurrentTime;
                T2Digital.T2DigitalCustomerElement customer = T2Digital.T2DigitalCustomerRetriever.GetCustomer(param.CustomerId.Value);
                T2Digital.T2DigitalGenericFactory factory = new T2Digital.T2DigitalGenericFactory(customer, param.PlateNumber, string.Empty);
                var t2ResLog = new VendorResponseEntity();
                logData.VendorData.Add(VendorNames.T2Digital, t2ResLog);
                tranDataOfServiceT2 = factory.GetRefreshPlateData(ref t2ResLog);
                tranDataOfServiceT2.EnforcementKey = EnforcementKey;

                ///////////////////////////////////////////////////
                logData.Response = new ResponseEntity() { CustomerId = param.CustomerId.Value.ToString(), MeterId = tranData.MeterName, SpaceId = tranData.SpaceNo, CityCurrentTime = currentCustTime.HasValue ? currentCustTime.Value : DateTime.Now };

            }
            catch (Exception ex)
            {
                logData.Response = new ResponseEntity() { CustomerId = param.CustomerId.Value.ToString(), MeterId = tranData.MeterName, SpaceId = tranData.SpaceNo, CityCurrentTime = customerCurrentTime.CurrentTime.HasValue ? customerCurrentTime.CurrentTime.Value : DateTime.Now };
                //throw ex;
                tranDataOfServiceT2.ReturnCode = ((int)ReturnCodeEnum.Error).ToString();
            }
            finally
            {

            }
            #endregion
            //////////////////  //Reconcile data
            List<TransactionTransform> trans = new List<TransactionTransform>();
            trans.Add(tranData); // Database
            trans.Add(tranDataOfServiceT2); //PayByCell

            var rightValue = trans.Where(t => t.ReturnCode == ((int)ReturnCodeEnum.Success).ToString()).OrderByDescending(t => t.MeterExpiredMinutes.ToNullableInt()).FirstOrDefault<TransactionTransform>();
            if (rightValue == default(TransactionTransform))
            {
                //Do nothing.just return database  object.
            }
            else
            {
                tranData = rightValue;
            }

            return tranData;
        }
        #endregion


        #region SanDiego
        public static TransactionTransform GetSanDiego(string EnforcementKey, string filePath, RequestResponseEntity logData, CustomerTime customerCurrentTime)
        {
            TransactionTransform tranData = new TransactionTransform();
            tranData.ReturnCode = ((int)ReturnCodeEnum.InvalidInputParameters).ToString();
            TimeZoneInfo TZ = null;
            TZ = TimeZoneInfo.FindSystemTimeZoneById("Eastern Standard Time");
            //int? customerID = null;
            //string MeterName = null;
            //string parkingSpaceID;
            //string PlateNumber = null;
            //string stateName = null;
            //////////////////////////////////////
            KeyParameter param = PrepareKeyParameters.PrepareGenericPayByPlate(EnforcementKey);
            if (param.ReturnCode == ((int)ReturnCodeEnum.InvalidInputParameters).ToString())
            {
                return tranData;
            }

            #region "DB Check"
            /////////////////////////////////////
            try
            {
                Transaction data = TransactionDataValidatioin.GetDataBayHarbor(param.CustomerId.Value, param.MeterName, param.parkingSpaceID, param.PlateNumber, param.StateName, EnforcementKey, filePath, logData);


                if (data.TimeZoneId.HasValue)
                    TZ = Utility.GetTimeZoneInfo(data.TimeZoneId.Value, filePath, param.CustomerId.Value.ToString());

                //For South Miami : PayStationId. Others it is MeterID
                tranData.MeterName = string.Empty; // (data.MeterName == null) ? string.Empty : data.MeterName;
                tranData.SpaceNo = string.Empty;  //data.ParkingSpaceID.ToString();

                tranData.MeterStreet = (data.MeterStreet == null) ? string.Empty : data.MeterStreet;

                DateTime _MeterRTC = data.PresentMeterTime.HasValue ? data.PresentMeterTime.Value : DateTime.Now;
                tranData.MeterRTC = String.Format("{0:yyyyMMdd_THHmmssZ}", Utility.findGMTTime(_MeterRTC, TZ));

                DateTime _SensorEventTime = data.SensorEventTime.HasValue ? data.SensorEventTime.Value : DateTime.Now;
                tranData.MeterLastUpdateTime = String.Format("{0:yyyyMMdd_THHmmssZ}", Utility.findGMTTime(_SensorEventTime, TZ));

                tranData.SensorEventTime = String.Format("{0:yyyyMMdd_THHmmssZ}", Utility.findGMTTime(_SensorEventTime, TZ));

                DateTime _endTime = data.ExpiryTime.HasValue ? data.ExpiryTime.Value : DateTime.Now;
                tranData.ExpiredTime = String.Format("{0:yyyyMMdd_THHmmssZ}", Utility.findGMTTime(_endTime, TZ));

                tranData.MeterExpiredMinutes = data.ExpiredMinutes.HasValue ? data.ExpiredMinutes.Value.ToString() : "0";
                tranData.METERLOC_BLOCKNUMBER = (data.Block == null) ? string.Empty : data.Block;
                tranData.METERLOC_STREETDIRECTION = (data.Direction == null) ? string.Empty : data.Direction;
                tranData.METERLOC_STREETNAME = (data.MeterStreet == null) ? string.Empty : data.MeterStreet;
                tranData.METERLOC_STREETTYPE = (data.StreetType == null) ? string.Empty : data.StreetType;
                tranData.METERLOC_CROSSSTREET1 = (data.CrossStreet1 == null) ? string.Empty : data.CrossStreet1;
                tranData.METERLOC_CROSSSTREET2 = (data.CrossStreet2 == null) ? string.Empty : data.CrossStreet2;
                tranData.METERLOC_ENFORCEMENTHOURSDESC = (data.EnfHourDesc == null) ? string.Empty : data.EnfHourDesc;
                tranData.LicensePlate = (data.PlateNumber == null) ? string.Empty : data.PlateNumber;
                tranData.State = (data.LPLocation == null) ? string.Empty : data.LPLocation;
                tranData.Make = (data.VMake == null) ? string.Empty : data.VMake;
                tranData.Model = (data.VModel == null) ? string.Empty : data.VModel;
                tranData.IsOccupied = Utility.getStatus((data.SpaceStatus == null) ? string.Empty : data.SpaceStatus).ToString();
                tranData.EnforcementKey = (data.EnforcementKey == null) ? string.Empty : data.EnforcementKey;
                tranData.MeterStatus = (data.MeterStatus == null) ? null : data.MeterStatus;

                tranData.METERLOC_METERTYPE = (data.METERLOC_METERTYPE == null) ? null : data.METERLOC_METERTYPE;
                tranData.ZoneName = (data.ZoneName == null) ? null : data.ZoneName;
                tranData.MeterID = String.Empty;
                tranData.ReturnCode = data.ReturnCode;

                logData.Response = new ResponseEntity() { CustomerId = param.CustomerId.Value.ToString(), MeterId = tranData.MeterName, SpaceId = tranData.SpaceNo, CityCurrentTime = data.PresentMeterTime.HasValue ? data.PresentMeterTime.Value : DateTime.Now };
            }
            catch (Exception ex)
            {
                ////Intentionally Error Condition Commented
                ////var appPath = System.Web.Hosting.HostingEnvironment.ApplicationPhysicalPath;
                ////string errorPath = Path.Combine(appPath, "GetTransformData", "Timezones");
                ////ExceptionLogging.SendErrorToText(ex, "GetCitationData", errorPath);
                ////tranData.ReturnCode = ((int)ReturnCodeEnum.Error).ToString();

                tranData.ReturnCode = ((int)ReturnCodeEnum.Error).ToString();
            }

            #endregion

            #region IPC
            TransactionTransform tranDataOfServiceIPC = new TransactionTransform();
            try
            {
                //////////////////////////////////////////
                logData.Request.CustomerId = param.CustomerId.Value.ToString();
                logData.Request.SpaceId = param.parkingSpaceID;
                logData.Request.MeterId = param.MeterName;
                //////////////////////////////////////////////////
                DateTime? currentCustTime = customerCurrentTime.CurrentTime;
                IPC.IPCCustomerElement customer = IPCCustomerRetriever.GetCustomer(param.CustomerId.Value);
                IPC.IPCGenericFactory factory = new IPC.IPCGenericFactory(customer, param.PlateNumber, string.Empty,  customerCurrentTime);
                var ipcResLog = new VendorResponseEntity();
                logData.VendorData.Add(VendorNames.IPC, ipcResLog);
                tranDataOfServiceIPC = factory.GetRefreshPlateData(ref ipcResLog);
                tranDataOfServiceIPC.EnforcementKey = EnforcementKey;

                ///////////////////////////////////////////////////
                logData.Response = new ResponseEntity() { CustomerId = param.CustomerId.Value.ToString(), MeterId = tranData.MeterName, SpaceId = tranData.SpaceNo, CityCurrentTime = currentCustTime.HasValue ? currentCustTime.Value : DateTime.Now };

            }
            catch (Exception ex)
            {
                logData.Response = new ResponseEntity() { CustomerId = param.CustomerId.Value.ToString(), MeterId = tranData.MeterName, SpaceId = tranData.SpaceNo, CityCurrentTime = customerCurrentTime.CurrentTime.HasValue ? customerCurrentTime.CurrentTime.Value : DateTime.Now };
                //throw ex;
                tranDataOfServiceIPC.ReturnCode = ((int)ReturnCodeEnum.Error).ToString();
            }
            finally
            {

            }
            #endregion

            //////////////////  //Reconcile data
            List<TransactionTransform> trans = new List<TransactionTransform>();
            trans.Add(tranData); // Database
            trans.Add(tranDataOfServiceIPC); // IPC

            var rightValue = trans.Where(t => t.ReturnCode == ((int)ReturnCodeEnum.Success).ToString()).OrderByDescending(t => t.MeterExpiredMinutes.ToNullableInt()).FirstOrDefault<TransactionTransform>();
            if (rightValue == default(TransactionTransform))
            {
                //Do nothing.just return database  object.
            }
            else
            {
                tranData = rightValue;
            }

            return tranData;
        }
        #endregion

        #region SalemOR
        public static TransactionTransform GetSalemOR(string EnforcementKey, string filePath, RequestResponseEntity logData, CustomerTime customerCurrentTime)
        {
            TransactionTransform tranData = new TransactionTransform();
            tranData.ReturnCode = ((int)ReturnCodeEnum.InvalidInputParameters).ToString();
            TimeZoneInfo TZ = null;
            TZ = TimeZoneInfo.FindSystemTimeZoneById("Eastern Standard Time");
            KeyParameter param = PrepareKeyParameters.PrepareGenericPayByPlate3(EnforcementKey);
            if (param.ReturnCode == ((int)ReturnCodeEnum.InvalidInputParameters).ToString())
            {
                return tranData;
            }
            /////////////////////////////////////
            #region "Cale Service Call"
            /////////////////////////////////////////////////
            TransactionTransform tranDataOfService = new TransactionTransform();
            try
            {
                //////////////////////////////////////////
                logData.Request.CustomerId = param.CustomerId.Value.ToString();
                logData.Request.SpaceId = param.parkingSpaceID;
                logData.Request.MeterId = param.MeterName;
                //////////////////////////////////////////////////
                DateTime? currentCustTime = customerCurrentTime.CurrentTime;
                Cale.CaleCustomerElement customer = Cale.CaleCustomerRetriever.GetCustomer(param.CustomerId.Value);
                Cale.CaleGenericFactory factory = new Cale.CaleGenericFactory(customer, param.PlateNumber, null);
                var caleResLog = new VendorResponseEntity();
                logData.VendorData.Add(VendorNames.Cale, caleResLog);
                if (customer.EnforcementType == EnumEnforcementType.PayByPlate.ToString())
                    tranDataOfService = factory.GetRefreshPlateDataArticle(ref caleResLog);
                //tranDataOfService = factory.GetRefreshPlateData();
                tranDataOfService.EnforcementKey = EnforcementKey;

                ///////////////////////////////////////////////////
                logData.Response = new ResponseEntity() { CustomerId = param.CustomerId.Value.ToString(), MeterId = tranData.MeterName, SpaceId = tranData.SpaceNo, CityCurrentTime = currentCustTime.HasValue ? currentCustTime.Value : DateTime.Now };
            }
            catch (Exception ex)
            {
                logData.Response = new ResponseEntity() { CustomerId = param.CustomerId.Value.ToString(), MeterId = tranData.MeterName, SpaceId = tranData.SpaceNo, CityCurrentTime = customerCurrentTime.CurrentTime.HasValue ? customerCurrentTime.CurrentTime.Value : DateTime.Now };
                //throw ex;
                tranData.ReturnCode = ((int)ReturnCodeEnum.Error).ToString();
            }
            finally
            {

            }
            #endregion

            //Reconcile data
            List<TransactionTransform> trans = new List<TransactionTransform>();
            if(tranDataOfService != null && tranDataOfService.ZoneName != null)
                if(tranDataOfService.ZoneName.ToLower() == param.ZoneID.ToLower())
                    trans.Add(tranDataOfService); // Cale

            if (trans.Count == 0)
                tranData.ReturnCode = ((int)ReturnCodeEnum.NoRecordExists).ToString();
            else
                tranData = tranDataOfService;
            //var rightValue = trans.Where(t => t.ReturnCode == ((int)ReturnCodeEnum.Success).ToString()).OrderByDescending(t => t.MeterExpiredMinutes.ToNullableInt()).FirstOrDefault<TransactionTransform>();
            //if (rightValue == default(TransactionTransform))
            //{
            //    //Do nothing.just return database  object.
            //}
            //else
            //{
            //    tranData = rightValue;
            //}

            return tranData;
        }
        #endregion



        private static Transaction GetDataGenericPayByPlate(int customerID, string MeterName, string parkingSpaceID, string PlateNumber, string statename, string EnforcementKey, string filePath, RequestResponseEntity logData)
        {
            string ZoneID = null;
            string SpaceStatus = null;
            Transaction data = new Transaction();
            try
            {
                List<Transaction> transactionList = new List<Transaction>();

                logData.Request.CustomerId = customerID.ToString();
                logData.Request.SpaceId = parkingSpaceID;
                logData.Request.MeterId = MeterName;

                transactionList = DataAccess.GetGenericPlateName(customerID, null, null, null, PlateNumber, statename, null);

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
            }
            catch (Exception ex)
            {
                var appPath = System.Web.Hosting.HostingEnvironment.ApplicationPhysicalPath;
                string errorPath = Path.Combine(appPath, "GetData", "Timezones");
                ExceptionLogging.SendErrorToText(ex, "GetCitationData", errorPath);
                data.ReturnCode = ((int)ReturnCodeEnum.Error).ToString();
                throw;
            }
            return data;
        }

        private static Transaction GetDataGenericPayByPlate2(int customerID, string MeterName, string parkingSpaceID, string PlateNumber, string statename, string EnforcementKey, string filePath, RequestResponseEntity logData)
        {
            string ZoneID = null;
            string SpaceStatus = null;
            Transaction data = new Transaction();
            try
            {
                List<Transaction> transactionList = new List<Transaction>();

                logData.Request.CustomerId = customerID.ToString();
                logData.Request.SpaceId = parkingSpaceID;
                logData.Request.MeterId = MeterName;

                transactionList = DataAccess.GetGenericPlateName2(customerID, null, null, null, PlateNumber, statename, null);

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
            }
            catch (Exception ex)
            {
                var appPath = System.Web.Hosting.HostingEnvironment.ApplicationPhysicalPath;
                string errorPath = Path.Combine(appPath, "GetData", "Timezones");
                ExceptionLogging.SendErrorToText(ex, "GetCitationData", errorPath);
                data.ReturnCode = ((int)ReturnCodeEnum.Error).ToString();
                throw;
            }
            return data;
        }


        private static Transaction GetDataGenericPayByPlate3(int customerID, string MeterName, string parkingSpaceID, string PlateNumber, string statename, string EnforcementKey, string filePath, RequestResponseEntity logData)
        {
            string ZoneID = null;
            string SpaceStatus = null;
            Transaction data = new Transaction();
            try
            {
                List<Transaction> transactionList = new List<Transaction>();

                logData.Request.CustomerId = customerID.ToString();
                logData.Request.SpaceId = parkingSpaceID;
                logData.Request.MeterId = MeterName;

                transactionList = DataAccess.GetGenericPlateName3(customerID, null, null, null, PlateNumber, statename, null);

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
            }
            catch (Exception ex)
            {
                var appPath = System.Web.Hosting.HostingEnvironment.ApplicationPhysicalPath;
                string errorPath = Path.Combine(appPath, "GetData", "Timezones");
                ExceptionLogging.SendErrorToText(ex, "GetCitationData", errorPath);
                data.ReturnCode = ((int)ReturnCodeEnum.Error).ToString();
                throw;
            }
            return data;
        }

        private static Transaction GetDataGenericPayBySpace(int customerID, string MeterName, string parkingSpaceID, string PlateNumber, string statename, string EnforcementKey, string filePath, RequestResponseEntity logData)
        {
            string ZoneID = null;
            string SpaceStatus = null;
            Transaction data = new Transaction();
            try
            {
                List<Transaction> transactionList = new List<Transaction>();

                logData.Request.CustomerId = customerID.ToString();
                logData.Request.SpaceId = parkingSpaceID;
                logData.Request.MeterId = MeterName;

                transactionList = DataAccess.GetGenericBySpaceData(customerID, null, MeterName, parkingSpaceID, null, null);

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
            }
            catch (Exception ex)
            {
                var appPath = System.Web.Hosting.HostingEnvironment.ApplicationPhysicalPath;
                string errorPath = Path.Combine(appPath, "GetData", "Timezones");
                ExceptionLogging.SendErrorToText(ex, "GetCitationData", errorPath);
                data.ReturnCode = ((int)ReturnCodeEnum.Error).ToString();
                throw;
            }
            return data;
        }




        private static Transactions GetPaymentData(string customerId, string zoneId, string spaceId)
        {
            Transactions data = new Transactions();
            int? customerID = Utility.NullableInt(customerId);
            if (!customerID.HasValue)
            {
                data.ReturnCode = ((int)ReturnCodeEnum.InvalidInputParameters).ToString();
                return data;
            }

            int? zoneID = Utility.NullableInt(zoneId);
            int? spaceID = Utility.NullableInt(spaceId);

            try
            {
                data.GetTransactions = DataAccess.GetCitationData(customerID.Value,
                    zoneID.HasValue ? zoneID.Value.ToString() : null,
                    null,
                     spaceID.HasValue ? spaceID.Value.ToString() : null,
                     null, null);

                //if(zoneID.HasValue)
                //{
                //    //data.GetTransactions = DataAccess.GetCitationData(customerID.Value, zoneID.Value.ToString(), null, null, null, null);
                //    data.GetTransactions = DataAccess.GetCitationData(customerID.Value, zoneID.Value.ToString(), null, null, null, null);
                //}
                //else
                //{
                //    data.GetTransactions = DataAccess.GetCitationData(customerID.Value, null, null, null, null, null);
                //}


                if (data.GetTransactions.Count == 0)
                {
                    data.ReturnCode = ((int)ReturnCodeEnum.NoRecordExists).ToString();
                }
                else
                {
                    data.ReturnCode = ((int)ReturnCodeEnum.Success).ToString();
                }
            }
            catch (Exception ex)
            {
                var appPath = System.Web.Hosting.HostingEnvironment.ApplicationPhysicalPath;
                string errorPath = Path.Combine(appPath, "GetPaymentData", "Timezones");
                ExceptionLogging.SendErrorToText(ex, "GetPaymentTransformData", errorPath);
                data.ReturnCode = ((int)ReturnCodeEnum.Error).ToString();
                throw;
            }
            return data;
        }
        public static PaymentsData GetPaymentTransformData(string customerId, string zoneId, string spaceId, string filePath)
        {
            //TimeZoneInfo TZ = TimeZoneInfo.FindSystemTimeZoneById("Eastern Standard Time");

            TimeZoneInfo TZ = null;
            if (customerId == "4102")
            {
                TZ = TimeZoneInfo.FindSystemTimeZoneById("Central Standard Time");
                // TZ = TimeZoneInfo.FindSystemTimeZoneById("Eastern Standard Time");
            }
            else
            {
                TZ = TimeZoneInfo.FindSystemTimeZoneById("Eastern Standard Time");
            }

            PaymentsData paymentsData = new PaymentsData();
            PaymentData tranData = null;
            paymentsData.ReturnCode = ((int)ReturnCodeEnum.Success).ToString();
            try
            {
                Transactions datas = TransactionDataValidatioin.GetPaymentData(customerId, zoneId, spaceId);
                paymentsData.ReturnCode = datas.ReturnCode;

                if (datas.GetTransactions.FirstOrDefault<Transaction>() != default(Transaction))
                    TZ = Utility.GetTimeZoneInfo(datas.GetTransactions.FirstOrDefault<Transaction>().TimeZoneId.Value, filePath, customerId);

                foreach (var data in datas.GetTransactions)
                {
                    tranData = new PaymentData();
                    tranData.MeterID = data.PayStationID.HasValue ? data.PayStationID.Value.ToString() : "0";
                    tranData.MeterName = (data.MeterName == null) ? string.Empty : data.MeterName;
                    tranData.SpaceNo = data.ParkingSpaceID.ToString();
                    tranData.MeterStreet = (data.MeterStreet == null) ? string.Empty : data.MeterStreet;
                    tranData.LicensePlate = (data.PlateNumber == null) ? string.Empty : data.PlateNumber;


                    //payment date, payment amount, payment duration, expiry date.
                    // Payment date: 
                    tranData.PaymentDay = String.Format("{0:yyyyMMdd}", Utility.findGMTTime(data.SensorEventTime.HasValue ? data.SensorEventTime.Value : DateTime.Now, TZ));
                    //data.SensorEventTime.HasValue ? data.SensorEventTime.Value.ToString("yyyy/MM/dd") : DateTime.Now.ToString("yyyy/MM/dd");
                    //Expiry date
                    //DateTime _endTime = data.ExpiryTime.HasValue ? data.ExpiryTime.Value : DateTime.Now;
                    tranData.ExpiredTime = String.Format("{0:yyyyMMdd_THHmmssZ}", Utility.findGMTTime(data.ExpiryTime.HasValue ? data.ExpiryTime.Value : DateTime.Now, TZ));

                    //Payment date
                    // DateTime _SensorEventTime = data.SensorEventTime.HasValue ? data.SensorEventTime.Value : DateTime.Now;
                    tranData.PaymentTime = String.Format("{0:yyyyMMdd_THHmmssZ}", Utility.findGMTTime(data.SensorEventTime.HasValue ? data.SensorEventTime.Value : DateTime.Now, TZ));



                    // payment duration
                    //Payment duration: SensorEventTime - expirytime
                    tranData.PaymentDuration = Utility.calcOccTimeDiff(data.ExpiryTime.HasValue ? data.ExpiryTime.Value : DateTime.Now,
                        data.SensorEventTime.HasValue ? data.SensorEventTime.Value : DateTime.Now);

                    //payment amount
                    tranData.AmountIncents = data.AmountIncents;
                    paymentsData.PaymentTransactions.Add(tranData);
                }

            }
            catch (Exception ex)
            {
                var appPath = System.Web.Hosting.HostingEnvironment.ApplicationPhysicalPath;
                string errorPath = Path.Combine(appPath, "GetPaymentData", "Timezones");
                ExceptionLogging.SendErrorToText(ex, "GetPaymentData", errorPath);
                paymentsData.ReturnCode = ((int)ReturnCodeEnum.Error).ToString();
            }
            return paymentsData;
        }
    }

}
