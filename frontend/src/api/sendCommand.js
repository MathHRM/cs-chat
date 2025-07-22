import axios from "axios";

export const sendCommand = async (command) => {
  try {
    const { data } = await axios.post(`/commands`, { command });

    return data;
  } catch (err) {
    return err;
  }
};