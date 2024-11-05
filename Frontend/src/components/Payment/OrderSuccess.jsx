// OrderSuccess.js
import React from "react";
import { FontAwesomeIcon } from "@fortawesome/react-fontawesome";
import { faCircleCheck } from "@fortawesome/free-solid-svg-icons";
import { useLocation } from "react-router-dom";
import CheckoutButton from "./CheckoutButton";

const OrderSuccess = () => {
  const location = useLocation();
  const { orderID, orderCode } = location.state || {};

  return (
    <div className="text-center py-10">
      <FontAwesomeIcon icon={faCircleCheck} size="5x" className="text-green-500" />
      <h1 className="text-2xl font-bold mt-4">Order Placed Successfully!</h1>
      <p className="mt-2">Thank you for your order.</p>
      
      <div className="mt-6">
        {orderID && orderCode ? (
          <CheckoutButton orderID={orderID} orderCode={orderCode} />
        ) : (
          <p className="text-red-500">Error: Missing order information.</p>
        )}
      </div>
    </div>
  );
};

export default OrderSuccess;
