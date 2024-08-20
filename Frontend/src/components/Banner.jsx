import React from "react";
import { motion } from "framer-motion";
import { useTranslation } from "react-i18next";
import { Link } from "react-router-dom";

function Banner() {
  const { t, i18n } = useTranslation("translation");
  const language = i18n.language;
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
              <p
                className={
                  language === "eng"
                    ? "font-alfa text-7xl text-wrap"
                    : "font-alfa text-7xl text-wrap"
                }
              >
                {t("banner.title")}
              </p>
              <p className="pt-10 pb-10 font-poppins text-xl text-wrap w-3/4">
                {t("banner.subtitle")}
              </p>
              <Link to="/product">
                <button className="bg-orange-500 font-poppins font-semibold text-white text-xl py-3 px-10 w-fit">
                  {t("banner.btn")}
                </button>
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
              className="flex justify-end w-2/5 pb-5 pr-20"
            >
              <img src="/assets/images/image.png" className="" />
            </motion.div>
          </div>
        </div>
      </div>
    </>
  );
}

export default Banner;
