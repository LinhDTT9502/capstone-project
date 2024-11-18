import { toast } from 'react-toastify';
import { placedOrderAPI } from '../../api/Checkout/apiCheckout';

export const placedOrder = async ( data) => {
  try {
    const response = await placedOrderAPI( data);
    return response.data;
  } catch (error) {
    console.error('Error  placed order:', error);
    toast.error('Error placed  order');
    throw error;
  }
};
