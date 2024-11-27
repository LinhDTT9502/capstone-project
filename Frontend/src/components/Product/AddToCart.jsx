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

const AddToCart = ({ product, quantity, selectedColor, selectedSize, selectedCondition }) => {
  const { t } = useTranslation();
  const dispatch = useDispatch();

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
        alert(`${product.productName} đã được thêm vào giỏ hàng`)
        toast.info('Added to cart');
        return;
      } else {
        const response = await addToCart(token, product.id, quantity)
        console.log(response);
        alert(`${product.productName} đã được thêm vào giỏ hàng`)
        toast.success(`${product.productName} has been added to the cart!`);
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
