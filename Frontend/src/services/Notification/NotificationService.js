import { getNotibyUser } from "../../api/Notification/apiNotification";

export const getNoti = async (userId, token) => {
    try {
      const response = await getNotibyUser(userId, token);
      return response.data.data.$values;
    } catch (error) {
      console.error('Error fetching noti:', error);
      throw error;
    }
  };