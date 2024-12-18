import React from 'react';
import { Link } from 'react-router-dom';
import { useTranslation } from "react-i18next";
import { motion } from 'framer-motion';
import { FontAwesomeIcon } from '@fortawesome/react-fontawesome';
import { faFaceFrown, faHome } from '@fortawesome/free-solid-svg-icons';

const NotFoundPage = () => {
  const { t } = useTranslation();

  // Animation cho icon lăn từ trái sang phải
  const rollingBallVariants = {
    roll: {
      x: ["-100vw", "100vw"], // Di chuyển từ trái sang phải
      rotate: [0, 360],       // Xoay tròn 360 độ
      transition: {
        x: {
          repeat: Infinity,   // Lặp vô hạn
          duration: 5,        // Thời gian lăn từ trái qua phải
          ease: "linear",     // Chuyển động đều
        },
        rotate: {
          repeat: Infinity,
          duration: 5,
          ease: "linear",
        },
      },
    },
  };

  return (
    <div className="bg-gray-100 min-h-screen flex items-center justify-center p-4 overflow-hidden">
      <div className="max-w-4xl w-full text-center">
        {/* Animation Icon */}
        <motion.div
          className="absolute top-1/2 left-0 transform -translate-y-1/2 text-blue-600 text-5xl"
          variants={rollingBallVariants}
          animate="roll"
        >
          <FontAwesomeIcon icon={faFaceFrown} />
        </motion.div>

        {/* Main Content */}
        <motion.div 
          initial={{ opacity: 0, y: -50 }}
          animate={{ opacity: 1, y: 0 }}
          transition={{ duration: 0.5 }}
        >
          <h1 className="text-9xl font-bold text-gray-800 mb-2">404</h1>
        </motion.div>
        
        <motion.h2 
          className="text-3xl font-semibold text-gray-700 mb-4 mt-8"
          initial={{ opacity: 0 }}
          animate={{ opacity: 1 }}
          transition={{ delay: 0.2, duration: 0.5 }}
        >
          {t("not_found.page_not_found")}
        </motion.h2>
        
        <motion.p 
          className="text-xl text-gray-600 mb-8"
          initial={{ opacity: 0 }}
          animate={{ opacity: 1 }}
          transition={{ delay: 0.4, duration: 0.5 }}
        >
          {t("not_found.oops")}
        </motion.p>
        
        <motion.div
          initial={{ opacity: 0, y: 20 }}
          animate={{ opacity: 1, y: 0 }}
          transition={{ delay: 0.6, duration: 0.5 }}
        >
          <Link 
            to="/" 
            className="inline-flex items-center justify-center px-8 py-3 border border-transparent text-base font-medium rounded-full text-white bg-orange-600 hover:bg-orange-400 focus:outline-none focus:ring-2 focus:ring-offset-2 focus:ring-blue-500 transition duration-300 ease-in-out transform hover:scale-105"
          >
            <FontAwesomeIcon icon={faHome} className="mr-2" />
            {t("not_found.go_home")}
          </Link>
        </motion.div>
      </div>
    </div>
  );
};

export default NotFoundPage;
