
import { toast } from "react-toastify";
import { addShipmentDetail, deleteShipmentDetail, getShipmentDetails, updateShipmentDetail } from "../api/apiShipment";

export const getUserShipmentDetails = async (token) => {
  try {
    const response = await getShipmentDetails(token);
    return response.data;
  } catch (error) {
    console.error('Error fetching shipments:', error);
    toast.error('Error fetching shipments');
    throw error;
  }
};

export const addUserShipmentDetail = async (token, data) => {
  try {
    const response = await addShipmentDetail(token, data);
    return response;
  } catch (error) {
    console.error('Error saving shipment details:', error);
    toast.error('Error saving shipment details');
    throw error;
  }
};

export const updateUserShipmentDetail = async (id, token, data) => {
  try {
    const response = await updateShipmentDetail(id, token, data);
    return response.data;
  } catch (error) {
    console.error('Error updating shipment details:', error);
    toast.error('Error updating shipment details');
    throw error;
  }
};

export const deleteUserShipmentDetail = async (id, token) => {
  try {
    const response = await deleteShipmentDetail(id, token);
    return response;
  } catch (error) {
    console.error('Error deleting shipment details:', error);
    toast.error('Error deleting shipment details');
    throw error;
  }
};