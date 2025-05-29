using System;
using System.Collections.Generic;
using System.Data;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace DataAccess.Model
{
    public class Utility
    {
       public static int getStatus(string status)
        {
                int isOccupied = 0 ;
                string[] occStatusDescArr = new string[] { "VIOLATED", "OCCUPIED", "EXPIRED", "VACANT" }; //Occupied,Expired
                var Index = Array.IndexOf(occStatusDescArr, status.ToUpper()); 
                if (Index != -1) {
                    isOccupied = 1;
                } else {
                    isOccupied = 0;
                }
                return isOccupied;
        }

       private string calcOccTimeDiff_Marked(DateTime CurrentMeterTime, DateTime markedSinceTime)
       {

           if (markedSinceTime == null)
           {
               return string.Empty;
           }
           TimeSpan span = CurrentMeterTime.Subtract(markedSinceTime);
           int HrsDiff = span.Hours;
           int minDiff = span.Minutes;
           string hrs = string.Empty;
           string min = string.Empty;


           if (HrsDiff.ToString().Length == 1)
           {
               hrs = "0" + HrsDiff.ToString();
           }
           else
           {
               hrs = HrsDiff.ToString();
           }

           if (minDiff.ToString().Length == 1)
           {
               min = "0" + minDiff.ToString();
           }
           else
           {
               min = minDiff.ToString();
           }

           string result = hrs + ":" + min;
           return result;
       }

       public static string calcOccTimeDiff(DateTime CurrentMeterTime, DateTime sensorEventTime)
       {

           if (sensorEventTime == null)
           {
               return string.Empty;
           }
           TimeSpan span = CurrentMeterTime.Subtract(sensorEventTime);
           int HrsDiff = span.Hours;
           int minDiff = span.Minutes;
           string hrs = string.Empty;
           string min = string.Empty;


           if (HrsDiff.ToString().Length == 1)
           {
               hrs = "0" + HrsDiff.ToString();
           }
           else
           {
               hrs = HrsDiff.ToString();
           }

           if (minDiff.ToString().Length == 1)
           {
               min = "0" + minDiff.ToString();
           }
           else
           {
               min = minDiff.ToString();
           }

           string result = hrs + ":" + min;
           return result;
       }

       private string calcTimeDiff(DateTime CurrentMeterTime, DateTime expiryTime)
       {
           TimeSpan span = CurrentMeterTime.Subtract(expiryTime);
           int daysDiff = span.Days;
           int HrsDiff = span.Hours;
           int minDiff = span.Minutes;
           string hrs = string.Empty;
           string min = string.Empty;


           if (HrsDiff.ToString().Length == 1)
           {
               hrs = "0" + HrsDiff.ToString();
           }
           else
           {
               hrs = HrsDiff.ToString();
           }

           if (minDiff.ToString().Length == 1)
           {
               min = "0" + minDiff.ToString();
           }
           else
           {
               min = minDiff.ToString();
           }

           string result = hrs + ":" + min;
           return result;
       }

       public static DateTime findGMTTime(DateTime? ReceivedTime, TimeZoneInfo TZ)
       {
           //** To convert the ReceivedTime to UTC / GMT Time zone, use the timezone xml file
           //** to retriev the standard time zone ID / Name for the given city name found in DB

           DateTime UTCDateTime = new DateTime();
           DateTime myRecdTime = Convert.ToDateTime(ReceivedTime);

           //** Now calculate the UTC time for the standard timezonename based on timezoneid and for given 'ReceivedTime'
           DateTime utcStart = DateTime.SpecifyKind(myRecdTime, DateTimeKind.Unspecified);

           //UTCDateTime = TimeZoneInfo.ConvertTimeToUtc(myRecdTime, TZ);

           UTCDateTime = TimeZoneInfo.ConvertTimeToUtc(utcStart, TZ);

           return UTCDateTime;
       }

       public static TimeZoneInfo GetTimeZoneInfo(int? myTimeZoneID, string filepath , string customerId)
       {
           List<SelectListItemCustom> Result = new List<SelectListItemCustom>();

           TimeZoneInfo TZ = null;
           if (customerId == "4102")
           {
               TZ = TimeZoneInfo.FindSystemTimeZoneById("Central Standard Time");
           }
           else
           {
               TZ = TimeZoneInfo.FindSystemTimeZoneById("Eastern Standard Time");
           }

           //TimeZoneInfo TZ = TimeZoneInfo.FindSystemTimeZoneById("Eastern Standard Time");
           try
           {
               //** Get the path of the xml file to load;
               String xmlFilePath = filepath; // System.Web.HttpContext.Current.Server.MapPath("~/App_Data/Timezones/TimeZones.xml");

               //** Create the Dataset and store the xml content in it;
               DataSet mydata = new DataSet();
               mydata.ReadXml(xmlFilePath);

               DataTable DestTable = mydata.Tables[4]; // Get the destination table data.

               Result = processDataTable(DestTable);

               //var shortTimeZoneName = (from tz in duncanDataContext.TimeZones
               //                         where tz.TimeZoneID == myTimeZoneID
               //                         select tz).FirstOrDefault();

               var shortTimeZoneName = DataAccess.GetTimeZones(myTimeZoneID.Value);


               var toBeSearched = shortTimeZoneName[0].TimeZoneName;
               var zoneId = "";


               //** Now process the Result List to find 'toBeSearched' item. If it matches, get the Time zone name.
               for (var i = 0; i < Result.Count(); i++)
               {
                   if (Result[i].Value.ToString().Trim().ToUpper() == toBeSearched.Trim().ToUpper())
                   {
                       zoneId = Result[i].Text;
                       break;
                   }
               }

               TZ = TimeZoneInfo.FindSystemTimeZoneById(zoneId);


           }
           catch (Exception e)
           {
           }
           return TZ;
       }

       public static List<SelectListItemCustom> processDataTable(DataTable TargetTable)
       {
           List<SelectListItemCustom> result = new List<SelectListItemCustom>();

           foreach (DataRow row in TargetTable.Rows) // Loop over the rows.
           {
               foreach (DataColumn col in row.Table.Columns)  //loop through the columns. 
               {

                   string columnToBeSearchedIs = "other";
                   string colNameFromDS = col.ColumnName;

                   if (columnToBeSearchedIs == colNameFromDS)
                   {
                       string TimeZoneNameIs = row["other"].ToString();
                       string TimeZoneShortNameIs = row["type"].ToString();
                       if (!string.IsNullOrEmpty(TimeZoneNameIs))
                       {
                           //** Only if the column name starts with "Col" has some value, insert into list;
                           result.Add(new SelectListItemCustom { Text = TimeZoneNameIs, Value = TimeZoneShortNameIs });


                       }
                   }

               }

           };

           return result;
       }

       public static int? NullableInt(string str)
       {
           int i;
           if (int.TryParse(str, out i))
               return i;
           return null;
       }

    }


}
