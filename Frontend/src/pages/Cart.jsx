import React, { useState } from "react";
import { useSelector, useDispatch } from "react-redux";
import { Link, useNavigate } from "react-router-dom";
import {
  selectCartItems,
  removeFromCart,
  decreaseQuantity,
  addCart,
} from "../redux/slices/cartSlice";
import { FontAwesomeIcon } from "@fortawesome/react-fontawesome";
import { faTrash, faArrowLeft } from "@fortawesome/free-solid-svg-icons";
import { ToastContainer, toast } from "react-toastify";
import "react-toastify/dist/ReactToastify.css";
import { useTranslation } from "react-i18next";

const Cart = () => {
  const { t } = useTranslation();
  const dispatch = useDispatch();
  const cartItems = useSelector(selectCartItems);
  const [selectedItems, setSelectedItems] = useState([]);
  const navigate = useNavigate();

  const handleRemoveFromCart = (productId) => {
    dispatch(removeFromCart(productId));
  };

  const handleQuantityChange = (product, change) => {
    if (change === "increase") {
      dispatch(addCart(product));
    } else if (change === "decrease") {
      dispatch(decreaseQuantity(product.id));
    }
  };

  const handleSelectItem = (productId) => {
    setSelectedItems((prevSelected) =>
      prevSelected.includes(productId)
        ? prevSelected.filter((id) => id !== productId)
        : [...prevSelected, productId]
    );
  };

  const handleSelectAll = () => {
    if (selectedItems.length === cartItems.length) {
      setSelectedItems([]);
    } else {
      setSelectedItems(cartItems.map((item) => item.id));
    }
  };

  const totalItems = cartItems.reduce((acc, item) => acc + item.quantity, 0);
  const totalPrice = selectedItems.reduce((acc, id) => {
    const item = cartItems.find((item) => item.id === id);
    return acc + item.price * item.quantity;
  }, 0);

  const handleCheckout = () => {
    if (selectedItems.length === 0) {
      toast.error("Please select at least one item to checkout.");
      return;
    }

    const selectedProducts = cartItems.filter((item) =>
      selectedItems.includes(item.id)
    );

    navigate("/checkout", { state: { selectedProducts } });
  };

  return (
    <div className="container mx-auto px-20 py-5">
      <ToastContainer />
      <div className="flex justify-between items-center mb-4">
        <h1 className="font-alfa text-orange-500 text-2xl">
          {t("cart.title")}
        </h1>
        <span className="font-alfa text-orange-500 text-xl">
          {totalItems} {t("cart.items")}
        </span>
      </div>
      {cartItems.length === 0 ? (
        <p>{t("cart.empty")}</p>
      ) : (
        <div className="w-full">
          <div className="bg-zinc-100 rounded-lg overflow-hidden shadow-lg">
            <div className="flex items-center justify-between p-4 bg-zinc-300">
              <div className="w-1/12 text-center ">
                <input
                  type="checkbox"
                  checked={selectedItems.length === cartItems.length}
                  onChange={handleSelectAll}
                />
              </div>
              <div className="w-5/12 text-center font-poppins text-lg font-bold">
                {t("cart.products")}
              </div>
              <div className="w-2/12 text-center font-poppins text-lg font-bold">
                {t("cart.quantity")}
              </div>
              <div className="w-1/12 text-center font-poppins text-lg font-bold">
                {t("cart.price")}
              </div>
              <div className="w-2/12 text-center font-poppins text-lg font-bold">
                {t("cart.total")}
              </div>
              <div className="w-1/12 text-center font-poppins text-lg font-bold">
                {t("cart.action")}
              </div>
            </div>
            {cartItems.map((item) => (
              <div
                key={item.id}
                className="flex items-center justify-between p-4 border-b hover:bg-zinc-200"
              >
                <div className="w-1/12 text-center">
                  <input
                    type="checkbox"
                    checked={selectedItems.includes(item.id)}
                    onChange={() => handleSelectItem(item.id)}
                  />
                </div>
                <div className="w-5/12 flex items-center">
                  <img
                    src={item.mainImagePath}
                    alt={item.mainImageName}
                    className="w-16 h-16 object-cover mr-4"
                  />
                  <Link
                    to={`/product/${item.id}`}
                    className="text-sm font-poppins font-bold text-wrap w-1/2"
                  >
                    {item.productName}
                  </Link>
                  <div className="pl-10">
                    <label>{t("cart.type")}:</label>
                    <p>
                      {item.size} {item.color}
                    </p>
                  </div>
                </div>
                <div className="w-2/12 text-center flex items-center justify-center">
                  <button
                    className="px-2 py-1 border"
                    onClick={() => handleQuantityChange(item, "decrease")}
                  >
                    -
                  </button>
                  <span className="mx-2">{item.quantity}</span>
                  <button
                    className="px-2 py-1 border"
                    onClick={() => handleQuantityChange(item, "increase")}
                  >
                    +
                  </button>
                </div>
                <div className="w-1/12 text-center">
                  {item.price.toLocaleString()} VND
                </div>
                <div className="w-2/12 text-center">
                  {" "}
                  {(item.price * item.quantity).toLocaleString()} VND
                </div>
                <div className="w-1/12 text-center">
                  <button
                    className="text-red-500"
                    onClick={() => handleRemoveFromCart(item.id)}
                  >
                    <FontAwesomeIcon icon={faTrash} />
                  </button>
                </div>
              </div>
            ))}
          </div>
          <div className="flex justify-between items-center mt-4">
            <Link
              to="/product"
              className="text-blue-500 flex items-center font-poppins"
            >
              <FontAwesomeIcon className="pr-2" icon={faArrowLeft} />{" "}
              {t("cart.continueshopping")}
            </Link>
            <div className="text-right">
              <p className="text-lg font-semibold">
                Total ({selectedItems.length} items):{" "}
                {totalPrice.toLocaleString()} VND
              </p>
              <button
                className="bg-orange-500 text-white px-4 py-2 mt-2"
                onClick={handleCheckout}
              >
                Checkout
              </button>
            </div>
          </div>
        </div>
      )}
    </div>
  );
};

export default Cart;
