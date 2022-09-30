using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace Restaurant.Notification
{
    [Flags]
    public enum Accepted
    {
        Rejected = 1,
        Kitchen = 2,
        Booking = 4,
        All = Kitchen | Booking,
        DishStopList = Booking | Rejected
    }
}
