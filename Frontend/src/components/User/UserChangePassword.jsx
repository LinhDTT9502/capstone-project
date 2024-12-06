import React, { useState } from "react";
import { Button } from "@material-tailwind/react";
import { useTranslation } from "react-i18next";
import axios from "axios";
import { toast, ToastContainer } from "react-toastify";
import "react-toastify/dist/ReactToastify.css";
import { faKey  } from "@fortawesome/free-solid-svg-icons";

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
      <div className="container mx-auto pt-2 rounded-lg max-w-4xl">
        <h2 className="text-orange-500 font-bold text-2xl mb-6">
          Thay đổi mật khẩu
        </h2>
        <Button
          color="gray"
          variant="text"
          onClick={handleToggleChangePassword}
          className="flex items-center text-gray-800 hover:bg-orange-500 hover:text-white focus:outline-none transition duration-300 ease-in-out transform hover:scale-105 hover:shadow-lg px-4 py-2 rounded-md"
        >
          <faKey  className="mr-2" /> 
          Đổi mật khẩu tại đây!!!
        </Button>
      </div>
    </>
  );
}
