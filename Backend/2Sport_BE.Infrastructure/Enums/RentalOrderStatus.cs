using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace _2Sport_BE.Service.Enums
{
    public enum RentalOrderStatus : int
    {
        CANCELED = 0,          // Đã hủy đơn
        PENDING = 1,            // Chờ xử lý
        CONFIRMED = 2,          // Đã xác nhận đơn
        PROCESSING = 3,         // Đang xử lý
        SHIPPED = 4,            // Đã giao hàng
        DELIVERED = 5,            // Đã giao hàng
        DECLINED = 6,
        AWAITING_PICKUP = 7,    // Chờ khách nhận hàng
        RENTED = 8,             // Đang thuê
        EXTENSION_REQUESTED = 9,// Yêu cầu gia hạn
        DELAYED = 10,            // Bị trì hoãn
        RETURN_REQUESTED = 11,   // Yêu cầu trả sản phẩm
        RETURNED = 12,          // Đã trả sản phẩm
        INSPECTING = 13,        // Đang kiểm tra sản phẩm trả
        COMPLETED = 14,         // Đơn thuê đã kết thúc
        FAILED = 15             // Xử lý đơn thất bại
    }
}
