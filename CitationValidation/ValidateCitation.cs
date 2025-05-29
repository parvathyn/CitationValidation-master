using DataAccess.Model;
using DataAccess.Model.CitationValidation;
using System;
using System.IO;
using System.ServiceModel;
using System.ServiceModel.Channels;
using System.ServiceModel.Web;
using System.Text;


namespace CitationValidation
{
    public class ValidateCitation : IValidateCitation
    {
        #region RefreshServce
        public TransactionTransform GetCitationData(string enfKey)
        {
            RequestResponseEntity logData = new RequestResponseEntity();
            logData.Request.CurrentUTCTime = DateTime.UtcNow;
            logData.Request.RequestBody = OperationContext.Current.IncomingMessageHeaders.To.ToString();
            if (OperationContext.Current.IncomingMessageProperties[RemoteEndpointMessageProperty.Name] != null)
            {
                RemoteEndpointMessageProperty endpoint = OperationContext.Current.IncomingMessageProperties[RemoteEndpointMessageProperty.Name] as RemoteEndpointMessageProperty;
                logData.Request.IPAddress = endpoint.Address;
            }

            var appPath = System.Web.Hosting.HostingEnvironment.ApplicationPhysicalPath;


            string logFilePath = Path.Combine(appPath, "LogRequestResponse");
            try
            {
                string filePath = Path.Combine(appPath, "App_Data", "Timezones", "TimeZones.xml");

                // TransactionTransform dataObj = TransactionDataValidatioin.GetTransformData(enfKey, filePath, logData);

                //////////////////////////////////
                TransactionTransform dataObj = new TransactionTransform();
                int? customerId = this.GetCustomerId(enfKey);
                if (customerId.HasValue)
                {
                    switch (customerId.Value)
                    {
                        case 7002: //CoralGables
                            //dataObj = TransactionDataValidatioin.GetCoralGablesTransformData(enfKey, filePath, logData);
                            dataObj = TransactionDataValidatioin.GetCoralGablesTransformDataOld(enfKey, filePath, logData);
                            break;
                        default:
                            dataObj = TransactionDataValidatioin.GetTransformData(enfKey, filePath, logData);
                            break;
                    }
                }
                //////////////////////////////////

                logData.Response.ResponseBody = dataObj.GetJsonData();
                logData.Response.CurrentUTCTime = DateTime.UtcNow;
                logFilePath = Path.Combine(appPath, "LogRequestResponse", logData.Request.CustomerId);
                RequestResponseLog.ReqResLog(logData, logFilePath);
                return dataObj;
            }
            catch (Exception ex)
            {
                string errorPath = Path.Combine(appPath, "App_Data", "Timezones");
                ExceptionLogging.SendErrorToText(ex, "GetCitationData", errorPath);
                ex = null;
                RequestResponseLog.ReqResLog(logData, logFilePath);
                return new TransactionTransform() { ReturnCode = ((int)ReturnCodeEnum.Error).ToString(), EnforcementKey = "fromd catach" };
            }
        }

        public ChicagoTransactionTransform GetChicagoCitationData(string enfKey)
        {
            RequestResponseEntity logData = new RequestResponseEntity();
            logData.Request.CurrentUTCTime = DateTime.UtcNow;
            logData.Request.RequestBody = OperationContext.Current.IncomingMessageHeaders.To.ToString();
            if (OperationContext.Current.IncomingMessageProperties[RemoteEndpointMessageProperty.Name] != null)
            {
                RemoteEndpointMessageProperty endpoint = OperationContext.Current.IncomingMessageProperties[RemoteEndpointMessageProperty.Name] as RemoteEndpointMessageProperty;
                logData.Request.IPAddress = endpoint.Address;
            }
            var appPath = System.Web.Hosting.HostingEnvironment.ApplicationPhysicalPath;

            string logFilePath = Path.Combine(appPath, "LogRequestResponse");
            try
            {
                string filePath = Path.Combine(appPath, "App_Data", "Timezones", "TimeZones.xml");

                ChicagoTransactionTransform dataObj = TransactionDataValidatioin.GetTransformDataChicago(enfKey, filePath, logData);

                logData.Response.ResponseBody = dataObj.GetJsonData();
                logData.Response.CurrentUTCTime = DateTime.UtcNow;
                logFilePath = Path.Combine(appPath, "LogRequestResponse", logData.Request.CustomerId);
                RequestResponseLog.ReqResLog(logData, logFilePath);
                return dataObj;
            }
            catch (Exception ex)
            {
                string errorPath = Path.Combine(appPath, "App_Data", "Timezones");
                ExceptionLogging.SendErrorToText(ex, "GetCitationData", errorPath);
                ex = null;
                RequestResponseLog.ReqResLog(logData, logFilePath);
                return new ChicagoTransactionTransform() { ReturnCode = ((int)ReturnCodeEnum.Error).ToString(), EnforcementKey = "fromd catach" };
            }
        }

        public string GetLogData(string customerId, string folderName)
        {
            string resonseData = string.Empty;
            try
            {
                var appPath = System.Web.Hosting.HostingEnvironment.ApplicationPhysicalPath;
                var logFilePath = Path.Combine(appPath, "LogRequestResponse", customerId, folderName + ".txt");
                resonseData = File.ReadAllText(logFilePath, Encoding.ASCII);
            }
            catch (Exception ex)
            {
                resonseData = "Opps error happend.";
            }
            return resonseData;
        }

