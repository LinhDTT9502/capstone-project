import React, { useState } from 'react';
import { useDispatch } from 'react-redux';
import { toast } from "react-toastify";
import { useTranslation } from "react-i18next";
import { addCart } from '../../redux/slices/cartSlice';
import { addCusCart } from '../../redux/slices/customerCartSlice';

const AddToCart = ({ product, warehouseId, initialQuantity = 0 }) => {
  const [quantity, setQuantity] = useState(initialQuantity);
  const { t } = useTranslation();
  const dispatch = useDispatch();

  const handleAddToCart = async (quantityToAdd = 1) => {
    const token = localStorage.getItem('token');
    if (!token) {
      dispatch(addCart(product));
      toast.info('Added to cart');
      return;
    } else {
      dispatch(addCusCart(product));
      toast.success(`${product.productName} has been added to the cart!`);
    }
  };

  return (
    <button
      className="absolute bottom-0 left-0 right-0 flex items-center justify-center bg-orange-600 bg-opacity-75 text-white opacity-0 hover:opacity-100 transition-opacity duration-300 py-4"
      onClick={() => handleAddToCart()}
    >
      {t("product_list.add_to_cart")}
    </button>
  );
};

export default AddToCart;
