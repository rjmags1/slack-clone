import Button from '../../../lib/Button'

type MessageLaterFlagBtnProps = {
    laterFlagId?: string
}

function MessageLaterFlagBtn({ laterFlagId }: MessageLaterFlagBtnProps) {
    return (
        <Button
            onClick={() => alert('not implemented')}
            className="h-fit rounded px-[.3rem] py-[1px] hover:bg-zinc-600"
        >
            ⚑
        </Button>
    )
}

export default MessageLaterFlagBtn
