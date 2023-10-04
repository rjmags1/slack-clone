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
import DmGroupMessageEditor from './DmGroupMessageEditor'
import type { DirectMessageGroupQuery as DirectMessageGroupQueryType } from '../../../../../relay/queries/__generated__/DirectMessageGroupQuery.graphql'
import DirectMessageGroupQuery from '../../../../../relay/queries/DirectMessageGroup'

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
    console.log(data)

    return (
        <Suspense fallback={<LoadingSpinner />}>
            <div className="flex h-full w-full flex-col">
                <DmGroupViewPaneContentHeader //headerInfo={data.viewDirectMessageGroup!}
                />
                <DmGroupViewPaneMessageList //messages={data.viewChannel!}
                />
                <DmGroupMessageEditor />
            </div>
        </Suspense>
    )
}

export default DirectMessageGroupViewPaneContent
