import { useFragment } from 'react-relay'
import { WorkspacePageSidebarStarredFragment$key } from '../../../../relay/fragments/__generated__/WorkspacePageSidebarStarredFragment.graphql'
import WorkspacePageSidebarStarredFragment from '../../../../relay/fragments/WorkspacePageSidebarStarred'
import { useNavigate, useParams } from 'react-router-dom'

type WorkspacePageSidebarStarredListingProps = {
    starred: WorkspacePageSidebarStarredFragment$key
    id: string
}

export enum StarredType {
    Channel = 'Channel',
    DirectMessageGroup = 'DirectMessageGroup',
}

function WorkspaceSidebarStarredListing({
    starred,
    id,
}: WorkspacePageSidebarStarredListingProps) {
    const { workspaceId } = useParams()
    const navigate = useNavigate()
    const { name, __typename } = useFragment(
        WorkspacePageSidebarStarredFragment,
        starred
    )
    const starredType = __typename === StarredType.Channel ? 'channel' : 'dms'

    return (
        <div
            className="px-2"
            onClick={() => {
                const url = `/workspace/${workspaceId}/${starredType}/${id}`
                navigate(url)
            }}
        >{`${starredType === 'channel' ? '# ' : ''}${name}`}</div>
    )
}

export default WorkspaceSidebarStarredListing
