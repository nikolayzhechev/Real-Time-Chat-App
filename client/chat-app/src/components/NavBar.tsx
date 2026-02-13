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

    const handleLogout = () => {
        logout();
        navigate('/');

        console.log("Cleaning up SignalR connection...");
        stopConnection();
        console.log("Connection stopped");
    };
  
    return (
        <header className="navbar">
            {isAuthenticated() ? (
                <div className="navbar-inner">
                    <div className="navbar-brand">
                        <span className="navbar-title">RealTime Chat</span>
                        <span className="navbar-user">Welcome, {authData?.username}!</span>
                    </div>
                    <nav>
                        <ul className="navbar-links">
                            <li>
                                <NavLink to="/chat" className="nav-link">
                                    Chat
                                </NavLink>
                            </li>
                            <li>
                                <button className="btn btn-ghost" onClick={handleLogout}>
                                    Logout
                                </button>
                            </li>
                            <li>
                                <NavLink to="/profile" className="nav-link">
                                    Profile
                                </NavLink>
                            </li>
                        </ul>
                    </nav>
                </div>
            ) : (
                <nav className="navbar-inner">
                    <div className="navbar-brand">
                        <span className="navbar-title">RealTime Chat</span>
                    </div>
                    <ul className="navbar-links">
                        <li>
                            <NavLink to="/login" className="nav-link">
                                Login
                            </NavLink>
                        </li>
                        <li>
                            <NavLink to="/register" className="nav-link">
                                Register
                            </NavLink>
                        </li>
                    </ul>
                </nav>
            )}
        </header>
    );
};

export default NavBar;