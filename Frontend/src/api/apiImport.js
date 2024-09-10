import axios from 'axios';

const API_BASE_URL = 'https://2sportapi-c6ajcce3ezh5h4gw.southeastasia-01.azurewebsites.net/api/Import';

export const importAPI = async (quantity, productId, supplierId) => {
  const url = `${API_BASE_URL}/import-product`;
  try {
    const response = await axios.post(url, { quantity, productId, supplierId }, {
      headers: {
        'accept': '*/*',
        'Content-Type': 'application/json'
      }
    });
    return response.data; // or return response if you need more data processing
  } catch (error) {
    throw error;
  }
};
