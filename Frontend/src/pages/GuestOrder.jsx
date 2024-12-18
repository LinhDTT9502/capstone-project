import React, { useState } from "react";
import { useSelector } from "react-redux";
import { selectGuestOrders } from "../redux/slices/guestOrderSlice";
import { Link, useNavigate } from "react-router-dom";
import { FontAwesomeIcon } from "@fortawesome/react-fontawesome";
import {
  faArrowLeft,
  faShoppingBag,
  faCaretDown,
  faCaretUp,
  faTag,
  faMoneyBillWave,
  faTruck,
  faCalendarAlt,
} from "@fortawesome/free-solid-svg-icons";
import { useTranslation } from "react-i18next";
import { Button } from "@material-tailwind/react";

const statusColors = {
  "Chờ xử lý": "bg-yellow-100 text-yellow-800",
  "Đã xác nhận": "bg-orange-100 text-orange-800",
  "Đã thanh toán": "bg-green-100 text-green-800",
  "Đang xử lý": "bg-purple-100 text-purple-800",
  "Đã giao hàng": "bg-indigo-100 text-indigo-800",
  "Bị trì hoãn": "bg-red-100 text-red-800",
  "Đã hủy": "bg-red-200 text-red-900",
  "Hoàn thành": "bg-teal-100 text-teal-800",
};

const paymentStatusColors = {
  "Đang chờ thanh toán": "text-yellow-800",
  "Đã đặt cọc": "text-orange-800",
  "Đã thanh toán": "text-green-800",
  "Đã hủy": "text-red-800",
};

