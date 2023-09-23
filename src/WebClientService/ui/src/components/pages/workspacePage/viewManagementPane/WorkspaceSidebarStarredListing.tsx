import { useFragment } from 'react-relay'
import { WorkspacePageSidebarStarredFragment$key } from '../../../../relay/fragments/__generated__/WorkspacePageSidebarStarredFragment.graphql'
import WorkspacePageSidebarStarredFragment from '../../../../relay/fragments/WorkspacePageSidebarStarred'

type WorkspacePageSidebarStarredListingProps = {
    starred: WorkspacePageSidebarStarredFragment$key
}

function WorkspaceSidebarStarredListing({
    starred,
}: WorkspacePageSidebarStarredListingProps) {
    const { name } = useFragment(WorkspacePageSidebarStarredFragment, starred)

    return (
        <div
            className="px-2"
            onClick={() => alert('not implemented')}
        >{`${name}`}</div>
    )
}

export default WorkspaceSidebarStarredListing
