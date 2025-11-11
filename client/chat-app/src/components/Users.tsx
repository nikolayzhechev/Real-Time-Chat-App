import { useState, useEffect, memo } from "react";
import { useUser } from '../contexts/userContext';
import IAppUser from "../interfaces/IAppUser";
import IChat from "../interfaces/IChat";

interface UsersProps {
  onNewChatCreated: (chat: IChat) => void;
  onFetchedUsers: (users: IAppUser[]) => void;
}

const Users = memo(({ onNewChatCreated, onFetchedUsers }: UsersProps) => {
  const [usersList, setUsersList] = useState<IAppUser[]>([]);
  const [selectedUsers, setSelectedUsers] = useState<IAppUser[]>([]);
  const [filteredUsersList, setFilteredUsersList] = useState<IAppUser[]>([]);
  const [loading, setLoading] = useState<boolean>(true);
  const [error, setError] = useState<string | null>(null);
  const [checkedItems, setcheckedItems] = useState<{ [key: number]: boolean }>({});
  const [query, setQuery] = useState<string>("");
  const [chatName, setChatName] = useState<string>("");
  const { authData } = useUser();

    useEffect(() => {
      if (!authData?.id) return;
      
      setLoading(true);
      setError(null);

      const fetchUsersLists = async (): Promise<void> => {
        try {
          const response: Response = await fetch(`${process.env.REACT_APP_API_BASE_URL}/api/users/${authData?.id}`);
          if (!response.ok) throw new Error(`User retreival error: ${response.status}`);

            const data: IAppUser[] = await response.json();
            setUsersList(data);
            onFetchedUsers(data);
        } catch (error: any) {
          setError(error.message);
        } finally {
          setLoading(false);
        }
      };

      fetchUsersLists();
    }, [authData?.id]);

    useEffect(() => {
      const filtered = usersList.filter(user =>
        user.username.toLowerCase().includes(query.toLowerCase())
      );

      setFilteredUsersList(filtered);
    }, [query]);

    const handleChatCreation = async (user: IAppUser, isGroupParam: boolean): Promise<void> => {
      try {
          const response: Response = await fetch(`${process.env.REACT_APP_API_BASE_URL}/api/chats/chat`, {
            method: 'POST',
            headers: {
              'Content-Type': 'application/json',
              Authorization: `Bearer ${localStorage.getItem("token")}`
            },
            body: JSON.stringify({
                title: `Chat with ${user.username} ${authData?.username}`,
                participantIds: [user.id, authData?.id],
                isGroup: isGroupParam
            })
          });

          if (!response.ok) throw new Error(`Chat creation error: ${response.status}`);

          const data = await response.json();
          onNewChatCreated(data);
          console.log('Chat created successfully:', data);
      } catch (error: any) {
        setError(error.message);
      }
    };

    const handleGroupChatCreation = async (chatName: string, participants: number[], isGroupParam: boolean): Promise<void> => {
      if (!participants.includes(authData!.id)) {
        participants.push(authData!.id);
      }

      try {
          const response: Response = await fetch(`${process.env.REACT_APP_API_BASE_URL}/api/chats/chat`, {
            method: 'POST',
            headers: {
              'Content-Type': 'application/json',
              Authorization: `Bearer ${localStorage.getItem("token")}`
            },
            body: JSON.stringify({
                title: chatName,
                participantIds: participants,
                isGroup: isGroupParam
            })
          });

          if (!response.ok) throw new Error(`Chat creation error: ${response.status}`);

          const data = await response.json();
          onNewChatCreated(data);
          console.log('Chat created successfully:', data);
      } catch (error: any) {
        setError(error.message);
      } finally {
        setSelectedUsers([]);
        setFilteredUsersList([]);
        setcheckedItems({});
        setChatName("");
      }
    };

    const handleSelectedUsers = (e: React.ChangeEvent<HTMLInputElement>, user: IAppUser): void => {
      if (e.target.checked) {
        setSelectedUsers(prev => [...prev, user]);
      } else {
        removeUser(user);
      }
    };

    const removeUser = (user: IAppUser): void => {
        let toRemove: IAppUser | undefined = selectedUsers.find(u => u.username == user.username);
        
        if (toRemove) {
          setSelectedUsers((prev) => prev.filter(user => user !== toRemove));
        }
    };
    
    const handleCheckboxChange = (id: number) => {
      setcheckedItems(prev => ({
        ...prev,
        [id]: !prev[id]
      }));
    };

    if (loading) return <p>Loading...</p>;
    if (error) return <p>Error: {error}</p>;

    return (
        <div>
          <form id="search-form"> 
            <input 
                type="search"
                id="query"
                name="q"
                placeholder={`Search users...`}
                onChange={(e) => {
                  e.preventDefault();
                  setQuery(e.target.value);
                }}
            />
          </form>
          <p>Users:</p>
            <ul>
              {query.length < 2 ? usersList.map(user => (
                <li key={user.id}>
                  <label>
                    {user.username}
                    <input
                      type="checkbox"
                      name="usersCheckbox"
                      checked={!!checkedItems[user.id]}
                      onChange={(e) => {
                        handleSelectedUsers(e, user);
                        handleCheckboxChange(user.id);
                      }}/>
                  </label>
                  <button onClick={() => {
                    handleChatCreation(user, false);
                  }}>Start Chat</button>
                </li>
              ))
              :
              filteredUsersList.length > 0 ? filteredUsersList.map(user => (
                <li key={user.id}>
                  <p>{user.username}</p>
                  <button onClick={() => {
                    handleChatCreation(user, false);
                  }}>Start Chat</button>
                </li>
              )) : <p>No search results.</p>}
            </ul>
            <ul>
            {selectedUsers.map((user) => (
              <li key={user.id}>
                {user.username}
                <button onClick={(e) => {
                  removeUser(user);
                  handleCheckboxChange(user.id);
                }}>x</button>
              </li>
              ))}
            </ul>
            <div>
              <label>Chat Name</label>
              <input
                type="text"
                placeholder="Enter chat name"
                value={chatName}
                onChange={(e) => setChatName(e.target.value)}
              ></input>
              <button
                onClick={() => {
                  handleGroupChatCreation(chatName, selectedUsers.map(x => x.id), true);
                }}
                disabled={chatName === "" ? true : false}
              >New Chat</button> 
            </div>
        </div>
    )
});

export default Users;