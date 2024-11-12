import React, { useState, useEffect } from "react";
import { Button } from "@material-tailwind/react";
import { useTranslation } from "react-i18next";
import { useSelector } from "react-redux"; 
import axios from "axios";
import { selectUser } from "../../redux/slices/authSlice";
import { fetchUserOrders } from "../../services/userOrderService";

export default function UserOrderStatus() {
  const { t } = useTranslation();
  const user = useSelector(selectUser); 
  const [selectedStatus, setSelectedStatus] = useState("Tất cả");
  const [orders, setOrders] = useState([]);
  const [isLoading, setIsLoading] = useState(true);
  const [error, setError] = useState(null); 
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

  useEffect(() => {
    const fetchOrders = async () => {
        try {
          const token = localStorage.getItem("token");
          const response = await fetchUserOrders(user.UserId, token);
          const test = response.flatMap(item => item.saleOrderDetailVMs);
          setOrders(response); 
          console.log(test);
          console.log(response);
          
          
        } catch (error) {
          setError(err.message || "Failed to fetch orders");
        } finally {
          setIsLoading(false);
        }
    };

    fetchOrders();
  }, [user.UserId]);

  const filteredOrders =
    selectedStatus === "Tất cả"
      ? orders
      : orders.filter((order) => t(order.orderStatus) === selectedStatus);

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

  if (isLoading) return <p>Loading orders...</p>;
  if (error) return <p>Error: {error}</p>;

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
            {/* Other status tabs */}
            {/* Add more statuses as needed */}
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
                  {t(order.status_key)}
                </p>
                {renderOrderStatusButton(order.status_key)}
                <p className="mt-4 font-semibold text-lg">
                  Total:{" "}
                  <span className="text-orange-500">${order.price}</span>
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
              {/* Order Cancellation Steps */}
            </div>

            {/* Display order products */}
            <div className="mt-8">
              <h3 className="font-semibold">Order</h3>
              {/* Loop through order products here */}
            </div>
          </div>
        </div>
      )}
    </>
  );
}
