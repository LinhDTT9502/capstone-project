import { getUserOrders } from "../api/apiUserOrder";

export const fetchUserOrders = async (id, token) => {
    try {
      const response = await getUserOrders(id, token);
      return response.data.data.$values;
    } catch (error) {
      throw error;
    }
  };
  