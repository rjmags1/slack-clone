import { useFragment } from 'react-relay'
import { DirectMessageFragment$key } from '../../../../../relay/fragments/__generated__/DirectMessageFragment.graphql'
import DirectMessageFragment from '../../../../../relay/fragments/DirectMessage'
import { createContext } from 'react'
import { MessageContextType } from '../channel/ChannelMessage'
import DirectMessageHeader from './DirectMessageHeader'
import DirectMessageBody from './DirectMessageBody'
import DirectMessageFooter from './DirectMessageFooter'

type DirectMessageProps = {
    id: string
    message: DirectMessageFragment$key
}

export const DirectMessageContext = createContext<MessageContextType>(null)

function DirectMessage({ id, message }: DirectMessageProps) {
    const {
        user,
        content,
        sentAtUTC,
        lastEditUTC,
        files,
        laterFlag,
        mentions,
        reactions,
        replyToId,
    } = useFragment(DirectMessageFragment, message)

    return (
        <DirectMessageContext.Provider
            value={{ messageId: id, authorId: user?.id || null, content }}
        >
            <div
                id={id}
                className="m-1 flex w-full flex-col items-start justify-start 
                gap-y-1 rounded px-3 py-2 text-sm text-white hover:bg-zinc-700"
            >
                <DirectMessageHeader
                    user={user}
                    sentAtUTC={sentAtUTC!}
                    replyToId={replyToId}
                />
                <DirectMessageBody
                    content={content}
                    files={files}
                    mentions={mentions}
                />
                <DirectMessageFooter
                    lastEditUTC={lastEditUTC}
                    laterFlag={laterFlag}
                    reactions={reactions}
                />
            </div>
        </DirectMessageContext.Provider>
    )
}

export default DirectMessage
