
import axios from 'axios';

const API_BASE_URL = 'https://twosport-api-offcial-685025377967.asia-southeast1.run.app//api/Comment';
export const getCommentbyProductCode = (productCode) => {
    const url = `${API_BASE_URL}/get-all-comments/${productCode}`;
    return axios.get(url, {
      headers: {
        'accept': '*/*'
      }
    });
  };