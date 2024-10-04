import React from 'react';
import { Route, Routes } from 'react-router-dom';
import Warehouse from '../components/Staff/Warehouse';

const StaffRoutes = () => {
  return (
    <Routes>
      <Route path="/warehouse" element={<Warehouse />} />
    </Routes>
  );
};

export default StaffRoutes;
