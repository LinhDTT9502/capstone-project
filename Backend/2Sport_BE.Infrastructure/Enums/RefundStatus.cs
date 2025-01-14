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
        Pending,       // Yêu cầu hoàn tiền đang chờ xử lý
        Approved,      // Yêu cầu hoàn tiền đã được chấp thuận
        Rejected,      // Yêu cầu hoàn tiền bị từ chối
        Processed,     // Đã thực hiện hoàn tiền cho khách hàng
        Failed,        // Hoàn tiền thất bại
        Completed      // Quy trình hoàn tiền đã hoàn tất
    }
}
