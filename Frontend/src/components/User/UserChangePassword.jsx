import React, { useState } from "react";
import { Button } from "@material-tailwind/react";
import { useTranslation } from "react-i18next";
import axios from "axios";
import { toast, ToastContainer } from "react-toastify";
import "react-toastify/dist/ReactToastify.css";

export default function UserChangePassword() {
  const { t } = useTranslation();
  const [isChangingPassword, setIsChangingPassword] = useState(false);
  const [oldPassword, setOldPassword] = useState("");
  const [newPassword, setNewPassword] = useState("");
  const [confirmNewPassword, setConfirmNewPassword] = useState("");

  const handleToggleChangePassword = () => {
    setIsChangingPassword(!isChangingPassword);
  };

  // const handleConfirmPasswordChange = async () => {
  //   if (newPassword !== confirmNewPassword) {
  //     toast.error(t("manage_account.passwords_do_not_match"));
  //     return;
  //   }

  //   try {
  //     const response = await axios.post("/api/Auth/reset-password", {
  //       oldPassword,
  //       newPassword,
  //     });

  //     if (response.status === 200) {
  //       toast.success(t("manage_account.password_change_successful"));
  //       setIsChangingPassword(false);
  //     }
  //   } catch (error) {
  //     toast.error(t("manage_account.password_change_failed"));
  //   }
  // };

  


  return (
    <>
      <ToastContainer />
      <Button
                color="gray"
                variant="text"
                onClick={handleToggleChangePassword}
              >
                Thay đổi mật khẩu
              </Button>
      {/* <div className="container mx-auto px-20 py-5 bg-white blur-none shadow-xl rounded-lg">
        <h2 className="text-orange-500 font-bold text-xl mb-6">
          {t("manage_account.change_password")}
        </h2>

        <div className="space-y-4">
          {isChangingPassword ? (
            <>
              <div className="w-4/12">
                <label className="block text-gray-700 font-semibold mb-2">
                  {t("manage_account.current_password")}:
                </label>
                <input
                  type="password"
                  className="w-full p-2 border border-gray-300"
                  value={oldPassword}
                  onChange={(e) => setOldPassword(e.target.value)}
                />
              </div>

              <div className="flex justify-between">
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
              </div>
            </>
          ) : (
            <div className="w-4/12">
              <label className="block text-gray-700 font-semibold mb-2">
                {t("manage_account.password")}:
              </label>
              <input
                type="password"
                className="w-full p-2 bg-gray-100 text-gray-500"
                value="*********"
                readOnly
              />
            </div>
          )}
        </div>

        <div className="flex justify-end mt-8">
          {isChangingPassword ? (
            <>
              <Button
                color="gray"
                variant="text"
                onClick={handleToggleChangePassword}
              >
                {t("manage_account.cancel_changing_password")}
              </Button>
              <Button
                color="orange"
                variant="filled"
                onClick={handleConfirmPasswordChange}
              >
                {t("manage_account.confirm_changing_password")}
              </Button>
            </>
          ) : (
            <Button
              color="orange"
              variant="filled"
              onClick={handleToggleChangePassword}
            >
              {t("manage_account.change_password")}
            </Button>
          )}
        </div>
      </div>{" "} */}
    </>
  );
}
