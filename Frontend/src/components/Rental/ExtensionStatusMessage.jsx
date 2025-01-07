import React from "react";
import { FontAwesomeIcon } from "@fortawesome/react-fontawesome";
import { faBolt } from "@fortawesome/free-solid-svg-icons";

const ExtensionStatusMessage = ({ extensionStatus }) => {
  if (!extensionStatus) return null; // Nếu extensionStatus không tồn tại, không hiển thị gì

  // Tạo một object để ánh xạ trạng thái với message
  const statusMessages = {
    1: "Yêu cầu gia hạn đã được gửi. Vui lòng chờ thông báo từ nhân viên.",
    2: "Yêu cầu gia hạn được chấp thuận.",
    3: "Yêu cầu gia hạn bị từ chối.",
  };

  // Kiểm tra nếu trạng thái hợp lệ
  const message = statusMessages[extensionStatus];
  if (!message) return null;

  return (
    <div className="bg-yellow-50 p-1 rounded-lg shadow-sm mb-6">
      <p className="flex items-center gap-2">
        <FontAwesomeIcon icon={faBolt} style={{ color: "#fd7272" }} />
        <span className="font-semibold">{message}</span>
      </p>
    </div>
  );
};

export default ExtensionStatusMessage;
