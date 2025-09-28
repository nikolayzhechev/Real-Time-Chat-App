import React, { useState, ReactElement, FormEvent, ChangeEvent } from "react";
import { useNavigate } from 'react-router-dom';

function Register(): ReactElement {
    const [email, setEmail] = useState('');
    const [userName, setUserName] = useState('');
    const [password, setPassword] = useState('');
    const [repeatPassword, setRepeatPassword] = useState('');
    const [error, setError] = useState('');
    const navigate = useNavigate();
    
    const handleRegister = async (event: FormEvent<HTMLFormElement>): Promise<void> => {
      event.preventDefault();

      if (password !== repeatPassword){
        setError("Passwords don't match.");
      } else {
        const response = await fetch(`${process.env.REACT_APP_API_BASE_URL}/api/auth/register`, {
          method: 'POST',
          headers: { 'Content-Type': 'application/json' },
          body: JSON.stringify({userName, email, password})
        });
 
        if (response.ok) {
          const data = await response.json();
          localStorage.setItem('token', data.token);
          setError('');
          console.log('Register successful!');
          navigate('/chat');
        } else {
          console.log('Register failed! ', response);
        }
      }
     };

     const handleChange = (event: ChangeEvent<HTMLInputElement>): void => {
      setUserName(event.target.value);
     };

     const handleSubmit = (event: FormEvent<HTMLFormElement>): void => {
       event.preventDefault();
       console.log('Username submitted:', userName);
     };

    return (
        <div>
        <h2>Register</h2>
        <form onSubmit={handleRegister}>
          <label htmlFor="username">Enter your email and password to register:</label>
          <input type="text" id="username" name="username" value={userName} onChange={handleChange} required placeholder="Username"/>
          <input type="email" value={email} onChange={e => setEmail(e.target.value)} placeholder="Email" />
          <input type="password" value={password} onChange={e => setPassword(e.target.value)} placeholder="Password" />
          <input type="password" value={repeatPassword} onChange={e => setRepeatPassword(e.target.value)} placeholder="Repeat Password" />
          {error && <p style={{ color: 'red' }}>{error}</p>}
          <input type="submit" value="Register" />
        </form>
    </div>
    )
};

export default Register;