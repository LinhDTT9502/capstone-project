import React, { useState } from "react";
import axios from "axios";
import { toast } from "react-toastify";
import { Button } from "@material-tailwind/react";

const ReturnRentalProductButton = (selectedOrderId) => {
  const [isPopupVisible, setIsPopupVisible] = useState(false);
  const [isLoading, setIsLoading] = useState(false);
    console.log(selectedOrderId);
  const togglePopup = () => {
    setIsPopupVisible(!isPopupVisible);
  };

  const handleConfirm = async () => {
    setIsLoading(true);
    try {
      const response = await axios.post(
        "https://localhost:7276/api/RentalOrder/request-return",
        {
          selectedReturnOrderId: selectedOrderId.selectedOrderId,
          requestTimestamp: new Date().toISOString(),
        },
        {
          headers: {
            Accept: "*/*",
            "Content-Type": "application/json",
          },
        }
      );

      if (response.status === 200) {
        toast.success("Yêu cầu trả hàng đã được gửi thành công!");
        setIsPopupVisible(false);
      } else {
        toast.error("Đã xảy ra lỗi khi gửi yêu cầu!");
      }
    } catch (error) {
      console.error("Lỗi khi gọi API:", error);
      toast.error("Không thể gửi yêu cầu, vui lòng thử lại sau.");
    } finally {
      setIsLoading(false);
    }
  };

  return (
    <div className="p-1">
      {/* Button mở popup */}
      <Button
        className="text-yellow-700 bg-white border border-yellow-700 rounded-md hover:bg-yellow-200"
        onClick={togglePopup}
        size="sm"
      >
        Yêu cầu trả hàng
      </Button>

      {/* Popup xác nhận */}
      {isPopupVisible && (
        <div className="fixed inset-0 bg-black bg-opacity-50 flex items-center justify-center z-50">
          <div className="bg-white p-6 rounded-md shadow-lg w-full max-w-md">
            <h2 className="text-lg font-semibold mb-4">
              Xác nhận gửi yêu cầu?
            </h2>
            <p className="text-sm text-gray-600">
              Bạn có chắc chắn muốn gửi yêu cầu trả hàng? Hành động này không
              thể hoàn tác.
            </p>
            <div className="mt-6 flex justify-end space-x-4">
              {/* Nút Đóng */}
              <button
                className="bg-gray-500 text-white px-4 py-2 rounded-md hover:bg-gray-600"
                onClick={togglePopup}
                disabled={isLoading}
              >
                Đóng
              </button>

              {/* Nút Xác nhận */}
              <Button
                className={`bg-red-500 text-white px-4 py-2 rounded-md hover:bg-red-600 ${
                  isLoading ? "opacity-50 cursor-not-allowed" : ""
                }`}
                onClick={handleConfirm}
                disabled={isLoading}
              >
                {isLoading ? "Đang gửi..." : "Xác nhận"}
              </Button>
            </div>
          </div>
        </div>
      )}
    </div>
  );
};

export default ReturnRentalProductButton;
