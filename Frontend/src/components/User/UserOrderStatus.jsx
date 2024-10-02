import React, { useState } from "react";
import { Button } from "@material-tailwind/react";
import { useTranslation } from "react-i18next";

// Mock database (in-memory)
const orders = [
  {
    id: 1,
    name: "Name of product 1",
    category: "Shoes",
    quantity: 1,
    price: 19.5,
    status_key: "order_status.order_shipping",
  },
  {
    id: 2,
    name: "Name of product 2",
    category: "Shirts",
    quantity: 2,
    price: 29.5,
    status_key: "order_status.order_pending_delivery",
  },
  {
    id: 3,
    name: "Name of product 3",
    category: "Pants",
    quantity: 1,
    price: 39.5,
    status_key: "order_status.order_completed",
  },
  {
    id: 4,
    name: "Name of product 4",
    category: "Accessories",
    quantity: 3,
    price: 25.5,
    status_key: "order_status.order_cancelled",
  },
  {
    id: 5,
    name: "Name of product 5",
    category: "Accessories",
    quantity: 1,
    price: 35.5,
    status_key: "order_status.order_return_refund",
  },
];

export default function UserOrderStatus() {
  const { t } = useTranslation();
  const [selectedStatus, setSelectedStatus] = useState("Tất cả");
  const [isCancelModalOpen, setCancelModalOpen] = useState(false);
  const [isProductModalOpen, setProductModalOpen] = useState(false);
  const [selectedProduct, setSelectedProduct] = useState(null);

  const openCancelModal = () => setCancelModalOpen(true);
  const closeCancelModal = () => setCancelModalOpen(false);

  const openProductModal = (product) => {
    setSelectedProduct(product);
    setProductModalOpen(true);
  };
  const closeProductModal = () => setProductModalOpen(false);

  const filteredOrders =
    selectedStatus === "Tất cả"
      ? orders
      : orders.filter((order) => t(order.status_key) === selectedStatus);

  const renderOrderStatusButton = (statusKey) => {
    const status = t(statusKey);
    switch (status) {
      case t("order_status.order_shipping"):
        return (
          <Button className="bg-pink-500 text-white text-sm rounded-full py-2 px-4">
            {status}
          </Button>
        );
      case t("order_status.order_completed"):
        return (
          <Button
            className="bg-orange-500 text-white text-sm rounded-full py-2 px-4"
            onClick={() => openProductModal(statusKey)}
          >
            {status}
          </Button>
        );
      case t("order_status.order_cancelled"):
        return (
          <>
            <Button className="bg-red-500 text-white text-sm rounded-full py-2 px-4">
              {status}
            </Button>
            <div className="mt-2">
              <Button
                className="bg-red-500 text-white text-sm rounded-full py-2 px-4 mr-2"
                onClick={openCancelModal}
              >
                Xem chi tiết
              </Button>
              <Button className="bg-gray-300 text-black text-sm rounded-full py-2 px-4">
                Mua lại
              </Button>
            </div>
          </>
        );
      case t("order_status.order_return_refund"):
        return (
          <Button className="bg-yellow-500 text-white text-sm rounded-full py-2 px-4">
            {status}
          </Button>
        );
      default:
        return (
          <Button className="bg-orange-500 text-white text-sm rounded-full py-2 px-4">
            {status}
          </Button>
        );
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
                {t("order_status.order_all")}
              </a>
            </li>
            <li className="mr-6">
              <a
                href="#"
                className={`inline-block p-4 border-b-2 rounded-t-lg ${
                  selectedStatus === t("order_status.order_pending_payment")
                    ? "text-orange-500 border-orange-500"
                    : "hover:text-gray-600 hover:border-gray-300"
                }`}
                onClick={() =>
                  setSelectedStatus(t("order_status.order_pending_payment"))
                }
              >
                {t("order_status.order_pending_payment")}
              </a>
            </li>
            <li className="mr-6">
              <a
                href="#"
                className={`inline-block p-4 border-b-2 rounded-t-lg ${
                  selectedStatus === t("order_status.order_shipping")
                    ? "text-orange-500 border-orange-500"
                    : "hover:text-gray-600 hover:border-gray-300"
                }`}
                onClick={() =>
                  setSelectedStatus(t("order_status.order_shipping"))
                }
              >
                {t("order_status.order_shipping")}
              </a>
            </li>
            <li className="mr-6">
              <a
                href="#"
                className={`inline-block p-4 border-b-2 rounded-t-lg ${
                  selectedStatus === t("order_status.order_pending_delivery")
                    ? "text-orange-500 border-orange-500"
                    : "hover:text-gray-600 hover:border-gray-300"
                }`}
                onClick={() =>
                  setSelectedStatus(t("order_status.order_pending_delivery"))
                }
              >
                {t("order_status.order_pending_delivery")}
              </a>
            </li>
            <li className="mr-6">
              <a
                href="#"
                className={`inline-block p-4 border-b-2 rounded-t-lg ${
                  selectedStatus === t("order_status.order_completed")
                    ? "text-orange-500 border-orange-500"
                    : "hover:text-gray-600 hover:border-gray-300"
                }`}
                onClick={() =>
                  setSelectedStatus(t("order_status.order_completed"))
                }
              >
                {t("order_status.order_completed")}
              </a>
            </li>
            <li className="mr-6">
              <a
                href="#"
                className={`inline-block p-4 border-b-2 rounded-t-lg ${
                  selectedStatus === t("order_status.order_return_refund")
                    ? "text-orange-500 border-orange-500"
                    : "hover:text-gray-600 hover:border-gray-300"
                }`}
                onClick={() =>
                  setSelectedStatus(t("order_status.order_return_refund"))
                }
              >
                {t("order_status.order_return_refund")}
              </a>
            </li>
            <li>
              <a
                href="#"
                className={`inline-block p-4 border-b-2 rounded-t-lg ${
                  selectedStatus === t("order_status.order_cancelled")
                    ? "text-orange-500 border-orange-500"
                    : "hover:text-gray-600 hover:border-gray-300"
                }`}
                onClick={() =>
                  setSelectedStatus(t("order_status.order_cancelled"))
                }
              >
                {t("order_status.order_cancelled")}
              </a>
            </li>
          </ul>
        </div>

        {/* Order items */}
        {filteredOrders.map((order) => (
          <div
            key={order.id}
            className="p-4 border border-gray-200 rounded-lg shadow-sm mt-4"
          >
            <div className="flex justify-between">
              <div className="flex">
                <img
                  src="https://via.placeholder.com/100"
                  alt={order.name}
                  className="w-24 h-24 object-cover rounded cursor-pointer"
                  onClick={() => openProductModal(order)}
                />
                <div className="ml-4">
                  <h4 className="font-semibold text-lg">{order.name}</h4>
                  <p className="text-gray-500">Category: {order.category}</p>
                  <p className="text-gray-500">x{order.quantity}</p>
                  <p className="mt-2 font-bold">${order.price}</p>
                </div>
              </div>
              <div className="flex flex-col justify-between items-end">
                <p className="text-sm text-gray-500">
                  {order.status_key === "order_status.order_shipping"
                    ? "Đơn hàng đang được giao đến"
                    : order.status_key === "order_status.order_completed"
                    ? "Đơn hàng đã giao thành công"
                    : order.status_key === "order_status.order_return_refund"
                    ? "Đơn hàng đang được xử lý trả hàng/hoàn tiền"
                    : ""}
                </p>
                {renderOrderStatusButton(order.status_key)}
                <p className="mt-4 font-semibold text-lg">
                  Total: <span className="text-orange-500">${order.price}</span>
                </p>
              </div>
            </div>
          </div>
        ))}
      </div>

      {/* Cancel Order Modal */}
      {isCancelModalOpen && (
        <div
          className="fixed inset-0 bg-black bg-opacity-50 flex items-center justify-center z-50"
          onClick={closeCancelModal}
        >
          <div
            className="bg-white w-full max-w-lg p-6 rounded-lg shadow-lg relative"
            onClick={(e) => e.stopPropagation()}
          >
            <button
              onClick={closeCancelModal}
              className="absolute top-4 right-4 text-red-500 hover:text-red-700"
            >
              &#x2715;
            </button>

            <h2 className="text-2xl font-bold text-orange-500">
              Cancel Order Detail
            </h2>
            <p className="text-right text-red-500">Đã hủy đơn hàng</p>

            <div className="flex justify-between items-center mt-6">
              <div className="flex flex-col items-center">
                <div className="bg-red-500 rounded-full w-8 h-8 flex items-center justify-center text-white">
                  1
                </div>
                <p className="text-center mt-2">Gửi yêu cầu</p>
              </div>
              <div className="flex-grow border-t border-black mx-4"></div>
              <div className="flex flex-col items-center">
                <div className="bg-red-500 rounded-full w-8 h-8 flex items-center justify-center text-white">
                  2
                </div>
                <p className="text-center mt-2">Tiếp nhận và xử lý</p>
              </div>
              <div className="flex-grow border-t border-black mx-4"></div>
              <div className="flex flex-col items-center">
                <div className="bg-red-500 rounded-full w-8 h-8 flex items-center justify-center text-white">
                  3
                </div>
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
              <p>
                <span className="font-semibold">Total:</span> $83
              </p>
              <p>
                <span className="font-semibold">Request:</span> Người mua
              </p>
              <p>
                <span className="font-semibold">Payment method:</span> COD
              </p>
              <p>
                <span className="font-semibold">Order Code:</span> #1982
              </p>
            </div>
          </div>
        </div>
      )}

      {/* Product Detail Modal */}
      {isProductModalOpen && selectedProduct && (
        <div
          className="fixed inset-0 bg-black bg-opacity-50 flex items-center justify-center z-50"
          onClick={closeProductModal}
        >
          <div
            className="bg-white w-full max-w-lg p-6 rounded-lg shadow-lg relative"
            onClick={(e) => e.stopPropagation()}
          >
            <button
              onClick={closeProductModal}
              className="absolute top-4 right-4 text-red-500 hover:text-red-700"
            >
              &#x2715;
            </button>

            <h2 className="text-2xl font-bold text-orange-500">
              Product Detail
            </h2>

            <div className="mt-8">
              <h3 className="font-semibold">Product</h3>
              <div className="flex mt-4">
                <img
                  src="https://via.placeholder.com/80"
                  alt={selectedProduct.name}
                  className="w-20 h-20 object-cover rounded"
                />
                <div className="ml-4">
                  <p className="font-semibold">{selectedProduct.name}</p>
                  <p className="text-gray-500">Category: {selectedProduct.category}</p>
                  <p className="font-bold">${selectedProduct.price}</p>
                </div>
              </div>
            </div>

            <div className="mt-8 border-t pt-4">
              <p><span className="font-semibold">Total:</span> ${selectedProduct.price}</p>
              <p><span className="font-semibold">Quantity:</span> {selectedProduct.quantity}</p>
              <p><span className="font-semibold">Status:</span> {t(selectedProduct.status_key)}</p>
            </div>
          </div>
        </div>
      )}
    </>
  );
}
