import React, { useState, useEffect } from "react";
import { Button } from "@material-tailwind/react";
import { useTranslation } from "react-i18next";
import { useSelector } from "react-redux";
import axios from "axios";
import { selectUser } from "../../redux/slices/authSlice";
import { fetchUserOrders } from "../../services/userOrderService";
import { useNavigate } from "react-router-dom";

export default function UserOrderStatus() {
  const { t } = useTranslation();
  const user = useSelector(selectUser);
  const [selectedStatus, setSelectedStatus] = useState("All");
  const [orders, setOrders] = useState([]);
  const [isLoading, setIsLoading] = useState(true);
  const [error, setError] = useState(null);
  const [isProductModalOpen, setProductModalOpen] = useState(false);
  const [selectedProduct, setSelectedProduct] = useState(null);
  const navigate = useNavigate();

  const openProductModal = (product) => {
    setSelectedProduct(product);
    setProductModalOpen(true);
  };
  const closeProductModal = () => setProductModalOpen(false);

  const statusList = [
    "All",
    "PENDING",
    "CONFIRMED",
    "PAID",
    "PROCESSING",
    "SHIPPED",
    "DELAYED",
    "COMPLETED",
  ];

  useEffect(() => {
    const fetchOrders = async () => {
      try {
        const token = localStorage.getItem("token");
        const response = await fetchUserOrders(user.UserId, token);
        setOrders(response);
      } catch (err) {
        setError(err.message || "Failed to fetch orders");
      } finally {
        setIsLoading(false);
      }
    };
    fetchOrders();
  }, [user.UserId]);

  const filteredOrders =
    selectedStatus === "All"
      ? orders
      : orders.filter((order) => order.orderStatus === selectedStatus);

      const renderOrderStatusButton = (order) => {
        console.log(order);
        
        if (order.paymentStatus === "IsWating" && order.deliveryMethod !== "HOME_DELIVERY") {
          return (
            <Button
              className="bg-purple-500 text-white text-sm rounded-full py-2 px-4"
              onClick={() => navigate("/checkout", { state: { selectedOrder: order } })}
            >
              Checkout
            </Button>
          );
        }
    
      
      };

  if (isLoading) return <p>Loading orders...</p>;
  if (error) return <p>Error: {error}</p>;

  return (
    <>
      <div className="container mx-auto">
        {/* Status Filter Tabs */}
        <div className="text-sm font-medium text-center text-gray-500 border-b border-gray-200">
          <ul className="flex flex-wrap -mb-px justify-center">
            {statusList.map((status) => (
              <li key={status} className="mr-6">
                <button
                  className={`inline-block p-4 border-b-2 rounded-t-lg ${
                    selectedStatus === status
                      ? "text-orange-500 border-orange-500"
                      : "hover:text-gray-600 hover:border-gray-300"
                  }`}
                  onClick={() => setSelectedStatus(status)}
                >
                  {status}
                </button>
              </li>
            ))}
          </ul>
        </div>

        {/* Order List */}
        {filteredOrders.map((order) => (
          <div
            key={order.saleOrderId}
            className="p-4 border border-gray-200 rounded-lg shadow-sm mt-4"
          >
            <div className="flex justify-between">
              <div className="flex">
                <img
                  src={
                    order.saleOrderDetailVMs.$values[0]?.imgAvatarPath ||
                    "https://via.placeholder.com/100"
                  }
                  alt={order.saleOrderDetailVMs.$values[0]?.productName}
                  className="w-24 h-24 object-cover rounded cursor-pointer"
                  onClick={() => openProductModal(order)}
                />
                <div className="ml-4">
                  <h4 className="font-semibold text-lg">
                    Order Code: {order.orderCode}
                  </h4>
                  <p className="text-gray-500">
                    Delivery: {order.deliveryMethod}
                  </p>
                  <p className="text-gray-500">
                    Payment: {order.paymentMethod}
                  </p>
                  <p className="text-gray-500">
                    Status: {order.orderStatus}
                  </p>
                  <p className="mt-2 font-bold">
                    Total: ${order.totalAmount}
                  </p>
                </div>
              </div>
              <div className="flex flex-col justify-between items-end">
                {renderOrderStatusButton(order)}
                <p className="mt-4 font-semibold text-lg">
                  Total Amount:{" "}
                  <span className="text-orange-500">${order.totalAmount}</span>
                </p>
              </div>
            </div>

            {/* Product Details */}
            <div className="mt-4">
              {order.saleOrderDetailVMs.$values.map((item) => (
                <div key={item.productId} className="flex justify-between">
                  <p className="text-gray-600">
                    {item.productName} (x{item.quantity})
                  </p>
                  <p className="text-gray-500">
                    Unit Price: ${item.unitPrice}
                  </p>
                </div>
              ))}
            </div>
          </div>
        ))}
      </div>

      {/* Product Modal */}
      {isProductModalOpen && (
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
              Product Details
            </h2>
            <p>{selectedProduct?.productName}</p>
          </div>
        </div>
      )}
    </>
  );
}
