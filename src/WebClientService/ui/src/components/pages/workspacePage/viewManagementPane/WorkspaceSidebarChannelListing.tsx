import { useFragment } from 'react-relay'
import WorkspacePageSidebarChannelFragment from '../../../../relay/fragments/WorkspacePageSidebarChannel'
import { WorkspacePageSidebarChannelFragment$key } from '../../../../relay/fragments/__generated__/WorkspacePageSidebarChannelFragment.graphql'
import { useNavigate, useParams } from 'react-router-dom'

type WorkspaceSidebarChannelListingProps = {
    channel: WorkspacePageSidebarChannelFragment$key
    id: string
}

function WorkspaceSidebarChannelListing({
    channel,
    id,
}: WorkspaceSidebarChannelListingProps) {
    const { workspaceId } = useParams()
    const navigate = useNavigate()
    const { name } = useFragment(WorkspacePageSidebarChannelFragment, channel)

    return (
        <div
            className="px-2"
            onClick={() => {
                const url = `/workspace/${workspaceId}/channel/${id}`
                navigate(url)
            }}
        >{`#${name}`}</div>
    )
}

export default WorkspaceSidebarChannelListing
