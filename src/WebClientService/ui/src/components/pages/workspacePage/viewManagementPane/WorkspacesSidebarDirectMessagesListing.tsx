import { useFragment } from 'react-relay'
import { WorkspacePageSidebarDirectMessageGroupFragment$key } from '../../../../relay/fragments/__generated__/WorkspacePageSidebarDirectMessageGroupFragment.graphql'
import WorkspacePageSidebarDirectMessageGroupFragment from '../../../../relay/fragments/WorkspacePageSidebarDirectMessageGroup'

type WorkspaceSidebarDirectMessageGroupListingProps = {
    group: WorkspacePageSidebarDirectMessageGroupFragment$key
}

function WorkspaceSidebarDirectMessagesListing({
    group,
}: WorkspaceSidebarDirectMessageGroupListingProps) {
    const { name } = useFragment(
        WorkspacePageSidebarDirectMessageGroupFragment,
        group
    )

    return (
        <div
            className="px-2"
            onClick={() => alert('not implemented')}
        >{`${name}`}</div>
    )
}

export default WorkspaceSidebarDirectMessagesListing
