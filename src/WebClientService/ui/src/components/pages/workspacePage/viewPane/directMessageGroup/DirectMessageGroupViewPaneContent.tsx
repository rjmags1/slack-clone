import { Suspense, useContext } from 'react'
import {
    SessionContext,
    getSubClaim,
} from '../../../../session/SessionProvider'
import { useParams } from 'react-router-dom'
import { useLazyLoadQuery } from 'react-relay'
import LoadingSpinner from '../../../../lib/LoadingSpinner'
import DmGroupViewPaneContentHeader from './DmGroupViewPaneContentHeader'
import DmGroupViewPaneMessageList from './DmGroupViewPaneMessageList'
import type { DirectMessageGroupQuery as DirectMessageGroupQueryType } from '../../../../../relay/queries/__generated__/DirectMessageGroupQuery.graphql'
import DirectMessageGroupQuery from '../../../../../relay/queries/DirectMessageGroup'
import MessageEditor from '../../../../lib/MessageEditor'
import SendButton from '../../../../lib/SendButton'

function DirectMessageGroupViewPaneContent() {
    const { directMessageGroupId, workspaceId } = useParams()
    const claims = useContext(SessionContext)!
    const sub = getSubClaim(claims)
    const data = useLazyLoadQuery<DirectMessageGroupQueryType>(
        DirectMessageGroupQuery,
        {
            userId: sub,
            directMessageGroupId: directMessageGroupId!,
        },
        {
            fetchPolicy: 'network-only',
        }
    )

    return (
        <Suspense fallback={<LoadingSpinner />}>
            <div className="flex h-full w-full flex-col">
                <DmGroupViewPaneContentHeader
                    headerInfo={data.viewDirectMessageGroup!}
                />
                <DmGroupViewPaneMessageList
                    messages={data.viewDirectMessageGroup!}
                />
                <MessageEditor />
                <div className="border-t-1 h-fit w-full bg-zinc-900 px-2 py-1">
                    <SendButton
                        className="h-max w-max justify-end rounded-md 
                            bg-sky-800 px-2 py-1 text-xs text-white 
                            hover:bg-sky-900"
                    />
                </div>
            </div>
        </Suspense>
    )
}

export default DirectMessageGroupViewPaneContent
