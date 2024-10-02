import { getAllSuppliers } from "../api/apiSupplier";


export const fetchSuppliers = async () => {
  try {
    const response = await getAllSuppliers();
    return response.data.data.$values;
  } catch (error) {
    console.error('Error fetching brand data:', error);
    throw error;
  }
};