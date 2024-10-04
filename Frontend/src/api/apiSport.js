import axios from 'axios';

const API_BASE_URL = 'https://twosportapi-295683427295.asia-southeast2.run.app/api/sport';

export const getAllSports = () => {
  return axios.get(`${API_BASE_URL}/list-sports`, {
    headers: {
      'accept': '*/*'
    }
  });
};