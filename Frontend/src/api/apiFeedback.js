import axios from 'axios';

const API_BASE_URL = 'https://twosport-api-offcial-685025377967.asia-southeast1.run.app/api/Feedback';

export const sendFeedbackApi = (feedbackData) => {
    return axios.post(`${API_BASE_URL}/send-feedback`, feedbackData, {
      headers: {
        'Content-Type': 'application/json'
      }
    });
  };