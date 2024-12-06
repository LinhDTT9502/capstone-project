import { FontAwesomeIcon } from "@fortawesome/react-fontawesome";
import { faCashRegister, faQrcode, faCreditCard, faUniversity } from "@fortawesome/free-solid-svg-icons";
import React from "react";
import { useTranslation } from "react-i18next";

const PaymentMethod = ({ selectedOption, handleOptionChange }) => {
  const { t } = useTranslation();

  return (
    <div className="bg-white p-6">
      {/* <h3 className="text-2xl font-semibold text-gray-800 mb-6">{t("payment.payment_method")}</h3> */}

      <div className="mb-6">
        <label className="flex items-center space-x-3 cursor-pointer">
          <input
            type="radio"
            name="paymentOption"
            value="1"
            checked={selectedOption === "1"}
            onChange={handleOptionChange}
            className="form-radio text-orange-500 focus:ring-orange-500"
          />
          <FontAwesomeIcon icon={faCashRegister} className="text-gray-600 w-6 h-6" />
          <span className="text-gray-800 text-lg">{t("payment.cash_on_delivery")}</span>
        </label>
        {selectedOption === "1" && (
          <p className="mt-3 ml-8 text-sm text-gray-600 bg-gray-100 p-3 rounded-lg">
            {t("payment.cash_on_delivery_description")}
          </p>
        )}
      </div>

      {/* VietQR Payment Option */}
      <div className="mb-6">
        <label className="flex items-center space-x-3 cursor-pointer">
          <input
            type="radio"
            name="paymentOption"
            value="2"
            checked={selectedOption === "2"}
            onChange={handleOptionChange}
            className="form-radio text-orange-500 focus:ring-orange-500"
          />
          <FontAwesomeIcon icon={faQrcode} className="text-gray-600 w-6 h-6" />
          <span className="text-gray-800 text-lg">Quét mã VietQR</span>
        </label>
        {selectedOption === "2" && (
          <p className="mt-3 ml-8 text-sm text-gray-600 bg-gray-100 p-3 rounded-lg">
            {t("payment.bank_transfer_description")}
          </p>
        )}
      </div>

      {/* VNPay Payment Option */}
      <div className="mb-6">
        <label className="flex items-center space-x-3 cursor-pointer">
          <input
            type="radio"
            name="paymentOption"
            value="3"
            checked={selectedOption === "3"}
            onChange={handleOptionChange}
            className="form-radio text-orange-500 focus:ring-orange-500"
          />
          <FontAwesomeIcon icon={faCreditCard} className="text-gray-600 w-6 h-6" />
          <span className="text-gray-800 text-lg">Thanh toán trực tuyến VNPay</span>
        </label>
        {selectedOption === "3" && (
          <p className="mt-3 ml-8 text-sm text-gray-600 bg-gray-100 p-3 rounded-lg">
            Khi lựa chọn hình thức thanh toán qua VNPay, quý khách vui lòng đảm bảo rằng thông tin thanh toán được điền đầy đủ và chính xác.
            Sau khi thực hiện thanh toán, quý khách sẽ nhận được thông báo xác nhận từ hệ thống. Vui lòng nhấn "Thanh toán" để thực hiện thanh toán đơn hàng ở bước tiếp theo.
          </p>
        )}
      </div>

      {/* Bank Transfer Option */}
      <div className="mb-6">
        <label className="flex items-center space-x-3 cursor-pointer">
          <input
            type="radio"
            name="paymentOption"
            value="4"
            checked={selectedOption === "4"}
            onChange={handleOptionChange}
            className="form-radio text-orange-500 focus:ring-orange-500"
          />
          <FontAwesomeIcon icon={faUniversity} className="text-gray-600 w-6 h-6" />
          <span className="text-gray-800 text-lg">{t("payment.bank_transfer")}</span>
        </label>
        {selectedOption === "4" && (
          <div className="mt-4 ml-8 text-sm text-gray-700 bg-gray-100 p-4 rounded-lg flex flex-col items-center text-center">
            <p className="font-bold text-gray-800">STK: 04072353101 - DUONG THI TRUC LINH</p>
            <p className="font-bold text-gray-800 mb-2">Ngân hàng TP Bank - Ngân hàng Thương mại Cổ phần Tiên Phong</p>
            <img
              src="/assets/images/QR/tpbank.jpg"
              alt="TP Bank QR"
              className="w-36 h-auto object-contain mb-4"
            />
            <p className="text-sm text-gray-600">
              Khi chuyển khoản, quý khách vui lòng ghi Mã số Đơn hàng vào phần nội dung thanh toán của lệnh chuyển khoản. (VD: Tên – Mã Đơn hàng – SĐT).
              Trong vòng 48h kể từ khi đặt hàng thành công (không kể thứ Bảy, Chủ nhật và ngày lễ), nếu quý khách vẫn chưa thanh toán, chúng tôi xin phép hủy đơn hàng.
            </p>
          </div>
        )}
      </div>
    </div>
  );
};

export default PaymentMethod;

