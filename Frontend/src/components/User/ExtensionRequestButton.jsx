import React, { useState } from "react";
import { toast } from "react-toastify";
import { Button, Input } from "@material-tailwind/react";
import axios from "axios";


const ExtensionRequestButton = ({
  parentOrder,
  selectedChildOrder,
  setExtendReload,
}) => {
  const [showExtendedModal, setExtendedShowModal] = useState(false);
  const [extensionDate, setExtensionDate] = useState("");
  const [selectedDate, setSelectedDate] = useState(null);
  const isDisabled =
    parentOrder.orderStatus !== "Đã giao hàng" ||
    selectedChildOrder.extensionStatus === "1";
  const [loading, setLoading] = useState(false);

  const handleExtendOrder = async (order) => {
    // console.log(order);//id la parentId

    if (!extensionDate) {
      alert("Ngày gia hạn chưa được chọn!");
      return;
    }
    setLoading(true);
    const selectedDateObj = new Date(extensionDate); // Convert selectedDate to a Date object
    const rentalEndDateObj = new Date(order.rentalEndDate); // Convert rentalEndDate to a Date object
    const extensionDays = Math.ceil(
      (selectedDateObj - rentalEndDateObj) / (1000 * 60 * 60 * 24)
    ) - 1;
   console.log(extensionDays);
    const payload = {
      parentOrderId: parentOrder.id,
      childOrderId:
        selectedChildOrder.parentOrderCode === parentOrder.rentalOrderCode
          ? selectedChildOrder.id
          : null,
      extensionDays: extensionDays,
    };
   console.log(payload)
    try {
      const response = await axios.post(
        `https://twosport-api-offcial-685025377967.asia-southeast1.run.app//api/RentalOrder/request-extension`,
        payload,
        {
          headers: {
            accept: "*/*",
          },
        }
      );
      console.log(response)

      if (response.data.isSuccess) {
        toast.success("Đã gửi yêu cầu gia hạn.");
        setExtendReload(true);
        setExtendedShowModal(false);
      }
      // console.log("Request thành công:", response);
    } catch (error) {
      console.error("Request thất bại:", error.response || error.message);
      console.log(error.response.data.message);
    } finally {
      setLoading(false);
    }
  };
  const getTomorrowDate = () => {
    const date = selectedChildOrder.rentalEndDate
      ? new Date(selectedChildOrder.rentalEndDate)
      : new Date();
    date.setDate(date.getDate() + 2);
    return date.toISOString().split("T")[0];
  };

  const handleExtensionDateChange = (e) => {
    const newStartDate = e.target.value;
    setExtensionDate(newStartDate);
    setSelectedDate(null);
  };
  return (
    <>
      <Button
        className={`p-2 ${
          isDisabled
            ? "text-yellow-700 bg-white border border-yellow-700 rounded-md hover:bg-yellow-200 cursor-not-allowed"
            : "text-yellow-700 bg-white border border-yellow-700 rounded-md hover:bg-yellow-200"
        }`}
        onClick={() => {
          if (!isDisabled) {
            setExtendedShowModal(true);
          }
        }}
        size="sm"
        disabled={isDisabled}
      >
        {selectedChildOrder.isExtended ? "Đang gia hạn" : "Gia Hạn Sản Phẩm"}
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
                  {selectedChildOrder.rentPrice.toLocaleString("Vn-vi")}₫
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
              <Button
                className="bg-gray-500 text-white py-2 px-4 rounded-md"
                onClick={() => setExtendedShowModal(false)}
              >
                Đóng
              </Button>
              <Button
                disabled={loading}
                className={`bg-red-500 text-white py-2 px-4 hover:bg-red-700 rounded-md  ${
                  loading ? "opacity-50 cursor-not-allowed" : ""
                }`}
                onClick={() => handleExtendOrder(selectedChildOrder)}
              >
                {loading ? "Đang xử lý..." : "Gửi yêu cầu"}
              </Button>
            </div>
          </div>
        </div>
      )}
    </>
  );
};

export default ExtensionRequestButton;
