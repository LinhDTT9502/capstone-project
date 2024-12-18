import React from "react";
import { FontAwesomeIcon } from "@fortawesome/react-fontawesome";
import { faCircleCheck, faArrowRight } from "@fortawesome/free-solid-svg-icons";
import { useLocation, Link } from "react-router-dom";

const OrderSuccess = () => {

  return (
    <div className="min-h-screen bg-gray-100 flex items-center justify-center px-4 sm:px-6 lg:px-8">
      <div className="max-w-md w-full space-y-8 bg-white p-10 rounded-xl shadow-lg">
        <div className="text-center">
          <FontAwesomeIcon icon={faCircleCheck} size="5x" className="text-green-500 mb-4" />
          <h1 className="text-3xl font-extrabold text-gray-900 mb-2">Đặt hàng thành công</h1>
          <p className="text-sm text-gray-600 mb-8">Bạn vui lòng thanh toán để hoàn thành đơn hàng!</p>
          
          {/* {orderID && orderCode ? (
            <div className="text-left bg-gray-50 p-4 rounded-md mb-6">
              <p className="text-sm text-gray-600">Mã đơn hàng: <span className="font-medium text-gray-900">{orderID}</span></p>
              <p className="text-sm text-gray-600">ID đơn hàng: <span className="font-medium text-gray-900">{orderCode}</span></p>
            </div>
          ) : null} */}
          
  
        </div>
      </div>
    </div>
  );
};

export default OrderSuccess;