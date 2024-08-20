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

  useEffect(() => {
    const getFeature = async () => {
      try {
        const productFeatured = await axios.get('https://twosportapiv2.azurewebsites.net/api/Product/list-products',{sortBy, isAscending}, {
          headers: {
            'accept': '*/*'
          }
        });
        // console.log('Fetched Products:', productFeatured.data.data.$values);
        const products = productFeatured.data.data.$values
        if (products && Array.isArray(products)) {
          setImages(products);

        } else {
          console.error("Fetched data is not an array");
        }
      } catch (error) {
        console.error("Error fetching feature products:", error);
      }
    };

    getFeature();
  }, []);

  // Use another useEffect to monitor changes in images
  useEffect(() => {
    // console.log('Updated images state:', images);
  }, [images]);

  const handlePrev = () => {
    setCurrentIndex(
      (prevIndex) => (prevIndex - 1 + images.length) % images.length
    );
  };

  const handleNext = () => {
    setCurrentIndex((prevIndex) => (prevIndex + 1) % images.length);
  };

  return (
    <>
      <div className="flex justify-between  px-20">
        <label className="font-alfa">{t("fearureproduct.featured")}</label>
        <Link to="/product">
          <button className="font-poppins font-semibold">
            {t("fearureproduct.viewall")}
          </button>
          <FontAwesomeIcon className="pl-2" icon={faArrowRight} />
        </Link>
      </div>
      <div className="relative px-20">
        <div className="overflow-hidden">
          <div
            className="flex transition-transform duration-500 ease-in-out "
            style={{ transform: `translateX(-${currentIndex * (100 / 8)}%)` }}
          >
            {images.map((product, index) => (
              <div
                key={index}
                className="min-w-64 px-2 flex flex-col hover:brightness-90"
              >
                <Link className=" flex flex-col " to={`/product/${product.id}`}>
                  <div className="bg-white">
                    <img
                      src={product.mainImagePath}
                      alt={`image ${index + 1}`}
                      className="object-scale-down h-48 w-96 shadow-lg "
                    />
                  </div>
                  <label className="text-wrap font-poppins text-orange-500 text-clip">
                    {product.brandName}
                  </label>
                  <label className="text-wrap font-poppins font-bold text-clip">
                    {product.productName}
                  </label>

                  <label className="text-wrap font-poppins text-zinc-500 text-clip">
                    {product.price} VND
                  </label>
                </Link>
                {/* <button
                className="absolute bottom-0 left-0 right-0 flex items-center justify-center bg-orange-600 bg-opacity-75 text-white opacity-0 hover:opacity-100 transition-opacity duration-300 py-4"
                onClick={() => handleAddToCart(product)}
              >
                ADD TO CART
              </button> */}
                {/* <div className="text-center mt-2">Category {index + 1}</div> */}
              </div>
            ))}
          </div>
        </div>
        <button
          onClick={handlePrev}
          className="absolute w-1/12 left-20 top-1/3 transform -translate-y-1/3 "
        >
          <FontAwesomeIcon
            className="text-orange-500 p-2 -translate-y-1/2 -left-3 absolute rounded-full bg-white border-orange-500 border"
            icon={faArrowLeft}
          />
        </button>
        <button
          onClick={handleNext}
          className="absolute w-1/12 right-20 top-1/3 transform -translate-y-1/3 "
        >
          <FontAwesomeIcon
            className="text-orange-500 p-2 -translate-y-1/2 -right-3 absolute rounded-full bg-white border-orange-500 border"
            icon={faArrowRight}
          />
        </button>
      </div>
    </>
  );
}
