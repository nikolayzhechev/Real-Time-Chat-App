import React from "react";
import { NavLink } from 'react-router-dom';
import { useUser } from '../contexts/userContext';
import { logout } from "../services/auth";
import { isAuthenticated } from '../services/auth';
import { useNavigate } from "react-router-dom";
import { stopConnection } from "../SignalR/signalRConnection";

const NavBar: React.FC = () => {
    const { authData } = useUser();
    const navigate = useNavigate();

    console.log(authData);

    const handleLogout = () => {
        logout();
        navigate('/');

        console.log("Cleaning up SignalR connection...");
        stopConnection();
        console.log("Connection stopped");
    };
  
    return (
        <div>
            {
                isAuthenticated()
                    ?
                <div>
                    <p>Welcome, {authData?.username}!</p>
                    <ul>
                        <li><button><NavLink to="/chat">Chat</NavLink></button></li>
                        <li><button onClick={handleLogout}>Logout</button></li>
                    </ul>
                </div>
                    :
                <ul>
                    <li><button><NavLink to="/login">Login</NavLink></button></li>
                    <li><button><NavLink to="/register">Register</NavLink></button></li>
                </ul>
            }
        </div>
    )
};

export default NavBar;