import IAttachment from "./IAttachment";

interface IMessage {
    id: number,
    chatId: number,
    senderId: number,
    username: string,
    content: string,
    sentAt: Date,
    attachments: IAttachment[],
    latitude: number | null,
    longitude: number | null
}

export default IMessage;