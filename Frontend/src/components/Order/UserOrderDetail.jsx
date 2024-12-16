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
  faTruck
} from "@fortawesome/free-solid-svg-icons";
import { Button } from "@material-tailwind/react";

export default function UserOrderDetail() {
  const { orderCode } = useParams();
  const navigate = useNavigate();
  const [orderDetail, setOrderDetail] = useState(null);
  const [isLoading, setIsLoading] = useState(true);
  const [error, setError] = useState(null);


  const statusColors = {
    "Chờ xử lý": "bg-yellow-100 text-yellow-800",
    "Đã xác nhận đơn": "bg-blue-100 text-blue-800",
    "Đã thanh toán": "bg-green-100 text-green-800",
    "Đang xử lý": "bg-purple-100 text-purple-800",
    "Đã giao hàng": "bg-indigo-100 text-indigo-800",
    "Bị trì hoãn": "bg-red-100 text-red-800",
    "Đã hủy": "bg-red-200 text-red-900",
    "Hoàn thành": "bg-teal-100 text-teal-800",

  };

  const paymentStatusColors = {
    "Đang chờ thanh toán": "text-yellow-800",
    "Đã đặt cọc": "text-blue-800",
    "Đã thanh toán": "text-green-800",
    "Đã hủy": "btext-red-800",
  };
  useEffect(() => {
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
        setError(
          err.message || "An error occurred while fetching order details"
        );
      } finally {
        setIsLoading(false);
      }
    };

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

  const products = saleOrderDetailVMs?.$values || [];

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
            Chi tiết đơn hàng - <span className="text-orange-500">#{saleOrderCode}</span>
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
                <span className="font-semibold">Số điện thoại:</span> {contactPhone}
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
                <span className="font-semibold flex-shrink-0">Phương thức nhận hàng:</span>
                <span className="break-words">{deliveryMethod}</span>
              </p>
              <p className="flex items-center gap-2 mb-2">
                <FontAwesomeIcon
                  icon={faMoneyBillWave}
                  className="text-blue-500"
                />
                <span className="font-semibold">Tình trạng thanh toán:</span>{" "}
                <span
                  className={`py-2 px-2.5 mr-1.5 rounded-full text-xs font-bold ${statusColors[paymentStatus] ||
                    "bg-gray-100 text-gray-800"
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
                  className={`px-2.5 py-2 mr-5 rounded-full text-xs font-bold ${statusColors[orderStatus] ||
                    "bg-gray-100 text-gray-800"
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
                      <span className="font-semibold">Giá thuê:</span>{" "}
                      <i>{product.unitPrice.toLocaleString("Vi-VN")}₫</i>
                    </p>
                    <p>
                      <span className="font-semibold">Tổng tiền:</span>{" "}
                      <i>{product.totalAmount.toLocaleString("Vi-VN")}₫</i>
                    </p>
                  </div>
                </div>
              </div>
            </div>)
          )}
        </div>
      </div>
    </div>
  )
}
