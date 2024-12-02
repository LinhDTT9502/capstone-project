import { useState } from "react";
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
    <div className="min-h-screen bg-slate-200 py-10 px-4 md:px-8">
      <div className="max-w-5xl mx-auto bg-white shadow-lg rounded-lg">
        <div className="grid grid-cols-1 md:grid-cols-2 gap-4 p-6">
          {/* Payment Method Section */}
          <div className="bg-gray-50 p-4 rounded-lg shadow-sm">
            <PaymentMethod
              selectedOption={selectedOption}
              handleOptionChange={handleOptionChange}
            />
          </div>

          {/* Order Summary Section */}
          {selectedOrder && (
            <div className="bg-gray-50 p-4 rounded-lg shadow-sm">
              

              <h2 className="text-xl font-semibold mb-4">Order Summary</h2>
              <p className="text-gray-600">
                <span className="font-medium">Order Code:</span>{" "}
                {selectedOrder.saleOrderCode}-{selectedOrder.id}
              </p>
              <p className="text-gray-600">
                <span className="font-medium">Order Status:</span> {selectedOrder.orderStatus}
              </p>
              <p className="text-gray-600">
                <span className="font-medium">Delivery Method:</span>{" "}
                {selectedOrder.deliveryMethod.replace("_", " ")}
              </p>
              <p className="text-gray-600">
                <span className="font-medium">Payment Method:</span>{" "}
                {selectedOrder.paymentMethod}
              </p>
              <p className="text-gray-600">
                <span className="font-medium">Recipient:</span> {selectedOrder.fullName}{" "}
                ({selectedOrder.gender})
              </p>
              <p className="text-gray-600">
                <span className="font-medium">Contact Phone:</span>{" "}
                {selectedOrder.contactPhone}
              </p>
              <p className="text-gray-600">
                <span className="font-medium">Email:</span> {selectedOrder.email}
              </p>
              <p className="text-gray-600">
                <span className="font-medium">Address:</span> {selectedOrder.address}
              </p>
              <p className="text-gray-600">
                <span className="font-medium">Estimated Delivery:</span>{" "}
                {new Date(selectedOrder.dateOfReceipt).toLocaleDateString()}
              </p>
              <h3 className="text-lg font-semibold mt-4">Order Items</h3>
              <ul className="space-y-2">
                {selectedOrder.saleOrderDetailVMs.$values.map((item) => (
                  <li
                    key={item.productId}
                    className="flex items-center justify-between bg-white p-2 rounded-lg shadow-sm"
                  >
                    <div className="flex items-center">
                      <img
                        src={item.imgAvatarPath}
                        alt={item.productName}
                        className="w-12 h-12 object-cover rounded-md mr-3"
                      />
                      <div>
                        <p className="font-medium">{item.productName}</p>
                        <p className="text-gray-500">x{item.quantity}</p>
                      </div>
                    </div>
                    <span className="text-gray-700 font-medium">
                      ${item.unitPrice.toFixed(2)}
                    </span>
                  </li>
                ))}
              </ul>
              <div className="mt-4 border-t pt-4">
                <p className="flex justify-between text-gray-700 font-medium">
                  <span>Subtotal:</span>
                  <span>${selectedOrder.subTotal.toFixed(2)}</span>
                </p>
                <p className="flex justify-between text-gray-700 font-medium">
                  <span>Transport Fee:</span>
                  <span>${selectedOrder.tranSportFee.toFixed(2)}</span>
                </p>
                <p className="flex justify-between text-lg font-semibold">
                  <span>Total:</span>
                  <span>${selectedOrder.totalAmount.toFixed(2)}</span>
                </p>
              </div>
              <CheckoutButton
                paymentMethodID={selectedOption}
                selectedOrder={selectedOrder}
                className="mt-4"
              />
            </div>
          )}
        </div>
      </div>
    </div>
  );
};

export default Checkout;
