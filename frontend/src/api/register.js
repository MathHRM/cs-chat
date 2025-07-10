import axios from "axios";

export const register = async ({
  username,
  password,
}) => {
  try {
    const { data } = await axios.post('/register', {
      username,
      password,
    });

    localStorage.setItem('@auth', `${data.token}`);

    return data.user;
  } catch (err) {
    console.log(err);

    return err;
  }
};