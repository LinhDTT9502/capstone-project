import React from "react";
import { useLocation, useNavigate } from "react-router-dom";
import axios from "axios";
import { Button } from "@material-tailwind/react";

export default function RentalCheckout() {
    const location = useLocation();
    const navigate = useNavigate();
    const orderDetail = location.state?.selectedOrder;

    if (!orderDetail) {
        return <p>No order details available. Please go back to the rental detail page.</p>;
    }

    const handleCheckout = async () => {
        try {
            const token = localStorage.getItem("token");
            const payload = {
                paymentMethodID: 2, 
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

    return (
        <div className="container mx-auto p-4">
            <button
                onClick={() => navigate(-1)}
                className="mb-4 px-4 py-2 bg-gray-200 rounded hover:bg-gray-300"
            >
                Back
            </button>
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
        </div>
    );
}
