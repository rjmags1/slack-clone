type DirectMessageBodyProps = {
    content: string
    files: Readonly<{ id: string; storeKey: string }[]> | null
    mentions: Readonly<{ mentionedId: string }[]> | null
}

function DirectMessageBody({
    content,
    files,
    mentions,
}: DirectMessageBodyProps) {
    return <div className="w-full">{content}</div>
}

export default DirectMessageBody
