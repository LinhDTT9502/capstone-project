import { useState, useEffect } from "react";
import PaymentMethod from "../components/Payment/PaymentMethod";
import { useLocation } from "react-router-dom";
import CheckoutButton from "../components/Payment/CheckoutButton";

const Checkout = () => {
  const location = useLocation();
  const [selectedOption, setSelectedOption] = useState(null);
  const selectedOrder = location.state?.selectedOrder || null;

  const handleOptionChange = (event) => {
    setSelectedOption(event.target.value);
  };

  return (
    <div className="px-5 py-5 flex flex-col bg-slate-200">
      <PaymentMethod
        selectedOption={selectedOption}
        handleOptionChange={handleOptionChange}
      />

      {/* Display the selected order details */}
      {selectedOrder && (
        <div className="mt-5 p-4 bg-white rounded-lg shadow-md">
          <h2 className="text-lg font-bold">Order Summary</h2>
          <p>Order Code: {selectedOrder.orderCode}-{selectedOrder.saleOrderId}</p>
          <p>Delivery Method: {selectedOrder.deliveryMethod}</p>
          <p>Payment Method: {selectedOrder.paymentMethod}</p>
          <p>Total Amount: ${selectedOrder.totalAmount}</p>
          <h3 className="text-md font-semibold mt-3">Order Items:</h3>
          <ul>
            {selectedOrder.saleOrderDetailVMs.$values.map((item) => (
              <li key={item.productId} className="flex justify-between py-1">
                <span>{item.productName} (x{item.quantity})</span>
                <span>${item.unitPrice}</span>
              </li>
            ))}
          </ul>
        </div>
      )}

      {/* Pass selectedOption (payment method) and selectedOrder to the CheckoutButton */}
      <CheckoutButton paymentMethodID={selectedOption} selectedOrder={selectedOrder} />
    </div>
  );
};

export default Checkout;
