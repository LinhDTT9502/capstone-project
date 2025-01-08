import { getAllReviews, getReviewsByProductCode, addReview, checkIsReview } from '../api/apiReview';

// Lấy tất cả reviews
export const fetchAllReviews = async () => {
  try {
    const response = await getAllReviews();
    return response.data.$values || [];
  } catch (error) {
    console.error("Error fetching all reviews:", error.message);
    throw error;
  }
};

// Lấy tất cả review theo productCode
export const fetchReviewsByProductCode = async (productCode) => {
  try {
    const response = await getReviewsByProductCode(productCode);
    return response.data.$values || [];
  } catch (error) {
    console.error(`Error fetching reviews for product ${productCode}:`, error.message);
    throw error;
  }
};

// Thêm mới review cho productCode
export const submitReview = async (productCode, reviewData) => {
  try {
    const response = await addReview(productCode, reviewData);
    return response.data;
  } catch (error) {
    console.error(`Error submitting review for product ${productCode}:`, error.message);
    throw error;
  }
};

export const fetchCheckReview = async (saleOrderId) => {
  try {
    const response = await checkIsReview(saleOrderId);
    return response.data
  } catch (error) {
    console.error("Error fetching all reviews:", error.message);
    throw error;
  }
};
