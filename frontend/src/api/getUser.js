import axios from "axios";
import router from "@/routes";

export const getUser = async () => {
  try {
    const { data } = await axios.get('/auth/me');

    return data;
  } catch (err) {
    if (err.response?.status === 401) {
      router.push('/login');

      return null;
    }

    return err;
  }
};