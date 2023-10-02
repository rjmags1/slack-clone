type ChannelMessageBodyProps = {
    content: string
    files: Readonly<{ id: string; storeKey: string }[]> | null
    mentions: Readonly<{ mentioned: { id: string } }[]> | null
}

function ChannelMessageBody({
    content,
    files,
    mentions,
}: ChannelMessageBodyProps) {
    return <div className="max-h-[150px] w-full">{content}</div>
}

export default ChannelMessageBody