        private int? GetCustomerId(string key)
        {
            string[] values = key.Split('-');
            int? tempcustomerID = null;
            if (values[0] == null) //CustomerrId
            {

            }
            else
            {
                tempcustomerID = values[0].ToNullableInt();
                if (tempcustomerID.HasValue == false)
                {

                }
            }

            return tempcustomerID;
        }

        #endregion
        #region PlateNo
        public PlateNos GetPlateNo(string customerId, string plateNo)
        {
            PlateNos plateNosObj = new PlateNos();
            var appPath = @"D:\Logs\CitationValidation\Plate";
            //var appPath = System.Web.Hosting.HostingEnvironment.ApplicationPhysicalPath;
            PlateNoRequestResponse logData = new PlateNoRequestResponse();
            string logFilePath = Path.Combine(appPath, "PlateLogRequestResponse");
            try
            {
                #region Log Declartion
                logData.Request.CustomerId = customerId;
                logData.Request.CurrentUTCTime = DateTime.UtcNow;
                logData.Request.RequestURL = OperationContext.Current.IncomingMessageHeaders.To.ToString();
                if (OperationContext.Current.IncomingMessageProperties[RemoteEndpointMessageProperty.Name] != null)
                {
                    RemoteEndpointMessageProperty endpoint = OperationContext.Current.IncomingMessageProperties[RemoteEndpointMessageProperty.Name] as RemoteEndpointMessageProperty;
                    logData.Request.IPAddress = endpoint.Address;
                }
                #endregion

                plateNosObj = new PlateFactory().GetData(Int32.Parse(customerId), plateNo);
                logData.Response.ResponseBody = plateNosObj.GetJsonData();
                logData.Response.CurrentUTCTime = DateTime.UtcNow;
                logFilePath = Path.Combine(appPath, "PlateLogRequestResponse", logData.Request.CustomerId);
                RequestResponseLog.PlateReqResLog(logData, logFilePath);
            }
            catch (Exception ex)
            {
                plateNosObj.Candidate.Add(PlateFactory.GetErrorPlate);
                ///////////////////Log//////////////////////////////
                logData.Response.ResponseBody = plateNosObj.GetJsonData();
                logData.Response.CurrentUTCTime = DateTime.UtcNow;
                logFilePath = Path.Combine(appPath, "PlateLogRequestResponse", logData.Request.CustomerId);
                RequestResponseLog.PlateReqResLog(logData, logFilePath);
            }

            return plateNosObj;
        }
        public PlateNos GetPlateNo(string customerId, string plateNo, string zoneid)
        {
            //Not fully implenmetd, as it is not required by Philli.
            PlateNos plateNosObj = new PlateNos();
            //var appPath = @"G:\Projects\Services-AutoIssueNEW\CitationValidation\CitationValidation\TestApplication\Plate";
            var appPath = System.Web.Hosting.HostingEnvironment.ApplicationPhysicalPath;
            PlateNoRequestResponse logData = new PlateNoRequestResponse();
            string logFilePath = Path.Combine(appPath, "PlateLogRequestResponse");
            try
            {
                #region Log Declartion
                logData.Request.CustomerId = customerId;
                logData.Request.CurrentUTCTime = DateTime.UtcNow;
                logData.Request.RequestURL = OperationContext.Current.IncomingMessageHeaders.To.ToString();
                if (OperationContext.Current.IncomingMessageProperties[RemoteEndpointMessageProperty.Name] != null)
                {
                    RemoteEndpointMessageProperty endpoint = OperationContext.Current.IncomingMessageProperties[RemoteEndpointMessageProperty.Name] as RemoteEndpointMessageProperty;
                    logData.Request.IPAddress = endpoint.Address;
                }
                #endregion

                plateNosObj = new PlateFactory().GetData(Int32.Parse(customerId), plateNo, zoneid);
                logData.Response.ResponseBody = plateNosObj.GetJsonData();
                logData.Response.CurrentUTCTime = DateTime.UtcNow;
                logFilePath = Path.Combine(appPath, "PlateLogRequestResponse", logData.Request.CustomerId);
                RequestResponseLog.PlateReqResLog(logData, logFilePath);
            }
            catch (Exception ex)
            {
                plateNosObj.Candidate.Add(PlateFactory.GetErrorPlate);
                ///////////////////Log//////////////////////////////
                //logData.Response.ResponseBody = plateNosObj.GetJsonData();
                logData.Response.ResponseBody = plateNosObj.GetJsonData();
                logData.Response.CurrentUTCTime = DateTime.UtcNow;
                logFilePath = Path.Combine(appPath, "PlateLogRequestResponse", logData.Request.CustomerId);
                RequestResponseLog.PlateReqResLog(logData, logFilePath);
            }

            return plateNosObj;
        }
        public string GetPlateLogData(string customerId, string folderName)
        {
            string resonseData = string.Empty;
            try
            {
                //var appPath = @"G:\Projects\Services-AutoIssueNEW\CitationValidation\CitationValidation\TestApplication\Plate";
                var appPath = System.Web.Hosting.HostingEnvironment.ApplicationPhysicalPath;
                var logFilePath = Path.Combine(appPath, "PlateLogRequestResponse", customerId, folderName + ".txt");
                resonseData = File.ReadAllText(logFilePath, Encoding.ASCII);
            }
            catch (Exception ex)
            {
                resonseData = "No data exists.";
            }
            return resonseData;
        }
        public string GetPlateLogDataSplited(string customerId, string folderName, string hour)
        {
            string resonseData = string.Empty;
            try
            {
                //var appPath = @"G:\Projects\Services-AutoIssueNEW\CitationValidation\CitationValidation\TestApplication\Plate";
                var appPath = System.Web.Hosting.HostingEnvironment.ApplicationPhysicalPath;
                var logFilePath = Path.Combine(appPath, "PlateLogRequestResponse", customerId, folderName, hour + ".txt");
                resonseData = File.ReadAllText(logFilePath, Encoding.ASCII);
            }
            catch (Exception ex)
            {
                resonseData = "No data exists.";
            }
            return resonseData;
        }

