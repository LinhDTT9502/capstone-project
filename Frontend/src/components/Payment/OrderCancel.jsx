
import React from "react";
import { FontAwesomeIcon } from "@fortawesome/react-fontawesome";
import { faXmark, faArrowLeft } from "@fortawesome/free-solid-svg-icons";
import { Link, useNavigate } from "react-router-dom";
import { useTranslation } from "react-i18next";


const OrderCancel = () => {
  const { t } = useTranslation();
  const navigate = useNavigate();

  return (
    <div className="px-20 py-5 flex flex-row bg-slate-200">
      <div className="whitespace-nowrap w-full py-10 bg-white mx-2 pr-14 flex items-center justify-center">
        <div className="text-center">
        <FontAwesomeIcon icon={faXmark}
            className="bg-red-500 text-white rounded-full px-3 text-8xl"
          />
          <div className="font-bold text-2xl pt-5">{t("order_cancel.canceled")}</div>
          {/* <div>
            Order Code <span className="font-bold text-xl">{}</span>
          </div> */}
          <div className="text-lg">{t("order_cancel.your_order_is_canceled")}</div>
          <div
            className="text-blue-500 font-bold pt-5 text-lg cursor-pointer"
    
          >
          
            <Link to="/product" className="text-blue-500 flex items-center font-poppins">
              <FontAwesomeIcon className="pr-2" icon={faArrowLeft} /> {t("order_cancel.continue_shopping")}
            </Link>
          </div>
        </div>
      </div>
    </div>
  );
};

export default OrderCancel;
