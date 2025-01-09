using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace _2Sport_BE.Service.Enums
{
    public enum ReturnStatus : int
    {
        Pending = 1,      // Yêu cầu đang chờ xử lý
        Approved = 2,     // Yêu cầu được chấp nhận
        Rejected= 3,     // Yêu cầu bị từ chối
        Refunded = 4,     // Đã hoàn tiền
        Completed = 5     // Hoàn tất quy trình trả hàng
    }
}
