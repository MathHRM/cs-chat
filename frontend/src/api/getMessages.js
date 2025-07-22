import axios from "axios";

export const getMessages = async (lastMessageId = null) => {
  try {
    const { data } = await axios.get(`/messages`, {
      params: {
        lastMessageId,
      },
    });

    return data;
  } catch (err) {
    return err;
  }
};