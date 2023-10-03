import { useContext } from 'react'
import { ChannelPermissionsContext } from './ChannelViewPaneContent'
import ChannelMessageReplyBtn from './ChannelMessageReplyBtn'
import ChannelMessageLaterFlagBtn from './ChannelMessageLaterFlagBtn'
import ChannelMessageReactionBtn from './ChannelMessageReactionBtn'

type ChannelMessageInteractionBarProps = {
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

const DEFAULT_EMOJIS = ['🙋', '😃', '👍', '😠', '👎']
const MAX_EMOJIS = 16

function ChannelMessageInteractionBar({
    laterFlag,
    reactions,
}: ChannelMessageInteractionBarProps) {
    const { allowThreads, allowedPostersMask } = useContext(
        ChannelPermissionsContext
    )!
    const reactions_ = (reactions || []) as any[]
    reactions_.sort((r1, r2) => r2.count - r1.count)
    const includedEmojis = reactions_.map((r) => r.emoji)
    const filteredDefaultEmojis = DEFAULT_EMOJIS.filter(
        (defaultEmoji) => !includedEmojis.includes(defaultEmoji)
    )
    const renderedReactions = [
        ...reactions_.slice(0, MAX_EMOJIS - filteredDefaultEmojis.length),
        ...filteredDefaultEmojis.map((emoji) => ({
            count: 0,
            emoji,
            id: crypto.randomUUID(),
            userReactionId: null,
        })),
    ]

    return (
        <div
            className="flex h-max gap-x-1 rounded border border-zinc-600 
                text-[.7rem]"
        >
            {allowThreads && <ChannelMessageReplyBtn />}
            <ChannelMessageLaterFlagBtn laterFlagId={laterFlag?.id} />
            <div className="min-h-full min-w-[1px] bg-zinc-600" />
            {renderedReactions.map((reaction) => (
                <ChannelMessageReactionBtn reaction={reaction} />
            ))}
        </div>
    )
}

export default ChannelMessageInteractionBar
