import React, { useEffect, useState } from "react";
import {
  motion,
  useAnimationFrame,
  useMotionValue,
  useTransform,
} from "framer-motion";

import { fetchBrands } from "../services/brandService";
import { useTranslation } from "react-i18next";
import { Link, useNavigate } from "react-router-dom";
import { FontAwesomeIcon } from "@fortawesome/react-fontawesome";
import { faArrowRight } from "@fortawesome/free-solid-svg-icons";

function ParallaxText({ brands, baseVelocity = 10 }) {
  const baseX = useMotionValue(0);
  const x = useTransform(baseX, (v) => `${v}%`);
  useAnimationFrame((t, delta) => {
    const moveBy = (baseVelocity * delta) / 1000;
    baseX.set(baseX.get() - moveBy);

    if (baseX.get() <= -100) {
      baseX.set(0);
    }
  });

  const repeatCount = Math.ceil(100 / brands.length);
  const repeatedBrands = Array.from({ length: repeatCount }, () => brands).flat();

  const navigate = useNavigate();
  const handleBrandClick = (brandId) => {

    navigate(`/product?brandID=${brandId}`);
  };

  return (
    <div
      className="parallax overflow-hidden"
      style={{
        position: "relative",
        width: "100%",
        display: "flex",
        alignItems: "center",
        height: "150px",
      }}
    >
      <motion.div
        className="scroller"
        style={{
          x,
          display: "flex",
          alignItems: "center",
          gap: "20px",
          willChange: "transform",
        }}
      >
        {repeatedBrands.map((brand) => (
          <img
            key={brand.id}
            src={brand.logo}
            onClick={() => handleBrandClick(brand.id)}
            style={{ cursor: "pointer" }}
            alt={brand.brandName}
            className="object-scale-down w-24 h-full"
          />
        ))}
      </motion.div>
    </div>
  );
}

export default function Brands() {
  const [brands, setBrands] = useState([]);
  const { t } = useTranslation();

  useEffect(() => {
    const getBrands = async () => {
      try {
        const brandsData = await fetchBrands();
        setBrands(brandsData);
      } catch (error) {
        console.error("Error fetching brand data:", error);
      }
    };

    getBrands();
  }, []);

  return (
    <div className="flex flex-col px-20">
      <div className="flex justify-between">
        <p className="font-alfa text-orange-500 text-3xl pt-10">
          {t("brand.name")}
        </p>
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
      {brands.length > 0 ? (
        <ParallaxText brands={brands} baseVelocity={0.4} />
      ) : (
        <p>Loading...</p>
      )}
    </div>
  );
}