const GuestOrder = () => {
  const orders = useSelector(selectGuestOrders);
  console.log(orders);
  
  const navigate = useNavigate();
  const { t } = useTranslation();
  const [selectedStatus, setSelectedStatus] = useState("Tất cả");
  const [expandedOrderId, setExpandedOrderId] = useState(null);

  const toggleExpand = (orderId) => {
    setExpandedOrderId((prevOrderId) =>
      prevOrderId === orderId ? null : orderId
    );
  };

  const filteredOrders =
    selectedStatus === "Tất cả"
      ? orders
      : orders.filter((order) => order.orderStatus === selectedStatus);

  const formatCurrency = (amount) => {
    return new Intl.NumberFormat("vi-VN", {
      style: "currency",
      currency: "VND",
    }).format(amount);
  };

  if (!orders || orders.length === 0) {
    return (
      <div className="flex flex-col items-center my-10 py-32">
              <img src="/assets/images/cart-icon.png" className="w-48 h-auto object-contain" />
              <p className="pt-4 text-lg font-poppins">{t("guest_order.empty")}</p>
              <Link
                to="/product"
                className="text-blue-500 flex items-center font-poppins"
              >
                <FontAwesomeIcon className="pr-2" icon={faArrowLeft} />{" "}
                {t("guest_order.continue_shopping")}
              </Link>
            </div>
    );
  }

  return (
    <div className="container mx-auto px-4 py-8">
      <div className="min-h-screen max-w-4xl mx-auto">
        <h1 className="text-3xl font-bold text-gray-800 mb-6">Đơn đặt hàng</h1>

        <div className="mb-6 bg-white rounded-lg shadow overflow-x-auto">
          <div className="flex p-4 space-x-2">
            {[
              "Tất cả",
              "Chờ xử lý",
              "Đã xác nhận",
              "Đã thanh toán",
              "Đang xử lý",
              "Đã giao hàng",
              "Hoàn thành",
            ].map((status) => (
              <button
                key={status}
                className={`px-4 py-2 rounded-full text-sm font-medium transition-colors duration-150 ease-in-out ${
                  selectedStatus === status
                    ? "bg-orange-500 text-white"
                    : "bg-gray-200 text-gray-700 hover:bg-gray-300"
                }`}
                onClick={() => setSelectedStatus(status)}
              >
                {status}
              </button>
            ))}
          </div>
        </div>

        <div className="space-y-6">
          {filteredOrders.map((order) => (
            <div
              key={order.id}
              className="bg-white rounded-lg shadow-md overflow-hidden"
            >
              <div
                className="p-6 cursor-pointer hover:bg-gray-50 transition-colors duration-150 ease-in-out"
                onClick={() => toggleExpand(order.id)}
              >
                <div className="flex justify-between items-start">
                  <div>
                    <div className="flex items-center gap-3 mb-2">
                      <h2 className="text-xl font-semibold text-gray-800">
                        Mã đơn hàng:{" "}
                        <span className="text-orange-600 font-medium">
                          {order.saleOrderCode}
                        </span>
                      </h2>
                      <span
                        className={`inline-flex items-center px-3 py-1 rounded-full text-xs font-semibold capitalize ${
                          statusColors[order.orderStatus]
                        }`}
                      >
                        {order.orderStatus}
                      </span>
                    </div>

                    <p className="text-sm text-gray-600 mb-1">
                      <FontAwesomeIcon icon={faCalendarAlt} className="mr-2" />
                      Ngày đặt hàng:{" "}
                      {new Date(order.createdAt).toLocaleDateString()}
                    </p>
                    {console.log(order)}
                    <p className="text-sm text-gray-600 mb-1">
                      <FontAwesomeIcon
                        icon={faMoneyBillWave}
                        className="mr-2"
                      />
                      Tổng tiền:{" "}
                      <span className="font-semibold">
                        {formatCurrency(order.totalAmount)}
                      </span>
                    </p>
                    <p className="text-sm text-gray-600 mb-1">
                      <FontAwesomeIcon icon={faTruck} className="mr-2" />
                      Hình thức giao hàng: {order.deliveryMethod}
                    </p>
                  </div>
                  <div className="flex flex-col w-1/7 h-auto items-end">
                    {" "}
                    <img
                      src={
                        order.orderImage ||
                        "/assets/images/sale-order-image.png"
                      }
                      alt={order.productName}
                      className="w-full h-32 object-contain rounded"
                    />
                    <Button
                      color="orange"
                      size="sm"
                      className="w-full"
                      onClick={() =>
                        navigate(`/guest-order/${order.id}`, {
                          state: { order },
                        })
                      }
                    >
                      Xem chi tiết
                    </Button>{" "}
                  </div>

                  {/* <FontAwesomeIcon
                    icon={
                      expandedOrderId === order.id ? faCaretUp : faCaretDown
                    }
                    className="text-gray-400"
                  /> */}
                </div>
              </div>
              {expandedOrderId === order.id && (
                <div className="px-6 pb-6 pt-2 border-t border-gray-200">
                  <h3 className="text-lg font-semibold text-gray-800 mb-3">
                    Đơn hàng chi tiết
                  </h3>
                  <div className="space-y-4">
                    {order.saleOrderDetailVMs.$values.map((item) => (
                      <div
                        key={item.productId}
                        className="flex items-center space-x-4"
                      >
                        <img
                          src={item.imgAvatarPath || "/placeholder.png"}
                          alt={item.productName}
                          className="w-16 h-16 object-contain rounded"
                        />
                        <div>
                          <h5 className="font-medium text-base">
                            {item.productName}
                          </h5>
                          <p className="text-sm text-gray-500">
                            Màu sắc: {item.color} - Kích thước: {item.size} -
                            Tình trạng: {item.condition}%
                          </p>
                          <p className="font-medium text-base text-rose-700">
                            Giá bán: {formatCurrency(item.unitPrice)}₫
                          </p>
                          <p className="font-medium text-sm">
                            Số lượng: {item.quantity}
                          </p>
                        </div>
                      </div>
                    ))}
                  </div>
                </div>
              )}
            </div>
          ))}
        </div>
      </div>
    </div>
  );
};

export default GuestOrder;