import ListBox from '../../../lib/Listbox'
import { Item } from 'react-stately'
import WorkspaceSidebarChannelListing from './WorkspaceSidebarChannelListing'
import { WorkspacePageSidebarChannelsFragment$key } from '../../../../relay/fragments/__generated__/WorkspacePageSidebarChannelsFragment.graphql'
import WorkspacePageSidebarChannelsFragment from '../../../../relay/fragments/WorkspacePageSidebarChannels'
import { usePaginationFragment } from 'react-relay'
import Button from '../../../lib/Button'

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
                <Button
                    className="mt-1 w-full rounded-md bg-sky-800 py-1 
                        text-xs hover:bg-sky-950"
                    onClick={() => loadNext(35)}
                >
                    Load more
                </Button>
            )}
        </div>
    )
}

export default WorkspaceSidebarChannelsList