        public string GetPlateLogDataNEWUnderDevelop(string customerId, string folderName)
        {
            string resonseData = string.Empty;
            try
            {
                //var appPath = @"G:\Projects\Services-AutoIssueNEW\CitationValidation\CitationValidation\TestApplication\Plate";
                var appPath = System.Web.Hosting.HostingEnvironment.ApplicationPhysicalPath;
                var logFilePath = Path.Combine(appPath, "PlateLogRequestResponse", customerId, folderName + ".txt");
                resonseData = File.ReadAllText(logFilePath, Encoding.ASCII);
            }
            catch (Exception ex)
            {
                resonseData = "No data exists.";
            }
            return resonseData;
        }
        #endregion

        #region RefreshServce- : PayBySpace & PayByPlate
        /// <summary>
        /// Key Structure : <EnforecementMode>-<CustomerId>-<MeterName>-<ParkingSpaceId>-<PlateNumber>-<stateName>
        /// For customers starting from August 11 ,2017: 
        /// Customer : 7002	 , Coral Gables
        /// </summary>
        /// <param name="enfKey"><EnforecementMode>-<CustomerId>-<MeterName>-<ParkingSpaceId>-<PlateNumber>-<stateName></param>
        /// <returns></returns>
        public TransactionTransform GetCitationStatus(string enfKey)
        {
            RequestResponseEntity logData = new RequestResponseEntity();
            logData.Request.CurrentUTCTime = DateTime.UtcNow;
            logData.Request.RequestBody = OperationContext.Current.IncomingMessageHeaders.To.ToString();
            if (OperationContext.Current.IncomingMessageProperties[RemoteEndpointMessageProperty.Name] != null)
            {
                RemoteEndpointMessageProperty endpoint = OperationContext.Current.IncomingMessageProperties[RemoteEndpointMessageProperty.Name] as RemoteEndpointMessageProperty;
                logData.Request.IPAddress = endpoint.Address;
            }

            //var appPath = @"D:\Logs\CitationValidation\Plate";
            var appPath = System.Web.Hosting.HostingEnvironment.ApplicationPhysicalPath;

            string logFilePath = Path.Combine(appPath, "LogRequestResponse");
            try
            {
                string filePath = Path.Combine(appPath, "App_Data", "Timezones", "TimeZones.xml");
                //File.WriteAllText(Path.Combine(appPath, "LogRequestResponse", "log.txt"), "Line 229");//test code
                ////////////////////////////////////////////
                TransactionTransform dataObj = new TransactionTransform();
                int? customerId = this.CheckCustomerId(enfKey);

                if (customerId.HasValue)
                {
                    switch (customerId.Value)
                    {
                        case 7002: //CoralGables
                            //dataObj = TransactionDataValidatioin.GetCoralGablesTransformData(enfKey, filePath, logData, CustomerTimes.GetCoralGablesTime);//Original
                            dataObj = TransactionDataValidatioin.GetCoralGablesTransformDataV2(enfKey, filePath, logData, CustomerTimes.GetCoralGablesTime);
                            break;
                        case 4120: //Atlanta : 
                            //dataObj = TransactionDataValidatioin.GetAtlantaTransformData(enfKey, filePath, logData, CustomerTimes.GetAtlantaTime);
                            //dataObj = TransactionDataValidatioin.GetAtlantaTransformDataV3(enfKey, filePath, logData, CustomerTimes.GetAtlantaTime);
                            //dataObj = TransactionDataValidatioin.GetAtlantaTransformDataV4(enfKey, filePath, logData, CustomerTimes.GetAtlantaTime);
                            dataObj = TransactionDataValidatioin.GetAtlantaTransformDataV5(enfKey, filePath, logData, CustomerTimes.GetAtlantaTime);// Passsport.TESTING on June-13
                            break;
                        case 7001: //South Miami
                            //dataObj = TransactionDataValidatioin.GetSouthMiamiTransformData(enfKey, filePath, logData);
                            //dataObj = TransactionDataValidatioin.GetSouthMiamiTransformDataV2(enfKey, filePath, logData, CustomerTimes.GetSouthMiamiTime);
                            //dataObj = TransactionDataValidatioin.GetSouthMiamiTransformDataV3(enfKey, filePath, logData, CustomerTimes.GetSouthMiamiTime);//Original
                            dataObj = TransactionDataValidatioin.GetSouthMiamiTransformDataFinal(enfKey, filePath, logData, CustomerTimes.GetSouthMiamiTime);
                            break;
                        case 7013: //Raleigh
                            dataObj = TransactionDataValidatioin.GetRaleighTransformData(enfKey, filePath, logData);
                            break;
                        case 4194: //	Birmingham, MI
                            //dataObj = TransactionDataValidatioin.GetBirminghamMITransformData(enfKey, filePath, logData);
                            // dataObj = TransactionDataValidatioin.GetBirminghamMITransformDataNew(enfKey, filePath, logData, CustomerTimes.GetBirminghamMITime);
                            dataObj = TransactionDataValidatioin.GetBirminghamMITransformDataNewV2(enfKey, filePath, logData, CustomerTimes.GetBirminghamMITime);
                            break;
                        case 4210: //	CrystalLake
                            dataObj = TransactionDataValidatioin.GetCrystalLakeTransformData(enfKey, filePath, logData, CustomerTimes.GetCrystalLakeTime);
                            break;
                        case 8014: //Sharjah Municipality
                            dataObj = TransactionDataValidatioin.GetSharjahMunicipalityTransformData(enfKey, filePath, logData);
                            break;
                        case 7025: //Indiana Borough
                            //dataObj = TransactionDataValidatioin.GetIndianaBoroughTransformData(enfKey, filePath, logData);
                            //dataObj = TransactionDataValidatioin.GetIndianaBoroughTransformDataV2(enfKey, filePath, logData, CustomerTimes.GetSouthMiamiTime);//Original
                            dataObj = TransactionDataValidatioin.GetIndianaBoroughTransformDataV3(enfKey, filePath, logData, CustomerTimes.GetSouthMiamiTime);//to Go
                            break;
                        case 7026: //Franklin
                            //dataObj = TransactionDataValidatioin.GetFranklinTransformData(enfKey, filePath, logData);
                            dataObj = TransactionDataValidatioin.GetFranklinTransformDataV2(enfKey, filePath, logData, CustomerTimes.GetFranklinTime);
                            break;
                        case 7032: //Auburn
                            //dataObj = TransactionDataValidatioin.GetAuburnTransformData(enfKey, filePath, logData, CustomerTimes.GetAuburnTime);//Original
                            dataObj = TransactionDataValidatioin.GetAuburnTransformDataV2(enfKey, filePath, logData, CustomerTimes.GetAuburnTime);
                            // dataObj = TransactionDataValidatioin.GetAuburnTransformDataV3Test(enfKey, filePath, logData, CustomerTimes.GetAuburnTime);
                            break;
                        case 7009://Sunny Isles Beach
                            //dataObj = TransactionDataValidatioin.GetSunnyIslesBeachTransformData(enfKey, filePath, logData);
                            // dataObj = TransactionDataValidatioin.GetSunnyIslesBeachTransformDataV2(enfKey, filePath, logData, CustomerTimes.GetSunnyIslesBeachTime);
                            dataObj = TransactionDataValidatioin.GetSunnyIslesBeachTransformDataV3(enfKey, filePath, logData, CustomerTimes.GetSunnyIslesBeachTime);
                            break;
                        case 7007://Surfside
                            //dataObj = TransactionDataValidatioin.GetSurfsideTransformData(enfKey, filePath, logData);
                            //dataObj = TransactionDataValidatioin.GetSurfsideTransformDataV2(enfKey, filePath, logData, CustomerTimes.GetSurfsideTime);
                            // dataObj = TransactionDataValidatioin.GetSurfsideTransformDataV3(enfKey, filePath, logData, CustomerTimes.GetSurfsideTime); // Original
                            dataObj = TransactionDataValidatioin.GetSurfsideTransformDataV4(enfKey, filePath, logData, CustomerTimes.GetSurfsideTime);
                            break;
                        case 7028: //SiouxCity
                                   //dataObj = TransactionDataValidatioin.GetSiouxCityTransformData(enfKey, filePath, logData);
                                   // dataObj = TransactionDataValidatioin.GetSiouxCityTransformDataV2(enfKey, filePath, logData, CustomerTimes.GetSiouxCityTime);// 

                            //dataObj = TransactionDataValidatioin.GetSiouxCityTransformDataV3(enfKey, filePath, logData, CustomerTimes.GetSiouxCityTime); //in prod

                            dataObj = TransactionDataValidatioin.GetSiouxCityTransformDataV4(enfKey, filePath, logData, CustomerTimes.GetSiouxCityTime); //dev:to add zone
                            break;
                        case 7034://Detroit
                            //dataObj = TransactionDataValidatioin.GetDetroitTransformData(enfKey, filePath, logData, CustomerTimes.GetDetroitTime);// Original
                            //dataObj = TransactionDataValidatioin.GetDetroitTransformDataV2(enfKey, filePath, logData, CustomerTimes.GetDetroitTime);//to Go
                            dataObj = TransactionDataValidatioin.GetDetroitTransformDataV3(enfKey, filePath, logData, CustomerTimes.GetDetroitTime);//to Go
                            break;
                        case 7008: //Bay Harbor
                            //dataObj = TransactionDataValidatioin.GetBayHarborTransformData(enfKey, filePath, logData, CustomerTimes.GetBayHarborTime);
                            //dataObj = TransactionDataValidatioin.GetBayHarborTransformDataV2(enfKey, filePath, logData, CustomerTimes.GetBayHarborTime);// Original
                            dataObj = TransactionDataValidatioin.GetBayHarborTransformDataV3(enfKey, filePath, logData, CustomerTimes.GetBayHarborTime);//to Go
                            break;
                        case 7078: //North Bay Village
                            dataObj = TransactionDataValidatioin.GetNorthBayVillage(enfKey, filePath, logData, CustomerTimes.GetNorthBayVillageTime);//to Go
                            break;
                        case 7006: //Metro Rail
                            //dataObj = TransactionDataValidatioin.GetMetroRailTransformData(enfKey, filePath, logData, CustomerTimes.GetBayHarborTime);// Original
                            dataObj = TransactionDataValidatioin.GetMetroRailTransformDataV2(enfKey, filePath, logData, CustomerTimes.GetBayHarborTime);//to Go
                            break;
                        case 7038://Port Hood River 
                            //dataObj = TransactionDataValidatioin.GetPortHoodRiverTransformData(enfKey, filePath, logData, CustomerTimes.GetPortHoodRiverTime);//Original
                            dataObj = TransactionDataValidatioin.GetPortHoodRiverTransformDataV2(enfKey, filePath, logData, CustomerTimes.GetPortHoodRiverTime);
                            break;
                        case 7029://RoyalOak
                            //dataObj = TransactionDataValidatioin.GetRoyalOakTransformData(enfKey, filePath, logData);
                            dataObj = TransactionDataValidatioin.GetRoyalOakTransformDataV2(enfKey, filePath, logData, CustomerTimes.GetRoyalOakTime);
                            break;
                        case 7010://Tempe
                            //dataObj = TransactionDataValidatioin.GetTempeTransformData(enfKey, filePath, logData, CustomerTimes.GetTempeTime);
                            //dataObj = TransactionDataValidatioin.GetTempeTransformDataV2(enfKey, filePath, logData, CustomerTimes.GetTempeTime);
                            dataObj = TransactionDataValidatioin.GetTempeTransformDataV3(enfKey, filePath, logData, CustomerTimes.GetTempeTime); //included IPS
                            break;
                        case 7030://Tybee Island
                            //dataObj = TransactionDataValidatioin.GetTybeeIslandTransformData(enfKey, filePath, logData, CustomerTimes.GetPortHoodRiverTime);//Original
                            dataObj = TransactionDataValidatioin.GetTybeeIslandTransformDataV2(enfKey, filePath, logData, CustomerTimes.GetPortHoodRiverTime);
                            break;
                        case 7003: //Miami Parking Authority
                            //dataObj = TransactionDataValidatioin.GetMiamiParkingAuthorityTransformData(enfKey, filePath, logData, CustomerTimes.GetBayHarborTime);
                            // dataObj = TransactionDataValidatioin.GetMiamiParkingAuthorityTransformDataV2(enfKey, filePath, logData, CustomerTimes.GetMPATime);
                            dataObj = TransactionDataValidatioin.GetMiamiParkingAuthorityTransformDataV3(enfKey, filePath, logData, CustomerTimes.GetMPATime);
                            break;
                        case 4195://Charleston
                            dataObj = TransactionDataValidatioin.GeCharlestonTransformData(enfKey, filePath, logData);
                            break;
                        case 8016: //Abu Dhabi
                            dataObj = TransactionDataValidatioin.GetAbuDhabiTransformData(enfKey, filePath, logData);
                            break;
                        case 4232: //Ardsley,NY
                            dataObj = TransactionDataValidatioin.GetArdsleyNYTransformData(enfKey, filePath, logData, CustomerTimes.GetArdsleyTime);
                            break;
                        case 7004://MiamiBeach
                            dataObj = TransactionDataValidatioin.GetMiamiBeachTransformData(enfKey, filePath, logData, CustomerTimes.GetMiamiBeachTime);
                            break;
                        case 7049://SanJose
                            dataObj = TransactionDataValidatioin.GetSanJoseTransformData(enfKey, filePath, logData, CustomerTimes.GetSanJoseTime);
                            break;
                        case 4140: //Spokane, WA
                            dataObj = TransactionDataValidatioin.GetSpokaneWATransformData(enfKey, filePath, logData, CustomerTimes.GetSpokaneWATime);
                            break;
                        case 7036://Huntsville
                            dataObj = TransactionDataValidatioin.GetHuntsvilleTransformData(enfKey, filePath, logData, CustomerTimes.GetHuntsvilleTime);
                            break;
                        case 4176://New Orleans
                            //dataObj = TransactionDataValidatioin.GetNewOrleansTransformData(enfKey, filePath, logData);
                            //dataObj = TransactionDataValidatioin.GetNewOrleansTransformDataV2(enfKey, filePath, logData, CustomerTimes.GetNOLATime);
                            //dataObj = TransactionDataValidatioin.GetNewOrleansTransformDataV3(enfKey, filePath, logData, CustomerTimes.GetNOLATime);// Current
                            dataObj = TransactionDataValidatioin.GetNewOrleansTransformDataV4(enfKey, filePath, logData, CustomerTimes.GetNOLATime); //For T2
                            break;
                        case 4243://Ace Parking
                            dataObj = TransactionDataValidatioin.GetAceParkingTransformData(enfKey, filePath, logData, CustomerTimes.GetAceParkingTime);
                            break;
                        case 4135: //	City of Chester
                            //dataObj = TransactionDataValidatioin.GetCityofChesterData(enfKey, filePath, logData);
                            dataObj = TransactionDataValidatioin.GetCityofChesterDataV2(enfKey, filePath, logData, CustomerTimes.GetChesterTime);
                            break;
                        case 7056://Philadelphia
                            //dataObj = TransactionDataValidatioin.GetPhiladelphiaTransformDataV2(enfKey, filePath, logData, CustomerTimes.GetPhiladelphiaTime);
                            //dataObj = TransactionDataValidatioin.GetPhiladelphiaTransformDataV3(enfKey, filePath, logData, CustomerTimes.GetPhiladelphiaTime);
                            //dataObj = TransactionDataValidatioin.GetPhiladelphiaTransformDataV4(enfKey, filePath, logData, CustomerTimes.GetPhiladelphiaTime);//
                            dataObj = TransactionDataValidatioin.GetPhiladelphiaTransformDataV5(enfKey, filePath, logData, CustomerTimes.GetPhiladelphiaTime);//current
                                                                                                                                                              //dataObj = TransactionDataValidatioin.GetPhiladelphiaTransformDataV5TestForSchema5(enfKey, filePath, logData, CustomerTimes.GetPhiladelphiaTime);//testing schema5
                            break;
                        case 4142: //Glendale
                            dataObj = TransactionDataValidatioin.GetGlendaleTransformData(enfKey, filePath, logData, CustomerTimes.GetGlendaleTime);
                            break;
                        case 7072://BoyntonBeach
                            dataObj = TransactionDataValidatioin.GetBoyntonBeachTransformData(enfKey, filePath, logData, CustomerTimes.GetBoyntonBeachTime);
                            break;
                        case 4265: //City of Mount Rainier
                            dataObj = TransactionDataValidatioin.GetCityofMountRainier(enfKey, filePath, logData, CustomerTimes.GetCityofMountRainierTime);
                            break;
                        case 7073: //Dammam (KSA)
                            dataObj = TransactionDataValidatioin.GetDammamTransformData(enfKey, filePath, logData);
                            break;
                        case 4280: //City of Leavenworth
                            //dataObj = TransactionDataValidatioin.GetCityofLeavenworth(enfKey, filePath, logData, CustomerTimes.GetCityofLeavenworth);
                            dataObj = TransactionDataValidatioin.GetCityofLeavenworth2(enfKey, filePath, logData, CustomerTimes.GetCityofLeavenworth);
                            break;
                        case 4254: //ColoradoSprings
                            //dataObj = TransactionDataValidatioin.GetColoradoSprings(enfKey, filePath, logData, CustomerTimes.GetCityofColoradoSprings);
                            dataObj = TransactionDataValidatioin.GetColoradoSprings2(enfKey, filePath, logData, CustomerTimes.GetCityofColoradoSprings);
                            break;
                        case 8070: //Fuel City
                            dataObj = TransactionDataValidatioin.GetCityofFuelcity(enfKey, filePath, logData, CustomerTimes.GetFuelCity);
                            break;
                        case 7062://Jacksonville FL
                            //dataObj = TransactionDataValidatioin.GetJacksonvilleFLTransformData(enfKey, filePath, logData, CustomerTimes.GetJacksonvilleFLTime);
                            dataObj = TransactionDataValidatioin.GetJacksonvilleFLTransformDataV1(enfKey, filePath, logData, CustomerTimes.GetJacksonvilleFLTime);
                            break;
                        case 7081://ChillicotheOH	
                            dataObj = TransactionDataValidatioin.GetChillicotheOHTransformData(enfKey, filePath, logData, CustomerTimes.GetChillicotheOHTime);
                            break;
                        case 7058://Sanibel X3	
                            dataObj = TransactionDataValidatioin.GetSanibelX3TransformData(enfKey, filePath, logData, CustomerTimes.GetSanibelX3Time);
                            break;
                        case 7079://Doral	
                            dataObj = TransactionDataValidatioin.GetDoral(enfKey, filePath, logData, CustomerTimes.GetDoralFLTime);
                            break;
                        case 7086://New Smyrna Beach FL
                            dataObj = TransactionDataValidatioin.GetNewSmyrnaBeachFL(enfKey, filePath, logData, CustomerTimes.GetNewSmyrnaBeachFLTime);
                            break;
                        case 7014://SanDiego
                            dataObj = TransactionDataValidatioin.GetSanDiego(enfKey, filePath, logData, CustomerTimes.GetSanDiegoTime);
                            break;
                        case 8075: //Muscat 
                            dataObj = TransactionDataValidatioin.GetMuscatTransformData(enfKey, filePath, logData);
                            break;
                        case 4337://Salem OR
                            dataObj = TransactionDataValidatioin.GetSalemOR(enfKey, filePath, logData, CustomerTimes.GetSalemORTime);
                            break;
                        default:
                            break;
                    }

                    logData.Response.ResponseBody = dataObj.GetJsonData();
                    logData.Response.CurrentUTCTime = DateTime.UtcNow;
                    logFilePath = Path.Combine(appPath, "LogRequestResponse", logData.Request.CustomerId);
                    //RequestResponseLog.ReqResLog(logData, logFilePath);
                    RequestResponseLog.ReqResLogV2(logData, logFilePath);
                }
                //////////////////////////////////

                return dataObj;
            }
            catch (Exception ex)
            {
                string errorPath = Path.Combine(appPath, "App_Data", "Timezones");
                ExceptionLogging.SendErrorToText(ex, "GetCitationStatus", errorPath);
                ex = null;
                RequestResponseLog.ReqResLog(logData, logFilePath);
                return new TransactionTransform() { ReturnCode = ((int)ReturnCodeEnum.Error).ToString(), EnforcementKey = "" };
            }
        }
        private int? CheckCustomerId(string key)
        {
            string[] values = key.Split('-');
            int? tempcustomerID = null;
            if (values[1] == null) //CustomerrId
            {

            }
            else
            {
                tempcustomerID = values[1].ToNullableInt();
                if (tempcustomerID.HasValue == false)
                {

                }
            }

            return tempcustomerID;
        }
        #endregion

