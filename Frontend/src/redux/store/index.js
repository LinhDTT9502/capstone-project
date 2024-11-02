import { configureStore } from "@reduxjs/toolkit";
import { persistStore } from "redux-persist";
import authSlice from "../slices/authSlice";
import productSlice from "../slices/productSlice";
import cartSlice from "../slices/cartSlice";
import shipmentSlice from "../slices/shipmentSlice";
import customerCartSlice from "../slices/customerCartSlice";

export const store = configureStore({
  reducer: {
    auth: authSlice,
    product: productSlice,
    cart: cartSlice,
    shipment: shipmentSlice,
    customerCart: customerCartSlice
  },
  middleware: (getDefaultMiddleware) =>
    getDefaultMiddleware({
      serializableCheck: {
        ignoredActions: ['persist/PERSIST', 'persist/REHYDRATE'], 
      },
    }),
});

export const persistor = persistStore(store);
