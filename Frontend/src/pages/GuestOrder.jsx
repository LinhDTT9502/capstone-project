import React from "react";
import { useSelector } from "react-redux";
import { Card, CardBody, Typography, Button } from "@material-tailwind/react";
import { selectGuestOrders } from "../redux/slices/guestOrderSlice";
import { Link, useNavigate } from "react-router-dom";
import { faArrowLeft } from "@fortawesome/free-solid-svg-icons";
import { FontAwesomeIcon } from "@fortawesome/react-fontawesome";
import { t } from "i18next";

const GuestOrder = () => {
  const orders = useSelector(selectGuestOrders);
  const navigate = useNavigate();

  if (!orders || orders.length === 0) {
    return (
      <div className="flex flex-col items-center my-10 py-32">
        <img src="/assets/images/cart-icon.png" className="w-48 h-auto object-contain" />
        <p className="pt-4 text-lg font-poppins">{t("guest_order.empty")}</p>
        <Link
          to="/product"
          className="text-blue-500 flex items-center font-poppins"
        >
          <FontAwesomeIcon className="pr-2" icon={faArrowLeft} />{" "}
          {t("guest_order.continue_shopping")}
        </Link>
      </div>
    );
  }

  return (
    <div className="max-w-4xl mx-auto py-10">
      <Typography variant="h4" color="blue-gray" className="mb-5">
        Guest Orders
      </Typography>

      {orders.map((order) => (
        <Card key={order.saleOrderId} className="p-6 mb-5 bg-white shadow-lg">
          <CardBody>
            <Typography variant="h6">
              Order Code: {order.orderCode}
            </Typography>
            <Typography>Order ID: {order.saleOrderId}</Typography>
            <Typography>Total Amount: {order.totalAmount.toLocaleString()} ₫</Typography>
            <Typography>Order Status: {order.orderStatus}</Typography>

            <Button
              color="blue"
              onClick={() =>
                navigate(`/guest-order/${order.saleOrderId}`, {
                  state: { order },
                })
              }
              className="mt-4"
            >
              View Order Details
            </Button>
          </CardBody>
        </Card>
      ))}
    </div>
  );
};

export default GuestOrder;
