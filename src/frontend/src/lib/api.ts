import axios, { AxiosError, type AxiosRequestConfig } from "axios";
import { useAuthStore } from "@/store/authStore";

// 1. Cấu hình instance
const api = axios.create({
  baseURL: import.meta.env.VITE_API_URL,
  headers: {
    "Content-Type": "application/json",
  },
  withCredentials: true,
});

// 2. Request Interceptor: Gắn Access Token (giữ nguyên)
api.interceptors.request.use((config) => {
  // Access Token vẫn lưu ở Store (Memory) nên phải tự gắn
  const token = useAuthStore.getState().token;
  if (token) {
    config.headers.Authorization = `Bearer ${token}`;
  }
  return config;
});

// 3. Response Interceptor: Xử lý Refresh Token tự động
interface FailedRequest {
  resolve: (token: string) => void;
  reject: (error: AxiosError) => void;
}

let isRefreshing = false;
let failedQueue: FailedRequest[] = [];

const processQueue = (error: AxiosError | null, token: string | null) => {
  failedQueue.forEach(({ resolve, reject }) => {
    if (error) reject(error);
    else if (token) resolve(token);
  });

  failedQueue = [];
};

api.interceptors.response.use(
  (res) => res,
  async (error: AxiosError) => {
    const originalRequest = error.config as AxiosRequestConfig & {
      _retry?: boolean;
    };

    const isRefreshRequest = originalRequest.url?.includes("/auth/refresh");

    // ❌ Nếu refresh fail → logout ngay
    if (error.response?.status === 401 && isRefreshRequest) {
      useAuthStore.getState().logout();
      window.location.href = "/login";
      return Promise.reject(error);
    }

    if (error.response?.status === 401 && !originalRequest._retry) {
      originalRequest._retry = true;

      if (isRefreshing) {
        return new Promise<string>((resolve, reject) => {
          failedQueue.push({ resolve, reject });
        }).then((token) => {
          originalRequest.headers = {
            ...originalRequest.headers,
            Authorization: `Bearer ${token}`,
          };
          return api(originalRequest);
        });
      }

      isRefreshing = true;

      try {
        const res = await api.post<{ accessToken: string }>("/auth/refresh");
        const { accessToken } = res.data;

        useAuthStore.getState().setToken(accessToken);
        processQueue(null, accessToken);

        originalRequest.headers = {
          ...originalRequest.headers,
          Authorization: `Bearer ${accessToken}`,
        };

        return api(originalRequest);
      } catch (err) {
        processQueue(err as AxiosError, null);
        useAuthStore.getState().logout();
        window.location.href = "/login";
        return Promise.reject(err);
      } finally {
        isRefreshing = false;
      }
    }

    return Promise.reject(error);
  },
);

export default api;
