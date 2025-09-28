import { ReactElement, useState, FormEvent } from 'react';
import { useNavigate } from 'react-router-dom';
import { useUser } from '../contexts/userContext';
import { useAuth } from '../contexts/authContext';

function Login(): ReactElement {
  const [email, setEmail] = useState('');
  const [password, setPassword] = useState('');
  const [inputValue, setInputValue] = useState('');
  const navigate = useNavigate();
  const { setAuthData } = useUser();
  const { setToken } = useAuth();

    const handleLogin = async (event: FormEvent<HTMLFormElement>): Promise<void> => {
      event.preventDefault();

      const response: Response = await fetch(`${process.env.REACT_APP_API_BASE_URL}/api/auth/login`, {
        method: 'POST',
        headers: { 'Content-Type': 'application/json' },
        body: JSON.stringify({email, password})
      });

      if (response.ok) {
        const data = await response.json();
        setAuthData({
          id: data.response.user.id,
          email: data.response.user.email,
          username: data.response.user.username,
          token: data.response.token
        });
        console.log('Login successful!');
        setToken(data.response.token);
        navigate('/chat', data.response.user.id);
      } else {
        console.log('Login failed!');
      }
    };

    return (
        <div>
            <h2>Login</h2>
            <form onSubmit={handleLogin}>
              <label htmlFor="username">Enter your email and password to login:</label>
              <input type="email" value={email} onChange={e => setEmail(e.target.value)} placeholder="Email" />
              <input type="password" value={password} onChange={e => setPassword(e.target.value)} placeholder="Password" />
              <input type="submit" value="Login" />
            </form>
        </div>
    )
};

export default Login;