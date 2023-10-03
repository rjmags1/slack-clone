import Button from '../../../../lib/Button'

type ChannelMessageLaterFlagBtnProps = {
    laterFlagId?: string
}

function ChannelMessageLaterFlagBtn({
    laterFlagId,
}: ChannelMessageLaterFlagBtnProps) {
    return (
        <Button
            onClick={() => alert('not implemented')}
            className="h-fit rounded px-[.3rem] py-[1px] hover:bg-zinc-600"
        >
            ⚑
        </Button>
    )
}

export default ChannelMessageLaterFlagBtn
