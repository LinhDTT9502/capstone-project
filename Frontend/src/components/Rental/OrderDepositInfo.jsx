import React from "react";
import { FontAwesomeIcon } from "@fortawesome/react-fontawesome";
import { faDollarSign } from "@fortawesome/free-solid-svg-icons";

const OrderDepositInfo = ({ depositAmount, depositDate, paymentMethod }) => {
  if (!depositAmount || depositAmount <= 0 || !depositDate || !paymentMethod)
    return null; // Nếu không có dữ liệu hoặc không hợp lệ, không hiển thị gì

  return (
    <div className="bg-green-500 p-4 rounded-lg shadow-sm mb-6">
      <p className="text-xl">
        <b className="text-white">Đơn hàng đã được đặt cọc </b>
        <i className="text-lg">
          vào lúc{" "}
          {new Date(depositDate).toLocaleString("vi-VN", {
            day: "2-digit",
            month: "2-digit",
            year: "numeric",
            hour: "2-digit",
            minute: "2-digit",
            second: "2-digit",
          })}{" "}
          - {paymentMethod}
        </i>
      </p>
      <p className="flex items-center gap-2 mb-2">
        <FontAwesomeIcon icon={faDollarSign} style={{ color: "#FFD43B" }} />
        <span className="font-semibold">Số tiền: </span>
        {depositAmount.toLocaleString("vi-VN")}₫
      </p>
    </div>
  );
};

export default OrderDepositInfo;
