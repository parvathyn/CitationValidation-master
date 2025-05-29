using System;
using System.Collections.Generic;

namespace DataAccess.Model.PayByPhone
{
    public class Eligibility
    {
        public string plate { get; set; }
        public IList<string> sectors { get; set; }
        public DateTime startDateTime { get; set; }
        public DateTime endDateTime { get; set; }
        public string eligibilityType { get; set; }
    }
}
