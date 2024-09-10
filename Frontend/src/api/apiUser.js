import axios from 'axios';

const API_BASE_URL = 'https://2sportapi-c6ajcce3ezh5h4gw.southeastasia-01.azurewebsites.net/api/User';

// GET all users
export const getAllUsers = () => {
  return axios.get(`${API_BASE_URL}/get-all-users`);
};

// GET search for users
export const searchUsers = (query) => {
  return axios.get(`${API_BASE_URL}/search`, {
    params: { query },
  });
};

// GET user details
export const getUserDetails = (userId) => {
  return axios.get(`${API_BASE_URL}/get-users-detail/${userId}`);
};

// GET user profile
export const getUserProfile = (userId) => {
  return axios.get(`${API_BASE_URL}/get-profile/${userId}`);
};

// POST create new user
export const createUser = (userData) => {
  return axios.post(`${API_BASE_URL}/create-user`, userData, {
    headers: {
      'Content-Type': 'application/json',
    },
  });
};

// PUT update user information
export const updateUser = (userId, updatedData) => {
  return axios.put(`${API_BASE_URL}/update-user/${userId}`, updatedData, {
    headers: {
      'Content-Type': 'application/json',
    },
  });
};

// PUT update user profile
export const updateProfile = (userId, profileData) => {
  return axios.put(`${API_BASE_URL}/update-profile?id=${userId}`, profileData, {
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
  return axios.put(`${API_BASE_URL}/change-status-user/${userId}`, statusData, {
    headers: {
      'Content-Type': 'application/json',
    },
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
