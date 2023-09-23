import { useFragment } from 'react-relay'
import WorkspacePageSidebarChannelFragment from '../../../../relay/fragments/WorkspacePageSidebarChannel'
import { WorkspacePageSidebarChannelFragment$key } from '../../../../relay/fragments/__generated__/WorkspacePageSidebarChannelFragment.graphql'

type WorkspaceSidebarChannelListingProps = {
    channel: WorkspacePageSidebarChannelFragment$key
}

function WorkspaceSidebarChannelListing({
    channel,
}: WorkspaceSidebarChannelListingProps) {
    const { name } = useFragment(WorkspacePageSidebarChannelFragment, channel)

    return (
        <div
            className="px-2"
            onClick={() => alert('not implemented')}
        >{`#${name}`}</div>
    )
}

export default WorkspaceSidebarChannelListing
