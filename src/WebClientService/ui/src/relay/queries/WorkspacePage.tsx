import graphql from 'babel-plugin-relay/macro'

const WorkspacePageDataQuery = graphql`
    query WorkspacePageQuery(
        $userId: ID!
        $workspaceId: ID!
        $channelsFilter: ChannelsFilter!
        $directMessageGroupsFilter: DirectMessageGroupsFilter!
        $starredFilter: StarredFilter!
    ) {
        workspacePageData(userId: $userId) {
            id
            workspace(id: $workspaceId) {
                id
                name
            }
            channels(first: 10, filter: $channelsFilter) {
                totalEdges
                pageInfo {
                    hasNextPage
                }
                edges {
                    cursor
                    node {
                        id
                    }
                }
            }
            directMessageGroups(first: 10, filter: $directMessageGroupsFilter) {
                totalEdges
                edges {
                    cursor
                    node {
                        id
                    }
                }
            }
            starred(first: 10, filter: $starredFilter) {
                totalEdges
                pageInfo {
                    hasNextPage
                }
                edges {
                    node {
                        id
                        createdAtUTC
                    }
                }
            }
        }
    }
`

export default WorkspacePageDataQuery
