import { getAllBrands } from '../api/apiBrand';

export const fetchBrands = async () => {
  try {
    const response = await getAllBrands();
    return response.data.data.$values;
  } catch (error) {
    console.error('Error fetching brand data:', error);
    throw error;
  }
};