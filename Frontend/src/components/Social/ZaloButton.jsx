import React from "react";
import { Link } from "react-router-dom";

const ZaloButton = () => {
  return (
    <div className="fixed bottom-16 right-0 z-50">
      <Link
        to="https://zalo.me/0385160050"
        target="_blank"
        rel="noopener noreferrer"
        className="group"
      >
        <div className="w-16 h-16 bg-blue-200 rounded-full flex items-center justify-center ">
          <img
            src="/assets/images/Zalo.png"
            alt="Zalo"
            className="w-10 h-10 object-contain"
          />
        </div>
      </Link>
    </div>
  );
};

export default ZaloButton;
