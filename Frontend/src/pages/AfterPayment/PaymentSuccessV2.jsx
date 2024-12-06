import React from "react";
import { useLocation, useNavigate } from "react-router-dom";
import { FontAwesomeIcon } from "@fortawesome/react-fontawesome";
import { faCheckCircle } from "@fortawesome/free-solid-svg-icons";

const PaymentSuccessV2 = () => {
  const navigate = useNavigate();
  const location = useLocation();

  // Dữ liệu đơn hàng nhận từ navigation state
  const orderData = location.state?.orderData;

  // Kiểm tra xem có dữ liệu đơn hàng không
  if (!orderData) {
    return (
      <div className="text-center p-10">
        <p>Thông tin đơn hàng không hợp lệ!</p>
      </div>
    );
  }

  return (
    <div className="container mx-auto px-4 md:px-20 py-10 bg-white rounded-lg shadow-lg">
      <div className="text-center">
        <FontAwesomeIcon icon={faCheckCircle} size="3x" className="text-green-500" />
        <h2 className="text-2xl font-bold text-green-600 mt-4">Thanh toán thành công qua PayOS/VNPay!</h2>
      </div>

      <div className="mt-8 space-y-4">
        <div className="bg-gray-100 p-6 rounded-lg">
          <h3 className="text-lg font-semibold text-orange-500">Thông tin đơn hàng</h3>
          <div className="mt-4">
            <p><strong>Mã đơn hàng:</strong> {orderData.orderId}</p>
            <p><strong>Ngày thanh toán:</strong> {new Date(orderData.paymentDate).toLocaleString()}</p>
            <p><strong>Tổng giá trị:</strong> {orderData.totalPrice.toLocaleString()} ₫</p>
            <p><strong>Phương thức thanh toán:</strong> {orderData.paymentMethod}</p>
          </div>
        </div>

        <div className="bg-gray-100 p-6 rounded-lg">
          <h3 className="text-lg font-semibold text-orange-500">Thông tin sản phẩm</h3>
          {orderData.products.map((product, index) => (
            <div key={index} className="mt-4">
              <p><strong>{product.name}</strong></p>
              <p><strong>Số lượng:</strong> {product.quantity}</p>
              <p><strong>Giá:</strong> {product.price.toLocaleString()} ₫</p>
            </div>
          ))}
        </div>

        <div className="bg-gray-100 p-6 rounded-lg">
          <h3 className="text-lg font-semibold text-orange-500">Thông tin giao hàng</h3>
          <p><strong>Địa chỉ giao hàng:</strong> {orderData.shippingAddress}</p>
          <p><strong>Ngày giao hàng dự kiến:</strong> {new Date(orderData.estimatedDeliveryDate).toLocaleDateString()}</p>
        </div>

        <div className="mt-8">
          <p className="font-semibold text-gray-700">
            Thanh toán qua PayOS/VNPay đã thành công. Quá trình xử lý đơn hàng sẽ được hoàn tất và sản phẩm sẽ được giao đến bạn trong thời gian dự kiến.
          </p>
        </div>

        <div className="flex justify-between mt-8">
          <button
            onClick={() => navigate("/")}
            className="px-6 py-2 bg-orange-500 text-white rounded-md hover:bg-orange-600"
          >
            Quay lại trang chủ
          </button>
          <button
            onClick={() => navigate("/orders")}
            className="px-6 py-2 bg-gray-300 text-gray-700 rounded-md hover:bg-gray-400"
          >
            Xem lịch sử đơn hàng
          </button>
        </div>
      </div>
    </div>
  );
};

export default PaymentSuccessV2;
