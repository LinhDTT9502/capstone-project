// src/Blog.js
import React from 'react';
import { useSelector } from 'react-redux';
import { selectUser } from '../redux/slices/authSlice';
import UserCart from '../components/Cart/UserCart';
import GuestCart from '../components/Cart/GuestCart';

const Cart = () => {

  const user = useSelector(selectUser)

  return (
    <>
      {user ? (
        <UserCart/>
      ) : (<GuestCart/>)}
    </>

  );
};

export default Cart;