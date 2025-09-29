import IMessage from "./IMessage";

interface IChat {
    id: number,
    messages: IMessage[]
    name: string,
    createdAt: string
}

export default IChat;