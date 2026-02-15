// src/store/auth.store.ts
import { create } from "zustand";

interface User {
  id: string;
  username: string;
}

interface AuthState {
  user: User | null;
  token: string | null;
  login: (user: User, token: string) => void;
  setToken: (token: string) => void;
  logout: () => void;
  isAuthenticated: () => boolean;
}

export const useAuthStore = create<AuthState>()((set, get) => ({
  user: null,
  token: null,

  login: (user, token) => set({ user, token }),
  setToken: (token) => set({ token }),
  logout: () => set({ user: null, token: null }),

  isAuthenticated: () => !!get().token, // Có token = đã đăng nhập
}));
