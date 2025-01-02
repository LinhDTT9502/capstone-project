import React, { useEffect, useState } from "react";
import { useLocation, Link, useNavigate } from "react-router-dom";
import { Button, Tooltip, Typography } from "@material-tailwind/react";
import { FontAwesomeIcon } from "@fortawesome/react-fontawesome";
import {
  faArrowLeft,
  faUser,
  faEnvelope,
  faPhone,
  faMapMarkerAlt,
  faShoppingCart,
  faMoneyBillWave,
  faCalendarAlt,
  faTruck,
  faBolt,
  faDollarSign,
  faVenusMars,
} from "@fortawesome/free-solid-svg-icons";
import axios from "axios";
import CancelSaleOrderButton from "../components/User/CancelSaleOrderButton";
import DoneSaleOrderButton from "../components/User/DoneSaleOrderButton";
import StarRating from "../components/Product/StarRating";

const statusColors = {
  "Chờ xử lý": "bg-yellow-100 text-yellow-800",
  "Đã xác nhận đơn": "bg-orange-100 text-orange-800",
  "Đã thanh toán": "bg-green-100 text-green-800",
  "Đang xử lý": "bg-purple-100 text-purple-800",
  "Đã giao hàng": "bg-indigo-100 text-indigo-800",
  "Bị trì hoãn": "bg-red-100 text-red-800",
  "Đã hủy": "bg-red-200 text-red-900",
  "Hoàn thành": "bg-teal-100 text-teal-800",
};

const paymentStatusColors = {
  "Đang chờ thanh toán": "text-yellow-800",
  "Đã đặt cọc": "text-orange-800",
  "Đã thanh toán": "text-green-800",
  "Đã hủy": "text-red-800",
};

