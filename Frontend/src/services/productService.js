// src/services/productService.js
import { toast } from 'react-toastify';
import { getProductList, getProductById, getProductFilterBy } from '../api/apiProduct';

export const fetchProducts = async (currentPage) => {
  try {
    const response = await getProductList(currentPage);
    const { total, data } = response.data;
    return { total, products: data.$values };
  } catch (error) {
    console.error('Error fetching products:', error);
    throw error;
  }
};

export const fetchProductsFiltered = async (
      sortBy,
      isAscending,
      selectedBrands,
      selectedCategories,
      minPrice,
      maxPrice
    ) => {
  try {
    const response = await getProductFilterBy(
      sortBy,
      isAscending,
      selectedBrands,
      selectedCategories,
      minPrice,
      maxPrice
    );
    const { total, data } = response.data;
    return { total, products: data.$values };
  } catch (error) {
    console.error('Error fetching sorted products:', error);
    throw error;
  }
};

export const fetchProductById = async (id) => {
  try {
    const response = await getProductById(id);
    return response.data;
  } catch (error) {
    console.error(`Error fetching product with id ${id}:`, error);
    throw error;
  }
};