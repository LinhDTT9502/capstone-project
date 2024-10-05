import React, { useState, useEffect  } from "react";
import { useSelector, useDispatch } from "react-redux";
import { Button } from "@material-tailwind/react";
import { selectUser, updateUser } from "../../redux/slices/authSlice";
import { useTranslation } from "react-i18next";
import { updateProfile, sendVerificationEmail, verifyEmail } from "../../api/apiUser";
import { toast, ToastContainer } from 'react-toastify';  
import 'react-toastify/dist/ReactToastify.css';  
import { refreshTokenAPI } from "../../api/apiAuth";
import { useLocation } from "react-router-dom";

const UserProfile = () => {
  const user = useSelector(selectUser);
  const dispatch = useDispatch(); 
  const { t } = useTranslation();
  const location = useLocation();
  const [isEmailVerified, setIsEmailVerified] = useState(user.IsEmailVerified || false);
  
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
  const [initialData, setInitialData] = useState({
    UserName: user.UserName,
    FullName: user.FullName,
    Email: user.Email,
    Gender: user.Gender || null,  
    Phone: user.Phone || null,
    Address: user.Address || null,
    BirthDate: user.BirthDate || null
  });

  const handleEditClick = () => {
    setIsEditing(true);
  };

  const handleSaveClick = () => {
    if (JSON.stringify(formData) === JSON.stringify(initialData)) {
      toast.warn(t("user_profile.no_changes"));
      return;
    }

    updateProfile(user.UserId, formData)
      .then(() => {
        setIsEditing(false);
        toast.success(t("user_profile.update_success"));
        setInitialData(formData);

        // Dispatch the updateUser action to update the Redux store
        dispatch(updateUser(formData)); 

        // Optional: refresh token logic
        // const { token, refreshToken } = user; 
        // refreshTokenAPI(token, refreshToken)
        //   .then((response) => {
        //     console.log("Token refreshed successfully", response.data);
        //   })
        //   .catch((err) => {
        //     console.error("Error refreshing token:", err);
        //     toast.error(t("Refresh token failed"));
        //   });
      })
      .catch((err) => {
        console.error("Error updating profile:", err);
        toast.error(t("user_profile.save_failed"));
      });
  };

  const handleCancelClick = () => {
    setFormData(initialData);
    setIsEditing(false);
  };

  const handleChange = (e) => {
    const { name, value } = e.target;

    if (name === "Phone") {
      const phoneNumberPattern = /^0\d*$/;
      if (value === "" || phoneNumberPattern.test(value)) {
        setFormData({
          ...formData,
          [name]: value,
        });
      }
    } else {
      setFormData({
        ...formData,
        [name]: value,
      });
    }
  };

  const handleVerifyEmail = () => {
    sendVerificationEmail(formData.Email)
      .then(() => {
        toast.success(t("user_profile.email_verification_sent"));
      })
      .catch((err) => {
        console.error("Error sending verification email:", err);
        toast.error(t("user_profile.email_verification_failed"));
      });
  };

// Capture the token from the query parameters and verify the email
useEffect(() => {
  const urlParams = new URLSearchParams(location.search);
  const emailVerifiedToken = urlParams.get("token");

  if (emailVerifiedToken) {
    verifyEmail(emailVerifiedToken)
      .then(() => {
        setIsEmailVerified(true);
        toast.success(t("user_profile.email_verified"));
      })
      .catch((err) => {
        console.error("Error verifying email:", err);
        toast.error(t("user_profile.email_verification_failed"));
      });
  }
}, [location.search]);

  return (
    <>
      <ToastContainer />
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
              name="UserName"
              className={`w-full p-2 ${
                isEditing ? 'border border-gray-300' : 'bg-gray-100 text-gray-500 pointer-events-none'
              }`}
              value={formData.UserName}
              onChange={handleChange}
              readOnly
            />
          </div>

          <div>
            <label className="block text-gray-700 font-semibold mb-2">
              {t("user_profile.fullname")}:
            </label>
            <input
              type="text"
              name="FullName"
              className={`w-full p-2 ${
                isEditing ? 'border border-gray-300' : 'bg-gray-100 text-gray-500 pointer-events-none'
              }`}
              value={formData.FullName}
              onChange={handleChange}
              readOnly={!isEditing}
            />
          </div>

          <div>
    <label className="block text-gray-700 font-semibold mb-2">
      {t("user_profile.email")}:
    </label>
    <div className="flex items-center">
      <input
        type="email"
        name="Email"
        className={`w-full p-2 ${
          isEditing ? 'border border-gray-300' : 'bg-gray-100 text-gray-500 pointer-events-none'
        }`}
        value={formData.Email}
        readOnly
      />
      <Button
        color="orange"
        variant="filled"
        onClick={handleVerifyEmail}
        className="ml-2"
      >
        {isEmailVerified ? t("user_profile.email_verified") : t("user_profile.verify_email")}
      </Button>
    </div>
    {!isEmailVerified && (
      <p className="text-red-500 mt-1">{t("user_profile.verify_email_warning")}</p>
    )}
  </div>

          <div>
            <label className="block text-gray-700 font-semibold mb-2">
              {t("user_profile.gender")}:
            </label>
            {isEditing ? (
              <select
                name="Gender"
                className="w-full p-2 border border-gray-300"
                value={formData.Gender}
                onChange={handleChange}
              >
                <option value="Nam">{t("user_profile.gender_male")}</option>
                <option value="Nữ">{t("user_profile.gender_female")}</option>
                <option value="Khác">{t("user_profile.gender_other")}</option>
              </select>
            ) : (
              <input
                type="text"
                name="Gender"
                className="w-full p-2 bg-gray-100 text-gray-500 pointer-events-none"
                value={formData.Gender}
                readOnly
              />
            )}
          </div>

          <div>
            <label className="block text-gray-700 font-semibold mb-2">
              {t("user_profile.phone")}:
            </label>
            <input
              type="text"
              name="Phone"
              className={`w-full p-2 ${
                isEditing ? 'border border-gray-300' : 'bg-gray-100 text-gray-500 pointer-events-none'
              }`}
              value={formData.Phone}
              onChange={handleChange}
              readOnly={!isEditing}
              maxLength={10}
            />
          </div>

          <div>
            <label className="block text-gray-700 font-semibold mb-2">
              {t("user_profile.address")}:
            </label>
            <input
              type="text"
              name="Address"
              className={`w-full p-2 ${
                isEditing ? 'border border-gray-300' : 'bg-gray-100 text-gray-500 pointer-events-none'
              }`}
              value={formData.Address}
              onChange={handleChange}
              readOnly={!isEditing}
            />
          </div>

          <div>
            <label className="block text-gray-700 font-semibold mb-2">
              {t("user_profile.birthDate")}:
            </label>
            <input
              type="date"
              name="BirthDate"
              className={`w-full p-2 ${
                isEditing ? 'border border-gray-300' : 'bg-gray-100 text-gray-500 pointer-events-none'
              }`}
              value={formData.BirthDate ? formData.BirthDate.split("T")[0] : ""}
              onChange={handleChange}
              readOnly={!isEditing}
            />
          </div>

          <div className="flex justify-end mt-8">
            {isEditing ? (
              <>
                <Button color="gray" variant="text" onClick={handleCancelClick}>
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
    </>
  );
};

export default UserProfile;
