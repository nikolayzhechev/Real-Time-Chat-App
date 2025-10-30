import { useState, useEffect, memo } from "react";
import IChat from "../interfaces/IChat";
import { useUser } from '../contexts/userContext';

interface ChatsProps {
  onSelectChat: (chat: IChat) => void;
  refreshTrigger: number;
}

const Chats = memo((props: ChatsProps) => {
    const { onSelectChat, refreshTrigger } = props;
    const [chats, setChats] = useState<IChat[]>([]);
    const [filteredChats, setFilteredChats] = useState<IChat[]>([]);
    const [loading, setLoading] = useState<boolean>(true);
    const [error, setError] = useState<string | null>(null);
    const [query, setQuery] = useState<string>("");
    const { authData } = useUser();

    const fetchChats = async (): Promise<void> => {
        setLoading(true);
        setError(null);

        try {
            const response: Response = await fetch(`${process.env.REACT_APP_API_BASE_URL}/api/chats/${authData?.id}`);
            if (!response.ok) throw new Error(`Unable to retreive chats ${response.status}`);

            const data: IChat[] = await response.json();
            setChats(data);
        } catch (error: any) {
            setError(error.message);
            console.log(error);
        } finally {
            setLoading(false);
        }
    };

    useEffect(() => {
        if (!authData?.id) return;

        fetchChats();
    }, [authData?.id, refreshTrigger]);

    useEffect(() => {
        const filtered = chats.filter(chat =>
            chat.name.toLowerCase().includes(query.toLowerCase())
        );

        setFilteredChats(filtered);
    }, [query]);

    const fetchChat = async (chatId: number): Promise<void> => {
        try {
            const response: Response = await fetch(`${process.env.REACT_APP_API_BASE_URL}/api/chats/chat/${chatId}`);
            if (!response.ok) throw new Error(`Error when retreiving chat with id ${chatId}: ${response.status}`);

            const chat: IChat = await response.json();
            onSelectChat(chat);
        } catch (error: any) {
            setError(error.message);
            console.log(error);
        }
    }

    const handleChatDeletion = async (chatId: number): Promise<void> => {
        setError(null);

        try {
            await fetch(`${process.env.REACT_APP_API_BASE_URL}/api/chats/delete/${chatId}`, {
                method: 'DELETE'
            });
        } catch (error: any) {
            setError(error.message);
            console.log(error);
        } finally {
            fetchChats();
        }
    }
    
    if (loading) return <p>Loading...</p>;
    if (error) return <p>Error: {error}</p>;

    return (
        <div>
            <form id="search-form"> 
                <input 
                    type="search" 
                    id="query"
                    name="q" 
                    placeholder={`Search chats...`}
                    onChange={(e) => {
                        e.preventDefault();
                        setQuery(e.target.value);
                    }}
                />
            </form>
            <ul>
                {query.length < 2 ?
                        chats.length > 0 ?
                        chats.map(chat => (
                            <li>
                                {chat.name.replace(authData?.username!, "")}
                                <button onClick={() => fetchChat(chat.id)}>Open Chat</button>
                                <button onClick={() => handleChatDeletion(chat.id)} >X</button>
                            </li>
                        ))
                        :
                        <p>There are no existing chats. Please start a new one.</p>
                    :
                    filteredChats.length > 0 ?
                        filteredChats.map(chat => (
                            <li>
                                {chat.name.replace(authData?.username!, "")}
                                <button onClick={() => fetchChat(chat.id)}>Open Chat</button>
                                <button onClick={() => handleChatDeletion(chat.id)} >X</button>
                            </li>
                        )) :
                        <p>No search results.</p>}
            </ul>
        </div>
    );
});

export default Chats;