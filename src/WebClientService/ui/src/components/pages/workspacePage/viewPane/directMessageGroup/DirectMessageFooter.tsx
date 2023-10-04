type DirectMessageFooterProps = {
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
}

function DirectMessageFooter({
    lastEditUTC,
    laterFlag,
    reactions,
}: DirectMessageFooterProps) {
    return null
}

export default DirectMessageFooter
