import { jwtDecode } from "jwt-decode";

export const getToken = (): string | null => {
    return localStorage.getItem("token");
};

export const isAuthenticated = (): boolean => {
    const token: string | null = getToken();

    if (!token) return false;

    try {
        const decode: any = jwtDecode(token);
        const exp = decode.exp * 1000;
        return Date.now() < exp;
    } catch (error: any) {
        console.log("Unable to verify authentication " + error)
        return false;
    }
};

export const logout = ():void => {
    localStorage.removeItem("token");
};

export const getUserEmail = (): string | null => {
    const token: string | null = getToken();

    if(!token) return null;

    try {
        const decoded: any = jwtDecode(token);
        return decoded?.unique_name || decoded?.email || null;
    } catch (error) {
        return null;
    }
};