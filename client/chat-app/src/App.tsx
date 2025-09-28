import { useEffect, useState, JSX } from 'react';
import { BrowserRouter as Router, Route, Routes, Navigate } from 'react-router-dom';
import './App.css';
import Home from './pages/Home';
import Login from './pages/Login';
import Chat from './pages/Chat';
import NavBar from './components/NavBar';
import { ensureConnected, stopConnection } from './SignalR/signalRConnection';
import { isAuthenticated } from './services/auth';
import { useAuth } from './contexts/authContext';
import Register from './pages/Register';

function App() {
  const [connectionLoader, setConnectionloader] = useState(false);
  const { token } = useAuth();

  const PrivateRoute = ({ children }: { children: JSX.Element }) => {
    return isAuthenticated() ? children : <Navigate to="/login" />;
  };

  useEffect(() => {
    if (token) {
      setConnectionloader(true);
      // Establish SignalR connection
      ensureConnected(token);
      setConnectionloader(false);
    }
  }, [token]);

  return (
    <Router>
      <NavBar />
      <Routes>
        <Route path="/" element={<Home />}></Route>
        <Route path="/login" element={<Login />} />
        <Route path="/register" element={<Register />} />
        <Route
          path="/chat"
          element={
            <PrivateRoute>
              <Chat />
            </PrivateRoute>
          }
        />
      </Routes>
    </Router>
  );
};

export default App;
