import graphql from 'babel-plugin-relay/macro'

const WorkspacePageSidebarChannelsFragment = graphql`
    fragment WorkspacePageSidebarChannelsFragment on WorkspacePageData
    @refetchable(queryName: "ChannelsListPaginationQuery")
    @argumentDefinitions(
        first: { type: "Int", defaultValue: 35 }
        after: { type: "ID" }
        filter: { type: "ChannelsFilter!" }
    ) {
        channels(filter: $filter, first: $first, after: $after)
            @connection(key: "WorkspacePageSidebarChannelsFragment_channels") {
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
                    ...WorkspacePageSidebarChannelFragment
                }
            }
        }
    }
`

export default WorkspacePageSidebarChannelsFragment
