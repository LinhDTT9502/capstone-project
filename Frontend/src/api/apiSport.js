import axios from 'axios';

const API_BASE_URL = 'https://capstone-project-703387227873.asia-southeast1.run.app/api/sport';

export const getAllSports = () => {
  return axios.get(`${API_BASE_URL}/list-sports`, {
    headers: {
      'accept': '*/*'
    }
  });
};