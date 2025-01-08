import React from "react";
import { FontAwesomeIcon } from "@fortawesome/react-fontawesome";
import { faBolt } from "@fortawesome/free-solid-svg-icons";

const OrderCancellationInfo = ({ updatedAt, reason }) => {
  if (!updatedAt || !reason) return null; // Nếu không có dữ liệu, không hiển thị gì

  return (
    <div className="bg-yellow-50 p-4 rounded-lg shadow-sm mb-6">
      <p className="text-xl">
        <b className="text-red-500">Đã hủy đơn hàng </b>
        <i className="text-lg">
          vào lúc{" "}
          {new Date(updatedAt).toLocaleString("vi-VN", {
            day: "2-digit",
            month: "2-digit",
            year: "numeric",
            hour: "2-digit",
            minute: "2-digit",
            second: "2-digit",
          })}
        </i>
      </p>
      <p className="flex items-center gap-2 mb-2">
        <FontAwesomeIcon icon={faBolt} style={{ color: "#fd7272" }} />
        <span className="font-semibold">Lý do:</span> {reason}
      </p>
    </div>
  );
};

export default OrderCancellationInfo;
