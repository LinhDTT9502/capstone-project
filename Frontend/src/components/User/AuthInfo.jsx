import { useState } from "react";
import {
  sendOtpForEmailChangeService,
  changeEmailService,
} from "../../services/ManageUserService";

import { updateProfile,
} from "../../api/apiUser";


export default function AuthInfo({
  userId,
  email,
  phone,
  emailConfirmed,
  phoneNumberConfirmed,
  onChangeEmail,
  onChangePhone,
  onVerifyEmail,
  onVerifyPhone,
}) {
  const [showEmailDialog, setShowEmailDialog] = useState(false);
  const [newEmail, setNewEmail] = useState("");
  const [otp, setOtp] = useState(["", "", "", "", "", ""]);
  const [isOtpSent, setIsOtpSent] = useState(false);
  const [token, setToken] = useState("");

  const [showPhoneDialog, setShowPhoneDialog] = useState(false);
  const [newPhone, setNewPhone] = useState("");

  const handlePhoneChange = async () => {
    if (newPhone) {
      try {
        await updateProfile(userId, { phone: newPhone });
        alert("Số điện thoại đã được cập nhật!");
        setShowPhoneDialog(false);
      } catch (error) {
        alert("Cập nhật số điện thoại thất bại");
      }
    } else {
      alert("Vui lòng nhập số điện thoại mới");
    }
  };


  const handleEmailChange = async () => {
    if (newEmail && otp) {
      try {
        await changeEmailService(userId, token, newEmail, otp.join(''));
        alert("Email updated successfully!");
        setShowEmailDialog(false);
      } catch (error) {
        alert("Failed to update email");
      }
    } else {
      alert("Please enter a valid email and OTP");
    }
  };

  const sendOtp = async () => {
    if (!newEmail) {
      alert("Error: Please enter a new email.");
      return;
    }

    const emailRegex = /^[^\s@]+@[^\s@]+\.[^\s@]+$/;
    if (!emailRegex.test(newEmail)) {
      alert("Địa chỉ email không đúng, vui lòng nhập lại");
      return;
    }

    try {
      const response = await sendOtpForEmailChangeService(userId, newEmail);
      if (response) {
        alert("Mã xác thực đã được gửi, vui lòng kiểm ta hộp thư");
        setIsOtpSent(true);
        setToken(response.token);
      } else {
        alert("Gửi mã xác thực thất bại");
      }
    } catch (error) {
      alert("Error sending OTP");
    }
  };

  const emailConfirmedBool = emailConfirmed ?? false;
  const phoneNumberConfirmedBool = phoneNumberConfirmed ?? false;

  return (
    <div className="grid grid-cols-1 sm:grid-cols-2 gap-6">
      <div className="relative">
        <label className="block text-gray-700 mb-2">Email:</label>
        <div className="relative">
          <span className="absolute left-4 top-3 text-gray-500">
            <svg
              xmlns="http://www.w3.org/2000/svg"
              className="h-5 w-5"
              viewBox="0 0 20 20"
              fill="currentColor"
            >
              <path d="M2.003 5.884L10 9.882l7.997-3.998A2 2 0 0016 4H4a2 2 0 00-1.997 1.884z" />
              <path d="M18 8.118l-8 4-8-4V14a2 2 0 002 2h12a2 2 0 002-2V8.118z" />
            </svg>
          </span>
          <input
            type="email"
            className="w-full p-3 pl-12 bg-gray-100 text-gray-500 cursor-not-allowed rounded-md"
            value={email}
            readOnly
          />
          <span
            className={`absolute right-4 top-3 ${
              emailConfirmedBool ? "text-green-500" : "text-red-500"
            }`}
          >
            {emailConfirmedBool ? "Verified" : "Not Verified"}
          </span>
        </div>
        {email ? (
          <button
            className={`mt-2 px-4 py-2 rounded-md text-white ${
              emailConfirmedBool
                ? "bg-orange-500 hover:bg-orange-600"
                : "bg-blue-500 hover:bg-blue-600"
            }`}
            onClick={
              emailConfirmedBool
                ? () => setShowEmailDialog(true)
                : onVerifyEmail
            }
          >
            {emailConfirmedBool ? "Change" : "Verify"}
          </button>
        ) : (
          <div className="w-full p-3 pl-12 bg-gray-100 text-gray-500 cursor-not-allowed rounded-md mt-2">
            No email provided
          </div>
        )}
      </div>

      <div className="relative">
        <label className="block text-gray-700 mb-2">Phone Number:</label>
        <div className="relative">
          <input
            type="text"
            className="w-full p-3 bg-gray-100 text-gray-500 cursor-not-allowed rounded-md"
            value={phone}
            readOnly
          />
          <span
            className={`absolute right-4 top-3 ${
              phoneNumberConfirmed ? "text-green-500" : "text-red-500"
            }`}
          >
            {phoneNumberConfirmed ? "Verified" : "Not Verified"}
          </span>
        </div>
        <div className="flex space-x-2">
          <button
            className={`mt-2 px-4 py-2 rounded-md text-white ${
              phoneNumberConfirmed
                ? "bg-orange-500 hover:bg-orange-600"
                : "bg-blue-500 hover:bg-blue-600"
            }`}
            onClick={onVerifyPhone}
          >
            Verify
          </button>
          
            <button
              className="mt-2 px-4 py-2 rounded-md text-white bg-green-500 hover:bg-green-600"
              onClick={() => setShowPhoneDialog(true)}
            >
              Change
            </button>
          
        </div>
      </div>
       {/* Popup thay đổi số điện thoại */}
       {showPhoneDialog && (
        <div className="fixed inset-0 bg-black bg-opacity-50 flex items-center justify-center">
          <div className="bg-white p-6 rounded-lg w-96">
            <h2 className="text-xl font-bold mb-4">Thay đổi số điện thoại</h2>
            <div className="space-y-4">
              <div>
                <label className="block text-gray-700 mb-2">Nhập số điện thoại mới</label>
                <input
                  type="text"
                  value={newPhone}
                  onChange={(e) => setNewPhone(e.target.value)}
                  className="w-full p-2 border rounded-md"
                  placeholder="Nhập số điện thoại mới"
                />
              </div>
              <div className="flex justify-end space-x-2">
                <button
                  className="px-4 py-2 bg-red-500 text-white rounded-md hover:bg-red-600"
                  onClick={() => {
                    setShowPhoneDialog(false);
                    setNewPhone("");
                  }}
                >
                  Hủy
                </button>
                <button
                  className="px-4 py-2 bg-green-500 text-white rounded-md hover:bg-green-600"
                  onClick={handlePhoneChange}
                >
                  Xác nhận
                </button>
              </div>
            </div>
          </div>
        </div>
      )}

      {showEmailDialog && (
        <div className="fixed inset-0 bg-black bg-opacity-50 flex items-center justify-center">
          <div className="bg-white p-6 rounded-lg w-96">
            <h2 className="text-xl font-bold mb-4">Thay đổi Email</h2>
            <div className="space-y-4">
              {!isOtpSent && (
                <div>
                  <label className="block text-gray-700 mb-2">Nhập email mới</label>
                  <input
                    type="email"
                    value={newEmail}
                    onChange={(e) => setNewEmail(e.target.value)}
                    className="w-full p-2 border rounded-md"
                    placeholder="Nhập email mới"
                  />
                </div>
              )}
              {isOtpSent && (
                <div>
                  <label className="block text-gray-700 mb-2">Nhập mã OTP:</label>
                  <div className="flex space-x-2">
                    {otp.map((digit, index) => (
                      <input
                        key={index}
                        type="text"
                        maxLength="1"
                        value={digit}
                        onChange={(e) => {
                          const newOtp = [...otp]
                          newOtp[index] = e.target.value
                          setOtp(newOtp)
                        }}
                        className="w-12 p-2 border rounded-md text-center"
                      />
                    ))}
                  </div>
                </div>
              )}
              <div className="flex justify-end space-x-2">
                <button
                  className="px-4 py-2 bg-red-500 text-white rounded-md hover:bg-red-600"
                  onClick={() => { setShowEmailDialog(false); setOtp(["", "", "", "", "", ""]); setNewEmail(""); }}
                >
                  Hủy
                </button>
                <button
                  className="px-4 py-2 bg-green-500 text-white rounded-md hover:bg-green-600"
                  onClick={isOtpSent ? handleEmailChange : sendOtp}
                >
                  {isOtpSent ? "Xác nhận" : "Gửi mã"}
                </button>
              </div>
            </div>
          </div>
        </div>
      )}
    </div>
  );
}
