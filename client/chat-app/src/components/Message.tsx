import Attachment from "./Attachment";
import IMessage from "../interfaces/IMessage";

function Message({ msg }: { msg: IMessage }) {
  if (msg.latitude && msg.longitude) {
    const url = `https://www.google.com/maps?q=${msg.latitude},${msg.longitude}`;
    return (
      <div className="message location-message">
        <a href={url} target="_blank" rel="noopener noreferrer">
          View shared location
        </a>
      </div>
    );
  } else {
    return (
      <div className="message">
        <div className="message-header">
          <span className="message-author">{msg.username}</span>
          <span className="message-time">
            {new Date(msg.sentAt).toLocaleString()}
          </span>
        </div>
        <p className="message-content">{msg.content}</p>
        
        {msg.attachments?.length > 0 && (
          <div className="attachments">
            {msg.attachments.map((file) => (
              <Attachment key={file.id ?? file.fileName} file={file} />
            ))}
          </div>
        )}
      </div>
    );
  }
}

export default Message;