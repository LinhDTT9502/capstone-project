import { fetchAllUsers as apiFetchAllUsers } from "../api/apiManageUser";
import {
  updatePassword as apiUpdatePassword,
  changeEmail,
  sendSmsOtpApi,
  editPhoneNumberApi,
  sendOtpForEmailChange,
} from "../api/apiUser";
import { toast } from "react-toastify";
import { getUserProfile as getUserProfile } from "../api/apiUser";

export const fetchAllUsers = async (token) => {
  try {
    const users = await apiFetchAllUsers(token);

    // toast.success("Users fetched successfully");
    toast.dismiss();
    return users;
  } catch (error) {
    console.error("Error fetching users:", error);
    // toast.error("Error fetching users: " + error.message);
    toast.dismiss();

    throw error;
  }
};

export const fetchUserProfile = async (userId) => {
  try {
    const response = await getUserProfile(userId);
    return response.data.data;
  } catch (error) {
    console.error("Error fetching user profile:", error);
    throw error;
  }
};

export const updatePassword =
  (userId, oldPassword, newPassword) => async (dispatch) => {
    try {
      const response = await apiUpdatePassword(
        userId,
        oldPassword,
        newPassword
      );

      return response.data;
    } catch (error) {
      throw error;
    }
  };

export const sendSmsOtp = async (phoneNumber, token) => {
  try {
    const response = await sendSmsOtpApi(phoneNumber, token);
    toast.success("OTP đã được gửi thành công!");
    return response.data;
  } catch (error) {
    throw error;
  }
};

export const editPhoneNumberService = async (newPhoneNumber, otp) => {
  try {
    const response = await editPhoneNumberApi(newPhoneNumber, otp);
    return response.data;
  } catch (error) {
    console.error("Error updating phone number:", error);
    throw new Error("Không thể cập nhật số điện thoại.");
  }
};

// Send OTP for email change
export const sendOtpForEmailChangeService = async (userId, email) => {
  console.log(email);
  try {
    const response = await sendOtpForEmailChange(userId, email);
    // console.log('check', response)
    return response.data;
  } catch (error) {
    console.error("Error sending OTP for email change:", error);
    throw new Error("Lỗi gửi OTP thay đổi email");
  }
};

// Change email
export const changeEmailService = async (userId, token, email, otp) => {
  try {
    console.log(userId, token, email, otp);
    const response = await changeEmail(userId, token, email, otp);
    return response.data;
  } catch (error) {
    console.error("Error changing email:", error);
    throw new Error("Lỗi thay đổi email");
  }
};
