import React, { useState } from "react";
import { toast } from "react-toastify";
import { Button } from "@material-tailwind/react";
import axios from "axios";


const CancelSaleOrderButton = ({ saleOrderId, setReload }) => {
  const [showModal, setShowModal] = useState(false);
  const [reason, setReason] = useState("");

  const handleCancelOrder = async () => {
    if (!reason.trim()) {
      alert("Vui lòng nhập lý do hủy đơn hàng.");
      return;
    }
    try {
      const response = await axios.post(
        `https://capstone-project-703387227873.asia-southeast1.run.app/api/SaleOrder/request-cancel/${saleOrderId}?reason=${encodeURIComponent(
          reason
        )}`,
        {
          headers: {
            accept: "*/*",
          },
        }
      );
      setReload(true)
      toast.info("Bạn đã hủy đơn hàng thành công");
      setShowModal(false);
    } catch (error) {
      console.error("Error cancel order:", error);
      alert("Failed to cancel the order. Please try again.");
    }
  };
  return (
    <>
      <Button
        color="white"
        size="sm"
        className="text-red-700 bg-white border border-red-700 rounded-md hover:bg-red-200"
        onClick={() => {
          setShowModal(true);
        }}
      >
        Hủy đơn hàng
      </Button>
      {showModal && (
        <div className="fixed inset-0 flex items-center justify-center bg-black bg-opacity-50 z-50">
          <div className="bg-white p-6 rounded-md shadow-lg w-1/2">
            <h2 className="text-lg font-semibold pb-2 text-red-700">
              Xác nhận hủy đơn hàng
            </h2>
            <div className="w-full border rounded-md p-4 mb-4">
              <p className="font-semibold mb-4">
                Vui lòng chọn lý do hủy đơn hàng:
              </p>
              <div className="space-y-2">
                {[
                  "Tôi muốn cập nhật địa chỉ/số điện thoại nhận hàng.",
                  "Tôi muốn thêm/thay đổi Mã giảm giá.",
                  "Tôi muốn thay đổi sản phẩm (kích thước, màu sắc, số lượng...).",
                  "Thủ tục thanh toán rắc rối.",
                  "Tôi tìm thấy chỗ mua khác tốt hơn (Rẻ hơn, uy tín hơn, giao nhanh hơn...).",
                  "Tôi không có nhu cầu mua nữa.",
                  "Tôi không tìm thấy lý do hủy phù hợp.",
                ].map((reasonText, index) => (
                  <label key={index} className="flex items-center gap-2">
                    <input
                      type="radio"
                      name="cancelReason"
                      value={reasonText}
                      className="form-radio"
                      onChange={(e) => setReason(e.target.value)}
                    />
                    <span>{reasonText}</span>
                  </label>
                ))}
              </div>
            </div>
            <div className="flex justify-end space-x-4">
              <button
                className="bg-gray-500 text-white py-2 px-4 rounded-md"
                onClick={() => setShowModal(false)}
              >
                Đóng
              </button>
              <button
                className="bg-red-500 text-white py-2 px-4 rounded-md hover:bg-red-700"
                onClick={handleCancelOrder}
              >
                Hủy đơn hàng
              </button>
            </div>
          </div>
        </div>
      )}
    </>
  );
};

export default CancelSaleOrderButton;
