import React from 'react';
import { useSelector } from 'react-redux';
import { Navigate } from 'react-router-dom';
import { selectUser } from '../redux/slices/authSlice';

function PrivateRoute({ children, allowedRoles }) {
  const user = useSelector(selectUser);

  if (!user) {
    alert('Bạn không có quyền để xem trang này!');
    // If the user is not logged in, redirect to the home page
    return <Navigate to="/" replace />;
  }

  // if (!allowedRoles.includes(user.role)) {
  //   // If the user does not have the required role, show an alert and redirect
  //   alert('Bạn không có quyền để xem trang này!');
  //   return <Navigate to="/" replace />;
  // }

  // If authenticated and has a valid role, render the children
  return children;
}

export default PrivateRoute;
