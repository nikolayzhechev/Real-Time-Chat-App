import React, { ReactElement, useState, ChangeEvent, FormEvent } from 'react';

function Login({onLoginData}: any): ReactElement {
  const [email, setEmail] = useState('');
  const [password, setPassword] = useState('');
  const [inputValue, setInputValue] = useState('');

    const handleLogin = async (event: FormEvent<HTMLFormElement>): Promise<void> => {
      handleSubmit(event);

      const response = await fetch('http://localhost:5258/api/auth/login', {
        method: 'POST',
        headers: { 'Content-Type': 'application/json' },
        body: JSON.stringify({email, password})
      });

      if (response.ok) {
        const data = await response.json();
        localStorage.setItem('token', data.token);
        console.log('Login successful!');
      } else {
        console.log('Login failed!');
      }
    };

    const handleChange = (event: ChangeEvent<HTMLInputElement>): void => {
      setInputValue(event.target.value);
    };

    const handleSubmit = (event: FormEvent<HTMLFormElement>): void => {
      event.preventDefault();
      console.log('Username submitted:', inputValue);
      onLoginData(inputValue);
    };

    return (
        <div>
            <h2>Username Form</h2>
            <form onSubmit={handleLogin}>
              <label htmlFor="username">Enter your username:</label>
              <input 
                type="text" 
                id="username" 
                name="username" 
                value={inputValue} 
                onChange={handleChange} 
                required 
              />
              <input type="email" value={email} onChange={e => setEmail(e.target.value)} placeholder="Email" />
              <input type="password" value={password} onChange={e => setPassword(e.target.value)} placeholder="Password" />
              <input type="submit" value="Submit" />
            </form>
        </div>
    )
};

export default Login;