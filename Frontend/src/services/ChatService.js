import { getCustomerChat, getListChat } from "../api/apiChat";

export const fetchListChat = async () => {
  try {
    const response = await getListChat();
    return response.data.$values;
  } catch (error) {
    console.error('Error fetching orders list:', error);
    throw error;
  }
};

export const fetchCustomerChat = async ( ) => {
    try {
      const response = await getCustomerChat();
      return response.data;
    } catch (error) {
      console.error('Error fetching orders list:', error);
      throw error;
    }
  };