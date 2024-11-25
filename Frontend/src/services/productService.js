// src/services/productService.js
import { toast } from 'react-toastify';
import { getProductList, getProductById, getProductFilterBy, getProductByProductCode, getProductColor, getProductSize, getProductCondition } from '../api/apiProduct';

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
    return response.data.$values;
  } catch (error) {
    console.error(`Error fetching product with id ${id}:`, error);
    throw error;
  }
};

export const fetchProductByProductCode = async (productCode, color, size, condition) => {
  try {
    const response = await getProductByProductCode(productCode, color, size, condition);
    return response.data.$values;
  } catch (error) {
    console.error(`Error fetching product:`, error);
    throw error;
  }
};

export const fetchProductColor = async (productCode) => {
  try {
    const response = await getProductColor(productCode);
    return response.data.data.$values;
  } catch (error) {
    console.error(`Error fetching product:`, error);
    throw error;
  }
};

export const fetchProductSize = async (productCode, color) => {
  try {
    const response = await getProductSize(productCode, color);
    return response.data.data.$values;
  } catch (error) {
    console.error(`Error fetching product with :`, error);
    throw error;
  }
};

export const fetchProductCondition = async (productCode, color, size) => {
  try {
    const response = await getProductCondition(productCode, color, size);
    return response.data.data.$values;
  } catch (error) {
    console.error(`Error fetching product with :`, error);
    throw error;
  }
};