const GuestOrderDetail = () => {
  const { state } = useLocation();
  const order = state?.order;
  const navigate = useNavigate();
  const [orderCode, setOrderCode] = useState(
    state?.order?.saleOrderCode || null
  );
  const [showModal, setShowModal] = useState(false);
  const [reason, setReason] = useState("");
  const [loading, setLoading] = useState(false);
  const [orderDetail, setOrderDetail] = useState(null);
  const [error, setError] = useState("");
  const [confirmReload, setConfirmReload] = useState(false)
  const [reload, setReload] = useState(false);
  const products = orderDetail?.saleOrderDetailVMs?.$values || [];
  {console.log(order);}
  if (!order) {
    return (
      <div className="flex justify-center items-center min-h-screen bg-gray-100">
        <div className="bg-white p-8 rounded-lg shadow-md text-center">
          <FontAwesomeIcon
            icon={faShoppingCart}
            className="text-6xl text-red-500 mb-4"
          />
          <h2 className="text-2xl font-semibold text-gray-800 mb-4">
            Order not found
          </h2>
          <Link
            to="/guest-order"
            className="inline-flex items-center justify-center px-4 py-2 border border-transparent text-base font-medium rounded-md text-white bg-orange-600 hover:bg-orange-700 transition duration-150 ease-in-out"
          >
            <FontAwesomeIcon icon={faArrowLeft} className="mr-2" />
            Back to Orders
          </Link>
        </div>
      </div>
    );
  }

  useEffect(() => {
    const fetchOrderDetail = async () => {
      try {
        const response = await axios.get(
          `https://capstone-project-703387227873.asia-southeast1.run.app/api/SaleOrder/get-order-by-code?orderCode=${orderCode}`,
          { headers: { accept: "*/*" } }
        );
        if (response.data.isSuccess) {
          setOrderDetail(response.data.data);
        } else {
          setError("Failed to fetch order details.");
        }
      } catch (err) {
        setError(
          err.message || "An error occurred while fetching order details."
        );
      }
    };

    if (orderCode) {
      fetchOrderDetail();
    }
  }, [orderCode, reload, confirmReload]);

if (error) {
  return (
    <div className="text-center text-red-600">
      <p>{error}</p>
      <Link to="/guest-order" className="text-blue-600 underline mt-4 block">
        Back to Orders
      </Link>
    </div>
  );
}

if (!orderDetail) {
  return (
    <div className="flex justify-center items-center min-h-screen">
      <p>Loading order details...</p>
    </div>
  );
}
  const {
    id,
    saleOrderCode,
    fullName,
    email,
    contactPhone,
    address,
    paymentStatus,
    orderStatus,
    deliveryMethod,
    saleOrderDetailVMs,
    createdAt,
    totalAmount,
    updatedAt,
  } = orderDetail;

  const formatCurrency = (amount) => {
    return new Intl.NumberFormat("vi-VN", {
      style: "currency",
      currency: "VND",
    }).format(amount);
  };
  return (
    <div className="container mx-auto p-4 min-h-screen bg-gray-200">
      <div className="max-w-4xl mx-auto">
        <div className="flex justify-between items-center mb-6">
          <button
            onClick={() => navigate(-1)}
            className="flex items-center gap-2 text-blue-500 hover:text-blue-700"
          >
            <FontAwesomeIcon icon={faArrowLeft} />
            Quay lại
          </button>
        </div>

        {orderStatus === "Đã hủy" && (
          <div className="bg-yellow-50 p-4 rounded-lg shadow-sm mb-6">
            <p className="text-xl">
              <b className="text-red-500">Đã hủy đơn hàng </b>
              <i className="text-lg">
                vào lúc{" "}
                {new Date(updatedAt).toLocaleString("vi-VN", {
                  day: "2-digit",
                  month: "2-digit",
                  year: "numeric",
                  hour: "2-digit",
                  minute: "2-digit",
                  second: "2-digit",
                })}
              </i>
            </p>
            <p className="flex items-center gap-2 mb-2">
              <FontAwesomeIcon icon={faBolt} style={{ color: "#fd7272" }} />
              <span className="font-semibold">Lý do:</span> {orderDetail.reason}
            </p>
          </div>
        )}
        {paymentStatus === "Đã thanh toán" && (
          <div className="bg-green-500 p-4 rounded-lg shadow-sm mb-6">
            <p className="text-xl">
              <b className="text-white">Đơn hàng đã được thanh toán </b>
              <i className="text-lg">
                vào lúc{" "}
                {new Date(paymentDate).toLocaleString("vi-VN", {
                  day: "2-digit",
                  month: "2-digit",
                  year: "numeric",
                  hour: "2-digit",
                  minute: "2-digit",
                  second: "2-digit",
                })}{" "}
                - {orderDetail.paymentMethod}
              </i>
            </p>
            <p className="flex items-center gap-2 mb-2 text-white">
              <FontAwesomeIcon
                icon={faDollarSign}
                style={{ color: "#FFD43B" }}
              />
              <span className="font-semibold">Số tiền: </span>
              {totalAmount.toLocaleString("Vi-vn")}₫
            </p>
          </div>
        )}
        <div className="bg-white p-6 rounded-lg shadow-lg mb-6">
          <div className="py-4 bg-white rounded-md shadow-sm">
            <div className="flex justify-between items-center">
              <h2 className="text-2xl font-bold text-gray-800 flex-1">
                Chi tiết đơn hàng -{" "}
                <span className="text-orange-500">#{saleOrderCode}</span>
              </h2>
              <div className="flex items-center gap-4">
                {paymentStatus === "N/A" && orderStatus === "Chờ xử lý" && (
                  <Button
                    size="sm"
                    className="w-40 text-green-700  bg-white border border-green-700 rounded-md hover:bg-green-200"
                    onClick={() =>
                      navigate("/checkout", {
                        state: { selectedOrder: orderDetail },
                      })
                    }
                  >
                    Thanh toán
                  </Button>
                )}
                {orderDetail.orderStatus === "Chờ xử lý" && (
                  <CancelSaleOrderButton
                    saleOrderId={id}
                    setReload={setReload}
                  />
                )}
              </div>
            </div>
          </div>
          <hr className="mb-5" />
          <div className="grid md:grid-cols-2 gap-6">
            <div>
              <h3 className="text-lg font-semibold mb-2 text-gray-700">
                Thông tin khách hàng
              </h3>

              {console.log(orderDetail)}
              <p className="flex items-center gap-2 mb-2">
                <FontAwesomeIcon icon={faUser} className="text-blue-500" />
                <span className="font-semibold">Tên:</span> <i>{fullName}</i>
              </p>
              <p className="flex items-center gap-2 mb-2">
                <FontAwesomeIcon
                  icon={faVenusMars}
                  className="text-blue-500 fa-xs"
                />
                <span className="font-semibold">Giới tính:</span>
                <i>{orderDetail.gender}</i>
              </p>
              <p className="flex items-center gap-2 mb-2">
                <FontAwesomeIcon icon={faEnvelope} className="text-blue-500" />
                <span className="font-semibold">Email:</span> <i>{email}</i>
              </p>
              <p className="flex items-center gap-2 mb-2">
                <FontAwesomeIcon icon={faPhone} className="text-blue-500" />
                <span className="font-semibold">Số điện thoại:</span>
                <i> {contactPhone}</i>
              </p>
              <p className="flex items-start gap-2 mb-2">
                <FontAwesomeIcon
                  icon={faMapMarkerAlt}
                  className="text-blue-500"
                />
                <span className="font-semibold flex-shrink-0">Địa chỉ:</span>
                <span className="break-words">
                  <i>{address}</i>
                </span>
              </p>
            </div>
            <div>
              <h3 className="text-lg font-semibold mb-2 text-gray-700">
                Thông tin đơn hàng
              </h3>
              <p className="flex items-center gap-2 mb-2">
                <FontAwesomeIcon
                  icon={faShoppingCart}
                  className="text-blue-500"
                />
                <span className="font-semibold">Mã đơn hàng:</span>{" "}
                <i>{saleOrderCode}</i>
              </p>
              <p className="flex items-start gap-2 mb-2 w-full">
                <FontAwesomeIcon icon={faTruck} className="text-blue-500" />
                <span className="font-semibold flex-shrink-0">
                  Phương thức nhận hàng:
                </span>
                <span className="break-words">
                  <i>{deliveryMethod}</i>
                </span>
              </p>
              <p className="flex items-center gap-2 mb-2">
                <FontAwesomeIcon
                  icon={faMoneyBillWave}
                  className="text-blue-500"
                />
                <span className="font-semibold">Tình trạng thanh toán:</span>{" "}
                <span
                  className={`py-2 px-2.5 mr-1.5 rounded-full text-xs font-bold ${
                    statusColors[paymentStatus] || "bg-gray-100 text-gray-800"
                  }`}
                >
                  {paymentStatus}
                </span>
              </p>
              <p className="flex items-center gap-2 mb-2">
                <FontAwesomeIcon
                  icon={faCalendarAlt}
                  className="text-blue-500"
                />
                <span className="font-semibold">Tình trạng đơn hàng:</span>{" "}
                <span
                  className={`px-2.5 py-2 mr-5 rounded-full text-xs font-bold ${
                    statusColors[orderStatus] || "bg-gray-100 text-gray-800"
                  }`}
                >
                  {orderStatus}
                </span>
              </p>
            </div>
          </div>
          <div className="flex justify-end items-center gap-3 mt-5">
            {orderStatus === "Đã giao cho đơn vị vận chuyển" && (
              <DoneSaleOrderButton
                saleOrderId={id}
                setConfirmReload={setConfirmReload}
              />
            )}
          </div>
        </div>

        <div className="bg-gray-50 p-4 rounded-lg shadow-sm">
          {products.map((product) => (
            <div
              key={product.productId}
              className="bg-gray-50 p-4 mb-4 rounded-lg shadow-sm"
            >
              <div className="flex flex-col md:flex-row gap-4">
                <img
                  src={product.imgAvatarPath}
                  alt={product.productName}
                  className="w-full md:w-32 h-32 object-cover rounded"
                />
                <div className="flex-grow">
                  <h4 className="font-semibold text-lg mb-2 text-orange-500">
                    <Link to={`/product/${product.productCode}`}>
                      {product.productName}
                    </Link>
                  </h4>
                  <div className="grid grid-cols-2 gap-2">
                    <p>
                      <span className="font-semibold">Màu sắc:</span>{" "}
                      <i>{product.color}</i>
                    </p>
                    <p>
                      <span className="font-semibold">Số lượng:</span>{" "}
                      <i>{product.quantity}</i>
                    </p>
                    <p>
                      <span className="font-semibold">Kích thước:</span>{" "}
                      <i>{product.size}</i>
                    </p>
                    <p>
                      <span className="font-semibold">Đơn giá bán:</span>{" "}
                      <i>{product.unitPrice.toLocaleString("Vi-VN")}₫</i>
                    </p>
                    <p>
                      <span className="font-semibold">Tình trạng:</span>{" "}
                      <i>{product.condition}%</i>
                    </p>
                    <p className="flex items-center">
                      <span className="flex items-center gap-1">
                        <span className="font-semibold">Tổng tiền:</span>
                        <i className="ml-2">
                          {product.totalAmount.toLocaleString("vi-VN")}₫
                        </i>
                        <Tooltip
                          content={
                            <div className="w-80">
                              <Typography color="white" className="font-medium">
                                Tổng tiền:
                              </Typography>
                              <Typography
                                variant="small"
                                color="white"
                                className="font-normal opacity-80"
                              >
                                Tổng tiền được tính: Số lượng x Đơn giá bán
                              </Typography>
                            </div>
                          }
                        >
                          <svg
                            xmlns="http://www.w3.org/2000/svg"
                            fill="none"
                            viewBox="0 0 24 24"
                            stroke="currentColor"
                            strokeWidth={2}
                            className="h-4 w-4 cursor-pointer text-blue-gray-500"
                          >
                            <path
                              strokeLinecap="round"
                              strokeLinejoin="round"
                              d="M11.25 11.25l.041-.02a.75.75 0 011.063.852l-.708 2.836a.75.75 0 001.063.853l.041-.021M21 12a9 9 0 11-18 0 9 9 0 0118 0zm-9-3.75h.008v.008H12V8.25z"
                            />
                          </svg>
                        </Tooltip>
                      </span>
                    </p>
                  </div>
                </div>
              </div>
            </div>
          ))}
        </div>
        <div className="bg-white p-6 rounded-lg shadow-lg mt-6 text-gray-700">
          <p className="text-xl flex justify-between">
            <b>Tạm tính: </b>
            <i>{orderDetail.subTotal.toLocaleString("vi-VN")}₫</i>
          </p>
          <p className="flex justify-between">
            <b className="text-xl py-2 ">Phí vận chuyển: </b>
            <p className="text-sm py-2 ">
              {orderDetail.transportFee || "2Sport sẽ liên hệ và thông báo sau"}
            </p>
          </p>
          <p className="text-xl flex justify-between items-center text-gray-700">
            <span className="flex items-center gap-1">
              <b>Thành tiền:</b>
            </span>
            <i className="text-orange-500 font-bold">
              {orderDetail.totalAmount.toLocaleString("vi-VN")}₫
            </i>
          </p>
        </div>
      </div>
    </div>
  );
};

export default GuestOrderDetail;
