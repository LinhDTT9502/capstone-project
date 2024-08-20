import { fetchAllUsers as apiFetchAllUsers } from '../api/apiManageUser';
import { toast } from "react-toastify";

export const fetchAllUsers = async (token) => {
  try {
    const users = await apiFetchAllUsers(token);
    toast.success("Users fetched successfully");
    return users;
  } catch (error) {
    console.error('Error fetching users:', error);
    toast.error("Error fetching users: " + error.message);
    throw error;
  }
};