        #region RefreshServce--Target- : PayBySpace & PayByPlate
        public TransactionTransform CitationTarget(string target, string enfKey)
        {
            TransactionTransform dataObj = new TransactionTransform();
            RequestResponseEntity logData = new RequestResponseEntity();
            logData.Request.CurrentUTCTime = DateTime.UtcNow;
            logData.Request.RequestBody = OperationContext.Current.IncomingMessageHeaders.To.ToString();
            if (OperationContext.Current.IncomingMessageProperties[RemoteEndpointMessageProperty.Name] != null)
            {
                RemoteEndpointMessageProperty endpoint = OperationContext.Current.IncomingMessageProperties[RemoteEndpointMessageProperty.Name] as RemoteEndpointMessageProperty;
                logData.Request.IPAddress = endpoint.Address;
            }
            VendorNames currentVendor = VendorNames.None;
            //var appPath = @"G:\Projects\Services-AutoIssueNEW\CitationValidation\CitationValidation\TestApplication";
            var appPath = System.Web.Hosting.HostingEnvironment.ApplicationPhysicalPath;
            try
            {
                string filePath = Path.Combine(appPath, "App_Data", "Timezones", "TimeZones.xml");
                //check traget type
                if (string.IsNullOrEmpty(target))
                {
                    dataObj = this.ChangeStatus(dataObj, ReturnCodeEnum.InvalidInputParameters);
                    return dataObj;
                }
                currentVendor = CheckTarget(target);
                int? customerId = this.CheckCustomerId(enfKey);
                if (customerId.HasValue)
                {
                    switch (customerId.Value)
                    {
                        case 7056://Philadelphia
                            dataObj = TransactionDataValidatioin.GetPhiladelphiaTransformVendor(currentVendor, enfKey, filePath, logData, CustomerTimes.GetPhiladelphiaTime);
                            break;
                        default:
                            break;
                    }
                }
                else
                {
                    dataObj = this.ChangeStatus(dataObj, ReturnCodeEnum.InvalidInputParameters);
                    return dataObj;
                }
            }
            catch (Exception ex)
            {

            }
            return dataObj;

        }

