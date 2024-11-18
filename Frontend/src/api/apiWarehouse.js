import axios from 'axios';

const API_BASE_URL = 'https://twosportapi-295683427295.asia-southeast2.run.app/api/Warehouse';

export const getWarehouse = () => {
  return axios.get(`${API_BASE_URL}/list-all`, {
    headers: {
      'accept': '*/*'
    }
  });
};


export const getProductofBranchAPI = (branchId) => {
  const url = `${API_BASE_URL}/list-products-of-branch/${branchId}`;
  return axios.get(url, {
    headers: {
      'accept': '*/*'
    }
  });
};
