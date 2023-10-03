import { useContext } from 'react'
import { MessageContext } from './ChannelMessage'
import Button from '../../../../lib/Button'

function ChannelMessageReplyBtn() {
    const { messageId } = useContext(MessageContext)!

    return (
        <Button
            onClick={() => alert('not implemented')}
            className="-mt-[1px] h-fit rounded px-[.45rem] py-[1px] 
                hover:bg-zinc-600"
        >
            ↰
        </Button>
    )
}

export default ChannelMessageReplyBtn
