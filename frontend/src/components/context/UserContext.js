import React, { createContext, useContext, useState } from "react";

const UserContext = createContext();

export const UserProvider = ({ children }) => {
    const [userData, setUserData] = useState({
        tenantId: localStorage.getItem("tenantId"),
        token: localStorage.getItem("token"),
    });

    return (
        <UserContext.Provider value={{ userData, setUserData }}>
            {children}
        </UserContext.Provider>
    );
};

export const useUser = () => useContext(UserContext);
