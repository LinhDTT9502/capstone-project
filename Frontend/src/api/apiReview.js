import axios from 'axios';

const API_BASE_URL = 'https://twosport-api-offcial-685025377967.asia-southeast1.run.app/api/Review';

// Lấy tất cả các review
export const getAllReviews = () => {
  return axios.get(`${API_BASE_URL}/get-all-reviews`, {
    headers: {
      'accept': '*/*'
    }
  });
};

// Lấy tất cả review theo productCode
export const getReviewsByProductCode = (productCode) => {
  return axios.get(`${API_BASE_URL}/get-all-reviews-of-product/${productCode}`, {
    headers: {
      'accept': '*/*',
      Authorization: `Bearer ${localStorage.getItem("token")}`,
    }
  });
};

// Thêm mới review cho một productCode
export const addReview = (productCode, reviewData) => {
  return axios.post(`${API_BASE_URL}/add-review/${productCode}`, reviewData, {
    headers: {
      'accept': '*/*',
      Authorization: `Bearer ${localStorage.getItem("token")}`,
      'Content-Type': 'application/json'
    }
  });
};

export const checkIsReview = (saleOrderId) => {
  return axios.get(`${API_BASE_URL}/check-is-review/${saleOrderId}`, {
    headers: {
      'accept': '*/*'
    }
  });
};