import axios from "axios";
import { useAuthStore } from "@/store/authStore";

// 1. Cấu hình instance
const api = axios.create({
  baseURL: import.meta.env.VITE_API_URL,
  headers: {
    "Content-Type": "application/json",
  },
  withCredentials: true, // <--- QUAN TRỌNG NHẤT: Để gửi/nhận Cookie HttpOnly
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
api.interceptors.response.use(
  (response) => response,
  async (error) => {
    const originalRequest = error.config;

    // Nếu gặp lỗi 401 (Hết hạn Access Token)
    if (error.response?.status === 401 && !originalRequest._retry) {
      originalRequest._retry = true; // Đánh dấu đã thử refresh

      try {
        // Gọi API refresh
        // LƯU Ý: Không cần gửi body chứa refreshToken nữa!
        // Cookie HttpOnly sẽ tự động đi kèm request này.
        const res = await api.post("/auth/refresh");

        // API trả về AccessToken mới (còn RefreshToken mới thì nằm trong Cookie rồi)
        const { accessToken } = res.data;

        // Lưu AccessToken mới vào Store
        useAuthStore.getState().setToken(accessToken);

        // Gắn token mới vào header request cũ và gọi lại
        originalRequest.headers.Authorization = `Bearer ${accessToken}`;
        return api(originalRequest);
      } catch (refreshError) {
        // Nếu refresh thất bại (Cookie hết hạn hoặc không hợp lệ) -> Logout
        useAuthStore.getState().logout();
        window.location.href = "/login";
        return Promise.reject(refreshError);
      }
    }
    return Promise.reject(error);
  },
);

export default api;
