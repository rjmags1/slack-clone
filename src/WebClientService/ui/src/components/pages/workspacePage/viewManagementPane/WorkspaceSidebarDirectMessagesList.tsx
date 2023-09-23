import ListBox from '../../../lib/Listbox'
import { Item } from 'react-stately'
import WorkspaceSidebarDirectMessagesListing from './WorkspacesSidebarDirectMessagesListing'
import { WorkspacePageSidebarDirectMessageGroupsFragment$key } from '../../../../relay/fragments/__generated__/WorkspacePageSidebarDirectMessageGroupsFragment.graphql'
import { usePaginationFragment } from 'react-relay'
import WorkspacePageSidebarDirectMessageGroupsFragment from '../../../../relay/fragments/WorkspacePageSidebarDirectMessageGroups'

type WorkspaceSidebarDirectMessagesListProps = {
    groups: WorkspacePageSidebarDirectMessageGroupsFragment$key
}

function WorkspaceSidebarDirectMessagesList({
    groups,
}: WorkspaceSidebarDirectMessagesListProps) {
    const { data, loadNext } = usePaginationFragment(
        WorkspacePageSidebarDirectMessageGroupsFragment,
        groups
    )

    return (
        <ListBox
            items={data.directMessageGroups.edges}
            selectionMode="single"
            listClassName="no-scrollbar overflow-y-auto text-xs"
        >
            {(item) => (
                <Item key={item.node.id}>
                    <WorkspaceSidebarDirectMessagesListing group={item.node} />
                </Item>
            )}
        </ListBox>
    )
}

export default WorkspaceSidebarDirectMessagesList
