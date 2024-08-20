// src/redux/slices/productSlice.js
import { createSlice } from "@reduxjs/toolkit";
import { getProductFilterBy } from "../../api/apiProduct";

const initialState = {
  products: [],
  total: 0,
};

const productSlice = createSlice({
  name: "product",
  initialState,
  reducers: {
    setProducts: (state, action) => {
      const { products, total } = action.payload.data;
      state.products = products;
      state.total = total;
    },
  },
});

export const { setProducts } = productSlice.actions;

export const fetchFilteredProducts = (params) => async (dispatch) => {
  try {
    const response = await getProductFilterBy(params);
    dispatch(setProducts({ data: response.data.data.$values }));
  } catch (error) {
    console.error('Error fetching filtered products:', error);
    // Handle error, e.g., dispatch an error action
  }
};

export const selectProducts = (state) => state.product;

export default productSlice.reducer;
