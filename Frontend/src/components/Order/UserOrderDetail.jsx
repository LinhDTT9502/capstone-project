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
  faTruck,
} from "@fortawesome/free-solid-svg-icons";
import { Button } from "@material-tailwind/react";
import { submitReview } from "../../services/reviewService";
import StarRating from "../Product/StarRating";

export default function UserOrderDetail() {
  const { orderCode } = useParams();
  const navigate = useNavigate();
  const [orderDetail, setOrderDetail] = useState(null);
  const [isLoading, setIsLoading] = useState(true);
  const [error, setError] = useState(null);
  const [showModal, setShowModal] = useState(false);
  const [reason, setReason] = useState("");
  const [loading, setLoading] = useState(false);

  const [showReviewModal, setShowReviewModal] = useState(false);
  const [reviewData, setReviewData] = useState({ star: 5, review: "" });
  const [currentProduct, setCurrentProduct] = useState(null);


  const statusColors = {
    "Chờ xử lý": "bg-yellow-100 text-yellow-800",
    "Đã xác nhận đơn": "bg-blue-100 text-blue-800",
    "Đã thanh toán": "bg-green-100 text-green-800",
    "Đang xử lý": "bg-purple-100 text-purple-800",
    "Đã giao hàng": "bg-indigo-100 text-indigo-800",
    "Đã giao cho đơn vị vận chuyển": "bg-indigo-100 text-indigo-800",
    "Đã từ chối": "bg-red-100 text-red-800",
    "Đã hủy": "bg-red-200 text-red-900",
    "Đã hoàn thành": "bg-orange-100 text-orange-800",
  };

  const paymentStatusColors = {
    "Đang chờ thanh toán": "text-yellow-800",
    "Đã đặt cọc": "text-blue-800",
    "Đã thanh toán": "text-green-800",
    "Đã hủy": "btext-red-800",
  };

  const fetchOrderDetail = async () => {
    try {
      const token = localStorage.getItem("token");
      const response = await axios.get(
        `https://capstone-project-703387227873.asia-southeast1.run.app/api/SaleOrder/get-order-by-code?orderCode=${orderCode}`,
        {
          headers: {
            accept: "*/*",
            Authorization: `Bearer ${token}`,
          },
        }
      );
      console.log(response);

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
      const { id: saleOrderId } = orderDetail;
      const response = await axios.post(
        `https://capstone-project-703387227873.asia-southeast1.run.app/api/SaleOrder/request-cancel/${saleOrderId}?reason=${encodeURIComponent(
          reason
        )}`,
        null,
        {
          headers: {
            accept: "*/*",
          },
        }
      );
      fetchOrderDetail();
      alert("Bạn đã hủy đơn hàng thành công");
      setShowModal(false);
    } catch (error) {
      console.error("Error cancel order:", error);
      alert("Failed to cancel the order. Please try again.");
    }
  };

  if (isLoading)
    return (
      <div className="flex justify-center items-center h-screen">
        <div className="animate-spin rounded-full h-32 w-32 border-t-2 border-b-2 border-blue-500"></div>
      </div>
    );
  if (error)
    return <div className="text-center text-red-500 mt-4">Error: {error}</div>;

  const {
    saleOrderId,
    fullName,
    email,
    contactPhone,
    address,
    saleOrderCode,
    paymentStatus,
    orderStatus,
    deliveryMethod,
    saleOrderDetailVMs,
  } = orderDetail;
  console.log(orderDetail);

  const products = orderDetail?.saleOrderDetailVMs?.$values || [];
  // Hàm render nút "Thanh toán"
  const renderPaymentButton = () => {
    if (
      orderDetail.paymentStatus === "Đang chờ thanh toán" &&
      orderDetail.deliveryMethod !== "HOME_DELIVERY"
    ) {
      return (
        <Button
          className="bg-green-700 text-white text-sm rounded-full py-2 px-4 w-40 mt-4"
          onClick={() =>
            navigate("/checkout", { state: { selectedOrder: orderDetail } })
          }
        >
          Thanh Toán
        </Button>
      );
    }
  };

  const handleDoneOrder = async () => {
    if (!orderDetail || !orderDetail.id) {
      alert("Không tìm thấy thông tin đơn hàng để hoàn tất.");
      return;
    }
  
    const confirmDone = window.confirm("Bạn có chắc chắn muốn hoàn tất đơn hàng này không?");
  
    if (!confirmDone) {
      return;
    }
  
    const newStatus = 5;
  
    try {
      const response = await axios.put(
        `https://capstone-project-703387227873.asia-southeast1.run.app/api/SaleOrder/update-order-status/${orderDetail.id}?status=${newStatus}`,
        {},
        {
          headers: {
            accept: "*/*",
          },
        }
      );
  
      if (response && response.data.isSuccess) {
        alert("Đơn hàng của bạn đã được hoàn tất thành công.");
        setShowReviewModal(true);
        fetchOrderDetail();
      } else {
        alert("Không thể hoàn tất đơn hàng. Vui lòng thử lại sau.");
      }
    } catch (error) {
      console.error("Error updating order status:", error);
      alert("Đã xảy ra lỗi khi hoàn tất đơn hàng. Vui lòng thử lại sau.");
    }
  };

  const handleSubmitReview = async () => {
    if (!currentProduct) return;

    try {
      await submitReview(currentProduct.productCode, {
        star: reviewData.star,
        review: reviewData.review,
        status: true,
      });
      alert("Cảm ơn bạn đã đánh giá sản phẩm!");
      setShowReviewModal(false);
    } catch (error) {
      console.error("Error submitting review:", error);
      alert("Gửi đánh giá thất bại. Vui lòng thử lại.");
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
          <div className="flex flex-col">
            {" "}
            {/* Nút thanh toán */}
            {renderPaymentButton()}
          </div>
        </div>

        <div className="bg-white p-6 rounded-lg shadow-lg mb-6">
          <h2 className="text-2xl font-bold mb-4 text-gray-800">
            Chi tiết đơn hàng -{" "}
            <span className="text-orange-500">#{saleOrderCode}</span>
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
              <p className="flex items-start gap-2 mb-2">
                <FontAwesomeIcon
                  icon={faMapMarkerAlt}
                  className="text-blue-500"
                />
                <span className="font-semibold flex-shrink-0">Địa chỉ:</span>
                <span className="break-words">{address}</span>
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
                <i>{saleOrderCode}</i>
              </p>
              <p className="flex items-start gap-2 mb-2 w-full">
                <FontAwesomeIcon icon={faTruck} className="text-blue-500" />
                <span className="font-semibold flex-shrink-0">
                  Phương thức nhận hàng:
                </span>
                <span className="break-words">{deliveryMethod}</span>
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
            </div>
          </div>
        </div>
        <div className="bg-gray-50 p-4 rounded-lg shadow-sm">
          {products.map((product) => (
            <div
              key={product.productId}
              className="bg-gray-50 p-4 mb-4 rounded-lg shadow-sm"
            >
              {console.log(product)}

              <div className="flex flex-col md:flex-row gap-4">
                <img
                  src={product.imgAvatarPath}
                  alt={product.productName}
                  className="w-full md:w-32 h-32 object-cover rounded"
                />
                <div className="flex-grow">
                  <h4 className="font-semibold text-lg mb-2 text-orange-500">
                    {product.productName}
                  </h4>
                  <div className="grid grid-cols-2 gap-2">
                    <p>
                      <span className="font-semibold">Màu sắc:</span>{" "}
                      {product.color}
                    </p>
                    <p>
                      <span className="font-semibold">Kích thước:</span>{" "}
                      {product.size}
                    </p>
                    <p>
                      <span className="font-semibold">Tình trạng:</span>{" "}
                      {product.condition}%
                    </p>
                    <p>
                      <span className="font-semibold">Số lượng:</span>{" "}
                      {product.quantity}
                    </p>
                    <p>
                      <span className="font-semibold">Giá bán:</span>{" "}
                      <i>{product.unitPrice.toLocaleString("Vi-VN")}₫</i>
                    </p>
                    <p>
                      <span className="font-semibold">Tổng tiền:</span>{" "}
                      <i>{product.totalAmount.toLocaleString("Vi-VN")}₫</i>
                    </p>
                  </div>
                  <div className="flex space-x-4 pt-4 justify-end">
                    {(paymentStatus === "Đang chờ thanh toán" ||
                      paymentStatus === "N/A") &&
                      orderDetail.deliveryMethod !== "HOME_DELIVERY" && (
                        <button
                          className="bg-purple-500 font-bold text-white text-sm rounded-full py-2 px-4 hover:bg-purple-600"
                          onClick={() =>
                            navigate("/rental-checkout", {
                              state: { selectedOrder: orderDetail },
                            })
                          }
                        >
                          Thanh toán
                        </button>
                      )}
                    {orderDetail.orderStatus === "Chờ xử lý" && (
                      <button
                        className="bg-red-500 text-white font-bold text-sm rounded-full py-2 px-4 hover:bg-red-600"
                        onClick={() => setShowModal(true)}
                      >
                        Hủy đơn hàng
                      </button>
                    )}
                    {orderDetail.orderStatus === "Đã giao cho đơn vị vận chuyển" && (
                      <button
                      className={`bg-red-500 text-white font-bold text-sm rounded-full py-2 px-4 hover:bg-red-600 ${
                        loading ? "opacity-50 cursor-not-allowed" : ""
                      }`}
                      onClick={handleDoneOrder}
                    >
                        Đã nhận được đơn hàng
                      </button>
                    )}
                  </div>
                </div>
              </div>
            </div>
          ))}
        </div>
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
      {/* Review Modal */}
      {showReviewModal && (
        <div className="fixed inset-0 bg-black bg-opacity-50 flex items-center justify-center z-50">
          <div className="bg-white p-6 rounded-lg shadow-xl w-96 max-w-full">
            <h3 className="text-2xl font-semibold mb-4 text-gray-800">Đánh giá sản phẩm</h3>
            <div className="mb-4">
              <label className="block text-sm font-medium text-gray-700 mb-2">Đánh giá của bạn</label>
              <StarRating
                rating={reviewData.star}
                onRatingChange={(newRating) => setReviewData({ ...reviewData, star: newRating })}
              />
            </div>
            <div className="mb-4">
              <label htmlFor="review" className="block text-sm font-medium text-gray-700 mb-2">
                Nhận xét
              </label>
              <textarea
                id="review"
                rows="4"
                className="w-full px-3 py-2 text-gray-700 border rounded-lg focus:outline-none focus:border-blue-500"
                value={reviewData.review}
                onChange={(e) => setReviewData({ ...reviewData, review: e.target.value })}
                placeholder="Chia sẻ trải nghiệm của bạn về sản phẩm này..."
              ></textarea>
            </div>
            <div className="flex justify-end space-x-2">
              <Button 
                color="gray" 
                onClick={() => setShowReviewModal(false)}
                className="px-4 py-2 rounded-lg"
              >
                Hủy
              </Button>
              <button
                onClick={handleSubmitReview}
                disabled={loading}
                className={`bg-blue-500 text-white px-4 py-2 rounded-lg ${
                  loading
                    ? "opacity-50 cursor-not-allowed"
                    : "hover:bg-blue-600"
                }`}
              >
                {loading ? "Đang tiến hành..." : "Gửi đánh giá"}
              </button>
            </div>
          </div>
        </div>
      )}
    </div>
  );
}
