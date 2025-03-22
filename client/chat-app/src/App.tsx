import React, { useEffect, useState } from 'react';
import './App.css';
import Home from './components/Home';
import Login from './components/Login';
import { connection, ensureConnected } from './SignalR/signalRConnection';

function App() {
  const [messages, setMessages] = useState<string[]>([]);
  const [message, setMessage] = useState("");
  const [connectionLoader, setConnectionloader] = useState(Boolean);
  const [username, setUsername] = useState('');

  useEffect(() => {
    setConnectionloader(false);
    connection.on("messageReceived", (user: string, newMessage: string) => {
      setMessages(prevMessages => [...prevMessages, `${user}: ${newMessage}`]);
    });

    // Establish SignalR connection
    ensureConnected();
    setConnectionloader(true);

    return () => {
      console.log("Cleaning up SignalR connection...");
      connection.off("messageReceived");
      connection.stop();
    };
  }, []);

  const sendMessage = async () => {
    if (message.trim()) {
      console.log("Sending message:", { username, message });
      try {
        await connection.invoke("NewMessage", username, message);
        setMessage("");
      } catch (error) {
        console.log("Message not sent, error:", error);
      }
    }
  };

  // Retreive username from child component Login
  const handleUsername = (data: string): void => {
    setUsername(data);
  };

  return (
    <div className="App">
      <Home/>
      <Login onLoginData={handleUsername}/>
      <div>
        {messages.map((msg, index) => (
          <p key={index}>{msg}</p>
        ))}
      </div>
      <input
        type="text"
        value={message}
        onChange={(e) => setMessage(e.target.value)}
        placeholder='Type a message'
      />
      {
        connectionLoader
          ?
        <button onClick={sendMessage}>Send Message</button>
          :
        <p>Connecting...</p>
      }
    </div>
  );
};

export default App;
