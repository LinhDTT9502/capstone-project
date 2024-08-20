// PaymentMethod.js
import React from "react";
import { useTranslation } from "react-i18next";

const PaymentMethod = ({ selectedOption, handleOptionChange }) => {
  const { t } = useTranslation();
  return (
    <div className="pl-20 py-10">
      <h3 className="text-2xl font-bold pt-1">{t("payment.payment_method")}</h3>
      <div className="flex pt-3">
        <form className="bg-white p-6 rounded shadow-md w-full border border-black">
          <div className="mb-4">
            <label className="inline-flex items-center">
              <input
                type="radio"
                name="option"
                value="1"
                className="form-radio text-[#FA7D0B]"
                onChange={handleOptionChange}
              />
              <span className="ml-2">{t("payment.cash_on_delivery")}</span>
            </label>
            {selectedOption === "1" && (
              <p className="mt-4 text-sm text-black bg-gray-300 p-2 rounded text-wrap">
                {t("payment.cash_on_delivery_description")}
              </p>
            )}
          </div>
          <div className="mb-4">
            <label className="inline-flex items-center">
              <input
                type="radio"
                name="option"
                value="2"
                className="form-radio text-[#FA7D0B]"
                onChange={handleOptionChange}
              />
              <span className="ml-2">{t("payment.bank_transfer")}</span>
            </label>
            {selectedOption === "2" && (
              <p className="mt-4 text-sm text-black bg-gray-300 p-2 rounded text-wrap">
                {t("payment.bank_transfer_description")}
              </p>
            )}
          </div>
        </form>
      </div>
    </div>
  );
};

export default PaymentMethod;
