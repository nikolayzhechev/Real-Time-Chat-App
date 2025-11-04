import IChatUser from "./IChatUser";
import IMessage from "./IMessage";

interface IChat {
    id: number,
    messages: IMessage[]
    name: string,
    participants: IChatUser[],
    createdAt: string,
    isGroup: boolean
}

export default IChat;