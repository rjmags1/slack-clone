import { Item } from 'react-stately'
import List from '../../lib/List'
import WorkspacesSearchbar from './WorkspacesSearchbar'
import WorkspaceListing from './WorkspaceListing'
import graphql from 'babel-plugin-relay/macro'
import type { WorkspacesListFragment$key } from './__generated__/WorkspacesListFragment.graphql'
import { useFragment } from 'react-relay'
import { useEffect, useState } from 'react'

const WorkspacesListFragment = graphql`
    fragment WorkspacesListFragment on WorkspacesConnection {
        totalEdges
        pageInfo {
            startCursor
            endCursor
            hasNextPage
            hasPreviousPage
        }
        edges {
            node {
                id
                ...WorkspaceListingFragment
                name
            }
        }
    }
`

type WorkspacesListProps = {
    workspaces: WorkspacesListFragment$key
}

function WorkspacesList({ workspaces }: WorkspacesListProps) {
    const data = useFragment(WorkspacesListFragment, workspaces)
    const [searchText, setSearchText] = useState('')
    const [renderedEdges, setRenderedEdges] = useState(data.edges)

    useEffect(() => {
        setRenderedEdges(
            data.edges.filter((e) => e.node.name.includes(searchText))
        )
    }, [data.edges, searchText])

    return (
        <div
            className="no-scrollbar max-h-[80%] min-h-max
                overflow-y-auto rounded-md border-0 bg-zinc-500 font-light"
        >
            <WorkspacesSearchbar
                searchText={searchText}
                setSearchText={setSearchText}
            />
            <List className="flex min-w-[36rem] flex-col gap-y-2 px-4 py-2">
                {renderedEdges.length === 0 ? (
                    <Item>
                        <h6
                            className="flex h-12 w-full items-center 
                                justify-center from-neutral-600"
                        >
                            No workspaces
                        </h6>
                    </Item>
                ) : (
                    renderedEdges.map((w) => (
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
        </div>
    )
}

export default WorkspacesList
