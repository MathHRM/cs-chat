import axios from "axios";

export const getGuestMessages = async (lastMessageId = null) => {
  try {
    const { data } = await axios.get(`/messages/guest`, {
      params: {
        lastMessageId,
      },
    });

    return data;
  } catch (err) {
    return err;
  }
};