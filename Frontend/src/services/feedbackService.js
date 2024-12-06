import { toast } from "react-toastify";
import { sendFeedbackApi } from "../api/apiFeedback";

export const sendFeedback = async (feedbackData) => {
  try {
    const response = await sendFeedbackApi(feedbackData);
    return response;
  } catch (error) {
    toast.error("Error sending feedback: " + error.message);
    throw error;
  }
};
