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
    public class PlateDataAccess
    {
        //public static List<PlateNo> GetPlateMatch(int customerID, string PlateNumber)
        //{
        //    List<PlateNo> plates = new List<PlateNo>();
        //    SqlConnection connection = null;
        //    try
        //    {
        //        connection = new SqlConnection(System.Configuration.ConfigurationManager.ConnectionStrings["Connector"].ToString());
        //        SqlCommand sqlCommand = new SqlCommand();
        //        sqlCommand.CommandType = CommandType.StoredProcedure;
        //        //sqlCommand.CommandText = "Space_Plate_status_Zone";
        //        sqlCommand.CommandText = "Space_Plate_status_Zone_v1";
        //        sqlCommand.Parameters.AddWithValue("@customerID", customerID);
        //        sqlCommand.Parameters.AddWithValue("@PlateNumber", PlateNumber);
        //        sqlCommand.Connection = connection;
        //        sqlCommand.CommandTimeout = 90;
        //        connection.Open();
        //        using (var reader = sqlCommand.ExecuteReader())
        //        {
        //            while (reader.Read())
        //            {
        //                var item = new PlateNo();
        //                item.CustomerId = reader["CustomerId"] == System.DBNull.Value ? default(int?) : (Int32)reader["CustomerId"];
        //                item.plate = reader["PlateNumber"] == System.DBNull.Value ? null : (string)reader["PlateNumber"];
        //                item.PresentMeterTime = reader["PresentMeterTime"] == System.DBNull.Value ? default(DateTime?) : (DateTime)reader["PresentMeterTime"];
        //                item.ExpiryTime = reader["ExpiryTime"] == System.DBNull.Value ? default(DateTime?) : (DateTime)reader["ExpiryTime"];
        //                item.StateName = reader["ENFState"] == System.DBNull.Value ? null : (string)reader["ENFState"];
        //                item.RateName = reader["RateNumber"] == System.DBNull.Value ? null : (string)reader["RateNumber"];
        //                item.EnfType = reader["EnfType"] == System.DBNull.Value ? null : (string)reader["EnfType"];
        //                item.SpaceStatus = reader["SpaceStatus"] == System.DBNull.Value ? null : (string)reader["SpaceStatus"];
        //                item.ZoneName = reader["Zonename"] == System.DBNull.Value ? null : (string)reader["Zonename"];
        //                if (string.IsNullOrWhiteSpace(item.StateName))
        //                {
        //                    item.plate = PlateNumber;
        //                }
        //                else
        //                {
        //                    item.plate = string.Format("{0}, [{1}]", PlateNumber, item.StateName);
        //                }

        //                if (DataRecordExtensions.HasColumn(reader, "Vendorname"))
        //                    item.VendorName = reader["Vendorname"] == System.DBNull.Value ? null : (string)reader["Vendorname"];

        //                item.Amount = reader["Amount"] == System.DBNull.Value ? default(decimal?) : (decimal)reader["Amount"];

        //                item.ZoneId = reader["Zoneid"] == System.DBNull.Value ? default(int?) : (Int32)reader["Zoneid"];

        //                item.ValidAnyZone = reader["ValidAnyZone"] == System.DBNull.Value ? default(bool?) : (bool)reader["ValidAnyZone"];
                        
        //                plates.Add(item);
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
        //    return plates;
        //}

        public static List<PlateNo> GetPlateMatch(int customerID, string PlateNumber)
        {
            //////////////////////////////////////////////////////////
            ////if (customerID == 7056 || customerID == 4135 || customerID == 4142)
            //if (customerID == 7056 || customerID == 4135)
            //    return GetPlateMatchPhili(customerID, PlateNumber);
            //else if (customerID == 4142)
            //    return GetPlateMatchGlendale(customerID, PlateNumber);
            //else
            //    return GetPlateMatchGeneric(customerID, PlateNumber);
            ///////////////////////////////////////////////////
            switch (customerID)
            {
                //case CustomerIds.Philadelphia:
                //case CustomerIds.CityofChester:
                case CustomerIds.MiamiParkingAuthority:
                case CustomerIds.CoralGables:
                case CustomerIds.SouthMiami:
                case CustomerIds.Surfside:
                case CustomerIds.BayHarbor:
                case CustomerIds.SunnyIslesBeach:
                //case CustomerIds.MetroRail:
                case CustomerIds.MiamiBeach:
                case CustomerIds.NorthBayVillage:
                case CustomerIds.Sweetwater:
                case CustomerIds.InnovationCity:
                case CustomerIds.SpokaneWA:
                    return GetPlateMatchPhili(customerID, PlateNumber);
                case CustomerIds.MetroRail:
                    return GetPlateMatchMetroRail(customerID, PlateNumber);
                case CustomerIds.Glendale:
                case CustomerIds.CityofChester:
                case CustomerIds.BoyntonBeach:
                case CustomerIds.Philadelphia:
                case CustomerIds.DammamKSA:
                case CustomerIds.DammamKSADev:
                case CustomerIds.Torrance:
                case CustomerIds.HyderabadGHMC:
                case CustomerIds.NOLA:
                case CustomerIds.Leavenworth:
                case CustomerIds.ColoradoSprings:
                case CustomerIds.MountRainier:
                case CustomerIds.PaloAltoCA:

                case CustomerIds.ChillicotheOH:
                case CustomerIds.BahrainDemo1:
                case CustomerIds.SANDIEGOAIRPORT:
                case CustomerIds.SanibelX3:
                //case CustomerIds.MuscatMunicipality:
                case CustomerIds.WestAllis:
                case CustomerIds.Doral:
                case CustomerIds.NewSmyrnaBeachFL:
                case CustomerIds.SalemOR:
                    //return GetPlateMatchGlendale(customerID, PlateNumber);
                    return GetPlateMatchGlendale51(customerID, PlateNumber);
                case CustomerIds.JacksonvilleFl:
                case CustomerIds.Tempe:
                    return GetPlateMatchIPS(customerID, PlateNumber);
                case CustomerIds.RoyalOak:
                    return GetPlateMatchRoyalOak(customerID, PlateNumber);
                case CustomerIds.MuscatMunicipality:
                    return GetPlateMatchGlendale51Muscat(customerID, PlateNumber);
                case CustomerIds.Atlanta:
                    return GetPlateMatchAtlanta(customerID, PlateNumber);
                default:
                    return GetPlateMatchGeneric(customerID, PlateNumber); 
            }
        }
        public static List<PlateNo> GetPlateMatch(int customerID, string PlateNumber, string ZoneId)
        {
            switch (customerID)
            {
                case CustomerIds.Detroit:
                    return GetPlateMatchGeneric(customerID, PlateNumber,ZoneId);
                case CustomerIds.Philadelphia:
                    return GetPlateMatchGlendale51(customerID, PlateNumber, ZoneId);
                default:
                    return GetPlateMatchGeneric(customerID, PlateNumber);
            }
        }

        private static List<PlateNo> GetPlateMatchGeneric(int customerID, string PlateNumber)
        {
            List<PlateNo> plates = new List<PlateNo>();
            SqlConnection connection = null;
            try
            {
                connection = new SqlConnection(System.Configuration.ConfigurationManager.ConnectionStrings["Connector"].ToString());
                SqlCommand sqlCommand = new SqlCommand();
                sqlCommand.CommandType = CommandType.StoredProcedure;
                //sqlCommand.CommandText = "Space_Plate_status_Zone";
                sqlCommand.CommandText = "Space_Plate_status_Zone_v1";
                sqlCommand.Parameters.AddWithValue("@customerID", customerID);
                sqlCommand.Parameters.AddWithValue("@PlateNumber", PlateNumber);
                sqlCommand.Connection = connection;
                sqlCommand.CommandTimeout = 90;
                connection.Open();
                using (var reader = sqlCommand.ExecuteReader())
                {
                    while (reader.Read())
                    {
                        var item = new PlateNo();
                        item.CustomerId = reader["CustomerId"] == System.DBNull.Value ? default(int?) : (Int32)reader["CustomerId"];
                        item.plate = reader["PlateNumber"] == System.DBNull.Value ? null : (string)reader["PlateNumber"];
                        item.PresentMeterTime = reader["PresentMeterTime"] == System.DBNull.Value ? default(DateTime?) : (DateTime)reader["PresentMeterTime"];
                        item.ExpiryTime = reader["ExpiryTime"] == System.DBNull.Value ? default(DateTime?) : (DateTime)reader["ExpiryTime"];
                        item.StateName = reader["ENFState"] == System.DBNull.Value ? null : (string)reader["ENFState"];
                        item.RateName = reader["RateNumber"] == System.DBNull.Value ? null : (string)reader["RateNumber"];
                        item.EnfType = reader["EnfType"] == System.DBNull.Value ? null : (string)reader["EnfType"];
                        item.SpaceStatus = reader["SpaceStatus"] == System.DBNull.Value ? null : (string)reader["SpaceStatus"];
                        item.ZoneName = reader["Zonename"] == System.DBNull.Value ? null : (string)reader["Zonename"];
                        if (string.IsNullOrWhiteSpace(item.StateName))
                        {
                            item.plate = PlateNumber;
                        }
                        else
                        {
                            item.plate = string.Format("{0}, [{1}]", PlateNumber, item.StateName);
                        }

                        if (DataRecordExtensions.HasColumn(reader, "Vendorname"))
                            item.VendorName = reader["Vendorname"] == System.DBNull.Value ? null : (string)reader["Vendorname"];

                        item.Amount = reader["Amount"] == System.DBNull.Value ? default(decimal?) : (decimal)reader["Amount"];

                        item.ZoneId = reader["Zoneid"] == System.DBNull.Value ? default(int?) : (Int32)reader["Zoneid"];

                        item.ValidAnyZone = reader["ValidAnyZone"] == System.DBNull.Value ? default(bool?) : (bool)reader["ValidAnyZone"];

                        plates.Add(item);
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
            return plates;
        }

      
        private static List<PlateNo> GetPlateMatchGeneric(int customerID, string PlateNumber, string ZoneId)
        {
            List<PlateNo> plates = new List<PlateNo>();
            SqlConnection connection = null;
            try
            {
                connection = new SqlConnection(System.Configuration.ConfigurationManager.ConnectionStrings["Connector"].ToString());
                SqlCommand sqlCommand = new SqlCommand();
                sqlCommand.CommandType = CommandType.StoredProcedure;
                //sqlCommand.CommandText = "Space_Plate_status_Zone";
                sqlCommand.CommandText = "[Space_Plate_status_Zone_v1_zoneid]";
                sqlCommand.Parameters.AddWithValue("@customerID", customerID);
                sqlCommand.Parameters.AddWithValue("@PlateNumber", PlateNumber);
                sqlCommand.Parameters.AddWithValue("@ZoneName", ZoneId);
                sqlCommand.Connection = connection;
                sqlCommand.CommandTimeout = 90;
                connection.Open();
                using (var reader = sqlCommand.ExecuteReader())
                {
                    while (reader.Read())
                    {
                        var item = new PlateNo();
                        item.CustomerId = reader["CustomerId"] == System.DBNull.Value ? default(int?) : (Int32)reader["CustomerId"];
                        item.plate = reader["PlateNumber"] == System.DBNull.Value ? null : (string)reader["PlateNumber"];
                        item.PresentMeterTime = reader["PresentMeterTime"] == System.DBNull.Value ? default(DateTime?) : (DateTime)reader["PresentMeterTime"];
                        item.ExpiryTime = reader["ExpiryTime"] == System.DBNull.Value ? default(DateTime?) : (DateTime)reader["ExpiryTime"];
                        item.StateName = reader["ENFState"] == System.DBNull.Value ? null : (string)reader["ENFState"];
                        item.RateName = reader["RateNumber"] == System.DBNull.Value ? null : (string)reader["RateNumber"];
                        item.EnfType = reader["EnfType"] == System.DBNull.Value ? null : (string)reader["EnfType"];
                        item.SpaceStatus = reader["SpaceStatus"] == System.DBNull.Value ? null : (string)reader["SpaceStatus"];
                        item.ZoneName = reader["Zonename"] == System.DBNull.Value ? null : (string)reader["Zonename"];
                        if (string.IsNullOrWhiteSpace(item.StateName))
                        {
                            item.plate = PlateNumber;
                        }
                        else
                        {
                            item.plate = string.Format("{0}, [{1}]", PlateNumber, item.StateName);
                        }

                        if (DataRecordExtensions.HasColumn(reader, "Vendorname"))
                            item.VendorName = reader["Vendorname"] == System.DBNull.Value ? null : (string)reader["Vendorname"];

                        item.Amount = reader["Amount"] == System.DBNull.Value ? default(decimal?) : (decimal)reader["Amount"];

                        item.ZoneId = reader["Zoneid"] == System.DBNull.Value ? default(int?) : (Int32)reader["Zoneid"];

                        item.ValidAnyZone = reader["ValidAnyZone"] == System.DBNull.Value ? default(bool?) : (bool)reader["ValidAnyZone"];

                        plates.Add(item);
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
            return plates;
        }

        private static List<PlateNo> GetPlateMatchPhili(int customerID, string PlateNumber)
        {
            List<PlateNo> plates = new List<PlateNo>();
            SqlConnection connection = null;
            try
            {
                connection = new SqlConnection(System.Configuration.ConfigurationManager.ConnectionStrings["Connector"].ToString());
                SqlCommand sqlCommand = new SqlCommand();
                sqlCommand.CommandType = CommandType.StoredProcedure;
                //sqlCommand.CommandText = "Space_Plate_status_Zone";
                sqlCommand.CommandText = "Space_Plate_status_Zone_v3";
                sqlCommand.Parameters.AddWithValue("@customerID", customerID);
                sqlCommand.Parameters.AddWithValue("@PlateNumber", PlateNumber);
                sqlCommand.Connection = connection;
                sqlCommand.CommandTimeout = 90;
                connection.Open();
                using (var reader = sqlCommand.ExecuteReader())
                {
                    while (reader.Read())
                    {
                        var item = new PlateNo();
                        item.CustomerId = reader["CustomerId"] == System.DBNull.Value ? default(int?) : (Int32)reader["CustomerId"];
                        item.plate = reader["PlateNumber"] == System.DBNull.Value ? null : (string)reader["PlateNumber"];
                        item.PresentMeterTime = reader["PresentMeterTime"] == System.DBNull.Value ? default(DateTime?) : (DateTime)reader["PresentMeterTime"];
                        item.ExpiryTime = reader["ExpiryTime"] == System.DBNull.Value ? default(DateTime?) : (DateTime)reader["ExpiryTime"];
                        item.StateName = reader["ENFState"] == System.DBNull.Value ? null : (string)reader["ENFState"];
                        item.RateName = reader["RateNumber"] == System.DBNull.Value ? null : (string)reader["RateNumber"];
                        item.EnfType = reader["EnfType"] == System.DBNull.Value ? null : (string)reader["EnfType"];
                        item.SpaceStatus = reader["SpaceStatus"] == System.DBNull.Value ? null : (string)reader["SpaceStatus"];
                        item.ZoneName = reader["Zonename"] == System.DBNull.Value ? null : (string)reader["Zonename"];
                        if (string.IsNullOrWhiteSpace(item.StateName))
                        {
                            item.plate = PlateNumber;
                        }
                        else
                        {
                            item.plate = string.Format("{0}, [{1}]", PlateNumber, item.StateName);
                        }

                        if (DataRecordExtensions.HasColumn(reader, "Vendorname"))
                            item.VendorName = reader["Vendorname"] == System.DBNull.Value ? null : (string)reader["Vendorname"];

                        item.Amount = reader["Amount"] == System.DBNull.Value ? default(decimal?) : (decimal)reader["Amount"];

                        item.ZoneId = reader["Zoneid"] == System.DBNull.Value ? default(int?) : (Int32)reader["Zoneid"];

                        item.ValidAnyZone = reader["ValidAnyZone"] == System.DBNull.Value ? default(bool?) : (bool)reader["ValidAnyZone"];

                        if (DataRecordExtensions.HasColumn(reader, "AC1"))
                            item.assignedClass = reader["AC1"] == System.DBNull.Value ? null : (string)reader["AC1"];

                        if (DataRecordExtensions.HasColumn(reader, "AC2"))
                            item.assignedClass2 = reader["AC2"] == System.DBNull.Value ? null : (string)reader["AC2"];

                        if (DataRecordExtensions.HasColumn(reader, "AC3"))
                            item.assignedClass3 = reader["AC3"] == System.DBNull.Value ? null : (string)reader["AC3"];

                        if (DataRecordExtensions.HasColumn(reader, "AC4"))
                            item.assignedClass4 = reader["AC4"] == System.DBNull.Value ? null : (string)reader["AC4"];


                        if (!string.IsNullOrEmpty(item.assignedClass4))
                            item.assignedClass3 = item.assignedClass4;

                        if (DataRecordExtensions.HasColumn(reader, "LE"))
                            item.VendorLpr = reader["LE"] == System.DBNull.Value ? null : (string)reader["LE"];

                        if (DataRecordExtensions.HasColumn(reader, "ENFPermitNo"))
                            item.ENFPermitNo = reader["ENFPermitNo"] == System.DBNull.Value ? null : (string)reader["ENFPermitNo"];
                        

                        plates.Add(item);
                    }

                    if(reader.NextResult())
                    {
                        List<Permit> permits = new List<Permit>();
                        while (reader.Read())
                        {
                            var item = new Permit();
                            item.permitNo = reader["permitNo"] == System.DBNull.Value ? null : (string)reader["permitNo"];
                            item.permitType = reader["permitType"] == System.DBNull.Value ? null : (string)reader["permitType"];
                            item.permitSource = reader["permitSource"] == System.DBNull.Value ? null : (string)reader["permitSource"];
                            item.permitZoneId = reader["permitZoneId"] == System.DBNull.Value ? null : (string)reader["permitZoneId"];
                            item.permitZoneName = reader["permitZoneName"] == System.DBNull.Value ? null : (string)reader["permitZoneName"];
                            item.PermitTxnReferenceId = reader["PermitTxnReferenceId"] == System.DBNull.Value ? null : (string)reader["PermitTxnReferenceId"];

                            item.StartDateTime = reader["permitStartDateTime"] == System.DBNull.Value ? default(DateTime?) : (DateTime)reader["permitStartDateTime"];
                            item.permitStartDateTime = DateTimeJson.ToString(item.StartDateTime);

                            item.EndDateTime = reader["permitEndDateTime"] == System.DBNull.Value ? default(DateTime?) : (DateTime)reader["permitEndDateTime"];
                            item.permitEndDateTime = DateTimeJson.ToString(item.EndDateTime);

                            item.TxnDateTime = reader["PermitTxnDateTime"] == System.DBNull.Value ? default(DateTime?) : (DateTime)reader["PermitTxnDateTime"];
                            item.PermitTxnDateTime = DateTimeJson.ToString(item.TxnDateTime);

                            permits.Add(item);

                        }

                        if (plates != null)
                            if (plates.Count > 0)
                                if (plates[0] != null)
                                    plates[0].Permits = permits;
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
            return plates;
        }


        private static List<PlateNo> GetPlateMatchMetroRail(int customerID, string PlateNumber)
        {
            List<PlateNo> plates = new List<PlateNo>();
            SqlConnection connection = null;
            try
            {
                connection = new SqlConnection(System.Configuration.ConfigurationManager.ConnectionStrings["Connector"].ToString());
                SqlCommand sqlCommand = new SqlCommand();
                sqlCommand.CommandType = CommandType.StoredProcedure;
                //sqlCommand.CommandText = "Space_Plate_status_Zone_v3";
                sqlCommand.CommandText = "Space_Plate_status_Zone_v3_MetroRail";
                sqlCommand.Parameters.AddWithValue("@customerID", customerID);
                sqlCommand.Parameters.AddWithValue("@PlateNumber", PlateNumber);
                sqlCommand.Connection = connection;
                sqlCommand.CommandTimeout = 90;
                connection.Open();
                using (var reader = sqlCommand.ExecuteReader())
                {
                    while (reader.Read())
                    {
                        var item = new PlateNo();
                        item.CustomerId = reader["CustomerId"] == System.DBNull.Value ? default(int?) : (Int32)reader["CustomerId"];
                        item.plate = reader["PlateNumber"] == System.DBNull.Value ? null : (string)reader["PlateNumber"];
                        item.PresentMeterTime = reader["PresentMeterTime"] == System.DBNull.Value ? default(DateTime?) : (DateTime)reader["PresentMeterTime"];
                        item.ExpiryTime = reader["ExpiryTime"] == System.DBNull.Value ? default(DateTime?) : (DateTime)reader["ExpiryTime"];
                        item.StateName = reader["ENFState"] == System.DBNull.Value ? null : (string)reader["ENFState"];
                        item.RateName = reader["RateNumber"] == System.DBNull.Value ? null : (string)reader["RateNumber"];
                        item.EnfType = reader["EnfType"] == System.DBNull.Value ? null : (string)reader["EnfType"];
                        item.SpaceStatus = reader["SpaceStatus"] == System.DBNull.Value ? null : (string)reader["SpaceStatus"];
                        item.ZoneName = reader["Zonename"] == System.DBNull.Value ? null : (string)reader["Zonename"];
                        if (string.IsNullOrWhiteSpace(item.StateName))
                        {
                            item.plate = PlateNumber;
                        }
                        else
                        {
                            item.plate = string.Format("{0}, [{1}]", PlateNumber, item.StateName);
                        }

                        if (DataRecordExtensions.HasColumn(reader, "Vendorname"))
                            item.VendorName = reader["Vendorname"] == System.DBNull.Value ? null : (string)reader["Vendorname"];

                        item.Amount = reader["Amount"] == System.DBNull.Value ? default(decimal?) : (decimal)reader["Amount"];

                        item.ZoneId = reader["Zoneid"] == System.DBNull.Value ? default(int?) : (Int32)reader["Zoneid"];

                        item.ValidAnyZone = reader["ValidAnyZone"] == System.DBNull.Value ? default(bool?) : (bool)reader["ValidAnyZone"];

                        if (DataRecordExtensions.HasColumn(reader, "AC1"))
                            item.assignedClass = reader["AC1"] == System.DBNull.Value ? null : (string)reader["AC1"];

                        if (DataRecordExtensions.HasColumn(reader, "AC2"))
                            item.assignedClass2 = reader["AC2"] == System.DBNull.Value ? null : (string)reader["AC2"];

                        if (DataRecordExtensions.HasColumn(reader, "AC3"))
                            item.assignedClass3 = reader["AC3"] == System.DBNull.Value ? null : (string)reader["AC3"];

                        if (DataRecordExtensions.HasColumn(reader, "AC4"))
                            item.assignedClass4 = reader["AC4"] == System.DBNull.Value ? null : (string)reader["AC4"];


                        if (!string.IsNullOrEmpty(item.assignedClass4))
                            item.assignedClass3 = item.assignedClass4;

                        if (DataRecordExtensions.HasColumn(reader, "LE"))
                            item.VendorLpr = reader["LE"] == System.DBNull.Value ? null : (string)reader["LE"];

                        if (DataRecordExtensions.HasColumn(reader, "ENFPermitNo"))
                            item.ENFPermitNo = reader["ENFPermitNo"] == System.DBNull.Value ? null : (string)reader["ENFPermitNo"];


                        plates.Add(item);
                    }

                    if (reader.NextResult())
                    {
                        List<Permit> permits = new List<Permit>();
                        while (reader.Read())
                        {
                            var item = new Permit();
                            item.permitNo = reader["permitNo"] == System.DBNull.Value ? null : (string)reader["permitNo"];
                            item.permitType = reader["permitType"] == System.DBNull.Value ? null : (string)reader["permitType"];
                            item.permitSource = reader["permitSource"] == System.DBNull.Value ? null : (string)reader["permitSource"];
                            item.permitZoneId = reader["permitZoneId"] == System.DBNull.Value ? null : (string)reader["permitZoneId"];
                            item.permitZoneName = reader["permitZoneName"] == System.DBNull.Value ? null : (string)reader["permitZoneName"];
                            item.PermitTxnReferenceId = reader["PermitTxnReferenceId"] == System.DBNull.Value ? null : (string)reader["PermitTxnReferenceId"];

                            item.StartDateTime = reader["permitStartDateTime"] == System.DBNull.Value ? default(DateTime?) : (DateTime)reader["permitStartDateTime"];
                            item.permitStartDateTime = DateTimeJson.ToString(item.StartDateTime);

                            item.EndDateTime = reader["permitEndDateTime"] == System.DBNull.Value ? default(DateTime?) : (DateTime)reader["permitEndDateTime"];
                            item.permitEndDateTime = DateTimeJson.ToString(item.EndDateTime);

                            item.TxnDateTime = reader["PermitTxnDateTime"] == System.DBNull.Value ? default(DateTime?) : (DateTime)reader["PermitTxnDateTime"];
                            item.PermitTxnDateTime = DateTimeJson.ToString(item.TxnDateTime);

                            permits.Add(item);

                        }

                        if (plates != null)
                            if (plates.Count > 0)
                                if (plates[0] != null)
                                    plates[0].Permits = permits;
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
            return plates;
        }

        private static List<PlateNo> GetPlateMatchGlendale(int customerID, string PlateNumber)
        {
            List<PlateNo> plates = new List<PlateNo>();
            SqlConnection connection = null;
            try
            {
                connection = new SqlConnection(System.Configuration.ConfigurationManager.ConnectionStrings["Connector"].ToString());
                SqlCommand sqlCommand = new SqlCommand();
                sqlCommand.CommandType = CommandType.StoredProcedure;
                sqlCommand.CommandText = "Space_Plate_status_Zone_v5";
                sqlCommand.Parameters.AddWithValue("@customerID", customerID);
                sqlCommand.Parameters.AddWithValue("@PlateNumber", PlateNumber);
                sqlCommand.Connection = connection;
                sqlCommand.CommandTimeout = 90;
                connection.Open();
                using (var reader = sqlCommand.ExecuteReader())
                {
                    while (reader.Read())
                    {
                        var item = new PlateNo();
                        item.CustomerId = reader["CustomerId"] == System.DBNull.Value ? default(int?) : (Int32)reader["CustomerId"];
                        item.plate = reader["PlateNumber"] == System.DBNull.Value ? null : (string)reader["PlateNumber"];
                        item.PresentMeterTime = reader["PresentMeterTime"] == System.DBNull.Value ? default(DateTime?) : (DateTime)reader["PresentMeterTime"];
                        item.ExpiryTime = reader["ExpiryTime"] == System.DBNull.Value ? default(DateTime?) : (DateTime)reader["ExpiryTime"];
                        item.StateName = reader["ENFState"] == System.DBNull.Value ? null : (string)reader["ENFState"];
                        item.RateName = reader["RateNumber"] == System.DBNull.Value ? null : (string)reader["RateNumber"];
                        item.EnfType = reader["EnfType"] == System.DBNull.Value ? null : (string)reader["EnfType"];
                        item.SpaceStatus = reader["SpaceStatus"] == System.DBNull.Value ? null : (string)reader["SpaceStatus"];
                        item.ZoneName = reader["Zonename"] == System.DBNull.Value ? null : (string)reader["Zonename"];
                        if (string.IsNullOrWhiteSpace(item.StateName))
                        {
                            item.plate = PlateNumber;
                        }
                        else
                        {
                            item.plate = string.Format("{0}, [{1}]", PlateNumber, item.StateName);
                        }

                        if (DataRecordExtensions.HasColumn(reader, "Vendorname"))
                            item.VendorName = reader["Vendorname"] == System.DBNull.Value ? null : (string)reader["Vendorname"];

                        item.Amount = reader["Amount"] == System.DBNull.Value ? default(decimal?) : (decimal)reader["Amount"];

                        item.ZoneId = reader["Zoneid"] == System.DBNull.Value ? default(int?) : (Int32)reader["Zoneid"];

                        item.ValidAnyZone = reader["ValidAnyZone"] == System.DBNull.Value ? default(bool?) : (bool)reader["ValidAnyZone"];

                        if (DataRecordExtensions.HasColumn(reader, "AC1"))
                            item.assignedClass = reader["AC1"] == System.DBNull.Value ? null : (string)reader["AC1"];

                        if (DataRecordExtensions.HasColumn(reader, "AC2"))
                            item.assignedClass2 = reader["AC2"] == System.DBNull.Value ? null : (string)reader["AC2"];

                        if (DataRecordExtensions.HasColumn(reader, "AC3"))
                            item.assignedClass3 = reader["AC3"] == System.DBNull.Value ? null : (string)reader["AC3"];

                        if (DataRecordExtensions.HasColumn(reader, "LE"))
                            item.VendorLpr = reader["LE"] == System.DBNull.Value ? null : (string)reader["LE"];

                        if (DataRecordExtensions.HasColumn(reader, "ENFPermitNo"))
                            item.ENFPermitNo = reader["ENFPermitNo"] == System.DBNull.Value ? null : (string)reader["ENFPermitNo"];

                        if (DataRecordExtensions.HasColumn(reader, "LE"))
                            item.LE = reader["LE"] == System.DBNull.Value ? null : (string)reader["LE"];

                        if (DataRecordExtensions.HasColumn(reader, "DeltaTime"))
                            item.DeltaTime = reader["DeltaTime"] == System.DBNull.Value ? default(DateTime?) : (DateTime)reader["DeltaTime"];

                        plates.Add(item);
                    }
               
                    if (reader.NextResult())
                    {
                        List<Permit> permits = new List<Permit>();
                        while (reader.Read())
                        {
                            var item = new Permit();
                            item.permitNo = reader["permitNo"] == System.DBNull.Value ? null : (string)reader["permitNo"];
                            item.permitType = reader["permitType"] == System.DBNull.Value ? null : (string)reader["permitType"];
                            item.permitSource = reader["permitSource"] == System.DBNull.Value ? null : (string)reader["permitSource"];
                            item.permitZoneId = reader["permitZoneId"] == System.DBNull.Value ? null : (string)reader["permitZoneId"];
                            item.permitZoneName = reader["permitZoneName"] == System.DBNull.Value ? null : (string)reader["permitZoneName"];
                            item.PermitTxnReferenceId = reader["PermitTxnReferenceId"] == System.DBNull.Value ? null : (string)reader["PermitTxnReferenceId"];

                            item.StartDateTime = reader["permitStartDateTime"] == System.DBNull.Value ? default(DateTime?) : (DateTime)reader["permitStartDateTime"];
                            item.permitStartDateTime = DateTimeJson.ToString(item.StartDateTime);

                            item.EndDateTime = reader["permitEndDateTime"] == System.DBNull.Value ? default(DateTime?) : (DateTime)reader["permitEndDateTime"];
                            item.permitEndDateTime = DateTimeJson.ToString(item.EndDateTime);

                            item.TxnDateTime = reader["PermitTxnDateTime"] == System.DBNull.Value ? default(DateTime?) : (DateTime)reader["PermitTxnDateTime"];
                            item.PermitTxnDateTime = DateTimeJson.ToString(item.TxnDateTime);

                            permits.Add(item);

                        }

                        if (plates != null)
                            if (plates.Count > 0)
                                if (plates[0] != null)
                                    plates[0].Permits = permits;
                       
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
            return plates;
        }

        private static List<PlateNo> GetPlateMatchGlendale51(int customerID, string PlateNumber)
        {
            List<PlateNo> plates = new List<PlateNo>();
            SqlConnection connection = null;
            try
            {
                connection = new SqlConnection(System.Configuration.ConfigurationManager.ConnectionStrings["Connector"].ToString());
                SqlCommand sqlCommand = new SqlCommand();
                sqlCommand.CommandType = CommandType.StoredProcedure;
                //sqlCommand.CommandText = "Space_Plate_status_Zone_v5_1"; //
                sqlCommand.CommandText = "Space_Plate_status_Zone_v5_1_1"; //Added for Restircting 16 hours to Genetec plate.
                //sqlCommand.CommandText = "Space_Plate_status_Zone_v5_1_1_TESTTT";  //to remove
                sqlCommand.Parameters.AddWithValue("@customerID", customerID);
                sqlCommand.Parameters.AddWithValue("@PlateNumber", PlateNumber);
                sqlCommand.Connection = connection;
                sqlCommand.CommandTimeout = 90;
                connection.Open();
                using (var reader = sqlCommand.ExecuteReader())
                {
                    while (reader.Read())
                    {
                        var item = new PlateNo();
                        item.CustomerId = reader["CustomerId"] == System.DBNull.Value ? default(int?) : (Int32)reader["CustomerId"];
                        item.plate = reader["PlateNumber"] == System.DBNull.Value ? null : (string)reader["PlateNumber"];
                        item.PresentMeterTime = reader["PresentMeterTime"] == System.DBNull.Value ? default(DateTime?) : (DateTime)reader["PresentMeterTime"];
                        item.ExpiryTime = reader["ExpiryTime"] == System.DBNull.Value ? default(DateTime?) : (DateTime)reader["ExpiryTime"];
                        item.StateName = reader["ENFState"] == System.DBNull.Value ? null : (string)reader["ENFState"];
                        item.RateName = reader["RateNumber"] == System.DBNull.Value ? null : (string)reader["RateNumber"];
                        item.EnfType = reader["EnfType"] == System.DBNull.Value ? null : (string)reader["EnfType"];
                        item.SpaceStatus = reader["SpaceStatus"] == System.DBNull.Value ? null : (string)reader["SpaceStatus"];
                        item.ZoneName = reader["Zonename"] == System.DBNull.Value ? null : (string)reader["Zonename"];
                        if (string.IsNullOrWhiteSpace(item.StateName))
                        {
                            item.plate = PlateNumber;
                        }
                        else
                        {
                            item.plate = string.Format("{0}, [{1}]", PlateNumber, item.StateName);
                        }

                        if (DataRecordExtensions.HasColumn(reader, "Vendorname"))
                            item.VendorName = reader["Vendorname"] == System.DBNull.Value ? null : (string)reader["Vendorname"];

                        item.Amount = reader["Amount"] == System.DBNull.Value ? default(decimal?) : (decimal)reader["Amount"];

                        item.ZoneId = reader["Zoneid"] == System.DBNull.Value ? default(int?) : (Int32)reader["Zoneid"];

                        item.ValidAnyZone = reader["ValidAnyZone"] == System.DBNull.Value ? default(bool?) : (bool)reader["ValidAnyZone"];

                        if (DataRecordExtensions.HasColumn(reader, "AC1"))
                            item.assignedClass = reader["AC1"] == System.DBNull.Value ? null : (string)reader["AC1"];

                        if (DataRecordExtensions.HasColumn(reader, "AC2"))
                            item.assignedClass2 = reader["AC2"] == System.DBNull.Value ? null : (string)reader["AC2"];

                        if (DataRecordExtensions.HasColumn(reader, "AC3"))
                            item.assignedClass3 = reader["AC3"] == System.DBNull.Value ? null : (string)reader["AC3"];

                        if (DataRecordExtensions.HasColumn(reader, "AC4"))
                            item.assignedClass4 = reader["AC4"] == System.DBNull.Value ? null : (string)reader["AC4"];

                        if (!string.IsNullOrEmpty(item.assignedClass4))
                            item.assignedClass3 = item.assignedClass4;

                        if (DataRecordExtensions.HasColumn(reader, "LE"))
                            item.VendorLpr = reader["LE"] == System.DBNull.Value ? null : (string)reader["LE"];

                        if (DataRecordExtensions.HasColumn(reader, "ENFPermitNo"))
                            item.ENFPermitNo = reader["ENFPermitNo"] == System.DBNull.Value ? null : (string)reader["ENFPermitNo"];

                        if (DataRecordExtensions.HasColumn(reader, "LE"))
                            item.LE = reader["LE"] == System.DBNull.Value ? null : (string)reader["LE"];

                        if (DataRecordExtensions.HasColumn(reader, "DeltaTime"))
                            item.DeltaTime = reader["DeltaTime"] == System.DBNull.Value ? default(DateTime?) : (DateTime)reader["DeltaTime"];


                        if (DataRecordExtensions.HasColumn(reader, "VendorID"))
                            item.VendorID = reader["VendorID"] == System.DBNull.Value ? default(long?) : (Int64)reader["VendorID"];
                        if (DataRecordExtensions.HasColumn(reader, "ImageRawData"))
                            item.ImageRawData = reader["ImageRawData"] == System.DBNull.Value ? null : (string)reader["ImageRawData"];
                        if (DataRecordExtensions.HasColumn(reader, "EnfVendorLPRId"))
                            item.EnfVendorLprID = reader["EnfVendorLPRId"] == System.DBNull.Value ? default(long?) : (Int64)reader["EnfVendorLPRId"];
                        if (DataRecordExtensions.HasColumn(reader, "ObservedTime"))
                            item.ObservedTime = reader["ObservedTime"] == System.DBNull.Value ? default(DateTime?) : (DateTime)reader["ObservedTime"];

                        plates.Add(item);
                    }

                    if (reader.NextResult())
                    {
                        List<Permit> permits = new List<Permit>();
                        while (reader.Read())
                        {
                            var item = new Permit();
                            item.permitNo = reader["permitNo"] == System.DBNull.Value ? null : (string)reader["permitNo"];
                            item.permitType = reader["permitType"] == System.DBNull.Value ? null : (string)reader["permitType"];
                            item.permitSource = reader["permitSource"] == System.DBNull.Value ? null : (string)reader["permitSource"];
                            item.permitZoneId = reader["permitZoneId"] == System.DBNull.Value ? null : (string)reader["permitZoneId"];
                            item.permitZoneName = reader["permitZoneName"] == System.DBNull.Value ? null : (string)reader["permitZoneName"];
                            item.PermitTxnReferenceId = reader["PermitTxnReferenceId"] == System.DBNull.Value ? null : (string)reader["PermitTxnReferenceId"];

                            item.StartDateTime = reader["permitStartDateTime"] == System.DBNull.Value ? default(DateTime?) : (DateTime)reader["permitStartDateTime"];
                            item.permitStartDateTime = DateTimeJson.ToString(item.StartDateTime);

                            item.EndDateTime = reader["permitEndDateTime"] == System.DBNull.Value ? default(DateTime?) : (DateTime)reader["permitEndDateTime"];
                            item.permitEndDateTime = DateTimeJson.ToString(item.EndDateTime);

                            item.TxnDateTime = reader["PermitTxnDateTime"] == System.DBNull.Value ? default(DateTime?) : (DateTime)reader["PermitTxnDateTime"];
                            item.PermitTxnDateTime = DateTimeJson.ToString(item.TxnDateTime);

                            permits.Add(item);

                        }

                        if (plates != null)
                            if (plates.Count > 0)
                                if (plates[0] != null)
                                    plates[0].Permits = permits;

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
            return plates;
        }
        private static List<PlateNo> GetPlateMatchIPS(int customerID, string PlateNumber)
        {
            List<PlateNo> plates = new List<PlateNo>();
            SqlConnection connection = null;
            try
            {
                connection = new SqlConnection(System.Configuration.ConfigurationManager.ConnectionStrings["Connector"].ToString());
                SqlCommand sqlCommand = new SqlCommand();
                sqlCommand.CommandType = CommandType.StoredProcedure;
                //sqlCommand.CommandText = "Space_Plate_status_Zone_v5_1"; //
                sqlCommand.CommandText = "Space_Plate_status_Zone_v5_1_1_IPS"; //Added for Restircting 16 hours to Genetec plate.
                //sqlCommand.CommandText = "Space_Plate_status_Zone_v5_1_1_TESTTT";  //to remove
                sqlCommand.Parameters.AddWithValue("@customerID", customerID);
                sqlCommand.Parameters.AddWithValue("@PlateNumber", PlateNumber);
                sqlCommand.Connection = connection;
                sqlCommand.CommandTimeout = 90;
                connection.Open();
                using (var reader = sqlCommand.ExecuteReader())
                {
                    while (reader.Read())
                    {
                        var item = new PlateNo();
                        item.CustomerId = reader["CustomerId"] == System.DBNull.Value ? default(int?) : (Int32)reader["CustomerId"];
                        item.plate = reader["PlateNumber"] == System.DBNull.Value ? null : (string)reader["PlateNumber"];
                        item.PresentMeterTime = reader["PresentMeterTime"] == System.DBNull.Value ? default(DateTime?) : (DateTime)reader["PresentMeterTime"];
                        item.ExpiryTime = reader["ExpiryTime"] == System.DBNull.Value ? default(DateTime?) : (DateTime)reader["ExpiryTime"];
                        item.StateName = reader["ENFState"] == System.DBNull.Value ? null : (string)reader["ENFState"];
                        item.RateName = reader["RateNumber"] == System.DBNull.Value ? null : (string)reader["RateNumber"];
                        item.EnfType = reader["EnfType"] == System.DBNull.Value ? null : (string)reader["EnfType"];
                        item.SpaceStatus = reader["SpaceStatus"] == System.DBNull.Value ? null : (string)reader["SpaceStatus"];
                        item.ZoneName = reader["Zonename"] == System.DBNull.Value ? null : (string)reader["Zonename"];
                        if (string.IsNullOrWhiteSpace(item.StateName))
                        {
                            item.plate = PlateNumber;
                        }
                        else
                        {
                            item.plate = string.Format("{0}, [{1}]", PlateNumber, item.StateName);
                        }

                        if (DataRecordExtensions.HasColumn(reader, "Vendorname"))
                            item.VendorName = reader["Vendorname"] == System.DBNull.Value ? null : (string)reader["Vendorname"];

                        item.Amount = reader["Amount"] == System.DBNull.Value ? default(decimal?) : (decimal)reader["Amount"];

                        item.ZoneId = reader["Zoneid"] == System.DBNull.Value ? default(int?) : (Int32)reader["Zoneid"];

                        item.ValidAnyZone = reader["ValidAnyZone"] == System.DBNull.Value ? default(bool?) : (bool)reader["ValidAnyZone"];

                        if (DataRecordExtensions.HasColumn(reader, "AC1"))
                            item.assignedClass = reader["AC1"] == System.DBNull.Value ? null : (string)reader["AC1"];

                        if (DataRecordExtensions.HasColumn(reader, "AC2"))
                            item.assignedClass2 = reader["AC2"] == System.DBNull.Value ? null : (string)reader["AC2"];

                        if (DataRecordExtensions.HasColumn(reader, "AC3"))
                            item.assignedClass3 = reader["AC3"] == System.DBNull.Value ? null : (string)reader["AC3"];

                        if (DataRecordExtensions.HasColumn(reader, "AC4"))
                            item.assignedClass4 = reader["AC4"] == System.DBNull.Value ? null : (string)reader["AC4"];

                        if (!string.IsNullOrEmpty(item.assignedClass4))
                            item.assignedClass3 = item.assignedClass4;

                        if (DataRecordExtensions.HasColumn(reader, "LE"))
                            item.VendorLpr = reader["LE"] == System.DBNull.Value ? null : (string)reader["LE"];

                        if (DataRecordExtensions.HasColumn(reader, "ENFPermitNo"))
                            item.ENFPermitNo = reader["ENFPermitNo"] == System.DBNull.Value ? null : (string)reader["ENFPermitNo"];

                        if (DataRecordExtensions.HasColumn(reader, "LE"))
                            item.LE = reader["LE"] == System.DBNull.Value ? null : (string)reader["LE"];

                        if (DataRecordExtensions.HasColumn(reader, "DeltaTime"))
                            item.DeltaTime = reader["DeltaTime"] == System.DBNull.Value ? default(DateTime?) : (DateTime)reader["DeltaTime"];


                        if (DataRecordExtensions.HasColumn(reader, "VendorID"))
                            item.VendorID = reader["VendorID"] == System.DBNull.Value ? default(long?) : (Int64)reader["VendorID"];
                        if (DataRecordExtensions.HasColumn(reader, "ImageRawData"))
                            item.ImageRawData = reader["ImageRawData"] == System.DBNull.Value ? null : (string)reader["ImageRawData"];
                        if (DataRecordExtensions.HasColumn(reader, "EnfVendorLPRId"))
                            item.EnfVendorLprID = reader["EnfVendorLPRId"] == System.DBNull.Value ? default(long?) : (Int64)reader["EnfVendorLPRId"];
                        if (DataRecordExtensions.HasColumn(reader, "ObservedTime"))
                            item.ObservedTime = reader["ObservedTime"] == System.DBNull.Value ? default(DateTime?) : (DateTime)reader["ObservedTime"];

                        plates.Add(item);
                    }

                    if (reader.NextResult())
                    {
                        List<Permit> permits = new List<Permit>();
                        while (reader.Read())
                        {
                            var item = new Permit();
                            item.permitNo = reader["permitNo"] == System.DBNull.Value ? null : (string)reader["permitNo"];
                            item.permitType = reader["permitType"] == System.DBNull.Value ? null : (string)reader["permitType"];
                            item.permitSource = reader["permitSource"] == System.DBNull.Value ? null : (string)reader["permitSource"];
                            item.permitZoneId = reader["permitZoneId"] == System.DBNull.Value ? null : (string)reader["permitZoneId"];
                            item.permitZoneName = reader["permitZoneName"] == System.DBNull.Value ? null : (string)reader["permitZoneName"];
                            item.PermitTxnReferenceId = reader["PermitTxnReferenceId"] == System.DBNull.Value ? null : (string)reader["PermitTxnReferenceId"];

                            item.StartDateTime = reader["permitStartDateTime"] == System.DBNull.Value ? default(DateTime?) : (DateTime)reader["permitStartDateTime"];
                            item.permitStartDateTime = DateTimeJson.ToString(item.StartDateTime);

                            item.EndDateTime = reader["permitEndDateTime"] == System.DBNull.Value ? default(DateTime?) : (DateTime)reader["permitEndDateTime"];
                            item.permitEndDateTime = DateTimeJson.ToString(item.EndDateTime);

                            item.TxnDateTime = reader["PermitTxnDateTime"] == System.DBNull.Value ? default(DateTime?) : (DateTime)reader["PermitTxnDateTime"];
                            item.PermitTxnDateTime = DateTimeJson.ToString(item.TxnDateTime);

                            permits.Add(item);

                        }

                        if (plates != null)
                            if (plates.Count > 0)
                                if (plates[0] != null)
                                    plates[0].Permits = permits;

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
            return plates;
        }
        private static List<PlateNo> GetPlateMatchRoyalOak(int customerID, string PlateNumber)
        {
            List<PlateNo> plates = new List<PlateNo>();
            SqlConnection connection = null;
            try
            {
                connection = new SqlConnection(System.Configuration.ConfigurationManager.ConnectionStrings["Connector"].ToString());
                SqlCommand sqlCommand = new SqlCommand();
                sqlCommand.CommandType = CommandType.StoredProcedure;
                //sqlCommand.CommandText = "Space_Plate_status_Zone_v5_1"; //
                sqlCommand.CommandText = "Space_Plate_status_Zone_RO"; //Added for Restircting 16 hours to Genetec plate.
                //sqlCommand.CommandText = "Space_Plate_status_Zone_v5_1_1_TESTTT";  //to remove
                sqlCommand.Parameters.AddWithValue("@customerID", customerID);
                sqlCommand.Parameters.AddWithValue("@PlateNumber", PlateNumber);
                sqlCommand.Connection = connection;
                sqlCommand.CommandTimeout = 90;
                connection.Open();
                using (var reader = sqlCommand.ExecuteReader())
                {
                    while (reader.Read())
                    {
                        var item = new PlateNo();
                        item.CustomerId = reader["CustomerId"] == System.DBNull.Value ? default(int?) : (Int32)reader["CustomerId"];
                        item.plate = reader["PlateNumber"] == System.DBNull.Value ? null : (string)reader["PlateNumber"];
                        item.PresentMeterTime = reader["PresentMeterTime"] == System.DBNull.Value ? default(DateTime?) : (DateTime)reader["PresentMeterTime"];
                        item.ExpiryTime = reader["ExpiryTime"] == System.DBNull.Value ? default(DateTime?) : (DateTime)reader["ExpiryTime"];
                        item.StateName = reader["ENFState"] == System.DBNull.Value ? null : (string)reader["ENFState"];
                        item.RateName = reader["RateNumber"] == System.DBNull.Value ? null : (string)reader["RateNumber"];
                        item.EnfType = reader["EnfType"] == System.DBNull.Value ? null : (string)reader["EnfType"];
                        item.SpaceStatus = reader["SpaceStatus"] == System.DBNull.Value ? null : (string)reader["SpaceStatus"];
                        item.ZoneName = reader["Zonename"] == System.DBNull.Value ? null : (string)reader["Zonename"];
                        if (string.IsNullOrWhiteSpace(item.StateName))
                        {
                            item.plate = PlateNumber;
                        }
                        else
                        {
                            item.plate = string.Format("{0}, [{1}]", PlateNumber, item.StateName);
                        }

                        if (DataRecordExtensions.HasColumn(reader, "Vendorname"))
                            item.VendorName = reader["Vendorname"] == System.DBNull.Value ? null : (string)reader["Vendorname"];

                        item.Amount = reader["Amount"] == System.DBNull.Value ? default(decimal?) : (decimal)reader["Amount"];

                        item.ZoneId = reader["Zoneid"] == System.DBNull.Value ? default(int?) : (Int32)reader["Zoneid"];

                        item.ValidAnyZone = reader["ValidAnyZone"] == System.DBNull.Value ? default(bool?) : (bool)reader["ValidAnyZone"];

                        if (DataRecordExtensions.HasColumn(reader, "AC1"))
                            item.assignedClass = reader["AC1"] == System.DBNull.Value ? null : (string)reader["AC1"];

                        if (DataRecordExtensions.HasColumn(reader, "AC2"))
                            item.assignedClass2 = reader["AC2"] == System.DBNull.Value ? null : (string)reader["AC2"];

                        if (DataRecordExtensions.HasColumn(reader, "AC3"))
                            item.assignedClass3 = reader["AC3"] == System.DBNull.Value ? null : (string)reader["AC3"];

                        if (DataRecordExtensions.HasColumn(reader, "AC4"))
                            item.assignedClass4 = reader["AC4"] == System.DBNull.Value ? null : (string)reader["AC4"];

                        if (!string.IsNullOrEmpty(item.assignedClass4))
                            item.assignedClass3 = item.assignedClass4;

                        if (DataRecordExtensions.HasColumn(reader, "LE"))
                            item.VendorLpr = reader["LE"] == System.DBNull.Value ? null : (string)reader["LE"];

                        if (DataRecordExtensions.HasColumn(reader, "ENFPermitNo"))
                            item.ENFPermitNo = reader["ENFPermitNo"] == System.DBNull.Value ? null : (string)reader["ENFPermitNo"];

                        if (DataRecordExtensions.HasColumn(reader, "LE"))
                            item.LE = reader["LE"] == System.DBNull.Value ? null : (string)reader["LE"];

                        if (DataRecordExtensions.HasColumn(reader, "DeltaTime"))
                            item.DeltaTime = reader["DeltaTime"] == System.DBNull.Value ? default(DateTime?) : (DateTime)reader["DeltaTime"];


                        if (DataRecordExtensions.HasColumn(reader, "VendorID"))
                            item.VendorID = reader["VendorID"] == System.DBNull.Value ? default(long?) : (Int64)reader["VendorID"];
                        if (DataRecordExtensions.HasColumn(reader, "ImageRawData"))
                            item.ImageRawData = reader["ImageRawData"] == System.DBNull.Value ? null : (string)reader["ImageRawData"];
                        if (DataRecordExtensions.HasColumn(reader, "EnfVendorLPRId"))
                            item.EnfVendorLprID = reader["EnfVendorLPRId"] == System.DBNull.Value ? default(long?) : (Int64)reader["EnfVendorLPRId"];
                        if (DataRecordExtensions.HasColumn(reader, "ObservedTime"))
                            item.ObservedTime = reader["ObservedTime"] == System.DBNull.Value ? default(DateTime?) : (DateTime)reader["ObservedTime"];

                        plates.Add(item);
                    }

                    if (reader.NextResult())
                    {
                        List<Permit> permits = new List<Permit>();
                        while (reader.Read())
                        {
                            var item = new Permit();
                            item.permitNo = reader["permitNo"] == System.DBNull.Value ? null : (string)reader["permitNo"];
                            item.permitType = reader["permitType"] == System.DBNull.Value ? null : (string)reader["permitType"];
                            item.permitSource = reader["permitSource"] == System.DBNull.Value ? null : (string)reader["permitSource"];
                            item.permitZoneId = reader["permitZoneId"] == System.DBNull.Value ? null : (string)reader["permitZoneId"];
                            item.permitZoneName = reader["permitZoneName"] == System.DBNull.Value ? null : (string)reader["permitZoneName"];
                            item.PermitTxnReferenceId = reader["PermitTxnReferenceId"] == System.DBNull.Value ? null : (string)reader["PermitTxnReferenceId"];

                            item.StartDateTime = reader["permitStartDateTime"] == System.DBNull.Value ? default(DateTime?) : (DateTime)reader["permitStartDateTime"];
                            item.permitStartDateTime = DateTimeJson.ToString(item.StartDateTime);

                            item.EndDateTime = reader["permitEndDateTime"] == System.DBNull.Value ? default(DateTime?) : (DateTime)reader["permitEndDateTime"];
                            item.permitEndDateTime = DateTimeJson.ToString(item.EndDateTime);

                            item.TxnDateTime = reader["PermitTxnDateTime"] == System.DBNull.Value ? default(DateTime?) : (DateTime)reader["PermitTxnDateTime"];
                            item.PermitTxnDateTime = DateTimeJson.ToString(item.TxnDateTime);

                            permits.Add(item);

                        }

                        if (plates != null)
                            if (plates.Count > 0)
                                if (plates[0] != null)
                                    plates[0].Permits = permits;

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
            return plates;
        }


        private static List<PlateNo> GetPlateMatchGlendale51Muscat(int customerID, string PlateNumber)
        {
            List<PlateNo> plates = new List<PlateNo>();
            SqlConnection connection = null;
            try
            {
                connection = new SqlConnection(System.Configuration.ConfigurationManager.ConnectionStrings["Connector"].ToString());
                SqlCommand sqlCommand = new SqlCommand();
                sqlCommand.CommandType = CommandType.StoredProcedure;
                //sqlCommand.CommandText = "Space_Plate_status_Zone_v5_1"; //
                sqlCommand.CommandText = "Space_Plate_status_Zone_v5_1_1_Muscat"; //Added for Restircting 16 hours to Genetec plate.
                //sqlCommand.CommandText = "Space_Plate_status_Zone_v5_1_1_TESTTT";  //to remove
                sqlCommand.Parameters.AddWithValue("@customerID", customerID);
                sqlCommand.Parameters.AddWithValue("@PlateNumber", PlateNumber);
                sqlCommand.Connection = connection;
                sqlCommand.CommandTimeout = 90;
                connection.Open();
                using (var reader = sqlCommand.ExecuteReader())
                {
                    while (reader.Read())
                    {
                        var item = new PlateNo();
                        item.CustomerId = reader["CustomerId"] == System.DBNull.Value ? default(int?) : (Int32)reader["CustomerId"];
                        item.plate = reader["PlateNumber"] == System.DBNull.Value ? null : (string)reader["PlateNumber"];
                        item.PresentMeterTime = reader["PresentMeterTime"] == System.DBNull.Value ? default(DateTime?) : (DateTime)reader["PresentMeterTime"];
                        item.ExpiryTime = reader["ExpiryTime"] == System.DBNull.Value ? default(DateTime?) : (DateTime)reader["ExpiryTime"];
                        item.StateName = reader["ENFState"] == System.DBNull.Value ? null : (string)reader["ENFState"];
                        item.RateName = reader["RateNumber"] == System.DBNull.Value ? null : (string)reader["RateNumber"];
                        item.EnfType = reader["EnfType"] == System.DBNull.Value ? null : (string)reader["EnfType"];
                        item.SpaceStatus = reader["SpaceStatus"] == System.DBNull.Value ? null : (string)reader["SpaceStatus"];
                        item.ZoneName = reader["Zonename"] == System.DBNull.Value ? null : (string)reader["Zonename"];
                        if (string.IsNullOrWhiteSpace(item.StateName))
                        {
                            item.plate = PlateNumber;
                        }
                        else
                        {
                            item.plate = string.Format("{0}, [{1}]", PlateNumber, item.StateName);
                        }

                        if (DataRecordExtensions.HasColumn(reader, "Vendorname"))
                            item.VendorName = reader["Vendorname"] == System.DBNull.Value ? null : (string)reader["Vendorname"];

                        item.Amount = reader["Amount"] == System.DBNull.Value ? default(decimal?) : (decimal)reader["Amount"];

                        item.ZoneId = reader["Zoneid"] == System.DBNull.Value ? default(int?) : (Int32)reader["Zoneid"];

                        item.ValidAnyZone = reader["ValidAnyZone"] == System.DBNull.Value ? default(bool?) : (bool)reader["ValidAnyZone"];

                        if (DataRecordExtensions.HasColumn(reader, "AC1"))
                            item.assignedClass = reader["AC1"] == System.DBNull.Value ? null : (string)reader["AC1"];

                        if (DataRecordExtensions.HasColumn(reader, "AC2"))
                            item.assignedClass2 = reader["AC2"] == System.DBNull.Value ? null : (string)reader["AC2"];

                        if (DataRecordExtensions.HasColumn(reader, "AC3"))
                            item.assignedClass3 = reader["AC3"] == System.DBNull.Value ? null : (string)reader["AC3"];

                        if (DataRecordExtensions.HasColumn(reader, "AC4"))
                            item.assignedClass4 = reader["AC4"] == System.DBNull.Value ? null : (string)reader["AC4"];

                        if (!string.IsNullOrEmpty(item.assignedClass4))
                            item.assignedClass3 = item.assignedClass4;

                        if (DataRecordExtensions.HasColumn(reader, "LE"))
                            item.VendorLpr = reader["LE"] == System.DBNull.Value ? null : (string)reader["LE"];

                        if (DataRecordExtensions.HasColumn(reader, "ENFPermitNo"))
                            item.ENFPermitNo = reader["ENFPermitNo"] == System.DBNull.Value ? null : (string)reader["ENFPermitNo"];

                        if (DataRecordExtensions.HasColumn(reader, "LE"))
                            item.LE = reader["LE"] == System.DBNull.Value ? null : (string)reader["LE"];

                        if (DataRecordExtensions.HasColumn(reader, "DeltaTime"))
                            item.DeltaTime = reader["DeltaTime"] == System.DBNull.Value ? default(DateTime?) : (DateTime)reader["DeltaTime"];


                        if (DataRecordExtensions.HasColumn(reader, "VendorID"))
                            item.VendorID = reader["VendorID"] == System.DBNull.Value ? default(long?) : (Int64)reader["VendorID"];
                        if (DataRecordExtensions.HasColumn(reader, "ImageRawData"))
                            item.ImageRawData = reader["ImageRawData"] == System.DBNull.Value ? null : (string)reader["ImageRawData"];
                        if (DataRecordExtensions.HasColumn(reader, "EnfVendorLPRId"))
                            item.EnfVendorLprID = reader["EnfVendorLPRId"] == System.DBNull.Value ? default(long?) : (Int64)reader["EnfVendorLPRId"];
                        if (DataRecordExtensions.HasColumn(reader, "ObservedTime"))
                            item.ObservedTime = reader["ObservedTime"] == System.DBNull.Value ? default(DateTime?) : (DateTime)reader["ObservedTime"];

                        plates.Add(item);
                    }

                    if (reader.NextResult())
                    {
                        List<Permit> permits = new List<Permit>();
                        while (reader.Read())
                        {
                            var item = new Permit();
                            item.permitNo = reader["permitNo"] == System.DBNull.Value ? null : (string)reader["permitNo"];
                            item.permitType = reader["permitType"] == System.DBNull.Value ? null : (string)reader["permitType"];
                            item.permitSource = reader["permitSource"] == System.DBNull.Value ? null : (string)reader["permitSource"];
                            item.permitZoneId = reader["permitZoneId"] == System.DBNull.Value ? null : (string)reader["permitZoneId"];
                            item.permitZoneName = reader["permitZoneName"] == System.DBNull.Value ? null : (string)reader["permitZoneName"];
                            item.PermitTxnReferenceId = reader["PermitTxnReferenceId"] == System.DBNull.Value ? null : (string)reader["PermitTxnReferenceId"];

                            item.StartDateTime = reader["permitStartDateTime"] == System.DBNull.Value ? default(DateTime?) : (DateTime)reader["permitStartDateTime"];
                            item.permitStartDateTime = DateTimeJson.ToString(item.StartDateTime);

                            item.EndDateTime = reader["permitEndDateTime"] == System.DBNull.Value ? default(DateTime?) : (DateTime)reader["permitEndDateTime"];
                            item.permitEndDateTime = DateTimeJson.ToString(item.EndDateTime);

                            item.TxnDateTime = reader["PermitTxnDateTime"] == System.DBNull.Value ? default(DateTime?) : (DateTime)reader["PermitTxnDateTime"];
                            item.PermitTxnDateTime = DateTimeJson.ToString(item.TxnDateTime);

                            permits.Add(item);

                        }

                        if (plates != null)
                            if (plates.Count > 0)
                                if (plates[0] != null)
                                    plates[0].Permits = permits;

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
            return plates;
        }

        private static List<PlateNo> GetPlateMatchGlendale51(int customerID, string PlateNumber, string Zone)
        {
            List<PlateNo> plates = new List<PlateNo>();
            SqlConnection connection = null;
            try
            {
               //Not imlemented
               
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
            return plates;
        }

        private static List<PlateNo> GetPlateMatchAtlanta(int customerID, string PlateNumber)
        {
            List<PlateNo> plates = new List<PlateNo>();
            SqlConnection connection = null;
            try
            {
                connection = new SqlConnection(System.Configuration.ConfigurationManager.ConnectionStrings["Connector"].ToString());
                SqlCommand sqlCommand = new SqlCommand();
                sqlCommand.CommandType = CommandType.StoredProcedure;
                //sqlCommand.CommandText = "Space_Plate_status_Zone";
                sqlCommand.CommandText = "Space_Plate_status_Zone_v1";
                sqlCommand.Parameters.AddWithValue("@customerID", customerID);
                sqlCommand.Parameters.AddWithValue("@PlateNumber", PlateNumber);
                sqlCommand.Connection = connection;
                sqlCommand.CommandTimeout = 90;
                connection.Open();
                using (var reader = sqlCommand.ExecuteReader())
                {
                    while (reader.Read())
                    {
                        var item = new PlateNo();
                        item.CustomerId = reader["CustomerId"] == System.DBNull.Value ? default(int?) : (Int32)reader["CustomerId"];
                        item.plate = reader["PlateNumber"] == System.DBNull.Value ? null : (string)reader["PlateNumber"];
                        item.PresentMeterTime = reader["PresentMeterTime"] == System.DBNull.Value ? default(DateTime?) : (DateTime)reader["PresentMeterTime"];
                        item.ExpiryTime = reader["ExpiryTime"] == System.DBNull.Value ? default(DateTime?) : (DateTime)reader["ExpiryTime"];
                        item.StateName = reader["ENFState"] == System.DBNull.Value ? null : (string)reader["ENFState"];
                        item.RateName = reader["RateNumber"] == System.DBNull.Value ? null : (string)reader["RateNumber"];
                        item.EnfType = reader["EnfType"] == System.DBNull.Value ? null : (string)reader["EnfType"];
                        item.SpaceStatus = reader["SpaceStatus"] == System.DBNull.Value ? null : (string)reader["SpaceStatus"];
                        item.ZoneName = reader["Zonename"] == System.DBNull.Value ? null : (string)reader["Zonename"];

                        if (item.PresentMeterTime.HasValue == true && item.ExpiryTime.HasValue == true)
                        {
                            var totalSeconds = (item.ExpiryTime.Value.Subtract(item.PresentMeterTime.Value)).TotalSeconds;
                            if (totalSeconds >= -(600))
                                if (item.SpaceStatus.ToLower() == "Expired".ToLower())
                                {
                                    item.SpaceStatus = "Paid";
                                }
                        }

                        if (string.IsNullOrWhiteSpace(item.StateName))
                        {
                            item.plate = PlateNumber;
                        }
                        else
                        {
                            item.plate = string.Format("{0}, [{1}]", PlateNumber, item.StateName);
                        }

                        if (DataRecordExtensions.HasColumn(reader, "Vendorname"))
                            item.VendorName = reader["Vendorname"] == System.DBNull.Value ? null : (string)reader["Vendorname"];

                        item.Amount = reader["Amount"] == System.DBNull.Value ? default(decimal?) : (decimal)reader["Amount"];

                        item.ZoneId = reader["Zoneid"] == System.DBNull.Value ? default(int?) : (Int32)reader["Zoneid"];

                        item.ValidAnyZone = reader["ValidAnyZone"] == System.DBNull.Value ? default(bool?) : (bool)reader["ValidAnyZone"];

                        plates.Add(item);
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
            return plates;
        }

        public static class DateTimeJson
        {
            public static string ToString(DateTime? value)
            {
                if (value.HasValue)
                {
                    // return value.Value.ToString("o");
                    return DateTime.SpecifyKind(value.Value, DateTimeKind.Utc).ToString("O");
                }
                else
                    return string.Empty;
            }
        }
    }
}
