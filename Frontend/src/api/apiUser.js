import axios from 'axios';
import axiosInstance from './axiosInstance';

const API_BASE_URL = 'https://twosportapi-295683427295.asia-southeast2.run.app/api/User';

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

// GET user details
export const getUserDetails = (userId) => {
  return axiosInstance.get(`${API_BASE_URL}/get-users-detail?userId=${userId}`);
};

// GET user profile
export const getUserProfile = (userId) => {
  return axios.get(`${API_BASE_URL}/get-profile/${userId}`);
};

// POST create new user
export const createUser = (userData) => {
  return axiosInstance.post(`${API_BASE_URL}/create-user`, userData, {
    headers: {
      'Content-Type': 'application/json',
    },
  });
};

// PUT update user information
export const updateUser = async (userId, updatedData) => {
  try {
    const response = await axiosInstance.put(`${API_BASE_URL}/update-user?id=${userId}`, updatedData, {
      headers: {
        'Content-Type': 'application/json',
      },
    });
    return response.data;
  } catch (error) {
    console.error('Error updating user:', error.response?.data || error.message);
    throw error;
  }
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

// PUT change user status 
export const changeUserStatus = (userId, statusData) => {
  return axios.put(`${API_BASE_URL}/change-status-user`, null, {
    params: {
      id: userId
    },
    headers: {
      'Content-Type': 'application/json',
    },
    data: statusData,
  });
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
