// CheckoutButton.js
import React, { useEffect, useState } from "react";
import { Button } from "@material-tailwind/react";
import { useSelector } from "react-redux";
import { selectUser } from "../../redux/slices/authSlice";
import { useNavigate } from "react-router-dom";
import axios from "axios";

const CheckoutButton = ({ paymentMethodID, selectedOrder }) => {
  const user = useSelector(selectUser);
  const navigate = useNavigate();

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
        transactionType: null,
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
      const orderData = response.data.data;
      console.log(orderData);
      console.log(response);
        const paymentLink = response.data.data.paymentLink;
        // console.log(paymentLink);
        if (paymentMethodID ==2 || paymentMethodID ==3) {
          window.location.href = paymentLink;
          return;
        } else (
          navigate('/payment-success', { state: { orderData } })
        )
       
     
    } catch (error) {
      console.error("Error during checkout:", error);
      alert("An error occurred during checkout. Please try again later.");
    }
  };
//   useEffect(() => {
//     const params = new URLSearchParams(window.location.search);
//     const isSuccess = params.get("isSuccess");
//     const orderCode = params.get("orderCode");
//     const status = params.get("status");
// console.log(isSuccess );
// console.log(orderCode);
// console.log(status);



//     if (isSuccess && orderCode) {
//       // Process the response data (you can save this to state or localStorage)
//       const orderDetails = {
//         isSuccess: isSuccess === "true", // Convert to boolean
//         orderCode,
//         status,
//       };

//       // Save order details for future use (localStorage or state)
//       localStorage.setItem("orderDetails", JSON.stringify(orderDetails));

//       if (isSuccess === "true") {
//         // Redirect to the desired link
//         window.location.href = "https://localhost:5173";
//       } else {
//         alert("Payment failed. Please try again.");
//       }
//     }
//   }, []);

  return (
    <Button
      className="mt-6 w-full bg-orange-500 text-white py-2 rounded-md"
      onClick={handleCheckout}
    >
      Thanh toán
    </Button>
  );
};

export default CheckoutButton;
