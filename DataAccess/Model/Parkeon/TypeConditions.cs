using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace DataAccess.Model.Parkeon
{
    public enum TypeConditions : int
    {
        control_groups = 1,
        initialize = 2,
        ticketsallzones = 3,
        control_areas = 4,
        check_plate = 5
    }
}
