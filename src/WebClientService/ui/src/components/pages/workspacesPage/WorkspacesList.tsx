import { Item } from 'react-stately'
import List from '../../lib/List'
import WorkspacesSearchbar from './WorkspacesSearchbar'
import WorkspaceListing from './WorkspaceListing'
import { usePaginationFragment } from 'react-relay'
import { useRef, useState } from 'react'
import WorkspacesListFragment from '../../../relay/fragments/WorkspacesList'
import { WorkspacesListFragment$key } from '../../../relay/fragments/__generated__/WorkspacesListFragment.graphql'
import ScrollTrigger from '../../lib/ScrollTrigger'

type WorkspacesListProps = {
    workspaces: WorkspacesListFragment$key
}

function WorkspacesList({ workspaces }: WorkspacesListProps) {
    const { data, loadNext, isLoadingNext, hasNext } = usePaginationFragment(
        WorkspacesListFragment,
        workspaces
    )
    const [searchText, setSearchText] = useState('')
    const ref = useRef<HTMLDivElement>(null)

    return (
        <div
            ref={ref}
            className="no-scrollbar max-h-[80%] min-h-max
                overflow-y-auto rounded-md border-0 bg-zinc-500 font-light"
        >
            <WorkspacesSearchbar
                searchText={searchText}
                setSearchText={setSearchText}
            />
            <List className="flex min-w-[36rem] flex-col gap-y-2 px-4 py-2">
                {data.workspaces.totalEdges === 0 ? (
                    <Item>
                        <h6
                            className="flex h-12 w-full items-center 
                                justify-center from-neutral-600"
                        >
                            No workspaces
                        </h6>
                    </Item>
                ) : (
                    data.workspaces.edges
                        .filter((w) => w.node.name.includes(searchText))
                        .map((w) => (
                            <Item>
                                <WorkspaceListing
                                    id={w.node.id}
                                    key={w.node.id}
                                    workspace={w.node}
                                    name={w.node.name}
                                />
                            </Item>
                        ))
                )}
            </List>
            {hasNext && (
                <ScrollTrigger
                    containerRef={ref}
                    observerCallback={(entries) => {
                        const observed = entries[0]
                        if (!observed.isIntersecting || isLoadingNext) return
                        loadNext(10)
                    }}
                />
            )}
        </div>
    )
}

export default WorkspacesList
