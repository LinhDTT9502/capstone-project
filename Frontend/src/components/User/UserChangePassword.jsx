import React, { useState } from "react";
import { useSelector, useDispatch } from "react-redux";
import { Button } from "@material-tailwind/react";
import { selectUser, updateUser } from "../../redux/slices/authSlice";
import { useTranslation } from "react-i18next";
import { updateProfile } from "../../api/apiUser";
import { toast, ToastContainer } from 'react-toastify';
import 'react-toastify/dist/ReactToastify.css';
import { faUser, faCaretDown, faVenusMars, faMapMarkerAlt, faBirthdayCake } from "@fortawesome/free-solid-svg-icons";
import { FontAwesomeIcon } from "@fortawesome/react-fontawesome";
import AuthInfo from "./AuthInfo";

const UserProfile = () => {
  const user = useSelector(selectUser);
  const dispatch = useDispatch();
  const { t } = useTranslation();

  const [isEditing, setIsEditing] = useState(false);
  const [formData, setFormData] = useState({
    UserName: user.UserName,
    FullName: user.FullName,
    Email: user.Email,
    Gender: user.Gender || null,
    Phone: user.Phone || null,
    Address: user.Address || null,
    BirthDate: user.BirthDate || null
  });

  const handleEditClick = () => setIsEditing(true);

  const handleSaveClick = () => {
    if (JSON.stringify(formData) === JSON.stringify(user)) {
      toast.warn(t("user_profile.no_changes"));
      return;
    }

    updateProfile(user.UserId, formData)
      .then(() => {
        setIsEditing(false);
        toast.success(t("user_profile.update_success"));
        dispatch(updateUser(formData));
      })
      .catch(() => toast.error(t("user_profile.save_failed")));
  };

  const handleCancelClick = () => {
    setFormData({ ...user });
    setIsEditing(false);
  };

  const handleChange = (e) => {
    const { name, value } = e.target;
    setFormData((prev) => ({ ...prev, [name]: value }));
  };

  return (
    <>
    <ToastContainer />
    <div className="container mx-auto pt-2 rounded-lg max-w-4xl">
      <h2 className="text-orange-500 font-bold text-2xl mb-6">{t("user_profile.user_profile")}</h2>

      <div className="space-y-6">
        {/* Column 1: 4 fields (UserName, FullName) */}
        <div className="grid grid-cols-1 sm:grid-cols-2 md:grid-cols-2 gap-6">
          <div className="relative">
            <FontAwesomeIcon icon={faUser} className="absolute left-4 top-10 text-gray-500" />
            <label className="block text-gray-700">{t("user_profile.username")}:</label>
            <input
              type="text"
              name="UserName"
              className="w-full p-3 pl-12 bg-gray-100 text-gray-500 cursor-not-allowed"
              value={formData.UserName}
              readOnly
            />
          </div>
          <div className="relative">
            <FontAwesomeIcon icon={faUser} className="absolute left-4 top-10 text-gray-500" />
            <label className="block text-gray-700">{t("user_profile.fullname")}:</label>
            <input
              type="text"
              name="FullName"
              className={`w-full p-3 pl-12 ${isEditing ? 'border border-gray-300' : 'bg-gray-100 text-gray-500 cursor-not-allowed'}`}
              value={formData.FullName}
              onChange={handleChange}
              readOnly={!isEditing}
            />
          </div>
        </div>

        {/* Column 2: Gender, Address, Birth Date */}
        <div className="grid grid-cols-1 sm:grid-cols-2 md:grid-cols-2 gap-6">
          <div className="relative">
            <FontAwesomeIcon icon={faVenusMars} className="absolute left-4 top-10 text-gray-500" />
            <label className="block text-gray-700">{t("user_profile.gender")}:</label>
            {isEditing ? (
              <>
                <select
                  name="Gender"
                  className="w-full p-3 pl-12 pr-10 border border-gray-300 appearance-none"
                  value={formData.Gender}
                  onChange={handleChange}
                >
                  <option value="Nam">{t("user_profile.gender_male")}</option>
                  <option value="Nữ">{t("user_profile.gender_female")}</option>
                  <option value="Khác">{t("user_profile.gender_other")}</option>
                </select>
                <FontAwesomeIcon icon={faCaretDown} className="absolute right-3 top-12 transform -translate-y-1/2 text-gray-500" />
              </>
            ) : (
              <input
                type="text"
                name="Gender"
                className="w-full p-3 pl-12 bg-gray-100 text-gray-500 cursor-not-allowed"
                value={formData.Gender}
                readOnly
              />
            )}
          </div>
          <div className="relative">
            <FontAwesomeIcon icon={faMapMarkerAlt} className="absolute left-4 top-10 text-gray-500" />
            <label className="block text-gray-700">{t("user_profile.address")}:</label>
            <input
              type="text"
              name="Address"
              className={`w-full p-3 pl-12 ${isEditing ? 'border border-gray-300' : 'bg-gray-100 text-gray-500 cursor-not-allowed'}`}
              value={formData.Address}
              onChange={handleChange}
              readOnly={!isEditing}
            />
          </div>
          <div className="relative">
            <FontAwesomeIcon icon={faBirthdayCake} className="absolute left-4 top-10 text-gray-500" />
            <label className="block text-gray-700">{t("user_profile.birthDate")}:</label>
            <input
              type="date"
              name="BirthDate"
              className={`w-full p-3 pl-12 ${isEditing ? 'border border-gray-300' : 'bg-gray-100 text-gray-500 cursor-not-allowed'}`}
              value={formData.BirthDate ? formData.BirthDate.split("T")[0] : ""}
              onChange={handleChange}
              readOnly={!isEditing}
            />
          </div>
        </div>

        {/* Thông tin xác thực */}
        <div className="mt-6 border-t pt-6">
          <h3 className="text-xl font-semibold text-orange-500">{t("user_profile.auth_info")}</h3>
          <AuthInfo formData={formData} handleChange={handleChange} isEditing={isEditing} />
        </div>

        {/* Action Buttons */}
        <div className="flex justify-end space-x-4 mt-6">
          {isEditing ? (
            <>
              <Button color="gray" variant="text" onClick={handleCancelClick}>{t("user_profile.cancel")}</Button>
              <Button color="orange" variant="filled" onClick={handleSaveClick}>{t("user_profile.save_changes")}</Button>
            </>
          ) : (
            <Button color="orange" variant="filled" onClick={handleEditClick}>{t("user_profile.edit_profile")}</Button>
          )}
        </div>
      </div>
    </div>
    </>
  );
};

export default UserProfile;
