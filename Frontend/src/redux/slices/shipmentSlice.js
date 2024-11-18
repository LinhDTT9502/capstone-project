import { createSlice } from "@reduxjs/toolkit";
import { persistReducer } from "redux-persist";
import storage from "redux-persist/lib/storage";

const shipmentPersistConfig = {
    key: "shipment",
    storage,
};

const initialState = {
    shipment: [],
    selectedShipments: null,
    wardCode: null,
    districtId: null,
};

const shipmentSlice = createSlice({
    name: "shipment",
    initialState,
    reducers: {
        setShipment: (state, action) => {
            state.shipment = action.payload;
            state.wardCode = action.payload.wardCode;
            state.districtId = action.payload.districtId;
        },
        selectShipments: (state, action) => {
            state.selectedShipments = action.payload;
        },
        updateShipment: (state, action) => {
            const index = state.shipment.findIndex(shipment => shipment.id === action.payload.id);
            if (index !== -1) {
                state.shipment[index] = action.payload;
            }
        },
        addShipment: (state, action) => {
            state.shipment.push(action.payload);
        },

        deleteShipment: (state, action) => {
            state.shipment = state.shipment.filter(shipment => shipment.id !== action.payload);
        },
    },
});

export const { setShipment, selectShipments, updateShipment, addShipment, deleteShipment } = shipmentSlice.actions;

export const selectShipment = (state) => state.shipment?.shipment || [];
export const selectedShipment = (state) => state.shipment.selectedShipments;

// export default persistReducer(shipmentPersistConfig, shipmentSlice.reducer);
export default shipmentSlice.reducer;
