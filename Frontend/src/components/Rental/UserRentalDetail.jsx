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

    const {
        fullName,
        email,
        contactPhone,
        address,
        rentalOrderCode,
        listChild,
        productName,
        rentPrice,
        rentalDays,
        totalAmount,
        orderStatus,
        paymentStatus,
    } = orderDetail;

    const children = listChild?.$values || [];
    

    return (
        <div className="container mx-auto p-4">
            <button
                onClick={() => navigate(-1)}
                className="mb-4 px-4 py-2 bg-gray-200 rounded hover:bg-gray-300"
            >
                Back
            </button>
            {orderDetail.paymentStatus === "Đang chờ thanh toán" && orderDetail.deliveryMethod !== "HOME_DELIVERY" &&

                <Button
                    className="bg-purple-500 text-white text-sm rounded-full py-2 px-4"
                    onClick={() => navigate("/rental-checkout", { state: { selectedOrder: orderDetail } })}
                >
                    Checkout
                </Button>
            }
            <div className="bg-white p-6 rounded shadow-lg">
                <h2 className="text-2xl font-bold mb-4">Order Details</h2>

                {/* User Information Section */}
                <div className="mb-6">
                    <h3 className="text-lg font-semibold mb-2">Customer Information</h3>
                    <p>
                        <span className="font-semibold">Name:</span> {fullName}
                    </p>
                    <p>
                        <span className="font-semibold">Email:</span> {email}
                    </p>
                    <p>
                        <span className="font-semibold">Contact Phone:</span> {contactPhone}
                    </p>
                    <p>
                        <span className="font-semibold">Address:</span> {address}
                    </p>
                    <p>
                        <span className="font-semibold">Order Code:</span> {rentalOrderCode}
                    </p>
                </div>

                {/* Order Details Section */}
                <div>
                    <h3 className="text-lg font-semibold mb-2">Product Details</h3>
                    {children.length > 0 ? (
                        children.map((child, index) => (
                            <div
                                key={child.id}
                                className="bg-gray-100 p-4 mb-4 rounded-lg shadow-sm"
                            >
                                <div className="flex items-center gap-4">
                                    <img
                                        src={child.imgAvatarPath}
                                        alt={child.productName}
                                        className="w-24 h-24 object-cover rounded"
                                    />
                                    <div>
                                        <p>
                                            <span className="font-semibold">Product Name:</span>{" "}
                                            {child.productName}
                                        </p>
                                        <p>
                                            <span className="font-semibold">Màu sắc:</span>{" "}
                                            {child.color}
                                        </p>
                                        <p>
                                            <span className="font-semibold">Kích cỡ:</span>{" "}
                                            {child.size}
                                        </p>
                                        <p>
                                            <span className="font-semibold">Tình trạng:</span>{" "}
                                            {child.condition}%
                                        </p>
                                        <p>
                                            <span className="font-semibold">Rent Price:</span>{" "}
                                            {child.rentPrice} ₫
                                        </p>
                                        <p>
                                            <span className="font-semibold">Quantity:</span>{" "}
                                            {child.quantity} 
                                        </p>
                                        <p>
                                            <span className="font-semibold">Rental Period:</span>{" "}
                                            {new Date(child.rentalStartDate).toLocaleDateString()} -{" "}
                                            {new Date(child.rentalEndDate).toLocaleDateString()}
                                        </p>
                                        <p>
                                            <span className="font-semibold">Total:</span>{" "}
                                            {child.totalAmount} ₫
                                        </p>
                                    </div>
                                </div>
                            </div>
                        ))
                    ) : (
                        <div className="bg-gray-100 p-4 rounded-lg shadow-sm">
                            <div className="flex items-center gap-4">
                                <img
                                    src={orderDetail.imgAvatarPath || "/placeholder.jpg"}
                                    alt={productName}
                                    className="w-24 h-24 object-cover rounded"
                                />
                                <div>
                                    <p>
                                        <span className="font-semibold">Product Name:</span>{" "}
                                        {productName}
                                    </p>
                                    <p>
                                        <span className="font-semibold">Rent Price:</span>{" "}
                                        {rentPrice || "N/A"} ₫
                                    </p>
                                    <p>
                                        <span className="font-semibold">Rental Days:</span>{" "}
                                        {rentalDays || "N/A"}
                                    </p>
                                    <p>
                                        <span className="font-semibold">Total:</span>{" "}
                                        {totalAmount || "N/A"} ₫
                                    </p>
                                </div>
                            </div>
                        </div>
                    )}
                </div>

                {/* Order Summary Section */}
                <div className="mt-6">
                    <h3 className="text-lg font-semibold mb-2">Order Summary</h3>
                    <p>
                        <span className="font-semibold">Order Status:</span> {orderStatus}
                    </p>
                    <p>
                        <span className="font-semibold">Payment Status:</span> {paymentStatus}
                    </p>
                </div>
            </div>
        </div>
    );
}
