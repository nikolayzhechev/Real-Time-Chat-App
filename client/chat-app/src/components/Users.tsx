import { ReactElement, useState, useEffect, memo } from "react";
import { useUser } from '../contexts/userContext';
import IAppUser from "../interfaces/IAppUser";
import IChat from "../interfaces/IChat";

interface UsersProps {
  onNewChatCreated: (chat: IChat) => void;
}

const Users = memo(({ onNewChatCreated }: UsersProps) => {
  const [usersList, setUsersList] = useState<IAppUser[]>([]);
  const [loading, setLoading] = useState<boolean>(true);
  const [error, setError] = useState<string | null>(null);
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
        } catch (error: any) {
          setError(error.message);
        } finally {
          setLoading(false);
        }
      };

      fetchUsersLists();
    }, [authData?.id]);

    const handleChatCreation = async (user: IAppUser): Promise<void> => {
      try {
        const response: Response = await fetch(`${process.env.REACT_APP_API_BASE_URL}/api/chats/chat`, {
          method: 'POST',
          headers: {
            'Content-Type': 'application/json',
            Authorization: `Bearer ${localStorage.getItem("token")}`
          },
          body: JSON.stringify({
              title: `Chat with ${user.username} ${authData?.username}`,
              participantIds: [user.id, authData?.id]
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

    if (loading) return <p>Loading...</p>;
    if (error) return <p>Error: {error}</p>;

    return (
        <div>
        <p>Users:</p>
            <ul>
              {usersList.map(user => (
                <li key={user.id}>
                  <p>{user.username}</p>
                  <button onClick={() => handleChatCreation(user)}>Start New Chat</button>
                </li>
              ))}
            </ul>
        </div>
    )
});

export default Users;