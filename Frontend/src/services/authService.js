import { refreshTokenAPI, signIn, signOut,signUp } from '../api/apiAuth';
import { jwtDecode } from 'jwt-decode';
import { login, logout } from '../redux/slices/authSlice';
import { toast } from 'react-toastify';
import { employeeLogin } from '../api/apiEmployee';

export const authenticateUser = async (dispatch, data, isEmployee = false) => {
  try {
    let response;

    if (isEmployee) {
      response = await employeeLogin(data.userName, data.password);
      console.log("Employee Login API Response:", response);
    } else {
      response = await signIn(data.userName, data.password);
      console.log("User Login API Response:", response);
    }

    // Check if the API returned a successful response
    if (!response.data.isSuccess) {
      throw new Error(response.data.message || "Invalid API response");
    }

    // Check if the response contains the token and refresh token
    const token = response.data.data.token;
    const refreshToken = response.data.data.refreshToken;

    if (!token || !refreshToken) {
      throw new Error("Missing token in API response");
    }

    // Store the tokens
    localStorage.setItem('token', token);
    localStorage.setItem('refreshToken', refreshToken);

    // Decode the token to extract user details
    const decoded = jwtDecode(token);
    dispatch(login(decoded));

    return decoded;
  } catch (error) {
    console.error('Login failed:', error);
    throw error;
  }
};


export const signUpUser = async (userData) => {
  try {
    const response = await signUp(userData);
    return response.data;
  } catch (error) {
    console.error('Error during sign-up:', error);
    throw error;
  }
};

export const signOutUser = async (data) => {
  try {
    const response = await signOut(data);
    return response;
  } catch (error) {
    console.error('Error during sign-out:', error);
    throw error;
  }
};


export const checkAndRefreshToken = async () => {
  let token = localStorage.getItem('token');
  const refreshToken = localStorage.getItem('refreshToken');

  if (!token || !refreshToken) {
    throw new Error('No token or refresh token found');
  }

  const decoded = jwtDecode(token);
  const currentTime = Date.now() / 1000;

  if (decoded.exp < currentTime) {
    try {
      const response = await refreshTokenAPI(token, refreshToken);
      // console.log(response);
      const newToken = response.data.data.token;
      const newRefreshToken = response.data.data.refreshToken;
      localStorage.setItem('token', newToken);
      localStorage.setItem('refreshToken', newRefreshToken);
      token = newToken; 
    } catch (error) {
      console.error('Token refresh failed', error);
      throw error;
    }
  }
  
  return token;
};