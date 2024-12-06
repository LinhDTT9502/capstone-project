import React, { useEffect, useState } from "react";
import {
  motion,
  useAnimationFrame,
  useMotionValue,
  useTransform,
} from "framer-motion";

import { fetchBrands } from "../services/brandService";
import { useTranslation } from "react-i18next";

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

  const repeatedBrands = Array.from({ length: 100 }, () => brands).flat();

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
        }}
      >
        {repeatedBrands.map((brand, index) => (
          <img
            key={index}
            src={brand.logo}
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
      <p className="font-alfa text-orange-500 text-3xl pt-10">
        {t("brand.name")}
      </p>
      <ParallaxText brands={brands} baseVelocity={1} />
    </div>
  );
}