using DataAccess.Model;
using System;
using System.Collections.Generic;
using System.Data;
using System.Data.SqlClient;
using System.IO;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace DataAccess
{
    public class DataAccess
    {

        #region Coral Gables
        public static List<Transaction> GetCoralGablesCitationDataOthersWithPlateName(int customerID, string ZoneID, string MeterName, string ParkingSpaceID, string PlateNumber, string stateName, string SpaceStatus)
        {
            List<Transaction> validatedData = new List<Transaction>();
            SqlConnection connection = null;
            try
            {
                connection = new SqlConnection(System.Configuration.ConfigurationManager.ConnectionStrings["Connector"].ToString());
                SqlCommand sqlCommand = new SqlCommand();
                sqlCommand.CommandType = CommandType.StoredProcedure;
                //sqlCommand.CommandText = "SpacePlateTxnFinal_railegh";
                sqlCommand.CommandText = "CoralGables_Refresh";
                sqlCommand.Parameters.AddWithValue("@customerID", customerID);
                sqlCommand.Parameters.AddWithValue("@ZoneID", ZoneID);
                sqlCommand.Parameters.AddWithValue("@MeterName", MeterName);
                sqlCommand.Parameters.AddWithValue("@ParkingSpaceID", ParkingSpaceID);
                sqlCommand.Parameters.AddWithValue("@PlateNumber", PlateNumber);
                sqlCommand.Parameters.AddWithValue("@SpaceStatus", SpaceStatus);
                sqlCommand.Parameters.AddWithValue("@State", stateName);
                sqlCommand.Connection = connection;
                sqlCommand.CommandTimeout = 120;
                connection.Open();
                using (var reader = sqlCommand.ExecuteReader())
                {
                    while (reader.Read())
                    {
                        var item = new Transaction();
                        item.EnforcementKey = reader["EnforcementKey"] == System.DBNull.Value ? null : (string)reader["EnforcementKey"];
                        item.CustomerId = (reader.GetInt64(reader.GetOrdinal("CustomerId"))).ToString();
                        item.ParkingSpaceID = reader["ParkingSpaceID"] == System.DBNull.Value ? default(int) : (int)reader["ParkingSpaceID"];
                        item.PlateNumber = reader["PlateNumber"] == System.DBNull.Value ? null : (string)reader["PlateNumber"];
                        item.MeterName = null; // reader["MeterName"] == System.DBNull.Value ? null : (string)reader["MeterName"];
                        item.MeterId = null; // reader["meterid"] == System.DBNull.Value ? null : (string)reader["meterid"];
                        item.Latitude = reader["Latitude"] == System.DBNull.Value ? default(double?) : (double)reader["Latitude"];
                        item.Latitude = reader["Longitude"] == System.DBNull.Value ? default(double?) : (double)reader["Longitude"];

                        item.LPLocation = reader["LPLocation"] == System.DBNull.Value ? null : (string)reader["LPLocation"];
                        item.VMake = reader["VMake"] == System.DBNull.Value ? null : (string)reader["VMake"];
                        item.VModel = reader["VModel"] == System.DBNull.Value ? null : (string)reader["VModel"];
                        item.VColour = reader["VColour"] == System.DBNull.Value ? null : (string)reader["VColour"];
                        item.MeterStreet = reader["MeterStreet"] == System.DBNull.Value ? null : (string)reader["MeterStreet"];
                        item.Location = reader["Location"] == System.DBNull.Value ? null : (string)reader["Location"];

                        item.EnfVendorStallId = reader["EnfVendorStallId"] == System.DBNull.Value ? default(int?) : (Int32)reader["EnfVendorStallId"];
                        item.PresentMeterTime = reader["PresentMeterTime"] == System.DBNull.Value ? default(DateTime?) : (DateTime)reader["PresentMeterTime"];
                        item.SpaceStatus = reader["SpaceStatus"] == System.DBNull.Value ? null : (string)reader["SpaceStatus"];
                        item.SensorEventTime = reader["SensorEventTime"] == System.DBNull.Value ? default(DateTime?) : (DateTime)reader["SensorEventTime"];
                        item.ExpiredMinutes = reader["ExpiredMinutes"] == System.DBNull.Value ? default(int?) : (Int32)reader["ExpiredMinutes"];
                        item.AmountIncents = reader["AmountIncents"] == System.DBNull.Value ? default(decimal?) : (decimal)reader["AmountIncents"];
                        item.ExpiryTime = reader["ExpiryTime"] == System.DBNull.Value ? default(DateTime?) : (DateTime)reader["ExpiryTime"];
                        item.TimeZoneId = reader["TimeZoneId"] == System.DBNull.Value ? default(int?) : (Int32)reader["TimeZoneId"];
                        //item.PayStationID = reader["PayStationID"] == System.DBNull.Value ? default(int?) : (Int32)reader["PayStationID"];
                        item.PayStationID = reader["meterid"] == System.DBNull.Value ? default(int?) : Convert.ToInt32(reader["meterid"]);
                        item.ZoneID = reader["ZoneID"] == System.DBNull.Value ? default(int?) : (Int32)reader["ZoneID"];
                        item.ZoneName = reader["ZoneName"] == System.DBNull.Value ? null : (string)reader["ZoneName"];
                        item.Block = reader["Block"] == System.DBNull.Value ? null : (string)reader["Block"];
                        item.Direction = reader["Direction"] == System.DBNull.Value ? null : (string)reader["Direction"];
                        item.MeterStreet = reader["MeterStreet"] == System.DBNull.Value ? null : (string)reader["MeterStreet"];
                        item.StreetType = reader["StreetType"] == System.DBNull.Value ? null : (string)reader["StreetType"];
                        item.MeterStreet = reader["MeterStreet"] == System.DBNull.Value ? null : (string)reader["MeterStreet"];
                        item.CrossStreet1 = reader["CrossStreet1"] == System.DBNull.Value ? null : (string)reader["CrossStreet1"];

                        item.CrossStreet2 = reader["CrossStreet2"] == System.DBNull.Value ? null : (string)reader["CrossStreet2"];
                        item.DigiELType = reader["DigiELType"] == System.DBNull.Value ? default(int?) : (Int32)reader["DigiELType"];
                        item.DigiErrorDescr = reader["DigiErrorDescr"] == System.DBNull.Value ? null : (string)reader["DigiErrorDescr"];
                        item.GeneticELType = reader["GeneticELType"] == System.DBNull.Value ? default(int?) : (Int32)reader["GeneticELType"];
                        item.GeneticErrorDescr = reader["GeneticErrorDescr"] == System.DBNull.Value ? null : (string)reader["GeneticErrorDescr"];

                        if (reader["PMELType"] == System.DBNull.Value)
                        {
                            item.PMELType = default(int?);
                        }
                        else
                        {
                            string pMELTypeValue = reader["PMELType"].ToString();
                            if (string.IsNullOrWhiteSpace(pMELTypeValue))
                            {
                                item.PMELType = default(int?);
                            }
                            else
                            {
                                item.PMELType = (Int32)reader["PMELType"];
                            }
                        }



                        item.PMErrorDescr = reader["PMErrorDescr"] == System.DBNull.Value ? null : (string)reader["PMErrorDescr"];
                        item.EnfHourDesc = reader["EnfHourDesc"] == System.DBNull.Value ? null : (string)reader["EnfHourDesc"];
                        item.MarkedSince = reader["MarkedSince"] == System.DBNull.Value ? default(DateTime?) : (DateTime)reader["MarkedSince"];
                        item.HitDescription = reader["HitDescription"] == System.DBNull.Value ? null : (string)reader["HitDescription"];


                        if (DataRecordExtensions.HasColumn(reader, "Metertype"))
                            item.METERLOC_METERTYPE = reader["Metertype"] == System.DBNull.Value ? null : (string)reader["Metertype"];
                        if (DataRecordExtensions.HasColumn(reader, "ZoneName"))
                            item.ZoneName = reader["ZoneName"] == System.DBNull.Value ? null : (string)reader["ZoneName"];

                        //if (DataRecordExtensions.HasColumn(reader, "MeterStatus"))
                        //    item.MeterStatus = reader["MeterStatus"] == System.DBNull.Value ? default(int?) : (Int32)reader["MeterStatus"];

                        validatedData.Add(item);
                    }
                }
            }
            catch (Exception ex)
            {
                throw;
            }
            finally
            {
                if (connection.State == ConnectionState.Open)
                    connection.Close();
            }
            return validatedData;
        }

        #endregion

        #region Atlanta
        public static List<Transaction> GetAtlantaCitationDataOthersWithPlateNameV5(int customerID, string ZoneID, string MeterName, string ParkingSpaceID, string PlateNumber, string stateName, string SpaceStatus)
        {
            List<Transaction> validatedData = new List<Transaction>();
            SqlConnection connection = null;
            try
            {
                connection = new SqlConnection(System.Configuration.ConfigurationManager.ConnectionStrings["Connector"].ToString());
                SqlCommand sqlCommand = new SqlCommand();
                sqlCommand.CommandType = CommandType.StoredProcedure;
             
                //sqlCommand.CommandText = "[RefreshService_Plate]";
                //sqlCommand.Parameters.AddWithValue("@customerID", customerID);
                //sqlCommand.Parameters.AddWithValue("@ZoneID", DBNull.Value);
                //sqlCommand.Parameters.AddWithValue("@MeterName", MeterName);
                //sqlCommand.Parameters.AddWithValue("@ParkingSpaceID", ParkingSpaceID);
                //sqlCommand.Parameters.AddWithValue("@PlateNumber", PlateNumber);
                //sqlCommand.Parameters.AddWithValue("@SpaceStatus", SpaceStatus);
                //sqlCommand.Parameters.AddWithValue("@State", stateName);
                //sqlCommand.Parameters.AddWithValue("@RateNumber", DBNull.Value);
                //////////////////////////////////////////////////////
                sqlCommand.CommandText = "[Sp_PaybyPlateDetails]";
                sqlCommand.Parameters.AddWithValue("@customerID", customerID);
                sqlCommand.Parameters.AddWithValue("@ZoneID", DBNull.Value);
                sqlCommand.Parameters.AddWithValue("@MeterName", MeterName);
                sqlCommand.Parameters.AddWithValue("@ParkingSpaceID", ParkingSpaceID);
                sqlCommand.Parameters.AddWithValue("@PlateNumber", PlateNumber);
                sqlCommand.Parameters.AddWithValue("@SpaceStatus", DBNull.Value);
                ///////////////////////////////////////////////////////
                sqlCommand.Connection = connection;
                sqlCommand.CommandTimeout = 120;
                connection.Open();
                using (var reader = sqlCommand.ExecuteReader())
                {
                    while (reader.Read())
                    {
                        var item = new Transaction();
                        item.EnforcementKey = reader["EnforcementKey"] == System.DBNull.Value ? null : (string)reader["EnforcementKey"];
                        item.CustomerId = (reader.GetInt64(reader.GetOrdinal("CustomerId"))).ToString();
                        item.ParkingSpaceID = reader["ParkingSpaceID"] == System.DBNull.Value ? default(int) : (int)reader["ParkingSpaceID"];
                        item.PlateNumber = reader["PlateNumber"] == System.DBNull.Value ? null : (string)reader["PlateNumber"];
                        item.MeterName = null; // reader["MeterName"] == System.DBNull.Value ? null : (string)reader["MeterName"];
                        item.MeterId = null; // reader["meterid"] == System.DBNull.Value ? null : (string)reader["meterid"];
                        item.Latitude = reader["Latitude"] == System.DBNull.Value ? default(double?) : (double)reader["Latitude"];
                        item.Latitude = reader["Longitude"] == System.DBNull.Value ? default(double?) : (double)reader["Longitude"];

                        item.LPLocation = reader["LPLocation"] == System.DBNull.Value ? null : (string)reader["LPLocation"];
                        item.VMake = null;// reader["VMake"] == System.DBNull.Value ? null : (string)reader["VMake"];
                        item.VModel = null;// reader["VModel"] == System.DBNull.Value ? null : (string)reader["VModel"];
                        item.VColour = null;// reader["VColour"] == System.DBNull.Value ? null : (string)reader["VColour"];
                        item.MeterStreet = reader["MeterStreet"] == System.DBNull.Value ? null : (string)reader["MeterStreet"];
                        item.Location = reader["Location"] == System.DBNull.Value ? null : (string)reader["Location"];

                        item.EnfVendorStallId = reader["EnfVendorStallId"] == System.DBNull.Value ? default(int?) : (Int32)reader["EnfVendorStallId"];
                        item.PresentMeterTime = reader["PresentMeterTime"] == System.DBNull.Value ? default(DateTime?) : (DateTime)reader["PresentMeterTime"];
                        item.SpaceStatus = reader["SpaceStatus"] == System.DBNull.Value ? null : (string)reader["SpaceStatus"];
                        item.SensorEventTime = reader["SensorEventTime"] == System.DBNull.Value ? default(DateTime?) : (DateTime)reader["SensorEventTime"];
                        item.ExpiredMinutes = reader["ExpiredMinutes"] == System.DBNull.Value ? -1 : (Int32)reader["ExpiredMinutes"];



                        item.AmountIncents = default(decimal?); //reader["AmountIncents"] == System.DBNull.Value ? default(decimal?) : (decimal)reader["AmountIncents"];
                        item.ExpiryTime = reader["ExpiryTime"] == System.DBNull.Value ? default(DateTime?) : (DateTime)reader["ExpiryTime"];
                        item.ExpiredMinutes = item.RemainingTime(10);
                        item.TimeZoneId = reader["TimeZoneId"] == System.DBNull.Value ? default(int?) : (Int32)reader["TimeZoneId"];
                        //item.PayStationID = reader["PayStationID"] == System.DBNull.Value ? default(int?) : (Int32)reader["PayStationID"];
                        item.PayStationID = reader["meterid"] == System.DBNull.Value ? default(int?) : Convert.ToInt32(reader["meterid"]);
                        item.ZoneID = reader["ZoneID"] == System.DBNull.Value ? default(int?) : (Int32)reader["ZoneID"];
                        item.ZoneName = reader["ZoneName"] == System.DBNull.Value ? null : (string)reader["ZoneName"];
                        item.Block = reader["Block"] == System.DBNull.Value ? null : (string)reader["Block"];
                        item.Direction = reader["Direction"] == System.DBNull.Value ? null : (string)reader["Direction"];
                        item.MeterStreet = reader["MeterStreet"] == System.DBNull.Value ? null : (string)reader["MeterStreet"];
                        item.StreetType = reader["StreetType"] == System.DBNull.Value ? null : (string)reader["StreetType"];
                        item.MeterStreet = reader["MeterStreet"] == System.DBNull.Value ? null : (string)reader["MeterStreet"];
                        item.CrossStreet1 = reader["CrossStreet1"] == System.DBNull.Value ? null : (string)reader["CrossStreet1"];

                        item.CrossStreet2 = reader["CrossStreet2"] == System.DBNull.Value ? null : (string)reader["CrossStreet2"];
                        item.DigiELType = default(int?); //reader["DigiELType"] == System.DBNull.Value ? default(int?) : (Int32)reader["DigiELType"];
                        item.DigiErrorDescr = null; // reader["DigiErrorDescr"] == System.DBNull.Value ? null : (string)reader["DigiErrorDescr"];
                        item.GeneticELType = default(int?);  //reader["GeneticELType"] == System.DBNull.Value ? default(int?) : (Int32)reader["GeneticELType"];
                        item.GeneticErrorDescr = null; // reader["GeneticErrorDescr"] == System.DBNull.Value ? null : (string)reader["GeneticErrorDescr"];

                        if (reader["PMELType"] == System.DBNull.Value)
                        {
                            item.PMELType = default(int?);
                        }
                        else
                        {
                            string pMELTypeValue = reader["PMELType"].ToString();
                            if (string.IsNullOrWhiteSpace(pMELTypeValue))
                            {
                                item.PMELType = default(int?);
                            }
                            else
                            {
                                item.PMELType = (Int32)reader["PMELType"];
                            }
                        }



                        item.PMErrorDescr = null; //reader["PMErrorDescr"] == System.DBNull.Value ? null : (string)reader["PMErrorDescr"];
                        item.EnfHourDesc = reader["EnfHourDesc"] == System.DBNull.Value ? null : (string)reader["EnfHourDesc"];
                        item.MarkedSince = reader["MarkedSince"] == System.DBNull.Value ? default(DateTime?) : (DateTime)reader["MarkedSince"];
                        item.HitDescription = reader["HitDescription"] == System.DBNull.Value ? null : (string)reader["HitDescription"];


                        if (DataRecordExtensions.HasColumn(reader, "Metertype"))
                            item.METERLOC_METERTYPE = reader["Metertype"] == System.DBNull.Value ? null : (string)reader["Metertype"];
                        if (DataRecordExtensions.HasColumn(reader, "ZoneName"))
                            item.ZoneName = reader["ZoneName"] == System.DBNull.Value ? null : (string)reader["ZoneName"];

                        //if (DataRecordExtensions.HasColumn(reader, "MeterStatus"))
                        //    item.MeterStatus = reader["MeterStatus"] == System.DBNull.Value ? default(int?) : (Int32)reader["MeterStatus"];

                        validatedData.Add(item);
                    }
                }
            }
            catch (Exception ex)
            {
                throw;
            }
            finally
            {
                if (connection.State == ConnectionState.Open)
                    connection.Close();
            }
            return validatedData;
        }
        public static List<Transaction> GetAtlantaCitationDataOthersWithPlateName(int customerID, string ZoneID, string MeterName, string ParkingSpaceID, string PlateNumber, string stateName, string SpaceStatus)
        {
            List<Transaction> validatedData = new List<Transaction>();
            SqlConnection connection = null;
            try
            {
                connection = new SqlConnection(System.Configuration.ConfigurationManager.ConnectionStrings["Connector"].ToString());
                SqlCommand sqlCommand = new SqlCommand();
                sqlCommand.CommandType = CommandType.StoredProcedure;
                //sqlCommand.CommandText = "[SpacePlateTxnFinal_Railegh_Metername_Chicago_test]";
                sqlCommand.CommandText = "[RefreshService_Plate]";
                sqlCommand.Parameters.AddWithValue("@customerID", customerID);
                sqlCommand.Parameters.AddWithValue("@ZoneID", DBNull.Value);
                sqlCommand.Parameters.AddWithValue("@MeterName", MeterName);
                sqlCommand.Parameters.AddWithValue("@ParkingSpaceID", ParkingSpaceID);
                sqlCommand.Parameters.AddWithValue("@PlateNumber", PlateNumber);
                sqlCommand.Parameters.AddWithValue("@SpaceStatus", SpaceStatus);
                sqlCommand.Parameters.AddWithValue("@State", stateName);
                sqlCommand.Parameters.AddWithValue("@RateNumber", DBNull.Value);
                //////////////////////////////////////////////////////
                //sqlCommand.CommandText = "CoralGables_Refresh";
                //sqlCommand.Parameters.AddWithValue("@customerID", customerID);
                //sqlCommand.Parameters.AddWithValue("@ZoneID", ZoneID);
                //sqlCommand.Parameters.AddWithValue("@MeterName", MeterName);
                //sqlCommand.Parameters.AddWithValue("@ParkingSpaceID", ParkingSpaceID);
                //sqlCommand.Parameters.AddWithValue("@PlateNumber", PlateNumber);
                //sqlCommand.Parameters.AddWithValue("@SpaceStatus", SpaceStatus);
                //sqlCommand.Parameters.AddWithValue("@State", stateName);
                ///////////////////////////////////////////////////////
                sqlCommand.Connection = connection;
                sqlCommand.CommandTimeout = 120;
                connection.Open();
                using (var reader = sqlCommand.ExecuteReader())
                {
                    while (reader.Read())
                    {
                        var item = new Transaction();
                        item.EnforcementKey = reader["EnforcementKey"] == System.DBNull.Value ? null : (string)reader["EnforcementKey"];
                        item.CustomerId = (reader.GetInt64(reader.GetOrdinal("CustomerId"))).ToString();
                        item.ParkingSpaceID = reader["ParkingSpaceID"] == System.DBNull.Value ? default(int) : (int)reader["ParkingSpaceID"];
                        item.PlateNumber = reader["PlateNumber"] == System.DBNull.Value ? null : (string)reader["PlateNumber"];
                        item.MeterName = null; // reader["MeterName"] == System.DBNull.Value ? null : (string)reader["MeterName"];
                        item.MeterId = null; // reader["meterid"] == System.DBNull.Value ? null : (string)reader["meterid"];
                        item.Latitude = reader["Latitude"] == System.DBNull.Value ? default(double?) : (double)reader["Latitude"];
                        item.Latitude = reader["Longitude"] == System.DBNull.Value ? default(double?) : (double)reader["Longitude"];

                        item.LPLocation = reader["LPLocation"] == System.DBNull.Value ? null : (string)reader["LPLocation"];
                        item.VMake = null;// reader["VMake"] == System.DBNull.Value ? null : (string)reader["VMake"];
                        item.VModel = null;// reader["VModel"] == System.DBNull.Value ? null : (string)reader["VModel"];
                        item.VColour = null;// reader["VColour"] == System.DBNull.Value ? null : (string)reader["VColour"];
                        item.MeterStreet = reader["MeterStreet"] == System.DBNull.Value ? null : (string)reader["MeterStreet"];
                        item.Location = reader["Location"] == System.DBNull.Value ? null : (string)reader["Location"];

                        item.EnfVendorStallId = reader["EnfVendorStallId"] == System.DBNull.Value ? default(int?) : (Int32)reader["EnfVendorStallId"];
                        item.PresentMeterTime = reader["PresentMeterTime"] == System.DBNull.Value ? default(DateTime?) : (DateTime)reader["PresentMeterTime"];
                        item.SpaceStatus = reader["SpaceStatus"] == System.DBNull.Value ? null : (string)reader["SpaceStatus"];
                        item.SensorEventTime = reader["SensorEventTime"] == System.DBNull.Value ? default(DateTime?) : (DateTime)reader["SensorEventTime"];
                        // item.ExpiredMinutes = reader["ExpiredMinutes"] == System.DBNull.Value ? default(int?) : (Int32)reader["ExpiredMinutes"];
                        item.ExpiredMinutes = reader["ExpiredMinutes"] == System.DBNull.Value ? -1 : (Int32)reader["ExpiredMinutes"];
                        item.AmountIncents = default(decimal?); //reader["AmountIncents"] == System.DBNull.Value ? default(decimal?) : (decimal)reader["AmountIncents"];
                        item.ExpiryTime = reader["ExpiryTime"] == System.DBNull.Value ? default(DateTime?) : (DateTime)reader["ExpiryTime"];
                        item.ExpiredMinutes = item.RemainingTime(4); // 5 mintues before + 4 == 9 (means end of 9 minutes 9.59 seconds)
                        item.TimeZoneId = reader["TimeZoneId"] == System.DBNull.Value ? default(int?) : (Int32)reader["TimeZoneId"];
                        //item.PayStationID = reader["PayStationID"] == System.DBNull.Value ? default(int?) : (Int32)reader["PayStationID"];
                        item.PayStationID = reader["meterid"] == System.DBNull.Value ? default(int?) : Convert.ToInt32(reader["meterid"]);
                        item.ZoneID = reader["ZoneID"] == System.DBNull.Value ? default(int?) : (Int32)reader["ZoneID"];
                        item.ZoneName = reader["ZoneName"] == System.DBNull.Value ? null : (string)reader["ZoneName"];
                        item.Block = reader["Block"] == System.DBNull.Value ? null : (string)reader["Block"];
                        item.Direction = reader["Direction"] == System.DBNull.Value ? null : (string)reader["Direction"];
                        item.MeterStreet = reader["MeterStreet"] == System.DBNull.Value ? null : (string)reader["MeterStreet"];
                        item.StreetType = reader["StreetType"] == System.DBNull.Value ? null : (string)reader["StreetType"];
                        item.MeterStreet = reader["MeterStreet"] == System.DBNull.Value ? null : (string)reader["MeterStreet"];
                        item.CrossStreet1 = reader["CrossStreet1"] == System.DBNull.Value ? null : (string)reader["CrossStreet1"];

                        item.CrossStreet2 = reader["CrossStreet2"] == System.DBNull.Value ? null : (string)reader["CrossStreet2"];
                        item.DigiELType = default(int?); //reader["DigiELType"] == System.DBNull.Value ? default(int?) : (Int32)reader["DigiELType"];
                        item.DigiErrorDescr = null; // reader["DigiErrorDescr"] == System.DBNull.Value ? null : (string)reader["DigiErrorDescr"];
                        item.GeneticELType = default(int?);  //reader["GeneticELType"] == System.DBNull.Value ? default(int?) : (Int32)reader["GeneticELType"];
                        item.GeneticErrorDescr = null; // reader["GeneticErrorDescr"] == System.DBNull.Value ? null : (string)reader["GeneticErrorDescr"];

                        if (reader["PMELType"] == System.DBNull.Value)
                        {
                            item.PMELType = default(int?);
                        }
                        else
                        {
                            string pMELTypeValue = reader["PMELType"].ToString();
                            if (string.IsNullOrWhiteSpace(pMELTypeValue))
                            {
                                item.PMELType = default(int?);
                            }
                            else
                            {
                                item.PMELType = (Int32)reader["PMELType"];
                            }
                        }



                        item.PMErrorDescr = null; //reader["PMErrorDescr"] == System.DBNull.Value ? null : (string)reader["PMErrorDescr"];
                        item.EnfHourDesc = reader["EnfHourDesc"] == System.DBNull.Value ? null : (string)reader["EnfHourDesc"];
                        item.MarkedSince = reader["MarkedSince"] == System.DBNull.Value ? default(DateTime?) : (DateTime)reader["MarkedSince"];
                        item.HitDescription = reader["HitDescription"] == System.DBNull.Value ? null : (string)reader["HitDescription"];


                        if (DataRecordExtensions.HasColumn(reader, "Metertype"))
                            item.METERLOC_METERTYPE = reader["Metertype"] == System.DBNull.Value ? null : (string)reader["Metertype"];
                        if (DataRecordExtensions.HasColumn(reader, "ZoneName"))
                            item.ZoneName = reader["ZoneName"] == System.DBNull.Value ? null : (string)reader["ZoneName"];

                        //if (DataRecordExtensions.HasColumn(reader, "MeterStatus"))
                        //    item.MeterStatus = reader["MeterStatus"] == System.DBNull.Value ? default(int?) : (Int32)reader["MeterStatus"];

                        validatedData.Add(item);
                    }
                }
            }
            catch (Exception ex)
            {
                throw;
            }
            finally
            {
                if (connection.State == ConnectionState.Open)
                    connection.Close();
            }
            return validatedData;
        }

        //public static List<Transaction> GetAtlantaCitationDataOthersWithPlateName(int customerID, string ZoneID, string MeterName, string ParkingSpaceID, string PlateNumber, string stateName, string SpaceStatus)
        //{
        //    List<Transaction> validatedData = new List<Transaction>();
        //    SqlConnection connection = null;
        //    try
        //    {
        //        connection = new SqlConnection(System.Configuration.ConfigurationManager.ConnectionStrings["Connector"].ToString());
        //        SqlCommand sqlCommand = new SqlCommand();
        //        sqlCommand.CommandType = CommandType.StoredProcedure;
        //        //sqlCommand.CommandText = "[SpacePlateTxnFinal_Railegh_Metername_Chicago_test]";
        //        sqlCommand.CommandText = "[RefreshService_Plate]";
        //        sqlCommand.Parameters.AddWithValue("@customerID", customerID);
        //        sqlCommand.Parameters.AddWithValue("@ZoneID", DBNull.Value);
        //        sqlCommand.Parameters.AddWithValue("@MeterName", MeterName);
        //        sqlCommand.Parameters.AddWithValue("@ParkingSpaceID", ParkingSpaceID);
        //        sqlCommand.Parameters.AddWithValue("@PlateNumber", PlateNumber);
        //        sqlCommand.Parameters.AddWithValue("@SpaceStatus", SpaceStatus);
        //        sqlCommand.Parameters.AddWithValue("@State", stateName);
        //        sqlCommand.Parameters.AddWithValue("@RateNumber", DBNull.Value);
        //        //////////////////////////////////////////////////////
        //        //sqlCommand.CommandText = "CoralGables_Refresh";
        //        //sqlCommand.Parameters.AddWithValue("@customerID", customerID);
        //        //sqlCommand.Parameters.AddWithValue("@ZoneID", ZoneID);
        //        //sqlCommand.Parameters.AddWithValue("@MeterName", MeterName);
        //        //sqlCommand.Parameters.AddWithValue("@ParkingSpaceID", ParkingSpaceID);
        //        //sqlCommand.Parameters.AddWithValue("@PlateNumber", PlateNumber);
        //        //sqlCommand.Parameters.AddWithValue("@SpaceStatus", SpaceStatus);
        //        //sqlCommand.Parameters.AddWithValue("@State", stateName);
        //        ///////////////////////////////////////////////////////
        //        sqlCommand.Connection = connection;
        //        sqlCommand.CommandTimeout = 120;
        //        connection.Open();
        //        using (var reader = sqlCommand.ExecuteReader())
        //        {
        //            while (reader.Read())
        //            {
        //                var item = new Transaction();
        //                item.EnforcementKey = reader["EnforcementKey"] == System.DBNull.Value ? null : (string)reader["EnforcementKey"];
        //                item.CustomerId = (reader.GetInt64(reader.GetOrdinal("CustomerId"))).ToString();
        //                item.ParkingSpaceID = reader["ParkingSpaceID"] == System.DBNull.Value ? default(int) : (int)reader["ParkingSpaceID"];
        //                item.PlateNumber = reader["PlateNumber"] == System.DBNull.Value ? null : (string)reader["PlateNumber"];
        //                item.MeterName = null; // reader["MeterName"] == System.DBNull.Value ? null : (string)reader["MeterName"];
        //                item.MeterId = null; // reader["meterid"] == System.DBNull.Value ? null : (string)reader["meterid"];
        //                item.Latitude = reader["Latitude"] == System.DBNull.Value ? default(double?) : (double)reader["Latitude"];
        //                item.Latitude = reader["Longitude"] == System.DBNull.Value ? default(double?) : (double)reader["Longitude"];

        //                item.LPLocation = reader["LPLocation"] == System.DBNull.Value ? null : (string)reader["LPLocation"];
        //                item.VMake = null;// reader["VMake"] == System.DBNull.Value ? null : (string)reader["VMake"];
        //                item.VModel = null;// reader["VModel"] == System.DBNull.Value ? null : (string)reader["VModel"];
        //                item.VColour = null;// reader["VColour"] == System.DBNull.Value ? null : (string)reader["VColour"];
        //                item.MeterStreet = reader["MeterStreet"] == System.DBNull.Value ? null : (string)reader["MeterStreet"];
        //                item.Location = reader["Location"] == System.DBNull.Value ? null : (string)reader["Location"];

        //                item.EnfVendorStallId = reader["EnfVendorStallId"] == System.DBNull.Value ? default(int?) : (Int32)reader["EnfVendorStallId"];
        //                item.PresentMeterTime = reader["PresentMeterTime"] == System.DBNull.Value ? default(DateTime?) : (DateTime)reader["PresentMeterTime"];
        //                item.SpaceStatus = reader["SpaceStatus"] == System.DBNull.Value ? null : (string)reader["SpaceStatus"];
        //                item.SensorEventTime = reader["SensorEventTime"] == System.DBNull.Value ? default(DateTime?) : (DateTime)reader["SensorEventTime"];
        //                // item.ExpiredMinutes = reader["ExpiredMinutes"] == System.DBNull.Value ? default(int?) : (Int32)reader["ExpiredMinutes"];
        //                item.ExpiredMinutes = reader["ExpiredMinutes"] == System.DBNull.Value ? -1 : (Int32)reader["ExpiredMinutes"];
        //                item.AmountIncents = default(decimal?); //reader["AmountIncents"] == System.DBNull.Value ? default(decimal?) : (decimal)reader["AmountIncents"];
        //                item.ExpiryTime = reader["ExpiryTime"] == System.DBNull.Value ? default(DateTime?) : (DateTime)reader["ExpiryTime"];
        //                item.TimeZoneId = reader["TimeZoneId"] == System.DBNull.Value ? default(int?) : (Int32)reader["TimeZoneId"];
        //                //item.PayStationID = reader["PayStationID"] == System.DBNull.Value ? default(int?) : (Int32)reader["PayStationID"];
        //                item.PayStationID = reader["meterid"] == System.DBNull.Value ? default(int?) : Convert.ToInt32(reader["meterid"]);
        //                item.ZoneID = reader["ZoneID"] == System.DBNull.Value ? default(int?) : (Int32)reader["ZoneID"];
        //                item.ZoneName = reader["ZoneName"] == System.DBNull.Value ? null : (string)reader["ZoneName"];
        //                item.Block = reader["Block"] == System.DBNull.Value ? null : (string)reader["Block"];
        //                item.Direction = reader["Direction"] == System.DBNull.Value ? null : (string)reader["Direction"];
        //                item.MeterStreet = reader["MeterStreet"] == System.DBNull.Value ? null : (string)reader["MeterStreet"];
        //                item.StreetType = reader["StreetType"] == System.DBNull.Value ? null : (string)reader["StreetType"];
        //                item.MeterStreet = reader["MeterStreet"] == System.DBNull.Value ? null : (string)reader["MeterStreet"];
        //                item.CrossStreet1 = reader["CrossStreet1"] == System.DBNull.Value ? null : (string)reader["CrossStreet1"];

        //                item.CrossStreet2 = reader["CrossStreet2"] == System.DBNull.Value ? null : (string)reader["CrossStreet2"];
        //                item.DigiELType = default(int?); //reader["DigiELType"] == System.DBNull.Value ? default(int?) : (Int32)reader["DigiELType"];
        //                item.DigiErrorDescr = null; // reader["DigiErrorDescr"] == System.DBNull.Value ? null : (string)reader["DigiErrorDescr"];
        //                item.GeneticELType = default(int?);  //reader["GeneticELType"] == System.DBNull.Value ? default(int?) : (Int32)reader["GeneticELType"];
        //                item.GeneticErrorDescr = null; // reader["GeneticErrorDescr"] == System.DBNull.Value ? null : (string)reader["GeneticErrorDescr"];

        //                if (reader["PMELType"] == System.DBNull.Value)
        //                {
        //                    item.PMELType = default(int?);
        //                }
        //                else
        //                {
        //                    string pMELTypeValue = reader["PMELType"].ToString();
        //                    if (string.IsNullOrWhiteSpace(pMELTypeValue))
        //                    {
        //                        item.PMELType = default(int?);
        //                    }
        //                    else
        //                    {
        //                        item.PMELType = (Int32)reader["PMELType"];
        //                    }
        //                }



        //                item.PMErrorDescr = null; //reader["PMErrorDescr"] == System.DBNull.Value ? null : (string)reader["PMErrorDescr"];
        //                item.EnfHourDesc = reader["EnfHourDesc"] == System.DBNull.Value ? null : (string)reader["EnfHourDesc"];
        //                item.MarkedSince = reader["MarkedSince"] == System.DBNull.Value ? default(DateTime?) : (DateTime)reader["MarkedSince"];
        //                item.HitDescription = reader["HitDescription"] == System.DBNull.Value ? null : (string)reader["HitDescription"];


        //                if (DataRecordExtensions.HasColumn(reader, "Metertype"))
        //                    item.METERLOC_METERTYPE = reader["Metertype"] == System.DBNull.Value ? null : (string)reader["Metertype"];
        //                if (DataRecordExtensions.HasColumn(reader, "ZoneName"))
        //                    item.ZoneName = reader["ZoneName"] == System.DBNull.Value ? null : (string)reader["ZoneName"];

        //                //if (DataRecordExtensions.HasColumn(reader, "MeterStatus"))
        //                //    item.MeterStatus = reader["MeterStatus"] == System.DBNull.Value ? default(int?) : (Int32)reader["MeterStatus"];

        //                validatedData.Add(item);
        //            }
        //        }
        //    }
        //    catch (Exception ex)
        //    {
        //        throw;
        //    }
        //    finally
        //    {
        //        if (connection.State == ConnectionState.Open)
        //            connection.Close();
        //    }
        //    return validatedData;
        //}
        #endregion

        #region Chicago
        public static List<Transaction> GetCitationDataOthersWithPlateNameChicago(int customerID, string ZoneID, string MeterName, string ParkingSpaceID, string PlateNumber, string SpaceStatus, string State, string RateNumber)
        {
            List<Transaction> validatedData = new List<Transaction>();
            SqlConnection connection = null;
            try
            {
                connection = new SqlConnection(System.Configuration.ConfigurationManager.ConnectionStrings["Connector"].ToString());
                SqlCommand sqlCommand = new SqlCommand();
                sqlCommand.CommandType = CommandType.StoredProcedure;
                //sqlCommand.CommandText = "SpacePlateTxnFinal_railegh";
                sqlCommand.CommandText = "[SpacePlateTxnFinal_Railegh_Metername_Chicago_test]";

                sqlCommand.Parameters.AddWithValue("@customerID", customerID);
                sqlCommand.Parameters.AddWithValue("@ZoneID", ZoneID);
                sqlCommand.Parameters.AddWithValue("@MeterName", MeterName);
                sqlCommand.Parameters.AddWithValue("@ParkingSpaceID", ParkingSpaceID);
                sqlCommand.Parameters.AddWithValue("@PlateNumber", PlateNumber);
                sqlCommand.Parameters.AddWithValue("@SpaceStatus", DBNull.Value);

                //sqlCommand.Parameters.AddWithValue("@ReceiptId", ReceiptId);
                sqlCommand.Parameters.AddWithValue("@State", State);
                sqlCommand.Parameters.AddWithValue("@RateNumber", DBNull.Value);

                sqlCommand.Connection = connection;
                sqlCommand.CommandTimeout = 120;
                connection.Open();
                using (var reader = sqlCommand.ExecuteReader())
                {
                    while (reader.Read())
                    {
                        var item = new Transaction();
                        item.EnforcementKey = reader["EnforcementKey"] == System.DBNull.Value ? null : (string)reader["EnforcementKey"];
                        item.CustomerId = (reader.GetInt64(reader.GetOrdinal("CustomerId"))).ToString();
                        item.ParkingSpaceID = reader["ParkingSpaceID"] == System.DBNull.Value ? default(int) : (int)reader["ParkingSpaceID"];
                        item.PlateNumber = reader["PlateNumber"] == System.DBNull.Value ? null : (string)reader["PlateNumber"];
                        item.MeterName = reader["MeterName"] == System.DBNull.Value ? null : (string)reader["MeterName"];
                        item.MeterId = reader["meterid"] == System.DBNull.Value ? null : (string)reader["meterid"];
                        item.Latitude = reader["Latitude"] == System.DBNull.Value ? default(double?) : (double)reader["Latitude"];
                        item.Latitude = reader["Longitude"] == System.DBNull.Value ? default(double?) : (double)reader["Longitude"];

                        item.LPLocation = reader["LPLocation"] == System.DBNull.Value ? null : (string)reader["LPLocation"];
                        item.VMake = null; // reader["VMake"] == System.DBNull.Value ? null : (string)reader["VMake"];
                        item.VModel = null; //reader["VModel"] == System.DBNull.Value ? null : (string)reader["VModel"];
                        item.VColour = null; //reader["VColour"] == System.DBNull.Value ? null : (string)reader["VColour"];
                        item.MeterStreet = reader["MeterStreet"] == System.DBNull.Value ? null : (string)reader["MeterStreet"];
                        item.Location = reader["Location"] == System.DBNull.Value ? null : (string)reader["Location"];

                        item.EnfVendorStallId = reader["EnfVendorStallId"] == System.DBNull.Value ? default(int?) : (Int32)reader["EnfVendorStallId"];
                        item.PresentMeterTime = reader["PresentMeterTime"] == System.DBNull.Value ? default(DateTime?) : (DateTime)reader["PresentMeterTime"];
                        item.SpaceStatus = reader["SpaceStatus"] == System.DBNull.Value ? null : (string)reader["SpaceStatus"];
                        item.SensorEventTime = reader["SensorEventTime"] == System.DBNull.Value ? default(DateTime?) : (DateTime)reader["SensorEventTime"];
                        //item.ExpiredMinutes = reader["ExpiredMinutes"] == System.DBNull.Value ? default(int?) : (Int32)reader["ExpiredMinutes"];
                        item.ExpiredMinutes = reader["ExpiredMinutes"] == System.DBNull.Value ? -1 : (Int32)reader["ExpiredMinutes"];
                        item.AmountIncents = default(decimal?); // reader["AmountIncents"] == System.DBNull.Value ? default(decimal?) : (decimal)reader["AmountIncents"];
                        item.ExpiryTime = reader["ExpiryTime"] == System.DBNull.Value ? default(DateTime?) : (DateTime)reader["ExpiryTime"];
                        item.TimeZoneId = reader["TimeZoneId"] == System.DBNull.Value ? default(int?) : (Int32)reader["TimeZoneId"];
                        //item.PayStationID = reader["PayStationID"] == System.DBNull.Value ? default(int?) : (Int32)reader["PayStationID"];
                        item.PayStationID = reader["meterid"] == System.DBNull.Value ? default(int?) : Convert.ToInt32(reader["meterid"]);
                        item.ZoneID = reader["ZoneID"] == System.DBNull.Value ? default(int?) : (Int32)reader["ZoneID"];
                        item.ZoneName = reader["ZoneName"] == System.DBNull.Value ? null : (string)reader["ZoneName"];
                        item.Block = reader["Block"] == System.DBNull.Value ? null : (string)reader["Block"];
                        item.Direction = reader["Direction"] == System.DBNull.Value ? null : (string)reader["Direction"];
                        item.MeterStreet = reader["MeterStreet"] == System.DBNull.Value ? null : (string)reader["MeterStreet"];
                        item.StreetType = reader["StreetType"] == System.DBNull.Value ? null : (string)reader["StreetType"];
                        item.MeterStreet = reader["MeterStreet"] == System.DBNull.Value ? null : (string)reader["MeterStreet"];
                        item.CrossStreet1 = reader["CrossStreet1"] == System.DBNull.Value ? null : (string)reader["CrossStreet1"];

                        item.CrossStreet2 = reader["CrossStreet2"] == System.DBNull.Value ? null : (string)reader["CrossStreet2"];
                        item.DigiELType = default(int?); //reader["DigiELType"] == System.DBNull.Value ? default(int?) : (Int32)reader["DigiELType"];
                        item.DigiErrorDescr = null; // reader["DigiErrorDescr"] == System.DBNull.Value ? null : (string)reader["DigiErrorDescr"];
                        item.GeneticELType = default(int?); // reader["GeneticELType"] == System.DBNull.Value ? default(int?) : (Int32)reader["GeneticELType"];
                        item.GeneticErrorDescr = null; //reader["GeneticErrorDescr"] == System.DBNull.Value ? null : (string)reader["GeneticErrorDescr"];

                        if (reader["PMELType"] == System.DBNull.Value)
                        {
                            item.PMELType = default(int?);
                        }
                        else
                        {
                            string pMELTypeValue = reader["PMELType"].ToString();
                            if (string.IsNullOrWhiteSpace(pMELTypeValue))
                            {
                                item.PMELType = default(int?);
                            }
                            else
                            {
                                item.PMELType = (Int32)reader["PMELType"];
                            }
                        }



                        item.PMErrorDescr = null; //reader["PMErrorDescr"] == System.DBNull.Value ? null : (string)reader["PMErrorDescr"];
                        item.EnfHourDesc = reader["EnfHourDesc"] == System.DBNull.Value ? null : (string)reader["EnfHourDesc"];
                        item.MarkedSince = reader["MarkedSince"] == System.DBNull.Value ? default(DateTime?) : (DateTime)reader["MarkedSince"];
                        item.HitDescription = reader["HitDescription"] == System.DBNull.Value ? null : (string)reader["HitDescription"];


                        if (DataRecordExtensions.HasColumn(reader, "Metertype"))
                            item.METERLOC_METERTYPE = reader["Metertype"] == System.DBNull.Value ? null : (string)reader["Metertype"];
                        if (DataRecordExtensions.HasColumn(reader, "ZoneName"))
                            item.ZoneName = reader["ZoneName"] == System.DBNull.Value ? null : (string)reader["ZoneName"];


                        if (DataRecordExtensions.HasColumn(reader, "Recieptid"))
                            item.Recieptid = reader["Recieptid"] == System.DBNull.Value ? null : (string)reader["Recieptid"];

                        if (DataRecordExtensions.HasColumn(reader, "RateNumber"))
                            item.RateNumber = reader["RateNumber"] == System.DBNull.Value ? null : (string)reader["RateNumber"];


                        validatedData.Add(item);
                    }
                }
            }
            catch (Exception ex)
            {
                throw;
            }
            finally
            {
                if (connection.State == ConnectionState.Open)
                    connection.Close();
            }
            return validatedData;
        }
        #endregion

        #region SouthMiami
        public static List<Transaction> GetSouthMiamiCitationData(int customerID, string ZoneID, string MeterName, string ParkingSpaceID, string PlateNumber, string SpaceStatus)
        {
            List<Transaction> validatedData = new List<Transaction>();
            SqlConnection connection = null;
            try
            {
                connection = new SqlConnection(System.Configuration.ConfigurationManager.ConnectionStrings["Connector"].ToString());
                SqlCommand sqlCommand = new SqlCommand();
                sqlCommand.CommandType = CommandType.StoredProcedure;
                //sqlCommand.CommandText = "SpacePlateTxnFinal";//Original
                sqlCommand.CommandText = "SpacePlateTxnFinal_SouthMiami"; //new testing
                sqlCommand.Parameters.AddWithValue("@customerID", customerID);
                sqlCommand.Parameters.AddWithValue("@ZoneID", ZoneID);
                //sqlCommand.Parameters.AddWithValue("@MeterName", MeterName);
                sqlCommand.Parameters.AddWithValue("@MeterName", MeterName);
                sqlCommand.Parameters.AddWithValue("@ParkingSpaceID", ParkingSpaceID);
                sqlCommand.Parameters.AddWithValue("@PlateNumber", PlateNumber);
                sqlCommand.Parameters.AddWithValue("@SpaceStatus", SpaceStatus);
                sqlCommand.Connection = connection;
                sqlCommand.CommandTimeout = 120;
                connection.Open();
                using (var reader = sqlCommand.ExecuteReader())
                {
                    while (reader.Read())
                    {
                        var item = new Transaction();
                        item.EnforcementKey = reader["EnforcementKey"] == System.DBNull.Value ? null : (string)reader["EnforcementKey"];
                        item.CustomerId = (reader.GetInt64(reader.GetOrdinal("CustomerId"))).ToString();
                        item.ParkingSpaceID = reader["ParkingSpaceID"] == System.DBNull.Value ? default(int) : (int)reader["ParkingSpaceID"];
                        item.PlateNumber = reader["PlateNumber"] == System.DBNull.Value ? null : (string)reader["PlateNumber"];
                        item.MeterName = reader["MeterName"] == System.DBNull.Value ? null : (string)reader["MeterName"];

                        item.Latitude = reader["Latitude"] == System.DBNull.Value ? default(double?) : (double)reader["Latitude"];
                        item.Latitude = reader["Longitude"] == System.DBNull.Value ? default(double?) : (double)reader["Longitude"];

                        item.LPLocation = reader["LPLocation"] == System.DBNull.Value ? null : (string)reader["LPLocation"];
                        item.VMake = reader["VMake"] == System.DBNull.Value ? null : (string)reader["VMake"];
                        item.VModel = reader["VModel"] == System.DBNull.Value ? null : (string)reader["VModel"];
                        item.VColour = reader["VColour"] == System.DBNull.Value ? null : (string)reader["VColour"];
                        item.MeterStreet = reader["MeterStreet"] == System.DBNull.Value ? null : (string)reader["MeterStreet"];
                        item.Location = reader["Location"] == System.DBNull.Value ? null : (string)reader["Location"];

                        item.EnfVendorStallId = reader["EnfVendorStallId"] == System.DBNull.Value ? default(int?) : (Int32)reader["EnfVendorStallId"];
                        item.PresentMeterTime = reader["PresentMeterTime"] == System.DBNull.Value ? default(DateTime?) : (DateTime)reader["PresentMeterTime"];
                        item.SpaceStatus = reader["SpaceStatus"] == System.DBNull.Value ? null : (string)reader["SpaceStatus"];
                        item.SensorEventTime = reader["SensorEventTime"] == System.DBNull.Value ? default(DateTime?) : (DateTime)reader["SensorEventTime"];
                        //item.ExpiredMinutes = reader["ExpiredMinutes"] == System.DBNull.Value ? default(int?) : (Int32)reader["ExpiredMinutes"];
                        item.ExpiredMinutes = reader["ExpiredMinutes"] == System.DBNull.Value ? -1 : (Int32)reader["ExpiredMinutes"];
                        item.AmountIncents = reader["AmountIncents"] == System.DBNull.Value ? default(decimal?) : (decimal)reader["AmountIncents"];
                        item.ExpiryTime = reader["ExpiryTime"] == System.DBNull.Value ? default(DateTime?) : (DateTime)reader["ExpiryTime"];
                        item.TimeZoneId = reader["TimeZoneId"] == System.DBNull.Value ? default(int?) : (Int32)reader["TimeZoneId"];
                        item.PayStationID = reader["PayStationID"] == System.DBNull.Value ? default(int?) : (Int32)reader["PayStationID"];
                        item.ZoneID = reader["ZoneID"] == System.DBNull.Value ? default(int?) : (Int32)reader["ZoneID"];
                        //item.ZoneName = reader["ZoneName"] == System.DBNull.Value ? null : (string)reader["ZoneName"];
                        item.ZoneName = null;
                        item.Block = reader["Block"] == System.DBNull.Value ? null : (string)reader["Block"];
                        item.Direction = reader["Direction"] == System.DBNull.Value ? null : (string)reader["Direction"];
                        item.MeterStreet = reader["MeterStreet"] == System.DBNull.Value ? null : (string)reader["MeterStreet"];
                        item.StreetType = reader["StreetType"] == System.DBNull.Value ? null : (string)reader["StreetType"];
                        item.MeterStreet = reader["MeterStreet"] == System.DBNull.Value ? null : (string)reader["MeterStreet"];
                        item.CrossStreet1 = reader["CrossStreet1"] == System.DBNull.Value ? null : (string)reader["CrossStreet1"];

                        item.CrossStreet2 = reader["CrossStreet2"] == System.DBNull.Value ? null : (string)reader["CrossStreet2"];
                        item.DigiELType = reader["DigiELType"] == System.DBNull.Value ? default(int?) : (Int32)reader["DigiELType"];
                        item.DigiErrorDescr = reader["DigiErrorDescr"] == System.DBNull.Value ? null : (string)reader["DigiErrorDescr"];
                        item.GeneticELType = reader["GeneticELType"] == System.DBNull.Value ? default(int?) : (Int32)reader["GeneticELType"];
                        item.GeneticErrorDescr = reader["GeneticErrorDescr"] == System.DBNull.Value ? null : (string)reader["GeneticErrorDescr"];
                        item.PMELType = reader["PMELType"] == System.DBNull.Value ? default(int?) : (Int32)reader["PMELType"];
                        item.PMErrorDescr = reader["PMErrorDescr"] == System.DBNull.Value ? null : (string)reader["PMErrorDescr"];
                        item.EnfHourDesc = reader["EnfHourDesc"] == System.DBNull.Value ? null : (string)reader["EnfHourDesc"];
                        item.MarkedSince = reader["MarkedSince"] == System.DBNull.Value ? default(DateTime?) : (DateTime)reader["MarkedSince"];
                        item.HitDescription = reader["HitDescription"] == System.DBNull.Value ? null : (string)reader["HitDescription"];
                        validatedData.Add(item);
                    }
                }
            }
            catch (Exception ex)
            {
                throw;
            }
            finally
            {
                if (connection.State == ConnectionState.Open)
                    connection.Close();
            }
            return validatedData;
        }
        #endregion

        #region Raleigh
        public static List<Transaction> GetCitationDataRaleigh(int customerID, string ZoneID, string MeterName, string ParkingSpaceID, string PlateNumber, string SpaceStatus)
        {
            List<Transaction> validatedData = new List<Transaction>();
            SqlConnection connection = null;
            try
            {
                connection = new SqlConnection(System.Configuration.ConfigurationManager.ConnectionStrings["Connector"].ToString());
                SqlCommand sqlCommand = new SqlCommand();
                sqlCommand.CommandType = CommandType.StoredProcedure;
                //sqlCommand.CommandText = "SpacePlateTxnFinal_railegh";
                sqlCommand.CommandText = "[SpacePlateTxnFinal_Railegh_Metername_Test]";
                sqlCommand.Parameters.AddWithValue("@customerID", customerID);
                sqlCommand.Parameters.AddWithValue("@ZoneID", ZoneID);
                sqlCommand.Parameters.AddWithValue("@MeterName", MeterName);
                sqlCommand.Parameters.AddWithValue("@ParkingSpaceID", ParkingSpaceID);
                sqlCommand.Parameters.AddWithValue("@PlateNumber", PlateNumber);
                sqlCommand.Parameters.AddWithValue("@SpaceStatus", SpaceStatus);
                sqlCommand.Connection = connection;
                sqlCommand.CommandTimeout = 120;
                connection.Open();
                using (var reader = sqlCommand.ExecuteReader())
                {
                    while (reader.Read())
                    {
                        var item = new Transaction();
                        item.EnforcementKey = reader["EnforcementKey"] == System.DBNull.Value ? null : (string)reader["EnforcementKey"];
                        item.CustomerId = (reader.GetInt64(reader.GetOrdinal("CustomerId"))).ToString();
                        item.ParkingSpaceID = reader["ParkingSpaceID"] == System.DBNull.Value ? default(int) : (int)reader["ParkingSpaceID"];
                        item.PlateNumber = reader["PlateNumber"] == System.DBNull.Value ? null : (string)reader["PlateNumber"];
                        item.MeterName = reader["MeterName"] == System.DBNull.Value ? null : (string)reader["MeterName"];
                        item.MeterId = reader["meterid"] == System.DBNull.Value ? null : (string)reader["meterid"];
                        item.Latitude = reader["Latitude"] == System.DBNull.Value ? default(double?) : (double)reader["Latitude"];
                        item.Latitude = reader["Longitude"] == System.DBNull.Value ? default(double?) : (double)reader["Longitude"];

                        item.LPLocation = reader["LPLocation"] == System.DBNull.Value ? null : (string)reader["LPLocation"];
                        item.VMake = reader["VMake"] == System.DBNull.Value ? null : (string)reader["VMake"];
                        item.VModel = reader["VModel"] == System.DBNull.Value ? null : (string)reader["VModel"];
                        item.VColour = reader["VColour"] == System.DBNull.Value ? null : (string)reader["VColour"];
                        item.MeterStreet = reader["MeterStreet"] == System.DBNull.Value ? null : (string)reader["MeterStreet"];
                        item.Location = reader["Location"] == System.DBNull.Value ? null : (string)reader["Location"];

                        item.EnfVendorStallId = reader["EnfVendorStallId"] == System.DBNull.Value ? default(int?) : (Int32)reader["EnfVendorStallId"];
                        item.PresentMeterTime = reader["PresentMeterTime"] == System.DBNull.Value ? default(DateTime?) : (DateTime)reader["PresentMeterTime"];
                        item.SpaceStatus = reader["SpaceStatus"] == System.DBNull.Value ? null : (string)reader["SpaceStatus"];
                        item.SensorEventTime = reader["SensorEventTime"] == System.DBNull.Value ? default(DateTime?) : (DateTime)reader["SensorEventTime"];
                        //item.ExpiredMinutes = reader["ExpiredMinutes"] == System.DBNull.Value ? default(int?) : (Int32)reader["ExpiredMinutes"];
                        item.ExpiredMinutes = reader["ExpiredMinutes"] == System.DBNull.Value ? -1 : (Int32)reader["ExpiredMinutes"];
                        item.AmountIncents = reader["AmountIncents"] == System.DBNull.Value ? default(decimal?) : (decimal)reader["AmountIncents"];
                        item.ExpiryTime = reader["ExpiryTime"] == System.DBNull.Value ? default(DateTime?) : (DateTime)reader["ExpiryTime"];
                        item.TimeZoneId = reader["TimeZoneId"] == System.DBNull.Value ? default(int?) : (Int32)reader["TimeZoneId"];
                        //item.PayStationID = reader["PayStationID"] == System.DBNull.Value ? default(int?) : (Int32)reader["PayStationID"];
                        item.PayStationID = reader["meterid"] == System.DBNull.Value ? default(int?) : Convert.ToInt32(reader["meterid"]);
                        item.ZoneID = reader["ZoneID"] == System.DBNull.Value ? default(int?) : (Int32)reader["ZoneID"];
                        item.ZoneName = reader["ZoneName"] == System.DBNull.Value ? null : (string)reader["ZoneName"];
                        item.Block = reader["Block"] == System.DBNull.Value ? null : (string)reader["Block"];
                        item.Direction = reader["Direction"] == System.DBNull.Value ? null : (string)reader["Direction"];
                        item.MeterStreet = reader["MeterStreet"] == System.DBNull.Value ? null : (string)reader["MeterStreet"];
                        item.StreetType = reader["StreetType"] == System.DBNull.Value ? null : (string)reader["StreetType"];
                        item.MeterStreet = reader["MeterStreet"] == System.DBNull.Value ? null : (string)reader["MeterStreet"];
                        item.CrossStreet1 = reader["CrossStreet1"] == System.DBNull.Value ? null : (string)reader["CrossStreet1"];

                        item.CrossStreet2 = reader["CrossStreet2"] == System.DBNull.Value ? null : (string)reader["CrossStreet2"];
                        item.DigiELType = reader["DigiELType"] == System.DBNull.Value ? default(int?) : (Int32)reader["DigiELType"];
                        item.DigiErrorDescr = reader["DigiErrorDescr"] == System.DBNull.Value ? null : (string)reader["DigiErrorDescr"];
                        item.GeneticELType = reader["GeneticELType"] == System.DBNull.Value ? default(int?) : (Int32)reader["GeneticELType"];
                        item.GeneticErrorDescr = reader["GeneticErrorDescr"] == System.DBNull.Value ? null : (string)reader["GeneticErrorDescr"];

                        if (reader["PMELType"] == System.DBNull.Value)
                        {
                            item.PMELType = default(int?);
                        }
                        else
                        {
                            string pMELTypeValue = reader["PMELType"].ToString();
                            if (string.IsNullOrWhiteSpace(pMELTypeValue))
                            {
                                item.PMELType = default(int?);
                            }
                            else
                            {
                                item.PMELType = (Int32)reader["PMELType"];
                            }
                        }



                        item.PMErrorDescr = reader["PMErrorDescr"] == System.DBNull.Value ? null : (string)reader["PMErrorDescr"];
                        item.EnfHourDesc = reader["EnfHourDesc"] == System.DBNull.Value ? null : (string)reader["EnfHourDesc"];
                        item.MarkedSince = reader["MarkedSince"] == System.DBNull.Value ? default(DateTime?) : (DateTime)reader["MarkedSince"];
                        item.HitDescription = reader["HitDescription"] == System.DBNull.Value ? null : (string)reader["HitDescription"];


                        if (DataRecordExtensions.HasColumn(reader, "Metertype"))
                            item.METERLOC_METERTYPE = reader["Metertype"] == System.DBNull.Value ? null : (string)reader["Metertype"];
                        if (DataRecordExtensions.HasColumn(reader, "ZoneName"))
                            item.ZoneName = reader["ZoneName"] == System.DBNull.Value ? null : (string)reader["ZoneName"];

                        //if (DataRecordExtensions.HasColumn(reader, "MeterStatus"))
                        //    item.MeterStatus = reader["MeterStatus"] == System.DBNull.Value ? default(int?) : (Int32)reader["MeterStatus"];

                        validatedData.Add(item);
                    }
                }
            }
            catch (Exception ex)
            {
                throw;
            }
            finally
            {
                if (connection.State == ConnectionState.Open)
                    connection.Close();
            }
            return validatedData;
        }
        #endregion

        #region BirminghamMI
        public static List<Transaction> GetCitationDataBirminghamMI(int customerID, string ZoneID, string MeterName, string ParkingSpaceID, string PlateNumber, string SpaceStatus)
        {
            List<Transaction> validatedData = new List<Transaction>();
            SqlConnection connection = null;
            try
            {
                connection = new SqlConnection(System.Configuration.ConfigurationManager.ConnectionStrings["Connector"].ToString());
                SqlCommand sqlCommand = new SqlCommand();
                sqlCommand.CommandType = CommandType.StoredProcedure;
                //sqlCommand.CommandText = "SpacePlateTxnFinal_railegh";
                sqlCommand.CommandText = "[SpacePlateTxnFinal_Railegh_Metername_Test]";
                sqlCommand.Parameters.AddWithValue("@customerID", customerID);
                sqlCommand.Parameters.AddWithValue("@ZoneID", ZoneID);
                sqlCommand.Parameters.AddWithValue("@MeterName", MeterName);
                sqlCommand.Parameters.AddWithValue("@ParkingSpaceID", ParkingSpaceID);
                sqlCommand.Parameters.AddWithValue("@PlateNumber", PlateNumber);
                sqlCommand.Parameters.AddWithValue("@SpaceStatus", SpaceStatus);
                sqlCommand.Connection = connection;
                sqlCommand.CommandTimeout = 120;
                connection.Open();
                using (var reader = sqlCommand.ExecuteReader())
                {
                    while (reader.Read())
                    {
                        var item = new Transaction();
                        item.EnforcementKey = reader["EnforcementKey"] == System.DBNull.Value ? null : (string)reader["EnforcementKey"];
                        item.CustomerId = (reader.GetInt64(reader.GetOrdinal("CustomerId"))).ToString();
                        item.ParkingSpaceID = reader["ParkingSpaceID"] == System.DBNull.Value ? default(int) : (int)reader["ParkingSpaceID"];
                        item.PlateNumber = reader["PlateNumber"] == System.DBNull.Value ? null : (string)reader["PlateNumber"];
                        item.MeterName = reader["MeterName"] == System.DBNull.Value ? null : (string)reader["MeterName"];
                        item.MeterId = reader["meterid"] == System.DBNull.Value ? null : (string)reader["meterid"];
                        item.Latitude = reader["Latitude"] == System.DBNull.Value ? default(double?) : (double)reader["Latitude"];
                        item.Latitude = reader["Longitude"] == System.DBNull.Value ? default(double?) : (double)reader["Longitude"];

                        item.LPLocation = reader["LPLocation"] == System.DBNull.Value ? null : (string)reader["LPLocation"];
                        item.VMake = reader["VMake"] == System.DBNull.Value ? null : (string)reader["VMake"];
                        item.VModel = reader["VModel"] == System.DBNull.Value ? null : (string)reader["VModel"];
                        item.VColour = reader["VColour"] == System.DBNull.Value ? null : (string)reader["VColour"];
                        item.MeterStreet = reader["MeterStreet"] == System.DBNull.Value ? null : (string)reader["MeterStreet"];
                        item.Location = reader["Location"] == System.DBNull.Value ? null : (string)reader["Location"];

                        item.EnfVendorStallId = reader["EnfVendorStallId"] == System.DBNull.Value ? default(int?) : (Int32)reader["EnfVendorStallId"];
                        item.PresentMeterTime = reader["PresentMeterTime"] == System.DBNull.Value ? default(DateTime?) : (DateTime)reader["PresentMeterTime"];
                        item.SpaceStatus = reader["SpaceStatus"] == System.DBNull.Value ? null : (string)reader["SpaceStatus"];
                        item.SensorEventTime = reader["SensorEventTime"] == System.DBNull.Value ? default(DateTime?) : (DateTime)reader["SensorEventTime"];
                        // item.ExpiredMinutes = reader["ExpiredMinutes"] == System.DBNull.Value ? default(int?) : (Int32)reader["ExpiredMinutes"];
                        item.ExpiredMinutes = reader["ExpiredMinutes"] == System.DBNull.Value ? -1 : (Int32)reader["ExpiredMinutes"];
                        item.AmountIncents = reader["AmountIncents"] == System.DBNull.Value ? default(decimal?) : (decimal)reader["AmountIncents"];
                        item.ExpiryTime = reader["ExpiryTime"] == System.DBNull.Value ? default(DateTime?) : (DateTime)reader["ExpiryTime"];
                        item.TimeZoneId = reader["TimeZoneId"] == System.DBNull.Value ? default(int?) : (Int32)reader["TimeZoneId"];
                        //item.PayStationID = reader["PayStationID"] == System.DBNull.Value ? default(int?) : (Int32)reader["PayStationID"];
                        item.PayStationID = reader["meterid"] == System.DBNull.Value ? default(int?) : Convert.ToInt32(reader["meterid"]);
                        item.ZoneID = reader["ZoneID"] == System.DBNull.Value ? default(int?) : (Int32)reader["ZoneID"];
                        item.ZoneName = reader["ZoneName"] == System.DBNull.Value ? null : (string)reader["ZoneName"];
                        item.Block = reader["Block"] == System.DBNull.Value ? null : (string)reader["Block"];
                        item.Direction = reader["Direction"] == System.DBNull.Value ? null : (string)reader["Direction"];
                        item.MeterStreet = reader["MeterStreet"] == System.DBNull.Value ? null : (string)reader["MeterStreet"];
                        item.StreetType = reader["StreetType"] == System.DBNull.Value ? null : (string)reader["StreetType"];
                        item.MeterStreet = reader["MeterStreet"] == System.DBNull.Value ? null : (string)reader["MeterStreet"];
                        item.CrossStreet1 = reader["CrossStreet1"] == System.DBNull.Value ? null : (string)reader["CrossStreet1"];

                        item.CrossStreet2 = reader["CrossStreet2"] == System.DBNull.Value ? null : (string)reader["CrossStreet2"];
                        item.DigiELType = reader["DigiELType"] == System.DBNull.Value ? default(int?) : (Int32)reader["DigiELType"];
                        item.DigiErrorDescr = reader["DigiErrorDescr"] == System.DBNull.Value ? null : (string)reader["DigiErrorDescr"];
                        item.GeneticELType = reader["GeneticELType"] == System.DBNull.Value ? default(int?) : (Int32)reader["GeneticELType"];
                        item.GeneticErrorDescr = reader["GeneticErrorDescr"] == System.DBNull.Value ? null : (string)reader["GeneticErrorDescr"];

                        if (reader["PMELType"] == System.DBNull.Value)
                        {
                            item.PMELType = default(int?);
                        }
                        else
                        {
                            string pMELTypeValue = reader["PMELType"].ToString();
                            if (string.IsNullOrWhiteSpace(pMELTypeValue))
                            {
                                item.PMELType = default(int?);
                            }
                            else
                            {
                                item.PMELType = (Int32)reader["PMELType"];
                            }
                        }



                        item.PMErrorDescr = reader["PMErrorDescr"] == System.DBNull.Value ? null : (string)reader["PMErrorDescr"];
                        item.EnfHourDesc = reader["EnfHourDesc"] == System.DBNull.Value ? null : (string)reader["EnfHourDesc"];
                        item.MarkedSince = reader["MarkedSince"] == System.DBNull.Value ? default(DateTime?) : (DateTime)reader["MarkedSince"];
                        item.HitDescription = reader["HitDescription"] == System.DBNull.Value ? null : (string)reader["HitDescription"];


                        if (DataRecordExtensions.HasColumn(reader, "Metertype"))
                            item.METERLOC_METERTYPE = reader["Metertype"] == System.DBNull.Value ? null : (string)reader["Metertype"];
                        if (DataRecordExtensions.HasColumn(reader, "ZoneName"))
                            item.ZoneName = reader["ZoneName"] == System.DBNull.Value ? null : (string)reader["ZoneName"];

                        //if (DataRecordExtensions.HasColumn(reader, "MeterStatus"))
                        //    item.MeterStatus = reader["MeterStatus"] == System.DBNull.Value ? default(int?) : (Int32)reader["MeterStatus"];

                        validatedData.Add(item);
                    }
                }
            }
            catch (Exception ex)
            {
                throw;
            }
            finally
            {
                if (connection.State == ConnectionState.Open)
                    connection.Close();
            }
            return validatedData;
        }
        #endregion


        #region SharjahMunicipality

        public static List<Transaction> GetSharjahMunicipalityData(int customerID, string ZoneID, string MeterName, string ParkingSpaceID, string PlateNumber, string SpaceStatus)
        {
            List<Transaction> validatedData = new List<Transaction>();
            SqlConnection connection = null;
            try
            {
                connection = new SqlConnection(System.Configuration.ConfigurationManager.ConnectionStrings["Connector"].ToString());
                SqlCommand sqlCommand = new SqlCommand();
                sqlCommand.CommandType = CommandType.StoredProcedure;
                //sqlCommand.CommandText = "SpacePlateTxnFinal_railegh";
                sqlCommand.CommandText = "[SpacePlateTxnFinal_Railegh_Metername_Test]";
                sqlCommand.Parameters.AddWithValue("@customerID", customerID);
                sqlCommand.Parameters.AddWithValue("@ZoneID", ZoneID);
                sqlCommand.Parameters.AddWithValue("@MeterName", MeterName);
                sqlCommand.Parameters.AddWithValue("@ParkingSpaceID", ParkingSpaceID);
                sqlCommand.Parameters.AddWithValue("@PlateNumber", PlateNumber);
                sqlCommand.Parameters.AddWithValue("@SpaceStatus", SpaceStatus);
                sqlCommand.Connection = connection;
                sqlCommand.CommandTimeout = 120;
                connection.Open();
                using (var reader = sqlCommand.ExecuteReader())
                {
                    while (reader.Read())
                    {
                        var item = new Transaction();
                        item.EnforcementKey = reader["EnforcementKey"] == System.DBNull.Value ? null : (string)reader["EnforcementKey"];
                        item.CustomerId = (reader.GetInt64(reader.GetOrdinal("CustomerId"))).ToString();
                        item.ParkingSpaceID = reader["ParkingSpaceID"] == System.DBNull.Value ? default(int) : (int)reader["ParkingSpaceID"];
                        item.PlateNumber = reader["PlateNumber"] == System.DBNull.Value ? null : (string)reader["PlateNumber"];
                        item.MeterName = reader["MeterName"] == System.DBNull.Value ? null : (string)reader["MeterName"];
                        item.MeterId = reader["meterid"] == System.DBNull.Value ? null : (string)reader["meterid"];
                        item.Latitude = reader["Latitude"] == System.DBNull.Value ? default(double?) : (double)reader["Latitude"];
                        item.Latitude = reader["Longitude"] == System.DBNull.Value ? default(double?) : (double)reader["Longitude"];

                        item.LPLocation = reader["LPLocation"] == System.DBNull.Value ? null : (string)reader["LPLocation"];
                        item.VMake = reader["VMake"] == System.DBNull.Value ? null : (string)reader["VMake"];
                        item.VModel = reader["VModel"] == System.DBNull.Value ? null : (string)reader["VModel"];
                        item.VColour = reader["VColour"] == System.DBNull.Value ? null : (string)reader["VColour"];
                        item.MeterStreet = reader["MeterStreet"] == System.DBNull.Value ? null : (string)reader["MeterStreet"];
                        item.Location = reader["Location"] == System.DBNull.Value ? null : (string)reader["Location"];

                        item.EnfVendorStallId = reader["EnfVendorStallId"] == System.DBNull.Value ? default(int?) : (Int32)reader["EnfVendorStallId"];
                        item.PresentMeterTime = reader["PresentMeterTime"] == System.DBNull.Value ? default(DateTime?) : (DateTime)reader["PresentMeterTime"];
                        item.SpaceStatus = reader["SpaceStatus"] == System.DBNull.Value ? null : (string)reader["SpaceStatus"];
                        item.SensorEventTime = reader["SensorEventTime"] == System.DBNull.Value ? default(DateTime?) : (DateTime)reader["SensorEventTime"];
                        //item.ExpiredMinutes = reader["ExpiredMinutes"] == System.DBNull.Value ? default(int?) : (Int32)reader["ExpiredMinutes"];
                        item.ExpiredMinutes = reader["ExpiredMinutes"] == System.DBNull.Value ? -1 : (Int32)reader["ExpiredMinutes"];
                        item.AmountIncents = reader["AmountIncents"] == System.DBNull.Value ? default(decimal?) : (decimal)reader["AmountIncents"];
                        item.ExpiryTime = reader["ExpiryTime"] == System.DBNull.Value ? default(DateTime?) : (DateTime)reader["ExpiryTime"];
                        item.TimeZoneId = reader["TimeZoneId"] == System.DBNull.Value ? default(int?) : (Int32)reader["TimeZoneId"];
                        //item.PayStationID = reader["PayStationID"] == System.DBNull.Value ? default(int?) : (Int32)reader["PayStationID"];
                        item.PayStationID = reader["meterid"] == System.DBNull.Value ? default(int?) : Convert.ToInt32(reader["meterid"]);
                        item.ZoneID = reader["ZoneID"] == System.DBNull.Value ? default(int?) : (Int32)reader["ZoneID"];
                        item.ZoneName = reader["ZoneName"] == System.DBNull.Value ? null : (string)reader["ZoneName"];
                        item.Block = reader["Block"] == System.DBNull.Value ? null : (string)reader["Block"];
                        item.Direction = reader["Direction"] == System.DBNull.Value ? null : (string)reader["Direction"];
                        item.MeterStreet = reader["MeterStreet"] == System.DBNull.Value ? null : (string)reader["MeterStreet"];
                        item.StreetType = reader["StreetType"] == System.DBNull.Value ? null : (string)reader["StreetType"];
                        item.MeterStreet = reader["MeterStreet"] == System.DBNull.Value ? null : (string)reader["MeterStreet"];
                        item.CrossStreet1 = reader["CrossStreet1"] == System.DBNull.Value ? null : (string)reader["CrossStreet1"];

                        item.CrossStreet2 = reader["CrossStreet2"] == System.DBNull.Value ? null : (string)reader["CrossStreet2"];
                        item.DigiELType = reader["DigiELType"] == System.DBNull.Value ? default(int?) : (Int32)reader["DigiELType"];
                        item.DigiErrorDescr = reader["DigiErrorDescr"] == System.DBNull.Value ? null : (string)reader["DigiErrorDescr"];
                        item.GeneticELType = reader["GeneticELType"] == System.DBNull.Value ? default(int?) : (Int32)reader["GeneticELType"];
                        item.GeneticErrorDescr = reader["GeneticErrorDescr"] == System.DBNull.Value ? null : (string)reader["GeneticErrorDescr"];

                        if (reader["PMELType"] == System.DBNull.Value)
                        {
                            item.PMELType = default(int?);
                        }
                        else
                        {
                            string pMELTypeValue = reader["PMELType"].ToString();
                            if (string.IsNullOrWhiteSpace(pMELTypeValue))
                            {
                                item.PMELType = default(int?);
                            }
                            else
                            {
                                item.PMELType = (Int32)reader["PMELType"];
                            }
                        }



                        item.PMErrorDescr = reader["PMErrorDescr"] == System.DBNull.Value ? null : (string)reader["PMErrorDescr"];
                        item.EnfHourDesc = reader["EnfHourDesc"] == System.DBNull.Value ? null : (string)reader["EnfHourDesc"];
                        item.MarkedSince = reader["MarkedSince"] == System.DBNull.Value ? default(DateTime?) : (DateTime)reader["MarkedSince"];
                        item.HitDescription = reader["HitDescription"] == System.DBNull.Value ? null : (string)reader["HitDescription"];


                        if (DataRecordExtensions.HasColumn(reader, "Metertype"))
                            item.METERLOC_METERTYPE = reader["Metertype"] == System.DBNull.Value ? null : (string)reader["Metertype"];
                        if (DataRecordExtensions.HasColumn(reader, "ZoneName"))
                            item.ZoneName = reader["ZoneName"] == System.DBNull.Value ? null : (string)reader["ZoneName"];

                        //if (DataRecordExtensions.HasColumn(reader, "MeterStatus"))
                        //    item.MeterStatus = reader["MeterStatus"] == System.DBNull.Value ? default(int?) : (Int32)reader["MeterStatus"];

                        validatedData.Add(item);
                    }
                }
            }
            catch (Exception ex)
            {
                throw;
            }
            finally
            {
                if (connection.State == ConnectionState.Open)
                    connection.Close();
            }
            return validatedData;
        }
        #endregion


        #region IndianaBorough
        public static List<Transaction> GetIndianaBoroughCitationData(int customerID, string ZoneID, string MeterName, string ParkingSpaceID, string PlateNumber, string stateName, string SpaceStatus)
        {
            List<Transaction> validatedData = new List<Transaction>();
            SqlConnection connection = null;
            try
            {
                connection = new SqlConnection(System.Configuration.ConfigurationManager.ConnectionStrings["Connector"].ToString());
                SqlCommand sqlCommand = new SqlCommand();
                sqlCommand.CommandType = CommandType.StoredProcedure;
                // sqlCommand.CommandText = "[SpacePlateTxnFinal_Railegh_Metername_Chicago_test]";
                //sqlCommand.CommandText = "[RefreshService_Plate]";
                sqlCommand.CommandText = "[RefreshService_Plate_Indianborough]";
                sqlCommand.Parameters.AddWithValue("@customerID", customerID);
                sqlCommand.Parameters.AddWithValue("@ZoneID", ZoneID);
                sqlCommand.Parameters.AddWithValue("@MeterName", MeterName);
                sqlCommand.Parameters.AddWithValue("@ParkingSpaceID", ParkingSpaceID);
                sqlCommand.Parameters.AddWithValue("@PlateNumber", PlateNumber);
                sqlCommand.Parameters.AddWithValue("@SpaceStatus", DBNull.Value);

                //sqlCommand.Parameters.AddWithValue("@ReceiptId", ReceiptId);
                sqlCommand.Parameters.AddWithValue("@State", DBNull.Value);
                sqlCommand.Parameters.AddWithValue("@RateNumber", DBNull.Value);

                sqlCommand.Connection = connection;
                sqlCommand.CommandTimeout = 120;
                connection.Open();
                using (var reader = sqlCommand.ExecuteReader())
                {
                    while (reader.Read())
                    {
                        var item = new Transaction();
                        item.EnforcementKey = reader["EnforcementKey"] == System.DBNull.Value ? null : (string)reader["EnforcementKey"];
                        item.CustomerId = (reader.GetInt64(reader.GetOrdinal("CustomerId"))).ToString();
                        item.ParkingSpaceID = reader["ParkingSpaceID"] == System.DBNull.Value ? default(int) : (int)reader["ParkingSpaceID"];
                        item.PlateNumber = reader["PlateNumber"] == System.DBNull.Value ? null : (string)reader["PlateNumber"];
                        item.MeterName = reader["MeterName"] == System.DBNull.Value ? null : (string)reader["MeterName"];
                        item.MeterId = reader["meterid"] == System.DBNull.Value ? null : (string)reader["meterid"];
                        item.Latitude = reader["Latitude"] == System.DBNull.Value ? default(double?) : (double)reader["Latitude"];
                        item.Latitude = reader["Longitude"] == System.DBNull.Value ? default(double?) : (double)reader["Longitude"];

                        item.LPLocation = reader["LPLocation"] == System.DBNull.Value ? null : (string)reader["LPLocation"];
                        item.VMake = null; // reader["VMake"] == System.DBNull.Value ? null : (string)reader["VMake"];
                        item.VModel = null; //reader["VModel"] == System.DBNull.Value ? null : (string)reader["VModel"];
                        item.VColour = null; //reader["VColour"] == System.DBNull.Value ? null : (string)reader["VColour"];
                        item.MeterStreet = reader["MeterStreet"] == System.DBNull.Value ? null : (string)reader["MeterStreet"];
                        item.Location = reader["Location"] == System.DBNull.Value ? null : (string)reader["Location"];

                        item.EnfVendorStallId = reader["EnfVendorStallId"] == System.DBNull.Value ? default(int?) : (Int32)reader["EnfVendorStallId"];
                        item.PresentMeterTime = reader["PresentMeterTime"] == System.DBNull.Value ? default(DateTime?) : (DateTime)reader["PresentMeterTime"];
                        item.SpaceStatus = reader["SpaceStatus"] == System.DBNull.Value ? null : (string)reader["SpaceStatus"];
                        item.SensorEventTime = reader["SensorEventTime"] == System.DBNull.Value ? default(DateTime?) : (DateTime)reader["SensorEventTime"];
                        //item.ExpiredMinutes = reader["ExpiredMinutes"] == System.DBNull.Value ? default(int?) : (Int32)reader["ExpiredMinutes"];
                        item.ExpiredMinutes = reader["ExpiredMinutes"] == System.DBNull.Value ? -1 : (Int32)reader["ExpiredMinutes"];
                        item.AmountIncents = default(decimal?); // reader["AmountIncents"] == System.DBNull.Value ? default(decimal?) : (decimal)reader["AmountIncents"];
                        item.ExpiryTime = reader["ExpiryTime"] == System.DBNull.Value ? default(DateTime?) : (DateTime)reader["ExpiryTime"];
                        item.TimeZoneId = reader["TimeZoneId"] == System.DBNull.Value ? default(int?) : (Int32)reader["TimeZoneId"];
                        //item.PayStationID = reader["PayStationID"] == System.DBNull.Value ? default(int?) : (Int32)reader["PayStationID"];
                        item.PayStationID = reader["meterid"] == System.DBNull.Value ? default(int?) : Convert.ToInt32(reader["meterid"]);
                        item.ZoneID = reader["ZoneID"] == System.DBNull.Value ? default(int?) : (Int32)reader["ZoneID"];
                        item.ZoneName = reader["ZoneName"] == System.DBNull.Value ? null : (string)reader["ZoneName"];
                        item.Block = reader["Block"] == System.DBNull.Value ? null : (string)reader["Block"];
                        item.Direction = reader["Direction"] == System.DBNull.Value ? null : (string)reader["Direction"];
                        item.MeterStreet = reader["MeterStreet"] == System.DBNull.Value ? null : (string)reader["MeterStreet"];
                        item.StreetType = reader["StreetType"] == System.DBNull.Value ? null : (string)reader["StreetType"];
                        item.MeterStreet = reader["MeterStreet"] == System.DBNull.Value ? null : (string)reader["MeterStreet"];
                        item.CrossStreet1 = reader["CrossStreet1"] == System.DBNull.Value ? null : (string)reader["CrossStreet1"];

                        item.CrossStreet2 = reader["CrossStreet2"] == System.DBNull.Value ? null : (string)reader["CrossStreet2"];
                        item.DigiELType = default(int?); //reader["DigiELType"] == System.DBNull.Value ? default(int?) : (Int32)reader["DigiELType"];
                        item.DigiErrorDescr = null; // reader["DigiErrorDescr"] == System.DBNull.Value ? null : (string)reader["DigiErrorDescr"];
                        item.GeneticELType = default(int?); // reader["GeneticELType"] == System.DBNull.Value ? default(int?) : (Int32)reader["GeneticELType"];
                        item.GeneticErrorDescr = null; //reader["GeneticErrorDescr"] == System.DBNull.Value ? null : (string)reader["GeneticErrorDescr"];

                        if (reader["PMELType"] == System.DBNull.Value)
                        {
                            item.PMELType = default(int?);
                        }
                        else
                        {
                            string pMELTypeValue = reader["PMELType"].ToString();
                            if (string.IsNullOrWhiteSpace(pMELTypeValue))
                            {
                                item.PMELType = default(int?);
                            }
                            else
                            {
                                item.PMELType = (Int32)reader["PMELType"];
                            }
                        }



                        item.PMErrorDescr = null; //reader["PMErrorDescr"] == System.DBNull.Value ? null : (string)reader["PMErrorDescr"];
                        item.EnfHourDesc = reader["EnfHourDesc"] == System.DBNull.Value ? null : (string)reader["EnfHourDesc"];
                        item.MarkedSince = reader["MarkedSince"] == System.DBNull.Value ? default(DateTime?) : (DateTime)reader["MarkedSince"];
                        item.HitDescription = reader["HitDescription"] == System.DBNull.Value ? null : (string)reader["HitDescription"];


                        if (DataRecordExtensions.HasColumn(reader, "Metertype"))
                            item.METERLOC_METERTYPE = reader["Metertype"] == System.DBNull.Value ? null : (string)reader["Metertype"];
                        if (DataRecordExtensions.HasColumn(reader, "ZoneName"))
                            item.ZoneName = reader["ZoneName"] == System.DBNull.Value ? null : (string)reader["ZoneName"];


                        if (DataRecordExtensions.HasColumn(reader, "Recieptid"))
                            item.Recieptid = reader["Recieptid"] == System.DBNull.Value ? null : (string)reader["Recieptid"];

                        if (DataRecordExtensions.HasColumn(reader, "RateNumber"))
                            item.RateNumber = reader["RateNumber"] == System.DBNull.Value ? null : (string)reader["RateNumber"];


                        validatedData.Add(item);
                    }
                }
            }
            catch (Exception ex)
            {
                throw;
            }
            finally
            {
                if (connection.State == ConnectionState.Open)
                    connection.Close();
            }
            return validatedData;
        }
        #endregion

        #region Franklin
        public static List<Transaction> GetFranklinCitationData(int customerID, string ZoneID, string MeterName, string ParkingSpaceID, string PlateNumber, string stateName, string SpaceStatus)
        {
            List<Transaction> validatedData = new List<Transaction>();
            SqlConnection connection = null;
            try
            {
                connection = new SqlConnection(System.Configuration.ConfigurationManager.ConnectionStrings["Connector"].ToString());
                SqlCommand sqlCommand = new SqlCommand();
                sqlCommand.CommandType = CommandType.StoredProcedure;
                //sqlCommand.CommandText = "[Sp_PaybyPlateDetails]";
                sqlCommand.CommandText = "[Sp_PaybyPlateDetails_Franklin]";
                sqlCommand.Parameters.AddWithValue("@customerID", customerID);
                sqlCommand.Parameters.AddWithValue("@ZoneID", ZoneID);
                sqlCommand.Parameters.AddWithValue("@MeterName", MeterName);
                sqlCommand.Parameters.AddWithValue("@ParkingSpaceID", ParkingSpaceID);
                sqlCommand.Parameters.AddWithValue("@PlateNumber", PlateNumber);
                sqlCommand.Parameters.AddWithValue("@SpaceStatus", DBNull.Value);

                //sqlCommand.Parameters.AddWithValue("@ReceiptId", ReceiptId);
                //sqlCommand.Parameters.AddWithValue("@State", DBNull.Value);
                //sqlCommand.Parameters.AddWithValue("@RateNumber", DBNull.Value);

                sqlCommand.Connection = connection;
                sqlCommand.CommandTimeout = 120;
                connection.Open();
                using (var reader = sqlCommand.ExecuteReader())
                {
                    while (reader.Read())
                    {
                        var item = new Transaction();
                        item.EnforcementKey = reader["EnforcementKey"] == System.DBNull.Value ? null : (string)reader["EnforcementKey"];
                        item.CustomerId = (reader.GetInt64(reader.GetOrdinal("CustomerId"))).ToString();
                        item.ParkingSpaceID = reader["ParkingSpaceID"] == System.DBNull.Value ? default(int) : (int)reader["ParkingSpaceID"];
                        item.PlateNumber = reader["PlateNumber"] == System.DBNull.Value ? null : (string)reader["PlateNumber"];
                        item.MeterName = reader["MeterName"] == System.DBNull.Value ? null : (string)reader["MeterName"];
                        item.MeterId = reader["meterid"] == System.DBNull.Value ? null : (string)reader["meterid"];
                        item.Latitude = reader["Latitude"] == System.DBNull.Value ? default(double?) : (double)reader["Latitude"];
                        item.Latitude = reader["Longitude"] == System.DBNull.Value ? default(double?) : (double)reader["Longitude"];

                        item.LPLocation = reader["LPLocation"] == System.DBNull.Value ? null : (string)reader["LPLocation"];
                        item.VMake = null; // reader["VMake"] == System.DBNull.Value ? null : (string)reader["VMake"];
                        item.VModel = null; //reader["VModel"] == System.DBNull.Value ? null : (string)reader["VModel"];
                        item.VColour = null; //reader["VColour"] == System.DBNull.Value ? null : (string)reader["VColour"];
                        item.MeterStreet = reader["MeterStreet"] == System.DBNull.Value ? null : (string)reader["MeterStreet"];
                        item.Location = reader["Location"] == System.DBNull.Value ? null : (string)reader["Location"];

                        item.EnfVendorStallId = reader["EnfVendorStallId"] == System.DBNull.Value ? default(int?) : (Int32)reader["EnfVendorStallId"];
                        item.PresentMeterTime = reader["PresentMeterTime"] == System.DBNull.Value ? default(DateTime?) : (DateTime)reader["PresentMeterTime"];
                        item.SpaceStatus = reader["SpaceStatus"] == System.DBNull.Value ? null : (string)reader["SpaceStatus"];
                        item.SensorEventTime = reader["SensorEventTime"] == System.DBNull.Value ? default(DateTime?) : (DateTime)reader["SensorEventTime"];
                        //item.ExpiredMinutes = reader["ExpiredMinutes"] == System.DBNull.Value ? default(int?) : (Int32)reader["ExpiredMinutes"];
                        item.ExpiredMinutes = reader["ExpiredMinutes"] == System.DBNull.Value ? -1 : (Int32)reader["ExpiredMinutes"];
                        item.AmountIncents = default(decimal?); // reader["AmountIncents"] == System.DBNull.Value ? default(decimal?) : (decimal)reader["AmountIncents"];
                        item.ExpiryTime = reader["ExpiryTime"] == System.DBNull.Value ? default(DateTime?) : (DateTime)reader["ExpiryTime"];
                        item.TimeZoneId = reader["TimeZoneId"] == System.DBNull.Value ? default(int?) : (Int32)reader["TimeZoneId"];
                        //item.PayStationID = reader["PayStationID"] == System.DBNull.Value ? default(int?) : (Int32)reader["PayStationID"];
                        item.PayStationID = reader["meterid"] == System.DBNull.Value ? default(int?) : Convert.ToInt32(reader["meterid"]);
                        item.ZoneID = reader["ZoneID"] == System.DBNull.Value ? default(int?) : (Int32)reader["ZoneID"];
                        item.ZoneName = reader["ZoneName"] == System.DBNull.Value ? null : (string)reader["ZoneName"];
                        item.Block = reader["Block"] == System.DBNull.Value ? null : (string)reader["Block"];
                        item.Direction = reader["Direction"] == System.DBNull.Value ? null : (string)reader["Direction"];
                        item.MeterStreet = reader["MeterStreet"] == System.DBNull.Value ? null : (string)reader["MeterStreet"];
                        item.StreetType = reader["StreetType"] == System.DBNull.Value ? null : (string)reader["StreetType"];
                        item.MeterStreet = reader["MeterStreet"] == System.DBNull.Value ? null : (string)reader["MeterStreet"];
                        item.CrossStreet1 = reader["CrossStreet1"] == System.DBNull.Value ? null : (string)reader["CrossStreet1"];

                        item.CrossStreet2 = reader["CrossStreet2"] == System.DBNull.Value ? null : (string)reader["CrossStreet2"];
                        item.DigiELType = default(int?); //reader["DigiELType"] == System.DBNull.Value ? default(int?) : (Int32)reader["DigiELType"];
                        item.DigiErrorDescr = null; // reader["DigiErrorDescr"] == System.DBNull.Value ? null : (string)reader["DigiErrorDescr"];
                        item.GeneticELType = default(int?); // reader["GeneticELType"] == System.DBNull.Value ? default(int?) : (Int32)reader["GeneticELType"];
                        item.GeneticErrorDescr = null; //reader["GeneticErrorDescr"] == System.DBNull.Value ? null : (string)reader["GeneticErrorDescr"];

                        if (reader["PMELType"] == System.DBNull.Value)
                        {
                            item.PMELType = default(int?);
                        }
                        else
                        {
                            string pMELTypeValue = reader["PMELType"].ToString();
                            if (string.IsNullOrWhiteSpace(pMELTypeValue))
                            {
                                item.PMELType = default(int?);
                            }
                            else
                            {
                                item.PMELType = (Int32)reader["PMELType"];
                            }
                        }



                        item.PMErrorDescr = null; //reader["PMErrorDescr"] == System.DBNull.Value ? null : (string)reader["PMErrorDescr"];
                        item.EnfHourDesc = reader["EnfHourDesc"] == System.DBNull.Value ? null : (string)reader["EnfHourDesc"];
                        item.MarkedSince = reader["MarkedSince"] == System.DBNull.Value ? default(DateTime?) : (DateTime)reader["MarkedSince"];
                        item.HitDescription = reader["HitDescription"] == System.DBNull.Value ? null : (string)reader["HitDescription"];


                        if (DataRecordExtensions.HasColumn(reader, "Metertype"))
                            item.METERLOC_METERTYPE = reader["Metertype"] == System.DBNull.Value ? null : (string)reader["Metertype"];
                        if (DataRecordExtensions.HasColumn(reader, "ZoneName"))
                            item.ZoneName = reader["ZoneName"] == System.DBNull.Value ? null : (string)reader["ZoneName"];


                        if (DataRecordExtensions.HasColumn(reader, "Recieptid"))
                            item.Recieptid = reader["Recieptid"] == System.DBNull.Value ? null : (string)reader["Recieptid"];

                        if (DataRecordExtensions.HasColumn(reader, "RateNumber"))
                            item.RateNumber = reader["RateNumber"] == System.DBNull.Value ? null : (string)reader["RateNumber"];


                        validatedData.Add(item);
                    }
                }
            }
            catch (Exception ex)
            {
                throw;
            }
            finally
            {
                if (connection.State == ConnectionState.Open)
                    connection.Close();
            }
            return validatedData;
        }
        #endregion

        #region Sunny Isles Beach
        public static List<Transaction> GetSunnyIslesBeachCitationData(int customerID, string ZoneID, string MeterName, string ParkingSpaceID, string PlateNumber, string stateName, string SpaceStatus)
        {
            List<Transaction> validatedData = new List<Transaction>();
            SqlConnection connection = null;
            try
            {
                connection = new SqlConnection(System.Configuration.ConfigurationManager.ConnectionStrings["Connector"].ToString());
                SqlCommand sqlCommand = new SqlCommand();
                sqlCommand.CommandType = CommandType.StoredProcedure;
                sqlCommand.CommandText = "[Sp_PaybyPlateDetails]";
                sqlCommand.Parameters.AddWithValue("@customerID", customerID);
                sqlCommand.Parameters.AddWithValue("@ZoneID", ZoneID);
                sqlCommand.Parameters.AddWithValue("@MeterName", MeterName);
                sqlCommand.Parameters.AddWithValue("@ParkingSpaceID", ParkingSpaceID);
                sqlCommand.Parameters.AddWithValue("@PlateNumber", PlateNumber);
                sqlCommand.Parameters.AddWithValue("@SpaceStatus", DBNull.Value);

                //sqlCommand.Parameters.AddWithValue("@ReceiptId", ReceiptId);
                //sqlCommand.Parameters.AddWithValue("@State", DBNull.Value);
                //sqlCommand.Parameters.AddWithValue("@RateNumber", DBNull.Value);

                sqlCommand.Connection = connection;
                sqlCommand.CommandTimeout = 120;
                connection.Open();
                using (var reader = sqlCommand.ExecuteReader())
                {
                    while (reader.Read())
                    {
                        var item = new Transaction();
                        item.EnforcementKey = reader["EnforcementKey"] == System.DBNull.Value ? null : (string)reader["EnforcementKey"];
                        item.CustomerId = (reader.GetInt64(reader.GetOrdinal("CustomerId"))).ToString();
                        item.ParkingSpaceID = reader["ParkingSpaceID"] == System.DBNull.Value ? default(int) : (int)reader["ParkingSpaceID"];
                        item.PlateNumber = reader["PlateNumber"] == System.DBNull.Value ? null : (string)reader["PlateNumber"];
                        item.MeterName = reader["MeterName"] == System.DBNull.Value ? null : (string)reader["MeterName"];
                        item.MeterId = reader["meterid"] == System.DBNull.Value ? null : (string)reader["meterid"];
                        item.Latitude = reader["Latitude"] == System.DBNull.Value ? default(double?) : (double)reader["Latitude"];
                        item.Latitude = reader["Longitude"] == System.DBNull.Value ? default(double?) : (double)reader["Longitude"];

                        item.LPLocation = reader["LPLocation"] == System.DBNull.Value ? null : (string)reader["LPLocation"];
                        item.VMake = null; // reader["VMake"] == System.DBNull.Value ? null : (string)reader["VMake"];
                        item.VModel = null; //reader["VModel"] == System.DBNull.Value ? null : (string)reader["VModel"];
                        item.VColour = null; //reader["VColour"] == System.DBNull.Value ? null : (string)reader["VColour"];
                        item.MeterStreet = reader["MeterStreet"] == System.DBNull.Value ? null : (string)reader["MeterStreet"];
                        item.Location = reader["Location"] == System.DBNull.Value ? null : (string)reader["Location"];

                        item.EnfVendorStallId = reader["EnfVendorStallId"] == System.DBNull.Value ? default(int?) : (Int32)reader["EnfVendorStallId"];
                        item.PresentMeterTime = reader["PresentMeterTime"] == System.DBNull.Value ? default(DateTime?) : (DateTime)reader["PresentMeterTime"];
                        item.SpaceStatus = reader["SpaceStatus"] == System.DBNull.Value ? null : (string)reader["SpaceStatus"];
                        item.SensorEventTime = reader["SensorEventTime"] == System.DBNull.Value ? default(DateTime?) : (DateTime)reader["SensorEventTime"];
                        //item.ExpiredMinutes = reader["ExpiredMinutes"] == System.DBNull.Value ? default(int?) : (Int32)reader["ExpiredMinutes"];
                        item.ExpiredMinutes = reader["ExpiredMinutes"] == System.DBNull.Value ? -1 : (Int32)reader["ExpiredMinutes"];
                        item.AmountIncents = default(decimal?); // reader["AmountIncents"] == System.DBNull.Value ? default(decimal?) : (decimal)reader["AmountIncents"];
                        item.ExpiryTime = reader["ExpiryTime"] == System.DBNull.Value ? default(DateTime?) : (DateTime)reader["ExpiryTime"];
                        item.TimeZoneId = reader["TimeZoneId"] == System.DBNull.Value ? default(int?) : (Int32)reader["TimeZoneId"];
                        //item.PayStationID = reader["PayStationID"] == System.DBNull.Value ? default(int?) : (Int32)reader["PayStationID"];
                        item.PayStationID = reader["meterid"] == System.DBNull.Value ? default(int?) : Convert.ToInt32(reader["meterid"]);
                        item.ZoneID = reader["ZoneID"] == System.DBNull.Value ? default(int?) : (Int32)reader["ZoneID"];
                        item.ZoneName = reader["ZoneName"] == System.DBNull.Value ? null : (string)reader["ZoneName"];
                        item.Block = reader["Block"] == System.DBNull.Value ? null : (string)reader["Block"];
                        item.Direction = reader["Direction"] == System.DBNull.Value ? null : (string)reader["Direction"];
                        item.MeterStreet = reader["MeterStreet"] == System.DBNull.Value ? null : (string)reader["MeterStreet"];
                        item.StreetType = reader["StreetType"] == System.DBNull.Value ? null : (string)reader["StreetType"];
                        item.MeterStreet = reader["MeterStreet"] == System.DBNull.Value ? null : (string)reader["MeterStreet"];
                        item.CrossStreet1 = reader["CrossStreet1"] == System.DBNull.Value ? null : (string)reader["CrossStreet1"];

                        item.CrossStreet2 = reader["CrossStreet2"] == System.DBNull.Value ? null : (string)reader["CrossStreet2"];
                        item.DigiELType = default(int?); //reader["DigiELType"] == System.DBNull.Value ? default(int?) : (Int32)reader["DigiELType"];
                        item.DigiErrorDescr = null; // reader["DigiErrorDescr"] == System.DBNull.Value ? null : (string)reader["DigiErrorDescr"];
                        item.GeneticELType = default(int?); // reader["GeneticELType"] == System.DBNull.Value ? default(int?) : (Int32)reader["GeneticELType"];
                        item.GeneticErrorDescr = null; //reader["GeneticErrorDescr"] == System.DBNull.Value ? null : (string)reader["GeneticErrorDescr"];

                        if (reader["PMELType"] == System.DBNull.Value)
                        {
                            item.PMELType = default(int?);
                        }
                        else
                        {
                            string pMELTypeValue = reader["PMELType"].ToString();
                            if (string.IsNullOrWhiteSpace(pMELTypeValue))
                            {
                                item.PMELType = default(int?);
                            }
                            else
                            {
                                item.PMELType = (Int32)reader["PMELType"];
                            }
                        }



                        item.PMErrorDescr = null; //reader["PMErrorDescr"] == System.DBNull.Value ? null : (string)reader["PMErrorDescr"];
                        item.EnfHourDesc = reader["EnfHourDesc"] == System.DBNull.Value ? null : (string)reader["EnfHourDesc"];
                        item.MarkedSince = reader["MarkedSince"] == System.DBNull.Value ? default(DateTime?) : (DateTime)reader["MarkedSince"];
                        item.HitDescription = reader["HitDescription"] == System.DBNull.Value ? null : (string)reader["HitDescription"];


                        if (DataRecordExtensions.HasColumn(reader, "Metertype"))
                            item.METERLOC_METERTYPE = reader["Metertype"] == System.DBNull.Value ? null : (string)reader["Metertype"];
                        if (DataRecordExtensions.HasColumn(reader, "ZoneName"))
                            item.ZoneName = reader["ZoneName"] == System.DBNull.Value ? null : (string)reader["ZoneName"];


                        if (DataRecordExtensions.HasColumn(reader, "Recieptid"))
                            item.Recieptid = reader["Recieptid"] == System.DBNull.Value ? null : (string)reader["Recieptid"];

                        if (DataRecordExtensions.HasColumn(reader, "RateNumber"))
                            item.RateNumber = reader["RateNumber"] == System.DBNull.Value ? null : (string)reader["RateNumber"];


                        validatedData.Add(item);
                    }
                }
            }
            catch (Exception ex)
            {
                throw;
            }
            finally
            {
                if (connection.State == ConnectionState.Open)
                    connection.Close();
            }
            return validatedData;
        }
        #endregion

        #region Surfside
        public static List<Transaction> GetSurfsideBeachCitationData(int customerID, string ZoneID, string MeterName, string ParkingSpaceID, string PlateNumber, string stateName, string SpaceStatus)
        {
            List<Transaction> validatedData = new List<Transaction>();
            SqlConnection connection = null;
            try
            {
                connection = new SqlConnection(System.Configuration.ConfigurationManager.ConnectionStrings["Connector"].ToString());
                SqlCommand sqlCommand = new SqlCommand();
                sqlCommand.CommandType = CommandType.StoredProcedure;
                sqlCommand.CommandText = "[Sp_PaybyPlateDetails]";
                sqlCommand.Parameters.AddWithValue("@customerID", customerID);
                sqlCommand.Parameters.AddWithValue("@ZoneID", ZoneID);
                sqlCommand.Parameters.AddWithValue("@MeterName", MeterName);
                sqlCommand.Parameters.AddWithValue("@ParkingSpaceID", ParkingSpaceID);
                sqlCommand.Parameters.AddWithValue("@PlateNumber", PlateNumber);
                sqlCommand.Parameters.AddWithValue("@SpaceStatus", DBNull.Value);

                //sqlCommand.Parameters.AddWithValue("@ReceiptId", ReceiptId);
                //sqlCommand.Parameters.AddWithValue("@State", DBNull.Value);
                //sqlCommand.Parameters.AddWithValue("@RateNumber", DBNull.Value);

                sqlCommand.Connection = connection;
                sqlCommand.CommandTimeout = 120;
                connection.Open();
                using (var reader = sqlCommand.ExecuteReader())
                {
                    while (reader.Read())
                    {
                        var item = new Transaction();
                        item.EnforcementKey = reader["EnforcementKey"] == System.DBNull.Value ? null : (string)reader["EnforcementKey"];
                        item.CustomerId = (reader.GetInt64(reader.GetOrdinal("CustomerId"))).ToString();
                        item.ParkingSpaceID = reader["ParkingSpaceID"] == System.DBNull.Value ? default(int) : (int)reader["ParkingSpaceID"];
                        item.PlateNumber = reader["PlateNumber"] == System.DBNull.Value ? null : (string)reader["PlateNumber"];
                        item.MeterName = reader["MeterName"] == System.DBNull.Value ? null : (string)reader["MeterName"];
                        item.MeterId = reader["meterid"] == System.DBNull.Value ? null : (string)reader["meterid"];
                        item.Latitude = reader["Latitude"] == System.DBNull.Value ? default(double?) : (double)reader["Latitude"];
                        item.Latitude = reader["Longitude"] == System.DBNull.Value ? default(double?) : (double)reader["Longitude"];

                        item.LPLocation = reader["LPLocation"] == System.DBNull.Value ? null : (string)reader["LPLocation"];
                        item.VMake = null; // reader["VMake"] == System.DBNull.Value ? null : (string)reader["VMake"];
                        item.VModel = null; //reader["VModel"] == System.DBNull.Value ? null : (string)reader["VModel"];
                        item.VColour = null; //reader["VColour"] == System.DBNull.Value ? null : (string)reader["VColour"];
                        item.MeterStreet = reader["MeterStreet"] == System.DBNull.Value ? null : (string)reader["MeterStreet"];
                        item.Location = reader["Location"] == System.DBNull.Value ? null : (string)reader["Location"];

                        item.EnfVendorStallId = reader["EnfVendorStallId"] == System.DBNull.Value ? default(int?) : (Int32)reader["EnfVendorStallId"];
                        item.PresentMeterTime = reader["PresentMeterTime"] == System.DBNull.Value ? default(DateTime?) : (DateTime)reader["PresentMeterTime"];
                        item.SpaceStatus = reader["SpaceStatus"] == System.DBNull.Value ? null : (string)reader["SpaceStatus"];
                        item.SensorEventTime = reader["SensorEventTime"] == System.DBNull.Value ? default(DateTime?) : (DateTime)reader["SensorEventTime"];
                        //item.ExpiredMinutes = reader["ExpiredMinutes"] == System.DBNull.Value ? default(int?) : (Int32)reader["ExpiredMinutes"];
                        item.ExpiredMinutes = reader["ExpiredMinutes"] == System.DBNull.Value ? -1 : (Int32)reader["ExpiredMinutes"];
                        item.AmountIncents = default(decimal?); // reader["AmountIncents"] == System.DBNull.Value ? default(decimal?) : (decimal)reader["AmountIncents"];
                        item.ExpiryTime = reader["ExpiryTime"] == System.DBNull.Value ? default(DateTime?) : (DateTime)reader["ExpiryTime"];
                        item.TimeZoneId = reader["TimeZoneId"] == System.DBNull.Value ? default(int?) : (Int32)reader["TimeZoneId"];
                        //item.PayStationID = reader["PayStationID"] == System.DBNull.Value ? default(int?) : (Int32)reader["PayStationID"];
                        item.PayStationID = reader["meterid"] == System.DBNull.Value ? default(int?) : Convert.ToInt32(reader["meterid"]);
                        item.ZoneID = reader["ZoneID"] == System.DBNull.Value ? default(int?) : (Int32)reader["ZoneID"];
                        item.ZoneName = reader["ZoneName"] == System.DBNull.Value ? null : (string)reader["ZoneName"];
                        item.Block = reader["Block"] == System.DBNull.Value ? null : (string)reader["Block"];
                        item.Direction = reader["Direction"] == System.DBNull.Value ? null : (string)reader["Direction"];
                        item.MeterStreet = reader["MeterStreet"] == System.DBNull.Value ? null : (string)reader["MeterStreet"];
                        item.StreetType = reader["StreetType"] == System.DBNull.Value ? null : (string)reader["StreetType"];
                        item.MeterStreet = reader["MeterStreet"] == System.DBNull.Value ? null : (string)reader["MeterStreet"];
                        item.CrossStreet1 = reader["CrossStreet1"] == System.DBNull.Value ? null : (string)reader["CrossStreet1"];

                        item.CrossStreet2 = reader["CrossStreet2"] == System.DBNull.Value ? null : (string)reader["CrossStreet2"];
                        item.DigiELType = default(int?); //reader["DigiELType"] == System.DBNull.Value ? default(int?) : (Int32)reader["DigiELType"];
                        item.DigiErrorDescr = null; // reader["DigiErrorDescr"] == System.DBNull.Value ? null : (string)reader["DigiErrorDescr"];
                        item.GeneticELType = default(int?); // reader["GeneticELType"] == System.DBNull.Value ? default(int?) : (Int32)reader["GeneticELType"];
                        item.GeneticErrorDescr = null; //reader["GeneticErrorDescr"] == System.DBNull.Value ? null : (string)reader["GeneticErrorDescr"];

                        if (reader["PMELType"] == System.DBNull.Value)
                        {
                            item.PMELType = default(int?);
                        }
                        else
                        {
                            string pMELTypeValue = reader["PMELType"].ToString();
                            if (string.IsNullOrWhiteSpace(pMELTypeValue))
                            {
                                item.PMELType = default(int?);
                            }
                            else
                            {
                                item.PMELType = (Int32)reader["PMELType"];
                            }
                        }



                        item.PMErrorDescr = null; //reader["PMErrorDescr"] == System.DBNull.Value ? null : (string)reader["PMErrorDescr"];
                        item.EnfHourDesc = reader["EnfHourDesc"] == System.DBNull.Value ? null : (string)reader["EnfHourDesc"];
                        item.MarkedSince = reader["MarkedSince"] == System.DBNull.Value ? default(DateTime?) : (DateTime)reader["MarkedSince"];
                        item.HitDescription = reader["HitDescription"] == System.DBNull.Value ? null : (string)reader["HitDescription"];


                        if (DataRecordExtensions.HasColumn(reader, "Metertype"))
                            item.METERLOC_METERTYPE = reader["Metertype"] == System.DBNull.Value ? null : (string)reader["Metertype"];
                        if (DataRecordExtensions.HasColumn(reader, "ZoneName"))
                            item.ZoneName = reader["ZoneName"] == System.DBNull.Value ? null : (string)reader["ZoneName"];


                        if (DataRecordExtensions.HasColumn(reader, "Recieptid"))
                            item.Recieptid = reader["Recieptid"] == System.DBNull.Value ? null : (string)reader["Recieptid"];

                        if (DataRecordExtensions.HasColumn(reader, "RateNumber"))
                            item.RateNumber = reader["RateNumber"] == System.DBNull.Value ? null : (string)reader["RateNumber"];


                        validatedData.Add(item);
                    }
                }
            }
            catch (Exception ex)
            {
                throw;
            }
            finally
            {
                if (connection.State == ConnectionState.Open)
                    connection.Close();
            }
            return validatedData;
        }
        #endregion

        #region NewOrleans
        public static List<Transaction> GetNewOrleansCitationData(int customerID, string ZoneID, string MeterName, string ParkingSpaceID, string PlateNumber, string stateName, string SpaceStatus)
        {
            List<Transaction> validatedData = new List<Transaction>();
            SqlConnection connection = null;
            try
            {
                connection = new SqlConnection(System.Configuration.ConfigurationManager.ConnectionStrings["Connector"].ToString());
                SqlCommand sqlCommand = new SqlCommand();
                sqlCommand.CommandType = CommandType.StoredProcedure;
                sqlCommand.CommandText = "[Sp_PaybyPlateDetails]";
                sqlCommand.Parameters.AddWithValue("@customerID", customerID);
                sqlCommand.Parameters.AddWithValue("@ZoneID", ZoneID);
                sqlCommand.Parameters.AddWithValue("@MeterName", MeterName);
                sqlCommand.Parameters.AddWithValue("@ParkingSpaceID", ParkingSpaceID);
                sqlCommand.Parameters.AddWithValue("@PlateNumber", PlateNumber);
                sqlCommand.Parameters.AddWithValue("@SpaceStatus", DBNull.Value);

                //sqlCommand.Parameters.AddWithValue("@ReceiptId", ReceiptId);
                //sqlCommand.Parameters.AddWithValue("@State", DBNull.Value);
                //sqlCommand.Parameters.AddWithValue("@RateNumber", DBNull.Value);

                sqlCommand.Connection = connection;
                sqlCommand.CommandTimeout = 120;
                connection.Open();
                using (var reader = sqlCommand.ExecuteReader())
                {
                    while (reader.Read())
                    {
                        var item = new Transaction();
                        item.EnforcementKey = reader["EnforcementKey"] == System.DBNull.Value ? null : (string)reader["EnforcementKey"];
                        item.CustomerId = (reader.GetInt64(reader.GetOrdinal("CustomerId"))).ToString();
                        item.ParkingSpaceID = reader["ParkingSpaceID"] == System.DBNull.Value ? default(int) : (int)reader["ParkingSpaceID"];
                        item.PlateNumber = reader["PlateNumber"] == System.DBNull.Value ? null : (string)reader["PlateNumber"];
                        item.MeterName = reader["MeterName"] == System.DBNull.Value ? null : (string)reader["MeterName"];
                        //item.MeterId = reader["meterid"] == System.DBNull.Value ? null : (string)reader["meterid"]; //to check
                        if (DataRecordExtensions.HasColumn(reader, "ZoneName"))
                        {
                            if ((reader["meterid"].GetType()) == typeof(Int32))
                                item.MeterId = reader["meterid"] == System.DBNull.Value ? null : ((Int32)reader["meterid"]).ToString();
                            else if ((reader["meterid"].GetType()) == typeof(Int64))
                                item.MeterId = reader["meterid"] == System.DBNull.Value ? null : ((Int64)reader["meterid"]).ToString();
                            else
                                item.MeterId = reader["meterid"] == System.DBNull.Value ? null : (string)reader["meterid"];
                        }
                        item.Latitude = reader["Latitude"] == System.DBNull.Value ? default(double?) : (double)reader["Latitude"];
                        item.Latitude = reader["Longitude"] == System.DBNull.Value ? default(double?) : (double)reader["Longitude"];

                        item.LPLocation = reader["LPLocation"] == System.DBNull.Value ? null : (string)reader["LPLocation"];
                        item.VMake = null; // reader["VMake"] == System.DBNull.Value ? null : (string)reader["VMake"];
                        item.VModel = null; //reader["VModel"] == System.DBNull.Value ? null : (string)reader["VModel"];
                        item.VColour = null; //reader["VColour"] == System.DBNull.Value ? null : (string)reader["VColour"];
                        item.MeterStreet = reader["MeterStreet"] == System.DBNull.Value ? null : (string)reader["MeterStreet"];
                        item.Location = reader["Location"] == System.DBNull.Value ? null : (string)reader["Location"];

                        item.EnfVendorStallId = reader["EnfVendorStallId"] == System.DBNull.Value ? default(int?) : (Int32)reader["EnfVendorStallId"];
                        item.PresentMeterTime = reader["PresentMeterTime"] == System.DBNull.Value ? default(DateTime?) : (DateTime)reader["PresentMeterTime"];
                        item.SpaceStatus = reader["SpaceStatus"] == System.DBNull.Value ? null : (string)reader["SpaceStatus"];
                        item.SensorEventTime = reader["SensorEventTime"] == System.DBNull.Value ? default(DateTime?) : (DateTime)reader["SensorEventTime"];
                        //item.ExpiredMinutes = reader["ExpiredMinutes"] == System.DBNull.Value ? default(int?) : (Int32)reader["ExpiredMinutes"];
                        item.ExpiredMinutes = reader["ExpiredMinutes"] == System.DBNull.Value ? -1 : (Int32)reader["ExpiredMinutes"];
                        item.AmountIncents = default(decimal?); // reader["AmountIncents"] == System.DBNull.Value ? default(decimal?) : (decimal)reader["AmountIncents"];
                        item.ExpiryTime = reader["ExpiryTime"] == System.DBNull.Value ? default(DateTime?) : (DateTime)reader["ExpiryTime"];
                        item.TimeZoneId = reader["TimeZoneId"] == System.DBNull.Value ? default(int?) : (Int32)reader["TimeZoneId"];
                        //item.PayStationID = reader["PayStationID"] == System.DBNull.Value ? default(int?) : (Int32)reader["PayStationID"];
                        item.PayStationID = reader["meterid"] == System.DBNull.Value ? default(int?) : Convert.ToInt32(reader["meterid"]);
                        item.ZoneID = reader["ZoneID"] == System.DBNull.Value ? default(int?) : (Int32)reader["ZoneID"];
                        //item.ZoneName = reader["ZoneName"] == System.DBNull.Value ? null : (string)reader["ZoneName"];//not required.added below
                        item.Block = reader["Block"] == System.DBNull.Value ? null : (string)reader["Block"];
                        item.Direction = reader["Direction"] == System.DBNull.Value ? null : (string)reader["Direction"];
                        item.MeterStreet = reader["MeterStreet"] == System.DBNull.Value ? null : (string)reader["MeterStreet"];
                        item.StreetType = reader["StreetType"] == System.DBNull.Value ? null : (string)reader["StreetType"];
                        item.MeterStreet = reader["MeterStreet"] == System.DBNull.Value ? null : (string)reader["MeterStreet"];
                        item.CrossStreet1 = reader["CrossStreet1"] == System.DBNull.Value ? null : (string)reader["CrossStreet1"];

                        item.CrossStreet2 = reader["CrossStreet2"] == System.DBNull.Value ? null : (string)reader["CrossStreet2"];
                        item.DigiELType = default(int?); //reader["DigiELType"] == System.DBNull.Value ? default(int?) : (Int32)reader["DigiELType"];
                        item.DigiErrorDescr = null; // reader["DigiErrorDescr"] == System.DBNull.Value ? null : (string)reader["DigiErrorDescr"];
                        item.GeneticELType = default(int?); // reader["GeneticELType"] == System.DBNull.Value ? default(int?) : (Int32)reader["GeneticELType"];
                        item.GeneticErrorDescr = null; //reader["GeneticErrorDescr"] == System.DBNull.Value ? null : (string)reader["GeneticErrorDescr"];

                        if (reader["PMELType"] == System.DBNull.Value)
                        {
                            item.PMELType = default(int?);
                        }
                        else
                        {
                            string pMELTypeValue = reader["PMELType"].ToString();
                            if (string.IsNullOrWhiteSpace(pMELTypeValue))
                            {
                                item.PMELType = default(int?);
                            }
                            else
                            {
                                item.PMELType = (Int32)reader["PMELType"];
                            }
                        }



                        item.PMErrorDescr = null; //reader["PMErrorDescr"] == System.DBNull.Value ? null : (string)reader["PMErrorDescr"];
                        item.EnfHourDesc = reader["EnfHourDesc"] == System.DBNull.Value ? null : (string)reader["EnfHourDesc"];
                        item.MarkedSince = reader["MarkedSince"] == System.DBNull.Value ? default(DateTime?) : (DateTime)reader["MarkedSince"];
                        item.HitDescription = reader["HitDescription"] == System.DBNull.Value ? null : (string)reader["HitDescription"];


                        if (DataRecordExtensions.HasColumn(reader, "Metertype"))
                            item.METERLOC_METERTYPE = reader["Metertype"] == System.DBNull.Value ? null : (string)reader["Metertype"];

                         if (DataRecordExtensions.HasColumn(reader, "ZoneName"))
                         {
                               if ((reader["ZoneName"].GetType()) == typeof(Int32))
                                   item.ZoneName = reader["ZoneName"] == System.DBNull.Value ? null : ((Int32)reader["ZoneName"]).ToString();
                             else if ((reader["ZoneName"].GetType()) == typeof(Int64))
                                   item.ZoneName = reader["ZoneName"] == System.DBNull.Value ? null : ((Int64)reader["ZoneName"]).ToString();
                             else
                                   item.ZoneName = reader["ZoneName"] == System.DBNull.Value ? null : (string)reader["ZoneName"];
                         }



                        //if (DataRecordExtensions.HasColumn(reader, "ZoneName")) ////to check
                        //    item.ZoneName = reader["ZoneName"] == System.DBNull.Value ? null : (string)reader["ZoneName"];


                        if (DataRecordExtensions.HasColumn(reader, "Recieptid"))
                            item.Recieptid = reader["Recieptid"] == System.DBNull.Value ? null : (string)reader["Recieptid"];

                        if (DataRecordExtensions.HasColumn(reader, "RateNumber"))
                            item.RateNumber = reader["RateNumber"] == System.DBNull.Value ? null : (string)reader["RateNumber"];


                        validatedData.Add(item);
                    }
                }
            }
            catch (Exception ex)
            {
                throw;
            }
            finally
            {
                if (connection.State == ConnectionState.Open)
                    connection.Close();
            }
            return validatedData;
        }
        #endregion

        #region Philadelphia
        public static List<Transaction> GetPhiladelphiaCitationData(int customerID, string ZoneName, string MeterName, string ParkingSpaceID, string PlateNumber, string stateName, string SpaceStatus)
        {
            List<Transaction> validatedData = new List<Transaction>();
            SqlConnection connection = null;
            try
            {
                connection = new SqlConnection(System.Configuration.ConfigurationManager.ConnectionStrings["Connector"].ToString());
                SqlCommand sqlCommand = new SqlCommand();
                sqlCommand.CommandType = CommandType.StoredProcedure;
                //sqlCommand.CommandText = "[SpacePlateTxnFinal_Multispaceplate]";
                if (string.IsNullOrEmpty(ZoneName))
                {
                    sqlCommand.CommandText = "[SpacePlateTxnFinal_Multispaceplate]";
                    sqlCommand.Parameters.AddWithValue("@customerID", customerID);
                    sqlCommand.Parameters.AddWithValue("@ZoneID", DBNull.Value);
                    sqlCommand.Parameters.AddWithValue("@MeterName", MeterName);
                    sqlCommand.Parameters.AddWithValue("@ParkingSpaceID", ParkingSpaceID);
                    sqlCommand.Parameters.AddWithValue("@PlateNumber", PlateNumber);
                    sqlCommand.Parameters.AddWithValue("@SpaceStatus", DBNull.Value);
                }
                else
                {
                    sqlCommand.CommandText = "[SpacePlateTxnFinal_Multispaceplate_Philly]";
                    sqlCommand.Parameters.AddWithValue("@customerID", customerID);
                    sqlCommand.Parameters.AddWithValue("@ZoneID", DBNull.Value);
                    sqlCommand.Parameters.AddWithValue("@MeterName", MeterName);
                    sqlCommand.Parameters.AddWithValue("@ParkingSpaceID", ParkingSpaceID);
                    sqlCommand.Parameters.AddWithValue("@PlateNumber", PlateNumber);
                    sqlCommand.Parameters.AddWithValue("@SpaceStatus", DBNull.Value);
                    sqlCommand.Parameters.AddWithValue("@ZoneName", ZoneName);
                }




                //sqlCommand.Parameters.AddWithValue("@ReceiptId", ReceiptId);
                //sqlCommand.Parameters.AddWithValue("@State", DBNull.Value);
                //sqlCommand.Parameters.AddWithValue("@RateNumber", DBNull.Value);

                sqlCommand.Connection = connection;
                sqlCommand.CommandTimeout = 120;
                connection.Open();
                using (var reader = sqlCommand.ExecuteReader())
                {
                    while (reader.Read())
                    {
                        var item = new Transaction();
                        item.EnforcementKey = reader["EnforcementKey"] == System.DBNull.Value ? null : (string)reader["EnforcementKey"];
                        item.CustomerId = (reader.GetInt64(reader.GetOrdinal("CustomerId"))).ToString();
                        item.ParkingSpaceID = reader["ParkingSpaceID"] == System.DBNull.Value ? default(int) : (int)reader["ParkingSpaceID"];
                        item.PlateNumber = reader["PlateNumber"] == System.DBNull.Value ? null : (string)reader["PlateNumber"];
                        item.MeterName = reader["MeterName"] == System.DBNull.Value ? null : (string)reader["MeterName"];
                        item.MeterId = reader["meterid"] == System.DBNull.Value ? null : (string)reader["meterid"];
                        item.Latitude = reader["Latitude"] == System.DBNull.Value ? default(double?) : (double)reader["Latitude"];
                        item.Latitude = reader["Longitude"] == System.DBNull.Value ? default(double?) : (double)reader["Longitude"];

                        item.LPLocation = reader["LPLocation"] == System.DBNull.Value ? null : (string)reader["LPLocation"];
                        item.VMake = null; // reader["VMake"] == System.DBNull.Value ? null : (string)reader["VMake"];
                        item.VModel = null; //reader["VModel"] == System.DBNull.Value ? null : (string)reader["VModel"];
                        item.VColour = null; //reader["VColour"] == System.DBNull.Value ? null : (string)reader["VColour"];
                        item.MeterStreet = reader["MeterStreet"] == System.DBNull.Value ? null : (string)reader["MeterStreet"];
                        item.Location = reader["Location"] == System.DBNull.Value ? null : (string)reader["Location"];

                        item.EnfVendorStallId = reader["EnfVendorStallId"] == System.DBNull.Value ? default(int?) : (Int32)reader["EnfVendorStallId"];
                        item.PresentMeterTime = reader["PresentMeterTime"] == System.DBNull.Value ? default(DateTime?) : (DateTime)reader["PresentMeterTime"];
                        item.SpaceStatus = reader["SpaceStatus"] == System.DBNull.Value ? null : (string)reader["SpaceStatus"];
                        item.SensorEventTime = reader["SensorEventTime"] == System.DBNull.Value ? default(DateTime?) : (DateTime)reader["SensorEventTime"];
                        //item.ExpiredMinutes = reader["ExpiredMinutes"] == System.DBNull.Value ? default(int?) : (Int32)reader["ExpiredMinutes"];
                        item.ExpiredMinutes = reader["ExpiredMinutes"] == System.DBNull.Value ? -1 : (Int32)reader["ExpiredMinutes"];
                        item.AmountIncents = default(decimal?); // reader["AmountIncents"] == System.DBNull.Value ? default(decimal?) : (decimal)reader["AmountIncents"];
                        item.ExpiryTime = reader["ExpiryTime"] == System.DBNull.Value ? default(DateTime?) : (DateTime)reader["ExpiryTime"];
                        item.TimeZoneId = reader["TimeZoneId"] == System.DBNull.Value ? default(int?) : (Int32)reader["TimeZoneId"];
                        //item.PayStationID = reader["PayStationID"] == System.DBNull.Value ? default(int?) : (Int32)reader["PayStationID"];
                        item.PayStationID = reader["meterid"] == System.DBNull.Value ? default(int?) : Convert.ToInt32(reader["meterid"]);
                        item.ZoneID = reader["ZoneID"] == System.DBNull.Value ? default(int?) : (Int32)reader["ZoneID"];
                        item.ZoneName = reader["ZoneName"] == System.DBNull.Value ? null : (string)reader["ZoneName"];
                        item.Block = reader["Block"] == System.DBNull.Value ? null : (string)reader["Block"];
                        item.Direction = reader["Direction"] == System.DBNull.Value ? null : (string)reader["Direction"];
                        item.MeterStreet = reader["MeterStreet"] == System.DBNull.Value ? null : (string)reader["MeterStreet"];
                        item.StreetType = reader["StreetType"] == System.DBNull.Value ? null : (string)reader["StreetType"];
                        item.MeterStreet = reader["MeterStreet"] == System.DBNull.Value ? null : (string)reader["MeterStreet"];
                        item.CrossStreet1 = reader["CrossStreet1"] == System.DBNull.Value ? null : (string)reader["CrossStreet1"];

                        item.CrossStreet2 = reader["CrossStreet2"] == System.DBNull.Value ? null : (string)reader["CrossStreet2"];
                        item.DigiELType = default(int?); //reader["DigiELType"] == System.DBNull.Value ? default(int?) : (Int32)reader["DigiELType"];
                        item.DigiErrorDescr = null; // reader["DigiErrorDescr"] == System.DBNull.Value ? null : (string)reader["DigiErrorDescr"];
                        item.GeneticELType = default(int?); // reader["GeneticELType"] == System.DBNull.Value ? default(int?) : (Int32)reader["GeneticELType"];
                        item.GeneticErrorDescr = null; //reader["GeneticErrorDescr"] == System.DBNull.Value ? null : (string)reader["GeneticErrorDescr"];

                        if (reader["PMELType"] == System.DBNull.Value)
                        {
                            item.PMELType = default(int?);
                        }
                        else
                        {
                            string pMELTypeValue = reader["PMELType"].ToString();
                            if (string.IsNullOrWhiteSpace(pMELTypeValue))
                            {
                                item.PMELType = default(int?);
                            }
                            else
                            {
                                item.PMELType = (Int32)reader["PMELType"];
                            }
                        }



                        item.PMErrorDescr = null; //reader["PMErrorDescr"] == System.DBNull.Value ? null : (string)reader["PMErrorDescr"];
                        item.EnfHourDesc = reader["EnfHourDesc"] == System.DBNull.Value ? null : (string)reader["EnfHourDesc"];
                        item.MarkedSince = reader["MarkedSince"] == System.DBNull.Value ? default(DateTime?) : (DateTime)reader["MarkedSince"];
                        item.HitDescription = reader["HitDescription"] == System.DBNull.Value ? null : (string)reader["HitDescription"];


                        if (DataRecordExtensions.HasColumn(reader, "Metertype"))
                            item.METERLOC_METERTYPE = reader["Metertype"] == System.DBNull.Value ? null : (string)reader["Metertype"];
                        if (DataRecordExtensions.HasColumn(reader, "ZoneName"))
                            item.ZoneName = reader["ZoneName"] == System.DBNull.Value ? null : (string)reader["ZoneName"];


                        if (DataRecordExtensions.HasColumn(reader, "Recieptid"))
                            item.Recieptid = reader["Recieptid"] == System.DBNull.Value ? null : (string)reader["Recieptid"];

                        if (DataRecordExtensions.HasColumn(reader, "RateNumber"))
                            item.RateNumber = reader["RateNumber"] == System.DBNull.Value ? null : (string)reader["RateNumber"];


                        validatedData.Add(item);
                    }
                }
            }
            catch (Exception ex)
            {
                throw;
            }
            finally
            {
                if (connection.State == ConnectionState.Open)
                    connection.Close();
            }
            return validatedData;
        }

        public static List<Transaction> GetPhiladelphiaCitationDataSpace(int customerID, string ZoneName, string MeterName, string ParkingSpaceID, string PlateNumber, string stateName, string SpaceStatus)
        {
            List<Transaction> validatedData = new List<Transaction>();
            SqlConnection connection = null;
            try
            {
                connection = new SqlConnection(System.Configuration.ConfigurationManager.ConnectionStrings["Connector"].ToString());
                SqlCommand sqlCommand = new SqlCommand();
                sqlCommand.CommandType = CommandType.StoredProcedure;
                //sqlCommand.CommandText = "[SpacePlateTxnFinal_Multispaceplate]";

                sqlCommand.CommandText = "[SpacePlateTxnFinal_Multispaceplate_Philly_Space]";
                sqlCommand.Parameters.AddWithValue("@customerID", customerID);
                sqlCommand.Parameters.AddWithValue("@ZoneID", DBNull.Value);
                sqlCommand.Parameters.AddWithValue("@MeterName", DBNull.Value);
                sqlCommand.Parameters.AddWithValue("@ParkingSpaceID", ParkingSpaceID);
                sqlCommand.Parameters.AddWithValue("@PlateNumber", DBNull.Value);
                sqlCommand.Parameters.AddWithValue("@SpaceStatus", DBNull.Value);
                sqlCommand.Parameters.AddWithValue("@ZoneName", MeterName); ///As Key is configured as Zone Name as Zone from PARA not available always.

                sqlCommand.Connection = connection;
                sqlCommand.CommandTimeout = 120;
                connection.Open();
                using (var reader = sqlCommand.ExecuteReader())
                {
                    while (reader.Read())
                    {
                        var item = new Transaction();
                        item.EnforcementKey = reader["EnforcementKey"] == System.DBNull.Value ? null : (string)reader["EnforcementKey"];
                        item.CustomerId = (reader.GetInt64(reader.GetOrdinal("CustomerId"))).ToString();
                        item.ParkingSpaceID = reader["ParkingSpaceID"] == System.DBNull.Value ? default(int) : (int)reader["ParkingSpaceID"];
                        //ParkingSpaceIDr = reader["ParkingSpaceID"] == System.DBNull.Value ? string.Empty : (string)reader["ParkingSpaceID"];
                        //item.ParkingSpaceID = string.IsNullOrEmpty(ParkingSpaceID) ? default(int) : Convert.ToInt32(ParkingSpaceIDr);
                        item.PlateNumber = reader["PlateNumber"] == System.DBNull.Value ? null : (string)reader["PlateNumber"];
                        item.MeterName = reader["MeterName"] == System.DBNull.Value ? null : (string)reader["MeterName"];
                        item.MeterId = reader["meterid"] == System.DBNull.Value ? null : (string)reader["meterid"];
                        item.Latitude = reader["Latitude"] == System.DBNull.Value ? default(double?) : (double)reader["Latitude"];
                        item.Latitude = reader["Longitude"] == System.DBNull.Value ? default(double?) : (double)reader["Longitude"];

                        item.LPLocation = reader["LPLocation"] == System.DBNull.Value ? null : (string)reader["LPLocation"];
                        item.VMake = null; // reader["VMake"] == System.DBNull.Value ? null : (string)reader["VMake"];
                        item.VModel = null; //reader["VModel"] == System.DBNull.Value ? null : (string)reader["VModel"];
                        item.VColour = null; //reader["VColour"] == System.DBNull.Value ? null : (string)reader["VColour"];
                        item.MeterStreet = reader["MeterStreet"] == System.DBNull.Value ? null : (string)reader["MeterStreet"];
                        item.Location = reader["Location"] == System.DBNull.Value ? null : (string)reader["Location"];

                        item.EnfVendorStallId = reader["EnfVendorStallId"] == System.DBNull.Value ? default(int?) : (Int32)reader["EnfVendorStallId"];
                        item.PresentMeterTime = reader["PresentMeterTime"] == System.DBNull.Value ? default(DateTime?) : (DateTime)reader["PresentMeterTime"];
                        item.SpaceStatus = reader["SpaceStatus"] == System.DBNull.Value ? null : (string)reader["SpaceStatus"];
                        item.SensorEventTime = reader["SensorEventTime"] == System.DBNull.Value ? default(DateTime?) : (DateTime)reader["SensorEventTime"];
                        //item.ExpiredMinutes = reader["ExpiredMinutes"] == System.DBNull.Value ? default(int?) : (Int32)reader["ExpiredMinutes"];
                        item.ExpiredMinutes = reader["ExpiredMinutes"] == System.DBNull.Value ? -1 : (Int32)reader["ExpiredMinutes"];
                        item.AmountIncents = default(decimal?); // reader["AmountIncents"] == System.DBNull.Value ? default(decimal?) : (decimal)reader["AmountIncents"];
                        item.ExpiryTime = reader["ExpiryTime"] == System.DBNull.Value ? default(DateTime?) : (DateTime)reader["ExpiryTime"];
                        item.TimeZoneId = reader["TimeZoneId"] == System.DBNull.Value ? default(int?) : (Int32)reader["TimeZoneId"];
                        //item.PayStationID = reader["PayStationID"] == System.DBNull.Value ? default(int?) : (Int32)reader["PayStationID"];
                        item.PayStationID = reader["meterid"] == System.DBNull.Value ? default(int?) : Convert.ToInt32(reader["meterid"]);
                        item.ZoneID = reader["ZoneID"] == System.DBNull.Value ? default(int?) : (Int32)reader["ZoneID"];
                        item.ZoneName = reader["ZoneName"] == System.DBNull.Value ? null : (string)reader["ZoneName"];
                        item.Block = reader["Block"] == System.DBNull.Value ? null : (string)reader["Block"];
                        item.Direction = reader["Direction"] == System.DBNull.Value ? null : (string)reader["Direction"];
                        item.MeterStreet = reader["MeterStreet"] == System.DBNull.Value ? null : (string)reader["MeterStreet"];
                        item.StreetType = reader["StreetType"] == System.DBNull.Value ? null : (string)reader["StreetType"];
                        item.MeterStreet = reader["MeterStreet"] == System.DBNull.Value ? null : (string)reader["MeterStreet"];
                        item.CrossStreet1 = reader["CrossStreet1"] == System.DBNull.Value ? null : (string)reader["CrossStreet1"];

                        item.CrossStreet2 = reader["CrossStreet2"] == System.DBNull.Value ? null : (string)reader["CrossStreet2"];
                        item.DigiELType = default(int?); //reader["DigiELType"] == System.DBNull.Value ? default(int?) : (Int32)reader["DigiELType"];
                        item.DigiErrorDescr = null; // reader["DigiErrorDescr"] == System.DBNull.Value ? null : (string)reader["DigiErrorDescr"];
                        item.GeneticELType = default(int?); // reader["GeneticELType"] == System.DBNull.Value ? default(int?) : (Int32)reader["GeneticELType"];
                        item.GeneticErrorDescr = null; //reader["GeneticErrorDescr"] == System.DBNull.Value ? null : (string)reader["GeneticErrorDescr"];

                        if (reader["PMELType"] == System.DBNull.Value)
                        {
                            item.PMELType = default(int?);
                        }
                        else
                        {
                            string pMELTypeValue = reader["PMELType"].ToString();
                            if (string.IsNullOrWhiteSpace(pMELTypeValue))
                            {
                                item.PMELType = default(int?);
                            }
                            else
                            {
                                item.PMELType = (Int32)reader["PMELType"];
                            }
                        }



                        item.PMErrorDescr = null; //reader["PMErrorDescr"] == System.DBNull.Value ? null : (string)reader["PMErrorDescr"];
                        item.EnfHourDesc = reader["EnfHourDesc"] == System.DBNull.Value ? null : (string)reader["EnfHourDesc"];
                        item.MarkedSince = reader["MarkedSince"] == System.DBNull.Value ? default(DateTime?) : (DateTime)reader["MarkedSince"];
                        item.HitDescription = reader["HitDescription"] == System.DBNull.Value ? null : (string)reader["HitDescription"];


                        if (DataRecordExtensions.HasColumn(reader, "Metertype"))
                            item.METERLOC_METERTYPE = reader["Metertype"] == System.DBNull.Value ? null : (string)reader["Metertype"];
                        if (DataRecordExtensions.HasColumn(reader, "ZoneName"))
                            item.ZoneName = reader["ZoneName"] == System.DBNull.Value ? null : (string)reader["ZoneName"];


                        if (DataRecordExtensions.HasColumn(reader, "Recieptid"))
                            item.Recieptid = reader["Recieptid"] == System.DBNull.Value ? null : (string)reader["Recieptid"];

                        if (DataRecordExtensions.HasColumn(reader, "RateNumber"))
                            item.RateNumber = reader["RateNumber"] == System.DBNull.Value ? null : (string)reader["RateNumber"];


                        validatedData.Add(item);
                    }
                }
            }
            catch (Exception ex)
            {
                throw;
            }
            finally
            {
                if (connection.State == ConnectionState.Open)
                    connection.Close();
            }
            return validatedData;
        }
        #endregion

        #region Auburn
        public static List<Transaction> GetAuburnWithPlateName(int customerID, string ZoneID, string MeterName, string ParkingSpaceID, string PlateNumber, string stateName, string SpaceStatus)
        {
            List<Transaction> validatedData = new List<Transaction>();
            SqlConnection connection = null;
            try
            {
                connection = new SqlConnection(System.Configuration.ConfigurationManager.ConnectionStrings["Connector"].ToString());
                SqlCommand sqlCommand = new SqlCommand();
                sqlCommand.CommandType = CommandType.StoredProcedure;
                sqlCommand.CommandText = "[Sp_PaybyPlateDetails_state]";
                sqlCommand.Parameters.AddWithValue("@customerID", customerID);
                sqlCommand.Parameters.AddWithValue("@ZoneID", ZoneID);
                sqlCommand.Parameters.AddWithValue("@MeterName", MeterName);
                sqlCommand.Parameters.AddWithValue("@ParkingSpaceID", ParkingSpaceID);
                sqlCommand.Parameters.AddWithValue("@PlateNumber", PlateNumber);
                sqlCommand.Parameters.AddWithValue("@SpaceStatus", DBNull.Value);
                sqlCommand.Parameters.AddWithValue("@state", stateName);

                //sqlCommand.Parameters.AddWithValue("@ReceiptId", ReceiptId);

                //sqlCommand.Parameters.AddWithValue("@RateNumber", DBNull.Value);

                sqlCommand.Connection = connection;
                sqlCommand.CommandTimeout = 120;
                connection.Open();
                using (var reader = sqlCommand.ExecuteReader())
                {
                    while (reader.Read())
                    {
                        var item = new Transaction();
                        item.EnforcementKey = reader["EnforcementKey"] == System.DBNull.Value ? null : (string)reader["EnforcementKey"];
                        item.CustomerId = (reader.GetInt64(reader.GetOrdinal("CustomerId"))).ToString();
                        item.ParkingSpaceID = reader["ParkingSpaceID"] == System.DBNull.Value ? default(int) : (int)reader["ParkingSpaceID"];
                        item.PlateNumber = reader["PlateNumber"] == System.DBNull.Value ? null : (string)reader["PlateNumber"];
                        item.MeterName = reader["MeterName"] == System.DBNull.Value ? null : (string)reader["MeterName"];
                        item.MeterId = reader["meterid"] == System.DBNull.Value ? null : (string)reader["meterid"];
                        item.Latitude = reader["Latitude"] == System.DBNull.Value ? default(double?) : (double)reader["Latitude"];
                        item.Latitude = reader["Longitude"] == System.DBNull.Value ? default(double?) : (double)reader["Longitude"];

                        item.LPLocation = reader["LPLocation"] == System.DBNull.Value ? null : (string)reader["LPLocation"];
                        item.VMake = null; // reader["VMake"] == System.DBNull.Value ? null : (string)reader["VMake"];
                        item.VModel = null; //reader["VModel"] == System.DBNull.Value ? null : (string)reader["VModel"];
                        item.VColour = null; //reader["VColour"] == System.DBNull.Value ? null : (string)reader["VColour"];
                        item.MeterStreet = reader["MeterStreet"] == System.DBNull.Value ? null : (string)reader["MeterStreet"];
                        item.Location = reader["Location"] == System.DBNull.Value ? null : (string)reader["Location"];

                        item.EnfVendorStallId = reader["EnfVendorStallId"] == System.DBNull.Value ? default(int?) : (Int32)reader["EnfVendorStallId"];
                        item.PresentMeterTime = reader["PresentMeterTime"] == System.DBNull.Value ? default(DateTime?) : (DateTime)reader["PresentMeterTime"];
                        item.SpaceStatus = reader["SpaceStatus"] == System.DBNull.Value ? null : (string)reader["SpaceStatus"];
                        item.SensorEventTime = reader["SensorEventTime"] == System.DBNull.Value ? default(DateTime?) : (DateTime)reader["SensorEventTime"];
                        //item.ExpiredMinutes = reader["ExpiredMinutes"] == System.DBNull.Value ? default(int?) : (Int32)reader["ExpiredMinutes"];
                        item.ExpiredMinutes = reader["ExpiredMinutes"] == System.DBNull.Value ? -1 : (Int32)reader["ExpiredMinutes"];
                        item.AmountIncents = default(decimal?); // reader["AmountIncents"] == System.DBNull.Value ? default(decimal?) : (decimal)reader["AmountIncents"];
                        item.ExpiryTime = reader["ExpiryTime"] == System.DBNull.Value ? default(DateTime?) : (DateTime)reader["ExpiryTime"];
                        item.TimeZoneId = reader["TimeZoneId"] == System.DBNull.Value ? default(int?) : (Int32)reader["TimeZoneId"];
                        //item.PayStationID = reader["PayStationID"] == System.DBNull.Value ? default(int?) : (Int32)reader["PayStationID"];
                        item.PayStationID = reader["meterid"] == System.DBNull.Value ? default(int?) : Convert.ToInt32(reader["meterid"]);
                        item.ZoneID = reader["ZoneID"] == System.DBNull.Value ? default(int?) : (Int32)reader["ZoneID"];
                        item.ZoneName = reader["ZoneName"] == System.DBNull.Value ? null : (string)reader["ZoneName"];
                        item.Block = reader["Block"] == System.DBNull.Value ? null : (string)reader["Block"];
                        item.Direction = reader["Direction"] == System.DBNull.Value ? null : (string)reader["Direction"];
                        item.MeterStreet = reader["MeterStreet"] == System.DBNull.Value ? null : (string)reader["MeterStreet"];
                        item.StreetType = reader["StreetType"] == System.DBNull.Value ? null : (string)reader["StreetType"];
                        item.MeterStreet = reader["MeterStreet"] == System.DBNull.Value ? null : (string)reader["MeterStreet"];
                        item.CrossStreet1 = reader["CrossStreet1"] == System.DBNull.Value ? null : (string)reader["CrossStreet1"];

                        item.CrossStreet2 = reader["CrossStreet2"] == System.DBNull.Value ? null : (string)reader["CrossStreet2"];
                        item.DigiELType = default(int?); //reader["DigiELType"] == System.DBNull.Value ? default(int?) : (Int32)reader["DigiELType"];
                        item.DigiErrorDescr = null; // reader["DigiErrorDescr"] == System.DBNull.Value ? null : (string)reader["DigiErrorDescr"];
                        item.GeneticELType = default(int?); // reader["GeneticELType"] == System.DBNull.Value ? default(int?) : (Int32)reader["GeneticELType"];
                        item.GeneticErrorDescr = null; //reader["GeneticErrorDescr"] == System.DBNull.Value ? null : (string)reader["GeneticErrorDescr"];

                        if (reader["PMELType"] == System.DBNull.Value)
                        {
                            item.PMELType = default(int?);
                        }
                        else
                        {
                            string pMELTypeValue = reader["PMELType"].ToString();
                            if (string.IsNullOrWhiteSpace(pMELTypeValue))
                            {
                                item.PMELType = default(int?);
                            }
                            else
                            {
                                item.PMELType = (Int32)reader["PMELType"];
                            }
                        }



                        item.PMErrorDescr = null; //reader["PMErrorDescr"] == System.DBNull.Value ? null : (string)reader["PMErrorDescr"];
                        item.EnfHourDesc = reader["EnfHourDesc"] == System.DBNull.Value ? null : (string)reader["EnfHourDesc"];
                        item.MarkedSince = reader["MarkedSince"] == System.DBNull.Value ? default(DateTime?) : (DateTime)reader["MarkedSince"];
                        item.HitDescription = reader["HitDescription"] == System.DBNull.Value ? null : (string)reader["HitDescription"];


                        if (DataRecordExtensions.HasColumn(reader, "Metertype"))
                            item.METERLOC_METERTYPE = reader["Metertype"] == System.DBNull.Value ? null : (string)reader["Metertype"];
                        if (DataRecordExtensions.HasColumn(reader, "ZoneName"))
                            item.ZoneName = reader["ZoneName"] == System.DBNull.Value ? null : (string)reader["ZoneName"];


                        if (DataRecordExtensions.HasColumn(reader, "Recieptid"))
                            item.Recieptid = reader["Recieptid"] == System.DBNull.Value ? null : (string)reader["Recieptid"];

                        if (DataRecordExtensions.HasColumn(reader, "RateNumber"))
                            item.RateNumber = reader["RateNumber"] == System.DBNull.Value ? null : (string)reader["RateNumber"];


                        validatedData.Add(item);
                    }
                }
            }
            catch (Exception ex)
            {
                throw;
            }
            finally
            {
                if (connection.State == ConnectionState.Open)
                    connection.Close();
            }
            return validatedData;
        }
        #endregion

        #region Detroit
        public static List<Transaction> GetDetroitCitationData(int customerID, string ZoneID, string MeterName, string ParkingSpaceID, string PlateNumber, string stateName, string SpaceStatus)
        {
            List<Transaction> validatedData = new List<Transaction>();
            SqlConnection connection = null;
            try
            {
                connection = new SqlConnection(System.Configuration.ConfigurationManager.ConnectionStrings["Connector"].ToString());
                SqlCommand sqlCommand = new SqlCommand();
                sqlCommand.CommandType = CommandType.StoredProcedure;

                //sqlCommand.CommandText = "[SpacePlateTxnFinal_Railegh_Metername_CoralGables]";
                if (string.IsNullOrEmpty(ZoneID))
                    sqlCommand.CommandText = "[SpacePlateTxnFinal_Railegh_Metername_CoralGables]";
                else
                    sqlCommand.CommandText = "[SpacePlateTxnFinal_Railegh_Metername_CoralGablesZone]";
                sqlCommand.Parameters.AddWithValue("@customerID", customerID);
                sqlCommand.Parameters.AddWithValue("@ZoneID", ZoneID);
                sqlCommand.Parameters.AddWithValue("@MeterName", MeterName);
                sqlCommand.Parameters.AddWithValue("@ParkingSpaceID", ParkingSpaceID);
                sqlCommand.Parameters.AddWithValue("@PlateNumber", PlateNumber);
                sqlCommand.Parameters.AddWithValue("@SpaceStatus", DBNull.Value);

                //sqlCommand.Parameters.AddWithValue("@ReceiptId", ReceiptId);
                //sqlCommand.Parameters.AddWithValue("@State", DBNull.Value);
                //sqlCommand.Parameters.AddWithValue("@RateNumber", DBNull.Value);

                sqlCommand.Connection = connection;
                sqlCommand.CommandTimeout = 30;
                connection.Open();
                using (var reader = sqlCommand.ExecuteReader())
                {
                    while (reader.Read())
                    {
                        var item = new Transaction();
                        item.EnforcementKey = reader["EnforcementKey"] == System.DBNull.Value ? null : (string)reader["EnforcementKey"];
                        item.CustomerId = (reader.GetInt64(reader.GetOrdinal("CustomerId"))).ToString();
                        item.ParkingSpaceID = reader["ParkingSpaceID"] == System.DBNull.Value ? default(int) : (int)reader["ParkingSpaceID"];
                        item.PlateNumber = reader["PlateNumber"] == System.DBNull.Value ? null : (string)reader["PlateNumber"];
                        item.MeterName = reader["MeterName"] == System.DBNull.Value ? null : (string)reader["MeterName"];
                        //item.MeterId = reader["meterid"] == System.DBNull.Value ? null : (string)reader["meterid"];
                        item.Latitude = reader["Latitude"] == System.DBNull.Value ? default(double?) : (double)reader["Latitude"];
                        item.Latitude = reader["Longitude"] == System.DBNull.Value ? default(double?) : (double)reader["Longitude"];

                        item.LPLocation = reader["LPLocation"] == System.DBNull.Value ? null : (string)reader["LPLocation"];
                        item.VMake = null; // reader["VMake"] == System.DBNull.Value ? null : (string)reader["VMake"];
                        item.VModel = null; //reader["VModel"] == System.DBNull.Value ? null : (string)reader["VModel"];
                        item.VColour = null; //reader["VColour"] == System.DBNull.Value ? null : (string)reader["VColour"];
                        item.MeterStreet = reader["MeterStreet"] == System.DBNull.Value ? null : (string)reader["MeterStreet"];
                        item.Location = reader["Location"] == System.DBNull.Value ? null : (string)reader["Location"];

                        item.EnfVendorStallId = reader["EnfVendorStallId"] == System.DBNull.Value ? default(int?) : (Int32)reader["EnfVendorStallId"];
                        item.PresentMeterTime = reader["PresentMeterTime"] == System.DBNull.Value ? default(DateTime?) : (DateTime)reader["PresentMeterTime"];
                        item.SpaceStatus = reader["SpaceStatus"] == System.DBNull.Value ? null : (string)reader["SpaceStatus"];
                        item.SensorEventTime = reader["SensorEventTime"] == System.DBNull.Value ? default(DateTime?) : (DateTime)reader["SensorEventTime"];
                        //item.ExpiredMinutes = reader["ExpiredMinutes"] == System.DBNull.Value ? default(int?) : (Int32)reader["ExpiredMinutes"];
                        item.ExpiredMinutes = reader["ExpiredMinutes"] == System.DBNull.Value ? -1 : (Int32)reader["ExpiredMinutes"];
                        item.AmountIncents = default(decimal?); // reader["AmountIncents"] == System.DBNull.Value ? default(decimal?) : (decimal)reader["AmountIncents"];
                        item.ExpiryTime = reader["ExpiryTime"] == System.DBNull.Value ? default(DateTime?) : (DateTime)reader["ExpiryTime"];
                        item.TimeZoneId = reader["TimeZoneId"] == System.DBNull.Value ? default(int?) : (Int32)reader["TimeZoneId"];
                        //item.PayStationID = reader["PayStationID"] == System.DBNull.Value ? default(int?) : (Int32)reader["PayStationID"];
                        item.PayStationID = reader["meterid"] == System.DBNull.Value ? default(int?) : Convert.ToInt32(reader["meterid"]);
                        item.ZoneID = reader["ZoneID"] == System.DBNull.Value ? default(int?) : (Int32)reader["ZoneID"];
                        item.ZoneName = reader["ZoneName"] == System.DBNull.Value ? null : (string)reader["ZoneName"];
                        item.Block = reader["Block"] == System.DBNull.Value ? null : (string)reader["Block"];
                        item.Direction = reader["Direction"] == System.DBNull.Value ? null : (string)reader["Direction"];
                        item.MeterStreet = reader["MeterStreet"] == System.DBNull.Value ? null : (string)reader["MeterStreet"];
                        item.StreetType = reader["StreetType"] == System.DBNull.Value ? null : (string)reader["StreetType"];
                        item.MeterStreet = reader["MeterStreet"] == System.DBNull.Value ? null : (string)reader["MeterStreet"];
                        item.CrossStreet1 = reader["CrossStreet1"] == System.DBNull.Value ? null : (string)reader["CrossStreet1"];

                        item.CrossStreet2 = reader["CrossStreet2"] == System.DBNull.Value ? null : (string)reader["CrossStreet2"];
                        item.DigiELType = default(int?); //reader["DigiELType"] == System.DBNull.Value ? default(int?) : (Int32)reader["DigiELType"];
                        item.DigiErrorDescr = null; // reader["DigiErrorDescr"] == System.DBNull.Value ? null : (string)reader["DigiErrorDescr"];
                        item.GeneticELType = default(int?); // reader["GeneticELType"] == System.DBNull.Value ? default(int?) : (Int32)reader["GeneticELType"];
                        item.GeneticErrorDescr = null; //reader["GeneticErrorDescr"] == System.DBNull.Value ? null : (string)reader["GeneticErrorDescr"];

                        if (reader["PMELType"] == System.DBNull.Value)
                        {
                            item.PMELType = default(int?);
                        }
                        else
                        {
                            string pMELTypeValue = reader["PMELType"].ToString();
                            if (string.IsNullOrWhiteSpace(pMELTypeValue))
                            {
                                item.PMELType = default(int?);
                            }
                            else
                            {
                                item.PMELType = (Int32)reader["PMELType"];
                            }
                        }



                        item.PMErrorDescr = null; //reader["PMErrorDescr"] == System.DBNull.Value ? null : (string)reader["PMErrorDescr"];
                        item.EnfHourDesc = reader["EnfHourDesc"] == System.DBNull.Value ? null : (string)reader["EnfHourDesc"];
                        item.MarkedSince = reader["MarkedSince"] == System.DBNull.Value ? default(DateTime?) : (DateTime)reader["MarkedSince"];
                        item.HitDescription = reader["HitDescription"] == System.DBNull.Value ? null : (string)reader["HitDescription"];


                        if (DataRecordExtensions.HasColumn(reader, "Metertype"))
                            item.METERLOC_METERTYPE = reader["Metertype"] == System.DBNull.Value ? null : (string)reader["Metertype"];
                        if (DataRecordExtensions.HasColumn(reader, "ZoneName"))
                            item.ZoneName = reader["ZoneName"] == System.DBNull.Value ? null : (string)reader["ZoneName"];


                        if (DataRecordExtensions.HasColumn(reader, "Recieptid"))
                            item.Recieptid = reader["Recieptid"] == System.DBNull.Value ? null : (string)reader["Recieptid"];

                        if (DataRecordExtensions.HasColumn(reader, "RateNumber"))
                            item.RateNumber = reader["RateNumber"] == System.DBNull.Value ? null : (string)reader["RateNumber"];


                        validatedData.Add(item);
                    }
                }
            }
            catch (Exception ex)
            {
                throw;
            }
            finally
            {
                if (connection.State == ConnectionState.Open)
                    connection.Close();
            }
            return validatedData;
        }
        #endregion

        #region SiouxCity
        public static List<Transaction> GetSiouxCityCitationData(int customerID, string ZoneID, string MeterName, string ParkingSpaceID, string PlateNumber, string SpaceStatus)
        {
            List<Transaction> validatedData = new List<Transaction>();
            SqlConnection connection = null;
            try
            {
                connection = new SqlConnection(System.Configuration.ConfigurationManager.ConnectionStrings["Connector"].ToString());
                SqlCommand sqlCommand = new SqlCommand();
                sqlCommand.CommandType = CommandType.StoredProcedure;
                //sqlCommand.CommandText = "SpacePlateTxnFinal_Multispace";
                //sqlCommand.CommandText = "SpacePlateTxnFinal_siouxcity";
                sqlCommand.CommandText = "SpacePlateTxnFinal_siouxcity_Plate";
                sqlCommand.Parameters.AddWithValue("@customerID", customerID);
                sqlCommand.Parameters.AddWithValue("@ZoneID", ZoneID);
                //sqlCommand.Parameters.AddWithValue("@MeterName", MeterName);
                sqlCommand.Parameters.AddWithValue("@MeterName", MeterName);
                sqlCommand.Parameters.AddWithValue("@ParkingSpaceID", ParkingSpaceID);
                sqlCommand.Parameters.AddWithValue("@PlateNumber", PlateNumber);
                sqlCommand.Parameters.AddWithValue("@SpaceStatus", SpaceStatus);
                sqlCommand.Connection = connection;
                sqlCommand.CommandTimeout = 120;
                connection.Open();
                using (var reader = sqlCommand.ExecuteReader())
                {
                    while (reader.Read())
                    {
                        var item = new Transaction();
                        item.EnforcementKey = reader["EnforcementKey"] == System.DBNull.Value ? null : (string)reader["EnforcementKey"];
                        item.CustomerId = (reader.GetInt64(reader.GetOrdinal("CustomerId"))).ToString();
                        item.ParkingSpaceID = reader["ParkingSpaceID"] == System.DBNull.Value ? default(int) : (int)reader["ParkingSpaceID"];
                        item.PlateNumber = reader["PlateNumber"] == System.DBNull.Value ? null : (string)reader["PlateNumber"];
                        item.MeterName = reader["MeterName"] == System.DBNull.Value ? null : (string)reader["MeterName"];

                        item.Latitude = reader["Latitude"] == System.DBNull.Value ? default(double?) : (double)reader["Latitude"];
                        item.Latitude = reader["Longitude"] == System.DBNull.Value ? default(double?) : (double)reader["Longitude"];

                        item.LPLocation = reader["LPLocation"] == System.DBNull.Value ? null : (string)reader["LPLocation"];
                        item.VMake = reader["VMake"] == System.DBNull.Value ? null : (string)reader["VMake"];
                        item.VModel = reader["VModel"] == System.DBNull.Value ? null : (string)reader["VModel"];
                        item.VColour = reader["VColour"] == System.DBNull.Value ? null : (string)reader["VColour"];
                        item.MeterStreet = reader["MeterStreet"] == System.DBNull.Value ? null : (string)reader["MeterStreet"];
                        item.Location = reader["Location"] == System.DBNull.Value ? null : (string)reader["Location"];

                        item.EnfVendorStallId = reader["EnfVendorStallId"] == System.DBNull.Value ? default(int?) : (Int32)reader["EnfVendorStallId"];
                        item.PresentMeterTime = reader["PresentMeterTime"] == System.DBNull.Value ? default(DateTime?) : (DateTime)reader["PresentMeterTime"];
                        item.SpaceStatus = reader["SpaceStatus"] == System.DBNull.Value ? null : (string)reader["SpaceStatus"];
                        item.SensorEventTime = reader["SensorEventTime"] == System.DBNull.Value ? default(DateTime?) : (DateTime)reader["SensorEventTime"];
                        //item.ExpiredMinutes = reader["ExpiredMinutes"] == System.DBNull.Value ? default(int?) : (Int32)reader["ExpiredMinutes"];
                        item.ExpiredMinutes = reader["ExpiredMinutes"] == System.DBNull.Value ? -1 : (Int32)reader["ExpiredMinutes"];
                        item.AmountIncents = reader["AmountIncents"] == System.DBNull.Value ? default(decimal?) : (decimal)reader["AmountIncents"];
                        item.ExpiryTime = reader["ExpiryTime"] == System.DBNull.Value ? default(DateTime?) : (DateTime)reader["ExpiryTime"];
                        item.TimeZoneId = reader["TimeZoneId"] == System.DBNull.Value ? default(int?) : (Int32)reader["TimeZoneId"];
                        item.PayStationID = reader["PayStationID"] == System.DBNull.Value ? default(int?) : (Int32)reader["PayStationID"];
                        item.ZoneID = reader["ZoneID"] == System.DBNull.Value ? default(int?) : (Int32)reader["ZoneID"];
                        //item.ZoneName = reader["ZoneName"] == System.DBNull.Value ? null : (string)reader["ZoneName"];
                        item.ZoneName = null;
                        item.Block = reader["Block"] == System.DBNull.Value ? null : (string)reader["Block"];
                        item.Direction = reader["Direction"] == System.DBNull.Value ? null : (string)reader["Direction"];
                        item.MeterStreet = reader["MeterStreet"] == System.DBNull.Value ? null : (string)reader["MeterStreet"];
                        item.StreetType = reader["StreetType"] == System.DBNull.Value ? null : (string)reader["StreetType"];
                        item.MeterStreet = reader["MeterStreet"] == System.DBNull.Value ? null : (string)reader["MeterStreet"];
                        item.CrossStreet1 = reader["CrossStreet1"] == System.DBNull.Value ? null : (string)reader["CrossStreet1"];

                        item.CrossStreet2 = reader["CrossStreet2"] == System.DBNull.Value ? null : (string)reader["CrossStreet2"];
                        item.DigiELType = reader["DigiELType"] == System.DBNull.Value ? default(int?) : (Int32)reader["DigiELType"];
                        item.DigiErrorDescr = reader["DigiErrorDescr"] == System.DBNull.Value ? null : (string)reader["DigiErrorDescr"];
                        // item.GeneticELType = reader["GeneticELType"] == System.DBNull.Value ? default(int?) : (Int32)reader["GeneticELType"];
                        //item.GeneticErrorDescr = reader["GeneticErrorDescr"] == System.DBNull.Value ? null : (string)reader["GeneticErrorDescr"];
                        //item.PMELType = reader["PMELType"] == System.DBNull.Value ? default(int?) : (Int32)reader["PMELType"];
                        //item.PMErrorDescr = reader["PMErrorDescr"] == System.DBNull.Value ? null : (string)reader["PMErrorDescr"];
                        //item.EnfHourDesc = reader["EnfHourDesc"] == System.DBNull.Value ? null : (string)reader["EnfHourDesc"];
                        //item.MarkedSince = reader["MarkedSince"] == System.DBNull.Value ? default(DateTime?) : (DateTime)reader["MarkedSince"];
                        //item.HitDescription = reader["HitDescription"] == System.DBNull.Value ? null : (string)reader["HitDescription"];
                        validatedData.Add(item);
                    }
                }
            }
            catch (Exception ex)
            {
                throw;
            }
            finally
            {
                if (connection.State == ConnectionState.Open)
                    connection.Close();
            }
            return validatedData;
        }

        public static List<Transaction> GetSiouxCityCitationDatZone(int customerID, string ZoneID, string MeterName, string ParkingSpaceID, string PlateNumber, string SpaceStatus)
        {
            List<Transaction> validatedData = new List<Transaction>();
            SqlConnection connection = null;
            try
            {
                connection = new SqlConnection(System.Configuration.ConfigurationManager.ConnectionStrings["Connector"].ToString());
                SqlCommand sqlCommand = new SqlCommand();
                sqlCommand.CommandType = CommandType.StoredProcedure;
                //sqlCommand.CommandText = "SpacePlateTxnFinal_Multispace";
                //sqlCommand.CommandText = "SpacePlateTxnFinal_siouxcity";
                sqlCommand.CommandText = "SpacePlateTxnFinal_siouxcity_Plate";
                sqlCommand.Parameters.AddWithValue("@customerID", customerID);
                sqlCommand.Parameters.AddWithValue("@ZoneID", ZoneID);
                //sqlCommand.Parameters.AddWithValue("@MeterName", MeterName);
                sqlCommand.Parameters.AddWithValue("@MeterName", MeterName);
                sqlCommand.Parameters.AddWithValue("@ParkingSpaceID", ParkingSpaceID);
                sqlCommand.Parameters.AddWithValue("@PlateNumber", PlateNumber);
                sqlCommand.Parameters.AddWithValue("@SpaceStatus", SpaceStatus);
                sqlCommand.Connection = connection;
                sqlCommand.CommandTimeout = 120;
                connection.Open();
                using (var reader = sqlCommand.ExecuteReader())
                {
                    while (reader.Read())
                    {
                        var item = new Transaction();
                        item.EnforcementKey = reader["EnforcementKey"] == System.DBNull.Value ? null : (string)reader["EnforcementKey"];
                        item.CustomerId = (reader.GetInt64(reader.GetOrdinal("CustomerId"))).ToString();
                        item.ParkingSpaceID = reader["ParkingSpaceID"] == System.DBNull.Value ? default(int) : (int)reader["ParkingSpaceID"];
                        item.PlateNumber = reader["PlateNumber"] == System.DBNull.Value ? null : (string)reader["PlateNumber"];
                        item.MeterName = reader["MeterName"] == System.DBNull.Value ? null : (string)reader["MeterName"];

                        item.Latitude = reader["Latitude"] == System.DBNull.Value ? default(double?) : (double)reader["Latitude"];
                        item.Latitude = reader["Longitude"] == System.DBNull.Value ? default(double?) : (double)reader["Longitude"];

                        item.LPLocation = reader["LPLocation"] == System.DBNull.Value ? null : (string)reader["LPLocation"];
                        item.VMake = reader["VMake"] == System.DBNull.Value ? null : (string)reader["VMake"];
                        item.VModel = reader["VModel"] == System.DBNull.Value ? null : (string)reader["VModel"];
                        item.VColour = reader["VColour"] == System.DBNull.Value ? null : (string)reader["VColour"];
                        item.MeterStreet = reader["MeterStreet"] == System.DBNull.Value ? null : (string)reader["MeterStreet"];
                        item.Location = reader["Location"] == System.DBNull.Value ? null : (string)reader["Location"];

                        item.EnfVendorStallId = reader["EnfVendorStallId"] == System.DBNull.Value ? default(int?) : (Int32)reader["EnfVendorStallId"];
                        item.PresentMeterTime = reader["PresentMeterTime"] == System.DBNull.Value ? default(DateTime?) : (DateTime)reader["PresentMeterTime"];
                        item.SpaceStatus = reader["SpaceStatus"] == System.DBNull.Value ? null : (string)reader["SpaceStatus"];
                        item.SensorEventTime = reader["SensorEventTime"] == System.DBNull.Value ? default(DateTime?) : (DateTime)reader["SensorEventTime"];
                        //item.ExpiredMinutes = reader["ExpiredMinutes"] == System.DBNull.Value ? default(int?) : (Int32)reader["ExpiredMinutes"];
                        item.ExpiredMinutes = reader["ExpiredMinutes"] == System.DBNull.Value ? -1 : (Int32)reader["ExpiredMinutes"];
                        item.AmountIncents = reader["AmountIncents"] == System.DBNull.Value ? default(decimal?) : (decimal)reader["AmountIncents"];
                        item.ExpiryTime = reader["ExpiryTime"] == System.DBNull.Value ? default(DateTime?) : (DateTime)reader["ExpiryTime"];
                        item.TimeZoneId = reader["TimeZoneId"] == System.DBNull.Value ? default(int?) : (Int32)reader["TimeZoneId"];
                        item.PayStationID = reader["PayStationID"] == System.DBNull.Value ? default(int?) : (Int32)reader["PayStationID"];
                        item.ZoneID = reader["ZoneID"] == System.DBNull.Value ? default(int?) : (Int32)reader["ZoneID"];
                        item.ZoneName = reader["ZoneName"] == System.DBNull.Value ? null : (string)reader["ZoneName"];
                        item.Block = reader["Block"] == System.DBNull.Value ? null : (string)reader["Block"];
                        item.Direction = reader["Direction"] == System.DBNull.Value ? null : (string)reader["Direction"];
                        item.MeterStreet = reader["MeterStreet"] == System.DBNull.Value ? null : (string)reader["MeterStreet"];
                        item.StreetType = reader["StreetType"] == System.DBNull.Value ? null : (string)reader["StreetType"];
                        item.MeterStreet = reader["MeterStreet"] == System.DBNull.Value ? null : (string)reader["MeterStreet"];
                        item.CrossStreet1 = reader["CrossStreet1"] == System.DBNull.Value ? null : (string)reader["CrossStreet1"];

                        item.CrossStreet2 = reader["CrossStreet2"] == System.DBNull.Value ? null : (string)reader["CrossStreet2"];
                        item.DigiELType = reader["DigiELType"] == System.DBNull.Value ? default(int?) : (Int32)reader["DigiELType"];
                        item.DigiErrorDescr = reader["DigiErrorDescr"] == System.DBNull.Value ? null : (string)reader["DigiErrorDescr"];
                        // item.GeneticELType = reader["GeneticELType"] == System.DBNull.Value ? default(int?) : (Int32)reader["GeneticELType"];
                        //item.GeneticErrorDescr = reader["GeneticErrorDescr"] == System.DBNull.Value ? null : (string)reader["GeneticErrorDescr"];
                        //item.PMELType = reader["PMELType"] == System.DBNull.Value ? default(int?) : (Int32)reader["PMELType"];
                        //item.PMErrorDescr = reader["PMErrorDescr"] == System.DBNull.Value ? null : (string)reader["PMErrorDescr"];
                        //item.EnfHourDesc = reader["EnfHourDesc"] == System.DBNull.Value ? null : (string)reader["EnfHourDesc"];
                        //item.MarkedSince = reader["MarkedSince"] == System.DBNull.Value ? default(DateTime?) : (DateTime)reader["MarkedSince"];
                        //item.HitDescription = reader["HitDescription"] == System.DBNull.Value ? null : (string)reader["HitDescription"];
                        validatedData.Add(item);
                    }
                }
            }
            catch (Exception ex)
            {
                throw;
            }
            finally
            {
                if (connection.State == ConnectionState.Open)
                    connection.Close();
            }
            return validatedData;
        }
        #endregion


        #region RoyalOak
        public static List<Transaction> GetRoyalOakCitationData(int customerID, string ZoneID, string MeterName, string ParkingSpaceID, string PlateNumber, string SpaceStatus)
        {
            List<Transaction> validatedData = new List<Transaction>();
            SqlConnection connection = null;
            try
            {
                connection = new SqlConnection(System.Configuration.ConfigurationManager.ConnectionStrings["Connector"].ToString());
                SqlCommand sqlCommand = new SqlCommand();
                sqlCommand.CommandType = CommandType.StoredProcedure;
                //sqlCommand.CommandText = "SpacePlateTxnFinal_Multispace";
                sqlCommand.CommandText = "SpacePlateTxnFinal_Multispace_Plate";
                sqlCommand.Parameters.AddWithValue("@customerID", customerID);
                sqlCommand.Parameters.AddWithValue("@ZoneID", ZoneID);
                //sqlCommand.Parameters.AddWithValue("@MeterName", MeterName);
                sqlCommand.Parameters.AddWithValue("@MeterName", DBNull.Value);
                sqlCommand.Parameters.AddWithValue("@ParkingSpaceID", DBNull.Value);
                sqlCommand.Parameters.AddWithValue("@PlateNumber", PlateNumber);
                sqlCommand.Parameters.AddWithValue("@SpaceStatus", SpaceStatus);
                sqlCommand.Connection = connection;
                sqlCommand.CommandTimeout = 120;
                connection.Open();
                using (var reader = sqlCommand.ExecuteReader())
                {
                    while (reader.Read())
                    {
                        var item = new Transaction();
                        item.EnforcementKey = reader["EnforcementKey"] == System.DBNull.Value ? null : (string)reader["EnforcementKey"];
                        item.CustomerId = (reader.GetInt64(reader.GetOrdinal("CustomerId"))).ToString();
                        item.ParkingSpaceID = reader["ParkingSpaceID"] == System.DBNull.Value ? default(int) : (int)reader["ParkingSpaceID"];
                        item.PlateNumber = reader["PlateNumber"] == System.DBNull.Value ? null : (string)reader["PlateNumber"];
                        item.MeterName = reader["MeterName"] == System.DBNull.Value ? null : (string)reader["MeterName"];

                        item.Latitude = reader["Latitude"] == System.DBNull.Value ? default(double?) : (double)reader["Latitude"];
                        item.Latitude = reader["Longitude"] == System.DBNull.Value ? default(double?) : (double)reader["Longitude"];

                        item.LPLocation = reader["LPLocation"] == System.DBNull.Value ? null : (string)reader["LPLocation"];
                        item.VMake = reader["VMake"] == System.DBNull.Value ? null : (string)reader["VMake"];
                        item.VModel = reader["VModel"] == System.DBNull.Value ? null : (string)reader["VModel"];
                        item.VColour = reader["VColour"] == System.DBNull.Value ? null : (string)reader["VColour"];
                        item.MeterStreet = reader["MeterStreet"] == System.DBNull.Value ? null : (string)reader["MeterStreet"];
                        item.Location = reader["Location"] == System.DBNull.Value ? null : (string)reader["Location"];

                        item.EnfVendorStallId = reader["EnfVendorStallId"] == System.DBNull.Value ? default(int?) : (Int32)reader["EnfVendorStallId"];
                        item.PresentMeterTime = reader["PresentMeterTime"] == System.DBNull.Value ? default(DateTime?) : (DateTime)reader["PresentMeterTime"];
                        item.SpaceStatus = reader["SpaceStatus"] == System.DBNull.Value ? null : (string)reader["SpaceStatus"];
                        item.SensorEventTime = reader["SensorEventTime"] == System.DBNull.Value ? default(DateTime?) : (DateTime)reader["SensorEventTime"];
                        //item.ExpiredMinutes = reader["ExpiredMinutes"] == System.DBNull.Value ? default(int?) : (Int32)reader["ExpiredMinutes"];
                        item.ExpiredMinutes = reader["ExpiredMinutes"] == System.DBNull.Value ? -1 : (Int32)reader["ExpiredMinutes"];
                        item.AmountIncents = reader["AmountIncents"] == System.DBNull.Value ? default(decimal?) : (decimal)reader["AmountIncents"];
                        item.ExpiryTime = reader["ExpiryTime"] == System.DBNull.Value ? default(DateTime?) : (DateTime)reader["ExpiryTime"];
                        item.TimeZoneId = reader["TimeZoneId"] == System.DBNull.Value ? default(int?) : (Int32)reader["TimeZoneId"];
                        item.PayStationID = reader["PayStationID"] == System.DBNull.Value ? default(int?) : (Int32)reader["PayStationID"];
                        item.ZoneID = reader["ZoneID"] == System.DBNull.Value ? default(int?) : (Int32)reader["ZoneID"];
                        //item.ZoneName = reader["ZoneName"] == System.DBNull.Value ? null : (string)reader["ZoneName"];
                        item.ZoneName = null;
                        item.Block = reader["Block"] == System.DBNull.Value ? null : (string)reader["Block"];
                        item.Direction = reader["Direction"] == System.DBNull.Value ? null : (string)reader["Direction"];
                        item.MeterStreet = reader["MeterStreet"] == System.DBNull.Value ? null : (string)reader["MeterStreet"];
                        item.StreetType = reader["StreetType"] == System.DBNull.Value ? null : (string)reader["StreetType"];
                        item.MeterStreet = reader["MeterStreet"] == System.DBNull.Value ? null : (string)reader["MeterStreet"];
                        item.CrossStreet1 = reader["CrossStreet1"] == System.DBNull.Value ? null : (string)reader["CrossStreet1"];

                        item.CrossStreet2 = reader["CrossStreet2"] == System.DBNull.Value ? null : (string)reader["CrossStreet2"];
                        item.DigiELType = reader["DigiELType"] == System.DBNull.Value ? default(int?) : (Int32)reader["DigiELType"];
                        item.DigiErrorDescr = reader["DigiErrorDescr"] == System.DBNull.Value ? null : (string)reader["DigiErrorDescr"];
                        // item.GeneticELType = reader["GeneticELType"] == System.DBNull.Value ? default(int?) : (Int32)reader["GeneticELType"];
                        //item.GeneticErrorDescr = reader["GeneticErrorDescr"] == System.DBNull.Value ? null : (string)reader["GeneticErrorDescr"];
                        //item.PMELType = reader["PMELType"] == System.DBNull.Value ? default(int?) : (Int32)reader["PMELType"];
                        //item.PMErrorDescr = reader["PMErrorDescr"] == System.DBNull.Value ? null : (string)reader["PMErrorDescr"];
                        //item.EnfHourDesc = reader["EnfHourDesc"] == System.DBNull.Value ? null : (string)reader["EnfHourDesc"];
                        //item.MarkedSince = reader["MarkedSince"] == System.DBNull.Value ? default(DateTime?) : (DateTime)reader["MarkedSince"];
                        //item.HitDescription = reader["HitDescription"] == System.DBNull.Value ? null : (string)reader["HitDescription"];
                        validatedData.Add(item);
                    }
                }
            }
            catch (Exception ex)
            {
                throw;
            }
            finally
            {
                if (connection.State == ConnectionState.Open)
                    connection.Close();
            }
            return validatedData;
        }
        #endregion

        #region Tempe
        public static List<Transaction> GetTempeCitationData(int customerID, string ZoneID, string MeterName, string ParkingSpaceID, string PlateNumber, string SpaceStatus)
        {
            List<Transaction> validatedData = new List<Transaction>();
            SqlConnection connection = null;
            try
            {
                connection = new SqlConnection(System.Configuration.ConfigurationManager.ConnectionStrings["Connector"].ToString());
                SqlCommand sqlCommand = new SqlCommand();
                sqlCommand.CommandType = CommandType.StoredProcedure;
                //sqlCommand.CommandText = "SpacePlateTxnFinal_Multispace";
                sqlCommand.CommandText = "SpacePlateTxnFinal_Multispace_Plate";
                sqlCommand.Parameters.AddWithValue("@customerID", customerID);
                sqlCommand.Parameters.AddWithValue("@ZoneID", ZoneID);
                //sqlCommand.Parameters.AddWithValue("@MeterName", MeterName);
                sqlCommand.Parameters.AddWithValue("@MeterName", DBNull.Value);
                sqlCommand.Parameters.AddWithValue("@ParkingSpaceID", DBNull.Value);
                sqlCommand.Parameters.AddWithValue("@PlateNumber", PlateNumber);
                sqlCommand.Parameters.AddWithValue("@SpaceStatus", SpaceStatus);
                sqlCommand.Connection = connection;
                sqlCommand.CommandTimeout = 120;
                connection.Open();
                using (var reader = sqlCommand.ExecuteReader())
                {
                    while (reader.Read())
                    {
                        var item = new Transaction();
                        item.EnforcementKey = reader["EnforcementKey"] == System.DBNull.Value ? null : (string)reader["EnforcementKey"];
                        item.CustomerId = (reader.GetInt64(reader.GetOrdinal("CustomerId"))).ToString();
                        item.ParkingSpaceID = reader["ParkingSpaceID"] == System.DBNull.Value ? default(int) : (int)reader["ParkingSpaceID"];
                        item.PlateNumber = reader["PlateNumber"] == System.DBNull.Value ? null : (string)reader["PlateNumber"];
                        item.MeterName = reader["MeterName"] == System.DBNull.Value ? null : (string)reader["MeterName"];

                        item.Latitude = reader["Latitude"] == System.DBNull.Value ? default(double?) : (double)reader["Latitude"];
                        item.Latitude = reader["Longitude"] == System.DBNull.Value ? default(double?) : (double)reader["Longitude"];

                        item.LPLocation = reader["LPLocation"] == System.DBNull.Value ? null : (string)reader["LPLocation"];
                        item.VMake = reader["VMake"] == System.DBNull.Value ? null : (string)reader["VMake"];
                        item.VModel = reader["VModel"] == System.DBNull.Value ? null : (string)reader["VModel"];
                        item.VColour = reader["VColour"] == System.DBNull.Value ? null : (string)reader["VColour"];
                        item.MeterStreet = reader["MeterStreet"] == System.DBNull.Value ? null : (string)reader["MeterStreet"];
                        item.Location = reader["Location"] == System.DBNull.Value ? null : (string)reader["Location"];

                        item.EnfVendorStallId = reader["EnfVendorStallId"] == System.DBNull.Value ? default(int?) : (Int32)reader["EnfVendorStallId"];
                        item.PresentMeterTime = reader["PresentMeterTime"] == System.DBNull.Value ? default(DateTime?) : (DateTime)reader["PresentMeterTime"];
                        item.SpaceStatus = reader["SpaceStatus"] == System.DBNull.Value ? null : (string)reader["SpaceStatus"];
                        item.SensorEventTime = reader["SensorEventTime"] == System.DBNull.Value ? default(DateTime?) : (DateTime)reader["SensorEventTime"];
                        //item.ExpiredMinutes = reader["ExpiredMinutes"] == System.DBNull.Value ? default(int?) : (Int32)reader["ExpiredMinutes"];
                        item.ExpiredMinutes = reader["ExpiredMinutes"] == System.DBNull.Value ? -1 : (Int32)reader["ExpiredMinutes"];
                        item.AmountIncents = reader["AmountIncents"] == System.DBNull.Value ? default(decimal?) : (decimal)reader["AmountIncents"];
                        item.ExpiryTime = reader["ExpiryTime"] == System.DBNull.Value ? default(DateTime?) : (DateTime)reader["ExpiryTime"];
                        item.TimeZoneId = reader["TimeZoneId"] == System.DBNull.Value ? default(int?) : (Int32)reader["TimeZoneId"];
                        item.PayStationID = reader["PayStationID"] == System.DBNull.Value ? default(int?) : (Int32)reader["PayStationID"];
                        item.ZoneID = reader["ZoneID"] == System.DBNull.Value ? default(int?) : (Int32)reader["ZoneID"];
                        //item.ZoneName = reader["ZoneName"] == System.DBNull.Value ? null : (string)reader["ZoneName"];
                        item.ZoneName = null;
                        item.Block = reader["Block"] == System.DBNull.Value ? null : (string)reader["Block"];
                        item.Direction = reader["Direction"] == System.DBNull.Value ? null : (string)reader["Direction"];
                        item.MeterStreet = reader["MeterStreet"] == System.DBNull.Value ? null : (string)reader["MeterStreet"];
                        item.StreetType = reader["StreetType"] == System.DBNull.Value ? null : (string)reader["StreetType"];
                        item.MeterStreet = reader["MeterStreet"] == System.DBNull.Value ? null : (string)reader["MeterStreet"];
                        item.CrossStreet1 = reader["CrossStreet1"] == System.DBNull.Value ? null : (string)reader["CrossStreet1"];

                        item.CrossStreet2 = reader["CrossStreet2"] == System.DBNull.Value ? null : (string)reader["CrossStreet2"];
                        item.DigiELType = reader["DigiELType"] == System.DBNull.Value ? default(int?) : (Int32)reader["DigiELType"];
                        item.DigiErrorDescr = reader["DigiErrorDescr"] == System.DBNull.Value ? null : (string)reader["DigiErrorDescr"];
                        // item.GeneticELType = reader["GeneticELType"] == System.DBNull.Value ? default(int?) : (Int32)reader["GeneticELType"];
                        //item.GeneticErrorDescr = reader["GeneticErrorDescr"] == System.DBNull.Value ? null : (string)reader["GeneticErrorDescr"];
                        //item.PMELType = reader["PMELType"] == System.DBNull.Value ? default(int?) : (Int32)reader["PMELType"];
                        //item.PMErrorDescr = reader["PMErrorDescr"] == System.DBNull.Value ? null : (string)reader["PMErrorDescr"];
                        //item.EnfHourDesc = reader["EnfHourDesc"] == System.DBNull.Value ? null : (string)reader["EnfHourDesc"];
                        //item.MarkedSince = reader["MarkedSince"] == System.DBNull.Value ? default(DateTime?) : (DateTime)reader["MarkedSince"];
                        //item.HitDescription = reader["HitDescription"] == System.DBNull.Value ? null : (string)reader["HitDescription"];
                        validatedData.Add(item);
                    }
                }
            }
            catch (Exception ex)
            {
                throw;
            }
            finally
            {
                if (connection.State == ConnectionState.Open)
                    connection.Close();
            }
            return validatedData;
        }
        #endregion

        #region Obsolet
        public static List<Transaction> GetCitationData(int customerID, string ZoneID, string MeterName, string ParkingSpaceID, string PlateNumber, string SpaceStatus)
        {
            List<Transaction> validatedData = new List<Transaction>();
            SqlConnection connection = null;
            try
            {
                connection = new SqlConnection(System.Configuration.ConfigurationManager.ConnectionStrings["Connector"].ToString());
                SqlCommand sqlCommand = new SqlCommand();
                sqlCommand.CommandType = CommandType.StoredProcedure;
                sqlCommand.CommandText = "SpacePlateTxnFinal";
                sqlCommand.Parameters.AddWithValue("@customerID", customerID);
                sqlCommand.Parameters.AddWithValue("@ZoneID", ZoneID);
                //sqlCommand.Parameters.AddWithValue("@MeterName", MeterName);
                sqlCommand.Parameters.AddWithValue("@MeterName", MeterName);
                sqlCommand.Parameters.AddWithValue("@ParkingSpaceID", ParkingSpaceID);
                sqlCommand.Parameters.AddWithValue("@PlateNumber", PlateNumber);
                sqlCommand.Parameters.AddWithValue("@SpaceStatus", SpaceStatus);
                sqlCommand.Connection = connection;
                sqlCommand.CommandTimeout = 120;
                connection.Open();
                using (var reader = sqlCommand.ExecuteReader())
                {
                    while (reader.Read())
                    {
                        var item = new Transaction();
                        item.EnforcementKey = reader["EnforcementKey"] == System.DBNull.Value ? null : (string)reader["EnforcementKey"];
                        item.CustomerId = (reader.GetInt64(reader.GetOrdinal("CustomerId"))).ToString();
                        item.ParkingSpaceID = reader["ParkingSpaceID"] == System.DBNull.Value ? default(int) : (int)reader["ParkingSpaceID"];
                        item.PlateNumber = reader["PlateNumber"] == System.DBNull.Value ? null : (string)reader["PlateNumber"];
                        item.MeterName = reader["MeterName"] == System.DBNull.Value ? null : (string)reader["MeterName"];

                        item.Latitude = reader["Latitude"] == System.DBNull.Value ? default(double?) : (double)reader["Latitude"];
                        item.Latitude = reader["Longitude"] == System.DBNull.Value ? default(double?) : (double)reader["Longitude"];

                        item.LPLocation = reader["LPLocation"] == System.DBNull.Value ? null : (string)reader["LPLocation"];
                        item.VMake = reader["VMake"] == System.DBNull.Value ? null : (string)reader["VMake"];
                        item.VModel = reader["VModel"] == System.DBNull.Value ? null : (string)reader["VModel"];
                        item.VColour = reader["VColour"] == System.DBNull.Value ? null : (string)reader["VColour"];
                        item.MeterStreet = reader["MeterStreet"] == System.DBNull.Value ? null : (string)reader["MeterStreet"];
                        item.Location = reader["Location"] == System.DBNull.Value ? null : (string)reader["Location"];

                        item.EnfVendorStallId = reader["EnfVendorStallId"] == System.DBNull.Value ? default(int?) : (Int32)reader["EnfVendorStallId"];
                        item.PresentMeterTime = reader["PresentMeterTime"] == System.DBNull.Value ? default(DateTime?) : (DateTime)reader["PresentMeterTime"];
                        item.SpaceStatus = reader["SpaceStatus"] == System.DBNull.Value ? null : (string)reader["SpaceStatus"];
                        item.SensorEventTime = reader["SensorEventTime"] == System.DBNull.Value ? default(DateTime?) : (DateTime)reader["SensorEventTime"];
                        // item.ExpiredMinutes = reader["ExpiredMinutes"] == System.DBNull.Value ? default(int?) : (Int32)reader["ExpiredMinutes"];
                        item.ExpiredMinutes = reader["ExpiredMinutes"] == System.DBNull.Value ? -1 : (Int32)reader["ExpiredMinutes"];
                        item.AmountIncents = reader["AmountIncents"] == System.DBNull.Value ? default(decimal?) : (decimal)reader["AmountIncents"];
                        item.ExpiryTime = reader["ExpiryTime"] == System.DBNull.Value ? default(DateTime?) : (DateTime)reader["ExpiryTime"];
                        item.TimeZoneId = reader["TimeZoneId"] == System.DBNull.Value ? default(int?) : (Int32)reader["TimeZoneId"];
                        item.PayStationID = reader["PayStationID"] == System.DBNull.Value ? default(int?) : (Int32)reader["PayStationID"];
                        item.ZoneID = reader["ZoneID"] == System.DBNull.Value ? default(int?) : (Int32)reader["ZoneID"];
                        item.ZoneName = reader["ZoneName"] == System.DBNull.Value ? null : (string)reader["ZoneName"];
                        //item.ZoneName = null;
                        item.Block = reader["Block"] == System.DBNull.Value ? null : (string)reader["Block"];
                        item.Direction = reader["Direction"] == System.DBNull.Value ? null : (string)reader["Direction"];
                        item.MeterStreet = reader["MeterStreet"] == System.DBNull.Value ? null : (string)reader["MeterStreet"];
                        item.StreetType = reader["StreetType"] == System.DBNull.Value ? null : (string)reader["StreetType"];
                        item.MeterStreet = reader["MeterStreet"] == System.DBNull.Value ? null : (string)reader["MeterStreet"];
                        item.CrossStreet1 = reader["CrossStreet1"] == System.DBNull.Value ? null : (string)reader["CrossStreet1"];

                        item.CrossStreet2 = reader["CrossStreet2"] == System.DBNull.Value ? null : (string)reader["CrossStreet2"];
                        item.DigiELType = reader["DigiELType"] == System.DBNull.Value ? default(int?) : (Int32)reader["DigiELType"];
                        item.DigiErrorDescr = reader["DigiErrorDescr"] == System.DBNull.Value ? null : (string)reader["DigiErrorDescr"];
                        item.GeneticELType = reader["GeneticELType"] == System.DBNull.Value ? default(int?) : (Int32)reader["GeneticELType"];
                        item.GeneticErrorDescr = reader["GeneticErrorDescr"] == System.DBNull.Value ? null : (string)reader["GeneticErrorDescr"];
                        item.PMELType = reader["PMELType"] == System.DBNull.Value ? default(int?) : (Int32)reader["PMELType"];
                        item.PMErrorDescr = reader["PMErrorDescr"] == System.DBNull.Value ? null : (string)reader["PMErrorDescr"];
                        item.EnfHourDesc = reader["EnfHourDesc"] == System.DBNull.Value ? null : (string)reader["EnfHourDesc"];
                        item.MarkedSince = reader["MarkedSince"] == System.DBNull.Value ? default(DateTime?) : (DateTime)reader["MarkedSince"];
                        item.HitDescription = reader["HitDescription"] == System.DBNull.Value ? null : (string)reader["HitDescription"];
                        validatedData.Add(item);
                    }
                }
            }
            catch (Exception ex)
            {
                throw;
            }
            finally
            {
                if (connection.State == ConnectionState.Open)
                    connection.Close();
            }
            return validatedData;
        }

        public static List<Transaction> GetCitationDataOthers(int customerID, string ZoneID, string MeterName, string ParkingSpaceID, string PlateNumber, string SpaceStatus)
        {
            List<Transaction> validatedData = new List<Transaction>();
            SqlConnection connection = null;
            try
            {
                connection = new SqlConnection(System.Configuration.ConfigurationManager.ConnectionStrings["Connector"].ToString());
                SqlCommand sqlCommand = new SqlCommand();
                sqlCommand.CommandType = CommandType.StoredProcedure;
                //sqlCommand.CommandText = "SpacePlateTxnFinal_railegh";
                sqlCommand.CommandText = "[SpacePlateTxnFinal_Railegh_Metername_Test]";
                sqlCommand.Parameters.AddWithValue("@customerID", customerID);
                sqlCommand.Parameters.AddWithValue("@ZoneID", ZoneID);
                sqlCommand.Parameters.AddWithValue("@MeterName", MeterName);
                sqlCommand.Parameters.AddWithValue("@ParkingSpaceID", ParkingSpaceID);
                sqlCommand.Parameters.AddWithValue("@PlateNumber", PlateNumber);
                sqlCommand.Parameters.AddWithValue("@SpaceStatus", SpaceStatus);
                sqlCommand.Connection = connection;
                sqlCommand.CommandTimeout = 120;
                connection.Open();
                using (var reader = sqlCommand.ExecuteReader())
                {
                    while (reader.Read())
                    {
                        var item = new Transaction();
                        item.EnforcementKey = reader["EnforcementKey"] == System.DBNull.Value ? null : (string)reader["EnforcementKey"];
                        item.CustomerId = (reader.GetInt64(reader.GetOrdinal("CustomerId"))).ToString();
                        item.ParkingSpaceID = reader["ParkingSpaceID"] == System.DBNull.Value ? default(int) : (int)reader["ParkingSpaceID"];
                        item.PlateNumber = reader["PlateNumber"] == System.DBNull.Value ? null : (string)reader["PlateNumber"];
                        item.MeterName = reader["MeterName"] == System.DBNull.Value ? null : (string)reader["MeterName"];
                        item.MeterId = reader["meterid"] == System.DBNull.Value ? null : (string)reader["meterid"];
                        item.Latitude = reader["Latitude"] == System.DBNull.Value ? default(double?) : (double)reader["Latitude"];
                        item.Latitude = reader["Longitude"] == System.DBNull.Value ? default(double?) : (double)reader["Longitude"];

                        item.LPLocation = reader["LPLocation"] == System.DBNull.Value ? null : (string)reader["LPLocation"];
                        item.VMake = reader["VMake"] == System.DBNull.Value ? null : (string)reader["VMake"];
                        item.VModel = reader["VModel"] == System.DBNull.Value ? null : (string)reader["VModel"];
                        item.VColour = reader["VColour"] == System.DBNull.Value ? null : (string)reader["VColour"];
                        item.MeterStreet = reader["MeterStreet"] == System.DBNull.Value ? null : (string)reader["MeterStreet"];
                        item.Location = reader["Location"] == System.DBNull.Value ? null : (string)reader["Location"];

                        item.EnfVendorStallId = reader["EnfVendorStallId"] == System.DBNull.Value ? default(int?) : (Int32)reader["EnfVendorStallId"];
                        item.PresentMeterTime = reader["PresentMeterTime"] == System.DBNull.Value ? default(DateTime?) : (DateTime)reader["PresentMeterTime"];
                        item.SpaceStatus = reader["SpaceStatus"] == System.DBNull.Value ? null : (string)reader["SpaceStatus"];
                        item.SensorEventTime = reader["SensorEventTime"] == System.DBNull.Value ? default(DateTime?) : (DateTime)reader["SensorEventTime"];
                        // item.ExpiredMinutes = reader["ExpiredMinutes"] == System.DBNull.Value ? default(int?) : (Int32)reader["ExpiredMinutes"];
                        item.ExpiredMinutes = reader["ExpiredMinutes"] == System.DBNull.Value ? -1 : (Int32)reader["ExpiredMinutes"];
                        item.AmountIncents = reader["AmountIncents"] == System.DBNull.Value ? default(decimal?) : (decimal)reader["AmountIncents"];
                        item.ExpiryTime = reader["ExpiryTime"] == System.DBNull.Value ? default(DateTime?) : (DateTime)reader["ExpiryTime"];
                        item.TimeZoneId = reader["TimeZoneId"] == System.DBNull.Value ? default(int?) : (Int32)reader["TimeZoneId"];
                        //item.PayStationID = reader["PayStationID"] == System.DBNull.Value ? default(int?) : (Int32)reader["PayStationID"];
                        item.PayStationID = reader["meterid"] == System.DBNull.Value ? default(int?) : Convert.ToInt32(reader["meterid"]);
                        item.ZoneID = reader["ZoneID"] == System.DBNull.Value ? default(int?) : (Int32)reader["ZoneID"];
                        item.ZoneName = reader["ZoneName"] == System.DBNull.Value ? null : (string)reader["ZoneName"];
                        item.Block = reader["Block"] == System.DBNull.Value ? null : (string)reader["Block"];
                        item.Direction = reader["Direction"] == System.DBNull.Value ? null : (string)reader["Direction"];
                        item.MeterStreet = reader["MeterStreet"] == System.DBNull.Value ? null : (string)reader["MeterStreet"];
                        item.StreetType = reader["StreetType"] == System.DBNull.Value ? null : (string)reader["StreetType"];
                        item.MeterStreet = reader["MeterStreet"] == System.DBNull.Value ? null : (string)reader["MeterStreet"];
                        item.CrossStreet1 = reader["CrossStreet1"] == System.DBNull.Value ? null : (string)reader["CrossStreet1"];

                        item.CrossStreet2 = reader["CrossStreet2"] == System.DBNull.Value ? null : (string)reader["CrossStreet2"];
                        item.DigiELType = reader["DigiELType"] == System.DBNull.Value ? default(int?) : (Int32)reader["DigiELType"];
                        item.DigiErrorDescr = reader["DigiErrorDescr"] == System.DBNull.Value ? null : (string)reader["DigiErrorDescr"];
                        item.GeneticELType = reader["GeneticELType"] == System.DBNull.Value ? default(int?) : (Int32)reader["GeneticELType"];
                        item.GeneticErrorDescr = reader["GeneticErrorDescr"] == System.DBNull.Value ? null : (string)reader["GeneticErrorDescr"];

                        if (reader["PMELType"] == System.DBNull.Value)
                        {
                            item.PMELType = default(int?);
                        }
                        else
                        {
                            string pMELTypeValue = reader["PMELType"].ToString();
                            if (string.IsNullOrWhiteSpace(pMELTypeValue))
                            {
                                item.PMELType = default(int?);
                            }
                            else
                            {
                                item.PMELType = (Int32)reader["PMELType"];
                            }
                        }



                        item.PMErrorDescr = reader["PMErrorDescr"] == System.DBNull.Value ? null : (string)reader["PMErrorDescr"];
                        item.EnfHourDesc = reader["EnfHourDesc"] == System.DBNull.Value ? null : (string)reader["EnfHourDesc"];
                        item.MarkedSince = reader["MarkedSince"] == System.DBNull.Value ? default(DateTime?) : (DateTime)reader["MarkedSince"];
                        item.HitDescription = reader["HitDescription"] == System.DBNull.Value ? null : (string)reader["HitDescription"];


                        if (DataRecordExtensions.HasColumn(reader, "Metertype"))
                            item.METERLOC_METERTYPE = reader["Metertype"] == System.DBNull.Value ? null : (string)reader["Metertype"];
                        if (DataRecordExtensions.HasColumn(reader, "ZoneName"))
                            item.ZoneName = reader["ZoneName"] == System.DBNull.Value ? null : (string)reader["ZoneName"];

                        //if (DataRecordExtensions.HasColumn(reader, "MeterStatus"))
                        //    item.MeterStatus = reader["MeterStatus"] == System.DBNull.Value ? default(int?) : (Int32)reader["MeterStatus"];

                        validatedData.Add(item);
                    }
                }
            }
            catch (Exception ex)
            {
                throw;
            }
            finally
            {
                if (connection.State == ConnectionState.Open)
                    connection.Close();
            }
            return validatedData;
        }
        #endregion


        #region Bay Harbor
        public static List<Transaction> GetBayHarborCitationData(int customerID, string ZoneID, string MeterName, string ParkingSpaceID, string PlateNumber, string stateName, string SpaceStatus)
        {
            List<Transaction> validatedData = new List<Transaction>();
            SqlConnection connection = null;
            try
            {
                connection = new SqlConnection(System.Configuration.ConfigurationManager.ConnectionStrings["Connector"].ToString());
                SqlCommand sqlCommand = new SqlCommand();
                sqlCommand.CommandType = CommandType.StoredProcedure;
                sqlCommand.CommandText = "[Sp_PaybyPlateDetails_RefreshService]";
                sqlCommand.Parameters.AddWithValue("@customerID", customerID);
                sqlCommand.Parameters.AddWithValue("@ZoneID", ZoneID);
                sqlCommand.Parameters.AddWithValue("@MeterName", MeterName);
                sqlCommand.Parameters.AddWithValue("@ParkingSpaceID", ParkingSpaceID);
                sqlCommand.Parameters.AddWithValue("@PlateNumber", PlateNumber);
                sqlCommand.Parameters.AddWithValue("@SpaceStatus", DBNull.Value);

                //sqlCommand.Parameters.AddWithValue("@ReceiptId", ReceiptId);
                //sqlCommand.Parameters.AddWithValue("@State", DBNull.Value);
                //sqlCommand.Parameters.AddWithValue("@RateNumber", DBNull.Value);

                sqlCommand.Connection = connection;
                sqlCommand.CommandTimeout = 120;
                connection.Open();
                using (var reader = sqlCommand.ExecuteReader())
                {
                    while (reader.Read())
                    {
                        var item = new Transaction();
                        item.EnforcementKey = reader["EnforcementKey"] == System.DBNull.Value ? null : (string)reader["EnforcementKey"];
                        item.CustomerId = (reader.GetInt64(reader.GetOrdinal("CustomerId"))).ToString();
                        item.ParkingSpaceID = reader["ParkingSpaceID"] == System.DBNull.Value ? default(int) : (int)reader["ParkingSpaceID"];
                        item.PlateNumber = reader["PlateNumber"] == System.DBNull.Value ? null : (string)reader["PlateNumber"];
                        item.MeterName = reader["MeterName"] == System.DBNull.Value ? null : (string)reader["MeterName"];
                        item.MeterId = reader["meterid"] == System.DBNull.Value ? null : (string)reader["meterid"];
                        item.Latitude = reader["Latitude"] == System.DBNull.Value ? default(double?) : (double)reader["Latitude"];
                        item.Latitude = reader["Longitude"] == System.DBNull.Value ? default(double?) : (double)reader["Longitude"];

                        item.LPLocation = reader["LPLocation"] == System.DBNull.Value ? null : (string)reader["LPLocation"];
                        item.VMake = null; // reader["VMake"] == System.DBNull.Value ? null : (string)reader["VMake"];
                        item.VModel = null; //reader["VModel"] == System.DBNull.Value ? null : (string)reader["VModel"];
                        item.VColour = null; //reader["VColour"] == System.DBNull.Value ? null : (string)reader["VColour"];
                        item.MeterStreet = reader["MeterStreet"] == System.DBNull.Value ? null : (string)reader["MeterStreet"];
                        item.Location = reader["Location"] == System.DBNull.Value ? null : (string)reader["Location"];

                        item.EnfVendorStallId = reader["EnfVendorStallId"] == System.DBNull.Value ? default(int?) : (Int32)reader["EnfVendorStallId"];
                        item.PresentMeterTime = reader["PresentMeterTime"] == System.DBNull.Value ? default(DateTime?) : (DateTime)reader["PresentMeterTime"];
                        item.SpaceStatus = reader["SpaceStatus"] == System.DBNull.Value ? null : (string)reader["SpaceStatus"];
                        item.SensorEventTime = reader["SensorEventTime"] == System.DBNull.Value ? default(DateTime?) : (DateTime)reader["SensorEventTime"];
                        //item.ExpiredMinutes = reader["ExpiredMinutes"] == System.DBNull.Value ? default(int?) : (Int32)reader["ExpiredMinutes"];
                        item.ExpiredMinutes = reader["ExpiredMinutes"] == System.DBNull.Value ? -1 : (Int32)reader["ExpiredMinutes"];
                        item.AmountIncents = default(decimal?); // reader["AmountIncents"] == System.DBNull.Value ? default(decimal?) : (decimal)reader["AmountIncents"];
                        item.ExpiryTime = reader["ExpiryTime"] == System.DBNull.Value ? default(DateTime?) : (DateTime)reader["ExpiryTime"];
                        item.TimeZoneId = reader["TimeZoneId"] == System.DBNull.Value ? default(int?) : (Int32)reader["TimeZoneId"];
                        //item.PayStationID = reader["PayStationID"] == System.DBNull.Value ? default(int?) : (Int32)reader["PayStationID"];
                        item.PayStationID = reader["meterid"] == System.DBNull.Value ? default(int?) : Convert.ToInt32(reader["meterid"]);
                        item.ZoneID = reader["ZoneID"] == System.DBNull.Value ? default(int?) : (Int32)reader["ZoneID"];
                        item.ZoneName = reader["ZoneName"] == System.DBNull.Value ? null : (string)reader["ZoneName"];
                        item.Block = reader["Block"] == System.DBNull.Value ? null : (string)reader["Block"];
                        item.Direction = reader["Direction"] == System.DBNull.Value ? null : (string)reader["Direction"];
                        item.MeterStreet = reader["MeterStreet"] == System.DBNull.Value ? null : (string)reader["MeterStreet"];
                        item.StreetType = reader["StreetType"] == System.DBNull.Value ? null : (string)reader["StreetType"];
                        item.MeterStreet = reader["MeterStreet"] == System.DBNull.Value ? null : (string)reader["MeterStreet"];
                        item.CrossStreet1 = reader["CrossStreet1"] == System.DBNull.Value ? null : (string)reader["CrossStreet1"];

                        item.CrossStreet2 = reader["CrossStreet2"] == System.DBNull.Value ? null : (string)reader["CrossStreet2"];
                        item.DigiELType = default(int?); //reader["DigiELType"] == System.DBNull.Value ? default(int?) : (Int32)reader["DigiELType"];
                        item.DigiErrorDescr = null; // reader["DigiErrorDescr"] == System.DBNull.Value ? null : (string)reader["DigiErrorDescr"];
                        item.GeneticELType = default(int?); // reader["GeneticELType"] == System.DBNull.Value ? default(int?) : (Int32)reader["GeneticELType"];
                        item.GeneticErrorDescr = null; //reader["GeneticErrorDescr"] == System.DBNull.Value ? null : (string)reader["GeneticErrorDescr"];

                        if (reader["PMELType"] == System.DBNull.Value)
                        {
                            item.PMELType = default(int?);
                        }
                        else
                        {
                            string pMELTypeValue = reader["PMELType"].ToString();
                            if (string.IsNullOrWhiteSpace(pMELTypeValue))
                            {
                                item.PMELType = default(int?);
                            }
                            else
                            {
                                item.PMELType = (Int32)reader["PMELType"];
                            }
                        }



                        item.PMErrorDescr = null; //reader["PMErrorDescr"] == System.DBNull.Value ? null : (string)reader["PMErrorDescr"];
                        item.EnfHourDesc = reader["EnfHourDesc"] == System.DBNull.Value ? null : (string)reader["EnfHourDesc"];
                        item.MarkedSince = reader["MarkedSince"] == System.DBNull.Value ? default(DateTime?) : (DateTime)reader["MarkedSince"];
                        item.HitDescription = reader["HitDescription"] == System.DBNull.Value ? null : (string)reader["HitDescription"];


                        if (DataRecordExtensions.HasColumn(reader, "Metertype"))
                            item.METERLOC_METERTYPE = reader["Metertype"] == System.DBNull.Value ? null : (string)reader["Metertype"];
                        if (DataRecordExtensions.HasColumn(reader, "ZoneName"))
                            item.ZoneName = reader["ZoneName"] == System.DBNull.Value ? null : (string)reader["ZoneName"];


                        if (DataRecordExtensions.HasColumn(reader, "Recieptid"))
                            item.Recieptid = reader["Recieptid"] == System.DBNull.Value ? null : (string)reader["Recieptid"];

                        if (DataRecordExtensions.HasColumn(reader, "RateNumber"))
                            item.RateNumber = reader["RateNumber"] == System.DBNull.Value ? null : (string)reader["RateNumber"];


                        validatedData.Add(item);
                    }
                }
            }
            catch (Exception ex)
            {
                throw;
            }
            finally
            {
                if (connection.State == ConnectionState.Open)
                    connection.Close();
            }
            return validatedData;
        }
        #endregion

        #region Metro Rail
        public static List<Transaction> GetMetroRailCitationData(int customerID, string ZoneID, string MeterName, string ParkingSpaceID, string PlateNumber, string stateName, string SpaceStatus)
        {
            List<Transaction> validatedData = new List<Transaction>();
            SqlConnection connection = null;
            try
            {
                connection = new SqlConnection(System.Configuration.ConfigurationManager.ConnectionStrings["Connector"].ToString());
                SqlCommand sqlCommand = new SqlCommand();
                sqlCommand.CommandType = CommandType.StoredProcedure;
                sqlCommand.CommandText = "[Sp_PaybyPlateDetails]";
                sqlCommand.Parameters.AddWithValue("@customerID", customerID);
                sqlCommand.Parameters.AddWithValue("@ZoneID", ZoneID);
                sqlCommand.Parameters.AddWithValue("@MeterName", MeterName);
                sqlCommand.Parameters.AddWithValue("@ParkingSpaceID", ParkingSpaceID);
                sqlCommand.Parameters.AddWithValue("@PlateNumber", PlateNumber);
                sqlCommand.Parameters.AddWithValue("@SpaceStatus", DBNull.Value);

                //sqlCommand.Parameters.AddWithValue("@ReceiptId", ReceiptId);
                //sqlCommand.Parameters.AddWithValue("@State", DBNull.Value);
                //sqlCommand.Parameters.AddWithValue("@RateNumber", DBNull.Value);

                sqlCommand.Connection = connection;
                sqlCommand.CommandTimeout = 120;
                connection.Open();
                using (var reader = sqlCommand.ExecuteReader())
                {
                    while (reader.Read())
                    {
                        var item = new Transaction();
                        item.EnforcementKey = reader["EnforcementKey"] == System.DBNull.Value ? null : (string)reader["EnforcementKey"];
                        item.CustomerId = (reader.GetInt64(reader.GetOrdinal("CustomerId"))).ToString();
                        item.ParkingSpaceID = reader["ParkingSpaceID"] == System.DBNull.Value ? default(int) : (int)reader["ParkingSpaceID"];
                        item.PlateNumber = reader["PlateNumber"] == System.DBNull.Value ? null : (string)reader["PlateNumber"];
                        item.MeterName = reader["MeterName"] == System.DBNull.Value ? null : (string)reader["MeterName"];
                        item.MeterId = reader["meterid"] == System.DBNull.Value ? null : (string)reader["meterid"];
                        item.Latitude = reader["Latitude"] == System.DBNull.Value ? default(double?) : (double)reader["Latitude"];
                        item.Latitude = reader["Longitude"] == System.DBNull.Value ? default(double?) : (double)reader["Longitude"];

                        item.LPLocation = reader["LPLocation"] == System.DBNull.Value ? null : (string)reader["LPLocation"];
                        item.VMake = null; // reader["VMake"] == System.DBNull.Value ? null : (string)reader["VMake"];
                        item.VModel = null; //reader["VModel"] == System.DBNull.Value ? null : (string)reader["VModel"];
                        item.VColour = null; //reader["VColour"] == System.DBNull.Value ? null : (string)reader["VColour"];
                        item.MeterStreet = reader["MeterStreet"] == System.DBNull.Value ? null : (string)reader["MeterStreet"];
                        item.Location = reader["Location"] == System.DBNull.Value ? null : (string)reader["Location"];

                        item.EnfVendorStallId = reader["EnfVendorStallId"] == System.DBNull.Value ? default(int?) : (Int32)reader["EnfVendorStallId"];
                        item.PresentMeterTime = reader["PresentMeterTime"] == System.DBNull.Value ? default(DateTime?) : (DateTime)reader["PresentMeterTime"];
                        item.SpaceStatus = reader["SpaceStatus"] == System.DBNull.Value ? null : (string)reader["SpaceStatus"];
                        item.SensorEventTime = reader["SensorEventTime"] == System.DBNull.Value ? default(DateTime?) : (DateTime)reader["SensorEventTime"];
                        //item.ExpiredMinutes = reader["ExpiredMinutes"] == System.DBNull.Value ? default(int?) : (Int32)reader["ExpiredMinutes"];
                        item.ExpiredMinutes = reader["ExpiredMinutes"] == System.DBNull.Value ? -1 : (Int32)reader["ExpiredMinutes"];
                        item.AmountIncents = default(decimal?); // reader["AmountIncents"] == System.DBNull.Value ? default(decimal?) : (decimal)reader["AmountIncents"];
                        item.ExpiryTime = reader["ExpiryTime"] == System.DBNull.Value ? default(DateTime?) : (DateTime)reader["ExpiryTime"];
                        item.TimeZoneId = reader["TimeZoneId"] == System.DBNull.Value ? default(int?) : (Int32)reader["TimeZoneId"];
                        //item.PayStationID = reader["PayStationID"] == System.DBNull.Value ? default(int?) : (Int32)reader["PayStationID"];
                        item.PayStationID = reader["meterid"] == System.DBNull.Value ? default(int?) : Convert.ToInt32(reader["meterid"]);
                        item.ZoneID = reader["ZoneID"] == System.DBNull.Value ? default(int?) : (Int32)reader["ZoneID"];
                        item.ZoneName = reader["ZoneName"] == System.DBNull.Value ? null : (string)reader["ZoneName"];
                        item.Block = reader["Block"] == System.DBNull.Value ? null : (string)reader["Block"];
                        item.Direction = reader["Direction"] == System.DBNull.Value ? null : (string)reader["Direction"];
                        item.MeterStreet = reader["MeterStreet"] == System.DBNull.Value ? null : (string)reader["MeterStreet"];
                        item.StreetType = reader["StreetType"] == System.DBNull.Value ? null : (string)reader["StreetType"];
                        item.MeterStreet = reader["MeterStreet"] == System.DBNull.Value ? null : (string)reader["MeterStreet"];
                        item.CrossStreet1 = reader["CrossStreet1"] == System.DBNull.Value ? null : (string)reader["CrossStreet1"];

                        item.CrossStreet2 = reader["CrossStreet2"] == System.DBNull.Value ? null : (string)reader["CrossStreet2"];
                        item.DigiELType = default(int?); //reader["DigiELType"] == System.DBNull.Value ? default(int?) : (Int32)reader["DigiELType"];
                        item.DigiErrorDescr = null; // reader["DigiErrorDescr"] == System.DBNull.Value ? null : (string)reader["DigiErrorDescr"];
                        item.GeneticELType = default(int?); // reader["GeneticELType"] == System.DBNull.Value ? default(int?) : (Int32)reader["GeneticELType"];
                        item.GeneticErrorDescr = null; //reader["GeneticErrorDescr"] == System.DBNull.Value ? null : (string)reader["GeneticErrorDescr"];

                        if (reader["PMELType"] == System.DBNull.Value)
                        {
                            item.PMELType = default(int?);
                        }
                        else
                        {
                            string pMELTypeValue = reader["PMELType"].ToString();
                            if (string.IsNullOrWhiteSpace(pMELTypeValue))
                            {
                                item.PMELType = default(int?);
                            }
                            else
                            {
                                item.PMELType = (Int32)reader["PMELType"];
                            }
                        }



                        item.PMErrorDescr = null; //reader["PMErrorDescr"] == System.DBNull.Value ? null : (string)reader["PMErrorDescr"];
                        item.EnfHourDesc = reader["EnfHourDesc"] == System.DBNull.Value ? null : (string)reader["EnfHourDesc"];
                        item.MarkedSince = reader["MarkedSince"] == System.DBNull.Value ? default(DateTime?) : (DateTime)reader["MarkedSince"];
                        item.HitDescription = reader["HitDescription"] == System.DBNull.Value ? null : (string)reader["HitDescription"];


                        if (DataRecordExtensions.HasColumn(reader, "Metertype"))
                            item.METERLOC_METERTYPE = reader["Metertype"] == System.DBNull.Value ? null : (string)reader["Metertype"];
                        if (DataRecordExtensions.HasColumn(reader, "ZoneName"))
                            item.ZoneName = reader["ZoneName"] == System.DBNull.Value ? null : (string)reader["ZoneName"];


                        if (DataRecordExtensions.HasColumn(reader, "Recieptid"))
                            item.Recieptid = reader["Recieptid"] == System.DBNull.Value ? null : (string)reader["Recieptid"];

                        if (DataRecordExtensions.HasColumn(reader, "RateNumber"))
                            item.RateNumber = reader["RateNumber"] == System.DBNull.Value ? null : (string)reader["RateNumber"];


                        validatedData.Add(item);
                    }
                }
            }
            catch (Exception ex)
            {
                throw;
            }
            finally
            {
                if (connection.State == ConnectionState.Open)
                    connection.Close();
            }
            return validatedData;
        }
        #endregion

        #region Port Hood River
        public static List<Transaction> GetPortHoodRiverWithPlateName(int customerID, string ZoneID, string MeterName, string ParkingSpaceID, string PlateNumber, string stateName, string SpaceStatus)
        {
            List<Transaction> validatedData = new List<Transaction>();
            SqlConnection connection = null;
            try
            {
                connection = new SqlConnection(System.Configuration.ConfigurationManager.ConnectionStrings["Connector"].ToString());
                SqlCommand sqlCommand = new SqlCommand();
                sqlCommand.CommandType = CommandType.StoredProcedure;
                sqlCommand.CommandText = "[Sp_PaybyPlateDetails]";
                sqlCommand.Parameters.AddWithValue("@customerID", customerID);
                sqlCommand.Parameters.AddWithValue("@ZoneID", ZoneID);
                sqlCommand.Parameters.AddWithValue("@MeterName", MeterName);
                sqlCommand.Parameters.AddWithValue("@ParkingSpaceID", ParkingSpaceID);
                sqlCommand.Parameters.AddWithValue("@PlateNumber", PlateNumber);
                sqlCommand.Parameters.AddWithValue("@SpaceStatus", DBNull.Value);
                //sqlCommand.Parameters.AddWithValue("@state", stateName);

                sqlCommand.Connection = connection;
                sqlCommand.CommandTimeout = 120;
                connection.Open();
                using (var reader = sqlCommand.ExecuteReader())
                {
                    while (reader.Read())
                    {
                        var item = new Transaction();
                        item.EnforcementKey = reader["EnforcementKey"] == System.DBNull.Value ? null : (string)reader["EnforcementKey"];
                        item.CustomerId = (reader.GetInt64(reader.GetOrdinal("CustomerId"))).ToString();
                        item.ParkingSpaceID = reader["ParkingSpaceID"] == System.DBNull.Value ? default(int) : (int)reader["ParkingSpaceID"];
                        item.PlateNumber = reader["PlateNumber"] == System.DBNull.Value ? null : (string)reader["PlateNumber"];
                        item.MeterName = reader["MeterName"] == System.DBNull.Value ? null : (string)reader["MeterName"];
                        item.MeterId = reader["meterid"] == System.DBNull.Value ? null : (string)reader["meterid"];
                        item.Latitude = reader["Latitude"] == System.DBNull.Value ? default(double?) : (double)reader["Latitude"];
                        item.Latitude = reader["Longitude"] == System.DBNull.Value ? default(double?) : (double)reader["Longitude"];

                        item.LPLocation = reader["LPLocation"] == System.DBNull.Value ? null : (string)reader["LPLocation"];
                        item.VMake = null; // reader["VMake"] == System.DBNull.Value ? null : (string)reader["VMake"];
                        item.VModel = null; //reader["VModel"] == System.DBNull.Value ? null : (string)reader["VModel"];
                        item.VColour = null; //reader["VColour"] == System.DBNull.Value ? null : (string)reader["VColour"];
                        item.MeterStreet = reader["MeterStreet"] == System.DBNull.Value ? null : (string)reader["MeterStreet"];
                        item.Location = reader["Location"] == System.DBNull.Value ? null : (string)reader["Location"];

                        item.EnfVendorStallId = reader["EnfVendorStallId"] == System.DBNull.Value ? default(int?) : (Int32)reader["EnfVendorStallId"];
                        item.PresentMeterTime = reader["PresentMeterTime"] == System.DBNull.Value ? default(DateTime?) : (DateTime)reader["PresentMeterTime"];
                        item.SpaceStatus = reader["SpaceStatus"] == System.DBNull.Value ? null : (string)reader["SpaceStatus"];
                        item.SensorEventTime = reader["SensorEventTime"] == System.DBNull.Value ? default(DateTime?) : (DateTime)reader["SensorEventTime"];
                        //item.ExpiredMinutes = reader["ExpiredMinutes"] == System.DBNull.Value ? default(int?) : (Int32)reader["ExpiredMinutes"];
                        item.ExpiredMinutes = reader["ExpiredMinutes"] == System.DBNull.Value ? -1 : (Int32)reader["ExpiredMinutes"];
                        item.AmountIncents = default(decimal?); // reader["AmountIncents"] == System.DBNull.Value ? default(decimal?) : (decimal)reader["AmountIncents"];
                        item.ExpiryTime = reader["ExpiryTime"] == System.DBNull.Value ? default(DateTime?) : (DateTime)reader["ExpiryTime"];
                        item.TimeZoneId = reader["TimeZoneId"] == System.DBNull.Value ? default(int?) : (Int32)reader["TimeZoneId"];
                        //item.PayStationID = reader["PayStationID"] == System.DBNull.Value ? default(int?) : (Int32)reader["PayStationID"];
                        item.PayStationID = reader["meterid"] == System.DBNull.Value ? default(int?) : Convert.ToInt32(reader["meterid"]);
                        item.ZoneID = reader["ZoneID"] == System.DBNull.Value ? default(int?) : (Int32)reader["ZoneID"];
                        item.ZoneName = reader["ZoneName"] == System.DBNull.Value ? null : (string)reader["ZoneName"];
                        item.Block = reader["Block"] == System.DBNull.Value ? null : (string)reader["Block"];
                        item.Direction = reader["Direction"] == System.DBNull.Value ? null : (string)reader["Direction"];
                        item.MeterStreet = reader["MeterStreet"] == System.DBNull.Value ? null : (string)reader["MeterStreet"];
                        item.StreetType = reader["StreetType"] == System.DBNull.Value ? null : (string)reader["StreetType"];
                        item.MeterStreet = reader["MeterStreet"] == System.DBNull.Value ? null : (string)reader["MeterStreet"];
                        item.CrossStreet1 = reader["CrossStreet1"] == System.DBNull.Value ? null : (string)reader["CrossStreet1"];

                        item.CrossStreet2 = reader["CrossStreet2"] == System.DBNull.Value ? null : (string)reader["CrossStreet2"];
                        item.DigiELType = default(int?); //reader["DigiELType"] == System.DBNull.Value ? default(int?) : (Int32)reader["DigiELType"];
                        item.DigiErrorDescr = null; // reader["DigiErrorDescr"] == System.DBNull.Value ? null : (string)reader["DigiErrorDescr"];
                        item.GeneticELType = default(int?); // reader["GeneticELType"] == System.DBNull.Value ? default(int?) : (Int32)reader["GeneticELType"];
                        item.GeneticErrorDescr = null; //reader["GeneticErrorDescr"] == System.DBNull.Value ? null : (string)reader["GeneticErrorDescr"];

                        if (reader["PMELType"] == System.DBNull.Value)
                        {
                            item.PMELType = default(int?);
                        }
                        else
                        {
                            string pMELTypeValue = reader["PMELType"].ToString();
                            if (string.IsNullOrWhiteSpace(pMELTypeValue))
                            {
                                item.PMELType = default(int?);
                            }
                            else
                            {
                                item.PMELType = (Int32)reader["PMELType"];
                            }
                        }



                        item.PMErrorDescr = null; //reader["PMErrorDescr"] == System.DBNull.Value ? null : (string)reader["PMErrorDescr"];
                        item.EnfHourDesc = reader["EnfHourDesc"] == System.DBNull.Value ? null : (string)reader["EnfHourDesc"];
                        item.MarkedSince = reader["MarkedSince"] == System.DBNull.Value ? default(DateTime?) : (DateTime)reader["MarkedSince"];
                        item.HitDescription = reader["HitDescription"] == System.DBNull.Value ? null : (string)reader["HitDescription"];


                        if (DataRecordExtensions.HasColumn(reader, "Metertype"))
                            item.METERLOC_METERTYPE = reader["Metertype"] == System.DBNull.Value ? null : (string)reader["Metertype"];
                        if (DataRecordExtensions.HasColumn(reader, "ZoneName"))
                            item.ZoneName = reader["ZoneName"] == System.DBNull.Value ? null : (string)reader["ZoneName"];


                        if (DataRecordExtensions.HasColumn(reader, "Recieptid"))
                            item.Recieptid = reader["Recieptid"] == System.DBNull.Value ? null : (string)reader["Recieptid"];

                        if (DataRecordExtensions.HasColumn(reader, "RateNumber"))
                            item.RateNumber = reader["RateNumber"] == System.DBNull.Value ? null : (string)reader["RateNumber"];


                        validatedData.Add(item);
                    }
                }
            }
            catch (Exception ex)
            {
                throw;
            }
            finally
            {
                if (connection.State == ConnectionState.Open)
                    connection.Close();
            }
            return validatedData;
        }

        #endregion

        #region TybeeIsland
        public static List<Transaction> GetTybeeIslandWithPlateName(int customerID, string ZoneID, string MeterName, string ParkingSpaceID, string PlateNumber, string stateName, string SpaceStatus)
        {
            List<Transaction> validatedData = new List<Transaction>();
            SqlConnection connection = null;
            try
            {
                connection = new SqlConnection(System.Configuration.ConfigurationManager.ConnectionStrings["Connector"].ToString());
                SqlCommand sqlCommand = new SqlCommand();
                sqlCommand.CommandType = CommandType.StoredProcedure;
                sqlCommand.CommandText = "[Sp_PaybyPlateDetails]";
                sqlCommand.Parameters.AddWithValue("@customerID", customerID);
                sqlCommand.Parameters.AddWithValue("@ZoneID", ZoneID);
                sqlCommand.Parameters.AddWithValue("@MeterName", MeterName);
                sqlCommand.Parameters.AddWithValue("@ParkingSpaceID", ParkingSpaceID);
                sqlCommand.Parameters.AddWithValue("@PlateNumber", PlateNumber);
                sqlCommand.Parameters.AddWithValue("@SpaceStatus", DBNull.Value);
                //sqlCommand.Parameters.AddWithValue("@state", stateName);

                sqlCommand.Connection = connection;
                sqlCommand.CommandTimeout = 120;
                connection.Open();
                using (var reader = sqlCommand.ExecuteReader())
                {
                    while (reader.Read())
                    {
                        var item = new Transaction();
                        item.EnforcementKey = reader["EnforcementKey"] == System.DBNull.Value ? null : (string)reader["EnforcementKey"];
                        item.CustomerId = (reader.GetInt64(reader.GetOrdinal("CustomerId"))).ToString();
                        item.ParkingSpaceID = reader["ParkingSpaceID"] == System.DBNull.Value ? default(int) : (int)reader["ParkingSpaceID"];
                        item.PlateNumber = reader["PlateNumber"] == System.DBNull.Value ? null : (string)reader["PlateNumber"];
                        item.MeterName = reader["MeterName"] == System.DBNull.Value ? null : (string)reader["MeterName"];
                        item.MeterId = reader["meterid"] == System.DBNull.Value ? null : (string)reader["meterid"];
                        item.Latitude = reader["Latitude"] == System.DBNull.Value ? default(double?) : (double)reader["Latitude"];
                        item.Latitude = reader["Longitude"] == System.DBNull.Value ? default(double?) : (double)reader["Longitude"];

                        item.LPLocation = reader["LPLocation"] == System.DBNull.Value ? null : (string)reader["LPLocation"];
                        item.VMake = null; // reader["VMake"] == System.DBNull.Value ? null : (string)reader["VMake"];
                        item.VModel = null; //reader["VModel"] == System.DBNull.Value ? null : (string)reader["VModel"];
                        item.VColour = null; //reader["VColour"] == System.DBNull.Value ? null : (string)reader["VColour"];
                        item.MeterStreet = reader["MeterStreet"] == System.DBNull.Value ? null : (string)reader["MeterStreet"];
                        item.Location = reader["Location"] == System.DBNull.Value ? null : (string)reader["Location"];

                        item.EnfVendorStallId = reader["EnfVendorStallId"] == System.DBNull.Value ? default(int?) : (Int32)reader["EnfVendorStallId"];
                        item.PresentMeterTime = reader["PresentMeterTime"] == System.DBNull.Value ? default(DateTime?) : (DateTime)reader["PresentMeterTime"];
                        item.SpaceStatus = reader["SpaceStatus"] == System.DBNull.Value ? null : (string)reader["SpaceStatus"];
                        item.SensorEventTime = reader["SensorEventTime"] == System.DBNull.Value ? default(DateTime?) : (DateTime)reader["SensorEventTime"];
                        //item.ExpiredMinutes = reader["ExpiredMinutes"] == System.DBNull.Value ? default(int?) : (Int32)reader["ExpiredMinutes"];
                        item.ExpiredMinutes = reader["ExpiredMinutes"] == System.DBNull.Value ? -1 : (Int32)reader["ExpiredMinutes"];
                        item.AmountIncents = default(decimal?); // reader["AmountIncents"] == System.DBNull.Value ? default(decimal?) : (decimal)reader["AmountIncents"];
                        item.ExpiryTime = reader["ExpiryTime"] == System.DBNull.Value ? default(DateTime?) : (DateTime)reader["ExpiryTime"];
                        item.TimeZoneId = reader["TimeZoneId"] == System.DBNull.Value ? default(int?) : (Int32)reader["TimeZoneId"];
                        //item.PayStationID = reader["PayStationID"] == System.DBNull.Value ? default(int?) : (Int32)reader["PayStationID"];
                        item.PayStationID = reader["meterid"] == System.DBNull.Value ? default(int?) : Convert.ToInt32(reader["meterid"]);
                        item.ZoneID = reader["ZoneID"] == System.DBNull.Value ? default(int?) : (Int32)reader["ZoneID"];
                        item.ZoneName = reader["ZoneName"] == System.DBNull.Value ? null : (string)reader["ZoneName"];
                        item.Block = reader["Block"] == System.DBNull.Value ? null : (string)reader["Block"];
                        item.Direction = reader["Direction"] == System.DBNull.Value ? null : (string)reader["Direction"];
                        item.MeterStreet = reader["MeterStreet"] == System.DBNull.Value ? null : (string)reader["MeterStreet"];
                        item.StreetType = reader["StreetType"] == System.DBNull.Value ? null : (string)reader["StreetType"];
                        item.MeterStreet = reader["MeterStreet"] == System.DBNull.Value ? null : (string)reader["MeterStreet"];
                        item.CrossStreet1 = reader["CrossStreet1"] == System.DBNull.Value ? null : (string)reader["CrossStreet1"];

                        item.CrossStreet2 = reader["CrossStreet2"] == System.DBNull.Value ? null : (string)reader["CrossStreet2"];
                        item.DigiELType = default(int?); //reader["DigiELType"] == System.DBNull.Value ? default(int?) : (Int32)reader["DigiELType"];
                        item.DigiErrorDescr = null; // reader["DigiErrorDescr"] == System.DBNull.Value ? null : (string)reader["DigiErrorDescr"];
                        item.GeneticELType = default(int?); // reader["GeneticELType"] == System.DBNull.Value ? default(int?) : (Int32)reader["GeneticELType"];
                        item.GeneticErrorDescr = null; //reader["GeneticErrorDescr"] == System.DBNull.Value ? null : (string)reader["GeneticErrorDescr"];

                        if (reader["PMELType"] == System.DBNull.Value)
                        {
                            item.PMELType = default(int?);
                        }
                        else
                        {
                            string pMELTypeValue = reader["PMELType"].ToString();
                            if (string.IsNullOrWhiteSpace(pMELTypeValue))
                            {
                                item.PMELType = default(int?);
                            }
                            else
                            {
                                item.PMELType = (Int32)reader["PMELType"];
                            }
                        }



                        item.PMErrorDescr = null; //reader["PMErrorDescr"] == System.DBNull.Value ? null : (string)reader["PMErrorDescr"];
                        item.EnfHourDesc = reader["EnfHourDesc"] == System.DBNull.Value ? null : (string)reader["EnfHourDesc"];
                        item.MarkedSince = reader["MarkedSince"] == System.DBNull.Value ? default(DateTime?) : (DateTime)reader["MarkedSince"];
                        item.HitDescription = reader["HitDescription"] == System.DBNull.Value ? null : (string)reader["HitDescription"];


                        if (DataRecordExtensions.HasColumn(reader, "Metertype"))
                            item.METERLOC_METERTYPE = reader["Metertype"] == System.DBNull.Value ? null : (string)reader["Metertype"];
                        if (DataRecordExtensions.HasColumn(reader, "ZoneName"))
                            item.ZoneName = reader["ZoneName"] == System.DBNull.Value ? null : (string)reader["ZoneName"];


                        if (DataRecordExtensions.HasColumn(reader, "Recieptid"))
                            item.Recieptid = reader["Recieptid"] == System.DBNull.Value ? null : (string)reader["Recieptid"];

                        if (DataRecordExtensions.HasColumn(reader, "RateNumber"))
                            item.RateNumber = reader["RateNumber"] == System.DBNull.Value ? null : (string)reader["RateNumber"];


                        validatedData.Add(item);
                    }
                }
            }
            catch (Exception ex)
            {
                throw;
            }
            finally
            {
                if (connection.State == ConnectionState.Open)
                    connection.Close();
            }
            return validatedData;
        }

        #endregion

        #region MiamiParkingAuthority
        public static List<Transaction> GetMiamiParkingAuthorityCitationData(int customerID, string ZoneID, string MeterName, string ParkingSpaceID, string PlateNumber, string stateName, string SpaceStatus)
        {
            List<Transaction> validatedData = new List<Transaction>();
            SqlConnection connection = null;
            try
            {
                connection = new SqlConnection(System.Configuration.ConfigurationManager.ConnectionStrings["Connector"].ToString());
                SqlCommand sqlCommand = new SqlCommand();
                sqlCommand.CommandType = CommandType.StoredProcedure;
                //sqlCommand.CommandText = "[Sp_PaybyPlateDetails_RefreshService]";
                sqlCommand.CommandText = "[Sp_PaybyPlateDetails]";
                sqlCommand.Parameters.AddWithValue("@customerID", customerID);
                sqlCommand.Parameters.AddWithValue("@ZoneID", ZoneID);
                sqlCommand.Parameters.AddWithValue("@MeterName", MeterName);
                sqlCommand.Parameters.AddWithValue("@ParkingSpaceID", ParkingSpaceID);
                sqlCommand.Parameters.AddWithValue("@PlateNumber", PlateNumber);
                sqlCommand.Parameters.AddWithValue("@SpaceStatus", DBNull.Value);

                //sqlCommand.Parameters.AddWithValue("@ReceiptId", ReceiptId);
                //sqlCommand.Parameters.AddWithValue("@State", DBNull.Value);
                //sqlCommand.Parameters.AddWithValue("@RateNumber", DBNull.Value);

                sqlCommand.Connection = connection;
                sqlCommand.CommandTimeout = 120;
                connection.Open();
                using (var reader = sqlCommand.ExecuteReader())
                {
                    while (reader.Read())
                    {
                        var item = new Transaction();
                        item.EnforcementKey = reader["EnforcementKey"] == System.DBNull.Value ? null : (string)reader["EnforcementKey"];
                        item.CustomerId = (reader.GetInt64(reader.GetOrdinal("CustomerId"))).ToString();
                        item.ParkingSpaceID = reader["ParkingSpaceID"] == System.DBNull.Value ? default(int) : (int)reader["ParkingSpaceID"];
                        item.PlateNumber = reader["PlateNumber"] == System.DBNull.Value ? null : (string)reader["PlateNumber"];
                        item.MeterName = reader["MeterName"] == System.DBNull.Value ? null : (string)reader["MeterName"];
                        item.MeterId = reader["meterid"] == System.DBNull.Value ? null : (string)reader["meterid"];
                        item.Latitude = reader["Latitude"] == System.DBNull.Value ? default(double?) : (double)reader["Latitude"];
                        item.Latitude = reader["Longitude"] == System.DBNull.Value ? default(double?) : (double)reader["Longitude"];

                        item.LPLocation = reader["LPLocation"] == System.DBNull.Value ? null : (string)reader["LPLocation"];
                        item.VMake = null; // reader["VMake"] == System.DBNull.Value ? null : (string)reader["VMake"];
                        item.VModel = null; //reader["VModel"] == System.DBNull.Value ? null : (string)reader["VModel"];
                        item.VColour = null; //reader["VColour"] == System.DBNull.Value ? null : (string)reader["VColour"];
                        item.MeterStreet = reader["MeterStreet"] == System.DBNull.Value ? null : (string)reader["MeterStreet"];
                        item.Location = reader["Location"] == System.DBNull.Value ? null : (string)reader["Location"];

                        item.EnfVendorStallId = reader["EnfVendorStallId"] == System.DBNull.Value ? default(int?) : (Int32)reader["EnfVendorStallId"];
                        item.PresentMeterTime = reader["PresentMeterTime"] == System.DBNull.Value ? default(DateTime?) : (DateTime)reader["PresentMeterTime"];
                        item.SpaceStatus = reader["SpaceStatus"] == System.DBNull.Value ? null : (string)reader["SpaceStatus"];
                        item.SensorEventTime = reader["SensorEventTime"] == System.DBNull.Value ? default(DateTime?) : (DateTime)reader["SensorEventTime"];
                        //item.ExpiredMinutes = reader["ExpiredMinutes"] == System.DBNull.Value ? default(int?) : (Int32)reader["ExpiredMinutes"];
                        item.ExpiredMinutes = reader["ExpiredMinutes"] == System.DBNull.Value ? -1 : (Int32)reader["ExpiredMinutes"];
                        item.AmountIncents = default(decimal?); // reader["AmountIncents"] == System.DBNull.Value ? default(decimal?) : (decimal)reader["AmountIncents"];
                        item.ExpiryTime = reader["ExpiryTime"] == System.DBNull.Value ? default(DateTime?) : (DateTime)reader["ExpiryTime"];
                        item.TimeZoneId = reader["TimeZoneId"] == System.DBNull.Value ? default(int?) : (Int32)reader["TimeZoneId"];
                        //item.PayStationID = reader["PayStationID"] == System.DBNull.Value ? default(int?) : (Int32)reader["PayStationID"];
                        item.PayStationID = reader["meterid"] == System.DBNull.Value ? default(int?) : Convert.ToInt32(reader["meterid"]);
                        item.ZoneID = reader["ZoneID"] == System.DBNull.Value ? default(int?) : (Int32)reader["ZoneID"];
                        item.ZoneName = reader["ZoneName"] == System.DBNull.Value ? null : (string)reader["ZoneName"];
                        item.Block = reader["Block"] == System.DBNull.Value ? null : (string)reader["Block"];
                        item.Direction = reader["Direction"] == System.DBNull.Value ? null : (string)reader["Direction"];
                        item.MeterStreet = reader["MeterStreet"] == System.DBNull.Value ? null : (string)reader["MeterStreet"];
                        item.StreetType = reader["StreetType"] == System.DBNull.Value ? null : (string)reader["StreetType"];
                        item.MeterStreet = reader["MeterStreet"] == System.DBNull.Value ? null : (string)reader["MeterStreet"];
                        item.CrossStreet1 = reader["CrossStreet1"] == System.DBNull.Value ? null : (string)reader["CrossStreet1"];

                        item.CrossStreet2 = reader["CrossStreet2"] == System.DBNull.Value ? null : (string)reader["CrossStreet2"];
                        item.DigiELType = default(int?); //reader["DigiELType"] == System.DBNull.Value ? default(int?) : (Int32)reader["DigiELType"];
                        item.DigiErrorDescr = null; // reader["DigiErrorDescr"] == System.DBNull.Value ? null : (string)reader["DigiErrorDescr"];
                        item.GeneticELType = default(int?); // reader["GeneticELType"] == System.DBNull.Value ? default(int?) : (Int32)reader["GeneticELType"];
                        item.GeneticErrorDescr = null; //reader["GeneticErrorDescr"] == System.DBNull.Value ? null : (string)reader["GeneticErrorDescr"];

                        if (reader["PMELType"] == System.DBNull.Value)
                        {
                            item.PMELType = default(int?);
                        }
                        else
                        {
                            string pMELTypeValue = reader["PMELType"].ToString();
                            if (string.IsNullOrWhiteSpace(pMELTypeValue))
                            {
                                item.PMELType = default(int?);
                            }
                            else
                            {
                                item.PMELType = (Int32)reader["PMELType"];
                            }
                        }



                        item.PMErrorDescr = null; //reader["PMErrorDescr"] == System.DBNull.Value ? null : (string)reader["PMErrorDescr"];
                        item.EnfHourDesc = reader["EnfHourDesc"] == System.DBNull.Value ? null : (string)reader["EnfHourDesc"];
                        item.MarkedSince = reader["MarkedSince"] == System.DBNull.Value ? default(DateTime?) : (DateTime)reader["MarkedSince"];
                        item.HitDescription = reader["HitDescription"] == System.DBNull.Value ? null : (string)reader["HitDescription"];


                        if (DataRecordExtensions.HasColumn(reader, "Metertype"))
                            item.METERLOC_METERTYPE = reader["Metertype"] == System.DBNull.Value ? null : (string)reader["Metertype"];
                        if (DataRecordExtensions.HasColumn(reader, "ZoneName"))
                            item.ZoneName = reader["ZoneName"] == System.DBNull.Value ? null : (string)reader["ZoneName"];


                        if (DataRecordExtensions.HasColumn(reader, "Recieptid"))
                            item.Recieptid = reader["Recieptid"] == System.DBNull.Value ? null : (string)reader["Recieptid"];

                        if (DataRecordExtensions.HasColumn(reader, "RateNumber"))
                            item.RateNumber = reader["RateNumber"] == System.DBNull.Value ? null : (string)reader["RateNumber"];


                        validatedData.Add(item);
                    }
                }
            }
            catch (Exception ex)
            {
                throw;
            }
            finally
            {
                if (connection.State == ConnectionState.Open)
                    connection.Close();
            }
            return validatedData;
        }
        #endregion


        #region AbuDhabi
        public static List<Transaction> GetAbuDhabiCitationData(int customerID, string ZoneID, string MeterName, string ParkingSpaceID, string PlateNumber, string SpaceStatus)
        {
            List<Transaction> validatedData = new List<Transaction>();
            SqlConnection connection = null;
            try
            {
                connection = new SqlConnection(System.Configuration.ConfigurationManager.ConnectionStrings["Connector"].ToString());
                SqlCommand sqlCommand = new SqlCommand();
                sqlCommand.CommandType = CommandType.StoredProcedure;
                //sqlCommand.CommandText = "SpacePlateTxnFinal_Multispace";
                //sqlCommand.CommandText = "SpacePlateTxnFinal_siouxcity";
                sqlCommand.CommandText = "SpacePlateTxnFinal_Multispace";
                sqlCommand.Parameters.AddWithValue("@customerID", customerID);
                sqlCommand.Parameters.AddWithValue("@ZoneID", ZoneID);
                //sqlCommand.Parameters.AddWithValue("@MeterName", MeterName);
                sqlCommand.Parameters.AddWithValue("@MeterName", MeterName);
                sqlCommand.Parameters.AddWithValue("@ParkingSpaceID", ParkingSpaceID);
                sqlCommand.Parameters.AddWithValue("@PlateNumber", PlateNumber);
                sqlCommand.Parameters.AddWithValue("@SpaceStatus", SpaceStatus);
                sqlCommand.Connection = connection;
                sqlCommand.CommandTimeout = 120;
                connection.Open();
                using (var reader = sqlCommand.ExecuteReader())
                {
                    while (reader.Read())
                    {
                        var item = new Transaction();
                        item.EnforcementKey = reader["EnforcementKey"] == System.DBNull.Value ? null : (string)reader["EnforcementKey"];
                        item.CustomerId = (reader.GetInt64(reader.GetOrdinal("CustomerId"))).ToString();
                        item.ParkingSpaceID = reader["ParkingSpaceID"] == System.DBNull.Value ? default(int) : (int)reader["ParkingSpaceID"];
                        item.PlateNumber = reader["PlateNumber"] == System.DBNull.Value ? null : (string)reader["PlateNumber"];
                        item.MeterName = reader["MeterName"] == System.DBNull.Value ? null : (string)reader["MeterName"];

                        item.Latitude = reader["Latitude"] == System.DBNull.Value ? default(double?) : (double)reader["Latitude"];
                        item.Latitude = reader["Longitude"] == System.DBNull.Value ? default(double?) : (double)reader["Longitude"];

                        item.LPLocation = reader["LPLocation"] == System.DBNull.Value ? null : (string)reader["LPLocation"];
                        item.VMake = reader["VMake"] == System.DBNull.Value ? null : (string)reader["VMake"];
                        item.VModel = reader["VModel"] == System.DBNull.Value ? null : (string)reader["VModel"];
                        item.VColour = reader["VColour"] == System.DBNull.Value ? null : (string)reader["VColour"];
                        item.MeterStreet = reader["MeterStreet"] == System.DBNull.Value ? null : (string)reader["MeterStreet"];
                        item.Location = reader["Location"] == System.DBNull.Value ? null : (string)reader["Location"];

                        item.EnfVendorStallId = reader["EnfVendorStallId"] == System.DBNull.Value ? default(int?) : (Int32)reader["EnfVendorStallId"];
                        item.PresentMeterTime = reader["PresentMeterTime"] == System.DBNull.Value ? default(DateTime?) : (DateTime)reader["PresentMeterTime"];
                        item.SpaceStatus = reader["SpaceStatus"] == System.DBNull.Value ? null : (string)reader["SpaceStatus"];
                        item.SensorEventTime = reader["SensorEventTime"] == System.DBNull.Value ? default(DateTime?) : (DateTime)reader["SensorEventTime"];
                        //item.ExpiredMinutes = reader["ExpiredMinutes"] == System.DBNull.Value ? default(int?) : (Int32)reader["ExpiredMinutes"];
                        item.ExpiredMinutes = reader["ExpiredMinutes"] == System.DBNull.Value ? -1 : (Int32)reader["ExpiredMinutes"];
                        item.AmountIncents = reader["AmountIncents"] == System.DBNull.Value ? default(decimal?) : (decimal)reader["AmountIncents"];
                        item.ExpiryTime = reader["ExpiryTime"] == System.DBNull.Value ? default(DateTime?) : (DateTime)reader["ExpiryTime"];
                        item.TimeZoneId = reader["TimeZoneId"] == System.DBNull.Value ? default(int?) : (Int32)reader["TimeZoneId"];
                        item.PayStationID = reader["PayStationID"] == System.DBNull.Value ? default(int?) : (Int32)reader["PayStationID"];
                        item.ZoneID = reader["ZoneID"] == System.DBNull.Value ? default(int?) : (Int32)reader["ZoneID"];
                        //item.ZoneName = reader["ZoneName"] == System.DBNull.Value ? null : (string)reader["ZoneName"];
                        item.ZoneName = null;
                        item.Block = reader["Block"] == System.DBNull.Value ? null : (string)reader["Block"];
                        item.Direction = reader["Direction"] == System.DBNull.Value ? null : (string)reader["Direction"];
                        item.MeterStreet = reader["MeterStreet"] == System.DBNull.Value ? null : (string)reader["MeterStreet"];
                        item.StreetType = reader["StreetType"] == System.DBNull.Value ? null : (string)reader["StreetType"];
                        item.MeterStreet = reader["MeterStreet"] == System.DBNull.Value ? null : (string)reader["MeterStreet"];
                        item.CrossStreet1 = reader["CrossStreet1"] == System.DBNull.Value ? null : (string)reader["CrossStreet1"];

                        item.CrossStreet2 = reader["CrossStreet2"] == System.DBNull.Value ? null : (string)reader["CrossStreet2"];
                        item.DigiELType = reader["DigiELType"] == System.DBNull.Value ? default(int?) : (Int32)reader["DigiELType"];
                        item.DigiErrorDescr = reader["DigiErrorDescr"] == System.DBNull.Value ? null : (string)reader["DigiErrorDescr"];
                        // item.GeneticELType = reader["GeneticELType"] == System.DBNull.Value ? default(int?) : (Int32)reader["GeneticELType"];
                        //item.GeneticErrorDescr = reader["GeneticErrorDescr"] == System.DBNull.Value ? null : (string)reader["GeneticErrorDescr"];
                        //item.PMELType = reader["PMELType"] == System.DBNull.Value ? default(int?) : (Int32)reader["PMELType"];
                        //item.PMErrorDescr = reader["PMErrorDescr"] == System.DBNull.Value ? null : (string)reader["PMErrorDescr"];
                        //item.EnfHourDesc = reader["EnfHourDesc"] == System.DBNull.Value ? null : (string)reader["EnfHourDesc"];
                        //item.MarkedSince = reader["MarkedSince"] == System.DBNull.Value ? default(DateTime?) : (DateTime)reader["MarkedSince"];
                        //item.HitDescription = reader["HitDescription"] == System.DBNull.Value ? null : (string)reader["HitDescription"];
                        validatedData.Add(item);
                    }
                }
            }
            catch (Exception ex)
            {
                throw;
            }
            finally
            {
                if (connection.State == ConnectionState.Open)
                    connection.Close();
            }
            return validatedData;
        }
        #endregion

        #region Charleston
        public static List<Transaction> GetCharlestonCitationData(int customerID, string ZoneID, string MeterName, string ParkingSpaceID, string PlateNumber, string SpaceStatus)
        {
            List<Transaction> validatedData = new List<Transaction>();
            SqlConnection connection = null;
            try
            {
                connection = new SqlConnection(System.Configuration.ConfigurationManager.ConnectionStrings["Connector"].ToString());
                SqlCommand sqlCommand = new SqlCommand();
                sqlCommand.CommandType = CommandType.StoredProcedure;
                //sqlCommand.CommandText = "SpacePlateTxnFinal_Multispace";
                //sqlCommand.CommandText = "SpacePlateTxnFinal_siouxcity";
                sqlCommand.CommandText = "SpacePlateTxnFinal_Multispace";
                sqlCommand.Parameters.AddWithValue("@customerID", customerID);
                sqlCommand.Parameters.AddWithValue("@ZoneID", ZoneID);
                //sqlCommand.Parameters.AddWithValue("@MeterName", MeterName);
                sqlCommand.Parameters.AddWithValue("@MeterName", MeterName);
                sqlCommand.Parameters.AddWithValue("@ParkingSpaceID", ParkingSpaceID);
                sqlCommand.Parameters.AddWithValue("@PlateNumber", PlateNumber);
                sqlCommand.Parameters.AddWithValue("@SpaceStatus", SpaceStatus);
                sqlCommand.Connection = connection;
                sqlCommand.CommandTimeout = 120;
                connection.Open();
                using (var reader = sqlCommand.ExecuteReader())
                {
                    while (reader.Read())
                    {
                        var item = new Transaction();
                        item.EnforcementKey = reader["EnforcementKey"] == System.DBNull.Value ? null : (string)reader["EnforcementKey"];
                        item.CustomerId = (reader.GetInt64(reader.GetOrdinal("CustomerId"))).ToString();
                        item.ParkingSpaceID = reader["ParkingSpaceID"] == System.DBNull.Value ? default(int) : (int)reader["ParkingSpaceID"];
                        item.PlateNumber = reader["PlateNumber"] == System.DBNull.Value ? null : (string)reader["PlateNumber"];
                        item.MeterName = reader["MeterName"] == System.DBNull.Value ? null : (string)reader["MeterName"];

                        item.Latitude = reader["Latitude"] == System.DBNull.Value ? default(double?) : (double)reader["Latitude"];
                        item.Latitude = reader["Longitude"] == System.DBNull.Value ? default(double?) : (double)reader["Longitude"];

                        item.LPLocation = reader["LPLocation"] == System.DBNull.Value ? null : (string)reader["LPLocation"];
                        item.VMake = reader["VMake"] == System.DBNull.Value ? null : (string)reader["VMake"];
                        item.VModel = reader["VModel"] == System.DBNull.Value ? null : (string)reader["VModel"];
                        item.VColour = reader["VColour"] == System.DBNull.Value ? null : (string)reader["VColour"];
                        item.MeterStreet = reader["MeterStreet"] == System.DBNull.Value ? null : (string)reader["MeterStreet"];
                        item.Location = reader["Location"] == System.DBNull.Value ? null : (string)reader["Location"];

                        item.EnfVendorStallId = reader["EnfVendorStallId"] == System.DBNull.Value ? default(int?) : (Int32)reader["EnfVendorStallId"];
                        item.PresentMeterTime = reader["PresentMeterTime"] == System.DBNull.Value ? default(DateTime?) : (DateTime)reader["PresentMeterTime"];
                        item.SpaceStatus = reader["SpaceStatus"] == System.DBNull.Value ? null : (string)reader["SpaceStatus"];
                        item.SensorEventTime = reader["SensorEventTime"] == System.DBNull.Value ? default(DateTime?) : (DateTime)reader["SensorEventTime"];
                        //item.ExpiredMinutes = reader["ExpiredMinutes"] == System.DBNull.Value ? default(int?) : (Int32)reader["ExpiredMinutes"];
                        item.ExpiredMinutes = reader["ExpiredMinutes"] == System.DBNull.Value ? -1 : (Int32)reader["ExpiredMinutes"];
                        item.AmountIncents = reader["AmountIncents"] == System.DBNull.Value ? default(decimal?) : (decimal)reader["AmountIncents"];
                        item.ExpiryTime = reader["ExpiryTime"] == System.DBNull.Value ? default(DateTime?) : (DateTime)reader["ExpiryTime"];
                        item.TimeZoneId = reader["TimeZoneId"] == System.DBNull.Value ? default(int?) : (Int32)reader["TimeZoneId"];
                        item.PayStationID = reader["PayStationID"] == System.DBNull.Value ? default(int?) : (Int32)reader["PayStationID"];
                        item.ZoneID = reader["ZoneID"] == System.DBNull.Value ? default(int?) : (Int32)reader["ZoneID"];
                        //item.ZoneName = reader["ZoneName"] == System.DBNull.Value ? null : (string)reader["ZoneName"];
                        item.ZoneName = null;
                        item.Block = reader["Block"] == System.DBNull.Value ? null : (string)reader["Block"];
                        item.Direction = reader["Direction"] == System.DBNull.Value ? null : (string)reader["Direction"];
                        item.MeterStreet = reader["MeterStreet"] == System.DBNull.Value ? null : (string)reader["MeterStreet"];
                        item.StreetType = reader["StreetType"] == System.DBNull.Value ? null : (string)reader["StreetType"];
                        item.MeterStreet = reader["MeterStreet"] == System.DBNull.Value ? null : (string)reader["MeterStreet"];
                        item.CrossStreet1 = reader["CrossStreet1"] == System.DBNull.Value ? null : (string)reader["CrossStreet1"];

                        item.CrossStreet2 = reader["CrossStreet2"] == System.DBNull.Value ? null : (string)reader["CrossStreet2"];
                        item.DigiELType = reader["DigiELType"] == System.DBNull.Value ? default(int?) : (Int32)reader["DigiELType"];
                        item.DigiErrorDescr = reader["DigiErrorDescr"] == System.DBNull.Value ? null : (string)reader["DigiErrorDescr"];
                        // item.GeneticELType = reader["GeneticELType"] == System.DBNull.Value ? default(int?) : (Int32)reader["GeneticELType"];
                        //item.GeneticErrorDescr = reader["GeneticErrorDescr"] == System.DBNull.Value ? null : (string)reader["GeneticErrorDescr"];
                        //item.PMELType = reader["PMELType"] == System.DBNull.Value ? default(int?) : (Int32)reader["PMELType"];
                        //item.PMErrorDescr = reader["PMErrorDescr"] == System.DBNull.Value ? null : (string)reader["PMErrorDescr"];
                        //item.EnfHourDesc = reader["EnfHourDesc"] == System.DBNull.Value ? null : (string)reader["EnfHourDesc"];
                        //item.MarkedSince = reader["MarkedSince"] == System.DBNull.Value ? default(DateTime?) : (DateTime)reader["MarkedSince"];
                        //item.HitDescription = reader["HitDescription"] == System.DBNull.Value ? null : (string)reader["HitDescription"];
                        validatedData.Add(item);
                    }
                }
            }
            catch (Exception ex)
            {
                throw;
            }
            finally
            {
                if (connection.State == ConnectionState.Open)
                    connection.Close();
            }
            return validatedData;
        }
        #endregion

        #region SanJose
        public static List<Transaction> GetSanJoseCitationData(int customerID, string ZoneID, string MeterName, string ParkingSpaceID, string PlateNumber, string SpaceStatus)
        {
            List<Transaction> validatedData = new List<Transaction>();
            SqlConnection connection = null;
            try
            {
                connection = new SqlConnection(System.Configuration.ConfigurationManager.ConnectionStrings["Connector"].ToString());
                SqlCommand sqlCommand = new SqlCommand();
                sqlCommand.CommandType = CommandType.StoredProcedure;
                //sqlCommand.CommandText = "SpacePlateTxnFinal_Multispace";
                //sqlCommand.CommandText = "SpacePlateTxnFinal_siouxcity";
                sqlCommand.CommandText = "SpacePlateTxnFinal_Multispace";
                sqlCommand.Parameters.AddWithValue("@customerID", customerID);
                sqlCommand.Parameters.AddWithValue("@ZoneID", ZoneID);
                //sqlCommand.Parameters.AddWithValue("@MeterName", MeterName);
                sqlCommand.Parameters.AddWithValue("@MeterName", MeterName);
                sqlCommand.Parameters.AddWithValue("@ParkingSpaceID", ParkingSpaceID);
                sqlCommand.Parameters.AddWithValue("@PlateNumber", PlateNumber);
                sqlCommand.Parameters.AddWithValue("@SpaceStatus", SpaceStatus);
                sqlCommand.Connection = connection;
                sqlCommand.CommandTimeout = 120;
                connection.Open();
                using (var reader = sqlCommand.ExecuteReader())
                {
                    while (reader.Read())
                    {
                        var item = new Transaction();
                        item.EnforcementKey = reader["EnforcementKey"] == System.DBNull.Value ? null : (string)reader["EnforcementKey"];
                        item.CustomerId = (reader.GetInt64(reader.GetOrdinal("CustomerId"))).ToString();
                        item.ParkingSpaceID = reader["ParkingSpaceID"] == System.DBNull.Value ? default(int) : (int)reader["ParkingSpaceID"];
                        item.PlateNumber = reader["PlateNumber"] == System.DBNull.Value ? null : (string)reader["PlateNumber"];
                        item.MeterName = reader["MeterName"] == System.DBNull.Value ? null : (string)reader["MeterName"];

                        item.Latitude = reader["Latitude"] == System.DBNull.Value ? default(double?) : (double)reader["Latitude"];
                        item.Latitude = reader["Longitude"] == System.DBNull.Value ? default(double?) : (double)reader["Longitude"];

                        item.LPLocation = reader["LPLocation"] == System.DBNull.Value ? null : (string)reader["LPLocation"];
                        item.VMake = reader["VMake"] == System.DBNull.Value ? null : (string)reader["VMake"];
                        item.VModel = reader["VModel"] == System.DBNull.Value ? null : (string)reader["VModel"];
                        item.VColour = reader["VColour"] == System.DBNull.Value ? null : (string)reader["VColour"];
                        item.MeterStreet = reader["MeterStreet"] == System.DBNull.Value ? null : (string)reader["MeterStreet"];
                        item.Location = reader["Location"] == System.DBNull.Value ? null : (string)reader["Location"];

                        item.EnfVendorStallId = reader["EnfVendorStallId"] == System.DBNull.Value ? default(int?) : (Int32)reader["EnfVendorStallId"];
                        item.PresentMeterTime = reader["PresentMeterTime"] == System.DBNull.Value ? default(DateTime?) : (DateTime)reader["PresentMeterTime"];
                        item.SpaceStatus = reader["SpaceStatus"] == System.DBNull.Value ? null : (string)reader["SpaceStatus"];
                        item.SensorEventTime = reader["SensorEventTime"] == System.DBNull.Value ? default(DateTime?) : (DateTime)reader["SensorEventTime"];
                        //item.ExpiredMinutes = reader["ExpiredMinutes"] == System.DBNull.Value ? default(int?) : (Int32)reader["ExpiredMinutes"];
                        item.ExpiredMinutes = reader["ExpiredMinutes"] == System.DBNull.Value ? -1 : (Int32)reader["ExpiredMinutes"];
                        item.AmountIncents = reader["AmountIncents"] == System.DBNull.Value ? default(decimal?) : (decimal)reader["AmountIncents"];
                        item.ExpiryTime = reader["ExpiryTime"] == System.DBNull.Value ? default(DateTime?) : (DateTime)reader["ExpiryTime"];
                        item.TimeZoneId = reader["TimeZoneId"] == System.DBNull.Value ? default(int?) : (Int32)reader["TimeZoneId"];
                        item.PayStationID = reader["PayStationID"] == System.DBNull.Value ? default(int?) : (Int32)reader["PayStationID"];
                        item.ZoneID = reader["ZoneID"] == System.DBNull.Value ? default(int?) : (Int32)reader["ZoneID"];
                        //item.ZoneName = reader["ZoneName"] == System.DBNull.Value ? null : (string)reader["ZoneName"];
                        item.ZoneName = null;
                        item.Block = reader["Block"] == System.DBNull.Value ? null : (string)reader["Block"];
                        item.Direction = reader["Direction"] == System.DBNull.Value ? null : (string)reader["Direction"];
                        item.MeterStreet = reader["MeterStreet"] == System.DBNull.Value ? null : (string)reader["MeterStreet"];
                        item.StreetType = reader["StreetType"] == System.DBNull.Value ? null : (string)reader["StreetType"];
                        item.MeterStreet = reader["MeterStreet"] == System.DBNull.Value ? null : (string)reader["MeterStreet"];
                        item.CrossStreet1 = reader["CrossStreet1"] == System.DBNull.Value ? null : (string)reader["CrossStreet1"];

                        item.CrossStreet2 = reader["CrossStreet2"] == System.DBNull.Value ? null : (string)reader["CrossStreet2"];
                        item.DigiELType = reader["DigiELType"] == System.DBNull.Value ? default(int?) : (Int32)reader["DigiELType"];
                        item.DigiErrorDescr = reader["DigiErrorDescr"] == System.DBNull.Value ? null : (string)reader["DigiErrorDescr"];
                        // item.GeneticELType = reader["GeneticELType"] == System.DBNull.Value ? default(int?) : (Int32)reader["GeneticELType"];
                        //item.GeneticErrorDescr = reader["GeneticErrorDescr"] == System.DBNull.Value ? null : (string)reader["GeneticErrorDescr"];
                        //item.PMELType = reader["PMELType"] == System.DBNull.Value ? default(int?) : (Int32)reader["PMELType"];
                        //item.PMErrorDescr = reader["PMErrorDescr"] == System.DBNull.Value ? null : (string)reader["PMErrorDescr"];
                        //item.EnfHourDesc = reader["EnfHourDesc"] == System.DBNull.Value ? null : (string)reader["EnfHourDesc"];
                        //item.MarkedSince = reader["MarkedSince"] == System.DBNull.Value ? default(DateTime?) : (DateTime)reader["MarkedSince"];
                        //item.HitDescription = reader["HitDescription"] == System.DBNull.Value ? null : (string)reader["HitDescription"];
                        validatedData.Add(item);
                    }
                }
            }
            catch (Exception ex)
            {
                throw;
            }
            finally
            {
                if (connection.State == ConnectionState.Open)
                    connection.Close();
            }
            return validatedData;
        }
        #endregion

        #region Ardsley,NY
        public static List<Transaction> GetArdsleyNYCitationData(int customerID, string ZoneID, string MeterName, string ParkingSpaceID, string PlateNumber, string SpaceStatus)
        {
            List<Transaction> validatedData = new List<Transaction>();
            SqlConnection connection = null;
            try
            {
                connection = new SqlConnection(System.Configuration.ConfigurationManager.ConnectionStrings["Connector"].ToString());
                SqlCommand sqlCommand = new SqlCommand();
                sqlCommand.CommandType = CommandType.StoredProcedure;
                //sqlCommand.CommandText = "SpacePlateTxnFinal_Multispace";
                //sqlCommand.CommandText = "SpacePlateTxnFinal_siouxcity";
                sqlCommand.CommandText = "SpacePlateTxnFinal_Multispace";
                sqlCommand.Parameters.AddWithValue("@customerID", customerID);
                sqlCommand.Parameters.AddWithValue("@ZoneID", ZoneID);
                //sqlCommand.Parameters.AddWithValue("@MeterName", MeterName);
                sqlCommand.Parameters.AddWithValue("@MeterName", MeterName);
                sqlCommand.Parameters.AddWithValue("@ParkingSpaceID", ParkingSpaceID);
                sqlCommand.Parameters.AddWithValue("@PlateNumber", PlateNumber);
                sqlCommand.Parameters.AddWithValue("@SpaceStatus", SpaceStatus);
                sqlCommand.Connection = connection;
                sqlCommand.CommandTimeout = 120;
                connection.Open();
                using (var reader = sqlCommand.ExecuteReader())
                {
                    while (reader.Read())
                    {
                        var item = new Transaction();
                        item.EnforcementKey = reader["EnforcementKey"] == System.DBNull.Value ? null : (string)reader["EnforcementKey"];
                        item.CustomerId = (reader.GetInt64(reader.GetOrdinal("CustomerId"))).ToString();
                        item.ParkingSpaceID = reader["ParkingSpaceID"] == System.DBNull.Value ? default(int) : (int)reader["ParkingSpaceID"];
                        item.PlateNumber = reader["PlateNumber"] == System.DBNull.Value ? null : (string)reader["PlateNumber"];
                        item.MeterName = reader["MeterName"] == System.DBNull.Value ? null : (string)reader["MeterName"];

                        item.Latitude = reader["Latitude"] == System.DBNull.Value ? default(double?) : (double)reader["Latitude"];
                        item.Latitude = reader["Longitude"] == System.DBNull.Value ? default(double?) : (double)reader["Longitude"];

                        item.LPLocation = reader["LPLocation"] == System.DBNull.Value ? null : (string)reader["LPLocation"];
                        item.VMake = reader["VMake"] == System.DBNull.Value ? null : (string)reader["VMake"];
                        item.VModel = reader["VModel"] == System.DBNull.Value ? null : (string)reader["VModel"];
                        item.VColour = reader["VColour"] == System.DBNull.Value ? null : (string)reader["VColour"];
                        item.MeterStreet = reader["MeterStreet"] == System.DBNull.Value ? null : (string)reader["MeterStreet"];
                        item.Location = reader["Location"] == System.DBNull.Value ? null : (string)reader["Location"];

                        item.EnfVendorStallId = reader["EnfVendorStallId"] == System.DBNull.Value ? default(int?) : (Int32)reader["EnfVendorStallId"];
                        item.PresentMeterTime = reader["PresentMeterTime"] == System.DBNull.Value ? default(DateTime?) : (DateTime)reader["PresentMeterTime"];
                        item.SpaceStatus = reader["SpaceStatus"] == System.DBNull.Value ? null : (string)reader["SpaceStatus"];
                        item.SensorEventTime = reader["SensorEventTime"] == System.DBNull.Value ? default(DateTime?) : (DateTime)reader["SensorEventTime"];
                        //item.ExpiredMinutes = reader["ExpiredMinutes"] == System.DBNull.Value ? default(int?) : (Int32)reader["ExpiredMinutes"];
                        item.ExpiredMinutes = reader["ExpiredMinutes"] == System.DBNull.Value ? -1 : (Int32)reader["ExpiredMinutes"];
                        item.AmountIncents = reader["AmountIncents"] == System.DBNull.Value ? default(decimal?) : (decimal)reader["AmountIncents"];
                        item.ExpiryTime = reader["ExpiryTime"] == System.DBNull.Value ? default(DateTime?) : (DateTime)reader["ExpiryTime"];
                        item.TimeZoneId = reader["TimeZoneId"] == System.DBNull.Value ? default(int?) : (Int32)reader["TimeZoneId"];
                        item.PayStationID = reader["PayStationID"] == System.DBNull.Value ? default(int?) : (Int32)reader["PayStationID"];
                        item.ZoneID = reader["ZoneID"] == System.DBNull.Value ? default(int?) : (Int32)reader["ZoneID"];
                        //item.ZoneName = reader["ZoneName"] == System.DBNull.Value ? null : (string)reader["ZoneName"];
                        item.ZoneName = null;
                        item.Block = reader["Block"] == System.DBNull.Value ? null : (string)reader["Block"];
                        item.Direction = reader["Direction"] == System.DBNull.Value ? null : (string)reader["Direction"];
                        item.MeterStreet = reader["MeterStreet"] == System.DBNull.Value ? null : (string)reader["MeterStreet"];
                        item.StreetType = reader["StreetType"] == System.DBNull.Value ? null : (string)reader["StreetType"];
                        item.MeterStreet = reader["MeterStreet"] == System.DBNull.Value ? null : (string)reader["MeterStreet"];
                        item.CrossStreet1 = reader["CrossStreet1"] == System.DBNull.Value ? null : (string)reader["CrossStreet1"];

                        item.CrossStreet2 = reader["CrossStreet2"] == System.DBNull.Value ? null : (string)reader["CrossStreet2"];
                        item.DigiELType = reader["DigiELType"] == System.DBNull.Value ? default(int?) : (Int32)reader["DigiELType"];
                        item.DigiErrorDescr = reader["DigiErrorDescr"] == System.DBNull.Value ? null : (string)reader["DigiErrorDescr"];
                        // item.GeneticELType = reader["GeneticELType"] == System.DBNull.Value ? default(int?) : (Int32)reader["GeneticELType"];
                        //item.GeneticErrorDescr = reader["GeneticErrorDescr"] == System.DBNull.Value ? null : (string)reader["GeneticErrorDescr"];
                        //item.PMELType = reader["PMELType"] == System.DBNull.Value ? default(int?) : (Int32)reader["PMELType"];
                        //item.PMErrorDescr = reader["PMErrorDescr"] == System.DBNull.Value ? null : (string)reader["PMErrorDescr"];
                        //item.EnfHourDesc = reader["EnfHourDesc"] == System.DBNull.Value ? null : (string)reader["EnfHourDesc"];
                        //item.MarkedSince = reader["MarkedSince"] == System.DBNull.Value ? default(DateTime?) : (DateTime)reader["MarkedSince"];
                        //item.HitDescription = reader["HitDescription"] == System.DBNull.Value ? null : (string)reader["HitDescription"];
                        validatedData.Add(item);
                    }
                }
            }
            catch (Exception ex)
            {
                throw;
            }
            finally
            {
                if (connection.State == ConnectionState.Open)
                    connection.Close();
            }
            return validatedData;
        }
        #endregion


        #region Spokane, WA
        public static List<Transaction> GetDataSpokaneWACitationData(int customerID, string ZoneID, string MeterName, string ParkingSpaceID, string PlateNumber, string stateName, string SpaceStatus)
        {
            List<Transaction> validatedData = new List<Transaction>();
            SqlConnection connection = null;
            try
            {
                connection = new SqlConnection(System.Configuration.ConfigurationManager.ConnectionStrings["Connector"].ToString());
                SqlCommand sqlCommand = new SqlCommand();
                sqlCommand.CommandType = CommandType.StoredProcedure;
                //sqlCommand.CommandText = "[Sp_PaybyPlateDetails]";
                sqlCommand.CommandText = "[Sp_PaybyPlateDetails_v1]";
                sqlCommand.Parameters.AddWithValue("@customerID", customerID);
                sqlCommand.Parameters.AddWithValue("@ZoneID", ZoneID);
                sqlCommand.Parameters.AddWithValue("@MeterName", MeterName);
                sqlCommand.Parameters.AddWithValue("@ParkingSpaceID", ParkingSpaceID);
                sqlCommand.Parameters.AddWithValue("@PlateNumber", PlateNumber);
                sqlCommand.Parameters.AddWithValue("@SpaceStatus", DBNull.Value);

                //sqlCommand.Parameters.AddWithValue("@ReceiptId", ReceiptId);
                //sqlCommand.Parameters.AddWithValue("@State", DBNull.Value);
                //sqlCommand.Parameters.AddWithValue("@RateNumber", DBNull.Value);

                sqlCommand.Connection = connection;
                sqlCommand.CommandTimeout = 120;
                connection.Open();
                using (var reader = sqlCommand.ExecuteReader())
                {
                    while (reader.Read())
                    {
                        var item = new Transaction();
                        item.EnforcementKey = reader["EnforcementKey"] == System.DBNull.Value ? null : (string)reader["EnforcementKey"];
                        item.CustomerId = (reader.GetInt64(reader.GetOrdinal("CustomerId"))).ToString();
                        item.ParkingSpaceID = reader["ParkingSpaceID"] == System.DBNull.Value ? default(int) : (int)reader["ParkingSpaceID"];
                        item.PlateNumber = reader["PlateNumber"] == System.DBNull.Value ? null : (string)reader["PlateNumber"];
                        item.MeterName = reader["MeterName"] == System.DBNull.Value ? null : (string)reader["MeterName"];
                        item.MeterId = reader["meterid"] == System.DBNull.Value ? null : (string)reader["meterid"];
                        item.Latitude = reader["Latitude"] == System.DBNull.Value ? default(double?) : (double)reader["Latitude"];
                        item.Latitude = reader["Longitude"] == System.DBNull.Value ? default(double?) : (double)reader["Longitude"];

                        item.LPLocation = reader["LPLocation"] == System.DBNull.Value ? null : (string)reader["LPLocation"];
                        item.VMake = null; // reader["VMake"] == System.DBNull.Value ? null : (string)reader["VMake"];
                        item.VModel = null; //reader["VModel"] == System.DBNull.Value ? null : (string)reader["VModel"];
                        item.VColour = null; //reader["VColour"] == System.DBNull.Value ? null : (string)reader["VColour"];
                        item.MeterStreet = reader["MeterStreet"] == System.DBNull.Value ? null : (string)reader["MeterStreet"];
                        item.Location = reader["Location"] == System.DBNull.Value ? null : (string)reader["Location"];

                        item.EnfVendorStallId = reader["EnfVendorStallId"] == System.DBNull.Value ? default(int?) : (Int32)reader["EnfVendorStallId"];
                        item.PresentMeterTime = reader["PresentMeterTime"] == System.DBNull.Value ? default(DateTime?) : (DateTime)reader["PresentMeterTime"];
                        item.SpaceStatus = reader["SpaceStatus"] == System.DBNull.Value ? null : (string)reader["SpaceStatus"];
                        item.SensorEventTime = reader["SensorEventTime"] == System.DBNull.Value ? default(DateTime?) : (DateTime)reader["SensorEventTime"];
                        //item.ExpiredMinutes = reader["ExpiredMinutes"] == System.DBNull.Value ? default(int?) : (Int32)reader["ExpiredMinutes"];
                        item.ExpiredMinutes = reader["ExpiredMinutes"] == System.DBNull.Value ? -1 : (Int32)reader["ExpiredMinutes"];
                        item.AmountIncents = default(decimal?); // reader["AmountIncents"] == System.DBNull.Value ? default(decimal?) : (decimal)reader["AmountIncents"];
                        item.ExpiryTime = reader["ExpiryTime"] == System.DBNull.Value ? default(DateTime?) : (DateTime)reader["ExpiryTime"];
                        item.TimeZoneId = reader["TimeZoneId"] == System.DBNull.Value ? default(int?) : (Int32)reader["TimeZoneId"];
                        //item.PayStationID = reader["PayStationID"] == System.DBNull.Value ? default(int?) : (Int32)reader["PayStationID"];
                        item.PayStationID = reader["meterid"] == System.DBNull.Value ? default(int?) : Convert.ToInt32(reader["meterid"]);
                        item.ZoneID = reader["ZoneID"] == System.DBNull.Value ? default(int?) : (Int32)reader["ZoneID"];
                        //item.ZoneName = reader["ZoneName"] == System.DBNull.Value ? null : (string)reader["ZoneName"];
                        item.Block = reader["Block"] == System.DBNull.Value ? null : (string)reader["Block"];
                        item.Direction = reader["Direction"] == System.DBNull.Value ? null : (string)reader["Direction"];
                        item.MeterStreet = reader["MeterStreet"] == System.DBNull.Value ? null : (string)reader["MeterStreet"];
                        item.StreetType = reader["StreetType"] == System.DBNull.Value ? null : (string)reader["StreetType"];
                        item.MeterStreet = reader["MeterStreet"] == System.DBNull.Value ? null : (string)reader["MeterStreet"];
                        item.CrossStreet1 = reader["CrossStreet1"] == System.DBNull.Value ? null : (string)reader["CrossStreet1"];

                        item.CrossStreet2 = reader["CrossStreet2"] == System.DBNull.Value ? null : (string)reader["CrossStreet2"];
                        item.DigiELType = default(int?); //reader["DigiELType"] == System.DBNull.Value ? default(int?) : (Int32)reader["DigiELType"];
                        item.DigiErrorDescr = null; // reader["DigiErrorDescr"] == System.DBNull.Value ? null : (string)reader["DigiErrorDescr"];
                        item.GeneticELType = default(int?); // reader["GeneticELType"] == System.DBNull.Value ? default(int?) : (Int32)reader["GeneticELType"];
                        item.GeneticErrorDescr = null; //reader["GeneticErrorDescr"] == System.DBNull.Value ? null : (string)reader["GeneticErrorDescr"];

                        if (reader["PMELType"] == System.DBNull.Value)
                        {
                            item.PMELType = default(int?);
                        }
                        else
                        {
                            string pMELTypeValue = reader["PMELType"].ToString();
                            if (string.IsNullOrWhiteSpace(pMELTypeValue))
                            {
                                item.PMELType = default(int?);
                            }
                            else
                            {
                                item.PMELType = (Int32)reader["PMELType"];
                            }
                        }



                        item.PMErrorDescr = null; //reader["PMErrorDescr"] == System.DBNull.Value ? null : (string)reader["PMErrorDescr"];
                        item.EnfHourDesc = reader["EnfHourDesc"] == System.DBNull.Value ? null : (string)reader["EnfHourDesc"];
                        item.MarkedSince = reader["MarkedSince"] == System.DBNull.Value ? default(DateTime?) : (DateTime)reader["MarkedSince"];
                        item.HitDescription = reader["HitDescription"] == System.DBNull.Value ? null : (string)reader["HitDescription"];


                        if (DataRecordExtensions.HasColumn(reader, "Metertype"))
                            item.METERLOC_METERTYPE = reader["Metertype"] == System.DBNull.Value ? null : (string)reader["Metertype"];
                        if (DataRecordExtensions.HasColumn(reader, "ZoneName"))
                            item.ZoneName = reader["ZoneName"] == System.DBNull.Value ? null : (string)reader["ZoneName"];


                        if (DataRecordExtensions.HasColumn(reader, "Recieptid"))
                            item.Recieptid = reader["Recieptid"] == System.DBNull.Value ? null : (string)reader["Recieptid"];

                        if (DataRecordExtensions.HasColumn(reader, "RateNumber"))
                            item.RateNumber = reader["RateNumber"] == System.DBNull.Value ? null : (string)reader["RateNumber"];


                        validatedData.Add(item);
                    }
                }
            }
            catch (Exception ex)
            {
                throw;
            }
            finally
            {
                if (connection.State == ConnectionState.Open)
                    connection.Close();
            }
            return validatedData;
        }
        #endregion


        #region Huntsville
        public static List<Transaction> GetHuntsvilleCitationData(int customerID, string ZoneID, string MeterName, string ParkingSpaceID, string PlateNumber, string stateName, string SpaceStatus)
        {
            List<Transaction> validatedData = new List<Transaction>();
            SqlConnection connection = null;
            try
            {
                connection = new SqlConnection(System.Configuration.ConfigurationManager.ConnectionStrings["Connector"].ToString());
                SqlCommand sqlCommand = new SqlCommand();
                sqlCommand.CommandType = CommandType.StoredProcedure;
                sqlCommand.CommandText = "[Sp_PaybyPlateDetails]";
                sqlCommand.Parameters.AddWithValue("@customerID", customerID);
                sqlCommand.Parameters.AddWithValue("@ZoneID", ZoneID);
                sqlCommand.Parameters.AddWithValue("@MeterName", MeterName);
                sqlCommand.Parameters.AddWithValue("@ParkingSpaceID", ParkingSpaceID);
                sqlCommand.Parameters.AddWithValue("@PlateNumber", PlateNumber);
                sqlCommand.Parameters.AddWithValue("@SpaceStatus", DBNull.Value);

                //sqlCommand.Parameters.AddWithValue("@ReceiptId", ReceiptId);
                //sqlCommand.Parameters.AddWithValue("@State", DBNull.Value);
                //sqlCommand.Parameters.AddWithValue("@RateNumber", DBNull.Value);

                sqlCommand.Connection = connection;
                sqlCommand.CommandTimeout = 120;
                connection.Open();
                using (var reader = sqlCommand.ExecuteReader())
                {
                    while (reader.Read())
                    {
                        var item = new Transaction();
                        item.EnforcementKey = reader["EnforcementKey"] == System.DBNull.Value ? null : (string)reader["EnforcementKey"];
                        item.CustomerId = (reader.GetInt64(reader.GetOrdinal("CustomerId"))).ToString();
                        item.ParkingSpaceID = reader["ParkingSpaceID"] == System.DBNull.Value ? default(int) : (int)reader["ParkingSpaceID"];
                        item.PlateNumber = reader["PlateNumber"] == System.DBNull.Value ? null : (string)reader["PlateNumber"];
                        item.MeterName = reader["MeterName"] == System.DBNull.Value ? null : (string)reader["MeterName"];
                        item.MeterId = reader["meterid"] == System.DBNull.Value ? null : (string)reader["meterid"];
                        item.Latitude = reader["Latitude"] == System.DBNull.Value ? default(double?) : (double)reader["Latitude"];
                        item.Latitude = reader["Longitude"] == System.DBNull.Value ? default(double?) : (double)reader["Longitude"];

                        item.LPLocation = reader["LPLocation"] == System.DBNull.Value ? null : (string)reader["LPLocation"];
                        item.VMake = null; // reader["VMake"] == System.DBNull.Value ? null : (string)reader["VMake"];
                        item.VModel = null; //reader["VModel"] == System.DBNull.Value ? null : (string)reader["VModel"];
                        item.VColour = null; //reader["VColour"] == System.DBNull.Value ? null : (string)reader["VColour"];
                        item.MeterStreet = reader["MeterStreet"] == System.DBNull.Value ? null : (string)reader["MeterStreet"];
                        item.Location = reader["Location"] == System.DBNull.Value ? null : (string)reader["Location"];

                        item.EnfVendorStallId = reader["EnfVendorStallId"] == System.DBNull.Value ? default(int?) : (Int32)reader["EnfVendorStallId"];
                        item.PresentMeterTime = reader["PresentMeterTime"] == System.DBNull.Value ? default(DateTime?) : (DateTime)reader["PresentMeterTime"];
                        item.SpaceStatus = reader["SpaceStatus"] == System.DBNull.Value ? null : (string)reader["SpaceStatus"];
                        item.SensorEventTime = reader["SensorEventTime"] == System.DBNull.Value ? default(DateTime?) : (DateTime)reader["SensorEventTime"];
                        //item.ExpiredMinutes = reader["ExpiredMinutes"] == System.DBNull.Value ? default(int?) : (Int32)reader["ExpiredMinutes"];
                        item.ExpiredMinutes = reader["ExpiredMinutes"] == System.DBNull.Value ? -1 : (Int32)reader["ExpiredMinutes"];
                        item.AmountIncents = default(decimal?); // reader["AmountIncents"] == System.DBNull.Value ? default(decimal?) : (decimal)reader["AmountIncents"];
                        item.ExpiryTime = reader["ExpiryTime"] == System.DBNull.Value ? default(DateTime?) : (DateTime)reader["ExpiryTime"];
                        item.TimeZoneId = reader["TimeZoneId"] == System.DBNull.Value ? default(int?) : (Int32)reader["TimeZoneId"];
                        //item.PayStationID = reader["PayStationID"] == System.DBNull.Value ? default(int?) : (Int32)reader["PayStationID"];
                        item.PayStationID = reader["meterid"] == System.DBNull.Value ? default(int?) : Convert.ToInt32(reader["meterid"]);
                        item.ZoneID = reader["ZoneID"] == System.DBNull.Value ? default(int?) : (Int32)reader["ZoneID"];
                        item.ZoneName = reader["ZoneName"] == System.DBNull.Value ? null : (string)reader["ZoneName"];
                        item.Block = reader["Block"] == System.DBNull.Value ? null : (string)reader["Block"];
                        item.Direction = reader["Direction"] == System.DBNull.Value ? null : (string)reader["Direction"];
                        item.MeterStreet = reader["MeterStreet"] == System.DBNull.Value ? null : (string)reader["MeterStreet"];
                        item.StreetType = reader["StreetType"] == System.DBNull.Value ? null : (string)reader["StreetType"];
                        item.MeterStreet = reader["MeterStreet"] == System.DBNull.Value ? null : (string)reader["MeterStreet"];
                        item.CrossStreet1 = reader["CrossStreet1"] == System.DBNull.Value ? null : (string)reader["CrossStreet1"];

                        item.CrossStreet2 = reader["CrossStreet2"] == System.DBNull.Value ? null : (string)reader["CrossStreet2"];
                        item.DigiELType = default(int?); //reader["DigiELType"] == System.DBNull.Value ? default(int?) : (Int32)reader["DigiELType"];
                        item.DigiErrorDescr = null; // reader["DigiErrorDescr"] == System.DBNull.Value ? null : (string)reader["DigiErrorDescr"];
                        item.GeneticELType = default(int?); // reader["GeneticELType"] == System.DBNull.Value ? default(int?) : (Int32)reader["GeneticELType"];
                        item.GeneticErrorDescr = null; //reader["GeneticErrorDescr"] == System.DBNull.Value ? null : (string)reader["GeneticErrorDescr"];

                        if (reader["PMELType"] == System.DBNull.Value)
                        {
                            item.PMELType = default(int?);
                        }
                        else
                        {
                            string pMELTypeValue = reader["PMELType"].ToString();
                            if (string.IsNullOrWhiteSpace(pMELTypeValue))
                            {
                                item.PMELType = default(int?);
                            }
                            else
                            {
                                item.PMELType = (Int32)reader["PMELType"];
                            }
                        }



                        item.PMErrorDescr = null; //reader["PMErrorDescr"] == System.DBNull.Value ? null : (string)reader["PMErrorDescr"];
                        item.EnfHourDesc = reader["EnfHourDesc"] == System.DBNull.Value ? null : (string)reader["EnfHourDesc"];
                        item.MarkedSince = reader["MarkedSince"] == System.DBNull.Value ? default(DateTime?) : (DateTime)reader["MarkedSince"];
                        item.HitDescription = reader["HitDescription"] == System.DBNull.Value ? null : (string)reader["HitDescription"];


                        if (DataRecordExtensions.HasColumn(reader, "Metertype"))
                            item.METERLOC_METERTYPE = reader["Metertype"] == System.DBNull.Value ? null : (string)reader["Metertype"];
                        if (DataRecordExtensions.HasColumn(reader, "ZoneName"))
                            item.ZoneName = reader["ZoneName"] == System.DBNull.Value ? null : (string)reader["ZoneName"];


                        if (DataRecordExtensions.HasColumn(reader, "Recieptid"))
                            item.Recieptid = reader["Recieptid"] == System.DBNull.Value ? null : (string)reader["Recieptid"];

                        if (DataRecordExtensions.HasColumn(reader, "RateNumber"))
                            item.RateNumber = reader["RateNumber"] == System.DBNull.Value ? null : (string)reader["RateNumber"];


                        validatedData.Add(item);
                    }
                }
            }
            catch (Exception ex)
            {
                throw;
            }
            finally
            {
                if (connection.State == ConnectionState.Open)
                    connection.Close();
            }
            return validatedData;
        }
        #endregion

        #region FuelCity
        public static List<Transaction> GetFuelCityCitationData(int customerID, string ZoneID, string MeterName, string ParkingSpaceID, string PlateNumber, string SpaceStatus)
        {
            List<Transaction> validatedData = new List<Transaction>();
            SqlConnection connection = null;
            try
            {
                connection = new SqlConnection(System.Configuration.ConfigurationManager.ConnectionStrings["Connector"].ToString());
                SqlCommand sqlCommand = new SqlCommand();
                sqlCommand.CommandType = CommandType.StoredProcedure;
                sqlCommand.CommandText = "SpacePlateTxnFinal_Multispace_FuelCity";
                sqlCommand.Parameters.AddWithValue("@customerID", customerID);
                sqlCommand.Parameters.AddWithValue("@ZoneID", ZoneID);
                //sqlCommand.Parameters.AddWithValue("@MeterName", MeterName);
                sqlCommand.Parameters.AddWithValue("@MeterName", MeterName);
                sqlCommand.Parameters.AddWithValue("@ParkingSpaceID", ParkingSpaceID);
                sqlCommand.Parameters.AddWithValue("@PlateNumber", PlateNumber);
                sqlCommand.Parameters.AddWithValue("@SpaceStatus", SpaceStatus);
                sqlCommand.Connection = connection;
                sqlCommand.CommandTimeout = 120;
                connection.Open();
                using (var reader = sqlCommand.ExecuteReader())
                {
                    while (reader.Read())
                    {
                        var item = new Transaction();
                        item.EnforcementKey = reader["EnforcementKey"] == System.DBNull.Value ? null : (string)reader["EnforcementKey"];
                        item.CustomerId = (reader.GetInt64(reader.GetOrdinal("CustomerId"))).ToString();
                        item.ParkingSpaceID = reader["ParkingSpaceID"] == System.DBNull.Value ? default(int) : (int)reader["ParkingSpaceID"];
                        item.PlateNumber = reader["PlateNumber"] == System.DBNull.Value ? null : (string)reader["PlateNumber"];
                        item.MeterName = reader["MeterName"] == System.DBNull.Value ? null : (string)reader["MeterName"];

                        item.Latitude = reader["Latitude"] == System.DBNull.Value ? default(double?) : (double)reader["Latitude"];
                        item.Latitude = reader["Longitude"] == System.DBNull.Value ? default(double?) : (double)reader["Longitude"];

                        item.LPLocation = reader["LPLocation"] == System.DBNull.Value ? null : (string)reader["LPLocation"];
                        item.VMake = reader["VMake"] == System.DBNull.Value ? null : (string)reader["VMake"];
                        item.VModel = reader["VModel"] == System.DBNull.Value ? null : (string)reader["VModel"];
                        item.VColour = reader["VColour"] == System.DBNull.Value ? null : (string)reader["VColour"];
                        item.MeterStreet = reader["MeterStreet"] == System.DBNull.Value ? null : (string)reader["MeterStreet"];
                        item.Location = reader["Location"] == System.DBNull.Value ? null : (string)reader["Location"];

                        item.EnfVendorStallId = reader["EnfVendorStallId"] == System.DBNull.Value ? default(int?) : (Int32)reader["EnfVendorStallId"];
                        item.PresentMeterTime = reader["PresentMeterTime"] == System.DBNull.Value ? default(DateTime?) : (DateTime)reader["PresentMeterTime"];
                        item.SpaceStatus = reader["SpaceStatus"] == System.DBNull.Value ? null : (string)reader["SpaceStatus"];
                        item.SensorEventTime = reader["SensorEventTime"] == System.DBNull.Value ? default(DateTime?) : (DateTime)reader["SensorEventTime"];
                        //item.ExpiredMinutes = reader["ExpiredMinutes"] == System.DBNull.Value ? default(int?) : (Int32)reader["ExpiredMinutes"];
                        item.ExpiredMinutes = reader["ExpiredMinutes"] == System.DBNull.Value ? -1 : (Int32)reader["ExpiredMinutes"];
                        item.AmountIncents = reader["AmountIncents"] == System.DBNull.Value ? default(decimal?) : (decimal)reader["AmountIncents"];
                        item.ExpiryTime = reader["ExpiryTime"] == System.DBNull.Value ? default(DateTime?) : (DateTime)reader["ExpiryTime"];
                        item.TimeZoneId = reader["TimeZoneId"] == System.DBNull.Value ? default(int?) : (Int32)reader["TimeZoneId"];
                        item.PayStationID = reader["PayStationID"] == System.DBNull.Value ? default(int?) : (Int32)reader["PayStationID"];
                        item.ZoneID = reader["ZoneID"] == System.DBNull.Value ? default(int?) : (Int32)reader["ZoneID"];
                        //item.ZoneName = reader["ZoneName"] == System.DBNull.Value ? null : (string)reader["ZoneName"];
                        item.ZoneName = null;
                        item.Block = reader["Block"] == System.DBNull.Value ? null : (string)reader["Block"];
                        item.Direction = reader["Direction"] == System.DBNull.Value ? null : (string)reader["Direction"];
                        item.MeterStreet = reader["MeterStreet"] == System.DBNull.Value ? null : (string)reader["MeterStreet"];
                        item.StreetType = reader["StreetType"] == System.DBNull.Value ? null : (string)reader["StreetType"];
                        item.MeterStreet = reader["MeterStreet"] == System.DBNull.Value ? null : (string)reader["MeterStreet"];
                        item.CrossStreet1 = reader["CrossStreet1"] == System.DBNull.Value ? null : (string)reader["CrossStreet1"];

                        item.CrossStreet2 = reader["CrossStreet2"] == System.DBNull.Value ? null : (string)reader["CrossStreet2"];
                        item.DigiELType = reader["DigiELType"] == System.DBNull.Value ? default(int?) : (Int32)reader["DigiELType"];
                        item.DigiErrorDescr = reader["DigiErrorDescr"] == System.DBNull.Value ? null : (string)reader["DigiErrorDescr"];
                        // item.GeneticELType = reader["GeneticELType"] == System.DBNull.Value ? default(int?) : (Int32)reader["GeneticELType"];
                        //item.GeneticErrorDescr = reader["GeneticErrorDescr"] == System.DBNull.Value ? null : (string)reader["GeneticErrorDescr"];
                        //item.PMELType = reader["PMELType"] == System.DBNull.Value ? default(int?) : (Int32)reader["PMELType"];
                        //item.PMErrorDescr = reader["PMErrorDescr"] == System.DBNull.Value ? null : (string)reader["PMErrorDescr"];
                        //item.EnfHourDesc = reader["EnfHourDesc"] == System.DBNull.Value ? null : (string)reader["EnfHourDesc"];
                        //item.MarkedSince = reader["MarkedSince"] == System.DBNull.Value ? default(DateTime?) : (DateTime)reader["MarkedSince"];
                        //item.HitDescription = reader["HitDescription"] == System.DBNull.Value ? null : (string)reader["HitDescription"];
                        validatedData.Add(item);
                    }
                }
            }
            catch (Exception ex)
            {
                throw;
            }
            finally
            {
                if (connection.State == ConnectionState.Open)
                    connection.Close();
            }
            return validatedData;
        }
        #endregion


        #region Muscat
        public static List<Transaction> GeMuscatPlateName(int customerID, string ZoneID, string MeterName, string ParkingSpaceID, string PlateNumber, string stateName, string SpaceStatus)
        {
            List<Transaction> validatedData = new List<Transaction>();
            SqlConnection connection = null;
            try
            {
                connection = new SqlConnection(System.Configuration.ConfigurationManager.ConnectionStrings["Connector"].ToString());
                SqlCommand sqlCommand = new SqlCommand();
                sqlCommand.CommandType = CommandType.StoredProcedure;
                sqlCommand.CommandText = "[Sp_PaybyPlateDetails_Muscat]";
                sqlCommand.Parameters.AddWithValue("@customerID", customerID);
                sqlCommand.Parameters.AddWithValue("@ZoneID", ZoneID);
                sqlCommand.Parameters.AddWithValue("@MeterName", MeterName);
                sqlCommand.Parameters.AddWithValue("@ParkingSpaceID", ParkingSpaceID);
                sqlCommand.Parameters.AddWithValue("@PlateNumber", PlateNumber);
                sqlCommand.Parameters.AddWithValue("@SpaceStatus", DBNull.Value);
                //sqlCommand.Parameters.AddWithValue("@state", stateName);

                sqlCommand.Connection = connection;
                sqlCommand.CommandTimeout = 120;
                connection.Open();
                using (var reader = sqlCommand.ExecuteReader())
                {
                    while (reader.Read())
                    {
                        var item = new Transaction();
                        item.EnforcementKey = reader["EnforcementKey"] == System.DBNull.Value ? null : (string)reader["EnforcementKey"];
                        item.CustomerId = (reader.GetInt64(reader.GetOrdinal("CustomerId"))).ToString();
                        item.ParkingSpaceID = reader["ParkingSpaceID"] == System.DBNull.Value ? default(int) : (int)reader["ParkingSpaceID"];
                        item.PlateNumber = reader["PlateNumber"] == System.DBNull.Value ? null : (string)reader["PlateNumber"];
                        item.MeterName = reader["MeterName"] == System.DBNull.Value ? null : (string)reader["MeterName"];
                        item.MeterId = reader["meterid"] == System.DBNull.Value ? null : (string)reader["meterid"];
                        item.Latitude = reader["Latitude"] == System.DBNull.Value ? default(double?) : (double)reader["Latitude"];
                        item.Latitude = reader["Longitude"] == System.DBNull.Value ? default(double?) : (double)reader["Longitude"];

                        item.LPLocation = reader["LPLocation"] == System.DBNull.Value ? null : (string)reader["LPLocation"];
                        item.VMake = null; // reader["VMake"] == System.DBNull.Value ? null : (string)reader["VMake"];
                        item.VModel = null; //reader["VModel"] == System.DBNull.Value ? null : (string)reader["VModel"];
                        item.VColour = null; //reader["VColour"] == System.DBNull.Value ? null : (string)reader["VColour"];
                        item.MeterStreet = reader["MeterStreet"] == System.DBNull.Value ? null : (string)reader["MeterStreet"];
                        item.Location = reader["Location"] == System.DBNull.Value ? null : (string)reader["Location"];

                        item.EnfVendorStallId = reader["EnfVendorStallId"] == System.DBNull.Value ? default(int?) : (Int32)reader["EnfVendorStallId"];
                        item.PresentMeterTime = reader["PresentMeterTime"] == System.DBNull.Value ? default(DateTime?) : (DateTime)reader["PresentMeterTime"];
                        item.SpaceStatus = reader["SpaceStatus"] == System.DBNull.Value ? null : (string)reader["SpaceStatus"];
                        item.SensorEventTime = reader["SensorEventTime"] == System.DBNull.Value ? default(DateTime?) : (DateTime)reader["SensorEventTime"];
                        //item.ExpiredMinutes = reader["ExpiredMinutes"] == System.DBNull.Value ? default(int?) : (Int32)reader["ExpiredMinutes"];
                        item.ExpiredMinutes = reader["ExpiredMinutes"] == System.DBNull.Value ? -1 : (Int32)reader["ExpiredMinutes"];
                        item.AmountIncents = default(decimal?); // reader["AmountIncents"] == System.DBNull.Value ? default(decimal?) : (decimal)reader["AmountIncents"];
                        item.ExpiryTime = reader["ExpiryTime"] == System.DBNull.Value ? default(DateTime?) : (DateTime)reader["ExpiryTime"];
                        item.TimeZoneId = reader["TimeZoneId"] == System.DBNull.Value ? default(int?) : (Int32)reader["TimeZoneId"];
                        //item.PayStationID = reader["PayStationID"] == System.DBNull.Value ? default(int?) : (Int32)reader["PayStationID"];
                        item.PayStationID = reader["meterid"] == System.DBNull.Value ? default(int?) : Convert.ToInt32(reader["meterid"]);
                        item.ZoneID = reader["ZoneID"] == System.DBNull.Value ? default(int?) : (Int32)reader["ZoneID"];
                        item.ZoneName = reader["ZoneName"] == System.DBNull.Value ? null : (string)reader["ZoneName"];
                        item.Block = reader["Block"] == System.DBNull.Value ? null : (string)reader["Block"];
                        item.Direction = reader["Direction"] == System.DBNull.Value ? null : (string)reader["Direction"];
                        item.MeterStreet = reader["MeterStreet"] == System.DBNull.Value ? null : (string)reader["MeterStreet"];
                        item.StreetType = reader["StreetType"] == System.DBNull.Value ? null : (string)reader["StreetType"];
                        item.MeterStreet = reader["MeterStreet"] == System.DBNull.Value ? null : (string)reader["MeterStreet"];
                        item.CrossStreet1 = reader["CrossStreet1"] == System.DBNull.Value ? null : (string)reader["CrossStreet1"];

                        item.CrossStreet2 = reader["CrossStreet2"] == System.DBNull.Value ? null : (string)reader["CrossStreet2"];
                        item.DigiELType = default(int?); //reader["DigiELType"] == System.DBNull.Value ? default(int?) : (Int32)reader["DigiELType"];
                        item.DigiErrorDescr = null; // reader["DigiErrorDescr"] == System.DBNull.Value ? null : (string)reader["DigiErrorDescr"];
                        item.GeneticELType = default(int?); // reader["GeneticELType"] == System.DBNull.Value ? default(int?) : (Int32)reader["GeneticELType"];
                        item.GeneticErrorDescr = null; //reader["GeneticErrorDescr"] == System.DBNull.Value ? null : (string)reader["GeneticErrorDescr"];

                        if (reader["PMELType"] == System.DBNull.Value)
                        {
                            item.PMELType = default(int?);
                        }
                        else
                        {
                            string pMELTypeValue = reader["PMELType"].ToString();
                            if (string.IsNullOrWhiteSpace(pMELTypeValue))
                            {
                                item.PMELType = default(int?);
                            }
                            else
                            {
                                item.PMELType = (Int32)reader["PMELType"];
                            }
                        }



                        item.PMErrorDescr = null; //reader["PMErrorDescr"] == System.DBNull.Value ? null : (string)reader["PMErrorDescr"];
                        item.EnfHourDesc = reader["EnfHourDesc"] == System.DBNull.Value ? null : (string)reader["EnfHourDesc"];
                        item.MarkedSince = reader["MarkedSince"] == System.DBNull.Value ? default(DateTime?) : (DateTime)reader["MarkedSince"];
                        item.HitDescription = reader["HitDescription"] == System.DBNull.Value ? null : (string)reader["HitDescription"];


                        if (DataRecordExtensions.HasColumn(reader, "Metertype"))
                            item.METERLOC_METERTYPE = reader["Metertype"] == System.DBNull.Value ? null : (string)reader["Metertype"];
                        if (DataRecordExtensions.HasColumn(reader, "ZoneName"))
                            item.ZoneName = reader["ZoneName"] == System.DBNull.Value ? null : (string)reader["ZoneName"];


                        if (DataRecordExtensions.HasColumn(reader, "Recieptid"))
                            item.Recieptid = reader["Recieptid"] == System.DBNull.Value ? null : (string)reader["Recieptid"];

                        if (DataRecordExtensions.HasColumn(reader, "RateNumber"))
                            item.RateNumber = reader["RateNumber"] == System.DBNull.Value ? null : (string)reader["RateNumber"];


                        validatedData.Add(item);
                    }
                }
            }
            catch (Exception ex)
            {
                throw;
            }
            finally
            {
                if (connection.State == ConnectionState.Open)
                    connection.Close();
            }
            return validatedData;
        }
        #endregion


        #region GenericBySpace

        public static List<Transaction> GetGenericBySpaceData(int customerID, string ZoneID, string MeterName, string ParkingSpaceID, string PlateNumber, string SpaceStatus)
        {
            List<Transaction> validatedData = new List<Transaction>();
            SqlConnection connection = null;
            try
            {
                connection = new SqlConnection(System.Configuration.ConfigurationManager.ConnectionStrings["Connector"].ToString());
                SqlCommand sqlCommand = new SqlCommand();
                sqlCommand.CommandType = CommandType.StoredProcedure;
                //sqlCommand.CommandText = "SpacePlateTxnFinal_railegh";
                sqlCommand.CommandText = "[SpacePlateTxnFinal_Multispace]";
                sqlCommand.Parameters.AddWithValue("@customerID", customerID);
                sqlCommand.Parameters.AddWithValue("@ZoneID", ZoneID);
                sqlCommand.Parameters.AddWithValue("@MeterName", MeterName);
                sqlCommand.Parameters.AddWithValue("@ParkingSpaceID", ParkingSpaceID);
                sqlCommand.Parameters.AddWithValue("@PlateNumber", PlateNumber);
                sqlCommand.Parameters.AddWithValue("@SpaceStatus", SpaceStatus);
                sqlCommand.Connection = connection;
                sqlCommand.CommandTimeout = 120;
                connection.Open();
                using (var reader = sqlCommand.ExecuteReader())
                {
                    while (reader.Read())
                    {
                        var item = new Transaction();
                        item.EnforcementKey = reader["EnforcementKey"] == System.DBNull.Value ? null : (string)reader["EnforcementKey"];
                        item.CustomerId = (reader.GetInt64(reader.GetOrdinal("CustomerId"))).ToString();
                        item.ParkingSpaceID = reader["ParkingSpaceID"] == System.DBNull.Value ? default(int) : (int)reader["ParkingSpaceID"];
                        item.PlateNumber = reader["PlateNumber"] == System.DBNull.Value ? null : (string)reader["PlateNumber"];
                        item.MeterName = reader["MeterName"] == System.DBNull.Value ? null : (string)reader["MeterName"];
                        item.MeterId = reader["meterid"] == System.DBNull.Value ? null : (string)reader["meterid"];
                        item.Latitude = reader["Latitude"] == System.DBNull.Value ? default(double?) : (double)reader["Latitude"];
                        item.Latitude = reader["Longitude"] == System.DBNull.Value ? default(double?) : (double)reader["Longitude"];

                        item.LPLocation = reader["LPLocation"] == System.DBNull.Value ? null : (string)reader["LPLocation"];
                        item.VMake = reader["VMake"] == System.DBNull.Value ? null : (string)reader["VMake"];
                        item.VModel = reader["VModel"] == System.DBNull.Value ? null : (string)reader["VModel"];
                        item.VColour = reader["VColour"] == System.DBNull.Value ? null : (string)reader["VColour"];
                        item.MeterStreet = reader["MeterStreet"] == System.DBNull.Value ? null : (string)reader["MeterStreet"];
                        item.Location = reader["Location"] == System.DBNull.Value ? null : (string)reader["Location"];

                        item.EnfVendorStallId = reader["EnfVendorStallId"] == System.DBNull.Value ? default(int?) : (Int32)reader["EnfVendorStallId"];
                        item.PresentMeterTime = reader["PresentMeterTime"] == System.DBNull.Value ? default(DateTime?) : (DateTime)reader["PresentMeterTime"];
                        item.SpaceStatus = reader["SpaceStatus"] == System.DBNull.Value ? null : (string)reader["SpaceStatus"];
                        item.SensorEventTime = reader["SensorEventTime"] == System.DBNull.Value ? default(DateTime?) : (DateTime)reader["SensorEventTime"];
                        //item.ExpiredMinutes = reader["ExpiredMinutes"] == System.DBNull.Value ? default(int?) : (Int32)reader["ExpiredMinutes"];
                        item.ExpiredMinutes = reader["ExpiredMinutes"] == System.DBNull.Value ? -1 : (Int32)reader["ExpiredMinutes"];
                        item.AmountIncents = reader["AmountIncents"] == System.DBNull.Value ? default(decimal?) : (decimal)reader["AmountIncents"];
                        item.ExpiryTime = reader["ExpiryTime"] == System.DBNull.Value ? default(DateTime?) : (DateTime)reader["ExpiryTime"];
                        item.TimeZoneId = reader["TimeZoneId"] == System.DBNull.Value ? default(int?) : (Int32)reader["TimeZoneId"];
                        //item.PayStationID = reader["PayStationID"] == System.DBNull.Value ? default(int?) : (Int32)reader["PayStationID"];
                        item.PayStationID = reader["meterid"] == System.DBNull.Value ? default(int?) : Convert.ToInt32(reader["meterid"]);
                        item.ZoneID = reader["ZoneID"] == System.DBNull.Value ? default(int?) : (Int32)reader["ZoneID"];
                        item.ZoneName = reader["ZoneName"] == System.DBNull.Value ? null : (string)reader["ZoneName"];
                        item.Block = reader["Block"] == System.DBNull.Value ? null : (string)reader["Block"];
                        item.Direction = reader["Direction"] == System.DBNull.Value ? null : (string)reader["Direction"];
                        item.MeterStreet = reader["MeterStreet"] == System.DBNull.Value ? null : (string)reader["MeterStreet"];
                        item.StreetType = reader["StreetType"] == System.DBNull.Value ? null : (string)reader["StreetType"];
                        item.MeterStreet = reader["MeterStreet"] == System.DBNull.Value ? null : (string)reader["MeterStreet"];
                        item.CrossStreet1 = reader["CrossStreet1"] == System.DBNull.Value ? null : (string)reader["CrossStreet1"];

                        item.CrossStreet2 = reader["CrossStreet2"] == System.DBNull.Value ? null : (string)reader["CrossStreet2"];
                        item.DigiELType = reader["DigiELType"] == System.DBNull.Value ? default(int?) : (Int32)reader["DigiELType"];
                        item.DigiErrorDescr = reader["DigiErrorDescr"] == System.DBNull.Value ? null : (string)reader["DigiErrorDescr"];
                        item.GeneticELType = reader["GeneticELType"] == System.DBNull.Value ? default(int?) : (Int32)reader["GeneticELType"];
                        item.GeneticErrorDescr = reader["GeneticErrorDescr"] == System.DBNull.Value ? null : (string)reader["GeneticErrorDescr"];

                        if (reader["PMELType"] == System.DBNull.Value)
                        {
                            item.PMELType = default(int?);
                        }
                        else
                        {
                            string pMELTypeValue = reader["PMELType"].ToString();
                            if (string.IsNullOrWhiteSpace(pMELTypeValue))
                            {
                                item.PMELType = default(int?);
                            }
                            else
                            {
                                item.PMELType = (Int32)reader["PMELType"];
                            }
                        }



                        item.PMErrorDescr = reader["PMErrorDescr"] == System.DBNull.Value ? null : (string)reader["PMErrorDescr"];
                        item.EnfHourDesc = reader["EnfHourDesc"] == System.DBNull.Value ? null : (string)reader["EnfHourDesc"];
                        item.MarkedSince = reader["MarkedSince"] == System.DBNull.Value ? default(DateTime?) : (DateTime)reader["MarkedSince"];
                        item.HitDescription = reader["HitDescription"] == System.DBNull.Value ? null : (string)reader["HitDescription"];


                        if (DataRecordExtensions.HasColumn(reader, "Metertype"))
                            item.METERLOC_METERTYPE = reader["Metertype"] == System.DBNull.Value ? null : (string)reader["Metertype"];
                        if (DataRecordExtensions.HasColumn(reader, "ZoneName"))
                            item.ZoneName = reader["ZoneName"] == System.DBNull.Value ? null : (string)reader["ZoneName"];

                        //if (DataRecordExtensions.HasColumn(reader, "MeterStatus"))
                        //    item.MeterStatus = reader["MeterStatus"] == System.DBNull.Value ? default(int?) : (Int32)reader["MeterStatus"];

                        validatedData.Add(item);
                    }
                }
            }
            catch (Exception ex)
            {
                throw;
            }
            finally
            {
                if (connection.State == ConnectionState.Open)
                    connection.Close();
            }
            return validatedData;
        }
        #endregion

        #region GenericPayByPlate
        public static List<Transaction> GetGenericPlateName(int customerID, string ZoneID, string MeterName, string ParkingSpaceID, string PlateNumber, string stateName, string SpaceStatus)
        {
            List<Transaction> validatedData = new List<Transaction>();
            SqlConnection connection = null;
            try
            {
                connection = new SqlConnection(System.Configuration.ConfigurationManager.ConnectionStrings["Connector"].ToString());
                SqlCommand sqlCommand = new SqlCommand();
                sqlCommand.CommandType = CommandType.StoredProcedure;
                sqlCommand.CommandText = "[Sp_PaybyPlateDetails]";
                sqlCommand.Parameters.AddWithValue("@customerID", customerID);
                sqlCommand.Parameters.AddWithValue("@ZoneID", ZoneID);
                sqlCommand.Parameters.AddWithValue("@MeterName", MeterName);
                sqlCommand.Parameters.AddWithValue("@ParkingSpaceID", ParkingSpaceID);
                sqlCommand.Parameters.AddWithValue("@PlateNumber", PlateNumber);
                sqlCommand.Parameters.AddWithValue("@SpaceStatus", DBNull.Value);
                //sqlCommand.Parameters.AddWithValue("@state", stateName);

                sqlCommand.Connection = connection;
                sqlCommand.CommandTimeout = 120;
                connection.Open();
                using (var reader = sqlCommand.ExecuteReader())
                {
                    while (reader.Read())
                    {
                        var item = new Transaction();
                        item.EnforcementKey = reader["EnforcementKey"] == System.DBNull.Value ? null : (string)reader["EnforcementKey"];
                        item.CustomerId = (reader.GetInt64(reader.GetOrdinal("CustomerId"))).ToString();
                        item.ParkingSpaceID = reader["ParkingSpaceID"] == System.DBNull.Value ? default(int) : (int)reader["ParkingSpaceID"];
                        item.PlateNumber = reader["PlateNumber"] == System.DBNull.Value ? null : (string)reader["PlateNumber"];
                        item.MeterName = reader["MeterName"] == System.DBNull.Value ? null : (string)reader["MeterName"];
                        item.MeterId = reader["meterid"] == System.DBNull.Value ? null : (string)reader["meterid"];
                        item.Latitude = reader["Latitude"] == System.DBNull.Value ? default(double?) : (double)reader["Latitude"];
                        item.Latitude = reader["Longitude"] == System.DBNull.Value ? default(double?) : (double)reader["Longitude"];

                        item.LPLocation = reader["LPLocation"] == System.DBNull.Value ? null : (string)reader["LPLocation"];
                        item.VMake = null; // reader["VMake"] == System.DBNull.Value ? null : (string)reader["VMake"];
                        item.VModel = null; //reader["VModel"] == System.DBNull.Value ? null : (string)reader["VModel"];
                        item.VColour = null; //reader["VColour"] == System.DBNull.Value ? null : (string)reader["VColour"];
                        item.MeterStreet = reader["MeterStreet"] == System.DBNull.Value ? null : (string)reader["MeterStreet"];
                        item.Location = reader["Location"] == System.DBNull.Value ? null : (string)reader["Location"];

                        item.EnfVendorStallId = reader["EnfVendorStallId"] == System.DBNull.Value ? default(int?) : (Int32)reader["EnfVendorStallId"];
                        item.PresentMeterTime = reader["PresentMeterTime"] == System.DBNull.Value ? default(DateTime?) : (DateTime)reader["PresentMeterTime"];
                        item.SpaceStatus = reader["SpaceStatus"] == System.DBNull.Value ? null : (string)reader["SpaceStatus"];
                        item.SensorEventTime = reader["SensorEventTime"] == System.DBNull.Value ? default(DateTime?) : (DateTime)reader["SensorEventTime"];
                        //item.ExpiredMinutes = reader["ExpiredMinutes"] == System.DBNull.Value ? default(int?) : (Int32)reader["ExpiredMinutes"];
                        item.ExpiredMinutes = reader["ExpiredMinutes"] == System.DBNull.Value ? -1 : (Int32)reader["ExpiredMinutes"];
                        item.AmountIncents = default(decimal?); // reader["AmountIncents"] == System.DBNull.Value ? default(decimal?) : (decimal)reader["AmountIncents"];
                        item.ExpiryTime = reader["ExpiryTime"] == System.DBNull.Value ? default(DateTime?) : (DateTime)reader["ExpiryTime"];
                        item.TimeZoneId = reader["TimeZoneId"] == System.DBNull.Value ? default(int?) : (Int32)reader["TimeZoneId"];
                        //item.PayStationID = reader["PayStationID"] == System.DBNull.Value ? default(int?) : (Int32)reader["PayStationID"];
                        item.PayStationID = reader["meterid"] == System.DBNull.Value ? default(int?) : Convert.ToInt32(reader["meterid"]);
                        item.ZoneID = reader["ZoneID"] == System.DBNull.Value ? default(int?) : (Int32)reader["ZoneID"];
                        item.ZoneName = reader["ZoneName"] == System.DBNull.Value ? null : (string)reader["ZoneName"];
                        item.Block = reader["Block"] == System.DBNull.Value ? null : (string)reader["Block"];
                        item.Direction = reader["Direction"] == System.DBNull.Value ? null : (string)reader["Direction"];
                        item.MeterStreet = reader["MeterStreet"] == System.DBNull.Value ? null : (string)reader["MeterStreet"];
                        item.StreetType = reader["StreetType"] == System.DBNull.Value ? null : (string)reader["StreetType"];
                        item.MeterStreet = reader["MeterStreet"] == System.DBNull.Value ? null : (string)reader["MeterStreet"];
                        item.CrossStreet1 = reader["CrossStreet1"] == System.DBNull.Value ? null : (string)reader["CrossStreet1"];

                        item.CrossStreet2 = reader["CrossStreet2"] == System.DBNull.Value ? null : (string)reader["CrossStreet2"];
                        item.DigiELType = default(int?); //reader["DigiELType"] == System.DBNull.Value ? default(int?) : (Int32)reader["DigiELType"];
                        item.DigiErrorDescr = null; // reader["DigiErrorDescr"] == System.DBNull.Value ? null : (string)reader["DigiErrorDescr"];
                        item.GeneticELType = default(int?); // reader["GeneticELType"] == System.DBNull.Value ? default(int?) : (Int32)reader["GeneticELType"];
                        item.GeneticErrorDescr = null; //reader["GeneticErrorDescr"] == System.DBNull.Value ? null : (string)reader["GeneticErrorDescr"];

                        if (reader["PMELType"] == System.DBNull.Value)
                        {
                            item.PMELType = default(int?);
                        }
                        else
                        {
                            string pMELTypeValue = reader["PMELType"].ToString();
                            if (string.IsNullOrWhiteSpace(pMELTypeValue))
                            {
                                item.PMELType = default(int?);
                            }
                            else
                            {
                                item.PMELType = (Int32)reader["PMELType"];
                            }
                        }



                        item.PMErrorDescr = null; //reader["PMErrorDescr"] == System.DBNull.Value ? null : (string)reader["PMErrorDescr"];
                        item.EnfHourDesc = reader["EnfHourDesc"] == System.DBNull.Value ? null : (string)reader["EnfHourDesc"];
                        item.MarkedSince = reader["MarkedSince"] == System.DBNull.Value ? default(DateTime?) : (DateTime)reader["MarkedSince"];
                        item.HitDescription = reader["HitDescription"] == System.DBNull.Value ? null : (string)reader["HitDescription"];


                        if (DataRecordExtensions.HasColumn(reader, "Metertype"))
                            item.METERLOC_METERTYPE = reader["Metertype"] == System.DBNull.Value ? null : (string)reader["Metertype"];
                        if (DataRecordExtensions.HasColumn(reader, "ZoneName"))
                            item.ZoneName = reader["ZoneName"] == System.DBNull.Value ? null : (string)reader["ZoneName"];


                        if (DataRecordExtensions.HasColumn(reader, "Recieptid"))
                            item.Recieptid = reader["Recieptid"] == System.DBNull.Value ? null : (string)reader["Recieptid"];

                        if (DataRecordExtensions.HasColumn(reader, "RateNumber"))
                            item.RateNumber = reader["RateNumber"] == System.DBNull.Value ? null : (string)reader["RateNumber"];


                        validatedData.Add(item);
                    }
                }
            }
            catch (Exception ex)
            {
                throw;
            }
            finally
            {
                if (connection.State == ConnectionState.Open)
                    connection.Close();
            }
            return validatedData;
        }

        public static List<Transaction> GetGenericPlateName2(int customerID, string ZoneID, string MeterName, string ParkingSpaceID, string PlateNumber, string stateName, string SpaceStatus)
        {
            List<Transaction> validatedData = new List<Transaction>();
            SqlConnection connection = null;
            try
            {
                connection = new SqlConnection(System.Configuration.ConfigurationManager.ConnectionStrings["Connector"].ToString());
                SqlCommand sqlCommand = new SqlCommand();
                sqlCommand.CommandType = CommandType.StoredProcedure;
                sqlCommand.CommandText = "[Sp_PaybyPlateDetails]";
                sqlCommand.Parameters.AddWithValue("@customerID", customerID);
                sqlCommand.Parameters.AddWithValue("@ZoneID", ZoneID);
                sqlCommand.Parameters.AddWithValue("@MeterName", MeterName);
                sqlCommand.Parameters.AddWithValue("@ParkingSpaceID", ParkingSpaceID);
                sqlCommand.Parameters.AddWithValue("@PlateNumber", PlateNumber);
                sqlCommand.Parameters.AddWithValue("@SpaceStatus", DBNull.Value);
                //sqlCommand.Parameters.AddWithValue("@state", stateName);

                sqlCommand.Connection = connection;
                sqlCommand.CommandTimeout = 120;
                connection.Open();
                using (var reader = sqlCommand.ExecuteReader())
                {
                    while (reader.Read())
                    {
                        var item = new Transaction();
                        item.EnforcementKey = reader["EnforcementKey"] == System.DBNull.Value ? null : (string)reader["EnforcementKey"];
                        item.CustomerId = (reader.GetInt64(reader.GetOrdinal("CustomerId"))).ToString();
                        item.ParkingSpaceID = reader["ParkingSpaceID"] == System.DBNull.Value ? default(int) : (int)reader["ParkingSpaceID"];
                        item.PlateNumber = reader["PlateNumber"] == System.DBNull.Value ? null : (string)reader["PlateNumber"];
                        item.MeterName = reader["MeterName"] == System.DBNull.Value ? null : (string)reader["MeterName"];
                        //item.MeterId = reader["meterid"] == System.DBNull.Value ? null : (string)reader["meterid"];

                        if (DataRecordExtensions.HasColumn(reader, "meterid"))
                        {
                            if ((reader["meterid"].GetType()) == typeof(Int32))
                                item.MeterId = reader["meterid"] == System.DBNull.Value ? null : ((Int32)reader["meterid"]).ToString();
                            else if ((reader["meterid"].GetType()) == typeof(Int64))
                                item.MeterId = reader["meterid"] == System.DBNull.Value ? null : ((Int64)reader["meterid"]).ToString();
                            else
                                item.MeterId = reader["meterid"] == System.DBNull.Value ? null : (string)reader["meterid"];
                        }

                        item.Latitude = reader["Latitude"] == System.DBNull.Value ? default(double?) : (double)reader["Latitude"];
                        item.Latitude = reader["Longitude"] == System.DBNull.Value ? default(double?) : (double)reader["Longitude"];

                        item.LPLocation = reader["LPLocation"] == System.DBNull.Value ? null : (string)reader["LPLocation"];
                        item.VMake = null; // reader["VMake"] == System.DBNull.Value ? null : (string)reader["VMake"];
                        item.VModel = null; //reader["VModel"] == System.DBNull.Value ? null : (string)reader["VModel"];
                        item.VColour = null; //reader["VColour"] == System.DBNull.Value ? null : (string)reader["VColour"];
                        item.MeterStreet = reader["MeterStreet"] == System.DBNull.Value ? null : (string)reader["MeterStreet"];
                        item.Location = reader["Location"] == System.DBNull.Value ? null : (string)reader["Location"];

                        item.EnfVendorStallId = reader["EnfVendorStallId"] == System.DBNull.Value ? default(int?) : (Int32)reader["EnfVendorStallId"];
                        item.PresentMeterTime = reader["PresentMeterTime"] == System.DBNull.Value ? default(DateTime?) : (DateTime)reader["PresentMeterTime"];
                        item.SpaceStatus = reader["SpaceStatus"] == System.DBNull.Value ? null : (string)reader["SpaceStatus"];
                        item.SensorEventTime = reader["SensorEventTime"] == System.DBNull.Value ? default(DateTime?) : (DateTime)reader["SensorEventTime"];
                        //item.ExpiredMinutes = reader["ExpiredMinutes"] == System.DBNull.Value ? default(int?) : (Int32)reader["ExpiredMinutes"];
                        item.ExpiredMinutes = reader["ExpiredMinutes"] == System.DBNull.Value ? -1 : (Int32)reader["ExpiredMinutes"];
                        item.AmountIncents = default(decimal?); // reader["AmountIncents"] == System.DBNull.Value ? default(decimal?) : (decimal)reader["AmountIncents"];
                        item.ExpiryTime = reader["ExpiryTime"] == System.DBNull.Value ? default(DateTime?) : (DateTime)reader["ExpiryTime"];
                        item.TimeZoneId = reader["TimeZoneId"] == System.DBNull.Value ? default(int?) : (Int32)reader["TimeZoneId"];
                        //item.PayStationID = reader["PayStationID"] == System.DBNull.Value ? default(int?) : (Int32)reader["PayStationID"];
                        //item.PayStationID = reader["meterid"] == System.DBNull.Value ? default(int?) : Convert.ToInt32(reader["meterid"]);
                        if (DataRecordExtensions.HasColumn(reader, "meterid"))
                        {
                            if ((reader["meterid"].GetType()) == typeof(Int32?))
                                item.PayStationID = reader["meterid"] == System.DBNull.Value ? null : ((Int32?)reader["meterid"]);
                        }
                        item.ZoneID = reader["ZoneID"] == System.DBNull.Value ? default(int?) : (Int32)reader["ZoneID"];
                        //item.ZoneName = reader["ZoneName"] == System.DBNull.Value ? null : (string)reader["ZoneName"];
                        item.Block = reader["Block"] == System.DBNull.Value ? null : (string)reader["Block"];
                        item.Direction = reader["Direction"] == System.DBNull.Value ? null : (string)reader["Direction"];
                        item.MeterStreet = reader["MeterStreet"] == System.DBNull.Value ? null : (string)reader["MeterStreet"];
                        item.StreetType = reader["StreetType"] == System.DBNull.Value ? null : (string)reader["StreetType"];
                        item.MeterStreet = reader["MeterStreet"] == System.DBNull.Value ? null : (string)reader["MeterStreet"];
                        item.CrossStreet1 = reader["CrossStreet1"] == System.DBNull.Value ? null : (string)reader["CrossStreet1"];

                        item.CrossStreet2 = reader["CrossStreet2"] == System.DBNull.Value ? null : (string)reader["CrossStreet2"];
                        item.DigiELType = default(int?); //reader["DigiELType"] == System.DBNull.Value ? default(int?) : (Int32)reader["DigiELType"];
                        item.DigiErrorDescr = null; // reader["DigiErrorDescr"] == System.DBNull.Value ? null : (string)reader["DigiErrorDescr"];
                        item.GeneticELType = default(int?); // reader["GeneticELType"] == System.DBNull.Value ? default(int?) : (Int32)reader["GeneticELType"];
                        item.GeneticErrorDescr = null; //reader["GeneticErrorDescr"] == System.DBNull.Value ? null : (string)reader["GeneticErrorDescr"];

                        if (reader["PMELType"] == System.DBNull.Value)
                        {
                            item.PMELType = default(int?);
                        }
                        else
                        {
                            string pMELTypeValue = reader["PMELType"].ToString();
                            if (string.IsNullOrWhiteSpace(pMELTypeValue))
                            {
                                item.PMELType = default(int?);
                            }
                            else
                            {
                                item.PMELType = (Int32)reader["PMELType"];
                            }
                        }



                        item.PMErrorDescr = null; //reader["PMErrorDescr"] == System.DBNull.Value ? null : (string)reader["PMErrorDescr"];
                        item.EnfHourDesc = reader["EnfHourDesc"] == System.DBNull.Value ? null : (string)reader["EnfHourDesc"];
                        item.MarkedSince = reader["MarkedSince"] == System.DBNull.Value ? default(DateTime?) : (DateTime)reader["MarkedSince"];
                        item.HitDescription = reader["HitDescription"] == System.DBNull.Value ? null : (string)reader["HitDescription"];


                        if (DataRecordExtensions.HasColumn(reader, "Metertype"))
                            item.METERLOC_METERTYPE = reader["Metertype"] == System.DBNull.Value ? null : (string)reader["Metertype"];
                        if (DataRecordExtensions.HasColumn(reader, "ZoneName"))
                        {
                            //item.ZoneName = reader["ZoneName"] == System.DBNull.Value ? null : (string)reader["ZoneName"];

                            if ((reader["ZoneName"].GetType()) == typeof(Int32))
                                item.ZoneName = reader["ZoneName"] == System.DBNull.Value ? null : ((Int32)reader["ZoneName"]).ToString();
                            else if ((reader["ZoneName"].GetType()) == typeof(Int64))
                                item.ZoneName = reader["ZoneName"] == System.DBNull.Value ? null : ((Int64)reader["ZoneName"]).ToString();
                            else
                                item.ZoneName = reader["ZoneName"] == System.DBNull.Value ? null : (string)reader["ZoneName"];
                        }
                           


                        if (DataRecordExtensions.HasColumn(reader, "Recieptid"))
                            item.Recieptid = reader["Recieptid"] == System.DBNull.Value ? null : (string)reader["Recieptid"];

                        if (DataRecordExtensions.HasColumn(reader, "RateNumber"))
                            item.RateNumber = reader["RateNumber"] == System.DBNull.Value ? null : (string)reader["RateNumber"];


                        validatedData.Add(item);
                    }
                }
            }
            catch (Exception ex)
            {
                throw;
            }
            finally
            {
                if (connection.State == ConnectionState.Open)
                    connection.Close();
            }
            return validatedData;
        }


        public static List<Transaction> GetGenericPlateName3(int customerID, string ZoneID, string MeterName, string ParkingSpaceID, string PlateNumber, string stateName, string SpaceStatus)
        {
            List<Transaction> validatedData = new List<Transaction>();
            SqlConnection connection = null;
            try
            {
                connection = new SqlConnection(System.Configuration.ConfigurationManager.ConnectionStrings["Connector"].ToString());
                SqlCommand sqlCommand = new SqlCommand();
                sqlCommand.CommandType = CommandType.StoredProcedure;
                sqlCommand.CommandText = "[Sp_PaybyPlateDetails_Test_Salem]";
                sqlCommand.Parameters.AddWithValue("@customerID", customerID);
                sqlCommand.Parameters.AddWithValue("@ZoneID", ZoneID);
                sqlCommand.Parameters.AddWithValue("@MeterName", MeterName);
                sqlCommand.Parameters.AddWithValue("@ParkingSpaceID", ParkingSpaceID);
                sqlCommand.Parameters.AddWithValue("@PlateNumber", PlateNumber);
                sqlCommand.Parameters.AddWithValue("@SpaceStatus", DBNull.Value);
                //sqlCommand.Parameters.AddWithValue("@state", stateName);

                sqlCommand.Connection = connection;
                sqlCommand.CommandTimeout = 120;
                connection.Open();
                using (var reader = sqlCommand.ExecuteReader())
                {
                    while (reader.Read())
                    {
                        var item = new Transaction();
                        item.EnforcementKey = reader["EnforcementKey"] == System.DBNull.Value ? null : (string)reader["EnforcementKey"];
                        item.CustomerId = (reader.GetInt64(reader.GetOrdinal("CustomerId"))).ToString();
                        item.ParkingSpaceID = reader["ParkingSpaceID"] == System.DBNull.Value ? default(int) : (int)reader["ParkingSpaceID"];
                        item.PlateNumber = reader["PlateNumber"] == System.DBNull.Value ? null : (string)reader["PlateNumber"];
                        item.MeterName = reader["MeterName"] == System.DBNull.Value ? null : (string)reader["MeterName"];
                        //item.MeterId = reader["meterid"] == System.DBNull.Value ? null : (string)reader["meterid"];

                        if (DataRecordExtensions.HasColumn(reader, "meterid"))
                        {
                            if ((reader["meterid"].GetType()) == typeof(Int32))
                                item.MeterId = reader["meterid"] == System.DBNull.Value ? null : ((Int32)reader["meterid"]).ToString();
                            else if ((reader["meterid"].GetType()) == typeof(Int64))
                                item.MeterId = reader["meterid"] == System.DBNull.Value ? null : ((Int64)reader["meterid"]).ToString();
                            else
                                item.MeterId = reader["meterid"] == System.DBNull.Value ? null : (string)reader["meterid"];
                        }

                        item.Latitude = reader["Latitude"] == System.DBNull.Value ? default(double?) : (double)reader["Latitude"];
                        item.Latitude = reader["Longitude"] == System.DBNull.Value ? default(double?) : (double)reader["Longitude"];

                        item.LPLocation = reader["LPLocation"] == System.DBNull.Value ? null : (string)reader["LPLocation"];
                        item.VMake = null; // reader["VMake"] == System.DBNull.Value ? null : (string)reader["VMake"];
                        item.VModel = null; //reader["VModel"] == System.DBNull.Value ? null : (string)reader["VModel"];
                        item.VColour = null; //reader["VColour"] == System.DBNull.Value ? null : (string)reader["VColour"];
                        item.MeterStreet = reader["MeterStreet"] == System.DBNull.Value ? null : (string)reader["MeterStreet"];
                        item.Location = reader["Location"] == System.DBNull.Value ? null : (string)reader["Location"];

                        item.EnfVendorStallId = reader["EnfVendorStallId"] == System.DBNull.Value ? default(int?) : (Int32)reader["EnfVendorStallId"];
                        item.PresentMeterTime = reader["PresentMeterTime"] == System.DBNull.Value ? default(DateTime?) : (DateTime)reader["PresentMeterTime"];
                        item.SpaceStatus = reader["SpaceStatus"] == System.DBNull.Value ? null : (string)reader["SpaceStatus"];
                        item.SensorEventTime = reader["SensorEventTime"] == System.DBNull.Value ? default(DateTime?) : (DateTime)reader["SensorEventTime"];
                        //item.ExpiredMinutes = reader["ExpiredMinutes"] == System.DBNull.Value ? default(int?) : (Int32)reader["ExpiredMinutes"];
                        item.ExpiredMinutes = reader["ExpiredMinutes"] == System.DBNull.Value ? -1 : (Int32)reader["ExpiredMinutes"];
                        item.AmountIncents = default(decimal?); // reader["AmountIncents"] == System.DBNull.Value ? default(decimal?) : (decimal)reader["AmountIncents"];
                        item.ExpiryTime = reader["ExpiryTime"] == System.DBNull.Value ? default(DateTime?) : (DateTime)reader["ExpiryTime"];
                        item.TimeZoneId = reader["TimeZoneId"] == System.DBNull.Value ? default(int?) : (Int32)reader["TimeZoneId"];
                        //item.PayStationID = reader["PayStationID"] == System.DBNull.Value ? default(int?) : (Int32)reader["PayStationID"];
                        //item.PayStationID = reader["meterid"] == System.DBNull.Value ? default(int?) : Convert.ToInt32(reader["meterid"]);
                        if (DataRecordExtensions.HasColumn(reader, "meterid"))
                        {
                            if ((reader["meterid"].GetType()) == typeof(Int32?))
                                item.PayStationID = reader["meterid"] == System.DBNull.Value ? null : ((Int32?)reader["meterid"]);
                        }
                        item.ZoneID = reader["ZoneID"] == System.DBNull.Value ? default(int?) : (Int32)reader["ZoneID"];
                        item.ZoneName = reader["ZoneName"] == System.DBNull.Value ? null : (string)reader["ZoneName"];
                        item.Block = reader["Block"] == System.DBNull.Value ? null : (string)reader["Block"];
                        item.Direction = reader["Direction"] == System.DBNull.Value ? null : (string)reader["Direction"];
                        item.MeterStreet = reader["MeterStreet"] == System.DBNull.Value ? null : (string)reader["MeterStreet"];
                        item.StreetType = reader["StreetType"] == System.DBNull.Value ? null : (string)reader["StreetType"];
                        item.MeterStreet = reader["MeterStreet"] == System.DBNull.Value ? null : (string)reader["MeterStreet"];
                        item.CrossStreet1 = reader["CrossStreet1"] == System.DBNull.Value ? null : (string)reader["CrossStreet1"];

                        item.CrossStreet2 = reader["CrossStreet2"] == System.DBNull.Value ? null : (string)reader["CrossStreet2"];
                        item.DigiELType = default(int?); //reader["DigiELType"] == System.DBNull.Value ? default(int?) : (Int32)reader["DigiELType"];
                        item.DigiErrorDescr = null; // reader["DigiErrorDescr"] == System.DBNull.Value ? null : (string)reader["DigiErrorDescr"];
                        item.GeneticELType = default(int?); // reader["GeneticELType"] == System.DBNull.Value ? default(int?) : (Int32)reader["GeneticELType"];
                        item.GeneticErrorDescr = null; //reader["GeneticErrorDescr"] == System.DBNull.Value ? null : (string)reader["GeneticErrorDescr"];

                        if (reader["PMELType"] == System.DBNull.Value)
                        {
                            item.PMELType = default(int?);
                        }
                        else
                        {
                            string pMELTypeValue = reader["PMELType"].ToString();
                            if (string.IsNullOrWhiteSpace(pMELTypeValue))
                            {
                                item.PMELType = default(int?);
                            }
                            else
                            {
                                item.PMELType = (Int32)reader["PMELType"];
                            }
                        }



                        item.PMErrorDescr = null; //reader["PMErrorDescr"] == System.DBNull.Value ? null : (string)reader["PMErrorDescr"];
                        item.EnfHourDesc = reader["EnfHourDesc"] == System.DBNull.Value ? null : (string)reader["EnfHourDesc"];
                        item.MarkedSince = reader["MarkedSince"] == System.DBNull.Value ? default(DateTime?) : (DateTime)reader["MarkedSince"];
                        item.HitDescription = reader["HitDescription"] == System.DBNull.Value ? null : (string)reader["HitDescription"];


                        if (DataRecordExtensions.HasColumn(reader, "Metertype"))
                            item.METERLOC_METERTYPE = reader["Metertype"] == System.DBNull.Value ? null : (string)reader["Metertype"];
                        if (DataRecordExtensions.HasColumn(reader, "ZoneName"))
                        {
                            //item.ZoneName = reader["ZoneName"] == System.DBNull.Value ? null : (string)reader["ZoneName"];

                            if ((reader["ZoneName"].GetType()) == typeof(Int32))
                                item.ZoneName = reader["ZoneName"] == System.DBNull.Value ? null : ((Int32)reader["ZoneName"]).ToString();
                            else if ((reader["ZoneName"].GetType()) == typeof(Int64))
                                item.ZoneName = reader["ZoneName"] == System.DBNull.Value ? null : ((Int64)reader["ZoneName"]).ToString();
                            else
                                item.ZoneName = reader["ZoneName"] == System.DBNull.Value ? null : (string)reader["ZoneName"];
                        }



                        if (DataRecordExtensions.HasColumn(reader, "Recieptid"))
                            item.Recieptid = reader["Recieptid"] == System.DBNull.Value ? null : (string)reader["Recieptid"];

                        if (DataRecordExtensions.HasColumn(reader, "RateNumber"))
                            item.RateNumber = reader["RateNumber"] == System.DBNull.Value ? null : (string)reader["RateNumber"];


                        validatedData.Add(item);
                    }
                }
            }
            catch (Exception ex)
            {
                throw;
            }
            finally
            {
                if (connection.State == ConnectionState.Open)
                    connection.Close();
            }
            return validatedData;
        }

        public static ZoneSpace GetZoneSpaceForMeter(int customerId, string meterName)
        {

            ///Not complete
            ZoneSpace zoneSpace = null;
            SqlConnection connection = null;
            try
            {
                connection = new SqlConnection(System.Configuration.ConfigurationManager.ConnectionStrings["Connector"].ToString());
                SqlCommand sqlCommand = new SqlCommand();
                sqlCommand.CommandType = CommandType.StoredProcedure;
                sqlCommand.CommandText = "[Sp_EnfZoneSpaceFromMeterName]";
                sqlCommand.Parameters.AddWithValue("@action", "GetZoneSpaceByMeterName");
                sqlCommand.Parameters.AddWithValue("@customerId", customerId);
                sqlCommand.Parameters.AddWithValue("@meterName", meterName);
                sqlCommand.Connection = connection;
                sqlCommand.CommandTimeout = 90;
                connection.Open();
                using (var reader = sqlCommand.ExecuteReader())
                {
                    while (reader.Read())
                    {
                        zoneSpace = new ZoneSpace();
                        if (DataRecordExtensions.HasColumn(reader, "ZoneId"))
                        {
                            if ((reader["ZoneId"].GetType()) == typeof(Int32))
                                zoneSpace.Zone = reader["ZoneId"] == System.DBNull.Value ? null : ((Int32)reader["ZoneId"]).ToString();
                            else if ((reader["ZoneId"].GetType()) == typeof(Int64))
                                zoneSpace.Zone = reader["ZoneId"] == System.DBNull.Value ? null : ((Int64)reader["ZoneId"]).ToString();
                            else
                                zoneSpace.Zone = reader["ZoneId"] == System.DBNull.Value ? null : (string)reader["ZoneId"];
                        }
                        if (DataRecordExtensions.HasColumn(reader, "StallNumber"))
                        {
                            if ((reader["StallNumber"].GetType()) == typeof(Int32))
                                zoneSpace.Space = reader["StallNumber"] == System.DBNull.Value ? null : ((Int32)reader["StallNumber"]).ToString();
                            else if ((reader["StallNumber"].GetType()) == typeof(Int64))
                                zoneSpace.Space = reader["StallNumber"] == System.DBNull.Value ? null : ((Int64)reader["StallNumber"]).ToString();
                            else
                                zoneSpace.Space = reader["StallNumber"] == System.DBNull.Value ? null : (string)reader["StallNumber"];
                        }
                    }
                }
            }
            catch (Exception ex)
            {
                throw;
            }
            finally
            {
                if (connection.State == ConnectionState.Open)
                    connection.Close();
            }
            return zoneSpace;
        }
        #endregion


        #region NEW and RnD And Testing

        public static List<Transaction> GetCitationDataSouthMaimi(int customerID, string ZoneID, string MeterName, string ParkingSpaceID, string PlateNumber, string SpaceStatus)
        {
            List<Transaction> validatedData = new List<Transaction>();
            SqlConnection connection = null;
            try
            {
                connection = new SqlConnection(System.Configuration.ConfigurationManager.ConnectionStrings["Connector"].ToString());
                SqlCommand sqlCommand = new SqlCommand();
                sqlCommand.CommandType = CommandType.StoredProcedure;
                sqlCommand.CommandText = "SpacePlateTxnFinal";
                sqlCommand.Parameters.AddWithValue("@customerID", customerID);
                sqlCommand.Parameters.AddWithValue("@ZoneID", ZoneID);
                //sqlCommand.Parameters.AddWithValue("@MeterName", MeterName);
                sqlCommand.Parameters.AddWithValue("@MeterName", MeterName);
                sqlCommand.Parameters.AddWithValue("@ParkingSpaceID", ParkingSpaceID);
                sqlCommand.Parameters.AddWithValue("@PlateNumber", PlateNumber);
                sqlCommand.Parameters.AddWithValue("@SpaceStatus", SpaceStatus);
                sqlCommand.Connection = connection;
                sqlCommand.CommandTimeout = 120;
                connection.Open();
                using (var reader = sqlCommand.ExecuteReader())
                {
                    while (reader.Read())
                    {
                        var item = new Transaction();
                        item.EnforcementKey = reader["EnforcementKey"] == System.DBNull.Value ? null : (string)reader["EnforcementKey"];
                        item.CustomerId = (reader.GetInt64(reader.GetOrdinal("CustomerId"))).ToString();
                        item.ParkingSpaceID = reader["ParkingSpaceID"] == System.DBNull.Value ? default(int) : (int)reader["ParkingSpaceID"];
                        item.PlateNumber = reader["PlateNumber"] == System.DBNull.Value ? null : (string)reader["PlateNumber"];
                        item.MeterName = reader["MeterName"] == System.DBNull.Value ? null : (string)reader["MeterName"];

                        item.Latitude = reader["Latitude"] == System.DBNull.Value ? default(double?) : (double)reader["Latitude"];
                        item.Latitude = reader["Longitude"] == System.DBNull.Value ? default(double?) : (double)reader["Longitude"];

                        item.LPLocation = reader["LPLocation"] == System.DBNull.Value ? null : (string)reader["LPLocation"];
                        item.VMake = reader["VMake"] == System.DBNull.Value ? null : (string)reader["VMake"];
                        item.VModel = reader["VModel"] == System.DBNull.Value ? null : (string)reader["VModel"];
                        item.VColour = reader["VColour"] == System.DBNull.Value ? null : (string)reader["VColour"];
                        item.MeterStreet = reader["MeterStreet"] == System.DBNull.Value ? null : (string)reader["MeterStreet"];
                        item.Location = reader["Location"] == System.DBNull.Value ? null : (string)reader["Location"];

                        item.EnfVendorStallId = reader["EnfVendorStallId"] == System.DBNull.Value ? default(int?) : (Int32)reader["EnfVendorStallId"];
                        item.PresentMeterTime = reader["PresentMeterTime"] == System.DBNull.Value ? default(DateTime?) : (DateTime)reader["PresentMeterTime"];
                        item.SpaceStatus = reader["SpaceStatus"] == System.DBNull.Value ? null : (string)reader["SpaceStatus"];
                        item.SensorEventTime = reader["SensorEventTime"] == System.DBNull.Value ? default(DateTime?) : (DateTime)reader["SensorEventTime"];
                        //item.ExpiredMinutes = reader["ExpiredMinutes"] == System.DBNull.Value ? default(int?) : (Int32)reader["ExpiredMinutes"];
                        item.ExpiredMinutes = reader["ExpiredMinutes"] == System.DBNull.Value ? -1 : (Int32)reader["ExpiredMinutes"];
                        item.AmountIncents = reader["AmountIncents"] == System.DBNull.Value ? default(decimal?) : (decimal)reader["AmountIncents"];
                        item.ExpiryTime = reader["ExpiryTime"] == System.DBNull.Value ? default(DateTime?) : (DateTime)reader["ExpiryTime"];
                        item.TimeZoneId = reader["TimeZoneId"] == System.DBNull.Value ? default(int?) : (Int32)reader["TimeZoneId"];
                        item.PayStationID = reader["PayStationID"] == System.DBNull.Value ? default(int?) : (Int32)reader["PayStationID"];
                        item.ZoneID = reader["ZoneID"] == System.DBNull.Value ? default(int?) : (Int32)reader["ZoneID"];
                        //item.ZoneName = reader["ZoneName"] == System.DBNull.Value ? null : (string)reader["ZoneName"];
                        item.ZoneName = null;
                        item.Block = reader["Block"] == System.DBNull.Value ? null : (string)reader["Block"];
                        item.Direction = reader["Direction"] == System.DBNull.Value ? null : (string)reader["Direction"];
                        item.MeterStreet = reader["MeterStreet"] == System.DBNull.Value ? null : (string)reader["MeterStreet"];
                        item.StreetType = reader["StreetType"] == System.DBNull.Value ? null : (string)reader["StreetType"];
                        item.MeterStreet = reader["MeterStreet"] == System.DBNull.Value ? null : (string)reader["MeterStreet"];
                        item.CrossStreet1 = reader["CrossStreet1"] == System.DBNull.Value ? null : (string)reader["CrossStreet1"];

                        item.CrossStreet2 = reader["CrossStreet2"] == System.DBNull.Value ? null : (string)reader["CrossStreet2"];
                        item.DigiELType = reader["DigiELType"] == System.DBNull.Value ? default(int?) : (Int32)reader["DigiELType"];
                        item.DigiErrorDescr = reader["DigiErrorDescr"] == System.DBNull.Value ? null : (string)reader["DigiErrorDescr"];
                        item.GeneticELType = reader["GeneticELType"] == System.DBNull.Value ? default(int?) : (Int32)reader["GeneticELType"];
                        item.GeneticErrorDescr = reader["GeneticErrorDescr"] == System.DBNull.Value ? null : (string)reader["GeneticErrorDescr"];
                        item.PMELType = reader["PMELType"] == System.DBNull.Value ? default(int?) : (Int32)reader["PMELType"];
                        item.PMErrorDescr = reader["PMErrorDescr"] == System.DBNull.Value ? null : (string)reader["PMErrorDescr"];
                        item.EnfHourDesc = reader["EnfHourDesc"] == System.DBNull.Value ? null : (string)reader["EnfHourDesc"];
                        item.MarkedSince = reader["MarkedSince"] == System.DBNull.Value ? default(DateTime?) : (DateTime)reader["MarkedSince"];
                        item.HitDescription = reader["HitDescription"] == System.DBNull.Value ? null : (string)reader["HitDescription"];
                        validatedData.Add(item);
                    }
                }
            }
            catch (Exception ex)
            {
                throw;
            }
            finally
            {
                if (connection.State == ConnectionState.Open)
                    connection.Close();
            }
            return validatedData;
        }

        /// <summary>
        /// For Raleigh, Birmingham, MI
        /// </summary>
        /// <param name="customerID"></param>
        /// <param name="ZoneID"></param>
        /// <param name="MeterName"></param>
        /// <param name="ParkingSpaceID"></param>
        /// <param name="PlateNumber"></param>
        /// <param name="SpaceStatus"></param>
        /// <returns></returns>


        public static List<Transaction> GetCitationDataOthersWithPlateNameCoralGables(int customerID, string ZoneID, string MeterName, string ParkingSpaceID, string PlateNumber, string SpaceStatus)
        {
            List<Transaction> validatedData = new List<Transaction>();
            SqlConnection connection = null;
            try
            {
                connection = new SqlConnection(System.Configuration.ConfigurationManager.ConnectionStrings["Connector"].ToString());
                SqlCommand sqlCommand = new SqlCommand();
                sqlCommand.CommandType = CommandType.StoredProcedure;
                //sqlCommand.CommandText = "SpacePlateTxnFinal_railegh";
                sqlCommand.CommandText = "[SpacePlateTxnFinal_Railegh_Metername_CoralGables]";
                sqlCommand.Parameters.AddWithValue("@customerID", customerID);
                sqlCommand.Parameters.AddWithValue("@ZoneID", ZoneID);
                sqlCommand.Parameters.AddWithValue("@MeterName", MeterName);
                sqlCommand.Parameters.AddWithValue("@ParkingSpaceID", ParkingSpaceID);
                sqlCommand.Parameters.AddWithValue("@PlateNumber", PlateNumber);
                sqlCommand.Parameters.AddWithValue("@SpaceStatus", SpaceStatus);
                sqlCommand.Connection = connection;
                sqlCommand.CommandTimeout = 120;
                connection.Open();
                using (var reader = sqlCommand.ExecuteReader())
                {
                    while (reader.Read())
                    {
                        var item = new Transaction();
                        item.EnforcementKey = reader["EnforcementKey"] == System.DBNull.Value ? null : (string)reader["EnforcementKey"];
                        item.CustomerId = (reader.GetInt64(reader.GetOrdinal("CustomerId"))).ToString();
                        item.ParkingSpaceID = reader["ParkingSpaceID"] == System.DBNull.Value ? default(int) : (int)reader["ParkingSpaceID"];
                        item.PlateNumber = reader["PlateNumber"] == System.DBNull.Value ? null : (string)reader["PlateNumber"];
                        item.MeterName = reader["MeterName"] == System.DBNull.Value ? null : (string)reader["MeterName"];
                        item.MeterId = reader["meterid"] == System.DBNull.Value ? null : (string)reader["meterid"];
                        item.Latitude = reader["Latitude"] == System.DBNull.Value ? default(double?) : (double)reader["Latitude"];
                        item.Latitude = reader["Longitude"] == System.DBNull.Value ? default(double?) : (double)reader["Longitude"];

                        item.LPLocation = reader["LPLocation"] == System.DBNull.Value ? null : (string)reader["LPLocation"];
                        item.VMake = reader["VMake"] == System.DBNull.Value ? null : (string)reader["VMake"];
                        item.VModel = reader["VModel"] == System.DBNull.Value ? null : (string)reader["VModel"];
                        item.VColour = reader["VColour"] == System.DBNull.Value ? null : (string)reader["VColour"];
                        item.MeterStreet = reader["MeterStreet"] == System.DBNull.Value ? null : (string)reader["MeterStreet"];
                        item.Location = reader["Location"] == System.DBNull.Value ? null : (string)reader["Location"];

                        item.EnfVendorStallId = reader["EnfVendorStallId"] == System.DBNull.Value ? default(int?) : (Int32)reader["EnfVendorStallId"];
                        item.PresentMeterTime = reader["PresentMeterTime"] == System.DBNull.Value ? default(DateTime?) : (DateTime)reader["PresentMeterTime"];
                        item.SpaceStatus = reader["SpaceStatus"] == System.DBNull.Value ? null : (string)reader["SpaceStatus"];
                        item.SensorEventTime = reader["SensorEventTime"] == System.DBNull.Value ? default(DateTime?) : (DateTime)reader["SensorEventTime"];
                        // item.ExpiredMinutes = reader["ExpiredMinutes"] == System.DBNull.Value ? default(int?) : (Int32)reader["ExpiredMinutes"];
                        item.ExpiredMinutes = reader["ExpiredMinutes"] == System.DBNull.Value ? -1 : (Int32)reader["ExpiredMinutes"];
                        item.AmountIncents = reader["AmountIncents"] == System.DBNull.Value ? default(decimal?) : (decimal)reader["AmountIncents"];
                        item.ExpiryTime = reader["ExpiryTime"] == System.DBNull.Value ? default(DateTime?) : (DateTime)reader["ExpiryTime"];
                        item.TimeZoneId = reader["TimeZoneId"] == System.DBNull.Value ? default(int?) : (Int32)reader["TimeZoneId"];
                        //item.PayStationID = reader["PayStationID"] == System.DBNull.Value ? default(int?) : (Int32)reader["PayStationID"];
                        item.PayStationID = reader["meterid"] == System.DBNull.Value ? default(int?) : Convert.ToInt32(reader["meterid"]);
                        item.ZoneID = reader["ZoneID"] == System.DBNull.Value ? default(int?) : (Int32)reader["ZoneID"];
                        item.ZoneName = reader["ZoneName"] == System.DBNull.Value ? null : (string)reader["ZoneName"];
                        item.Block = reader["Block"] == System.DBNull.Value ? null : (string)reader["Block"];
                        item.Direction = reader["Direction"] == System.DBNull.Value ? null : (string)reader["Direction"];
                        item.MeterStreet = reader["MeterStreet"] == System.DBNull.Value ? null : (string)reader["MeterStreet"];
                        item.StreetType = reader["StreetType"] == System.DBNull.Value ? null : (string)reader["StreetType"];
                        item.MeterStreet = reader["MeterStreet"] == System.DBNull.Value ? null : (string)reader["MeterStreet"];
                        item.CrossStreet1 = reader["CrossStreet1"] == System.DBNull.Value ? null : (string)reader["CrossStreet1"];

                        item.CrossStreet2 = reader["CrossStreet2"] == System.DBNull.Value ? null : (string)reader["CrossStreet2"];
                        item.DigiELType = reader["DigiELType"] == System.DBNull.Value ? default(int?) : (Int32)reader["DigiELType"];
                        item.DigiErrorDescr = reader["DigiErrorDescr"] == System.DBNull.Value ? null : (string)reader["DigiErrorDescr"];
                        item.GeneticELType = reader["GeneticELType"] == System.DBNull.Value ? default(int?) : (Int32)reader["GeneticELType"];
                        item.GeneticErrorDescr = reader["GeneticErrorDescr"] == System.DBNull.Value ? null : (string)reader["GeneticErrorDescr"];

                        if (reader["PMELType"] == System.DBNull.Value)
                        {
                            item.PMELType = default(int?);
                        }
                        else
                        {
                            string pMELTypeValue = reader["PMELType"].ToString();
                            if (string.IsNullOrWhiteSpace(pMELTypeValue))
                            {
                                item.PMELType = default(int?);
                            }
                            else
                            {
                                item.PMELType = (Int32)reader["PMELType"];
                            }
                        }



                        item.PMErrorDescr = reader["PMErrorDescr"] == System.DBNull.Value ? null : (string)reader["PMErrorDescr"];
                        item.EnfHourDesc = reader["EnfHourDesc"] == System.DBNull.Value ? null : (string)reader["EnfHourDesc"];
                        item.MarkedSince = reader["MarkedSince"] == System.DBNull.Value ? default(DateTime?) : (DateTime)reader["MarkedSince"];
                        item.HitDescription = reader["HitDescription"] == System.DBNull.Value ? null : (string)reader["HitDescription"];


                        if (DataRecordExtensions.HasColumn(reader, "Metertype"))
                            item.METERLOC_METERTYPE = reader["Metertype"] == System.DBNull.Value ? null : (string)reader["Metertype"];
                        if (DataRecordExtensions.HasColumn(reader, "ZoneName"))
                            item.ZoneName = reader["ZoneName"] == System.DBNull.Value ? null : (string)reader["ZoneName"];

                        //if (DataRecordExtensions.HasColumn(reader, "MeterStatus"))
                        //    item.MeterStatus = reader["MeterStatus"] == System.DBNull.Value ? default(int?) : (Int32)reader["MeterStatus"];

                        validatedData.Add(item);
                    }
                }
            }
            catch (Exception ex)
            {
                throw;
            }
            finally
            {
                if (connection.State == ConnectionState.Open)
                    connection.Close();
            }
            return validatedData;
        }


        #endregion
        public static List<TimeZones> GetTimeZones(int timezoneId)
        {
            List<TimeZones> timezones = new List<TimeZones>();
            SqlConnection connection = null;
            try
            {
                connection = new SqlConnection(System.Configuration.ConfigurationManager.ConnectionStrings["Connector"].ToString());
                SqlCommand sqlCommand = new SqlCommand();
                sqlCommand.CommandType = CommandType.Text;
                sqlCommand.CommandText = string.Format("select TimeZoneID,LocalTimeUTCDifference,DaylightSavingAdjustment,UTCSummerTimeStart,UTCSummerTimeEnd,TimeZoneName from TimeZones where TimeZoneID = {0}", timezoneId);
                sqlCommand.Connection = connection;
                sqlCommand.CommandTimeout = 120;
                connection.Open();
                using (var reader = sqlCommand.ExecuteReader())
                {
                    while (reader.Read())
                    {
                        var item = new TimeZones();
                        item.TimeZoneID = reader["TimeZoneID"] == System.DBNull.Value ? 0 : (Int32)reader["TimeZoneID"];
                        item.LocalTimeUTCDifference = reader["LocalTimeUTCDifference"] == System.DBNull.Value ? 0 : (Int32)reader["LocalTimeUTCDifference"];
                        item.DaylightSavingAdjustment = reader["DaylightSavingAdjustment"] == System.DBNull.Value ? 0 : (Int32)reader["DaylightSavingAdjustment"];
                        item.UTCSummerTimeStart = reader["UTCSummerTimeStart"] == System.DBNull.Value ? DateTime.Now : (DateTime)reader["UTCSummerTimeStart"];
                        item.UTCSummerTimeEnd = reader["UTCSummerTimeEnd"] == System.DBNull.Value ? DateTime.Now : (DateTime)reader["UTCSummerTimeEnd"];
                        item.TimeZoneName = reader["TimeZoneName"] == System.DBNull.Value ? null : (string)reader["TimeZoneName"];
                        timezones.Add(item);
                    }
                }
            }
            catch (Exception ex)
            {
                var appPath = System.Web.Hosting.HostingEnvironment.ApplicationPhysicalPath;
                string errorPath = Path.Combine(appPath, "GetData", "Timezones");
                ExceptionLogging.SendErrorToText(ex, "GetCitationData", errorPath);
                throw;
            }
            finally
            {
                if (connection.State == ConnectionState.Open)
                    connection.Close();
            }
            return timezones;
        }

        public static List<ZoneIdName> GetZones(int customerId,string zoneName)
        {
            List<ZoneIdName> zones = new List<ZoneIdName>();
            SqlConnection connection = null;
            try
            {
                connection = new SqlConnection(System.Configuration.ConfigurationManager.ConnectionStrings["Connector"].ToString());
                SqlCommand sqlCommand = new SqlCommand();
                sqlCommand.CommandType = CommandType.Text;
                //sqlCommand.CommandText = string.Format("select TimeZoneID,LocalTimeUTCDifference,DaylightSavingAdjustment,UTCSummerTimeStart,UTCSummerTimeEnd,TimeZoneName from TimeZones where TimeZoneID = {0}", timezoneId);

                sqlCommand.CommandText = string.Format("SELECT TOP 1 Id,EnfCustomerId,Name FROM enfZone   WHERE EnfCustomerid = {0} AND Name = '{1}'", customerId, zoneName);
                sqlCommand.Connection = connection;
                sqlCommand.CommandTimeout = 60;
                connection.Open();
                using (var reader = sqlCommand.ExecuteReader())
                {
                    while (reader.Read())
                    {
                        var item = new ZoneIdName();
                        item.Id = reader["Id"] == System.DBNull.Value ? 0 : (Int32)reader["Id"];
                        item.Name = reader["Name"] == System.DBNull.Value ? null : (string)reader["Name"];
                        zones.Add(item);
                    }
                }
            }
            catch (Exception ex)
            {
                var appPath = System.Web.Hosting.HostingEnvironment.ApplicationPhysicalPath;
                string errorPath = Path.Combine(appPath, "GetData", "Zone");
                ExceptionLogging.SendErrorToText(ex, "GetCitationData", errorPath);
               
            }
            finally
            {
                if (connection.State == ConnectionState.Open)
                    connection.Close();
            }
            return zones;
        }
    }
}
