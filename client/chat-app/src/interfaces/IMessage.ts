import IAttachment from "./IAttachment";

interface IMessage {
    chatId: number,
    senderId: number,
    username: string,
    content: string,
    sentAt: Date,
    attachments: IAttachment[]
}

export default IMessage;