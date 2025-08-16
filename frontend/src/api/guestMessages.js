import axios from "axios";

export const getGuestMessages = async (lastMessageId = null) => {
  return axios.get(`/messages/guest`, {
    params: {
      lastMessageId,
    },
  });
};
