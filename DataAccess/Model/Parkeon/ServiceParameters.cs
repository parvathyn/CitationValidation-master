using System;
using System.Collections.Generic;
using System.Configuration;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace DataAccess.Model.Parkeon
{
    public class ServiceParameters
    {
        public string userName { get; set; }
        public string userPassword { get; set; }
        //public ServiceType serviceType { get; set; }
        //public string token { get; set; }
        public long customerId { get; set; }
        public long vendorId { get; set; }
        public string MethodName { get; set; }
        public string Value { get; set; }
        public TimeZoneInfo CustomerTimeZoneInfo { get; set; }
        public string URI
        {
            get
            {
                //return ConfigurationManager.AppSettings["ParkeonbaseURL"];
                return @"http://www.prm.parkeonsmartcenter.com:90/pbp_enforcement_api/";
            }
        }
    }
}
