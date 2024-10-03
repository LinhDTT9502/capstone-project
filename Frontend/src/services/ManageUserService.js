import { fetchAllUsers as apiFetchAllUsers } from '../api/apiManageUser';
import { toast } from "react-toastify";

export const fetchAllUsers = async (token) => {
  try {
    const users = await apiFetchAllUsers(token);
    console.log(users);
    console.log("hello");
    // toast.success("Users fetched successfully");
    toast.dismiss();
    return users;
  } catch (error) {
    console.error('Error fetching users:', error);
    // toast.error("Error fetching users: " + error.message);
    toast.dismiss();

    throw error;
  }
};

