import { configureStore } from "@reduxjs/toolkit";
import { persistStore } from "redux-persist";
import authSlice from "../slices/authSlice";
import productSlice from "../slices/productSlice";
import cartSlice from "../slices/cartSlice";
import shipmentSlice from "../slices/shipmentSlice";

export const store = configureStore({
  reducer: {
    auth: authSlice,
    product: productSlice,
    cart: cartSlice,
    shipment: shipmentSlice
  },
});

export const persistor = persistStore(store);
