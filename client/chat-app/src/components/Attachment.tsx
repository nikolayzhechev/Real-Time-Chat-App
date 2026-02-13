import IAttachment from "../interfaces/IAttachment";

function Attachment({ file }: { file: IAttachment }) {
  const isImage = file.fileType.includes("image");
  const url = `${process.env.REACT_APP_API_BASE_URL}/${file.fileUrl.replace("\\", "/")}`;

  return (
    <div className="attachment">
      {isImage ? (
        <img className="attachment-image" src={url} alt={file.fileName} />
      ) : (
        <a className="attachment-link" href={url} download>
          {file.fileName}
        </a>
      )}
    </div>
  );
}

export default Attachment;