import React from "react";
import { motion } from "framer-motion";
import { useTranslation } from "react-i18next";
import { Link } from "react-router-dom";

function Banner() {
  const { t, i18n } = useTranslation("translation");
  const language = i18n.language;

  const floatingAnimation = {
    y: ["-15px", "15px"],
    transition: {
      duration: 3,
      repeat: Infinity,
      repeatType: "reverse",
      ease: "easeInOut",
    },
  };

  const textGlowAnimation = {
    opacity: [0.7, 1, 0.7],
    scale: [0.98, 1, 0.98],
    transition: {
      duration: 3,
      repeat: Infinity,
      repeatType: "reverse",
      ease: "easeInOut",
    },
  };

  return (
    <div className="bg-banner bg-cover bg-center h-[85vh] flex items-center relative overflow-hidden">
      {/* Background Overlay */}
      <div className="absolute inset-0 bg-gradient-to-r from-sky-100/70 via-sky-100/50 to-transparent z-0"></div>

      <div className="container mx-auto flex justify-between items-center relative z-10 px-8 md:px-16 lg:px-32">
        {/* Title Section */}
        <motion.div
          initial={{ x: "-100px", opacity: 0 }}
          animate={{ x: 0, opacity: 1 }}
          transition={{
            duration: 1.5,
            type: "ease-in-out",
          }}
          className="flex flex-col max-w-lg space-y-6 w-3/4"
        >
          <h1
            className={`${
              language === "eng"
                ? "font-alfa text-5xl md:text-6xl lg:text-7xl"
                : "font-alfa text-5xl md:text-6xl lg:text-7xl"
            } text-gray-800 leading-tight`}
          >
            {t("banner.title")}
          </h1>
          <p className="text-gray-600 font-poppins text-lg md:text-xl leading-relaxed ">
            {t("banner.subtitle")}
          </p>
          <Link to="/product">
            <motion.button
              className="rounded-full bg-orange-500 font-poppins font-semibold text-white text-lg md:text-xl py-3 px-8 shadow-lg hover:bg-orange-600 focus:outline-none"
              whileHover={{ scale: 1.05 }}
              whileTap={{ scale: 0.95 }}
            >
              {t("banner.btn")}
            </motion.button>
          </Link>
        </motion.div>

        {/* Image Section */}
        <motion.div
  initial={{ opacity: 0, scale: 0.8 }}
  animate={{ opacity: 1, scale: 1 }}
  transition={{
    duration: 1.5,
    ease: "easeInOut",
  }}
  className="relative flex justify-end items-center w-1/2"
>
  {/* Hiệu ứng tỏa sáng */}
  <div
    className="absolute inset-0 w-80 h-80 rounded-full bg-gradient-to-r from-yellow-400 via-white-300 to-transparent opacity-50 blur-2xl z-0"
    style={{
      top: "50%",
      left: "50%",
      transform: "translate(-50%, -50%)",
    }}
  ></div>

  {/* Hình giày */}
  <motion.img
    src="/assets/images/image.png"
    alt="2SPORT"
    className="w-3/4 lg:w-4/5 object-contain z-10"
    animate={{
      y: ["-15px", "15px"],
      transition: {
        duration: 3,
        repeat: Infinity,
        repeatType: "reverse",
        ease: "easeInOut",
      },
    }}
  />
  {/* Glow Text */}
  <motion.div
    className="absolute inset-0 flex items-center justify-center z-0"
    animate={{
      opacity: [0.7, 1, 0.7],
      scale: [0.98, 1, 0.98],
      transition: {
        duration: 3,
        repeat: Infinity,
        repeatType: "reverse",
        ease: "easeInOut",
      },
    }}
  >
    <p className="text-8xl lg:text-9xl font-bold text-gray-300/50 opacity-40 tracking-wider">
      2SPORT
    </p>
  </motion.div>
</motion.div>

      </div>
    </div>
  );
}

export default Banner;
