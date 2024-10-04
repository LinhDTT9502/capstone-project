import { getAllSports } from "../api/apiSport";

export const fetchSports = async () => {
  try {
    const response = await getAllSports();
    return response.data.data.$values;
  } catch (error) {
    console.error('Error fetching brand data:', error);
    throw error;
  }
};