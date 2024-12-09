import React, { useState, useEffect } from "react";
import { Button } from "@material-tailwind/react";
import { useTranslation } from "react-i18next";
import { useSelector } from "react-redux";
import { selectUser } from "../../redux/slices/authSlice";
import { fetchUserOrders } from "../../services/userOrderService";
import { useNavigate } from "react-router-dom";
import { faCaretDown, faCaretUp, faExclamationCircle, faShoppingBag } from "@fortawesome/free-solid-svg-icons";
import { FontAwesomeIcon } from "@fortawesome/react-fontawesome";

const statusColors = {
  "Chờ xử lý": "bg-yellow-100 text-yellow-800",
  "Đã xác nhận": "bg-blue-100 text-blue-800",
  "Đã thanh toán": "bg-green-100 text-green-800",
  "Đang xử lý": "bg-purple-100 text-purple-800",
  "Đã giao hàng": "bg-indigo-100 text-indigo-800",
  "Bị trì hoãn": "bg-red-100 text-red-800",
  "Đã hủy": "bg-red-200 text-red-900",
  "Hoàn thành": "bg-teal-100 text-teal-800",
};

const paymentStatusColors = {
  "Đang chờ thanh toán": "text-yellow-800",
  "Đã đặt cọc": "text-blue-800",
  "Đã thanh toán": "text-green-800",
  "Đã hủy": "btext-red-800",
};

export default function UserOrderStatus() {
  const { t } = useTranslation();
  const user = useSelector(selectUser);
  const [selectedStatus, setSelectedStatus] = useState("Tất cả");
  const [orders, setOrders] = useState([]);
  const [isLoading, setIsLoading] = useState(true);
  const [error, setError] = useState(null);
  const [expandedOrderId, setExpandedOrderId] = useState(null); 
  const navigate = useNavigate();

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

const toggleExpand = (orderId) => {
  setExpandedOrderId((prevOrderId) => (prevOrderId === orderId ? null : orderId)); 
};


  const filteredOrders =
    selectedStatus === "Tất cả"
      ? orders
      : orders.filter((order) => order.orderStatus === selectedStatus);

  const formatCurrency = (amount) => {
    let formattedAmount = new Intl.NumberFormat("vi-VN").format(amount);
    formattedAmount = formattedAmount
      .replace(/,/g, "TEMP_COMMA")
      .replace(/\./g, ",")
      .replace(/TEMP_COMMA/g, ".");
    return formattedAmount;
  };

  // Hàm render nút "Thanh toán"
  const renderPaymentButton = (order) => {
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

  if (isLoading)
    return (
      <div className="flex justify-center items-center h-64">
        <div className="animate-spin rounded-full h-32 w-32 border-t-2 border-b-2 border-orange-500"></div>
      </div>
    );
  if (error)
    return <div className="text-center text-gray-500  mt-32 flex flex-col items-center justify-center">
  <FontAwesomeIcon icon={faShoppingBag} className="text-6xl mb-2" />
  <p>Bạn chưa có sản phẩm nào</p>
</div>

  return (
    <div className="container mx-auto pt-2 rounded-lg max-w-4xl max-h-[70vh] overflow-y-auto">
      <h2 className="text-orange-500 font-bold text-2xl">Danh sách đơn mua </h2>

      {/* Status Filter Tabs */}
      <div className="rounded-lg overflow-hidden">
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
          key={order.saleOrderCode}
          className="p-4 border border-gray-200 rounded-lg shadow-sm mt-4 relative flex flex-col"
        >
          <div
            className="flex justify-between items-center p-6 cursor-pointer hover:bg-slate-200 transition-colors duration-150 ease-in-out"
            onClick={() => toggleExpand(order.saleOrderCode)}
          >
            <div className="flex flex-col">
              <h4 className="font-semibold text-lg text-gray-800">
                Mã đơn hàng:{" "}
                <span className="text-orange-500">{order.saleOrderCode}</span>
              </h4>
              <p className=" text-gray-600">
                Trạng thái thanh toán:
                <span
                  className={`ml-2 font-medium ${
                    paymentStatusColors[order.paymentStatus] ||
                    "text-gray-800"
                  }`}
                >
                  {order.paymentStatus}
                </span>
              </p>
              <p className="text-gray-600">
                Hình thức giao hàng: {order.deliveryMethod}
              </p>
              <p className="text-gray-600">
                  Ngày đặt: {new Date(order.createdAt).toLocaleDateString()}
                </p>
              <p className="mt-2 font-bold text-lg">
                Tổng giá: <span className="text-orange-500">{formatCurrency(order.totalAmount)}₫</span>
              </p>

            </div>
            <div className="flex flex-col items-end">
              <div className="flex flex-row">
                <span
                  className={`px-3 py-1 mr-5 rounded-full text-xs font-medium ${
                    statusColors[order.orderStatus] ||
                    "bg-gray-100 text-gray-800"
                  }`}
                >
                  {order.orderStatus}
                </span>
                <FontAwesomeIcon
                key={order.saleOrderCode}
                  icon={
                    expandedOrderId === order.saleOrderCode
                      ? faCaretUp
                      : faCaretDown
                  }
                  className="w-6 h-6 text-gray-500"
                />
              </div>
              <div className="flex flex-col">
                {" "}
                {/* Nút thanh toán */}
                {renderPaymentButton(order)}
              </div>
            </div>
          </div>

          {/* Product Details */}
          {expandedOrderId === order.saleOrderCode && (
            <div className="mt-4 pl-8 border-l">
              {order.saleOrderDetailVMs.$values.map((item) => (
                <div
                  key={item.productId}
                  className="flex p-2 border-b last:border-none cursor-pointer"
                >
                  <img
                    src={item.imgAvatarPath || "default-image.jpg"}
                    alt={item.productName}
                    className="w-24 h-24 object-contain rounded"
                  />
                  <div className="ml-4">
                    <h5 className="font-medium text-base">
                      {item.productName}
                    </h5>
                    <p className="text-sm text-gray-500">
                      Màu sắc: {item.color} - Kích thước: {item.size} - Tình trạng:{" "}
                      {item.condition}%
                    </p>
                    <p className="font-medium text-base text-rose-700">
                      Giá: {formatCurrency(item.unitPrice)}₫
                    </p>
                    <p className="font-medium text-sm">
                      Số lượng: {item.quantity}
                    </p>
                  </div>
                </div>
              ))}
            </div>
          )}
        </div>
      ))}
    </div>
  );
}
