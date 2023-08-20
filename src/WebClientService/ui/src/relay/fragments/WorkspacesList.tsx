import graphql from 'babel-plugin-relay/macro'

const WorkspacesListFragment = graphql`
    fragment WorkspacesListFragment on WorkspacesPageData
    @refetchable(queryName: "WorkspacesListPaginationQuery")
    @argumentDefinitions(
        first: { type: "Int", defaultValue: 10 }
        after: { type: "ID" }
        filter: { type: "WorkspacesFilter" }
    ) {
        workspaces(filter: $filter, first: $first, after: $after)
            @connection(key: "WorkspacesListFragment_workspaces") {
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
    }
`

export default WorkspacesListFragment
