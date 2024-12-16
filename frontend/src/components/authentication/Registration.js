import React, { useState } from "react";
import { useNavigate } from "react-router-dom";
import { registerUser } from "../../services/userService";

const Registration = () => {
    const [formData, setFormData] = useState({
        email: "",
        password: "",
        confirmPassword: "",
        organisation: "",
    });

    const [errorMessage, setErrorMessage] = useState("");
    const [successMessage, setSuccessMessage] = useState("");
    const navigate = useNavigate();

    const handleChange = (e) => {
        const { name, value } = e.target;
        setFormData({
            ...formData,
            [name]: value,
        });
    };

    const handleSubmit = async (e) => {
        e.preventDefault();

        if (formData.password !== formData.confirmPassword) {
            setErrorMessage("Passwords do not match");
            return;
        }

        try {
            await registerUser(formData);
            setSuccessMessage("Registration successful! Please log in.");
            setErrorMessage("");
            setFormData({ email: "", password: "", confirmPassword: "", organisation: "" });

            setTimeout(() => {
                navigate("/login");
            }, 1500);
        } catch (error) {
            setErrorMessage(error.response?.data?.message || "Registration failed. Please try again.");
        }
    };

    return (
        <div className="flex items-center justify-center min-h-screen bg-neutral-900 text-neutral-50">
            <div className="w-full max-w-md p-8 bg-neutral-800 rounded shadow-md">
                <h2 className="text-2xl font-bold mb-6 text-center">Register</h2>
                {errorMessage && <p className="text-red-500 text-center mb-4">{errorMessage}</p>}
                {successMessage && <p className="text-green-500 text-center mb-4">{successMessage}</p>}
                <form onSubmit={handleSubmit} className="space-y-4">
                    <div>
                        <label className="block text-sm font-medium mb-1">Email:</label>
                        <input
                            type="email"
                            name="email"
                            value={formData.email}
                            onChange={handleChange}
                            required
                            className="w-full p-2 border border-neutral-700 rounded bg-neutral-900 text-neutral-50"
                        />
                    </div>
                    <div>
                        <label className="block text-sm font-medium mb-1">Organisation:</label>
                        <input
                            type="text"
                            name="organisation"
                            value={formData.organisation}
                            onChange={handleChange}
                            required
                            className="w-full p-2 border border-neutral-700 rounded bg-neutral-900 text-neutral-50"
                        />
                    </div>
                    <div>
                        <label className="block text-sm font-medium mb-1">Password:</label>
                        <input
                            type="password"
                            name="password"
                            value={formData.password}
                            onChange={handleChange}
                            required
                            className="w-full p-2 border border-neutral-700 rounded bg-neutral-900 text-neutral-50"
                        />
                    </div>
                    <div>
                        <label className="block text-sm font-medium mb-1">Confirm Password:</label>
                        <input
                            type="password"
                            name="confirmPassword"
                            value={formData.confirmPassword}
                            onChange={handleChange}
                            required
                            className="w-full p-2 border border-neutral-700 rounded bg-neutral-900 text-neutral-50"
                        />
                    </div>
                    <button
                        type="submit"
                        className="w-full py-2 mt-4 bg-blue-500 rounded text-white font-semibold hover:bg-blue-600 transition-colors"
                    >
                        Register
                    </button>
                </form>
            </div>
        </div>
    );
};

export default Registration;