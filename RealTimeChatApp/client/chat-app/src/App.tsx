import React, { useEffect, useState } from 'react';
import './App.css';
import Home from './components/Home';
import connection from './SignalR/signalRConnection';

function App() {
  const [messages, setMessages] = useState<string[]>([]);
  const [message, setMessage] = useState("");

  useEffect(() => {
    connection
      .start()
      .then(() => console.log("Connection to SignalR successful."))
      .catch(err => console.error("Connection failes:", err));

      connection.on("ReceiveMessage", (user: string, newMessage: string) => {
        setMessages(prevMessages => [...prevMessages, `${user}: ${newMessage}`]);
      });

      return () => {
        connection.stop();
      };
  }, []);

  const sendMessage = async () => {
    if (message.trim()) {
      await connection.invoke("SendMessage", "User1", message);
      setMessage("");
    }
  };

  return (
    <div className="App">
      <Home/>
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
      <button onClick={sendMessage}>Send Message</button>
    </div>
  );
}

export default App;
