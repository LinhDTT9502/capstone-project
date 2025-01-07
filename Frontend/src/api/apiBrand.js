import axios from 'axios';

const API_BASE_URL = 'https://capstone-project-703387227873.asia-southeast1.run.app/api/Brand';

export const getAllBrands = () => {
  return axios.get(`${API_BASE_URL}/list-all`, {
    params: { status: true } ,

    headers: {
      'accept': '*/*'
    }
  });
};