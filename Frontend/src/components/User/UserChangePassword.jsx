import React, { useState, useEffect } from "react";
import { Button } from "@material-tailwind/react";
import { useTranslation } from "react-i18next";
import axios from "axios";
import { useSelector } from "react-redux";
import { toast, ToastContainer } from "react-toastify";
import "react-toastify/dist/ReactToastify.css";
import { useLocation } from "react-router-dom";
import { forgotPasswordRequest, resetPassword } from "../../api/apiAuth";
import { selectUser } from "../../redux/slices/authSlice";

export default function UserChangePassword() {
  const { t } = useTranslation();
  const user = useSelector(selectUser);
  const [isChangingPassword, setIsChangingPassword] = useState(false);
  const [oldPassword, setOldPassword] = useState("");
  const [newPassword, setNewPassword] = useState("");
  const [confirmNewPassword, setConfirmNewPassword] = useState("");
  const [isEmailVerified, setIsEmailVerified] = useState(false);
  const location = useLocation();

 // Function to handle password change request
 const handlePasswordChange = async () => {
  if (newPassword !== confirmNewPassword) {
    toast.error(t("manage_account.passwords_do_not_match"));
    return;
  }

  try {
    const response = await resetPassword({
      newPassword,
    });

    if (response.status === 200) {
      toast.success(t("manage_account.password_change_successful"));
      setIsChangingPassword(false);
    }
  } catch (error) {
    toast.error(t("manage_account.password_change_failed"));
  }
};

const handleToggleChangePassword = async () => {
  if (!isEmailVerified) {
    // If email is not verified, send verification email using user's email from Redux
    try {
      const response = await forgotPasswordRequest(user.Email); 
      toast.success(t("manage_account.email_verification_sent"));
    } catch (error) {
      toast.error(t("manage_account.email_verification_failed"));
    }
  } else {
    setIsChangingPassword(true);
  }
};

// Effect to capture email verification status from query params after redirect
useEffect(() => {
  const urlParams = new URLSearchParams(location.search);
  const emailVerified = urlParams.get("verified");

  if (emailVerified === "true") {
    setIsEmailVerified(true); 
    toast.success(t("manage_account.email_verified"));
  }
}, [location.search]);

  return (
<>
      <ToastContainer />

      {/* Show the button to start changing password */}
      <Button color="gray" variant="text" onClick={handleToggleChangePassword}>
        Thay đổi mật khẩu
      </Button>

      {isChangingPassword && (
        <div className="container mx-auto px-20 py-5 bg-white blur-none shadow-xl rounded-lg">
          <h2 className="text-orange-500 font-bold text-xl mb-6">
            {t("manage_account.change_password")}
          </h2>

          <div className="space-y-4">
            <div className="w-4/12">
              <label className="block text-gray-700 font-semibold mb-2">
                {t("manage_account.new_password")}:
              </label>
              <input
                type="password"
                className="w-full p-2 border border-gray-300"
                value={newPassword}
                onChange={(e) => setNewPassword(e.target.value)}
              />
            </div>

            <div className="w-4/12">
              <label className="block text-gray-700 font-semibold mb-2">
                {t("manage_account.confirm_new_password")}:
              </label>
              <input
                type="password"
                className="w-full p-2 border border-gray-300"
                value={confirmNewPassword}
                onChange={(e) => setConfirmNewPassword(e.target.value)}
              />
            </div>

            <div className="flex justify-end mt-8">
              <Button
                color="orange"
                variant="filled"
                onClick={handlePasswordChange}
              >
                {t("manage_account.confirm_changing_password")}
              </Button>
            </div>
          </div>
        </div>
      )}
    </>
  );
}
