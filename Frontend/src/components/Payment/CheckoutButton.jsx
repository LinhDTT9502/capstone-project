// CheckoutButton.js
import React from "react";
import { Button } from "@material-tailwind/react";
import { useSelector } from "react-redux";
import { selectUser } from "../../redux/slices/authSlice";
import { useNavigate } from "react-router-dom";
import axios from "axios";

const CheckoutButton = ({ orderID, orderCode }) => {
  const user = useSelector(selectUser);
  const navigate = useNavigate();

  const handleCheckout = async () => {
    try {
      const token = localStorage.getItem("token");
      const userId = token ? user.UserId : 0;
      const data = {
        paymentMethodID: 2,
        orderID,
        orderCode,
        userId,
      };

      const response = await axios.post(
        "https://twosportapi-295683427295.asia-southeast2.run.app/api/Checkout/checkout-by-payOs",
        data,
        {
          headers: {
            Authorization: `Bearer ${token}`,
            "Content-Type": "application/json",
          },
        }
      );

      if (response.data.isSuccess && response.data.data.paymentLink) {
        // Redirect to the payment link
        window.location.href = response.data.data.paymentLink;
      } else {
        console.error("Checkout failed:", response.data.message);
      }
    } catch (error) {
      console.error("Error during checkout:", error);
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
