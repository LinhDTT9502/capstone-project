import { getCommentbyId } from "../../api/Comment/apiComment";

export const getComment = async (productId) => {
    try {
      const response = await getCommentbyId(productId);
      return response.data.data.$values;
    } catch (error) {
      console.error('Error fetching comment:', error);
      throw error;
    }
  };