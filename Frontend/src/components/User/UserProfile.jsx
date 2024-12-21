import React, { useState, useEffect } from "react";
import { useSelector, useDispatch } from "react-redux";
import { Button } from "@material-tailwind/react";
import { selectUser, updateUser } from "../../redux/slices/authSlice";
import { useTranslation } from "react-i18next";
import { updateProfile } from "../../api/apiUser";
import { toast, ToastContainer } from "react-toastify";
import "react-toastify/dist/ReactToastify.css";
import {
  faUser,
  faCaretDown,
  faVenusMars,
  faMapMarkerAlt,
  faBirthdayCake,
} from "@fortawesome/free-solid-svg-icons";
import { FontAwesomeIcon } from "@fortawesome/react-fontawesome";
import AuthInfo from "./AuthInfo";
import { fetchUserProfile } from "../../services/ManageUserService";
import AvatarUpload from "./AvatarUpload";

const UserProfile = () => {
  const user = useSelector(selectUser);
  const dispatch = useDispatch();
  const { t } = useTranslation();
  const [loading, setLoading] = useState(true);
  const [isEditing, setIsEditing] = useState(false);
  const [avatar, setAvatar] = useState(
    user?.imgAvatarPath || "/assets/images/default-avatar.jpg"
  );
  const [phone, setPhone] = useState("");
  const [phoneNumberConfirmed, setPhoneNumberConfirmed] = useState(false);
  const [formData, setFormData] = useState({
    UserName: user.UserName,
    FullName: user.FullName,
    Gender: user.Gender || null,
    Address: user.Address || null,
    BirthDate: user.BirthDate ? user.BirthDate.split("T")[0] : "",
    EmailConfirmed: user.EmailConfirmed || false,
  });

  const fetchUserData = async () => {
    try {
      setLoading(true);
      const response = await fetchUserProfile(user.UserId);
      console.log("Full Response:", response);
      const { phoneNumber, phoneNumberConfirmed, imgAvatarPath } = response;
      setPhone(phoneNumber || "");
      setPhoneNumberConfirmed(phoneNumberConfirmed || false);
      setAvatar(imgAvatarPath || "/assets/images/default-avatar.jpg");
      setFormData(prev => ({
        ...prev,
        EmailConfirmed: response.emailConfirmed || false,
      }));
    } catch (error) {
      console.error("Error fetching user profile:", error);
      toast.error(t("user_profile.fetch_failed"));
    } finally {
      setLoading(false);
    }
  };

  useEffect(() => {
    fetchUserData();
  }, [user.UserId]);

  const handleEditClick = () => setIsEditing(true);

  const handleSaveClick = () => {
    if (
      JSON.stringify(formData) ===
      JSON.stringify({
        UserName: user.UserName,
        FullName: user.FullName,
        Gender: user.Gender,
        Address: user.Address,
        BirthDate: user.BirthDate,
        EmailConfirmed: user.EmailConfirmed || false,
      })
    ) {
      toast.warn(t("user_profile.no_changes"));
      return;
    }

    updateProfile(user.UserId, formData)
      .then(() => {
        setIsEditing(false);
        toast.success(t("user_profile.update_success"));
        dispatch(updateUser({ ...user, ...formData }));
        fetchUserData(); 
      })
      .catch(() => toast.error(t("user_profile.save_failed")));
  };

  const handleCancelClick = () => {
    setFormData({
      UserName: user.UserName,
      FullName: user.FullName,
      Gender: user.Gender || null,
      Address: user.Address || null,
      BirthDate: user.BirthDate ? user.BirthDate.split("T")[0] : "",
      EmailConfirmed: user.EmailConfirmed || false,
    });
    setIsEditing(false);
  };

  const handleChange = (e) => {
    const { name, value } = e.target;
    setFormData((prev) => ({ ...prev, [name]: value }));
  };

  const handleAvatarChange = async (newAvatarPath) => {
    setAvatar(newAvatarPath);
    await fetchUserData(); 
  };

  const handleVerifyEmail = async (newEmail) => {
    dispatch(updateUser({ ...user, Email: newEmail, EmailConfirmed: true }));
    setFormData(prev => ({ ...prev, EmailConfirmed: true }));
    await fetchUserData();
  };

  const handleVerifyPhone = async (newPhone) => {
    setPhone(newPhone);
    setPhoneNumberConfirmed(true);
    await fetchUserData(); 
  };

  if (loading) {
    return <div>Loading...</div>;
  }

  return (
    <>
      <ToastContainer />
      <div className="container mx-auto pt-2 rounded-lg max-w-4xl">
        <h2 className="text-orange-500 font-bold text-2xl mb-6">
          {t("user_profile.user_profile")}
        </h2>

        <div className="space-y-6">
          <div className="flex items-center space-x-4 mb-6">
            <div className="relative">
              <img
                src={avatar}
                alt="Avatar"
                className="w-20 h-20 rounded-full object-cover border-2 border-orange-500"
                onError={(e) => (e.target.src = "/assets/images/default-avatar.jpg")}
              />
              <div className="absolute bottom-0 right-0">
              <AvatarUpload
                  userId={user.UserId}
                  onAvatarChange={handleAvatarChange}
                  imgAvatarPath={avatar}
                  setAvatar={setAvatar}
                  fetchUserData={fetchUserData}
                />
              </div>
            </div>
            <div>
              <h3 className="text-xl font-semibold">{formData.FullName}</h3>
            </div>
          </div>

          <div className="grid grid-cols-1 sm:grid-cols-2 md:grid-cols-2 gap-6">
            <div className="relative">
              <FontAwesomeIcon
                icon={faUser}
                className="absolute left-4 top-10 text-gray-500"
              />
              <label className="block text-gray-700">
                {t("user_profile.username")}:
              </label>
              <input
                type="text"
                name="UserName"
                className="w-full p-3 pl-12 bg-gray-100 text-gray-500 cursor-not-allowed"
                value={formData.UserName}
                readOnly
              />
            </div>
            <div className="relative">
              <FontAwesomeIcon
                icon={faUser}
                className="absolute left-4 top-10 text-gray-500"
              />
              <label className="block text-gray-700">
                {t("user_profile.fullname")}:
              </label>
              <input
                type="text"
                name="FullName"
                className={`w-full p-3 pl-12 ${
                  isEditing
                    ? "border border-gray-300"
                    : "bg-gray-100 text-gray-500 cursor-not-allowed"
                }`}
                value={formData.FullName}
                onChange={handleChange}
                readOnly={!isEditing}
              />
            </div>
          </div>

          <div className="grid grid-cols-1 sm:grid-cols-2 md:grid-cols-2 gap-6">
          <div className="relative">
  <FontAwesomeIcon
    icon={faVenusMars}
    className="absolute left-4 top-10 text-gray-500"
  />
  <label className="block text-gray-700">
    {t("user_profile.gender")}:
  </label>
  {isEditing ? (
    <>
      <select
        name="Gender"
        className="w-full p-3 pl-12 pr-10 border border-gray-300 appearance-none"
        value={formData.Gender}
        onChange={handleChange}
      >
        <option value="Male">{t("user_profile.gender_male")}</option>
        <option value="Female">{t("user_profile.gender_female")}</option>
        <option value="Other">{t("user_profile.gender_other")}</option>
      </select>
      <FontAwesomeIcon
        icon={faCaretDown}
        className="absolute right-3 top-12 transform -translate-y-1/2 text-gray-500"
      />
    </>
  ) : (
    <input
      type="text"
      name="Gender"
      className="w-full p-3 pl-12 bg-gray-100 text-gray-500 cursor-not-allowed"
      value={formData.Gender === "male" ? t("user_profile.gender_male") : formData.Gender === "female" ? t("user_profile.gender_female") : t("user_profile.gender_other")}
      readOnly
    />
  )}
</div>
            <div className="relative">
              <FontAwesomeIcon
                icon={faMapMarkerAlt}
                className="absolute left-4 top-10 text-gray-500"
              />
              <label className="block text-gray-700">
                {t("user_profile.address")}:
              </label>
              <input
                type="text"
                name="Address"
                className={`w-full p-3 pl-12 ${
                  isEditing
                    ? "border border-gray-300"
                    : "bg-gray-100 text-gray-500 cursor-not-allowed"
                }`}
                value={formData.Address}
                onChange={handleChange}
                readOnly={!isEditing}
              />
            </div>
            <div className="relative">
              <FontAwesomeIcon
                icon={faBirthdayCake}
                className="absolute left-4 top-10 text-gray-500"
              />
              <label className="block text-gray-700">
                {t("user_profile.birthDate")}:
              </label>
              <input
                type="date"
                name="BirthDate"
                value={formData.BirthDate}
                onChange={(e) =>
                  setFormData({ ...formData, BirthDate: e.target.value })
                }
                readOnly={!isEditing}
                className={`w-full p-3 pl-12 ${
                  isEditing
                    ? "border border-gray-300"
                    : "bg-gray-100 text-gray-500 cursor-not-allowed"
                }`}
              />
            </div>
          </div>

          <div className="flex justify-end mb-4">
            {isEditing ? (
              <>
                <Button color="gray" variant="text" onClick={handleCancelClick}>
                  {t("user_profile.cancel")}
                </Button>
                <Button
                  color="orange"
                  variant="filled"
                  onClick={handleSaveClick}
                >
                  {t("user_profile.save_changes")}
                </Button>
              </>
            ) : (
              <Button color="orange" variant="filled" onClick={handleEditClick}>
                {t("user_profile.edit_profile")}
              </Button>
            )}
          </div>

          <div className="mt-6">
            <h3 className="text-xl font-semibold text-orange-500">
              Thông tin xác thực
            </h3>
            <AuthInfo
              email={user.Email}
              phone={phone}
              emailConfirmed={formData.EmailConfirmed}
              phoneNumberConfirmed={phoneNumberConfirmed}
              userId={user.UserId}
              onVerifyEmail={handleVerifyEmail}
              onVerifyPhone={handleVerifyPhone}
            />
          </div>
        </div>
      </div>
    </>
  );
};

export default UserProfile;
