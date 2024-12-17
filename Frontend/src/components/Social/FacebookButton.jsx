import React, { } from 'react';
import { Link } from 'react-router-dom'
import { FontAwesomeIcon } from "@fortawesome/react-fontawesome";
import { faMapLocation } from "@fortawesome/free-solid-svg-icons";
const FacebookButton = () => {

  return (


    <Link to='https://www.facebook.com/profile.php?id=61560697567321'>
    <img src="/assets/images/facebook.png" alt="facebook" className="w-16 h-16 object-contain" />
    </Link>

  );
};

export default FacebookButton;