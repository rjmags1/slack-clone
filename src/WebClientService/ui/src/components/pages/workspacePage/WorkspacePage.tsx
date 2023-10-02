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
        <div className="h-full w-full">
            <WorkspacePageNavbar data={data} />
            <div className="flex h-[calc(100%_-_2.5rem)] w-full bg-zinc-800">
                <WorkspacePageViewManagementPane data={data} />
                <WorkspacePageViewPane />
            </div>
        </div>
    )
}

export default WorkspacePage
