import axios from "axios";

const API_BASE_URL = 'https://twosportapi-295683427295.asia-southeast2.run.app/api/SaleOrder';

export const getUserOrders = (id, token) => {
  const url = `${API_BASE_URL}/get-orders-by-user?userId=${id}`;
  return axios.get(url, {
    headers: {
      'accept': '*/*',
      "Authorization": `Bearer ${token}`,
    }
  });
};
