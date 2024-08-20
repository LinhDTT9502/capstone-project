import React from 'react';
import { Route, Routes } from 'react-router-dom';
import ProductList from '../pages/ProductList';
import ProductDetails from '../pages/ProductDetails';
import ProductPage from '../pages/ProductPage';

const ProductRoutes = () => {
  return (
    <Routes>
      <Route path="/" element={<ProductPage />} />
      <Route path=":productId" element={<ProductDetails />} />
    </Routes>
  );
};

export default ProductRoutes;
