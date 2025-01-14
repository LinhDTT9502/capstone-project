import axios from 'axios';

const API_BASE_URL = 'https://twosport-api-offcial-685025377967.asia-southeast1.run.app/api/Chat';

export const getListChat = () => {
    return axios.get(`${API_BASE_URL}/get-all-chat-sessions-of-coordinator`, {
        headers: {
            'accept': '*/*',
            "Authorization": `Bearer ${localStorage.getItem("token")}`,
            "Content-Type": "application/json",
        }
    });
};

export const getCustomerChat = () => {
    return axios.get(`${API_BASE_URL}/get-messages-of-customer`, {
        headers: {
            'accept': '*/*',
            "Authorization": `Bearer ${localStorage.getItem("token")}`,
            "Content-Type": "application/json",
        }
    });
};