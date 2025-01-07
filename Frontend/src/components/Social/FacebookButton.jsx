import { faFacebook } from "@fortawesome/free-brands-svg-icons";
import { FontAwesomeIcon } from "@fortawesome/react-fontawesome";
import React from "react";
import { Link } from "react-router-dom";

const FacebookButton = () => {
  return (
    <div className="fixed bottom-28  right-0 z-50">
      <Link
        to="https://www.facebook.com/profile.php?id=61560697567321"
        target="_blank"
        rel="noopener noreferrer"
        className="group"
      >
        <div className="w-16 h-16 bg-blue-200 rounded-full flex items-center justify-center ">
          <FontAwesomeIcon
            icon={faFacebook}
            beat
            size="2xl"
            style={{ color: "#4f5f9c" }}
          />
        </div>
      </Link>
    </div>
  );
};

export default FacebookButton;
