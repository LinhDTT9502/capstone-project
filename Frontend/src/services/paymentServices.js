import { toast } from 'react-toastify';
import { checkoutOrder } from '../api/apiPayment';

export const checkout = async (token, orderMethodId, data) => {
  try {
    const response = await checkoutOrder(token, orderMethodId, data);
    return response.data;
  } catch (error) {
    console.error('Error checking out order:', error);
    toast.error('Error checking out order');
    throw error;
  }
};
