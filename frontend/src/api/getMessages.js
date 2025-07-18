import axios from "axios";

export const getMessages = async (lastMessageId = 0) => {
  try {
    const { data } = await axios.get(`/messages`, {
      params: {
        lastMessageId,
      },
    });

    return data;
  } catch (err) {
    console.log(err);

    return err;
  }
};