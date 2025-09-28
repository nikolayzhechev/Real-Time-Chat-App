import { ReactElement, useState, useEffect, useCallback } from "react";
import { getConnection } from '../SignalR/signalRConnection';
import { useUser } from '../contexts/userContext';
import Users from "../components/Users";
import Chats from "../components/Chats";
import IChat from "../interfaces/IChat";
import IMessage from "../interfaces/IMessage";

function Chat (): ReactElement {
    const [messages, setMessages] = useState<IMessage[]>([]);
    const [message, setMessage] = useState<IMessage | null>();
    const [chats, setChats] = useState<IChat[]>([]);
    const [chatRefreshTrigger, setChatRefreshTrigger] = useState(0);
    const [activeChat, setActiveChat] = useState<IChat | null>(null);
    const { authData } = useUser();
    const username: string | undefined = authData?.username;
    const connection = getConnection();

    useEffect(() => {
        const handler: any = connection?.on("ReceiveMessage", (message: IMessage) => {
          setMessages(prevMessages => [...prevMessages, message]);
        });

        return () => {
          connection?.off("ReceiveMessage", handler); // Clean up when the component unmounts or re-renders
        };
    }, []);

    const sendMessage = async () => {
        if (message?.content.trim()) {
          console.log("Sending message:", { username, message });

          try {
            console.log(activeChat?.id);

            await connection?.invoke("SendMessage", activeChat?.id, username, message.content);

          } catch (error) {
            console.log("Message not sent, error:", error);
          }
          finally
          {
            message.content = "";
          }
        }
    };

    const handleSelectChat = useCallback((chat: IChat) => {
        setActiveChat(chat);
    }, []);

    try {
      return (
          <div>
              <h2>Start a New Chat</h2>
                <Users onNewChatCreated={(newChat) => {
                  setChats(prev => [...prev, newChat]);
                  setChatRefreshTrigger(prev => prev + 1);
                  setActiveChat(newChat);
                }} />
              <h2>Continue Existing Chats</h2>
                <Chats refreshTrigger={chatRefreshTrigger} onSelectChat={handleSelectChat} />
              <h2>Active chat:</h2>
              {activeChat ? (
                <div>
                  <p>Chat Name: {activeChat.name.replace(authData?.username!, "")}</p>
                  <p>Created At: {new Date(activeChat.createdAt).toLocaleString()}</p>
                  {activeChat.messages.map((msg, index) => (
                      <p key={index}>
                        <strong>{msg.username}</strong>: {msg.content}
                        <small> | {new Date(msg.sentAt).toLocaleString()}</small>
                      </p>
                  ))}
                </div>
              ) : (
                <p>No chat selected.</p>
              )}
              <div>
                  {messages.map((msg, index) => (
                      <p key={index}>
                        <strong>{msg.username}</strong>: {msg.content}
                        <small> | {new Date(msg.sentAt).toLocaleString()}</small>
                      </p>
                  ))}
              </div>
              <input
                  type="text"
                  value={message?.content}
                  onChange={(e) => setMessage({
                    chatId: activeChat?.id!,
                    senderId: authData?.id!,
                    username: authData?.username!,
                    content: e.target.value,
                    sentAt: new Date()
                  })} 
                  placeholder="Type a message"
              ></input>
              <button onClick={sendMessage}>Send</button>
          </div>
      )
    }
    catch (err){ console.log(err); }
    return <p>Something went wrong.</p>;
};

export default Chat;