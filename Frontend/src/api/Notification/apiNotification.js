import axios from 'axios';

const API_BASE_URL = 'https://capstone-project-703387227873.asia-southeast1.run.app/api/Notification';

export const getNotibyUser = (userId, token) => {
    return axios.get(`${API_BASE_URL}/get-by-user/${userId}`, {
        headers: {
            'accept': '*/*',
            'Authorization': `Bearer ${token}`,

        }
    });
};