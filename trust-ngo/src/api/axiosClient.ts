import axios from "axios";
import { API_BASE_URL, REQUEST_TIMEOUT_MS } from "@/constants/config";

export const axiosClient = axios.create({
  baseURL: API_BASE_URL,
  timeout: REQUEST_TIMEOUT_MS,
  headers: {
    "Content-Type": "application/json",
  },
});

axiosClient.interceptors.response.use(
  (response) => response,
  (error) => Promise.reject(error),
);

export default axiosClient;
