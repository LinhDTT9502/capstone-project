import axios from 'axios';

const API_BASE_URL = 'https://twosportapiv2.azurewebsites.net/api/User';

export const fetchAllUsers = async (token) => {
  try {
    const response = await axios.get(`${API_BASE_URL}/get-all-users`, {
      headers: {
        'Authorization': `Bearer ${token}`,
        'Content-Type': 'application/json'
      }
    });
    return response.data.$values;
  } catch (error) {
    console.error('Error fetching users:', error);
    throw error;
  }
};