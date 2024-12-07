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

export default function UserOrderDetail() {
  const { orderCode } = useParams();
  const navigate = useNavigate();
  const [orderDetail, setOrderDetail] = useState(null);
  const [isLoading, setIsLoading] = useState(true);
  const [error, setError] = useState(null);
  console.log(orderCode);
  

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
    saleOrderDetailVMs,
  } = orderDetail;

  const products = saleOrderDetailVMs?.$values || [];

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
                {saleOrderCode}
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
          {products.map((product) => (
            <div
              key={product.productId}
              className="bg-gray-50 p-4 mb-4 rounded-lg shadow-sm"
            >
              <div className="flex flex-col md:flex-row gap-4">
                <img
                  src={product.imgAvatarPath}
                  alt={product.productName}
                  className="w-full md:w-32 h-32 object-cover rounded"
                />
                <div className="flex-grow">
                  <h4 className="font-semibold text-lg mb-2">
                    {product.productName}
                  </h4>
                  <p>
                    <span className="font-semibold">Mã sản phẩm:</span>{" "}
                    {product.productCode}
                  </p>
                  <p>
                    <span className="font-semibold">Màu sắc:</span>{" "}
                    {product.color}
                  </p>
                  <p>
                    <span className="font-semibold">Kích thước:</span>{" "}
                    {product.size}
                  </p>
                  <p>
                    <span className="font-semibold">Số lượng:</span>{" "}
                    {product.quantity}
                  </p>
                  <p>
                    <span className="font-semibold">Đơn giá:</span>{" "}
                    {product.unitPrice} ₫
                  </p>
                  <p>
                    <span className="font-semibold">Thành tiền:</span>{" "}
                    {product.totalAmount} ₫
                  </p>
                </div>
              </div>
            </div>
          ))}
        </div>
      </div>
    </div>
  );
}
