"use client";

import React, { useEffect, useState } from "react";
import { useTranslation } from "react-i18next";
import { fetchCategories } from "../services/categoryService";

export default function Categories() {
  const [categories, setCategories] = useState([]);
  const { t } = useTranslation();

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

  return (
    <div className="container mx-auto px-20 py-12" >
      <h2 className="font-alfa text-orange-500 text-3xl mb-8">
        DANH MỤC
      </h2>

      <div className="flex flex-wrap gap-8 pb-4">
        {categories.map((category, index) => (
          <div
            key={index}
            className="flex flex-col items-center w-32 group transition-transform duration-300 ease-in-out transform hover:scale-110"
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
