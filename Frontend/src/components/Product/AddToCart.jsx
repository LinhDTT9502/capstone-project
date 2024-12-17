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
import { checkQuantityProduct } from '../../services/warehouseService';

const AddToCart = ({ product, quantity, selectedColor, selectedSize, selectedCondition }) => {
  const { t } = useTranslation();
  const dispatch = useDispatch();
  const { setCartCount } = useCart();

  const handleAddToCart = async () => {
    if (!selectedColor) {
      alert("Vui lòng chọn màu sắc, kích cỡ và tình trạng của sản phẩm!");
    } else if (!selectedSize) {
      alert("Vui lòng chọn kích cỡ và tình trạng của sản phẩm!");
    } else if (!selectedCondition) {
      alert("Vui lòng chọn tình trạng của sản phẩm!");
    } else {
      try {
        const response = await checkQuantityProduct(product.id);
  
        if (quantity <= response.availableQuantity) {
          const token = localStorage.getItem("token");
          if (!token) {
            dispatch(addCart(product));
            toast.info(`${product.productName} đã được thêm vào giỏ hàng`);
          } else {
            const addToCartResponse = await addToCart(token, product.id, quantity);
            console.log(addToCartResponse);
            toast.success(`${product.productName} đã được thêm vào giỏ hàng`);
            setCartCount((prevCount) => prevCount + quantity);
          }
        } else {
          alert(
            `Sản phẩm này chỉ còn lại ${response.availableQuantity} sản phẩm trong kho`
          );
        }
      } catch (error) {
  
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
