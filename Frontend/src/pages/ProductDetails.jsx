import { useEffect, useState } from "react";
import { useParams } from "react-router-dom";
import { fetchProductById } from "../services/productService";
import AddToCart from "../components/Product/AddToCart";
import { Rating } from "@material-tailwind/react";
import { useTranslation } from "react-i18next";

const ProductDetails = () => {
  const { productId } = useParams();
  const [product, setProduct] = useState(null);
  const [loading, setLoading] = useState(true);
  const [error, setError] = useState(null);
  const { t } = useTranslation();

  useEffect(() => {
    const getProduct = async () => {
      try {
        const productData = await fetchProductById(productId);
        setProduct(productData);
        setLoading(false);
      } catch (error) {
        setError(error);
        setLoading(false);
      }
    };

    getProduct();
  }, [productId]);

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
            <h4 className="text-lg text-orange-500">Shoes</h4>
            <h2 className="text-3xl font-bold text-black mt-2">
              {product.productName}
            </h2>
            <div className="flex items-center mt-4">
              <Rating unratedColor="amber" ratedColor="amber" className="pt-5" value={5} readonly />
              <span className="text-gray-600 ml-2">(15)</span>
            </div>
            <p className="text-gray-600 my-4">Describe product's details. Lorem Ipsum...</p>
            <div className="flex  items-center mt-6 space-x-20">
              <div>
                <h4 className="text-lg font-bold text-black">Size</h4>
                <select className="block appearance-none w-full bg-white border border-gray-400 hover:border-gray-500 px-4 py-2 pr-8 rounded shadow leading-tight focus:outline-none focus:shadow-outline">
                  <option>{product.size}</option>
                </select>
              </div>
              {/* <div>
                <h4 className="text-lg font-bold text-black">Stock</h4>
                <span className="text-black">15 Available</span>
              </div> */}
            </div>
            <div className="flex  items-center space-x-5 mt-4">
              <div className="flex items-center">
                <button className="text-xl p-2 bg-gray-200 rounded-md">-</button>
                <span className="text-xl mx-4">1</span>
                <button className="text-xl p-2 bg-gray-200 rounded-md">+</button>
              </div>
            </div>
            <div className="flex items-center mt-4 space-x-4">
              <button className="bg-black text-white px-6 py-2 text-lg font-semibold rounded-md">
                Add to Cart
              </button>
              <button className="bg-blue-500 text-white px-6 py-2 text-lg font-semibold rounded-md">
                Rent
              </button>
            </div>
          </div>
          <div className="md:w-1/2 flex justify-center items-center">
            <img
              src={product.imgAvatarPath}
              alt={product.imgAvatarName}
              className="w-1/2 h-auto object-cover rounded-lg"
            />
          </div>
        </div>
      )}
    </div>
  );
};

export default ProductDetails;
