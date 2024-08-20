import React from "react";
import { useLocation, useNavigate } from "react-router-dom";
import { FontAwesomeIcon } from '@fortawesome/react-fontawesome';
import { faRightFromBracket } from '@fortawesome/free-solid-svg-icons';
import axios from 'axios';
import { useDispatch } from "react-redux";
import { logout } from "../../redux/slices/authSlice";
import { toast } from "react-toastify";
import "react-toastify/dist/ReactToastify.css";
import { signOutUser } from "../../services/authService";
import { useTranslation } from "react-i18next";

const Logout = () => {
    const navigate = useNavigate();
    const dispatch = useDispatch();
    const { t } = useTranslation();
    const location = useLocation();
    const handleLogout = async () => {
        // // Get all cookies
        // const cookiesArray = document.cookie.split(';');
        // let token = null;
        // let refreshToken = null;
        // // Iterate through the array to find the token and refreshToken
        // cookiesArray.forEach(cookie => {
        //     const [name, value] = cookie.trim().split('=');
        //     if (name === 'token') {
        //         token = value;
        //     } else if (name === 'refreshToken') {
        //         refreshToken = value;
        //     }
        // });

        // console.log('Token:', token);
        // console.log('RefreshToken:', refreshToken);

        // // Ensure tokens are not null
        const token = window.localStorage.getItem('token');
        const refreshToken = window.localStorage.getItem('refreshToken');
        if (!token || !refreshToken) {
            console.error('Token or RefreshToken is missing!');
            return;
        }

        const data = {
            token: token,
            refreshToken: refreshToken
        };

        // console.log('Request Payload:', JSON.stringify(data));

        try {
            // const response = await signOutUser(data);
            // console.log('Response:', response.data);
            toast.success("Bạn đã đăng xuất thành công");
            navigate('/');
            dispatch(logout());
            localStorage.clear();
        } catch (error) {
            console.error('There was an error making the request!', error);
            console.error('Response data:', error.response?.data);
        }
    };

    return (
        <button onClick={handleLogout} className="">
            <FontAwesomeIcon className="pr-1" icon={faRightFromBracket} />
                {t("logout.logout")}
        </button>
    );
};

export default Logout;
