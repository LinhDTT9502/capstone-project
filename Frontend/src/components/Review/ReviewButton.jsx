import React, { useEffect, useState } from 'react';
import { Button } from "@material-tailwind/react";
import { fetchCheckReview } from '../../services/reviewService';
import ReviewSaleOrderModal from './ReviewSaleOrderModal';

const ReviewButton = ({ orderStatus, saleOrderId, setConfirmReload }) => {
    const [showButton, setShowButton] = useState(false);
    const [reviewModal, setReviewModal] = useState(false);

    const checkReview = async () => {
        try {
            const response = await fetchCheckReview(saleOrderId);
            console.log(response);

            setShowButton(response);
        } catch (err) {
            console.error("Error in checkReview:", err);
        }
    };

    useEffect(() => {
        checkReview();
    }, [orderStatus, saleOrderId, setReviewModal]);

    return (
        <>
            {!showButton && (
                <Button
                    onClick={() => setReviewModal(true)}
                    color="white"
                    size="sm"
                    className="text-yellow-500 bg-white border border-yellow-500 rounded-md hover:bg-yellow-200"
                >
                    Đánh giá
                </Button>
            )}
            <ReviewSaleOrderModal
                saleOrderId={saleOrderId}
                setReviewModal={setReviewModal}
                reviewModal={reviewModal}
                setConfirmReload={setConfirmReload}
            />
        </>
    );
};

export default ReviewButton;
