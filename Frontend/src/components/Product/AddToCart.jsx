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
import { useCart } from '../Cart/CartContext';

const AddToCart = ({ product, quantity, selectedColor, selectedSize, selectedCondition }) => {
  const { t } = useTranslation();
  const dispatch = useDispatch();
  const { setCartCount } = useCart();

  const handleAddToCart = async () => {
    if (!selectedColor) {
      alert('Vui lòng chọn màu sắc, kích cỡ và tình trạng của sản phẩm!')
    } else if (!selectedSize) {
      alert('Vui lòng chọn kích cỡ và tình trạng của sản phẩm!')
    } else if (!selectedCondition) {
      alert('Vui lòng chọn tình trạng của sản phẩm!')
    } else {
      const token = localStorage.getItem('token');
      if (!token) {
        dispatch(addCart(product));
        // alert(`${product.productName} đã được thêm vào giỏ hàng`)
        toast.info(`${product.productName} đã được thêm vào giỏ hàng`);
        return;
      } else {
        const response = await addToCart(token, product.id, quantity)
        console.log(response);
        toast.success(`${product.productName} đã được thêm vào giỏ hàng`);
        setCartCount((prevCount) => prevCount + quantity);
        // alert(`${product.productName} đã được thêm vào giỏ hàng`)
        
      }
    }
  };

  return (

    <Button
      className="w-full"
      onClick={() => handleAddToCart()}
    >
      {t("product_list.add_to_cart")}
    </Button>
  );
};

export default AddToCart;
