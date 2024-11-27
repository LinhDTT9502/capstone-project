import axios from 'axios';

const API_BASE_URL = 'https://capstone-project-703387227873.asia-southeast1.run.app/api/Category';

export const getAllCategories = () => {
  return axios.get(`${API_BASE_URL}/list-categories`, {
    headers: {
      'accept': '*/*'
    }
  });
};
