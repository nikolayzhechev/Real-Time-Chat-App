interface IMessage {
    chatId: number,
    senderId: number,
    username: string,
    content: string,
    sentAt: Date
}

export default IMessage;