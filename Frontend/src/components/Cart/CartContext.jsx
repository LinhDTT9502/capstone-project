import React, { createContext, useContext, useState, useEffect } from 'react';
import { useSelector } from 'react-redux';
import { selectCartItems } from '../../redux/slices/cartSlice';
import { getUserCart } from '../../services/cartService';

const CartContext = createContext();

export const CartProvider = ({ children, reload }) => {
  const [cartCount, setCartCount] = useState(0);
  const guestCartItems = useSelector(selectCartItems);

  useEffect(() => {
    const updateCartCount = async () => {
      const token = localStorage.getItem("token");

      if (token) {
        try {
          const cartData = await getUserCart(token);
          const totalItems = cartData.length;
          setCartCount(totalItems);
        } catch (error) {
          console.error("Failed to fetch user cart count:", error);
        }
      } else {
        const guestCartCount = guestCartItems.length;
        setCartCount(guestCartCount);
      }
    };

    updateCartCount();
  }, [guestCartItems, reload]);

  return (
    <CartContext.Provider value={{ cartCount, setCartCount }}>
      {children}
    </CartContext.Provider>
  );
};

export const useCart = () => {
  const context = useContext(CartContext);
  if (!context) {
    throw new Error('useCart must be used within a CartProvider');
  }
  return context;
};
