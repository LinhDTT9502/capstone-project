import { getWarehouse } from '../api/apiWarehouse';

export const fetchWarehouse = async () => {
  try {
    const response = await getWarehouse();
    return response.data.data.$values;
  } catch (error) {
    console.error('Error fetching Warehouse data:', error);
    throw error;
  }
};
