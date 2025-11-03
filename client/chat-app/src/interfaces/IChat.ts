import IMessage from "./IMessage";

interface IChat {
    id: number,
    messages: IMessage[]
    name: string,
    participants: string[],
    createdAt: string,
    isGroup: boolean
}

export default IChat;