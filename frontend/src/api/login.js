import axios from "axios";

export const login = async (username, password) => {
  try {
    const { data } = await axios.post('/auth/login', {
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