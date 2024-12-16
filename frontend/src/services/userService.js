import axios from "axios";

const BASE_URL = "http://localhost:5182/api";

export const registerUser = async (userData) => {
    return await axios.post(`${BASE_URL}/User/register`, userData);
};