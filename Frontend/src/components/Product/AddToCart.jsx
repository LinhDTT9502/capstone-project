import { useState } from "react";
import { FontAwesomeIcon } from "@fortawesome/react-fontawesome";
import { faHeart } from "@fortawesome/free-regular-svg-icons";
import { useTranslation } from "react-i18next";

const AddToCart = () => {
  const { t } = useTranslation();
  const [quantity, setQuantity] = useState(1);
  const increaseQuantity = () => setQuantity(quantity + 1);
  const decreaseQuantity = () => {
    if (quantity > 1) {
      setQuantity(quantity - 1);
    }
  };

  return (
    <div className="flex flex-col space-y-4 mt-6">
      <div className="flex items-center">
        <button
          className="px-2 py-1 text-gray-700 hover:text-black focus:outline-none"
          onClick={decreaseQuantity}
        >
          -
        </button>
        <span className="px-4 py-1 text-black">{quantity}</span>
        <button
          className="px-2 py-1 text-gray-700 hover:text-black focus:outline-none"
          onClick={increaseQuantity}
        >
          +
        </button>
      </div>
      <div className="flex  space-x-4">
        <button className="px-6 py-2 bg-orange-500 text-white rounded shadow hover:bg-orange-700 focus:outline-none">
          {t("product_list.add_to_cart")}
        </button>
        <button className="p-2 border border-gray-300 rounded hover:bg-gray-100 focus:outline-none">
          <FontAwesomeIcon icon={faHeart} />
        </button>
      </div>
    </div>
  );
};

export default AddToCart;
