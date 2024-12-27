import React, { useState, useEffect } from "react";
import { Button } from "@material-tailwind/react";
import { useTranslation } from "react-i18next";
import { useSelector } from "react-redux";
import { selectUser } from "../../redux/slices/authSlice";
import { fetchUserOrders } from "../../services/userOrderService";
import { useNavigate } from "react-router-dom";
import {
  faCaretDown,
  faCaretUp,
  faExclamationCircle,
  faShoppingBag,
} from "@fortawesome/free-solid-svg-icons";
import { FontAwesomeIcon } from "@fortawesome/react-fontawesome";
import { toast } from "react-toastify";

const statusColors = {
  "Chờ xử lý": "bg-yellow-100 text-yellow-800",
  "Đã xác nhận": "bg-green-100 text-green-800",
  "Đang xử lý": "bg-purple-100 text-purple-800",
  "Đã giao hàng": "bg-indigo-100 text-indigo-800",
  "Đã từ chối": "bg-red-100 text-red-800",
  "Đã hủy": "bg-red-200 text-red-900",
  "Đã hoàn thành": "bg-orange-100 text-orange-800",
};

const paymentStatusColors = {
  "Đang chờ thanh toán": "text-yellow-800",
  "Đã đặt cọc": "text-blue-800",
  "Đã thanh toán": "text-green-800",
  "Đã hủy": "text-red-800",
};

