import axios from "axios";

axios.defaults.baseURL = `${process.env.VUE_APP_API_URL}/api`;
axios.interceptors.request.use(
  (config) => {
    const token = localStorage.getItem("@auth");

    if (token) {
      config.headers.Authorization = `Bearer ${token}`;
    }

    return config;
  },
  (error) => Promise.reject(error)
);
