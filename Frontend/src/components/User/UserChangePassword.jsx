import React, { useState } from "react";
import { Button } from "@material-tailwind/react";
import { useTranslation } from "react-i18next";

export default function UserChangePassword() {
  const { t } = useTranslation();
  const [isChangingPassword, setIsChangingPassword] = useState(false);

  const handleToggleChangePassword = () => {
    setIsChangingPassword(!isChangingPassword);
  };

  return (
    <div className="container mx-auto px-20 py-5 bg-white blur-none shadow-xl rounded-lg">
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
                />
              </div>

              <div className="w-4/12">
                <label className="block text-gray-700 font-semibold mb-2">
                  {t("manage_account.confirm_new_password")}:
                </label>
                <input
                  type="password"
                  className="w-full p-2 border border-gray-300"
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
            <Button color="gray" variant="text" onClick={handleToggleChangePassword}>
              {t("manage_account.cancel_changing_password")}
            </Button>
            <Button color="orange" variant="filled" onClick={() => {}}>
              {t("manage_account.confirm_changing_password")}
            </Button>
          </>
        ) : (
          <Button color="orange" variant="filled" onClick={handleToggleChangePassword}>
            {t("manage_account.change_password")}
          </Button>
        )}
      </div>
    </div>
  );
}
