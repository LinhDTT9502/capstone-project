import React from "react";
import { useTranslation } from "react-i18next";

const PaymentMethod = ({ selectedOption, handleOptionChange }) => {
  const { t } = useTranslation();

  return (
    <div className="">
      <div className="">
        {/* <h3 className="text-xl font-semibold text-gray-800 mb-6">{t("payment.payment_method")}</h3> */}
        
        <form>
          <div className="mb-6">
            <label className="inline-flex items-center text-gray-800">
              <input
                type="radio"
                name="option"
                value="1"
                className="form-radio text-[#FA7D0B]"
                onChange={handleOptionChange}
              />
              <span className="ml-3">{t("payment.cash_on_delivery")}</span>
            </label>
            {selectedOption === "1" && (
              <p className="mt-4 text-sm text-gray-700 bg-gray-200 p-3 rounded-lg">
                {t("payment.cash_on_delivery_description")}
              </p>
            )}
          </div>

          <div className="mb-6">
            <label className="inline-flex items-center text-gray-800">
              <input
                type="radio"
                name="option"
                value="2"
                className="form-radio text-[#FA7D0B]"
                onChange={handleOptionChange}
              />
              <span className="ml-3">Quét mã VietQR</span>
            </label>
            {selectedOption === "2" && (
              <p className="mt-4 text-sm text-gray-700 bg-gray-200 p-3 rounded-lg">
                {t("payment.bank_transfer_description")}
              </p>
            )}
          </div>

          <div className="mb-6">
            <label className="inline-flex items-center text-gray-800">
              <input
                type="radio"
                name="option"
                value="3"
                className="form-radio text-[#FA7D0B]"
                onChange={handleOptionChange}
              />
              <span className="ml-3">Thanh toán trực tuyến VNPay</span>
            </label>
            {selectedOption === "3" && (
              <p className="mt-4 text-sm text-gray-700 bg-gray-200 p-3 rounded-lg">
                Khi lựa chọn hình thức thanh toán qua VNPay, quý khách vui lòng đảm bảo rằng thông tin thanh toán được điền đầy đủ và chính xác. Sau khi thực hiện thanh toán, quý khách sẽ nhận được thông báo xác nhận từ hệ thống. Vui lòng nhấn "Thanh toán" để thực hiện thanh toán đơn hàng ở bước tiếp theo.
              </p>
            )}
          </div>

          <div className="mb-6">
            <label className="inline-flex items-center text-gray-800">
              <input
                type="radio"
                name="option"
                value="4"
                className="form-radio text-[#FA7D0B]"
                onChange={handleOptionChange}
              />
              <span className="ml-3">{t("payment.bank_transfer")}</span>
            </label>
            {selectedOption === "4" && (
              <div className="mt-4 text-sm text-gray-700 bg-gray-200 p-3 flex flex-col items-center text-center rounded-lg">
                <p className="font-bold text-gray-800">STK: 04072353101 - DUONG THI TRUC LINH</p>
                <p className="font-bold text-gray-800">Ngân hàng TP Bank - Ngân hàng Thương mại Cổ phần Tiên Phong</p>
                <img
                  src="/assets/images/QR/tpbank.jpg"
                  alt="TP Bank QR"
                  className="w-36 py-2 h-auto object-contain"
                />
                <p className="pt-4 text-sm text-gray-700">
                  Khi chuyển khoản, quý khách vui lòng ghi Mã số Đơn hàng vào phần nội dung thanh toán của lệnh chuyển khoản. (VD: Tên – Mã Đơn hàng – SĐT). Trong vòng 48h kể từ khi đặt hàng thành công (không kể thứ Bảy, Chủ nhật và ngày lễ), nếu quý khách vẫn chưa thanh toán, chúng tôi xin phép hủy đơn hàng.
                </p>
              </div>
            )}
          </div>
        </form>
      </div>
    </div>
  );
};

export default PaymentMethod;