        private TransactionTransform ChangeStatus(TransactionTransform dataObj, ReturnCodeEnum code)
        {
            dataObj.ReturnCode = ((int)code).ToString();
            return dataObj;
        }

        private VendorNames CheckTarget(string target)
        {
            VendorNames currentVendor = VendorNames.None;
            switch (target.ToLower())
            {
                case "t2digital":
                    currentVendor = VendorNames.T2Digital;
                    break;
                case "parkmobile":
                    currentVendor = VendorNames.ParkMobile;
                    break;
                case "parkmobile2":
                    currentVendor = VendorNames.ParkMobile2;
                    break;
                case "cale":
                    currentVendor = VendorNames.Cale;
                    break;
                case "paybyphone":
                    currentVendor = VendorNames.PayByPhone;
                    break;
                case "database":
                    currentVendor = VendorNames.Database;
                    break;
                default:
                    break;
            }

            return currentVendor;
        }
        #endregion

        #region Testing and rnd purpose
        public Stream Test(string enfKey)
        {
            WebOperationContext.Current.OutgoingResponse.ContentType = "application/json; charset=utf-8";
            RequestResponseEntity logData = new RequestResponseEntity();
            logData.Request.CurrentUTCTime = DateTime.UtcNow;
            logData.Request.RequestBody = OperationContext.Current.IncomingMessageHeaders.To.ToString();
            if (OperationContext.Current.IncomingMessageProperties[RemoteEndpointMessageProperty.Name] != null)
            {
                RemoteEndpointMessageProperty endpoint = OperationContext.Current.IncomingMessageProperties[RemoteEndpointMessageProperty.Name] as RemoteEndpointMessageProperty;
                logData.Request.IPAddress = endpoint.Address;
            }
            // var appPath = System.Web.Hosting.HostingEnvironment.ApplicationPhysicalPath;
            var appPath = @"G:\Projects\Services-AutoIssueNEW\CitationValidation\CitationValidation\TestApplication";
            string logFilePath = Path.Combine(appPath, "LogRequestResponse");
            try
            {
                string filePath = Path.Combine(appPath, "App_Data", "Timezones", "TimeZones.xml");
                DataAccess.Model.CitationValidation.CitationValidationGeneralFactory factory = null;
                int? customerId = this.GetCustomerId(enfKey);
                if (customerId.HasValue)
                {
                    switch (customerId.Value)
                    {
                        case 7012: //Chicago
                            factory = new DataAccess.Model.CitationValidation.CitationValidationChicagoFactory(enfKey, customerId.Value, filePath, logData);
                            break;
                        case 7002: //CoralGables
                            factory = new DataAccess.Model.CitationValidation.CitationValidationCoralGablesFactory(enfKey, customerId.Value, filePath, logData);
                            break;
                        case 7013: //Raleigh
                            factory = new DataAccess.Model.CitationValidation.CitationValidationRaleighFactory(enfKey, customerId.Value, filePath, logData);
                            break;
                        case 7001:
                            factory = new DataAccess.Model.CitationValidation.CitationValidationSouthMiamiFactory(enfKey, customerId.Value, filePath, logData);
                            break;
                        case 4194:
                            factory = new DataAccess.Model.CitationValidation.CitationValidationBirminghamMIFactory(enfKey, customerId.Value, filePath, logData);
                            break;
                        default:
                            break;
                    }

                    var dataObj = factory.GetTransformData();
                    string jsonData = dataObj.GetJsonData();
                    logData.Response.ResponseBody = jsonData;
                    logData.Response.CurrentUTCTime = DateTime.UtcNow;
                    logFilePath = Path.Combine(appPath, "LogRequestResponse", logData.Request.CustomerId);
                    RequestResponseLog.ReqResLog(logData, logFilePath);
                    MemoryStream ms = new MemoryStream(Encoding.UTF8.GetBytes(jsonData));
                    return ms;
                }
                else
                {
                    return HandleError(logFilePath, appPath, ref logData, null);
                }
            }
            catch (Exception ex)
            {
                return HandleError(logFilePath, appPath, ref logData, null);
            }
        }
        public TransactionTransform GetCoralGablesCitationData(string enfKey)
        {
            RequestResponseEntity logData = new RequestResponseEntity();
            logData.Request.CurrentUTCTime = DateTime.UtcNow;
            logData.Request.RequestBody = OperationContext.Current.IncomingMessageHeaders.To.ToString();
            if (OperationContext.Current.IncomingMessageProperties[RemoteEndpointMessageProperty.Name] != null)
            {
                RemoteEndpointMessageProperty endpoint = OperationContext.Current.IncomingMessageProperties[RemoteEndpointMessageProperty.Name] as RemoteEndpointMessageProperty;
                logData.Request.IPAddress = endpoint.Address;
            }
            var appPath = @"G:\Projects\Services-AutoIssueNEW\CitationValidation\CitationValidation\TestApplication";
            //var appPath = System.Web.Hosting.HostingEnvironment.ApplicationPhysicalPath;

            string logFilePath = Path.Combine(appPath, "LogRequestResponse");
            try
            {
                string filePath = Path.Combine(appPath, "App_Data", "Timezones", "TimeZones.xml");

                TransactionTransform dataObj = TransactionDataValidatioin.GetCoralGablesTransformDataOld(enfKey, filePath, logData);

                logData.Response.ResponseBody = dataObj.GetJsonData();
                logData.Response.CurrentUTCTime = DateTime.UtcNow;
                logFilePath = Path.Combine(appPath, "LogRequestResponse", logData.Request.CustomerId);
                RequestResponseLog.ReqResLog(logData, logFilePath);
                return dataObj;
            }
            catch (Exception ex)
            {
                string errorPath = Path.Combine(appPath, "App_Data", "Timezones");
                ExceptionLogging.SendErrorToText(ex, "GetCitationData", errorPath);
                ex = null;
                RequestResponseLog.ReqResLog(logData, logFilePath);
                return new TransactionTransform() { ReturnCode = ((int)ReturnCodeEnum.Error).ToString(), EnforcementKey = "fromd catach" };
            }
        }
        private Stream HandleError(string logFilePath, string appPath, ref RequestResponseEntity logData, Exception ex)
        {
            string errorPath = Path.Combine(appPath, "App_Data", "Timezones");
            if (ex != null)
            {
                ExceptionLogging.SendErrorToText(ex, "GetCitationData", errorPath);
                ex = null;
            }
            RequestResponseLog.ReqResLog(logData, logFilePath);
            MemoryStream ms = new MemoryStream(Encoding.UTF8.GetBytes((new TransactionPaymentDetail() { ReturnCode = ((int)ReturnCodeEnum.Error).ToString(), EnforcementKey = "from catch" }).GetJsonData()));
            return ms;
        }
        #endregion

