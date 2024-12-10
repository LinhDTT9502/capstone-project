import { fetchAllUsers as apiFetchAllUsers,  } from '../api/apiManageUser';
import { updatePassword as apiUpdatePassword, changeEmail, sendOtpForEmailChange} from '../api/apiUser'
import { toast } from "react-toastify";

export const fetchAllUsers = async (token) => {
  try {
    const users = await apiFetchAllUsers(token);
    console.log(users); 
    console.log("hello");
    // toast.success("Users fetched successfully");
    toast.dismiss();
    return users;
  } catch (error) {
    console.error('Error fetching users:', error);
    // toast.error("Error fetching users: " + error.message);
    toast.dismiss();

    throw error;
  }
};

export const updatePassword = (userId, oldPassword, newPassword) => async (dispatch) => {
  try {
    const response = await apiUpdatePassword(userId, oldPassword, newPassword); 

    return response.data;
  } catch (error) {
    throw error;
  }
};


export const sendSmsOtpService = async (phoneNumber) => {
  try {
    const response = await sendSmsOtp(phoneNumber);
    return response.data;
  } catch (error) {
    console.error("Error sending SMS OTP:", error);
    throw new Error("Lỗi gửi OTP đến số điện thoại");
  }
};

export const verifyPhoneNumberService = async (otp) => {
  try {
    const response = await verifyPhoneNumber(otp);
    return response.data;
  } catch (error) {
    console.error("Error verifying phone number:", error);
    throw new Error("Lỗi xác thực số điện thoại bằng OTP");
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
    console.log(userId, token, email, otp)
    const response = await changeEmail(userId, token, email, otp);
    return response.data;
  } catch (error) {
    console.error("Error changing email:", error);
    throw new Error("Lỗi thay đổi email");
  }
};

