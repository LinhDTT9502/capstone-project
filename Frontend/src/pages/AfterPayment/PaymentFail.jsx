import React from "react";
import { useNavigate } from "react-router-dom";
import { FontAwesomeIcon } from "@fortawesome/react-fontawesome";
import { faTimesCircle } from "@fortawesome/free-solid-svg-icons";

const PaymentFail = () => {
  const navigate = useNavigate();

  return (
    <div className="container min-h-screen mt-10 mx-auto px-4 md:px-20 py-10 bg-white rounded-lg shadow-lg">
      <div className="text-center">
        <FontAwesomeIcon
          icon={faTimesCircle}
          size="3x"
          className="text-red-500"
        />
        <h2 className="text-2xl font-bold text-red-600 mt-4">
          Thanh toán thất bại!
        </h2>
      </div>

      <div className="mt-8 space-y-4">
        <div className="bg-gray-100 p-6 rounded-lg">
          <p className="text-gray-700 text-center">
            Chúng tôi rất tiếc, quá trình thanh toán không thành công. Vui lòng
            kiểm tra lại thông tin thanh toán hoặc thử lại sau.
          </p>
        </div>

        <div className="bg-gray-100 p-6 rounded-lg">
          <p className="text-lg text-center text-gray-700">
            2sport xin chân thành cảm ơn quý khách đã quan tâm đến sản phẩm của
            chúng tôi. Chúng tôi rất mong được phục vụ quý khách trong tương
            lai!
          </p>
        </div>

        <div className="mt-8 flex justify-center">
          <button
            onClick={() => navigate("/product")}
            className="px-6 py-2 bg-blue-500 text-white rounded-md hover:bg-blue-600"
          >
            Xem thêm sản phẩm
          </button>
        </div>
      </div>
    </div>
  );
};

export default PaymentFail;
