import axios from 'axios';

const API_BASE_URL = 'https://twosport-api-offcial-685025377967.asia-southeast1.run.app/api/sport';

export const getAllSports = () => {
  return axios.get(`${API_BASE_URL}/list-sports`, {
    headers: {
      'accept': '*/*'
    }
  });
};