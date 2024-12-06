import { getLikesAPI, toggleLikeProductAPI } from "../api/apiLike";

const getToken = () => {
  try {
    const token = localStorage.getItem("token");
    if (!token) {
      console.warn("Token not found");
      return null;
    }
    return token;
  } catch (error) {
    console.error("Error retrieving token:", error);
    return null;
  }
};

// Hàm lấy số lượt like của sản phẩm
export const fetchLikes = async () => {
  try {
    const response = await getLikesAPI(); 
    return response.data; 
  } catch (error) {
    console.error("Error fetching likes:", error);
    throw error; 
  }
};

export const handleToggleLike = async (productId) => {
  try {
    const token = await getToken();
    if (!token) {
      console.warn("Cannot like product. User is not logged in.");
      return;
    }
    const response = await toggleLikeProductAPI(productId, token); 
    return response.data; 
  } catch (error) {
    console.error("Error toggling like:", error);
    throw error;
  }
};
