import { useFragment } from 'react-relay'
import ChannelMessageFragment from '../../../../../relay/fragments/ChannelMessage'
import { ChannelMessageFragment$key } from '../../../../../relay/fragments/__generated__/ChannelMessageFragment.graphql'
import ChannelMessageFooter from './ChannelMessageFooter'
import ChannelMessageBody from './ChannelMessageBody'
import ChannelMessageHeader from './ChannelMessageHeader'

type ChannelMessageProps = {
    message: ChannelMessageFragment$key
    id: string
}

function ChannelMessage({ message, id }: ChannelMessageProps) {
    const {
        user,
        content,
        sentAtUTC,
        lastEditUTC,
        files,
        laterFlag,
        mentions,
        reactions,
        threadId,
    } = useFragment(ChannelMessageFragment, message)

    return (
        <div
            className="flex w-full flex-col items-start justify-start 
                px-2 py-1"
        >
            <ChannelMessageHeader user={user} sentAtUTC={sentAtUTC!} />
            <ChannelMessageBody
                content={content}
                files={files}
                mentions={mentions}
            />
            <ChannelMessageFooter
                lastEditUTC={lastEditUTC}
                laterFlag={laterFlag}
                reactions={reactions}
                threadId={threadId}
            />
        </div>
    )
}

export default ChannelMessage
