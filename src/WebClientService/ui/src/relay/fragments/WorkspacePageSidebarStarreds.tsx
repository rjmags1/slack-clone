import graphql from 'babel-plugin-relay/macro'

const WorkspacePageSidebarStarredsFragment = graphql`
    fragment WorkspacePageSidebarStarredsFragment on WorkspacePageData
    @refetchable(queryName: "StarredsListPaginationQuery")
    @argumentDefinitions(
        first: { type: "Int", defaultValue: 10 }
        after: { type: "ID" }
        filter: { type: "StarredFilter" }
    ) {
        starred(first: $first, after: $after, filter: $filter)
            @connection(key: "WorkspacePageSidebarStarredsFragment_starred") {
            totalEdges
            pageInfo {
                hasNextPage
            }
            edges {
                node {
                    id
                    ...WorkspacePageSidebarStarredFragment
                }
            }
        }
    }
`

export default WorkspacePageSidebarStarredsFragment
