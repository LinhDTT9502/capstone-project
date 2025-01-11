import React, { useEffect, useState } from "react";
import { useSelector } from "react-redux";
import { selectGuestOrders } from "../redux/slices/guestOrderSlice";
import { Link, useNavigate } from "react-router-dom";
import { FontAwesomeIcon } from "@fortawesome/react-fontawesome";
import { faArrowLeft } from "@fortawesome/free-solid-svg-icons";
import { useTranslation } from "react-i18next";
import { Button } from "@material-tailwind/react";
import axios from "axios";
import { toast } from "react-toastify";
import CancelSaleOrderButton from "../components/User/CancelSaleOrderButton";
import DoneSaleOrderButton from "../components/User/DoneSaleOrderButton";
import ReviewButton from "../components/Review/ReviewButton";

const statusColors = {
  "Chờ xử lý": "bg-yellow-100 text-yellow-800",
  "Đã xác nhận": "bg-orange-100 text-orange-800",
  "Đang xử lý": "bg-purple-100 text-purple-800",
  "Đã giao hàng": "bg-indigo-100 text-indigo-800",
  "Đã giao cho ĐVVC": "bg-blue-100 text-blue-800",
  "Đã hủy": "bg-red-200 text-red-900",
  "Đã hoàn thành": "bg-green-100 text-green-800",
};

const paymentStatusColors = {
  "Đang chờ thanh toán": "text-yellow-800",
  "Đã đặt cọc": "text-blue-800",
  "Đã thanh toán": "text-green-800",
  "Đã hủy": "text-red-800",
};

