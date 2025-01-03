import React, { useState, useEffect } from "react";
import axios from "axios";
import { Button, Rating } from "@material-tailwind/react";

const ReviewSaleOrderModal = ({ saleOrderId, reviewModal, setReviewModal, setConfirmReload }) => {
    const [products, setProducts] = useState([]);
    const [selectedProductCode, setSelectedProductCode] = useState("");
    const [star, setStar] = useState(0);
    const [review, setReview] = useState("");
    console.log(saleOrderId);
    console.log(products);
    console.log(selectedProductCode);
    console.log(star);
    console.log(review);






    useEffect(() => {
        const fetchOrderDetails = async () => {
            try {
                const token = localStorage.getItem("token");
                if (!token) {
                    alert("Error: User is not authenticated.");
                    return;
                }

                const response = await axios.get(
                    `https://capstone-project-703387227873.asia-southeast1.run.app/api/SaleOrder/get-sale-order-detail?orderId=${saleOrderId}`,
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
                                product,
                            ])
                        ).values()
                    );
                    setProducts(uniqueProducts);
                } else {
                    alert(response.data.message || "Failed to fetch order details.");
                }
            } catch (error) {
                console.error(error);
                alert("Error: Something went wrong. Please try again later.");
            }
        };

        if (saleOrderId) {
            fetchOrderDetails();
        }
    }, [saleOrderId, reviewModal, setReviewModal,]);

    const handleSubmit = async () => {
        if (!selectedProductCode) {
            alert("Error: Please select a product to review!");
            return;
        }

        try {
            const token = localStorage.getItem("token");
            if (!token) {
                alert("Error: User is not authenticated.");
                return;
            }

            const response = await axios.post(
                `https://capstone-project-703387227873.asia-southeast1.run.app/api/Review/add-review/${saleOrderId}`,
                {
                    productCode: selectedProductCode,
                    star,
                    review,
                },
                {
                    headers: {
                        Authorization: `Bearer ${token}`,
                    },
                }
            );

            if (response.status === 200) {
                alert("2Sport cảm ơn bạn đã chia sẻ cảm nhận!");
                setProducts((prev) =>
                    prev.filter((product) => product.productCode !== selectedProductCode)
                );
                setSelectedProductCode("");
                setStar(0);
                setReview("");
                setReviewModal(false)
                setConfirmReload(true)
            } else {
                alert(response.data.message || "Failed to submit review.");
            }
        } catch (error) {
            console.error(error);
            alert("Error: Something went wrong. Please try again later.");
        }
    };

    return (
        <>
            {reviewModal &&
                <div className="fixed inset-0 flex items-center justify-center bg-black bg-opacity-50 z-50">
                    <div className="bg-white rounded-lg p-6 w-3/4 max-w-lg">
                        <h2 className="text-lg font-bold mb-4">Đánh giá sản phẩm</h2>
                        <label htmlFor="product-select" className="block font-medium mb-2">
                            Chọn sản phẩm:
                        </label>
                        <select
                            id="product-select"
                            className="w-full p-2 border border-gray-300 rounded-md mb-4"
                            value={selectedProductCode}
                            onChange={(e) => setSelectedProductCode(e.target.value)}
                        >
                            <option value="">--Chọn sản phẩm--</option>
                            {products.map((product) => (
                                <option key={product.productCode} value={product.productCode}>
                                    {product.productName}
                                </option>
                            ))}
                        </select>

                            <>
                                <label className="block font-medium mb-2">
                                    Đánh giá chất lượng sản phẩm:
                                </label>
                                <Rating
                                unratedColor="amber" ratedColor="amber"
                                    value={star}
                                    onChange={(e, newValue) => setStar(newValue)}
                                />

                                <textarea
                                    className="w-full p-2 border border-gray-300 rounded-md mt-4 mb-4"
                                    placeholder="Hãy chia sẻ nhận xét của bạn nhé!"
                                    rows="4"
                                    value={review}
                                    onChange={(e) => setReview(e.target.value)}
                                />

                                <Button
                                    className="bg-green-500 text-white w-full"
                                    onClick={handleSubmit}
                                >
                                    Gửi đánh giá
                                </Button>
                            </>
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
                </div>}
        </>
    );
};

export default ReviewSaleOrderModal;
