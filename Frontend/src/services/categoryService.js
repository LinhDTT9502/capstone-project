import { getAllCategories } from '../api/apiCategory';

export const fetchCategories = async () => {
  try {
    const response = await getAllCategories();
    const filteredCategory = response.data.data.$values.filter(category => category.status === true);
        return filteredCategory;
  } catch (error) {
    console.error('Error fetching category data:', error);
    throw error;
  }
};
