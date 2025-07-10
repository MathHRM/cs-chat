import axios from "axios";

axios.defaults.baseURL = import.meta.env.VITE_API_URL;
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
