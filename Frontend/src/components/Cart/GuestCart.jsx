import React, { useEffect, useState } from "react";
import { Link, useNavigate } from "react-router-dom";
import { FontAwesomeIcon } from "@fortawesome/react-fontawesome";
import { faTrash, faArrowLeft } from "@fortawesome/free-solid-svg-icons";
import { useSelector, useDispatch } from "react-redux"; // Added for Redux integration
import { ToastContainer, toast } from "react-toastify";
import "react-toastify/dist/ReactToastify.css";
import { useTranslation } from "react-i18next";
import {
  selectCartItems,
  removeFromCart,
  decreaseQuantity,
  addCart,
} from "../../redux/slices/cartSlice";
import {
  addCusCart,
  decreaseCusQuantity,
  removeFromCusCart,
  selectCustomerCartItems,
} from "../../redux/slices/customerCartSlice";
import { checkQuantityProduct } from "../../services/warehouseService";
// import { ProductType } from "../Product/ProductType";

const GuestCart = () => {
  const { t } = useTranslation();
  const [cartData, setCartData] = useState([]);
  const [selectedItems, setSelectedItems] = useState([]);
  const navigate = useNavigate();
  const token = localStorage.getItem("token");
  const dispatch = useDispatch();
  const guestCartItems = useSelector(selectCartItems);

  useEffect(() => {
    const getCart = async () => {
      if (!token) {
        setCartData(guestCartItems);
        console.log(cartData);
      }
    };

    getCart();
  }, [token, guestCartItems]);

  const handleRemoveFromCart = async (itemId) => {
    const confirmed = window.confirm(
      "Bạn có chắc chắn muốn xóa sản phẩm này khỏi giỏ hàng không?"
    );
    if (!confirmed) return;
  
    try {
      if (token) {
        dispatch(removeFromCusCart(itemId));
      } else {
        dispatch(removeFromCart(itemId));
      }
  
      setCartData((prevData) => prevData.filter((item) => item.id !== itemId));
  
      setSelectedItems((prevSelected) =>
        prevSelected.filter((id) => id !== itemId)
      );
  
      toast.success("Xóa sản phẩm khỏi giỏ hàng thành công!");
    } catch (error) {
      console.error("Lỗi khi xóa sản phẩm:", error);
  
      toast.error("Không thể xóa sản phẩm. Vui lòng thử lại sau.");
    }
  };
  

  const handleReduceQuantity = async (id) => {
    if (token) {
      dispatch(decreaseCusQuantity(id));
    } else {
      dispatch(decreaseQuantity(id));
    }
  };

  const handleIncreaseQuantity = (item) => {
    const quantityToAdd = 1; 
    dispatch(
      addCart({
        ...item,
        quantity: quantityToAdd,
      })
    );
  };
  

  const handleSelectItem = (productId) => {
    setSelectedItems((prevSelected) =>
      prevSelected.includes(productId)
        ? prevSelected.filter((id) => id !== productId)
        : [...prevSelected, productId]
    );
  };

  const handleSelectAll = () => {
    if (selectedItems.length === cartData.length) {
      setSelectedItems([]);
    } else {
      setSelectedItems(cartData.map((item) => item.id));
    }
  };

  const totalItems = cartData.reduce((acc, item) => acc + item.quantity, 0);

  const totalPrice = selectedItems.reduce((acc, id) => {
    const item = cartData.find((item) => item.id === id);
    return acc + item.price * item.quantity;
  }, 0);

  const handleSalePlaceOrder = async () => {
    if (selectedItems.length === 0) {
      toast.error("Bạn cần phải chọn ít nhất một sản phẩm");
      return;
    }

    const selectedProducts = cartData.filter((item) =>
      selectedItems.includes(item.id)
    );

    try {
      for (const product of selectedProducts) {
        const response = await checkQuantityProduct(product.id);

        if (product.quantity > response.availableQuantity) {
          toast.error(
            `Sản phẩm "${product.productName}" chỉ còn lại ${response.availableQuantity} sản phẩm trong kho.`
          );
          return;
        }
      }

      navigate("/placed-order", { state: { selectedProducts } });
    } catch (error) {
      console.error("Error checking product quantities:", error);
      toast.error("Có lỗi xảy ra khi kiểm tra số lượng sản phẩm.");
    }
  };

  const handleRentalPlaceOrder = async () => {
    if (selectedItems.length === 0) {
      toast.error("Bạn cần phải chọn ít nhất một sản phẩm");
      return;
    }

    const selectedProducts = cartData.filter((item) =>
      selectedItems.includes(item.id)
    );

    try {
      for (const product of selectedProducts) {
        const response = await checkQuantityProduct(product.id);

        if (product.quantity > response.availableQuantity) {
          toast.error(
            `Sản phẩm "${product.productName}" chỉ còn lại ${response.availableQuantity} sản phẩm trong kho.`
          );
          return;
        }
      }

      navigate("/rental-placed-order", { state: { selectedProducts } });
    } catch (error) {
      console.error("Error checking product quantities:", error);
      toast.error("Có lỗi xảy ra khi kiểm tra số lượng sản phẩm.");
    }
  };

    const isRentalDisabled = selectedItems.some((cartItemId) => {
      const item = cartData.find((item) => item.id === cartItemId);
      return item && item.rentPrice === 0;
    });

  return (
    <div className="container mx-auto px-20 py-10">
      <div className="flex justify-between items-center mb-4">
        <h1 className="font-alfa text-orange-500 text-2xl">
          {t("user_cart.shopping_cart")}
        </h1>
        <span className="font-alfa text-orange-500 text-xl">
          {totalItems} {t("user_cart.items")}
        </span>
      </div>
      <div className="items-center font-poppins mb-2 justify-end flex  text-rose-700">
        * Đơn vị tiền tệ: ₫
      </div>
      {cartData.length === 0 ? (
        <>
          <div className="flex flex-col items-center my-10 ">
            <img
              src="/assets/images/cart-icon.png"
              className="w-48 h-auto object-contain"
            />
            <p className="pt-4 text-lg font-poppins">{t("user_cart.empty")}</p>
            <Link
              to="/product"
              className="text-blue-500 flex items-center font-poppins"
            >
              <FontAwesomeIcon className="pr-2" icon={faArrowLeft} />{" "}
              {t("user_cart.continue_shopping")}
            </Link>
          </div>
        </>
      ) : (
        <div className="w-full">
          <div className="bg-zinc-100 rounded-lg overflow-hidden shadow-lg">
            <div className="flex items-center justify-between p-4 bg-zinc-300">
              <div className="w-1/12 text-center">
                <input
                  type="checkbox"
                  checked={selectedItems.length === cartData.length}
                  onChange={handleSelectAll}
                />
              </div>
              <div className="w-5/12 text-center font-poppins text-lg font-bold">
                {t("user_cart.products")}
              </div>
              <div className="w-2/12 text-center font-poppins text-lg font-bold">
                {t("user_cart.quantity")}
              </div>
              <div className="w-2/12 text-center font-poppins text-lg font-bold">
                {t("user_cart.price")}
              </div>
              <div className="w-2/12 text-center font-poppins text-lg font-bold">
                {t("user_cart.total")}
              </div>
              <div className="w-2/12 text-center text-lg font-bold">
                Giá thuê
                <p className="text-xs">(cho 1 ngày)</p>
              </div>
              <div className="w-1/12 text-center text-lg font-bold"></div>
            </div>
            {cartData.map((item) => (
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
                    src={item.imgAvatarPath}
                    alt={item.productName}
                    className="w-16 h-16 object-cover mr-4"
                  />
                  <Link
                    to={`/product/${item.productCode}`}
                    className="text-sm font-poppins font-bold text-wrap w-1/2"
                  >
                    {item.productName}
                  </Link>
                  <div>
                    {/* <ProductType
                      productCode={item.productCode}
                      color={item.color}
                      size={item.size}
                      condition={item.condition} /> */}
                    <p>
                      {item.color}, {item.size}, {item.condition}%
                    </p>
                  </div>
                </div>
                <div className="w-2/12 text-center flex items-center justify-center">
                  <button
                    className="px-2 py-1"
                    onClick={() => handleReduceQuantity(item.id)}
                  >
                    -
                  </button>
                  <input
                    type="number"
                    className="w-12 mx-2 text-center "
                    value={item.quantity}
                    onChange={(e) =>
                      handleQuantityChange(item, parseInt(e.target.value))
                    }
                    min="1"
                  />
                  <button
                    className="px-2 py-1 "
                    onClick={() => handleIncreaseQuantity(item)}
                  >
                    +
                  </button>
                </div>
                <div className="w-2/12 text-center">
                  {item.price.toLocaleString("vi-VN")} ₫
                </div>

                <div className="w-2/12 text-center">
                  {(item.price * item.quantity).toLocaleString("vi-VN")} ₫
                </div>
                <div className="w-2/12 text-center">
                  {item.rentPrice !== 0
                    ? item.rentPrice.toLocaleString("vi-VN") + " ₫"
                    : "Sản phẩm chỉ bán"}
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
          <div className="flex justify-between items-center mt-5">
            <div className="text-left">
              <p className="text-lg font-semibold">
                {t("user_cart.total")} ({selectedItems.length}{" "}
                {t("user_cart.items")}):
              </p>
            </div>
            <p className="font-bold"> {totalPrice.toLocaleString()} ₫ </p>
          </div>
          <div className="flex justify-between w-full mt-5">
            <Link
              to="/product"
              className="text-blue-500 flex items-center font-poppins"
            >
              <FontAwesomeIcon className="pr-2" icon={faArrowLeft} />{" "}
              {t("user_cart.continue_shopping")}
            </Link>
            <div className="space-x-5 items-center">
              <button
                className="bg-orange-500 rounded-md text-white px-4 py-2"
                onClick={handleSalePlaceOrder}
              >
                Mua ngay
              </button>
              <button
                className={`rounded-md px-4 py-2 ${
                  isRentalDisabled
                    ? "bg-gray-400 text-gray-600 cursor-not-allowed"
                    : "bg-rose-700 text-white"
                }`}
                onClick={handleRentalPlaceOrder}
                disabled={isRentalDisabled}
                title={
                  isRentalDisabled
                    ? "Có sản phẩm không thể thuê trong giỏ hàng."
                    : ""
                }
              >
                Thuê ngay
              </button>
            </div>
          </div>
        </div>
      )}
    </div>
  );
};

export default GuestCart;
