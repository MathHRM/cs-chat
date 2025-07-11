import axios from "axios";

export const getUser = async () => {
  try {
    const { data } = await axios.get('/user');

    return data.user;
  } catch (err) {
    console.log(err);

    return err;
  }
};