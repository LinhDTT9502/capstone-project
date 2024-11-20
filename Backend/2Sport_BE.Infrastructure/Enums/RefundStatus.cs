using Google.Apis.Util;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Runtime.Serialization;
using System.Text;
using System.Threading.Tasks;

namespace _2Sport_BE.Service.Enums
{
    public enum RefundStatus : int
    {
        [EnumMember(Value = "Pending")]
        Pending,       // Yêu cầu hoàn tiền đang chờ xử lý

        [EnumMember(Value = "Approved")]
        Approved,      // Yêu cầu hoàn tiền đã được chấp thuận

        [EnumMember(Value = "Rejected")]
        Rejected,      // Yêu cầu hoàn tiền bị từ chối

        [EnumMember(Value = "Processed")]
        Processed,     // Đã thực hiện hoàn tiền cho khách hàng

        [EnumMember(Value = "Failed")]
        Failed,        // Hoàn tiền thất bại

        [EnumMember(Value = "Completed")]
        Completed      // Quy trình hoàn tiền đã hoàn tất
    }
}
