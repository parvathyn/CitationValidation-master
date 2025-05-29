using System;

namespace DataAccess.Model.IPC
{
    public  class IPCParkingData
    {
        public IPCParkingData() { }
        public string LicensePlateNumber { get; set; }
        public DateTime ParkingExpiryTime { get; set; }
        public DateTime ParkingStartTime { get; set; }
        public string PoleNumber { get; set; }
        public string SubAreaName { get; set; }
        public TimeSpan TimeZone { get; set; }


      
    }
}
