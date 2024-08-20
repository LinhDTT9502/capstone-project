import { getAllCategories } from '../api/apiCategory';

export const fetchCategories = async () => {
  try {
    const response = await getAllCategories();
    return response.data.data.$values;
  } catch (error) {
    console.error('Error fetching category data:', error);
    throw error;
  }
};
