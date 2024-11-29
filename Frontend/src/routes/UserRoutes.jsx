import React from 'react';
import { Route, Routes } from 'react-router-dom';
import UserShipment from '../components/User/UserShipment';
import UserProfile from '../components/User/UserProfile';
import ManageAccount from '../pages/ManageAccount';
import NotFoundPage from '../pages/NotFoundPage';
import UserChangePassword from '../components/User/UserChangePassword';
import UserOrderStatus from '../components/User/UserOrderStatus';
import UserListRental from '../components/User/UserListRental';
import RefundForm from '../components/Refund/RefundForm';
import UserRentalDetail from '../components/Rental/UserRentalDetail';

const UserRoutes = () => {
  return (
    <Routes>
      <Route path="/" element={<ManageAccount />}>
        <Route path="profile" element={<UserProfile />} />
        <Route path="shipment" element={<UserShipment />} />
        <Route path="change-password" element={<UserChangePassword />} />
        <Route path="sale-order" element={<UserOrderStatus />} />
        <Route path="user-rental" element={<UserListRental />} />
        <Route path="user-rental/:orderId" element={<UserRentalDetail />} />
        <Route path="refund-request" element={<RefundForm />} />
        <Route path="*" element={<NotFoundPage />} />
      </Route>
    </Routes>
  );
};

export default UserRoutes;
