import { useEffect, useState } from "react";
import { useParams } from "react-router-dom";
import { fetchProductById } from "../services/productService";
import { Input, Rating } from "@material-tailwind/react";
import { useTranslation } from "react-i18next";
import AddToCart from "../components/Product/AddToCart";

const ProductDetails = () => {
  const { productId } = useParams();
  const [product, setProduct] = useState(null);
  const [loading, setLoading] = useState(true);
  const [error, setError] = useState(null);
  const { t } = useTranslation();
  const [quantity, setQuantity] = useState(1);

  useEffect(() => {
    const getProduct = async () => {
      try {
        const productData = await fetchProductById(productId);
        if (productData.length > 0) {
          setProduct(productData[0]);
        }
        setLoading(false);
      } catch (error) {
        setError(error);
        setLoading(false);
      }
    };

    getProduct();
  }, [productId]);

  const handleIncrease = () => {
    setQuantity(prev => prev + 1);
  };

  // Function to handle quantity decrease
  const handleDecrease = () => {
    if (quantity > 1) {
      setQuantity(prev => prev - 1);
    }
  };

  // Function to handle manual input
  const handleInputChange = (e) => {
    const value = Math.max(1, Number(e.target.value)); // Prevent negative or zero values
    setQuantity(value);
  };

  if (loading) {
    return <div>{t("product_details.loading")}</div>;
  }

  if (error) {
    return <div>{t("product_details.error")}</div>;
  }

  return (
    <div className="container mx-auto px-20 py-6 bg-white rounded-lg shadow-lg">
      {product && (
        <div className="flex flex-col justify-center items-center md:flex-row gap-1">
          <div className="md:w-1/2">
            <h4 className="text-lg text-orange-500">{product.categoryName}</h4>
            <h2 className="text-3xl font-bold text-black mt-2">{product.productName}</h2>
            <p className="text-gray-600 my-4">{product.description || "No description available"}</p>

            <div className="flex items-center mt-4">
              <Rating unratedColor="amber" ratedColor="amber" className="pt-5" value={5} readonly />
              <span className="text-gray-600 ml-2">(15)</span>
            </div>

            <div className="my-4 text-gray-800">
              <p><strong>Brand:</strong> {product.brandName}</p>
              <p><strong>Color:</strong> {product.color}</p>
              <p><strong>Condition:</strong> {product.condition}%</p>
              <p><strong>Sport:</strong> {product.sportName}</p>
              <p><strong>Price:</strong> {product.price ? `${product.price} VND` : "N/A"}</p>
              <p><strong>Rent Price:</strong> {product.rentPrice ? `${product.rentPrice} VND` : "N/A"}</p>
              <button
                onClick={handleDecrease}
                className="px-3 py-1 border rounded-md"
              >
                -
              </button>
              <Input
                type="number"
                value={quantity}
                onChange={handleInputChange}
                className="w-16 text-center"
                min="1"
              />
              <button
                onClick={handleIncrease}
                className="px-3 py-1 border rounded-md"
              >
                +
              </button>

            </div>

            <div className="flex items-center mt-4 space-x-4">
              <AddToCart product={product} quantity={quantity} />
            </div>
          </div>
          <div className="md:w-1/2 flex justify-center items-center">
            <img src={product.imgAvatarPath} alt={product.imgAvatarName || "Product Image"} className="w-1/2 h-auto object-cover rounded-lg" />
          </div>
        </div>
      )}
    </div>
  );
};

export default ProductDetails;
