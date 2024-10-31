import { toast } from 'react-toastify';
import { checkoutOrder } from '../api/apiPayment';

export const checkout = async (token, data) => {
  try {
    const response = await checkoutOrder(token, data);
    return response.data;
  } catch (error) {
    console.error('Error checking out order:', error);
    toast.error('Error checking out order');
    throw error;
  }
};
