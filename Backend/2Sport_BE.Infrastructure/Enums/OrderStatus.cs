using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace _2Sport_BE.Service.Enums
{
    public enum OrderStatus : int
    {
        CANCELLED = 0,
        PENDING = 1,
        PROCESSING = 2,
        PAID = 3,
        DELIVERED = 4,
        DELAYED = 5
    }
}
