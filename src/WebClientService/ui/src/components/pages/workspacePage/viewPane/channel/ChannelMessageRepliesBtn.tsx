import { useState } from 'react'
import Button from '../../../../lib/Button'
import ExpandIconTriangle from '../../../../lib/ExpandIconTriangle'

type ChannelMessageRepliesBtnProps = {
    threadId: string
}

function ChannelMessageRepliesBtn({ threadId }: ChannelMessageRepliesBtnProps) {
    const [expand, setExpand] = useState(false)

    return (
        <Button
            className="h-max truncate rounded-md bg-sky-700 px-1 
                text-[.7rem] font-light hover:bg-sky-800"
            onClick={() => alert('not implemented')}
        >
            <span>View replies </span>
            <ExpandIconTriangle expand={expand} />
        </Button>
    )
}

export default ChannelMessageRepliesBtn
