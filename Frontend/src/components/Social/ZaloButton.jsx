import React from "react";
import { Link } from "react-router-dom";

const ZaloButton = () => {
  return (
    <div className="fixed bottom-10 right-0 z-50">
      <Link
        to="https://zalo.me/0385160050"
        target="_blank"
        rel="noopener noreferrer"
        className="group"
      >
        <div className="w-16 h-16 bg-blue-500 rounded-l-full flex items-center justify-center shadow-lg 
          transform translate-x-8 group-hover:translate-x-0 transition-transform duration-300 ease-in-out overflow-hidden"
        >
          <img
            src="/assets/images/Zalo.png"
            alt="Zalo"
            className="w-10 h-10 object-contain opacity-0 group-hover:opacity-100 transition-opacity duration-300 ease-in-out"
          />
        </div>
      </Link>
    </div>
  );
};

export default ZaloButton;
