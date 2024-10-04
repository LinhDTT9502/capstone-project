import axios from 'axios';
import axiosInstance from './axiosInstance';

const API_BASE_URL = 'https://twosportapi-295683427295.asia-southeast2.run.app/api/Employee';

export const employeeLogin = async (userName, password) => {
  try {
    const response = await axiosInstance.post(`${API_BASE_URL}/log-in`, {
      userName,
      password 
    }, {
      headers: {
        'accept': '*/*',
        'Content-Type': 'application/json',
      }
    });

    console.log("Employee login response:", response);
    return response;
  } catch (error) {
    console.error("Employee login failed:", error);
    throw error;
  }
};

export const employeeLogout = (data) => {
  return axiosInstance.post(`${API_BASE_URL}/log-out`, data, {
    headers: {
      'accept': '*/*',
      'Content-Type': 'application/json',
    }
  });
};

export const getEmployeeProfile = () => {
  return axios.get(`${API_BASE_URL}/get-profile`, {
    headers: {
      'accept': '*/*',
    }
  });
};

export const updateEmployeePassword = (data) => {
  return axios.put(`${API_BASE_URL}/update-password`, data, {
    headers: {
      'accept': '*/*',
      'Content-Type': 'application/json',
    }
  });
};
