import axios from "axios";
import { toast } from "react-toastify";

function apiClient() {
  const baseUrl = `${process.env.REACT_APP_BASE_URL}api/`;
  const axiosConfig = {
    baseURL: baseUrl,
    headers: {
      Accept: "*/*",
    },
  };

  const client = axios.create(axiosConfig);

  client.interceptors.request.use(
    async (config) => {
      config.headers["Content-Type"] = "application/json";
      return config;
    },
    (error) => {
      return Promise.reject(error);
    }
  );

  return client;
}

export async function PostAsync(url: string, postData: {}) {
  let abortController = new AbortController();
  const client = apiClient();
  const response = await client
    .post(
      url,
      { ...postData },
      {
        signal: abortController.signal,
      }
    )
    .catch((err) => {
      if (err.response?.data?.errors) {
        Object.keys(err.response?.data?.errors).forEach((itm: string) => {
          toast.error(err.response?.data?.errors[itm][0]);
        });
      }

      return Promise.reject(err);
    });

  return response;
}
