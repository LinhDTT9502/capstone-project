import React from "react";
import { FontAwesomeIcon } from "@fortawesome/react-fontawesome";
import { faCircleCheck, faArrowRight } from "@fortawesome/free-solid-svg-icons";
import { useLocation, Link } from "react-router-dom";

const OrderSuccess = () => {
   const location = useLocation();
  const { orderID, orderCode, userId } = location.state || {};
  
  return (
    <div className="min-h-screen bg-gray-100 flex items-center justify-center px-4 sm:px-6 lg:px-8">
      <div className="max-w-md w-full space-y-8 bg-white p-10 rounded-xl shadow-lg">
        <div className="text-center">
          <FontAwesomeIcon
            icon={faCircleCheck}
            size="5x"
            className="text-green-500 mb-4"
          />
          <h1 className="text-3xl font-extrabold text-gray-900 mb-2">
            Đặt hàng thành công
          </h1>
          <p className="text-sm text-gray-600 mb-8">
            Bạn vui lòng thanh toán để hoàn thành đơn hàng!
          </p>

          {orderID && orderCode ? (
            <div className="text-left bg-gray-50 p-4 rounded-md mb-6">
              <p className="text-sm text-gray-600">
                Mã đơn hàng:{" "}
                <span className="font-medium text-orange-700">{orderCode}</span>
              </p>
              {userId !== null && (
                <Link to={`/manage-account/sale-order/${orderCode}`}>
                  Xem Chi Tiết Đơn Hàng
                </Link>
              )}
              <Link to={`/guest/guest-sale-order/${orderCode}`}>
                Xem Xem Chi Tiết Đơn Hàng
              </Link>
            </div>
          ) : null}
        </div>
      </div>
    </div>
  );
};

export default OrderSuccess;