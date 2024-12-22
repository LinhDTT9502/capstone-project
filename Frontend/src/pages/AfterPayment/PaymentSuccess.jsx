import React from "react";
import { useNavigate } from "react-router-dom";
import { FontAwesomeIcon } from "@fortawesome/react-fontawesome";
import { faCheckCircle } from "@fortawesome/free-solid-svg-icons";

const PaymentSuccess = () => {
  const navigate = useNavigate();

  return (
    <div className="min-h-screen container mx-auto px-4 md:px-20 py-10 bg-white rounded-lg shadow-lg mt-10">
      <div className="text-center">
        <FontAwesomeIcon icon={faCheckCircle} size="3x" className="text-green-500" />
        <h2 className="text-2xl font-bold text-green-600 mt-4">Thanh toán thành công!</h2>
      </div>

      <div className="mt-8 space-y-4">
        <div className="bg-gray-100 p-6 rounded-lg">
          <p className="text-lg text-center text-gray-700">
            2sport xin chân thành cảm ơn quý khách đã tin tưởng và sử dụng dịch vụ của chúng tôi.
            Chúng tôi rất trân trọng sự ủng hộ của quý khách!
          </p>
        </div>

        <div className="flex justify-center mt-8 space-x-4">
          <button
            onClick={() => navigate("/")}
            className="px-6 py-2 bg-orange-500 text-white rounded-md hover:bg-orange-600"
          >
            Quay lại trang chủ
          </button>
          <button
            onClick={() => navigate("/product")}
            className="px-6 py-2 bg-blue-500 text-white rounded-md hover:bg-blue-600"
          >
            Mua thêm sản phẩm
          </button>
        </div>
      </div>
    </div>
  );
};

export default PaymentSuccess;

