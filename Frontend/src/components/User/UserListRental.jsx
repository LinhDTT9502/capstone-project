import React, { useState, useEffect } from "react";
import { Button } from "@material-tailwind/react";
import { useSelector } from "react-redux";
import axios from "axios";
import { selectUser } from "../../redux/slices/authSlice";
import { Link, useNavigate } from "react-router-dom";
import {
  faArrowLeft,
  faCaretDown,
  faCaretUp,
  faShoppingBag,
} from "@fortawesome/free-solid-svg-icons";
import { FontAwesomeIcon } from "@fortawesome/react-fontawesome";
import CancelRentalOrderButton from "./CancelRentalOrderButton";
import { toast } from "react-toastify";
import DoneRentalOrderButton from "./DoneRentalOrderButton";
import ReturnRequestButton from "../../pages/ReturnPage";
import RentalRefundRequestForm from "../Refund/RentalRefundRequestForm";
import RefundRequestPopup from "../Order/RefundRequestPopup";

const statusColors = {
  "Chờ xử lý": "bg-yellow-100 text-yellow-800",
  "Đã xác nhận": "bg-orange-100 text-orange-800",
  "Đang xử lý": "bg-purple-100 text-purple-800",
  "Đã giao hàng": "bg-indigo-100 text-indigo-800",
  "Đã giao cho ĐVVC": "bg-blue-100 text-blue-800",
  "Đã hủy": "bg-red-200 text-red-900",
  "Đang gia hạn": "bg-fuchsia-200 text-fuchsia-900",
  "Đã hoàn thành": "bg-green-100 text-green-800",
  "Đang thuê": "bg-gray-200 text-gray-800",
};

const paymentStatusColors = {
  "Đang chờ thanh toán": "text-yellow-800",
  "Đã đặt cọc": "text-blue-800",
  "Đã thanh toán": "text-green-800",
  "Đã hủy": "btext-red-800",
};

const depositStatusColors = {
  "Đã thanh toán": "text-green-800",
  "Đã thanh toán một phần": "text-blue-800",
  "Chưa thanh toán": "text-yellow-800",
  "Đã hoàn trả": "text-red-800",
};

