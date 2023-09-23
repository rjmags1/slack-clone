import ListBox from '../../../lib/Listbox'
import { Item } from 'react-stately'
import WorkspaceSidebarStarredListing from './WorkspaceSidebarStarredListing'
import { WorkspacePageSidebarStarredsFragment$key } from '../../../../relay/fragments/__generated__/WorkspacePageSidebarStarredsFragment.graphql'
import { usePaginationFragment } from 'react-relay'
import WorkspacePageSidebarStarredsFragment from '../../../../relay/fragments/WorkspacePageSidebarStarreds'

type WorkspaceSidebarStarredListProps = {
    starred: WorkspacePageSidebarStarredsFragment$key
}

function WorkspaceSidebarStarredList({
    starred,
}: WorkspaceSidebarStarredListProps) {
    const { data, loadNext } = usePaginationFragment(
        WorkspacePageSidebarStarredsFragment,
        starred
    )

    return (
        <ListBox
            items={data.starred.edges}
            selectionMode="single"
            listClassName="no-scrollbar overflow-y-auto text-xs"
        >
            {(item) => (
                <Item key={item.node.id}>
                    <WorkspaceSidebarStarredListing starred={item.node} />
                </Item>
            )}
        </ListBox>
    )
}

export default WorkspaceSidebarStarredList
