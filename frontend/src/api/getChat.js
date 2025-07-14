import axios from "axios";
import router from "@/routes";

export const getChat = async (chatId) => {
  try {
    const { data } = await axios.get(`/chat/${chatId}`);

    return data;
  } catch (err) {
    console.log(err);

    return err;
  }
};