import axios from "axios";
import router from "@/routes";

export const getUser = async () => {
  try {
    const { data } = await axios.get('/user');

    return data.user;
  } catch (err) {
    if (err.response.status === 401) {
        router.push('/login');
    }

    console.log(err);

    return err;
  }
};