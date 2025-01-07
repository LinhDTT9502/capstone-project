import React, { useEffect, useState } from 'react';
import { Button } from "@material-tailwind/react";
import { fetchCheckReview } from '../../services/reviewService';

const ReviewButton = ({ orderStatus, saleOrderId, setReviewModal }) => {
    const [showButton, setShowButton] = useState(false);

    const checkReview = async () => {
        try {
            const response = await fetchCheckReview(saleOrderId);
            setShowButton(response === false);
        } catch (err) {
            console.error("Error in checkReview:", err);
        }
    };

    useEffect(() => {
        checkReview();
    }, [orderStatus, saleOrderId, setReviewModal]);

    return (
        <>
            {showButton && (
                <Button
                onClick={() => setReviewModal(true)}
                    color="white"
                    size="sm"
                    className="text-yellow-500 bg-white border border-yellow-500 rounded-md hover:bg-yellow-200"
                >
                    Đánh giá
                </Button>
            )}
        </>
    );
};

export default ReviewButton;
