import React, { useState } from 'react';
import { useDispatch } from 'react-redux';
import { toast } from "react-toastify";
import { useTranslation } from "react-i18next";
import { addCart } from '../../redux/slices/cartSlice';
import { addCusCart } from '../../redux/slices/customerCartSlice';
import {

  Button,

} from "@material-tailwind/react";
import { addToCart } from '../../services/cartService';

const AddToCart = ({ product, quantity, isFormValid}) => {
  const { t } = useTranslation();
  const dispatch = useDispatch();

  const handleAddToCart = async () => {
    const token = localStorage.getItem('token');
    if (!token) {
      dispatch(addCart(product));
      toast.info('Added to cart');
      return;
    } else {
      const response = await addToCart(token, product.id, quantity)
      console.log(response);
      
      toast.success(`${product.productName} has been added to the cart!`);
    }
  };

  return (

    <Button
      className="py-4"
      onClick={() => handleAddToCart()}
      disabled={!isFormValid}
    >
      {t("product_list.add_to_cart")}
    </Button>
  );
};

export default AddToCart;
