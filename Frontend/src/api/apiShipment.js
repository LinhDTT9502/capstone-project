import axiosInstance from './axiosInstance';

const API_BASE_URL = 'https://twosportapi-295683427295.asia-southeast2.run.app/';

export const getShipmentDetails = ( token) => {

  return axiosInstance.get(`${API_BASE_URL}/list-shipment-details`,{
    headers: {
      'Accept': '*/*',
      "Authorization": `Bearer ${token}`,
      "Content-Type": "application/json"
    }
  });
};

export const addShipmentDetail = ( token, data) => {

  return axiosInstance.post(`${API_BASE_URL}/add-shipment-detail`,data,{
    headers: {
      'Accept': '*/*',
      "Authorization": `Bearer ${token}`,
      "Content-Type": "application/json"
    }
  });
};

export const updateShipmentDetail = ( id, token, data) => {

  return axiosInstance.put(`${API_BASE_URL}/update-shipment-detail/${id}`,data,{
    headers: {
      'Accept': '*/*',
      "Authorization": `Bearer ${token}`,
      "Content-Type": "application/json"
    }
  });
};

export const deleteShipmentDetail = (id, token) => {
  // console.log(id, token);
  return axiosInstance.delete(`${API_BASE_URL}/delete-shipment-detail`, {
    headers: {
      'Accept': '*/*',
      'Authorization': `Bearer ${token}`,
      "Content-Type": "application/json"
    },
    params: {
      id: id
    }
  });
};