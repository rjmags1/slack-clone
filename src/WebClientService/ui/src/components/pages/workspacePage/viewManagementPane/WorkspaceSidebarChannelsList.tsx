import ListBox from '../../../lib/Listbox'
import { Item } from 'react-stately'
import WorkspaceSidebarChannelListing from './WorkspaceSidebarChannelListing'
import { WorkspacePageSidebarChannelsFragment$key } from '../../../../relay/fragments/__generated__/WorkspacePageSidebarChannelsFragment.graphql'
import WorkspacePageSidebarChannelsFragment from '../../../../relay/fragments/WorkspacePageSidebarChannels'
import { usePaginationFragment } from 'react-relay'

type WorkspaceSidebarChannelsListProps = {
    channels: WorkspacePageSidebarChannelsFragment$key
}

function WorkspaceSidebarChannelsList({
    channels,
}: WorkspaceSidebarChannelsListProps) {
    const { data, loadNext } = usePaginationFragment(
        WorkspacePageSidebarChannelsFragment,
        channels
    )

    return (
        <ListBox
            items={data.channels.edges}
            selectionMode="single"
            listClassName="no-scrollbar overflow-y-auto text-xs"
        >
            {(item) => (
                <Item key={item.node.id}>
                    <WorkspaceSidebarChannelListing channel={item.node} />
                </Item>
            )}
        </ListBox>
    )
}

export default WorkspaceSidebarChannelsList
