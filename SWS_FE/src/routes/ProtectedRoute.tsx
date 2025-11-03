import React from "react";
import { Navigate } from "react-router-dom";

interface ProtectedRouteProps {
  children: JSX.Element;
}

const ProtectedRoute: React.FC<ProtectedRouteProps> = ({ children }) => {
  const token = localStorage.getItem("token");
  console.log("🔐 ProtectedRoute check — token:", token);

  if (!token) {
    console.log("❌ No token found → redirect to /login");
    return <Navigate to="/login" replace />;
  }

  console.log("✅ Token found → access granted");
  return children;
};

export default ProtectedRoute;
