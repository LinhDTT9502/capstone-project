
import axios from 'axios';
import { useDispatch, useSelector } from 'react-redux';
import { login, selectUser } from '../../redux/slices/authSlice';
import { useTranslation } from "react-i18next";

import { jwtDecode } from "jwt-decode";

export default function LoginGoogle({ setIsSignInOpen }) {
  const dispatch = useDispatch();
  const { t } = useTranslation();
  const handleSubmitButton = () => {
    const newTab = window.open('https://twosport-api-offcial-685025377967.asia-southeast1.run.app/api/Auth/oauth-login', '_blank');

    window.addEventListener('message', (event) => {
      if (event.origin === 'http://demo-api.ap-southeast-2.elasticbeanstalk.com') {

        const { token, refreshToken } = event.data;
        console.log(token, refreshToken);
        if (token && refreshToken) {
          localStorage.setItem('token', token);
          localStorage.setItem('refreshToken', refreshToken);
          const decoded = jwtDecode(token);
          dispatch(login(decoded));
          // console.log('Login successful', decoded);
          setIsSignInOpen(false);
          newTab.close();
        }
      }
    }, { once: true });
  };
  return (
    <>
      <button
        onClick={handleSubmitButton}
        className="border-zinc-400 border-2 rounded-lg"
      > {t("login_google.btn")}</button>

    </>
  );
}
