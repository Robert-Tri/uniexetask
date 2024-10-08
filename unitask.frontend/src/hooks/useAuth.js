import { jwtDecode } from "jwt-decode";

const getCookie = (name) => {
    const value = `; ${document.cookie}`;
    const parts = value.split(`; ${name}=`);
    if (parts.length === 2) return parts.pop().split(';').shift();
};

const useAuth = () => {
    const token = getCookie("AccessToken");
    if (token) {
        try {
            const decoded = jwtDecode(token);
            const id = decoded["http://schemas.xmlsoap.org/ws/2005/05/identity/claims/nameidentifier"];
            const username = decoded["http://schemas.xmlsoap.org/ws/2005/05/identity/claims/name"];
            const role = decoded["http://schemas.microsoft.com/ws/2008/06/identity/claims/role"];
            const email = decoded["http://schemas.xmlsoap.org/ws/2005/05/identity/claims/emailaddress"];
            const permissions = decoded.permissions;
            return { id, username, role, email, permissions };
        } catch (error) {
            console.error("Invalid token:", error);
        }
    }
    return { id: '', username: '', role: '', email: '', permissions: [] };
}

export default useAuth