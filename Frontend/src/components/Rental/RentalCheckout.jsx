import React, { useEffect, useRef, useState } from "react";
import { useLocation, useNavigate } from "react-router-dom";
import axios from "axios";
import { Button, Tooltip, Typography } from "@material-tailwind/react";
import PaymentMethod from "../Payment/PaymentMethod";
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
  faCircleDollarToSlot,
  faBan,
  faVenusMars,
  faCircle,
  faCircleHalfStroke,
} from "@fortawesome/free-solid-svg-icons";

export default function RentalCheckout() {
  const location = useLocation();
  const navigate = useNavigate();
  const selectedOrder = location.state?.selectedOrder || null;
  const [selectedOption, setSelectedOption] = useState(null);
  const [selectedDeposit, setSelectedDeposit] = useState("");
  const newTabRef = useRef(null);
  const [loading, setLoading] = useState(false);

  const handleOptionChange = (event) => {
    setSelectedOption(event.target.value);
  };

  const handleCheckout = async () => {
    try {
      setLoading(true);
      const token = localStorage.getItem("token");
      const payload = {
        paymentMethodID: selectedOption,
        orderID: selectedOrder.id,
        orderCode: selectedOrder.rentalOrderCode,
        transactionType: selectedDeposit,
      };

      const response = await axios.post(
        "https://capstone-project-703387227873.asia-southeast1.run.app/api/Checkout/checkout-rental-order",
        payload,
        {
          headers: {
            accept: "*/*",
            "Content-Type": "application/json",
            Authorization: `Bearer ${token}`,
          },
        }
      );
      const paymentLink = response.data.data.paymentLink;
      if (selectedOption == 2 || selectedOption == 3) {
        window.location.href = paymentLink;
        return;
      } else navigate("/manage-account/rental-order/");

      if (response.data.isSuccess) {
        const paymentLink = response.data.data.paymentLink;
        newTabRef.current = window.open(paymentLink, "_blank");
      } else {
        alert("Checkout failed: " + response.data.message);
      }
    } catch (err) {
      alert("An error occurred: " + err.message);
    } finally {
      setLoading(false);
    }
  };

  return (
    <>
      <div className="min-h-screen bg-gray-100 py-12 px-4 md:px-8">
        <div className="max-w-7xl mx-auto">
          {/* Page Title */}
          <div className="text-center mb-5 flex items-center justify-between">
            <button
              onClick={() => navigate(-1)}
              className="px-4 py-2 bg-gray-200 rounded hover:bg-gray-300 text-gray-700 font-medium"
            >
              Quay lại
            </button>
            <h2 className="text-orange-500 font-bold text-3xl flex-1 text-center">
              Tiến hành thanh toán
            </h2>
          </div>

          <hr className="mb-5" />

          <div className="grid grid-cols-1 lg:grid-cols-3 gap-8">
            {/* Column 1 */}
            {selectedOrder && (
              <div className="bg-white p-6 shadow-lg rounded-lg">
                <h3 className="text-xl font-bold text-gray-800 mb-4">
                  Thông tin khách hàng
                </h3>
                <hr className="mb-5" />
                <div className="space-y-1">
                  {/* <div className="flex">
                <img
                  src={
                    selectedOrder.orderImage ||
                    "/assets/images/default_package.png"
                  }
                  alt={selectedOrder.orderImage}
                  className="w-40 h-40 object-contain rounded"
                />
              </div>
              <hr className="mb-5" /> */}
                  <div className="flex">
                    <p className="flex items-center gap-2 mb-2">
                      <FontAwesomeIcon
                        icon={faUser}
                        className="text-blue-500"
                      />
                      <span className="font-semibold">Tên:</span>{" "}
                      <i>{selectedOrder.fullName}</i>
                    </p>
                  </div>
                  <div className="flex">
                    <p className="flex items-center gap-2 mb-2">
                      <FontAwesomeIcon
                        icon={faVenusMars}
                        className="text-blue-500 fa-xs"
                      />
                      <span className="font-semibold">Giới tính:</span>
                      <i>{selectedOrder.gender}</i>
                    </p>
                  </div>
                  <div className="flex">
                    <p className="flex items-center gap-2 mb-2">
                      <FontAwesomeIcon
                        icon={faPhone}
                        className="text-blue-500"
                      />
                      <span className="font-semibold">Số điện thoại:</span>
                      <i> {selectedOrder.contactPhone}</i>
                    </p>
                  </div>
                  <div className="flex">
                    <p className="flex items-center gap-2 mb-2">
                      <FontAwesomeIcon
                        icon={faEnvelope}
                        className="text-blue-500"
                      />
                      <span className="font-semibold">Email:</span>{" "}
                      <i>{selectedOrder.email}</i>
                    </p>
                  </div>
                  <div className="flex">
                    <p className="flex items-start gap-2 mb-2">
                      <FontAwesomeIcon
                        icon={faMapMarkerAlt}
                        className="text-blue-500"
                      />
                      <span className="font-semibold flex-shrink-0">
                        Địa chỉ:
                      </span>
                      <span className="break-words">
                        <i>{selectedOrder.address}</i>
                      </span>
                    </p>
                  </div>
                </div>
              </div>
            )}

            {/* Column 2 */}
            {selectedOrder && (
              <div className="bg-white p-6 shadow-lg rounded-lg">
                <div>
                  <h3 className="text-xl font-bold text-gray-800 mb-4">
                    Phương thức thanh toán
                  </h3>
                  <hr className="" />
                  <PaymentMethod
                    selectedOption={selectedOption}
                    handleOptionChange={handleOptionChange}
                  />
                </div>

                <div>
                  <div className="flex">
                    <h3 className="text-xl font-bold text-gray-800 mb-4">
                      Hình thức đặt cọc
                    </h3>
                    <Tooltip
                      content={
                        <div className="w-96 ">
                          <Typography color="white" className="font-medium">
                            Đặt cọc:
                          </Typography>
                          <Typography
                            variant="small"
                            color="white"
                            className="font-normal opacity-80"
                          >
                            + Đặt cọc 50%: Khách thanh toán trước một nửa giá
                            trị thuê
                            <br /> + Đặt cọc 100%: Thanh toán toàn bộ giá trị
                            thuê.
                            <br />
                            Phần còn lại trả sau khi nhận hoặc trả sản phẩm.
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
                        className="h-5 w-5 cursor-pointer text-blue-gray-500"
                      >
                        <path
                          strokeLinecap="round"
                          strokeLinejoin="round"
                          d="M11.25 11.25l.041-.02a.75.75 0 011.063.852l-.708 2.836a.75.75 0 001.063.853l.041-.021M21 12a9 9 0 11-18 0 9 9 0 0118 0zm-9-3.75h.008v.008H12V8.25z"
                        />
                      </svg>
                    </Tooltip>
                  </div>
                  <hr className="mb-5" />
                  <div className="mb-4 pl-6">
                    <label className="inline-flex items-center">
                      <input
                        type="radio"
                        name="option"
                        value="DEPOSIT_50"
                        className="form-radio text-[#FA7D0B]"
                        onChange={() => setSelectedDeposit("DEPOSIT_50")}
                      />
                      <FontAwesomeIcon
                        icon={faCircleHalfStroke}
                        className="text-gray-600 w-6 h-6 ml-3"
                      />
                      <span className="ml-3 text-gray-800 text-lg">
                        Đặt cọc một nửa | 50%
                      </span>
                    </label>
                  </div>
                  <div className="mb-4 pl-6">
                    <label className="inline-flex items-center">
                      <input
                        type="radio"
                        name="option"
                        value="FULL_PAYMENT"
                        className="form-radio text-[#FA7D0B]"
                        onChange={() => setSelectedDeposit("DEPOSIT_100")}
                      />
                      <FontAwesomeIcon
                        icon={faCircle}
                        className="text-gray-600 w-6 h-6 ml-3"
                      />
                      <span className="ml-3 text-gray-800 text-lg">
                        Đặt cọc toàn bộ | 100%
                      </span>
                    </label>
                  </div>
                </div>
              </div>
            )}

            {/* Right Panel: Order Summary */}
            <div className="bg-white p-6 shadow-lg rounded-lg">
              <h3 className="text-xl font-bold text-gray-800 mb-4">
                Thông tin đơn hàng
              </h3>
              <hr className="mb-5" />
              {selectedOrder && (
                <div className="space-y-4">
                  <div className="flex">
                    <p className="flex items-center gap-2 mb-2">
                      <FontAwesomeIcon
                        icon={faShoppingCart}
                        className="text-blue-500"
                      />
                      <span className="font-semibold">Mã đơn hàng:</span>{" "}
                      <i>{selectedOrder.rentalOrderCode}</i>
                    </p>
                  </div>
                  <div className="flex">
                    <p className="flex items-start gap-2 mb-2 w-full">
                      <FontAwesomeIcon
                        icon={faTruck}
                        className="text-blue-500"
                      />
                      <span className="font-semibold flex-shrink-0">
                        Phương thức nhận hàng:
                      </span>
                      <span className="break-words">
                        <i>{selectedOrder.deliveryMethod}</i>
                      </span>
                    </p>
                  </div>
                  {/* <div className="flex">
                    <p className="font-medium text-gray-700 mr-2">
                      Trạng thái đơn hàng:
                    </p>
                    <p className=" text-gray-700">
                      {selectedOrder.orderStatus}
                    </p>
                  </div>
                  <div className="flex">
                    <p className="font-medium text-gray-700 mr-2">
                      Phương thức vận chuyển:{" "}
                    </p>
                    <p className=" text-gray-700">
                      {" "}
                      {selectedOrder.deliveryMethod.replace("_", " ")}
                    </p>
                  </div> */}
                </div>
              )}
              <hr className="mt-5 mb-5" />
              <h4 className="text-base font-semibold text-gray-800 mt-4 mb-2">
                Chi tiết sản phẩm
              </h4>
              <div className="max-h-50vh] overflow-y-auto">
                <div className="space-y-2">
                  {selectedOrder?.childOrders?.$values?.length > 0 ? (
                    selectedOrder.childOrders.$values.map((product) => (
                      <div
                        key={product.cartItemId}
                        className="flex flex-col p-2 hover:bg-gray-100"
                      >
                        <div className="flex items-center">
                          <img
                            src={product.imgAvatarPath}
                            alt={product.productName}
                            className="w-16 h-16 object-cover mr-4"
                          />
                          <div>
                            <p className="font-medium text-orange-600">
                              {product.productName}
                            </p>
                            <p className="text-sm text-gray-500">
                              {product.color} -{product.size} -{" "}
                              {product.condition}%
                            </p>
                            <p className="font-sm text-base text-rose-700">
                              <span className="font-semibold text-black">
                                Đơn giá thuê:
                              </span>{" "}
                              {product.rentPrice.toLocaleString("vi-VN")}₫
                              <span className="font-medium text-sm text-black">
                                {" "}
                                x {product.quantity}
                              </span>
                            </p>
                            <p>
                              <span className="font-semibold">Thời gian:</span>{" "}
                              <i className="text-sm">
                                {new Date(
                                  product.rentalStartDate
                                ).toLocaleDateString()}{" "}
                                -{" "}
                                {new Date(
                                  product.rentalEndDate
                                ).toLocaleDateString()}
                              </i>
                            </p>
                          </div>
                        </div>
                        <p className="text-base font-bold text-gray-700 text-right">
                          Tổng tiền: {product.subTotal.toLocaleString("vi-VN")}₫
                        </p>
                      </div>
                    ))
                  ) : (
                    <div
                      key={selectedOrder.cartItemId}
                      className="flex flex-col p-2 hover:bg-gray-100"
                    >
                      <div className="flex items-center">
                        <img
                          src={selectedOrder.imgAvatarPath}
                          alt={selectedOrder.productName}
                          className="w-16 h-16 object-cover rounded-md mr-4"
                        />
                        <div>
                          <p className="font-medium text-orange-600">
                            {selectedOrder.productName}
                          </p>
                          <p className="text-sm text-gray-500">
                            {selectedOrder.color} -{selectedOrder.size} -{" "}
                            {selectedOrder.condition}%
                          </p>
                          <p className="font-sm text-base text-rose-700">
                            <span className="font-semibold text-black">
                              Đơn giá thuê:
                            </span>{" "}
                            {selectedOrder.rentPrice.toLocaleString("vi-VN")}₫
                            <span className="font-medium text-sm text-black">
                              {" "}
                              x {selectedOrder.quantity}
                            </span>
                          </p>

                          <p>
                            <span className="font-semibold">Thời gian:</span>{" "}
                            <i className="text-sm">
                              {new Date(
                                selectedOrder.rentalStartDate
                              ).toLocaleDateString()}{" "}
                              -{" "}
                              {new Date(
                                selectedOrder.rentalEndDate
                              ).toLocaleDateString()}
                            </i>
                          </p>
                        </div>
                      </div>
                      <p className="text-base font-bold text-gray-700 text-right">
                        Tổng tiền:{" "}
                        {selectedOrder.subTotal.toLocaleString("vi-VN")}₫
                      </p>
                    </div>
                  )}
                </div>
              </div>
              <div className="mt-6 border-t pt-4">
                <div className="flex justify-between mb-2">
                  <p className="font-medium text-gray-700 mr-2">Tạm tính:</p>
                  <p className="font-medium text-gray-700">
                    {selectedOrder.subTotal.toLocaleString("vi-VN")}₫
                  </p>
                </div>
                <div className="flex justify-between mb-2">
                  <p className="font-medium text-gray-700 mr-2">
                    Phí vận chuyển:
                  </p>
                  <p className="font-medium text-gray-700">
                    {selectedOrder.tranSportFee.toLocaleString("vi-VN")}₫
                  </p>
                </div>
                <div className="flex justify-between mb-4">
                  <p className="text-lg font-semibold text-gray-800">
                    Tổng cộng:
                  </p>
                  <p className="text-lg font-semibold text-gray-800">
                    {selectedOrder.totalAmount.toLocaleString("vi-VN")}₫
                  </p>
                </div>
              </div>

              <div className="flex pb-10 justify-center items-center mt-4">
                <Button
                  disabled={loading}
                  className={`mt-6 w-full bg-orange-500 text-white py-2 rounded-md ${
                    loading ? "opacity-50 cursor-not-allowed" : ""
                  }`}
                  onClick={handleCheckout}
                >
                  {loading ? "Đang xử lý..." : "Thanh toán"}
                </Button>
              </div>
            </div>
          </div>
        </div>
      </div>
    </>
  );
}
