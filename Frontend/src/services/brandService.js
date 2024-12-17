import { getAllBrands } from '../api/apiBrand';

export const fetchBrands = async () => {
  try {
    const response = await getAllBrands();
    const filteredBrands = response.data.data.$values.filter(
      (brand) => brand.status === true
    );

    return filteredBrands;
  } catch (error) {
    console.error('Error fetching brand data:', error);
    throw error;
  }
};