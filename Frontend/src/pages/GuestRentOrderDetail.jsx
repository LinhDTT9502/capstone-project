import React, { useState, useEffect } from "react";
import { useParams, useNavigate, Link } from "react-router-dom";
import axios from "axios";
import { FontAwesomeIcon } from "@fortawesome/react-fontawesome";
import {
  faArrowLeft,
  faUser,
  faEnvelope,
  faPhone,
  faMapMarkerAlt,
  faShoppingCart,
  faMoneyBillWave,
  faCalendarAlt,
  faTruck,
  faCoins,
  faBolt,
  faVenusMars,
  faDollarSign,
} from "@fortawesome/free-solid-svg-icons";
import { Button, Tooltip, Typography, Input } from "@material-tailwind/react";
import CancelRentalOrderButton from "../components/User/CancelRentalOrderButton";

const statusColors = {
  "Chờ xử lý": "bg-yellow-100 text-yellow-800",
  "Đã xác nhận đơn": "bg-blue-100 text-blue-800",
  "Đã thanh toán": "bg-green-100 text-green-800",
  "Đang xử lý": "bg-purple-100 text-purple-800",
  "Đã giao hàng": "bg-indigo-100 text-indigo-800",
  "Đã giao cho đơn vị vận chuyển": "bg-indigo-100 text-indigo-800",
  "Bị trì hoãn": "bg-red-100 text-red-800",
  "Đã hủy": "bg-red-200 text-red-900",
  "Hoàn thành": "bg-teal-100 text-teal-800",
};

const paymentStatusColors = {
  "Đang chờ thanh toán": "text-yellow-800",
  "Đã đặt cọc": "text-blue-800",
  "Đã thanh toán": "text-green-800",
  "Đã hủy": "text-red-800",
};

