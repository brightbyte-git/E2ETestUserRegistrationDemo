import React from "react";
import { Navigate, Outlet } from "react-router-dom";

const PrivateRoute = () => {
    const token = localStorage.getItem("token"); // Check for JWT token

    return token ? <Outlet /> : <Navigate to="/login" replace />; // If no token, redirect to login
};

export default PrivateRoute;