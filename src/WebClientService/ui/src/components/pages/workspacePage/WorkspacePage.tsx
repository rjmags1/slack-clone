import { useContext } from 'react'
import WorkspacePageNavbar from './navbar/WorkspacePageNavbar'
import WorkspacePageViewManagementPane from './viewManagementPane/WorkspacePageViewManagementPane'
import WorkspacePageViewPane, {
    ViewPaneContent,
} from './viewPane/WorkspacePageViewPane'
import { SessionContext, getSubClaim } from '../../session/SessionProvider'
import { useParams } from 'react-router-dom'
import { useLazyLoadQuery } from 'react-relay'
import WorkspacePageDataQuery from '../../../relay/queries/WorkspacePage'
import type { WorkspacePageQuery as WorkspacePageQueryType } from '../../../relay/queries/__generated__/WorkspacePageQuery.graphql'
import SignalRProvider from '../../session/SignalRProvider'

type WorkspacePageProps = {
    content?: ViewPaneContent
}

function WorkspacePage({ content }: WorkspacePageProps) {
    const claims = useContext(SessionContext)!
    const sub = getSubClaim(claims)
    const { workspaceId } = useParams() as { workspaceId: string }
    const baseFilter = { workspaceId, userId: sub }
    const data = useLazyLoadQuery<WorkspacePageQueryType>(
        WorkspacePageDataQuery,
        {
            userId: sub,
            workspaceId,
            channelsFilter: baseFilter,
            directMessageGroupsFilter: baseFilter,
            starredFilter: baseFilter,
        }
    )

    return (
        <SignalRProvider>
            <div className="h-full w-full">
                <WorkspacePageNavbar data={data} />
                <div
                    className="flex h-[calc(100vh_-_2.5rem)] w-[100vw] 
                    overflow-hidden bg-zinc-800"
                >
                    <WorkspacePageViewManagementPane data={data} />
                    <WorkspacePageViewPane />
                </div>
            </div>
        </SignalRProvider>
    )
}

export default WorkspacePage
