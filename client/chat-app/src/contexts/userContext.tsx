import React, { createContext, useContext, useEffect, useState } from 'react';

type AuthData = {
    id: number;
    email: string;
    username: string;
    token: string;
};

type UserContextType = {
    authData: AuthData | null;
    setAuthData: (authData: AuthData) => void;
};

const UserContext = createContext<UserContextType | undefined>(undefined);

export const UserProvider: React.FC<{ children: React.ReactNode }> = ({ children }) => {
    const [authData, setAuthDataState] = useState<AuthData | null>(null);

    useEffect(() => {
       const stored = localStorage.getItem("authData");
       if (stored) setAuthDataState(JSON.parse(stored));
    }, []);

    const setAuthData = (data: AuthData) => {
        setAuthDataState(data);
        localStorage.setItem("authData", JSON.stringify(data));
    };

    return (
        <UserContext.Provider value={{ authData, setAuthData }}>
          {children}
        </UserContext.Provider>
    );
};

export const useUser = () => {
    const context = useContext(UserContext);
    if (!context) throw new Error("useUser must be used within a UserProvider");
    return context;
};