import React from 'react';
import { Link } from 'react-router-dom';
import { useTranslation } from "react-i18next";

const NotFoundPage = () => {
  const { t } = useTranslation();

  return (
    <div className="bg-banner bg-scroll justify-center items-center bg-cover bg-center min-h-screen flex">
      <div>
        <div className='relative'>
          <div className='bg-neutral-950 blur-2xl p-20 absolute inset-0'></div>
          <div className='relative text-white z-10'>
            <h1 className='flex items-center justify-center text-3xl p-2 font-pacifico'>
              {t("not_found.page_not_found")}
            </h1>
            <p className='flex items-center justify-center'>
              {t("not_found.oops")}
            </p>
            <Link to="/" className="flex items-center justify-center text-white">
              {t("not_found.go_home")}
            </Link>
          </div>
        </div>
      </div>
    </div>
  );
};

export default NotFoundPage;
