import React, { useEffect, useRef, useState } from "react";
import { useLocation, useNavigate } from "react-router-dom";
import axios from "axios";
import { Button } from "@material-tailwind/react";
import PaymentMethod from "../Payment/PaymentMethod";

export default function RentalCheckout() {
  const location = useLocation();
  const navigate = useNavigate();
  const orderDetail = location.state?.selectedOrder;
  const [selectedOption, setSelectedOption] = useState(null);
  const [selectedDeposit, setSelectedDeposit] = useState("");
  const newTabRef = useRef(null);

  const handleDepositChange = (value) => {
    if (value === "DEPOSIT_50") {
      setSelectedDeposit("DEPOSIT_50");
    } else {
      setSelectedDeposit(null);
    }
  };

  const handleOptionChange = (event) => {
    setSelectedOption(event.target.value);
  };

  const handleCheckout = async () => {
    try {
      const token = localStorage.getItem("token");
      const payload = {
        paymentMethodID: selectedOption,
        orderID: orderDetail.id,
        orderCode: orderDetail.rentalOrderCode,
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

      if (response.data.isSuccess) {
        const paymentLink = response.data.data.paymentLink;
        newTabRef.current = window.open(paymentLink, "_blank");
      } else {
        alert("Checkout failed: " + response.data.message);
      }
    } catch (err) {
      alert("An error occurred: " + err.message);
    }
  };

  return (
    <div className="flex bg-slate-200">
      <div className="basis-2/3 bg-white p-6 px-12 py-10">
        <div className="bg-white mt-2">
          <h3 className="text-xl font-semibold mb-4">Hình thức thanh toán</h3>
          <PaymentMethod
            selectedOption={selectedOption}
            handleOptionChange={handleOptionChange}
          />
        </div>

        <div className="bg-white mt-6">
          <h3 className="text-xl font-semibold mb-4">Hình thức đặt trước</h3>
          <div className="mb-4">
            <label className="inline-flex items-center">
              <input
                type="radio"
                name="option"
                value="DEPOSIT_50"
                className="form-radio text-[#FA7D0B]"
                onChange={(e) => handleDepositChange(e.target.value)}
              />
              <span className="ml-2">Đặt cọc 50%</span>
            </label>
          </div>
          <div className="mb-4">
            <label className="inline-flex items-center">
              <input
                type="radio"
                name="option"
                value="FULL_PAYMENT"
                className="form-radio text-[#FA7D0B]"
                onChange={(e) => handleDepositChange(null)}
              />
              <span className="ml-2">Thanh toán toàn bộ đơn hàng</span>
            </label>
          </div>
        </div>

        <button
          onClick={() => navigate(-1)}
          className="mb-4 px-4 py-2 bg-gray-200 rounded hover:bg-gray-300"
        >
          Quay lại
        </button>
      </div>

      {/* Right Panel: Order Summary */}
      <div className="basis-3/5 pr-20 pl-5 mt-10 h-1/4">
        <div className="font-alfa text-center p-5 border rounded text-black mb-4">
          Tóm tắt đơn hàng
        </div>
        <div className="overflow-auto h-3/4">
          {orderDetail?.listChild?.$values?.length > 0 ? (
            orderDetail.listChild.$values.map((product) => (
              <div
                key={product.cartItemId}
                className="border rounded p-4 space-x-2 mb-4"
              >
                <div className="flex">
                  <div className="relative bg-white mr-4">
                    <img
                      src={product.imgAvatarPath}
                      alt={product.productName}
                      className="w-32 h-32 object-contain rounded"
                    />
                    <span className="absolute top-0 right-0 bg-orange-500 text-white text-xs font-bold rounded-full w-5 h-5 flex items-center justify-center">
                      {product.quantity}
                    </span>
                  </div>
                  <div className="flex justify-between w-full">
                    <div className="flex flex-col space-y-4 text-wrap mr-2">
                      <h3 className="text-lg font-semibold">{product.productName}</h3>
                      <div className="text-sm">
                        <li>Màu sắc: {product.color}</li>
                        <li>Kích cỡ: {product.size}</li>
                        <li>Tình trạng: {product.condition}%</li>
                        <li>
                          Thời gian thuê:{" "}
                          {new Date(product.rentalStartDate).toLocaleDateString()} -{" "}
                          {new Date(product.rentalEndDate).toLocaleDateString()}
                        </li>
                      </div>
                    </div>
                    <p className="text-lg text-black text-center flex items-center justify-center">
                      {product.subTotal.toLocaleString()} ₫
                    </p>
                  </div>
                </div>
              </div>
            ))
          ) : (
            <div className="border rounded p-4 space-x-2 mb-4">
              <div className="flex">
                <div className="relative bg-white mr-4">
                  <img
                    src={orderDetail.imgAvatarPath}
                    alt={orderDetail.productName}
                    className="w-32 h-32 object-contain rounded"
                  />
                  <span className="absolute top-0 right-0 bg-orange-500 text-white text-xs font-bold rounded-full w-5 h-5 flex items-center justify-center">
                    {orderDetail.quantity}
                  </span>
                </div>
                <div className="flex justify-between w-full">
                  <div className="flex flex-col space-y-4 text-wrap mr-2">
                    <h3 className="text-lg font-semibold">{orderDetail.productName}</h3>
                    <div className="text-sm">
                      <li>Màu sắc: {orderDetail.color}</li>
                      <li>Kích cỡ: {orderDetail.size}</li>
                      <li>Tình trạng: {orderDetail.condition}%</li>
                      <li>
                        Thời gian thuê:{" "}
                        {new Date(orderDetail.rentalStartDate).toLocaleDateString()} -{" "}
                        {new Date(orderDetail.rentalEndDate).toLocaleDateString()}
                      </li>
                    </div>
                  </div>
                  <p className="text-lg text-black text-center flex items-center justify-center">
                    {orderDetail.subTotal.toLocaleString()} ₫
                  </p>
                </div>
              </div>
            </div>
          )}
          <div className="flex justify-between items-center pt-1 border rounded mt-4">
            <h3 className="text-lg font-semibold">Tạm tính</h3>
            <p className="text-lg text-black">
              {orderDetail.subTotal.toLocaleString()} ₫
            </p>
          </div>
          <div className="flex justify-between items-center pt-1 border rounded mt-4">
            <h3 className="text-lg font-semibold">Phí vận chuyển</h3>
            <p className="text-lg text-black">
              2Sport sẽ liên hệ và thông báo sau
            </p>
          </div>
          <div className="flex justify-between items-center pt-1 border rounded mt-4">
            <h3 className="text-lg font-semibold">Tổng cộng</h3>
            <p className="text-lg text-black">
              {orderDetail.subTotal.toLocaleString()} ₫
            </p>
          </div>
          <div className="flex pb-10 justify-center items-center mt-4">
            <Button
              className="bg-orange-500 text-white text-sm py-2 px-4"
              onClick={handleCheckout}
            >
              Thanh toán
            </Button>
          </div>
        </div>
      </div>
    </div>
  );
}