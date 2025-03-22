import React, { ReactElement, useState, ChangeEvent, FormEvent } from 'react';

function Login({onLoginData}: any): ReactElement {
  const [inputValue, setInputValue] = useState('');

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
            <form onSubmit={handleSubmit}>
              <label htmlFor="username">Enter your username:</label><br /><br />
              <input 
                type="text" 
                id="username" 
                name="username" 
                value={inputValue} 
                onChange={handleChange} 
                required 
              /><br /><br />
              <input type="submit" value="Submit" />
            </form>
        </div>
    )
};

export default Login;