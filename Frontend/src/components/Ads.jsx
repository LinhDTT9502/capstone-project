import React, { useEffect } from "react";
import { motion, useAnimation } from "framer-motion";
import { Link } from "react-router-dom";
import { useTranslation } from "react-i18next";
import { faArrowRight } from "@fortawesome/free-solid-svg-icons";
import { FontAwesomeIcon } from "@fortawesome/react-fontawesome";

function Ads() {
    const controls = useAnimation();
    const { t, i18n } = useTranslation("translation");

    useEffect(() => {
        const scrollHandler = () => {
            const scrollY = window.scrollY || window.pageYOffset;
            controls.start({
                opacity: scrollY > 100 ? 1 : 0,
                y: scrollY > 100 ? 0 : 50,
                transition: {
                    opacity: { duration: 0.5 },
                    y: { duration: 0.5 }
                }
            });
        };
        window.addEventListener("scroll", scrollHandler);
        return () => {
            window.removeEventListener("scroll", scrollHandler);
        };
    }, [controls]);

    return (
        <div className="flex py-5 space-x-5 justify-between items-center">
            {/* title */}
            <motion.div
                animate={controls}
                initial={{ y: "2rem", opacity: 0 }}
                className="flex-col flex relative"
            >
                <div className="relative">
                    <img src="/assets/images/ads/badmintion.png" alt="Badminton" className="w-full" />
                    <Link to="/product" className="absolute bottom-4 left-4">
                        <button className=" text-white font-poppins font-semibold pl-24 pb-32 rounded">
                            {t("banner.btn")}
                            <FontAwesomeIcon className="pl-5" icon={faArrowRight} />
                        </button>
                    </Link>
                </div>
            </motion.div>

            {/* photo */}
            <motion.div
                animate={controls}
                initial={{ y: "7rem", opacity: 0 }}
                className="flex-col flex pr-20 relative"
            >
               <div className="relative">
                    <img src="/assets/images/ads/image.png" alt="Badminton" className="w-full" />
                    <Link to="/product" className="absolute bottom-4 ">
                        <button className=" text-black font-poppins font-semibold pl-24 pb-36 rounded">
                            {t("ads.btn")}
                            <FontAwesomeIcon className="pl-5" icon={faArrowRight} />
                        </button>
                    </Link>
                </div>
            </motion.div>
        </div>
    );
}

export default Ads;
