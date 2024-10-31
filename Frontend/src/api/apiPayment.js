import axios from 'axios';

const API_BASE_URL = 'https://twosportapi-295683427295.asia-southeast2.run.app/api/Order';

export const checkoutOrder = (token, data) => {
  return axios.post(`${API_BASE_URL}/checkout-sale-order-for-customer`, data, {
    headers: {
      'Accept': '*/*',
      'Authorization': `Bearer ${token}`,
      'Content-Type': 'application/json'
    }
  });
};