const GuestOrder = () => {
  const orders = useSelector(selectGuestOrders);
  const [isLoading, setIsLoading] = useState(true);
  const navigate = useNavigate();
  const { t } = useTranslation();
  const [selectedStatus, setSelectedStatus] = useState("Tất cả");
  const [expandedOrderId, setExpandedOrderId] = useState(null);
  const [detailedOrders, setDetailedOrders] = useState([]);
  const [searchQuery, setSearchQuery] = useState("");
  const [reload, setReload] = useState(false);
  const [confirmReload, setConfirmReload] = useState(false);
  const [filteredSaleOrders, setFilteredSaleOrders] = useState([]);

  const toggleExpand = (orderId) => {
    setExpandedOrderId((prevOrderId) =>
      prevOrderId === orderId ? null : orderId
    );
  };

  const fetchAllOrderDetails = async () => {
    const detailedOrderList = []; // Danh sách tạm thời
    for (const order of orders) {
      const response = await axios.get(
        `https://capstone-project-703387227873.asia-southeast1.run.app/api/SaleOrder/get-order-by-code?orderCode=${order.saleOrderCode}`,
        { headers: { accept: "*/*" } }
      );
      const data = response.data.data;
      if (response.data.isSuccess) {
        detailedOrderList.push(data);
      }
    }
    setDetailedOrders(
      detailedOrderList.sort((a, b) => {
        return new Date(b.createdAt) - new Date(a.createdAt);
      })
    ); // Cập nhật danh sách mới
    setFilteredSaleOrders(
      detailedOrderList.sort((a, b) => {
        return new Date(b.createdAt) - new Date(a.createdAt);
      })
    );
  };

  useEffect(() => {
    if (orders && orders.length > 0) {
      fetchAllOrderDetails();
    }
  }, [orders, reload, confirmReload]);

  if (!orders || orders.length === 0) {
    return (
      <div className="flex flex-col items-center my-10 py-32">
        <img
          src="/assets/images/cart-icon.png"
          className="w-48 h-auto object-contain"
        />
        <p className="pt-4 text-lg font-poppins">{t("guest_order.empty")}</p>
        <Link
          to="/product"
          className="text-blue-500 flex items-center font-poppins"
        >
          <FontAwesomeIcon className="pr-2" icon={faArrowLeft} />{" "}
          {t("guest_order.continue_shopping")}
        </Link>
      </div>
    );
  }
  // if(filteredOrders){
  //   setIsLoading(false);
  // }
  // if (isLoading)
  //   return (
  //     <div className="flex justify-center items-center h-64">
  //       <div className="animate-spin rounded-full h-32 w-32 border-t-2 border-b-2 border-orange-500"></div>
  //     </div>
  //   );

  const handleSearch = () => {
    toast.info(`Tìm kiếm với từ khóa: ${searchQuery}`);
    setSearchQuery(searchQuery);
    if (!searchQuery) {
      setFilteredSaleOrders(detailedOrders);
      return;
    }

    if (searchQuery) {
      const filtered = detailedOrders.filter((order) => {
        return (
          order.saleOrderCode
            .toLowerCase()
            .includes(searchQuery.toLowerCase()) ||
          order.saleOrderDetailVMs.$values.some((item) => {
            return (
              item.productName
                .toLowerCase()
                .includes(searchQuery.toLowerCase()) ||
              item.color.toLowerCase().includes(searchQuery.toLowerCase()) ||
              item.size.toLowerCase().includes(searchQuery.toLowerCase())
            );
          })
        );
      });

      if (filtered.length === 0) {
        toast.info(
          "Không tìm thấy sản phẩm hoặc đơn hàng nào khớp với từ khóa"
        );
        setFilteredSaleOrders([]);
        return;
      }
      setFilteredSaleOrders(filtered);
    }
  };

  const filteredOrders =
    selectedStatus === "Tất cả"
      ? filteredSaleOrders
      : filteredSaleOrders.filter(
          (order) => order.orderStatus === selectedStatus
        );

  return (
    <div className="container mx-auto pt-2 rounded-lg max-w-5xl">
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
            "Đã giao cho ĐVVC",
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
        {filteredOrders.length === 0 && (
          <div className="flex flex-col items-center my-10">
            <img
              src="/assets/images/cart-icon.png"
              className="w-48 h-auto object-contain"
            />
            <p className="pt-4 text-lg font-poppins">
              Hiện tại chưa có đơn hàng để hiển thị
            </p>
            <Link
              to="/product"
              className="text-blue-500 flex items-center font-poppins"
            >
              <FontAwesomeIcon className="pr-2" icon={faArrowLeft} /> Bấm vào
              đây để mua sắm bạn nhé
            </Link>
          </div>
        )}
        {/* Order List */}
        {filteredOrders &&
          filteredOrders.map((order) => (
            <div
              key={order.saleOrderCode}
              className="border border-gray-200 rounded-lg shadow-sm mt-4 relative flex flex-col"
            >
              <div
                className="flex justify-between items-center p-2 cursor-pointer hover:bg-slate-200 transition-colors duration-150 ease-in-out"
                onClick={() => toggleExpand(order.saleOrderCode)}
              >
                <div className="flex flex-col w-3/4 pl-4">
                  <h4 className="font-bold text-lg text-gray-800">
                    Mã đơn hàng:{" "}
                    <span className="text-orange-500">
                      {order.saleOrderCode}
                    </span>
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
                </div>

                <div className="flex flex-col w-1/4 h-auto items-end">
                  <img
                    src={
                      order.orderImage || "/assets/images/default_package.png"
                    }
                    alt={order.orderImage}
                    className="w-32 h-32 object-contain rounded"
                  />
                </div>
              </div>
              {/* Product Details */}
              {expandedOrderId === order.saleOrderCode && (
                <div className="my-2 pl-8 border-l bg-orange-50">
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

              <div>
                <div className="h-px bg-gray-300 my-2 sm:my-2"></div>
                {/* Container for Thành tiền and buttons */}
                <div className="flex items-center justify-between my-4 px-2">
                  <p className="text-gray-600 font-semibold text-lg pl-2  ">
                    Thành tiền:{" "}
                    <span className="text-orange-500">
                      {order.totalAmount.toLocaleString("Vi-vn")} ₫
                    </span>
                  </p>
                  <div className="flex gap-2">
                    {order.paymentStatus === "N/A" &&
                      order.orderStatus === "Chờ xử lý" && (
                        <Button
                          color="white"
                          size="sm"
                          className="w-40 text-blue-700 border border-blue-700 rounded-md hover:bg-blue-200"
                          onClick={() =>
                            navigate("/checkout", {
                              state: { selectedOrder: order },
                            })
                          }
                        >
                          Thanh toán
                        </Button>
                      )}
                    {order.orderStatus === "Chờ xử lý" &&
                      order.paymentStatus != "Đã hủy" && (
                        <CancelSaleOrderButton
                          saleOrderId={order.id}
                          setReload={setReload}
                        />
                      )}
                    {order.orderStatus === "Đã giao cho ĐVVC" && (
                      <DoneSaleOrderButton
                        saleOrderId={order.id}
                        setConfirmReload={setConfirmReload}
                      />
                    )}
                    {order.orderStatus === "Đã hoàn thành" && (
                      <ReviewButton
                        orderStatus={order.orderStatus}
                        saleOrderId={order.id}
                        setConfirmReload={setConfirmReload}
                      />
                    )}

                    <Button
                      color="orange"
                      className="w-40 text-white rounded-md hover:bg-orange-700"
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
              </div>
            </div>
          ))}
      </div>
    </div>
  );
};

export default GuestOrder;
