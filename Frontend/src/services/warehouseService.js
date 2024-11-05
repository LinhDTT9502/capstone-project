import { getProductofBranchAPI, getWarehouse } from '../api/apiWarehouse';

export const fetchWarehouse = async () => {
  try {
    const response = await getWarehouse();
    const data = await response.json();
    return { total: data.total, products: data.data.$values }
  } catch (error) {
    console.error('Error fetching Warehouse data:', error);
    throw error;
  }
};

export const fetchProductsbyBranch = async (branchId) => {
  try {
    const response = await getProductofBranchAPI(branchId);
    return response.data.data.$values
  } catch (error) {
    console.error('Error fetching Warehouse data:', error);
    throw error;
  }
};
