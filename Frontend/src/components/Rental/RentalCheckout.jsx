import React, { useState } from "react";
import { useLocation, useNavigate } from "react-router-dom";
import axios from "axios";
import { Button } from "@material-tailwind/react";
import PaymentMethod from "../Payment/PaymentMethod";

export default function RentalCheckout() {
    const location = useLocation();
    const navigate = useNavigate();
    const orderDetail = location.state?.selectedOrder;
    const [selectedOption, setSelectedOption] = useState(null);
    console.log(orderDetail);


    const handleOptionChange = (event) => {
        setSelectedOption(event.target.value);
        // console.log(selectedOption);

    };

    if (!orderDetail) {
        return <p>No order details available. Please go back to the rental detail page.</p>;
    }

    const handleCheckout = async () => {
        try {
            const token = localStorage.getItem("token");
            const payload = {
                paymentMethodID: selectedOption,
                orderID: orderDetail.id,
                orderCode: orderDetail.rentalOrderCode,
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
                // console.log(paymentLink);
                window.location.href = paymentLink;
            } else {
                alert("Checkout failed: " + response.data.message);
            }
        } catch (err) {
            alert("An error occurred: " + err.message);
        }
    };

    return (<>

        <div className="flex flex-row bg-slate-200">
            <div className="text-nowrap basis-2/3 bg-white ">

                <PaymentMethod
                    selectedOption={selectedOption}
                    handleOptionChange={handleOptionChange}
                />
                <button
                    onClick={() => navigate(-1)}
                    className="mb-4 px-4 py-2 bg-gray-200 rounded hover:bg-gray-300"
                >
                    Back
                </button>
            </div>
            <div className="basis-3/5  pr-20 pl-5 h-1/4 mt-10">
                <div className="font-alfa text-center p-5 border rounded text-black">
                    Tóm tắt đơn hàng
                </div>
                <div className="overflow-auto h-3/4">
                    <div className="grid grid-cols-1 gap-4">
                        {orderDetail?.listChild?.$values?.length > 0 ? (
                            orderDetail.listChild.$values.map((product) => (
                                <div key={product.cartItemId} className="border rounded p-4 space-x-2">
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
                                                    <li>Thời gian thuê: {product.rentalStartDate} - {product.rentalEndDate}</li>
                                                </div>
                                            </div>
                                            <p className="text-lg text-black text-center flex items-center justify-center">
                                                {product.subTotal}
                                            </p>
                                        </div>
                                    </div>
                                </div>
                            ))
                        ) : (
                            orderDetail && (
                                <div className="border rounded p-4 space-x-2">
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
                                                    <li>Thời gian thuê: {orderDetail.rentalStartDate} - {orderDetail.rentalEndDate}</li>
                                                </div>
                                            </div>
                                            <p className="text-lg text-black text-center flex items-center justify-center">
                                                {orderDetail.subTotal.toLocaleString()}
                                            </p>
                                        </div>
                                    </div>
                                    <div className="text-red-700 flex justify-end text-sm font-bold my-2">* Đơn vị tiền tệ: VND</div>
                                    <div className="h-px bg-gray-300 mx-auto font-bold"></div>
                                    <div className="flex justify-between items-center pt-1 border rounded mt-4">
                                        <h3 className="text-lg font-semibold">
                                            Tạm tính
                                        </h3>
                                        <p className="text-lg text-black">
                                        {orderDetail.subTotal.toLocaleString()}
                                        </p>
                                    </div>
                                    <div className="flex justify-between items-center pt-1 border rounded mt-4">
                                        <h3 className="text-lg font-semibold">
                                            Phí vận chuyển
                                        </h3>
                                        <p className="text-lg text-black">
                                            2Sport sẽ liên hệ và thông báo sau
                                        </p>
                                    </div>
                                    <div className="flex justify-between items-center pt-1 border rounded mt-4">
                                        <h3 className="text-lg font-semibold">Tổng cộng</h3>
                                        <p className="text-lg text-black">
                                        {orderDetail.subTotal.toLocaleString()}
                                        </p>
                                    </div>
                                </div>
                            )
                        )}
                    </div>


                </div>
                <div className="flex pb-10 justify-center items-center">
                    <Button
                        className="bg-orange-500 text-white text-sm  py-2 px-4"
                        onClick={handleCheckout}
                    >
                        Thanh toán
                    </Button>
                </div>
            </div>
        </div>

        {/* <div className="container mx-auto p-4">
          
            <div className="bg-white p-6 rounded shadow-lg">
                <h2 className="text-2xl font-bold mb-4">Checkout</h2>
                <div className="grid grid-cols-1 md:grid-cols-2 gap-4">
                    <div>
                        <img
                            src={orderDetail.imgAvatarPath}
                            alt={orderDetail.productName}
                            className="w-full h-64 object-cover rounded"
                        />
                    </div>
                    <div>
                        <h3 className="text-xl font-semibold mb-2">{orderDetail.productName}</h3>
                        <p>
                            <span className="font-semibold">Order Code:</span> {orderDetail.rentalOrderCode}
                        </p>
                        <p>
                            <span className="font-semibold">Customer Name:</span> {orderDetail.fullName}
                        </p>
                        <p>
                            <span className="font-semibold">Total Amount:</span> {orderDetail.totalAmount} VND
                        </p>
                    </div>
                </div>
                <div className="mt-4">
                    <Button
                        className="bg-green-500 text-white text-sm rounded-full py-2 px-4"
                        onClick={handleCheckout}
                    >
                        Confirm Checkout
                    </Button>
                </div>
            </div>
        </div> */}
    </>
    );
}
