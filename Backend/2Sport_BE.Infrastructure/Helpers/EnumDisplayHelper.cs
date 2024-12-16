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
        { PaymentStatus.PENDING, "Đang chờ thanh toán" },
        { PaymentStatus.PAID, "Đã thanh toán" },
        { PaymentStatus.REFUNDED, "Đã hoàn tiền" },
        { PaymentStatus.CANCELED, "Đã hủy" },

        { OrderStatus.CANCELLED, "Đã hủy" },
        { OrderStatus.PENDING, "Chờ xử lý" },
        { OrderStatus.CONFIRMED, "Đã xác nhận" },
        { OrderStatus.PROCESSING, "Đang xử lý" },
        { OrderStatus.SHIPPED, "Đã giao cho đơn vị vận chuyển" },
        { OrderStatus.DELIVERED, "Đã giao hàng" },
        { OrderStatus.DECLINED, "Đã từ chối" },
        { OrderStatus.RETURN_PROCESSING, "Đang xử lý trả hàng" },
        { OrderStatus.RETURNED, "Đã trả hàng" },
        { OrderStatus.REFUND_PROCESSING, "Đang xử lý hoàn tiền" },
        { OrderStatus.REFUNDED, "Đã hoàn tiền" },
        { OrderStatus.COMPLETED, "Đã hoàn thành" },

        { RentalOrderStatus.CANCELED, "Đã hủy" },
        { RentalOrderStatus.PENDING, "Chờ xử lý" },
        { RentalOrderStatus.CONFIRMED, "Đã xác nhận đơn" },
        { RentalOrderStatus.PROCESSING, "Đang xử lý" },
        { RentalOrderStatus.SHIPPED, "Đã giao cho đơn vị vận chuyển" },
        { RentalOrderStatus.DELIVERED, "Đã giao hàng" },
        { RentalOrderStatus.AWAITING_PICKUP, "Chờ khách nhận hàng" },
        { RentalOrderStatus.DECLINED, "Đã từ chối" },
        { RentalOrderStatus.RENTED, "Đang thuê" },
        { RentalOrderStatus.EXTENSION_REQUESTED, "Yêu cầu gia hạn" },
        { RentalOrderStatus.DELAYED, "Bị trì hoãn" },
        { RentalOrderStatus.RETURN_REQUESTED, "Yêu cầu trả sản phẩm" },
        { RentalOrderStatus.RETURNED, "Đã trả sản phẩm" },
        { RentalOrderStatus.INSPECTING, "Đang kiểm tra sản phẩm trả" },
        { RentalOrderStatus.COMPLETED, "Đơn thuê đã kết thúc" },
        { RentalOrderStatus.FAILED, "Xử lý đơn thất bại" },

        { DepositStatus.FULL_PAID, "Đã thanh toán cọc 100% tiền thuê" },
        { DepositStatus.PARTIALLY_PAID, "Đã thanh toán cọc 50% tiền thuê" },
        { DepositStatus.NOT_PAID, "Chưa thanh toán" },
        { DepositStatus.REFUNDED, "Đã hoàn trả" },
        { DepositStatus.PENDING, "Đang chờ thanh toán cọc 100% tiền thuê" },
        { DepositStatus.PARTIALLY_PENDING, "Đang chờ thanh toán cọc 50% tiền thuê" },

        { OrderMethods.COD, "Giao hàng thu tiền hộ - COD" },
        { OrderMethods.PayOS, "Quét mã QR - PayOs" },
        { OrderMethods.VnPay, "Giao dịch trực tuyến - VnPay" },
        { OrderMethods.BankTransfer, "Chuyển khoản ngân hàng" },

        { MemberLevel.Normal_Member, "Thành viên thường" },
        { MemberLevel.Silver_Member, "Thành viên bạc" },
        { MemberLevel.Gold_Member, "Thành viên vàng" },
        { MemberLevel.Diamond_Member, "Thành viên kim cương" },

        {ExtensionRequestStatus.PENDING, "Đang chờ xử lý"},
        {ExtensionRequestStatus.APPROVED, "Chấp nhận yêu cầu gia hạn"},
        {ExtensionRequestStatus.REJECTED, "Từ chối yêu cầu gia hạn"},
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
            return "N/A";
        }
    }
}
