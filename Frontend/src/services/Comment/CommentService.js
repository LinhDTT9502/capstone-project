import { getCommentbyProductCode } from "../../api/Comment/apiComment";

export const getComment = async (productCode) => {
    try {
      const response = await getCommentbyProductCode(productCode);
      return response.data.data.$values;
    } catch (error) {
      console.error('Error fetching comment:', error);
      throw error;
    }
  };