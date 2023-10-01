import { useFragment } from 'react-relay'
import { WorkspacePageSidebarDirectMessageGroupFragment$key } from '../../../../relay/fragments/__generated__/WorkspacePageSidebarDirectMessageGroupFragment.graphql'
import WorkspacePageSidebarDirectMessageGroupFragment from '../../../../relay/fragments/WorkspacePageSidebarDirectMessageGroup'
import { useNavigate, useParams } from 'react-router-dom'

type WorkspaceSidebarDirectMessageGroupListingProps = {
    group: WorkspacePageSidebarDirectMessageGroupFragment$key
    id: string
}

function WorkspaceSidebarDirectMessagesListing({
    group,
    id,
}: WorkspaceSidebarDirectMessageGroupListingProps) {
    const { workspaceId } = useParams()
    const navigate = useNavigate()
    const { name } = useFragment(
        WorkspacePageSidebarDirectMessageGroupFragment,
        group
    )

    return (
        <div
            className="px-2"
            onClick={() => {
                const url = `/workspace/${workspaceId}/dms/${id}`
                navigate(url)
            }}
        >{`${name}`}</div>
    )
}

export default WorkspaceSidebarDirectMessagesListing
