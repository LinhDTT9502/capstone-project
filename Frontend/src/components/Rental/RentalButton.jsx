import React, { } from 'react';
import { Link } from 'react-router-dom';
import { Button } from "@material-tailwind/react";

const RentalButton = () => {

    return (


        <Link to='/branch-system'>
            <Button color="orange">
                Thuê ngay
            </Button>
        </Link>

    );
};

export default RentalButton;
