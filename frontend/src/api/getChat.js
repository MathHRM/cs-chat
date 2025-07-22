import axios from "axios";

export const getChat = async (chatId) => {
  try {
    const { data } = await axios.get(`/chats/${chatId}`);

    return data;
  } catch (err) {
    return err;
  }
};