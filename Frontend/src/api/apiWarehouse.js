import axios from 'axios';

const API_BASE_URL = 'https://2sportapi-c6ajcce3ezh5h4gw.southeastasia-01.azurewebsites.net/api/Warehouse';

export const getWarehouse = () => {
  return axios.get(`${API_BASE_URL}/list-all`, {
    headers: {
      'accept': '*/*'
    }
  });
};
