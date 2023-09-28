import ListBox from '../../../lib/Listbox'
import { Item } from 'react-stately'
import WorkspaceSidebarDirectMessagesListing from './WorkspacesSidebarDirectMessagesListing'
import { WorkspacePageSidebarDirectMessageGroupsFragment$key } from '../../../../relay/fragments/__generated__/WorkspacePageSidebarDirectMessageGroupsFragment.graphql'
import { usePaginationFragment } from 'react-relay'
import WorkspacePageSidebarDirectMessageGroupsFragment from '../../../../relay/fragments/WorkspacePageSidebarDirectMessageGroups'
import SidebarLoadMoreBtn from './SidebarLoadMoreBtn'

type WorkspaceSidebarDirectMessagesListProps = {
    groups: WorkspacePageSidebarDirectMessageGroupsFragment$key
}

function WorkspaceSidebarDirectMessagesList({
    groups,
}: WorkspaceSidebarDirectMessagesListProps) {
    const { data, loadNext, isLoadingNext, hasNext } = usePaginationFragment(
        WorkspacePageSidebarDirectMessageGroupsFragment,
        groups
    )

    return (
        <div className="no-scrollbar overflow-y-auto">
            <ListBox
                items={data.directMessageGroups.edges}
                selectionMode="single"
                listClassName="text-xs"
            >
                {(item) => (
                    <Item key={item.node.id}>
                        <WorkspaceSidebarDirectMessagesListing
                            group={item.node}
                        />
                    </Item>
                )}
            </ListBox>
            {!isLoadingNext && hasNext && (
                <SidebarLoadMoreBtn loadMore={() => loadNext(35)} />
            )}
        </div>
    )
}

export default WorkspaceSidebarDirectMessagesList
