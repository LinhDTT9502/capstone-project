import { useEffect, useState } from "react";
import { useParams } from "react-router-dom";
import { fetchProductById } from "../services/productService";
// import { Rating } from "@material-tailwind/react";
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
            <h4 className="text-lg text-orange-500">{product.brandName}</h4>
            <h2 className="text-3xl font-bold text-black mt-2">
              {product.productName}
            </h2>
            {product.reviews?.$values.map(review => (
                <div>
                  <Rating unratedColor="amber" ratedColor="amber" key={review.id} className="pt-5" value={review.star} readonly />
                </div>
              ))}
               <h4 className="text-lg font-bold text-black mb-2">{t("product_details.price")}</h4>
                <span className="text-2xl font-semibold text-orange-500 mt-10">
                  {product.price} {t("product_details.vnd")}
                </span>
                <AddToCart />
            {/* <div className="flex items-center mt-4">
              <Rating value={product.rating} readOnly />
              <span className="text-gray-600 ml-2">(15)</span>
            </div> */}
            <p className="text-gray-600 my-4">{product.description}</p>
            <div className="flex justify-center items-center mt-6 space-x-20">
              {/* <div className="space-y-2">
                <h4 className="text-lg font-bold text-black">Size</h4>
                <select className="block appearance-none w-full bg-white border border-gray-400 hover:border-gray-500 px-4 py-2 pr-8 rounded shadow leading-tight focus:outline-none focus:shadow-outline">
                  <option>{product.size}</option>
                </select>
              </div> */}
              {/* <div className="text-left">
                <h4 className="text-lg font-bold text-black mb-2">Price</h4>
                <span className="text-2xl font-semibold text-orange-500 mt-10">
                  {product.price} VND
                </span>
              </div> */}
            </div>
            
          </div>
          <div className="md:w-1/2 flex justify-center items-center">
            <img
              src={product.mainImagePath}
              alt={product.mainImageName}
              className="w-1/2 h-auto object-cover rounded-lg"
            />
          </div>
        </div>
      )}
    </div>
  );
};

export default ProductDetails;
