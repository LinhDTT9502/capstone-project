import React from 'react';
import { Route, Routes } from 'react-router-dom';
import Dashboard from '../components/Admin/Dashboard';
import ManageUser from '../components/Admin/ManageUser';

const AdminRoutes = () => {
  return (
    <Routes>
      <Route path="/dashboard" element={<Dashboard />} />
      <Route path="/manage-user" element={<ManageUser />} />
      {/* <Route path=":productId" element={<ProductDetails />} /> */}
    </Routes>
  );
};

export default AdminRoutes;
