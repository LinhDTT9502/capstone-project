import React from "react";
import { motion } from "framer-motion";
import { useTranslation } from "react-i18next";
import { Link } from "react-router-dom";

function Banner() {
  const { t, i18n } = useTranslation("translation");
  const language = i18n.language;

  const floatingAnimation = {
    y: ["-10px", "10px"],
    transition: {
      y: {
        duration: 2,
        repeat: Infinity,
        repeatType: "reverse",
        ease: "easeInOut"
      }
    }
  };

  const textAnimation = {
    opacity: [0.5, 1, 0.5],
    scale: [0.98, 1, 0.98],
    transition: {
      duration: 3,
      repeat: Infinity,
      repeatType: "reverse",
      ease: "easeInOut"
    }
  };

  return (
    <>
      <div className="bg-banner bg-cover bg-center h-full">
        <div className="bg-sky-100 bg-opacity-65 h-full">
          <div className="flex py-32 justify-between items-center">
            {/* title */}
            <motion.div
              initial={{ x: "7rem", opacity: 0 }}
              animate={{ x: 0, opacity: 1 }}
              transition={{
                duration: 2,
                type: "ease-in",
              }}
              className="flex-col flex px-20 w-3/4"
            >
              <p className={ language === "eng" ? "font-alfa text-7xl text-wrap" : "font-alfa text-7xl text-wrap" }
              >
                {t("banner.title")}
              </p>
              <p className="pt-10 pb-10 font-poppins text-xl text-wrap w-3/4">
                {t("banner.subtitle")}
              </p>
              <Link to="/product">
                <motion.button 
                  className="rounded-full bg-orange-500 font-poppins font-semibold text-white text-xl py-3 px-10 w-fit"
                  whileHover={{ scale: 1.05 }}
                  whileTap={{ scale: 0.95 }}
                >
                  {t("banner.btn")}
                </motion.button>
              </Link>
            </motion.div>
            {/* photo */}
            <motion.div
              initial={{ opacity: 0, scale: 0.5 }}
              animate={{ opacity: 1, scale: 1 }}
              transition={{
                duration: 2,
                ease: [0, 0.71, 0.2, 1.01],
                scale: {
                  type: "spring",
                  damping: 5,
                  stiffness: 100,
                  restDelta: 0.001,
                },
              }}
              className="flex justify-end w-2/5 pb-5 pr-20 relative"
            >
              <motion.img 
                src="/assets/images/image.png" 
                className="z-10"
                animate={floatingAnimation}
              />
              <motion.div 
                className="absolute inset-0 flex items-center justify-center z-0"
                animate={textAnimation}
              >
                <p className="text-9xl font-bold text-gray-200 opacity-50">2SPORT</p>
              </motion.div>
            </motion.div>
          </div>
        </div>
      </div>
    </>
  );
}

export default Banner;

