type ChannelMessageHeaderProps = {
    user: {
        id: string
        username: string
        avatar: {
            id: string
            storeKey: string
        }
    } | null
    sentAtUTC: string
}

function ChannelMessageHeader({ user, sentAtUTC }: ChannelMessageHeaderProps) {
    return null
}

export default ChannelMessageHeader
