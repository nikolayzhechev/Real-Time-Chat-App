import { ReactElement, useState, useEffect, useCallback, ChangeEvent, useRef } from "react";
import { getConnection } from '../SignalR/signalRConnection';
import { useUser } from '../contexts/userContext';
import Users from "../components/Users";
import Chats from "../components/Chats";
import Search from "../components/Search";
import IChat from "../interfaces/IChat";
import IMessage from "../interfaces/IMessage";
import IAppUser from "../interfaces/IAppUser";
import IChatUser from "../interfaces/IChatUser";
import Message from "../components/Message";
import IAttachment from "../interfaces/IAttachment";

function Chat (): ReactElement {
    const [messages, setMessages] = useState<IMessage[]>([]);
    const [currentMessageContent, setCurrentMessageContent] = useState<string>("");
    const [chats, setChats] = useState<IChat[]>([]);
    const [chatRefreshTrigger, setChatRefreshTrigger] = useState(0);
    const [activeChat, setActiveChat] = useState<IChat | null>(null);
    const { authData } = useUser();
    const [error, setError] = useState<string | null>(null);
    const [loading, setLoading] = useState<boolean>(true);
    const [isGroup, setIsGroup] = useState<boolean>(false);
    const [activeUsers, setActiveUsers] = useState<IAppUser[]>([]);
    const [selectedGroupUsers, setSelectedGroupUsers] = useState<IChatUser[]>([]);
    const [selectedActiveGroupUsers, setSelectedActiveGroupUsers] = useState<IChatUser[]>([]);
    const [chatTitle, setChatTitle] = useState<string>("");
    const [isEditingTitle, setIsEditingTitle] = useState<boolean>(false);
    const [attachments, setAttachments] = useState<FileList | null>(null);
    const username: string | undefined = authData?.username;
    const connection = getConnection();
    const inputFile: any = useRef(null);

    useEffect(() => {
        const handler: any = connection?.on("ReceiveMessage", (message: IMessage): void => {
          setMessages(prevMessages => [...prevMessages, message]);
        });

        connection?.on("ReceiveMessage", handler);
        return () => {
          connection?.off("ReceiveMessage", handler); // Clean up when the component unmounts or re-renders
        };
    }, [activeChat, attachments]);

    const sendMessage = async (): Promise<void> => {
        if (currentMessageContent.trim()) {
          console.log("Sending message:", { username, content: currentMessageContent });

          await sendAttachment();

          try {
            await connection?.invoke("SendMessage",
              activeChat?.id,
              username,
              currentMessageContent
            );

            if (activeChat?.id) {
              try {
                const res: Response = await fetch(`${process.env.REACT_APP_API_BASE_URL}/api/chats/chat/${activeChat.id}`, {
                  headers: {
                    Authorization: `Bearer ${localStorage.getItem("token")}`
                  }
                });

                if (res.ok) {
                  const updatedChat: IChat = await res.json();
                  setActiveChat(updatedChat);
                  setChats(prev => prev.map(c => c.id === updatedChat.id ? updatedChat : c));
                  setMessages(prev => {
                    const existingKeys = new Set(prev.map(m => `${m.chatId}_${m.sentAt}`));
                    const merged = [...prev];
                    for (const m of updatedChat.messages) {
                      const key = `${m.chatId}_${m.sentAt}`;
                      if (!existingKeys.has(key)) {
                        merged.push(m);
                      }
                    }
                    return merged.sort((a, b) => Date.parse(a.sentAt.toString()) - Date.parse(b.sentAt.toString()));
                  });
                } else {
                  console.log("Failed to fetch updated chat after sending message:", res.status);
                }
              } catch (err: any) {
                console.warn("Error fetching updated chat:", err);
              }
            }
            
          } catch (error: any) {
            console.log("Message not sent, error:", error);
            setError(error.message);
          }
          finally
          {
            setCurrentMessageContent("");
            setAttachments(null);
            handleFileReset();
          }
        }
    };

    const sendAttachment = async (): Promise<IAttachment[] | null> => {
      const data = new FormData();

      if (attachments !== null) {
        const files = Array.from(attachments);

        for (const file of files) {
          data.append('files', file, file.name)
        }

        const response: Response = await fetch(`${process.env.REACT_APP_API_BASE_URL}/api/chats/chat/${activeChat?.id}/attachment`, {
          method: 'POST',
          body: data,
          headers: {
            Authorization: `Bearer ${localStorage.getItem("token")}`
          }
        });

        return await response.json();
      }

      return null;
    };

    const sendLocationMessage = async (latitude: number, longitude: number): Promise<void> => {
      try {
        await connection?.invoke("SendLocationMessage", activeChat?.id, username, latitude, longitude);

        if (activeChat?.id) {
          try {
            const res: Response = await fetch(`${process.env.REACT_APP_API_BASE_URL}/api/chats/chat/${activeChat.id}`, {
              headers: {
                Authorization: `Bearer ${localStorage.getItem("token")}`
              }
            });

            if (res.ok) {
              const updatedChat: IChat = await res.json();
              setActiveChat(updatedChat);
              setChats(prev => prev.map(c => c.id === updatedChat.id ? updatedChat : c));
              setMessages(prev => {
                const existingKeys = new Set(prev.map(m => `${m.chatId}_${m.sentAt}`));
                const merged = [...prev];
                for (const m of updatedChat.messages) {
                  const key = `${m.chatId}_${m.sentAt}`;
                  if (!existingKeys.has(key)) {
                    merged.push(m);
                  }
                }
                return merged.sort((a, b) => Date.parse(a.sentAt.toString()) - Date.parse(b.sentAt.toString()));
              });
            } else {
              console.log("Failed to fetch updated chat after sending location:", res.status);
            }
          } catch (err: any) {
            console.warn("Error fetching updated chat:", err);
          }
        }

      } catch (error: any) {
        console.log("Location message not sent, error:", error);
        setError(error.message);
      }
      finally {
        setCurrentMessageContent("");
        setAttachments(null);
        handleFileReset();
      }
    };

    const handleShareLocation = (): void => {
      if (navigator.geolocation) {
        navigator.geolocation.getCurrentPosition((position) => {
          const latitude = position.coords.latitude;
          const longitude = position.coords.longitude;

          sendLocationMessage(latitude, longitude);
        });
      }
    };

    const handleSelectChat = useCallback((chat: IChat): void => {
        setActiveChat(chat);
        setChatTitle(chat.name.replace(authData?.username!, ""));

        try {
          connection?.invoke("AddToGroup", chat.id.toString());
        } catch (err) {
          console.log("Failed to join chat group", err);
        }
    }, []);

    const handleSelectedGroupUsers = (e: ChangeEvent<HTMLInputElement>, user: IChatUser): void => {
      if (e.target.checked) {
        setSelectedGroupUsers(prev => [...prev, user])
      } else {
        removeUser(selectedGroupUsers, setSelectedGroupUsers, user.userId);
      }
    };

    const handleActiveSelectedGroupUsers = (e: ChangeEvent<HTMLInputElement>, user: IChatUser): void => {
      if (e.target.checked) {
        setSelectedActiveGroupUsers(prev => [...prev, user])
      } else {
        removeUser(selectedActiveGroupUsers, setSelectedActiveGroupUsers, user.userId);
      }
    };
    
    const removeUser = <T extends { userId: number }> (collection: T[], setCollection: React.Dispatch<React.SetStateAction<T[]>>, userId: number): void => {
        let toRemove = collection.find(u => u.userId == userId);

        if (toRemove) {
          setCollection((prev) => prev.filter(user => user.userId !== toRemove!.userId));
        }
    };

    const handleChatNameUpdate = async (chatId: number, chatTitle: string): Promise<void> => {
      try {
        await fetch(`${process.env.REACT_APP_API_BASE_URL}/api/chats/chat/${chatId}`, {
          method: 'PATCH',
          headers: {
            'Content-Type': 'application/json',
            Authorization: `Bearer ${localStorage.getItem("token")}`
          },
          body: JSON.stringify(chatTitle)
        });

        setChatRefreshTrigger(prev => prev + 1);
        setActiveChat(prev => prev && prev.id === chatId ? { ...prev, name: chatTitle } : prev);
      } catch (err: any) {
        console.log(err);
        setError(err);
      }
    };

    const handleUpdateChatUsers = async (chatId: number, userIdList: IChatUser[], action: string): Promise<void> => {
      try {
        const response = await fetch(`${process.env.REACT_APP_API_BASE_URL}/api/chats/chat/${chatId}/users?action=${action}`, {
          method: 'PATCH',
          headers: {
            'Content-Type': 'application/json',
            Authorization: `Bearer ${localStorage.getItem("token")}`
          },
          body: JSON.stringify(userIdList.map(x => x.userId))
        });

        const updatedChat: IChat = await response.json();

        if (!response.ok) {
          throw new Error(`Failed to add users: ${response.status}`);
        }

        setActiveChat(updatedChat);
        setChats(prev => prev.map(c => c.id === chatId ? updatedChat : c));
        setChatRefreshTrigger(prev => prev + 1);
      } catch (err: any) {
        console.log(err);
        setError(err);
      } finally {
        setSelectedGroupUsers([]);
        setSelectedActiveGroupUsers([]);
      }
    };

    const handleFileReset = () => {
        if (inputFile.current) {
            inputFile.current.value = null;
        }
    };

    if (error) return <p>Error: {error}</p>

    try {
      return (
          <section className="chat-page">
              <div className="chat-layout">
                <div className="chat-column chat-column--left">
                  <h2 className="section-title">Start Chat</h2>
                  <Users
                    onNewChatCreated={(newChat) => {
                      setChats(prev => [...prev, newChat]);
                      setChatRefreshTrigger(prev => prev + 1);
                      setActiveChat(newChat);
                    }}
                    onFetchedUsers={(users) => {
                      setActiveUsers(users);
                    }}
                  />

                  <h2 className="section-title">Existing Chats</h2>
                  <Chats
                    refreshTrigger={chatRefreshTrigger}
                    onSelectChat={handleSelectChat}
                  />
              </div>

              <div className="chat-column chat-column--right">
                <h2 className="section-title">Active chat</h2>
                {activeChat ? (
                  <div className="chat-active">
                    <div className="chat-header">
                      <div className="chat-title-group">
                        <span className="chat-header-label">Chat Name</span>
                        {isEditingTitle ? (
                            <input
                              className="input chat-title-input"
                              type="text"
                              autoFocus
                              value={chatTitle}
                              onChange={(e) => setChatTitle(e.target.value)}
                            />
                          ) : (
                            <span className="chat-title">
                              {activeChat.name.replace(authData?.username!, "")}
                            </span>
                          )
                        }
                      </div>
                      <button
                        className="btn btn-secondary"
                        onClick={() => {
                          setIsEditingTitle(prev => !prev);
                          handleChatNameUpdate(activeChat.id, chatTitle);
                        }}
                      >
                        {isEditingTitle ? "Save" : "Edit"}
                      </button>
                      <button
                        className="btn btn-secondary"
                        onClick={handleShareLocation}
                      >
                        Share Location
                      </button>
                    </div>

                    <p className="chat-meta">
                      Created At:{" "}
                      {new Date(activeChat.createdAt).toLocaleString()}
                    </p>

                    <div className="chat-participants">
                      <h3 className="chat-subtitle">Participants</h3>
                      <ul className="chat-participants-list">
                        {activeChat.isGroup
                          ? activeChat.participants
                              .filter((p) => p.user.username !== authData?.username)
                              .map((p) => (
                                <li key={p.userId} className="chat-participant">
                                  <label>
                                    <span>{p.user.username}</span>
                                    <input
                                      type="checkbox"
                                      name="activeGroupUsersCheckbox"
                                      onChange={(e) =>
                                        handleActiveSelectedGroupUsers(e, {
                                          userId: p.user.id,
                                          user: p.user,
                                          chatId: activeChat.id,
                                          joinedAt: new Date()
                                        })}
                                    />
                                  </label>
                                </li>
                              ))
                          : activeChat.participants.find(
                              (p) => p.user.username !== authData?.username
                            )?.user.username}
                      </ul>
                    </div>

                    <div className="chat-group-actions">
                      {isGroup ? (
                        <div className="chat-group-panel">
                          <Search<IAppUser>
                            collection="users"
                            onResults={setActiveUsers}
                          />
                          <ul className="pill-list">
                            {selectedGroupUsers.map((user) => (
                              <li key={user.user.id} className="pill">
                                <span>{user.user.username}</span>
                                <button
                                  className="pill-remove"
                                  onClick={() =>
                                    removeUser(
                                      selectedGroupUsers,
                                      setSelectedGroupUsers,
                                      user.userId
                                    )
                                  }
                                >
                                  ×
                                </button>
                              </li>
                            ))}
                          </ul>
                          <div className="chat-group-users">
                            {activeUsers.map((user) => (
                              <label key={user.id} className="checkbox-row">
                                  <span>{user.username}</span>
                                  <input
                                    type="checkbox"
                                    name="groupCheckbox"
                                    onChange={(e) =>
                                      handleSelectedGroupUsers(e, {
                                        userId: user.id,
                                        user: user,
                                        chatId: activeChat.id,
                                        joinedAt: new Date()
                                      })
                                    }
                                  />
                              </label>
                            ))}
                          </div>
                          <button
                            className="btn btn-primary"
                            onClick={() => {
                              setIsGroup(prev => !prev);
                              handleUpdateChatUsers(
                                activeChat.id,
                                selectedGroupUsers,
                                "add"
                              );
                            }}
                          >
                            Add Selected Participants to Chat
                          </button>
                        </div>
                      ) : (
                        <div className="chat-group-toggle">
                          <button
                            className="btn btn-secondary"
                            onClick={() => setIsGroup(prev => !prev)}
                          >
                            Add More Participants to Chat
                          </button>
                          {selectedActiveGroupUsers.length > 0 ? (
                            <button
                              className="btn btn-ghost"
                              onClick={() => {
                                handleUpdateChatUsers(
                                  activeChat.id,
                                  selectedActiveGroupUsers,
                                  "remove"
                                );
                              }}
                            >
                              Remove Selected Participants from Chat
                            </button>
                          ) : null}
                        </div>
                      )}
                    </div>

                    <div className="chat-messages">
                      {activeChat.messages.map((msg) => (
                        <Message key={msg.chatId ?? msg.sentAt} msg={msg} />
                      ))}
                    </div>
                  </div>
                ) : (
                  <p className="chat-empty-state">No chat selected.</p>
                )}

                {activeChat ? (
                  <div className="chat-input-row">
                    <input
                        className="input chat-input"
                        type="text"
                        value={currentMessageContent}
                        onChange={(e) => setCurrentMessageContent(e.target.value)} 
                        placeholder="Type a message"
                    />
                    <label className="file-input-label">
                      <span>Attach</span>
                      <input
                        className="file-input"
                        type="file"
                        multiple
                        name="file-input"
                        ref={inputFile}
                        onChange={(e) => setAttachments(e.target.files)}
                      />
                    </label>
                    <button
                      className="btn btn-primary"
                      onClick={sendMessage}
                    >
                      Send
                    </button>
                  </div>
                ) : null}

                {attachments !== null && (
                  <ul className="attachment-preview-list">
                    {Array.from(attachments).map((file: File, index: number) => (
                      <li key={index} className="attachment-preview-item">
                        <span>{file.name}</span>
                        <img src={URL.createObjectURL(file)} alt={file.name} />
                      </li>
                    ))}
                  </ul>
                )}
              </div>
            </div>
          </section>
      )
    }
    catch (err){ console.log(err); }
    return <p>Something went wrong.</p>;
};

export default Chat;