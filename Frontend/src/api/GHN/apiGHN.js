import axios from 'axios';

const API_BASE_URL = 'https://online-gateway.ghn.vn/shiip/public-api/master-data';
const apiToken = import.meta.env.VITE_GHN_TOKEN;
const shopId = import.meta.env.VITE_SHOP_ID;

export const getProvince = () => {
  const url = `${API_BASE_URL}/province`;
  
  return axios.get(url, {
    headers: {
      'accept': '*/*',
      'token': `${apiToken}`,
      'Content-Type': 'application/json'

    }
  });
};

  export const getDistrict = (provinceId) => {
    const url = `${API_BASE_URL}/district`;
    return axios.post(
      url,
      {
        province_id: Number(provinceId), 
      },
      {
        headers: {
          'accept': '*/*',
          'token': `${apiToken}`,
          'Content-Type': 'application/json',
        },
      }
    );
  };
  
  export const getWard = (districtId) => {
    const url = `${API_BASE_URL}/ward`;
    return axios.post(
      url,
      {
        district_id: Number(districtId), 
      },
      {
        headers: {
          'accept': '*/*',
          'token': `${apiToken}`,
          'Content-Type': 'application/json',
        },
      }
    );
  };

  export const getShippingFee = (data) => {
    const url = `${API_BASE_URL}/shipping-order/fee`;
    return axios.post(
      url,
      data,
      {
        headers: {
          'Accept': '*/*',
          'token': `${apiToken}`,  
          'shop_id': `${shopId}`, 
          'Content-Type': 'application/json',
        },
      }
    );
  };
  