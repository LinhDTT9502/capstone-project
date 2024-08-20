import React from "react";
import { FontAwesomeIcon } from "@fortawesome/react-fontawesome";
import {
  faTruck,
  faThumbsUp,
  faCreditCard,
  faArrowRight,
} from "@fortawesome/free-solid-svg-icons";
import { useTranslation } from "react-i18next";

const WebServices = () => {
  const { t } = useTranslation();
  return (
    <div className="flex flex-col items-center px-20 pb-32 pt-5">
      <div className=" bg-zinc-700 text-white items-center  w-full flex justify-around pb-20 pt-5 relative">
        <h2 className="font-alfa text-4xl pl-4 border-l-4 border-orange-500">
          {t("webservice.title1")}
          <br />
          {t("webservice.title2")}
        </h2>
        <p className="font-poppins">
          {t("webservice.subtitle1")}
          <br />
          {t("webservice.subtitle2")}
        </p>
        <p className=" hover:text-orange-500 text-white transition">
          {t("webservice.btn")}
          <FontAwesomeIcon
            className="pl-2 text-orange-500 text-xl"
            icon={faArrowRight}
          />
        </p>
      </div>
      <div className="flex w-3/4 justify-around bg-white border-zinc-600 border-2 absolute mt-32 py-5 drop-shadow-lg">
        <div className="text-center">
          <FontAwesomeIcon icon={faTruck} className="text-4xl text-gray-400" />
          <h3 className="mt-4 text-xl font-semibold">
            {t("webservice.title3")}
          </h3>
          <p className="mt-2 text-gray-4000">{t("webservice.subtitle3")}</p>
        </div>
        <div className="text-center">
          <FontAwesomeIcon
            icon={faThumbsUp}
            className="text-4xl text-gray-400"
          />
          <h3 className="mt-4 text-xl font-semibold">
            {t("webservice.title4")}
          </h3>
          <p className="mt-2 text-gray-400">{t("webservice.subtitle4")}</p>
        </div>
        <div className="text-center">
          <FontAwesomeIcon
            icon={faCreditCard}
            className="text-4xl text-gray-4000"
          />
          <h3 className="mt-4 text-xl font-semibold">
            {t("webservice.title5")}
          </h3>
          <p className="mt-2 text-gray-400">{t("webservice.subtitle5")}</p>
        </div>
      </div>
    </div>
  );
};

export default WebServices;
