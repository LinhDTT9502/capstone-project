import React, { useEffect, useState } from "react";
import { Link, useNavigate } from "react-router-dom";
import { FontAwesomeIcon } from "@fortawesome/react-fontawesome";
import { faTrash, faArrowLeft } from "@fortawesome/free-solid-svg-icons";
import {
    getUserCart,
    reduceCartItem,
    removeCartItem,
    addToCart,
    updateCartItemQuantity,
} from "../../services/cartService";
import { ToastContainer, toast } from "react-toastify";
import "react-toastify/dist/ReactToastify.css";
import { useTranslation } from "react-i18next";
// import { ProductType } from "../Product/ProductType";

const UserCart = () => {
    const { t } = useTranslation();
    const [cartData, setCartData] = useState([]);
    const [selectedItems, setSelectedItems] = useState([]);
    const navigate = useNavigate();
    const token = localStorage.getItem("token");

    const getCart = async () => {
        if (token) {
            const customerCartData = await getUserCart(token);
            setCartData(customerCartData);
            console.log(customerCartData);
        }
    };

    useEffect(() => {
        getCart(); // Fetch the cart initially
    }, [token]);

    const handleRemoveFromCart = async (itemId) => {
        const response = await removeCartItem(itemId, token)
        console.log(response);
        getCart();
    };

    const handleReduceQuantity = async (id) => {

    };

    const handleIncreaseQuantity = async (item) => {

        getCart();

    };

    const handleQuantityChange = async (item, quantity) => {
        const response = await updateCartItemQuantity(item.id, quantity, token)
        console.log(response);
        getCart();

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
            setSelectedItems(cartData.map((item) => item.cartItemId));
        }
    };

    const totalItems = cartData.reduce((acc, item) => acc + item.quantity, 0);
    const totalPrice = selectedItems.reduce((acc, cartItemId) => {
        const item = cartData.find((item) => item.cartItemId === cartItemId);
        return acc + item.price;
    }, 0);
    // console.log(cartData);

    const handleCheckout = () => {
        if (selectedItems.length === 0) {
            toast.error("Please select at least one item to checkout.");
            return;
        }

        const selectedProducts = cartData.filter((item) =>
            selectedItems.includes(item.cartItemId)
        );
        navigate("/placed-order", { state: { selectedProducts } });
    };

    const handleRental = () => {

        if (selectedItems.length === 0) {
            toast.error("Please select at least one item to checkout.");
            return;
        }

        const selectedProducts = cartData.filter((item) =>
            selectedItems.includes(item.cartItemId)
        );

        const nonRentableItems = selectedProducts.filter((item) => item.rentPrice === 0);

        if (nonRentableItems.length > 0) {
            const nonRentableItemNames = nonRentableItems.map((item) => item.productName).join(", ");
            alert(`Sản phẩm không thể thuê: ${nonRentableItemNames}`);
            return;
        }
        navigate("/rental-placed-order", { state: { selectedProducts } });
    }

    return (
        <div className="container mx-auto px-20 py-10">
            <ToastContainer />
            <div className="flex justify-between items-center">
                <h1 className="font-alfa text-orange-500 text-2xl">
                    {t("user_cart.shopping_cart")}
                </h1>
                <span className="font-alfa text-orange-500 text-xl">
                    {totalItems} {t("user_cart.items")}
                </span>
            </div>
            <div className="items-center mb-2 justify-end flex font-medium text-rose-700">* Đơn vị tiền tệ: VND</div>
            {cartData.length === 0 ? (
                <p>{t("user_cart.empty")}</p>
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
                            <div className="w-5/12 text-center text-lg font-bold">
                                {t("user_cart.products")}
                            </div>
                            <div className="w-2/12 text-center text-lg font-bold">
                                {t("user_cart.quantity")}
                            </div>
                            <div className="w-2/12 text-center text-lg font-bold">
                                {t("user_cart.price")}
                            </div>
                            <div className="w-2/12 text-center text-lg font-bold">
                                {t("user_cart.total")}
                            </div>
                            <div className="w-2/12 text-center text-lg font-bold">
                                Giá thuê 
                                <p className="text-xs">(cho 1 ngày)</p>
                            </div>
                            <div className="w-1/12 text-center text-lg font-bold">

                            </div>
                        </div>
                        {cartData.map((item) => (
                            <div
                                key={item.cartItemId}
                                className="flex items-center justify-between p-4 border-b hover:bg-zinc-200"
                            >
                                <div className="w-1/12 text-center">
                                    <input
                                        type="checkbox"
                                        checked={selectedItems.includes(item.cartItemId)}
                                        onChange={() => handleSelectItem(item.cartItemId)}
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
                                        className="text-sm font-poppins font-bold text-wrap w-1/2 pr-4"
                                    >
                                        {item.productName}
                                    </Link>
                                    <div>
                                        {/* <ProductType
                                            productCode={item.productCode}
                                            color={item.color}
                                            size={item.size}
                                            condition={item.condition} /> */}
                                        <p>{item.color}, {item.size}, {item.condition}%</p>
                                    </div>

                                </div>
                                <div className="w-2/12 text-center flex items-center justify-center">
                                    <button
                                        className="px-2 py-1"
                                        onClick={() => handleReduceQuantity(item.cartItemId)}
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
                                    {(item.price.toLocaleString())}{" "}
                                </div>
                                <div className="w-2/12 text-center">
                                    {(item.price * item.quantity).toLocaleString()}
                                </div>
                                <div className="w-2/12 text-center">
                                    {item.rentPrice !== 0
                                        ? item.rentPrice.toLocaleString()
                                        : "Sản phẩm chỉ bán"}
                                </div>
                                <div className="w-1/12 text-center">
                                    <button
                                        className="text-red-500"
                                        onClick={() => handleRemoveFromCart(item.cartItemId)}
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
                                {t("user_cart.total")} ({selectedItems.length} {t("user_cart.items")}):
                            </p>
                        </div>
                        <p className="font-bold"> {totalPrice.toLocaleString()}{" "}₫ </p>
                    </div>
                    <div className="flex justify-between mt-5">
                        <Link
                            to="/product"
                            className="text-blue-500 flex items-center font-poppins"
                        >
                            <FontAwesomeIcon className="pr-2" icon={faArrowLeft} />{" "}
                            {t("user_cart.continue_shopping")}
                        </Link>
                        <div className="flex space-x-5">
                            <button
                                className="bg-orange-500 rounded-md text-white px-4 py-2"
                                onClick={handleCheckout}
                            >
                                Mua ngay
                            </button>
                            <button
                                className="bg-rose-700 rounded-md text-white px-4 py-2"
                                onClick={handleRental}
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

export default UserCart;
