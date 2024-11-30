import React from "react";
import { Button, Card, Typography } from "@material-tailwind/react";

const Invoice = ({ data }) => {
  if (!data) return <div>Loading...</div>;

  const {
    saleOrderId,
    orderCode,
    fullName,
    gender,
    email,
    contactPhone,
    address,
    deliveryMethod,
    paymentMethod,
    subTotal,
    tranSportFee,
    totalAmount,
    dateOfReceipt,
    note,
    orderStatus,
    paymentStatus,
    createdAt,
    saleOrderDetailVMs,
  } = data;

  return (
    <div className="container mx-auto p-4">
      <Card className="shadow-lg p-6 rounded-lg max-w-4xl mx-auto">
        <Typography variant="h4" className="text-center font-bold mb-4">
          Invoice #{orderCode}
        </Typography>

        <div className="mb-6">
          <Typography variant="h6" className="mb-2">
            Customer Information
          </Typography>
          <div className="grid grid-cols-1 sm:grid-cols-2 gap-4">
            <div>
              <p><strong>Name:</strong> {fullName}</p>
              <p><strong>Gender:</strong> {gender}</p>
              <p><strong>Email:</strong> {email}</p>
              <p><strong>Phone:</strong> {contactPhone}</p>
              <p><strong>Address:</strong> {address}</p>
            </div>
            <div>
              <p><strong>Delivery Method:</strong> {deliveryMethod}</p>
              <p><strong>Payment Method:</strong> {paymentMethod}</p>
              <p><strong>Date of Receipt:</strong> {new Date(dateOfReceipt).toLocaleDateString()}</p>
              <p><strong>Order Status:</strong> {orderStatus}</p>
              <p><strong>Payment Status:</strong> {paymentStatus}</p>
            </div>
          </div>
        </div>

        <div className="mb-6">
          <Typography variant="h6" className="mb-2">
            Order Details
          </Typography>
          <div className="overflow-x-auto">
            <table className="table-auto w-full border border-gray-200">
              <thead>
                <tr className="bg-gray-100">
                  <th className="px-4 py-2">Image</th>
                  <th className="px-4 py-2">Product Name</th>
                  <th className="px-4 py-2">Unit Price</th>
                  <th className="px-4 py-2">Quantity</th>
                  <th className="px-4 py-2">Total</th>
                </tr>
              </thead>
              <tbody>
                {saleOrderDetailVMs.$values.map((item, index) => (
                  <tr key={index} className="text-center border-t">
                    <td className="px-4 py-2">
                      <img
                        src={item.imgAvatarPath}
                        alt={item.productName}
                        className="w-16 h-16 mx-auto rounded-md"
                      />
                    </td>
                    <td className="px-4 py-2">{item.productName}</td>
                    <td className="px-4 py-2">{item.unitPrice.toLocaleString()} ₫</td>
                    <td className="px-4 py-2">{item.quantity}</td>
                    <td className="px-4 py-2">{item.totalPrice.toLocaleString()} ₫</td>
                  </tr>
                ))}
              </tbody>
            </table>
          </div>
        </div>

        <div className="mb-6">
          <Typography variant="h6" className="mb-2">
            Payment Summary
          </Typography>
          <div className="grid grid-cols-1 sm:grid-cols-2 gap-4">
            <p><strong>Subtotal:</strong> {subTotal.toLocaleString()} ₫</p>
            <p><strong>Transport Fee:</strong> {tranSportFee.toLocaleString()} ₫</p>
            <p><strong>Total Amount:</strong> {totalAmount.toLocaleString()} ₫</p>
            <p><strong>Created At:</strong> {new Date(createdAt).toLocaleString()}</p>
            <p><strong>Note:</strong> {note}</p>
          </div>
        </div>

        <div className="text-center mt-6">
          <Button
            href={data.paymentLink}
            target="_blank"
            className="bg-blue-500 hover:bg-blue-700 text-white"
          >
            Proceed to Payment
          </Button>
        </div>
      </Card>
    </div>
  );
};

export default Invoice;
