using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace _2Sport_BE.Service.Enums
{
    public enum RentalOrderStatus : int
    {
        CANCELLED = 0,          // Đã hủy
        PENDING = 1,            // Chờ xử lý
        CONFIRMED = 2,          // Đã xác nhận
        PAID = 3,               // Đã thanh toán
        PROCESSING = 4,         // Đang xử lý
        SHIPPED = 5,            // Đã giao hàng
        DELAYED = 6,            // Bị trì hoãn
        COMPLETED = 7,          // Đã hoàn tất thuê
        AWAITING_PICKUP = 8,    // Chờ nhận hàng
        RETURN_REQUESTED = 9,  // Yêu cầu trả sản phẩm
        RETURNED = 10,          // Đã trả sản phẩm
        INSPECTING = 11,        // Đang kiểm tra sản phẩm
        CLOSED = 12,            // Đơn thuê đã kết thúc
        FAILED = 13,          // Xử lý đơn thất bại,
    }
}
