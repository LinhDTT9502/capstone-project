import React from 'react';
import { Route, Routes } from 'react-router-dom';
import Dashboard from '../components/Admin/Dashboard';

const AdminRoutes = () => {
  return (
    <Routes>
      <Route path="/dashboard" element={<Dashboard />} />
      {/* <Route path=":productId" element={<ProductDetails />} /> */}
    </Routes>
  );
};

export default AdminRoutes;
