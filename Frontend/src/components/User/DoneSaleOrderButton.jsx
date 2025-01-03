import React, { useEffect, useState } from "react";
import { toast } from "react-toastify";
import { Button } from "@material-tailwind/react";
import axios from "axios";
import { FontAwesomeIcon } from "@fortawesome/react-fontawesome";
import { faCircleCheck } from "@fortawesome/free-solid-svg-icons";
import ReviewSaleOrderModal from "../Review/ReviewSaleOrderModal";

const DoneSaleOrderButton = ({ saleOrderId, setConfirmReload }) => {
  const [showModal, setShowModal] = useState(false);
  const [reviewModal, setReviewModal] = useState(false);

  const handleDoneOrder = async () => {
    const newStatus = 5;
    try {
      const response = await axios.put(
        `https://capstone-project-703387227873.asia-southeast1.run.app/api/SaleOrder/update-order-status/${saleOrderId}?status=${newStatus}`,
        
        {
          headers: {
            accept: "*/*",
          },
        }
      );
      
      if (response && response.data.isSuccess) {
        toast.success("Đơn hàng của bạn đã được hoàn tất thành công.");
              // setConfirmReload(true);
              setShowModal(false);
              setReviewModal(true);
      } else {
        alert("Không thể hoàn tất đơn hàng. Vui lòng thử lại sau.");
      }
    } catch (error) {
      console.error("Error updating order status:", error);
      alert("Đã xảy ra lỗi khi hoàn tất đơn hàng. Vui lòng thử lại sau.");
    }
  };

  useEffect(() => {

  }, [saleOrderId,reviewModal]);

  return (
    <>
      <Button
        className={`w-40 text-green-700 bg-white border border-green-700 rounded-md hover:bg-green-200`}
        onClick={() => setShowModal(true)}
      >
        Đã nhận hàng
      </Button>

      {showModal && (
        <div className="fixed inset-0 flex items-center justify-center bg-black bg-opacity-50 z-50">
          <div className="bg-white p-6 rounded-md shadow-lg w-full max-w-md">
            <h2 className="text-lg font-semibold text-green-700 mb-4 text-center">
              Xác nhận bạn đã nhận được hàng
            </h2>
            <div className="flex justify-center">
              <FontAwesomeIcon
                icon={faCircleCheck}
                size="5x"
                className="text-green-500 mb-4"
              />
            </div>
            <div className="flex justify-center space-x-4 mt-4">
              <button
                className="bg-gray-500 text-white py-2 px-4 rounded-md hover:bg-gray-600 transition-all"
                onClick={() => setShowModal(false)}
              >
                Đóng
              </button>
              <button
                className="bg-red-500 text-white py-2 px-4 rounded-md hover:bg-red-700 transition-all"
                onClick={handleDoneOrder}
              >
                Hoàn thành
              </button>
            </div>
          </div>
        </div>
      )}

   
      <ReviewSaleOrderModal 
      saleOrderId={saleOrderId}
      setReviewModal={setReviewModal}
      reviewModal={reviewModal}
      setConfirmReload={setConfirmReload}
      />
    </>
  );
};

export default DoneSaleOrderButton;
