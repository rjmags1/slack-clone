type DirectMessageBodyProps = {
    content: string
    files: Readonly<{ id: string; storeKey: string }[]> | null
    mentions: Readonly<{ mentioned: { id: string } }[]> | null
}

function DirectMessageBody({
    content,
    files,
    mentions,
}: DirectMessageBodyProps) {
    return null
}

export default DirectMessageBody
