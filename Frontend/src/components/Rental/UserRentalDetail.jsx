import React, { useState, useEffect } from "react";
import { useParams, useNavigate } from "react-router-dom";
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
} from "@fortawesome/free-solid-svg-icons";

export default function UserRentalDetail() {
  const { orderCode } = useParams();
  const navigate = useNavigate();
  const [orderDetail, setOrderDetail] = useState(null);
  const [isLoading, setIsLoading] = useState(true);
  const [error, setError] = useState(null);
  const [selectedDate, setSelectedDate] = useState(null);
  const [expandedId, setExpandedId] = useState(null);
  const [showModal, setShowModal] = useState(false);
  const [reason, setReason] = useState("");


  const fetchOrderDetail = async () => {
    try {
      const token = localStorage.getItem("token");
      const response = await axios.get(
        `https://capstone-project-703387227873.asia-southeast1.run.app/api/RentalOrder/get-rental-order-by-orderCode?orderCode=${orderCode}`,
        {
          headers: {
            accept: "*/*",
            Authorization: `Bearer ${token}`,
          },
        }
      );

      if (response.data.isSuccess) {
        setOrderDetail(response.data.data);
      } else {
        setError("Failed to fetch order details");
      }
    } catch (err) {
      setError(
        err.message || "An error occurred while fetching order details"
      );
    } finally {
      setIsLoading(false);
    }
  };

  useEffect(() => {
    fetchOrderDetail();
  }, [orderCode]);

  if (isLoading)
    return (
      <div className="flex justify-center items-center h-screen">
        <div className="animate-spin rounded-full h-32 w-32 border-t-2 border-b-2 border-blue-500"></div>
      </div>
    );
  if (error)
    return <div className="text-center text-red-500 mt-4">Error: {error}</div>;

  const {
    id,
    fullName,
    email,
    contactPhone,
    address,
    rentalOrderCode,
    childOrders,
    isExtended,
    productName,
    rentPrice,
    rentalStartDate,
    rentalEndDate,
    totalAmount,
    orderStatus,
    paymentStatus,
  } = orderDetail;
  console.log(orderDetail);


  const children = childOrders?.$values || [];

  const handleExtendOrder = async (order) => {
    console.log(order);
    console.log(id);
    const payload = id === order.id
    ? { parentOrderId: id, childOrderId: null }
    : { parentOrderId: id, childOrderId: order.id };
console.log(payload);
  
    if (!selectedDate) {
      alert("Please select a valid date before extending the order.");
      return;
    }
  
    const selectedDateObj = new Date(selectedDate); // Convert selectedDate to a Date object
    const rentalEndDateObj = new Date(order.rentalEndDate); // Convert rentalEndDate to a Date object
  
    const extensionDays = Math.ceil(
      (selectedDateObj - rentalEndDateObj) / (1000 * 60 * 60 * 24)
    );
  
    console.log(extensionDays);
  
    // Determine parentOrderId and childOrderId based on the condition

  
    try {
      const response = await axios.post(
        `https://capstone-project-703387227873.asia-southeast1.run.app/api/RentalOrder/request-extension`,
        {
          ...payload, // Spread the payload object into the request body
          extensionDays: extensionDays,
        },
        {
          headers: {
            accept: "*/*",
          },
        }
      );
      alert("Bạn đã gia hạn thành công");
    } catch (error) {
      console.error("Error extending order:", error);
      alert("Failed to extend the order. Please try again.");
    }
  };
  

  const handleCancelOrder = async () => {
    if (!reason.trim()) {
      alert("Vui lòng nhập lý do hủy đơn hàng.");
      return;
    }

    const confirmCancel = window.confirm(
      "Bạn có chắc chắn muốn hủy đơn hàng này không?"
    );

    if (!confirmCancel) {
      return;
    }

    try {
      const response = await axios.post(
        `https://capstone-project-703387227873.asia-southeast1.run.app/api/RentalOrder/request-cancel/${id}?reason=${encodeURIComponent(
          reason
        )}`,
        null, // No request body needed
        {
          headers: {
            accept: "*/*",
          },
        }
      );
      fetchOrderDetail();
      alert("Bạn đã hủy đơn hàng thành công");
      setShowModal(false); // Close the modal after success
    } catch (error) {
      console.error("Error cancel order:", error);
      alert("Failed to cancel the order. Please try again.");
    }
  };

  return (
    <div className="container mx-auto p-4 min-h-screen">
      <div className="max-w-4xl mx-auto">
        <div className="flex justify-between items-center mb-6">
          <button
            onClick={() => navigate(-1)}
            className="flex items-center gap-2 text-blue-500 hover:text-blue-700"
          >
            <FontAwesomeIcon icon={faArrowLeft} />
            Quay lại
          </button>
          {orderDetail.orderStatus === "Chờ xử lý" &&
            <button
              className="bg-red-500 text-white text-sm rounded-full py-2 px-4"
              onClick={() => setShowModal(true)}
            >
              Hủy đơn hàng
            </button>
          }
          {showModal && (
            <div className="fixed inset-0 flex items-center justify-center bg-black bg-opacity-50 z-50">
              <div className="bg-white p-6 rounded-md shadow-lg w-96">
                <h2 className="text-lg font-semibold mb-4">Xác nhận hủy đơn hàng</h2>
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
                    Xác nhận hủy
                  </button>
                </div>
              </div>
            </div>
          )}
          {orderDetail.paymentStatus === "Đang chờ thanh toán" &&
            orderDetail.deliveryMethod !== "HOME_DELIVERY" && (
              <button
                className="bg-purple-500 text-white text-sm rounded-full py-2 px-4 hover:bg-purple-600"
                onClick={() =>
                  navigate("/rental-checkout", {
                    state: { selectedOrder: orderDetail },
                  })
                }
              >
                Tiến hành thanh toán
              </button>
            )}
        </div>

        <div className="bg-white p-6 rounded-lg shadow-lg mb-6">
          <h2 className="text-2xl font-bold mb-4 text-gray-800">
            Chi tiết đơn hàng
          </h2>
          <hr className="mb-5" />
          <div className="grid md:grid-cols-2 gap-6">
            <div>
              <h3 className="text-lg font-semibold mb-2 text-gray-700">
                Thông tin khách hàng
              </h3>
              <p className="flex items-center gap-2 mb-2">
                <FontAwesomeIcon icon={faUser} className="text-blue-500" />
                <span className="font-semibold">Tên:</span> {fullName}
              </p>
              <p className="flex items-center gap-2 mb-2">
                <FontAwesomeIcon icon={faEnvelope} className="text-blue-500" />
                <span className="font-semibold">Email:</span> {email}
              </p>
              <p className="flex items-center gap-2 mb-2">
                <FontAwesomeIcon icon={faPhone} className="text-blue-500" />
                <span className="font-semibold">Số điện thoại:</span>{" "}
                {contactPhone}
              </p>
              <p className="flex items-center gap-2 mb-2">
                <FontAwesomeIcon
                  icon={faMapMarkerAlt}
                  className="text-blue-500"
                />
                <span className="font-semibold">Địa chỉ:</span> {address}
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
                {rentalOrderCode}
              </p>
              <p className="flex items-center gap-2 mb-2">
                <FontAwesomeIcon
                  icon={faMoneyBillWave}
                  className="text-blue-500"
                />
                <span className="font-semibold">Tình trạng thanh toán:</span>{" "}
                {paymentStatus}
              </p>
              <p className="flex items-center gap-2 mb-2">
                <FontAwesomeIcon
                  icon={faCalendarAlt}
                  className="text-blue-500"
                />
                <span className="font-semibold">Tình trạng đơn hàng:</span>{" "}
                {orderStatus}
              </p>
            </div>
          </div>
        </div>

        <div className="bg-white p-6 rounded-lg shadow-lg">
          <h3 className="text-lg font-semibold mb-4 text-gray-700">
            Sản phẩm chi tiết
          </h3>
          {children.length > 0 ? (
            children.map((child, index) => (
              <div
                key={child.id}
                className="bg-gray-50 p-4 mb-4 rounded-lg shadow-sm"
              >
                {!child.isExtended && orderStatus ==="Đã giao hàng" &&
                  <button
                    className="bg-orange-500 rounded text-white p-2"
                    onClick={() => setExpandedId(expandedId === child.id ? null : child.id)}>
                    Gia hạn đơn thuê
                  </button>}
                  {expandedId === child.id && (
                  <div>
                    <input
                      type="date"
                      onChange={(e) => setSelectedDate(e.target.value)}
                      min={child.rentalEndDate.split("T")[0]}
                    />
                    <button onClick={() => handleExtendOrder(child)}>
                      Xác nhận ngày gia hạn
                    </button>
                  </div>
                )}
                <div className="flex flex-col md:flex-row gap-4">
                  <img
                    src={child.imgAvatarPath}
                    alt={child.productName}
                    className="w-full md:w-32 h-32 object-cover rounded"
                  />
                  <div className="flex-grow">
                    <h4 className="font-semibold text-lg mb-2">
                      {child.productName}
                    </h4>
                    <div className="grid grid-cols-2 gap-2">
                      <p>
                        <span className="font-semibold">Color:</span>{" "}
                        {child.color}
                      </p>
                      <p>
                        <span className="font-semibold">Size:</span>{" "}
                        {child.size}
                      </p>
                      <p>
                        <span className="font-semibold">Condition:</span>{" "}
                        {child.condition}%
                      </p>
                      <p>
                        <span className="font-semibold">Quantity:</span>{" "}
                        {child.quantity}
                      </p>
                      <p>
                        <span className="font-semibold">Rent Price:</span>{" "}
                        {child.rentPrice} ₫
                      </p>
                      <p>
                        <span className="font-semibold">Total:</span>{" "}
                        {child.totalAmount} ₫
                      </p>
                    </div>
                    <p className="mt-2">
                      <span className="font-semibold">Rental Period:</span>{" "}
                      {new Date(child.rentalStartDate).toLocaleDateString()} -{" "}
                      {new Date(child.rentalEndDate).toLocaleDateString()}
                    </p>
                  </div>
                </div>
              </div>
            ))
          ) : (
            <div className="bg-gray-50 p-4 rounded-lg shadow-sm">
              {!isExtended &&
                <button
                  className="bg-orange-500 rounded text-white p-2"
                  onClick={() => setExpandedId(expandedId === id ? null : id)}>
                  Gia hạn đơn thuê
                </button>}
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
                  src={orderDetail.imgAvatarPath || "/placeholder.jpg"}
                  alt={productName}
                  className="w-full md:w-32 h-32 object-cover rounded"
                />
                <div className="flex-grow">
                  <h4 className="font-semibold text-lg mb-2">{productName}</h4>
                  <p>
                    <span className="font-semibold">Rent Price:</span>{" "}
                    {rentPrice || "N/A"} ₫
                  </p>
                  <p className="mt-2">
                    <span className="font-semibold">Rental Period:</span>{" "}
                    {new Date(rentalStartDate).toLocaleDateString()} -{" "}
                    {new Date(rentalEndDate).toLocaleDateString()}
                  </p>
                  <p>
                    <span className="font-semibold">Total:</span>{" "}
                    {totalAmount || "N/A"} ₫
                  </p>
                </div>
              </div>
            </div>
          )}
        </div>
      </div>
    </div>
  );
}
