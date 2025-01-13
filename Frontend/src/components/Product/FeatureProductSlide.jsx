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
  const itemsPerPage = 5;

  useEffect(() => {
    const getFeature = async () => {
      try {
        const productFeatured = await axios.get(
          "https://twosport-api-offcial-685025377967.asia-southeast1.run.app//api/Product/list-products",
          { sortBy, isAscending },
          {
            headers: {
              accept: "*/*",
            },
          }
        );
        const products = productFeatured.data.data.$values;
        if (products && Array.isArray(products)) {
          setImages(products.reverse());
        } else {
          console.error("Fetched data is not an array");
        }
      } catch (error) {
        console.error("Error fetching feature products:", error);
      }
    };

    getFeature();
  }, []);

  useEffect(() => {
    // console.log('Updated images state:', images);
  }, [images]);

  const handlePrev = () => {
    setCurrentIndex((prevIndex) => Math.max(prevIndex - itemsPerPage, 0));
  };

  const handleNext = () => {
    setCurrentIndex((prevIndex) =>
      Math.min(prevIndex + itemsPerPage, images.length - itemsPerPage)
    );
  };

  return ( 
    <div className="container mx-auto py-8 px-20">
      <div className="flex justify-between">
        <h2 className="font-alfa text-2xl mb-10">Sản Phẩm Mới</h2>
        <Link
          to="/product"
          className="flex items-center text-orange-500 hover:text-orange-600 transition-colors duration-200"
        >
          <span className="font-poppins font-semibold mr-2">
            {t("fearureproduct.viewall")}
          </span>
          <FontAwesomeIcon icon={faArrowRight} />
        </Link>
      </div>

      <div className="relative">
        <div className="overflow-hidden">
          <div
            className="flex transition-transform duration-500 ease-in-out"
            style={{
              transform: `translateX(-${(currentIndex / itemsPerPage) * 100}%)`,
            }}
          >
            {images.map((product, index) => (
              <div
                key={index}
                className="min-w-64 px-2 flex flex-col relative group"
                style={{ flex: `0 0 ${100 / itemsPerPage}%` }}
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
                    <div className="p-4">
                      <p className="font-poppins text-orange-500 text-sm mb-1 "
                      >
                        {product.brandName}
                      </p>
                      <h3 className="font-poppins font-bold text-lg mb-2 line-clamp-2 "
                        style={{ width: "200px", }}>
                        {product.productName}
                      </h3>
                      <p className="font-poppins text-red-700 ">
                        {product.price.toLocaleString("vi-VN")}₫
                      </p>
                    </div>
                  </div>
                </Link>
              </div>
            ))}
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
          className={`absolute w-1/12 right-0 top-1/3 transform -translate-y-1/3 ${currentIndex >= images.length - itemsPerPage
            ? "opacity-50 cursor-not-allowed"
            : ""
            }`}
          disabled={currentIndex >= images.length - itemsPerPage}
        >
          <FontAwesomeIcon
            className="text-orange-500 p-2 -translate-y-1/2 -right-3 absolute rounded-full bg-white border-orange-500 border"
            icon={faArrowRight}
          />
        </button>
      </div>
      <div className="mt-10 px-20">
        <hr className="border-t border-gray-200" />
      </div>
    </div>
  );
}