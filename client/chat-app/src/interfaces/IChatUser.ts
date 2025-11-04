import IAppUser from "./IAppUser"

interface IChatUser {
    userId: number,
    user: IAppUser,
    chatId: number,
    joinedAt: Date
}

export default IChatUser;