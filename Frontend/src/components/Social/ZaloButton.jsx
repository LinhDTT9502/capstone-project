import React, { } from 'react';
import { Link } from 'react-router-dom'
import { FontAwesomeIcon } from "@fortawesome/react-fontawesome";
import { faMapLocation } from "@fortawesome/free-solid-svg-icons";
const ZaloButton = () => {

  return (


    <Link to='https://zalo.me/0385160050'>
    <img src="/assets/images/Zalo.png" alt="Zalo" className="w-16 h-16 object-contain" />
    </Link>

  );
};

export default ZaloButton;