import { useFragment } from 'react-relay'
import ChannelMessageFragment from '../../../../../relay/fragments/ChannelMessage'
import { ChannelMessageFragment$key } from '../../../../../relay/fragments/__generated__/ChannelMessageFragment.graphql'
import ChannelMessageFooter from './ChannelMessageFooter'
import ChannelMessageBody from './ChannelMessageBody'
import ChannelMessageHeader from './ChannelMessageHeader'
import { createContext } from 'react'

type ChannelMessageProps = {
    message: ChannelMessageFragment$key
    id: string
}

type MessageContextType = {
    messageId: string
    authorId: string | null
    content: string
} | null

export const MessageContext = createContext<MessageContextType>(null)

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
        <MessageContext.Provider
            value={{ messageId: id, authorId: user?.id || null, content }}
        >
            <div
                className="m-1 flex w-full flex-col items-start justify-start 
                gap-y-1 rounded px-3 py-2 text-sm text-white hover:bg-zinc-700"
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
        </MessageContext.Provider>
    )
}

export default ChannelMessage
