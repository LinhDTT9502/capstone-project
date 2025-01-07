import { useState, useRef } from "react";
import {
  sendOtpForEmailChangeService,
  editPhoneNumberService,
  changeEmailService,
  sendSmsOtp,
} from "../../services/ManageUserService";

export default function AuthInfo({
  userId,
  email,
  phone,
  emailConfirmed,
  phoneNumberConfirmed,
  onVerifyEmail,
  onVerifyPhone,
}) {
  const [showEmailDialog, setShowEmailDialog] = useState(false);
  const [showPhoneDialog, setShowPhoneDialog] = useState(false);
  const [isOtpSent, setIsOtpSent] = useState(false);
  const [newEmail, setNewEmail] = useState("");
  const [newPhone, setNewPhone] = useState("");

  const [otp, setOtp] = useState(Array(6).fill(""));
  const [token, setToken] = useState("");
  const otpInputs = useRef([]);

  const resetOtp = () => setOtp(Array(6).fill(""));

  const resetAllState = () => {
    setNewEmail("");
    setNewPhone("");
    setOtp(Array(6).fill(""));
    setIsOtpSent(false);
    setToken("");
  };

  const handleSendOtp = async (type) => {
    try {
      let response;
      if (type === "email") {
        response = await sendOtpForEmailChangeService(userId, newEmail);
      } else {
        response = await sendSmsOtp(newPhone);
      }
      if (response) {
        alert("Mã OTP đã được gửi, vui lòng kiểm tra.");
        setToken(response.token);
        setIsOtpSent(true);
      }
    } catch {
      alert("Không thể gửi mã OTP, vui lòng thử lại.");
    }
  };

  const handleOtpChange = (index, value) => {
    if (!/^\d?$/.test(value)) return;

    const newOtp = [...otp];
    newOtp[index] = value;
    setOtp(newOtp);

    if (value && index < 5) otpInputs.current[index + 1]?.focus();
  };

  const handleBackspace = (index, e) => {
    if (e.key === "Backspace" && !otp[index] && index > 0) {
      otpInputs.current[index - 1]?.focus();
    }
  };

  const handleConfirmChange = async (type) => {
    try {
      const otpCode = otp.join("");
      if (type === "email") {
        await changeEmailService(userId, token, newEmail, otpCode);
        alert("Email cập nhật thành công!");
        setShowEmailDialog(false);
        onVerifyEmail(newEmail);
      } else {
        await editPhoneNumberService(newPhone, otpCode);
        alert("Số điện thoại cập nhật thành công!");
        setShowPhoneDialog(false);
        onVerifyPhone(newPhone);
      }
      resetAllState();
    } catch {
      alert("Xác thực thất bại, vui lòng thử lại.");
    }
  };

  return (
    <div className="grid grid-cols-1 sm:grid-cols-2 gap-6">
      <div>
        <label className="block text-gray-700 mb-2">Email:</label>
        <div className="relative">
          <input
            type="email"
            className="w-full p-3 bg-gray-100 text-gray-500 rounded-md"
            value={email}
            readOnly
          />
          <span className={`absolute right-4 top-3 ${emailConfirmed ? "text-green-500" : "text-red-500"}`}>
            {emailConfirmed ? "Đã xác thực" : "Chưa xác thực"}
          </span>
        </div>
        <button
          className="mt-2 px-4 py-2 bg-blue-500 text-white rounded-md hover:bg-blue-600"
          onClick={() => {
            resetAllState();
            setShowEmailDialog(true);
          }}
        >
          Chỉnh sửa email
        </button>
      </div>

      <div>
        <label className="block text-gray-700 mb-2">Số điện thoại:</label>
        <div className="relative">
          <input
            type="text"
            className="w-full p-3 bg-gray-100 text-gray-500 rounded-md"
            value={phone}
            readOnly
          />
          <span className={`absolute right-4 top-3 ${phoneNumberConfirmed ? "text-green-500" : "text-red-500"}`}>
            {phoneNumberConfirmed ? "Đã xác thực" : "Chưa xác thực"}
          </span>
        </div>
        <button
          className="mt-2 px-4 py-2 bg-green-500 text-white rounded-md hover:bg-green-600"
          onClick={() => {
            resetAllState();
            setShowPhoneDialog(true);
          }}
        >
          Chỉnh sửa SĐT
        </button>
      </div>

      {showEmailDialog && (
        <div className="fixed inset-0 bg-black bg-opacity-50 flex items-center justify-center">
          <div className="bg-white p-6 rounded-lg w-96">
            <h2 className="text-xl font-bold mb-4">Thay đổi Email</h2>
            {!isOtpSent ? (
              <input
                type="email"
                value={newEmail}
                onChange={(e) => setNewEmail(e.target.value)}
                placeholder="Nhập email mới"
                className="w-full p-3 border rounded-md mb-4"
              />
            ) : (
              <div>
                <p className="mb-2 text-gray-600">Nhập mã OTP:</p>
                <div className="flex justify-center space-x-2 mb-4">
                  {otp.map((digit, index) => (
                    <input
                      key={index}
                      ref={(el) => (otpInputs.current[index] = el)}
                      type="text"
                      maxLength="1"
                      value={digit}
                      onChange={(e) => handleOtpChange(index, e.target.value)}
                      onKeyDown={(e) => handleBackspace(index, e)}
                      className="w-10 h-10 text-center text-2xl border border-gray-300 rounded-md focus:border-blue-500 focus:outline-none"
                    />
                  ))}
                </div>
              </div>
            )}
            <div className="flex justify-end space-x-2">
              <button
                className="px-4 py-2 bg-gray-400 text-white rounded-md"
                onClick={() => {
                  setShowEmailDialog(false);
                  resetAllState();
                }}
              >
                Hủy
              </button>
              <button
                className="px-4 py-2 bg-blue-500 text-white rounded-md hover:bg-blue-600"
                onClick={() => (isOtpSent ? handleConfirmChange("email") : handleSendOtp("email"))}
              >
                {isOtpSent ? "Xác nhận" : "Gửi mã"}
              </button>
            </div>
          </div>
        </div>
      )}

      {showPhoneDialog && (
        <div className="fixed inset-0 bg-black bg-opacity-50 flex items-center justify-center z-[999]">
          <div className="bg-white p-6 rounded-lg w-96">
            <h2 className="text-xl font-bold mb-4">Thay đổi Số Điện Thoại</h2>
            {!isOtpSent ? (
              <input
                type="text"
                value={newPhone}
                onChange={(e) => setNewPhone(e.target.value)}
                placeholder="Nhập số điện thoại mới"
                className="w-full p-3 border rounded-md mb-4"
              />
            ) : (
              <div>
                <p className="mb-2 text-gray-600">Nhập mã OTP:</p>
                <div className="flex justify-center space-x-2 mb-4">
                  {otp.map((digit, index) => (
                    <input
                      key={index}
                      ref={(el) => (otpInputs.current[index] = el)}
                      type="text"
                      maxLength="1"
                      value={digit}
                      onChange={(e) => handleOtpChange(index, e.target.value)}
                      onKeyDown={(e) => handleBackspace(index, e)}
                      className="w-10 h-10 text-center text-2xl border border-gray-300 rounded-md focus:border-blue-500 focus:outline-none"
                    />
                  ))}
                </div>
              </div>
            )}
            <div className="flex justify-end space-x-2">
              <button
                className="px-4 py-2 bg-gray-400 text-white rounded-md"
                onClick={() => {
                  setShowPhoneDialog(false);
                  resetAllState();
                }}
              >
                Hủy
              </button>
              <button
                className="px-4 py-2 bg-green-500 text-white rounded-md hover:bg-green-600"
                onClick={() => (isOtpSent ? handleConfirmChange("phone") : handleSendOtp("phone"))}
              >
                {isOtpSent ? "Xác nhận" : "Gửi mã"}
              </button>
            </div>
          </div>
        </div>
      )}
    </div>
  );
}

