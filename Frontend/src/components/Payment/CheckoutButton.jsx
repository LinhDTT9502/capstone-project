// CheckoutButton.js
import React, { useEffect, useState } from "react";
import { Button } from "@material-tailwind/react";
import { useSelector } from "react-redux";
import { selectUser } from "../../redux/slices/authSlice";
import { useNavigate } from "react-router-dom";
import axios from "axios";
import { toast } from "react-toastify";

const CheckoutButton = ({ paymentMethodID, selectedOrder }) => {
  const user = useSelector(selectUser);
  const navigate = useNavigate();
  const [loading, setLoading] = useState(false);

  const handleCheckout = async () => {
    if (!selectedOrder ) {
      toast.error("Chưa chọn phương thức thanh toán!");
      return;
    }
    setLoading(true);
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
        "https://twosport-api-offcial-685025377967.asia-southeast1.run.app/api/Checkout/checkout-sale-order",
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
    }finally{
      setLoading(false)
    }
  };

  return (
    <Button
      disabled={loading}
      className={`mt-6 w-full bg-orange-500 text-white py-2 rounded-md ${
        loading ? "opacity-50 cursor-not-allowed" : ""
      }`}
      onClick={handleCheckout}
    >
      {loading ? "Đang xử lý..." : "Thanh toán"}
    </Button>
  );
};

export default CheckoutButton;
