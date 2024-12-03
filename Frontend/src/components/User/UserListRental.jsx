import React, { useState, useEffect } from "react";
import { Button } from "@material-tailwind/react";
import { useSelector } from "react-redux";
import axios from "axios";
import { selectUser } from "../../redux/slices/authSlice";
import { useNavigate } from "react-router-dom";
import { faCaretDown, faCaretUp } from "@fortawesome/free-solid-svg-icons";
import { FontAwesomeIcon } from "@fortawesome/react-fontawesome";

const statusColors = {
  "Đang chờ": "bg-yellow-100 text-yellow-800",
  "Xác nhận": "bg-blue-100 text-blue-800",
  "Đã thanh toán": "bg-green-100 text-green-800",
  "Đang xử lý": "bg-purple-100 text-purple-800",
  "Đã giao": "bg-indigo-100 text-indigo-800",
  "Bị hoãn": "bg-red-100 text-red-800",
  "Hoàn thành": "bg-teal-100 text-teal-800",
};

export default function UserListRental() {
  const user = useSelector(selectUser);
  const [selectedStatus, setSelectedStatus] = useState("Tất cả");
  const [rentalOrders, setRentalOrders] = useState([]);
  const [isLoading, setIsLoading] = useState(true);
  const [error, setError] = useState(null);
  const navigate = useNavigate();
  const [expandedOrderId, setExpandedOrderId] = useState(null);

  useEffect(() => {
    const fetchRentalOrders = async () => {
      try {
        const token = localStorage.getItem("token");
        const response = await axios.get(
          `https://capstone-project-703387227873.asia-southeast1.run.app/api/RentalOrder/get-rental-order-by-user?userId=${user.UserId}`,
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
          console.log(rentalOrders);
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
  }, [user.UserId]);

  const groupedOrders = rentalOrders.reduce(
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

  const toggleExpand = (orderId) => {
    setExpandedOrderId((prev) => (prev === orderId ? null : orderId));
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
    <div className="container mx-auto pt-2 rounded-lg max-w-4xl">
      <h2 className="text-orange-500 font-bold text-2xl">
        Danh sách đơn thuê{" "}
      </h2>
      <div className=" rounded-lg overflow-hidden">
        <div className="flex flex-wrap justify-center p-4 bg-gray-50 border-b">
          {[
            "Tất cả",
            "Đang chờ",
            "Xác nhận",
            "Đã thanh toán",
            "Đang xử lý",
            "Đã giao",
            "Bị hoãn",
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
      <div className="space-y-6">
        {groupedOrders.parents.map((parent) => (
          <div
            key={parent.id}
            className="p-4 border border-gray-200 rounded-lg shadow-sm mt-4"
          >
            <div
              className="flex justify-between items-center p-6 cursor-pointer hover:bg-gray-50 transition-colors duration-150 ease-in-out"
              onClick={() => toggleExpand(parent.id)}
            >
              <div>
                <h4 className="font-semibold text-lg text-gray-800">
                  Mã đơn hàng: {parent.rentalOrderCode}
                </h4>
                <p className="text-gray-600">
                  Hình thức nhận hàng: {parent.deliveryMethod}
                </p>
                <p className="text-gray-600">
                  Ngày đặt: {new Date(parent.createdAt).toLocaleDateString()}
                </p>
                <p className="mt-2 font-bold text-lg text-orange-500">
                  Tổng tiền:{" "}
                  {new Intl.NumberFormat("vi-VN", {
                    style: "currency",
                    currency: "VND",
                  }).format(parent.totalAmount)}
                </p>
              </div>
              <div className="flex flex-col items-end">
                <span
                  className={`px-3 py-1 rounded-full text-xs font-medium ${
                    statusColors[parent.orderStatus] ||
                    "bg-gray-100 text-gray-800"
                  }`}
                >
                  {parent.orderStatus}
                </span>
                <Button
                  color="orange"
                  size="sm"
                  className="mt-2"
                  onClick={(e) => {
                    e.stopPropagation();
                    navigate(`/manage-account/user-rental/${parent.id}`);
                  }}
                >
                  Xem chi tiết
                </Button>
              </div>
              {/* {expandedOrderId === parent.id ? (
                <FontAwesomeIcon
                  icon={faCaretDown}
                  className="w-6 h-6 text-gray-500"
                />
              ) : (
                <FontAwesomeIcon
                  icon={faCaretUp}
                  className="w-6 h-6 text-gray-500"
                />
              )} */}
            </div>

            {expandedOrderId === parent.id && (
              <div className="mt-4 pl-8 border-l">
                {groupedOrders.children[parent.rentalOrderCode]?.length > 0 ? (
                  groupedOrders.children[parent.rentalOrderCode].map(
                    (child) => (
                      <div
                        key={child.id}
                        className="flex p-2 border-b last:border-none cursor-pointer"
                      >
                        <img
                          src={child.imgAvatarPath || "default-image.jpg"}
                          alt=" Order"
                          className="w-24 h-24 object-cover rounded"
                        />
                        <div>
                          <h5 className="font-medium text-base">
                            {child.productName}
                          </h5>
                          <p className="text-sm text-gray-500">{child.color}</p>
                          <p className="text-sm text-gray-500">
                            Số tiền: {child.totalAmount}
                          </p>
                        </div>
                      </div>
                    )
                  )
                ) : (
                  <div>
                    <img
                      src={parent.imgAvatarPath || "default-image.jpg"}
                      alt=" Order"
                      className="w-24 h-24 object-cover rounded"
                    />
                    <div>
                      <h5 className="font-medium text-base">
                        Mã đơn hàngg: {parent.rentalOrderCode}
                      </h5>
                      <p className="text-sm text-gray-500">
                        {parent.productName}
                      </p>
                      <p className="text-sm text-gray-500">
                        Số tiền: {parent.totalAmount}
                      </p>
                    </div>
                  </div>
                )}
              </div>
            )}
          </div>
        ))}
      </div>
    </div>
  );
}
