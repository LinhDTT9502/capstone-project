import React from 'react';
import { useSelector } from 'react-redux';
import { Button } from '@material-tailwind/react';
import { selectUser } from "../../redux/slices/authSlice";
import { useTranslation } from "react-i18next";


const UserProfile = ({ onEditClick }) => {
    const user = useSelector(selectUser);
    const { t } = useTranslation();

  return (
    <div>
      <h2 className="text-orange-500 font-semibold text-xl mb-4">{t("user_profile.user_profile")}</h2>
      <div className="space-y-4">
        <div>
          <label className="block text-gray-700">{t("user_profile.username")}:</label>
          <p>{user.UserName}</p>
        </div>
        <div>
          <label className="block text-gray-700">{t("user_profile.fullname")}:</label>
          <p>{user.FullName}</p>
        </div>
        {/* <div>
          <label className="block text-gray-700">Gender:</label>
          <p>{user.Gender}</p>
        </div> */}
        <div>
          <label className="block text-gray-700">{t("user_profile.email")}:</label>
          <p>{user.Email}</p>
        </div>
        {/* <div>
          <label className="block text-gray-700">Phone:</label>
          <p>{user.Phone}</p>
        </div> */}
        {/* <div>
          <label className="block text-gray-700">Address:</label>
          <p>{user.Address}</p>
        </div> */}
        <div className="flex justify-end space-x-4">
          <Button
            color="orange"
            variant="filled"
            onClick={onEditClick}
          >
            {t("user_profile.edit_profile")}
          </Button>
        </div>
      </div>
    </div>
  );
};

export default UserProfile;