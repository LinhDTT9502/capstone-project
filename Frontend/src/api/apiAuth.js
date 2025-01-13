import axios from 'axios';

const API_BASE_URL = 'https://twosport-api-offcial-685025377967.asia-southeast1.run.app//api/Auth';

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

// export const signUp = (userData) => {
//   return axios.post(`${API_BASE_URL}/sign-up`, userData, {
//     headers: {
//       'Content-Type': 'application/json'
//     }
//   });
// };

export const mobileSignUp = (userData) => {
  return axios.post(`${API_BASE_URL}/sign-up-mobile`, userData, {
    headers: {
      "Content-Type": "application/json",
    },
  });
};


export const signOut = (data) => {
  return axios.post(`${API_BASE_URL}/sign-out`, data, {
    headers: {
      'Content-Type': 'application/json'
    }
  });
};

export const refreshTokenAPI = (token, refreshToken, userId) => {
  return axios.post(`${API_BASE_URL}/refresh-token`, {
    token,
    refreshToken,
    userId
  }, {
    headers: {
      'Content-Type': 'application/json'
    }
  });
};

export const changePassword = (data) => {
  return axios.post(`${API_BASE_URL}/change-password`, data, {
    headers: {
      'Content-Type': 'application/json'
    }
  });
};

export const resetPassword = (data) => {
  return axios.post(`${API_BASE_URL}/reset-password`, data, {
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

export const verifyAccountMobileAPI = async ({ username, email, otpCode }) => {
  return axios.post(
    `${API_BASE_URL}/verify-account-mobile`,
    { username, email, otpCode },
    {
      headers: {
        "Content-Type": "application/json",
      },
    }
  );
};

// for forgot password only
export const forgotPasswordRequestMobile = (email) => {
  return axios.post(
    `${API_BASE_URL}/forgot-password-request-mobile`,
    { email },
    {
      headers: {
        "Content-Type": "application/json",
      },
    }
  );
};

export const resetPasswordMobile = ({ otpCode, email, newPassword }) => {
  return axios.post(
    `${API_BASE_URL}/reset-password-mobile`,
    {
      otpCode,
      email,
      newPassword,
    },
    {
      headers: {
        "Content-Type": "application/json",
      },
    }
  );
};
