import React from "react";
import { useLocation } from "react-router-dom";
import { Card, CardBody, Typography } from "@material-tailwind/react";

const GuestOrderDetail = () => {
  const { state } = useLocation();
  const order = state?.order;

  if (!order) {
    return (
      <div className="flex justify-center items-center py-10">
        <Typography variant="h5" color="red">
          Order not found.
        </Typography>
      </div>
    );
  }

  const {
    saleOrderId,
    orderCode,
    fullName,
    totalAmount,
    saleOrderDetailVMs,
  } = order;

  return (
    <div className="max-w-4xl mx-auto py-10">
      <Card className="p-6 bg-white shadow-lg">
        <CardBody>
          <Typography variant="h4" color="blue-gray" className="mb-5">
            Order Detail
          </Typography>

          <Typography variant="h6">Order ID: {saleOrderId}</Typography>
          <Typography>Order Code: {orderCode}</Typography>
          <Typography>Total Amount: {totalAmount.toLocaleString()} VND</Typography>

          <Typography variant="h5" className="mt-8 mb-4">
            Order Items
          </Typography>
          {saleOrderDetailVMs.$values.map((item) => (
            <div key={item.productId} className="flex items-center border-b py-4">
              <img
                src={item.imgAvatarPath}
                alt={item.productName}
                className="w-20 h-20 object-cover rounded"
              />
              <div className="ml-4">
                <Typography>{item.productName}</Typography>
                <Typography>Quantity: {item.quantity}</Typography>
                <Typography>Price: {item.unitPrice.toLocaleString()} VND</Typography>
              </div>
            </div>
          ))}
        </CardBody>
      </Card>
    </div>
  );
};

export default GuestOrderDetail;
