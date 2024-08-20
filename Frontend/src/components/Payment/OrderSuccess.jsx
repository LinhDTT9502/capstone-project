// OrderSuccess.js
import React from "react";
import { FontAwesomeIcon } from "@fortawesome/react-fontawesome";
import { faCircleCheck, faArrowLeft } from "@fortawesome/free-solid-svg-icons";
import { Link, useNavigate } from "react-router-dom";
import { useTranslation } from "react-i18next";


const OrderSuccess = ({ orderCode }) => {
  const { t } = useTranslation();
  const navigate = useNavigate();

  return (
    <div className="px-20 py-5 flex flex-row bg-slate-200">
      <div className="whitespace-nowrap w-full py-10 bg-white mx-2 pr-14 flex items-center justify-center">
        <div className="text-center">
          <FontAwesomeIcon
            icon={faCircleCheck}
            className="text-green-500 text-8xl"
          />
          <div className="font-bold text-2xl pt-5">{t("order_success.order_success")}</div>
          <div>
            {t("order_success.order_code")} <span className="font-bold text-xl">{orderCode}</span>
          </div>
          <div className="text-lg">{t("order_success.thank_you_for_your_purchase")}</div>
          <div
            className="text-blue-500 font-bold pt-5 text-lg cursor-pointer"
           
          >
          
            <Link to="/product" className="text-blue-500 flex items-center font-poppins">
              <FontAwesomeIcon className="pr-2" icon={faArrowLeft} /> {t("order_success.continue_shopping")}
            </Link>
          </div>
        </div>
      </div>
    </div>
  );
};

export default OrderSuccess;
