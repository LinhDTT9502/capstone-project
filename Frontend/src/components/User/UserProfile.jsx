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
  faGem,
  faCrown,
} from "@fortawesome/free-solid-svg-icons";
import { FontAwesomeIcon } from "@fortawesome/react-fontawesome";
import AuthInfo from "./AuthInfo";
import { fetchUserProfile } from "../../services/ManageUserService";
import AvatarUpload from "./AvatarUpload";
import axios from "axios";

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
  const [email, setEmail] = useState("");
  const [emailConfirmed, setEmailConfirmed] = useState(false);
  const [fullName, setFullName] = useState("");
  const [userData, setUserData] = useState(null);

  const [formData, setFormData] = useState({
    UserName: user.UserName,
    FullName: user.FullName,
    Gender: user.Gender || null,
    Address: user.Address || null,
    BirthDate: user.DOB ? user.DOB.split("T")[0] : "",
  });

  const fetchUserData = async () => {
    try {
      setLoading(true);
      const response = await axios.get(
        `https://twosport-api-offcial-685025377967.asia-southeast1.run.app//api/User/get-users-detail?userId=${user.UserId}`,
        { headers: { accept: "*/*" } }
      );
      if (response.data.user.isSuccess) {
        const data = response.data.user.data;
        setUserData(data);

        const {
          phoneNumber,
          phoneNumberConfirmed,
          imgAvatarPath,
          fullName,
          gender,
          address,
          dob,
          email,
          emailConfirmed,
        } = data;
        setPhone(phoneNumber || "");
        setPhoneNumberConfirmed(phoneNumberConfirmed || false);
        setAvatar(imgAvatarPath || "/assets/images/default-avatar.jpg");
        setEmail(email);
        setEmailConfirmed(emailConfirmed);
        setFullName(fullName);
        setFormData((prev) => ({
          ...prev,
          FullName: fullName || prev.FullName,
          Gender: gender || prev.Gender,
          Address: address || prev.Address,
          BirthDate: dob || prev.BirthDate,
        }));
      }
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
        userName: user.UserName,
        fullName: user.FullName,
        gender: user.Gender,
        address: user.Address,
        birthDate: user.BirthDate,
      })
    ) {
      toast.warn(t("user_profile.no_changes"));
      return;
    }
    var response = updateProfile(user.UserId, formData)
      .then(() => {
        setIsEditing(false);
        toast.success("Cập nhật hồ sơ thành công");
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
      BirthDate: user.BirthDate
        ? convertToISODate(user.BirthDate)
        : formData.BirthDate || "",
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
    dispatch(updateUser({ ...user, Email: newEmail }));
    await fetchUserData();
  };

  const handleVerifyPhone = async (newPhone) => {
    setPhone(newPhone);
    setPhoneNumberConfirmed(true);
    await fetchUserData();
  };

  const convertToISODate = (date) => {
    if (!date) return "";
    const d = new Date(date);

    // Lấy ngày/tháng/năm theo múi giờ local mà không bị ảnh hưởng bởi UTC
    const year = d.getFullYear();
    const month = String(d.getMonth() + 1).padStart(2, "0"); // Tháng bắt đầu từ 0
    const day = String(d.getDate()).padStart(2, "0");

    return `${year}-${month}-${day}`; // Trả về 'YYYY-MM-DD'
  };
  // Hàm chuyển đổi ngược lại 'YYYY-MM-DD' thành 'YYYY-MM-DDTHH:MM:SS.MMM'
  const convertToFullISODate = (date) => {
    if (!date) return "";
    const d = new Date(date);
    return d.toISOString(); // Trả về 'YYYY-MM-DDTHH:MM:SS.MMM'
  };

  const getMembershipStyles = (membershipLevel) => {
    switch (membershipLevel) {
      case "Gold_Member":
        return {
          label: "Thành viên vàng",
          textColor: "#FFD700", // Vàng
          bgColor: "#FFF8DC", // Màu nền nhạt
          icon: faGem,
        };
      case "Silver_Member":
        return {
          label: "Thành viên bạc",
          textColor: "#C0C0C0", // Bạc
          bgColor: "#F5F5F5", // Màu nền nhạt
          icon: faGem,
        };
      case "Diamond_Member":
        return {
          label: "Thành viên kim cương",
          textColor: "#1E90FF", // Xanh ngọc
          bgColor: "#E6F7FF", // Màu nền nhạt
          icon: faGem,
        };
      default:
        return {
          label: "Thành viên đồng",
          textColor: "#CD7F32", // Đồng
          bgColor: "#FDF5E6", // Màu nền nhạt
          icon: faGem,
        };
    }
  };

  if (loading) {
    return <div>Loading...</div>;
  }

  return (
    <>
      <div className="container mx-auto pt-2 rounded-lg max-w-4xl">
        <h2 className="text-orange-500 font-bold text-2xl mb-6">
          {t("user_profile.user_profile")}
        </h2>
        <div className="space-y-6">
          <div className="flex items-center space-x-6 mb-8 bg-white p-6 rounded-lg shadow-md">
            <div className="relative">
              <img
                src={avatar}
                alt="Avatar"
                className="w-24 h-24 rounded-full object-cover border-4 border-orange-500 shadow-lg"
                onError={(e) =>
                  (e.target.src = "/assets/images/default-avatar.jpg")
                }
              />
              <div className="absolute -bottom-2 -right-2">
                <AvatarUpload
                  userId={user.UserId}
                  onAvatarChange={handleAvatarChange}
                  imgAvatarPath={avatar}
                  setAvatar={setAvatar}
                  fetchUserData={fetchUserData}
                />
              </div>
            </div>

            <div className="h-24 border-l-2 border-gray-200 mx-4"></div>

            <div className="flex-grow">
              <div className="flex items-center justify-between mb-2">
                <h3 className="text-2xl font-bold text-gray-800">
                  {userData.fullName}
                </h3>
                {userData.customerDetail && (
                  <span
                    style={{
                      backgroundColor: getMembershipStyles(
                        userData.customerDetail.membershipLevel
                      ).bgColor,
                      color: getMembershipStyles(
                        userData.customerDetail.membershipLevel
                      ).textColor,
                    }}
                    className="flex items-center px-4 py-2 rounded-full shadow-md transition-all duration-300 hover:shadow-lg"
                  >
                    <FontAwesomeIcon
                      icon={
                        getMembershipStyles(
                          userData.customerDetail.membershipLevel
                        ).icon
                      }
                      className="mr-2 text-lg"
                    />
                    <span className="text-sm font-medium">
                      {
                        getMembershipStyles(
                          userData.customerDetail.membershipLevel
                        ).label
                      }
                    </span>
                  </span>
                )}
              </div>

              {userData.customerDetail && (
                <div className="text-gray-600 font-medium">
                  <div className="flex items-center">
                    <FontAwesomeIcon
                      icon={faCrown}
                      className="mr-2 text-yellow-500"
                    />
                    <span>
                      Điểm tích lũy: {userData.customerDetail.loyaltyPoints}
                    </span>
                  </div>
                </div>
              )}
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
                    <option value="Nam">{t("user_profile.gender_male")}</option>
                    <option value="Nữ">
                      {t("user_profile.gender_female")}
                    </option>
                    <option value="Khác">
                      {t("user_profile.gender_other")}
                    </option>
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
                  value={
                    formData.Gender === "Nam"
                      ? t("user_profile.gender_male")
                      : formData.Gender === "Nữ"
                      ? t("user_profile.gender_female")
                      : t("user_profile.gender_other")
                  }
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
                value={
                  formData.BirthDate ? convertToISODate(formData.BirthDate) : ""
                }
                onChange={(e) =>
                  setFormData({
                    ...formData,
                    BirthDate: convertToFullISODate(e.target.value), // Chuyển đổi ngày khi người dùng thay đổi
                  })
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
              email={email}
              phone={phone}
              emailConfirmed={emailConfirmed}
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
