import axios from 'axios';

const API_BASE_URL = 'https://twosportapiv2.azurewebsites.net/api/Auth';

export const signIn = (userName, password) => {
  return axios.post(`${API_BASE_URL}/sign-in`, {
    userName,
    password,
  }, {
    headers: {
      'accept': '*/*'
    }
  });
};

export const signUp = (userData) => {
  return axios.post(`${API_BASE_URL}/sign-up`, userData, {
    headers: {
      'Content-Type': 'application/json'
    }
  });
};

export const signOut = (data) => {
  return axios.post(`${API_BASE_URL}/sign-out`, data, {
    headers: {
      'Content-Type': 'application/json'
    }
  });
};

export const refreshTokenAPI = (token, refreshToken) => {
  return axios.post(`${API_BASE_URL}/refresh-token`, {
    token,
    refreshToken,
  }, {
    headers: {
      'Content-Type': 'application/json'
    }
  });
};

// const axiosInstance = axios.create();

// axiosInstance.interceptors.request.use(async (config) => {
//   const token = await checkAndRefreshToken();
//   config.headers['Authorization'] = `Bearer ${token}`;
//   return config;
// }, (error) => {
//   return Promise.reject(error);
// });
