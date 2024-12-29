import React, { useState } from "react";
import axios from "axios";
import { useNavigate } from "react-router-dom";

const Login = () => {
    const navigate = useNavigate(); // Initialize the navigate function

    const handleRegisterClick = () => {
        navigate("/register"); // Navigate to the /register page
    };

    return (
        <div className="flex items-center justify-center min-h-screen bg-neutral-900 text-neutral-50">
            <div className="w-full max-w-md p-8 bg-neutral-800 rounded shadow-md">
                <h2 className="text-2xl font-bold mb-6 text-center">Login</h2>
                <form className="space-y-4">
                    <div>
                        <label className="block text-sm font-medium mb-1">Email:</label>
                        <input
                            type="email"
                            name="email"
                            required
                            className="w-full p-2 border border-neutral-700 rounded bg-neutral-900 text-neutral-50"
                        />
                    </div>
                    <div>
                        <label className="block text-sm font-medium mb-1">Password:</label>
                        <input
                            type="password"
                            name="password"
                            required
                            className="w-full p-2 border border-neutral-700 rounded bg-neutral-900 text-neutral-50"
                        />
                    </div>
                    <button
                        type="submit"
                        className="w-full py-2 mt-4 bg-blue-500 rounded text-white font-semibold hover:bg-blue-600 transition-colors"
                    >
                        Login
                    </button>
                </form>
                <button
                    onClick={handleRegisterClick} // Attach the click handler here
                    className="w-full py-2 mt-4 bg-gray-500 rounded text-white font-semibold hover:bg-gray-600 transition-colors"
                >
                    Register
                </button>
            </div>
        </div>
    );
};

export default Login;