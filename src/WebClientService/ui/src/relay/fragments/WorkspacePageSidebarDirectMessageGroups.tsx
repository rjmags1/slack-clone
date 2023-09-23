import graphql from 'babel-plugin-relay/macro'

const WorkspacePageSidebarDirectMessageGroupsFragment = graphql`
    fragment WorkspacePageSidebarDirectMessageGroupsFragment on WorkspacePageData
    @refetchable(queryName: "DirectMessageGroupsListPaginationQuery")
    @argumentDefinitions(
        first: { type: "Int", defaultValue: 10 }
        after: { type: "ID" }
        filter: { type: "DirectMessageGroupsFilter" }
    ) {
        directMessageGroups(first: $first, after: $after, filter: $filter)
            @connection(
                key: "WorkspacePageSidebarDirectMessageGroupsFragment_directMessageGroups"
            ) {
            totalEdges
            pageInfo {
                startCursor
                endCursor
                hasNextPage
                hasPreviousPage
            }
            edges {
                cursor
                node {
                    id
                    ...WorkspacePageSidebarDirectMessageGroupFragment
                }
            }
        }
    }
`

export default WorkspacePageSidebarDirectMessageGroupsFragment
