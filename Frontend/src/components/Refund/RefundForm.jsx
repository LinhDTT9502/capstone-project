import React, { useEffect, useState } from "react";
import axios from "axios";
import { Button, Input, Textarea, Checkbox } from "@material-tailwind/react";
import { toast, ToastContainer } from "react-toastify";
import {
  fetchUserOrders,
  fetchUserRentalOrders,
} from "../../services/userOrderService";
import { FontAwesomeIcon } from "@fortawesome/react-fontawesome";
import { faShoppingBag } from "@fortawesome/free-solid-svg-icons";
const RefundForm = () => {
  const [userOrders, setOrders] = useState([]);
  const [orderCode, setOrderCode] = useState("");
  const [orderType, setOrderType] = useState(1);
  const [reason, setReason] = useState("");
  const [notes, setNotes] = useState("");
  const [isAgreementAccepted, setIsAgreementAccepted] = useState(false);
  const [isSubmitting, setIsSubmitting] = useState(false);
  const [expandedOrderId, setExpandedOrderId] = useState(null);
  const [filteredSaleOrders, setFilteredSaleOrders] = useState([]);
  const [filteredRentalOrders, setFilteredRentalOrders] = useState([]);
  const [groupedOrders, setgroupedOrders] = useState(null);
  const [isOpen, setIsOpen] = useState(false);
  const [selected, setSelected] = useState(null);
  const [expandedItem, setExpandedItem] = useState(null);

  const statusColors = {
    "Chờ xử lý": "bg-yellow-100 text-yellow-800",
    "Đã xác nhận đơn": "bg-blue-100 text-blue-800",
    "Đã thanh toán": "bg-green-100 text-green-800",
    "Đang xử lý": "bg-purple-100 text-purple-800",
    "Đã giao hàng": "bg-indigo-100 text-indigo-800",
    "Đã từ chối": "bg-red-100 text-red-800",
    "Đã hủy": "bg-red-200 text-red-900",
    "Đã hoàn thành": "bg-orange-100 text-orange-800",
  };
  const paymentStatusColors = {
    "Đang chờ thanh toán": "bg-yellow-100 text-yellow-800",
    "Đã đặt cọc": "bg-blue-100 text-blue-800",
    "Đã thanh toán": "bg-green-100 text-green-800",
    "Đã hủy": "bg-red-100 text-red-800",
  };
  const reasons = [
    { name: "Thiếu hàng" },
    { name: "Người bán gửi sai hàng" },
    {
      name: "Hàng bể vỡ",
      children: [
        { name: "Thùng hàng không nguyên vẹn" },
        { name: "Hàng trầy / xước/ nứt" },
        { name: "Hàng bể / vỡ vụn" },
      ],
    },
    { name: "Hàng lỗi, không hoạt động" },
    { name: "Hàng hết hạn sử dụng" },
    { name: "Khác với mô tả" },
    { name: "Hàng nguyên vẹn nhưng không còn nhu cầu" },
    { name: "Khác" },
  ];
  const handleSubmit = async (e) => {
    e.preventDefault();

    if (!isAgreementAccepted) {
      toast.warning("'Bạn phải đồng ý với chính sách hoàn tiền của hệ thống.");
      return;
    }

    const payload = {
      orderCode,
      orderType: Number(orderType), // Đảm bảo orderType là số
      reason,
      notes,
      isAgreementAccepted,
    };

    setIsSubmitting(true);
    // console.log(payload);
    try {
      const response = await axios.post(
        "https://twosport-api-offcial-685025377967.asia-southeast1.run.app//api/RefundRequest/create",
        payload,
        {
          headers: {
            accept: "*/*",
            "Content-Type": "application/json",
          },
        }
      );
      // console.log(response);
      if (response.data.isSuccess === true) {
        toast.success("Yêu cầu hoàn tiền đã được gửi thành công!");
        setOrderCode("");
        setOrderType(1);
        setReason("");
        setNotes("");
        setFilteredRentalOrders([]);
        setFilteredSaleOrders([]);
        setIsAgreementAccepted(false);

        setIsSubmitting(false);
      } else {
        toast.warning(response.data.message);
      }
    } catch (error) {
        // console.log("Lỗi khi tải đơn hàng:", error.response.data.message);
      toast.error(error.response.data.message ?? "Đã xảy ra lỗi. Vui lòng kiểm tra lại thông tin đơn hàng.");
      setIsSubmitting(false);
    }
  };
  
  const toggleExpand = (orderId) => {
    setExpandedOrderId((prevOrderId) =>
      prevOrderId === orderId ? null : orderId
    );
    setOrderCode(orderId);
  };

  const ordersToRefund = async () => {
    const token = localStorage.getItem("token");
    if (!token) {
      toast.error("Bạn chưa đăng nhập");
      return;
    }

    try {
      const decodeToken = JSON.parse(atob(token.split(".")[1]));

      if (!decodeToken || !decodeToken.UserId) {
        toast.error("Token không hợp lệ");
        return;
      }

      let response = [];
      if (orderType === 1) {
        response = await fetchUserOrders(decodeToken.UserId, token);
      } else if (orderType === 2) {
        response = await fetchUserRentalOrders(decodeToken.UserId, token);
      } else {
        alert("OrderType không hợp lệ");
        return;
      }

      if (!response || response.length === 0) {
        toast.info("Không có đơn hàng phù hợp");
        return;
      }

      const sortedOrders = response.sort(
        (a, b) => new Date(b.createdAt) - new Date(a.createdAt)
      );
      setOrders(sortedOrders);

      if (orderType === 1) {
        const filtered = sortedOrders.filter(
          (order) =>
            order.orderStatus === "Đã hủy" &&
            order.paymentStatus === "Đã thanh toán"
        );
        setFilteredSaleOrders(filtered);
        setFilteredRentalOrders(null);
      } else if (orderType === 2) {
        const filtered = sortedOrders.filter(
          (order) =>
            order.orderStatus === "Đã hủy" &&
            (order.paymentStatus === "Đã thanh toán" || order.depositAmount > 0)
        );
        setFilteredRentalOrders(filtered);
        setFilteredSaleOrders(null);

        const groupOrders = filtered.reduce(
          (acc, order) => {
            if (!order.parentOrderCode) {
              acc.parents.push(order);
            } else {
              acc.children[order.parentOrderCode] =
                acc.children[order.parentOrderCode] || [];
              acc.children[order.parentOrderCode].push(order);
            }
            return acc;
          },
          { parents: [], children: {} }
        );
        setgroupedOrders(groupOrders);
      }
    } catch (error) {
      console.log("Lỗi khi tải đơn hàng:", error.response.data.message);
      toast.error("Đã xảy ra lỗi khi tải đơn hàng");
    }
  };

  const handleSelect = (reason) => {
    setSelected(reason);
    setReason(reason.name);
    setIsOpen(false);
  };

  return (
    <>
      <ToastContainer position="top-right" autoClose={3000} />
      <div className="min-h-screen flex bg-slate-200">
        {/* Left Section: Search Product */}
        <div className="flex-1 bg-white p-6 shadow-lg rounded-md mx-4 my-8">
          <h2 className="text-xl font-semibold text-gray-800 mb-6">
            Tìm kiếm đơn hàng
          </h2>
          <div className="space-y-4">
            <div>
              <label className="block text-sm font-medium text-gray-700">
                Loại đơn hàng
              </label>
              <select
                value={orderType}
                onChange={(e) => setOrderType(Number(e.target.value))}
                className="mt-1 w-full p-2 border border-gray-300 rounded-md"
                required
              >
                <option value={1}>Đơn Mua</option>
                <option value={2}>Đơn Thuê</option>
              </select>
            </div>
            <Button
              type="button"
              onClick={ordersToRefund}
              className="w-full mt-4 bg-black text-white py-2 rounded-md hover:bg-gray-500"
            >
              Tìm kiếm
            </Button>
          </div>
          <div className="container mx-auto pt-2 rounded-lg max-w-4xl max-h-[65vh] overflow-y-auto">
            {(filteredSaleOrders && filteredSaleOrders.length > 0) ||
            (filteredRentalOrders && filteredRentalOrders.length > 0) ? (
              <>
                {/* Hiển thị đơn hàng bán */}
                {filteredSaleOrders &&
                  filteredSaleOrders.length > 0 &&
                  filteredSaleOrders.map((saleOrder) => (
                    <div
                      key={saleOrder.saleOrderCode}
                      className="p-4 border border-gray-200 rounded-lg shadow-sm mt-4 relative flex flex-col"
                    >
                      <div
                        className="flex justify-between items-center p-6 cursor-pointer hover:bg-slate-200 transition-colors duration-150 ease-in-out"
                        onClick={() => toggleExpand(saleOrder.saleOrderCode)}
                      >
                        <div className="flex flex-col">
                          <h4 className="font-semibold text-lg text-gray-800">
                            Mã đơn hàng:{" "}
                            <span className="text-orange-500">
                              {saleOrder.saleOrderCode}
                            </span>
                          </h4>
                          <p className="text-gray-600">
                            Trạng thái đơn hàng:
                            <span
                              className={`px-3 ml-2.5 py-1.5 rounded-full text-xs font-medium ${
                                statusColors[saleOrder.orderStatus] ||
                                "bg-gray-100 text-gray-800"
                              }`}
                            >
                              {saleOrder.orderStatus}
                            </span>
                          </p>
                          <p className="py-2 text-gray-600">
                            Trạng thái thanh toán:
                            <span
                              className={`px-3 ml-2.5 py-1.5 rounded-full text-xs font-medium ${
                                paymentStatusColors[saleOrder.paymentStatus] ||
                                "bg-gray-100 text-gray-800"
                              }`}
                            >
                              {saleOrder.paymentStatus}
                            </span>
                          </p>
                          <p className="text-gray-600">
                            <span>Hình thức nhận hàng:</span>{" "}
                            <span className="italic">
                              {saleOrder.deliveryMethod}
                            </span>
                          </p>
                          <p className="text-gray-600">
                            <span>Ngày đặt:</span>{" "}
                            <span className="italic">
                              {new Date(
                                saleOrder.createdAt
                              ).toLocaleDateString()}
                            </span>
                          </p>
                          <p className="mt-2 font-bold text-lg">
                            Thành tiền:{" "}
                            <span className="text-orange-500">
                              {saleOrder.totalAmount.toLocaleString("Vi-vn")} ₫
                            </span>
                          </p>
                        </div>
                        <div className="flex flex-col w-1/4 h-auto items-end">
                          <img
                            src={
                              saleOrder.orderImage ||
                              "/assets/images/default_package.png"
                            }
                            alt="Order"
                            className="w-full h-auto object-contain rounded"
                          />
                        </div>
                      </div>
                      {expandedOrderId === saleOrder.saleOrderCode && (
                        <div className="mt-4 pl-8 border-l">
                          {saleOrder.saleOrderDetailVMs.$values.map((item) => (
                            <div
                              key={item.productId}
                              className="flex p-2 border-b last:border-none cursor-pointer"
                            >
                              <img
                                src={item.imgAvatarPath || "default-image.jpg"}
                                alt={item.productName}
                                className="w-24 h-24 object-contain rounded"
                              />
                              <div className="ml-4">
                                <h5 className="font-medium text-base">
                                  {item.productName}
                                </h5>
                                <p className="text-sm text-gray-500">
                                  Màu sắc: {item.color} - Kích thước:{" "}
                                  {item.size} - Tình trạng: {item.condition}%
                                </p>

                                <p className="font-medium text-base text-rose-700">
                                  Đơn giá bán:{" "}
                                  {item.unitPrice.toLocaleString("Vi-vn")} ₫
                                </p>
                                <p className="font-medium text-sm">
                                  Số lượng: {item.quantity}
                                </p>
                              </div>
                            </div>
                          ))}
                        </div>
                      )}
                    </div>
                  ))}

                {/* Hiển thị đơn hàng thuê */}
                {filteredRentalOrders &&
                  filteredRentalOrders.length > 0 &&
                  filteredRentalOrders.map((parent) => (
                    <div
                      key={parent.rentalOrderCode}
                      className="p-4 border border-gray-200 rounded-lg shadow-sm mt-4 relative flex flex-col"
                    >
                      <div
                        className="flex justify-between items-center p-6 cursor-pointer hover:bg-slate-200 transition-colors duration-150 ease-in-out"
                        onClick={() => toggleExpand(parent.rentalOrderCode)}
                      >
                        <div className="flex flex-col">
                          <h4 className="font-semibold text-lg text-gray-800">
                            Mã đơn hàng:{" "}
                            <span className="text-orange-500">
                              {parent.rentalOrderCode}
                            </span>
                          </h4>
                          <p className="text-gray-600">
                            Trạng thái đơn hàng:
                            <span
                              className={`px-3 ml-2.5 py-1.5 rounded-full text-xs font-medium ${
                                statusColors[parent.orderStatus] ||
                                "bg-gray-100 text-gray-800"
                              }`}
                            >
                              {parent.orderStatus}
                            </span>
                          </p>
                          <p className="py-2 text-gray-600">
                            Trạng thái thanh toán:
                            <span
                              className={`px-3 ml-2.5 py-1.5 rounded-full text-xs font-medium ${
                                paymentStatusColors[parent.paymentStatus] ||
                                "bg-gray-100 text-gray-800"
                              }`}
                            >
                              {parent.paymentStatus}
                            </span>
                          </p>

                          <p className="text-gray-600">
                            <span>Ngày đặt:</span>{" "}
                            <span className="italic">
                              {new Date(parent.createdAt).toLocaleDateString()}
                            </span>
                          </p>
                          <p className="text-green-700 font-semibold">
                            Đã đặt cọc:{" "}
                            {(parent.depositAmount
                              ? parent.depositAmount
                              : 0
                            ).toLocaleString("vi-VN")}{" "}
                            ₫
                          </p>
                          <p className="mt-2 font-bold text-lg">
                            Thành tiền:{" "}
                            <span className="text-orange-500">
                              {parent.totalAmount.toLocaleString("Vi-vn")}₫
                            </span>
                          </p>
                        </div>
                        <div className="flex flex-col w-1/4 h-auto items-end">
                          <img
                            src={
                              parent.orderImage ||
                              "/assets/images/default_package.png"
                            }
                            alt="Order"
                            className="w-full h-auto object-contain rounded"
                          />
                        </div>
                      </div>
                      {expandedOrderId === parent.rentalOrderCode && (
                        <div className="mt-4 pl-8 border-l">
                          {groupedOrders.children[parent.rentalOrderCode]
                            ?.length > 0 ? (
                            groupedOrders.children[parent.rentalOrderCode].map(
                              (child) => (
                                <div
                                  key={child.id}
                                  className="flex p-2 border-b last:border-none cursor-pointer"
                                >
                                  <img
                                    src={
                                      child.imgAvatarPath || "default-image.jpg"
                                    }
                                    alt="Order"
                                    className="w-24 h-24 object-contain rounded"
                                  />
                                  <div>
                                    <h5 className="font-medium text-base">
                                      {child.productName}
                                    </h5>
                                    <p className="text-sm text-gray-500">
                                      Màu sắc: {child.color} - Kích thước:{" "}
                                      {child.size} - Tình trạng:{" "}
                                      {child.condition}%
                                    </p>
                                    <p className="mt-2">
                                      <span className="font-semibold">
                                        Thời gian thuê:
                                      </span>{" "}
                                      {new Date(
                                        child.rentalStartDate
                                      ).toLocaleDateString()}{" "}
                                      -{" "}
                                      {new Date(
                                        child.rentalEndDate
                                      ).toLocaleDateString()}
                                    </p>
                                    <p className="font-medium text-base text-rose-700">
                                      Đơn giá thuê:{" "}
                                      {new Intl.NumberFormat("vi-VN", {
                                        style: "currency",
                                        currency: "VND",
                                      }).format(child.rentPrice)}
                                    </p>
                                    <p className="font-medium text-sm text-gray-500">
                                      Số lượng: {child.quantity}
                                    </p>
                                  </div>
                                </div>
                              )
                            )
                          ) : (
                            <div>
                              <img
                                src={
                                  parent.imgAvatarPath || "default-image.jpg"
                                }
                                alt="Order"
                                className="w-24 h-24 object-contain rounded"
                              />
                              <div>
                                <h3 className="font-medium text-base">
                                  {parent.productName}
                                </h3>
                                <p className="text-sm text-gray-500">
                                  Màu sắc: {parent.color} - Kích thước:{" "}
                                  {parent.size} - Tình trạng: {parent.condition}
                                  %
                                </p>
                                <p className="mt-2">
                                  <span className="font-semibold">
                                    Thời gian thuê:
                                  </span>{" "}
                                  {new Date(
                                    parent.rentalStartDate
                                  ).toLocaleDateString()}{" "}
                                  -{" "}
                                  {new Date(
                                    parent.rentalEndDate
                                  ).toLocaleDateString()}
                                </p>
                                <p className="font-medium text-base text-rose-700">
                                  Đơn giá thuê:{" "}
                                  {new Intl.NumberFormat("vi-VN", {
                                    style: "currency",
                                    currency: "VND",
                                  }).format(parent.rentPrice)}
                                </p>
                                <p className="font-medium text-sm text-gray-500">
                                  Số lượng: {parent.quantity}
                                </p>
                              </div>
                            </div>
                          )}
                        </div>
                      )}
                    </div>
                  ))}
              </>
            ) : (
              <div className="text-center text-gray-500 mt-32 flex flex-col items-center justify-center">
                <FontAwesomeIcon
                  icon={faShoppingBag}
                  className="text-6xl mb-2"
                />
                <p>Bạn chưa có đơn hàng nào</p>
              </div>
            )}
          </div>
        </div>

        <div className="flex-1 bg-white p-6 shadow-lg rounded-md mx-4 my-8 flex justify-center items-center">
          <div className="max-w-lg bg-white p-8 rounded-md shadow-md  items-center">
            <h2 className="text-2xl font-semibold text-gray-800 text-center mb-4">
              Biểu mẫu yêu cầu hoàn tiền
            </h2>
            <p className="text-sm text-gray-600 mb-6">
              Nếu bạn không hài lòng với đơn hàng của mình, bạn có thể yêu cầu
              hoàn tiền. Vui lòng cung cấp thông tin đầy đủ và lý do chi tiết.
            </p>
            <form onSubmit={handleSubmit} className="space-y-4">
              <div>
                <label className="block text-sm font-medium text-gray-700 pb-2">
                  Mã đơn hàng
                </label>
                <Input
                  type="text"
                  label="VD: 2411041041"
                  value={orderCode}
                  readOnly
                  className="mt-1 w-full border border-gray-300 bg-gray-100 rounded-md cursor-not-allowed"
                  required
                />
              </div>
              <div className="relative w-80 text-gray-700">
                <label className="block text-sm font-medium pb-2">Lý do</label>
                <button
                  type="button"
                  className="w-full text-left bg-white border border-gray-300 rounded-md py-2 px-3 shadow-sm focus:outline-none focus:ring-2 focus:ring-indigo-500 focus:border-indigo-500"
                  onClick={() => setIsOpen((prev) => !prev)}
                >
                  {selected ? selected.name : "Chọn lý do hoàn tiền"}
                </button>
                {isOpen && (
                  <div className="absolute z-10 bg-white border border-gray-300 rounded-md mt-1 shadow-lg w-full">
                    <ul className="py-2">
                      {reasons.map((reason, idx) => (
                        <li key={idx} className="relative">
                          <button
                            type="button"
                            className={`w-full text-left px-4 py-2 text-gray-700 hover:bg-orange-200 ${
                              reason.children
                                ? "flex justify-between items-center"
                                : ""
                            }`}
                            onClick={() =>
                              reason.children
                                ? setExpandedItem(
                                    expandedItem === reason.name
                                      ? null
                                      : reason.name
                                  )
                                : handleSelect(reason)
                            }
                          >
                            {reason.name}
                            {reason.children && (
                              <svg
                                xmlns="http://www.w3.org/2000/svg"
                                fill="none"
                                viewBox="0 0 24 24"
                                strokeWidth={2}
                                stroke="currentColor"
                                className="w-4 h-4 text-gray-500"
                              >
                                <path
                                  strokeLinecap="round"
                                  strokeLinejoin="round"
                                  d="M9 5l7 7-7 7"
                                />
                              </svg>
                            )}
                          </button>
                          {reason.children && expandedItem === reason.name && (
                            <ul className="absolute top-5 left-full bg-white border border-gray-300 rounded-md mt-0 shadow-lg w-max">
                              {reason.children.map((child, childIdx) => (
                                <li key={childIdx}>
                                  <button
                                    type="button"
                                    className="w-full text-left px-4 py-2 text-gray-700 hover:bg-orange-200"
                                    onClick={() => handleSelect(child)}
                                  >
                                    {child.name}
                                  </button>
                                </li>
                              ))}
                            </ul>
                          )}
                        </li>
                      ))}
                    </ul>
                  </div>
                )}
              </div>

              <div>
                <label className="block text-sm font-medium text-gray-700 pb-2">
                  Ghi chú bổ sung (không bắt buộc)
                </label>
                <Textarea
                  label="Chi tiết vấn đề bạn gặp phải (nếu có)"
                  value={notes}
                  onChange={(e) => setNotes(e.target.value)}
                  rows={3}
                  className="mt-1 w-full border border-gray-300 rounded-md text-gray-700"
                />
              </div>

              <div className="flex items-center">
                <Checkbox
                  checked={isAgreementAccepted}
                  onChange={() => setIsAgreementAccepted(!isAgreementAccepted)}
                />
                <span className="text-sm text-gray-600 ml-2">
                  Tôi đồng ý với chính sách hoàn tiền của hệ thống.
                </span>
              </div>

              <Button
                type="submit"
                color="orange"
                className="w-full mt-4 hover:bg-gray-500"
                disabled={
                  isSubmitting || !orderCode || !reason || !isAgreementAccepted
                }
              >
                {isSubmitting ? "Đang gửi..." : "Gửi yêu cầu hoàn tiền"}
              </Button>
            </form>
          </div>
        </div>
      </div>
    </>
  );
};

export default RefundForm;
