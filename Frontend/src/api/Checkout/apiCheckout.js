import axios from 'axios';

const API_BASE_URL = 'https://twosportapi-295683427295.asia-southeast2.run.app/api/SaleOrder';

export const placedOrderAPI = ( data) => {
  return axios.post(`${API_BASE_URL}/create-sale-order`, data, {
    headers: {
      'Accept': '*/*',
      'Content-Type': 'application/json'
    }
  });
};

