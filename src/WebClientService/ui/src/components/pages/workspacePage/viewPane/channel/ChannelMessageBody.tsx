type ChannelMessageBodyProps = {
    content: string
    files: Readonly<{ id: string; storeKey: string }[]> | null
    mentions: Readonly<{ mentionedId: string }[]> | null
}

function ChannelMessageBody({
    content,
    files,
    mentions,
}: ChannelMessageBodyProps) {
    return <div className="w-full">{content}</div>
}

export default ChannelMessageBody
