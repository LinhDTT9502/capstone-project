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

  const statusColors = {
    "Chờ xử lý": "bg-yellow-100 text-yellow-800",
    "Đã xác nhận": "bg-blue-100 text-blue-800",
    "Đã thanh toán": "bg-green-100 text-green-800",
    "Đang xử lý": "bg-purple-100 text-purple-800",
    "Đã giao hàng": "bg-indigo-100 text-indigo-800",
    "Bị trì hoãn": "bg-red-100 text-red-800",
    "Hoàn thành": "bg-teal-100 text-teal-800",
  };

  useEffect(() => {
    const fetchOrders = async () => {
      try {
        const token = localStorage.getItem("token");
        const response = await fetchUserOrders(user.UserId, token);
        const sortedOrders = response.sort((a, b) => {
          return new Date(b.createdAt) - new Date(a.createdAt);
        });
        setOrders(sortedOrders);
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
    if (
      order.paymentStatus === "Đang chờ thanh toán" &&
      order.deliveryMethod !== "HOME_DELIVERY"
    ) {
      return (
        <Button
          className="bg-green-700 text-white text-sm rounded-full py-2 px-4 w-40 mt-4"
          onClick={() =>
            navigate("/checkout", { state: { selectedOrder: order } })
          }
        >
          Thanh Toán
        </Button>
      );
    }
  };

  const formatCurrency = (amount) => {
    let formattedAmount = new Intl.NumberFormat("vi-VN").format(amount);
    formattedAmount = formattedAmount
      .replace(/,/g, "TEMP_COMMA")
      .replace(/\./g, ",")
      .replace(/TEMP_COMMA/g, ".");
    return formattedAmount;
  };

  if (isLoading)
    return (
      <div className="flex justify-center items-center h-64">
        <div className="animate-spin rounded-full h-32 w-32 border-t-2 border-b-2 border-orange-500"></div>
      </div>
    );
  if (error)
    return <p className="text-center text-red-500 mt-4">Lỗi: {error}</p>;

  return (
    <div className="container mx-auto pt-2 rounded-lg max-w-4xl max-h-[70vh] overflow-y-auto">
      <h2 className="text-orange-500 font-bold text-2xl">Danh sách đơn mua </h2>
      {/* Status Filter Tabs */}
      <div className=" rounded-lg overflow-hidden">
        <div className="flex flex-wrap justify-center p-4 bg-gray-50 border-b">
          {[
            "Tất cả",
            "Chờ xử lý",
            "Đã xác nhận",
            "Đã thanh toán",
            "Đang xử lý",
            "Đã giao hàng",
            "Hoàn thành",
          ].map((status) => (
            <button
              key={status}
              className={`px-4 py-2 m-1 rounded-full text-sm font-medium transition-colors duration-150 ease-in-out ${
                selectedStatus === status
                  ? "bg-orange-500 text-white"
                  : "bg-gray-200 text-gray-700 hover:bg-gray-300"
              }`}
              onClick={() => setSelectedStatus(status)}
            >
              {status}
            </button>
          ))}
        </div>
      </div>

      {/* Order List */}
      {filteredOrders.map((order) => (
        <div
          key={order.saleOrderId}
          className="p-4 border border-gray-200 rounded-lg shadow-sm mt-4 relative flex flex-col"
        >
          <div className="flex flex-col items-start mb-4">
            <div className="mb-3 text-lg font-medium text-gray-500 text-right border-b border-gray-300">
              {order.orderStatus}
            </div>
            {/* {renderOrderStatusButton(order)} */}
            <div className="flex justify-between mt-4">
              <div className="flex">
                <img
                  src={order.saleOrderDetailVMs.$values[0]?.imgAvatarPath}
                  alt={order.saleOrderDetailVMs.$values[0]?.productName}
                  className="w-24 h-24 object-cover rounded cursor-pointer"
                  onClick={() => openProductModal(order)}
                />
                <div className="ml-4">
                  <h4 className="font-semibold text-lg">
                    Mã đơn hàng:{" "}
                    <span className="text-orange-500">
                      {order.saleOrderCode}
                    </span>
                  </h4>
                  <p className="text-gray-500">
                    Trạng thái thanh toán: {order.paymentStatus}
                  </p>
                  <p className="mt-2 font-bold">
                    Tổng giá: {formatCurrency(order.totalAmount)}₫
                  </p>
                </div>
              </div>

              {/* <div className="flex flex-col justify-between items-end">
                <p className="mt-4 font-semibold text-lg">
                  Total Amount:{" "}
                  <span className="text-orange-500">
                    {formatCurrency(order.totalAmount)}₫
                  </span>
                </p>
              </div> */}
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
                 Giá: {formatCurrency(item.unitPrice)}₫
                </p>
              </div>
            ))}
          </div>

          {/* Place "Thanh Toán" button at the bottom of the order card */}
          <div className="flex flex-col justify-between items-end ">
          {renderOrderStatusButton(order)}
          </div>
        </div>
      ))}
    </div>
  );
}
