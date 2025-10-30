import { ReactElement, useState, useEffect, useCallback, ChangeEvent } from "react";
import { getConnection } from '../SignalR/signalRConnection';
import { useUser } from '../contexts/userContext';
import Users from "../components/Users";
import Chats from "../components/Chats";
import Search from "../components/Search";
import IChat from "../interfaces/IChat";
import IMessage from "../interfaces/IMessage";
import IAppUser from "../interfaces/IAppUser";

function Chat (): ReactElement {
    const [messages, setMessages] = useState<IMessage[]>([]);
    const [message, setMessage] = useState<IMessage | null>();
    const [chats, setChats] = useState<IChat[]>([]);
    const [chatRefreshTrigger, setChatRefreshTrigger] = useState(0);
    const [activeChat, setActiveChat] = useState<IChat | null>(null);
    const { authData } = useUser();
    const [error, setError] = useState<string | null>(null);
    const [loading, setLoading] = useState<boolean>(true);
    const [isGroup, setIsGroup] = useState<boolean>(false);
    const [activeUsers, setActiveUsers] = useState<IAppUser[]>([]);
    const [selectedGroupUsers, setSelectedGroupUsers] = useState<IAppUser[]>([]);
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

          } catch (error: any) {
            console.log("Message not sent, error:", error);
            setError(error.message);
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

    const handleSelectedGroupUsers = (e: ChangeEvent<HTMLInputElement>, user: IAppUser): void => {
      if (e.target.checked) {
        setSelectedGroupUsers(prev => [...prev, user])
      } else {
        removeUser(user);
      }
    };

    const removeUser = (user: IAppUser): void => {
        let toRemove: IAppUser | undefined = selectedGroupUsers.find(u => u.username == user.username);
        console.log(user.username);
        if (toRemove) {
          setSelectedGroupUsers((prev) => prev.filter(user => user !== toRemove));
        }
    };

    if (error) return <p>Error: {error}</p>

    try {
      return (
          <div>
              <h2>Start Chat</h2>
                <Users onNewChatCreated={(newChat) => {
                  setChats(prev => [...prev, newChat]);
                  setChatRefreshTrigger(prev => prev + 1);
                  setActiveChat(newChat);
                }} onFetchedUsers={(users) => {
                  setActiveUsers(users);
                }} />
              <h2>Existing Chats</h2>
                <Chats refreshTrigger={chatRefreshTrigger} onSelectChat={handleSelectChat} />
              <h2>Active chat:</h2>
              {activeChat ? (
                <div>
                  <p>Chat Name: {activeChat.name.replace(authData?.username!, "")}</p>
                  <button>Edit</button>
                  <p>Created At: {new Date(activeChat.createdAt).toLocaleString()}</p>
                  <div>
                    {
                      isGroup
                      ?
                      <div>
                        <Search<IAppUser>
                          collection="users"
                          onResults={setActiveUsers}
                        />
                        <ul>
                          {selectedGroupUsers.map((user) => (
                            <li>
                              {user.username}
                              <button onClick={() => removeUser(user)}>x</button>
                            </li>
                          ))}
                        </ul>
                        {activeUsers.map((user) => (
                          <div>
                              <label>
                                {user.username}
                                <input
                                  type="checkbox"
                                  name="groupCheckbox"
                                  onChange={(e) => handleSelectedGroupUsers(e, user)}/>
                              </label>
                          </div>
                        ))}

                        <button onClick={() => setIsGroup(!isGroup)}>Add Selected Participants to Chat</button>
                      </div>
                      :
                      <button onClick={() => setIsGroup(!isGroup)}>Add More Participants to Chat</button>
                    }
                  </div>
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