import React, { useState } from "react";
import { Button } from "@material-tailwind/react";
import { useTranslation } from "react-i18next";

// Mock database (in-memory)
const orders = [
  { id: 1, name: "Name of product 1", category: "Shoes", quantity: 1, price: 19.5, status: "Đang giao" },
  { id: 2, name: "Name of product 2", category: "Shirts", quantity: 2, price: 29.5, status: "Chờ giao hàng" },
  { id: 3, name: "Name of product 3", category: "Pants", quantity: 1, price: 39.5, status: "Hoàn thành" },
  { id: 4, name: "Name of product 4", category: "Accessories", quantity: 3, price: 25.5, status: "Đã hủy" },
];

export default function UserOrderStatus() {
  const { t } = useTranslation();
  const [selectedStatus, setSelectedStatus] = useState("Tất cả");
  const [isCancelModalOpen, setCancelModalOpen] = useState(false);
  const [isOtherModalOpen, setOtherModalOpen] = useState(false); // You can add more modals as needed

  const openCancelModal = () => setCancelModalOpen(true);
  const closeCancelModal = () => setCancelModalOpen(false);

  const openOtherModal = () => setOtherModalOpen(true);
  const closeOtherModal = () => setOtherModalOpen(false);

  const filteredOrders = selectedStatus === "Tất cả"
    ? orders
    : orders.filter(order => order.status === selectedStatus);

  const renderOrderStatusButton = (status) => {
    switch (status) {
      case "Đang giao":
        return <Button className="bg-pink-500 text-white text-sm rounded-full py-2 px-4">Đang giao</Button>;
      case "Hoàn thành":
        return <Button className="bg-orange-500 text-white text-sm rounded-full py-2 px-4" onClick={openOtherModal}>Hoàn thành</Button>;
      case "Đã hủy":
        return (
          <>
            <Button className="bg-red-500 text-white text-sm rounded-full py-2 px-4">Đã hủy</Button>
            <div className="mt-2">
              <Button className="bg-red-500 text-white text-sm rounded-full py-2 px-4 mr-2" onClick={openCancelModal}>Xem chi tiết</Button>
              <Button className="bg-gray-300 text-black text-sm rounded-full py-2 px-4">Mua lại</Button>
            </div>
          </>
        );
      default:
        return <Button className="bg-orange-500 text-white text-sm rounded-full py-2 px-4">{status}</Button>;
    }
  };

  return (
    <>
      <div className="container mx-auto">
        {/* Tab navigation */}
        <div className="text-sm font-medium text-center text-gray-500 border-b border-gray-200">
          <ul className="flex flex-wrap -mb-px justify-center">
            <li className="mr-6">
              <a
                href="#"
                className={`inline-block p-4 border-b-2 rounded-t-lg ${
                  selectedStatus === "Tất cả"
                    ? "text-orange-500 border-orange-500"
                    : "hover:text-gray-600 hover:border-gray-300"
                }`}
                onClick={() => setSelectedStatus("Tất cả")}
              >
                Tất cả
              </a>
            </li>
            <li className="mr-6">
              <a
                href="#"
                className={`inline-block p-4 border-b-2 rounded-t-lg ${
                  selectedStatus === "Chờ thanh toán"
                    ? "text-orange-500 border-orange-500"
                    : "hover:text-gray-600 hover:border-gray-300"
                }`}
                onClick={() => setSelectedStatus("Chờ thanh toán")}
              >
                Chờ thanh toán
              </a>
            </li>
            <li className="mr-6">
              <a
                href="#"
                className={`inline-block p-4 border-b-2 rounded-t-lg ${
                  selectedStatus === "Đang vận chuyển"
                    ? "text-orange-500 border-orange-500"
                    : "hover:text-gray-600 hover:border-gray-300"
                }`}
                onClick={() => setSelectedStatus("Đang vận chuyển")}
              >
                Vận chuyển
              </a>
            </li>
            <li className="mr-6">
              <a
                href="#"
                className={`inline-block p-4 border-b-2 rounded-t-lg ${
                  selectedStatus === "Chờ giao hàng"
                    ? "text-orange-500 border-orange-500"
                    : "hover:text-gray-600 hover:border-gray-300"
                }`}
                onClick={() => setSelectedStatus("Chờ giao hàng")}
              >
                Chờ giao hàng
              </a>
            </li>
            <li className="mr-6">
              <a
                href="#"
                className={`inline-block p-4 border-b-2 rounded-t-lg ${
                  selectedStatus === "Hoàn thành"
                    ? "text-orange-500 border-orange-500"
                    : "hover:text-gray-600 hover:border-gray-300"
                }`}
                onClick={() => setSelectedStatus("Hoàn thành")}
              >
                Hoàn thành
              </a>
            </li>
            <li>
              <a
                href="#"
                className={`inline-block p-4 border-b-2 rounded-t-lg ${
                  selectedStatus === "Đã hủy"
                    ? "text-orange-500 border-orange-500"
                    : "hover:text-gray-600 hover:border-gray-300"
                }`}
                onClick={() => setSelectedStatus("Đã hủy")}
              >
                Đã hủy
              </a>
            </li>
          </ul>
        </div>

        {/* Order items */}
        {filteredOrders.map((order) => (
          <div key={order.id} className="p-4 border border-gray-200 rounded-lg shadow-sm mt-4">
            <div className="flex justify-between">
              <div className="flex">
                <img
                  src="https://via.placeholder.com/100" // Replace with your image source
                  alt={order.name}
                  className="w-24 h-24 object-cover rounded"
                />
                <div className="ml-4">
                  <h4 className="font-semibold text-lg">{order.name}</h4>
                  <p className="text-gray-500">Category: {order.category}</p>
                  <p className="text-gray-500">x{order.quantity}</p>
                  <p className="mt-2 font-bold">${order.price}</p>
                </div>
              </div>
              <div className="flex flex-col justify-between items-end">
                <p className="text-sm text-gray-500">{order.status === "Đang giao" ? "Đơn hàng đang được giao đến" : 
                order.status === "Hoàn thành" ? "Đơn hàng đã giao thành công" : ""}</p>
                {renderOrderStatusButton(order.status)}
                <p className="mt-4 font-semibold text-lg">Total: <span className="text-orange-500">${order.price}</span></p>
              </div>
            </div>
          </div>
        ))}
      </div>

      {/* Cancel Order Modal */}
      {isCancelModalOpen && (
        <div className="fixed inset-0 bg-black bg-opacity-50 flex items-center justify-center z-50">
          <div className="bg-white w-full max-w-lg p-6 rounded-lg shadow-lg relative">
            <button
              onClick={closeCancelModal}
              className="absolute top-4 right-4 text-red-500 hover:text-red-700"
            >
              &#x2715; {/* Close icon */}
            </button>

            <h2 className="text-2xl font-bold text-orange-500">Cancel Order Detail</h2>
            <p className="text-right text-red-500">Đã hủy đơn hàng</p>

            <div className="flex justify-between items-center mt-6">
              <div className="flex flex-col items-center">
                <div className="bg-red-500 rounded-full w-8 h-8 flex items-center justify-center text-white">1</div>
                <p className="text-center mt-2">Gửi yêu cầu</p>
              </div>
              <div className="flex-grow border-t border-black mx-4"></div>
              <div className="flex flex-col items-center">
                <div className="bg-red-500 rounded-full w-8 h-8 flex items-center justify-center text-white">2</div>
                <p className="text-center mt-2">Tiếp nhận và xử lý</p>
              </div>
              <div className="flex-grow border-t border-black mx-4"></div>
              <div className="flex flex-col items-center">
                <div className="bg-red-500 rounded-full w-8 h-8 flex items-center justify-center text-white">3</div>
                <p className="text-center mt-2">Được chấp nhận</p>
              </div>
            </div>

            <div className="mt-8">
              <h3 className="font-semibold">Order</h3>
              <div className="flex mt-4">
                <img
                  src="https://via.placeholder.com/80"
                  alt="Product 1"
                  className="w-20 h-20 object-cover rounded"
                />
                <div className="ml-4">
                  <p className="font-semibold">Name of product</p>
                  <p className="text-gray-500">Size S, Black</p>
                  <p className="font-bold">$19.5</p>
                </div>
              </div>
              <div className="flex mt-4">
                <img
                  src="https://via.placeholder.com/80"
                  alt="Product 2"
                  className="w-20 h-20 object-cover rounded"
                />
                <div className="ml-4">
                  <p className="font-semibold">Name of product</p>
                  <p className="text-gray-500">Size S, Black</p>
                  <p className="font-bold">$58.5</p>
                </div>
              </div>
            </div>

            <div className="mt-8 border-t pt-4">
              <p><span className="font-semibold">Total:</span> $83</p>
              <p><span className="font-semibold">Request:</span> Người mua</p>
              <p><span className="font-semibold">Payment method:</span> COD</p>
              <p><span className="font-semibold">Order Code:</span> #1982</p>
            </div>
          </div>
        </div>
      )}

      {/* Other Modal (Example) */}
      {isOtherModalOpen && (
        <div className="fixed inset-0 bg-black bg-opacity-50 flex items-center justify-center z-50">
          <div className="bg-white w-full max-w-lg p-6 rounded-lg shadow-lg relative">
            <button
              onClick={closeOtherModal}
              className="absolute top-4 right-4 text-red-500 hover:text-red-700"
            >
              &#x2715; {/* Close icon */}
            </button>

            <h2 className="text-2xl font-bold text-orange-500">Other Order Detail</h2>
            <p className="text-right text-green-500">Đơn hàng đã hoàn thành</p>

            {/* Add your other modal content here */}
          </div>
        </div>
      )}
    </>
  );
}
