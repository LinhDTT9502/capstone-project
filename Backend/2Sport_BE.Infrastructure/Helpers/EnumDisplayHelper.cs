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

        { RentalOrderStatus.CANCELLED, "Đã hủy" },
        { RentalOrderStatus.PENDING, "Chờ xử lý" },
        { RentalOrderStatus.CONFIRMED, "Đã xác nhận" },
        { RentalOrderStatus.PAID, "Đã thanh toán" },
        { RentalOrderStatus.PROCESSING, "Đang xử lý" },
        { RentalOrderStatus.SHIPPED, "Đã giao hàng" },
        { RentalOrderStatus.DELAYED, "Bị trì hoãn" },
        { RentalOrderStatus.COMPLETED, "Đã hoàn tất thuê" },
        { RentalOrderStatus.AWAITING_PICKUP, "Chờ nhận hàng" },
        { RentalOrderStatus.RETURN_REQUESTED, "Yêu cầu trả sản phẩm" },
        { RentalOrderStatus.RETURNED, "Đã trả sản phẩm" },
        { RentalOrderStatus.INSPECTING, "Đang kiểm tra sản phẩm" },
        { RentalOrderStatus.CLOSED, "Đơn thuê đã kết thúc" },
        { RentalOrderStatus.FAILED, "Thất bại" },

        { DepositStatus.Paid, "Đã thanh toán" },
        { DepositStatus.Partially_Paid, "Đã thanh toán cọc 50%" },
        { DepositStatus.Not_Paid, "Chưa thanh toán" },
        { DepositStatus.Refunded, "Đã hoàn trả" },
        { DepositStatus.Pending, "Đang chờ thanh toán" },
        { DepositStatus.Partially_Pending, "Đang chờ thanh toán cọc 50%" },

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
