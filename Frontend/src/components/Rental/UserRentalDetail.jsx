import React, { useState, useEffect } from "react";
import { useParams, useNavigate } from "react-router-dom";
import axios from "axios";
import { Button } from "@material-tailwind/react";

export default function UserRentalDetail() {
    const { orderId } = useParams();
    const navigate = useNavigate();
    const [orderDetail, setOrderDetail] = useState(null);
    const [isLoading, setIsLoading] = useState(true);
    const [error, setError] = useState(null);

    useEffect(() => {
        const fetchOrderDetail = async () => {
            try {
                const token = localStorage.getItem("token");
                const response = await axios.get(
                    `https://capstone-project-703387227873.asia-southeast1.run.app/api/RentalOrder/get-rental-order-detail?orderId=${orderId}`,
                    {
                        headers: {
                            accept: "*/*",
                            Authorization: `Bearer ${token}`,
                        },
                    }
                );

                if (response.data.isSuccess) {
                    setOrderDetail(response.data.data);
                    console.log(orderDetail);

                } else {
                    setError("Failed to fetch order details");
                }
            } catch (err) {
                setError(err.message || "An error occurred while fetching order details");
            } finally {
                setIsLoading(false);
            }
        };

        fetchOrderDetail();
    }, [orderId]);

    if (isLoading) return <p>Loading order details...</p>;
    if (error) return <p>Error: {error}</p>;

    return (
        <div className="container mx-auto p-4">
            <button
                onClick={() => navigate(-1)}
                className="mb-4 px-4 py-2 bg-gray-200 rounded hover:bg-gray-300"
            >
                Back
            </button>
            <Button
                className="bg-purple-500 text-white text-sm rounded-full py-2 px-4"
                onClick={() => navigate("/rental-checkout", { state: { selectedOrder: orderDetail } })}
            >
                Checkout
            </Button>

            <div className="bg-white p-6 rounded shadow-lg">
                <h2 className="text-2xl font-bold mb-4">Order Details</h2>
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
                            <span className="font-semibold">Contact:</span> {orderDetail.contactPhone} | {orderDetail.email}
                        </p>
                        <p>
                            <span className="font-semibold">Address:</span> {orderDetail.address}
                        </p>
                        <p>
                            <span className="font-semibold">Branch:</span> {orderDetail.branchName}
                        </p>
                        <p>
                            <span className="font-semibold">Rent Price:</span> {orderDetail.rentPrice} VND
                        </p>
                        <p>
                            <span className="font-semibold">Rental Days:</span> {orderDetail.rentalDays}
                        </p>
                        <p>
                            <span className="font-semibold">Total Amount:</span> {orderDetail.totalAmount} VND
                        </p>
                        <p>
                            <span className="font-semibold">Order Status:</span> {orderDetail.orderStatus}
                        </p>
                    </div>
                </div>
                <div className="mt-4">
                    <h4 className="text-lg font-semibold mb-2">Rental Period</h4>
                    <p>
                        <span className="font-semibold">Start Date:</span>{" "}
                        {new Date(orderDetail.rentalStartDate).toLocaleDateString()}
                    </p>
                    <p>
                        <span className="font-semibold">End Date:</span>{" "}
                        {new Date(orderDetail.rentalEndDate).toLocaleDateString()}
                    </p>
                </div>
            </div>
        </div>
    );
}
