import React, { useEffect, useState } from "react";
import { FontAwesomeIcon } from "@fortawesome/react-fontawesome";
import { faArrowLeft, faArrowRight } from "@fortawesome/free-solid-svg-icons";
import { fetchProducts } from "../../services/productService";
import { Link } from "react-router-dom";
import { useTranslation } from "react-i18next";
import axios from "axios";

export default function FeatureProductSlide() {
  const { t } = useTranslation();
  const [currentIndex, setCurrentIndex] = useState(0);
  const sortBy = "likes";
  const isAscending = true;
  const [images, setImages] = useState([]);
  const [flashSaleProducts, setFlashSaleProducts] = useState([]);

  useEffect(() => {
    const getFeature = async () => {
      try {
        const productFeatured = await axios.get(
          "https://capstone-project-703387227873.asia-southeast1.run.app/api/Product/list-products",
          { sortBy, isAscending },
          {
            headers: {
              accept: "*/*",
            },
          }
        );
        // console.log('Fetched Products:', productFeatured.data.data.$values);
        const products = productFeatured.data.data.$values;
        const discountedProducts = products.filter(
          (product) => product.discount > 0
        );

        setFlashSaleProducts(discountedProducts);
      } catch (error) {
        console.error("Error fetching feature products:", error);
      }
    };

    getFeature();
  }, []);

  // Use another useEffect to monitor changes in images

  const handlePrev = () => {
    setCurrentIndex(
      (prevIndex) =>
        (prevIndex - 1 + flashSaleProducts.length) % flashSaleProducts.length
    );
  };

  const handleNext = () => {
    setCurrentIndex((prevIndex) => (prevIndex + 1) % flashSaleProducts.length);
  };

  const calculateDiscountPercentage = (product) => {
    if (product.discount > 0) {
      return Math.round((product.discount / product.price) * 100);
    }
    return 0;
  };

  return (
    <div className="container mx-auto py-8">
      <div className="flex justify-between">
        <h2 className="font-alfa text-2xl mb-10">Khuyến Mãi</h2>
        <Link
          to="/product"
          className="flex items-center text-orange-500 hover:text-orange-600 transition-colors duration-200"
        >
          <button className="font-poppins font-semibold mr-2">
            {t("fearureproduct.viewall")}
          </button>
          <FontAwesomeIcon
            className="pl-2 text-orange-500"
            icon={faArrowRight}
          />
        </Link>
      </div>
      <div className="relative">
        <div className="overflow-hidden ">
          <div
            className="flex transition-transform duration-500 ease-in-out"
            style={{ transform: `translateX(-${currentIndex * (100 / 4)}%)` }}
          >
            {flashSaleProducts.map((product, index) => {
              const discountPercentage = calculateDiscountPercentage(product);

              return (
                <div
                  key={index}
                  className="min-w-64 px-2 flex flex-col relative shadow-md bg-white rounded-lg group"
                >
                  <Link
                    className="flex flex-col"
                    to={`/product/${product.productCode}`}
                  >
                    <div className="bg-white rounded-lg shadow-md overflow-hidden transition-transform duration-300 transform group-hover:scale-105">
                      <img
                        src={product.imgAvatarPath}
                        alt={`image ${index + 1}`}
                        className="object-contain w-full h-48"
                      />
                      <div className="absolute top-2 right-2 bg-orange-500 text-white text-xs font-bold py-1 px-2 rounded">
                        -{product.discount}%
                      </div>
                      <div></div>

                      <div className="p-4">
                        <p className="font-poppins text-orange-500 text-sm mb-1">
                          {product.brandName}
                        </p>
                        <h3 className="font-poppins font-bold text-lg mb-2 line-clamp-2">
                          {product.productName}
                        </h3>
                        <p className="font-poppins text-red-700">
                          {product.price.toLocaleString("vi-VN")}₫
                        </p>
                      </div>
                    </div>{" "}
                  </Link>
                </div>
              );
            })}
          </div>
        </div>
        <button
          onClick={handlePrev}
          className={`absolute w-1/12 left-0 top-1/3 transform -translate-y-1/3 ${currentIndex === 0 ? "opacity-50 cursor-not-allowed" : ""
            }`}
          disabled={currentIndex === 0}
        >
          <FontAwesomeIcon
            className="text-orange-500 p-2 -translate-y-1/2 -left-3 absolute rounded-full bg-white border-orange-500 border"
            icon={faArrowLeft}
          />
        </button>
        <button
          onClick={handleNext}
          className={`absolute w-1/12 right-0 top-1/3 transform -translate-y-1/3 ${currentIndex === images.length - 5
            ? "opacity-50 cursor-not-allowed"
            : ""
            }`}
          disabled={currentIndex === images.length - 5}
        >
          <FontAwesomeIcon
            className="text-orange-500 p-2 -translate-y-1/2 -right-3 absolute rounded-full bg-white border-orange-500 border"
            icon={faArrowRight}
          />
        </button>
      </div>
      <div className="px-20 mt-10">
        <hr className="py-5" />
      </div>
    </div>
  );
}
