import axios from "axios";

export const getMessages = async (lastMessageId = null) => {
  try {
    const { data } = await axios.get(`/messages`, {
      params: {
        lastMessageId,
      },
    });

    console.log(data);

    return data;
  } catch (err) {
    console.log(err);

    return err;
  }
};