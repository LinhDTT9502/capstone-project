// CheckoutButton.js
import React from "react";
import { Button } from "@material-tailwind/react";
import { useSelector } from "react-redux";
import { selectUser } from "../../redux/slices/authSlice";
import { useNavigate } from "react-router-dom";
import axios from "axios";

const CheckoutButton = ({ paymentMethodID, selectedOrder }) => {
  const user = useSelector(selectUser);
  const navigate = useNavigate();

  const handleCheckout = async () => {
    if (!selectedOrder || paymentMethodID !== "3") {
      alert("Please select a valid payment method.");
      return;
    }

    try {
      const token = localStorage.getItem("token");
      const userId = token ? user.UserId : 0;

      // Prepare the API request payload
      const data = {
        paymentMethodID: 3,
        orderID: selectedOrder.saleOrderId,
        orderCode: selectedOrder.orderCode,
        userId,
      };

      // Call the API for VNPay checkout
      const response = await axios.post(
        "https://twosportapi-295683427295.asia-southeast2.run.app/api/VnPay/checkout-sale-order",
        data,
        {
          headers: {
            Authorization: `Bearer ${token}`,
            "Content-Type": "application/json",
          },
        }
      );
      console.log(response);
      
        const paymentLink = response.data.data.paymentLink;
        // console.log(paymentLink);
        window.location.href = paymentLink;
     
    } catch (error) {
      console.error("Error during checkout:", error);
      alert("An error occurred during checkout. Please try again later.");
    }
  };

  return (
    <Button
      className="text-white bg-orange-500 w-40 py-3 rounded"
      onClick={handleCheckout}
    >
      Checkout
    </Button>
  );
};

export default CheckoutButton;
