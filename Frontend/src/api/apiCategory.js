import axios from 'axios';

const API_BASE_URL = 'https://twosportapi-295683427295.asia-southeast2.run.app/api/Category';

export const getAllCategories = () => {
  return axios.get(`${API_BASE_URL}/list-categories`, {
    headers: {
      'accept': '*/*'
    }
  });
};
