import axios from 'axios';
import axiosInstance from './axiosInstance';

const API_BASE_URL = 'https://capstone-project-703387227873.asia-southeast1.run.app/api/User';

// GET all users
export const getAllUsers = () => {
  return axios.get(`${API_BASE_URL}/get-all-users`);
};

// GET search for users
export const searchUsers = (fullName, username) => {
  return axios.get(`${API_BASE_URL}/search`, {
    params: {
      ...(fullName && { fullName }),
      ...(username && { username }),
    },
  });
};



// GET user profile
export const getUserProfile = (userId) => {
  return axios.get(`${API_BASE_URL}/get-profile/${userId}`);
};





// PUT update user profile
export const updateProfile = (userId, profileData) => {
  return axiosInstance.put(`${API_BASE_URL}/update-profile?id=${userId}`, profileData, {
    headers: {
      'Content-Type': 'application/json',
    },
  });
};

// DELETE user
export const deleteUser = (userId) => {
  return axios.delete(`${API_BASE_URL}/delete-user/${userId}`);
};



// POST send verification email
export const sendVerificationEmail = (email) => {
  return axios.post(`${API_BASE_URL}/send-verification-email`, { email }, {
    headers: {
      'Content-Type': 'application/json',
    },
  });
};

// GET verify email
export const verifyEmail = (token) => {
  return axios.get(`${API_BASE_URL}/verify-email`, {
    params: { token },
  });
};