export default function GuestRentOrderDetail() {
  const { orderCode } = useParams();
  const navigate = useNavigate();
  const [orderDetail, setOrderDetail] = useState(null);
  const [isLoading, setIsLoading] = useState(true);
  const [error, setError] = useState(null);
  const [selectedDate, setSelectedDate] = useState(null);
  const [expandedId, setExpandedId] = useState(null);
  const [showModal, setShowModal] = useState(false);
  const [showExtendedModal, setExtendedShowModal] = useState(false);
  const [reload, setReload] = useState(false);
  const [loading, setLoading] = useState(true);
  const [confirmReload, setConfirmReload] = useState(false);

  const fetchOrderDetail = async () => {
    try {
      const response = await axios.get(
        `https://capstone-project-703387227873.asia-southeast1.run.app/api/RentalOrder/get-rental-order-by-orderCode?orderCode=${orderCode}`,
        {
          headers: {
            accept: "*/*",
          },
        }
      );

      if (response.data.isSuccess) {
        setOrderDetail(response.data.data);
      } else {
        setError("Failed to fetch order details");
      }
    } catch (err) {
      setError(err.message || "An error occurred while fetching order details");
    } finally {
      setIsLoading(false);
    }
  };
  useEffect(() => {
    fetchOrderDetail();
  }, [orderCode]);

  const handleExtendOrder = async (order) => {
    if (!selectedDate) {
      alert("Vui lòng chọn ngày gia hạn trước khi xác nhận.");
      return;
    }

    const selectedDateObj = new Date(selectedDate);
    const rentalEndDateObj = new Date(order.rentalEndDate);
    const extensionDays = Math.ceil(
      (selectedDateObj - rentalEndDateObj) / (1000 * 60 * 60 * 24)
    );

    try {
      const response = await axios.post(
        `https://capstone-project-703387227873.asia-southeast1.run.app/api/RentalOrder/request-extension`,
        {
          parentOrderId: orderDetail.id,
          childOrderId: order.id === orderDetail.id ? null : order.id,
          extensionDays: extensionDays,
        },
        {
          headers: {
            accept: "*/*",
          },
        }
      );
      alert("Bạn đã gia hạn thành công");
      // Refresh order details
      fetchOrderDetail();
    } catch (error) {
      console.error("Error extending order:", error);
      alert("Không thể gia hạn đơn hàng. Vui lòng thử lại sau.");
    }
  };

  const handleDoneOrder = async () => {
    if (!orderDetail || !orderDetail.id) {
      alert("Không tìm thấy thông tin đơn hàng để hoàn tất.");
      return;
    }

    const confirmDone = window.confirm(
      "Bạn có chắc chắn muốn hoàn tất đơn hàng này không?"
    );

    if (!confirmDone) {
      return;
    }

    try {
      const response = await axios.put(
        `https://capstone-project-703387227873.asia-southeast1.run.app/api/RentalOrder/update-rental-order-status/${orderDetail.id}?orderStatus=5`,
        {},
        {
          headers: {
            accept: "*/*",
          },
        }
      );
      if (response && response.data.isSuccess) {
        alert("Đơn hàng của bạn đã được hoàn tất thành công.");
        fetchOrderDetail();
      } else {
        alert("Không thể hoàn tất đơn hàng. Vui lòng thử lại sau.");
      }
    } catch (error) {
      console.error("Error updating order status:", error);
      alert("Đã xảy ra lỗi khi hoàn tất đơn hàng. Vui lòng thử lại sau.");
    }
  };

  if (isLoading) {
    return (
      <div className="flex justify-center items-center h-screen">
        <div className="animate-spin rounded-full h-32 w-32 border-t-2 border-b-2 border-blue-500"></div>
      </div>
    );
  }

  if (error) {
    return <div className="text-center text-red-500 mt-4">Error: {error}</div>;
  }
  const {
    id,
    fullName,
    email,
    contactPhone,
    gender,
    address,
    rentalOrderCode,
    childOrders,
    isExtended,
    productName,
    rentPrice,
    rentalStartDate,
    rentalEndDate,
    totalAmount,
    depositAmount,
    orderStatus,
    paymentStatus,
    deliveryMethod,
    imgAvatarPath,
    updatedAt,
  } = orderDetail;

  const children = childOrders?.$values || [];
  return (
    <div className="container mx-auto p-4 min-h-screen bg-gray-200">
      <div className="max-w-4xl mx-auto">
        <div className="flex justify-between items-center mb-6">
          <button
            onClick={() => navigate(-1)}
            className="flex items-center gap-2 text-blue-500 hover:text-blue-700"
          >
            <FontAwesomeIcon icon={faArrowLeft} />
            Quay lại
          </button>
        </div>
        {orderStatus === "Đã hủy" && (
          <div className="bg-yellow-50 p-4 rounded-lg shadow-sm mb-6">
            {console.log(orderDetail)}
            <p className="text-xl">
              <b className="text-red-500">Đã hủy đơn hàng </b>
              <i className="text-lg">
                vào lúc{" "}
                {new Date(orderDetail.updatedAt).toLocaleString("vi-VN", {
                  day: "2-digit",
                  month: "2-digit",
                  year: "numeric",
                  hour: "2-digit",
                  minute: "2-digit",
                  second: "2-digit",
                })}
              </i>
            </p>
            <p className="flex items-center gap-2 mb-2">
              <FontAwesomeIcon icon={faBolt} style={{ color: "#fd7272" }} />
              <span className="font-semibold">Lý do:</span> {orderDetail.reason}
            </p>
          </div>
        )}
        {depositAmount != null && depositAmount > 0 && (
          <div className="bg-green-100 p-4 rounded-lg shadow-sm mb-6">
            <p className="text-xl">
              <b className="text-yellow-300">Đơn hàng đã được đặt cọc </b>
              <i className="text-lg">
                vào lúc{" "}
                {new Date(orderDetail.depositDate).toLocaleString("vi-VN", {
                  day: "2-digit",
                  month: "2-digit",
                  year: "numeric",
                  hour: "2-digit",
                  minute: "2-digit",
                  second: "2-digit",
                })}{" "}
                - {orderDetail.paymentMethod}
              </i>
            </p>
            <p className="flex items-center gap-2 mb-2">
              <FontAwesomeIcon
                icon={faDollarSign}
                style={{ color: "#FFD43B" }}
              />
              <span className="font-semibold">Số tiền: </span>
              {depositAmount.toLocaleString("Vi-vn")}₫
            </p>
          </div>
        )}

        <div className="bg-white p-6 rounded-lg shadow-lg mb-6">
          <div className="py-4 bg-white rounded-md shadow-sm">
            <div className="flex justify-between items-center">
              {console.log(orderDetail)}
              <h2 className="text-2xl font-bold text-gray-800 flex-1">
                Chi tiết đơn hàng -{" "}
                <span className="text-orange-500">#{rentalOrderCode}</span>
              </h2>
              <div className="flex items-center gap-4">
                {paymentStatus === "N/A" &&
                  orderStatus === "Chờ xử lý" &&
                  orderDetail.deliveryMethod !== "HOME_DELIVERY" && (
                    <Button
                      size="sm"
                      className="w-40 text-green-700  bg-white border border-green-700 rounded-md hover:bg-green-200"
                      onClick={() =>
                        navigate("/rental-checkout", {
                          state: { selectedOrder: orderDetail },
                        })
                      }
                    >
                      Thanh toán
                    </Button>
                  )}
                {orderDetail.orderStatus === "Chờ xử lý" && (
                  <CancelRentalOrderButton
                    rentalOrderId={id}
                    setReload={setReload}
                  />
                )}
              </div>
            </div>
          </div>

          <hr className="mb-5" />
          <div className="grid md:grid-cols-2 gap-6">
            <div>
              <h3 className="text-lg font-semibold mb-2 text-gray-700">
                Thông tin khách hàng
              </h3>
              <p className="flex items-center gap-2 mb-2">
                <FontAwesomeIcon
                  icon={faUser}
                  className="text-blue-500 fa-sm"
                />
                <span className="font-semibold">Tên:</span>
                <i>{fullName}</i>
              </p>
              <p className="flex items-center gap-2 mb-2">
                <FontAwesomeIcon
                  icon={faVenusMars}
                  className="text-blue-500 fa-xs"
                />
                <span className="font-semibold">Giới tính:</span>
                <i>{gender}</i>
              </p>
              <p className="flex items-center gap-2 mb-2">
                <FontAwesomeIcon
                  icon={faEnvelope}
                  className="text-blue-500 fa-sm"
                />
                <span className="font-semibold">Email:</span>
                <i>{email}</i>
              </p>
              <p className="flex items-center gap-2 mb-2">
                <FontAwesomeIcon
                  icon={faPhone}
                  className="text-blue-500 fa-sm"
                />
                <span className="font-semibold">Số điện thoại:</span>
                <i>{contactPhone}</i>
              </p>
              <p className="flex items-center gap-2 mb-2">
                <FontAwesomeIcon
                  icon={faMapMarkerAlt}
                  className="text-blue-500"
                />
                <span className="font-semibold flex-shrink-0">Địa chỉ:</span>
                <span className="break-words">
                  <i>{address}</i>
                </span>
              </p>
            </div>
            <div>
              <h3 className="text-lg font-semibold mb-2 text-gray-700">
                Thông tin đơn hàng
              </h3>
              <p className="flex items-center gap-2 mb-2">
                <FontAwesomeIcon
                  icon={faShoppingCart}
                  className="text-blue-500"
                />
                <span className="font-semibold">Mã đơn hàng:</span>{" "}
                <i>{rentalOrderCode}</i>
              </p>
              <p className="flex items-start gap-2 mb-2">
                <FontAwesomeIcon icon={faCoins} className="text-blue-500" />
                <span className="font-semibold flex-shrink-0">
                  Số tiền đặt cọc:{" "}
                </span>
                <i>
                  {(depositAmount ? depositAmount : 0).toLocaleString("vi-VN")}₫
                </i>
              </p>
              <p className="flex items-center gap-2 mb-2">
                <FontAwesomeIcon
                  icon={faMoneyBillWave}
                  className="text-blue-500"
                />
                <span className="font-semibold">Tình trạng thanh toán:</span>{" "}
                <span
                  className={`py-2 px-2.5 mr-1.5 rounded-full text-xs font-bold ${
                    statusColors[paymentStatus] || "bg-gray-100 text-gray-800"
                  }`}
                >
                  {paymentStatus}
                </span>
              </p>
              <p className="flex items-center gap-2 mb-2">
                <FontAwesomeIcon
                  icon={faCalendarAlt}
                  className="text-blue-500"
                />
                <span className="font-semibold">Tình trạng đơn hàng:</span>{" "}
                <span
                  className={`px-2.5 py-2 mr-5 rounded-full text-xs font-bold ${
                    statusColors[orderStatus] || "bg-gray-100 text-gray-800"
                  }`}
                >
                  {orderStatus}
                </span>
              </p>
              {orderDetail.orderStatus === "Đã giao cho đơn vị vận chuyển" && (
                <button
                  className={`bg-red-500 text-white font-semibold text-sm rounded-full py-2 px-4 hover:bg-red-600 ${
                    loading ? "opacity-50 cursor-not-allowed" : ""
                  }`}
                  onClick={handleDoneOrder}
                >
                  Đã nhận được đơn hàng
                </button>
              )}
              <p className="flex items-start gap-2 mb-2 w-full">
                <FontAwesomeIcon
                  icon={faTruck}
                  className="text-blue-500 fa-sm"
                />
                <span className="font-semibold flex-shrink-0">
                  Phương thức nhận hàng:
                </span>
                <span className="break-words">
                  <i>{deliveryMethod}</i>
                </span>
              </p>
            </div>
          </div>
        </div>

        <div className="bg-white p-4 rounded-lg shadow-lg">
          <h3 className="text-lg font-semibold mb-4 text-gray-700">
            Chi tiết sản phẩm
          </h3>
          {children.length > 0 ? (
            children.map((child) => (
              <div
                key={child.id}
                className="bg-gray-100 p-4 mt-4 rounded-lg shadow-sm "
              >
                <div className="flex flex-col md:flex-row gap-4">
                  <img
                    src={child.imgAvatarPath}
                    alt={child.productName}
                    className="w-full md:w-32 h-32 object-cover rounded"
                  />
                  <div className="flex-grow">
                    <h4 className="font-semibold text-lg mb-2 text-orange-500">
                      <Link to={`/product/${child.productCode}`}>
                        {child.productName}
                      </Link>
                    </h4>
                    <div className="grid grid-cols-2 gap-2">
                      <p>
                        <span className="font-semibold">Màu sắc:</span>{" "}
                        <i>{child.color}</i>
                      </p>
                      <p>
                        <span className="font-semibold">Số lượng:</span>{" "}
                        <i>{child.quantity}</i>
                      </p>
                      <p>
                        <span className="font-semibold">Tình trạng:</span>{" "}
                        <i>{child.condition}%</i>
                      </p>
                      <p>
                        <span className="font-semibold">Giá thuê:</span>{" "}
                        <i>{child.rentPrice.toLocaleString("vi-VN")}₫</i>
                      </p>

                      <p>
                        <span className="font-semibold">Kích thước:</span>{" "}
                        <i>{child.size}</i>
                      </p>
                      <p>
                        <span className="font-semibold">Thời gian thuê:</span>{" "}
                        <i>
                          {new Date(child.rentalStartDate).toLocaleDateString()}{" "}
                          - {new Date(child.rentalEndDate).toLocaleDateString()}
                        </i>
                      </p>
                    </div>
                  </div>
                </div>
                <div className="pt-2 flex justify-between items-center gap-4">
                  <div className="flex items-center gap-2">
                    <p className="flex items-center gap-1">
                      <span className="font-semibold text-black text-xl">
                        Tổng tiền:
                      </span>
                      <span>
                        <i className="text-gray-500 text-lg">
                          {child.subTotal.toLocaleString("vi-VN")}₫
                        </i>
                      </span>
                    </p>
                    <Tooltip
                      content={
                        <div className="w-90">
                          <Typography color="white" className="font-medium">
                            Tổng tiền:
                          </Typography>
                          <Typography
                            variant="small"
                            color="white"
                            className="font-normal opacity-80"
                          >
                            Tổng tiền được tính: Số lượng x Đơn giá bán x số
                            ngày thuê
                          </Typography>
                        </div>
                      }
                    >
                      <svg
                        xmlns="http://www.w3.org/2000/svg"
                        fill="none"
                        viewBox="0 0 24 24"
                        stroke="currentColor"
                        strokeWidth={2}
                        className="h-4 w-4 cursor-pointer text-blue-gray-500"
                      >
                        <path
                          strokeLinecap="round"
                          strokeLinejoin="round"
                          d="M11.25 11.25l.041-.02a.75.75 0 011.063.852l-.708 2.836a.75.75 0 001.063.853l.041-.021M21 12a9 9 0 11-18 0 9 9 0 0118 0zm-9-3.75h.008v.008H12V8.25z"
                        />
                      </svg>
                    </Tooltip>
                  </div>
                  <button
                    className={`rounded text-white p-2 font-semibold ${
                      orderStatus !== "Đã giao hàng"
                        ? "bg-orange-200 cursor-not-allowed rounded-full"
                        : "bg-orange-500 hover:bg-orange-600 rounded-full"
                    }`}
                    onClick={() => {
                      if (orderStatus === "Đã giao hàng") {
                        setSelectedChildOrder(child);
                        setExtendedShowModal(true);
                      }
                    }}
                    disabled={orderStatus !== "Đã giao hàngg"}
                  >
                    Yêu cầu gia hạn
                  </button>
                </div>
              </div>
            ))
          ) : (
            <div className="bg-gray-100 p-4 rounded-lg shadow-sm">
              {expandedId === id && (
                <div>
                  <input
                    type="date"
                    onChange={(e) => setSelectedDate(e.target.value)}
                    min={rentalEndDate.split("T")[0]}
                  />
                  <button onClick={() => handleExtendOrder(orderDetail)}>
                    Xác nhận ngày gia hạn
                  </button>
                </div>
              )}
              <div className="flex flex-col md:flex-row gap-4">
                <img
                  src={imgAvatarPath}
                  alt={orderDetail.productName}
                  className="w-full md:w-32 h-32 object-cover rounded"
                />
                <div className="flex-grow">
                  <h4 className="font-semibold text-lg mb-2 text-orange-500">
                    <Link to={`/product/${orderDetail.productCode}`}>
                      {orderDetail.productName}
                    </Link>
                  </h4>
                  <div className="grid grid-cols-2 gap-2">
                    <p>
                      <span className="font-semibold">Màu sắc:</span>{" "}
                      <i>{orderDetail.color}</i>
                    </p>
                    <p>
                      <span className="font-semibold">Số lượng:</span>{" "}
                      <i>{orderDetail.quantity}</i>
                    </p>
                    <p>
                      <span className="font-semibold">Tình trạng:</span>{" "}
                      <i>{orderDetail.condition}%</i>
                    </p>
                    <p>
                      <span className="font-semibold">Giá thuê:</span>{" "}
                      <i>{orderDetail.rentPrice.toLocaleString("vi-VN")}₫</i>
                    </p>
                    <p>
                      <span className="font-semibold">Kích thước:</span>{" "}
                      <i>{orderDetail.size}</i>
                    </p>
                    <p>
                      <span className="font-semibold">Thời gian thuê:</span>{" "}
                      <i>
                        {new Date(
                          orderDetail.rentalStartDate
                        ).toLocaleDateString()}{" "}
                        -{" "}
                        {new Date(
                          orderDetail.rentalEndDate
                        ).toLocaleDateString()}
                      </i>
                    </p>
                  </div>
                </div>
              </div>
              <div className="pt-2 flex justify-between items-center gap-4">
                {/* Phần thông tin Tổng tiền */}
                <div className="flex items-center gap-2">
                  <p className="flex items-center gap-1">
                    <span className="font-semibold text-black text-xl">
                      Tổng tiền:
                    </span>
                    <span>
                      <i className="text-gray-500 text-lg">
                        {orderDetail.subTotal.toLocaleString("vi-VN")}₫
                      </i>
                    </span>
                  </p>
                  <Tooltip
                    content={
                      <div className="w-90">
                        <Typography color="white" className="font-medium">
                          Tổng tiền:
                        </Typography>
                        <Typography
                          variant="small"
                          color="white"
                          className="font-normal opacity-80"
                        >
                          Tổng tiền được tính: Số lượng x Đơn giá bán x số ngày
                          thuê
                        </Typography>
                      </div>
                    }
                  >
                    <svg
                      xmlns="http://www.w3.org/2000/svg"
                      fill="none"
                      viewBox="0 0 24 24"
                      stroke="currentColor"
                      strokeWidth={2}
                      className="h-4 w-4 cursor-pointer text-blue-gray-500"
                    >
                      <path
                        strokeLinecap="round"
                        strokeLinejoin="round"
                        d="M11.25 11.25l.041-.02a.75.75 0 011.063.852l-.708 2.836a.75.75 0 001.063.853l.041-.021M21 12a9 9 0 11-18 0 9 9 0 0118 0zm-9-3.75h.008v.008H12V8.25z"
                      />
                    </svg>
                  </Tooltip>
                </div>

                {/* Button Yêu cầu gia hạn */}
                <button
                  className={`rounded text-white p-2 font-semibold ${
                    orderStatus !== "Đã giao hàng"
                      ? "bg-orange-200 cursor-not-allowed rounded-full"
                      : "bg-orange-500 hover:bg-orange-600 rounded-full"
                  }`}
                  onClick={() => {
                    if (orderStatus === "Đã giao hàng") {
                      setSelectedChildOrder(orderDetail);
                      setExtendedShowModal(true);
                    }
                  }}
                  disabled={orderStatus !== "Đã giao hàng"}
                >
                  Yêu cầu gia hạn
                </button>
              </div>
            </div>
          )}
        </div>
        <div className="bg-white p-6 rounded-lg shadow-lg mt-6">
          <p className="text-xl flex justify-between">
            <b>Tạm tính: </b>
            <i>{orderDetail.subTotal.toLocaleString("vi-VN")}₫</i>
          </p>
          <p className="flex justify-between">
            <b className="text-xl py-2 ">Phí vận chuyển: </b>
            <p className="text-sm py-2 ">
              {orderDetail.transportFee || "2Sport sẽ liên hệ và thông báo sau"}
            </p>
          </p>
          <p className="text-xl flex justify-between">
            <b>Thành tiền: </b>
            <i className="text-orange-500 font-bold">
              {orderDetail.totalAmount.toLocaleString("vi-VN")}₫
            </i>
          </p>
        </div>

        {showModal && (
          <div className="fixed inset-0 flex items-center justify-center bg-black bg-opacity-50 z-50">
            <div className="bg-white p-6 rounded-md shadow-lg w-96">
              <h2 className="text-lg font-semibold">Xác nhận hủy đơn hàng</h2>
              <p className="mb-4 text-sm">
                <i>Bạn có chắc rằng hủy đơn hàng này không?</i>
              </p>
              <textarea
                className="w-full border rounded-md p-2 mb-4"
                rows="4"
                placeholder="Vui lòng nhập lý do hủy đơn hàng"
                value={reason}
                onChange={(e) => setReason(e.target.value)}
              ></textarea>
              <div className="flex justify-end space-x-4">
                <button
                  className="bg-gray-500 text-white py-2 px-4 rounded-md"
                  onClick={() => setShowModal(false)}
                >
                  Đóng
                </button>
                <button
                  className="bg-red-500 text-white py-2 px-4 rounded-md"
                  onClick={handleCancelOrder}
                >
                  Xác nhận
                </button>
              </div>
            </div>
          </div>
        )}
        {showExtendedModal && selectedChildOrder && (
          <div className="fixed inset-0 flex items-center justify-center bg-black bg-opacity-50 z-50">
            <div className="bg-white p-6 rounded-md shadow-lg w-2/5">
              <h2 className="text-lg font-semibold">Gửi yêu cầu gia hạn</h2>
              <p className="mb-4 text-sm">
                <i>Tóm tắt thông tin sản phẩm</i>
              </p>
              <div className="flex items-center space-x-4">
                <img
                  src={selectedChildOrder.imgAvatarPath || "default-image.jpg"}
                  alt="Order"
                  className="w-24 h-24 object-contain rounded"
                />
                <div>
                  <h3 className="font-medium text-base">
                    {selectedChildOrder.productName}
                  </h3>
                  <p className="text-sm text-gray-500">
                    Màu sắc: {selectedChildOrder.color} - Kích thước:{" "}
                    {selectedChildOrder.size} - Tình trạng:{" "}
                    {selectedChildOrder.condition}%
                  </p>
                  <p className="font-medium text-base text-rose-700">
                    Giá thuê:{" "}
                    {new Intl.NumberFormat("vi-VN", {
                      style: "currency",
                      currency: "VND",
                    }).format(selectedChildOrder.rentPrice)}
                  </p>
                  <p className="font-medium text-sm">
                    Số lượng: {selectedChildOrder.quantity}
                  </p>
                </div>
              </div>
              <div className="flex items-start justify-between space-x-6">
                {/* Cột 1 */}
                <div className="flex-1">
                  <p className="pt-2 mb-2 text-sm">
                    <i>Tóm tắt thời gian thuê</i>
                  </p>
                  <p className="text-sm text-gray-500">
                    Ngày bắt đầu:{" "}
                    {new Date(
                      selectedChildOrder.rentalStartDate
                    ).toLocaleString("vi-VN", {
                      day: "2-digit",
                      month: "2-digit",
                      year: "numeric",
                    })}
                  </p>
                  <p className="text-sm text-gray-500">
                    Ngày kết thúc:{" "}
                    {new Date(selectedChildOrder.rentalEndDate).toLocaleString(
                      "vi-VN",
                      {
                        day: "2-digit",
                        month: "2-digit",
                        year: "numeric",
                      }
                    )}
                  </p>
                </div>

                {/* Cột 2 */}
                <div className="flex-1">
                  <p className="pt-2 mb-2 text-sm">
                    <i>Chọn ngày gia hạn</i>
                  </p>
                  <Input
                    label="ngày gia hạn"
                    type="date"
                    min={getTomorrowDate()}
                    value={selectedDate}
                    onChange={handleExtensionDateChange}
                    className="border rounded px-4 py-2 w-full"
                  />
                </div>
              </div>

              <div className="flex justify-end space-x-4 mt-4">
                <button
                  className="bg-gray-500 text-white py-2 px-4 rounded-md"
                  onClick={() => setExtendedShowModal(false)}
                >
                  Đóng
                </button>
                <button
                  className="bg-red-500 text-white py-2 px-4 rounded-md"
                  onClick={() => handleExtendOrder(selectedChildOrder)}
                >
                  Gửi yêu cầu
                </button>
              </div>
            </div>
          </div>
        )}
      </div>
    </div>
  );
}
