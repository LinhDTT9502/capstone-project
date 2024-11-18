import { createSlice } from "@reduxjs/toolkit";
import { persistReducer } from "redux-persist";
import storage from "redux-persist/lib/storage";

const orderPersistConfig = {
  key: "order",
  storage,
};

const guestOrderSlice = createSlice({
  name: "guestOrder",
  initialState: {
    orders: [], 
  },
  reducers: {
    addGuestOrder: (state, action) => {
      state.orders.push(action.payload);
    },
  },
});

export const { addGuestOrder } = guestOrderSlice.actions;
export const selectGuestOrders = (state) => state.guestOrder.orders;
export default persistReducer(orderPersistConfig, guestOrderSlice.reducer);