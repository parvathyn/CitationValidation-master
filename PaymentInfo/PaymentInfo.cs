using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using System.Data;
using System.IO;
using DataAccess.Model;

namespace PaymentInfo
{
    public class PaymentInfo : IPaymentInfo
    {
        public PaymentsData GetPaymentInfo(string customerId, string zoneId = "")
        {
            var appPath = System.Web.Hosting.HostingEnvironment.ApplicationPhysicalPath;
            try
            {
                string filePath = Path.Combine(appPath, "App_Data", "Timezones", "TimeZones.xml");
                return TransactionDataValidatioin.GetPaymentTransformData(customerId, zoneId,null, filePath);
            }
            catch (Exception ex)
            {
                string errorPath = Path.Combine(appPath, "App_Data", "Timezones");
                ExceptionLogging.SendErrorToText(ex, "GetCitationData", errorPath);
                ex = null;
                return new PaymentsData() { ReturnCode = ((int)ReturnCodeEnum.Error).ToString() };
            }
        }


        public PaymentsData GetPaymentBySpaceInfo(string customerId, string zoneId, string spaceId)
        {
            var appPath = System.Web.Hosting.HostingEnvironment.ApplicationPhysicalPath;
            try
            {
                string filePath = Path.Combine(appPath, "App_Data", "Timezones", "TimeZones.xml");
                return TransactionDataValidatioin.GetPaymentTransformData(customerId, zoneId,spaceId, filePath);
            }
            catch (Exception ex)
            {
                string errorPath = Path.Combine(appPath, "App_Data", "Timezones");
                ExceptionLogging.SendErrorToText(ex, "GetCitationData", errorPath);
                ex = null;
                return new PaymentsData() { ReturnCode = ((int)ReturnCodeEnum.Error).ToString() };
            }
        }
    }
}
