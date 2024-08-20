import axios from 'axios';

const API_BASE_URL = 'https://twosportapiv2.azurewebsites.net/api/Category';

export const getAllCategories = () => {
  return axios.get(`${API_BASE_URL}/list-categories`, {
    headers: {
      'accept': '*/*'
    }
  });
};
