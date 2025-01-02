import axios from 'axios';
import axiosInstance from './axiosInstance'; 
const API_BASE_URL = 'https://capstone-project-703387227873.asia-southeast1.run.app/api/Like';

export const getLikesAPI = (productCode) => {
  return axios.get(`${API_BASE_URL}/get-likes-of-product/${productCode}`, {
    headers: {
      Accept: "*/*",
      "Content-Type": "application/json",
    },
  });
};

export const toggleLikeProductAPI = (productCode, token) => {
  return axiosInstance.post(
    `${API_BASE_URL}/like-unlike-product/${productCode}`,
    {},
    {
      headers: {
        Accept: "*/*",
        Authorization: `Bearer ${token}`,
        "Content-Type": "application/json",
      },
    }
  );
};
