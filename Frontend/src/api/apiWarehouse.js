import axios from 'axios';

const API_BASE_URL = 'https://twosportapiv2.azurewebsites.net/api/Warehouse';

export const getWarehouse = () => {
  return axios.get(`${API_BASE_URL}/list-all`, {
    headers: {
      'accept': '*/*'
    }
  });
};
