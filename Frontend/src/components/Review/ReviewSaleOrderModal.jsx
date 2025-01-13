import React, { useState, useEffect } from "react";
import axios from "axios";
import { Button, Rating } from "@material-tailwind/react";

const ReviewSaleOrderModal = ({ saleOrderId, reviewModal, setReviewModal, setConfirmReload }) => {
    const [products, setProducts] = useState([]);
    const fetchOrderDetails = async () => {
        try {
            const token = localStorage.getItem("token");
            if (!token) {
                alert("Error: User is not authenticated.");
                return;
            }

            const response = await axios.get(
                `https://twosport-api-offcial-685025377967.asia-southeast1.run.app//api/SaleOrder/get-sale-order-detail?orderId=${saleOrderId}`,
                {
                    headers: {
                        Authorization: `Bearer ${token}`,
                    },
                }
            );

            if (response.data.isSuccess) {
                const uniqueProducts = Array.from(
                    new Map(
                        response.data.data.saleOrderDetailVMs.$values.map((product) => [
                            product.productCode,
                            { ...product, rated: 0, review: "" }, // Initialize rating and review for each product
                        ])
                    ).values()
                );
                setProducts(uniqueProducts);
            } else {
                alert(response.data.message || "Failed to fetch order details.");
            }
        } catch (error) {
            console.error(error);
            // alert("Error: Something went wrong. Please try again later.");
        }
    };
    useEffect(() => {
       

            if (saleOrderId) {
                fetchOrderDetails();
            }
    }, [saleOrderId, setConfirmReload]);

    const handleSubmit = async () => {
        const token = localStorage.getItem("token");
        if (!token) {
            alert("Error: User is not authenticated.");
            return;
        }

        try {
            const reviewsToSubmit = products.filter(
                (product) => product.rated > 0 && product.review.trim() !== ""
            );

            if (reviewsToSubmit.length === 0) {
                alert("Vui lòng đánh giá ít nhất 1 sản phẩm!");
                return;
            }

            for (const product of reviewsToSubmit) {
                await axios.post(
                    `https://twosport-api-offcial-685025377967.asia-southeast1.run.app//api/Review/add-review/${saleOrderId}`,
                    {
                        productId: product.productId,
                        star: product.rated,
                        reviewContent: product.review,
                    },
                    {
                        headers: {
                            Authorization: `Bearer ${token}`,
                        },
                    }
                );
            }
            setConfirmReload(true)
            alert("2Sport cảm ơn bạn đã chia sẻ cảm nhận!");
            setReviewModal(false)
            setProducts((prev) =>
                prev.map((product) => ({
                    ...product,
                    rated: 0,
                    review: "",
                }))
            );
            setConfirmReload(true);
        } catch (error) {
            console.error(error);
            alert("Error: Something went wrong. Please try again later.");
        }
    };

    const handleInputChange = (index, field, value) => {
        setProducts((prev) =>
            prev.map((product, idx) =>
                idx === index ? { ...product, [field]: value } : product
            )
        );
    };

    return (
        <>
            {reviewModal && (
                <div className="fixed inset-0 flex items-center justify-center bg-black bg-opacity-50 z-50">
                    <div className="bg-white rounded-lg p-6 w-3/4 max-w-lg">
                        <h2 className="text-lg font-bold mb-4">Đánh giá sản phẩm</h2>
                        <div className="space-y-6 max-h-[50vh] overflow-y-auto">
                            {products.map((product, index) => (
                                <div key={product.productCode} className="border-b pb-4">
                                    <div className="flex items-center space-x-4">
                                        <img
                                            src={product.imgAvatarPath}
                                            alt={product.productName}
                                            className="object-cover w-16 h-16 rounded-lg shadow-sm"
                                        />
                                        <div>
                                        <h3 className="font-medium mb-2">{product.productName}</h3>
                                        <i>{product.color} - {product.size} - {product.condition}%</i>
                                        </div>
                                        
                                    </div>

                                    <Rating
                                        unratedColor="amber"
                                        ratedColor="amber"
                                        value={product.rated}
                                        onChange={(value) =>
                                            handleInputChange(index, "rated", value)
                                        }
                                    />
                                    <textarea
                                        className="w-full p-2 border border-gray-300 rounded-md mt-2"
                                        placeholder="Hãy chia sẻ nhận xét của bạn nhé!"
                                        rows="4"
                                        value={product.review}
                                        onChange={(e) =>
                                            handleInputChange(index, "review", e.target.value)
                                        }
                                    />
                                </div>
                            ))}
                        </div>
                        <Button
                            className="mt-4 bg-green-500 text-white w-full"
                            onClick={handleSubmit}
                        >
                            Gửi đánh giá
                        </Button>
                        <Button
                            className="mt-4 bg-gray-500 text-white w-full"
                            onClick={() => {
                                setReviewModal(false);
                                setConfirmReload(true);
                            }}
                        >
                            Đóng
                        </Button>
                    </div>
                </div>
            )}
        </>
    );
};

export default ReviewSaleOrderModal;
