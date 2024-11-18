import { getDistrict, getProvince, getShippingFee, getWard } from "../../api/GHN/apiGHN";

export const fetchProvince = async () => {
    try {
      const response = await getProvince();
      return response.data.data;
    } catch (error) {
      // console.error('Error fetching cart:', error);
      // toast.error('Error fetching cart');
      throw error;
    }
  }

  export const fetchDistrict = async (provinceId) => {
    try {
      const response = await getDistrict(provinceId);
      return response.data.data;
    } catch (error) {
      // console.error('Error fetching cart:', error);
      // toast.error('Error fetching cart');
      throw error;
    }
  }

  export const fetchWard = async (districtId) => {
    try {
      const response = await getWard(districtId);
      return response.data.data;
    } catch (error) {
      // console.error('Error fetching cart:', error);
      // toast.error('Error fetching cart');
      throw error;
    }
  }

  export const fetchShippingFee = async (requestData) => {
    try {
      const response = await getShippingFee(requestData);
      return response.data.data; 
    } catch (error) {
      console.error('Error fetching shipping fee:', error);
      throw error;
    }
  };
  