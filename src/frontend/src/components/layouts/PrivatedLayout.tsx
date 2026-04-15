import api from "@/lib/api";
import { useAuthStore } from "@/store/authStore";
import { useEffect, useState } from "react";
import { Navigate, Outlet, useLocation } from "react-router-dom";

const PrivatedLayout = () => {
  const [isChecking, setChecking] = useState(true);
  const { token, setToken, logout } = useAuthStore();
  const location = useLocation();
  useEffect(() => {
    const restoreAuth = async () => {
      try {
        setChecking(true);
        const res = await api.post("/auth/refresh");
        setToken(res.data.accessToken);
      } catch {
        logout();
      } finally {
        setChecking(false);
      }
    };

    restoreAuth();
  }, [logout, setToken]);

  if (isChecking) {
    return <div>Loading...</div>;
  }
  // Chưa login → redirect login
  if (!token) {
    return <Navigate to="/login" replace state={{ from: location.pathname }} />;
  }

  return <Outlet />;
};

export default PrivatedLayout;
