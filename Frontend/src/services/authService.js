import {
  refreshTokenAPI,
  signIn,
  signOut,
  mobileSignUp,
  verifyAccountMobileAPI,
  forgotPasswordRequestMobile,
  resetPasswordMobile,
} from "../api/apiAuth";
import { jwtDecode } from "jwt-decode";
import { login, logout } from "../redux/slices/authSlice";
import { toast } from "react-toastify";

export const authenticateUser = async (dispatch, data) => {
  try {
    const response = await signIn(data.userName, data.password);
    localStorage.setItem("token", response.data.data.token);
    localStorage.setItem("refreshToken", response.data.data.refreshToken);
    const decoded = jwtDecode(response.data.data.token);
    dispatch(login(decoded));
    toast.success("Đăng nhập thành công");
    return decoded;
  } catch (error) {
    console.error("Login failed", error);
    toast.error("Đăng nhập thất bại");
    throw error;
  }
};

// export const signUpUser = async (userData) => {
//   try {
//     const response = await signUp(userData);
//     return response.data;
//   } catch (error) {
//     console.error('Error during sign-up:', error);
//     throw error;
//   }
// };

export const signUpUser = async (userData) => {
  try {
    const response = await mobileSignUp(userData);
    return response.data;
  } catch (error) {
    console.error("Error during mobile sign-up:", error);
    throw error;
  }
};

export const verifyAccountMobile = async ({ username, email, OtpCode }) => {
  // console.log("Payload sent to API:", { username, email, otpCode: OtpCode });
  try {
    const response = await verifyAccountMobileAPI({
      username,
      email,
      otpCode: OtpCode,
    });
    // console.log("API Response in verifyAccountMobile:", response.data);
    return response.data;
  } catch (error) {
    console.error(
      "Error in verifyAccountMobile:",
      error.response?.data || error.message
    );
    throw error.response ? error.response.data : error;
  }
};

export const signOutUser = async (data) => {
  try {
    const response = await signOut(data);
    return response;
  } catch (error) {
    console.error("Error during sign-out:", error);
    throw error;
  }
};

export const checkAndRefreshToken = async () => {
  let token = localStorage.getItem("token");
  const refreshToken = localStorage.getItem("refreshToken");

  if (!token || !refreshToken) {
    throw new Error("No token or refresh token found");
  }

  const decoded = jwtDecode(token);
  const currentTime = Date.now() / 1000;

  if (decoded.exp < currentTime) {
    try {
      const response = await refreshTokenAPI(
        token,
        refreshToken,
        decoded.UserId
      );
      const newToken = response.data.data.token;
      const newRefreshToken = response.data.data.refreshToken;
      localStorage.setItem("token", newToken);
      localStorage.setItem("refreshToken", newRefreshToken);
      token = newToken;
    } catch (error) {
      console.error("Token refresh failed", error);
      throw error;
    }
  }

  return token;
};

export const requestPasswordReset = async (email) => {
  try {
    const response = await forgotPasswordRequestMobile(email);
    return response.data;
  } catch (error) {
    console.error("Error requesting password reset:", error);
    throw error.response ? error.response.data : error;
  }
};

export const performPasswordReset = async ({ otpCode, email, newPassword }) => {
  try {
    const response = await resetPasswordMobile({ otpCode, email, newPassword });
    return response.data;
  } catch (error) {
    console.error(
      "Error in performPasswordReset:",
      error.response?.data || error.message
    );
    throw error.response ? error.response.data : error;
  }
};