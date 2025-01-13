import axios from 'axios';

const API_BASE_URL = 'https://twosport-api-offcial-685025377967.asia-southeast1.run.app/api/Category';

export const getAllCategories = () => {
  return axios.get(`${API_BASE_URL}/list-categories`, {
    params: { status: true } ,

    headers: {
      'accept': '*/*'
    }
  });
};
