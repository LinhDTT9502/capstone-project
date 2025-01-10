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
        {!showButton ? (
          <Button
            onClick={() => setReviewModal(true)}
            color="white"
            size="sm"
            className="text-yellow-500 bg-white border border-yellow-500 rounded-md hover:bg-yellow-200"
          >
            Đánh giá
          </Button>
        ) : (
          <div className="relative">
            <div className="absolute top-[-70px] left-[-230px]">
              <Button
                disabled={true}
                color="white"
                size="sm"
                className="text-green-500 bg-white border border-green-500 rounded-md hover:bg-yellow-200"
              >
                Đã đánh giá
              </Button>
            </div>
          </div>
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
