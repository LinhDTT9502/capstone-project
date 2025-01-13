import axios from 'axios';

const API_BASE_URL = 'https://twosport-api-offcial-685025377967.asia-southeast1.run.app/api/Product';
const perPage = 15;

export const getProductList = (currentPage) => {
  const url = `${API_BASE_URL}/list-products`;
  const params = {
    perPage : perPage,
    currentPage: currentPage
  };
  // console.log(params);
  return axios.get(url, {
    params,
    headers: {
      'accept': '*/*'
    }
  });
};
export const getProductById = (id) => {
  const url = `${API_BASE_URL}/get-product/${id}`;
  return axios.get(url, {
    headers: {
      'accept': '*/*'
    }
  });
};

export const getProductByProductCode = (productCode, color, size, condition) => {
  const url = `${API_BASE_URL}/get-product-by-product-code/${productCode}?color=${color}&size=${size}&condition=${condition}`;
  return axios.get(url, {
    headers: {
      'accept': '*/*'
    }
  });
};

export const getProductColor = (productCode) => {
  const url = `${API_BASE_URL}/list-colors-of-product/${productCode}`;
  return axios.get(url, {
    headers: {
      'accept': '*/*'
    }
  });
};

export const getProductSize = (productCode, color) => {
  const url = `${API_BASE_URL}/list-sizes-of-product/${productCode}?color=${color}`;
  return axios.get(url, {
    headers: {
      'accept': '*/*'
    }
  });
};

export const getProductCondition = (productCode, color, size) => {
  const url = `${API_BASE_URL}/list-conditions-of-product/${productCode}?color=${color}&size=${size}`;
  return axios.get(url, {
    headers: {
      'accept': '*/*'
    }
  });
};

export const getProductFilterBy = ( sortBy, isAscending, brandIds, categoryIds, minPrice, maxPrice) => {
  const url = `${API_BASE_URL}/filter-sort-products`;
  const params = {perPage};
  if (sortBy) {
    params.sortBy = sortBy;
  }
  if (typeof isAscending === 'boolean') {
    params.isAscending = isAscending;
  }

  if (brandIds && brandIds.length > 0) {
    brandIds.forEach((id, index) => {
      params[`brandIds[${index}]`] = id;
    });
  }

  if (categoryIds && categoryIds.length > 0) {
    categoryIds.forEach((id, index) => {
      params[`categoryIds[${index}]`] = id;
    });
  }

  if (minPrice !== 0) {
    params.minPrice = minPrice;
  }
  if (maxPrice !== 0) {
    params.maxPrice = maxPrice;
  }
  return axios.get(url, {
    params,
    headers: {
      'accept': '*/*'
    }
  });
};


export const searchProducts = (keywords) => {
  const url = `${API_BASE_URL}/search-products`;
  return axios.get(url, {
    params: { keywords },
    headers: {
      'accept': '*/*'
    }
  });
};