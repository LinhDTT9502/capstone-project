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
      setReload(true);
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
        className="w-40 text-red-700  bg-white border border-red-700 rounded-md hover:bg-red-200"
        onClick={() => {
          setShowModal(true);
        }}
      >
        Đánh giá
      </Button>
      {showModal && (
        <div className="fixed inset-0 flex items-center justify-center bg-black bg-opacity-50 z-50">
          <div className="bg-white p-6 rounded-md shadow-lg w-1/2">
            <h2 className="text-lg font-semibold pb-2 text-red-700">
              Đánh Giá Sản Phẩm
            </h2>
            <div>
              <p>Chất lượng sản phẩm</p>
              <div class="grid min-h-[140px] w-full place-items-center overflow-x-scroll rounded-lg p-6 lg:overflow-visible">
                <div class="inline-flex items-center">
                  <span>
                    <svg
                      xmlns="http://www.w3.org/2000/svg"
                      viewBox="0 0 24 24"
                      fill="currentColor"
                      class="w-6 h-6 text-yellow-700 cursor-pointer"
                    >
                      <path
                        fill-rule="evenodd"
                        d="M10.788 3.21c.448-1.077 1.976-1.077 2.424 0l2.082 5.007 5.404.433c1.164.093 1.636 1.545.749 2.305l-4.117 3.527 1.257 5.273c.271 1.136-.964 2.033-1.96 1.425L12 18.354 7.373 21.18c-.996.608-2.231-.29-1.96-1.425l1.257-5.273-4.117-3.527c-.887-.76-.415-2.212.749-2.305l5.404-.433 2.082-5.006z"
                        clip-rule="evenodd"
                      ></path>
                    </svg>
                  </span>
                  <span>
                    <svg
                      xmlns="http://www.w3.org/2000/svg"
                      viewBox="0 0 24 24"
                      fill="currentColor"
                      class="w-6 h-6 text-yellow-700 cursor-pointer"
                    >
                      <path
                        fill-rule="evenodd"
                        d="M10.788 3.21c.448-1.077 1.976-1.077 2.424 0l2.082 5.007 5.404.433c1.164.093 1.636 1.545.749 2.305l-4.117 3.527 1.257 5.273c.271 1.136-.964 2.033-1.96 1.425L12 18.354 7.373 21.18c-.996.608-2.231-.29-1.96-1.425l1.257-5.273-4.117-3.527c-.887-.76-.415-2.212.749-2.305l5.404-.433 2.082-5.006z"
                        clip-rule="evenodd"
                      ></path>
                    </svg>
                  </span>
                  <span>
                    <svg
                      xmlns="http://www.w3.org/2000/svg"
                      viewBox="0 0 24 24"
                      fill="currentColor"
                      class="w-6 h-6 text-yellow-700 cursor-pointer"
                    >
                      <path
                        fill-rule="evenodd"
                        d="M10.788 3.21c.448-1.077 1.976-1.077 2.424 0l2.082 5.007 5.404.433c1.164.093 1.636 1.545.749 2.305l-4.117 3.527 1.257 5.273c.271 1.136-.964 2.033-1.96 1.425L12 18.354 7.373 21.18c-.996.608-2.231-.29-1.96-1.425l1.257-5.273-4.117-3.527c-.887-.76-.415-2.212.749-2.305l5.404-.433 2.082-5.006z"
                        clip-rule="evenodd"
                      ></path>
                    </svg>
                  </span>
                  <span>
                    <svg
                      xmlns="http://www.w3.org/2000/svg"
                      viewBox="0 0 24 24"
                      fill="currentColor"
                      class="w-6 h-6 text-yellow-700 cursor-pointer"
                    >
                      <path
                        fill-rule="evenodd"
                        d="M10.788 3.21c.448-1.077 1.976-1.077 2.424 0l2.082 5.007 5.404.433c1.164.093 1.636 1.545.749 2.305l-4.117 3.527 1.257 5.273c.271 1.136-.964 2.033-1.96 1.425L12 18.354 7.373 21.18c-.996.608-2.231-.29-1.96-1.425l1.257-5.273-4.117-3.527c-.887-.76-.415-2.212.749-2.305l5.404-.433 2.082-5.006z"
                        clip-rule="evenodd"
                      ></path>
                    </svg>
                  </span>
                  <span>
                    <svg
                      xmlns="http://www.w3.org/2000/svg"
                      fill="none"
                      viewBox="0 0 24 24"
                      stroke-width="1.5"
                      stroke="currentColor"
                      class="w-6 h-6 cursor-pointer text-blue-gray-500"
                    >
                      <path
                        stroke-linecap="round"
                        stroke-linejoin="round"
                        d="M11.48 3.499a.562.562 0 011.04 0l2.125 5.111a.563.563 0 00.475.345l5.518.442c.499.04.701.663.321.988l-4.204 3.602a.563.563 0 00-.182.557l1.285 5.385a.562.562 0 01-.84.61l-4.725-2.885a.563.563 0 00-.586 0L6.982 20.54a.562.562 0 01-.84-.61l1.285-5.386a.562.562 0 00-.182-.557l-4.204-3.602a.563.563 0 01.321-.988l5.518-.442a.563.563 0 00.475-.345L11.48 3.5z"
                      ></path>
                    </svg>
                  </span>
                </div>
              </div>
            </div>
            <div className="w-full border rounded-md p-4 mb-4">
              <p className="font-semibold mb-4">
                Chất lượng sản phẩm:
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
