import axios from "axios";
import router from "@/routes";

export const joinChat = async (chatId) => {
  try {
    const { data } = await axios.post('/chats/join', { chatId });

    return data;
  } catch (err) {
    if (err.response?.status === 401) {
      router.push('/login');

      return null;
    }

    console.log(err);

    return err;
  }
};