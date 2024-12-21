import { getUserOrders, getUserRentalOrders } from "../api/apiUserOrder";

export const fetchUserOrders = async (id, token) => {
    try {
      const response = await getUserOrders(id, token);
      return response.data.data.$values;
    } catch (error) {
      throw error;
    }
  };

export const fetchUserRentalOrders = async (id, token) => {
    try {
      const response = await getUserRentalOrders(id, token);
      return response.data.data.$values;
    } catch (error) {
      throw error;
    }
  }; 