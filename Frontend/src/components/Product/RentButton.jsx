import React, { useState } from 'react';
import { useDispatch } from 'react-redux';
import { toast } from "react-toastify";
import { useTranslation } from "react-i18next";
import { addCart } from '../../redux/slices/cartSlice';
import { addCusCart } from '../../redux/slices/customerCartSlice';
import {

  Button,

} from "@material-tailwind/react";

const RentButton = ({ product, warehouseId, initialQuantity = 0 }) => {
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

    <Button
      className="py-4"
      onClick={() => handleAddToCart()}
    >
     Thuê 
    </Button>
  );
};

export default RentButton;
