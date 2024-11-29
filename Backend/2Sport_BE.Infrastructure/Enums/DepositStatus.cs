using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace _2Sport_BE.Service.Enums
{
    public enum DepositStatus : int
    {
        Paid = 1,
        Partially_Paid = 2,
        Not_Paid = 3,
        Refunded = 4,
        Pending = 6,
        Partially_Pending = 7
    }
}
