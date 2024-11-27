import axios from 'axios';

const API_BASE_URL = 'https://capstone-project-703387227873.asia-southeast1.run.app/api/SaleOrder';

export const placedOrderAPI = ( data) => {
  return axios.post(`${API_BASE_URL}/create-sale-order`, data, {
    headers: {
      'Accept': '*/*',
      'Content-Type': 'application/json'
    }
  });
};