export default function UserListRental() {
  const user = useSelector(selectUser);
  const [selectedStatus, setSelectedStatus] = useState("Tất cả");
  const [rentalOrders, setRentalOrders] = useState([]);
  const [filteredRentalOrders, setFilteredRentalOrders] = useState([]);
  const [isLoading, setIsLoading] = useState(true);
  const [error, setError] = useState(null);
  const navigate = useNavigate();
  const [expandedOrderId, setExpandedOrderId] = useState(null);
  const [searchQuery, setSearchQuery] = useState("");
  const [reload, setReload] = useState(false);
  const [confirmReload, setConfirmReload] = useState(false);
  const [refundReload, setRefundReload] = useState(false);

  useEffect(() => {
    const fetchRentalOrders = async () => {
      try {
        const token = localStorage.getItem("token");
        const response = await axios.get(
          `https://twosport-api-offcial-685025377967.asia-southeast1.run.app/api/RentalOrder/get-rental-order-by-user?userId=${user.UserId}`,
          {
            headers: {
              accept: "*/*",
              Authorization: `Bearer ${token}`,
            },
          }
        );
        if (response.data.isSuccess) {
          const sortedOrders = response.data.data.$values.sort((a, b) => {
            return new Date(b.createdAt) - new Date(a.createdAt);
          });
          setRentalOrders(sortedOrders);
          setFilteredRentalOrders(sortedOrders);
        } else {
          setError("Không thể lấy danh sách đơn thuê.");
        }
      } catch (err) {
        setError(err.message || "Không thể lấy danh sách đơn thuê.");
      } finally {
        setIsLoading(false);
      }
    };
    fetchRentalOrders();
  }, [user.UserId, reload, confirmReload, refundReload]);

  const handleSearch = () => {
    if (!searchQuery) {
      setFilteredRentalOrders(rentalOrders);
      return;
    }
    toast.info(`Tìm kiếm với từ khóa: ${searchQuery}`);
    const filtered = rentalOrders.filter((order) => {
      return (
        order.rentalOrderCode
          ?.toLowerCase()
          .includes(searchQuery.toLowerCase()) ||
        order.productName?.toLowerCase().includes(searchQuery.toLowerCase()) ||
        order.color?.toLowerCase().includes(searchQuery.toLowerCase()) ||
        order.size?.toLowerCase().includes(searchQuery.toLowerCase()) ||
        order.childOrders?.$values?.some((item) => {
          return (
            item.productName
              ?.toLowerCase()
              .includes(searchQuery.toLowerCase()) ||
            item.color?.toLowerCase().includes(searchQuery.toLowerCase()) ||
            item.size?.toLowerCase().includes(searchQuery.toLowerCase())
          );
        })
      );
    });

    if (filtered.length === 0) {
      toast.info("Không tìm thấy sản phẩm nào khớp với từ khóa");
    }
    setFilteredRentalOrders(filtered);
  };

 const groupedOrders = filteredRentalOrders.reduce(
   (acc, order) => {
     if (!order.parentOrderCode) {
       acc.parents.push(order);
     } else {
       acc.children[order.parentOrderCode] =
         acc.children[order.parentOrderCode] || [];
       acc.children[order.parentOrderCode].push(order);
     }
     return acc;
   },
   { parents: [], children: {} }
 );
  const filteredOrders =
    selectedStatus === "Tất cả"
      ? groupedOrders.parents
      : groupedOrders.parents.filter(
          (order) => order.orderStatus === selectedStatus
        );

  const toggleExpand = (orderId) => {
    setExpandedOrderId((prev) => (prev === orderId ? null : orderId));
  };
  
  if (isLoading)
    return (
      <div className="flex justify-center items-center h-64">
        <div className="animate-spin rounded-full h-32 w-32 border-t-2 border-b-2 border-orange-500">
        </div>
      </div>
    );
  // if (error)
 
  //   return (
  //     <div className="text-center text-gray-500 mt-32 flex flex-col items-center justify-center">
  //       <FontAwesomeIcon icon={faShoppingBag} className="text-6xl mb-2" />
  //       { console.log(filteredOrders)}
  //       <p>Bạn chưa có sản phẩm nào</p>
  //     </div>
  //   );

  return (
    <div className="container mx-auto pt-2 rounded-lg max-w-5xl">
      <h2 className="text-orange-500 font-bold text-2xl pb-2">
        Danh sách đơn thuê{" "}
      </h2>
      {/* Trạng thái filter */}
      <div className="rounded-lg overflow-x-auto w-full">
        <div className="flex justify-start p-4 bg-gray-50 border-b space-x-2 whitespace-nowrap">
          {[
            "Tất cả",
            "Chờ xử lý",
            "Đã xác nhận",
            "Đã giao cho ĐVVC",
            "Đã giao hàng",
            "Đang thuê",
            "Đang gia hạn",
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
        {filteredOrders.map((parent) => (
          <div
            key={parent.id}
            className="border border-gray-200 rounded-lg shadow-sm mt-4"
          >
            <div
              className="flex justify-between items-center p-2 cursor-pointer hover:bg-slate-200 transition-colors duration-150 ease-in-out"
              onClick={() => toggleExpand(parent.id)}
            >
              <div className="flex flex-col w-3/4 pl-4">
                <h4 className="font-bold text-lg text-gray-800">
                  Mã đơn hàng:{" "}
                  <span className="text-orange-500">
                    {parent.rentalOrderCode}
                  </span>
                  <span
                    className={`px-3 py-1 ml-2.5 rounded-full text-xs font-medium ${
                      statusColors[parent.orderStatus] ||
                      "bg-gray-100 text-gray-800"
                    }`}
                  >
                    {parent.orderStatus}
                  </span>
                </h4>
                <p className=" text-gray-600">
                  Trạng thái thanh toán:
                  <span
                    className={`ml-2 font-medium ${
                      paymentStatusColors[parent.paymentStatus] ||
                      "text-gray-800"
                    }`}
                  >
                    {parent.paymentStatus}
                  </span>
                </p>
                <p className="text-gray-600">
                  Hình thức nhận hàng: <i>{parent.deliveryMethod}</i>
                </p>
                <p className="text-gray-600">
                  Ngày đặt:{" "}
                  <i>{new Date(parent.createdAt).toLocaleDateString()}</i>
                </p>
              </div>
              <div className="flex flex-col w-1/4 h-auto items-end">
                <img
                  src={
                    parent.orderImage || "/assets/images/default_package.png"
                  }
                  alt={parent.orderImage}
                  className="w-32 h-32 object-contain rounded"
                />
              </div>
            </div>

            {expandedOrderId === parent.id && (
              <div className="mt-4 pl-8 border-l">
                {groupedOrders.children[parent.rentalOrderCode]?.length > 0 ? (
                  groupedOrders.children[parent.rentalOrderCode].map(
                    (child) => (
                      <div
                        key={child.id}
                        className="flex p-2 border-b last:border-none cursor-pointer gap-4"
                      >
                        {/* Hình ảnh */}
                        <div className="flex-none">
                          <img
                            src={child.imgAvatarPath || "default-image.jpg"}
                            alt="Order"
                            className="w-24 h-24 object-contain rounded"
                          />
                        </div>
                        {/* Nội dung */}
                        <div className="flex-1">
                          <h5 className="font-medium text-base">
                            {child.productName}
                          </h5>
                          <p className="text-sm text-gray-500">
                            Màu sắc: {child.color} - Kích thước: {child.size} -
                            Tình trạng: {child.condition}%
                          </p>
                          <p className="font-medium text-base text-rose-700">
                            Giá thuê:{" "}
                            {new Intl.NumberFormat("vi-VN", {
                              style: "currency",
                              currency: "VND",
                            }).format(child.rentPrice)}
                          </p>
                          <p className="font-semibold text-sm">
                            Số lượng: {child.quantity}
                          </p>
                          <p className="font-semibold text-sm text-gray-500">
                            Thời gian thuê:{" "}
                            {child.rentalStartDate.split("T")[0]} -{" "}
                            {child.rentalEndDate.split("T")[0]}
                          </p>
                        </div>
                      </div>
                    )
                  )
                ) : (
                  <div className="flex p-2 gap-4">
                    {/* Hình ảnh */}
                    <div className="flex-none">
                      <img
                        src={parent.imgAvatarPath || "default-image.jpg"}
                        alt="Order"
                        className="w-24 h-24 object-contain rounded"
                      />
                    </div>
                    {/* Nội dung */}
                    <div className="flex-1">
                      <h3 className="font-medium text-base">
                        {parent.productName}
                      </h3>
                      <p className="text-sm text-gray-500">
                        Màu sắc: {parent.color} - Kích thước: {parent.size} -
                        Tình trạng: {parent.condition}%
                      </p>
                      <p className="font-medium text-sm text-rose-700">
                        Giá thuê:{" "}
                        {new Intl.NumberFormat("vi-VN", {
                          style: "currency",
                          currency: "VND",
                        }).format(parent.rentPrice)}
                      </p>
                      <p className="font-semibold text-sm">
                        Số lượng: {parent.quantity}
                      </p>
                      <p className="font-semibold text-sm text-gray-500">
                        Thời gian thuê: {parent.rentalStartDate.split("T")[0]} -{" "}
                        {parent.rentalEndDate.split("T")[0]}
                      </p>
                    </div>
                  </div>
                )}
              </div>
            )}
            <div>
              <div className="h-px bg-gray-300 my-2 sm:my-2"></div>
              <div className="flex items-center justify-between my-4 px-2">
                <p className="text-gray-600 font-semibold text-lg pl-2  ">
                  Thành tiền:{" "}
                  <span className="text-orange-500">
                    {parent.totalAmount.toLocaleString("Vi-vn")}₫
                  </span>
                </p>
                <div className="flex gap-2">
                  {/* Thanh toan button */}
                  {parent.paymentStatus !== "Đã đặt cọc" &&
                    (parent.orderStatus === "Đã xác nhận" ||
                      parent.orderStatus === "Chờ xử lý") && (
                      <Button
                        color="white"
                        size="sm"
                        className="w-40 text-blue-700 border border-blue-700 rounded-md hover:bg-blue-200"
                        onClick={() =>
                          navigate("/rental-checkout", {
                            state: { selectedOrder: parent },
                          })
                        }
                      >
                        Thanh toán
                      </Button>
                    )}
                  {/* {parent.orderStatus === "Đã hủy" &&
                    parent.depositAmount > 0 && (
                      <button
                        className="text-red-700 bg-white border border-red-700 rounded-md hover:bg-red-200 px-4 py-2"
                        onClick={() => navigate("/return")}
                      >
                        Trả Hàng/Hoàn Tiền
                      </button>
                    )} */}
                  {parent.orderStatus === "Đã hủy" &&
                    parent.depositAmount > 0 &&
                    (parent.refundRequests === null)&& (
                      <RentalRefundRequestForm
                        orderDetail={parent}
                        setRefundReload={setRefundReload}
                      />
                    )}
                  {(parent.refundRequests !== null) && (
                      <RefundRequestPopup
                        refundRequests={parent.refundRequests}
                      />
                    )}
                  {parent.orderStatus === "Chờ xử lý" && (
                    <CancelRentalOrderButton
                      rentalOrderId={parent.id}
                      setReload={setReload}
                    />
                  )}
                  {parent.orderStatus === "Đã giao cho ĐVVC" && (
                    <DoneRentalOrderButton
                      rentalOrderId={parent.id}
                      setConfirmReload={setConfirmReload}
                    />
                  )}
                  <Button
                    color="orange"
                    size="sm"
                    className="w-40"
                    onClick={(e) => {
                      e.stopPropagation();
                      navigate(
                        `/manage-account/user-rental/${parent.rentalOrderCode}`
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
      </div>
    </div>
  );
}
