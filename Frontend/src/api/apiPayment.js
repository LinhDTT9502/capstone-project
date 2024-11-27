import axios from 'axios';

const API_BASE_URL = 'https://capstone-project-703387227873.asia-southeast1.run.app/api/Order';

export const checkoutOrder = (token, data) => {
  return axios.post(`${API_BASE_URL}/checkout-sale-order-for-customer`, data, {
    headers: {
      'Accept': '*/*',
      'Authorization': `Bearer ${token}`,
      'Content-Type': 'application/json'
    }
  });
};

