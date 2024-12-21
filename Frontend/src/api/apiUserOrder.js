import axios from "axios";

const API_BASE_URL = 'https://capstone-project-703387227873.asia-southeast1.run.app/api';

export const getUserOrders = (id, token) => {
  const url = `${API_BASE_URL}/SaleOrder/get-orders-by-user?userId=${id}`;
  return axios.get(url, {
    headers: {
      'accept': '*/*',
      "Authorization": `Bearer ${token}`,
    }
  });
};

export const getUserRentalOrders = (id, token) => {
  const url = `${API_BASE_URL}/RentalOrder/get-rental-order-by-user?userId=${id}`;
  return axios.get(url, {
    headers: {
      'accept': '*/*',
      "Authorization": `Bearer ${token}`,
    }
  });
};