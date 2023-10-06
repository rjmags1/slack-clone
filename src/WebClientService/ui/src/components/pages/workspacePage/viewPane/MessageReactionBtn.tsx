import Button from '../../../lib/Button'

type MessageReactionBtnProps = {
    reaction: {
        count: number
        emoji: string
        id: string
        userReactionId: string | null
    }
}

function MessageReactionBtn({ reaction }: MessageReactionBtnProps) {
    const { emoji, count } = reaction
    return (
        <Button
            onClick={() => alert('not implemented')}
            className="h-fit rounded px-1 py-[1px] hover:bg-zinc-600"
        >
            <span>
                {emoji}
                {count > 0 && <span className="text-xxs">{` ${count}`}</span>}
            </span>
        </Button>
    )
}

export default MessageReactionBtn
