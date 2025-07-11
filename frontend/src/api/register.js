import axios from "axios";

export const register = async ({
  username,
  password,
}) => {
  try {
    const { data } = await axios.post('/auth/register', {
      username,
      password,
    });

    localStorage.setItem('@auth', `${data.token}`);

    return data;
  } catch (err) {
    console.log(err);

    return err;
  }
};