        public Stream getallactivepurchases(string firstParameter, string secondParamter)
        {
            var value = @"<ArrayOfValidParkingData/>";
            Encoding encoding = Encoding.GetEncoding("ISO-8859-1");
            WebOperationContext.Current.OutgoingResponse.ContentType = "text/xml";
            byte[] returnBytes = null;
            try
            {
                value = DataAccess.Model.Cale.CaleGenericFactory.GetAllActivePurchases(firstParameter, secondParamter);
                returnBytes = encoding.GetBytes(value);
                return new MemoryStream(returnBytes);
            }
            catch (Exception ex)
            {

            }
            //value.Position = 0;
            returnBytes = encoding.GetBytes(value);
            return new MemoryStream(returnBytes);
        }



        public Stream DownloadLogData(string customerId, string folderName)
        {
            Stream resonseData = null;
            try
            {
                var appPath = System.Web.Hosting.HostingEnvironment.ApplicationPhysicalPath;
                var logFilePath = Path.Combine(appPath, "LogRequestResponse", customerId, folderName + ".txt");
                //resonseData = File.ReadAllText(logFilePath, Encoding.ASCII);

                //var fileName = Path.GetFileName(path);
                WebOperationContext.Current.OutgoingResponse.ContentType = "application/octet-stream";
                WebOperationContext.Current.OutgoingResponse.Headers.Add("content-disposition", "inline; filename=" + customerId + "-" + folderName);

                resonseData = File.OpenRead(logFilePath);
            }
            catch (Exception ex)
            {
                //resonseData = "Opps error happend.";
            }
            return resonseData;
        }



    }
}
