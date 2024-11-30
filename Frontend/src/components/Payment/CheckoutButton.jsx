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
  console.log(selectedOrder);
  

  const handleCheckout = async () => {
    if (!selectedOrder ) {
      alert("Please select a valid payment method.");
      return;
    }

    try {
      const token = localStorage.getItem("token");

      // Prepare the API request payload
      const data = {
        paymentMethodID: paymentMethodID,
        orderID: selectedOrder.id,
        orderCode: selectedOrder.saleOrderCode,
        transactionType: selectedOrder.transactionType,
      };

      // Call the API for VNPay checkout
      const response = await axios.post(
        "https://capstone-project-703387227873.asia-southeast1.run.app/api/Checkout/checkout-sale-order",
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
        if (paymentMethodID ==2 || paymentMethodID ==3) {
          window.location.href = paymentLink;
          return;
        } else (
          navigate('/manage-account/sale-order')
        )
       
     
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
