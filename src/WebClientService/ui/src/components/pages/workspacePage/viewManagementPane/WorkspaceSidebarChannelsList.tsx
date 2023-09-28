import ListBox from '../../../lib/Listbox'
import { Item } from 'react-stately'
import WorkspaceSidebarChannelListing from './WorkspaceSidebarChannelListing'
import { WorkspacePageSidebarChannelsFragment$key } from '../../../../relay/fragments/__generated__/WorkspacePageSidebarChannelsFragment.graphql'
import WorkspacePageSidebarChannelsFragment from '../../../../relay/fragments/WorkspacePageSidebarChannels'
import { usePaginationFragment } from 'react-relay'
import SidebarLoadMoreBtn from './SidebarLoadMoreBtn'

type WorkspaceSidebarChannelsListProps = {
    channels: WorkspacePageSidebarChannelsFragment$key
}

function WorkspaceSidebarChannelsList({
    channels,
}: WorkspaceSidebarChannelsListProps) {
    const { data, loadNext, isLoadingNext, hasNext } = usePaginationFragment(
        WorkspacePageSidebarChannelsFragment,
        channels
    )

    return (
        <div
            className="no-scrollbar overflow-y-auto"
            id="channels-list-container"
        >
            <ListBox
                items={data.channels.edges}
                selectionMode="single"
                listClassName="text-xs"
            >
                {(item) => (
                    <Item key={item.node.id}>
                        <WorkspaceSidebarChannelListing channel={item.node} />
                    </Item>
                )}
            </ListBox>
            {!isLoadingNext && hasNext && (
                <SidebarLoadMoreBtn loadMore={() => loadNext(35)} />
            )}
        </div>
    )
}

export default WorkspaceSidebarChannelsList
