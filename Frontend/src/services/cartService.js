import { addToCartAPI, getCartAPI, reduceCartItemAPI, remmoveCartItemAPI, updateCartItemQuantityAPI } from '../api/apiCart';
import { toast } from "react-toastify";
 
export const addToCart = async (token, productId, quantity) => {
  try {
    const response = await addToCartAPI(token, productId, quantity);
  
    return response.data;
  } catch (error) {
    console.error('Add to cart failed', error);
    throw error;
  }
};

export const getUserCart = async (token) => {
  try {
    const response = await getCartAPI(token);
    return response.data.data.$values;
  } catch (error) {
    // console.error('Error fetching cart:', error);
    // toast.error('Error fetching cart');
    throw error;
  }
};

export const reduceCartItem = async (id, token) => {
  try {
    const response = await reduceCartItemAPI(id, token);
    return response;
  } catch (error) {
    console.error('Error fetching cart:', error);
    // toast.error('Error fetching cart');
    throw error;
  }
};

export const removeCartItem = async (id, token) => {
  try {
    const response = await remmoveCartItemAPI(id, token);
    return response;
  } catch (error) {
    console.error('Error fetching cart:', error);
    toast.error('Error fetching cart');
    throw error;
  }
};

export const updateCartItemQuantity = async (cartItemId, quantity, token) => {
  try {
    const response = await updateCartItemQuantityAPI(cartItemId, quantity, token);
    toast.success("Cart item quantity updated successfully");
    return response.data;
  } catch (error) {
    console.error('Error updating cart item quantity:', error);
    toast.error('Error updating cart item quantity: ' + error.message);
    throw error;
  }
};