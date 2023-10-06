import { useLazyLoadQuery } from 'react-relay'
import ChannelViewPaneContentHeader from './ChannelViewPaneContentHeader'
import ChannelViewPaneMessageList from './ChannelViewPaneMessageList'
import ChannelQuery from '../../../../../relay/queries/Channel'
import { Suspense, createContext, useContext } from 'react'
import {
    SessionContext,
    getSubClaim,
} from '../../../../session/SessionProvider'
import { useParams } from 'react-router-dom'
import type { ChannelMessagesQuery as ChannelMessagesQueryType } from '../../../../../relay/queries/__generated__/ChannelMessagesQuery.graphql'
import LoadingSpinner from '../../../../lib/LoadingSpinner'
import MessageEditor from '../../../../lib/MessageEditor'
import SendButton from '../../../../lib/SendButton'

type ChannelPermissionsType = {
    allowedPostersMask: number
    allowThreads: boolean
}

export const ChannelPermissionsContext =
    createContext<ChannelPermissionsType | null>(null)

function ChannelViewPaneContent() {
    const { channelId, workspaceId } = useParams()
    const claims = useContext(SessionContext)!
    const sub = getSubClaim(claims)
    const data = useLazyLoadQuery<ChannelMessagesQueryType>(
        ChannelQuery,
        {
            userId: sub,
            channelId: channelId!,
            messagesFilter: {
                workspaceId,
                channelIds: [channelId!],
            },
        },
        {
            fetchPolicy: 'network-only',
        }
    )

    return (
        <Suspense fallback={<LoadingSpinner />}>
            <ChannelPermissionsContext.Provider
                value={{
                    allowedPostersMask: data.viewChannel!.allowedPostersMask,
                    allowThreads: data.viewChannel!.allowThreads,
                }}
            >
                <div className="flex h-full w-full flex-col">
                    <ChannelViewPaneContentHeader
                        headerInfo={data.viewChannel!}
                    />
                    <ChannelViewPaneMessageList messages={data.viewChannel!} />
                    <MessageEditor />
                    <div className="border-t-1 h-fit w-full bg-zinc-900 px-2 py-1">
                        <SendButton
                            className="h-max w-max justify-end rounded-md 
                            bg-sky-800 px-2 py-1 text-xs text-white 
                            hover:bg-sky-900"
                        />
                    </div>
                </div>
            </ChannelPermissionsContext.Provider>
        </Suspense>
    )
}

export default ChannelViewPaneContent
