import React, { useState, useEffect } from "react";
import { Button } from "@material-tailwind/react";
import { useTranslation } from "react-i18next";
import { useSelector } from "react-redux";
import { selectUser } from "../../redux/slices/authSlice";
import { fetchUserOrders } from "../../services/userOrderService";
import { useNavigate } from "react-router-dom";

export default function UserOrderStatus() {
  const { t } = useTranslation();
  const user = useSelector(selectUser);
  const [selectedStatus, setSelectedStatus] = useState("Tất cả");
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
    "Tất cả",
    "Đang chờ",
    "Đã xác nhận",
    "Đã thanh toán",
    "Đang đóng gói",
    "Đang vận chuyển",
    "Thành công",
    "Tạm hoãn",
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
    selectedStatus === "Tất cả"
      ? orders
      : orders.filter((order) => order.orderStatus === selectedStatus);

  const renderOrderStatusButton = (order) => {
    if (order.paymentStatus === "Đang chờ thanh toán" && order.deliveryMethod !== "HOME_DELIVERY") {
      return (
        <Button
          className="bg-green-700 text-white text-sm rounded-full py-2 px-4 w-40 mt-4"
          onClick={() => navigate("/checkout", { state: { selectedOrder: order } })}
        >
          Thanh Toán
        </Button>
      );
    }
  };

  const formatCurrency = (amount) => {
    let formattedAmount = new Intl.NumberFormat("vi-VN").format(amount);
    formattedAmount = formattedAmount.replace(/,/g, 'TEMP_COMMA').replace(/\./g, ',').replace(/TEMP_COMMA/g, '.');
    return formattedAmount;
  };

  if (isLoading) return <p>Loading orders...</p>;
  if (error) return <p>Error: {error}</p>;

  return (
    <div className="container mx-auto max-h-[70vh] overflow-y-auto">
      {/* Status Filter Tabs */}
      <div className="text-sm font-medium text-center text-gray-700 border-b border-gray-300 mb-6">
        <ul className="flex flex-wrap justify-center space-x-6">
          {statusList.map((status) => (
            <li key={status}>
              <button
                className={`inline-block p-4 border-b-2 rounded-t-lg ${selectedStatus === status
                  ? "text-orange-500 border-orange-500"
                  : "hover:text-gray-600 hover:border-gray-300"
                  } transition duration-300`}
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
          className="p-4 border border-gray-200 rounded-lg shadow-sm mt-4 relative flex flex-col"
        >
          <div className="mb-3 text-lg font-medium text-gray-500 text-right border-b border-gray-300">
            {order.orderStatus}
          </div>

          <div className="flex justify-between">
            <div className="flex">
              <img
                src={order.saleOrderDetailVMs.$values[0]?.imgAvatarPath}
                alt={order.saleOrderDetailVMs.$values[0]?.productName}
                className="w-24 h-24 object-cover rounded cursor-pointer"
                onClick={() => openProductModal(order)}
              />
              <div className="ml-4">
                <h4 className="font-semibold text-lg">
                  Mã đơn hàng: <span className="text-orange-500">{order.saleOrderCode}</span>
                </h4>
                <p className="text-gray-500">
                  Trạng thái thanh toán: {order.paymentStatus}
                </p>
                <p className="mt-2 font-bold">
                  Total: {formatCurrency(order.totalAmount)}₫
                </p>
              </div>
            </div>

            <div className="flex flex-col justify-between items-end">
              <p className="mt-4 font-semibold text-lg">
                Total Amount: <span className="text-orange-500">{formatCurrency(order.totalAmount)}₫</span>
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
                  Đơn giá: {formatCurrency(item.unitPrice)}₫
                </p>
              </div>
            ))}
          </div>

          {/* Place "Thanh Toán" button at the bottom of the order card */}
          {renderOrderStatusButton(order)}
        </div>
      ))}
    </div>
  );
}
