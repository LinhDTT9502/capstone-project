import axios from 'axios';

const API_BASE_URL = 'https://capstone-project-703387227873.asia-southeast1.run.app/api/User';

export const fetchAllUsers = async (token) => {
  try {
    const response = await axios.get(`${API_BASE_URL}/get-all-users`, {
      headers: {
        'Authorization': `Bearer ${token}`,
        'Content-Type': 'application/json'
      }
    });

    // console.log(response.data);
    return response.data.data.$values;
    
  } catch (error) {
    console.error('Error fetching users:', error);
    throw error;
  }
};