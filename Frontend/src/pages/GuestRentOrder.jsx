import React, { useState } from "react";
import { Button } from "@material-tailwind/react";
import { useSelector } from "react-redux";
import { useNavigate } from "react-router-dom";
import {
  faCaretDown,
  faCaretUp,
  faShoppingBag,
} from "@fortawesome/free-solid-svg-icons";
import { FontAwesomeIcon } from "@fortawesome/react-fontawesome";
import { selectGuestRentalOrders } from "../redux/slices/guestOrderSlice";

const statusColors = {
  "Chờ xử lý": "bg-yellow-100 text-yellow-800",
  "Đã xác nhận đơn": "bg-blue-100 text-blue-800",
  "Đang xử lý": "bg-purple-100 text-purple-800",
  "Đã giao cho đơn vị vận chuyển": "bg-indigo-100 text-indigo-800",
  "Đã giao hàng": "bg-green-100 text-green-800",
  "Đã hủy": "bg-red-100 text-red-800",
};

const paymentStatusColors = {
  "Đang chờ thanh toán": "text-yellow-800",
  "Đã đặt cọc": "text-blue-800",
  "Đã thanh toán": "text-green-800",
  "Đã hủy": "text-red-800",
};

export default function GuestRentalOrderList() {
  const rentalOrders = useSelector(selectGuestRentalOrders);
  const [selectedStatus, setSelectedStatus] = useState("Tất cả");
  const [expandedOrderId, setExpandedOrderId] = useState(null);
  const navigate = useNavigate();
console.log(rentalOrders)
  const groupedOrders = rentalOrders.reduce(
    (acc, order) => {
      if (!order.parentOrderCode) {
        acc.parents.push(order);
      } else {
        acc.children[order.parentOrderCode] = acc.children[order.parentOrderCode] || [];
        acc.children[order.parentOrderCode].push(order);
      }
      return acc;
    },
    { parents: [], children: {} }
  );

  const filteredOrders =
    selectedStatus === "Tất cả"
      ? groupedOrders.parents
      : groupedOrders.parents.filter((order) => order.orderStatus === selectedStatus);

  const toggleExpand = (orderId) => {
    setExpandedOrderId((prev) => (prev === orderId ? null : orderId));
  };

  if (filteredOrders.length === 0) {
    return (
      <div className="text-center text-gray-500 mt-32 flex flex-col items-center justify-center">
        <FontAwesomeIcon icon={faShoppingBag} className="text-6xl mb-2" />
        <p>Bạn chưa có sản phẩm nào</p>
      </div>
    );
  }

  return (
    <div className="container mx-auto px-4 py-8">
      <div className="min-h-screen max-w-4xl mx-auto">
        <h2 className="text-orange-500 font-bold text-2xl mb-4">
          Danh sách đơn thuê
        </h2>
        <div className="rounded-lg overflow-x-auto w-full mb-6">
          <div className="flex justify-start p-4 bg-gray-50 border-b space-x-2 whitespace-nowrap">
            {[
              "Tất cả",
              "Chờ xử lý",
              "Đã xác nhận đơn",
              "Đang xử lý",
              "Đã giao cho đơn vị vận chuyển",
              "Đã giao hàng",
              "Đã hủy",
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
          {filteredOrders.map((parent) => (
            <div
              key={parent.id}
              className="p-4 border border-gray-200 rounded-lg shadow-sm"
            >
              <div
                className="flex justify-between items-center p-6 cursor-pointer hover:bg-slate-200 transition-colors duration-150 ease-in-out"
                onClick={() => toggleExpand(parent.id)}
              >
                <div>
                  <h4 className="font-semibold text-lg text-gray-800">
                    Mã đơn hàng:{" "}
                    <span className="text-orange-500">
                      {parent.rentalOrderCode}
                    </span>
                    <span
                      className={`px-3 py-1 ml-2.5 rounded-full text-xs font-medium ${
                        statusColors[parent.orderStatus] ||
                        "bg-gray-100 text-gray-800"
                      }`}
                    >
                      {parent.orderStatus}
                    </span>
                  </h4>
                  <p className="text-gray-600">
                    Trạng thái thanh toán:
                    <span
                      className={`ml-2 font-medium ${
                        paymentStatusColors[parent.paymentStatus] ||
                        "text-gray-800"
                      }`}
                    >
                      {parent.paymentStatus}
                    </span>
                  </p>
                  <p className="text-gray-600">
                    Hình thức nhận hàng: {parent.deliveryMethod}
                  </p>
                  <p className="text-gray-600">
                    Ngày đặt: {new Date(parent.createdAt).toLocaleDateString()}
                  </p>
                  <p className="mt-2 font-bold text-lg">
                    Tổng giá:{" "}
                    <span className="text-orange-500">
                      {parent.totalAmount.toLocaleString()} ₫
                    </span>
                  </p>
                </div>
                <div className="flex flex-col w-1/4 h-auto items-end">
                  <img
                    src={parent.orderImage || "/assets/images/default_package.png"}
                    alt={parent.orderImage}
                    className="w-full h-auto object-contain rounded mb-2"
                  />
                  <Button
                    color="orange"
                    size="sm"
                    className="w-full"
                    onClick={(e) => {
                      e.stopPropagation();
                      navigate(`/guest-rent-order/${parent.rentalOrderCode}`);
                    }}
                  >
                    Xem chi tiết
                  </Button>
                </div>
                <FontAwesomeIcon
                  icon={expandedOrderId === parent.id ? faCaretUp : faCaretDown}
                  className="w-6 h-6 text-gray-500"
                />
              </div>

              {expandedOrderId === parent.id && (
                <div className="mt-4 pl-8 border-l">
                  {groupedOrders.children[parent.rentalOrderCode]?.length > 0 ? (
                    groupedOrders.children[parent.rentalOrderCode].map(
                      (child) => (
                        <div
                          key={child.id}
                          className="flex p-2 border-b last:border-none"
                        >
                          <img
                            src={child.imgAvatarPath || "default-image.jpg"}
                            alt="Order"
                            className="w-24 h-24 object-contain rounded mr-4"
                          />
                          <div>
                            <h5 className="font-medium text-base">
                              {child.productName}
                            </h5>
                            <p className="text-sm text-gray-500">
                              {child.color} - {child.size} - {child.condition}%
                            </p>
                            <p className="font-medium text-base text-rose-700">
                              Giá thuê:{" "}
                              {child.totalAmount.toLocaleString()} ₫
                            </p>
                            <p className="text-sm text-gray-500">
                              Số lượng: {child.quantity}
                            </p>
                          </div>
                        </div>
                      )
                    )
                  ) : (
                    <div className="flex p-2">
                      <img
                        src={parent.imgAvatarPath || "default-image.jpg"}
                        alt="Order"
                        className="w-24 h-24 object-contain rounded mr-4"
                      />
                      <div>
                        <h3 className="font-medium text-base">
                          {parent.productName}
                        </h3>
                        <p className="text-sm text-gray-500">
                          {parent.color} - {parent.size} - {parent.condition}%
                        </p>
                        <p className="font-medium text-base text-rose-700">
                          Số tiền: {parent.totalAmount.toLocaleString()} ₫
                        </p>
                      </div>
                    </div>
                  )}
                </div>
              )}
            </div>
          ))}
        </div>
      </div>
    </div>
  );
}
