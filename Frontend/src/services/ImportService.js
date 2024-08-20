import { importAPI } from '../api/apiImport';

export const fetchImport = async (quantity, productId, supplierId) => {
  try {
    const response = await importAPI(quantity, productId, supplierId); 
    return response;
  } catch (error) {
    console.error('Error fetching Import data:', error);
    throw error;
  }
};
