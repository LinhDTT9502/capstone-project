import React, { useState } from "react";
import { useLocation, Link, useNavigate } from "react-router-dom";
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
} from "@fortawesome/free-solid-svg-icons";

const statusColors = {
  "Chờ xử lý": "bg-yellow-100 text-yellow-800",
  "Đã xác nhận đơn": "bg-orange-100 text-orange-800",
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

const GuestOrderDetail = () => {
  const { state } = useLocation();
  const order = state?.order;
  const navigate = useNavigate();

  const [showModal, setShowModal] = useState(false);
  const [reason, setReason] = useState("");
  const [loading, setLoading] = useState(false);

  if (!order) {
    return (
      <div className="flex justify-center items-center min-h-screen bg-gray-100">
        <div className="bg-white p-8 rounded-lg shadow-md text-center">
          <FontAwesomeIcon
            icon={faShoppingCart}
            className="text-6xl text-red-500 mb-4"
          />
          <h2 className="text-2xl font-semibold text-gray-800 mb-4">
            Order not found
          </h2>
          <Link
            to="/guest-order"
            className="inline-flex items-center justify-center px-4 py-2 border border-transparent text-base font-medium rounded-md text-white bg-orange-600 hover:bg-orange-700 transition duration-150 ease-in-out"
          >
            <FontAwesomeIcon icon={faArrowLeft} className="mr-2" />
            Back to Orders
          </Link>
        </div>
      </div>
    );
  }

  const {
    id,
    saleOrderCode,
    fullName,
    email,
    contactPhone,
    address,
    paymentStatus,
    orderStatus,
    deliveryMethod,
    saleOrderDetailVMs,
    createdAt,
    totalAmount,
  } = order;
  console.log(order);
  const formatCurrency = (amount) => {
    return new Intl.NumberFormat("vi-VN", {
      style: "currency",
      currency: "VND",
    }).format(amount);
  };

  const handleCancelOrder = async () => {
    if (!reason.trim()) {
      alert("Vui lòng nhập lý do hủy đơn hàng.");
      return;
    }
    try {
      setLoading(true);
      const response = await axios.post(
        `https://capstone-project-703387227873.asia-southeast1.run.app/api/SaleOrder/request-cancel/${id}?reason=${encodeURIComponent(
          reason
        )}`,
        null,
        { headers: { accept: "*/*" } }
      );

      if (response.data.isSuccess) {
        alert("Bạn đã hủy đơn hàng thành công");
        setShowModal(false);
        navigate("/guest-order");
      } else {
        alert("Không thể hủy đơn hàng. Vui lòng thử lại sau.");
      }
    } catch (error) {
      alert("Đã xảy ra lỗi khi hủy đơn hàng. Vui lòng thử lại.");
    } finally {
      setLoading(false);
    }
  };

  const renderPaymentButton = () => {
    if (paymentStatus === "N/A" && deliveryMethod !== "HOME_DELIVERY") {
      return (
        <button
          className="bg-green-500 text-white font-bold text-sm rounded-full py-2 px-4 hover:bg-green-600"
          onClick={() =>
            navigate("/checkout", { state: { selectedOrder: order } })
          }
        >
          Thanh toán
        </button>
      );
    }
    return null;
  };

  return (
    <div className="container mx-auto px-4 py-20">
      <div className="max-w-4xl mx-auto">
        <div className="flex justify-between items-center mb-6">
          <Link
            to="/guest-order"
            className="flex items-center text-orange-600 hover:text-orange-800"
          >
            <FontAwesomeIcon icon={faArrowLeft} className="mr-2" />
            Quay lại
          </Link>

          <h1 className="text-3xl font-bold text-gray-800">
            Chi tiết đơn hàng
          </h1>
        </div>

        <div className="bg-white rounded-lg shadow-md overflow-hidden mb-8">
          <div className="p-6">
            <div className="flex justify-between items-start mb-6">
              <h2 className="text-2xl font-semibold text-gray-800">
                Mã đơn hàng{" "}
                <span className="text-orange-600">#{saleOrderCode}</span>
              </h2>
              <div>
                <span
                  className={`inline-block px-3 py-1 rounded-full text-xs font-semibold ${statusColors[orderStatus]}`}
                >
                  {orderStatus}
                </span>
                <p
                  className={`text-sm font-medium mt-2 ${paymentStatusColors[paymentStatus]}`}
                >
                  {paymentStatus}
                </p>
              </div>
            </div>

            <div className="grid md:grid-cols-2 gap-6">
              <div>
                <h3 className="text-lg font-semibold text-gray-700 mb-3">
                  Thông tin khách hàng
                </h3>
                <p className="flex items-center mb-2">
                  <FontAwesomeIcon
                    icon={faUser}
                    className="mr-2 text-orange-500"
                  />
                  <span className="font-medium mr-2">Tên:</span> {fullName}
                </p>
                <p className="flex items-center mb-2">
                  <FontAwesomeIcon
                    icon={faEnvelope}
                    className="mr-2 text-orange-500"
                  />
                  <span className="font-medium mr-2">Email:</span> {email}
                </p>
                <p className="flex items-center mb-2">
                  <FontAwesomeIcon
                    icon={faPhone}
                    className="mr-2 text-orange-500"
                  />
                  <span className="font-medium mr-2">Số điện thoại:</span>{" "}
                  {contactPhone}
                </p>

                <p className="flex items-start mb-2">
                  <FontAwesomeIcon
                    icon={faMapMarkerAlt}
                    className="mr-2 mt-1 text-orange-500"
                  />
                  <span className="font-medium mr-2">Địa chỉ: </span>
                  <span className="ml-1">{address || "N/A"}</span>
                </p>
              </div>
              <div>
                <h3 className="text-lg font-semibold text-gray-700 mb-3">
                  Thông tin đơn hàng
                </h3>
                {/* <p className="flex items-center mb-2">
                  <FontAwesomeIcon icon={faShoppingCart} className="mr-2 text-orange-500" />
                  <span className="font-medium mr-2">ID đơn hàng:</span> {id}
                </p> */}
                <p className="flex items-center mb-2">
                  <FontAwesomeIcon
                    icon={faCalendarAlt}
                    className="mr-2 text-orange-500"
                  />
                  <span className="font-medium mr-2">Ngày đặt hàng:</span>{" "}
                  {new Date(createdAt).toLocaleDateString()}
                </p>
                <p className="flex items-center mb-2">
                  <FontAwesomeIcon
                    icon={faTruck}
                    className="mr-2 text-orange-500"
                  />
                  <span className="font-medium mr-2">Hình thức giao hàng:</span>{" "}
                  {deliveryMethod}
                </p>
                <p className="flex items-center mb-2">
                  <FontAwesomeIcon
                    icon={faMoneyBillWave}
                    className="mr-2 text-orange-500"
                  />
                  <span className="font-medium mr-2">Tổng tiền:</span>{" "}
                  {formatCurrency(totalAmount)}
                </p>
              </div>
            </div>
            <div className="flex justify-end items-center gap-3">
              {orderStatus === "Chờ xử lý" && (
                <button
                  className="bg-red-500 text-white font-bold text-sm rounded-full py-2 px-4 hover:bg-red-600"
                  onClick={() => setShowModal(true)}
                >
                  Hủy đơn hàng
                </button>
              )}
              {renderPaymentButton()}
              {orderStatus === "Đã giao hàng" && (
                <button
                  className="bg-green-500 text-white font-bold text-sm rounded-full py-2 px-4 hover:bg-green-600"
                  onClick={handleCompleteOrder}
                >
                  Đã nhận đơn hàng
                </button>
              )}
            </div>
          </div>
        </div>

        <div className="bg-white rounded-lg shadow-md overflow-hidden">
          <div className="p-6">
            <h3 className="text-xl font-semibold text-gray-800 mb-4">
              Chi tiết sản phẩm
            </h3>
            <div className="space-y-6">
              {saleOrderDetailVMs.$values.map((item) => (
                <div
                  key={item.productId}
                  className="flex items-center space-x-4 border-b pb-4 last:border-b-0 last:pb-0"
                >
                  <img
                    src={item.imgAvatarPath || "/placeholder.png"}
                    alt={item.productName}
                    className="w-24 h-24 object-cover rounded"
                  />
                  <div className="flex-grow">
                    <h4 className="font-semibold text-lg text-gray-800">
                      {item.productName}
                    </h4>
                    <p className="text-sm text-gray-600">
                      Màu sắc {item.color} | Size: {item.size} | Tình trạng{" "}
                      {item.condition}%
                    </p>
                    <p className="text-sm text-gray-600">
                      Số lượng: {item.quantity} | Giá bán:{" "}
                      {formatCurrency(item.unitPrice)}
                    </p>
                    <p className="text-sm font-medium text-gray-800 mt-1">
                      Tạm tính: {formatCurrency(item.totalAmount)}
                    </p>
                  </div>
                </div>
              ))}
            </div>
          </div>
        </div>
        {/* Cancel Order Modal */}
        {showModal && (
          <div className="fixed inset-0 flex items-center justify-center bg-black bg-opacity-50 z-50">
            <div className="bg-white p-6 rounded-md shadow-lg w-96">
              <h2 className="text-lg font-semibold">Xác nhận hủy đơn hàng</h2>
              <p className="mb-4 text-sm">
                <i>Bạn có chắc rằng muốn hủy đơn hàng này không?</i>
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
      </div>
    </div>
  );
};

export default GuestOrderDetail;
