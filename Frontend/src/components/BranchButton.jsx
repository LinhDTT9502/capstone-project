import React, { } from 'react';
import { Link } from 'react-router-dom'
import { FontAwesomeIcon } from "@fortawesome/react-fontawesome";
import { faMapLocation } from "@fortawesome/free-solid-svg-icons";
const BranchSystem = () => {

  return (


    <Link to='/branch-system'>
      <FontAwesomeIcon icon={faMapLocation} />
      <button className='ml-1'>
        Hệ thống cửa hàng
      </button>
    </Link>

  );
};

export default BranchSystem;