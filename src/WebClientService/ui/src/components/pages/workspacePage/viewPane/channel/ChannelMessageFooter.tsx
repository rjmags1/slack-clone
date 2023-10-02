type ChannelMessageFooterProps = {
    lastEditUTC: string | null
    laterFlag: { id: string } | null
    reactions: Readonly<
        {
            count: number
            emoji: string
            id: string
            userReactionId: string | null
        }[]
    > | null
    threadId: string | null
}

function ChannelMessageFooter({
    lastEditUTC,
    laterFlag,
    reactions,
    threadId,
}: ChannelMessageFooterProps) {
    return null
}

export default ChannelMessageFooter
