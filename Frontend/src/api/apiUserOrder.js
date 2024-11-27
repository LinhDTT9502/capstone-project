import axios from "axios";

const API_BASE_URL = 'https://capstone-project-703387227873.asia-southeast1.run.app/api/SaleOrder';

export const getUserOrders = (id, token) => {
  const url = `${API_BASE_URL}/get-orders-by-user?userId=${id}`;
  return axios.get(url, {
    headers: {
      'accept': '*/*',
      "Authorization": `Bearer ${token}`,
    }
  });
};
