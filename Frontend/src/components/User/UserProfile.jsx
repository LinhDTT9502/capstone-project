import React, { useState, useEffect  } from "react";
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
import {fetchUserProfile} from "../../services/ManageUserService";

const UserProfile = () => {
  const user = useSelector(selectUser);
  const dispatch = useDispatch();
  const { t } = useTranslation();
  const [loading, setLoading] = useState(true);
  const [isEditing, setIsEditing] = useState(false);
  const [formData, setFormData] = useState({
    UserName: user.UserName,
    FullName: user.FullName,
    Gender: user.Gender || null,
    Address: user.Address || null,
    BirthDate: user.BirthDate || null,
    EmailConfirmed: user.EmailConfirmed || false, 
    PhoneNumberConfirmed: user.PhoneNumberConfirmed || false, 
    
  });

  useEffect(() => {
    if (user.PhoneNumberConfirmed === undefined || user.PhoneNumberConfirmed === null) {
      setLoading(true);
      fetchUserProfile(user.UserId)
        .then((response) => {
          console.log("Full Response:", response);
          // Lấy trực tiếp từ response thay vì response.data
          const phoneNumberConfirmed = response.phoneNumberConfirmed;
          setFormData((prev) => ({
            ...prev,
            PhoneNumberConfirmed: phoneNumberConfirmed ?? false,
          }));
        })
        .catch((error) => {
          console.error("API Error:", error);
          toast.error("Failed to fetch user profile!");
        })
        .finally(() => setLoading(false));
    }
  }, [user.UserId, user.PhoneNumberConfirmed]);
  
  
  const handleEditClick = () => setIsEditing(true);


  const handleSaveClick = () => {
    if (JSON.stringify(formData) === JSON.stringify({
      UserName: user.UserName,
      FullName: user.FullName,
      Gender: user.Gender,
      Address: user.Address,
      BirthDate: user.BirthDate,
      EmailConfirmed: user.EmailConfirmed || false, 
      PhoneNumberConfirmed: user.PhoneNumberConfirmed || false,  
    })) {
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
    setFormData({
      UserName: user.UserName,
      FullName: user.FullName,
      Gender: user.Gender || null,
      Address: user.Address || null,
      BirthDate: user.BirthDate || null,
      EmailConfirmed: user.EmailConfirmed || false,  
      PhoneNumberConfirmed: user.PhoneNumberConfirmed || false,  

    });
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

          {/* Column 2: 3 fields (Gender, Address, Birth Date) */}
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

          {/* Nút chỉnh sửa và lưu */}
          <div className="flex justify-end mb-4">
            {isEditing ? (
              <>
                <Button color="gray" variant="text" onClick={handleCancelClick}>{t("user_profile.cancel")}</Button>
                <Button color="orange" variant="filled" onClick={handleSaveClick}>{t("user_profile.save_changes")}</Button>
              </>
            ) : (
              <Button color="orange" variant="filled" onClick={handleEditClick}>{t("user_profile.edit_profile")}</Button>
            )}
          </div>

          {/* Thông tin xác thực */}
          <div className="mt-6">
            <h3 className="text-xl font-semibold text-orange-500">Thông tin xác thực</h3>
            <AuthInfo 
              email={user.Email} 
              phone={user.Phone} 
              emailConfirmed={user.EmailConfirmed}
              phoneNumberConfirmed={formData.PhoneNumberConfirmed}
              userId={user.UserId}
            />
          </div>
        </div>
      </div>
    </>
  );
};

export default UserProfile;
