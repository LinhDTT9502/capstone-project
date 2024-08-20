import axios from 'axios';

const API_BASE_URL = 'https://twosportapiv2.azurewebsites.net/api/Payment';

export const checkoutOrder = (token, orderMethodId, data) => {
  return axios.post(`${API_BASE_URL}/checkout-orders?orderMethodId=${orderMethodId}`, data, {
    headers: {
      'Accept': '*/*',
      'Authorization': `Bearer ${token}`,
      'Content-Type': 'application/json'
    }
  });
};
