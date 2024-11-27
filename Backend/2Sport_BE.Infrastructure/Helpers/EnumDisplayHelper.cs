using _2Sport_BE.Infrastructure.Enums;
using _2Sport_BE.Service.Enums;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace _2Sport_BE.Service.Helpers
{
    public class EnumDisplayHelper
    {
        private static readonly Dictionary<Enum, string> displayNames = new Dictionary<Enum, string>
    {
        { PaymentStatus.IsWating, "Đang chờ thanh toán" },
        { PaymentStatus.IsPaid, "Đã thanh toán" },
        { PaymentStatus.IsDeposited, "Đã đặt cọc" },
        { PaymentStatus.IsCanceled, "Đã hủy" },

        { OrderStatus.PENDING, "Chờ xử lý" },
        { OrderStatus.CONFIRMED, "Đã xác nhận" },
        { OrderStatus.PAID, "Đã thanh toán" },
        { OrderStatus.PROCESSING, "Đang xử lý" },
        { OrderStatus.SHIPPED, "Đã giao hàng" },
        { OrderStatus.DELAYED, "Bị trì hoãn" },
        { OrderStatus.COMPLETED, "Hoàn thành" },
        { OrderStatus.CANCELLED, "Đã hủy" },

         { RentalOrderStatus.PENDING, "Chờ xử lý" },
        { RentalOrderStatus.CONFIRMED, "Đã xác nhận" },
        { RentalOrderStatus.PAID, "Đã thanh toán" },
        { RentalOrderStatus.PROCESSING, "Đang xử lý" },
        { RentalOrderStatus.SHIPPED, "Đã giao hàng" },
        { RentalOrderStatus.DELAYED, "Bị trì hoãn" },
        { RentalOrderStatus.COMPLETED, "Hoàn thành" },
        { RentalOrderStatus.CANCELLED, "Đã hủy" },
        { RentalOrderStatus.EXTEND, "Gia hạn" },

         { DepositStatus.Paid, "Đã thanh toán" },
        { DepositStatus.Partially_Paid, "Đã thanh toán một phần" },
        { DepositStatus.Not_Paid, "Chưa thanh toán" },
        { DepositStatus.Refunded, "Đã hoàn trả" }
    };

        public static string GetDisplayName(Enum enumValue)
        {
            return displayNames.TryGetValue(enumValue, out var name) ? name : enumValue.ToString();
        }
        public static string GetEnumDescription<TEnum>(int value) where TEnum : Enum
        {
            if (Enum.IsDefined(typeof(TEnum), value))
            {
                var enumValue = (TEnum)Enum.ToObject(typeof(TEnum), value);
                if (displayNames.TryGetValue(enumValue, out var description))
                {
                    return description;
                }
            }
            return "N/A"; // Trả về "N/A" nếu không tìm thấy mô tả
        }
    }
}
