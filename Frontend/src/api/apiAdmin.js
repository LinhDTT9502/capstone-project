import axios from "axios";
import axiosInstance from "./axiosInstance";

const API_ADMIN_BASE_URL =
  "https://capstone-project-703387227873.asia-southeast1.run.app/api/Admin";

// Admin API Endpoints

// Get user details by ID
export const getUserDetail = (id) => {
  return axiosInstance.get(`${API_ADMIN_BASE_URL}/get-user-detail`, {
    params: { id },
    headers: {
      Authorization: `Bearer ${localStorage.getItem("token")}`,
      "Content-Type": "application/json",
    },
  });
};

// change change user status
export const changeUserStatus = (userId, statusData) => {
    return axiosInstance.put(`${API_ADMIN_BASE_URL}/change-status-user`, null, {
      params: { id: userId },
      headers: {
        "Content-Type": "application/json",
      },
      data: statusData,
    });
  };

// Admin Dashboard (example)
export const getAdminDashboard = () => {
  return axios.get(`${API_ADMIN_BASE_URL}/dashboard`, {
    headers: {
      Authorization: `Bearer ${localStorage.getItem("token")}`,
      "Content-Type": "application/json",
    },
  });
};

// Create Admin User
export const createAdminUser = (adminData) => {
  return axiosInstance.post(`${API_ADMIN_BASE_URL}/create-user`, adminData, {
    headers: {
      Authorization: `Bearer ${localStorage.getItem("token")}`,
      "Content-Type": "application/json",
    },
  });
};
// PUT update user information
export const updateUser = async (userId, updatedData) => {
  try {
    const response = await axiosInstance.put(
      `${API_BASE_URL}/update-user?id=${userId}`,
      updatedData,
      {
        headers: {
          "Content-Type": "application/json",
        },
      }
    );
    return response.data;
  } catch (error) {
    console.error(
      "Error updating user:",
      error.response?.data || error.message
    );
    throw error;
  }
};

// Get user details 
export const getUserDetails = (userId) => {
  return axiosInstance.get(`${API_ADMIN_BASE_URL}/get-user-detail`, {
    params: { id: userId }, // Passing the userId as a query parameter 'id'
    headers: {
      "Content-Type": "application/json",
      Authorization: `Bearer ${localStorage.getItem("token")}`, 
    },
  });
};

// Update Admin User
export const updateAdminUser = (userId, updatedData) => {
  return axios.put(`${API_ADMIN_BASE_URL}/update-user/${userId}`, updatedData, {
    headers: {
      Authorization: `Bearer ${localStorage.getItem("token")}`,
      "Content-Type": "application/json",
    },
  });
};

// Delete Admin User
export const deleteAdminUser = (userId) => {
  return axios.delete(`${API_ADMIN_BASE_URL}/delete-user/${userId}`, {
    headers: {
      Authorization: `Bearer ${localStorage.getItem("token")}`,
      "Content-Type": "application/json",
    },
  });
};
