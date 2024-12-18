import React, { useEffect, useState } from "react";
import { useTranslation } from "react-i18next";
import { fetchCategories } from "../services/categoryService";
import { Link, useNavigate } from "react-router-dom"; // Import useNavigate
import { FontAwesomeIcon } from "@fortawesome/react-fontawesome";
import { faArrowRight } from "@fortawesome/free-solid-svg-icons";


export default function Categories() {
  const [categories, setCategories] = useState([]);
  const { t } = useTranslation();
  const navigate = useNavigate();

  useEffect(() => {
    const getCategories = async () => {
      try {
        const categoriesData = await fetchCategories();
        setCategories(categoriesData);
      } catch (error) {
        console.error("Error fetching category data:", error);
      }
    };

    getCategories();
  }, []);

  const handleCategoryClick = (categoryId) => {

    navigate(`/product?categoryID=${categoryId}`);
  };

  return (
    <div className="flex flex-col px-20">
      <div className="flex justify-between">
        <h2 className="font-alfa text-orange-500 text-3xl pt-10">DANH MỤC</h2>
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
      <div className="flex flex-wrap gap-8 pb-4 mt-10">
        {categories.map((category) => (
          <div
            key={category.id}
            className="flex flex-col items-center w-32 group transition-transform duration-300 ease-in-out transform hover:scale-110"
            onClick={() => handleCategoryClick(category.id)}
            style={{ cursor: "pointer" }}
          >
            <div className="w-24 h-24 bg-gray-100 rounded-full flex items-center justify-center overflow-hidden shadow-md transition-shadow duration-300 ease-in-out group-hover:shadow-lg">
              <img
                src={category.categoryImgPath}
                alt={category.categoryName}
                className="w-20 h-20 object-contain transition-transform duration-300 ease-in-out group-hover:scale-110"
              />
            </div>
            <p className="mt-3 text-sm text-center font-medium text-gray-700 group-hover:text-orange-500 transition-colors duration-300 ease-in-out">
              {category.categoryName}
            </p>
          </div>
        ))}
      </div>
    </div>
  );
}
