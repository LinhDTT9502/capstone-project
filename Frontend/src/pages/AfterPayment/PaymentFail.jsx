import React from "react";
import { useNavigate } from "react-router-dom";
import { FontAwesomeIcon } from "@fortawesome/react-fontawesome";
import { faTimesCircle } from "@fortawesome/free-solid-svg-icons";

const PaymentFail = () => {
  const navigate = useNavigate();

  // Fake data
  const fakeOrderData = {
    orderId: "123456",
    paymentDate: new Date().toISOString(),
    totalPrice: 1500000,
    paymentMethod: "VNPay",
  };

  return (
    <div className="container mx-auto px-4 md:px-20 py-10 bg-white rounded-lg shadow-lg">
      <div className="text-center">
        <FontAwesomeIcon icon={faTimesCircle} size="3x" className="text-red-500" />
        <h2 className="text-2xl font-bold text-red-600 mt-4">Thanh toán thất bại!</h2>
      </div>

      <div className="mt-8 space-y-4">
        <div className="bg-gray-100 p-6 rounded-lg">
          <h3 className="text-lg font-semibold text-orange-500">Thông tin đơn hàng</h3>
          <div className="mt-4">
            <p><strong>Mã đơn hàng:</strong> {fakeOrderData.orderId}</p>
            <p><strong>Ngày thực hiện:</strong> {new Date(fakeOrderData.paymentDate).toLocaleString()}</p>
            <p><strong>Tổng giá trị:</strong> {fakeOrderData.totalPrice.toLocaleString()} ₫</p>
            <p><strong>Phương thức thanh toán:</strong> {fakeOrderData.paymentMethod}</p>
          </div>
        </div>

        <div className="bg-gray-100 p-6 rounded-lg">
          <p className="text-gray-700 text-center">
            Chúng tôi rất tiếc, quá trình thanh toán không thành công. 
            Vui lòng kiểm tra lại thông tin thanh toán hoặc thử lại sau.
          </p>
        </div>

        <div className="mt-8 flex justify-center">
          <button
            onClick={() => navigate("/checkout")}
            className="px-6 py-2 bg-red-500 text-white rounded-md hover:bg-red-600"
          >
            Thử lại thanh toán
          </button>
          <button
            onClick={() => navigate("/contact")}
            className="ml-4 px-6 py-2 bg-gray-300 text-gray-700 rounded-md hover:bg-gray-400"
          >
            Liên hệ hỗ trợ
          </button>
        </div>
      </div>
    </div>
  );
};

export default PaymentFail;
