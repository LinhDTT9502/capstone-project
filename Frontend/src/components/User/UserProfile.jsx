import React, { useState } from "react";
import { useSelector } from "react-redux";
import { Button } from "@material-tailwind/react";
import { selectUser } from "../../redux/slices/authSlice";
import { useTranslation } from "react-i18next";

const UserProfile = () => {
  const user = useSelector(selectUser);
  const { t } = useTranslation();
  const [isEditing, setIsEditing] = useState(false);

  const handleEditClick = () => {
    setIsEditing(true);
  };

  const handleSaveClick = () => {
    setIsEditing(false);
    // Code lưu thông tin
  };

  return (
    <div className="container mx-auto px-20 py-5 bg-white blur-none shadow-xl rounded-lg">
      <h2 className="text-orange-500 font-bold text-xl mb-6">
        {t("user_profile.user_profile")}
      </h2>

      <div className="space-y-4">
        <div>
          <label className="block text-gray-700 font-semibold mb-2">
            {t("user_profile.username")}:
          </label>
          <input 
            type="text" 
            className={`w-full p-2 ${isEditing ? 'border border-gray-300' : 'bg-gray-100 text-gray-500 pointer-events-none'}`}
            value={user.UserName}
            readOnly={!isEditing} 
          />
        </div>

        <div>
          <label className="block text-gray-700 font-semibold mb-2">
            {t("user_profile.fullname")}:
          </label>
          <input 
            type="text" 
            className={`w-full p-2 ${isEditing ? 'border border-gray-300 ' : 'bg-gray-100 text-gray-500 pointer-events-none'}`}
            value={user.FullName}
            readOnly={!isEditing} 
          />
        </div>

        <div>
          <label className="block text-gray-700 font-semibold mb-2">
            {t("user_profile.email")}:
          </label>
          <input 
            type="email" 
            className={`w-full p-2 ${isEditing ? 'border border-gray-300' : 'bg-gray-100 text-gray-500 pointer-events-none'}`}
            value={user.Email}
            readOnly={!isEditing} 
          />
        </div>

        <div className="flex justify-end mt-8">
          {isEditing ? (
            <>
              <Button color="gray" variant="text" onClick={() => setIsEditing(false)}>
                {t("user_profile.cancel")}
              </Button>
              <Button color="orange" variant="filled" onClick={handleSaveClick}>
                {t("user_profile.save_changes")}
              </Button>
            </>
          ) : (
            <Button color="orange" variant="filled" onClick={handleEditClick}>
              {t("user_profile.edit_profile")}
            </Button>
          )}
        </div>
      </div>
    </div>
  );
};

export default UserProfile;
