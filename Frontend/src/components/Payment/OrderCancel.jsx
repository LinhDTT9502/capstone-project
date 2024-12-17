import React from "react";
import { FontAwesomeIcon } from "@fortawesome/react-fontawesome";
import { faXmark, faArrowLeft } from "@fortawesome/free-solid-svg-icons";
import { Link } from "react-router-dom";
import { useTranslation } from "react-i18next";

const OrderCancel = () => {
  const { t } = useTranslation();

  return (
    <div className="min-h-screen bg-gray-100 flex items-center justify-center px-4 sm:px-6 lg:px-8">
      <div className="max-w-md w-full space-y-8 bg-white p-10 rounded-xl shadow-lg text-center">
        <div className="inline-flex items-center justify-center w-20 h-20 rounded-full bg-red-100 mb-6">
          <FontAwesomeIcon icon={faXmark} className="text-red-600 text-4xl" />
        </div>
        <h1 className="text-3xl font-extrabold text-gray-900 mb-2">
          {t("order_cancel.canceled")}
        </h1>
        <p className="text-sm text-gray-600 mb-8">
          {t("order_cancel.your_order_is_canceled")}
        </p>
        <Link
          to="/product"
          className="inline-flex items-center justify-center px-5 py-3 border border-transparent text-base font-medium rounded-md text-blue-600 bg-blue-100 hover:bg-blue-200 transition duration-150 ease-in-out"
        >
          <FontAwesomeIcon icon={faArrowLeft} className="mr-2" />
          {t("order_cancel.continue_shopping")}
        </Link>
      </div>
    </div>
  );
};

export default OrderCancel;

