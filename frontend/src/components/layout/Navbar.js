import React from "react";
import axios from "axios";
import { useNavigate } from "react-router-dom";

const NavBar = () => {
    const navigate = useNavigate();
    const token = localStorage.getItem("token");

    const handleLogout = async () => {
        if (!token) {
            navigate("/login");
            return;
        }

        try {
            await axios.post(
                "http://localhost:5271/api/User/logout",
                {}, // Empty body if only token is in header
                {
                    headers: {
                        Authorization: `Bearer ${token}`,
                    },
                }
            );

            // Clear storage on successful logout
            localStorage.removeItem("token");
            localStorage.removeItem("tenantId");

            // Redirect to login page
            navigate("/login");
        } catch (error) {
            console.error("Error during logout:", error);
        }
    };

    return (
        <nav
            style={{
                display: "flex",
                justifyContent: "space-between",
                padding: "1rem",
                background: "#333",
                color: "#fff",
            }}
        >
            <h1>E2E Tests Demo</h1>
            <button
                onClick={handleLogout}
                style={{
                    background: token ? "#ff5c5c" : "#007bff",
                    color: "#fff",
                    padding: "0.5rem 1rem",
                    border: "none",
                    borderRadius: "4px",
                }}
            >
                {token ? "Logout" : "Login"}
            </button>
        </nav>
    );
};

export default NavBar;
