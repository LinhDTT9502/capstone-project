import { createSlice } from '@reduxjs/toolkit';

const loadCustomerCart = () => {
  try {
    const serializedState = localStorage.getItem('customerCart');
    if (serializedState === null) {
      return [];
    }
    return JSON.parse(serializedState);
  } catch (err) {
    return [];
  }
};

const saveCustomerCart = (state) => {
  try {
    const serializedState = JSON.stringify(state);
    localStorage.setItem('customerCart', serializedState);
  } catch {
  }
};

const initialState = {
  items: loadCustomerCart(),
};

const customerCartSlice = createSlice({
  name: 'customerCart',
  initialState,
  reducers: {
    addCusCart: (state, action) => {
      const product = state.items.find(item => item.id === action.payload.id);
      if (product) {
        product.quantity += 1;
      } else {
        state.items.push({ ...action.payload, quantity: 1 });
      }
      saveCustomerCart(state.items);
    },
    removeFromCusCart: (state, action) => {
      state.items = state.items.filter(item => item.id !== action.payload);
      saveCustomerCart(state.items);
    },
    decreaseCusQuantity: (state, action) => {
      const product = state.items.find(item => item.id === action.payload);
      if (product && product.quantity > 1) {
        product.quantity -= 1;
      } else {
        state.items = state.items.filter(item => item.id !== action.payload);
      }
      saveCustomerCart(state.items);
    },
    clearCusCart: (state) => {
      state.items = [];
      saveCustomerCart(state.items);
    },
  },
});

export const { addCusCart, removeFromCusCart, decreaseCusQuantity, clearCusCart } = customerCartSlice.actions;

export const selectCustomerCartItems = (state) => state.customerCart.items;

export default customerCartSlice.reducer;
