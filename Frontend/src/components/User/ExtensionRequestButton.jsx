import React, { useState } from "react";
import { toast } from "react-toastify";
import { Button, Input } from "@material-tailwind/react";
import axios from "axios";


const ExtensionRequestButton = ({
  selectedChildOrder,
  setExtendReload,
}) => {
  const [showExtendedModal, setExtendedShowModal] = useState(false);
  const [extensionDate, setExtensionDate] = useState("");
  const [selectedDate, setSelectedDate] = useState(null);

  const handleExtendOrder = async (order) => {
    // console.log(order);//id la parentId

    if (!extensionDate) {
      alert("Please select a valid date before extending the order.");
      return;
    }

    const selectedDateObj = new Date(extensionDate); // Convert selectedDate to a Date object
    const rentalEndDateObj = new Date(order.rentalEndDate); // Convert rentalEndDate to a Date object
    console.log(extensionDate);
    const extensionDays = Math.ceil(
      (selectedDateObj - rentalEndDateObj) / (1000 * 60 * 60 * 24)
    );

    console.log(extensionDays);

    const payload = {
      parentOrderId: id,
      childOrderId: order.parentOrderCode === rentalOrderCode ? order.id : null,
      extensionDays: extensionDays,
    };

    try {
      const response = await axios.post(
        `https://capstone-project-703387227873.asia-southeast1.run.app/api/RentalOrder/request-extension`,
        payload,
        {
          headers: {
            accept: "*/*",
          },
        }
      );
      if (!error.response.data.isSuccess) {
        console.log(error.response.data.message);
        toast.success("Đơn hàng của bạn đã được hoàn tất thành công.");
        setExtendReload(true);
        setExtendedShowModal(false);
      }

      console.log("Request thành công:", response);
    } catch (error) {
      console.error("Request thất bại:", error.response || error.message);
      console.log(error.response.data.message);
    }
  };
  const getTomorrowDate = () => {
    const tomorrow = new Date();
    tomorrow.setDate(tomorrow.getDate() + 1);
    return tomorrow.toISOString().split("T")[0];
  };

  const handleExtensionDateChange = (e) => {
    const newStartDate = e.target.value;
    setExtensionDate(newStartDate);
    setSelectedDate(null);
  };
  return (
    <>
      <Button
        size="sm"
        className={`text-yellow-700 bg-white border border-yellow-700 rounded-md hover:bg-yellow-200`}
        onClick={() => setExtendedShowModal(true)}
      >
        Gia Hạn Sản Phẩm
      </Button>

      {showExtendedModal && selectedChildOrder && (
        <div className="fixed inset-0 flex items-center justify-center bg-black bg-opacity-50 z-50">
          <div className="bg-white p-6 rounded-md shadow-lg w-2/5">
            <h2 className="text-lg font-semibold">Gửi yêu cầu gia hạn</h2>
            <p className="mb-4 text-sm">
              <i>Tóm tắt thông tin sản phẩm</i>
            </p>
            <div className="flex items-center space-x-4">
              <img
                src={selectedChildOrder.imgAvatarPath || "default-image.jpg"}
                alt="Order"
                className="w-24 h-24 object-contain rounded"
              />
              <div>
                <h3 className="font-medium text-base">
                  {selectedChildOrder.productName}
                </h3>
                <p className="text-sm text-gray-500">
                  Màu sắc: {selectedChildOrder.color} - Kích thước:{" "}
                  {selectedChildOrder.size} - Tình trạng:{" "}
                  {selectedChildOrder.condition}%
                </p>
                <p className="font-medium text-base text-rose-700">
                  Giá thuê:{" "}
                  {selectedChildOrder.rentPrice.toLocaleString("Vn-vi")}
                </p>
                <p className="font-medium text-sm">
                  Số lượng: {selectedChildOrder.quantity}
                </p>
              </div>
            </div>
            <div className="flex items-start justify-between space-x-6">
              {/* Cột 1 */}
              <div className="flex-1">
                <p className="pt-2 mb-2 text-sm">
                  <i>Tóm tắt thời gian thuê</i>
                </p>
                <p className="text-sm text-gray-500">
                  Ngày bắt đầu:{" "}
                  {new Date(selectedChildOrder.rentalStartDate).toLocaleString(
                    "vi-VN",
                    {
                      day: "2-digit",
                      month: "2-digit",
                      year: "numeric",
                    }
                  )}
                </p>
                <p className="text-sm text-gray-500">
                  Ngày kết thúc:{" "}
                  {new Date(selectedChildOrder.rentalEndDate).toLocaleString(
                    "vi-VN",
                    {
                      day: "2-digit",
                      month: "2-digit",
                      year: "numeric",
                    }
                  )}
                </p>
              </div>

              {/* Cột 2 */}
              <div className="flex-1">
                <p className="pt-2 mb-2 text-sm">
                  <i>Chọn ngày gia hạn</i>
                </p>
                <Input
                  label="ngày gia hạn"
                  type="date"
                  min={getTomorrowDate()}
                  value={selectedDate}
                  onChange={handleExtensionDateChange}
                  className="border rounded px-4 py-2 w-full"
                />
              </div>
            </div>

            <div className="flex justify-end space-x-4 mt-4">
              <button
                className="bg-gray-500 text-white py-2 px-4 rounded-md"
                onClick={() => setExtendedShowModal(false)}
              >
                Đóng
              </button>
              <button
                className="bg-red-500 text-white py-2 px-4 rounded-md"
                onClick={() => handleExtendOrder(selectedChildOrder)}
              >
                Gửi yêu cầu
              </button>
            </div>
          </div>
        </div>
      )}
    </>
  );
};

export default ExtensionRequestButton;