export default function UserOrderStatus() {
  const { t } = useTranslation();
  const user = useSelector(selectUser);
  const [selectedStatus, setSelectedStatus] = useState("Tất cả");
  const [orders, setOrders] = useState([]);
  const [filteredSaleOrder, setFilteredSaleOrders] = useState([]);
  const [isLoading, setIsLoading] = useState(true);
  const [error, setError] = useState(null);
  const [expandedOrderId, setExpandedOrderId] = useState(null);
  const [searchQuery, setSearchQuery] = useState("");

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
        setFilteredSaleOrders(sortedOrders);
      } catch (err) {
        setError(err.message || "Failed to fetch orders");
      } finally {
        setIsLoading(false);
      }
    };
    fetchOrders();
  }, [user.UserId]);

  const toggleExpand = (orderId) => {
    setExpandedOrderId((prevOrderId) =>
      prevOrderId === orderId ? null : orderId
    );
  };

  const filteredOrders =
    selectedStatus === "Tất cả"
      ? filteredSaleOrder
      : filteredSaleOrder.filter(
          (order) => order.orderStatus === selectedStatus
        );

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
    return (
      <div className="text-center text-gray-500  mt-32 flex flex-col items-center justify-center">
        <FontAwesomeIcon icon={faShoppingBag} className="text-6xl mb-2" />
        <p>Bạn chưa có sản phẩm nào</p>
      </div>
    );
  const handleSearch = () => {
    toast.info(`Tìm kiếm với từ khóa: ${searchQuery}`);
    setSearchQuery(searchQuery);
    if (searchQuery) {
      const filtered = orders.filter((order) => {
        return order.saleOrderDetailVMs.$values.some((item) => {
          // Kiểm tra nếu tên sản phẩm, màu sắc, hoặc kích thước chứa từ khóa tìm kiếm
          return (
            item.productName
              .toLowerCase()
              .includes(searchQuery.toLowerCase()) ||
            item.color.toLowerCase().includes(searchQuery.toLowerCase()) ||
            item.size.toLowerCase().includes(searchQuery.toLowerCase())
          );
        });
      });
      if (filtered.length === 0) {
        toast.info("Không tìm thấy sản phẩm nào khớp với từ khóa");
        return;
      }
      setFilteredSaleOrders(filtered);
    } else {
      setFilteredSaleOrders(orders);
    }
  };
  return (
    <div className="container mx-auto pt-2 rounded-lg max-w-4xl">
      <h2 className="text-orange-500 font-bold text-2xl pb-2">
        Danh sách đơn mua{" "}
      </h2>

      {/* Status Filter Tabs */}
      <div className="rounded-lg overflow-x-auto w-full">
        {/* Button Group */}
        <div className="flex justify-start p-4 bg-gray-50 border-b space-x-2 whitespace-nowrap">
          {[
            "Tất cả",
            "Chờ xử lý",
            "Đã xác nhận",
            "Đã giao cho đơn vị vận chuyển",
            "Đã giao hàng",
            "Đã hoàn thành",
            "Đã hủy",
          ].map((status) => (
            <button
              key={status}
              className={`px-4 py-2 m-1 rounded-full text-sm font-medium transition-colors duration-150 ease-in-out ${
                selectedStatus === status
                  ? "bg-orange-500 text-white" // Màu khi được chọn
                  : statusColors[status] || "bg-gray-200 text-gray-700" // Áp dụng màu từ statusColors
              }`}
              onClick={() => setSelectedStatus(status)}
            >
              {status}
            </button>
          ))}
        </div>

        {/* Search Bar */}
        {selectedStatus === "Tất cả" && (
          <div className="flex justify-start items-center p-4 bg-gray-50 border-t">
            <input
              type="text"
              placeholder="Tìm kiếm theo tên sản phẩm, màu sắc, kích thước..."
              className="w-96 px-4 py-2 border rounded-md text-sm focus:outline-none focus:ring-2 focus:ring-indigo-500 focus:border-indigo-500"
              value={searchQuery}
              onChange={(e) => setSearchQuery(e.target.value)}
            />
            <button
              className="ml-2 px-4 py-2 bg-orange-500 text-white rounded-md text-sm font-medium hover:bg-orange-600 transition-colors"
              onClick={handleSearch}
            >
              Tìm kiếm
            </button>
          </div>
        )}
      </div>
      <div className="max-h-[60vh] overflow-y-auto">
        {/* Order List */}
        {filteredOrders.map((order) => (
          <div
            key={order.saleOrderCode}
            className="border border-gray-200 rounded-lg shadow-sm mt-4 relative flex flex-col"
          >
            <div
              className="flex justify-between items-center p-4 cursor-pointer hover:bg-slate-200 transition-colors duration-150 ease-in-out"
              onClick={() => toggleExpand(order.saleOrderCode)}
            >
              <div className="flex flex-col">
                <h4 className="font-bold text-lg text-gray-800">
                  Mã đơn hàng:{" "}
                  <span className="text-orange-500">{order.saleOrderCode}</span>
                  <span
                    className={`px-3 ml-2.5 py-1 mr-5 rounded-full text-xs font-medium ${
                      statusColors[order.orderStatus] ||
                      "bg-gray-100 text-gray-800"
                    }`}
                  >
                    {order.orderStatus}
                  </span>
                </h4>
                <p className=" text-gray-600">
                  Trạng thái thanh toán:
                  <span
                    className={`ml-2 font-medium ${
                      paymentStatusColors[order.paymentStatus] ||
                      "text-gray-800"
                    }`}
                  >
                    <i> {order.paymentStatus}</i>
                  </span>
                </p>
                <p className="text-gray-600">
                  Hình thức nhận hàng: <i> {order.deliveryMethod}</i>
                </p>
                <p className="text-gray-600">
                  Ngày đặt:{" "}
                  <i>{new Date(order.createdAt).toLocaleDateString()}</i>
                </p>
                <p className="text-gray-600 mt-2 font-semibold text-lg pt-5">
                  Thành tiền:{" "}
                  <span className="text-orange-500">
                    {order.totalAmount.toLocaleString("Vi-vn")}₫
                  </span>
                </p>
              </div>

              <div className="flex flex-col w-1/4 h-auto items-end">
                <img
                  src={order.orderImage || "/assets/images/default_package.png"}
                  alt={order.orderImage}
                  className="w-40 h-40 object-contain rounded"
                />
                <Button
                  color="orange"
                  size="sm"
                  className="w-40"
                  onClick={(e) => {
                    e.stopPropagation();
                    navigate(
                      `/manage-account/sale-order/${order.saleOrderCode}`
                    );
                  }}
                >
                  Xem chi tiết
                </Button>
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
                        Màu sắc: {item.color} - Kích thước: {item.size} - Tình
                        trạng: {item.condition}%
                      </p>
                      <p className="font-medium text-base text-rose-700">
                        Giá bán: {item.unitPrice.toLocaleString("Vi-vn")} ₫
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
    </div>
  );
}
