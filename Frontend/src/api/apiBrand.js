import axios from 'axios';

const API_BASE_URL = 'https://twosportapiv2.azurewebsites.net/api/Brand';

export const getAllBrands = () => {
  return axios.get(`${API_BASE_URL}/list-all`, {
    headers: {
      'accept': '*/*'
    }
  });
};