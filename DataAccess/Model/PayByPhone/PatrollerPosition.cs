using System.Collections.Generic;

namespace DataAccess.Model.PayByPhone
{
    public class PatrollerPosition
    {
        public IList<string> zones { get; set; }
        public IList<string> sectors { get; set; }
    }
}
