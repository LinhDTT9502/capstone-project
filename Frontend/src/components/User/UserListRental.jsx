import React, { useState, useEffect } from "react";
import { Button } from "@material-tailwind/react";
import { useSelector } from "react-redux";
import axios from "axios";
import { selectUser } from "../../redux/slices/authSlice";
import { useNavigate } from "react-router-dom";

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
        acc.children[order.parentOrderCode] = acc.children[order.parentOrderCode] || [];
        acc.children[order.parentOrderCode].push(order);
      }
      return acc;
    },
    { parents: [], children: {} }
  );

  const toggleExpand = (orderId) => {
    setExpandedOrderId((prev) => (prev === orderId ? null : orderId));
  };

  if (isLoading) return <p>Đang tải danh sách đơn thuê...</p>;
  if (error) return <p>Lỗi: {error}</p>;

  return (
    <div className="container mx-auto max-h-[70vh] overflow-y-auto">
      <div className="text-sm font-medium text-center text-gray-500 border-b border-gray-200">
        <ul className="flex flex-wrap -mb-px justify-center">
          {["Tất cả", "Đang chờ", "Xác nhận", "Đã thanh toán", "Đang xử lý", "Đã giao", "Bị hoãn", "Hoàn thành"].map(
            (status) => (
              <li key={status} className="mr-6">
                <button
                  className={`inline-block p-4 border-b-2 rounded-t-lg ${selectedStatus === status
                      ? "text-orange-500 border-orange-500"
                      : "hover:text-gray-600 hover:border-gray-300"
                    }`}
                  onClick={() => setSelectedStatus(status)}
                >
                  {status}
                </button>
              </li>
            )
          )}
        </ul>
      </div>

      {groupedOrders.parents.map((parent) => (
        <div key={parent.id} className="p-4 border border-gray-200 rounded-lg shadow-sm mt-4">
          <div
            className="flex justify-between cursor-pointer"
            onClick={() => toggleExpand(parent.id)}
          >
            <div className="flex">
              <div className="ml-4">
                <h4 className="font-semibold text-lg">Mã đơn hàng: {parent.rentalOrderCode}</h4>
                <p className="text-gray-500">Hình thức nhận hàng: {parent.deliveryMethod}</p>
                <p className="text-gray-500">Trạng thái: {parent.orderStatus}</p>
                <p className="text-gray-500">Ngày đặt: {parent.createdAt}</p>
                <p className="mt-2 font-bold">Tổng tiền: {parent.totalAmount}</p>
              </div>
            </div>
            <button onClick={() => navigate(`/manage-account/user-rental/${parent.id}`)}>Xem chi tiết</button>
          </div>

          {expandedOrderId === parent.id && (
            <div className="mt-4 pl-8 border-l">
              {groupedOrders.children[parent.rentalOrderCode]?.length > 0 ? (
                groupedOrders.children[parent.rentalOrderCode].map((child) => (
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
                      <h5 className="font-medium text-base">{child.productName}</h5>
                      <p className="text-sm text-gray-500">{child.color}</p>
                      <p className="text-sm text-gray-500">Số tiền: {child.totalAmount}</p>
                    </div>
                  </div>
                ))
              ) : (
                <div>
                  <img
                    src={parent.imgAvatarPath || "default-image.jpg"}
                    alt=" Order"
                    className="w-24 h-24 object-cover rounded"
                  />
                  <div>
                    <h5 className="font-medium text-base">Mã đơn hàngg: {parent.rentalOrderCode}</h5>
                    <p className="text-sm text-gray-500">{parent.productName}</p>
                    <p className="text-sm text-gray-500">Số tiền: {parent.totalAmount}</p>
                  </div>
                </div>
              )}
            </div>
          )}
        </div>
      ))}
    </div>
  );
}
