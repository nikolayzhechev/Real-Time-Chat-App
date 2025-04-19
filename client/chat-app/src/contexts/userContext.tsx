import React, { createContext, useContext, useState } from 'react';

type UserContextType = {
    username: string;
    setUsername: (name: string) => void;
};

const UserContext = createContext<UserContextType | undefined>(undefined);

export const UserProvider: React.FC<{ children: React.ReactNode }> = ({ children }) => {
    const [username, setUsername] = useState('');

    return (
        <UserContext.Provider value={{ username, setUsername }}>
          {children}
        </UserContext.Provider>
    );
};

export const useUser = () => {
    const context = useContext(UserContext);
    if (!context) throw new Error("useUser must be used within a UserProvider");
    return context;
};